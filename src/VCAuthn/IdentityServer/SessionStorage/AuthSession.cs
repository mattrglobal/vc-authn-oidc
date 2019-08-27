using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VCAuthn.Controllers;

namespace VCAuthn.IdentityServer.SessionStorage
{
    public class AuthSession
    {
        public string Id { get; set; }
        public string PresentationRequestId { get; set; }
        public DateTime ExpiredTimestamp { get; set; }
        public bool PresentationRequestSatisfied { get; set; }

        private string _presentation;


        public PartialPresentation Presentation
        {
            get => _presentation == null ? null : JsonConvert.DeserializeObject<PartialPresentation>(_presentation);
            set => _presentation = JsonConvert.SerializeObject(value);
        }
    }
    
    public class PartialPresentation
    {
        [JsonProperty("requested_proof")]
        public RequestedProof RequestedProof { get; set; }
    }

    public class RequestedProof
    {
        [JsonProperty("revealed_attrs")]
        public Dictionary<string, ProofAttribute> RevealedAttributes { get; set; }
            
        /// <summary>
        /// ignore structural mapping of other properties
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> Rest { get; set; }
    }

    public class ProofAttribute
    {
        [JsonProperty("raw")]
        public string Raw { get; set; }
            
        /// <summary>
        /// ignore structural mapping of other properties
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> Rest { get; set; }
    }
}