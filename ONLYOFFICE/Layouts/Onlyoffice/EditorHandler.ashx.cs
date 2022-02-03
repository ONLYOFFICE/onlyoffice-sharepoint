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

using Microsoft.SharePoint;
using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Text;
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

                default:
                    context.Response.Write("{\"error\": \"Action is not supported\"}");
                    break;
            }
        }

        static void SaveAs(string SPUrl, HttpContext context)
        {
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
                    var userName = context.User.Identity.Name;
                    var userToken = web.GetUserToken(userName);

                    SPSite s = new SPSite(SPUrl, userToken);

                    SPWeb w = s.OpenWeb();

                    w.AllowUnsafeUpdates = true;
                    w.Update();

                    SPFolder SPFolder = w.GetFolder(folder);

                    byte[] fileDataArr = null;
                    using (var wc = new WebClient())
                        fileDataArr = wc.DownloadData(url);

                    SPFolder.Files.Add(title, fileDataArr, false);
                    SPFolder.Update();

                    w.AllowUnsafeUpdates = false;
                    w.Update();
                }
            });

            context.Response.Write("{\"message\": \"Saveas is completed\"}");
        }
        public bool IsReusable
        {
            get { return false; }
        }
    }
}
