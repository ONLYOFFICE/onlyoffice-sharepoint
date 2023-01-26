﻿/*
 *
 * (c) Copyright Ascensio System SIA 2023
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
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;

namespace Onlyoffice.Layouts
{
    public partial class Settings : LayoutsPageBase
    {
        private AppConfig appConfig;

        protected string path = AppDomain.CurrentDomain.BaseDirectory;
        protected string url = HttpUtility.HtmlEncode(HttpContext.Current.Request.Url.Scheme) + "://" + HttpContext.Current.Request.Url.Authority +
                                                                                                            HttpContext.Current.Request.RawUrl.Substring(0, HttpContext.Current.Request.RawUrl.IndexOf("_layouts"));
        protected void Page_Load(object sender, EventArgs e)
        {
            DocumentServerTitle.Text = SPUtility.GetLocalizedString("$Resources:Resource,DocumentServer", "core", (uint)SPContext.Current.Web.UICulture.LCID);
            JwtSecretTitle.Text = SPUtility.GetLocalizedString("$Resources:Resource,JwtSecret", "core", (uint)SPContext.Current.Web.UICulture.LCID);
            JwtHeaderTitle.Text = SPUtility.GetLocalizedString("$Resources:Resource,JwtHeader", "core", (uint)SPContext.Current.Web.UICulture.LCID);
            SaveSettings.Text = SPUtility.GetLocalizedString("$Resources:Resource,Save", "core", (uint)SPContext.Current.Web.UICulture.LCID);
            using (SPSite site = new SPSite(url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    appConfig = new AppConfig(web);

                    var isAdmin = web.UserIsSiteAdmin;
                    if (isAdmin == true)
                    {
                        if (IsPostBack == false)
                        {
                            try
                            {
                                DocumentServerHost.Text = appConfig.GetDocumentServerHost();
                                JwtSecret.Text = appConfig.GetJwtSecret();
                                JwtHeader.Text = appConfig.GetJwtHeader();
                            }
                            catch (Exception ex) 
                            {
                                Log.LogError(ex.Message);
                            }
                        }
                    }
                    else
                    {
                        Response.Redirect(url);
                    }
                }
            }
        }
        protected void Save_Click(object sender, System.EventArgs e)
        {
            String DSHost = DocumentServerHost.Text;
            String JWTSecret = JwtSecret.Text;
            String JWTHeader = JwtHeader.Text;

            using (SPSite site = new SPSite(url))
            using (SPWeb web = site.OpenWeb())
            {
                appConfig = new AppConfig(web);

                var isAdmin = web.UserIsSiteAdmin;
                if (isAdmin == true)
                {
                    try
                    {
                        appConfig.SetDocumentServerHost(DSHost);
                        appConfig.SetJwtSecret(JWTSecret);
                        appConfig.SetJwtHeader(JWTHeader);

                        Message.Text = SPUtility.GetLocalizedString("$Resources:Resource,SuccessfulSave", "core", (uint)SPContext.Current.Web.UICulture.LCID);
                    }
                    catch(Exception ex)
                    {
                        Log.LogError(ex.Message);
                        Message.Text = ex.Message;
                    }
                }
            } 
        }
    }
}
