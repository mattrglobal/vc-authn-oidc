using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VCAuthn.Models
{
    public class CreatePresentationResponse
    {
        [JsonProperty("presentation_request")]
        public JObject PresentationRequest { get; set; }
            
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
            
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
            
        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }
            
        [JsonProperty("presentation_exchange_id")]
        public string PresentationExchangeId { get; set; }
            
        [JsonProperty("connection_id")]
        public string ConnectionId { get; set; }
            
        [JsonProperty("initiator")]
        public string Initiator { get; set; }
            
        [JsonProperty("state")]
        public string State { get; set; }
    }

    public class WalletDidPublicResponse
    {
        public class ResponseResult
        {
            [JsonProperty("did")]
            public string DID { get; set; }
            
            [JsonProperty("verkey")]
            public string Verkey { get; set; }
            
            [JsonProperty("public")]
            public bool Public { get; set; }
        }
        
        public ResponseResult Result { get; set; }
    }
}