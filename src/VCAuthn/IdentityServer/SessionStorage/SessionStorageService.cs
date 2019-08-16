using System;
using System.Threading.Tasks;
using VCAuthn.UrlShortener;

namespace VCAuthn.IdentityServer.SessionStorage
{
    public class SessionStorageServiceOptions
    {
        public int SessionLifetimeInSeconds { get; set; }
    }

    public class SessionStorageService : ISessionStorageService
    {
        private readonly SessionStorageDbContext _context;
        private readonly SessionStorageServiceOptions _options;

        public SessionStorageService(SessionStorageDbContext context, SessionStorageServiceOptions options)
        {
            _context = context;
            _options = options;
        }

        public async Task<string> CreateSessionAsync(string presentationRequestId)
        {
            var session = new AuthSession
            {
                Id = Guid.NewGuid().ToString(),
                PresentationRequestId = presentationRequestId,
                ExpiredTimestamp = DateTime.Now.AddSeconds(_options.SessionLifetimeInSeconds)
            };
            
            if (await AddSession(session))
                return session.Id;

            return null;
        }

        public async Task<bool> AddSession(AuthSession session)
        {
            _context.Add(session);
            return await _context.SaveChangesAsync() == 1;
        }
    }
}