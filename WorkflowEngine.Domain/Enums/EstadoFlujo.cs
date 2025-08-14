using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Domain.Enums
{
    public enum EstadoFlujo
    {
        Iniciado = 1,
        EnProgreso = 2,
        EsperandoDatos = 3,
        Fallido = 4,
        Completado = 5,
        Cancelado = 6
    }
}
