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
