/*
 *
 * (c) Copyright Ascensio System Limited 2010-2017
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using System.Web.Script.Serialization;
using System.IO;

namespace Onlyoffice
{
    class Encryption
    {
        public static string GetUrlHash(string SPListItemId, string folder, string SPListURLDir, string action, string Secret)
        {
            var serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(new Payload(SPListItemId, folder, SPListURLDir, action));

            var hash = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(str + Secret)));

            var payload = hash + "?" + str;

            payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));

            return payload;
        }

        public static bool Decode(string data, ref string SPListItemId, ref string Folder, ref string SPListDir, ref string action, int secret)
        {
            string  hash = string.Empty,
                    payload = string.Empty,
                    currentHash = string.Empty;

            data = HttpUtility.UrlDecode(data);
            var plainTextBytes = Convert.FromBase64String(data);
            data = Encoding.UTF8.GetString(plainTextBytes);

            try
            {
                var dataParts = data.Split('?');

                hash = dataParts[0];
                payload = dataParts[1];

                var info = new JavaScriptSerializer().Deserialize<Payload>(payload);

                SPListItemId = info.SPListItemId;
                Folder = info.Folder;
                SPListDir = info.SPListDir;
                action = info.action;
            }
            catch (Exception ex) 
            {
                Log.LogError(ex.Message);
            }

            currentHash = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(payload + secret)));

            return hash == currentHash;
        }
    }
}
