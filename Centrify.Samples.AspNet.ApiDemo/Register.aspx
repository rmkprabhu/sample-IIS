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

<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="Register" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="form-horizontal">
            <br />
            <h3>New User Registration</h3>
            <br />

            <div class="row">
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
                    </asp:PlaceHolder>
                </div>

                <div id="Registration" runat="server">
                    <div class="row">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="LoginName" CssClass="text-danger" ErrorMessage="The Login Name field is required." />
                        <div class="form-inline" style="display: block;">
                        <asp:Label runat="server" ID="LoginName_Label" AssociatedControlID="LoginName" CssClass="col-md-2 control-label">Login Name</asp:Label>
                        
                            
                            <asp:TextBox runat="server" ID="LoginName" CssClass="form-control" /> <a id="defaultDomainLabel" runat="server" ></a>
      
                        </div>
                    </div>
                    <br />

                    <div class="row">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Email" CssClass="text-danger" ErrorMessage="The Email field is required." />
                        <asp:Label runat="server" ID="Email_Label" AssociatedControlID="Email" CssClass="col-md-2 control-label">Email</asp:Label>
                        <div class="col-md-3">
                            <asp:TextBox runat="server" ID="Email" CssClass="form-control" />
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="DisplayName" CssClass="text-danger" ErrorMessage="The Display Name field is required." />
                        <asp:Label runat="server" ID="DisplayName_Label" AssociatedControlID="DisplayName" CssClass="col-md-2 control-label">Display Name</asp:Label>
                        <div class="col-md-3">
                            <asp:TextBox runat="server" ID="DisplayName" CssClass="form-control" />
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="The Password field is required." />
                        <asp:Label runat="server" ID="Passwword_Label" AssociatedControlID="Password" CssClass="col-md-2 control-label">Password</asp:Label>
                        <div class="col-md-3">
                            <asp:TextBox runat="server" ID="Password" CssClass="form-control" />
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Confirm_Password" CssClass="text-danger" ErrorMessage="The Confirm Password field is required." />
                        <asp:Label runat="server" ID="Confirm_Password_Label" AssociatedControlID="Confirm_Password" CssClass="col-md-2 control-label">Confirm Password</asp:Label>
                        <div class="col-md-3">
                            <asp:TextBox runat="server" ID="Confirm_Password" CssClass="form-control" />
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
                    <br />
                    <div class="col-md-offset-1 col-md-8" runat="server">
                        <div class="row">
                            <div class="col-md-offset-5" runat="server" id="Submit_Button_Div">
                                <asp:Button ID="Submit_Button" runat="server" OnClick="Submit_Registration" Text="Submit" CssClass="btn btn-default" OnClientClick="if (!Page_ClientValidate()){ return false; } this.disabled = true; this.value = 'Submitting...';" UseSubmitBehavior="false" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
