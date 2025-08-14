using Microsoft.Extensions.Logging;
using WorkflowEngine.Application.Interfaces.Services;
using WorkflowEngine.Application.Interfaces.UseCases;

namespace WorkflowEngine.Application.UseCases
{
    public class EjecutarPasosUseCase : IEjecutarPasosUseCase
    {
        private readonly IPasoExecutorService _pasoExecutorService;
        private readonly ILogger<EjecutarPasosUseCase> _logger;

        public EjecutarPasosUseCase(
            IPasoExecutorService pasoExecutorService,
            ILogger<EjecutarPasosUseCase> logger)
        {
            _pasoExecutorService = pasoExecutorService;
            _logger = logger;
        }

        public async Task ExecuteAsync(List<int> idsPasos, Dictionary<int, string> datosPreparados)
        {
            try
            {
                if (!idsPasos?.Any() == true)
                {
                    _logger.LogWarning("Se intentó ejecutar pasos con lista vacía o nula");
                    return;
                }

                _logger.LogInformation("Iniciando ejecución de {Count} pasos: {Pasos}",
                    idsPasos.Count, string.Join(", ", idsPasos));

                await _pasoExecutorService.ExecuteAsync(idsPasos, datosPreparados);

                _logger.LogInformation("Ejecución de pasos completada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar pasos {Pasos}", string.Join(", ", idsPasos ?? new List<int>()));
                throw;
            }
        }
    }
}