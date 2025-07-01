using System.IO;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides PCL support for the FontSample tool.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class ToolFontSamplePCL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const string _hexChars = "0123456789ABCDEF";

        const int _macroId = 1;
        const short _gridDim = 16;
        const int _gridDimHalf = _gridDim / 2;
        const short _gridCols = _gridDim;
        const short _gridRows = _gridDim;
        const ushort _unitsPerInch = PCLWriter.sessionUPI;

        const short _lineSpacing = _unitsPerInch / 4;
        const short _cellWidth = (_unitsPerInch * 1) / 3;
        const short _cellHeight = (_unitsPerInch * 25) / 60;

        const short _marginX = (_unitsPerInch * 7) / 6;
        const short _posYDesc = (_unitsPerInch * 3) / 4;
        const short _posYGrid = _posYDesc + (_lineSpacing * 4);
        const short _posYSelData = _posYGrid +
                                          (_cellHeight * (_gridRows + 2)) +
                                          (_lineSpacing * 2);

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static ushort _symSetUserMapMax = 0;

        private static ushort[] _symSetUserMap = null;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e J o b                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate test data.                                                //
        //                                                                    //
        // Some sequences are built up as (Unicode) strings, then converted   //
        // to byte arrays before writing out - this works OK because all the  //
        // characters we're using are within the ASCII range (0x00-0x7f) and  //
        // are hence represented using a single byte in the UTF-8 encoding.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void generateJob(
            BinaryWriter prnWriter,
            PCLFonts.eFontType fontType,
            int indxPaperSize,
            int indxPaperType,
            int indxOrientation,
            bool formAsMacro,
            bool showC0Chars,
            bool optGridVertical,
            bool fontBound,
            string fontId,
            string fontDesc,
            string symbolSet,
            string fontLoadDesc,
            string fontSelDesc,
            string fontSelSeq,
            string symbolSetName,
            ushort[] sampleRangeOffsets,
            PCLTextParsingMethods.eIndex indxTextParseMethod,
            double pointSize,
            bool sizeIsHeight,
            bool downloadFontRemove,
            bool fontSelectById,
            bool prnDiskFontDataKnown,
            bool prnDiskFontLoadViaMacro,
            ushort fontIdNo,
            ushort fontMacroIdNo,
            string fontFilename,
            bool symSetUserSet,
            bool showMapCodesUCS2,
            bool showMapCodesUTF8,
            bool symSetUserActEmbed,
            string symSetUserFile)
        {
            PCLOrientations.eAspect aspect;

            ushort logXOffset;
            ushort symSetKind1 = 0;

            int pageCt = sampleRangeOffsets.Length;

            bool downloadFont,
                    fontMacroRemove;

            //----------------------------------------------------------------//

            aspect = PCLOrientations.getAspect(indxOrientation);

            logXOffset = PCLPaperSizes.getLogicalOffset(indxPaperSize,
                                                        _unitsPerInch, aspect);

            //----------------------------------------------------------------//

            if (symSetUserSet)
            {
                symSetKind1 = PCLSymbolSets.translateIdToKind1(symbolSet);

                if ((!symSetUserActEmbed) ||
                    (showMapCodesUCS2) || (showMapCodesUTF8))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Using a user-defined symbol set file.                  //
                    // Extract the mapping because either (or both):          //
                    //                                                        //
                    //  - Not embedding the symbol set file (e.g. because it  //
                    //    is a 16-bit one (not supported by printers?) but    //
                    //    just using the mapping to determine the target      //
                    //    Unicode code-points.                                //
                    //                                                        //
                    //  - Want to display the target code-point values in the //
                    //    grid.                                               //
                    //                                                        //
                    //--------------------------------------------------------//

                    _symSetUserMapMax = PCLSymbolSets.getMapArrayMax(
                                          PCLSymbolSets.IndexUserSet);

                    _symSetUserMap = new ushort[_symSetUserMapMax + 1];

                    _symSetUserMap = PCLSymbolSets.getMapArrayUserSet();
                }
            }

            if (fontType == PCLFonts.eFontType.Download)
                downloadFont = true;
            else
                downloadFont = false;

            //----------------------------------------------------------------//

            generateJobHeader(prnWriter,
                              fontType,
                              prnDiskFontLoadViaMacro,
                              indxPaperSize,
                              indxPaperType,
                              indxOrientation,
                              formAsMacro,
                              optGridVertical,
                              logXOffset,
                              fontIdNo,
                              fontMacroIdNo,
                              fontFilename,
                              symSetUserSet,
                              symSetUserActEmbed,
                              symSetUserFile);

            for (int i = 0; i < pageCt; i++)
            {
                ushort rangeOffset = sampleRangeOffsets[i];

                generatePage(prnWriter,
                             fontType,
                             prnDiskFontDataKnown,
                             formAsMacro,
                             showC0Chars,
                             optGridVertical,
                             fontBound,
                             fontId,
                             fontDesc,
                             symbolSet,
                             fontLoadDesc,
                             fontSelDesc,
                             fontSelSeq,
                             symbolSetName,
                             rangeOffset,
                             indxTextParseMethod,
                             pointSize,
                             sizeIsHeight,
                             logXOffset,
                             fontSelectById,
                             fontIdNo,
                             fontFilename,
                             symSetUserSet,
                             showMapCodesUCS2,
                             showMapCodesUTF8,
                             symSetUserActEmbed,
                             symSetUserFile);
            }

            if (fontType == PCLFonts.eFontType.PrnDisk)
                fontMacroRemove = prnDiskFontLoadViaMacro;
            else
                fontMacroRemove = false;

            generateJobTrailer(prnWriter,
                               formAsMacro,
                               downloadFont,
                               downloadFontRemove,
                               fontMacroRemove,
                               fontIdNo,
                               fontMacroIdNo,
                               symSetUserSet,
                               symSetUserActEmbed,
                               symSetKind1);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e J o b H e a d e r                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write stream initialisation sequences to output file.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateJobHeader(
            BinaryWriter prnWriter,
            PCLFonts.eFontType fontType,
            bool prnDiskFontLoadViaMacro,
            int indxPaperSize,
            int indxPaperType,
            int indxOrientation,
            bool formAsMacro,
            bool optGridVertical,
            ushort logXOffset,
            ushort fontIdNo,
            ushort fontMacroIdNo,
            string fontFilename,
            bool symSetUserSet,
            bool symSetUserActEmbed,
            string symSetUserFile)
        {
            PCLWriter.stdJobHeader(prnWriter, "");

            if ((symSetUserSet) && (symSetUserActEmbed))
            {
                PCLDownloadSymSet.symSetFileCopy(prnWriter, symSetUserFile);
            }

            if (fontType == PCLFonts.eFontType.Download)
            {
                PCLWriter.fontDownloadID(prnWriter, fontIdNo);

                PCLDownloadFont.fontFileCopy(prnWriter, fontFilename);

                PCLWriter.fontDownloadSave(prnWriter, true);
            }
            else if (fontType == PCLFonts.eFontType.PrnDisk)
            {
                if (prnDiskFontLoadViaMacro)
                {
                    PCLWriter.fontFileIdAssociate(prnWriter,
                                                   fontIdNo,
                                                   fontMacroIdNo,
                                                   fontFilename);
                }
                else
                {
                    PCLWriter.fontFileIdAssociate(prnWriter,
                                                   fontIdNo,
                                                   fontFilename);
                }

                PCLWriter.fontDownloadSave(prnWriter, true);
            }

            if (formAsMacro)
            {
                generateOverlay(prnWriter, true, optGridVertical, logXOffset);
            }

            PCLWriter.pageHeader(prnWriter,
                                  indxPaperSize,
                                  indxPaperType,
                                  indxOrientation,
                                  PCLPlexModes.eSimplex);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e J o b T r a i l e r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write job termination sequences to output file.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateJobTrailer(BinaryWriter prnWriter,
                                               bool formAsMacro,
                                               bool downloadFont,
                                               bool downloadFontRemove,
                                               bool fontMacroRemove,
                                               ushort downloadID,
                                               ushort fontMacroIdNo,
                                               bool symSetUserSet,
                                               bool symSetUserActEmbed,
                                               ushort symSetUserNo)
        {
            if ((downloadFont) && (downloadFontRemove))
            {
                PCLWriter.fontDownloadRemove(prnWriter, downloadID);
            }

            if (fontMacroRemove)
            {
                PCLWriter.macroControl(prnWriter,
                                        (short)fontMacroIdNo,
                                        PCLWriter.eMacroControl.Delete);
            }

            if ((symSetUserSet) && (symSetUserActEmbed))
            {
                PCLWriter.symSetDownloadRemove(prnWriter, symSetUserNo);
            }

            PCLWriter.stdJobTrailer(prnWriter, formAsMacro, _macroId);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e O v e r l a y                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write background data sequences to output file.                    //
        // Optionally top and tail these with macro (user-defined stream)     //
        // definition sequences.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateOverlay(BinaryWriter prnWriter,
                                            bool formAsMacro,
                                            bool optGridVertical,
                                            ushort logXOffset)
        {
            const short twoCellWidth = _cellWidth * 2;
            const short twoCellHeight = _cellHeight * 2;

            const short gridWidthInner = _cellWidth * _gridCols;
            const short gridHeightInner = _cellHeight * _gridRows;
            const short gridWidthOuter = _cellWidth * (_gridCols + 2);
            const short gridHeightOuter = _cellHeight * (_gridRows + 2);

            short marginX;

            short posX,
                  posY;

            short stroke,
                  shade;

            //----------------------------------------------------------------//
            //                                                                //
            // Header                                                         //
            //                                                                //
            //----------------------------------------------------------------//

            if (formAsMacro)
                PCLWriter.macroControl(prnWriter, _macroId,
                                       PCLWriter.eMacroControl.StartDef);

            PCLWriter.font(prnWriter, true, "19U", "s1p24v0s3b16602T");

            //----------------------------------------------------------------//
            //                                                                //
            // Block rectangle 1 (includes row headers).                      //
            //                                                                //
            //----------------------------------------------------------------//

            marginX = (short)(_marginX - logXOffset);
            stroke = 8;

            posX = marginX;
            posY = _posYGrid + _cellHeight;

            PCLWriter.rectangleOutline(prnWriter, posX, posY,
                                       gridHeightInner, gridWidthOuter, stroke,
                                       false, false);

            //----------------------------------------------------------------//
            //                                                                //
            // Block rectangle 2 (includes column headers).                   //
            //                                                                //
            //----------------------------------------------------------------//

            posX = (short)(marginX + _cellWidth);
            posY = _posYGrid;

            PCLWriter.rectangleOutline(prnWriter, posX, posY,
                                       gridHeightOuter, gridWidthInner, stroke,
                                       false, false);

            //----------------------------------------------------------------//
            //                                                                //
            // Top-left and bottom-right corners.                             //
            //                                                                //
            //----------------------------------------------------------------//

            shade = 10;

            posX = marginX;
            posY = _posYGrid;

            PCLWriter.rectangleOutline(prnWriter, posX, posY,
                                       _cellHeight, _cellWidth, stroke,
                                       false, false);

            PCLWriter.rectangleShaded(prnWriter, posX, posY,
                                      _cellHeight, _cellWidth, shade,
                                      false, false);

            posX = (short)(marginX + gridWidthInner + _cellWidth);
            posY = _posYGrid + gridHeightInner + _cellHeight;

            PCLWriter.rectangleOutline(prnWriter, posX, posY,
                                       _cellHeight, _cellWidth, stroke,
                                       false, false);

            PCLWriter.rectangleShaded(prnWriter, posX, posY,
                                      _cellHeight, _cellWidth, shade,
                                      false, false);

            //----------------------------------------------------------------//
            //                                                                //
            // Grid lines - Horizontal.                                       //
            //                                                                //
            //----------------------------------------------------------------//

            stroke = 1;

            posX = marginX;
            posY = _posYGrid + twoCellHeight;

            for (int i = 1; i < _gridRows; i++)
            {
                PCLWriter.lineHorizontal(prnWriter, posX, posY, gridWidthOuter,
                                         stroke);

                posY += _cellHeight;
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Grid lines - Vertical.                                         //
            //                                                                //
            //----------------------------------------------------------------//

            posX = (short)(marginX + twoCellWidth);
            posY = _posYGrid;

            for (int i = 1; i < _gridCols; i++)
            {
                PCLWriter.lineVertical(prnWriter, posX, posY, gridHeightOuter,
                                       stroke);

                posX += _cellWidth;
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Shade control code cells.                                      //
            // Position depends on whether the Horizontal or Vertical grid    //
            // option was selected.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            shade = 1;

            posX = (short)(marginX + _cellWidth);
            posY = _posYGrid + _cellHeight;

            if (optGridVertical)
            {
                PCLWriter.rectangleShaded(prnWriter, posX, posY,
                                          gridHeightInner, twoCellWidth, shade,
                                          false, false);

                posX += (_gridDimHalf * _cellWidth);

                PCLWriter.rectangleShaded(prnWriter, posX, posY,
                                          gridHeightInner, twoCellWidth, shade,
                                          false, false);
            }
            else
            {
                PCLWriter.rectangleShaded(prnWriter, posX, posY,
                                          twoCellHeight, gridWidthInner, shade,
                                          false, false);

                posY += (_gridDimHalf * _cellHeight);

                PCLWriter.rectangleShaded(prnWriter, posX, posY,
                                          twoCellHeight, gridWidthInner, shade,
                                          false, false);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Cell column header and trailer labels.                         //
            // Content depends on whether the Horizontal or Vertical grid     //
            // option was selected.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.font(prnWriter, true, "19U", "s0p20h0s0b4099T");

            posX = (short)(marginX + (_cellWidth / 3));
            posY = _posYGrid + _lineSpacing;

            PCLWriter.text(prnWriter, posX, posY, 0, "hex");

            posX = (short)(marginX + gridWidthInner +
                           _cellWidth + (_cellWidth / 5));

            posY = _posYGrid + gridHeightInner + _cellHeight + _lineSpacing;

            PCLWriter.text(prnWriter, posX, posY, 0, "dec");

            //----------------------------------------------------------------//

            PCLWriter.font(prnWriter, true, "19U", "s0p12h0s0b4099T");

            posX = (short)(marginX + _cellWidth + (_cellWidth / 4));
            posY = _posYGrid + _lineSpacing;

            if (optGridVertical)
            {
                for (int i = 0; i < _gridCols; i++)
                {
                    PCLWriter.text(prnWriter, posX, posY, 0, _hexChars[i] + "_");

                    posX += _cellWidth;
                }
            }
            else
            {
                for (int i = 0; i < _gridCols; i++)
                {
                    PCLWriter.text(prnWriter, posX, posY, 0, "_" + _hexChars[i]);

                    posX += _cellWidth;
                }
            }

            posX = (short)(marginX + _cellWidth + (_cellWidth / 5));
            posY = _posYGrid + gridHeightInner + _cellHeight + _lineSpacing;

            if (optGridVertical)
            {
                for (int i = 0; i < _gridCols; i++)
                {
                    string tmpStr = (i * _gridDim).ToString();

                    int len = tmpStr.Length;

                    if (len == 2)
                        tmpStr = " " + tmpStr;
                    else if (len == 1)
                        tmpStr = "  " + tmpStr;

                    PCLWriter.text(prnWriter, posX, posY, 0, tmpStr);

                    posX += _cellWidth;
                }
            }
            else
            {
                for (int i = 0; i < _gridCols; i++)
                {
                    PCLWriter.text(prnWriter, posX, posY, 0, "+" + i);

                    posX += _cellWidth;
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Cell row labels.                                               //
            // Content depends on whether the Horizontal or Vertical grid     //
            // option was selected.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            posX = (short)(marginX + (_cellWidth / 4));
            posY = _posYGrid + _cellHeight + _lineSpacing;

            if (optGridVertical)
            {
                for (int i = 0; i < _gridRows; i++)
                {
                    PCLWriter.text(prnWriter, posX, posY, 0, "_" + _hexChars[i]);

                    posY += _cellHeight;
                }
            }
            else
            {
                for (int i = 0; i < _gridRows; i++)
                {
                    PCLWriter.text(prnWriter, posX, posY, 0, _hexChars[i] + "_");

                    posY += _cellHeight;
                }
            }

            posX = (short)(marginX + gridWidthInner + _cellWidth +
                                                      (_cellWidth / 8));
            posY = _posYGrid + _cellHeight + _lineSpacing;

            if (optGridVertical)
            {
                for (int i = 0; i < _gridRows; i++)
                {
                    PCLWriter.text(prnWriter, posX, posY, 0, "+" + i);

                    posY += _cellHeight;
                }
            }
            else
            {
                for (int i = 0; i < _gridRows; i++)
                {
                    string tmpStr = (i * _gridDim).ToString();

                    int len = tmpStr.Length;

                    if (len == 2)
                        tmpStr = " " + tmpStr;
                    else if (len == 1)
                        tmpStr = "  " + tmpStr;

                    PCLWriter.text(prnWriter, posX, posY, 0, tmpStr);

                    posY += _cellHeight;
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Header text.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.font(prnWriter, true, "19U", "s1p12v0s0b16602T");

            posX = marginX;
            posY = _posYDesc;

            PCLWriter.text(prnWriter, posX, posY, 0, "PCL font:");

            posY += _lineSpacing;

            PCLWriter.text(prnWriter, posX, posY, 0, "Size:");

            posY += _lineSpacing;

            PCLWriter.text(prnWriter, posX, posY, 0, "Symbol set:");

            posY += _lineSpacing;

            PCLWriter.text(prnWriter, posX, posY, 0, "Description:");

            //----------------------------------------------------------------//
            //                                                                //
            // Footer text.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            posY = _posYSelData;

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "PCL font selection sequence:");

            //----------------------------------------------------------------//

            PCLWriter.font(prnWriter, true, "19U", "s1p6v0s0b16602T");

            posX = (short)(marginX + (_cellWidth * _gridDimHalf));

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "(the content of the grid is undefined if no" +
                           " font with these characteristics is available)");

            //----------------------------------------------------------------//
            //                                                                //
            // Overlay end.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            if (formAsMacro)
                PCLWriter.macroControl(prnWriter, _macroId,
                                       PCLWriter.eMacroControl.StopDef);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e P a g e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write individual test data page sequences to output file.          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generatePage(
            BinaryWriter prnWriter,
            PCLFonts.eFontType fontType,
            bool prnDiskFontDataKnown,
            bool formAsMacro,
            bool showC0Chars,
            bool optGridVertical,
            bool fontBound,
            string fontId,
            string fontDesc,
            string symSetId,
            string fontLoadDesc,
            string fontSelDesc,
            string fontSelSeq,
            string symbolSetName,
            ushort sampleRangeOffset,
            PCLTextParsingMethods.eIndex indxTextParseMethod,
            double fontSize,
            bool sizeIsHeight,
            ushort logXOffset,
            bool fontSelectById,
            ushort fontIdNo,
            string fontFilename,
            bool symSetUserSet,
            bool showMapCodesUCS2,
            bool showMapCodesUTF8,
            bool symSetUserActEmbed,
            string symSetUserFile)
        {
            const int rangeC0Max = 0x1f;
            const int singleByteMax = 0xff;
            const int indxMajorC0Start = 0;
            const int indxMajorC0End = 2;
            const int sizeSingleByteSet = 256;

            short marginX = (short)(_marginX - logXOffset);

            short posX,
                  posY,
                  posXStart,
                  posYStart;

            //----------------------------------------------------------------//
            //                                                                //
            // Background image.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            if (formAsMacro)
                PCLWriter.macroControl(prnWriter, _macroId,
                                       PCLWriter.eMacroControl.Call);
            else
                generateOverlay(prnWriter, false, optGridVertical, logXOffset);

            //----------------------------------------------------------------//
            //                                                                //
            // Descriptive text.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.font(prnWriter, true, "19U", "s0p10h0s3b4099T");

            posX = (short)(marginX + ((_cellWidth * 7) / 2));
            posY = _posYDesc;

            if ((fontType == PCLFonts.eFontType.Download) ||
                (fontType == PCLFonts.eFontType.PrnDisk))
            {
                const int maxLen = 51;
                const int halfLen = (maxLen - 5) / 2;

                int len = fontFilename.Length;

                if (len < maxLen)
                    PCLWriter.text(prnWriter, posX, posY, 0, fontFilename);
                else
                    PCLWriter.text(prnWriter, posX, posY, 0,
                                   fontFilename.Substring(0, halfLen) +
                                   " ... " +
                                   fontFilename.Substring(len - halfLen,
                                                          halfLen));
            }
            else
                PCLWriter.text(prnWriter, posX, posY, 0, fontId);

            //----------------------------------------------------------------//

            posY += _lineSpacing;

            if ((fontType == PCLFonts.eFontType.PrnDisk) &&
                (!prnDiskFontDataKnown))
                PCLWriter.text(prnWriter, posX, posY, 0,
                               "characteristics not known");
            else if (sizeIsHeight)
                PCLWriter.text(prnWriter, posX, posY, 0,
                               fontSize.ToString("F2") + " point");
            else
                PCLWriter.text(prnWriter, posX, posY, 0,
                               fontSize.ToString("F2") +
                               " characters-per-inch");

            //----------------------------------------------------------------//

            posY += _lineSpacing;

            if ((fontType == PCLFonts.eFontType.PrnDisk) &&
                (!prnDiskFontDataKnown))
            {
                PCLWriter.text(prnWriter, posX, posY, 0,
                               "characteristics not known");
            }
            else if (sampleRangeOffset == 0)
            {
                PCLWriter.text(prnWriter, posX, posY, 0,
                                symSetId + " (" + symbolSetName + ")");
            }
            else
            {
                string offsetText;

                offsetText = ": Range offset 0x" +
                             sampleRangeOffset.ToString("X4");

                PCLWriter.text(prnWriter, posX, posY, 0,
                                symSetId + " (" + symbolSetName + ")" +
                                offsetText);
            }

            //----------------------------------------------------------------//

            PCLWriter.font(prnWriter, true, "19U", "s0p15h0s3b4099T");

            posY += _lineSpacing;

            PCLWriter.text(prnWriter, posX, posY, 0, fontDesc);

            //----------------------------------------------------------------//

            if (symSetUserSet)
            {
                const int maxLen = 61;
                const int halfLen = (maxLen - 5) / 2;

                int len = symSetUserFile.Length;

                posY += _lineSpacing / 2;

                if (len < maxLen)
                    PCLWriter.text(prnWriter, posX, posY, 0,
                                   "Symbol set file: " + symSetUserFile);
                else
                    PCLWriter.text(prnWriter, posX, posY, 0,
                                   "Symbol set file: " +
                                   symSetUserFile.Substring(0, halfLen) +
                                   " ... " +
                                   symSetUserFile.Substring(len - halfLen,
                                                            halfLen));
            }

            //----------------------------------------------------------------//

            posX = (short)(marginX + _cellWidth);
            posY = _posYSelData + _cellHeight;

            PCLWriter.font(prnWriter, true, "19U", "s0p10h0s3b4099T");

            if (fontLoadDesc != "")
            {
                PCLWriter.text(prnWriter, posX, posY, 0,
                                fontLoadDesc);

                posY += _lineSpacing;
            }

            PCLWriter.text(prnWriter, posX, posY, 0,
                            fontSelDesc);

            //----------------------------------------------------------------//

            if (((fontType == PCLFonts.eFontType.Download) ||
                 (fontType == PCLFonts.eFontType.PrnDisk)) &&
                (fontSelectById))
            {
                if (fontBound)
                    PCLWriter.font(prnWriter, true, "",
                                   (fontIdNo + "X"));
                else
                    PCLWriter.font(prnWriter, true, symSetId,
                                   (fontIdNo + "X"));

                if (fontSelSeq != "")
                    PCLWriter.font(prnWriter, true, "", fontSelSeq);
            }
            else
            {
                PCLWriter.font(prnWriter, true, symSetId, fontSelSeq);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Grid.                                                          //
            //                                                                //
            //----------------------------------------------------------------//

            int startIndxMajor;

            bool checkSingleByteCodes = false;
            bool utf8;
            bool twoByteMethod;
            bool validCodePoint = true;

            bool[] validSingleByteCodes = new bool[sizeSingleByteSet];

            //----------------------------------------------------------------//

            utf8 = false;
            twoByteMethod = false;

            if ((indxTextParseMethod ==
                    PCLTextParsingMethods.eIndex.m83_UTF8) ||
                (indxTextParseMethod ==
                    PCLTextParsingMethods.eIndex.m1008_UTF8_alt))
                utf8 = true;
            else if (indxTextParseMethod ==
                      PCLTextParsingMethods.eIndex.m2_2_byte)
                twoByteMethod = true;

            //----------------------------------------------------------------//

            if (indxTextParseMethod !=
                PCLTextParsingMethods.eIndex.not_specified)
            {
                PCLWriter.textParsingMethod(
                    prnWriter,
                    PCLTextParsingMethods.getValue((int)indxTextParseMethod));

                //------------------------------------------------------------//

                if ((indxTextParseMethod ==
                        PCLTextParsingMethods.eIndex.m83_UTF8) ||
                    (indxTextParseMethod ==
                        PCLTextParsingMethods.eIndex.m1008_UTF8_alt))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // A UTF-8 Text Parsing Method has been specified - the   //
                    // 'character set' may consist of:                        //
                    //                                                        //
                    //  - A mix of single-byte and variable multi-byte        //
                    //    ranges.                                             //
                    //    We expect that all single-byte code-points (before  //
                    //    transformation) should be valid (hence we exclude   //
                    //    UTF-8 from the 'check single bytes'check).          //
                    //                                                        //
                    //--------------------------------------------------------//

                    utf8 = true;

                    checkSingleByteCodes = false;
                }
                else if (indxTextParseMethod ==
                            PCLTextParsingMethods.eIndex.m2_2_byte)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // A Text Parsing Method (other than UTF-8) has been      //
                    // specified where the 'character set' consists of:       //
                    // may consist of:                                        //
                    //                                                        //
                    //  - All double-byte code-point values                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    checkSingleByteCodes = false;
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // A Text Parsing Method (other than UTF-8) has been      //
                    // specified - dependent on method, the 'character set'   //
                    // may consist of:                                        //
                    //                                                        //
                    //  - All single-byte code-point values                   //
                    //  - All double-byte code-point values                   //
                    //  - A mix of single-byte and double-byte ranges         //
                    //                                                        //
                    // For the single-byte range (0x00 -> 0xff), set up a     //
                    // table showing which single-byte values are permitted.  //
                    //                                                        //
                    //--------------------------------------------------------//

                    int blockMin,
                          blockMax;

                    ushort[] rangesSingle =
                        PCLTextParsingMethods.getRangeDataSingle(
                                                (int)indxTextParseMethod);

                    int ctRangeData = 0;

                    utf8 = false;

                    checkSingleByteCodes = true;

                    if (rangesSingle != null)
                        ctRangeData = (rangesSingle.Length / 2);

                    for (int i = 0; i < sizeSingleByteSet; i++)
                    {
                        validSingleByteCodes[i] = false;
                    }

                    for (int i = 0; i < ctRangeData; i += 2)
                    {
                        blockMin = rangesSingle[i];
                        blockMax = rangesSingle[i + 1];

                        for (int j = blockMin; j <= blockMax; j++)
                        {
                            validSingleByteCodes[j] = true;
                        }
                    }
                }
            }

            //----------------------------------------------------------------//

            if ((showC0Chars) || (sampleRangeOffset != 0))
                startIndxMajor = indxMajorC0Start;
            else
                startIndxMajor = indxMajorC0End;

            //----------------------------------------------------------------//

            if (optGridVertical)
            {
                posX = (short)(marginX + (_cellWidth * (startIndxMajor + 1)) +
                                          (_cellWidth / 3));
                posY = (short)(_posYGrid + (_cellHeight * 2 / 3));
            }
            else
            {
                posX = (short)(marginX + (_cellWidth / 3));
                posY = (short)(_posYGrid + (_cellHeight * (startIndxMajor + 1)) +
                                            (_cellHeight * 2 / 3));
            }

            posXStart = posX;
            posYStart = posY;

            //----------------------------------------------------------------//

            for (int indxMajor = startIndxMajor;
                       indxMajor < _gridDim;
                       indxMajor++)
            {
                PCLWriter.cursorPosition(prnWriter, posX, posY);

                PCLWriter.cursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

                for (int indxMinor = 0; indxMinor < _gridDim; indxMinor++)
                {
                    PCLWriter.cursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);

                    if (optGridVertical)
                        PCLWriter.cursorRelative(prnWriter, 0, _cellHeight);
                    else
                        PCLWriter.cursorRelative(prnWriter, _cellWidth, 0);

                    PCLWriter.cursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

                    ushort codeVal = (ushort)(sampleRangeOffset +
                                               (indxMajor * _gridDim) +
                                               indxMinor);

                    validCodePoint = true;

                    if ((checkSingleByteCodes) &&
                        (codeVal <= singleByteMax))
                    {
                        //------------------------------------------------//
                        //                                                //
                        // Text Parsing Method (other than UTF-8)         //
                        // specified; check if this code-point is valid   //
                        // for the method.                                //
                        // Don't print it if it is not in the allowed     //
                        // list appropriate to the method.                //
                        //                                                //
                        //------------------------------------------------//

                        validCodePoint = validSingleByteCodes[codeVal];
                    }

                    if (validCodePoint)
                    {
                        if (utf8)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Convert code-point value to UTF-8 sequence.    //
                            //                                                //
                            // If we are using a user-defined symbol set file //
                            // with an 'index' (rather than 'embed') action,  //
                            // determine the target 'mapped' code-point.      //
                            //                                                //
                            //------------------------------------------------//

                            byte[] utf8Seq = new byte[4];
                            int utf8Len = 0;

                            if ((symSetUserSet) && (!symSetUserActEmbed))
                            {
                                ushort mapVal;

                                if (codeVal <= _symSetUserMapMax)
                                {
                                    mapVal = _symSetUserMap[(int)codeVal];
                                }
                                else
                                {
                                    mapVal = 0xffff;
                                }

                                codeVal = mapVal;
                            }

                            if (codeVal <= rangeC0Max)
                            {
                                //--------------------------------------------//
                                //                                            //
                                // Code-point is in the C0 range - print it   //
                                // via the 'transparent print' mechanism.     //
                                //                                            //
                                //--------------------------------------------//

                                PCLWriter.transparentPrint(prnWriter, 1);

                                byte[] x = new byte[1];

                                x[0] = (byte)(codeVal);

                                prnWriter.Write(x, 0, 1);
                            }
                            else
                            {
                                //--------------------------------------------//
                                //                                            //
                                // Print UTF-8 coded value.                   //
                                //                                            //
                                //--------------------------------------------//

                                PrnParseDataUTF8.convertUTF32ToUTF8Bytes(
                                    codeVal,
                                    ref utf8Len,
                                    ref utf8Seq);

                                prnWriter.Write(utf8Seq, 0, utf8Len);
                            }
                        }
                        else if ((codeVal <= singleByteMax) &&
                                 (!twoByteMethod))
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Not UTF-8 and not using two-byte parse method. //
                            // Convert code-point to single-byte value.       //
                            // If code-point is in the C0 range, print it via //
                            // the 'transparent print' mechanism.             //
                            //                                                //
                            //------------------------------------------------//

                            if (codeVal <= rangeC0Max)
                            {
                                PCLWriter.transparentPrint(prnWriter, 1);
                            }

                            byte[] x = new byte[1];

                            x[0] = (byte)(codeVal);

                            prnWriter.Write(x, 0, 1);
                        }
                        else
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Not UTF-8; convert code-point to double-byte   //
                            // value.                                         //
                            // If code-point is in the C0 range, print it via //
                            // the 'transparent print' mechanism.             //
                            //                                                //
                            //------------------------------------------------//

                            if (codeVal <= rangeC0Max)
                            {
                                PCLWriter.transparentPrint(prnWriter, 2);
                            }

                            byte[] x = new byte[2];

                            x[0] = (byte)((codeVal >> 8) & 0xff);
                            x[1] = (byte)(codeVal & 0xff);

                            prnWriter.Write(x, 0, 2);
                        }
                    }
                }

                PCLWriter.cursorPushPop(prnWriter,
                         PCLWriter.ePushPop.Pop);

                if (optGridVertical)
                    posX += _cellWidth;
                else
                    posY += _cellHeight;
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Resets, etc.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            if (indxTextParseMethod !=
                PCLTextParsingMethods.eIndex.not_specified)
            {
                PCLWriter.textParsingMethod(
                    prnWriter,
                    PCLTextParsingMethods.eIndex.m0_1_byte_default);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Mapping of target code-points.                                 //
            //                                                                //
            //----------------------------------------------------------------//

            if (showMapCodesUCS2)
            {
                if ((showC0Chars) || (sampleRangeOffset != 0))
                    startIndxMajor = indxMajorC0Start;
                else
                    startIndxMajor = indxMajorC0End;

                //------------------------------------------------------------//
                //                                                            //
                // Unicode target code-point value (print at top of cell).    //
                // Print target Unicode code-points referenced by the         //
                // user-defined symbol set file, or the current Unicode range //
                // (whichever is relevant).                                   //
                //                                                            //
                //------------------------------------------------------------//

                posX = (short)(posXStart - ((_cellWidth * 7) / 24));
                posY = (short)(posYStart - (_cellHeight / 2));

                PCLWriter.font(prnWriter, true, "19U", "s1p6v0s0b16602T");

                for (int indxMajor = startIndxMajor;
                           indxMajor < _gridDim;
                           indxMajor++)
                {
                    ushort codeVal,
                           mapVal;

                    PCLWriter.cursorPosition(prnWriter, posX, posY);

                    PCLWriter.cursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

                    for (int indxMinor = 0; indxMinor < _gridDim; indxMinor++)
                    {
                        PCLWriter.cursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);

                        if (optGridVertical)
                            PCLWriter.cursorRelative(prnWriter, 0, _cellHeight);
                        else
                            PCLWriter.cursorRelative(prnWriter, _cellWidth, 0);

                        PCLWriter.cursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

                        codeVal = (ushort)(((indxMajor * _gridDim) +
                                             indxMinor) + sampleRangeOffset);

                        if (symSetUserSet)
                        {
                            if (codeVal <= _symSetUserMapMax)
                            {
                                mapVal = _symSetUserMap[(int)codeVal];
                            }
                            else
                            {
                                mapVal = 0xffff;
                            }
                        }
                        else
                        {
                            mapVal = codeVal;
                        }

                        if (mapVal != 0xffff)
                        {
                            string seq = mapVal.ToString("X4");
                            int seqLen = seq.Length;

                            prnWriter.Write(("U+" + seq).ToCharArray(),
                                            0, seqLen + 2);
                        }
                    }

                    PCLWriter.cursorPushPop(prnWriter,
                         PCLWriter.ePushPop.Pop);

                    if (optGridVertical)
                        posX += _cellWidth;
                    else
                        posY += _cellHeight;
                }
            }

            if (showMapCodesUTF8)
            {
                if ((showC0Chars) || (sampleRangeOffset != 0))
                    startIndxMajor = indxMajorC0Start;
                else
                    startIndxMajor = indxMajorC0End;

                //------------------------------------------------------------//
                //                                                            //
                // Equivalent UTF-8 representation of target code-point       //
                // value (print at bottom of cell).                           //
                //                                                            //
                //------------------------------------------------------------//

                posX = (short)(posXStart - ((_cellWidth * 7) / 24));
                posY = (short)(posYStart + ((_cellHeight * 3) / 10));

                PCLWriter.font(prnWriter, true, "19U", "s1p5v0s0b16602T");

                for (int indxMajor = startIndxMajor;
                           indxMajor < _gridDim;
                           indxMajor++)
                {
                    ushort codeVal,
                           mapVal;

                    PCLWriter.cursorPosition(prnWriter, posX, posY);

                    PCLWriter.cursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

                    for (int indxMinor = 0; indxMinor < _gridDim; indxMinor++)
                    {
                        PCLWriter.cursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);

                        if (optGridVertical)
                            PCLWriter.cursorRelative(prnWriter, 0, _cellHeight);
                        else
                            PCLWriter.cursorRelative(prnWriter, _cellWidth, 0);

                        PCLWriter.cursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

                        codeVal = (ushort)(((indxMajor * _gridDim) +
                                             indxMinor) + sampleRangeOffset);

                        if (symSetUserSet)
                        {
                            if (codeVal <= _symSetUserMapMax)
                            {
                                mapVal = _symSetUserMap[(int)codeVal];
                            }
                            else
                            {
                                mapVal = 0xffff;
                            }
                        }
                        else
                        {
                            mapVal = codeVal;
                        }

                        if (mapVal != 0xffff)
                        {
                            string utf8Hex = null;
                            int utf8HexLen = 0;

                            PrnParseDataUTF8.convertUTF32ToUTF8HexString(
                                mapVal,
                                true,
                                ref utf8Hex);

                            utf8HexLen = utf8Hex.Length;

                            prnWriter.Write(utf8Hex.ToCharArray(),
                                             0, utf8HexLen);
                        }
                    }

                    PCLWriter.cursorPushPop(prnWriter,
                         PCLWriter.ePushPop.Pop);

                    if (optGridVertical)
                        posX += _cellWidth;
                    else
                        posY += _cellHeight;
                }
            }

            //----------------------------------------------------------------//

            PCLWriter.formFeed(prnWriter);
        }
    }
}
