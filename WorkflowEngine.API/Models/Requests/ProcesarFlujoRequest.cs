using System.ComponentModel.DataAnnotations;

namespace WorkflowEngine.API.Models.Requests
{
    public class ProcesarFlujoRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del flujo activo debe ser mayor a 0")]
        public int FlujoActivoId { get; set; }

        [Required]
        public Dictionary<int, string> DatosFormulario { get; set; } = new Dictionary<int, string>();
    }
}
