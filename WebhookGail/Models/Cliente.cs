namespace WebhookGail.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Last_Name { get; set; }
        public string? Document_Number { get; set; }
        public string? Email { get; set; }
        public string? Whatsapp_Phone { get; set; }
        public string? Organization { get; set; }
        public int Organization_Id { get; set; }
        public List<Poliza>? Poliza { get; set; }
        public List<Pedido>? Pedidos { get; set; }
        public Pedido? Last_Pedido { get; set; }
        public List<object>? Visit_Pending { get; set; }
        public VisitPendingNext? Visit_Pending_Next { get; set; }
        public string? Cbu { get; set; }
        public string? Cuit_Cuil { get; set; }
    }

    public class Poliza
    {
        public int Id { get; set; }
        public string? Policy_Number { get; set; }
        public string? Alias { get; set; }
        public string? Ramo { get; set; }
        public string? Address { get; set; }
        public string? Pet_Name { get; set; }
        public string? Balance_Due_Date { get; set; }
        public List<Coverage>? Coverages { get; set; }
        public Product? Product { get; set; }
    }

    public class Coverage
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string? Type { get; set; }
        public string? Title { get; set; }
        public int? Cant { get; set; }
        public double Annual_Balance { get; set; }
        public double Current_Used_Balance { get; set; }
        public double Benefit_Available { get; set; }
        public string? Legend_2 { get; set; }
        public string? Limit { get; set; }
        public string? Limit_For_Event { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int Ext_Cod_Ramo { get; set; }
        public int Ramo { get; set; }
    }

    public class Pedido
    {
        public int Id { get; set; }
        public string? Created_Time { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public StatusRequest? Status_Request { get; set; }
        public string? Fecha_Finalizacion { get; set; }
        public string? Rubro { get; set; }
        public Poliza? Poliza { get; set; }
        public Scheme? Scheme { get; set; }
        public Organization? Organization { get; set; }
    }

    public class StatusRequest
    {
        public int Id { get; set; }
        public string? Description { get; set; }
    }

    public class Scheme
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
    }

    public class Organization
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Logo { get; set; }
        public string? Iso_Logo { get; set; }
        public string? Principal_Color { get; set; }
        public string? Letter_Color { get; set; }
        public string? Background_Color { get; set; }
    }

    public class VisitPendingNext
    {
        public string? Tipo { get; set; }
        public string? Status { get; set; }
        public bool Confirm_Client { get; set; }
        public string? Prestador_Name { get; set; }
    }

}
