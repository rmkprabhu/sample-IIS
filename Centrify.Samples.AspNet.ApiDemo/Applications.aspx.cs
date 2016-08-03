using System;
using Centrify.Samples.AspNet.ApiLib;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Configuration;

public partial class _Default : System.Web.UI.Page
{
    public static string TenantUrl = ConfigurationManager.AppSettings["TenantUrl"].ToString();
    public static string RunAppUrl = ConfigurationManager.AppSettings["RunAppUrl"].ToString();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Session["AuthenticaitonClient"] != null)
            {
                RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];
                ApplicationManagement appManagementClient = new ApplicationManagement(authenticationClient);


                Dictionary<string, dynamic> dApps = appManagementClient.GetUPData();

                var appsList = dApps["Apps"];

                int iCount = 0;

                foreach (var app in appsList)
                {
                    string strDisplayName = app["DisplayName"];
                    string strAppKey = app["AppKey"];
                    string strIcon = app["Icon"];

                    AddUrls(strAppKey, strDisplayName, strIcon, iCount);

                    iCount++;
                }
            }
            else
            {
                Apps.Visible = false;
                ErrorMessage.Visible = true;
                FailureText.Text = "You are not logged in. Please click <a href=\"Login.aspx?redirect=Applications.aspx\">here.</a> to log in.";
            }
        }
        catch (Exception ex)
        {
            Apps.Visible = false;
            ErrorMessage.Visible = true;
            FailureText.Text = ex.InnerException.ToString();
        }
    }

    protected void AddUrls(string strAppKey, string strName, string strIcon, int count)
    {
        RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];

        HyperLink link = new HyperLink();

        link.ID = "CentrifyApp" + count;
        link.NavigateUrl = TenantUrl + RunAppUrl +strAppKey + "&Auth=" + Session["OTP"].ToString();
        link.Text = strName;

        //If image is unsecured global
        if (strIcon.Contains("vfslow"))
        {
            link.ImageUrl = TenantUrl + strIcon;
        }
        else//If image needs a cookie or header to access
        {
            link.ImageUrl = "Helpers/GetSecureImage.aspx?Icon=" + strIcon;
        }

        link.ImageHeight = 75;
        link.ImageWidth = 75;

        if (count % 7 == 0)
        {
            Apps.Controls.Add(new LiteralControl("<br />"));
        }
        else
        {
            Apps.Controls.Add(new LiteralControl("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;"));

        }

        Apps.Controls.Add(link);
    }
}