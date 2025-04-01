using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using WebhookGail.Data;

namespace WebhookGail.Middleware
{
    public class TokenValidationMiddleware : ActionFilterAttribute
    {
        private readonly string? _validToken;
        private readonly WebhookGailContext? _dbContext;
        private readonly ILogger<TokenValidationMiddleware> _logger;

        public TokenValidationMiddleware(IConfiguration configuration, WebhookGailContext dbcontext, ILogger<TokenValidationMiddleware> logger)
        {
            _validToken = configuration["AppSettings:ApiToken"];
            _dbContext = dbcontext;
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var validToken = _dbContext.Customer.FirstOrDefaultAsync(c => c.AccessToken == token).Result;

            if (string.IsNullOrEmpty(token) || token != validToken.AccessToken)
            {
                context.Result = new UnauthorizedResult();
            }

            base.OnActionExecuting(context);
        }
    }
}
