namespace WorkflowEngine.Domain.Entities
{
    public class Campo
    {
        public int Id { get; set; }
        public string MensajeCliente { get; set; } = string.Empty;
        public string RegexCliente { get; set; } = string.Empty;
        public string RegexServidor { get; set; } = string.Empty;
        public bool EsInterno { get; set; }
        public string TipoDato { get; set; } = string.Empty;

        // Navegación: Relación muchos-a-muchos con Paso
        public ICollection<PasoCampo> PasosCampos { get; set; } = new List<PasoCampo>();

        // Navegación: Acceso directo a pasos
        public ICollection<Paso> Pasos { get; set; } = new List<Paso>();

        // Navegación: Campos en flujos activos
        public ICollection<CampoFlujoActivo> CamposFlujosActivos { get; set; } = new List<CampoFlujoActivo>();
    }
}