using Microsoft.EntityFrameworkCore;
using WorkflowEngine.API.Extensions;
using WorkflowEngine.Infrastructure.Data;
using WorkflowEngine.API.Models.Responses;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddWorkflowEngineServices(builder.Configuration);
builder.Services.AddApiConfiguration();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Middleware de manejo global de errores
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var contextFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

        if (contextFeature != null)
        {
            logger.LogError(contextFeature.Error, "Error no controlado en la aplicación");

            var response = ApiResponse<object>.ErrorResponse("Error interno del servidor");
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }
    });
});

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Workflow Engine API V1");
        //c.RoutePrefix = string.Empty; // Swagger en la raíz
    });

    // Aplicar migraciones automáticamente 
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<WorkflowContext>();
        try
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            app.Logger.LogInformation("Base de datos creada exitosamente");

            // Insertar datos semilla
            await SeedData.InitializeAsync(context);
            app.Logger.LogInformation("Datos semilla cargados correctamente");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Error al crear base de datos y cargar datos semilla");
        }
    }
//}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();


// Middleware de logging de requests
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("Request: {Method} {Path}",
        context.Request.Method,
    context.Request.Path);

    await next.Invoke();

    logger.LogInformation("Response: {StatusCode}",
        context.Response.StatusCode);
});

app.MapControllers();
app.Logger.LogInformation("Workflow Engine API iniciada correctamente");


app.Run();
