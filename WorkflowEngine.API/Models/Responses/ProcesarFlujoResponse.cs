namespace WorkflowEngine.API.Models.Responses
{
    public class ProcesarFlujoResponse
    {
        public int FlujoActivoId { get; set; }
        public bool RequiereDatos { get; set; }
        public bool FlujoCompleto { get; set; }
        public List<CampoRequeridoDto>? CamposRequeridos { get; set; }
    }
}
