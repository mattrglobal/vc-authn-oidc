using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Hosting;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VCAuthn.Services;

namespace VCAuthn.IdentityServer.Endpoints
{
    public class AuthorizeEndpoint : IEndpointHandler
    {
        public const string Name = "VCAuthorize";
        public const string Path = "vc/connect/authorize";
        
        private readonly IClientSecretValidator _clientValidator;
        private readonly IPresentationConfigurationService _presentationConfigurationService;
        private readonly ILogger _logger;

        public AuthorizeEndpoint(
            IClientSecretValidator clientValidator,
            IPresentationConfigurationService presentationConfigurationService,
            ILogger<AuthorizeEndpoint> logger
            )
        {
            _clientValidator = clientValidator;
            _presentationConfigurationService = presentationConfigurationService;
            _logger = logger;
        }
        
        public async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            if (!HttpMethods.IsPost(context.Request.Method))
            {
                return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }
            
            if (!context.Request.HasFormContentType)
            {
                return new StatusCodeResult(HttpStatusCode.UnsupportedMediaType);
            }
                
            var values = context.Request.Form.AsNameValueCollection();
            
            var clientResult = await _clientValidator.ValidateAsync(context);
            if (clientResult.Client == null)
            {
                return Error(OidcConstants.TokenErrors.InvalidClient);
            }

            var scopes = values.Get(IdentityConstants.ScopeParamName).Split(' ');
            if (!scopes.Contains(IdentityConstants.VCAuthnScopeName))
            {
                return Error(IdentityConstants.MissingVCAuthnScopeError, IdentityConstants.MissingVCAuthnScopeDesc);
            }
            
            var presentationConfigId = values.Get(IdentityConstants.PresentationRequestConfigIDParamName);
            if (string.IsNullOrEmpty(presentationConfigId))
            {
                return Error(IdentityConstants.InvalidPresentationRequestConfigIDError, IdentityConstants.InvalidPresentationRequestConfigIDDesc);
            }
            
            var redirectUrl = values.Get(IdentityConstants.RedirectUriParameterName);
            if (string.IsNullOrEmpty(redirectUrl))
            {
                return Error(IdentityConstants.InvalidRedirectUriError);
            }
            
            if (clientResult.Client.RedirectUris.Any() && !clientResult.Client.RedirectUris.Contains(redirectUrl))
            {
                return Error(IdentityConstants.InvalidRedirectUriError);
            }
            
            var responseType = values.Get(IdentityConstants.ResponseTypeUriParameterName);
            if (string.IsNullOrEmpty(responseType))
            {
                responseType = IdentityConstants.DefaultResponseType;
            }

            var responseMode = values.Get(IdentityConstants.ResponseModeUriParameterName);
            if (string.IsNullOrEmpty(responseMode))
            {
                responseMode = IdentityConstants.DefaultResponseMode;
            }

//            var challenge = await _presentationConfigurationService.Find(presentationConfigId);
//            - calls ACA-Py asking to create VC presentation Request
//            - calculates `base64(<..>)` from the response.
//                Example of the message that would be encoded (Example Presentation Request From OP): https://github.com/mattrglobal/vc-authn-oidc/tree/master/docs#data-model
//            - builds a didcomm url with the base64 param: didcomms://?m=<..>&r_uri= (url format?)
//            - shortens url
//            - creates a QR code from the url
//            - creates a new session-id (uuid), persists `(session-id, presentation-request-id, expired-timestamp)` in psql.
//                `presentation-request-id` comes from `@id` field of the presentation request

//            - return http page with QR code
//                - set a `session-id` cookie
//                - page long polls

            return new AuthorizationEndpointResult(new AuthorizationRequest("CHALLENGE AWAITED"));
        }
        
        private AuthorizationFlowErrorResult Error(string error, string errorDescription = null)
        {
            var response = new AuthorizationErrorResponse
            {
                Error = error,
                ErrorDescription = errorDescription
            };

            return new AuthorizationFlowErrorResult(response);
        }

        /// <summary>
        /// Models a authorization flow error response
        /// </summary>
        public class AuthorizationErrorResponse
        {
            public string Error { get; set; } = OidcConstants.TokenErrors.InvalidRequest;
            
            public string ErrorDescription { get; set; }
        }

        internal class AuthorizationFlowErrorResult : IEndpointResult
        {
            public AuthorizationErrorResponse Response { get; }

            public AuthorizationFlowErrorResult(AuthorizationErrorResponse error)
            {
                Response = error;
            }

            public async Task ExecuteAsync(HttpContext context)
            {
                context.Response.StatusCode = 400;
                context.Response.SetNoCache();

                var dto = new ResultDto
                {
                    error = Response.Error,
                    error_description = Response.ErrorDescription
                };

                await context.Response.WriteJsonAsync(dto);
            }

            internal class ResultDto
            {
                public string error { get; set; }
                public string error_description { get; set; }
            }
        }
    }

    public class AuthorizationEndpointResult : IEndpointResult
    {
        private readonly AuthorizationRequest _authorizationRequest;

        public AuthorizationEndpointResult(AuthorizationRequest authorizationRequest)
        {
            _authorizationRequest = authorizationRequest;
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            await context.Response.WriteHtmlAsync("Doodsy do");
        }
    }

    public class AuthorizationRequest
    {
        public string Channlenge { get; }

        public AuthorizationRequest(string channlenge)
        {
            Channlenge = channlenge;
        }
    }
}