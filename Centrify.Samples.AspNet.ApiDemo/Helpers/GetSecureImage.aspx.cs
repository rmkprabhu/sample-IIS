using System;
using System.Net;
using Centrify.Samples.AspNet.ApiLib;
using System.Configuration;

public partial class GetSecureImage : System.Web.UI.Page
{
    public static string TenantUrl = ConfigurationManager.AppSettings["TenantUrl"].ToString();

    private RestClient m_restClient = null;
  
    public string BearerToken
    {       
        get
        {
            m_restClient = (RestClient)Session["AuthenticaitonClient"];
            if (m_restClient.BearerToken != null)
            {
                return m_restClient.BearerToken;
            }
            else
            {
                if (m_restClient.Cookies != null)
                {
                    CookieCollection endpointCookies = m_restClient.Cookies.GetCookies(new Uri(m_restClient.Endpoint));
                    if (endpointCookies != null)
                    {
                        Cookie bearerCookie = endpointCookies[".ASPXAUTH"];
                        if (bearerCookie != null)
                        {
                            return bearerCookie.Value;
                        }
                    }
                }
            }
            return null;
        }

        set
        {
            m_restClient.BearerToken = value;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        string strIcon = Request.QueryString["Icon"].ToString();
        RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];

        //Get Icon from Centrify
        WebClient wc = new WebClient();
        wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.CacheIfAvailable);
        wc.Headers.Add("Authorization", "Bearer " + BearerToken);
        byte[] bytes = wc.DownloadData(TenantUrl + strIcon);
        Response.ContentType = "image/jpg";
        Response.BinaryWrite(bytes);
    }
}