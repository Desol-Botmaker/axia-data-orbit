namespace WebhookGail.Models
{
    public class Instructions
    {
        public int Id { get; set; }
        public string? Business { get; set; }
        public string? Name { get; set; }
        public string? Instruction { get; set; }
        public string? ScriptId { get; set; }
        public string? Variable { get; set; }
    }
}
