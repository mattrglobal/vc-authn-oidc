using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Hosting;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VCAuthn.ACAPy;
using VCAuthn.PresentationConfiguration;
using VCAuthn.UrlShortener;
using VCAuthn.Utils;

namespace VCAuthn.IdentityServer.Endpoints
{
    public class AuthorizeEndpoint : IEndpointHandler
    {
        public const string Name = "VCAuthorize";
        public const string Path = "vc/connect/authorize";
        
        private readonly IClientSecretValidator _clientValidator;
        private readonly IPresentationConfigurationService _presentationConfigurationService;
        private readonly IACAPYClient _acapyClient;
        private readonly IUrlShortenerService _urlShortenerService;
        private readonly IdentityServerOptions _options;
        private readonly ILogger _logger;

        public AuthorizeEndpoint(
            IClientSecretValidator clientValidator,
            IPresentationConfigurationService presentationConfigurationService,
            IACAPYClient acapyClient,
            IUrlShortenerService urlShortenerService,
            IOptions<IdentityServerOptions> options,
            ILogger<AuthorizeEndpoint> logger
            )
        {
            _clientValidator = clientValidator;
            _presentationConfigurationService = presentationConfigurationService;
            _acapyClient = acapyClient;
            _urlShortenerService = urlShortenerService;
            _options = options.Value;
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
            
            var presentationRecordId = values.Get(IdentityConstants.PresentationRequestConfigIDParamName);
            if (string.IsNullOrEmpty(presentationRecordId))
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

            var presentationRecord = await _presentationConfigurationService.Find(presentationRecordId);

            var presentationRequest = BuildPresentationRequest(presentationRecord);
            
            var url = string.Format("{0}?m={1}&r_uri={2}", _options.PublicOrigin , presentationRequest.ToJson().ToBase64(), redirectUrl);
            
            var shortUrl = await _urlShortenerService.CreateShortUrlAsync(url);
            
//            - creates a new session-id (uuid), persists `(session-id, presentation-request-id, expired-timestamp)` in psql.
//                `presentation-request-id` comes from `@id` field of the presentation request

//            - return http page with QR code
//                - set a `session-id` cookie
//                - page long polls

            return new AuthorizationEndpointResult(new AuthorizationRequest("CHALLENGE AWAITED"));
        }

        private string BuildUrl(string baseUrl, PresentationRequest presentationRequest)
        {
            
        }

        private PresentationRequest BuildPresentationRequest(PresentationRecord record)
        {
            record.Configuration.Nonce = $"0{Guid.NewGuid().ToString("N")}";

            var request = new PresentationRequest
            {
                Id = Guid.NewGuid().ToString(),
                Request = record.Configuration,
                ThreadId = Guid.NewGuid().ToString(),
            };
            return request;
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