using Microsoft.Extensions.Logging;
using WorkflowEngine.Application.DTOs;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Application.Interfaces.Services;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Application.Services
{
    public class PasoExecutorService : IPasoExecutorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnumerable<IPasoHandler> _pasoHandlers;
        private readonly ILogger<PasoExecutorService> _logger;

        public PasoExecutorService(
            IUnitOfWork unitOfWork,
            IEnumerable<IPasoHandler> pasoHandlers,
            ILogger<PasoExecutorService> logger)
        {
            _unitOfWork = unitOfWork;
            _pasoHandlers = pasoHandlers;
            _logger = logger;
        }

        public async Task ExecuteAsync(List<int> idsPasos, Dictionary<int, string> datos)
        {
            try
            {
                // Obtener información de los pasos
                var pasos = await _unitOfWork.Pasos.FindAsync(p => idsPasos.Contains(p.Id));
                if (!pasos.Any())
                {
                    throw new ArgumentException("No se encontraron pasos con los IDs proporcionados");
                }

                // Crear contextos de ejecución para cada paso
                var contextosEjecucion = new List<PasoEjecucionContext>();

                foreach (var paso in pasos)
                {
                    var contexto = new PasoEjecucionContext
                    {
                        IdPaso = paso.Id,
                        TipoPaso = paso.TipoPaso,
                        Datos = datos,
                        IdsCamposRequeridos = ParsearIdsCampos(paso.ListaIdCampos)
                    };

                    contextosEjecucion.Add(contexto);
                }

                // Ejecutar pasos en paralelo usando Task.WhenAll
                var tareasEjecucion = contextosEjecucion.Select(async contexto =>
                {
                    await EjecutarPasoIndividual(contexto);
                });

                await Task.WhenAll(tareasEjecucion);

                _logger.LogInformation("Todos los pasos fueron ejecutados exitosamente en paralelo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la ejecución paralela de pasos");
                throw;
            }
        }

        private async Task EjecutarPasoIndividual(PasoEjecucionContext contexto)
        {
            try
            {
                _logger.LogInformation("Ejecutando paso {IdPaso} de tipo {TipoPaso}",
                    contexto.IdPaso, contexto.TipoPaso);

                // Filtrar datos relevantes para este paso específico
                var datosRelevantes = contexto.Datos
                    .Where(kvp => contexto.IdsCamposRequeridos.Contains(kvp.Key))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // Encontrar el handler apropiado para este tipo de paso
                var handler = await EncontrarHandler(contexto.TipoPaso);
                if (handler == null)
                {
                    throw new NotSupportedException($"No se encontró handler para el tipo de paso: {contexto.TipoPaso}");
                }

                // Ejecutar el paso
                await handler.ExecuteAsync(datosRelevantes);

                _logger.LogInformation("Paso {IdPaso} ejecutado exitosamente", contexto.IdPaso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar paso individual {IdPaso} de tipo {TipoPaso}",
                    contexto.IdPaso, contexto.TipoPaso);
                throw;
            }
        }

        private async Task<IPasoHandler?> EncontrarHandler(PasoTipo tipoPaso)
        {
            foreach (var handler in _pasoHandlers)
            {
                if (await handler.CanHandle(tipoPaso))
                {
                    return handler;
                }
            }
            return null;
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
    }
}