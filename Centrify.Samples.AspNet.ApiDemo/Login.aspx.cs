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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Centrify.Samples.AspNet.ApiLib;
using System.Configuration;


public partial class Login : System.Web.UI.Page
{
    public static string TenantUrl = ConfigurationManager.AppSettings["TenantUrl"].ToString();
    public static string DefaultDomain = ConfigurationManager.AppSettings["DefaultDomain"].ToString();

    protected void Page_Load(object sender, EventArgs e)
    {
        //Used for continuing social login
        if (Request.QueryString["ExtIdpAuthChallengeState"] != null)
        {
            Continue_Social_Login(Request.QueryString["ExtIdpAuthChallengeState"]);
            Login_Div.Visible = false;
            SocialLogin_Div.Visible = false;
        }        
    }
    protected void StartAuthentication(object sender, EventArgs e)
    {
        RestClient authenticationClient = UiDrivenLogin.Authenticate(TenantUrl, UserName_TextBox.Text + DefaultDomain);

        if (authenticationClient != null)
        {
            ProcessChallenges(authenticationClient.ChallengeCollection);
            Session["AuthenticaitonClient"] = authenticationClient;

            UserName_TextBox.Enabled = false;
            SocialLogin_Div.Visible = false;
            Register_HyperLink_Div.Visible = false;

            if (authenticationClient.AllowPasswordReset)
            {
                ForgotPass_Button.Visible = true;
            }

            RememberMe_Div.Visible = true;
        }
        else
        {
            //There was an error
            Login_Div.Visible = false;
            FailureText.Text = "There was an unexpected error. Please contact your system administrator. Click here to <a href=\"Login.aspx\">start over.</a>";
            ErrorMessage.Visible = true;
        }
    }

    protected void ProcessChallenges(dynamic challengeCollection, bool doSecondChallenge = false)
    {
        //We should clear our dropdown list every time to ensure we are starting with a clean list.
        MFAChallenge_DropDownList.Items.Clear();

        // We need to satisfy one of each challenge collection:
        SortedList<string, string> items = new SortedList<string, string>();

        //If doSecondChallenge is true we will change the challenge set array to the second set. Otherwise we will use the first Challenge set
        int challengeSet = 0;

        if (doSecondChallenge)
        {
            challengeSet = 1;
        }

        //The first step for the drop down list is to populate a SortedList with our MFA mechanisms
        for (int mechIdx = 0; mechIdx < challengeCollection[challengeSet]["Mechanisms"].Count; mechIdx++)
        {
            string mechValue = ConvertDicToString(challengeCollection[challengeSet]["Mechanisms"][mechIdx]);
            items.Add(UiDrivenLogin.MechToDescription(challengeCollection[challengeSet]["Mechanisms"][mechIdx]), mechValue);
        }

        //The second step for the drop down list is to bind the SortedList to our dropdown element
        try
        {
            MFAChallenge_DropDownList.DataTextField = "Key";
            MFAChallenge_DropDownList.DataValueField = "Value";
            MFAChallenge_DropDownList.DataSource = items;
            MFAChallenge_DropDownList.DataBind();

        }
        catch (Exception ex)
        {
            //There was an error
            Login_Div.Visible = false;
            FailureText.Text = ex.InnerException.ToString();
            ErrorMessage.Visible = true;
        }

        StartAuthentication_Next_Button.Visible = false;
        AdvanceAuthentication_Next_Button.Visible = true;
        MFAChallenge_DropDownList.Visible = true;
        MFAChallenge_DropDownList_Label.Visible = true;

        Dictionary<string, string> mech = MFAChallenge_DropDownList.SelectedItem.Value.TrimEnd(';').Split(';').ToDictionary(item => item.Split('=')[0], item => item.Split('=')[1]);

        //If our first mechanism in our dropdown has a text answer type we should show our MFA answer text box.
        if (mech["AnswerType"] == "Text")
        {
            MFAAnswer_Label.Text = UiDrivenLogin.MechToPrompt(mech);
            MFAAnswer_Label.Visible = true;
            MFAAnswer_Textbox.Visible = true;
        }
        else if (mech["AnswerType"] != "Text" && mech["AnswerType"] != "StartTextOob")
        {
            //Set the Loading text for polling            
            MFALoad_Label.Text = MFAAnswer_Label.Text = UiDrivenLogin.MechToPrompt(mech);
        }
    }

    //Called when the user selects an MFA type from the MFA DropDown and presses next.
    protected void AdvanceAuthentication(object sender, EventArgs e)
    {
        Dictionary<string, string> mech = MFAChallenge_DropDownList.SelectedItem.Value.TrimEnd(';').Split(';').ToDictionary(item => item.Split('=')[0], item => item.Split('=')[1]);
        RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];

        Dictionary<string, dynamic> resultsDictionary = new Dictionary<string, dynamic>();

        if (sender.Equals(ForgotPass_Button))
        {
            resultsDictionary = UiDrivenLogin.AdvanceForMech(authenticationClient, authenticationClient.TenantId, authenticationClient.SessionId, RememberMe.Checked, null, null, null, false, true);
            MFAAnswer_Label.Visible = false;
            MFAAnswer_Textbox.Visible = false;
            RememberMe_Div.Visible = false;
            ForgotPass_Button_Div.Visible = false;
        }
        else
        {
            //If the mechanism is StartTextOob we need to allow the user to answer via text and poll at the same time. 
            if (mech["AnswerType"] == "StartTextOob")
            {
                if (StartTextOob_Timer.Enabled) //We have already called AdvanceAuthentication once and are calling it again using the MFA Answer Text Box submit button.
                {
                    resultsDictionary = UiDrivenLogin.AdvanceForMech(authenticationClient, authenticationClient.TenantId, authenticationClient.SessionId, RememberMe.Checked, null, MFAChallenge_DropDownList.SelectedItem.Value, MFAAnswer_Textbox.Text, true);
                }
                else //This is the first time calling AdvanceAuthenticaiton and we need to start polling and turn on our TimerTick while also displaying the MFA Answer Text Box for user input.
                {
                    resultsDictionary = UiDrivenLogin.AdvanceForMech(authenticationClient, authenticationClient.TenantId, authenticationClient.SessionId, RememberMe.Checked, null, MFAChallenge_DropDownList.SelectedItem.Value);

                    MFAAnswer_Label.Text = UiDrivenLogin.MechToPrompt(mech);
                    MFAAnswer_Label.Visible = true;
                    MFAAnswer_Textbox.Visible = true;
                    StartTextOob_Timer.Enabled = true; //Used to poll again every tick while waiting for user input in the form.
                }
            }
            else
            {
                //User input is required for standard text mechanisms
                if (mech["AnswerType"] == "Text")
                {
                    resultsDictionary = UiDrivenLogin.AdvanceForMech(authenticationClient, authenticationClient.TenantId, authenticationClient.SessionId, RememberMe.Checked, null, MFAChallenge_DropDownList.SelectedItem.Value, MFAAnswer_Textbox.Text);
                }
                //No user input required for polling mechanisms
                else
                {
                    resultsDictionary = UiDrivenLogin.AdvanceForMech(authenticationClient, authenticationClient.TenantId, authenticationClient.SessionId, RememberMe.Checked, null, MFAChallenge_DropDownList.SelectedItem.Value);
                    MFALoad_Label.Text = ""; //Change the loading text back to default
                }

                MFAAnswer_Label.Visible = false;
                MFAAnswer_Textbox.Visible = false;
            }
        }

        //Check results
        if (resultsDictionary["success"])
        {
            MFAChallenge_DropDownList.Visible = false;
            MFAChallenge_DropDownList_Label.Visible = false;

            //MFA was successful. We need to process the results to determine what should happen next.
            if (resultsDictionary["Result"]["Summary"] == "StartNextChallenge" || resultsDictionary["Result"]["Summary"] == "LoginSuccess" || resultsDictionary["Result"]["Summary"] == "NewPackage")
            {
                StartTextOob_Timer.Enabled = false; //Turn off timer so that we are not polling in the background for StartTextOob
                ProcessResults(resultsDictionary);
            }
            //If the summary is OobPending this means that StartTextOob has returned pending and we need to stop and wait. If any other results are present there is an error.
            else if (resultsDictionary["Result"]["Summary"] != "OobPending")
            {
                //There was an error
                Login_Div.Visible = false;
                FailureText.Text = "There was an unexpected error. Please contact your system administrator. Click here to <a href=\"Login.aspx\">start over.</a>";
                ErrorMessage.Visible = true;
            }
        }
        else
        {
            //There was an error
            Login_Div.Visible = false;
            FailureText.Text = resultsDictionary["Message"] + " Click here to <a href=\"Login.aspx\">start over.</a>";
            ErrorMessage.Visible = true;
        }
    }

    //Called every time the MFA Dropdown is changed by the user.
    protected void MFAChallenge_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Show text answer UI elements if selected MFA mechanism requires text input from the user.
        Dictionary<string, string> mech = MFAChallenge_DropDownList.SelectedItem.Value.TrimEnd(';').Split(';').ToDictionary(item => item.Split('=')[0], item => item.Split('=')[1]);
        if (mech["AnswerType"] == "Text")
        {
            MFAAnswer_Label.Text = UiDrivenLogin.MechToPrompt(mech);
            MFAAnswer_Label.Visible = true;
            MFAAnswer_Textbox.Visible = true;
        }
        else
        {
            MFAAnswer_Label.Visible = false;
            MFAAnswer_Textbox.Visible = false;
        }

        //make sure that the MFA Label is modified to match selected mech.
        if (mech["AnswerType"] != "StartOob")
        {
            //Set the Loading text for polling            
            MFALoad_Label.Text = "";
        }
        else
        {
            MFALoad_Label.Text = MFAAnswer_Label.Text = UiDrivenLogin.MechToPrompt(mech);
        }
    }

    protected void StartTextOob_TimerTick(object sender, EventArgs e)
    {
        //Start polling to see if user completed MFA remotely
        RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];
        Dictionary<string, dynamic> resultsDictionary = UiDrivenLogin.AdvanceForMech(authenticationClient, authenticationClient.TenantId, authenticationClient.SessionId, RememberMe.Checked, null, MFAChallenge_DropDownList.SelectedItem.Value, "PollOnce", true);

        //MFA was successful. We need to process the results to determine what should happen next.
        if (resultsDictionary["Result"]["Summary"] == "StartNextChallenge" || resultsDictionary["Result"]["Summary"] == "LoginSuccess" || resultsDictionary["Result"]["Summary"] == "NewPackage")
        {
            StartTextOob_Timer.Enabled = false;
            ProcessResults(resultsDictionary);
        }
        //If there is an error we need to stop polling
        else if (!resultsDictionary["success"])
        {
            StartTextOob_Timer.Enabled = false;

            Login_Div.Visible = false;
            FailureText.Text = resultsDictionary["Message"] + " Click here to <a href=\"Login.aspx\">start over.</a>";
            ErrorMessage.Visible = true;
        }
    }

    protected void ProcessResults(Dictionary<string, dynamic> resultsDictionary)
    {
        RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];

        //Once one challenge list has been completed, start the next one.
        if (resultsDictionary["Result"]["Summary"] == "StartNextChallenge")
        {            
            ProcessChallenges(authenticationClient.ChallengeCollection, true);
        }
        //If a new package was presented, start advance auth over again.
        else if (resultsDictionary["Result"]["Summary"] == "NewPackage")
        {
            authenticationClient.ChallengeCollection = resultsDictionary["Result"]["Challenges"];
            Session["AuthenticaitonClient"] = authenticationClient;
            ProcessChallenges(resultsDictionary["Result"]["Challenges"]);
        }
        //Successful Login
        else if (resultsDictionary["Result"]["Summary"] == "LoginSuccess")
        {
            //Login Complete
            Login_Div.Visible = false;
            Session["OTP"] = resultsDictionary["Result"]["Auth"];
            //Make sure our fully authenticated client is updated in the session.
            Session["AuthenticaitonClient"] = authenticationClient;

            if (Request.QueryString["redirect"] != null)
            {
                Server.Transfer(Request.QueryString["redirect"], true);
            }
            else
            {
                Server.Transfer("Default.aspx", true);
            }
        }
    }

    //Helper method to convert Dictionaries to String.
    public static string ConvertDicToString(IDictionary<string, dynamic> dict)
    {
        string result = string.Empty;
        if (dict != null)
        {
            foreach (var v in dict)
            {
                result += string.Format("{0}={1};", v.Key, v.Value.ToString());
            }
        }
        return result;
    }

    protected void SocialLogin(object sender, EventArgs e)
    {
        string strIDPName = "";

        if (sender.Equals(SocialLogin_FB_Button))
        {
            strIDPName = "Facebook";
        }
        else if (sender.Equals(SocialLogin_Google_Button))
        {
            strIDPName = "Google";
        }
        else if (sender.Equals(SocialLogin_LinkedIn_Button))
        {
            strIDPName = "LinkedIn";
        }
        else if (sender.Equals(SocialLogin_Microsoft_Button))
        {
            strIDPName = "Microsoft";
        }

        RestClient authenticationClient = UiDrivenLogin.StartSocialAuth(TenantUrl, strIDPName, Request.Url.ToString());

        if (authenticationClient != null)
        {
            Session["AuthenticaitonClient"] = authenticationClient;
            Response.Redirect(authenticationClient.SocialIdpRedirectUrl);
        }
        else
        {
            //There was an error
            Login_Div.Visible = false;
            FailureText.Text = "There was an unexpected error. Please contact your system administrator. Click here to <a href=\"Login.aspx\">start over.</a>";
            ErrorMessage.Visible = true;
        }
    }
    protected void Continue_Social_Login(string strExtIdpAuthChallengeState)
    {
        RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];
        Dictionary<string, dynamic> socialLoginResult = UiDrivenLogin.ContinueSocialAuth(authenticationClient, strExtIdpAuthChallengeState);

        if (socialLoginResult["success"].ToString() == "True")
        {
            //Login Complete
            Login_Div.Visible = false;
            Session["OTP"] = socialLoginResult["Result"]["Auth"];

            if (Request.QueryString["redirect"] != null)
            {
                Server.Transfer(Request.QueryString["redirect"], true);
            }
            else
            {
                Server.Transfer("Default.aspx", true);
            }
        }
        else
        {
            //There was an error
            Login_Div.Visible = false;
            FailureText.Text = socialLoginResult["Message"] + " Click here to <a href=\"Login.aspx\">start over.</a>";
            ErrorMessage.Visible = true;
        }

    }
}