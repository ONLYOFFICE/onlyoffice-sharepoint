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
        <asp:Label
            ID="DocumentServerTitle"
            runat="server"
            CssClass="document_server_title"
        />
        <asp:TextBox
            ID="DocumentServerHost"
            MaxLength="40"
            runat="server"
            CssClass="document_server_host"
        />
    </div>
    <asp:Label
            ID="Message"
            runat="server"
            CssClass="document_server_message"
        />
    <asp:Button
        ID="SaveSettings"
        runat="server"
        OnClick="Save_Click"
        CssClass="save_settings"
        />
</asp:Content>
