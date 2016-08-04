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



public partial class Register : Page
{
    HttpContext context = HttpContext.Current;

    public static string TenantUrl = ConfigurationManager.AppSettings["TenantUrl"].ToString();
    public static string AdminServiceAccount = ConfigurationManager.AppSettings["AdminServiceAccount"].ToString();
    public static string AdminServicePass = ConfigurationManager.AppSettings["AdminServicePass"].ToString();
    public static string DefaultDomain = ConfigurationManager.AppSettings["DefaultDomain"].ToString();

    protected void Page_Load(object sender, EventArgs e)
    {
        defaultDomainLabel.InnerText = DefaultDomain;
    }

    protected void Submit_Registration(object sender, EventArgs e)
    {
        //Create a new client to store authentication
        RestClient authenticationClient = UiDrivenLogin.Authenticate(TenantUrl, AdminServiceAccount);
        //Parse result challenge list. This is a service account so we can assume the results will be password and that there will only be one result.
        Dictionary<string, dynamic> mech = authenticationClient.ChallengeCollection[0]["Mechanisms"][0];

        //Login for service account is assumed to only require password and no MFA.
        if (mech["AnswerType"] == "Text")
        {
            //Call advance authentication with our service account credentials
            UiDrivenLogin.AdvanceForMech(authenticationClient, authenticationClient.TenantId, authenticationClient.SessionId, true, mech, null, AdminServicePass);
        }
        //Something other then text was returned which indicates that the service account is not set up correctly.
        else
        {
            FailureText.Text = "The service account is not set up correctly. It should only require a password an no MFA.";
            ErrorMessage.Visible = true;

            Registration.Visible = false; //Hide our registration form. 
        }

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

            createUserResult = userManagementClient.CreateUser(cUser, false, true, true, false, true);
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

