namespace WebhookGail.Models
{
    public class AxiaCampaign { 
        public class CampaignRequest
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public List<Sequence>? Sequences { get; set; }
            public List<Guid>? RedialingRules { get; set; }
            public List<ContactList>? ContactLists { get; set; }
        }

        public class Sequence
        {
            public Guid? SequenceId { get; set; }
            public int? Rank { get; set; }
        }

        public class ContactList
        {
            public Guid? Id { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
        }
    }

    public class CampaignResponse
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public List<AxiaCampaign.Sequence>? Sequences { get; set; }
        public List<Guid>? RedialingRules { get; set; }
        public List<AxiaCampaign.ContactList>? ContactLists { get; set; }
        public string? CreatedAt { get; set; }
    }
}
