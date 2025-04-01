using System.Text.Json;
using System.Text;
using WebhookGail.Data;
using WebhookGail.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace WebhookGail.Services
{
    public class AxiaApiService
    {
        private readonly ILogger<AxiaApiService> _logger;
        private readonly HttpClient _httpClient;
        private readonly WebhookGailContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string? _apiKey;
        private readonly string? _endpoint;

        public AxiaApiService(ILogger<AxiaApiService> logger, HttpClient httpClient, WebhookGailContext dbContext, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _dbContext = dbContext;
            _configuration = configuration;
            _apiKey = configuration["GailAPI:ApiKey"];
            _endpoint = configuration["GailAPI:Endpoint"];
        }

        private void AddCommonHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("X-Api-Key", "api-ddd3b3db50ba4c598ac7428cc3e02104-w4og5paWpdgRUZkwbMAKuupZ8-RH_tRpgSGCa_N3uZ4");
        }

        // Campaign
        public async Task<HttpResponseMessage> GetCampaignsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _endpoint + "/v1/campaigns");
            AddCommonHeaders(request);
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> GetCampaignAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _endpoint + $"/v1/campaigns/{id}");
            AddCommonHeaders(request);
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> PostCampaignAsync(AxiaCampaign axiaCampaign)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _endpoint + "/v1/campaigns");
            AddCommonHeaders(request);
            string jsonContent = JsonSerializer.Serialize(axiaCampaign);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> UpdateCampaignAsync(AxiaCampaign axiaCampaign, string id)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, _endpoint + $"/v1/campaigns/{id}");
            AddCommonHeaders(request);
            string jsonContent = JsonSerializer.Serialize(axiaCampaign);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> StartCampaignAsync(string id)
        {
            return await ExecutePostRequest($"{_endpoint}/v1/campaigns/{id}/start");
        }

        public async Task<HttpResponseMessage> StopCampaignAsync(string id)
        {
            return await ExecutePostRequest($"{_endpoint}/v1/campaigns/{id}/stop");
        }

        public async Task<HttpResponseMessage> ArchiveCampaignAsync(string id)
        {
            return await ExecutePostRequest($"{_endpoint}/v1/campaigns/{id}/archive");
        }

        public async Task<HttpResponseMessage> RestoreCampaignAsync(string id)
        {
            return await ExecutePostRequest($"{_endpoint}/v1/campaigns/{id}/restore");
        }

        // Contact
        public async Task<HttpResponseMessage> GetContactsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _endpoint + "/v1/contacts");
            AddCommonHeaders(request);
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> GetContactAsync(string id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _endpoint + $"/v1/contacts/{id}");
            AddCommonHeaders(request);
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> PostContactAsync(Contact contact)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _endpoint + "/v1/contacts");
            AddCommonHeaders(request);
            string jsonContent = JsonSerializer.Serialize(contact);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> UpdateContactAsync(string id, Contact contact)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, _endpoint + $"/v1/contacts/{id}");
            AddCommonHeaders(request);
            string jsonContent = JsonSerializer.Serialize(contact);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> BulkAddContactsAsync(List<Contact> contacts)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _endpoint + "/v1/contacts/bulk_add");
            AddCommonHeaders(request);
            string jsonContent = JsonSerializer.Serialize(contacts);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> ArchiveContactAsync(string id)
        {
            return await ExecutePostRequest($"{_endpoint}/v1/contacts/{id}/archive");
        }

        // Contact List
        public async Task<HttpResponseMessage> GetContactListsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _endpoint + "/v1/contact_lists");
            AddCommonHeaders(request);
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> GetContactListAsync(string id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _endpoint + $"/v1/contact_lists/{id}");
            AddCommonHeaders(request);
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> CreateContactListAsync(ContactList contactList)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _endpoint + "/v1/contact_lists");
            AddCommonHeaders(request);
            string jsonContent = JsonSerializer.Serialize(contactList);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> UpdateContactListAsync(string id, ContactList contactList)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, _endpoint + $"/v1/contact_lists/{id}");
            AddCommonHeaders(request);
            string jsonContent = JsonSerializer.Serialize(contactList);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> AddContactToListAsync(string contactListId, List<string> contactIds)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_endpoint}/v1/contact_lists/{contactListId}/add");
            AddCommonHeaders(request);

            var requestBody = new
            {
                contactIds = contactIds
            };

            string jsonContent = JsonSerializer.Serialize(requestBody);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> RemoveContactFromListAsync(string contactListId, List<string> contactIds)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_endpoint}/v1/contact_lists/{contactListId}/remove");
            AddCommonHeaders(request);

            var requestBody = new
            {
                contactIds = contactIds
            };

            string jsonContent = JsonSerializer.Serialize(requestBody);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> ArchiveContactListAsync(string id)
        {
            return await ExecutePostRequest($"{_endpoint}/v1/contact_lists/{id}/archive");
        }

        public async Task<HttpResponseMessage> RestoreContactListAsync(string id)
        {
            return await ExecutePostRequest($"{_endpoint}/v1/contact_lists/{id}/restore");
        }

        public async Task<HttpResponseMessage> AddSingleContactToListAsync(string contactListId, string contactId)
        {
            return await AddContactToListAsync(contactListId, new List<string> { contactId });
        }

        public async Task<HttpResponseMessage> RemoveSingleContactFromListAsync(string contactListId, string contactId)
        {
            return await RemoveContactFromListAsync(contactListId, new List<string> { contactId });
        }

        public async Task<string> CreateCampaignAsync(string name, string description, string sequenceId, string redialingRuleId, string contactListId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_endpoint}/v1/campaigns");
            AddCommonHeaders(request);

            var campaign = new AxiaCampaign.CampaignRequest
            {
                Name = name,
                Description = description,
                Sequences = new List<AxiaCampaign.Sequence>
                {
                    new AxiaCampaign.Sequence { SequenceId = Guid.Parse(sequenceId), Rank = 0 }
                },
                RedialingRules = new List<Guid> { Guid.Parse(redialingRuleId) },
                ContactLists = new List<AxiaCampaign.ContactList>
                {
                    new AxiaCampaign.ContactList { Id = Guid.Parse(contactListId), Name = name, Description = description }
                }
            };

            string jsonContent = JsonSerializer.Serialize(campaign);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var campaignResponse = JsonSerializer.Deserialize<CampaignResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return campaignResponse?.Id ?? throw new Exception("Failed to parse campaign ID");
            }
            else
            {
                throw new Exception($"Failed to create campaign: {response.ReasonPhrase}");
            }
        }

        private async Task<HttpResponseMessage> ExecutePostRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            AddCommonHeaders(request);
            return await _httpClient.SendAsync(request);
        }
    }
}
