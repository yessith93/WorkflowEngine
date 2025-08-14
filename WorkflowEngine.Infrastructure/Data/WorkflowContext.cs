using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Infrastructure.Data
{
    public class WorkflowContext : DbContext
    {
        public WorkflowContext(DbContextOptions<WorkflowContext> options) : base(options)
        {
        }

        // DbSets - Mapeo de entidades a tablas
        public DbSet<TipoFlujo> TiposFlujo { get; set; }
        public DbSet<Secuencia> Secuencias { get; set; }
        public DbSet<Paso> Pasos { get; set; }
        public DbSet<Campo> Campos { get; set; }
        public DbSet<PasoCampo> PasosCampos { get; set; }
        public DbSet<FlujoActivo> FlujosActivos { get; set; }
        public DbSet<CampoFlujoActivo> CamposFlujosActivos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar TipoFlujo
            modelBuilder.Entity<TipoFlujo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.OrdenSecuencias)
                    .HasMaxLength(500);
            });

            // Configurar Secuencia
            modelBuilder.Entity<Secuencia>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ListaIdPasos)
                    .HasMaxLength(500);

                // Relación con TipoFlujo
                entity.HasOne(e => e.TipoFlujo)
                    .WithMany(t => t.Secuencias)
                    .HasForeignKey(e => e.IdTipoFlujo)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurar Paso
            modelBuilder.Entity<Paso>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TipoPaso)
                    .IsRequired();
                entity.Property(e => e.ListaIdCampos)
                    .HasMaxLength(500);
            });

            // Configurar Campo
            modelBuilder.Entity<Campo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MensajeCliente)
                    .IsRequired()
                    .HasMaxLength(500);
                entity.Property(e => e.RegexCliente)
                    .HasMaxLength(200);
                entity.Property(e => e.RegexServidor)
                    .HasMaxLength(200);
                entity.Property(e => e.TipoDato)
                    .HasMaxLength(50);
            });

            // Configurar PasoCampo (tabla intermedia)
            modelBuilder.Entity<PasoCampo>(entity =>
            {
                // Clave compuesta
                entity.HasKey(pc => new { pc.IdPaso, pc.IdCampo });

                // Relación con Paso
                entity.HasOne(pc => pc.Paso)
                    .WithMany(p => p.PasosCampos)
                    .HasForeignKey(pc => pc.IdPaso)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Campo
                entity.HasOne(pc => pc.Campo)
                    .WithMany(c => c.PasosCampos)
                    .HasForeignKey(pc => pc.IdCampo)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurar muchos-a-muchos Paso-Campo (navegación directa)
            modelBuilder.Entity<Paso>()
                .HasMany(p => p.Campos)
                .WithMany(c => c.Pasos)
                .UsingEntity<PasoCampo>(
                    j => j.HasOne(pc => pc.Campo).WithMany(c => c.PasosCampos),
                    j => j.HasOne(pc => pc.Paso).WithMany(p => p.PasosCampos),
                    j => j.HasKey(pc => new { pc.IdPaso, pc.IdCampo })
                );

            // Configurar FlujoActivo
            modelBuilder.Entity<FlujoActivo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EstadoFlujo)
                    .IsRequired();
                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relación con TipoFlujo
                entity.HasOne(e => e.TipoFlujo)
                    .WithMany(t => t.FlujosActivos)
                    .HasForeignKey(e => e.IdTipoFlujo)
                    .OnDelete(DeleteBehavior.Restrict); // No eliminar TipoFlujo si hay flujos activos
            });

            // Configurar CampoFlujoActivo
            modelBuilder.Entity<CampoFlujoActivo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Valor)
                    .IsRequired()
                    .HasMaxLength(1000);

                // Relación con FlujoActivo
                entity.HasOne(e => e.FlujoActivo)
                    .WithMany(f => f.CamposFlujosActivos)
                    .HasForeignKey(e => e.IdFlujoActivo)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Campo
                entity.HasOne(e => e.Campo)
                    .WithMany(c => c.CamposFlujosActivos)
                    .HasForeignKey(e => e.IdCampo)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índice único: Un campo solo puede tener un valor por flujo activo
                entity.HasIndex(e => new { e.IdFlujoActivo, e.IdCampo })
                    .IsUnique();
            });

            // Configurar nombres de tablas (opcional)
            modelBuilder.Entity<TipoFlujo>().ToTable("TiposFlujo");
            modelBuilder.Entity<Secuencia>().ToTable("Secuencias");
            modelBuilder.Entity<Paso>().ToTable("Pasos");
            modelBuilder.Entity<Campo>().ToTable("Campos");
            modelBuilder.Entity<PasoCampo>().ToTable("PasosCampos");
            modelBuilder.Entity<FlujoActivo>().ToTable("FlujosActivos");
            modelBuilder.Entity<CampoFlujoActivo>().ToTable("CamposFlujosActivos");
        }
    }
}
