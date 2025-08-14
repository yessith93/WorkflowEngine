using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Application.DTOs
{
    public class CampoRequerido
    {
        public int IdCampo { get; set; }
        public string MensajeSolicitud { get; set; } = string.Empty;
        public string RegexValidacionCliente { get; set; } = string.Empty;
        public string TipoDato { get; set; } = string.Empty;
        public bool EsInterno { get; set; }
    }
}
