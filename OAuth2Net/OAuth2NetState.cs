using System;
using System.Collections.Generic;
using System.Text;

namespace OAuth2Net
{
    public class OAuth2NetState
    {
        public string StateId { get; set; }

        public string ProviderName { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string AuthorizationParams { get; set; }

        public string RedirectUri { get; set; }

        public string ReturnUrl { get; set; }

        public string OpenIdDiscoveryUrl { get; set; }

        public string AuthorizationUrl { get; set; }

        public string AccessTokenUrl { get; set; }

        public string UserInfoEndpoint { get; set; }

        public string Scope { get; set; }

        public string AccessTokenType { get; set; }

        public string AccessToken { get; set; }

        public string AuthorizationCode { get; set; }

        public string PersonId { get; set; }

        public string PersonName { get; set; }

        public string PersonPhotoUrl { get; set; }

        public string PersonEmail { get; set; }

        public string PersonProfileUrl { get; set; }

        public string PersonLocation { get; set; }

        public string PersonInfo { get; set; }

        public string PersonLocale { get; set; }

        public string Result { get; set; }

        public string Error { get; set; }

        public string ErrorDescription { get; set; }

        public string ErrorUri { get; set; }

        public OAuth2NetState Clone()
            => new OAuth2NetState
            {
                AccessToken = AccessToken,
                AccessTokenType = AccessTokenType,
                AccessTokenUrl = AccessTokenUrl,
                AuthorizationCode = AuthorizationCode,
                AuthorizationParams = AuthorizationParams,
                AuthorizationUrl = AuthorizationUrl,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                Error = Error,
                ErrorDescription = ErrorDescription,
                ErrorUri = ErrorUri,
                OpenIdDiscoveryUrl = OpenIdDiscoveryUrl,
                PersonEmail = PersonEmail,
                PersonId = PersonId,
                PersonInfo = PersonInfo,
                PersonLocale = PersonLocale,
                PersonLocation = PersonLocation,
                PersonName = PersonName,
                PersonPhotoUrl = PersonPhotoUrl,
                PersonProfileUrl = PersonProfileUrl,
                ProviderName = ProviderName,
                RedirectUri = RedirectUri,
                Result = Result,
                ReturnUrl = ReturnUrl,
                Scope = Scope,
                StateId = StateId,
                UserInfoEndpoint = UserInfoEndpoint
            };
    }
}
