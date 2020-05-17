using System;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace OAuth2Net
{
    public class OAuth2GitHub : OAuth2App
    {
        public OAuth2GitHub(
            string client_id,
            string client_secret,
            string redirect_uri)
            : base(
                "GitHub",
                "https://github.com/login/oauth/authorize",
                "https://github.com/login/oauth/access_token",
                client_id,
                client_secret,
                redirect_uri,
                null)
        {
        }


        public static void SuccessCallback(OAuth2NetState state)
        {
            using (var cli = NewAuthorizedClient(state.AccessTokenType, state.AccessToken,
                                                 accept: "application/vnd.github.machine-man-preview+json"))
            {
                var jsonText = cli.DownloadString("https://api.github.com/user");
                var json = JObject.Parse(jsonText);

                state.PersonId = json["id"]?.Value<string>();
                state.PersonName = json["name"]?.Value<string>() ?? json["login"]?.Value<string>();
                state.PersonEmail = json["email"]?.Value<string>();
                state.PersonPhotoUrl = json["avatar_url"]?.Value<string>();
                state.PersonProfileUrl = json["html_url"]?.Value<string>();
                state.PersonLocation = json["location"]?.Value<string>();
                state.PersonInfo = json["bio"]?.Value<string>();

                if (string.IsNullOrWhiteSpace(state.PersonEmail))
                    try
                    {
                        var text = cli.DownloadString("https://api.github.com/user/emails");
                        var mails = JArray.Parse(text);

                        state.PersonEmail =
                            mails.Where(m => m["primary"]?.Value<bool>() ?? false)
                            .Select(m => m["email"]?.Value<string>())
                            .FirstOrDefault();

                    }
                    catch
                    {
                        // tried
                    }
            }
        }
    }
}
