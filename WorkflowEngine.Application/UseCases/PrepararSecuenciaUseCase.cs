using Microsoft.Extensions.Logging;
using WorkflowEngine.Application.DTOs;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Application.Interfaces.UseCases;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Application.UseCases
{
    public class PrepararSecuenciaUseCase : IPrepararSecuenciaUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PrepararSecuenciaUseCase> _logger;

        public PrepararSecuenciaUseCase(IUnitOfWork unitOfWork, ILogger<PrepararSecuenciaUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResultadoPrepararSecuencia> ExecuteAsync(int idSecuencia, Dictionary<int, string> datosRecibidos, int idFlujoActivo)
        {
            try
            {
                if (idSecuencia <= 0)
                {
                    throw new ArgumentException("ID de secuencia no puede ser null o menor igual a 0");
                }

                // Consultar la secuencia
                var secuencia = await _unitOfWork.Secuencias.GetByIdAsync(idSecuencia);
                if (secuencia == null)
                {
                    throw new ArgumentException($"Secuencia con ID {idSecuencia} no encontrada");
                }

                // Obtener los IDs de pasos de la secuencia
                var idsPasos = ParsearIdsPasos(secuencia.ListaIdPasos);
                if (!idsPasos.Any())
                {
                    throw new InvalidOperationException($"La secuencia {idSecuencia} no tiene pasos definidos");
                }

                // Obtener todos los pasos de la secuencia
                var pasos = await _unitOfWork.Pasos.FindAsync(p => idsPasos.Contains(p.Id));
                if (!pasos.Any())
                {
                    throw new InvalidOperationException($"No se encontraron pasos para la secuencia {idSecuencia}");
                }

                // Obtener todos los IDs de campos requeridos por todos los pasos
                var todosIdsCampos = new List<int>();
                foreach (var paso in pasos)
                {
                    var idsCamposPaso = ParsearIdsCampos(paso.ListaIdCampos);
                    todosIdsCampos.AddRange(idsCamposPaso);
                }

                // Eliminar duplicados
                todosIdsCampos = todosIdsCampos.Distinct().ToList();

                if (!todosIdsCampos.Any())
                {
                    throw new InvalidOperationException($"No se encontraron campos requeridos para los pasos de la secuencia {idSecuencia}");
                }

                // Consultar qué información ya existe en el flujo activo
                var resultadoConsulta = await ConsultarInformacionFlujoActivo(todosIdsCampos, idFlujoActivo);

                // Procesar datos recibidos si los hay
                if (datosRecibidos.Any())
                {
                    var resultadoProcesamiento = await ProcesarDatosRecibidos(
                        datosRecibidos,
                        resultadoConsulta.IdsCamposFaltantes,
                        idFlujoActivo);

                    // Actualizar resultado de consulta con los nuevos datos
                    resultadoConsulta = await ConsultarInformacionFlujoActivo(todosIdsCampos, idFlujoActivo);
                }

                // Verificar si todos los campos están completos
                if (resultadoConsulta.TodosCompletos)
                {
                    return new ResultadoPrepararSecuencia
                    {
                        EsValido = true,
                        IdsPasos = idsPasos,
                        DatosPreparados = resultadoConsulta.DatosCompletos,
                        Mensaje = "Secuencia preparada exitosamente"
                    };
                }
                else
                {
                    // Obtener información de los campos faltantes
                    var camposRequeridos = await ObtenerInformacionCamposFaltantes(resultadoConsulta.IdsCamposFaltantes);

                    return new ResultadoPrepararSecuencia
                    {
                        EsValido = false,
                        CamposRequeridos = camposRequeridos,
                        Mensaje = "Se requieren campos adicionales"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al preparar secuencia {IdSecuencia} para flujo activo {IdFlujoActivo}",
                    idSecuencia, idFlujoActivo);

                return new ResultadoPrepararSecuencia
                {
                    EsValido = false,
                    Error = ex.Message,
                    Mensaje = "Error interno al preparar secuencia"
                };
            }
        }

        private List<int> ParsearIdsPasos(string listaIdPasos)
        {
            if (string.IsNullOrWhiteSpace(listaIdPasos))
                return new List<int>();

            return listaIdPasos.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id.Trim(), out int result) ? result : 0)
                .Where(id => id > 0)
                .ToList();
        }

        private List<int> ParsearIdsCampos(string listaIdCampos)
        {
            if (string.IsNullOrWhiteSpace(listaIdCampos))
                return new List<int>();

            return listaIdCampos.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id.Trim(), out int result) ? result : 0)
                .Where(id => id > 0)
                .ToList();
        }

        private async Task<CamposConsultaResult> ConsultarInformacionFlujoActivo(List<int> idsCamposRequeridos, int idFlujoActivo)
        {
            var camposFlujoActivo = await _unitOfWork.CamposFlujosActivos.FindAsync(
                cf => cf.IdFlujoActivo == idFlujoActivo && idsCamposRequeridos.Contains(cf.IdCampo));

            var datosExistentes = camposFlujoActivo.ToDictionary(cf => cf.IdCampo, cf => cf.Valor);
            var idsCamposFaltantes = idsCamposRequeridos.Except(datosExistentes.Keys).ToList();

            return new CamposConsultaResult
            {
                TodosCompletos = !idsCamposFaltantes.Any(),
                IdsCamposFaltantes = idsCamposFaltantes,
                DatosCompletos = datosExistentes
            };
        }

        private async Task<bool> ProcesarDatosRecibidos(
            Dictionary<int, string> datosRecibidos,
            List<int> idsCamposFaltantes,
            int idFlujoActivo)
        {
            var datosParaGuardar = new List<CampoFlujoActivo>();

            foreach (var kvp in datosRecibidos)
            {
                // Solo procesar campos que efectivamente son requeridos
                if (idsCamposFaltantes.Contains(kvp.Key))
                {
                    // Aquí podrías agregar validaciones adicionales si fuera necesario
                    // Por ahora, guardamos directamente
                    datosParaGuardar.Add(new CampoFlujoActivo
                    {
                        IdFlujoActivo = idFlujoActivo,
                        IdCampo = kvp.Key,
                        Valor = kvp.Value
                    });
                }
            }

            if (datosParaGuardar.Any())
            {
                await _unitOfWork.CamposFlujosActivos.AddRangeAsync(datosParaGuardar);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Guardados {Count} campos para flujo activo {IdFlujoActivo}",
                    datosParaGuardar.Count, idFlujoActivo);

                return true;
            }

            return false;
        }

        private async Task<List<CampoRequerido>> ObtenerInformacionCamposFaltantes(List<int> idsCamposFaltantes)
        {
            var campos = await _unitOfWork.Campos.FindAsync(c => idsCamposFaltantes.Contains(c.Id));

            return campos.Select(c => new CampoRequerido
            {
                IdCampo = c.Id,
                MensajeSolicitud = c.MensajeCliente,
                RegexValidacionCliente = c.RegexCliente,
                TipoDato = c.TipoDato,
                EsInterno = c.EsInterno
            }).ToList();
        }
    }
}