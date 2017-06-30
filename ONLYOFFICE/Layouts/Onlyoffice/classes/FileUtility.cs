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
