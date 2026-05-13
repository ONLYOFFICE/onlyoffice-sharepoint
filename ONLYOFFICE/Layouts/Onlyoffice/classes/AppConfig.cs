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
using System;
using System.Globalization;

namespace Onlyoffice
{
    public class AppConfig
    {
        private SPWeb _web;

        private const string _documentServerHost = "DocumentServerHost";

        private const string _sharePointSecret = "SharePointSecret";

        private const string _jwtSecret = "JwtSecret";

        private const string _jwtHeader = "JwtHeader";

        private const string _demoStartDate = "DocsDemoStartDate";

        private const string _demoEnabled = "DocsDemoEnabled";

        public AppConfig(SPWeb Web) 
        {
            _web = Web;
        }

        public bool UseDemo()
        {
            return GetDemoEnabled() && DemoAvailable();
        }

        public bool DemoAvailable()
        {
            try
            {
                var startDate = GetDemoStartDate();

                if (startDate == null)
                    return true;

                var endDate = startDate.Value.AddDays(DocsDemo.Trial);

                return endDate > DateTime.Now;
            } catch (Exception ex) 
            {
                Log.LogError("DocsDemo check error: " + ex.Message);
                return false;
            }
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

        public void SetJwtHeader(string value)
        {
            if (_web.Properties[_jwtHeader] == null)
                _web.Properties.Add(_jwtHeader, value);
            else
                _web.Properties[_jwtHeader] = value;

            _web.Properties.Update();
        }

        public string GetJwtHeader()
        {
            var value = _web.Properties[_jwtHeader];

            if (value == null)
                return "AuthorizationJWT";

            return value;
        }

        public void SetDemoStartDate(DateTime value)
        {
            var dateString = value.ToString("O");

            if (_web.Properties[_demoStartDate] == null)
                _web.Properties.Add(_demoStartDate, dateString);
            else
                _web.Properties[_demoStartDate] = dateString;

            _web.Properties.Update();
        }

        public Nullable<DateTime> GetDemoStartDate()
        {
            var value = _web.Properties[_demoStartDate];

            if (value == null)
                return null;

            return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind); ;
        }

        public void SetDemoEnabled(bool value)
        {
            if (value && GetDemoStartDate() == null)
                SetDemoStartDate(DateTime.Now);

            var enabled = DemoAvailable() ? value.ToString() : "False";

            if (_web.Properties[_demoEnabled] == null)
                _web.Properties.Add(_demoEnabled, enabled);
            else
                _web.Properties[_demoEnabled] = enabled;

            _web.Properties.Update();
        }

        public bool GetDemoEnabled()
        {
            var value = _web.Properties[_demoEnabled];

            if (value == null)
                return false;

            return value.ToLower() == "true";
        }
    }

    public class DocsDemo
    {
        public static string Host = "https://onlinedocs.docs.onlyoffice.com/";
        public static string Header = "AuthorizationJWT";
        public static string Secret = "sn2puSUF7muF5Jas";
        public static int Trial = 30;
    }
}
