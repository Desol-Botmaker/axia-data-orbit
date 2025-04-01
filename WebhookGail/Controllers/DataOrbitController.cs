using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebhookGail.Models;
using WebhookGail.Data;
using WebhookGail.Services;
using WebhookGail.Middleware;
using System.Net;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace WebhookGail.Controllers
{
    [ApiController]
    [Route("axia")]
    public class DataOrbitController : ControllerBase
    {
        private readonly ILogger<DataOrbitController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly WebhookGailContext _dbContext;
        private readonly Rapihogar _rapiInstance;

        public DataOrbitController(ILogger<DataOrbitController> logger, IConfiguration configuration, HttpClient httpClient, WebhookGailContext dbContext, Rapihogar rapiInstance)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _dbContext = dbContext;
            _rapiInstance = rapiInstance;
        }

        [HttpGet]
        [Route("getCustomers")]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> GetCustomersAsync()
        {
            try
            {
                var customers = await _dbContext.Customer.ToListAsync();

                if (customers == null || !customers.Any())
                {
                    return NotFound("No se encontraron clientes.");
                }

                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("getCustomer/{id}")]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> GetCustomerAsync(int id)
        {
            try
            {
                var customer = await _dbContext.Customer.FindAsync(id);

                if (customer == null)
                {
                    return NotFound("No se encontro customer con el id especificado.");
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("getCalls")]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> GetCallsAsync()
        {
            try
            {
                var calls = await _dbContext.LogWebhookGailView.ToListAsync();

                if (calls == null || !calls.Any())
                {
                    return NotFound("No se encontraron clientes.");
                }

                return Ok(calls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("getBusinessCalls")]
        [ServiceFilter(typeof(TokenValidationMiddleware))]

        public async Task<IActionResult> GetBusinessCallsAsync([FromQuery] string businessName)
        {
            try
            {
                var callLog = await _dbContext.LogWebhookGailView
                    .Where(l => l.BusinessName == businessName)
                    .ToListAsync();


                if (callLog != null)
                {
                    return Ok(callLog);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("getCallsByDate")]
        [ServiceFilter(typeof(TokenValidationMiddleware))]

        public async Task<IActionResult> GetCallsByDateAsync([FromQuery] string businessName, DateTime from, DateTime to)
        {
            if (string.IsNullOrEmpty(businessName))
            {
                return BadRequest("Business name is required.");
            }

            try
            {
                var callLog = await _dbContext.LogWebhookGailView
                    .Where(l => l.BusinessName == businessName && l.Date >= from && l.Date <= to)
                    .ToListAsync();


                if (callLog != null)
                {
                    return Ok(callLog);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("createCustomer")]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> CreateCustomerAsync([FromBody] JsonElement customerInfo)
        {
            Customer customerData = new Customer();
            customerData = JsonSerializer.Deserialize<Customer>(customerInfo, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException("Error de deserializacion");
            try
            {
                var createCustomer = new Customer
                {
                    Name = customerData.Name,
                    Email = customerData.Email,
                    Whatsapp = customerData.Whatsapp
                };

                _dbContext.Customer.Add(createCustomer);
                await _dbContext.SaveChangesAsync();
                return StatusCode(201, createCustomer);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creando customer: {Message}", ex.Message);
                return StatusCode(500, new { message = "Error al crear el cliente", detail = ex.Message });
            }

        }

        [HttpGet]
        [Route("getCustomerData")]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> GetCustomerDataAsync([FromQuery] long CUIT, int NroBoca)
        {
            try
            {
                //BigInteger cuitNum = BigInteger.Parse(CUIT);

                List<DataTest> dataTest = await _dbContext.DataTest
                    .Where(l => l.CUIT == CUIT && l.NroBoca == NroBoca)
                    .ToListAsync();

                if (dataTest.Any())
                {
                    return Ok(dataTest);
                }
                else
                {
                    return NotFound("Data not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet]
        [Route("getDistribuidoresOficiales")]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> GetDistribuidoresOficialesAsync([FromQuery] int? codigoPostal, [FromQuery] string? localidad)
        {
            if (string.IsNullOrEmpty(localidad) && codigoPostal < 1)
            {
                return BadRequest("Ingrese un código postal o la localidad.");
            }

            try
            {
                List<YPFDistribuidoresOficiales> datos;

                if (codigoPostal > 0)
                {
                    datos = await _dbContext.YPFDistribuidoresOficiales
                        .Where(l => l.CP == codigoPostal)
                        .ToListAsync();
                }
                else
                {
                    datos = await _dbContext.YPFDistribuidoresOficiales
                        .Where(l => l.Localidad.ToLower().Contains(localidad.ToLower()))
                        .ToListAsync();
                }

                if (datos.Any())
                {
                    return Ok(datos);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]
        [Route("getInfoByBoca")]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> GetInfoByBocaAsync([FromQuery] string? CUIT, [FromQuery] string? nroBoca)
        {
            if ((string.IsNullOrEmpty(CUIT) && string.IsNullOrEmpty(nroBoca)) || string.IsNullOrEmpty(nroBoca))
            {
                return BadRequest("Ingrese un CUIT y número de boca.");
            }

            try
            {                
                return Ok(CUIT);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet]
        [Route("getInfoRapiHogar")]
        [ServiceFilter(typeof(TokenValidationMiddleware))]
        public async Task<IActionResult> GetCustomerInfoAsync([FromQuery] string id)
        {
            Cliente? client = new Cliente();
            try
            {
                _logger.LogInformation("rapiToken");
                string rapiToken = await _rapiInstance.GetAccessTokenAsync();

                _logger.LogInformation(rapiToken);
                var stringRapiToken = rapiToken.ToString();
                    
                var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"https://backend.rapihogar.com.ar/v1/axia/client/305/0/{id}");
                request.Headers.Add("Authorization", "Bearer " + stringRapiToken);
                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Cliente? cliente = JsonSerializer.Deserialize<Cliente>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    jsonResponse = JsonSerializer.Serialize(cliente);

                    if (client?.Id != null)
                    {
                        return Ok(jsonResponse);
                    }
                    else
                    {
                        return BadRequest(new { status = "Cliente no encontrado" });
                    }
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new { status = "Recurso no encontrado" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new { status = "Request failed", reason = response.ReasonPhrase });
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { status = "Error al realizar el req", error = ex.Message });
            }
            catch (JsonException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "JSON error", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "Error inesperado", error = ex.Message });
            }

        }
    }
}




