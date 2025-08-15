// WorkflowEngine.Infrastructure/Data/SeedData.cs
using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace WorkflowEngine.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(WorkflowContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (await context.Campos.AnyAsync())
                return;

            // 1️⃣ Insertar Campos
            var campos = await SeedCamposAsync(context);

            // 2️⃣ Insertar Pasos
            var pasos = await SeedPasosAsync(context, campos);

            // 3️⃣ Crear TiposFlujo en memoria
            var tipoFlujoBasico = new TipoFlujo
            {
                Nombre = "Registro Básico de Usuario"
            };

            var tipoFlujoOnboarding = new TipoFlujo
            {
                Nombre = "Onboarding Completo"
            };

            // 4️⃣ Crear Secuencias usando navegación
            var secuencias = new List<Secuencia>
            {
                // Flujo básico
                new Secuencia { ListaIdPasos = $"{pasos[0].Id}", TipoFlujo = tipoFlujoBasico },
                new Secuencia { ListaIdPasos = $"{pasos[1].Id}", TipoFlujo = tipoFlujoBasico },
                new Secuencia { ListaIdPasos = $"{pasos[2].Id}", TipoFlujo = tipoFlujoBasico },
                new Secuencia { ListaIdPasos = $"{pasos[3].Id}", TipoFlujo = tipoFlujoBasico },

                // Flujo onboarding
                new Secuencia { ListaIdPasos = $"{pasos[0].Id}", TipoFlujo = tipoFlujoOnboarding },
                new Secuencia { ListaIdPasos = $"{pasos[1].Id},{pasos[2].Id}", TipoFlujo = tipoFlujoOnboarding },
                new Secuencia { ListaIdPasos = $"{pasos[4].Id}", TipoFlujo = tipoFlujoOnboarding },
                new Secuencia { ListaIdPasos = $"{pasos[5].Id}", TipoFlujo = tipoFlujoOnboarding },
                new Secuencia { ListaIdPasos = $"{pasos[6].Id}", TipoFlujo = tipoFlujoOnboarding },
                new Secuencia { ListaIdPasos = $"{pasos[7].Id},{pasos[3].Id}", TipoFlujo = tipoFlujoOnboarding }
            };

            // 5️⃣ Guardar todo en un solo SaveChanges
            await context.TiposFlujo.AddRangeAsync(tipoFlujoBasico, tipoFlujoOnboarding);
            await context.Secuencias.AddRangeAsync(secuencias);
            await context.SaveChangesAsync();

            // 6️⃣ Actualizar los campos de IdSecuenciaInicial y IdSecuenciaFinal
            tipoFlujoBasico.IdSecuenciaInicial = secuencias[0].Id;
            tipoFlujoBasico.IdSecuenciaFinal = secuencias[3].Id;
            tipoFlujoBasico.OrdenSecuencias = $"{secuencias[0].Id},{secuencias[1].Id},{secuencias[2].Id},{secuencias[3].Id}";

            tipoFlujoOnboarding.IdSecuenciaInicial = secuencias[4].Id;
            tipoFlujoOnboarding.IdSecuenciaFinal = secuencias[9].Id;
            tipoFlujoOnboarding.OrdenSecuencias = $"{secuencias[4].Id},{secuencias[5].Id},{secuencias[6].Id},{secuencias[7].Id},{secuencias[8].Id},{secuencias[9].Id}";

            await context.SaveChangesAsync();
        }

        private static async Task<List<Campo>> SeedCamposAsync(WorkflowContext context)
        {
            var campos = new List<Campo>
            {
                new Campo { MensajeCliente = "Ingrese su nombre completo", RegexCliente = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]{2,50}$", RegexServidor = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]{2,50}$", EsInterno = false, TipoDato = "string" },
                new Campo { MensajeCliente = "Ingrese su correo electrónico", RegexCliente = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexServidor = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", EsInterno = false, TipoDato = "email" },
                new Campo { MensajeCliente = "Ingrese su número de teléfono", RegexCliente = @"^[\+]?[1-9][\d]{0,15}$", RegexServidor = @"^[\+]?[1-9][\d]{0,15}$", EsInterno = false, TipoDato = "phone" },
                new Campo { MensajeCliente = "Ingrese su fecha de nacimiento (DD/MM/YYYY)", RegexCliente = @"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[0-2])/\d{4}$", RegexServidor = @"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[0-2])/\d{4}$", EsInterno = false, TipoDato = "date" },
                new Campo { MensajeCliente = "Código de confirmación generado", RegexCliente = @"^\d{6}$", RegexServidor = @"^\d{6}$", EsInterno = true, TipoDato = "string" },
                new Campo { MensajeCliente = "Ingrese el código de confirmación recibido por email", RegexCliente = @"^\d{6}$", RegexServidor = @"^\d{6}$", EsInterno = false, TipoDato = "string" },
                new Campo { MensajeCliente = "Seleccione un documento (PDF máx. 5MB)", RegexCliente = @".*\.(pdf|PDF)$", RegexServidor = @".*\.(pdf|PDF)$", EsInterno = false, TipoDato = "file" },
                new Campo { MensajeCliente = "Número de documento de identidad", RegexCliente = @"^\d{8,12}$", RegexServidor = @"^\d{8,12}$", EsInterno = false, TipoDato = "string" }
            };

            await context.Campos.AddRangeAsync(campos);
            await context.SaveChangesAsync();
            return campos;
        }

        private static async Task<List<Paso>> SeedPasosAsync(WorkflowContext context, List<Campo> campos)
        {
            var pasos = new List<Paso>
            {
                new Paso { TipoPaso = PasoTipo.FormularioDatos, ListaIdCampos = $"{campos[0].Id},{campos[1].Id}" },
                new Paso { TipoPaso = PasoTipo.EnvioCorreo, ListaIdCampos = $"{campos[1].Id},{campos[4].Id}" },
                new Paso { TipoPaso = PasoTipo.ConfirmacionCorreo, ListaIdCampos = $"{campos[5].Id}" },
                new Paso { TipoPaso = PasoTipo.RegistroUsuario, ListaIdCampos = $"{campos[0].Id},{campos[1].Id}" },
                new Paso { TipoPaso = PasoTipo.FormularioDatos, ListaIdCampos = $"{campos[2].Id},{campos[3].Id},{campos[7].Id}" },
                new Paso { TipoPaso = PasoTipo.ConsultaTercero, ListaIdCampos = $"{campos[7].Id}" },
                new Paso { TipoPaso = PasoTipo.CargaDocumento, ListaIdCampos = $"{campos[6].Id}" },
                new Paso { TipoPaso = PasoTipo.ServicioExterno, ListaIdCampos = $"{campos[6].Id}" }
            };

            await context.Pasos.AddRangeAsync(pasos);
            await context.SaveChangesAsync();
            return pasos;
        }
    }
}
