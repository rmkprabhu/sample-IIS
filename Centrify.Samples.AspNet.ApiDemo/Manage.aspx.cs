using System;
using System.Web;
using System.Web.UI.WebControls;
using Centrify.Samples.AspNet.ApiLib;
using System.Collections.Generic;
using System.Configuration;

public partial class Manage : System.Web.UI.Page
{
    public static string TenantUrl = ConfigurationManager.AppSettings["TenantUrl"].ToString();
    public static string AdminServiceAccount = ConfigurationManager.AppSettings["AdminServiceAccount"].ToString();
    public static string AdminServicePass = ConfigurationManager.AppSettings["AdminServicePass"].ToString();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["AuthenticaitonClient"] != null)
        {
            //Pull authentication client from session
            RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];

            //Create a new userManagementClient and pass our authenticated rest client from our login call.
            UserManagement userManagementClient = new UserManagement(authenticationClient);

            //Get a list of all tenant Aliases
            Dictionary<string, dynamic> getUserInfo = userManagementClient.GetUserInfo();

            if (!getUserInfo["Result"]["IsSysAdmin"])
            {
                Manage_Start_div.Visible = false;
                ErrorMessage.Visible = true;
                FailureText.Text = "You do not have administration access. Please log in with an admin account.";
            }
        }
        else
        {
            Manage_Start_div.Visible = false;
            ErrorMessage.Visible = true;
            FailureText.Text = "You are not logged in. Please click <a href=\"Login.aspx?redirect=Manage.aspx\">here.</a> to log in.";
        }
    }
    protected void CreateUser(object sender, EventArgs e)
    {
        //Pull authentication client from session
        RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];

        //Create a new userManagementClient and pass our authenticated rest client from our login call.
        UserManagement userManagementClient = new UserManagement(authenticationClient);

        //Get a list of all tenant Aliases
        Dictionary<string, dynamic> getAliases = userManagementClient.GetAliasesForTenant();

        int iCount = 0;
        foreach (var alias in getAliases["Result"]["Results"])
        {
            Alias_DropDownList.Items.Insert(0, new ListItem(alias["Row"]["ID"], iCount.ToString()));
            iCount++;
        }

        Manage_Start_div.Visible = false;
        CreateUser_div.Visible = true;
        UserInfo_div.Visible = true;
        UserUUID.Visible = false;
        Create_Submit_Button_Div.Visible = true;
    }

    protected void User_Search(object sender, EventArgs e)
    {
        Alias_DropDownList.Items.Clear();

        //Pull authentication client from session
        RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];

        //Create a new userManagementClient and pass our authenticated rest client from our login call.
        UserManagement userManagementClient = new UserManagement(authenticationClient);
       
        Dictionary<string, dynamic> getAliases = userManagementClient.GetAliasesForTenant();

        //Get a list of all tenant Aliases
        int iCount = 0;
        foreach (var alias in getAliases["Result"]["Results"])
        {
            Alias_DropDownList.Items.Insert(0, new ListItem(alias["Row"]["ID"], iCount.ToString()));
            iCount++;
        }

        //Query Centrify to see if user exists
        Dictionary<string, dynamic> queryResult = userManagementClient.Query(@"select ID from cduser where Name ='" + User_Search_Textbox.Text + "'");

        int queryCount = queryResult["Count"];

        if (queryCount == 1)
        {
            string userUuid = queryResult["Results"][0]["Row"]["ID"];
            Dictionary<string, dynamic> getUser = userManagementClient.GetUser(userUuid);
            Dictionary<string, dynamic> getUserResult = getUser["Result"];

            //split the username and domain name. add user name into LoginName text box and select the correct domain in the alias dropdown.
            string s = getUserResult["Name"];
            string[] userName = s.Split('@');

            Alias_DropDownList.Items.FindByText(userName[1]).Selected = true;

            //populate user info
            LoginName.Text = userName[0];
            UserUUID.Text = queryResult["Results"][0]["Row"]["ID"];
            Email.Text = getUserResult["Mail"];

            //check that optional attributes are populated
            if (getUserResult.ContainsKey("DisplayName"))
            {
                DisplayName.Text = getUserResult["DisplayName"];
            }

            if (getUserResult.ContainsKey("OfficeNumber"))
            {
                OfficeNumber.Text = getUserResult["OfficeNumber"];
            }

            if (getUserResult.ContainsKey("MobileNumber"))
            {
                MobileNumber.Text = getUserResult["MobileNumber"];
            }

            if (getUserResult.ContainsKey("HomeNumber"))
            {
                HomeNumber.Text = getUserResult["HomeNumber"];
            }

            if (getUserResult.ContainsKey("InEverybodyRole"))
            {
                InEverybodyRole.Checked = getUserResult["InEverybodyRole"];
            }

            //show/hide elements
            Manage_Start_div.Visible = false;
            ModifyUser_div.Visible = true;
            UserInfo_div.Visible = true;
            Modify_Submit_Button_Div.Visible = true;
            ForcePassChange.Visible = false;
            ForcePassChange_Label.Visible = false;
            SendEmailInvite.Visible = false;
            SendEmailInvite_Label.Visible = false;
            SendSmsInvite.Visible = false;
            SendSmsInvite_Label.Visible = false;

        }
        else if (queryCount == 0)
        {
            SuccessText.Text = "User " + User_Search_Textbox.Text + " was not found. Please try again.";
            SuccessMessage.Visible = true;
        }
        else
        {
            FailureText.Text = "More then one user was found with the same username. Please make sure usernames are unique in Centrify.";

            //hide unneeded elements.
            ErrorMessage.Visible = true;
            ModifyUser_div.Visible = false;
            UserInfo_div.Visible = false;
            Modify_Submit_Button_Div.Visible = false;
        }

    }
    protected void Submit_UserCreate(object sender, EventArgs e)
    {
        //Pull authentication client from session
        RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];

        //Set up dictionary to hold our create user results.
        Dictionary<string, dynamic> createUserResult = null;

        //Check if the passwords match

        //Create a new userManagementClient and pass our authenticated rest client from our login call.
        UserManagement userManagementClient = new UserManagement(authenticationClient);

        //Call the create user api and pass the contents of our registration form.
        CDUser cUser = new CDUser();
        cUser.Name = LoginName.Text + "@" + Alias_DropDownList.SelectedItem.ToString();
        cUser.Mail = Email.Text;
        cUser.DisplayName = DisplayName.Text;
        cUser.OfficeNumber = OfficeNumber.Text;
        cUser.MobileNumber = MobileNumber.Text;
        cUser.HomeNumber = HomeNumber.Text;

        createUserResult = userManagementClient.CreateUser(cUser, PassNeverExpires.Checked, ForcePassChange.Checked, SendEmailInvite.Checked, SendSmsInvite.Checked, InEverybodyRole.Checked);

        //Check if the create user call was successful and present the results if it was.
        if (createUserResult["success"])
        {
            SuccessText.Text = "User " + LoginName.Text + " was successfully created. The new users UUID is: " + createUserResult["Result"] + ".";
            SuccessMessage.Visible = true;

            CreateUser_div.Visible = false;
            UserInfo_div.Visible = false;
            Create_Submit_Button_Div.Visible = false;
        }
        //The create user api call was not successful. Present the returned error message from the api.
        else
        {
            FailureText.Text = createUserResult["Message"];
            ErrorMessage.Visible = true;

            CreateUser_div.Visible = false;
            UserInfo_div.Visible = false;
            Create_Submit_Button_Div.Visible = false;
        }
    }
    protected void Submit_UserModify(object sender, EventArgs e)
    {
        //Pull authentication client from session
        RestClient authenticationClient = (RestClient)Session["AuthenticaitonClient"];

        //Create a new userManagementClient and pass our authenticated rest client from our login call.
        UserManagement userManagementClient = new UserManagement(authenticationClient);
        CDUser cUser = new CDUser();
        cUser.ID = UserUUID.Text;
        cUser.Name = LoginName.Text + "@" + Alias_DropDownList.SelectedItem.ToString();
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
            ModifyUser_div.Visible = false;
            UserInfo_div.Visible = false;
            Modify_Submit_Button_Div.Visible = false;
        }
        else
        {
            FailureText.Text = "There was an error modifying the user: " + modifyUserResult["Message"];

            //hide unneeded elements.
            ErrorMessage.Visible = true;
            ModifyUser_div.Visible = false;
            UserInfo_div.Visible = false;
            Modify_Submit_Button_Div.Visible = false;
        }
    }
}