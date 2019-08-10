using System.Threading.Tasks;

namespace VCAuthn.Services
{
    public interface IPresentationConfigurationService
    {
        Task<object> Find(string presentationConfigId);
    }

    public class PresentationConfigurationService : IPresentationConfigurationService
    {
        public async Task<object> Find(string presentationConfigId)
        {
            return 1;
            throw new System.NotImplementedException();
        }
    }
}