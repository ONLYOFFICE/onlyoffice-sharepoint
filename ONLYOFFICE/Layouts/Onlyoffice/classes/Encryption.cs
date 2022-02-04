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
        public static string GetUrlHash(string action, string Secret, string SPListItemId, string Folder, string SPListURLDir)
        {
            Payload payload = new Payload();
            payload.action = action;
            payload.SPListItemId = SPListItemId;
            payload.Folder = Folder;
            payload.SPListURLDir = SPListURLDir;

            var serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(payload);

            var hash = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(str + Secret)));

            var payloadStr = hash + "?" + str;

            payloadStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadStr));

            return payloadStr;
        }

        public static bool Decode(string data, int secret, Dictionary<string, object> validData)
        {
            string  hash = string.Empty,
                    payload = string.Empty,
                    currentHash = string.Empty;

            data = HttpUtility.UrlDecode(data).Replace(" ", "+");
            var plainTextBytes = Convert.FromBase64String(data);
            data = Encoding.UTF8.GetString(plainTextBytes);

            try
            {
                var dataParts = data.Split('?');

                hash = dataParts[0];
                payload = dataParts[1];

                var info = new JavaScriptSerializer().Deserialize<Payload>(payload);

                validData["SPListItemId"] = info.SPListItemId;
                validData["Folder"] = info.Folder;
                validData["SPListURLDir"] = info.SPListURLDir;
                validData["action"] = info.action;
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
