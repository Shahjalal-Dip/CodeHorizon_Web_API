using Hangfire.Dashboard;

namespace CodeHorizon.API.Security
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // For development, allow local access only
            var httpContext = context.GetHttpContext();

            // Allow access only from localhost
            return httpContext.Connection.LocalIpAddress == null ||
                   httpContext.Connection.RemoteIpAddress == null ||
                   httpContext.Connection.LocalIpAddress.Equals(httpContext.Connection.RemoteIpAddress);
        }
    }
}