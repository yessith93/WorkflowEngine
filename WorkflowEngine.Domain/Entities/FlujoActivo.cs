using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Domain.Entities
{
    public class FlujoActivo
    {
        public int Id { get; set; }
        public int IdTipoFlujo { get; set; }
        public int IdSecuenciaActual { get; set; }
        public EstadoFlujo EstadoFlujo { get; set; }

        // NUEVAS propiedades para auditor�a
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaCompletado { get; set; }

        // Navegaci�n
        public TipoFlujo TipoFlujo { get; set; } = null!;
        public ICollection<CampoFlujoActivo> CamposFlujosActivos { get; set; } = new List<CampoFlujoActivo>();
    }
}