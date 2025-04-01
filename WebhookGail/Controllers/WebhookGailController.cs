using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WebhookGail.Models;
using WebhookGail.Data;
using WebhookGail.Services;
using WebhookGail.Middleware;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace WebhookGail.Controllers
{
    [ApiController]
    [Route("axia/webhook")]
    public class WebhookGailController : ControllerBase
    {
        private readonly ILogger<WebhookGailController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly WebhookGailContext _dbContext;
        private readonly OpenAIService _openAI;
        private readonly WebhookService _webhookService;
        //private readonly SendWhatsappTemplate _sendWhatsappTemplate;

        public WebhookGailController(ILogger<WebhookGailController> logger, IConfiguration configuration, HttpClient httpClient, WebhookGailContext dbContext, OpenAIService openAI, WebhookService webhookService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _dbContext = dbContext;
            _openAI = openAI;
            _webhookService = webhookService;
        }

        [HttpGet]
        public IActionResult GetWebhook()
        {
            return Ok("ADO activo");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JsonElement payload, [FromQuery] string businessName)
        {
            _logger.LogDebug("Payload recibido: {Payload}", payload.ToString());

            //string payloadRecibido = JsonSerializer.Serialize(payload);

           // if (accessToken == _configuration["AppSettings:WebhookAccessToken"])
           // {
                try
                {
                    Payload payloadObject = JsonSerializer.Deserialize<Payload>(payload, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new CustomDateTimeConverter() }
                    }) ?? throw new InvalidOperationException("Error de deserializacion");

                    var logEntry = await _webhookService.ProcessPayloadAsync(payloadObject, businessName);
                    _logger.LogDebug($"logEntry: {JsonSerializer.Serialize(logEntry)}");
                    

                foreach (var question in payloadObject.call_information?.questions ?? new List<Question>())
                {
                    _logger.LogDebug("Pregunta: {Question}, Respuesta: {Answer}", question.question, question.answer);
                }

                _logger.LogInformation("Sumario: {Summary}, Resolucion: {Resolution}", payloadObject.call_information?.summary, payloadObject.call_information?.resolution);

                    return Ok(new { status = "success", receivedAt = DateTime.UtcNow, processedName = payloadObject.call_information?.name });
                }
                catch (Exception ex)
                {
                    var exceptionDetails = new
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        InnerException = ex.InnerException?.Message
                    };

                    _logger.LogError(JsonSerializer.Serialize(exceptionDetails), "Ha ocurrido un error al intentar recuperar la informacion");

                    return StatusCode(500, exceptionDetails);
                }
           // }
           // else return Unauthorized();
        }

        [HttpPost("failed")]

        public async Task<IActionResult> PostFailed([FromBody] JsonElement payload, [FromQuery] string businessName)
        {
            _logger.LogDebug(JsonSerializer.Serialize($"Llamada fallida: {payload}"));

            try { 
                Payload payloadObject = JsonSerializer.Deserialize<Payload>(payload, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new CustomDateTimeConverter() }
                }) ?? throw new InvalidOperationException("Error de deserializacion");

                var logEntry = await _webhookService.ProcessPayloadAsync(payloadObject, businessName);

                if (logEntry == null)
                {
                    return BadRequest("Bad payload");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                var exceptionDetails = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Source = ex.Source,
                    InnerException = ex.InnerException?.Message
                };

                _logger.LogError(JsonSerializer.Serialize(exceptionDetails), "Ha ocurrido un error al intentar recuperar la informacion");

                return StatusCode(500, exceptionDetails);
            }

        }
    }
}
