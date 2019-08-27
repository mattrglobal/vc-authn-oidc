using System.Threading.Tasks;
using VCAuthn.Controllers;

namespace VCAuthn.IdentityServer.SessionStorage
{
    public interface ISessionStorageService
    {
        Task<string> CreateSessionAsync(string presentationRequestId);
        Task<bool> AddSession(AuthSession session);
        Task<bool> SatisfyPresentationRequestIdAsync(string presentationRequestId, PartialPresentation partialPresentation);
        Task<AuthSession> FindByPresentationIdAsync(string presentationRequestId);
        Task<AuthSession> FindBySessionIdAsync(string sessionId);
    }
}