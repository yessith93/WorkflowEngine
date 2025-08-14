using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Application.DTOs;

namespace WorkflowEngine.Application.Interfaces.UseCases
{
    public interface IIniciarFlujoUseCase
    {
        Task<ResultadoIniciarFlujo> ExecuteAsync(int tipoFlujoId);
    }
}
