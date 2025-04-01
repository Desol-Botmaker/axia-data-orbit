using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebhookGail.Middleware;
using WebhookGail.Models;
using WebhookGail.Services;

namespace WebhookGail.Controllers
{
    [ApiController]
    [Route("axia/survey")]
    public class SurveysController : ControllerBase
    {
        private readonly ILogger<SurveysController> _logger;
        private readonly Rapihogar _rapiInstance;
        private readonly IConfiguration _configuration;

        public SurveysController(ILogger<SurveysController> logger, Rapihogar rapiInstance, IConfiguration configuration)
        {
            _logger = logger;
            _rapiInstance = rapiInstance;
            _configuration = configuration;
        }


        [HttpPost]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> PostSurveyRapihogar([FromBody] JsonElement json)
        {

            if (json.ValueKind == JsonValueKind.Undefined || json.ValueKind == JsonValueKind.Null)
            {
                return BadRequest("La respuesta de la encuesta no puede ser nula.");
            }

            try
            {

                SurveyRapihogar? surveyRapihogar = JsonSerializer.Deserialize<SurveyRapihogar>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

                if (surveyRapihogar == null)
                {
                    return BadRequest("No se pudo deserializar la respuesta de la encuesta.");
                }

                await _rapiInstance.StartSurveyRapihogar(surveyRapihogar);

                return Ok();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error al insertar los datos de la encuesta");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Servicio no disponible.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al enviar la respuesta de la encuesta.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor.");
            }
        }

        [HttpPost("results")]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> PostSurveyResults([FromBody] JsonElement json)
        {

            if (json.ValueKind == JsonValueKind.Undefined || json.ValueKind == JsonValueKind.Null)
            {
                return BadRequest("La respuesta de la encuesta no puede ser nula.");
            }

            try
            {

                SurveyRapihogar? surveyRapihogar = JsonSerializer.Deserialize<SurveyRapihogar>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (surveyRapihogar == null)
                {
                    return BadRequest("No se pudo deserializar la respuesta de la encuesta.");
                }

                await _rapiInstance.StartSurveyRapihogar(surveyRapihogar);

                return Ok();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error al insertar los datos de la encuesta");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Servicio no disponible.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al enviar la respuesta de la encuesta.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor.");
            }
        }
    }
}
