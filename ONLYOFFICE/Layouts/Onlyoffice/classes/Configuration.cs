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

using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Onlyoffice
{
    public enum EditorType
    {
        Desktop,
        Mobile,
    }

    public enum EditorMode
    {
        View,
        Edit,
    }

    [DataContract(Name = "editorConfig", Namespace = "")]
    public class Configuration
    {
        private string _documentType;

        public Configuration()
        {
            Document = new DocumentConfig();
            EditorConfig = new EditorConfig();
            EditorType = EditorType.Desktop;

            _documentType = FileUtility.TypeUnknown;
        }

        [DataMember(Name = "document")]
        public DocumentConfig Document { get; set; }

        [DataMember(Name = "editorConfig")]
        public EditorConfig EditorConfig { get; set; }

        [DataMember(Name="documentType")]
        public string DocumentType 
        { 
            get
            {
                if (!string.IsNullOrEmpty(Document.FileType))
                    return FileUtility.GetDocType(Document.FileType);

                return _documentType;
            }
            set 
            {
                _documentType = value;
            }
        }

        public EditorType EditorType { get; set; }

        [DataMember(Name = "type")]
        public string Type
        { 
            get
            {
                return EditorType.ToString().ToLower();
            }
            set { }
        }

        [DataMember(Name = "token", EmitDefaultValue = false)]
        public string Token { get; set; }

        public static string Serialize(Configuration configuration)
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(Configuration));
                serializer.WriteObject(ms, configuration);
                ms.Seek(0, SeekOrigin.Begin);
                return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }
    }

    [DataContract(Name = "document", Namespace = "")]
    public class DocumentConfig
    {
        public DocumentConfig()
        {
            Info = new InfoConfig();
            Permissions = new PermissionsConfig();
        }

        [DataMember(Name = "fileType")]
        public string FileType { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "info")]
        public InfoConfig Info { get; set; }

        [DataMember(Name = "permissions")]
        public PermissionsConfig Permissions { get; set; }
    }

    [DataContract(Name = "editorConfig", Namespace = "")]
    public class EditorConfig
    {
        public EditorConfig()
        {
            User = new UserConfig();
            Customization = new CustomizationConfig();
        }

        [DataMember(Name = "lang")]
        public string Lang { get; set; }

        public EditorMode EditorMode { get; set; }

        [DataMember(Name = "mode")]
        public string Mode 
        { 
            get
            {
                return EditorMode.ToString().ToLower();
            }
            set { }
        }

        [DataMember(Name = "user")]
        public UserConfig User { get; set; }

        [DataMember(Name = "customization")]
        public CustomizationConfig Customization { get; set; }

        [DataMember(Name = "callbackUrl")]
        public string CallbackUrl { get; set; }

    }

    [DataContract(Name = "info", Namespace = "")]
    public class InfoConfig
    {
        [DataMember(Name = "author")]
        public string Author { get; set; }

        [DataMember(Name = "created")]
        public string Created { get; set; }

        [DataMember(Name = "folder")]
        public string Folder { get; set; }

        [DataMember(Name = "owner")]
        public string Owner { get; set; }

        [DataMember(Name = "uploaded")]
        public string Uploaded { get; set; }
    }

    [DataContract(Name = "permissions", Namespace = "")]
    public class PermissionsConfig
    {
        private Dictionary<string, bool> permissions;

        public PermissionsConfig() 
        {
            permissions = new Dictionary<string, bool>();
        }

        [DataMember(Name = "edit")]
        public bool Edit 
        { 
            get 
            {
                if (permissions.ContainsKey("edit"))
                    return permissions["edit"];

                return false;
            }
            set 
            {
                permissions["edit"] = value;
            } 
        }
    }

    [DataContract(Name = "user", Namespace = "")]
    public class UserConfig
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract(Name = "customization", Namespace = "")]
    public class CustomizationConfig
    {
        public CustomizationConfig()
        {
            GoBack = new GoBackConfig();
        }

        [DataMember(Name = "goback")]
        public GoBackConfig GoBack { get; set; }
    }

    [DataContract(Name = "goback", Namespace = "")]
    public class GoBackConfig
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

    }
}
