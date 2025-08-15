using System.ComponentModel.DataAnnotations;

namespace WorkflowEngine.API.Models.Requests
{
    public class IniciarFlujoRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del tipo de flujo debe ser mayor a 0")]
        public int TipoFlujoId { get; set; }
    }
}
