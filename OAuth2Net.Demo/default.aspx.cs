using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OAuth2Net.Demo
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var fid = (FormsIdentity)Context.User.Identity;

            var claim = new Func<string, string>(cName => 
                fid.Claims
                .Where(c => c.Type == cName)
                .Select(c => c.Value)
                .FirstOrDefault());

            PersonId.Text = claim(ClaimTypes.Sid);
            PersonName.Text = claim(ClaimTypes.Name);
            PersonEmail.Text = claim(ClaimTypes.Email);
            PersonPicture.ImageUrl = claim("ProfileImageUrl");
        }
    }
}