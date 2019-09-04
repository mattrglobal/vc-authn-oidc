using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace VCAuthn.IdentityServer
{
    public class QueryStringSecretParser : ISecretParser
    {
        private readonly IdentityServerOptions _options;
        private readonly ILogger<QueryStringSecretParser> _logger;

        public QueryStringSecretParser(IdentityServerOptions options, ILogger<QueryStringSecretParser> logger)
        {
            _options = options;
            _logger = logger;
        }
        
        public string AuthenticationMethod => "client_secret_query";

        public async Task<ParsedSecret> ParseAsync(HttpContext context)
        {
            _logger.LogDebug("Start parsing for secret in request query");

            var query = context.Request.Query;

            if (query == null)
            {
                _logger.LogDebug("No secret in query found");
                return null;
            }

            var id = query["client_id"].FirstOrDefault();
            var secret = query["client_secret"].FirstOrDefault();

            // client id must be present
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogDebug("No client in query found");
                return null;
            }
            
            if (id.Length > _options.InputLengthRestrictions.ClientId)
            {
                _logger.LogError("Client ID exceeds maximum length.");
                return null;
            }

            if (string.IsNullOrEmpty(secret))
            {
                // client secret is optional
                _logger.LogDebug("client id without secret found");

                return new ParsedSecret
                {
                    Id = id,
                    Type = IdentityServerConstants.ParsedSecretTypes.NoSecret
                };
            }
            
            if (secret.Length > _options.InputLengthRestrictions.ClientSecret)
            {
                _logger.LogError("Client secret exceeds maximum length.");
                return null;
            }

            return new ParsedSecret
            {
                Id = id,
                Credential = secret,
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };
            
        }
    }
}