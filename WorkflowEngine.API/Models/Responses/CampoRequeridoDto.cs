namespace WorkflowEngine.API.Models.Responses
{
    public class CampoRequeridoDto
    {
        public int IdCampo { get; set; }
        public string MensajeSolicitud { get; set; } = string.Empty;
        public string RegexValidacionCliente { get; set; } = string.Empty;
        public string TipoDato { get; set; } = string.Empty;
        public bool EsInterno { get; set; }
    }
}
