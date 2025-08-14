namespace WorkflowEngine.Domain.Entities
{
    public class TipoFlujo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int IdSecuenciaInicial { get; set; }
        public int IdSecuenciaFinal { get; set; }
        public string OrdenSecuencias { get; set; } = string.Empty;
        public ICollection<Secuencia> Secuencias { get; set; } = new List<Secuencia>();
        public ICollection<FlujoActivo> FlujosActivos { get; set; } = new List<FlujoActivo>();
    }
}
