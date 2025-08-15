using Microsoft.Extensions.Logging;
using WorkflowEngine.Application.DTOs;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Application.Interfaces.UseCases;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Application.UseCases
{
    public class ProcesarFlujoUseCase : IProcesarFlujoUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPrepararSecuenciaUseCase _prepararSecuenciaUseCase;
        private readonly IEjecutarPasosUseCase _ejecutarPasosUseCase;
        private readonly IIdentificarSiguienteSecuenciaUseCase _siguienteSecuenciaUseCase;
        private readonly ILogger<ProcesarFlujoUseCase> _logger;

        public ProcesarFlujoUseCase(
            IUnitOfWork unitOfWork,
            IPrepararSecuenciaUseCase prepararSecuenciaUseCase,
            IEjecutarPasosUseCase ejecutarPasosUseCase,
            IIdentificarSiguienteSecuenciaUseCase siguienteSecuenciaUseCase,
            ILogger<ProcesarFlujoUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _prepararSecuenciaUseCase = prepararSecuenciaUseCase;
            _ejecutarPasosUseCase = ejecutarPasosUseCase;
            _siguienteSecuenciaUseCase = siguienteSecuenciaUseCase;
            _logger = logger;
        }

        public async Task<ResultadoProcesarFlujo> ExecuteAsync(int idFlujoActivo, Dictionary<int, string> datosFormulario)
        {
            try
            {
                // Validar ID del flujo
                if (idFlujoActivo <= 0)
                {
                    return new ResultadoProcesarFlujo
                    {
                        Exitoso = false,
                        Mensaje = "El ID del flujo activo no es válido",
                        IdFlujoActivo = idFlujoActivo
                    };
                }

                // Consultar el flujo activo
                var flujoActivo = await _unitOfWork.FlujosActivos.GetWithTipoFlujoAsync(idFlujoActivo);
                if (flujoActivo?.TipoFlujo == null)
                {
                    return new ResultadoProcesarFlujo
                    {
                        Exitoso = false,
                        Mensaje = "El flujo al que se quiere acceder no es válido",
                        IdFlujoActivo = idFlujoActivo
                    };
                }

                // Validar que el flujo no esté completado o cancelado
                if (flujoActivo.EstadoFlujo == EstadoFlujo.Completado || flujoActivo.EstadoFlujo == EstadoFlujo.Cancelado)
                {
                    return new ResultadoProcesarFlujo
                    {
                        Exitoso = false,
                        Mensaje = "El flujo ya está completado o cancelado",
                        IdFlujoActivo = idFlujoActivo,
                        FlujoCompleto = flujoActivo.EstadoFlujo == EstadoFlujo.Completado
                    };
                }

                _logger.LogInformation("Procesando flujo {IdFlujoActivo}, secuencia actual: {SecuenciaActual}",
                    idFlujoActivo, flujoActivo.IdSecuenciaActual);

                // Preparar la secuencia actual con los datos recibidos
                var resultadoPreparacion = await _prepararSecuenciaUseCase.ExecuteAsync(
                    flujoActivo.IdSecuenciaActual,
                    datosFormulario,
                    idFlujoActivo);

                var resultado = new ResultadoProcesarFlujo
                {
                    IdFlujoActivo = idFlujoActivo,
                    Exitoso =true
                };

                // Si la preparación es válida, ejecutar pasos
                if (resultadoPreparacion.EsValido && resultadoPreparacion.IdsPasos?.Any() == true)
                {
                    await EjecutarSecuenciasHastaCompletarOSolicitarDatos(flujoActivo, resultadoPreparacion, resultado);
                }
                else if (resultadoPreparacion.CamposRequeridos?.Any() == true)
                {
                    // Necesita más datos del cliente
                    resultado.Exitoso = true;
                    resultado.CamposRequeridos = resultadoPreparacion.CamposRequeridos;
                    resultado.Mensaje = "Se requieren datos adicionales para continuar";

                    // Actualizar estado del flujo
                    await _unitOfWork.FlujosActivos.UpdateEstadoAsync(idFlujoActivo, EstadoFlujo.EsperandoDatos);
                }
                else
                {
                    resultado.Exitoso = false;
                    resultado.Mensaje = resultadoPreparacion.Mensaje;
                    resultado.Error = resultadoPreparacion.Error;
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar flujo {IdFlujoActivo}", idFlujoActivo);

                return new ResultadoProcesarFlujo
                {
                    Exitoso = false,
                    Mensaje = "Error interno del servidor",
                    IdFlujoActivo = idFlujoActivo,
                    Error = ex.Message
                };
            }
        }

        private async Task EjecutarSecuenciasHastaCompletarOSolicitarDatos(
            Domain.Entities.FlujoActivo flujoActivo,
            ResultadoPrepararSecuencia preparacionInicial,
            ResultadoProcesarFlujo resultado)
        {
            bool continuarEjecucion = true;
            var preparacionActual = preparacionInicial;

            do
            {
                // Ejecutar pasos de la secuencia actual
                if (preparacionActual.IdsPasos?.Any() == true && preparacionActual.DatosPreparados != null)
                {
                    await _ejecutarPasosUseCase.ExecuteAsync(preparacionActual.IdsPasos, preparacionActual.DatosPreparados);

                    // Actualizar estado del flujo
                    await _unitOfWork.FlujosActivos.UpdateEstadoAsync(flujoActivo.Id, EstadoFlujo.EnProgreso);
                }

                // Identificar siguiente secuencia
                var idSiguienteSecuencia = await _siguienteSecuenciaUseCase.ExecuteAsync(flujoActivo.Id);

                if (idSiguienteSecuencia.HasValue)
                {
                    // Actualizar secuencia actual
                    await _unitOfWork.FlujosActivos.UpdateSecuenciaActualAsync(flujoActivo.Id, idSiguienteSecuencia.Value);
                    flujoActivo.IdSecuenciaActual = idSiguienteSecuencia.Value; // Actualizar para próxima iteración

                    // Preparar siguiente secuencia
                    preparacionActual = await _prepararSecuenciaUseCase.ExecuteAsync(
                        idSiguienteSecuencia.Value,
                        preparacionActual.DatosPreparados,
                        flujoActivo.Id);

                    if (!preparacionActual.EsValido && preparacionActual.CamposRequeridos?.Any() == true)
                    {
                        // Necesita datos del cliente, detener ejecución
                        resultado.Exitoso = true;
                        resultado.CamposRequeridos = preparacionActual.CamposRequeridos;
                        resultado.Mensaje = "Se requieren datos adicionales para continuar";
                        continuarEjecucion = false;

                        await _unitOfWork.FlujosActivos.UpdateEstadoAsync(flujoActivo.Id, EstadoFlujo.EsperandoDatos);
                    }
                }
                else
                {
                    // No hay más secuencias, verificar si el flujo está completo
                    await VerificarFinalizacionFlujo(flujoActivo, resultado);
                    continuarEjecucion = false;
                }

            } while (continuarEjecucion);
        }

        private async Task VerificarFinalizacionFlujo(Domain.Entities.FlujoActivo flujoActivo, ResultadoProcesarFlujo resultado)
        {
            if (flujoActivo?.TipoFlujo != null)
            {
                // Verificar si la secuencia actual es la final
                if (flujoActivo.IdSecuenciaActual == flujoActivo.TipoFlujo.IdSecuenciaFinal)
                {
                    await _unitOfWork.FlujosActivos.UpdateEstadoAsync(flujoActivo.Id, EstadoFlujo.Completado);

                    resultado.Exitoso = true;
                    resultado.FlujoCompleto = true;
                    resultado.Mensaje = "Flujo completado exitosamente";

                    _logger.LogInformation("Flujo {FlujoActivoId} completado exitosamente", flujoActivo.Id);
                }
                else
                {
                    _logger.LogWarning("Flujo {FlujoActivoId} en estado inconsistente. Secuencia actual: {SecuenciaActual}, Final esperada: {SecuenciaFinal}",
                        flujoActivo.Id, flujoActivo.IdSecuenciaActual, flujoActivo.TipoFlujo.IdSecuenciaFinal);

                    resultado.Exitoso = false;
                    resultado.Mensaje = "Estado del flujo inconsistente";
                }
            }
        }
    }
}