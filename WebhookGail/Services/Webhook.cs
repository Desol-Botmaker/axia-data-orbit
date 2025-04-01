using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using WebhookGail.Data;
using WebhookGail.Models;

namespace WebhookGail.Services
{
    public class WebhookService
    {
        private readonly WebhookGailContext _dbContext;
        private readonly OpenAIService _openAI;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WebhookService> _logger;
        private readonly Rapihogar _rapiInstance;

        public WebhookService(WebhookGailContext dbContext, OpenAIService openAI, IConfiguration configuration, ILogger<WebhookService> logger, Rapihogar rapiInstance)
        {
            _dbContext = dbContext;
            _openAI = openAI;
            _configuration = configuration;
            _logger = logger;
            _rapiInstance = rapiInstance;
        }


        public JsonElement ConvertStringToJsonElement(string jsonString)
        {
            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                return document.RootElement.Clone();
            }
        }

        public string RemoveNewLinesFromJsonString(string jsonString)
        {
            try
            {
                var jsonObject = JsonSerializer.Deserialize<object>(jsonString);
                return JsonSerializer.Serialize(jsonObject);
            }
            catch (JsonException ex)
            {
                _logger.LogDebug($"Error al deserializar el JSON: {ex.Message}");
                return null;
            }
        }

        public async Task<LogWebhookGail> ProcessPayloadAsync(Payload payloadObject, string businessName)
        {
            bool voicemail = payloadObject.call_information?.voicemail == "true";
            string structuredData = string.Empty;
            string translatedSummary = string.Empty;
            int tokensConsumed = 0;

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = false
            };

            int hourOffset = _configuration.GetValue<int>("TimeSettings:HourOffset");
            DateTime adjustedTime = DateTime.UtcNow.AddHours(hourOffset);
            _logger.LogDebug(JsonSerializer.Serialize(payloadObject));

            //string strQuestions = payloadObject.call_information?.questions;

            string strDataCollected = JsonSerializer.Serialize(payloadObject.call_information?.data_collected ?? new Dictionary<string, object>(), jsonSerializerOptions);

            try
            {
                Instructions? instructions = await _dbContext.Instructions.FirstOrDefaultAsync(i => i.Business == businessName);

                if ((instructions != null && payloadObject.call_information?.voicemail != "true") && payloadObject.duration > 0)
                {
                    if (instructions.Variable == "questions")
                    {
                        string questions = JsonSerializer.Serialize(payloadObject.call_information?.questions ?? new List<Question>(), jsonSerializerOptions);
                        //string questions = payloadObject.call_information?.questions;

                        if (questions != "[]" || !string.IsNullOrEmpty(questions)) { 
                            structuredData = await _openAI.ProcessTranscriptionAsync($"Fecha/Hora: {adjustedTime}.Transcripcion: {questions} Datos: {strDataCollected}. Duracion (s): {payloadObject.duration}", instructions.Instruction);
                        }
                        _logger.LogDebug($"Instruction: {instructions.Instruction} Data: {strDataCollected}");
                    }
                    else if (instructions.Variable == "summary")
                    {
                        string summary = JsonSerializer.Serialize(payloadObject.call_information.summary);
                        structuredData = await _openAI.ProcessTranscriptionAsync(summary, instructions.Instruction);
                    }
                    else if (instructions.Variable == "traduce")
                    {
                        string summary = JsonSerializer.Serialize(payloadObject.call_information.summary);
                        translatedSummary = await _openAI.ProcessTranscriptionAsync(summary, instructions.Instruction);
                    }

                    if (structuredData.StartsWith("`"))
                    {
                        int startIndex = structuredData.IndexOf('{');
                        int endIndex = structuredData.LastIndexOf('}');

                        string json = structuredData.Substring(startIndex, endIndex - startIndex + 1);

                        structuredData = JsonSerializer.Serialize(json.Trim(), jsonSerializerOptions);
                    }
                    tokensConsumed = (structuredData.Length + instructions.Instruction.Length) / 4;
                    _logger.LogDebug($"Structured Data: {structuredData.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar instrucciones con OpenAI");
            }

            var structuredDataSerilize = RemoveNewLinesFromJsonString(structuredData);

            var logEntry = new LogWebhookGail
            {
                IdGail = Guid.Parse(payloadObject.id),
                Sid = payloadObject.sid,
                Date = adjustedTime,
                ScriptId = payloadObject.call_information?.script_id ?? null,
                ScriptName = !string.IsNullOrEmpty(payloadObject.call_information.script_name) ? payloadObject.call_information.script_name : null,
                Direction = payloadObject.direction ?? null,
                Duration = payloadObject.duration,
                FromNumber = payloadObject.from_number,
                ToNumber = payloadObject.to_number,
                Status = payloadObject.status ?? null,
                CustomerName = payloadObject.call_information?.name ?? null,
                StatedPhoneNumber = payloadObject.call_information?.stated_phone_number ?? null,
                Interested = payloadObject.call_information?.interested ?? null,
                Questions = JsonSerializer.Serialize(payloadObject.call_information?.questions) ?? null,
                Note = payloadObject.call_information?.note ?? null,
                Email = payloadObject.call_information?.email ?? null,
                Summary = translatedSummary ?? payloadObject.call_information?.summary ?? null,
                DataCollected = (string.IsNullOrEmpty(structuredData) ? JsonSerializer.Serialize(payloadObject.call_information?.data_collected ?? new Dictionary<string, object>(), jsonSerializerOptions): structuredDataSerilize) ?? null,  
                CategoryOfCall = payloadObject.call_information?.category_of_call ?? null,
                ReasonOfCall = payloadObject.call_information?.reason_of_call ?? null,
                CallBackRequested = payloadObject.call_information?.call_back_requested ?? null,
                CallBackTime = payloadObject.call_information?.call_back_time ?? null,
                Resolution = payloadObject.call_information?.resolution ?? null,
                Information = payloadObject.call_information?.information ?? null,
                Business = payloadObject.call_information?.business ?? null,
                AppointmentRequested = payloadObject.call_information.appointment_requested ?? null,
                AppointmentTime = payloadObject.call_information.appointment_time == "n/a" ? null : payloadObject.call_information.appointment_time,
                ConsentToSendSms = payloadObject.call_information.consent_to_send_sms ?? null,
                VoiceMail = voicemail,
                CallerSatisfactionRating = payloadObject.call_information?.caller_satisfaction_rating ?? 0,
                CallDisconnectReason = payloadObject.call_information?.call_disconnect_reason ?? null,
                OrgSatisfactionRating = payloadObject.call_information?.org_satisfaction_rating ?? 0,
                Flagged = payloadObject.call_information?.flagged ?? false,
                Reviewed = payloadObject.call_information?.reviewed ?? false,
                ExtendedInformation = JsonSerializer.Serialize(payloadObject.call_information?.extended_information ?? new Dictionary<string, object>(), jsonSerializerOptions) ?? null,
                QuestionsText = string.Empty,
                DataCollectedText = payloadObject.call_information?.data_collected_text ?? null,
                CallInformationJson = payloadObject.call_information_json ?? null,
                RecordingUrl = payloadObject.recording_url ?? null,
                CallBackStartTime = payloadObject.callback_start_time ?? null,
                Notes = JsonSerializer.Serialize(payloadObject.notes ?? new List<Note>(), jsonSerializerOptions) ?? null,
                BusinessName = businessName,
                TokensConsumed = tokensConsumed
            };
            _logger.LogDebug($"LogEntry = {JsonSerializer.Serialize(logEntry)}");
            try
            {
                await _dbContext.LogWebhookGail.AddAsync(logEntry);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar los datos en la base de datos");
                return null;
            }

            //try
            //{
            //    Endpoints? endpoints = await _dbContext.Endpoints.FirstOrDefaultAsync(s => s.Business == businessName &&
            //                        EF.Functions.Like(payloadObject.call_information.script_name, "%" + s.SurveyId + "%"));

            //    if (endpoints != null)
            //    {
            //        JsonElement surveyResult = ConvertStringToJsonElement(structuredData);
            //        _logger.LogDebug($"SurveyResult: {JsonSerializer.Serialize(surveyResult)}");
            //        SurveyResponseResult surveyResponseResult = await _rapiInstance.SendSurveyResponseAsync(surveyResult, endpoints);

            //        if (surveyResponseResult != null)
            //        {
            //            _logger.LogDebug("Respuesta de encuesta enviada correctamente al endpoint: " + endpoints.Endpoint);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error al enviar la respuesta de la encuesta");
            //}

            return logEntry;
        }

    }
}
