using System;
using System.Net;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VCAuthn.IdentityServer;
using VCAuthn.IdentityServer.Endpoints;
using VCAuthn.IdentityServer.SessionStorage;
using VCAuthn.UrlShortener;

namespace VCAuthn.Controllers
{
    public class PresentationRequestController : ControllerBase
    {
        private readonly ISessionStorageService _sessionStorageService;
        private readonly IUrlShortenerService _urlShortenerService;
        private readonly ITokenIssuerService _tokenIssuerService;
        private readonly ILogger<WebHooksController> _logger;

        public PresentationRequestController(ISessionStorageService sessionStorageService, IUrlShortenerService urlShortenerService, ITokenIssuerService tokenIssuerService, ILogger<WebHooksController> logger)
        {
            _sessionStorageService = sessionStorageService;
            _urlShortenerService = urlShortenerService;
            _tokenIssuerService = tokenIssuerService;
            _logger = logger;
        }

        [HttpPost(IdentityConstants.VerificationChallengePollUri)]
        public async Task<ActionResult> Poll([FromQuery(Name = IdentityConstants.ChallengeIdQueryParameterName)] string presentationRequestId)
        {
            if (string.IsNullOrEmpty(presentationRequestId))
            {
                _logger.LogDebug($"Missing presentation request Id");
                return NotFound();
            }

            var authSession = await _sessionStorageService.FindByPresentationIdAsync(presentationRequestId);
            if (authSession == null)
            {
                _logger.LogDebug($"Cannot find a session corresponding to the presentation request. Presentation request Id: [{presentationRequestId}]");
                return NotFound();
            }

            if (authSession.PresentationRequestSatisfied == false)
            {
                return BadRequest();
            }

            if (authSession.ExpiredTimestamp >= DateTime.UtcNow)
            {
                _logger.LogDebug($"Session expired. Session id: [{authSession.Id}]");
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet(IdentityConstants.AuthorizeCallbackUri)]
        public async Task<ActionResult> AuthorizeCallback([FromQuery(Name = IdentityConstants.ChallengeIdQueryParameterName)] string sessionId)
        {
            if (!HttpMethods.IsGet(Request.Method))
            {
                _logger.LogDebug($"Invalid HTTP method for authorize endpoint. Method: [{Request.Method}]");
                return StatusCode((int)HttpStatusCode.MethodNotAllowed);
            }
            
            _logger.LogDebug("Start authorize callback request");

            if (string.IsNullOrEmpty(sessionId))
            {
                return StatusCode((int)HttpStatusCode.BadRequest, $"Empty {IdentityConstants.ChallengeIdQueryParameterName} param");
            }

            var session = await _sessionStorageService.FindBySessionIdAsync(sessionId);
            if (session == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "Cannot find corresponding session");
            }

            if (session.ResponseType == "code")
            {
                var url = $"{session.RedirectUrl}?code={session.Id}";
                _logger.LogDebug($"Code flow. Redirecting to {url}");
                
                return Redirect(url);
            }

            if (session.ResponseType == "token")
            {
                _logger.LogDebug("Token flow. Creating a token");
                var presentation = session.Presentation;
                var issuer = HttpContext.GetIdentityServerIssuerUri();
                var token = await _tokenIssuerService.IssueJwtAsync(10000, issuer, session.PresentationRecordId, presentation);

                if (_sessionStorageService.DeleteSession(session) == false)
                {
                    _logger.LogError("Failed to delete a session");
                }

                var url = $"{session.RedirectUrl}#access_token={token}&token_type=Bearer";
                _logger.LogDebug($"Token flow. Redirecting to {url}");

                return Redirect(url);
            }
            
            _logger.LogError("Unknown response type");
            return StatusCode((int)HttpStatusCode.BadRequest, $"Unknown response type: [{session.ResponseType}]");
        }

        [HttpGet("/url/{key}")]
        public async Task<ActionResult> ResolveUrl(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogDebug("Url key is null or empty");
                return BadRequest();
            }

            var url = await _urlShortenerService.GetUrlAsync(key);
            if (string.IsNullOrEmpty(url))
            {
                _logger.LogDebug($"Url is empty. Url key: [{key}]");
                return BadRequest();
            }
            
            return Redirect(url);
        }
    }
}