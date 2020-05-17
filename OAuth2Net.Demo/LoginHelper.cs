using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OAuth2Net.Demo
{
    public static class LoginHelper
    {
        public static void LoginUser(
            string id,
            string name,
            string email,
            string pictureUrl,
            params string[] roles)
        {
            var session = HttpContext.Current.Session;

            session["PersonId"] = id;
            session["PersonName"] = name;
            session["PersonEmail"] = email;
            session["PersonPicture"] = pictureUrl;
            session["Roles"] = roles;
        }
    }
}