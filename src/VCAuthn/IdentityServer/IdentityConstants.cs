namespace VCAuthn.IdentityServer
{
    public class IdentityConstants
    {
        public const string ScopeParamName = "scope";
        public const string VCAuthnScopeName = "vc_authn";
        public const string MissingVCAuthnScopeError = "missing_vc_authn_scope";
        public static string MissingVCAuthnScopeDesc = $"Missing {VCAuthnScopeName} scope"; 
        
        public const string PresentationRequestConfigIDParamName = "pres_req_conf_id";
        public const string InvalidPresentationRequestConfigIDError = "invalid_pres_req_conf_id";
        public static string InvalidPresentationRequestConfigIDDesc = $"Missing {PresentationRequestConfigIDParamName} param";
        
        public const string RedirectUriParameterName = "redirect_uri";
        public const string InvalidRedirectUriError = "invalid_redirect_uri";
        
        public const string ResponseTypeUriParameterName = "response_type";
        public const string DefaultResponseType = "form_post";
        
        public const string ResponseModeUriParameterName = "response_mode";
        public const string DefaultResponseMode = "verification_code";

    }
}