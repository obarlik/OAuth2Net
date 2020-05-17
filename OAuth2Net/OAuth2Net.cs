using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace OAuth2Net
{
    public abstract class OAuth2App
    {
        string ClientId;
        string ClientSecret;
        string AuthorizationUrl;
        string AccessTokenUrl;
        string RedirectUri;
        string Scope;

        public readonly string ProviderName;

        public string PersonId { get; protected set; }
        public string PersonName { get; protected set; }
        public string PersonEmail { get; protected set; }
        public string PersonPhotoUrl { get; protected set; }
        public string PersonProfileUrl { get; protected set; }
        public string PersonLocation { get; protected set; }
        public string PersonInfo { get; protected set; }
        public string PersonLocale { get; protected set; }


        public string ReturnUrl { get; protected set; }

        public string State;

        public string AuthorizationCode;

        public string AccessToken { get; protected set; }
        public string AccessTokenType { get; protected set; }

        public string Error { get; protected set; }
        public string ErrorDescription { get; protected set; }
        public string ErrorUri { get; protected set; }

        public string UserInfoEndpoint { get; protected set; }

        public static IOAuth2NetStateProvider StateProvider { get; set;  } = new OAuth2NetStaticStateProvider();

        Action<OAuth2App> Success;
        Action<OAuth2App> Failure;

        protected Dictionary<string, string> AuthorizationParams = new Dictionary<string, string>();

        public OAuth2App(
            string providerName,
            string authorizationUrl,
            string accessTokenUrl,
            string client_id,
            string client_secret,
            string redirect_uri,
            string scope = null,
            Action<OAuth2App> success = null,
            Action<OAuth2App> failure = null)
        {
            ProviderName = providerName;
            Initialize(
                authorizationUrl,
                accessTokenUrl,
                client_id,
                client_secret,
                redirect_uri,
                scope,
                success,
                failure);
        }


        void Initialize(
            string authorizationUrl,
            string accessTokenUrl,
            string client_id,
            string client_secret,
            string redirect_uri,
            string scope = null,
            Action<OAuth2App> success = null,
            Action<OAuth2App> failure = null)
        {
            AuthorizationUrl = authorizationUrl;
            AccessTokenUrl = accessTokenUrl;
            ClientId = client_id;
            ClientSecret = client_secret;
            RedirectUri = redirect_uri;
            Scope = scope;
            State = Guid.NewGuid().ToString("N");
            AccessTokenType = "Bearer";

            Success = success;
            Failure = failure;
        }


        public OAuth2App(
            string providerName,
            string openIdDiscoveryUrl,
            string client_id,
            string client_secret,
            string redirect_uri,
            string scope = null,
            Action<OAuth2App> success = null,
            Action<OAuth2App> failure = null) 
        {
            ProviderName = providerName;

            var cli = NewClient("application/json");

            var json = cli.DownloadString(openIdDiscoveryUrl);
            var data = JObject.Parse(json);

            UserInfoEndpoint = data["userinfo_endpoint"]?.Value<string>();

            Initialize(
                data["authorization_endpoint"]?.Value<string>(),
                data["token_endpoint"]?.Value<string>(),
                client_id,
                client_secret,
                redirect_uri,
                scope,
                success,
                failure);
        }


        public string GetAuthorizationUrl(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            var authorizationUri = new UriBuilder(AuthorizationUrl);

            authorizationUri.Query +=
                  (string.IsNullOrWhiteSpace(authorizationUri.Query) ? "" : "&") +
                  $"response_type=code" +
                  $"&client_id={ClientId}" +
                  //(ClientSecret != null ? $"&client_secret={ClientSecret}" : "") +
                  (!AuthorizationParams.Any() ? "" : $"&{string.Join("&", AuthorizationParams.Select(p => p.Key + "=" + Uri.EscapeDataString(p.Value)))}") +
                  (Scope == null ? "" : $"&scope={Uri.EscapeDataString(Scope)}") +
                  $"&state={State}" +
                  (RedirectUri == null ? "" : $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}");

            AddAuthentication(this);

            return authorizationUri.Uri.AbsoluteUri;
        }


        void AddAuthentication(OAuth2App api)
            => StateProvider.SetState(api.State, api);
        


        static OAuth2App FindApi(string state)
            => (OAuth2App)StateProvider.RemoveState(state);
        

        public static void Callback(
            string code,
            string error,
            string state = null,
            string error_description = null,
            string error_uri = null)
        {
            FindApi(state)?.CallbackInternal(code, error, error_description, error_uri);
        }


        void CallbackInternal(
            string code,
            string error,
            string error_description = null,
            string error_uri = null)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                Error = error;
                ErrorDescription = error_description;
                ErrorUri = error_uri;
                Failure?.Invoke(this);
                return;
            }

            AuthorizationCode = code;

            var cli = new WebClient();

            cli.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            cli.Headers["User-Agent"] = "OAuth2Api";
            cli.Headers["Accept"] = "application/json";

            var json = cli.UploadString(
                AccessTokenUrl,
                $"client_id={ClientId}" +
                (ClientSecret != null ? $"&client_secret={ClientSecret}" : "") +
                (!AuthorizationParams.Any() ? "" : $"&{string.Join("&", AuthorizationParams.Select(p => p.Key + "=" + Uri.EscapeDataString(p.Value)))}") +
                $"&grant_type=authorization_code" +
                $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
                $"&code={AuthorizationCode}" +
                $"&state={State}");

            AccessToken = 
                Regex.Matches(json, "\"access_token\" *: *\"(.+?)\"")
                .OfType<Match>()
                .Select(m => m.Groups[1].Value)
                .FirstOrDefault();

            var tokenType = 
                Regex.Matches(json, "\"token_type\" *: *\"(.+?)\"")
                .OfType<Match>()
                .Select(m => m.Groups[1].Value)
                .FirstOrDefault() ?? "Bearer";

            AccessTokenType = (tokenType?.Length ?? 0) > 0 ?
                new string(
                    AccessTokenType
                    .Select((c, i) => i == 0 ? char.ToUpperInvariant(c) : c)
                    .ToArray()) :
                tokenType;

            if (AccessToken == null)
            {
                Error = "TokenError";
                ErrorDescription = "Token retrieval failure!";
                Failure?.Invoke(this);
                return;
            }

            Success?.Invoke(this);
        }


        public WebClient NewClient(string accept = null, string agent = null)
        {
            var cli = new WebClient();

            if (accept != null)
                cli.Headers["Accept"] = accept;

            cli.Headers["User-Agent"] = agent ?? "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134";
            cli.Encoding = Encoding.UTF8;

            return cli;
        }


        public WebClient NewAuthorizedClient(string accept = null, string agent = null)
        {
            var cli = NewClient(accept, agent);
            cli.Headers["Authorization"] = $"{AccessTokenType} {AccessToken}";
            return cli;
        }
    }
}
