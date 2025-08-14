namespace WorkflowEngine.Domain.Entities
{
    public class CampoFlujoActivo
    {
        public int Id { get; set; }
        public int IdFlujoActivo { get; set; }
        public int IdCampo { get; set; }
        public string Valor { get; set; } = string.Empty;

        // Navigation properties
        public FlujoActivo FlujoActivo { get; set; } = null!;
        public Campo Campo { get; set; } = null!;
    }
}