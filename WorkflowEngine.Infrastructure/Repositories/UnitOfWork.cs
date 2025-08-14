using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Application.Interfaces;
using WorkflowEngine.Infrastructure.Data;

namespace WorkflowEngine.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WorkflowContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(WorkflowContext context)
        {
            _context = context;
            TiposFlujo = new TipoFlujoRepository(_context);
            Secuencias = new SecuenciaRepository(_context);
            Pasos = new PasoRepository(_context);
            Campos = new CampoRepository(_context);
            FlujosActivos = new FlujoActivoRepository(_context);
            CamposFlujosActivos = new CampoFlujoActivoRepository(_context);
        }

        public ITipoFlujoRepository TiposFlujo { get; private set; }
        public ISecuenciaRepository Secuencias { get; private set; }
        public IPasoRepository Pasos { get; private set; }
        public ICampoRepository Campos { get; private set; }
        public IFlujoActivoRepository FlujosActivos { get; private set; }
        public ICampoFlujoActivoRepository CamposFlujosActivos { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
