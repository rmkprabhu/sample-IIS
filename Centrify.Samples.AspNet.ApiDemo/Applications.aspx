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

<%@ Page Title="Sign in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Applications.aspx.cs" Inherits="_Default" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="row">
        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
            <p class="text-danger">
                <asp:Literal runat="server" ID="FailureText" />
            </p>
        </asp:PlaceHolder>
    </div>
    <div class="row" runat="server" id="Apps">
        <br />
        <h2>Centrify Applications</h2>
        <p>
            This is a list of applications from Centrify that you are allowed to access. This page was created using the <a runat="server" href="http://developer.centrify.com/site/global/documentation/api_reference/uprest/get_up_data/index.gsp">uprest/get_up_data </a> API. For more information, please visit the <a runat="server" href="http://developer.centrify.com/site/global/documentation/api_guide/adding_applications_to_a_website/index.gsp">Adding Applications to a Website </a> guide on the developer portal. 
        </p>
        <br />
        <hr />
    </div>
</asp:Content>
