using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebhookGail.Models
{
    public class LogWebhookGail
    {
        [Key]
        public int Id { get; set; }
        public Guid IdGail { get; set; }
        public string? Sid { get; set; }
        public DateTime? Date { get; set; }
        public string? ScriptId { get; set; }
        public string? ScriptName { get; set; }
        public string? Direction { get; set; }
        public int Duration { get; set; }
        public string? FromNumber { get; set; }
        public string? ToNumber { get; set; }
        public string? Status { get; set; }
        public string? CustomerName { get; set; }
        public string? StatedPhoneNumber { get; set; }
        public string? Interested { get; set; }
        public string? Questions { get; set; }
        public string? Note { get; set; }
        public string? Email { get; set; }
        public string? Summary { get; set; }
        public string? DataCollected { get; set; }
        public string? CategoryOfCall { get; set; }
        public string? ReasonOfCall { get; set; }
        public string? CallBackRequested { get; set; }
        public string? CallBackTime { get; set; }
        public string? Resolution { get; set; }
        public string? Information { get; set; }
        public string? Business { get; set; }
        public string? AppointmentRequested { get; set; }
        public string? AppointmentTime { get; set; } 
        public string? ConsentToSendSms { get; set; }
        public bool VoiceMail { get; set; }
        public int CallerSatisfactionRating { get; set; }
        public string? CallDisconnectReason { get; set; }
        public int OrgSatisfactionRating { get; set; }
        public bool Flagged { get; set; }
        public bool Reviewed { get; set; }
        public string? ExtendedInformation { get; set; }
        public string? QuestionsText { get; set; }
        public string? DataCollectedText { get; set; }
        public string? CallInformationJson { get; set; }
        public string? RecordingUrl { get; set; }
        public string? CallBackStartTime { get; set; }
        public string? Notes { get; set; }
        public string? BusinessName { get; set; }
        public int? TokensConsumed { get; set; }
    }

    public class LogWebhookGailView : LogWebhookGail
    {
        public LogWebhookGailView() { }
    }
}
