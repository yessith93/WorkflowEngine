using Microsoft.EntityFrameworkCore;
using WorkflowEngine.Infrastructure.Data;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Infrastructure.Repositories;
using Microsoft.Win32;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<WorkflowContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar repositorios y UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITipoFlujoRepository, TipoFlujoRepository>();
builder.Services.AddScoped<ISecuenciaRepository, SecuenciaRepository>();
builder.Services.AddScoped<IPasoRepository, PasoRepository>();
builder.Services.AddScoped<ICampoRepository, CampoRepository>();
builder.Services.AddScoped<IFlujoActivoRepository, FlujoActivoRepository>();
builder.Services.AddScoped<ICampoFlujoActivoRepository, CampoFlujoActivoRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Aplicar migraciones automáticamente en desarrollo
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<WorkflowContext>();
    await context.Database.MigrateAsync();

    // Seed inicial de datos si la BD está vacía
    //await SeedDataAsync(context);
}

app.Run();
