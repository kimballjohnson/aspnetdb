/*
This software is provided under GPL v3 license http://www.gnu.org/licenses/gpl-3.0.txt.
*/
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace org.bsodhi.SecuApp
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            if (Membership.ValidateUser(Login1.UserName, Login1.Password))
            {
                // User has supplied valid credentials

                // In the following method call, the second Boolean parameter 
                // determines whether a persistent authentication cookie
                // is created.
                e.Authenticated = true;
                FormsAuthentication.RedirectFromLoginPage(Login1.UserName, Login1.RememberMeSet);
            }

        }
    }
}
