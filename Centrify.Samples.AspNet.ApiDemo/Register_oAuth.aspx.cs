/**
 * Copyright 2016 Centrify Corporation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 **/

using System;
using System.Web;
using System.Web.UI;
using Centrify.Samples.AspNet.ApiLib;
using System.Collections.Generic;
using System.Configuration;



public partial class Register_oAuth : Page
{
    HttpContext context = HttpContext.Current;

    public static string TenantUrl = ConfigurationManager.AppSettings["TenantUrl"].ToString();
    public static string ClientId = ConfigurationManager.AppSettings["ClientId"].ToString();
    public static string ClientSecret = ConfigurationManager.AppSettings["ClientSecret"].ToString();
    public static string OAuthAppId = ConfigurationManager.AppSettings["OAuthAppId"].ToString();
    public static string DefaultDomain = ConfigurationManager.AppSettings["DefaultDomain"].ToString();

    protected void Page_Load(object sender, EventArgs e)
    {      
    }

    protected void Submit_Registration(object sender, EventArgs e)
    {
        //ClientCreds Flow Example. Scope to CreateUser
        RestClient authenticationClient = new OAuthClient().Centrify_OAuthClientCredentials(TenantUrl, OAuthAppId, ClientId, ClientSecret, "CreateUser");
 
        //Set up dictionary to hold our create user results.
        Dictionary<string, dynamic> createUserResult = null;

        //Check if the passwords match
        if (Password.Text == Confirm_Password.Text)
        {
            //Create a new userManagementClient and pass our authenticated rest client from our login call.
            UserManagement userManagementClient = new UserManagement(authenticationClient);

            //Call the create user api and pass the contents of our registration form.
            CDUser cUser = new CDUser();
            cUser.Name = LoginName.Text + DefaultDomain;
            cUser.Mail = Email.Text;
            cUser.DisplayName = DisplayName.Text;
            cUser.Password = Password.Text;
            cUser.OfficeNumber = OfficeNumber.Text;
            cUser.MobileNumber = MobileNumber.Text;
            cUser.HomeNumber = HomeNumber.Text;

            createUserResult = userManagementClient.CreateUser(cUser, false, false, false, false, true);
        }
        //The passwords did not match
        else
        {
            FailureText.Text = "Password and Confirm Password do not match. Please refresh the page and try again.";
            ErrorMessage.Visible = true;

            Registration.Visible = false; //Hide our registration form. 
        }

        //Check if the create user call was successful and present the results if it was.
        if (createUserResult["success"])
        {
            SuccessText.Text = "User " + LoginName.Text + " was successfully created. Please here to <a href=\"Login.aspx\">login.";
            SuccessMessage.Visible = true;

            Registration.Visible = false; //Hide our registration form. 
        }
        //The create user api call was not successful. Present the returned error message from the api.
        else
        {
            FailureText.Text = createUserResult["Message"];
            ErrorMessage.Visible = true;

            Registration.Visible = false; //Hide our registration form. 
        }
    }
}

