﻿using System;
using System.Data;
using System.Reflection;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides the Prn Analyse 'save report' function.
    /// 
    /// © Chris Hutchinson 2010-2017
    /// 
    /// </summary>

    static class ToolPrnAnalyseReport
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const int _maxSizeNameTag = 15;
        const int _colSpanNone = -1;

        const bool _flagNone = false;
        const bool _flagBlankBefore = true;

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
        // Generate the report.                                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void generate(
            ToolPrnAnalyse.eInfoType indxInfoType,
            ReportCore.eRptFileFmt rptFileFmt,
            DataTable table,
            string prnFilename,
            long fileSize,
            bool flagOffsetHex,
            PrnParseOptions options)
        {
            object stream = null;
            object writer = null;

            bool OK = false;

            int reportSize;

            string fileExt;
            string saveFilename = null;

            if (rptFileFmt == ReportCore.eRptFileFmt.html)
                fileExt = "html";
            else if (rptFileFmt == ReportCore.eRptFileFmt.xml)
                fileExt = "xml";
            else
                fileExt = "txt";

            if (indxInfoType == ToolPrnAnalyse.eInfoType.Analysis)
            {
                saveFilename = prnFilename + "_analysis." + fileExt;

                OK = ReportCore.DocOpen(rptFileFmt,
                                         ref saveFilename,
                                         ref stream,
                                         ref writer);

                if (OK)
                {
                    int ctClrMapRowTypes = PrnParseRowTypes.GetCount();

                    bool useClr = options.FlagClrMapUseClr;

                    reportSize = table.Rows.Count;

                    if (useClr)
                    {
                        string[] rowClasses = new string[ctClrMapRowTypes];
                        string[] rowClrBack = new string[ctClrMapRowTypes];
                        string[] rowClrFore = new string[ctClrMapRowTypes];

                        getRowColourStyleData(options,
                                               ref rowClasses,
                                               ref rowClrBack,
                                               ref rowClrFore);

                        ReportCore.DocInitialise(rptFileFmt, writer, true, false,
                                                  ctClrMapRowTypes, rowClasses,
                                                  rowClrBack, rowClrFore);
                    }
                    else
                    {
                        ReportCore.DocInitialise(rptFileFmt, writer, true, false,
                                                  0, null,
                                                  null, null);
                    }

                    reportHeader(indxInfoType, rptFileFmt, writer,
                                  prnFilename, fileSize, reportSize);

                    reportBodyAnalysis(rptFileFmt, writer,
                                        table, flagOffsetHex);

                    ReportCore.DocFinalise(rptFileFmt, writer);

                    ReportCore.DocClose(rptFileFmt, stream, writer);
                }
            }
            else if (indxInfoType == ToolPrnAnalyse.eInfoType.Content)
            {
                saveFilename = prnFilename + "_content." + fileExt;

                OK = ReportCore.DocOpen(rptFileFmt,
                                         ref saveFilename,
                                         ref stream,
                                         ref writer);
                if (OK)
                {
                    reportSize = table.Rows.Count;

                    ReportCore.DocInitialise(rptFileFmt, writer, true, false,
                                              0, null,
                                              null, null);

                    reportHeader(indxInfoType, rptFileFmt, writer,
                                  prnFilename, fileSize, reportSize);

                    reportBodyContent(rptFileFmt, writer,
                                       table, flagOffsetHex);

                    ReportCore.DocFinalise(rptFileFmt, writer);

                    ReportCore.DocClose(rptFileFmt, stream, writer);
                }
            }
            else if (indxInfoType == ToolPrnAnalyse.eInfoType.Statistics)
            {
                saveFilename = prnFilename + "_statistics." + fileExt;

                OK = ReportCore.DocOpen(rptFileFmt,
                                         ref saveFilename,
                                         ref stream,
                                         ref writer);
                if (OK)
                {
                    reportSize = table.Rows.Count;

                    ReportCore.DocInitialise(rptFileFmt, writer, true, false,
                                              0, null,
                                              null, null);

                    reportHeader(indxInfoType, rptFileFmt, writer,
                                  prnFilename, fileSize, reportSize);

                    reportBodyStatistics(rptFileFmt, writer,
                                          table);

                    ReportCore.DocFinalise(rptFileFmt, writer);

                    ReportCore.DocClose(rptFileFmt, stream, writer);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t R o w C o l o u r S t y l e D a t a                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get references to colour style data for colour coded analysis.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void getRowColourStyleData(
            PrnParseOptions options,
            ref string[] classes,
            ref string[] clrBack,
            ref string[] clrFore)
        {
            int indxClrBack;
            int indxClrFore;

            PropertyInfo[] stdClrsPropertyInfo = null;

            bool flagClrMapUseClr = false;

            PropertyInfo pInfoBack,
                         pInfoFore;

            int ctClrMapRowTypes = PrnParseRowTypes.GetCount();
            int ctClrMapStdClrs = 0;

            int[] indxClrMapBack = new int[ctClrMapRowTypes];
            int[] indxClrMapFore = new int[ctClrMapRowTypes];

            options.GetOptClrMap(ref flagClrMapUseClr,
                                  ref indxClrMapBack,
                                  ref indxClrMapFore);

            options.GetOptClrMapStdClrs(ref ctClrMapStdClrs,
                                         ref stdClrsPropertyInfo);

            //----------------------------------------------------------------//

            for (int i = 0; i < ctClrMapRowTypes; i++)
            {
                string rowType =
                    Enum.GetName(typeof(PrnParseRowTypes.eType), i);

                indxClrBack = indxClrMapBack[i];
                indxClrFore = indxClrMapFore[i];

                pInfoBack = stdClrsPropertyInfo[indxClrBack];
                pInfoFore = stdClrsPropertyInfo[indxClrFore];

                classes[i] = rowType;
                clrBack[i] = pInfoBack.Name;
                clrFore[i] = pInfoFore.Name;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t B o d y A n a l y s i s                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write details of Analysis to report file.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportBodyAnalysis(
            ReportCore.eRptFileFmt rptFileFmt,
            object writer,
            DataTable table,
            bool flagOffsetHex)
        {
            const int colCt = 4;

            const string c0Name = PrnParseConstants.cRptA_colName_Offset;
            const string c1Name = PrnParseConstants.cRptA_colName_Type;
            const string c2Name = PrnParseConstants.cRptA_colName_Seq;
            const string c3Name = PrnParseConstants.cRptA_colName_Desc;

            const int lc0 = PrnParseConstants.cRptA_colMax_Offset;
            const int lc1 = PrnParseConstants.cRptA_colMax_Type;
            const int lc2 = PrnParseConstants.cRptA_colMax_Seq;
            const int lc3 = PrnParseConstants.cRptA_colMax_Desc;

            const string rtName = PrnParseConstants.cRptA_colName_RowType;

            string c0Hddr;

            string[] colHddrs;
            string[] colNames;
            int[] colSizes;

            int ctItems;

            ctItems = table.Rows.Count;

            if (flagOffsetHex)
                c0Hddr = c0Name + ": hex";
            else
                c0Hddr = c0Name + ": dec";

            colHddrs = new string[colCt] { c0Hddr, c1Name, c2Name, c3Name };
            colNames = new string[colCt] { c0Name, c1Name, c2Name, c3Name };
            colSizes = new int[colCt] { lc0, lc1, lc2, lc3 };

            //----------------------------------------------------------------//
            //                                                                //
            // Open the table and Write the column header text.               //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.TableHddrData(writer, rptFileFmt, false,
                                  colCt, colHddrs, colSizes);

            //----------------------------------------------------------------//
            //                                                                //
            // Write the data rows.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < ctItems; i++)
            {
                DataRow row = table.Rows[i];

                int indxRowType = (int)row[rtName];

                string rowType = Enum.GetName
                                    (typeof(PrnParseRowTypes.eType),
                                     indxRowType);

                ReportCore.TableRowData(
                    writer, rptFileFmt,
                    ReportCore.eRptChkMarks.text,   // not used by this tool //
                    colCt, rowType,
                    row, colNames, colSizes);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write any required end tags.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.TableClose(writer, rptFileFmt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t B o d y C o n t e n t                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write details of Content to report file.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportBodyContent(
            ReportCore.eRptFileFmt rptFileFmt,
            object writer,
            DataTable table,
            bool flagOffsetHex)
        {
            const int colCt = 3;

            const string c0Name = PrnParseConstants.cRptC_colName_Offset;
            const string c1Name = PrnParseConstants.cRptC_colName_Hex;
            const string c2Name = PrnParseConstants.cRptC_colName_Text;

            const int lc0 = PrnParseConstants.cRptC_colMax_Offset;
            const int lc1 = PrnParseConstants.cRptC_colMax_Hex;
            const int lc2 = PrnParseConstants.cRptC_colMax_Text;

            string c0Hddr;

            string[] colHddrs;
            string[] colNames;
            int[] colSizes;

            int ctItems;

            ctItems = table.Rows.Count;

            if (flagOffsetHex)
                c0Hddr = c0Name + ": hex";
            else
                c0Hddr = c0Name + ": dec";

            colHddrs = new string[colCt] { c0Hddr, c1Name, c2Name };
            colNames = new string[colCt] { c0Name, c1Name, c2Name };
            colSizes = new int[colCt] { lc0, lc1, lc2 };

            ctItems = table.Rows.Count;

            if (flagOffsetHex)
                c0Hddr = c0Name + ": hex";
            else
                c0Hddr = c0Name + ": dec";

            //----------------------------------------------------------------//
            //                                                                //
            // Open the table and Write the column header text.               //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.TableHddrData(writer, rptFileFmt, false,
                                  colCt, colHddrs, colSizes);

            //----------------------------------------------------------------//
            //                                                                //
            // Write the data rows.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < ctItems; i++)
            {
                DataRow row = table.Rows[i];

                ReportCore.TableRowData(
                    writer, rptFileFmt,
                    ReportCore.eRptChkMarks.text,   // not used by this tool //
                    colCt, null,
                    row, colNames, colSizes);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write any required end tags.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.TableClose(writer, rptFileFmt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t B o d y S t a t i s t i c s                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write details of Statistics to report file.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportBodyStatistics(
            ReportCore.eRptFileFmt rptFileFmt,
            object writer,
            DataTable table)
        {
            const int colCt = 5;

            const string c0Name = PrnParseConstants.cRptS_colName_Seq;
            const string c1Name = PrnParseConstants.cRptS_colName_Desc;
            const string c2Name = PrnParseConstants.cRptS_colName_CtP;
            const string c3Name = PrnParseConstants.cRptS_colName_CtE;
            const string c4Name = PrnParseConstants.cRptS_colName_CtT;

            const int lc0 = PrnParseConstants.cRptS_colMax_Seq;
            const int lc1 = PrnParseConstants.cRptS_colMax_Desc;
            const int lc2 = PrnParseConstants.cRptS_colMax_CtP;
            const int lc3 = PrnParseConstants.cRptS_colMax_CtE;
            const int lc4 = PrnParseConstants.cRptS_colMax_CtT;

            string[] colNames;
            int[] colSizes;

            int ctItems;

            ctItems = table.Rows.Count;

            colNames = new string[colCt] { c0Name, c1Name, c2Name, c3Name, c4Name };
            colSizes = new int[colCt] { lc0, lc1, lc2, lc3, lc4 };

            //----------------------------------------------------------------//
            //                                                                //
            // Open the table and Write the column header text.               //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.TableHddrData(writer, rptFileFmt, false,
                                  colCt, colNames, colSizes);

            //----------------------------------------------------------------//
            //                                                                //
            // Write the data rows.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < ctItems; i++)
            {
                DataRow row = table.Rows[i];

                ReportCore.TableRowData(
                    writer, rptFileFmt,
                    ReportCore.eRptChkMarks.text,   // not used by this tool //
                    colCt, null,
                    row, colNames, colSizes);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write any required end tags.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.TableClose(writer, rptFileFmt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t H e a d e r                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report header.                                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportHeader(
            ToolPrnAnalyse.eInfoType indxInfoType,
            ReportCore.eRptFileFmt rptFileFmt,
            object writer,
            string prnFilename,
            long fileSize,
            int reportSize)
        {
            int maxLineLen = 0;

            string title = string.Empty;

            if (indxInfoType == ToolPrnAnalyse.eInfoType.Analysis)
            {
                title = "*** Prn Analysis ***";

                maxLineLen = PrnParseConstants.cRptA_colMax_Offset +
                             PrnParseConstants.cRptA_colMax_Type +
                             PrnParseConstants.cRptA_colMax_Seq +
                             PrnParseConstants.cRptA_colMax_Desc +
                             (PrnParseConstants.cColSeparatorLen * 3) - 12;
            }
            else if (indxInfoType == ToolPrnAnalyse.eInfoType.Content)
            {
                title = "*** Prn Content ***";

                maxLineLen = PrnParseConstants.cRptC_colMax_Offset +
                             PrnParseConstants.cRptC_colMax_Hex +
                             PrnParseConstants.cRptC_colMax_Text +
                             (PrnParseConstants.cColSeparatorLen * 2) - 12;
            }
            else if (indxInfoType == ToolPrnAnalyse.eInfoType.Statistics)
            {
                title = "*** Prn Analysis Statistics ***";

                maxLineLen = PrnParseConstants.cRptS_colMax_Seq +
                             PrnParseConstants.cRptS_colMax_Desc +
                             PrnParseConstants.cRptS_colMax_CtP +
                             PrnParseConstants.cRptS_colMax_CtE +
                             PrnParseConstants.cRptS_colMax_CtT +
                             (PrnParseConstants.cColSeparatorLen * 4) - 12;
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write out the title.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.HddrTitle(writer, rptFileFmt, false, title);

            //----------------------------------------------------------------//
            //                                                                //
            // Open the table and Write Write out the date, time, input file  //
            // identity and size, and count of report rows.                   //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.TableHddrPair(writer, rptFileFmt);

            ReportCore.TableRowPair(writer, rptFileFmt,
                                 "Date_time", DateTime.Now.ToString(),
                                 _colSpanNone, _colSpanNone,
                                 _maxSizeNameTag, maxLineLen,
                                 _flagNone, _flagNone, _flagNone);

            ReportCore.TableRowPair(writer, rptFileFmt,
                                 "Filename", prnFilename,
                                 _colSpanNone, _colSpanNone,
                                 _maxSizeNameTag, maxLineLen,
                                 _flagNone, _flagNone, _flagNone);

            ReportCore.TableRowPair(writer, rptFileFmt,
                                 "Filesize", fileSize.ToString() + " bytes",
                                 _colSpanNone, _colSpanNone,
                                 _maxSizeNameTag, maxLineLen,
                                 _flagNone, _flagNone, _flagNone);

            ReportCore.TableRowPair(writer, rptFileFmt,
                                 "Report_size", reportSize +
                                    " rows (excluding header and trailer lines)",
                                 _colSpanNone, _colSpanNone,
                                 _maxSizeNameTag, maxLineLen,
                                 _flagNone, _flagNone, _flagNone);

            ReportCore.TableClose(writer, rptFileFmt);
        }
    }
}
