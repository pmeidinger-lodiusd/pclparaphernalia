﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class provides the Status Readback 'save report' function.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    static class ToolStatusReadbackReport
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate the PDL Data report.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void generate (ReportCore.eRptFileFmt rptFileFmt,
                                     TextBox txtReply,
                                     ref string saveFilename)
        {
            object stream = null;
            object writer = null;

            bool OK = false;

            string saveFolder = null,
                   fileExt;

            ToolCommonFunctions.getFolderName (saveFilename,
                                               ref saveFolder);

            if (rptFileFmt == ReportCore.eRptFileFmt.html)
                fileExt = "html";
            else if (rptFileFmt == ReportCore.eRptFileFmt.xml)
                fileExt = "xml";
            else
                fileExt = "txt";

            saveFilename = saveFolder + "\\SR_Resp." + fileExt;

            OK = ReportCore.docOpen (rptFileFmt,
                                     ref saveFilename,
                                     ref stream,
                                     ref writer);
            if (OK)
            {
                ReportCore.docInitialise (rptFileFmt, writer, false, true,
                                          0, null,
                                          null, null);

                ReportCore.hddrTitle (writer, rptFileFmt, false,
                                      "*** Status Readback response data ***");

                reportBody (rptFileFmt, writer, txtReply);

                ReportCore.docFinalise (rptFileFmt, writer);

                ReportCore.docClose (rptFileFmt, stream, writer);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t B o d y                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write details of response to report file.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportBody(
            ReportCore.eRptFileFmt rptFileFmt,
            object writer,
            TextBox txtReply)
        {
            const int maxLineLen = 127;

            int ct;

            ReportCore.lineBlockOpen (writer, rptFileFmt);

            ct = txtReply.LineCount;

            for (int i = 0; i < ct; i++)
            {
                string line = txtReply.GetLineText(i);

                string removedCC = line.Replace ("\r\n", string.Empty)    // not "<CR><LF>")
                                       .Replace ("\n",   string.Empty)    // not "<LF>")
                                       .Replace ("\r",   string.Empty)    // not "<CR>")
                                       .Replace ("\f", "<FF>")
                                       .Replace ("\x1b", "<Esc>");

                ReportCore.lineItem (writer, rptFileFmt, removedCC, maxLineLen,
                                     false);
            }

            ReportCore.lineBlockClose (writer, rptFileFmt);
        }
    }
}
