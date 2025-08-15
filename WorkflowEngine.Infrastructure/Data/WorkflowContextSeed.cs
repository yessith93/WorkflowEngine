using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Infrastructure.Data
{
    public static class WorkflowContextSeed
    {
        public static void Seed(WorkflowContext context)
        {
            // Ejemplo: Agregar un TipoFlujo si no existe
            if (!context.TiposFlujo.Any())
            {
                context.TiposFlujo.Add(new TipoFlujo
                {
                    Nombre = "Flujo de prueba"
                });
                context.SaveChanges();
            }

            
        }
    }
}
