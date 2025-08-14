namespace WorkflowEngine.Domain.Entities
{
    public class Secuencia
    {
        public int Id { get; set; }
        public int IdTipoFlujo { get; set; }
        // Lista de IDs de pasos como cadena (puedes normalizar más si quieres)
        public string ListaIdPasos { get; set; } = string.Empty;
        // Navegación
        public TipoFlujo TipoFlujo { get; set; } = null!;
        public ICollection<Paso> Pasos { get; set; } = new List<Paso>();
    }
}
