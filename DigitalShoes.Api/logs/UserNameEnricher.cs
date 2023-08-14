using Serilog.Core;
using Serilog.Events;
namespace DigitalShoes.Api.logs
{

    public class UserNameEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserNameEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {            
            var userName = GetUserNameFromHttpContext();
            var userNameProperty = propertyFactory.CreateProperty("user_name", userName);
            logEvent.AddPropertyIfAbsent(userNameProperty);
        }

        private string GetUserNameFromHttpContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated ?? false)
            {
                return httpContext.User.Identity.Name;
            }
            return "N/A"; // Default value if not authenticated
        }
    }

}
