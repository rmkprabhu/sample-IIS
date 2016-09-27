using System;
using Centrify.Samples.AspNet.ApiLib;

public partial class _Logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];
        Security SecurityClient = new Security(authenticationClient);

        Session["isLoggedIn"] = "false";
        Session["isAdmin"] = "false";

        SecurityClient.Logout();
    }
}