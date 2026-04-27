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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Onlyoffice
{
    public class FileFormat
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("actions")]
        public List<string> Actions { get; set; }

        [JsonProperty("convert")]
        public List<string> Convert { get; set; }

        [JsonProperty("mime")]
        public List<string> Mime { get; set; }
    }

    public static class FileUtility
    {
        public const string TypeDocument = "word";
        public const string TypePresentation = "slide";
        public const string TypeSpreadsheet = "cell";
        public const string TypePdf = "pdf";
        public const string TypeDiagram = "diagram";
        public const string TypeUnknown = "";

        public const string ActionView = "view";
        public const string ActionAutoConvert = "auto-convert";
        public const string ActionEdit = "edit";
        public const string ActionReview = "review";
        public const string ActionComment = "comment";
        public const string ActionEncrypt = "encrypt";
        public const string ActionLossyEdit = "lossy-edit";
        public const string ActionCustomfilter = "customfilter";
        public const string ActionFill = "fill";

        private static List<FileFormat> _formats;
        private static readonly object _lock = new object();

        public static IReadOnlyList<FileFormat> Formats
        {
            get
            {
                if (_formats == null)
                {
                    lock (_lock)
                    {
                        if (_formats == null)
                        {
                            LoadFormats();
                        }
                    }
                }
                return _formats;
            }
        }

        private static void LoadFormats()
        {
            try
            {
                var assembly = typeof(FileUtility).Assembly;

                const string resourceName = "ONLYOFFICE.Layouts.Onlyoffice.formats.onlyoffice-docs-formats.json";

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        Log.LogError("Embedded JSON resource not found: " + resourceName);
                        _formats = new List<FileFormat>();
                        return;
                    }

                    using (var reader = new StreamReader(stream))
                    {
                        string json = reader.ReadToEnd();

                        _formats = JsonConvert.DeserializeObject<List<FileFormat>>(json)
                                   ?? new List<FileFormat>();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError($"Failed to load formats: {ex.Message}");
            }
        }

        public static FileFormat GetFormat(string ext)
        {
            return Formats.FirstOrDefault(f => f.Name == ext.TrimStart('.'));
        }

        public static string GenerateRevisionId(Guid uniqueId, DateTime lastModified)
        {
            var input = $"{uniqueId:N}_{lastModified.Ticks}";

            byte[] hash;

            using (var sha = SHA256.Create())
            {
                hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            }

            var base64 = Convert.ToBase64String(hash)
                .Replace("+", "_")
                .Replace("/", "_")
                .Replace("=", "");

            return base64.Substring(0, 20);
        }
    }
}
