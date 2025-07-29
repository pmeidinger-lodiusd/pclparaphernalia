using System;
using System.Data;
using System.Reflection;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides the Make Overlay 'save report' function.
/// 
/// © Chris Hutchinson 2012
/// 
/// </summary>

static class ToolMakeOverlayReport
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

    public static void Generate(
        ReportCore.eRptFileFmt rptFileFmt,
        DataTable table,
        string prnFilename,
        string ovlFilename,
        bool flagOffsetHex,
        PrnParseOptions options)
    {
        object stream = null;
        object writer = null;

        bool OK = false;

        string fileExt;
        string saveFilename = null;

        if (rptFileFmt == ReportCore.eRptFileFmt.html)
            fileExt = "html";
        else if (rptFileFmt == ReportCore.eRptFileFmt.xml)
            fileExt = "xml";
        else
            fileExt = "txt";

        saveFilename = ovlFilename + "_report." + fileExt;

        OK = ReportCore.DocOpen(rptFileFmt,
                                 ref saveFilename,
                                 ref stream,
                                 ref writer);

        if (OK)
        {
            int ctClrMapRowTypes = PrnParseRowTypes.GetCount();

            bool useClr = options.FlagClrMapUseClr;

            if (useClr)
            {
                string[] rowClasses = new string[ctClrMapRowTypes];
                string[] rowClrBack = new string[ctClrMapRowTypes];
                string[] rowClrFore = new string[ctClrMapRowTypes];

                GetRowColourStyleData(options,
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

            ReportHeader(rptFileFmt, writer,
                          prnFilename, ovlFilename);

            reportBody(rptFileFmt, writer,
                        table, flagOffsetHex);

            ReportCore.DocFinalise(rptFileFmt, writer);

            ReportCore.DocClose(rptFileFmt, stream, writer);
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

    private static void GetRowColourStyleData(
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
    // r e p o r t B o d y                                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write details of process to report file.                           //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void reportBody(ReportCore.eRptFileFmt rptFileFmt,
                                    object writer,
                                    DataTable table,
                                    bool flagOffsetHex)
    {
        const int colCt = 5;

        const string c0Name = PrnParseConstants.cRptA_colName_Action;
        const string c1Name = PrnParseConstants.cRptA_colName_Offset;
        const string c2Name = PrnParseConstants.cRptA_colName_Type;
        const string c3Name = PrnParseConstants.cRptA_colName_Seq;
        const string c4Name = PrnParseConstants.cRptA_colName_Desc;

        const int lc0 = PrnParseConstants.cRptA_colMax_Action;
        const int lc1 = PrnParseConstants.cRptA_colMax_Offset;
        const int lc2 = PrnParseConstants.cRptA_colMax_Type;
        const int lc3 = PrnParseConstants.cRptA_colMax_Seq;
        const int lc4 = PrnParseConstants.cRptA_colMax_Desc;

        const string rtName = PrnParseConstants.cRptA_colName_RowType;

        string c1Hddr;

        string[] colHddrs;
        string[] colNames;
        int[] colSizes;

        int ctItems;

        ctItems = table.Rows.Count;

        if (flagOffsetHex)
            c1Hddr = c1Name + ": hex";
        else
            c1Hddr = c1Name + ": dec";

        colHddrs = new string[colCt] { c0Name, c1Hddr, c2Name, c3Name, c4Name };
        colNames = new string[colCt] { c0Name, c1Name, c2Name, c3Name, c4Name };
        colSizes = new int[colCt] { lc0, lc1, lc2, lc3, lc4 };

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
    // r e p o r t H e a d e r                                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write report header.                                               //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void ReportHeader(
        ReportCore.eRptFileFmt rptFileFmt,
        object writer,
        string prnFilename,
        string ovlFilename)
    {
        int maxLineLen = 0;

        string title = string.Empty;

        title = "*** Make Overlay report ***:";

        maxLineLen = PrnParseConstants.cRptA_colMax_Action +
                     PrnParseConstants.cRptA_colMax_Offset +
                     PrnParseConstants.cRptA_colMax_Type +
                     PrnParseConstants.cRptA_colMax_Seq +
                     PrnParseConstants.cRptA_colMax_Desc +
                     (PrnParseConstants.cColSeparatorLen * 4) - 15;

        //----------------------------------------------------------------//
        //                                                                //
        // Write out the title.                                           //
        //                                                                //
        //----------------------------------------------------------------//

        ReportCore.HddrTitle(writer, rptFileFmt, false, title);

        //----------------------------------------------------------------//
        //                                                                //
        // Write out the date, time, input file identity, and             //
        // count of report rows.                                          //
        //                                                                //
        //----------------------------------------------------------------//

        ReportCore.TableHddrPair(writer, rptFileFmt);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "Date_time", DateTime.Now.ToString(),
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "Print_file", prnFilename,
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "Overlay_file", ovlFilename,
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableClose(writer, rptFileFmt);
    }
}