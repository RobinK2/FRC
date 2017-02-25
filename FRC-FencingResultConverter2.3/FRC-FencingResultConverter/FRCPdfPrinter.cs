﻿#region PDFsharp - A .NET library for processing PDF
//
// Copyright (c) 2005-2012 empira Software GmbH, Troisdorf (Germany)
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace FRC_FencingResultConverter
{
    public class FRCPdfPrinter
    {
        //The interpreter that holds info from the xml file.
        private FRCXmlInterpreter interpreter;
        private string title;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">The title of a file.</param>
        /// <param name="interpreter">The xml interpreter.</param>
        public FRCPdfPrinter(string title, FRCXmlInterpreter interpreter)
        {
            this.title = title;
            this.interpreter = interpreter;
        }

        /// <summary>
        /// Uses correct printer class to print the pdf document.
        /// </summary>
        /// <returns></returns>
        public PdfDocument printResults()
        {
            if (interpreter.Formula.IsIndiviualCompetiton)
            {
                FRCPdfPrinterIND indPrinter = new FRCPdfPrinterIND(title, interpreter);
                return indPrinter.printResults();
            }
            else
            {
                FRCPdfPrinterTEAM teamPrinter = new FRCPdfPrinterTEAM(title, interpreter);
                return teamPrinter.printResults();
            }
        }
         
    }
}
