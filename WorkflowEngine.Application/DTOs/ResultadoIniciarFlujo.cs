using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Application.DTOs
{
    public class ResultadoIniciarFlujo
    {
        public bool Exitoso { get; set; }
        public int IdFlujoActivo { get; set; }
        public int IdSecuenciaInicial { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<CampoRequerido>? CamposRequeridos { get; set; }
        public bool FlujoCompleto { get; set; }
    }
}
