using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using System.Text.Json;
using System.Threading.Tasks;
using WebhookGail.Models;

namespace WebhookGail.Services
{
    public class OpenAIService
    {
        private readonly ChatClient _openAiClient;
        private readonly ILogger<OpenAIService> _logger;

        public OpenAIService(IConfiguration configuration, ILogger<OpenAIService> logger)
        {
            var openAIApiKey = configuration["OpenAI:ApiKey"];

            _openAiClient = new(model: "gpt-4o-mini", apiKey: openAIApiKey);
            _logger = logger;
        }

        public async Task<string> ProcessTranscriptionAsync(string transcriptionText, string instructions)
        {
            try
            {
                ChatCompletion completion = _openAiClient.CompleteChat(instructions + " Transcription text: " + transcriptionText);

                string structuredText = completion.Content[0].Text;

                return structuredText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando la transcripción con OpenAI.");
                throw;
            }
        }

        public async Task<string> TranslateToSpanishAsync(string text)
        {
            try
            {
                ChatCompletion completion = _openAiClient.CompleteChat("Translate to spanish: " + text);

                string translatedText = completion.Content[0].Text;

                return translatedText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando la transcripción con OpenAI.");
                throw;
            }
        }
    }
}
