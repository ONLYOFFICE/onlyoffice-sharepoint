﻿/*
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

namespace Onlyoffice
{
    public class Payload
    {
        public Payload() 
        {
            this.SPListItemId = string.Empty;
            this.Folder = string.Empty;
            this.SPListURLDir = string.Empty;
            this.action = string.Empty;
            this.userId = 0;
        }

        public Payload(string action, string SPListItemId, string Folder, string SPListURLDir, int userId = 0)
        {
            this.SPListItemId = SPListItemId;
            this.Folder = Folder;
            this.SPListURLDir = SPListURLDir;
            this.action = action;
            this.userId = userId;
         }

        public string SPListItemId { get; set; }
        public string Folder { get; set; }
        public string SPListURLDir { get; set; }
        public string action { get; set; }
        public int userId { get; set; }
    }
}
