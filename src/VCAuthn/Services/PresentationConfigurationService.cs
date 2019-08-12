using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VCAuthn.Models;

namespace VCAuthn.Services
{
    public interface IPresentationConfigurationService
    {
        Task<PresentationConfiguration> Find(string presentationConfigId);
    }

    public class PresentationConfigurationService : IPresentationConfigurationService
    {
        public async Task<PresentationConfiguration> Find(string presentationConfigId)
        {
            return new PresentationConfiguration
            {
                Id = "tmp",
                SubjectIdentifier = "attribute1",
                Configuration = JObject.Parse(
                    @"{
	""name"" : ""tmp config"",
	""version"" : ""0.0.1"",
	""connection_id"" : ""string"",
	""requested_atrributes"" : {
		""attribute_referent"" : {
			""name"" : ""attribute1"",
			""restrictions"" : {
				{
					""schema_id"": """",
					""schema_issuer_did"": """",
					""schema_name"": """",
					""schema_version"": """",
					""issuer_did"": """",
					""cred_def_id"": """",
				}
			}
		}
		""attribute_referent"" : {
			""name"" : ""attribute2"",
			""restrictions"" : {
				{
					""schema_id"": """",
					""schema_issuer_did"": """",
					""schema_name"": """",
					""schema_version"": """",
					""issuer_did"": """",
					""cred_def_id"": """",
				}
			}
		}
	}
}")
            };
        }
    }
}