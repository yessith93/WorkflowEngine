using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Application.DTOs
{
    public class CamposConsultaResult
    {
        public bool TodosCompletos { get; set; }
        public List<int> IdsCamposFaltantes { get; set; } = new List<int>();
        public Dictionary<int, string> DatosCompletos { get; set; } = new Dictionary<int, string>();
    }
}
