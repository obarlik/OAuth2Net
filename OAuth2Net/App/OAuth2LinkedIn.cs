using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Text;

namespace OAuth2Net
{
    public class OAuth2LinkedIn : OAuth2App
    {
        public OAuth2LinkedIn(
            string client_id,
            string client_secret,
            string redirect_uri,
            string scope = "r_liteprofile r_emailaddress")
            : base(
                "LinkedIn",
                "https://www.linkedin.com/oauth/v2/authorization",
                "https://www.linkedin.com/oauth/v2/accessToken",
                client_id,
                client_secret,
                redirect_uri,
                scope)
        {
        }


        public static void SuccessCallback(OAuth2NetState state)
        {
            using (var cli = NewAuthorizedClient(state.AccessTokenType, state.AccessToken))
            {
                var json = cli.DownloadString(
                    "https://api.linkedin.com/v2/me" +
                    "?projection=(id,firstName,lastName,profilePicture(displayImage~:playableStreams))");

                var profileJson = JObject.Parse(json);

                state.PersonId = profileJson["id"]?.Value<string>();

                state.PersonName =
                    profileJson["firstName"]?["localized"]?.First?.First?.Value<string>()
                    + " " + profileJson["lastName"]?["localized"]?.First?.First?.Value<string>();

                state.PersonPhotoUrl =
                   profileJson["profilePicture"]?["displayImage~"]?["elements"]?
                   .Select(el =>
                       new
                       {
                           width = el["data"]?["com.linkedin.digitalmedia.mediaartifact.StillImage"]?["storageSize"]?["width"]?.Value<int>(),
                           url = el["identifiers"]?.First?["identifier"]?.Value<string>()
                       })
                       .OrderByDescending(el => el.width)
                       .Select(el => el.url)
                       .FirstOrDefault();
            
                json = cli.DownloadString(
                    "https://api.linkedin.com/v2/emailAddress" +
                    "?q=members&projection=(elements*(handle~))");

                var emailJson = JObject.Parse(json);

                state.PersonEmail =
                   emailJson["elements"].First?["handle~"]?["emailAddress"]?.Value<string>();
            }
        }

    }
}
