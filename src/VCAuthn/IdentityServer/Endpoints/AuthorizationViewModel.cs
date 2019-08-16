using Newtonsoft.Json;

namespace VCAuthn.IdentityServer.Endpoints
{
    public class AuthorizationViewModel
    {
        [JsonProperty("resolution_url")]
        public string ResolutionUrl { get; set; }

        [JsonProperty("poll_url")]
        public string PollUrl { get; set; }

        [JsonProperty("challenge")]
        public string Challenge { get; set; }

        [JsonProperty("interval")]
        public int Interval { get; set; }
    }
}