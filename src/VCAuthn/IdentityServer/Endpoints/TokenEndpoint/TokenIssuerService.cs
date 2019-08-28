using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;

namespace VCAuthn.IdentityServer.Endpoints
{
   
        /// <summary>
        /// A token issuer service.
        /// </summary>
        public interface ITokenIssuerService
        {
            /// <summary>
            /// Issues a JWT.
            /// </summary>
            /// <exception cref="System.ArgumentNullException">claims</exception>
            Task<string> IssueJwtAsync(int lifetime, string issuer, IEnumerable<Claim> claims);
        }
        
        
        public class TokenIssuerService : ITokenIssuerService
        {
            private readonly ITokenCreationService _tokenCreation;
            private readonly ISystemClock _clock;

            public TokenIssuerService(ITokenCreationService tokenCreation, ISystemClock clock)
            {
                _tokenCreation = tokenCreation;
                _clock = clock;
            }

            public async Task<string> IssueJwtAsync(int lifetime, string issuer, IEnumerable<Claim> claims)
            {
                if (claims == null) throw new ArgumentNullException(nameof(claims));

                var token = new Token
                {
                    CreationTime = _clock.UtcNow.UtcDateTime,
                    Issuer = issuer,
                    Lifetime = lifetime,
                    Claims = new HashSet<Claim>(claims, new ClaimComparer())
                };

                return await _tokenCreation.CreateTokenAsync(token);
            }
        }
    }
}