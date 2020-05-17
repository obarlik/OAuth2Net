using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OAuth2Net.Demo.OAuth.LinkedIn
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OAuth2LinkedIn.Callback(
                Request.Params["code"],
                Request.Params["error"],
                Request.Params["state"],
                Request.Params["error_description"],
                Request.Params["error_uri"],
                success: state =>
                {
                    OAuth2LinkedIn.SuccessCallback(state);

                    LoginHelper.LoginUser(
                        state.ProviderName + "-" + state.PersonId,
                        state.PersonName,
                        state.PersonEmail,
                        state.PersonPhotoUrl,
                        "User");

                    HttpContext.Current.Response.Redirect(state.ReturnUrl);
                },
                failure: state => HttpContext.Current.Response.Redirect(state.ReturnUrl));
        }
    }
}