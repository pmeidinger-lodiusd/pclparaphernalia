using System;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides the Symbol Set Generator 'save report' function.
/// 
/// © Chris Hutchinson 2013
/// 
/// </summary>

static class ToolSymbolSetGenReport
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

    private const int cCodePointUnused = 65535;
    private const int cCodePointC1Min = 0x80;
    private const int cCodePointC1Max = 0x9f;

    const int lm0 = 21;
    const int lm1 = 57;

    const int lcDec = 5;
    const int lcHex = 4;
    const int lrDec = 5;
    const int lrHex = 4;

    const int lSep = 1;

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

    public static void Generate(ReportCore.eRptFileFmt rptFileFmt,
                                 string symSetFilename,
                                 ushort symSetNo,
                                 ushort[] symSetMap,
                                 ushort codeMin,
                                 ushort codeMax,
                                 ushort codeCt,
                                 ulong charCollReq,
                                 bool flagIgnoreC0,
                                 bool flagIgnoreC1,
                                 bool flagMapHex,
                                 PCLSymSetTypes.eIndex symSetType)
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

        saveFilename = symSetFilename + "_report." + fileExt;

        OK = ReportCore.DocOpen(rptFileFmt,
                                 ref saveFilename,
                                 ref stream,
                                 ref writer);

        if (OK)
        {
            ReportCore.DocInitialise(rptFileFmt, writer, true, false,
                                      0, null,
                                      null, null);

            ReportHddr(rptFileFmt, writer, symSetFilename);

            ReportBodyMain(rptFileFmt, writer, symSetNo,
                            codeMin, codeMax, codeCt, charCollReq,
                            flagIgnoreC0, flagIgnoreC1, flagMapHex,
                            symSetType);

            ReportBodyMap(rptFileFmt, writer, symSetMap,
                           codeMin, codeMax,
                           flagIgnoreC0, flagIgnoreC1, flagMapHex);

            ReportCore.DocFinalise(rptFileFmt, writer);

            ReportCore.DocClose(rptFileFmt, stream, writer);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r e p o r t B o d y M a i n                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write report header.                                               //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void ReportBodyMain(
        ReportCore.eRptFileFmt rptFileFmt,
        object writer,
    //  String symSetFilename,
        ushort symSetNo,
        ushort codeMin,
        ushort codeMax,
        ushort codeCt,
        ulong charCollReq,
        bool flagIgnoreC0,
        bool flagIgnoreC1,
        bool flagMapHex,
        PCLSymSetTypes.eIndex symSetType)
    {
        const int maxLineLen = 80;        // ***************** constant elsewhere ???????????????

        //----------------------------------------------------------------//
        //                                                                //
        // Write out the title.                                           //
        //                                                                //
        //----------------------------------------------------------------//

        ReportCore.HddrTitle(writer, rptFileFmt, true,
                              "Symbol set details:");

        //----------------------------------------------------------------//
        //                                                                //
        // Write out the symbol set basic details.                        //
        //                                                                //
        //----------------------------------------------------------------//

        ReportCore.TableHddrPair(writer, rptFileFmt);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "SymSetNo", symSetNo.ToString(),
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "SymSetId",
                             PCLSymbolSets.TranslateKind1ToId(symSetNo).ToString(),
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "IgnoreC0Codes",
                             (flagIgnoreC0 ? "true" : "false"),
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "IgnoreC1Codes",
                             (flagIgnoreC1 ? "true" : "false"),
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "FirstCode",
                             (flagMapHex ? "0x" + codeMin.ToString("x4")
                                         : codeMin.ToString()),
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "Lastcode",
                             (flagMapHex ? "0x" + codeMax.ToString("x4")
                                         : codeMax.ToString()),
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "CharCount",
                             (flagMapHex ? "0x" + codeCt.ToString("x4")
                                         : codeCt.ToString()),
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "CharReqBits",
                             "0x" + charCollReq.ToString("x16"),
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableClose(writer, rptFileFmt);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r e p o r t B o d y M a p                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write details of mapping to report file.                           //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void ReportBodyMap(
        ReportCore.eRptFileFmt rptFileFmt,
        object writer,
        ushort[] symSetMap,
        ushort codeMin,
        ushort codeMax,
        bool flagIgnoreC0,
        bool flagIgnoreC1,
        bool flagMapHex)
    {
        const int maxLineLen = 80;        // ***************** constant elsewhere ???????????????

        const int lcDec = 5;
        const int lcHex = 4;
        const int lrDec = 5;
        const int lrHex = 4;

        const int colCt = 17;

        int lcCol,
              lrHddr;

        string fmtHddr,
               fmtVal;

        int mapIndx,
              rowIndx;

        string[] colHddrs = new string[colCt];
        string[] colNames = new string[colCt];
        int[] colSizes = new int[colCt];

        int ctItems;

        ctItems = symSetMap.Length;

        //----------------------------------------------------------------//
        //                                                                //
        // Write out the header.                                          //
        //                                                                //
        //----------------------------------------------------------------//

        ReportCore.HddrTitle(writer, rptFileFmt, true,
                              "Mapping detail:");

        ReportCore.TableHddrPair(writer, rptFileFmt);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "Format",
                             (flagMapHex ? "hexadecimal" : "decimal"),
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableClose(writer, rptFileFmt);

        //----------------------------------------------------------------//
        //                                                                //
        // Open the table and write the column header text.               //
        //                                                                //
        //----------------------------------------------------------------//

        if (flagMapHex)
        {
            lcCol = lcHex;
            lrHddr = lrHex;

            fmtHddr = "x4";
            fmtVal = "x4";

            colSizes[0] = lrHex;
            colNames[0] = "row";
            colHddrs[0] = string.Empty;

            for (int i = 1; i < colCt; i++)
            {
                colSizes[i] = lcHex;
                colNames[i] = "col" + (i - 1).ToString("D2");
                colHddrs[i] = "_" + (i - 1).ToString("x");
            }
        }
        else
        {
            lcCol = lcDec;
            lrHddr = lrDec;

            fmtHddr = string.Empty;
            fmtVal = string.Empty;

            colSizes[0] = lrDec;
            colNames[0] = "row";
            colHddrs[0] = string.Empty;

            for (int i = 1; i < colCt; i++)
            {
                colSizes[i] = lcDec;
                colNames[i] = "col" + (i - 1).ToString("D2");
                colHddrs[i] = "+" + (i - 1).ToString("d");
            }
        }

        ReportCore.TableHddrData(writer, rptFileFmt, true,
                                  colCt, colHddrs, colSizes);

        //----------------------------------------------------------------//
        //                                                                //
        // Write the data rows.                                           //
        //                                                                //
        //----------------------------------------------------------------//

        int colCtData = colCt - 1;

        mapIndx = 0;
        rowIndx = codeMin / colCtData;

        for (int i = rowIndx; mapIndx < codeMax; i++)
        {
            string[] rowData = new string[colCt];

            rowIndx = (i * colCtData);

            if (flagMapHex)
            {
                rowData[0] = (rowIndx.ToString(fmtHddr).
                                Substring(0, 3) + "_").
                                PadLeft(lrHddr, ' ');
            }
            else
            {
                rowData[0] = rowIndx.ToString(fmtHddr).
                                PadLeft(lrHddr, ' ');
            }

            for (int j = 0; j < colCtData; j++)
            {
                string val;

                mapIndx = rowIndx + j;

                if ((mapIndx < codeMin) || (mapIndx > codeMax))
                {
                    val = " ".PadLeft(lcCol, ' ');
                }
                else if (flagIgnoreC1 &&
                         ((mapIndx >= cCodePointC1Min) &&
                          (mapIndx <= cCodePointC1Max)))
                {
                    val = cCodePointUnused.
                            ToString(fmtVal).PadLeft(lcCol, ' ');
                }
                else
                {
                    val = symSetMap[mapIndx].
                            ToString(fmtVal).PadLeft(lcCol, ' ');
                }

                rowData[j + 1] = val;
            }

            ReportCore.TableRowText(writer, rptFileFmt, colCt,
                                 rowData, colNames, colSizes);
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

    private static void ReportHddr(ReportCore.eRptFileFmt rptFileFmt,
                                    object writer,
                                    string symSetFilename)
    {
        int maxLineLen = 80;

        string title = "*** Symbol Set Generator ***";

        //----------------------------------------------------------------//
        //                                                                //
        // Write out the title.                                           //
        //                                                                //
        //----------------------------------------------------------------//

        ReportCore.HddrTitle(writer, rptFileFmt, false, title);

        //----------------------------------------------------------------//
        //                                                                //
        // Open the table and Write Write out the date, time and input    //
        // file identity.                                                 //
        //                                                                //
        //----------------------------------------------------------------//

        ReportCore.TableHddrPair(writer, rptFileFmt);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "Date_time", DateTime.Now.ToString(),
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableRowPair(writer, rptFileFmt,
                             "Symbol set file", symSetFilename,
                             _colSpanNone, _colSpanNone,
                             _maxSizeNameTag, maxLineLen,
                             _flagNone, _flagNone, _flagNone);

        ReportCore.TableClose(writer, rptFileFmt);
    }
}
