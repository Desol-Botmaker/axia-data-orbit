using System.Numerics;

namespace WebhookGail.Models
{
    public class DataTest
    {
        public DateOnly PaymentDate { get; set; }
        public int Payment { get; set; }
        public string? Detail { get; set; }
        public long CUIT { get; set; }
        public int NroBoca { get; set; }
    }
}
