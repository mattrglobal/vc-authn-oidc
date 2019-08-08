using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Hosting;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;

namespace VCAuthn.IdentityServer.Endpoints
{
    public class AuthorizeEndpoint : IEndpointHandler
    {
        public const string Name = "VCAuthorize";
        public const string Path = "vc/connect/authorize";
        
        private readonly IClientSecretValidator _clientValidator;

        public AuthorizeEndpoint(
            IClientSecretValidator clientValidator
            )
        {
            _clientValidator = clientValidator;
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

            var scopes = values.GetValues(IdentityConstants.ScopeParamName);
            if (!scopes.Contains(IdentityConstants.VCAuthnScopeName))
            {
                return Error(IdentityConstants.MissingVCAuthnScopeError, IdentityConstants.MissingVCAuthnScopeDesc);
            }
            
            var challengeConfigId = values.Get(IdentityConstants.PresentationRequestConfigIDParamName);
            if (string.IsNullOrEmpty(challengeConfigId))
            {
                return Error(IdentityConstants.InvalidPresentationRequestConfigIDError, IdentityConstants.InvalidPresentationRequestConfigIDDesc);
            }
            
            var redirectUrl = values.Get(IdentityConstants.RedirectUriParameterName);
            if (string.IsNullOrEmpty(redirectUrl))
            {
                return Error(IdentityConstants.InvalidRedirectUriError);
            }
            
            if (!clientResult.Client.RedirectUris.Contains(redirectUrl))
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
            
//            return new AuthorizationEndpointResult(new ValidatedAuthorizationRequest
//            {
//                ChallengeId = challengeRecord.Id
//            });
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
}