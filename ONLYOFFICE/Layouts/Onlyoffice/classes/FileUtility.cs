/*
 *
 * (c) Copyright Ascensio System SIA 2024
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

using System.Collections.Generic;

namespace Onlyoffice
{
    public static class FileUtility
    {
        public const string TypeText = "word";
        public const string TypePresentation = "slide";
        public const string TypeSpreadsheet = "cell";
        public const string TypeUnknown = "";

        public static readonly List<string> CanEditTypes = new List<string>
            {
                "docx", "docxf", "oform", "xlsx", "pptx", "ppsx"
            };

        public static readonly List<string> CanViewTypes = new List<string>
            {
                "docx", "docxf", "oform", "xlsx", "pptx", "ppsx",
                "txt", "csv", "odt", "ods", "odp",
                "doc", "xls", "ppt", "pps", "epub",
                "rtf", "mht", "html", "htm", "xps", "pdf", "djvu"
            };

        public static readonly List<string> TextDoc = new List<string>
            {
                "docx", "docxf", "oform", "txt", "odt", "doc", "rtf", "html",
                "htm", "xps", "pdf", "djvu", "epub", "mht"
            };

        public static readonly List<string> PresentationDoc = new List<string>
            {
                "pptx", "ppsx", "odp", "ppt", "pps"
            };

        public static readonly List<string> SpreadsheetDoc = new List<string>
            {
                "xlsx", "csv", "ods", "xls"
            };

        public static string GetDocType(string extension)
        {
            if (TextDoc.Contains(extension))
            {
                return TypeText;
            }
            else if (PresentationDoc.Contains(extension))
            {
                return TypePresentation;
            }
            else if (SpreadsheetDoc.Contains(extension))
            {
                return TypeSpreadsheet;
            }
            else 
            {
                return TypeUnknown;
            }
        }
    }
}
