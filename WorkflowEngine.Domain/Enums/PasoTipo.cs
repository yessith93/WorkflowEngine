using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Domain.Enums
{
    public enum PasoTipo
    {
        RegistroUsuario = 1,
        FormularioDatos = 2,
        EnvioCorreo = 3,
        ConfirmacionCorreo = 4,
        CargaDocumento = 5,
        ConsultaTercero = 6,
        ServicioExterno = 7
    }
}
