using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VCAuthn.Models
{
    public class PresentationConfiguration
    {
        [JsonProperty("id")] 
        public string Id { get; set; }
        
        [JsonProperty("subject_identifier")] 
        public string SubjectIdentifier { get; set; }
        
        [JsonProperty("configuration")] 
        public JObject Configuration { get; set; }
        
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}