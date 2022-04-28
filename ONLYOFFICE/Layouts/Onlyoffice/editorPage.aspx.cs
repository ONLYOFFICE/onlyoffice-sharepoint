/*
 *
 * (c) Copyright Ascensio System SIA 2022
 *
 * The MIT License (MIT)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
*/

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
        private string urlHashDownload = string.Empty,
                       urlHashTrack = string.Empty,
                       CurrentUserLogin = string.Empty,
                       SPListItemId = string.Empty,
                       SPListURLDir = string.Empty,
                       SPSourceAction = string.Empty,
                       Secret = string.Empty,
                       JwtSecret = string.Empty,
                       Host = HttpUtility.HtmlEncode(HttpContext.Current.Request.Url.Scheme) + "://" + HttpContext.Current.Request.Url.Authority,
                       SubSite = HttpContext.Current.Request.RawUrl.Substring(0, HttpContext.Current.Request.RawUrl.IndexOf("_layouts"));

        private int CurrentUserId = 0;
        private bool canEdit = false;
        private Configuration Configuration = new Configuration();
        private SPUser currentUser;

        protected string DocumentSeverHost = "@http://localhost",
                         Folder = string.Empty,
                         SPVersion = SPFarm.Local.BuildVersion.Major == 14 ? "" : "15/",
                         ConfigurationJSON = string.Empty,
                         SPUrl = HttpUtility.HtmlEncode(HttpContext.Current.Request.Url.Scheme) + "://" + HttpContext.Current.Request.Url.Authority +
                                                                                                            HttpContext.Current.Request.RawUrl.Substring(0, HttpContext.Current.Request.RawUrl.IndexOf("_layouts"));

        protected void Page_Load(object sender, EventArgs e)
        {
            SPListItemId = Request["SPListItemId"];
            SPListURLDir = Request["SPListURLDir"];
            SPSourceAction = Request["SPSourceAction"];

            if (SPSourceAction == "Ribbon")
            {
                SPListURLDir = SubSite + SPListURLDir;
            }

            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(SPUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        //read settings
//==================================================================================
                        if (web.Properties["DocumentServerHost"] != null)
                        {
                            DocumentSeverHost = web.Properties["DocumentServerHost"];
                        }
                        DocumentSeverHost += DocumentSeverHost.EndsWith("/") ? "" : "/";

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

                        if (web.Properties["JwtSecret"] != null)
                            JwtSecret = web.Properties["JwtSecret"];

                        // get current user ID and Name
//==================================================================================
                        CurrentUserLogin = User.Identity.Name.Substring(User.Identity.Name.LastIndexOf("\\") + 1);
                        for (var i = 0; i < web.AllUsers.Count; i++) 
                        {
                            if (string.Compare(web.AllUsers[i].LoginName.Substring(web.AllUsers[i].LoginName.LastIndexOf("\\") + 1),
                                                CurrentUserLogin, StringComparison.CurrentCultureIgnoreCase) == 0)
                            {
                                currentUser = web.AllUsers[i];
                                break;
                            }
                        }

                        if (currentUser != null)
                        {
                            Configuration.EditorConfig.User.Id = CurrentUserId = currentUser.ID;
                            Configuration.EditorConfig.User.Name = currentUser.Name;
                        }

                        //get language
//==================================================================================
                        var lcid = (int)web.Language;
                        var defaultCulture = new CultureInfo(lcid);
                        Configuration.EditorConfig.Lang = defaultCulture.IetfLanguageTag;                     

                        //generate key and get file info for DocEditor 
//==================================================================================               
                        try
                        {
                            SPSite s = new SPSite(SPUrl, currentUser.UserToken);

                            SPWeb w = s.OpenWeb();
                            var list = w.GetList(SPListURLDir);

                            SPListItem item = list.GetItemById(Int32.Parse(SPListItemId));
                            SPFile file = item.File;
                            if (file == null)
                                Response.Redirect(SPUrl);

                            canEdit = item.DoesUserHavePermissions(currentUser, SPBasePermissions.EditListItems);

                            Configuration.Document.Key = GenerateRevisionId(file.ETag);

                            Folder = Path.GetDirectoryName(file.ServerRelativeUrl);
                            Folder = Folder.Replace("\\", "/");

                            Configuration.Document.Info.Folder = Folder;
                            Configuration.EditorConfig.Customization.GoBack.Text = LoadResource("GoToBack");
                            Configuration.EditorConfig.Customization.GoBack.Url = Host + Folder;

                            Configuration.Document.Info.Author = file.Author.Name;
                            Configuration.Document.Info.Owner = file.Author.Name;

                            var tzi = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.Id);
                            Configuration.Document.Info.Created = Configuration.Document.Info.Uploaded = TimeZoneInfo.ConvertTimeFromUtc(file.TimeCreated, tzi).ToString();

                            Configuration.Document.Title = file.Name;

                            var tmp = Configuration.Document.Title.Split('.');
                            Configuration.Document.FileType = tmp[tmp.Length - 1];

                            //check document format
                            if (string.IsNullOrEmpty(Configuration.DocumentType))
                                Response.Redirect(SPUrl);

                            if (FileUtility.CanViewTypes.Contains(Configuration.Document.FileType))
                            {
                                var canEditType = FileUtility.CanEditTypes.Contains(Configuration.Document.FileType);
                                canEdit = canEdit & canEditType;
                                Configuration.EditorConfig.EditorMode = canEdit ? EditorMode.Edit : EditorMode.View;
                            }
                            else
                            {
                                Response.Redirect(SPUrl);
                            }

                            Configuration.Document.Permissions.Edit = canEdit;
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
            urlHashDownload = Encryption.GetUrlHash("download", Secret, SPListItemId, Folder, SPListURLDir, CurrentUserId);
            urlHashTrack = Encryption.GetUrlHash("track", Secret, SPListItemId, Folder, SPListURLDir);

            Configuration.Document.Url = string.Format("{0}_layouts/{1}Onlyoffice/CallbackHandler.ashx?data={2}", SPUrl, SPVersion, urlHashDownload);
            Configuration.EditorConfig.CallbackUrl = string.Format("{0}_layouts/{1}Onlyoffice/CallbackHandler.ashx?data={2}", SPUrl, SPVersion, urlHashTrack);

            if (!string.IsNullOrEmpty(JwtSecret))
                Configuration.Token = Encryption.GetSignature(JwtSecret, Configuration);

            ConfigurationJSON = Configuration.Serialize(Configuration);
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
    }
}
