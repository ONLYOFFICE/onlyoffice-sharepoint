using System;
using System.Web.Script.Serialization;
using System.Web;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;

namespace Onlyoffice.Layouts
{
    public partial class Settings : LayoutsPageBase
    {
        protected string path = AppDomain.CurrentDomain.BaseDirectory;
        protected string url = HttpUtility.HtmlEncode(HttpContext.Current.Request.Url.Scheme) + "://" + HttpContext.Current.Request.Url.Authority;

        protected void Page_Load(object sender, EventArgs e)
        {
            DocumentServerTitle.Text = Microsoft.SharePoint.Utilities.SPUtility.GetLocalizedString("$Resources:Resource,DocumentServer", "core", (uint)SPContext.Current.Web.UICulture.LCID);
            SaveSettings.Text = Microsoft.SharePoint.Utilities.SPUtility.GetLocalizedString("$Resources:Resource,Save", "core", (uint)SPContext.Current.Web.UICulture.LCID);
            using (SPSite site = new SPSite(url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    var isAdmin = web.UserIsSiteAdmin;
                    if (isAdmin == true)
                    {
                        if (IsPostBack == false)
                        {
                            try
                            {
                                if (web.Properties["DocumentServerHost"] != null)
                                {
                                    DocumentServerHost.Text = web.Properties["DocumentServerHost"];
                                }
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

            using (SPSite site = new SPSite(url))
            using (SPWeb web = site.OpenWeb())
            {
                var isAdmin = web.UserIsSiteAdmin;
                if (isAdmin == true)
                {
                    try
                    {
                        if (web.Properties["DocumentServerHost"] == null)
                        {
                            web.Properties.Add("DocumentServerHost", DSHost);
                        }
                        else
                        {
                            web.Properties["DocumentServerHost"] = DSHost;
                        }
                        web.Properties.Update();
                        Message.Text = Microsoft.SharePoint.Utilities.SPUtility.GetLocalizedString("$Resources:Resource,SuccessfulSave", "core", (uint)SPContext.Current.Web.UICulture.LCID);
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
