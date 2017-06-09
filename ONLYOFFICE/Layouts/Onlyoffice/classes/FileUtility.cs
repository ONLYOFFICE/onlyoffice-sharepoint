using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;

namespace Onlyoffice
{
    public static class FileUtility
    {        
        public static readonly List<string> CanEditTypes = new List<string>
            {
                "docx", "xlsx", "pptx", "ppsx"
            };

        public static readonly List<string> CanViewTypes = new List<string>
            {
                "docx", "xlsx", "pptx", "ppsx",
                "txt", "csv", "odt", "ods","odp",
                "doc", "xls", "ppt", "pps","epub",
                "rtf", "mht", "html", "htm","xps","pdf","djvu"
            };

        public static readonly List<string> TextDoc = new List<string>
            {
                "docx", "txt", "odt", "doc", "rtf", "html",
                "htm", "xps", "pdf", "djvu"
            };

        public static readonly List<string> PresentationDoc = new List<string>
            {
                "pptx", "ppsx", "odp", "ppt", "pps"
            };

        public static readonly List<string> SpreadsheetDoc = new List<string>
            {
                "xlsx", "csv", "ods", "xls"
            };

        public static readonly List<string> ConvDoc = new List<string>
            {
                "epub"
            };

        public static string GetDocType(string extension)
        {
            if (TextDoc.Contains(extension))
            {
                return "text";
            }
            else if (PresentationDoc.Contains(extension))
            {
                return "presentation";
            }
            else if (SpreadsheetDoc.Contains(extension))
            {
                return "spreadsheet";
            }
            else if (ConvDoc.Contains(extension))
            {
                return "conv";
            }
            else 
            {
                return "";
            }
        }
    }
}
