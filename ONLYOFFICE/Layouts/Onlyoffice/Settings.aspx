<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="Onlyoffice.Layouts.Settings" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link rel="stylesheet" runat="server" media="screen" href="./css/styles.css" />
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    ONLYOFFICE
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="settings_container">
        <asp:Label ID="SettingsHeader" runat="server" CssClass="setting_title settings_header"/> <br />
        <asp:Label ID="SettingsDescription" runat="server" CssClass="settings_description"/> <br />
        <div style="margin-bottom: 30px;">
            <a href="https://helpcenter.onlyoffice.com/integration/sharepoint.aspx" target="_blank" style="margin-right: 12px;">
                <asp:Label ID="LearnMoreLink" runat="server"/>
            </a>
            <a href="https://feedback.onlyoffice.com/forums/966080-your-voice-matters?category_id=519288" target="_blank">
                <asp:Label ID="SuggestFeatureLink" runat="server"/>
            </a>
        </div>
        <div class="input_container">
            <asp:Label ID="DocumentServerTitle" runat="server" CssClass="setting_title" />
            <div class="tooltip_container">
                <img src="./images/i.svg"/>
                <asp:Label ID="DocsTooltip" runat="server" CssClass="tooltip_text"/>
            </div>
            <br />
            <asp:TextBox ID="DocumentServerHost" MaxLength="255" runat="server" CssClass="setting_input" />
        </div>
        <div class="input_container">
            <asp:Label ID="JwtSecretTitle" runat="server" CssClass="setting_title" />
            <div class="tooltip_container">
                <img src="./images/i.svg"/>
                <asp:Label ID="SecretTooltip" runat="server" CssClass="tooltip_text"/>
            </div>
            <br />
            <asp:TextBox ID="JwtSecret" MaxLength="255" runat="server" CssClass="setting_input" />
        </div>
        <div class="input_container">
            <asp:Label ID="JwtHeaderTitle" runat="server" CssClass="setting_title" />
            <div class="tooltip_container">
                <img src="./images/i.svg"/>
                <asp:Label ID="JwtHeaderTooltip" runat="server" CssClass="tooltip_text"/>
            </div>
            <br />
            <asp:TextBox ID="JwtHeader" MaxLength="255" runat="server" CssClass="setting_input" />
        </div>

        <asp:CheckBox id="DemoCheckbox" runat="server" Text=""
            TextAlign="Right"
            CssClass="demo_checkbox"
        /> <br />
        <asp:Label ID="DemoDescription" runat="server" CssClass="demo_description"/> <br />

        <asp:Label ID="Message" runat="server" CssClass="setting_message" />
    </div>

    <asp:Button ID="SaveSettings" runat="server" OnClick="Save_Click" CssClass="setting_save" />

    <a href="https://www.onlyoffice.com/docs-registration" target="_blank" class="banner">
        <img src="./images/DocsCloudBanner.svg"/>
    </a>
</asp:Content>
