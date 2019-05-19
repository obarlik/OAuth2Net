using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;

namespace OAuth2Net
{
    public class OAuth2App
    {
        string ClientId;
        string AuthorizationUrl;
        string AccessTokenUrl;
        string RedirectUri;
        string RedirectUri2;
        string Scope;

        public string State;

        public string AuthorizationCode;
        public string AccessToken;
        public string AccessTokenType;

        public string Error;
        public string ErrorDescription;
        public string ErrorUri;

        Action<OAuth2App> Success;
        Action<OAuth2App> Failure;

        protected Dictionary<string, string> AuthorizationParams = new Dictionary<string, string>();

        static Dictionary<string, OAuth2App> Authentications = new Dictionary<string, OAuth2App>();


        public OAuth2App(
            string authorizationUrl,
            string accessTokenUrl,
            string client_id,
            string redirect_uri = null,
            string scope = null,
            Action<OAuth2App> success = null, 
            Action<OAuth2App> failure = null)
        {
            AuthorizationUrl = authorizationUrl;
            AccessTokenUrl = accessTokenUrl;
            ClientId = client_id;
            RedirectUri = redirect_uri;
            Scope = scope;
            State = Guid.NewGuid().ToString("N");

            Success = success;
            Failure = failure;
        }


        public string GetAuthorizationUrl(string returnUrl = null)
        {
            var cbUri = new UriBuilder(RedirectUri);

            cbUri.Query +=
                string.IsNullOrWhiteSpace(returnUrl) ? "" :
                    ((string.IsNullOrWhiteSpace(cbUri.Query) ? "" : "&")
                   + $"ReturnUrl={Uri.EscapeDataString(returnUrl)}");

            RedirectUri2 = cbUri.Uri.AbsoluteUri;

            var authorizationUri = new UriBuilder(AuthorizationUrl);

            authorizationUri.Query +=
                  (string.IsNullOrWhiteSpace(authorizationUri.Query) ? "" : "&")
                + $"response_type=code"
                + $"&client_id={ClientId}"
                + (!AuthorizationParams.Any() ? "" : $"&{string.Join("&", AuthorizationParams.Select(p => p.Key + "=" + Uri.EscapeDataString(p.Value)))}")
                + (Scope == null ? "" : $"&scope={Uri.EscapeDataString(Scope)}")
                + $"&state={State}"
                + (RedirectUri == null ? "" : $"&redirect_uri={Uri.EscapeDataString(RedirectUri2)}");

            AddAuthentication(this);

            return authorizationUri.Uri.AbsoluteUri;
        }


        static void AddAuthentication(OAuth2App api)
        {
            if (!Authentications.ContainsKey(api.State))
                lock (Authentications)
                {
                    if (!Authentications.ContainsKey(api.State))
                        Authentications[api.State] = api;
                }
        }


        static OAuth2App FindApi(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                return null;

            if (Authentications.ContainsKey(state))
                lock (Authentications)
                {
                    if (Authentications.TryGetValue(state, out OAuth2App api))
                    {
                        Authentications.Remove(api.State);
                        return api;
                    }
                }

            return null;
        }


        public static void Callback(
            string code,
            string error,
            string state = null,
            string error_description = null,
            string error_uri = null)
        {
            FindApi(state)?._Callback(code, error, error_description, error_uri);
        }


        void _Callback(
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
                (!AuthorizationParams.Any() ? "" : $"&{string.Join("&", AuthorizationParams.Select(p => p.Key + "=" + Uri.EscapeDataString(p.Value)))}") +
                $"&grant_type=authorization_code" +
                $"&redirect_uri={Uri.EscapeDataString(RedirectUri2)}" +
                $"&code={AuthorizationCode}" +
                $"&state={State}");

            AccessToken = Regex.Matches(json, "\"access_token\" *: *\"(.+?)\"")
                .OfType<Match>()
                .Select(m => m.Groups[1].Value)
                .FirstOrDefault();

            AccessTokenType = Regex.Matches(json, "\"token_type\" *: *\"(.+?)\"")
                            .OfType<Match>()
                            .Select(m => m.Groups[1].Value)
                            .FirstOrDefault() ?? "bearer";

            if (AccessToken == null)
            {
                Error = "TokenError";
                ErrorDescription = "Token retrieval failure!";
                Failure?.Invoke(this);
                return;
            }

            Success?.Invoke(this);
        }
    }
}
