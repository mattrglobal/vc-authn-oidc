using System.Net;
using System.Threading.Tasks;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace VCAuthn.IdentityServer.Endpoints
{
    public class AuthorizeCallbackEndpoint : IEndpointHandler
    {
        private readonly ILogger<AuthorizeCallbackEndpoint> _logger;

        public AuthorizeCallbackEndpoint(ILogger<AuthorizeCallbackEndpoint> logger)
        {
            _logger = logger;
        }
        
        public async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            if (!HttpMethods.IsGet(context.Request.Method))
            {
                _logger.LogDebug($"Invalid HTTP method for authorize endpoint. Method: [{context.Request.Method}]");
                return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }
            
            _logger.LogDebug("Start authorize callback request");
            
            
        }
    }
}