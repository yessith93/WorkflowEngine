namespace WorkflowEngine.Domain.Entities
{
    public class PasoCampo
    {
        public int IdPaso { get; set; }
        public int IdCampo { get; set; }

        // Propiedades adicionales opcionales para la relación
        public bool EsRequerido { get; set; } = true;
        public int Orden { get; set; } = 1; // Para ordenar campos en el formulario

        // Navigation properties
        public Paso Paso { get; set; } = null!;
        public Campo Campo { get; set; } = null!;
    }
}