﻿using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class provides the Status Readback 'save report' function.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class ToolStatusReadbackReport
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

        public static void Generate(ReportCore.RptFileFmt rptFileFmt, TextBox txtReply, ref string saveFilename)
        {
            object stream = null;
            object writer = null;
            string fileExt;

            var saveFolder = System.IO.Path.GetDirectoryName(saveFilename);

            if (rptFileFmt == ReportCore.RptFileFmt.html)
                fileExt = "html";
            else if (rptFileFmt == ReportCore.RptFileFmt.xml)
                fileExt = "xml";
            else
                fileExt = "txt";

            saveFilename = saveFolder + "\\SR_Resp." + fileExt;

            bool OK = ReportCore.DocOpen(rptFileFmt,
                         ref saveFilename,
                         ref stream,
                         ref writer);
            if (OK)
            {
                ReportCore.DocInitialise(rptFileFmt, writer, false, true,
                                          0, null,
                                          null, null);

                ReportCore.HddrTitle(writer, rptFileFmt, false, "*** Status Readback response data ***");

                ReportBody(rptFileFmt, writer, txtReply);

                ReportCore.DocFinalise(rptFileFmt, writer);

                ReportCore.DocClose(rptFileFmt, stream, writer);
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

        private static void ReportBody(ReportCore.RptFileFmt rptFileFmt, object writer, TextBox txtReply)
        {
            const int maxLineLen = 127;

            int ct;

            ReportCore.LineBlockOpen(writer, rptFileFmt);

            ct = txtReply.LineCount;

            for (int i = 0; i < ct; i++)
            {
                string line = txtReply.GetLineText(i);

                string removedCC = line.Replace("\r\n", string.Empty)    // not "<CR><LF>")
                                       .Replace("\n", string.Empty)    // not "<LF>")
                                       .Replace("\r", string.Empty)    // not "<CR>")
                                       .Replace("\f", "<FF>")
                                       .Replace("\x1b", "<Esc>");

                ReportCore.LineItem(writer, rptFileFmt, removedCC, maxLineLen, false);
            }

            ReportCore.LineBlockClose(writer, rptFileFmt);
        }
    }
}