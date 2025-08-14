using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Application.DTOs
{
    public class ResultadoPrepararSecuencia
    {
        public bool EsValido { get; set; }
        public List<int>? IdsPasos { get; set; }
        public Dictionary<int, string>? DatosPreparados { get; set; }
        public List<CampoRequerido>? CamposRequeridos { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string? Error { get; set; }
    }
}
