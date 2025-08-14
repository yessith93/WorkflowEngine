using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Domain.Entities
{
    public class Paso
    {
        public int Id { get; set; }
        // Tipo de paso: registro, envío de correo, etc.
        public PasoTipo TipoPaso { get; set; }
        // MANTENER: Lista de IDs de campos como cadena (funciona perfecto)
        public string ListaIdCampos { get; set; } = string.Empty;

        // Navegación: Opción normalizada muchos-a-muchos
        public ICollection<PasoCampo> PasosCampos { get; set; } = new List<PasoCampo>();

        // Navegación: Acceso directo a campos (EF Core generará esto automáticamente)
        public ICollection<Campo> Campos { get; set; } = new List<Campo>();
    }
}
