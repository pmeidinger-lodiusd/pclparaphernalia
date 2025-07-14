using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides the PDLData 'save report' function.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class ToolPrintLangReport
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
        const bool _flagBlankAfter = true;
        const bool _flagNameAsHddr = true;

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

        public static void generate(
            int indxInfoType,
            ReportCore.eRptFileFmt rptFileFmt,
            ReportCore.eRptChkMarks rptChkMarks,
            DataGrid dgSeq,
            ref string saveFilename,
            bool flagPCLSeqControl,
            bool flagPCLSeqSimple,
            bool flagPCLSeqComplex,
            bool flagPCLOptObsolete,
            bool flagPCLOptDiscrete,
            bool flagPCLXLTagDataType,
            bool flagPCLXLTagAttribute,
            bool flagPCLXLTagOperator,
            bool flagPCLXLTagAttrDef,
            bool flagPCLXLTagEmbedDataLen,
            bool flagPCLXLTagWhitespace,
            bool flagPCLXLOptReserved,
            bool flagPMLTagDataType,
            bool flagPMLTagAction,
            bool flagPMLTagOutcome,
            bool flagSymSetList,
            bool flagSymSetMap,
            bool flagOptRptWrap,
            ToolPrintLang.eSymSetMapType symSetMapType)
        {
            object stream = null;
            object writer = null;

            bool OK;

            string saveFolder = null;
            string fileExt;

            ToolCommonData.eToolSubIds infoType =
                (ToolCommonData.eToolSubIds)indxInfoType;

            ToolCommonFunctions.getFolderName(saveFilename,
                                               ref saveFolder);

            if (rptFileFmt == ReportCore.eRptFileFmt.html)
                fileExt = "html";
            else if (rptFileFmt == ReportCore.eRptFileFmt.xml)
                fileExt = "xml";
            else
                fileExt = "txt";

            saveFilename = saveFolder +
                           "\\PDLData_" + infoType.ToString() +
                           "." + fileExt;

            OK = ReportCore.docOpen(rptFileFmt,
                                     ref saveFilename,
                                     ref stream,
                                     ref writer);

            if (OK)
            {
                ReportCore.docInitialise(rptFileFmt, writer, true, false,
                                          0, null,
                                          null, null);
                reportHeader(infoType,
                              rptFileFmt,
                              writer,
                              dgSeq,
                              flagPCLSeqControl,
                              flagPCLSeqSimple,
                              flagPCLSeqComplex,
                              flagPCLOptObsolete,
                              flagPCLOptDiscrete,
                              flagPCLXLTagDataType,
                              flagPCLXLTagAttribute,
                              flagPCLXLTagOperator,
                              flagPCLXLTagAttrDef,
                              flagPCLXLTagEmbedDataLen,
                              flagPCLXLTagWhitespace,
                              flagPCLXLOptReserved,
                              flagPMLTagDataType,
                              flagPMLTagAction,
                              flagPMLTagOutcome,
                              flagSymSetList,
                              flagSymSetMap);

                if (infoType == ToolCommonData.eToolSubIds.PCL)
                    reportBodyPCLSeqs(rptFileFmt, rptChkMarks,
                                      writer, dgSeq);
                else if (infoType == ToolCommonData.eToolSubIds.HPGL2)
                    reportBodyHPGL2Commands(rptFileFmt, writer, dgSeq);
                else if (infoType == ToolCommonData.eToolSubIds.PCLXLTags)
                    reportBodyPCLXLTags(rptFileFmt, rptChkMarks,
                                        writer, dgSeq);
                else if (infoType == ToolCommonData.eToolSubIds.PCLXLEnums)
                    reportBodyPCLXLEnums(rptFileFmt, writer, dgSeq);
                else if (infoType == ToolCommonData.eToolSubIds.PJLCmds)
                    reportBodyPJLCommands(rptFileFmt, writer, dgSeq);
                else if (infoType == ToolCommonData.eToolSubIds.PMLTags)
                    reportBodyPMLTags(rptFileFmt, writer, dgSeq);
                else if (infoType == ToolCommonData.eToolSubIds.SymbolSets)
                    reportBodySymbolSets(rptFileFmt, rptChkMarks,
                                          writer, dgSeq,
                                          flagSymSetMap, flagOptRptWrap,
                                          symSetMapType);
                else if (infoType == ToolCommonData.eToolSubIds.Fonts)
                    reportBodyFonts(rptFileFmt, rptChkMarks,
                                     writer, dgSeq,
                                     flagSymSetList, flagOptRptWrap);
                else if (infoType == ToolCommonData.eToolSubIds.PaperSizes)
                    reportBodyPaperSizes(rptFileFmt, writer, dgSeq);
                else if (infoType == ToolCommonData.eToolSubIds.PrescribeCmds)
                    reportBodyPrescribeCommands(rptFileFmt, writer, dgSeq);

                ReportCore.docFinalise(rptFileFmt, writer);

                ReportCore.docClose(rptFileFmt, stream, writer);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t B o d y F o n t s                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write details of displayed Fonts to report file.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportBodyFonts(
            ReportCore.eRptFileFmt rptFileFmt,
            ReportCore.eRptChkMarks rptChkMarks,
            object writer,
            DataGrid dgSeq,
            bool flagSymSetList,
            bool flagOptRptWrap)
        {
            const int colCtStd = 11;
            const int colCtExt = 12;

            const string c0Name = "Typeface";
            const string c1Name = "Name";
            const string c2Name = "Spacing";
            const string c3Name = "Scalable";
            const string c4Name = "BoundSymbolSet";
            const string c5Name = "Pitch";
            const string c6Name = "Height";
            const string c7Name = "Var_Regular";
            const string c8Name = "Var_Italic";
            const string c9Name = "Var_Bold";
            const string c10Name = "Var_BoldItalic";
            const string c11Name = "SymbolSets";

            const string c0Hddr = "Typeface";
            const string c1Hddr = "Name";
            const string c2Hddr = "Spacing";
            const string c3Hddr = "Scalable?";
            const string c4Hddr = "Bound?";
            const string c5Hddr = "Pitch";
            const string c6Hddr = "Height";
            const string c7Hddr = "Regular";
            const string c8Hddr = "Italic ";
            const string c9Hddr = " Bold  ";
            const string c10Hddr = "Bold-It";
            const string c11Hddr = "Supported Symbol Sets?";

            const int lcSep = 2;

            const int lc0 = 8;
            const int lc1 = 22;
            const int lc2 = 12;
            const int lc3 = 9;
            const int lc4 = 6;
            const int lc5 = 6;
            const int lc6 = 6;
            const int lc7 = 7;
            const int lc8 = 7;
            const int lc9 = 7;
            const int lc10 = 7;
            const int lc11 = 25;

            string[] colHddrs;
            string[] colNames;
            int[] colSizes;

            int ctItems,
                  colCt;

            int colSpanName = -1,
                  colSpanVal = -1;

            PCLFont pclFont;

            string chkTrue,
                   chkTrue3,
                   chkTrue7,
                   chkTrue8,
                   chkTrue9,
                   chkTrue10;

            string chkFalse,
                   chkFalse3,
                   chkFalse7,
                   chkFalse8,
                   chkFalse9,
                   chkFalse10;

            if (rptChkMarks == ReportCore.eRptChkMarks.boxsym)
            {
                chkTrue = ReportCore._chkMarkBoxSymTrue;
                chkFalse = ReportCore._chkMarkBoxSymFalse;
            }
            else if (rptChkMarks == ReportCore.eRptChkMarks.txtsym)
            {
                chkTrue = ReportCore._chkMarkTxtSymTrue;
                chkFalse = ReportCore._chkMarkTxtSymFalse;
            }
            else // if (rptChkMarks == ReportCore.eRptChkMarks.text)
            {
                chkTrue = ReportCore._chkMarkTextTrue;
                chkFalse = ReportCore._chkMarkTextFalse;
            }

            chkTrue3 = (chkTrue.PadLeft((lc3 / 2) + 1, ' '));
            chkTrue7 = (chkTrue.PadLeft((lc7 / 2) + 1, ' '));
            chkTrue8 = (chkTrue.PadLeft((lc8 / 2) + 1, ' '));
            chkTrue9 = (chkTrue.PadLeft((lc9 / 2) + 1, ' '));
            chkTrue10 = (chkTrue.PadLeft((lc10 / 2) + 1, ' '));

            chkFalse3 = (chkFalse.PadLeft((lc3 / 2) + 1, ' '));
            chkFalse7 = (chkFalse.PadLeft((lc7 / 2) + 1, ' '));
            chkFalse8 = (chkFalse.PadLeft((lc8 / 2) + 1, ' '));
            chkFalse9 = (chkFalse.PadLeft((lc9 / 2) + 1, ' '));
            chkFalse10 = (chkFalse.PadLeft((lc10 / 2) + 1, ' '));

            ctItems = dgSeq.Items.Count;

            if ((flagSymSetList) && (!flagOptRptWrap))
            {
                colCt = colCtExt;

                colHddrs = new string[colCtExt] { c0Hddr, c1Hddr, c2Hddr,
                                                  c3Hddr, c4Hddr, c5Hddr,
                                                  c6Hddr, c7Hddr, c8Hddr,
                                                  c9Hddr, c10Hddr, c11Hddr};
                colNames = new string[colCtExt] { c0Name, c1Name, c2Name,
                                                  c3Name, c4Name, c5Name,
                                                  c6Name, c7Name, c8Name,
                                                  c9Name, c10Name, c11Name};
                colSizes = new int[colCtExt] { lc0, lc1, lc2,
                                                 lc3, lc4, lc5,
                                                 lc6, lc7, lc8,
                                                 lc9, lc10, lc11};
            }
            else
            {
                colCt = colCtStd;

                colSpanName = 2;
                colSpanVal = colCt - colSpanName;

                colHddrs = new string[colCtStd] { c0Hddr, c1Hddr, c2Hddr,
                                                  c3Hddr, c4Hddr, c5Hddr,
                                                  c6Hddr, c7Hddr, c8Hddr,
                                                  c9Hddr, c10Hddr};
                colNames = new string[colCtStd] { c0Name, c1Name, c2Name,
                                                  c3Name, c4Name, c5Name,
                                                  c6Name, c7Name, c8Name,
                                                  c9Name, c10Name};
                colSizes = new int[colCtStd] { lc0, lc1, lc2,
                                                 lc3, lc4, lc5,
                                                 lc6, lc7, lc8,
                                                 lc9, lc10};
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Open the table and Write the column header text.               //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableHddrData(writer, rptFileFmt, false,
                                  colCt, colHddrs, colSizes);

            //----------------------------------------------------------------//
            //                                                                //
            // Write the data rows.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < ctItems; i++)
            {
                string[] data = new string[colCt];

                pclFont = (PCLFont)dgSeq.Items[i];

                data[0] = pclFont.Typeface.ToString();
                data[1] = pclFont.Name;
                data[2] = pclFont.Spacing;
                data[3] = (pclFont.Scalable) ? chkTrue3 : chkFalse3;
                data[4] = pclFont.BoundSymbolSet;
                data[5] = pclFont.Pitch;
                data[6] = pclFont.Height;
                data[7] = (pclFont.Var_Regular) ? chkTrue7 : chkFalse7;
                data[8] = (pclFont.Var_Italic) ? chkTrue8 : chkFalse8;
                data[9] = (pclFont.Var_Bold) ? chkTrue9 : chkFalse9;
                data[10] = (pclFont.Var_BoldItalic) ? chkTrue10 : chkFalse10;

                if (!flagSymSetList)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Mapping not to be shown.                               //
                    //                                                        //
                    //--------------------------------------------------------//

                    ReportCore.tableRowText(writer, rptFileFmt, colCt, data,
                                             colNames, colSizes);
                }
                else if (!flagOptRptWrap)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Mapping to be shown without wrapping.                  //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (rptFileFmt != ReportCore.eRptFileFmt.text)
                    {
                        //--------------------------------------------------------//
                        //                                                        //
                        // No wrap mapping for html or xml report format.         //
                        //                                                        //
                        //--------------------------------------------------------//

                        data[11] = pclFont.SymbolSets;

                        ReportCore.tableRowText(writer, rptFileFmt, colCt, data,
                                                 colNames, colSizes);
                    }
                    else
                    {
                        //--------------------------------------------------------//
                        //                                                        //
                        // No wrap mapping for text report format.                //
                        //                                                        //
                        //--------------------------------------------------------//

                        string[][] arrData = new string[colCt][];

                        arrData[0] = new string[1];
                        arrData[1] = new string[1];
                        arrData[2] = new string[1];
                        arrData[3] = new string[1];
                        arrData[4] = new string[1];
                        arrData[5] = new string[1];
                        arrData[6] = new string[1];
                        arrData[7] = new string[1];
                        arrData[8] = new string[1];
                        arrData[9] = new string[1];
                        arrData[10] = new string[1];

                        arrData[0][0] = data[0];
                        arrData[1][0] = data[1];
                        arrData[2][0] = data[2];
                        arrData[3][0] = data[3];
                        arrData[4][0] = data[4];
                        arrData[5][0] = data[5];
                        arrData[6][0] = data[6];
                        arrData[7][0] = data[7];
                        arrData[8][0] = data[8];
                        arrData[9][0] = data[9];
                        arrData[10][0] = data[10];

                        arrData[11] = pclFont.SymbolSetRows;

                        ReportCore.tableMultiRowText(writer, rptFileFmt, colCt,
                                                      arrData, colSizes,
                                                      false, false, false);
                    }
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Mapping to be shown with wrapping.                     //
                    //                                                        //
                    //--------------------------------------------------------//

                    int maxLineLen = 120;

                    ReportCore.tableRowText(writer, rptFileFmt, colCt, data,
                                             colNames, colSizes);

                    if (pclFont.SymbolSetCt > 0)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Mapping data available for this symbol set.        //
                        //                                                    //
                        //----------------------------------------------------//

                        if (rptFileFmt != ReportCore.eRptFileFmt.text)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Wrapped mapping for html or xml report format. //
                            //                                                //
                            //------------------------------------------------//

                            ReportCore.tableRowPair(
                                writer, rptFileFmt,
                                c11Hddr, pclFont.SymbolSets,
                                colSpanName, colSpanVal,
                                _maxSizeNameTag, maxLineLen,
                                _flagBlankBefore, _flagBlankAfter, _flagNameAsHddr);
                        }
                        else
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Wrapped mapping for text report format.        //
                            //                                                //
                            //------------------------------------------------//

                            const int colCtPair = 2;

                            int tmpInt;

                            int[] colSizesPair = new int[colCtPair];

                            string[][] arrData = new string[colCtPair][];

                            tmpInt = 0;
                            for (int col = 0; col < colSpanName; col++)
                            {
                                if (col != 0)
                                    tmpInt += lcSep;

                                tmpInt += colSizes[col];
                            }

                            colSizesPair[0] = tmpInt;

                            tmpInt = 0;
                            for (int col = 0; col < colSpanVal; col++)
                            {
                                if (col != 0)
                                    tmpInt += lcSep;

                                tmpInt += colSizes[col + colSpanName];
                            }

                            colSizesPair[1] = tmpInt;

                            arrData[0] = new string[1];

                            arrData[0][0] = data[0];
                            arrData[0][0] = c11Hddr;
                            arrData[1] = pclFont.SymbolSetRows;

                            ReportCore.tableMultiRowText(
                                writer, rptFileFmt, colCtPair,
                                arrData, colSizesPair, true, true, true);
                        }
                    }
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write any required end tags.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableClose(writer, rptFileFmt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t B o d y H P G L 2 C o m m a n d s                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write details of displayed HP-GL/2 commands to report file.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportBodyHPGL2Commands(
            ReportCore.eRptFileFmt rptFileFmt,
            object writer,
            DataGrid dgSeq)
        {
            const int colCt = 2;

            const string c0Name = "Mnemonic";
            const string c1Name = "Description";

            const string c0Hddr = "Mnemonic";
            const string c1Hddr = "Description";

            const int lc0 = 8;
            const int lc1 = 35;

            string[] colHddrs;
            string[] colNames;
            int[] colSizes;

            int ctItems;

            HPGL2Command hpgl2Command;

            ctItems = dgSeq.Items.Count;

            colHddrs = new string[colCt] { c0Hddr, c1Hddr };
            colNames = new string[colCt] { c0Name, c1Name };
            colSizes = new int[colCt] { lc0, lc1 };

            //----------------------------------------------------------------//
            //                                                                //
            // Open the table and Write the column header text.               //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableHddrData(writer, rptFileFmt, false,
                                  colCt, colHddrs, colSizes);

            //----------------------------------------------------------------//
            //                                                                //
            // Write the data rows.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < ctItems; i++)
            {
                string[] data = new string[colCt];

                hpgl2Command = (HPGL2Command)dgSeq.Items[i];

                data[0] = hpgl2Command.Mnemonic;
                data[1] = hpgl2Command.Description;

                ReportCore.tableRowText(writer, rptFileFmt, colCt, data,
                                      colNames, colSizes);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write any required end tags.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableClose(writer, rptFileFmt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t B o d y P a p e r S i z e s                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write details of displayed paper size details to report file.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportBodyPaperSizes(
            ReportCore.eRptFileFmt rptFileFmt,
            object writer,
            DataGrid dgSeq)
        {
            const int colCt = 6;

            const string c0Name = "Name";
            const string c1Name = "Desc";
            const string c2Name = "EdgeShort";
            const string c3Name = "EdgeLong";
            const string c4Name = "IdPCL";
            const string c5Name = "IdNamePCLXL";

            const string c0Hddr = "Name";
            const string c1Hddr = "Description";
            const string c2Hddr = "Short edge";
            const string c3Hddr = "Long edge";
            const string c4Hddr = "PCL Id";
            const string c5Hddr = "PCL XL Id/Name";

            const int lc0 = 25;
            const int lc1 = 45;
            const int lc2 = 10;
            const int lc3 = 10;
            const int lc4 = 10;
            const int lc5 = 15;

            string[] colHddrs;
            string[] colNames;
            int[] colSizes;

            int ctItems;

            PCLPaperSize paperSize;

            ctItems = dgSeq.Items.Count;

            colHddrs = new string[colCt] { c0Hddr, c1Hddr, c2Hddr, c3Hddr,
                                           c4Hddr, c5Hddr };
            colNames = new string[colCt] { c0Name, c1Name, c2Name, c3Name,
                                           c4Name, c5Name };
            colSizes = new int[colCt] { lc0, lc1, lc2, lc3, lc4, lc5 };

            //----------------------------------------------------------------//
            //                                                                //
            // Open the table and Write the column header text.               //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableHddrData(writer, rptFileFmt, false,
                                  colCt, colHddrs, colSizes);

            //----------------------------------------------------------------//
            //                                                                //
            // Write the data rows.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < ctItems; i++)
            {
                string[] data = new string[colCt];

                paperSize = (PCLPaperSize)dgSeq.Items[i];

                data[0] = paperSize.Name;
                data[1] = paperSize.Desc;
                data[2] = paperSize.EdgeShort;
                data[3] = paperSize.EdgeLong;
                data[4] = paperSize.IdPCL;
                data[5] = paperSize.IdNamePCLXL;

                ReportCore.tableRowText(writer, rptFileFmt, colCt, data,
                                      colNames, colSizes);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write any required end tags.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableClose(writer, rptFileFmt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t B o d y P C L S e q s                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write details of displayed PCL sequences to report file.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportBodyPCLSeqs(
            ReportCore.eRptFileFmt rptFileFmt,
            ReportCore.eRptChkMarks rptChkMarks,
            object writer,
            DataGrid dgSeq)
        {
            const int colCt = 5;

            const string c0Name = "Sequence";
            const string c1Name = "Type";
            const string c2Name = "Obsolete";
            const string c3Name = "ValIsLen";
            const string c4Name = "Description";

            const string c2Hddr = "Obsolete?";
            const string c3Hddr = "#=length?";

            const int lc0 = 20;
            const int lc1 = 7;
            const int lc2 = 9;
            const int lc3 = 9;
            const int lc4 = 35;

            string[] colHddrs;
            string[] colNames;
            int[] colSizes;

            int ctItems;

            string typeName;

            PCLControlCode pclControlCode;

            PCLSimpleSeq pclSimpleSeq;

            PCLComplexSeq pclComplexSeq;

            string chkTrue,
                   chkTrue2,
                   chkTrue3;

            string chkFalse,
                   chkFalse2,
                   chkFalse3;

            if (rptChkMarks == ReportCore.eRptChkMarks.boxsym)
            {
                chkTrue = ReportCore._chkMarkBoxSymTrue;
                chkFalse = ReportCore._chkMarkBoxSymFalse;
            }
            else if (rptChkMarks == ReportCore.eRptChkMarks.txtsym)
            {
                chkTrue = ReportCore._chkMarkTxtSymTrue;
                chkFalse = ReportCore._chkMarkTxtSymFalse;
            }
            else // if (rptChkMarks == ReportCore.eRptChkMarks.text)
            {
                chkTrue = ReportCore._chkMarkTextTrue;
                chkFalse = ReportCore._chkMarkTextFalse;
            }

            chkTrue2 = (chkTrue.PadLeft((lc2 / 2) + 1, ' '));
            chkTrue3 = (chkTrue.PadLeft((lc3 / 2) + 1, ' '));

            chkFalse2 = (chkFalse.PadLeft((lc2 / 2) + 1, ' '));
            chkFalse3 = (chkFalse.PadLeft((lc3 / 2) + 1, ' '));

            ctItems = dgSeq.Items.Count;

            colHddrs = new string[colCt] { c0Name, c1Name, c2Hddr, c3Hddr, c4Name };
            colNames = new string[colCt] { c0Name, c1Name, c2Name, c3Name, c4Name };
            colSizes = new int[colCt] { lc0, lc1, lc2, lc3, lc4 };

            //----------------------------------------------------------------//
            //                                                                //
            // Open the table and Write the column header text.               //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableHddrData(writer, rptFileFmt, false,
                                  colCt, colHddrs, colSizes);

            //----------------------------------------------------------------//
            //                                                                //
            // Write the data rows.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < ctItems; i++)
            {
                string[] data = new string[colCt];

                typeName = dgSeq.Items[i].GetType().Name;

                if (typeName == "PCLControlCode")
                {
                    pclControlCode = (PCLControlCode)dgSeq.Items[i];

                    data[0] = pclControlCode.Sequence;
                    data[1] = pclControlCode.Type;
                    data[2] = (pclControlCode.FlagObsolete) ?
                                chkTrue2 : chkFalse2;
                    data[3] = (pclControlCode.FlagValIsLen) ?
                                chkTrue3 : chkFalse3;
                    data[4] = pclControlCode.Description;
                }
                else if (typeName == "PCLSimpleSeq")
                {
                    pclSimpleSeq = (PCLSimpleSeq)dgSeq.Items[i];

                    data[0] = pclSimpleSeq.Sequence;
                    data[1] = pclSimpleSeq.Type;
                    data[2] = (pclSimpleSeq.FlagObsolete) ?
                                chkTrue2 : chkFalse2;
                    data[3] = (pclSimpleSeq.FlagValIsLen) ?
                                chkTrue3 : chkFalse3;
                    data[4] = pclSimpleSeq.Description;
                }
                else if (typeName == "PCLComplexSeq")
                {
                    pclComplexSeq = (PCLComplexSeq)dgSeq.Items[i];

                    data[0] = pclComplexSeq.Sequence;
                    data[1] = pclComplexSeq.Type;
                    data[2] = (pclComplexSeq.FlagObsolete) ?
                                chkTrue2 : chkFalse2;
                    data[3] = (pclComplexSeq.FlagValIsLen) ?
                                chkTrue3 : chkFalse3;
                    data[4] = pclComplexSeq.Description;
                }

                ReportCore.tableRowText(writer, rptFileFmt, colCt, data,
                                      colNames, colSizes);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write any required end tags.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableClose(writer, rptFileFmt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t B o d y P C L X L E n u m s                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write details of displayed PCL XL enumerations to report file.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportBodyPCLXLEnums(
            ReportCore.eRptFileFmt rptFileFmt,
            object writer,
            DataGrid dgSeq)
        {
            const int colCt = 4;

            const string c0Name = "Operator";
            const string c1Name = "Attribute";
            const string c2Name = "Value";
            const string c3Name = "Description";

            const string c0Hddr = "Operator";
            const string c1Hddr = "Attribute";
            const string c2Hddr = "Value";
            const string c3Hddr = "Description";

            const int lc0 = 21;
            const int lc1 = 20;
            const int lc2 = 10;
            const int lc3 = 35;

            string[] colHddrs;
            string[] colNames;
            int[] colSizes;

            int ctItems;

            PCLXLAttrEnum xlAttrEnum;

            ctItems = dgSeq.Items.Count;

            colHddrs = new string[colCt] { c0Hddr, c1Hddr, c2Hddr, c3Hddr };
            colNames = new string[colCt] { c0Name, c1Name, c2Name, c3Name };
            colSizes = new int[colCt] { lc0, lc1, lc2, lc3 };

            //----------------------------------------------------------------//
            //                                                                //
            // Open the table and Write the column header text.               //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableHddrData(writer, rptFileFmt, false,
                                  colCt, colHddrs, colSizes);

            //----------------------------------------------------------------//
            //                                                                //
            // Write the data rows.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < ctItems; i++)
            {
                string[] data = new string[colCt];

                xlAttrEnum = (PCLXLAttrEnum)dgSeq.Items[i];

                data[0] = xlAttrEnum.Operator;
                data[1] = xlAttrEnum.Attribute;
                data[2] = xlAttrEnum.Value;
                data[3] = xlAttrEnum.Description;

                ReportCore.tableRowText(writer, rptFileFmt, colCt, data,
                                      colNames, colSizes);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write any required end tags.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableClose(writer, rptFileFmt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t B o d y P C L X L T a g s                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write details of displayed PCL XL tags to report file.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportBodyPCLXLTags(
            ReportCore.eRptFileFmt rptFileFmt,
            ReportCore.eRptChkMarks rptChkMarks,
            object writer,
            DataGrid dgSeq)
        {
            const int colCt = 4;

            const string c0Name = "Operator";
            const string c1Name = "Attribute";
            const string c2Name = "Value";
            const string c3Name = "Description";

            const string c0Hddr = "Tag";
            const string c1Hddr = "Type";
            const string c2Hddr = "Reserved";
            const string c3Hddr = "Description";

            const int lc0 = 4;
            const int lc1 = 18;
            const int lc2 = 5;
            const int lc3 = 35;

            string[] colHddrs;
            string[] colNames;
            int[] colSizes;

            int ctItems;

            string typeName;

            PCLXLAttrDefiner xlAttrDef;
            PCLXLAttribute xlAttribute;
            PCLXLDataType xlDataType;
            PCLXLEmbedDataDef xlEmbedDef;
            PCLXLOperator xlOperator;
            PCLXLWhitespace xlWhitespace;

            string chkTrue,
                   chkTrue2;

            string chkFalse,
                   chkFalse2;

            if (rptChkMarks == ReportCore.eRptChkMarks.boxsym)
            {
                chkTrue = ReportCore._chkMarkBoxSymTrue;
                chkFalse = ReportCore._chkMarkBoxSymFalse;
            }
            else if (rptChkMarks == ReportCore.eRptChkMarks.txtsym)
            {
                chkTrue = ReportCore._chkMarkTxtSymTrue;
                chkFalse = ReportCore._chkMarkTxtSymFalse;
            }
            else // if (rptChkMarks == ReportCore.eRptChkMarks.text)
            {
                chkTrue = ReportCore._chkMarkTextTrue;
                chkFalse = ReportCore._chkMarkTextFalse;
            }

            chkTrue2 = (chkTrue.PadLeft((lc2 / 2) + 1, ' '));

            chkFalse2 = (chkFalse.PadLeft((lc2 / 2) + 1, ' '));

            ctItems = dgSeq.Items.Count;

            colHddrs = new string[colCt] { c0Hddr, c1Hddr, c2Hddr, c3Hddr };
            colNames = new string[colCt] { c0Name, c1Name, c2Name, c3Name };
            colSizes = new int[colCt] { lc0, lc1, lc2, lc3 };

            //----------------------------------------------------------------//
            //                                                                //
            // Open the table and Write the column header text.               //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableHddrData(writer, rptFileFmt, false,
                                  colCt, colHddrs, colSizes);

            //----------------------------------------------------------------//
            //                                                                //
            // Write the data rows.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < ctItems; i++)
            {
                string[] data = new string[colCt];

                typeName = dgSeq.Items[i].GetType().Name;

                if (typeName == "PCLXLAttrDefiner")
                {
                    xlAttrDef = (PCLXLAttrDefiner)dgSeq.Items[i];

                    data[0] = xlAttrDef.Tag;
                    data[1] = xlAttrDef.Type;
                    data[2] = (xlAttrDef.FlagReserved) ?
                                chkTrue2 : chkFalse2;
                    data[3] = xlAttrDef.Description;
                }
                else if (typeName == "PCLXLAttribute")
                {
                    xlAttribute = (PCLXLAttribute)dgSeq.Items[i];

                    data[0] = xlAttribute.Tag;
                    data[1] = xlAttribute.Type;
                    data[2] = (xlAttribute.FlagReserved) ?
                                chkTrue2 : chkFalse2;
                    data[3] = xlAttribute.Description;
                }
                else if (typeName == "PCLXLDataType")
                {
                    xlDataType = (PCLXLDataType)dgSeq.Items[i];

                    data[0] = xlDataType.Tag;
                    data[1] = xlDataType.Type;
                    data[2] = (xlDataType.FlagReserved) ?
                                chkTrue2 : chkFalse2;
                    data[3] = xlDataType.Description;
                }
                else if (typeName == "PCLXLEmbedDataDef")
                {
                    xlEmbedDef = (PCLXLEmbedDataDef)dgSeq.Items[i];

                    data[0] = xlEmbedDef.Tag;
                    data[1] = xlEmbedDef.Type;
                    data[2] = (xlEmbedDef.FlagReserved) ?
                                chkTrue2 : chkFalse2;
                    data[3] = xlEmbedDef.Description;
                }
                else if (typeName == "PCLXLOperator")
                {
                    xlOperator = (PCLXLOperator)dgSeq.Items[i];

                    data[0] = xlOperator.Tag;
                    data[1] = xlOperator.Type;
                    data[2] = (xlOperator.FlagReserved) ?
                                chkTrue2 : chkFalse2;
                    data[3] = xlOperator.Description;
                }
                else if (typeName == "PCLXLWhitespace")
                {
                    xlWhitespace = (PCLXLWhitespace)dgSeq.Items[i];

                    data[0] = xlWhitespace.Tag;
                    data[1] = xlWhitespace.Type;
                    data[2] = (xlWhitespace.FlagReserved) ?
                                chkTrue2 : chkFalse2;
                    data[3] = xlWhitespace.Description;
                }

                ReportCore.tableRowText(writer, rptFileFmt, colCt, data,
                                      colNames, colSizes);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write any required end tags.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableClose(writer, rptFileFmt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t B o d y P J L C o m m a n d s                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write details of displayed PJL commands to report file.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportBodyPJLCommands(
            ReportCore.eRptFileFmt rptFileFmt,
            object writer,
            DataGrid dgSeq)
        {
            const int colCt = 2;

            const string c0Name = "Name";
            const string c1Name = "Description";

            const string c0Hddr = "Command";
            const string c1Hddr = "Description";

            const int lc0 = 10;
            const int lc1 = 35;

            string[] colHddrs;
            string[] colNames;
            int[] colSizes;

            int ctItems;

            PJLCommand pjlCommand;

            ctItems = dgSeq.Items.Count;

            colHddrs = new string[colCt] { c0Hddr, c1Hddr };
            colNames = new string[colCt] { c0Name, c1Name };
            colSizes = new int[colCt] { lc0, lc1 };

            //----------------------------------------------------------------//
            //                                                                //
            // Open the table and Write the column header text.               //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableHddrData(writer, rptFileFmt, false,
                                  colCt, colHddrs, colSizes);

            //----------------------------------------------------------------//
            //                                                                //
            // Write the data rows.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < ctItems; i++)
            {
                string[] data = new string[colCt];

                pjlCommand = (PJLCommand)dgSeq.Items[i];

                data[0] = pjlCommand.Name;
                data[1] = pjlCommand.Description;

                ReportCore.tableRowText(writer, rptFileFmt, colCt, data,
                                      colNames, colSizes);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write any required end tags.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableClose(writer, rptFileFmt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t B o d y P M L T a g s                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write details of displayed PML tags to report file.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportBodyPMLTags(
            ReportCore.eRptFileFmt rptFileFmt,
            object writer,
            DataGrid dgSeq)
        {
            const int colCt = 3;

            const string c0Name = "Tag";
            const string c1Name = "Type";
            const string c2Name = "Description";

            const string c0Hddr = "Tag";
            const string c1Hddr = "Type";
            const string c2Hddr = "Description";

            const int lc0 = 4;
            const int lc1 = 10;
            const int lc2 = 35;

            string[] colHddrs;
            string[] colNames;
            int[] colSizes;

            int ctItems;
            string typeName;

            PMLDataType pmlDataType;
            PMLAction pmlAction;
            PMLOutcome pmlOutcome;

            ctItems = dgSeq.Items.Count;

            colHddrs = new string[colCt] { c0Hddr, c1Hddr, c2Hddr };
            colNames = new string[colCt] { c0Name, c1Name, c2Name };
            colSizes = new int[colCt] { lc0, lc1, lc2 };

            //----------------------------------------------------------------//
            //                                                                //
            // Open the table and Write the column header text.               //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableHddrData(writer, rptFileFmt, false,
                                  colCt, colHddrs, colSizes);

            //----------------------------------------------------------------//
            //                                                                //
            // Write the data rows.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < ctItems; i++)
            {
                string[] data = new string[colCt];

                typeName = dgSeq.Items[i].GetType().Name;

                if (typeName == "PMLDataType")
                {
                    pmlDataType = (PMLDataType)dgSeq.Items[i];

                    data[0] = pmlDataType.Tag;
                    data[1] = pmlDataType.Type;
                    data[2] = pmlDataType.Description;
                }
                else if (typeName == "PMLAction")
                {
                    pmlAction = (PMLAction)dgSeq.Items[i];

                    data[0] = pmlAction.Tag;
                    data[1] = pmlAction.Type;
                    data[2] = pmlAction.Description;
                }
                else if (typeName == "PMLOutcome")
                {
                    pmlOutcome = (PMLOutcome)dgSeq.Items[i];

                    data[0] = pmlOutcome.Tag;
                    data[1] = pmlOutcome.Type;
                    data[2] = pmlOutcome.Description;
                }

                ReportCore.tableRowText(writer, rptFileFmt, colCt, data,
                                      colNames, colSizes);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write any required end tags.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableClose(writer, rptFileFmt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t B o d y P r e s c r i b e C o m m a n d s              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write details of displayed Prescribe commands to report file.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportBodyPrescribeCommands(
            ReportCore.eRptFileFmt rptFileFmt,
            object writer,
            DataGrid dgSeq)
        {
            const int colCt = 2;

            const string c0Name = "Name";
            const string c1Name = "Description";

            const string c0Hddr = "Command";
            const string c1Hddr = "Description";

            const int lc0 = 7;
            const int lc1 = 35;

            string[] colHddrs;
            string[] colNames;
            int[] colSizes;

            int ctItems;

            PrescribeCommand prescribeCommand;

            ctItems = dgSeq.Items.Count;

            colHddrs = new string[colCt] { c0Hddr, c1Hddr };
            colNames = new string[colCt] { c0Name, c1Name };
            colSizes = new int[colCt] { lc0, lc1 };

            //----------------------------------------------------------------//
            //                                                                //
            // Open the table and Write the column header text.               //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableHddrData(writer, rptFileFmt, false,
                                  colCt, colHddrs, colSizes);

            //----------------------------------------------------------------//
            //                                                                //
            // Write the data rows.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < ctItems; i++)
            {
                string[] data = new string[colCt];

                prescribeCommand = (PrescribeCommand)dgSeq.Items[i];

                data[0] = prescribeCommand.Name;
                data[1] = prescribeCommand.Description;

                ReportCore.tableRowText(writer, rptFileFmt, colCt, data,
                                      colNames, colSizes);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write any required end tags.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableClose(writer, rptFileFmt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t B o d y S y m b o l S e t s                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write details of displayed Symbol Sets to report file.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportBodySymbolSets(
            ReportCore.eRptFileFmt rptFileFmt,
            ReportCore.eRptChkMarks rptChkMarks,
            object writer,
            DataGrid dgSeq,
            bool flagSymSetMap,
            bool flagOptRptWrap,
            ToolPrintLang.eSymSetMapType symSetMapType)
        {
            const int colCtNoMap = 8;
            const int colCtMapWrap = 6;
            const int colCtMapOne = 7;
            const int colCtMapBoth = 9;

            const string c0Name = "Groupname";
            const string c1Name = "TypeDescShort";
            const string c2Name = "Id";
            const string c3Name = "Kind1";
            const string c4Name = "Alias";
            const string c5Name = "Name";
            const string c6aName = "FlagMapStd";
            const string c6bName = "MappingStd";
            const string c7aName = "FlagMapPCL";
            const string c7bName = "MappingPCL";
            const string c8Name = "MappingDiff";

            const string c0Hddr = "Group";
            const string c1Hddr = "Type";
            const string c2Hddr = "Id";
            const string c3Hddr = "Kind1";
            const string c4Hddr = "Alias";
            const string c5Hddr = "Name";
            const string c6aHddr = "Map (Strict)?";
            const string c6bHddr = "Mapping (Strict)";
            const string c7aHddr = "Map (LaserJet)?";
            const string c7bHddr = "Mapping (LaserJet)";
            const string c8Hddr = "Mapping (differences)";

            const int lc0 = 7;
            const int lc1 = 13;
            const int lc2 = 5;
            const int lc3 = 5;
            const int lc4 = 8;
            const int lc5 = 50;
            const int lc6a = 15;
            const int lc6b = 89;
            const int lc7a = 15;
            const int lc7b = 89;
            const int lc8 = 89;

            string[] colHddrs;
            string[] colNames;
            int[] colSizes;

            int ctItems,
                  colCt;

            int colSpanName = -1,
                  colSpanVal = -1;

            PCLSymbolSet symbolSet;

            string chkTrue,
                   chkTrue6a,
                   chkTrue7a;

            string chkFalse,
                   chkFalse6a,
                   chkFalse7a;

            if (rptChkMarks == ReportCore.eRptChkMarks.boxsym)
            {
                chkTrue = ReportCore._chkMarkBoxSymTrue;
                chkFalse = ReportCore._chkMarkBoxSymFalse;
            }
            else if (rptChkMarks == ReportCore.eRptChkMarks.txtsym)
            {
                chkTrue = ReportCore._chkMarkTxtSymTrue;
                chkFalse = ReportCore._chkMarkTxtSymFalse;
            }
            else // if (rptChkMarks == ReportCore.eRptChkMarks.text)
            {
                chkTrue = ReportCore._chkMarkTextTrue;
                chkFalse = ReportCore._chkMarkTextFalse;
            }

            chkTrue6a = (chkTrue.PadLeft((lc6a / 2) + 1, ' '));
            chkTrue7a = (chkTrue.PadLeft((lc7a / 2) + 1, ' '));

            chkFalse6a = (chkFalse.PadLeft((lc6a / 2) + 1, ' '));
            chkFalse7a = (chkFalse.PadLeft((lc7a / 2) + 1, ' '));

            ctItems = dgSeq.Items.Count;

            if (!flagSymSetMap)
            {
                colCt = colCtNoMap;

                colHddrs = new string[colCtNoMap] { c0Hddr, c1Hddr, c2Hddr,
                                                    c3Hddr, c4Hddr, c5Hddr,
                                                    c6aHddr, c7aHddr };
                colNames = new string[colCtNoMap] { c0Name, c1Name, c2Name,
                                                    c3Name, c4Name, c5Name,
                                                    c6aName, c7aName };
                colSizes = new int[colCtNoMap] { lc0, lc1, lc2,
                                                   lc3, lc4, lc5,
                                                   lc6a, lc7a };
            }
            else
            {
                if (flagOptRptWrap)
                {
                    colCt = colCtMapWrap;

                    colSpanName = 2;
                    colSpanVal = colCt - colSpanName;

                    colHddrs = new string[colCtMapWrap] { c0Hddr, c1Hddr, c2Hddr,
                                                          c3Hddr, c4Hddr, c5Hddr };
                    colNames = new string[colCtMapWrap] { c0Name, c1Name, c2Name,
                                                          c3Name, c4Name, c5Name };
                    colSizes = new int[colCtMapWrap] { lc0, lc1, lc2,
                                                         lc3, lc4, lc5 };
                }
                else if (symSetMapType == ToolPrintLang.eSymSetMapType.Std)
                {
                    colCt = colCtMapOne;

                    colHddrs = new string[colCtMapOne] { c0Hddr, c1Hddr, c2Hddr,
                                                         c3Hddr, c4Hddr, c5Hddr,
                                                         c6bHddr };
                    colNames = new string[colCtMapOne] { c0Name, c1Name, c2Name,
                                                         c3Name, c4Name, c5Name,
                                                         c6bName };
                    colSizes = new int[colCtMapOne] { lc0, lc1, lc2,
                                                        lc3, lc4, lc5,
                                                        lc6b };
                }
                else if (symSetMapType == ToolPrintLang.eSymSetMapType.PCL)
                {
                    colCt = colCtMapOne;

                    colHddrs = new string[colCtMapOne] { c0Hddr, c1Hddr, c2Hddr,
                                                         c3Hddr, c4Hddr, c5Hddr,
                                                         c7bHddr };
                    colNames = new string[colCtMapOne] { c0Name, c1Name, c2Name,
                                                         c3Name, c4Name, c5Name,
                                                         c7bName };
                    colSizes = new int[colCtMapOne] { lc0, lc1, lc2,
                                                        lc3, lc4, lc5,
                                                        lc7b };
                }
                else //if (symSetMapType == ToolPrintLang.eSymSetMapType.Both)
                {
                    colCt = colCtMapBoth;

                    colHddrs = new string[colCtMapBoth] { c0Hddr, c1Hddr, c2Hddr,
                                                          c3Hddr, c4Hddr, c5Hddr,
                                                          c6bHddr, c7bHddr, c8Hddr };
                    colNames = new string[colCtMapBoth] { c0Name, c1Name, c2Name,
                                                          c3Name, c4Name, c5Name,
                                                          c6bName, c7bName, c8Name };
                    colSizes = new int[colCtMapBoth] { lc0, lc1, lc2,
                                                         lc3, lc4, lc5,
                                                         lc6b, lc7b, lc8 };
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Open the main table and Write the column header text.          //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableHddrData(writer, rptFileFmt, false,
                                  colCt, colHddrs, colSizes);

            //----------------------------------------------------------------//
            //                                                                //
            // Write the data rows.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < ctItems; i++)
            {
                string[] data = new string[colCt];

                bool mapStd,
                        mapPCL;

                symbolSet = (PCLSymbolSet)dgSeq.Items[i];

                mapStd = symbolSet.FlagMapStd;
                mapPCL = symbolSet.FlagMapPCL;

                data[0] = symbolSet.Groupname;
                data[1] = symbolSet.TypeDescShort;
                data[2] = symbolSet.Id;
                data[3] = symbolSet.Kind1.ToString();
                data[4] = symbolSet.Alias;
                data[5] = symbolSet.Name;

                if (!flagSymSetMap)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Mapping not to be shown.                               //
                    //                                                        //
                    //--------------------------------------------------------//

                    data[6] = (mapStd) ? chkTrue6a : chkFalse6a;
                    data[7] = (mapPCL) ? chkTrue7a : chkFalse7a;

                    ReportCore.tableRowText(writer, rptFileFmt, colCt, data,
                                             colNames, colSizes);
                }
                else if (flagOptRptWrap)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Mapping to be shown with wrapping.                     //
                    //                                                        //
                    //--------------------------------------------------------//

                    int maxLineLen = 120;

                    ReportCore.tableRowText(writer, rptFileFmt, colCt, data,
                                             colNames, colSizes);

                    if ((mapStd) || (mapPCL))
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Mapping data available for this symbol set.        //
                        //                                                    //
                        //----------------------------------------------------//

                        if (rptFileFmt != ReportCore.eRptFileFmt.text)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Wrapped mapping for html or xml report format. //
                            //                                                //
                            //------------------------------------------------//

                            if (symSetMapType == ToolPrintLang.eSymSetMapType.Std)
                            {
                                if (mapStd)
                                    ReportCore.tableRowPair(
                                        writer, rptFileFmt, c6bHddr,
                                        symbolSet.MappingStd,
                                        colSpanName, colSpanVal,
                                        _maxSizeNameTag, maxLineLen,
                                        _flagBlankBefore, _flagBlankAfter, _flagNameAsHddr);
                                else
                                    ReportCore.tableRowPair(
                                        writer, rptFileFmt, c6bHddr,
                                        "Not defined - see LaserJet mapping definition",
                                        colSpanName, colSpanVal,
                                        _maxSizeNameTag, maxLineLen,
                                        _flagBlankBefore, _flagBlankAfter, _flagNameAsHddr);
                            }
                            else if (symSetMapType == ToolPrintLang.eSymSetMapType.PCL)
                            {
                                if (mapPCL)
                                    ReportCore.tableRowPair(
                                        writer, rptFileFmt, c7bHddr,
                                        symbolSet.MappingPCL,
                                        colSpanName, colSpanVal,
                                        _maxSizeNameTag, maxLineLen,
                                        _flagBlankBefore, _flagBlankAfter, _flagNameAsHddr);
                                else
                                    ReportCore.tableRowPair(
                                        writer, rptFileFmt, c7bHddr,
                                        "Not defined - see Standard (Strict) mapping definition",
                                        colSpanName, colSpanVal,
                                        _maxSizeNameTag, maxLineLen,
                                        _flagBlankBefore, _flagBlankAfter, _flagNameAsHddr);
                            }
                            else // if (symSetMapType == ToolPrintLang.eSymSetMapType.Both)
                            {
                                if (mapStd)
                                    ReportCore.tableRowPair(
                                        writer, rptFileFmt, c6bHddr,
                                        symbolSet.MappingStd,
                                        colSpanName, colSpanVal,
                                        _maxSizeNameTag, maxLineLen,
                                        _flagBlankBefore, _flagNone, _flagNameAsHddr);
                                else
                                    ReportCore.tableRowPair(
                                        writer, rptFileFmt, c6bHddr,
                                        "Not defined - see LaserJet mapping definition",
                                        colSpanName, colSpanVal,
                                        _maxSizeNameTag, maxLineLen,
                                        _flagBlankBefore, _flagNone, _flagNameAsHddr);

                                if (mapPCL)
                                    ReportCore.tableRowPair(
                                        writer, rptFileFmt, c7bHddr,
                                        symbolSet.MappingPCL,
                                        colSpanName, colSpanVal,
                                        _maxSizeNameTag, maxLineLen,
                                        _flagBlankBefore, _flagNone, _flagNameAsHddr);
                                else
                                    ReportCore.tableRowPair(
                                        writer, rptFileFmt, c7bHddr,
                                        "Not defined - see Standard (Strict) mapping definition",
                                        colSpanName, colSpanVal,
                                        _maxSizeNameTag, maxLineLen,
                                        _flagBlankBefore, _flagNone, _flagNameAsHddr);

                                if ((mapStd) && (mapPCL))
                                    ReportCore.tableRowPair(
                                        writer, rptFileFmt, c8Hddr,
                                        symbolSet.MappingPCL,
                                        colSpanName, colSpanVal,
                                        _maxSizeNameTag, maxLineLen,
                                        _flagBlankBefore, _flagBlankAfter, _flagNameAsHddr);
                                else
                                    ReportCore.tableRowPair(
                                        writer, rptFileFmt, c8Hddr,
                                        "Not applicable (only one set defined)",
                                        colSpanName, colSpanVal,
                                        _maxSizeNameTag, maxLineLen,
                                        _flagBlankBefore, _flagBlankAfter, _flagNameAsHddr);
                            }
                        }
                        else
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Wrapped mapping for text report format.        //
                            //                                                //
                            //------------------------------------------------//

                            const int colCtPair = 2;
                            int[] colSizesPair = new int[colCtPair] {
                                                        22, 100 };          // *************** do this another way ????? **************

                            string[][] arrData = new string[colCtPair][];

                            arrData[0] = new string[1];

                            arrData[0][0] = data[0];

                            if (symSetMapType == ToolPrintLang.eSymSetMapType.Std)
                            {
                                arrData[0][0] = c6bHddr;
                                arrData[1] = symbolSet.MapRowsStd;

                                ReportCore.tableMultiRowText(
                                    writer, rptFileFmt, colCtPair,
                                    arrData, colSizesPair, true, true, true);
                            }
                            else if (symSetMapType == ToolPrintLang.eSymSetMapType.PCL)
                            {
                                arrData[0][0] = c7bHddr;
                                arrData[1] = symbolSet.MapRowsPCL;

                                ReportCore.tableMultiRowText(
                                    writer, rptFileFmt, colCtPair,
                                    arrData, colSizesPair, true, true, true);
                            }
                            else // if (symSetMapType == ToolPrintLang.eSymSetMapType.Both)
                            {
                                arrData[0][0] = c6bHddr;
                                arrData[1] = symbolSet.MapRowsStd;

                                ReportCore.tableMultiRowText(
                                    writer, rptFileFmt, colCtPair,
                                    arrData, colSizesPair, true, false, false);

                                arrData[0][0] = c7bHddr;
                                arrData[1] = symbolSet.MapRowsPCLDiff;

                                ReportCore.tableMultiRowText(
                                    writer, rptFileFmt, colCtPair,
                                    arrData, colSizesPair, true, false, false);

                                arrData[0][0] = c8Hddr;
                                arrData[1] = symbolSet.MapRowsDiff;

                                ReportCore.tableMultiRowText(
                                    writer, rptFileFmt, colCtPair,
                                    arrData, colSizesPair, true, true, true);
                            }
                        }
                    }
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Mapping to be shown without wrapping.                  //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (rptFileFmt != ReportCore.eRptFileFmt.text)
                    {
                        //--------------------------------------------------------//
                        //                                                        //
                        // No wrap mapping for html or xml report format.         //
                        //                                                        //
                        //--------------------------------------------------------//

                        if (symSetMapType == ToolPrintLang.eSymSetMapType.Std)
                        {
                            data[6] = symbolSet.MappingStd;
                        }
                        else if (symSetMapType == ToolPrintLang.eSymSetMapType.PCL)
                        {
                            data[6] = symbolSet.MappingPCL;
                        }
                        else // if (symSetMapType == ToolPrintLang.eSymSetMapType.Both)
                        {
                            data[6] = symbolSet.MappingStd;
                            data[7] = symbolSet.MappingPCLDiff;
                            data[8] = symbolSet.MappingDiff;
                        }

                        ReportCore.tableRowText(writer, rptFileFmt, colCt, data,
                                                 colNames, colSizes);
                    }
                    else
                    {
                        //--------------------------------------------------------//
                        //                                                        //
                        // No wrap mapping for text report format.                //
                        //                                                        //
                        //--------------------------------------------------------//

                        string[][] arrData = new string[colCt][];

                        arrData[0] = new string[1];
                        arrData[1] = new string[1];
                        arrData[2] = new string[1];
                        arrData[3] = new string[1];
                        arrData[4] = new string[1];
                        arrData[5] = new string[1];

                        arrData[0][0] = data[0];
                        arrData[1][0] = data[1];
                        arrData[2][0] = data[2];
                        arrData[3][0] = data[3];
                        arrData[4][0] = data[4];
                        arrData[5][0] = data[5];

                        if (symSetMapType == ToolPrintLang.eSymSetMapType.Std)
                        {
                            arrData[6] = symbolSet.MapRowsStd;
                        }
                        else if (symSetMapType == ToolPrintLang.eSymSetMapType.PCL)
                        {
                            arrData[6] = symbolSet.MapRowsPCL;
                        }
                        else // if (symSetMapType == ToolPrintLang.eSymSetMapType.Both)
                        {
                            arrData[6] = symbolSet.MapRowsStd;
                            arrData[7] = symbolSet.MapRowsPCLDiff;
                            arrData[8] = symbolSet.MapRowsDiff;
                        }

                        ReportCore.tableMultiRowText(writer, rptFileFmt, colCt,
                                                      arrData, colSizes,
                                                      //   false, false, true);
                                                      false, false, false);
                    }
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write any required end tags.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            ReportCore.tableClose(writer, rptFileFmt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e p o r t H e a d e r                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report header.                                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void reportHeader(ToolCommonData.eToolSubIds infoType,
                                         ReportCore.eRptFileFmt rptFileFmt,
                                         object writer,
                                         DataGrid dgSeq,
                                         bool flagPCLSeqControl,
                                         bool flagPCLSeqSimple,
                                         bool flagPCLSeqComplex,
                                         bool flagPCLOptObsolete,
                                         bool flagPCLOptDiscrete,
                                         bool flagPCLXLTagDataType,
                                         bool flagPCLXLTagAttribute,
                                         bool flagPCLXLTagOperator,
                                         bool flagPCLXLTagAttrDef,
                                         bool flagPCLXLTagEmbedDataLen,
                                         bool flagPCLXLTagWhitespace,
                                         bool flagPCLXLOptReserved,
                                         bool flagPMLTagDataType,
                                         bool flagPMLTagAction,
                                         bool flagPMLTagOutcome,
                                         bool flagSymSetList,
                                         bool flagSymSetMap)
        {
            int maxLineLen = 80;          // ********************** set this from column sizes ????????????????

            int ctCols;

            string sort = string.Empty;
            string colHddr = string.Empty;
            string colSort = string.Empty;

            bool selHddrStarted = false;

            ctCols = dgSeq.Columns.Count;

            for (int i = 0; i < ctCols; i++)
            {
                colSort = dgSeq.ColumnFromDisplayIndex(i).SortDirection.ToString();

                if (colSort != string.Empty)
                {
                    colHddr = dgSeq.ColumnFromDisplayIndex(i).Header.ToString();

                    if (sort != string.Empty)
                        sort += "; ";

                    sort += colHddr + "(" + colSort + ")";
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Write out the title and selection criteria details.            //
            //                                                                //
            //----------------------------------------------------------------//

            if (sort != string.Empty)
            {
                if (!selHddrStarted)
                {
                    ReportCore.tableHddrPair(writer, rptFileFmt);

                    selHddrStarted = true;
                }

                ReportCore.tableRowPair(writer, rptFileFmt,
                        "Sort", sort,
                       _colSpanNone, _colSpanNone,
                        _maxSizeNameTag, maxLineLen,
                       _flagNone, _flagNone, _flagNone);
            }

            if (infoType == ToolCommonData.eToolSubIds.PCL)
            {
                ReportCore.hddrTitle(writer, rptFileFmt, false,
                                      "PCL sequence list:");

                if (flagPCLSeqControl)
                {
                    if (!selHddrStarted)
                    {
                        ReportCore.tableHddrPair(writer, rptFileFmt);

                        selHddrStarted = true;
                    }

                    ReportCore.tableRowPair(writer, rptFileFmt,
                                         "Include", "Control Codes",
                                         _colSpanNone, _colSpanNone,
                                         _maxSizeNameTag, maxLineLen,
                                         _flagNone, _flagNone, _flagNone);
                }

                if (flagPCLSeqSimple)
                {
                    if (!selHddrStarted)
                    {
                        ReportCore.tableHddrPair(writer, rptFileFmt);

                        selHddrStarted = true;
                    }

                    ReportCore.tableRowPair(writer, rptFileFmt,
                                         "Include", "Simple escape sequences",
                                         _colSpanNone, _colSpanNone,
                                         _maxSizeNameTag, maxLineLen,
                                         _flagNone, _flagNone, _flagNone);
                }

                if (flagPCLSeqComplex)
                {
                    if (!selHddrStarted)
                    {
                        ReportCore.tableHddrPair(writer, rptFileFmt);

                        selHddrStarted = true;
                    }

                    ReportCore.tableRowPair(writer, rptFileFmt,
                                         "Include",
                                         "Complex (parameterised) escape sequences",
                                         _colSpanNone, _colSpanNone,
                                         _maxSizeNameTag, maxLineLen,
                                         _flagNone, _flagNone, _flagNone);
                }

                if (!selHddrStarted)
                {
                    ReportCore.tableHddrPair(writer, rptFileFmt);

                    selHddrStarted = true;
                }

                ReportCore.tableRowPair(writer, rptFileFmt,
                                     "Select",
                                     ((flagPCLOptDiscrete == true) ?
                                        "Show" : "Do not show") +
                                     " discrete values for enumerated sequences",
                                     _colSpanNone, _colSpanNone,
                                     _maxSizeNameTag, maxLineLen,
                                     _flagNone, _flagNone, _flagNone);

                ReportCore.tableRowPair(writer, rptFileFmt,
                                     "Select",
                                     ((flagPCLOptObsolete == true) ?
                                        "Show" : "Do not show") +
                                     " obsolete sequences",
                                     _colSpanNone, _colSpanNone,
                                     _maxSizeNameTag, maxLineLen,
                                     _flagNone, _flagNone, _flagNone);
            }
            else if (infoType == ToolCommonData.eToolSubIds.HPGL2)
            {
                ReportCore.hddrTitle(writer, rptFileFmt, false,
                                      "HP-GL/2 command list:");
            }
            else if (infoType == ToolCommonData.eToolSubIds.PCLXLTags)
            {
                ReportCore.hddrTitle(writer, rptFileFmt, false,
                                      "PCL XL tag list:");

                if (flagPCLXLTagDataType)
                {
                    if (!selHddrStarted)
                    {
                        ReportCore.tableHddrPair(writer, rptFileFmt);

                        selHddrStarted = true;
                    }

                    ReportCore.tableRowPair(writer, rptFileFmt,
                       "Include",
                       "Data Type tags",
                       _colSpanNone, _colSpanNone,
                       _maxSizeNameTag, maxLineLen,
                       _flagNone, _flagNone, _flagNone);
                }

                if (flagPCLXLTagAttribute)
                {
                    if (!selHddrStarted)
                    {
                        ReportCore.tableHddrPair(writer, rptFileFmt);

                        selHddrStarted = true;
                    }

                    ReportCore.tableRowPair(writer, rptFileFmt,
                       "Include",
                       "Attribute tags",
                       _colSpanNone, _colSpanNone,
                       _maxSizeNameTag, maxLineLen,
                       _flagNone, _flagNone, _flagNone);
                }

                if (flagPCLXLTagOperator)
                {
                    if (!selHddrStarted)
                    {
                        ReportCore.tableHddrPair(writer, rptFileFmt);

                        selHddrStarted = true;
                    }

                    ReportCore.tableRowPair(writer, rptFileFmt,
                       "Include",
                       "Operator tags",
                       _colSpanNone, _colSpanNone,
                       _maxSizeNameTag, maxLineLen,
                       _flagNone, _flagNone, _flagNone);
                }

                if (flagPCLXLTagAttrDef)
                {
                    if (!selHddrStarted)
                    {
                        ReportCore.tableHddrPair(writer, rptFileFmt);

                        selHddrStarted = true;
                    }

                    ReportCore.tableRowPair(writer, rptFileFmt,
                       "Include",
                       "Attribute Definer tags",
                       _colSpanNone, _colSpanNone,
                       _maxSizeNameTag, maxLineLen,
                       _flagNone, _flagNone, _flagNone);
                }

                if (flagPCLXLTagOperator)
                {
                    if (!selHddrStarted)
                    {
                        ReportCore.tableHddrPair(writer, rptFileFmt);

                        selHddrStarted = true;
                    }

                    ReportCore.tableRowPair(writer, rptFileFmt,
                       "Include",
                       "Embedded Data Length tags",
                       _colSpanNone, _colSpanNone,
                       _maxSizeNameTag, maxLineLen,
                       _flagNone, _flagNone, _flagNone);
                }

                if (flagPCLXLTagWhitespace)
                {
                    if (!selHddrStarted)
                    {
                        ReportCore.tableHddrPair(writer, rptFileFmt);

                        selHddrStarted = true;
                    }

                    ReportCore.tableRowPair(writer, rptFileFmt,
                       "Include",
                       "Whitespace tags",
                       _colSpanNone, _colSpanNone,
                       _maxSizeNameTag, maxLineLen,
                       _flagNone, _flagNone, _flagNone);
                }

                if (!selHddrStarted)
                {
                    ReportCore.tableHddrPair(writer, rptFileFmt);

                    selHddrStarted = true;
                }

                ReportCore.tableRowPair(writer, rptFileFmt,
                                     "Select",
                                     ((flagPCLXLOptReserved == true) ?
                                        "Show" : "Do not show") +
                                     " reserved values",
                                     _colSpanNone, _colSpanNone,
                                     _maxSizeNameTag, maxLineLen,
                                     _flagNone, _flagNone, _flagNone);
            }
            else if (infoType == ToolCommonData.eToolSubIds.PCLXLEnums)
            {
                ReportCore.hddrTitle(writer, rptFileFmt, false,
                                      "PCL XL enumeration list:");
            }
            else if (infoType == ToolCommonData.eToolSubIds.PJLCmds)
            {
                ReportCore.hddrTitle(writer, rptFileFmt, false,
                                      "PJL command list:");
            }
            else if (infoType == ToolCommonData.eToolSubIds.PMLTags)
            {
                ReportCore.hddrTitle(writer, rptFileFmt, false,
                                      "PML tag list:");

                if (flagPMLTagDataType)
                {
                    if (!selHddrStarted)
                    {
                        ReportCore.tableHddrPair(writer, rptFileFmt);

                        selHddrStarted = true;
                    }

                    ReportCore.tableRowPair(writer, rptFileFmt,
                       "Include",
                       "Data Type tags",
                       _colSpanNone, _colSpanNone,
                       _maxSizeNameTag, maxLineLen,
                       _flagNone, _flagNone, _flagNone);
                }

                if (flagPMLTagAction)
                {
                    if (!selHddrStarted)
                    {
                        ReportCore.tableHddrPair(writer, rptFileFmt);

                        selHddrStarted = true;
                    }

                    ReportCore.tableRowPair(writer, rptFileFmt,
                       "Include",
                       "Action tags",
                       _colSpanNone, _colSpanNone,
                       _maxSizeNameTag, maxLineLen,
                       _flagNone, _flagNone, _flagNone);
                }

                if (flagPMLTagOutcome)
                {
                    if (!selHddrStarted)
                    {
                        ReportCore.tableHddrPair(writer, rptFileFmt);

                        selHddrStarted = true;
                    }

                    ReportCore.tableRowPair(writer, rptFileFmt,
                       "Include",
                       "Outcome tags",
                       _colSpanNone, _colSpanNone,
                       _maxSizeNameTag, maxLineLen,
                       _flagNone, _flagNone, _flagNone);
                }
            }
            else if (infoType == ToolCommonData.eToolSubIds.SymbolSets)
            {
                ReportCore.hddrTitle(writer, rptFileFmt, false,
                                      "Symbol Set list:");

                if (flagSymSetMap)
                {
                    if (!selHddrStarted)
                    {
                        ReportCore.tableHddrPair(writer, rptFileFmt);

                        selHddrStarted = true;
                    }

                    ReportCore.tableRowPair(writer, rptFileFmt,
                       "Include",
                       "Character mapping",
                       _colSpanNone, _colSpanNone,
                       _maxSizeNameTag, maxLineLen,
                       _flagNone, _flagNone, _flagNone);
                }
            }
            else if (infoType == ToolCommonData.eToolSubIds.Fonts)
            {
                ReportCore.hddrTitle(writer, rptFileFmt, false,
                                      "Font list:");

                if (flagSymSetList)
                {
                    if (!selHddrStarted)
                    {
                        ReportCore.tableHddrPair(writer, rptFileFmt);

                        selHddrStarted = true;
                    }

                    ReportCore.tableRowPair(writer, rptFileFmt,
                       "Include",
                       "Supported Symbol Sets",
                       _colSpanNone, _colSpanNone,
                       _maxSizeNameTag, maxLineLen,
                       _flagNone, _flagNone, _flagNone);
                }
            }
            else if (infoType == ToolCommonData.eToolSubIds.PaperSizes)
            {
                ReportCore.hddrTitle(writer, rptFileFmt, false,
                                      "Paper size list:");

            }
            else if (infoType == ToolCommonData.eToolSubIds.PrescribeCmds)
            {
                ReportCore.hddrTitle(writer, rptFileFmt, false,
                                      "Prescribe command list:");
            }

            if (selHddrStarted)
            {
                ReportCore.tableClose(writer, rptFileFmt);
            }
        }
    }
}
