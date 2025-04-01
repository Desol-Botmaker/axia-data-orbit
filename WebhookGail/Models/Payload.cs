namespace WebhookGail.Models
{
    public class Question
    {
        public string? speaker { get; set; }
        public string? question { get; set; }
        public string? answer { get; set; }
    }

    public class CallInformation
    {
        public string? name { get; set; }
        public string? stated_phone_number { get; set; }
        public string? interested { get; set; }
        public List<Question>? questions { get; set; }
        //public string? questions { get; set; }
        public string? note { get; set; }
        public string? email { get; set; }
        public string? summary { get; set; }
        public Dictionary<string, object>? data_collected { get; set; }
        //public string? data_collected { get; set; }
        public string? category_of_call { get; set; }
        public string? reason_of_call { get; set; }
        public string? call_back_requested { get; set; }
        public string? call_back_time { get; set; }
        public string? resolution { get; set; }
        public string? information { get; set; }
        public string? business { get; set; }
        public string? appointment_requested { get; set; } 
        public string? appointment_time { get; set; } 
        public string? consent_to_send_sms { get; set; } 
        public string? voicemail { get; set; }
        public int caller_satisfaction_rating { get; set; }
        public string? call_disconnect_reason { get; set; }
        public int org_satisfaction_rating { get; set; }
        public bool flagged { get; set; }
        public bool reviewed { get; set; }
        public string? script_id { get; set; }
        public string? script_name { get; set; }
        public Dictionary<string, object>? extended_information { get; set; }
        public string? questions_text { get; set; }
        public string? data_collected_text { get; set; }
    }

    public class Note
    {
        public string? id { get; set; }
        public string? writer { get; set; }
        public string? time { get; set; }
        public string? note { get; set; }
    }

    public class Payload
    {
        public string? id { get; set; }
        public string? sid { get; set; }
        public string? date { get; set; }
        public string? from_number { get; set; }
        public string? to_number { get; set; }
        public CallInformation? call_information { get; set; }
        public string? recording_url { get; set; }
        public int duration { get; set; }
        public string? direction { get; set; }
        public string? status { get; set; }
        public string? callback_start_time { get; set; }
        public List<Note>? notes { get; set; }
        public string? call_information_json { get; set; }
    }
}


