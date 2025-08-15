using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.API.Models.Requests;
using WorkflowEngine.API.Models.Responses;
using WorkflowEngine.Application.Interfaces.UseCases;
using System.ComponentModel.DataAnnotations;

namespace WorkflowEngine.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class WorkflowController : ControllerBase
    {
        private readonly IIniciarFlujoUseCase _iniciarFlujoUseCase;
        private readonly IProcesarFlujoUseCase _procesarFlujoUseCase;
        private readonly ILogger<WorkflowController> _logger;

        public WorkflowController(
            IIniciarFlujoUseCase iniciarFlujoUseCase,
            IProcesarFlujoUseCase procesarFlujoUseCase,
            ILogger<WorkflowController> logger)
        {
            _iniciarFlujoUseCase = iniciarFlujoUseCase;
            _procesarFlujoUseCase = procesarFlujoUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Inicia un nuevo flujo de trabajo
        /// </summary>
        /// <param name="request">Datos para iniciar el flujo</param>
        /// <returns>Información del flujo iniciado</returns>
        [HttpPost("iniciar")]
        [ProducesResponseType(typeof(ApiResponse<IniciarFlujoResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IniciarFlujoResponse>>> IniciarFlujo([FromBody] IniciarFlujoRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando flujo para tipo de flujo: {TipoFlujoId}", request.TipoFlujoId);

                // Validar modelo
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResponse("Datos de entrada inválidos", errors));
                }

                // Ejecutar caso de uso
                var resultado = await _iniciarFlujoUseCase.ExecuteAsync(request.TipoFlujoId);

                if (!resultado.Exitoso)
                {
                    _logger.LogWarning("Error al iniciar flujo {TipoFlujoId}: {Mensaje}",
                        request.TipoFlujoId, resultado.Mensaje);

                    return BadRequest(ApiResponse<object>.ErrorResponse(resultado.Mensaje));
                }

                // Mapear respuesta
                var response = new IniciarFlujoResponse
                {
                    FlujoActivoId = resultado.IdFlujoActivo,
                    RequiereDatos = resultado.CamposRequeridos?.Any() == true,
                    FlujoCompleto = resultado.FlujoCompleto,
                    CamposRequeridos = resultado.CamposRequeridos?.Select(c => new CampoRequeridoDto
                    {
                        IdCampo = c.IdCampo,
                        MensajeSolicitud = c.MensajeSolicitud,
                        RegexValidacionCliente = c.RegexValidacionCliente,
                        TipoDato = c.TipoDato,
                        EsInterno = c.EsInterno
                    }).ToList()
                };

                var message = resultado.FlujoCompleto
                    ? "Flujo iniciado y completado exitosamente"
                    : resultado.CamposRequeridos?.Any() == true
                        ? "Flujo iniciado. Se requieren datos adicionales"
                        : "Flujo iniciado exitosamente";

                _logger.LogInformation("Flujo iniciado exitosamente. ID: {FlujoActivoId}", resultado.IdFlujoActivo);

                return Ok(ApiResponse<IniciarFlujoResponse>.SuccessResponse(response, message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido al iniciar flujo: {TipoFlujoId}", request.TipoFlujoId);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al iniciar flujo: {TipoFlujoId}", request.TipoFlujoId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Procesa datos de un flujo activo y continúa con la ejecución
        /// </summary>
        /// <param name="request">Datos del formulario para el flujo</param>
        /// <returns>Estado actualizado del flujo</returns>
        [HttpPost("procesar")]
        [ProducesResponseType(typeof(ApiResponse<ProcesarFlujoResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<ProcesarFlujoResponse>>> ProcesarFlujo([FromBody] ProcesarFlujoRequest request)
        {
            try
            {
                _logger.LogInformation("Procesando flujo: {FlujoActivoId} con {CantidadDatos} campos",
                    request.FlujoActivoId, request.DatosFormulario.Count);

                // Validar modelo
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResponse("Datos de entrada inválidos", errors));
                }

                // Ejecutar caso de uso
                var resultado = await _procesarFlujoUseCase.ExecuteAsync(request.FlujoActivoId, request.DatosFormulario);

                if (!resultado.Exitoso)
                {
                    _logger.LogWarning("Error al procesar flujo {FlujoActivoId}: {Mensaje}",
                        request.FlujoActivoId, resultado.Mensaje);

                    return BadRequest(ApiResponse<object>.ErrorResponse(
                        resultado.Mensaje,
                        !string.IsNullOrEmpty(resultado.Error) ? new List<string> { resultado.Error } : null));
                }

                // Mapear respuesta
                var response = new ProcesarFlujoResponse
                {
                    FlujoActivoId = resultado.IdFlujoActivo,
                    RequiereDatos = resultado.CamposRequeridos?.Any() == true,
                    FlujoCompleto = resultado.FlujoCompleto,
                    CamposRequeridos = resultado.CamposRequeridos?.Select(c => new CampoRequeridoDto
                    {
                        IdCampo = c.IdCampo,
                        MensajeSolicitud = c.MensajeSolicitud,
                        RegexValidacionCliente = c.RegexValidacionCliente,
                        TipoDato = c.TipoDato,
                        EsInterno = c.EsInterno
                    }).ToList()
                };

                var message = resultado.FlujoCompleto
                    ? "Flujo completado exitosamente"
                    : resultado.CamposRequeridos?.Any() == true
                        ? "Flujo procesado. Se requieren datos adicionales"
                        : "Flujo procesado exitosamente";

                _logger.LogInformation("Flujo procesado exitosamente. ID: {FlujoActivoId}, Completo: {FlujoCompleto}",
                    resultado.IdFlujoActivo, resultado.FlujoCompleto);

                return Ok(ApiResponse<ProcesarFlujoResponse>.SuccessResponse(response, message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido al procesar flujo: {FlujoActivoId}", request.FlujoActivoId);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al procesar flujo: {FlujoActivoId}", request.FlujoActivoId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Obtiene el estado actual de un flujo
        /// </summary>
        /// <param name="flujoActivoId">ID del flujo activo</param>
        /// <returns>Estado del flujo</returns>
        [HttpGet("{flujoActivoId}/estado")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<object>>> ObtenerEstadoFlujo([FromRoute] int flujoActivoId)
        {
            try
            {
                // Aquí podrías implementar un caso de uso para consultar estado
                // Por ahora retornamos un placeholder

                _logger.LogInformation("Consultando estado del flujo: {FlujoActivoId}", flujoActivoId);

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    FlujoActivoId = flujoActivoId,
                    Mensaje = "Endpoint de consulta de estado - Por implementar"
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar estado del flujo: {FlujoActivoId}", flujoActivoId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Error interno del servidor"));
            }
        }
    }
}