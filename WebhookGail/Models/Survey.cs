using System.Text.Json.Serialization;

namespace WebhookGail.Models
{
    public class SurveyResponseResult
    {
        [JsonPropertyName("survey_id")]
        public int SurveyId { get; set; }

        [JsonPropertyName("updated")]
        public DateTime Updated { get; set; }

        [JsonPropertyName("answers")]
        public List<AnswerResult>? Answers { get; set; }
    }

    public class SurveyRapihogar
    {
        [JsonPropertyName("id_pedido")]
        public int IdPedido { get; set; }

        [JsonPropertyName("aseguradora")]
        public string? Aseguradora { get; set; }

        [JsonPropertyName("nombre_prestador")]
        public string? NombrePrestador { get; set; }

        [JsonPropertyName("nombre_asegurado")]
        public string? NombreAsegurado { get; set; }

        [JsonPropertyName("rubro")]
        public string? Rubro { get; set; }

        [JsonPropertyName("fecha_del_servicio")]
        public string? FechaSevicio { get; set; }

        [JsonPropertyName("esquema")]
        public string? Esquema { get; set; }

        [JsonPropertyName("phone_number")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("campaigns")]
        public string? Campaign { get; set; }
    }

    public class AnswerResult
    {
        [JsonPropertyName("question")]
        public int Question { get; set; }

        [JsonPropertyName("question_text")]
        public string? QuestionText { get; set; }

        [JsonPropertyName("body")]
        public string? Body { get; set; }
    }
}
