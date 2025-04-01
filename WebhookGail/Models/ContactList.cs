namespace WebhookGail.Models
{
    public class ContactList
    {
        public string? name { get; set; }
        public string? description { get; set; }
    }               
    public class ContactListResponse
    {           
        public string? id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
    }

    public class GetContactListsResponse
    {
        public List<ContactListResponse> ContactLists { get; set; }

        public GetContactListsResponse()
        {
            ContactLists = new List<ContactListResponse>();
        }
    }


    public class ContactListAddRequest
    {
        public List<string>? contactIds { get; set; }
    }
}
