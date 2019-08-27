using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Hosting;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using VCAuthn.IdentityServer.SessionStorage;

namespace VCAuthn.IdentityServer.Endpoints
{
    public class TokenEndpoint: IEndpointHandler
    {
        private readonly IClientSecretValidator _clientValidator;
        private readonly ISessionStorageService _sessionStore;
        public const string Name = "VCToken";

        public TokenEndpoint(IClientSecretValidator clientValidator, ISessionStorageService sessionStore)
        {
            _clientValidator = clientValidator;
            _sessionStore = sessionStore;
        }

        public async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            NameValueCollection values;

            if (HttpMethods.IsPost(context.Request.Method))
            {
                if (!context.Request.HasFormContentType)
                {
                    return new StatusCodeResult(HttpStatusCode.UnsupportedMediaType);
                }
                values = context.Request.Form.AsNameValueCollection();
            }
            else
            {
                return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }
            
            var clientResult = await _clientValidator.ValidateAsync(context);
            if (clientResult.Client == null)
            {
                return VCResponseHelpers.Error(OidcConstants.TokenErrors.InvalidClient);
            }
            
            string grantType = values.Get(IdentityConstants.GrantTypeParameterName);

            if (string.IsNullOrEmpty(grantType))
                return VCResponseHelpers.Error(IdentityConstants.InvalidGrantTypeError);

            if (grantType != IdentityConstants.VerificationCodeGrantType)
            {
                return VCResponseHelpers.Error(IdentityConstants.InvalidGrantTypeError);
            }

            string sessionId;
                if (context.Request.Cookies.TryGetValue(IdentityConstants.SessionIdCookieName, out sessionId) == false)
                {
                    return VCResponseHelpers.Error(IdentityConstants.MissingSessionCookieError, $"Missing ${IdentityConstants.SessionIdCookieName} cookie");
                }

                var session = await _sessionStore.FindBySessionIdAsync(sessionId);
                if (session == null)
                {
                    return VCResponseHelpers.Error(IdentityConstants.InvalidSessionError, $"Cannot find stored session");
                }

                if (session.PresentationRequestSatisfied == false)
                {
                    return VCResponseHelpers.Error(IdentityConstants.InvalidSessionError, "Presentation request wasn't satisfied");
                }

                return new TokenResult(session);
        }

        public class TokenResult : IEndpointResult
        {
            public TokenResult(AuthSession session)
            {
                throw new System.NotImplementedException();
            }

            public Task ExecuteAsync(HttpContext context)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}