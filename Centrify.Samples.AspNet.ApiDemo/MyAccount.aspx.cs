using System;
using System.Web;
using System.Web.UI.WebControls;
using Centrify.Samples.AspNet.ApiLib;
using System.Collections.Generic;
using System.Configuration;

public partial class MyAccount : System.Web.UI.Page
{
    public static string TenantUrl = ConfigurationManager.AppSettings["TenantUrl"].ToString();
    public static string AdminServiceAccount = ConfigurationManager.AppSettings["AdminServiceAccount"].ToString();
    public static string AdminServicePass = ConfigurationManager.AppSettings["AdminServicePass"].ToString();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["AuthenticaitonClient"] == null)
        {
            MyAccount_Start_div.Visible = false;
            ErrorMessage.Visible = true;
            FailureText.Text = "You are not logged in. Please click <a href=\"Login.aspx?redirect=MyAccount.aspx\">here.</a> to log in.";
        }
        else
        {
            GetUserInfo(MyAccount_Start_div, new EventArgs());
        }
    }


    protected void GetUserInfo(object sender, EventArgs e)
    {
        Alias_DropDownList.Items.Clear();
        //Create a new client to store authentication
        RestClient serviceAuthenticationClient = UiDrivenLogin.Authenticate(TenantUrl, AdminServiceAccount);
        //Parse result challenge list. This is a service account so we can assume the results will be password and that there will only be one result.
        Dictionary<string, dynamic> mech = serviceAuthenticationClient.ChallengeCollection[0]["Mechanisms"][0];

        //Login for service account is assumed to only require password and no MFA.
        if (mech["AnswerType"] == "Text")
        {
            //Call advance authentication with our service account credentials
            UiDrivenLogin.AdvanceForMech(serviceAuthenticationClient, serviceAuthenticationClient.TenantId, serviceAuthenticationClient.SessionId, true, mech, null, AdminServicePass);
        }
        //Something other then text was returned which indicates that the service account is not set up correctly.
        else
        {
            FailureText.Text = "The service account is not set up correctly. It should only require a password an no MFA.";
            ErrorMessage.Visible = true;

            UserInfo_div.Visible = false;
            MyAccount_Start_div.Visible = false;
        }

        //Pull authentication client from session
        RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];

        //Create a new userManagementClient and pass our authenticated rest client from our login call.
        UserManagement userManagementClient = new UserManagement(authenticationClient);
        UserManagement serviceUserManagementClient = new UserManagement(serviceAuthenticationClient);

        Dictionary<string, dynamic> getAliases = userManagementClient.GetAliasesForTenant();

        //Get a list of all tenant Aliases
        int iCount = 0;
        foreach (var alias in getAliases["Result"]["Results"])
        {
            Alias_DropDownList.Items.Insert(0, new ListItem(alias["Row"]["ID"], iCount.ToString()));
            iCount++;
        }

        //Get User Info
        Dictionary<string, dynamic> getUserResult = userManagementClient.GetUser();
        //Get User UUID
        Dictionary<string, dynamic> queryResult = serviceUserManagementClient.Query(@"select ID from cduser where Name ='" + getUserResult["Result"]["Name"] + "'");

        int queryCount = queryResult["Count"];

        if (queryCount == 1)
        {
            //split the username and domain name. add user name into LoginName text box and select the correct domain in the alias dropdown.
            string s = getUserResult["Result"]["Name"];
            string[] userName = s.Split('@');

            Alias_DropDownList.Items.FindByText(userName[1]).Selected = true;

            //populate user info
            LoginName.Text = userName[0];
            UserUUID.Text = queryResult["Results"][0]["Row"]["ID"];
            DisplayName.Text = getUserResult["Result"]["DisplayName"];
            Email.Text = getUserResult["Result"]["Mail"];
            OfficeNumber.Text = getUserResult["Result"]["OfficeNumber"];
            MobileNumber.Text = getUserResult["Result"]["MobileNumber"];
            HomeNumber.Text = getUserResult["Result"]["HomeNumber"];
            InEverybodyRole.Checked = getUserResult["Result"]["InEverybodyRole"];

            //show/hide elements        
            UserInfo_div.Visible = true;
        }
        else {
            FailureText.Text = "There was an error getting the logged in users ID.";

            //hide unneeded elements.
            ErrorMessage.Visible = true;
            UserInfo_div.Visible = false;
            MyAccount_Start_div.Visible = false;
        }
    }

    protected void Submit_UserModify(object sender, EventArgs e)
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

            UserInfo_div.Visible = false;
            MyAccount_Start_div.Visible = false;
        }

        //Create a new userManagementClient and pass our authenticated rest client from our login call.
        UserManagement userManagementClient = new UserManagement(authenticationClient);
        CDUser cUser = new CDUser();
        cUser.Name = LoginName.Text + "@" + Alias_DropDownList.SelectedItem.ToString();
        cUser.ID = UserUUID.Text;
        cUser.DisplayName = DisplayName.Text;
        cUser.Mail = Email.Text;
        cUser.OfficeNumber = OfficeNumber.Text;
        cUser.MobileNumber = MobileNumber.Text;
        cUser.HomeNumber = HomeNumber.Text;

        Dictionary<string, dynamic> modifyUserResult = userManagementClient.ChangeUser(cUser, InEverybodyRole.Checked);

        if (modifyUserResult["success"])
        {
            SuccessText.Text = "User " + LoginName.Text + " was successfully modified.";
            SuccessMessage.Visible = true;

            //hide unneeded elements.
            UserInfo_div.Visible = false;
            MyAccount_Start_div.Visible = false;
        }
        else
        {
            FailureText.Text = "There was an error modifying the user: " + modifyUserResult["Message"];
            ErrorMessage.Visible = true;

            //hide unneeded elements.
            UserInfo_div.Visible = false;
            MyAccount_Start_div.Visible = false;
        }
    }
    protected void ResetPass(object sender, EventArgs e)
    {
        UserInfo_div.Visible = false;
        MyAccount_Start_div.Visible = false;
        ResetPass_Div.Visible = true;
    }
    protected void Submit_ResetPass(object sender, EventArgs e)
    {
        ErrorMessage.Visible = false;

        if (NewPass.Text == ConfirmPass.Text)
        {
            //Pull authentication client from session
            RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];

            //Create a new userManagementClient and pass our authenticated rest client from our login call.
            UserManagement userManagementClient = new UserManagement(authenticationClient);
            Dictionary<string, dynamic> changeUserPassResult = userManagementClient.ChangeUserPassword(OldPass.Text, NewPass.Text);

            if (changeUserPassResult["success"])
            {
                SuccessText.Text = "Password successfully changed.";
                SuccessMessage.Visible = true;

                //hide unneeded elements.
                ResetPass_Div.Visible = false;
                UserInfo_div.Visible = false;
                MyAccount_Start_div.Visible = false;
            }
            else
            {
                FailureText.Text = "There was an error resetting the password: " + changeUserPassResult["Message"];
                ErrorMessage.Visible = true;

                //hide unneeded elements.
                ResetPass_Div.Visible = false;
            }

        }
        else
        {
            FailureText.Text = "The passwords do not match.";
            //hide unneeded elements.
            ErrorMessage.Visible = true;
        }
    }
}