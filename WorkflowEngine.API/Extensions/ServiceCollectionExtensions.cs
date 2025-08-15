// WorkflowEngine.API/Extensions/ServiceCollectionExtensions.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.API.Models.Responses;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Application.Interfaces.Services;
using WorkflowEngine.Application.Interfaces.UseCases;
using WorkflowEngine.Application.Services;
using WorkflowEngine.Application.Services.PasoHandlers;
using WorkflowEngine.Application.UseCases;
using WorkflowEngine.Infrastructure.Data;
using WorkflowEngine.Infrastructure.Repositories;

namespace WorkflowEngine.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkflowEngineServices(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<WorkflowContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Unit of Work y Repositorios
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITipoFlujoRepository, TipoFlujoRepository>();
            services.AddScoped<ISecuenciaRepository, SecuenciaRepository>();
            services.AddScoped<IPasoRepository, PasoRepository>();
            services.AddScoped<ICampoRepository, CampoRepository>();
            services.AddScoped<IFlujoActivoRepository, FlujoActivoRepository>();
            services.AddScoped<ICampoFlujoActivoRepository, CampoFlujoActivoRepository>();

            // Casos de Uso
            services.AddScoped<IIniciarFlujoUseCase, IniciarFlujoUseCase>();
            services.AddScoped<IPrepararSecuenciaUseCase, PrepararSecuenciaUseCase>();
            services.AddScoped<IIdentificarSiguienteSecuenciaUseCase, IdentificarSiguienteSecuenciaUseCase>();
            services.AddScoped<IEjecutarPasosUseCase, EjecutarPasosUseCase>();
            services.AddScoped<IProcesarFlujoUseCase, ProcesarFlujoUseCase>();

            // Servicios
            services.AddScoped<IPasoExecutorService, PasoExecutorService>();

            // Handlers de Pasos
            services.AddScoped<IPasoHandler, RegistroUsuarioPasoHandler>();
            services.AddScoped<IPasoHandler, EnvioCorreoPasoHandler>();
            services.AddScoped<IPasoHandler, FormularioDatosPersonalesPasoHandler>();
            services.AddScoped<IPasoHandler, ConfirmacionCorreoPasoHandler>();
            services.AddScoped<IPasoHandler, CargarDocumentoPasoHandler>();
            services.AddScoped<IPasoHandler, ConsultarInformacionTerceroPasoHandler>();
            services.AddScoped<IPasoHandler, ServicioExternoPasoHandler>();

            return services;
        }

        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            // Configuración de controladores
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    // Personalizar respuesta de validación automática
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList();

                        var response = ApiResponse<object>.ErrorResponse("Datos de entrada inválidos", errors);

                        return new BadRequestObjectResult(response);
                    };
                });

            // Swagger/OpenAPI
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Workflow Engine API",
                    Version = "v1",
                    Description = "API para el manejo de flujos de trabajo dinámicos"
                });

                // Incluir comentarios XML si están disponibles
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            // CORS si es necesario
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            return services;
        }
    }
}
