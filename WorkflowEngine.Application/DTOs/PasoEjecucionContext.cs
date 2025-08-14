using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Application.DTOs
{
    public class PasoEjecucionContext
    {
        public int IdPaso { get; set; }
        public PasoTipo TipoPaso { get; set; }
        public Dictionary<int, string> Datos { get; set; } = new Dictionary<int, string>();
        public List<int> IdsCamposRequeridos { get; set; } = new List<int>();
    }
}
