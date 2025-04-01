namespace WebhookGail.Models
{
    public class Endpoints
    {
        public int? Id { get; set; }
        public string? Business { get; set; }
        public string? SurveyId { get; set; }
        public string[]? Questions { get; set; }
        public string? Endpoint { get; set; }
        public string? Sequence { get; set; }
        public string? RedialingRules { get; set; }
    }
}
