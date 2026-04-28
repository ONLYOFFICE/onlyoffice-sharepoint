/*
 *
 * (c) Copyright Ascensio System SIA 2026
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

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace Onlyoffice
{
    public class EditorHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string action = context.Request["action"];
            string SPUrl = HttpUtility.HtmlEncode(HttpContext.Current.Request.Url.Scheme) + "://" + HttpContext.Current.Request.Url.Authority +
                                                                                                            HttpContext.Current.Request.RawUrl.Substring(0, HttpContext.Current.Request.RawUrl.IndexOf("_layouts"));

            context.Response.AddHeader("Content-Type", "application/json");

            switch (action)
            {
                case "saveas":
                    SaveAs(SPUrl, context);
                    break;
                case "history":
                    GetHistory(SPUrl, context);
                    break;
                case "version":
                    GetVersion(SPUrl, context);
                    break;

                default:
                    context.Response.Write("{\"error\": \"Action is not supported\"}");
                    break;
            }
        }

        static void SaveAs(string SPUrl, HttpContext context)
        {
            bool canCreate = false;
            bool success = false;

            SPUserToken userToken = null;

            string bodyStr;
            using (StreamReader reader = new StreamReader(context.Request.InputStream))
                bodyStr = reader.ReadToEnd();

            var body = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(bodyStr);

            string fileType = (string)body["fileType"];
            string title = (string)body["title"];
            string url = (string)body["url"];
            string folder = (string)body["folder"];

            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(SPUrl))
                using (SPWeb web = site.OpenWeb())
                {
                    var userName = context.User.Identity.Name.Substring(context.User.Identity.Name.LastIndexOf("\\") + 1);
                    for (var i = 0; i < web.AllUsers.Count; i++)
                    {
                        if (string.Compare(web.AllUsers[i].LoginName.Substring(web.AllUsers[i].LoginName.LastIndexOf("\\") + 1),
                                            userName, StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            userToken = web.AllUsers[i].UserToken;
                            break;
                        }
                    }

                    SPSite s = new SPSite(SPUrl, userToken);
                    SPWeb w = s.OpenWeb();

                    SPFolder SPFolder = w.GetFolder(folder);

                    if (SPFolder.Item != null)
                    {
                        canCreate = SPFolder.Item.DoesUserHavePermissionsForUI(SPBasePermissions.AddListItems);
                    }
                    else
                    {
                        canCreate = SPFolder.DocumentLibrary.DoesUserHavePermissionsForUI(SPBasePermissions.AddListItems);
                    }

                    if (!canCreate)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.Write("{\"message\": \"Saveas is forbidden\"}");
                        return;
                    }

                    byte[] fileDataArr = null;
                    using (var wc = new WebClient())
                        fileDataArr = wc.DownloadData(url);

                    try
                    {
                        w.AllowUnsafeUpdates = true;
                        w.Update();

                        SPFolder.Files.Add(title, fileDataArr, false);
                        SPFolder.Update();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Log.LogError(ex.Message);
                        context.Response.Write("{\"message\": \"Saveas failed\"}");
                    }
                    finally
                    {
                        w.AllowUnsafeUpdates = false;
                        w.Update();
                    }
                }
            });

            if(success)
                context.Response.Write("{\"message\": \"Saveas is completed\"}");
        }

        static void GetHistory(string SPUrl, HttpContext context)
        {
            string SPListItemId = context.Request.Params["SPListItemId"];
            string SPListURLDir = context.Request.Params["SPListURLDir"];

            SPUserToken userToken = null;
            string result = null;

            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(SPUrl))
                using (SPWeb web = site.OpenWeb())
                {
                    var userName = context.User.Identity.Name.Substring(context.User.Identity.Name.LastIndexOf("\\") + 1);
                    for (var i = 0; i < web.AllUsers.Count; i++)
                    {
                        if (string.Compare(web.AllUsers[i].LoginName.Substring(web.AllUsers[i].LoginName.LastIndexOf("\\") + 1),
                                            userName, StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            userToken = web.AllUsers[i].UserToken;
                            break;
                        }
                    }

                    SPSite s = new SPSite(SPUrl, userToken);
                    SPWeb w = s.OpenWeb();

                    var list = w.GetList(SPListURLDir);
                    SPListItem item = list.GetItemById(Int32.Parse(SPListItemId));
                    SPFile file = item.File;

                    var historyList = new List<Dictionary<string, object>>();
                    int versionNum = 1;

                    foreach (SPFileVersion version in file.Versions)
                    {
                        historyList.Add(new Dictionary<string, object>
                        {
                            { "version", versionNum++ },
                            { "key", FileUtility.GenerateRevisionId(file.UniqueId, version.Created) },
                            { "created", version.Created.ToString("yyyy-MM-dd HH:mm:ss") },
                            { "user", new Dictionary<string, object>
                                {
                                    { "id", version.CreatedBy.ID },
                                    { "name", version.CreatedBy.Name }
                                }
                            }
                        });
                    }

                    historyList.Add(new Dictionary<string, object>
                    {
                        { "version", versionNum },
                        { "key", FileUtility.GenerateRevisionId(file.UniqueId, file.TimeLastModified) },
                        { "created", file.TimeLastModified.ToString("yyyy-MM-dd HH:mm:ss") },
                        { "user", new Dictionary<string, object>
                            {
                                { "id", file.ModifiedBy.ID },
                                { "name", file.ModifiedBy.Name }
                            }
                        }
                    });

                    var response = new Dictionary<string, object>
                    {
                        { "currentVersion", versionNum },
                        { "history", historyList }
                    };

                    result = new JavaScriptSerializer().Serialize(response);
                }
            });

            context.Response.Write(result ?? "{\"error\": \"Failed to get history\"}");
        }

        static void GetVersion(string SPUrl, HttpContext context)
        {
            string SPListItemId = context.Request.Params["SPListItemId"];
            string SPListURLDir = context.Request.Params["SPListURLDir"];
            int version = Int32.Parse(context.Request.Params["version"]);
            string SPVersion = SPFarm.Local.BuildVersion.Major == 14 ? "" : "15/";

            SPUserToken userToken = null;
            string result = null;

            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(SPUrl))
                using (SPWeb web = site.OpenWeb())
                {
                    AppConfig appConfig = new AppConfig(web);
                    string secret = appConfig.GetSharePointSecret();
                    string JwtSecret = appConfig.UseDemo() ? DocsDemo.Secret : appConfig.GetJwtSecret();

                    var userName = context.User.Identity.Name.Substring(context.User.Identity.Name.LastIndexOf("\\") + 1);
                    int userId = 0;
                    for (var i = 0; i < web.AllUsers.Count; i++)
                    {
                        if (string.Compare(web.AllUsers[i].LoginName.Substring(web.AllUsers[i].LoginName.LastIndexOf("\\") + 1),
                                            userName, StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            userToken = web.AllUsers[i].UserToken;
                            userId = web.AllUsers[i].ID;
                            break;
                        }
                    }

                    SPSite s = new SPSite(SPUrl, userToken);
                    SPWeb w = s.OpenWeb();

                    var list = w.GetList(SPListURLDir);
                    SPListItem item = list.GetItemById(Int32.Parse(SPListItemId));
                    SPFile file = item.File;

                    string fileType = file.Name.Split('.')[file.Name.Split('.').Length - 1].ToLower();
                    string folder = Path.GetDirectoryName(file.ServerRelativeUrl).Replace("\\", "/");
                    int totalVersions = file.Versions.Count + 1;

                    string url, key;
                    int? versionNumber = null;
                    if (version < totalVersions)
                    {
                        SPFileVersion ver = file.Versions[version - 1];
                        versionNumber = version;
                        key = FileUtility.GenerateRevisionId(file.UniqueId, ver.Created);
                    }
                    else
                    {
                        key = FileUtility.GenerateRevisionId(file.UniqueId, file.TimeLastModified);
                    }

                    string urlHash = Encryption.GetUrlHash("download", secret, SPListItemId, folder, SPListURLDir, userId, versionNumber);
                    url = string.Format("{0}_layouts/{1}Onlyoffice/CallbackHandler.ashx?data={2}", SPUrl, SPVersion, urlHash);

                    var response = new Dictionary<string, object>
                    {
                        { "version", version },
                        { "url", url },
                        { "fileType", fileType },
                        { "key", key }
                    };

                    if (version > 1)
                    {
                        SPFileVersion prevVer = file.Versions[version - 2];
                        string prevUrlHash = Encryption.GetUrlHash("download", secret, SPListItemId, folder, SPListURLDir, userId, version - 1);
                        response["previous"] = new Dictionary<string, object>
                        {
                            { "url", string.Format("{0}_layouts/{1}Onlyoffice/CallbackHandler.ashx?data={2}", SPUrl, SPVersion, prevUrlHash) },
                            { "fileType", fileType },
                            { "key", FileUtility.GenerateRevisionId(file.UniqueId, prevVer.Created) }
                        };
                    }

                    if (!string.IsNullOrEmpty(JwtSecret))
                        response["token"] = Encryption.GetSignature(JwtSecret, response);

                    result = new JavaScriptSerializer().Serialize(response);
                }
            });

            context.Response.Write(result ?? "{\"error\": \"Failed to get version\"}");
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
