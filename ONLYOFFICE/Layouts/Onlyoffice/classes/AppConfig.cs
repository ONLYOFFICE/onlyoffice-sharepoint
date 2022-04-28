using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace Onlyoffice
{
    public class AppConfig
    {
        private SPWeb _web;

        private const string _documentServerHost = "DocumentServerHost";

        private const string _sharePointSecret = "SharePointSecret";

        private const string _jwtSecret = "JwtSecret";

        public AppConfig(SPWeb Web) 
        {
            _web = Web;
        }

        public string GetSharePointSecret()
        {
            var value = _web.Properties[_sharePointSecret];

            if(value == null)
            {
                var rnd = new Random();
                var spSecret = string.Empty;

                for (var i = 0; i < 6; i++)
                {
                    spSecret = spSecret + rnd.Next(1, 9).ToString();
                }

                _web.AllowUnsafeUpdates = true;
                _web.Update();
                _web.Properties.Add(_sharePointSecret, spSecret);
                _web.Properties.Update();
                _web.AllowUnsafeUpdates = true;
                _web.Update();

                value = _web.Properties[_sharePointSecret];
            }

            return value;
        }

        public void SetDocumentServerHost(string value)
        {
            value += value.EndsWith("/") ? "" : "/";

            if (_web.Properties[_documentServerHost] == null)
                _web.Properties.Add(_documentServerHost, value);
            else
                _web.Properties[_documentServerHost] = value;

            _web.Properties.Update();
        }

        public string GetDocumentServerHost()
        {
            var value = _web.Properties[_documentServerHost];

            if (value == null)
                return string.Empty;

            return value;
        }

        public void SetJwtSecret(string value)
        {
            if (_web.Properties[_jwtSecret] == null)
                _web.Properties.Add(_jwtSecret, value);
            else
                _web.Properties[_jwtSecret] = value;

            _web.Properties.Update();
        }

        public string GetJwtSecret()
        {
            var value = _web.Properties[_jwtSecret];

            if (value == null)
                return null;

            return value;
        }
    }
}
