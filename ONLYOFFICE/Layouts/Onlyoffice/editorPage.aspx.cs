using System;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Onlyoffice;

namespace Onlyoffice.Layouts
{
    public partial class editorPage : LayoutsPageBase
    {
        protected string Key = "YQXK78GQD4FF", //random for initialization
                         FileName = "",
                         FileType = "",
                         FileAuthor = "",
                         FileTimeCreated = "",
                         FileEditorMode = "view",
                         urlDocDownload = "",
                         documentType = "",
                         urlDocTrack = "",
                         GoToBack = "",
                         GoToBackText = "",
                         lang = "",
                         CurrentUserName = "",
                         CurrentUserLogin = "",
                         SPListItemId, SPListURLDir, SPSource, SPListId, Folder, Secret,
                         DocumentSeverHost = "@http://localhost",
                         host = HttpContext.Current.Request.Url.Host,
                         SPUrl = HttpUtility.HtmlEncode(HttpContext.Current.Request.Url.Scheme) + "://" + HttpContext.Current.Request.Url.Authority,

                         SPVersion = SPFarm.Local.BuildVersion.Major == 14 ? "": "15/";

        protected int CurrentUserId = 0;

        protected bool canEdit = false;

        SPUser currentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            SPListItemId = Request["SPListItemId"];
            SPListURLDir = Request["SPListURLDir"];
            SPListId = Request["SPListId"];
            SPSource = Request["SPSource"];

            SPUserToken userToken;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(SPUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        //check secret key
//==================================================================================
                        if (web.Properties["SharePointSecret"] == null)
                        {
                            var rnd = new Random();
                            var spSecret = "";
                            for (var i = 0; i < 6; i++ )
                            {
                                spSecret = spSecret + rnd.Next(1, 9).ToString();
                            }
                            web.AllowUnsafeUpdates = true;
                            web.Update();
                            web.Properties.Add("SharePointSecret", spSecret);
                            web.Properties.Update();
                            web.AllowUnsafeUpdates = true; 
                            web.Update();
                        }
                        Secret = web.Properties["SharePointSecret"];

                        //read settings
//==================================================================================
                        if (web.Properties["DocumentServerHost"] != null)
                        {
                            DocumentSeverHost = web.Properties["DocumentServerHost"];
                        }
                        DocumentSeverHost += DocumentSeverHost.EndsWith("/") ? "" : "/";

                        // get current user ID and Name
//==================================================================================
                        userToken = web.AllUsers[0].UserToken;
                        SPSite s = new SPSite(SPUrl, userToken);
                        
                        var currentUserName = User.Identity.Name.Substring(User.Identity.Name.LastIndexOf("\\") + 1); 
                        var users = web.AllUsers;

                        for (var i=0; i< users.Count; i++)
                        {
                            var userNameOfList = users[i].LoginName.Substring(users[i].LoginName.LastIndexOf("\\") + 1);
                            if (userNameOfList == currentUserName)
                            {
                                currentUser = users[i];
                                CurrentUserId = users[i].ID;
                                CurrentUserName = users[i].Name;
                                break;
                            }
                        }

                        //get language
//==================================================================================

                        var lcid = (int)web.Language;
                        var defaultCulture = new CultureInfo(lcid);
                        lang = defaultCulture.IetfLanguageTag;

                        GoToBackText = LoadResource("GoToBack");                       


                        //get user/group roles
//==================================================================================
                        canEdit = CheckForEditing(SPUrl, SPListURLDir, currentUser);

                        //generate key and get file info for DocEditor 
//==================================================================================               
                        try
                        {
                            SPWeb w = s.OpenWeb();
                            //SPRoleAssignmentCollection ss = w.RoleAssignments;
                            SPList list = w.GetList(SPListURLDir);
                            SPListItem item = list.GetItemById(Int32.Parse(SPListItemId));

                            SPFile file = item.File;

                            //SPBasePermissions bp =SPContext.Current.Web.GetUserEffectivePermissions(SPContext.Current.Web.CurrentUser.LoginName);

                            if (file != null)
                            {
                                Key = file.ETag;
                                Key = GenerateRevisionId(Key);

                                Folder = Path.GetDirectoryName(file.ServerRelativeUrl);
                                Folder = Folder.Replace("\\", "/");
                                GoToBack = SPUrl + Folder;

                                FileAuthor = file.Author.Name;

                                var tzi = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.Id);
                                FileTimeCreated = TimeZoneInfo.ConvertTimeFromUtc(file.TimeCreated, tzi).ToString();

                                FileName = file.Name;

                                var tmp = FileName.Split('.');
                                FileType = tmp[tmp.Length - 1];

                                //check document format
                                try
                                {
                                    if (FileUtility.CanViewTypes.Contains(FileType))
                                    {
                                        var canEditType = FileUtility.CanEditTypes.Contains(FileType);
                                        canEdit = canEdit & canEditType;
                                        FileEditorMode = canEdit == true ? "edit" : FileEditorMode;
                                        //documentType = FileUtility.docTypes[FileType];   DocType.GetDocType(FileName)   
                                        documentType = FileUtility.GetDocType(FileType);
                                    }
                                    else
                                    {
                                        Response.Redirect(SPUrl);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    //if a error - redirect to home page
                                    Log.LogError(ex.Message);
                                    Response.Redirect(SPUrl);
                                }
                            }
                            else
                            {
                                Response.Redirect(SPUrl);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.LogError(ex.Message);
                            Response.Redirect(SPUrl + "/_layouts/" + SPVersion + "error.aspx");
                        }
                    }
                }
            });

            //generate url hash 
//==================================================================================  
            urlDocDownload = Encryption.GetUrlHash(SPListItemId, Folder, SPListURLDir, "download", Secret);
            urlDocTrack    = Encryption.GetUrlHash(SPListItemId, Folder, SPListURLDir, "track", Secret);
        }

        /// <summary>
        /// Translation key to a supported form.
        /// </summary>
        /// <param name="expectedKey">Expected key</param>
        /// <returns>Supported key</returns>
        public static string GenerateRevisionId(string expectedKey)
        {
            expectedKey = expectedKey ?? "";
            const int maxLength = 20;
            if (expectedKey.Length > maxLength) expectedKey = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(expectedKey)));
            var key = Regex.Replace(expectedKey, "[^0-9a-zA-Z_]", "_");
            return key.Substring(key.Length - Math.Min(key.Length, maxLength));
        }

        private string LoadResource(string _resName)
        {
            return Microsoft.SharePoint.Utilities.SPUtility.GetLocalizedString("$Resources:Resource," + _resName,
                "core", (uint)SPContext.Current.Web.UICulture.LCID);           
        }

        public static bool CheckForEditing(string SPUrl, string SPListURLDir, SPUser currentUser)
        {
            var canEdit = false;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(SPUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList docLibrary = web.GetList(SPListURLDir);
                        try
                        {
                            SPRoleAssignment userRoles = docLibrary.RoleAssignments.GetAssignmentByPrincipal(currentUser);
                            canEdit = CheckRolesForEditing(userRoles);
                        }
                        catch (Exception)
                        {
                            SPGroupCollection groupColl = web.Groups;

                            foreach (SPGroup group in groupColl)
                            {
                                try
                                {
                                    SPRoleAssignment groupsRoles = docLibrary.RoleAssignments.GetAssignmentByPrincipal(group);
                                    canEdit = CheckRolesForEditing(groupsRoles);
                                    if (canEdit) break;
                                }
                                catch (Exception) { }
                            }
                        }
                    }
                }
            });
            return canEdit;
        }

        public static bool CheckRolesForEditing(SPRoleAssignment Roles)
        {
            foreach (SPRoleDefinition role in Roles.RoleDefinitionBindings)
            {
                if (role.Type.ToString() == "Editor" // in SP10 SPRoleType.Editor does not exist
                    || role.Type == SPRoleType.Administrator
                    || role.Type == SPRoleType.Contributor
                    || role.Type == SPRoleType.WebDesigner)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
