using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace OAuth2Net
{
    public class OAuth2Google : OAuth2App
    {
        public OAuth2Google(
                string client_id,
                string client_secret,
                string redirect_uri,
                string scope = "openid profile email") 
            : base(
            "Google",
            openIdDiscoveryUrl: "https://accounts.google.com/.well-known/openid-configuration",
            client_id,
            client_secret,
            redirect_uri,
            scope)
        {
        }


        public static void SuccessCallback(OAuth2NetState state)
        {
            using (var cli = NewAuthorizedClient(state.AccessTokenType, state.AccessToken,
                                                 accept: "application/json"))
            {
                var json = cli.DownloadString(state.UserInfoEndpoint);
                var data = JObject.Parse(json);

                state.PersonId = data["sub"]?.Value<string>();
                state.PersonName = data["name"]?.Value<string>();
                state.PersonEmail = data["email"]?.Value<string>();
                state.PersonPhotoUrl = data["picture"]?.Value<string>();
                state.PersonProfileUrl = data["profile"]?.Value<string>();
                state.PersonLocale = data["locale"]?.Value<string>();
            }
        }
    }
}
