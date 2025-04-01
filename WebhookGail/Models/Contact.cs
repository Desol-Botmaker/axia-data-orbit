namespace WebhookGail.Models
{
    public class Contact
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public List<string>? emails { get; set; }
        public List<PhoneNumber>? phoneNumbers { get; set; }
        public string? businessName { get; set; }
        public Dictionary<string, string>? additionalData { get; set; }
    }

    public class ContactResponse
    {
        public string? id { get; set; }
        public string? status { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public List<string>? emails { get; set; }
        public List<PhoneNumber>? phoneNumbers { get; set; }
        public string? businessName { get; set; }
        public string? source { get; set; }
        public Dictionary<string, string>? additionalData { get; set; }
    }

    public class PhoneNumber
    {
        public string? number { get; set; }
        public string? type { get; set; }
    }
}
