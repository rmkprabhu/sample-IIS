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

<%@ Page Title="Sign in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <div id="Main_div" runat="server">
        <br />
        <h3>Centrify API Demo</h3>
        A demonstration of the power of integrating with the Centrify Identify Platform.
                <br />
        <br />
        <hr />
        <br />
        <h4>Introduction</h4>
        The Centrify Identity Platform (CIP) is the power behind many popular Centrify products such as <a runat="server" href="https://www.centrify.com/products/identity-service/">Centrtify Identity Service </a>, and <a runat="server" href="https://www.centrify.com/products/privilege-service/">Centrify Privilege Service. </a>CIP is not just available for Centrify products to use, however, which is why every powerful feature of the platform is available to developers via RESTful API's. Every feature of Centrify Identify Service can be integrated into your own websites and products. MFA can be integrated into your own applications. Identity management can be simplified for your customer facing products and websites. The possibilities are many with the Centrify Identity Platform. 

        <br />
        <br />
        The purpose of this demonstration is to show a few of the integration possibilities of the Centrify Identity Platform. This entire site has been programmed in ASP.Net and C#, and every page is independent. This demo is completely open source and can be used in integration projects or simply to learn. The source code can be found on <a runat="server" href="https://github.com/centrify/centrify-samples-aspnet-server">Github </a>. To learn more about the Centrify Identity Platform API's, and how to use them, please visit the <a runat="server" href="http://developer.centrify.com/site/global/home/index.gsp">Centrify Developer Portal </a>. If you have any questions, or would like to speak with a Developer Advocate regarding the Centrify API's, custom developement, or custom integration, please email us at <a href="mailto:devsupport@centrify.com" target="_top">devsupport@centrify.com</a>.
        <br />
        <br />
        To get started with this demo, you should first login. Once you are logged in, more options will be available in the navigation menu at the top of each page.
    </div>

</asp:Content>
