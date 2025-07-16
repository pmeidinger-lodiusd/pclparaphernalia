using System;
using System.Data;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides the Soft Font Generator 'save report' function.
/// 
/// © Chris Hutchinson 2012
/// 
/// </summary>

static class ToolSoftFontGenReport
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    const int _maxSizeNameTag = 22;
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

    public static void generate(ReportCore.eRptFileFmt rptFileFmt,
                                 ReportCore.eRptChkMarks rptChkMarks,
                                 DataTable tableDonor,
                                 DataTable tableMapping,
                                 DataTable tableTarget,
                                 DataTable tableChars,
                                 string fontNameTTF,
                                 string fontFilenameTTF,
                                 string fontFilenamePCL)
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

        saveFilename = fontFilenamePCL + "_report." + fileExt;

        OK = ReportCore.DocOpen(rptFileFmt,
                                 ref saveFilename,
                                 ref stream,
                                 ref writer);
        if (OK)
        {
            ReportCore.DocInitialise(rptFileFmt, writer, true, false,
                                      0, null,
                                      null, null);

            reportHddr(rptFileFmt, writer,
                        fontNameTTF, fontFilenameTTF, fontFilenamePCL);

            reportHddrSub(rptFileFmt, writer, "Donor font details");

            reportBodyStd(rptFileFmt, rptChkMarks, writer, tableDonor);

            reportHddrSub(rptFileFmt, writer, "Mapping details");

            reportBodyStd(rptFileFmt, rptChkMarks, writer, tableMapping);

            reportHddrSub(rptFileFmt, writer, "Target font details");

            reportBodyStd(rptFileFmt, rptChkMarks, writer, tableTarget);

            reportHddrSub(rptFileFmt, writer, "Generated character details");

            reportBodyChars(rptFileFmt, rptChkMarks, writer, tableChars);

            ReportCore.DocFinalise(rptFileFmt, writer);

            ReportCore.DocClose(rptFileFmt, stream, writer);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r e p o r t B o d y C h a r s                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write details of generated characters to report file.              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void reportBodyChars(
        ReportCore.eRptFileFmt rptFileFmt,
        ReportCore.eRptChkMarks rptChkMarks,
        object writer,
        DataTable table)
    {
        const int colCt = 13;

        const string c0Name = "DecCode";
        const string c1Name = "HexCode";
        const string c2Name = "Unicode";
        const string c3Name = "Glyph";
        const string c4Name = "Abs";
        const string c5Name = "Prev";
        const string c6Name = "Comp";
        const string c7Name = "Depth";
        const string c8Name = "Width";
        const string c9Name = "LSB";
        const string c10Name = "Height";
        const string c11Name = "TSB";
        const string c12Name = "Length";

        const string c0Hddr = "DecCode";
        const string c1Hddr = "HexCode";
        const string c2Hddr = "Unicode";
        const string c3Hddr = "Glyph";
        const string c4Hddr = "Abs?";
        const string c5Hddr = "Prev?";
        const string c6Hddr = "Comp?";
        const string c7Hddr = "Depth";
        const string c8Hddr = "Width";
        const string c9Hddr = "LSB";
        const string c10Hddr = "Height";
        const string c11Hddr = "TSB";
        const string c12Hddr = "Length";

        const int lc0 = 7;
        const int lc1 = 7;
        const int lc2 = 7;
        const int lc3 = 5;
        const int lc4 = 5;
        const int lc5 = 5;
        const int lc6 = 5;
        const int lc7 = 5;
        const int lc8 = 5;
        const int lc9 = 6;
        const int lc10 = 6;
        const int lc11 = 6;
        const int lc12 = 6;

        string[] colHddrs;
        string[] colNames;
        int[] colSizes;

        int ctItems;

        ctItems = table.Rows.Count;

        colHddrs = new string[colCt] { c0Hddr, c1Hddr, c2Hddr, c3Hddr,
                                       c4Hddr, c5Hddr, c6Hddr, c7Hddr,
                                       c8Hddr, c9Hddr, c10Hddr, c11Hddr,
                                       c12Hddr };
        colNames = new string[colCt] { c0Name, c1Name, c2Name, c3Name,
                                       c4Name, c5Name, c6Name, c7Name,
                                       c8Name, c9Name, c10Name, c11Name,
                                       c12Name };
        colSizes = new int[colCt] { lc0, lc1, lc2, lc3,
                                      lc4, lc5, lc6, lc7,
                                      lc8, lc9, lc10, lc11,
                                      lc12};

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

            ReportCore.TableRowData(writer, rptFileFmt, rptChkMarks,
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
    // r e p o r t B o d y S t d                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write details of specified two-column table to report file.        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void reportBodyStd(
        ReportCore.eRptFileFmt rptFileFmt,
        ReportCore.eRptChkMarks rptChkMarks,
        object writer,
        DataTable table)
    {
        const int colCt = 2;

        const string c0Name = "Name";
        const string c1Name = "Value";

        const int lc0 = 21;
        const int lc1 = 57;

        string[] colHddrs;
        string[] colNames;
        int[] colSizes;

        int ctItems;

        ctItems = table.Rows.Count;

        colHddrs = new string[colCt] { c0Name, c1Name };
        colNames = new string[colCt] { c0Name, c1Name };
        colSizes = new int[colCt] { lc0, lc1 };

        ctItems = table.Rows.Count;

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

            ReportCore.TableRowData(writer, rptFileFmt, rptChkMarks,
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
    // r e p o r t H d d r                                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write main report header.                                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void reportHddr(ReportCore.eRptFileFmt rptFileFmt,
                                    object writer,
                                    string fontNameTTF,
                                    string fontFilenameTTF,
                                    string fontFilenamePCL)
    {
        int maxLineLen = 80;

        string title = "*** Soft Font Generator ***";

        //----------------------------------------------------------------//
        //                                                                //
        // Write out the title.                                           //
        //                                                                //
        //----------------------------------------------------------------//

        ReportCore.HddrTitle(writer, rptFileFmt, false, title);

        //----------------------------------------------------------------//
        //                                                                //
        // Write out the date, time, input file identity and size, and    //
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
                             "Target_PCL_font_file", fontFilenamePCL,
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "Donor_TTF_name", fontNameTTF,
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "Donor_TTF_file", fontFilenameTTF,
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableClose(writer, rptFileFmt);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r e p o r t H d d r S u b                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write sub report header.                                           //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void reportHddrSub(ReportCore.eRptFileFmt rptFileFmt,
                                       object writer,
                                       string subHead)
    {
        //----------------------------------------------------------------//
        //                                                                //
        // Write out the sub-header.                                      //
        //                                                                //
        //----------------------------------------------------------------//

        ReportCore.HddrTitle(writer, rptFileFmt, true, subHead);
    }
}
