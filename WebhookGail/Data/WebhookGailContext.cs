using Microsoft.EntityFrameworkCore;
using WebhookGail.Models;

namespace WebhookGail.Data
{
    public class WebhookGailContext : DbContext
    {
        public WebhookGailContext(DbContextOptions<WebhookGailContext> options) : base(options)
        {
        }
        public DbSet<Instructions> Instructions { get; set; }
        public DbSet<LogWebhookGail> LogWebhookGail { get; set; }
        public DbSet<LogWebhookGail> LogWebhookGailView { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Endpoints> Endpoints { get; set; }
        public DbSet<DataTest> DataTest { get; set; }
        public DbSet<YPFDistribuidoresOficiales> YPFDistribuidoresOficiales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogWebhookGail>().ToTable("LOG_WEBHOOK_AXIA");
            modelBuilder.Entity<LogWebhookGail>().ToView("LIST_CALLS_AXIA");
            modelBuilder.Entity<Customer>().ToTable("CUSTOMERS_AXIA");
            modelBuilder.Entity<Instructions>().ToTable("OPENAI_INSTRUCTIONS_AXIA");
            modelBuilder.Entity<Endpoints>().ToTable("ENDPOINTS_AXIA");
            modelBuilder.Entity<DataTest>().ToTable("DATA_TEST").HasNoKey();
            modelBuilder.Entity<YPFDistribuidoresOficiales>().ToTable("YPF_DISTRIBUIDORES_OFICIALES");
        }
    }

}
