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
            Action<OAuth2LinkedIn> success,
            Action<OAuth2LinkedIn> failure,
            string scope = "r_liteprofile r_emailaddress")
            : base(
                "https://www.linkedin.com/oauth/v2/authorization",
                "https://www.linkedin.com/oauth/v2/accessToken",
                client_id,
                client_secret,
                redirect_uri,
                scope,
                success_api =>
                {
                    var linkedInApi = success_api as OAuth2LinkedIn;

                    using (var cli = linkedInApi.NewAuthorizedClient())
                    {
                        var json = cli.DownloadString(
                            "https://api.linkedin.com/v2/me" +
                            "?projection=(id,firstName,lastName,profilePicture(displayImage~:playableStreams))");

                        var profileJson = JObject.Parse(json);
                        
                        linkedInApi.PersonId = profileJson["id"]?.Value<string>();

                        linkedInApi.PersonName =
                            profileJson["firstName"]?["localized"]?.First?.First?.Value<string>()
                            + " " + profileJson["lastName"]?["localized"]?.First?.First?.Value<string>();

                        linkedInApi.PersonPhotoUrl =
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
                    }

                    using (var cli = linkedInApi.NewAuthorizedClient())
                    {
                        var json = cli.DownloadString(
                            "https://api.linkedin.com/v2/emailAddress" +
                            "?q=members&projection=(elements*(handle~))");

                        var emailJson = JObject.Parse(json);

                        linkedInApi.PersonEmail =
                           emailJson["elements"].First?["handle~"]?["emailAddress"]?.Value<string>();
                    }


                    success(linkedInApi);
                },
                failure_api => failure((OAuth2LinkedIn)failure_api))
        {
        }

    }
}
