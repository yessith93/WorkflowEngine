using Microsoft.Extensions.Logging;
using WorkflowEngine.Application.Interfaces.Services;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Application.Services.PasoHandlers
{
    public class RegistroUsuarioPasoHandler : IPasoHandler
    {
        private readonly ILogger<RegistroUsuarioPasoHandler> _logger;

        public RegistroUsuarioPasoHandler(ILogger<RegistroUsuarioPasoHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> CanHandle(PasoTipo tipoPaso)
        {
            return await Task.FromResult(tipoPaso == PasoTipo.RegistroUsuario);
        }

        public async Task ExecuteAsync(Dictionary<int, string> datos)
        {
            _logger.LogInformation("Iniciando registro de usuario con datos: {Datos}",
                string.Join(", ", datos.Select(d => $"{d.Key}:{d.Value}")));

            await Task.Delay(1000);

            _logger.LogInformation("Registro de usuario completado exitosamente");
        }
    }

    public class EnvioCorreoPasoHandler : IPasoHandler
    {
        private readonly ILogger<EnvioCorreoPasoHandler> _logger;

        public EnvioCorreoPasoHandler(ILogger<EnvioCorreoPasoHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> CanHandle(PasoTipo tipoPaso)
        {
            return await Task.FromResult(tipoPaso == PasoTipo.EnvioCorreo);
        }

        public async Task ExecuteAsync(Dictionary<int, string> datos)
        {
            _logger.LogInformation("Enviando correo con datos: {Datos}",
                string.Join(", ", datos.Select(d => $"{d.Key}:{d.Value}")));

            // Simular trabajo asíncrono
            await Task.Delay(500);

            _logger.LogInformation("Correo electrónico enviado exitosamente");
        }
    }

    public class FormularioDatosPersonalesPasoHandler : IPasoHandler
    {
        private readonly ILogger<FormularioDatosPersonalesPasoHandler> _logger;

        public FormularioDatosPersonalesPasoHandler(ILogger<FormularioDatosPersonalesPasoHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> CanHandle(PasoTipo tipoPaso)
        {
            return await Task.FromResult(tipoPaso == PasoTipo.FormularioDatos);
        }

        public async Task ExecuteAsync(Dictionary<int, string> datos)
        {
            _logger.LogInformation("Procesando formulario de datos personales: {Datos}",
                string.Join(", ", datos.Select(d => $"{d.Key}:{d.Value}")));

            // Simular trabajo asíncrono
            await Task.Delay(300);

            _logger.LogInformation("Formulario de datos personales procesado exitosamente");
        }
    }

    public class ConfirmacionCorreoPasoHandler : IPasoHandler
    {
        private readonly ILogger<ConfirmacionCorreoPasoHandler> _logger;

        public ConfirmacionCorreoPasoHandler(ILogger<ConfirmacionCorreoPasoHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> CanHandle(PasoTipo tipoPaso)
        {
            return await Task.FromResult(tipoPaso == PasoTipo.ConfirmacionCorreo);
        }

        public async Task ExecuteAsync(Dictionary<int, string> datos)
        {
            _logger.LogInformation("Procesando confirmación de correo: {Datos}",
                string.Join(", ", datos.Select(d => $"{d.Key}:{d.Value}")));

            await Task.Delay(200);

            _logger.LogInformation("Confirmación de correo procesada exitosamente");
        }
    }

    public class CargarDocumentoPasoHandler : IPasoHandler
    {
        private readonly ILogger<CargarDocumentoPasoHandler> _logger;

        public CargarDocumentoPasoHandler(ILogger<CargarDocumentoPasoHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> CanHandle(PasoTipo tipoPaso)
        {
            return await Task.FromResult(tipoPaso == PasoTipo.CargaDocumento);
        }

        public async Task ExecuteAsync(Dictionary<int, string> datos)
        {
            _logger.LogInformation("Cargando documento con datos: {Datos}",
                string.Join(", ", datos.Select(d => $"{d.Key}:{d.Value}")));

            await Task.Delay(1500);

            _logger.LogInformation("Documento cargado exitosamente");
        }
    }

    public class ConsultarInformacionTerceroPasoHandler : IPasoHandler
    {
        private readonly ILogger<ConsultarInformacionTerceroPasoHandler> _logger;

        public ConsultarInformacionTerceroPasoHandler(ILogger<ConsultarInformacionTerceroPasoHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> CanHandle(PasoTipo tipoPaso)
        {
            return await Task.FromResult(tipoPaso == PasoTipo.ConsultaTercero);
        }

        public async Task ExecuteAsync(Dictionary<int, string> datos)
        {
            _logger.LogInformation("Consultando información de tercero: {Datos}",
                string.Join(", ", datos.Select(d => $"{d.Key}:{d.Value}")));

            await Task.Delay(800);

            _logger.LogInformation("Información de tercero consultada exitosamente");
        }
    }

    public class ServicioExternoPasoHandler : IPasoHandler
    {
        private readonly ILogger<ServicioExternoPasoHandler> _logger;

        public ServicioExternoPasoHandler(ILogger<ServicioExternoPasoHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> CanHandle(PasoTipo tipoPaso)
        {
            return await Task.FromResult(tipoPaso == PasoTipo.ServicioExterno);
        }

        public async Task ExecuteAsync(Dictionary<int, string> datos)
        {
            _logger.LogInformation("Ejecutando servicio externo: {Datos}",
                string.Join(", ", datos.Select(d => $"{d.Key}:{d.Value}")));

            
            await Task.Delay(2000);

            _logger.LogInformation("Servicio externo ejecutado exitosamente");
        }
    }
}