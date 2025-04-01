using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using WebhookGail.Data;
using WebhookGail.Models;

namespace WebhookGail.Services
{
    public class YPF
    {
        private readonly WebhookGailContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WebhookService> _logger;

        public YPF(WebhookGailContext dbContext, IConfiguration configuration, ILogger<WebhookService> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IActionResult> GetDatosAsync(string CUIT, string nroBoca)
        {
            return null;
        }

    }
}
