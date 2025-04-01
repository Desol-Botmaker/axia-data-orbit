using System.Net.Http;
using System.Text;

namespace WebhookGail.Services
{
    public class SendWhatsappTemplate
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SendWhatsappTemplate> _logger;
        private readonly HttpClient _httpClient;


        public SendWhatsappTemplate(IConfiguration configuration, ILogger<SendWhatsappTemplate> logger, HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }


        public async Task PostTemplateAsync(string contactId, string template, string phoneNumberId, string displayPhoneNumber, string campaignId, int agentId, int customerId)
        {
            try
            {
                string url = _configuration["AppSettings.SendTemplateUri"] + $"/sendtemplate?contactId={contactId}&template={template}&phoneNumberId={phoneNumberId}&displayPhoneNumber={displayPhoneNumber}&campaignId={campaignId}&agentId={agentId}&customerId={customerId}";

                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Mensaje de agradecimiento enviado a {ContactId}", contactId);
                }
                else
                {
                    _logger.LogWarning("No se pudo enviar mensaje de agradecimiento a {ContactId}. Status Code: {StatusCode}", contactId, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar la solicitud a {ContactId}", contactId);
            }
        }
    }
}
