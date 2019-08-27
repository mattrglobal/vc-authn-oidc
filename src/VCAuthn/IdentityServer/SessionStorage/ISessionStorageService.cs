using System.Threading.Tasks;

namespace VCAuthn.IdentityServer.SessionStorage
{
    public interface ISessionStorageService
    {
        Task<string> CreateSessionAsync(string presentationRequestId);
        Task<bool> AddSession(AuthSession session);
        Task<bool> SatisfyPresentationRequestIdAsync(string presentationRequestId);
        Task<AuthSession> FindByPresentationIdAsync(string presentationRequestId);
        Task<AuthSession> FindBySessionIdAsync(string sessionId);
    }
}