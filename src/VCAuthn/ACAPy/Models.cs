using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VCAuthn.ACAPy
{
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