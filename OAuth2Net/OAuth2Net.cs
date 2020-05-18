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
        public static IOAuth2NetStateProvider StateProvider { get; set; }
            = new OAuth2NetStaticStateProvider();

        protected Dictionary<string, string> AuthorizationParams = new Dictionary<string, string>();

        public OAuth2NetState State;

        public OAuth2App(
            string providerName,
            string authorizationUrl,
            string accessTokenUrl,
            string client_id,
            string client_secret,
            string redirect_uri,
            string scope = null)
        {
            Initialize(
                providerName,
                authorizationUrl,
                accessTokenUrl,
                client_id,
                client_secret,
                redirect_uri,
                scope);
        }


        void Initialize(
            string providerName,
            string authorizationUrl,
            string accessTokenUrl,
            string client_id,
            string client_secret,
            string redirect_uri,
            string scope = null,
            string userInfoEndpoint = null,
            string openIdDiscoveryUrl = null)
        {
            State = new OAuth2NetState
            {
                ProviderName = providerName,
                AuthorizationUrl = authorizationUrl,
                AccessTokenUrl = accessTokenUrl,
                ClientId = client_id,
                ClientSecret = client_secret,
                RedirectUri = redirect_uri,
                Scope = scope,
                AuthorizationParams = AuthorizationParams.Any()
                    ? string.Join("&", AuthorizationParams.Select(p => p.Key + "=" + Uri.EscapeDataString(p.Value)))
                    : null,
                UserInfoEndpoint = userInfoEndpoint,
                OpenIdDiscoveryUrl = openIdDiscoveryUrl
            };
        }


        public OAuth2App(
            string providerName,
            string openIdDiscoveryUrl,
            string client_id,
            string client_secret,
            string redirect_uri,
            string scope = null) 
        {   
            using (var cli = NewClient("application/json"))
            {
                var json = cli.DownloadString(openIdDiscoveryUrl);
                var data = JObject.Parse(json);

                Initialize(
                    providerName,
                    data["authorization_endpoint"]?.Value<string>(),
                    data["token_endpoint"]?.Value<string>(),
                    client_id,
                    client_secret,
                    redirect_uri,
                    scope,
                    data["userinfo_endpoint"]?.Value<string>(),
                    openIdDiscoveryUrl);
            }        
        }


        public string GetAuthorizationUrl(string returnUrl = null)
        {
            var state = State.Clone();

            state.StateId = state.ProviderName + "_" + Guid.NewGuid().ToString("N");                
            state.ReturnUrl = returnUrl;

            var authorizationUri = new UriBuilder(state.AuthorizationUrl);

            authorizationUri.Query +=
                  (string.IsNullOrWhiteSpace(authorizationUri.Query) ? "" : "&") +
                  $"response_type=code" +
                  $"&client_id={state.ClientId}" +
                  //(ClientSecret != null ? $"&client_secret={ClientSecret}" : "") +
                  (state.AuthorizationParams != null ? $"&{state.AuthorizationParams}" : "") +
                  (state.Scope == null ? "" : $"&scope={Uri.EscapeDataString(state.Scope)}") +
                  $"&state={state.StateId}" +
                  (state.RedirectUri == null ? "" : $"&redirect_uri={Uri.EscapeDataString(state.RedirectUri)}");

            StateProvider[state.StateId] = state;

            return authorizationUri.Uri.AbsoluteUri;
        }


        public static void Callback(
            string code,
            string error,
            string state = null,
            string error_description = null,
            string error_uri = null,
            Action<OAuth2NetState> success = null,
            Action<OAuth2NetState> failure = null)
        {
            var stateValues = StateProvider[state];

            if (!string.IsNullOrWhiteSpace(error))
            {
                stateValues.Result = "failure";
                stateValues.Error = error;
                stateValues.ErrorDescription = error_description;
                stateValues.ErrorUri = error_uri;

                failure?.Invoke(stateValues);
                return;
            }

            stateValues.AuthorizationCode = code;

            var cli = new WebClient();

            cli.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            cli.Headers["User-Agent"] = "OAuth2Api";
            cli.Headers["Accept"] = "application/json";

            var json = cli.UploadString(
                stateValues.AccessTokenUrl,
                $"client_id={stateValues.ClientId}" +
                (stateValues.ClientSecret != null ? $"&client_secret={stateValues.ClientSecret}" : "") +
                (stateValues.AuthorizationParams != null ? $"&{stateValues.AuthorizationParams}" : "") +
                $"&grant_type=authorization_code" +
                $"&redirect_uri={Uri.EscapeDataString(stateValues.RedirectUri)}" +
                $"&code={stateValues.AuthorizationCode}" +
                $"&state={state}");

            var token =
                Regex.Matches(json, "\"access_token\" *: *\"(.+?)\"")
                .OfType<Match>()
                .Select(m => m.Groups[1].Value)
                .FirstOrDefault();

            var tokenType = 
                Regex.Matches(json, "\"token_type\" *: *\"(.+?)\"")
                .OfType<Match>()
                .Select(m => m.Groups[1].Value)
                .FirstOrDefault() ?? "Bearer";

            stateValues.AccessTokenType = (tokenType?.Length ?? 0) > 0 ?
                new string(
                    tokenType
                    .Select((c, i) => i == 0 ? char.ToUpperInvariant(c) : c)
                    .ToArray()) :
                tokenType;

            if (token == null)
            {
                error = "TokenError";
                error_description = "Token retrieval failure!";

                stateValues.Result = "failure";
                stateValues.Error = error;
                stateValues.ErrorDescription = error_description;
                
                failure?.Invoke(stateValues);
                return;
            }

            stateValues.AccessToken = token;

            success?.Invoke(stateValues);
        }


        public static WebClient NewClient(string accept = null, string agent = null)
        {
            var cli = new WebClient();

            if (accept != null)
                cli.Headers["Accept"] = accept;

            cli.Headers["User-Agent"] = agent ?? "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134";
            cli.Encoding = Encoding.UTF8;

            return cli;
        }


        public static WebClient NewAuthorizedClient(string accessTokenType, string accessToken, string accept = null, string agent = null)
        {
            var cli = NewClient(accept, agent);
            cli.Headers["Authorization"] = $"{accessTokenType} {accessToken}";
            return cli;
        }
    }
}
