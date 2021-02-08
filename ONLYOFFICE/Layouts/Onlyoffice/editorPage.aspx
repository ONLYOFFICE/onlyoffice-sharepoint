<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="editorPage.aspx.cs" Inherits="Onlyoffice.Layouts.editorPage" MasterPageFile="default.master" %>

<asp:Content id="Content1" ContentPlaceholderID="MainContent" runat="server" style="height: 100%; margin: 0;">
    <asp:panel runat="server" id="panelMain" style="height: 100%; margin: 0;">
        <div id="placeholder" style="height: 100%"></div>

        <script type="text/javascript" src= "<%= DocumentSeverHost %>web-apps/apps/api/documents/api.js"></script>
        <script type="text/javascript">

        window.docEditor = new DocsAPI.DocEditor("placeholder",
            {
                "document": {
                    "fileType":     "<%= FileType %>",
                    "info": {
                        "author":   "<%= FileAuthor %>",
                        "created":  "<%= FileTimeCreated %>",
                        "folder":   "<%= Folder %>",
                        "owner":   "<%= FileAuthor %>",
                        "uploaded":  "<%= FileTimeCreated %>"
                    },
                    "key":          "<%= Key %>",
                    "permissions": {
                        "edit":      "<%= canEdit.ToString().ToLower() %>",
                    },
                    "title":        "<%= FileName %>",
                    "url": "<%= SPUrl %>/_layouts/<%= SPVersion %>Onlyoffice/CallbackHandler.ashx?data=<%= HttpUtility.UrlEncode(urlDocDownload) %>"
                },
                "documentType": "<%= documentType %>",
                "editorConfig": {
                    "lang":         "<%= lang %>",
                    "mode":         "<%= FileEditorMode %>",
                    "user": {
                        "id":       "<%= CurrentUserId %>",
                        "name":     "<%= CurrentUserName %>"
                    },
                    "customization": {
                        "goback": {
                            "text": "<%= GoToBackText %>",
                            "url":  "<%= GoToBack %>"
                        },
                    },
                    "callbackUrl": "<%= SPUrl %>/_layouts/<%= SPVersion %>Onlyoffice/CallbackHandler.ashx?data=<%= HttpUtility.UrlEncode(urlDocTrack)%>"
                },
                "height": "100%",
                "type": "desktop",
                "width": "100%"
            });

        </script>
   </asp:panel>
</asp:Content>