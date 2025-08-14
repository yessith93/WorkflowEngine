// WorkflowEngine.Application/UseCases/IniciarFlujoUseCase.cs
using Microsoft.Extensions.Logging;
using WorkflowEngine.Application.DTOs;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Application.Interfaces.UseCases;
using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Application.UseCases
{
    public class IniciarFlujoUseCase : IIniciarFlujoUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPrepararSecuenciaUseCase _prepararSecuenciaUseCase;
        private readonly IEjecutarPasosUseCase _ejecutarPasosUseCase;
        private readonly IIdentificarSiguienteSecuenciaUseCase _siguienteSecuenciaUseCase;
        private readonly ILogger<IniciarFlujoUseCase> _logger;

        public IniciarFlujoUseCase(
            IUnitOfWork unitOfWork,
            IPrepararSecuenciaUseCase prepararSecuenciaUseCase,
            IEjecutarPasosUseCase ejecutarPasosUseCase,
            IIdentificarSiguienteSecuenciaUseCase siguienteSecuenciaUseCase,
            ILogger<IniciarFlujoUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _prepararSecuenciaUseCase = prepararSecuenciaUseCase;
            _ejecutarPasosUseCase = ejecutarPasosUseCase;
            _siguienteSecuenciaUseCase = siguienteSecuenciaUseCase;
            _logger = logger;
        }

        public async Task<ResultadoIniciarFlujo> ExecuteAsync(int tipoFlujoId)
        {
            try
            {
                // Validar que el tipo de flujo existe
                var tipoFlujo = await _unitOfWork.TiposFlujo.GetWithSecuenciasAsync(tipoFlujoId);
                if (tipoFlujo == null)
                {
                    _logger.LogWarning("Intento de iniciar flujo con ID inexistente: {TipoFlujoId}", tipoFlujoId);
                    throw new ArgumentException($"El tipo de flujo con ID {tipoFlujoId} no existe");
                }

                // Crear nuevo flujo activo
                var flujoActivo = new FlujoActivo
                {
                    IdTipoFlujo = tipoFlujoId,
                    IdSecuenciaActual = tipoFlujo.IdSecuenciaInicial,
                    EstadoFlujo = EstadoFlujo.Iniciado,
                    FechaCreacion = DateTime.UtcNow
                };

                await _unitOfWork.FlujosActivos.AddAsync(flujoActivo);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Flujo activo creado: {FlujoActivoId} para tipo de flujo: {TipoFlujoId}",
                    flujoActivo.Id, tipoFlujoId);

                // Preparar la secuencia inicial con datos vacíos
                var resultadoPreparacion = await _prepararSecuenciaUseCase.ExecuteAsync(
                    tipoFlujo.IdSecuenciaInicial,
                    new Dictionary<int, string>(),
                    flujoActivo.Id);

                var resultado = new ResultadoIniciarFlujo
                {
                    Exitoso = true,
                    IdFlujoActivo = flujoActivo.Id,
                    IdSecuenciaInicial = tipoFlujo.IdSecuenciaInicial
                };

                // Si la preparación es válida, ejecutar pasos
                if (resultadoPreparacion.EsValido && resultadoPreparacion.IdsPasos?.Any() == true)
                {
                    await EjecutarSecuenciasHastaCompletarOSolicitarDatos(flujoActivo.Id, resultadoPreparacion, resultado);
                }
                else if (resultadoPreparacion.CamposRequeridos?.Any() == true)
                {
                    // Necesita datos del cliente
                    resultado.CamposRequeridos = resultadoPreparacion.CamposRequeridos;
                    resultado.Mensaje = "Se requieren datos adicionales para continuar";

                    // Actualizar estado del flujo
                    await _unitOfWork.FlujosActivos.UpdateEstadoAsync(flujoActivo.Id, EstadoFlujo.EsperandoDatos);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar flujo de tipo {TipoFlujoId}", tipoFlujoId);
                return new ResultadoIniciarFlujo
                {
                    Exitoso = false,
                    Mensaje = "Error interno al iniciar el flujo"
                };
            }
        }

        private async Task EjecutarSecuenciasHastaCompletarOSolicitarDatos(
            int idFlujoActivo,
            ResultadoPrepararSecuencia preparacionInicial,
            ResultadoIniciarFlujo resultado)
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
                    await _unitOfWork.FlujosActivos.UpdateEstadoAsync(idFlujoActivo, EstadoFlujo.EnProgreso);
                }

                // Identificar siguiente secuencia
                var idSiguienteSecuencia = await _siguienteSecuenciaUseCase.ExecuteAsync(idFlujoActivo);

                if (idSiguienteSecuencia.HasValue)
                {
                    // Actualizar secuencia actual
                    await _unitOfWork.FlujosActivos.UpdateSecuenciaActualAsync(idFlujoActivo, idSiguienteSecuencia.Value);

                    // Preparar siguiente secuencia
                    preparacionActual = await _prepararSecuenciaUseCase.ExecuteAsync(
                        idSiguienteSecuencia.Value,
                        new Dictionary<int, string>(),
                        idFlujoActivo);

                    if (!preparacionActual.EsValido && preparacionActual.CamposRequeridos?.Any() == true)
                    {
                        // Necesita datos del cliente, detener ejecución
                        resultado.CamposRequeridos = preparacionActual.CamposRequeridos;
                        resultado.Mensaje = "Se requieren datos adicionales para continuar";
                        continuarEjecucion = false;

                        await _unitOfWork.FlujosActivos.UpdateEstadoAsync(idFlujoActivo, EstadoFlujo.EsperandoDatos);
                    }
                }
                else
                {
                    // No hay más secuencias, verificar si el flujo está completo
                    await VerificarFinalizacionFlujo(idFlujoActivo, resultado);
                    continuarEjecucion = false;
                }

            } while (continuarEjecucion);
        }

        private async Task VerificarFinalizacionFlujo(int idFlujoActivo, ResultadoIniciarFlujo resultado)
        {
            var flujoActivo = await _unitOfWork.FlujosActivos.GetWithTipoFlujoAsync(idFlujoActivo);
            if (flujoActivo?.TipoFlujo != null)
            {
                // Verificar si la secuencia actual es la final
                if (flujoActivo.IdSecuenciaActual == flujoActivo.TipoFlujo.IdSecuenciaFinal)
                {
                    await _unitOfWork.FlujosActivos.UpdateEstadoAsync(idFlujoActivo, EstadoFlujo.Completado);
                    resultado.FlujoCompleto = true;
                    resultado.Mensaje = "Flujo completado exitosamente";

                    _logger.LogInformation("Flujo {FlujoActivoId} completado exitosamente", idFlujoActivo);
                }
                else
                {
                    _logger.LogWarning("Flujo {FlujoActivoId} en estado inconsistente. Secuencia actual: {SecuenciaActual}, Final esperada: {SecuenciaFinal}",
                        idFlujoActivo, flujoActivo.IdSecuenciaActual, flujoActivo.TipoFlujo.IdSecuenciaFinal);

                    resultado.Mensaje = "Estado del flujo inconsistente";
                }
            }
        }
    }
}