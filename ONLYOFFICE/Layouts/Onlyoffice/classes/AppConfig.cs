/*
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
using Microsoft.SharePoint;

namespace Onlyoffice
{
    public class AppConfig
    {
        private SPWeb _web;

        private const string _documentServerHost = "DocumentServerHost";

        private const string _sharePointSecret = "SharePointSecret";

        private const string _jwtSecret = "JwtSecret";

        private const string _jwtHeader = "JwtHeader";

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
    }
}
