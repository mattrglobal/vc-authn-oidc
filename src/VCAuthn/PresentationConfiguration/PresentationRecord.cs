using System.Collections.Generic;
using Newtonsoft.Json;

namespace VCAuthn.PresentationConfiguration
{
    public class PresentationRequest
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("@type")]
        public string Type => "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/credential-presentation/0.1/presentation-request";

        [JsonProperty("request")]
        public PresentationConfiguration Request { get; set; }
        
        [JsonProperty("comment")]
        public string Comment { get; set; }
        
        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }
    }
    
    
    public class PresentationRecord
    {
        [JsonProperty("id")] 
        public string Id { get; set; }
        
        [JsonProperty("subject_identifier")] 
        public string SubjectIdentifier { get; set; }
        
        [JsonProperty("configuration")] 
        public PresentationConfiguration Configuration { get; set; }
    }
    
    public class PresentationConfiguration
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("requested_attributes")]
        public Dictionary<string, PresentationAttributeInfo> RequestedAttributes { get; set; }

        [JsonProperty("requested_predicates", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, PresentationPredicateInfo> RequestedPredicates { get; set; } =
            new Dictionary<string, PresentationPredicateInfo>();

        [JsonProperty("non_revoked", NullValueHandling = NullValueHandling.Ignore)]
        public RevocationInterval NonRevoked { get; set; }
        
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Name={Name}, " +
            $"Version={Version}, " +
            $"Nonce={Nonce}, " +
            $"RequestedAttributes={string.Join(",", RequestedAttributes ?? new Dictionary<string, PresentationAttributeInfo>())}, " +
            $"RequestedPredicates={string.Join(",", RequestedPredicates ?? new Dictionary<string, PresentationPredicateInfo>())}, " +
            $"NonRevoked={NonRevoked}";
    }

    public class PresentationAttributeInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the restrictions.
        /// <code>
        /// filter_json: filter for credentials
        ///    {
        ///        "schema_id": string, (Optional)
        ///        "schema_issuer_did": string, (Optional)
        ///        "schema_name": string, (Optional)
        ///        "schema_version": string, (Optional)
        ///        "issuer_did": string, (Optional)
        ///        "cred_def_id": string, (Optional)
        ///    }
        /// </code>
        /// </summary>
        /// <value>The restrictions.</value>
        [JsonProperty("restrictions", NullValueHandling = NullValueHandling.Ignore)]
        public List<AttributeFilter> Restrictions { get; set; }

        /// <summary>
        /// Gets or sets the non revoked interval.
        /// </summary>
        [JsonProperty("non_revoked", NullValueHandling = NullValueHandling.Ignore)]
        public RevocationInterval NonRevoked { get; set; }
        
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Name={Name}, " +
            $"Restrictions={string.Join(",", Restrictions ?? new List<AttributeFilter>())}, " +
            $"NonRevoked={NonRevoked}";
    }
    
    /// <inheritdoc />
    public class PresentationPredicateInfo : PresentationAttributeInfo
    {
        [JsonProperty("p_type")]
        public string PredicateType { get; set; }

        [JsonProperty("p_value")]
        public string PredicateValue { get; set; }
        
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"PredicateType={PredicateType}, " +
            $"PredicateValue={PredicateValue}";
    }
    
    public class AttributeFilter
    {
        [JsonProperty("schema_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SchemaId { get; set; }

        [JsonProperty("schema_issuer_did", NullValueHandling = NullValueHandling.Ignore)]
        public string SchemaIssuerDid { get; set; }

        [JsonProperty("schema_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SchemaName { get; set; }

        [JsonProperty("schema_version", NullValueHandling = NullValueHandling.Ignore)]
        public string SchemaVersion { get; set; }

        [JsonProperty("issuer_did", NullValueHandling = NullValueHandling.Ignore)]
        public string IssuerDid { get; set; }

        [JsonProperty("cred_def_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CredentialDefinitionId { get; set; }
    }
    
    public class RevocationInterval
    {
        [JsonProperty("from")]
        public uint From { get; set; }

        [JsonProperty("to")]
        public uint To { get; set; }
        
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"From={From}, " +
            $"To={To}";
    }
}