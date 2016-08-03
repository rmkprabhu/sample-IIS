<%--
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
--%>

<%@ Page Title="Sign in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Manage.aspx.cs" Inherits="Manage" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="form-horizontal">
            <div class="row">
                <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                    <p class="text-danger">
                        <asp:Literal runat="server" ID="FailureText" />
                    </p>
                </asp:PlaceHolder>
            </div>

            <div class="row">
                <asp:PlaceHolder runat="server" ID="SuccessMessage" Visible="false">
                    <asp:Literal runat="server" ID="SuccessText" />
                    <br />
                    <a runat="server" href="~/Manage">Back </a>
                </asp:PlaceHolder>
            </div>
            <div id="Manage_Start_div" runat="server">
                <br />
                <h3>Account Managment</h3>
                Create a new user by clicking "new user" or search for a user to modify using the search box.               
                <br />
                <hr />

                <div class="row">
                    <br />
                    <div class="form-inline" style="display: block;">
                        <div class="col-md-3">
                            <asp:Button ID="Create_User_Button" runat="server" OnClick="CreateUser" Text="New User" CssClass="btn btn-default" />
                        </div>

                        <asp:Button ID="User_Search_Button" runat="server" OnClick="User_Search" Text="Search" CssClass="btn btn-default" />
                        <asp:TextBox runat="server" ID="User_Search_Textbox" CssClass="form-control" />
                    </div>
                </div>
            </div>
            <div id="CreateUser_div" runat="server" visible="false">
                <br />
                <h3>Account Managment - Create New User</h3>
                Populate the new user form and click submit
                <br />
                <a runat="server" href="~/Manage">Back </a>
                <br />
                <hr />
            </div>
            <div id="ModifyUser_div" runat="server" visible="false">
                <br />
                <h3>Account Managment - Modify User</h3>
                Modify user information and click submit.
                <br />
                <a runat="server" href="~/Manage">Back </a>
                <br />
                <hr />
            </div>
            <div id="UserInfo_div" runat="server" visible="false">
                <div class="row">
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="LoginName" CssClass="text-danger" ErrorMessage="The Login Name field is required." />
                    <div class="form-inline" style="display: block;">
                        <asp:Label runat="server" ID="LoginName_Label" AssociatedControlID="LoginName" CssClass="col-md-2 control-label">*Login Name</asp:Label>
                        <asp:TextBox runat="server" ID="LoginName" CssClass="form-control" />
                        <asp:DropDownList ID="Alias_DropDownList" runat="server" AppendDataBoundItems="true" CssClass="form-control" />
                    </div>
                </div>
                <br />
                <div class="row">
                    <asp:Label runat="server" ID="UserUUID_Label" AssociatedControlID="UserUUID" CssClass="col-md-2 control-label">User UUID</asp:Label>
                    <div class="col-md-3">
                        <asp:TextBox runat="server" ID="UserUUID" CssClass="form-control" Enabled="false" />
                    </div>
                </div>
                <br />
                <div class="row">
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="Email" CssClass="text-danger" ErrorMessage="The Email field is required." />
                    <asp:Label runat="server" ID="Email_Label" AssociatedControlID="Email" CssClass="col-md-2 control-label">*Email</asp:Label>
                    <div class="col-md-3">
                        <asp:TextBox runat="server" ID="Email" CssClass="form-control" />
                    </div>
                </div>
                <br />
                <div class="row">
                    <asp:Label runat="server" ID="DisplayName_Label" AssociatedControlID="DisplayName" CssClass="col-md-2 control-label">Display Name</asp:Label>
                    <div class="col-md-3">
                        <asp:TextBox runat="server" ID="DisplayName" CssClass="form-control" />
                    </div>
                </div>
                <br />
                <div class="row">
                    <asp:Label runat="server" ID="OfficeNumber_Label" AssociatedControlID="OfficeNumber" CssClass="col-md-2 control-label">Office Number</asp:Label>
                    <div class="col-md-3">
                        <asp:TextBox runat="server" ID="OfficeNumber" CssClass="form-control" />
                    </div>
                </div>
                <br />
                <div class="row">
                    <asp:Label runat="server" ID="MobileNumber_Label" AssociatedControlID="MobileNumber" CssClass="col-md-2 control-label">Mobile Number</asp:Label>
                    <div class="col-md-3">
                        <asp:TextBox runat="server" ID="MobileNumber" CssClass="form-control" />
                    </div>
                </div>
                <br />
                <div class="row">
                    <asp:Label runat="server" ID="HomeNumber_Label" AssociatedControlID="HomeNumber" CssClass="col-md-2 control-label">Home Number</asp:Label>
                    <div class="col-md-3">
                        <asp:TextBox runat="server" ID="HomeNumber" CssClass="form-control" />
                    </div>
                </div>
                <div class="row">
                    <asp:CheckBox runat="server" ID="PassNeverExpires" />
                    <asp:Label runat="server" ID="PassNeverExpires_Label" AssociatedControlID="PassNeverExpires" CssClass="col-md-2 control-label">Password Never Expires</asp:Label>
                </div>
                <div class="row">
                    <asp:CheckBox runat="server" ID="ForcePassChange" />
                    <asp:Label runat="server" ID="ForcePassChange_Label" AssociatedControlID="ForcePassChange" CssClass="col-md-2 control-label">Force Password Change</asp:Label>
                </div>
                <div class="row">
                    <asp:CheckBox runat="server" ID="SendEmailInvite" />
                    <asp:Label runat="server" ID="SendEmailInvite_Label" AssociatedControlID="SendEmailInvite" CssClass="col-md-2 control-label">Send Email Invite</asp:Label>
                </div>
                <div class="row">
                    <asp:CheckBox runat="server" ID="SendSmsInvite" />
                    <asp:Label runat="server" ID="SendSmsInvite_Label" AssociatedControlID="SendSmsInvite" CssClass="col-md-2 control-label">Send Sms Invite</asp:Label>
                </div>
                <div class="row">
                    <asp:CheckBox runat="server" ID="InEverybodyRole" />
                    <asp:Label runat="server" ID="InEverybodyRole_Label" AssociatedControlID="InEverybodyRole" CssClass="col-md-2 control-label">In Everybody Role</asp:Label>
                </div>
                <br />
                <div class="col-md-offset-1 col-md-8" runat="server">
                    <div class="row">
                        <div class="col-md-offset-5" runat="server" id="Create_Submit_Button_Div" visible="false">
                            <asp:Button ID="CreateUser_Submit" runat="server" OnClick="Submit_UserCreate" Text="Submit" CssClass="btn btn-default" OnClientClick="if (!Page_ClientValidate()){ return false; } this.disabled = true; this.value = 'Submitting...';" UseSubmitBehavior="false" />
                        </div>
                        <div class="col-md-offset-5" runat="server" id="Modify_Submit_Button_Div" visible="false">
                            <asp:Button ID="ModifyUser_Submit" runat="server" OnClick="Submit_UserModify" Text="Submit" CssClass="btn btn-default" OnClientClick="if (!Page_ClientValidate()){ return false; } this.disabled = true; this.value = 'Submitting...';" UseSubmitBehavior="false" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
