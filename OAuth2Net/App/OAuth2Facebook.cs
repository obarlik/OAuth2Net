using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace OAuth2Net
{
    public class OAuth2Facebook : OAuth2App
    {
        public OAuth2Facebook(
                string client_id,
                string client_secret,
                string redirect_uri,
                string scope = "public_profile,email") : base(
            "Facebook",
            "https://www.facebook.com/v3.3/dialog/oauth",
            "https://graph.facebook.com/v3.3/oauth/access_token",
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
                var json = cli.DownloadString($"https://graph.facebook.com/v3.3/me?access_token={state.AccessToken}&fields=id,name,email");
                var data = JObject.Parse(json);

                state.PersonId = data["id"]?.Value<string>();
                state.PersonName = data["name"]?.Value<string>();
                state.PersonEmail = data["email"]?.Value<string>();

                json = cli.DownloadString($"https://graph.facebook.com/{state.PersonId}/picture?type=large&redirect=false");
                data = JObject.Parse(json);

                state.PersonPhotoUrl = data["data"]?["url"]?.Value<string>();
            }
        }
    }
}
