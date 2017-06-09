using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Onlyoffice
{
    public static class MimeMapping
    {
        private static readonly Hashtable extensionToMimeMappingTable = new Hashtable(200, StringComparer.CurrentCultureIgnoreCase);

        static MimeMapping()
        {
            AddMimeMapping(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            AddMimeMapping(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            AddMimeMapping(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            AddMimeMapping(".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow");

            AddMimeMapping(".txt", "text/plain");
            AddMimeMapping(".csv", "text/csv");
            AddMimeMapping(".odt", "application/vnd.oasis.opendocument.text");
            AddMimeMapping(".ods", "application/vnd.oasis.opendocument.spreadsheet");
            AddMimeMapping(".odp", "application/vnd.oasis.opendocument.presentation");

            AddMimeMapping(".doc", "application/msword");
            AddMimeMapping(".xls",  "application/vnd.ms-excel");
            AddMimeMapping(".ppt",  "application/vnd.ms-powerpoint");
            AddMimeMapping(".pps",  "application/vnd.ms-powerpoint");
            AddMimeMapping(".epub",  "application/epub+zip");

            AddMimeMapping(".rtf",  "text/rtf");
            AddMimeMapping(".mht",  "message/rfc822");
            AddMimeMapping(".html",  "text/html");
            AddMimeMapping(".htm",  "text/html");
            AddMimeMapping(".xps",  "application/vnd.ms-xpsdocument");

            AddMimeMapping(".pdf",  "application/pdf");
            AddMimeMapping(".djvu",  "image/vnd.djvu");
        }

        private static void AddMimeMapping(string extension, string MimeType)
        {
            extensionToMimeMappingTable.Add(extension, MimeType);
        }

        public static string GetMimeMapping(string fileName)
        {
            string str = null;
            var startIndex = fileName.LastIndexOf('.');
            if (0 <= startIndex && fileName.LastIndexOf('\\') < startIndex)
            {
                str = (string)extensionToMimeMappingTable[fileName.Substring(startIndex)];
            }
            if (str == null)
            {
                str = (string)extensionToMimeMappingTable[".*"];
            }
            return str;
        }
    }
}
