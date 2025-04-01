using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WebhookGail.Models;
using WebhookGail.Data;
using Microsoft.EntityFrameworkCore;
using Azure;

namespace WebhookGail.Services
{
    public class Rapihogar
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Rapihogar> _logger; 
        private readonly HttpClient _httpClient;
        private readonly WebhookGailContext _dbContext;
        private readonly AxiaApiService _axiaApiService;

        public Rapihogar(IConfiguration configuration, ILogger<Rapihogar> logger, HttpClient httpClient, WebhookGailContext dbContext, AxiaApiService axiaApiService)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
            _dbContext = dbContext;
            _axiaApiService = axiaApiService;
        }

        private string _currentAccessToken;
        private DateTime _tokenExpirationTime;

        public async Task<string> GetAccessTokenAsync()
        {
            try
            {
                _logger.LogDebug("Token expiration time: " + _tokenExpirationTime);

                if (!string.IsNullOrEmpty(_currentAccessToken) && DateTime.UtcNow < _tokenExpirationTime)
                {
                    _logger.LogDebug("Reutilizando el token existente.");
                    return _currentAccessToken;
                }

                string? uri = _configuration["AppSettings:RapihogarAPI"] + "oauth/token/";
                if (string.IsNullOrEmpty(uri))
                {
                    throw new Exception("El URI para obtener el token no está configurado.");
                }

                _logger.LogDebug($"Requesting token from URI: {uri}");

                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, uri);

                var parameters = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", _configuration["AppSettings:RapihogarClientId"] ?? "" },
                    { "client_secret", _configuration["AppSettings:RapihogarClientSecret"] ?? "" }
                };

                request.Content = new FormUrlEncodedContent(parameters);

                _logger.LogDebug("Sending token request...");
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error al obtener el token: {response.StatusCode} - {response.ReasonPhrase}");
                    throw new Exception($"Error al obtener el token: {response.ReasonPhrase}");
                }

                var contentString = await response.Content.ReadAsStringAsync();
                _logger.LogDebug($"Token response content: {contentString}");

                var accessTokenResponse = JsonSerializer.Deserialize<RapiToken>(contentString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (accessTokenResponse == null || string.IsNullOrEmpty(accessTokenResponse.access_token))
                {
                    throw new Exception("El token de acceso no es válido o no fue proporcionado en la respuesta.");
                }

                _currentAccessToken = accessTokenResponse.access_token;
                _tokenExpirationTime = DateTime.UtcNow.AddSeconds(36000);

                _logger.LogDebug($"Nuevo token obtenido: {_currentAccessToken}");
                _logger.LogDebug($"Token expira en: {_tokenExpirationTime}");

                return _currentAccessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el token.");
                throw;
            }
        }



        public async Task<SurveyResponseResult> SendSurveyResponseAsync(JsonElement surveyResponse, Endpoints endpoints)
        {
            _logger.LogInformation("Enviando respuestas de encuesta.");

            //string? baseUrl = _configuration["AppSettings:RapihogarAPI"];
            //string? version = _configuration["AppSettings:RapihogarAPIVersion"];
            string? endpoint = endpoints.Endpoint; //$"{baseUrl}/{version}/axia/survey/";
            _logger.LogDebug($"Endpoint: {endpoint}");
            string bearerToken = await GetAccessTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            _logger.LogDebug($"Request: {JsonSerializer.Serialize(request)}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            request.Content = new StringContent(JsonSerializer.Serialize(surveyResponse), Encoding.UTF8, "application/json");
            var response = new HttpResponseMessage();
            try
            {
                response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string contentString = await response.Content.ReadAsStringAsync();
                var surveyResponseResult = JsonSerializer.Deserialize<SurveyResponseResult>(contentString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation("Respuestas de encuesta enviadas correctamente.");
                return surveyResponseResult;
            }
            catch (HttpRequestException ex)
            {
                if (ex.Data.Contains("response"))
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error del servidor: {errorContent}");
                }
                _logger.LogError(ex, $"Error al enviar las respuestas de la encuesta. {ex.Message}");
                throw;
            }

        }

        public async Task StartSurveyRapihogar(SurveyRapihogar surveyRapihogar)
        {
            try
            {
                Contact contact = CreateContact(surveyRapihogar);
                var createContactResponse = await _axiaApiService.PostContactAsync(contact);

                if (!createContactResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error al crear el contacto: {createContactResponse.StatusCode}");
                    return;
                }

                var contactResponseContent = await createContactResponse.Content.ReadAsStringAsync();
                var createdContact = JsonSerializer.Deserialize<ContactResponse>(contactResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (createdContact == null)
                    throw new Exception("Failed to parse contact creation response");

                string? contactId = createdContact.id;

                string contactListId = await GetOrCreateContactListAsync(surveyRapihogar.Campaign);

                if (string.IsNullOrEmpty(contactListId))
                    throw new Exception("Contact list creation or retrieval failed");

                var addContactResponse = await _axiaApiService.AddSingleContactToListAsync(contactListId, contactId);
                if (!addContactResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error al añadir el contacto a la lista: {addContactResponse.StatusCode}");
                    return;
                }

                await GetOrCreateAndStartCampaignAsync(surveyRapihogar, contactListId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while starting survey for Rapihogar");
            }
        }

        private Contact CreateContact(SurveyRapihogar surveyRapihogar)
        {
            return new Contact
            {
                firstName = surveyRapihogar.NombreAsegurado,
                lastName = "",
                emails = new List<string> { "info@desol.cloud" },
                phoneNumbers = new List<PhoneNumber>
        {
            new PhoneNumber
            {
                number = "+54" + surveyRapihogar.PhoneNumber,
                type = "home"
            }
        },
                businessName = surveyRapihogar.Aseguradora,
                additionalData = new Dictionary<string, string>
                {
                    { "idPedido", surveyRapihogar.IdPedido.ToString() ?? "" },
                    { "aseguradora", surveyRapihogar.Aseguradora ?? "" },
                    { "prestador", surveyRapihogar.NombrePrestador ?? "" },
                    { "rubro", surveyRapihogar.Rubro ?? "" },
                    { "fechaServicio", surveyRapihogar.FechaSevicio ?? "" }
                }
            };
        }

        private async Task<string> GetOrCreateContactListAsync(string campaignName)
        {
            var contactListsResponse = await _axiaApiService.GetContactListsAsync();

            if (contactListsResponse.IsSuccessStatusCode)
            {
                var responseContent = await contactListsResponse.Content.ReadAsStringAsync();
                var contactLists = JsonSerializer.Deserialize<List<ContactListResponse>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var existingList = contactLists?.FirstOrDefault(cl => cl.name == campaignName);
                if (existingList != null)
                {
                    _logger.LogDebug($"Utilizando lista de contactos existente: {existingList.name}");
                    return existingList.id;
                }
            }
            else
            {
                _logger.LogError($"Error al obtener la lista de contactos: {contactListsResponse.StatusCode}");
            }

            var newContactList = new ContactList
            {
                name = campaignName ?? "",
                description = campaignName ?? ""
            };

            var createContactListResponse = await _axiaApiService.CreateContactListAsync(newContactList);
            if (!createContactListResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"Error al crear la lista de contactos: {createContactListResponse.StatusCode}");
                return string.Empty;
            }

            var contactListResponseContent = await createContactListResponse.Content.ReadAsStringAsync();
            var createdContactList = JsonSerializer.Deserialize<ContactListResponse>(contactListResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (createdContactList == null)
                throw new Exception("Failed to parse contact list creation response");

            _logger.LogDebug($"Lista de contactos creada: {createdContactList.name}");
            return createdContactList.id;
        }

        private async Task GetOrCreateAndStartCampaignAsync(SurveyRapihogar surveyRapihogar, string contactListId)
        {
            var responseGetCampaigns = await _axiaApiService.GetCampaignsAsync();

            if (responseGetCampaigns.IsSuccessStatusCode)
            {
                var campaignResponseContent = await responseGetCampaigns.Content.ReadAsStringAsync();
                var campaigns = JsonSerializer.Deserialize<List<CampaignResponse>>(campaignResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var existingCampaign = campaigns?.FirstOrDefault(c => c.Name == surveyRapihogar.Campaign && c.Status == "active");
                if (existingCampaign != null)
                {
                    _logger.LogDebug($"Campaña existente: {existingCampaign.Name}");
                    //var startCampaignResp = await _axiaApiService.StartCampaignAsync(existingCampaign.Id);

                    //if (!startCampaignResp.IsSuccessStatusCode)
                    //{
                    //    _logger.LogError($"Error al iniciar la campaña: {startCampaignResp.StatusCode}");
                    //}
                    return;
                }
            }

            var endpoint = await _dbContext.Endpoints.FirstOrDefaultAsync(c => c.SurveyId == surveyRapihogar.Campaign);
            if (endpoint == null || string.IsNullOrEmpty(endpoint.Sequence) || string.IsNullOrEmpty(endpoint.RedialingRules))
            {
                _logger.LogDebug("Sequence or Redialing Rule not found in the database");
                throw new Exception("Sequence or Redialing Rule not found in the database");
            }

            var createCampaignResponse = await _axiaApiService.CreateCampaignAsync(
                surveyRapihogar.Campaign,
                surveyRapihogar.Campaign,
                endpoint.Sequence,
                endpoint.RedialingRules,
                contactListId);

            if (string.IsNullOrEmpty(createCampaignResponse))
            {
                _logger.LogDebug("Failed to create campaign");
                throw new Exception("Failed to create campaign");
            }
              
            _logger.LogDebug($"Nueva campaña creada: {surveyRapihogar.Campaign}");

            var startCampaignResponse = await _axiaApiService.StartCampaignAsync(createCampaignResponse);

            if (!startCampaignResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"Error al iniciar la campaña: {startCampaignResponse.StatusCode}");
            }
        }
    }
}
