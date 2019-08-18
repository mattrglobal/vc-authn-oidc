using Newtonsoft.Json;

namespace VCAuthn.IdentityServer.Endpoints
{
    public class PresentationRequest
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("@type")]
        public string Type => "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/credential-presentation/0.1/presentation-request";

        [JsonProperty("request")]
        public PresentationConfiguration.PresentationConfiguration Request { get; set; }
        
        [JsonProperty("comment")]
        public string Comment { get; set; }
        
        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }
    }
}