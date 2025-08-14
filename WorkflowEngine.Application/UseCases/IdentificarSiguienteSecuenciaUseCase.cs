using Microsoft.Extensions.Logging;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Application.Interfaces.UseCases;

namespace WorkflowEngine.Application.UseCases
{
    public class IdentificarSiguienteSecuenciaUseCase : IIdentificarSiguienteSecuenciaUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IdentificarSiguienteSecuenciaUseCase> _logger;

        public IdentificarSiguienteSecuenciaUseCase(IUnitOfWork unitOfWork, ILogger<IdentificarSiguienteSecuenciaUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int?> ExecuteAsync(int idFlujoActivo)
        {
            try
            {
                // Obtener flujo activo con su tipo de flujo
                var flujoActivo = await _unitOfWork.FlujosActivos.GetWithTipoFlujoAsync(idFlujoActivo);
                if (flujoActivo?.TipoFlujo == null)
                {
                    _logger.LogWarning("Flujo activo {IdFlujoActivo} no encontrado o sin tipo de flujo asociado", idFlujoActivo);
                    throw new ArgumentException($"Flujo activo con ID {idFlujoActivo} no encontrado");
                }

                // Obtener el orden de secuencias del tipo de flujo
                var ordenSecuencias = ParsearOrdenSecuencias(flujoActivo.TipoFlujo.OrdenSecuencias);
                if (!ordenSecuencias.Any())
                {
                    _logger.LogWarning("Tipo de flujo {TipoFlujoId} no tiene orden de secuencias definido", flujoActivo.IdTipoFlujo);
                    throw new InvalidOperationException($"Tipo de flujo {flujoActivo.IdTipoFlujo} no tiene orden de secuencias definido");
                }

                // Encontrar la posición de la secuencia actual
                var posicionActual = ordenSecuencias.FindIndex(id => id == flujoActivo.IdSecuenciaActual);
                if (posicionActual == -1)
                {
                    _logger.LogWarning("Secuencia actual {SecuenciaActual} no encontrada en el orden de secuencias del flujo {IdFlujoActivo}",
                        flujoActivo.IdSecuenciaActual, idFlujoActivo);
                    throw new InvalidOperationException($"Secuencia actual {flujoActivo.IdSecuenciaActual} no encontrada en el orden de secuencias");
                }

                // Verificar si hay una siguiente secuencia
                if (posicionActual + 1 < ordenSecuencias.Count)
                {
                    var siguienteSecuenciaId = ordenSecuencias[posicionActual + 1];

                    _logger.LogInformation("Siguiente secuencia identificada: {SiguienteSecuencia} para flujo {IdFlujoActivo}",
                        siguienteSecuenciaId, idFlujoActivo);

                    return siguienteSecuenciaId;
                }

                // No hay siguiente secuencia (es la última)
                _logger.LogInformation("No hay siguiente secuencia para flujo {IdFlujoActivo}, secuencia actual {SecuenciaActual} es la última",
                    idFlujoActivo, flujoActivo.IdSecuenciaActual);

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al identificar siguiente secuencia para flujo activo {IdFlujoActivo}", idFlujoActivo);
                throw;
            }
        }

        private List<int> ParsearOrdenSecuencias(string ordenSecuencias)
        {
            if (string.IsNullOrWhiteSpace(ordenSecuencias))
                return new List<int>();

            return ordenSecuencias.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id.Trim(), out int result) ? result : 0)
                .Where(id => id > 0)
                .ToList();
        }
    }
}