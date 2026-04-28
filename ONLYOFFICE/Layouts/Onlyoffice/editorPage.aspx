<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="editorPage.aspx.cs" Inherits="Onlyoffice.Layouts.editorPage" MasterPageFile="default.master" %>

<asp:Content id="Content1" ContentPlaceholderID="MainContent" runat="server" style="height: 100%; margin: 0;">
    <asp:panel runat="server" id="panelMain" style="height: 100%; margin: 0;">
        <div id="placeholder" style="height: 100%"></div>

        <script type="text/javascript" src= "<%= ApiUrl %>"></script>
        <script type="text/javascript">
            var config = <%= ConfigurationJSON %>;

            config["events"] = {
                "onRequestSaveAs": onRequestSaveAs,
                "onRequestHistory": onRequestHistory,
                "onRequestHistoryData": onRequestHistoryData
            }

            if ("<%= UsingDemoMessage %>") {
                config.events.onAppReady = () => {
                    window.docEditor.showMessage("<%= UsingDemoMessage %>");
                }
            }

            window.docEditor = new DocsAPI.DocEditor("placeholder", config);

            function onRequestSaveAs (event) {

                let data = {
                    fileType: event.data.fileType,
                    title: event.data.title,
                    url: event.data.url,
                    folder: "<%= Folder %>"
                };

                fetch("<%= SPUrl %>/_layouts/<%= SPVersion %>Onlyoffice/EditorHandler.ashx?action=saveas", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json;charset=utf-8"
                    },
                    body: JSON.stringify(data)
                })
                    .then(response => response.json())
                    .then(json => {
                        console.info(json.message);
                    })
                    .catch(e => {
                        console.error("SaveAs Error: ", e);
                    });
            }

            function onRequestHistoryData(event) {
                var params = new URLSearchParams({
                    SPListItemId: "<%= SPListItemId %>",
                    SPListURLDir: "<%= SPListURLDir %>",
                    version: event.data
                });

                fetch("<%= SPUrl %>/_layouts/<%= SPVersion %>Onlyoffice/EditorHandler.ashx?action=version&" + params)
                    .then(response => response.json())
                    .then(json => {
                        docEditor.setHistoryData(json);
                    })
                    .catch(e => {
                        console.error("HistoryData Error: ", e);
                    });
            }

            function onRequestHistory() {
                var params = new URLSearchParams({
                    SPListItemId: "<%= SPListItemId %>",
                    SPListURLDir: "<%= SPListURLDir %>"
                });

                fetch("<%= SPUrl %>/_layouts/<%= SPVersion %>Onlyoffice/EditorHandler.ashx?action=history&" + params)
                    .then(response => response.json())
                    .then(json => {
                        docEditor.refreshHistory(json);
                    })
                    .catch(e => {
                        console.error("History Error: ", e);
                    });
            }

        </script>
   </asp:panel>
</asp:Content>