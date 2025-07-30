using System.IO;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides PCL XL support for the FontSample tool.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

static class ToolFontSamplePCLXL
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    const string _formName = "FontSampleForm";
    const string _hexChars = "0123456789ABCDEF";

    const int _symSet_19U = 629;
    const short _gridDim = 16;
    const int _gridDimHalf = _gridDim / 2;
    const short _gridCols = _gridDim;
    const short _gridRows = _gridDim;
    const ushort _unitsPerInch = PCLXLWriter._sessionUPI;

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

    public static void generateJob(BinaryWriter prnWriter,
                                   int indxPaperSize,
                                   int indxPaperType,
                                   int indxOrientation,
                                   bool formAsMacro,
                                   bool showC0Chars,
                                   bool optGridVertical,
                                   string fontId,
                                   string fontDesc,
                                   ushort symbolSet,
                                   string fontName,
                                   string symbolSetName,
                                   ushort[] sampleRangeOffsets,
                                   double pointSize,
                                   bool downloadFont,
                                   bool downloadFontRemove,
                                   string fontFilename,
                                   bool symSetUserSet,
                                   bool showMapCodesUCS2,
                                   bool showMapCodesUTF8,
                                   string symSetUserFile)
    {
        int pageCt = sampleRangeOffsets.Length;

        ushort symSetUserMapMax = 0;

        ushort[] symSetUserMap = null;

        //----------------------------------------------------------------//

        GenerateJobHeader(prnWriter);

        if (formAsMacro)
            GenerateOverlay(prnWriter, true, optGridVertical);

        if (downloadFont)
        {
            PCLXLDownloadFont.FontFileCopy(prnWriter, fontFilename);
        }

        //----------------------------------------------------------------//

        if (symSetUserSet)
        {
            symSetUserMapMax = PCLSymbolSets.GetMapArrayMax(
                      PCLSymbolSets.IndexUserSet);

            symSetUserMap = new ushort[symSetUserMapMax + 1];

            symSetUserMap = PCLSymbolSets.GetMapArrayUserSet();
        }

        //----------------------------------------------------------------//

        for (int i = 0; i < pageCt; i++)
        {
            ushort rangeOffset = sampleRangeOffsets[i];

            generatePage(prnWriter,
                     indxPaperSize,
                     indxPaperType,
                     indxOrientation,
                     formAsMacro,
                     showC0Chars,
                     optGridVertical,
                     fontId,
                     fontDesc,
                     symbolSet,
                     fontName,
                     symbolSetName,
                     rangeOffset,
                     pointSize,
                     downloadFont,
                     fontFilename,
                     symSetUserSet,
                     showMapCodesUCS2,
                     showMapCodesUTF8,
                     symSetUserFile,
                     symSetUserMapMax,
                     symSetUserMap);
        }

        GenerateJobTrailer(prnWriter, formAsMacro,
                           downloadFont, downloadFontRemove, fontName);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e J o b H e a d e r                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write stream initialisation sequences to output file.              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateJobHeader(BinaryWriter prnWriter)
    {
        PCLXLWriter.StdJobHeader(prnWriter, string.Empty);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e J o b T r a i l e r                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write job termination sequences to output file.                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateJobTrailer(BinaryWriter prnWriter,
                                           bool formAsMacro,
                                           bool downloadFont,
                                           bool downloadFontRemove,
                                           string fontName)
    {
        if ((downloadFont) && (downloadFontRemove))
        {
            PCLXLWriter.FontRemove(prnWriter, false, fontName);
        }

        PCLXLWriter.StdJobTrailer(prnWriter, formAsMacro, _formName);
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

    private static void GenerateOverlay(BinaryWriter prnWriter,
                                        bool formAsMacro,
                                        bool optGridVertical)
    {
        const short twoCellWidth = _cellWidth * 2;
        const short twoCellHeight = _cellHeight * 2;

        const short gridWidthInner = _cellWidth * _gridCols;
        const short gridHeightInner = _cellHeight * _gridRows;
        const short gridWidthOuter = _cellWidth * (_gridCols + 2);
        const short gridHeightOuter = _cellHeight * (_gridRows + 2);

        const int lenBuf = 1024;
        const short patternID = 1;
        const ushort patWidth = 24;
        const ushort patHeight = 24;

        byte[] buffer = new byte[lenBuf];

        byte[] pattern_1 = { 0x00, 0x0c, 0x00, 0x00, 0x00, 0x00,
                             0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                             0x00, 0xc0, 0xc0, 0x00, 0x00, 0x00,
                             0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                             0x0c, 0x00, 0x0c, 0x00, 0x00, 0x00,
                             0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                             0xc0, 0x00, 0x00, 0x00, 0x00, 0x00,
                             0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                             0x0c, 0x00, 0x0c, 0x00, 0x00, 0x00,
                             0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                             0x00, 0xc0, 0xc0, 0x00, 0x00, 0x00,
                             0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        short crntPtSize;

        short posX1,
              posX2,
              posY1,
              posY2;

        int indBuf = 0;

        //----------------------------------------------------------------//
        //                                                                //
        // Header                                                         //
        //                                                                //
        //----------------------------------------------------------------//

        if (formAsMacro)
        {
            PCLXLWriter.StreamHeader(prnWriter, true, _formName);
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Colour space, pen & brush definitions.                         //
        //                                                                //
        //----------------------------------------------------------------//

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.ColorSpace,
                                 (byte)PCLXLAttrEnums.eVal.eGray);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetColorSpace);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.NullBrush,
                                 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetBrushSource);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.GrayLevel,
                                 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetPenSource);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.PenWidth,
                                 8);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetPenWidth);

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                     buffer, ref indBuf);

        //----------------------------------------------------------------//
        //                                                                //
        // Block rectangle 1 (includes row headers).                      //
        //                                                                //
        //----------------------------------------------------------------//

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.NewPath);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.Point,
                                    _marginX, _posYGrid);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetCursor);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.Point,
                                    0, _cellHeight);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetCursorRel);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.EndPoint,
                                    gridWidthOuter, 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.LineRelPath);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.EndPoint,
                                    0, gridHeightInner);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.LineRelPath);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.EndPoint,
                                    -gridWidthOuter, 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.LineRelPath);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.EndPoint,
                                    0, -gridHeightInner);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.LineRelPath);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.PaintPath);

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                     buffer, ref indBuf);

        //----------------------------------------------------------------//
        //                                                                //
        // Block rectangle 2 (includes column headers).                   //
        //                                                                //
        //----------------------------------------------------------------//

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.NewPath);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.Point,
                                    _marginX, _posYGrid);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetCursor);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.Point,
                                    _cellWidth, 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetCursorRel);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.EndPoint,
                                    gridWidthInner, 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.LineRelPath);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.EndPoint,
                                    0, gridHeightOuter);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.LineRelPath);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.EndPoint,
                                    -gridWidthInner, 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.LineRelPath);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.EndPoint,
                                    0, -gridHeightOuter);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.LineRelPath);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.PaintPath);

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                     buffer, ref indBuf);

        //----------------------------------------------------------------//
        //                                                                //
        // Top-left and bottom-right corners.                             //
        //                                                                //
        //----------------------------------------------------------------//

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.GrayLevel,
                                 224);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetBrushSource);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.NewPath);

        PCLXLWriter.AddAttrSint16Box(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.BoundingBox,
                                     _marginX,
                                     _posYGrid,
                                     _marginX + twoCellWidth,
                                     _posYGrid + twoCellHeight);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.StartPoint,
                                    _marginX + _cellWidth,
                                    _posYGrid);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.EndPoint,
                                    _marginX,
                                    _posYGrid + _cellHeight);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.ArcDirection,
                                 (byte)PCLXLAttrEnums.eVal.eCounterClockWise);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.ArcPath);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.EndPoint,
                                    _cellWidth, 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.LineRelPath);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.EndPoint,
                                    0, -_cellHeight);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.LineRelPath);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.PaintPath);

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                     buffer, ref indBuf);

        posX1 = _marginX + gridWidthOuter - twoCellWidth;
        posY1 = _posYGrid + gridHeightOuter - twoCellHeight;

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.NewPath);

        PCLXLWriter.AddAttrSint16Box(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.BoundingBox,
                                     posX1,
                                     posY1,
                                     (short)(posX1 + twoCellWidth),
                                     (short)(posY1 + twoCellHeight));

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.StartPoint,
                                    (short)(posX1 + _cellWidth),
                                    (short)(posY1 + twoCellHeight));

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.EndPoint,
                                    (short)(posX1 + twoCellWidth),
                                    (short)(posY1 + _cellHeight));

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.ArcDirection,
                                 (byte)PCLXLAttrEnums.eVal.eCounterClockWise);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.ArcPath);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.EndPoint,
                                    -_cellWidth, 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.LineRelPath);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.EndPoint,
                                    0, _cellHeight);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.LineRelPath);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.PaintPath);

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                     buffer, ref indBuf);

        //----------------------------------------------------------------//
        //                                                                //
        // Grid lines - Horizontal.                                       //
        //                                                                //
        //----------------------------------------------------------------//

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.NullBrush,
                                 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetBrushSource);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.PenWidth,
                                 1);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetPenWidth);

        //----------------------------------------------------------------//

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.NewPath);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.Point,
                                    _marginX, _posYGrid);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetCursor);

        posX1 = 0;                      // relative movement X
        posY1 = twoCellHeight;          // relative movement Y
        posX2 = gridWidthOuter;         // relative draw X
        posY2 = 0;                      // relative draw Y

        for (int i = 1; i < _gridRows; i++)
        {
            PCLXLWriter.AddAttrSint16XY(ref buffer,
                                        ref indBuf,
                                        PCLXLAttributes.eTag.Point,
                                        posX1, posY1);

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.SetCursorRel);

            PCLXLWriter.AddAttrSint16XY(ref buffer,
                                        ref indBuf,
                                        PCLXLAttributes.eTag.EndPoint,
                                        posX2, posY2);

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.LineRelPath);

            if (i == 1)
            {
                posX1 = -gridWidthOuter;
                posY1 = _cellHeight;
            }
        }

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.PaintPath);

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                     buffer, ref indBuf);

        //----------------------------------------------------------------//
        //                                                                //
        // Grid lines - Vertical.                                         //
        //                                                                //
        //----------------------------------------------------------------//

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.NewPath);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.Point,
                                    _marginX, _posYGrid);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetCursor);

        posX1 = twoCellWidth;           // relative movement X
        posY1 = 0;                      // relative movement Y
        posX2 = 0;                      // relative draw X
        posY2 = gridHeightOuter;        // relative draw Y

        for (int i = 1; i < _gridCols; i++)
        {
            PCLXLWriter.AddAttrSint16XY(ref buffer,
                                        ref indBuf,
                                        PCLXLAttributes.eTag.Point,
                                        posX1, posY1);

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.SetCursorRel);

            PCLXLWriter.AddAttrSint16XY(ref buffer,
                                        ref indBuf,
                                        PCLXLAttributes.eTag.EndPoint,
                                        posX2, posY2);

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.LineRelPath);

            if (i == 1)
            {
                posX1 = _cellWidth;
                posY1 = -gridHeightOuter;
            }
        }

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.PaintPath);

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                     buffer, ref indBuf);

        //----------------------------------------------------------------//
        //                                                                //
        // Shade control code cells.                                      //
        // Position depends on whether the Horizontal or Vertical grid    //
        // option was selected.                                           //
        //                                                                //
        //----------------------------------------------------------------//

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.ColorSpace,
                                 (byte)PCLXLAttrEnums.eVal.eGray);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.PaletteDepth,
                                 (byte)PCLXLAttrEnums.eVal.e8Bit);

        PCLXLWriter.AddAttrUbyteArray(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.eTag.PaletteData,
                                      2,
                                      PCLXLWriter.monoPalette);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetColorSpace);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.NullPen,
                                 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetPenSource);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.TxMode,
                                 (byte)PCLXLAttrEnums.eVal.eTransparent);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetPatternTxMode);

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                     buffer, ref indBuf);

        PCLXLWriter.PatternDefine(prnWriter,
                                   formAsMacro,
                                   patternID,
                                   patWidth,
                                   patHeight,
                                   patWidth,
                                   patHeight,
                                   PCLXLAttrEnums.eVal.eIndexedPixel,
                                   PCLXLAttrEnums.eVal.e1Bit,
                                   PCLXLAttrEnums.eVal.eTempPattern,
                                   PCLXLAttrEnums.eVal.eNoCompression,
                                   pattern_1);

        PCLXLWriter.AddAttrSint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.PatternSelectID,
                                  patternID);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.PatternOrigin,
                                    0, 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetBrushSource);

        posX1 = _marginX + _cellWidth;
        posY1 = _posYGrid + _cellHeight;

        if (optGridVertical)
        {
            posX2 = (short)(posX1 + twoCellWidth);
            posY2 = (short)(posY1 + gridHeightInner);

            PCLXLWriter.AddAttrUint16Box(ref buffer,
                                         ref indBuf,
                                         PCLXLAttributes.eTag.BoundingBox,
                                         (ushort)posX1,
                                         (ushort)posY1,
                                         (ushort)posX2,
                                         (ushort)posY2);

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.Rectangle);

            posX1 += (_cellWidth * _gridDimHalf);
            posX2 = (short)(posX1 + twoCellWidth);

            PCLXLWriter.AddAttrUint16Box(ref buffer,
                                         ref indBuf,
                                         PCLXLAttributes.eTag.BoundingBox,
                                         (ushort)posX1,
                                         (ushort)posY1,
                                         (ushort)posX2,
                                         (ushort)posY2);

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.Rectangle);
        }
        else
        {
            posX2 = (short)(posX1 + gridWidthInner);
            posY2 = (short)(posY1 + twoCellHeight);

            PCLXLWriter.AddAttrUint16Box(ref buffer,
                                         ref indBuf,
                                         PCLXLAttributes.eTag.BoundingBox,
                                         (ushort)posX1,
                                         (ushort)posY1,
                                         (ushort)posX2,
                                         (ushort)posY2);

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.Rectangle);

            posY1 += (_cellHeight * _gridDimHalf);
            posY2 = (short)(posY1 + twoCellHeight);

            PCLXLWriter.AddAttrUint16Box(ref buffer,
                                         ref indBuf,
                                         PCLXLAttributes.eTag.BoundingBox,
                                         (ushort)posX1,
                                         (ushort)posY1,
                                         (ushort)posX2,
                                         (ushort)posY2);

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.Rectangle);
        }

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                     buffer, ref indBuf);

        //----------------------------------------------------------------//
        //                                                                //
        // Cell column header and trailer labels.                         //
        // Content depends on whether the Horizontal or Vertical grid     //
        // option was selected.                                           //
        //                                                                //
        //----------------------------------------------------------------//

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.GrayLevel,
                                 128);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetBrushSource);

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                     buffer, ref indBuf);

        //----------------------------------------------------------------//

        crntPtSize = 6;

        PCLXLWriter.Font(prnWriter, formAsMacro, 6,
                         _symSet_19U, "Courier       Bd");

        posX1 = _marginX + (_cellWidth / 3);
        posY1 = _posYGrid + _lineSpacing;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                         PCLXLWriter.advances_Courier, crntPtSize,
                         posX1, posY1, "hex");

        posX1 = _marginX + gridWidthInner + _cellWidth + (_cellWidth / 5);
        posY1 = _posYGrid + gridHeightInner + _cellHeight + _lineSpacing;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                         PCLXLWriter.advances_Courier, crntPtSize,
                         posX1, posY1, "dec");

        //----------------------------------------------------------------//

        crntPtSize = 10;

        PCLXLWriter.Font(prnWriter, formAsMacro, crntPtSize,
                         _symSet_19U, "Courier       Bd");

        posX1 = _marginX + ((_cellWidth * 17) / 20);
        posY1 = _posYGrid + _lineSpacing;

        if (optGridVertical)
        {
            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                             PCLXLWriter.advances_Courier, crntPtSize,
                             posX1, posY1,
                             "  0_  1_  2_  3_  4_  5_  6_  7_" +
                             "  8_  9_  A_  B_  C_  D_  E_  F_");
        }
        else
        {
            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                             PCLXLWriter.advances_Courier, crntPtSize,
                             posX1, posY1,
                             "  _0  _1  _2  _3  _4  _5  _6  _7" +
                             "  _8  _9  _A  _B  _C  _D  _E  _F");
        }

        posY1 = _posYGrid + gridHeightInner + _cellHeight + _lineSpacing;

        if (optGridVertical)
        {
            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                             PCLXLWriter.advances_Courier, crntPtSize,
                             posX1, posY1,
                             "   0  16  32  48  64  80  96 112" +
                             " 128 144 160 176 192 208 224 240");
        }
        else
        {
            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                             PCLXLWriter.advances_Courier, crntPtSize,
                             posX1, posY1,
                             "  +0  +1  +2  +3  +4  +5  +6  +7" +
                             "  +8  +9 +10 +11 +12 +13 +14 +15");
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Cell row labels.                                               //
        // Content depends on whether the Horizontal or Vertical grid     //
        // option was selected.                                           //
        //                                                                //
        //----------------------------------------------------------------//

        posX1 = _marginX + (_cellWidth / 4);
        posY1 = _posYGrid + _cellHeight + _lineSpacing;

        if (optGridVertical)
        {
            for (int i = 0; i < _gridRows; i++)
            {
                PCLXLWriter.Text(prnWriter, formAsMacro, false,
                                 PCLXLWriter.advances_Courier, crntPtSize,
                                 posX1, posY1,
                                 "_" + _hexChars[i].ToString());

                posY1 += _cellHeight;
            }
        }
        else
        {
            for (int i = 0; i < _gridRows; i++)
            {
                PCLXLWriter.Text(prnWriter, formAsMacro, false,
                                 PCLXLWriter.advances_Courier, crntPtSize,
                                 posX1, posY1,
                                 _hexChars[i].ToString() + "_");

                posY1 += _cellHeight;
            }
        }

        posX1 = _marginX + gridWidthInner + _cellWidth + (_cellWidth / 8);
        posY1 = _posYGrid + _cellHeight + _lineSpacing;

        if (optGridVertical)
        {
            for (int i = 0; i < _gridRows; i++)
            {
                PCLXLWriter.Text(prnWriter, formAsMacro, false,
                                 PCLXLWriter.advances_Courier, crntPtSize,
                                 posX1, posY1,
                                 "+" + i);

                posY1 += _cellHeight;
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

                PCLXLWriter.Text(prnWriter, formAsMacro, false,
                                 PCLXLWriter.advances_Courier, crntPtSize,
                                 posX1, posY1,
                                 tmpStr);

                posY1 += _cellHeight;
            }
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Header text.                                                   //
        //                                                                //
        //----------------------------------------------------------------//

        crntPtSize = 12;

        PCLXLWriter.Font(prnWriter, formAsMacro, crntPtSize,
                         _symSet_19U, "Arial         Bd");

        posX1 = _marginX;
        posY1 = _posYDesc;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                         PCLXLWriter.advances_ArialBold, crntPtSize,
                         posX1, posY1, "PCL XL font:");

        posY1 += _lineSpacing;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                         PCLXLWriter.advances_ArialBold, crntPtSize,
                         posX1, posY1, "Size:");

        posY1 += _lineSpacing;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                         PCLXLWriter.advances_ArialBold, crntPtSize,
                         posX1, posY1, "Symbol set:");

        posY1 += _lineSpacing;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                         PCLXLWriter.advances_ArialBold, crntPtSize,
                         posX1, posY1, "Description:");

        //----------------------------------------------------------------//
        //                                                                //
        // Footer text.                                                   //
        //                                                                //
        //----------------------------------------------------------------//

        posX1 = _marginX;
        posY1 = _posYSelData;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                         PCLXLWriter.advances_ArialBold, crntPtSize,
                         posX1, posY1, "PCL XL font characteristics:");

        posX1 += _cellWidth;
        posY1 += _cellHeight;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                         PCLXLWriter.advances_ArialBold, crntPtSize,
                         posX1, posY1, "Font name:");

        posY1 += _lineSpacing;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                         PCLXLWriter.advances_ArialBold, crntPtSize,
                         posX1, posY1, "Symbol set:");

        posY1 += _lineSpacing;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                         PCLXLWriter.advances_ArialBold, crntPtSize,
                         posX1, posY1, "Size (@ " + _unitsPerInch + " upi):");

        //----------------------------------------------------------------//

        crntPtSize = 6;

        PCLXLWriter.Font(prnWriter, formAsMacro, crntPtSize,
                         _symSet_19U, "Arial         Bd");

        posX1 = _marginX + (_cellWidth * _gridDimHalf);
        posY1 = _posYSelData;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                         PCLXLWriter.advances_ArialBold, crntPtSize,
                         posX1, posY1,
                         "(the content of the grid is undefined if no" +
                         " font with these characteristics is available)");

        //----------------------------------------------------------------//
        //                                                                //
        // Overlay end.                                                   //
        //                                                                //
        //----------------------------------------------------------------//

        if (formAsMacro)
        {
            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.EndStream);

            prnWriter.Write(buffer, 0, indBuf);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e P a g e                                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write individual test data page sequences to output file.          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void generatePage(BinaryWriter prnWriter,
                                     int indxPaperSize,
                                     int indxPaperType,
                                     int indxOrientation,
                                     bool formAsMacro,
                                     bool showC0Chars,
                                     bool optGridVertical,
                                     string fontId,
                                     string fontDesc,
                                     ushort symSetKind1,
                                     string fontName,
                                     string symbolSetName,
                                     int sampleRangeOffset,
                                     double pointSize,
                                     bool downloadFont,
                                     string fontFilename,
                                     bool symSetUserSet,
                                     bool showMapCodesUCS2,
                                     bool showMapCodesUTF8,
                                     string symSetUserFile,
                                     ushort symSetUserMapMax,
                                     ushort[] symSetUserMap)
    {
        const int indxMajorC0Start = 0;
        const int indxMajorC0End = 2;
        const int lenBuf = 1024;

        byte[] buffer = new byte[lenBuf];

        short posX,
              posY,
              posXStart,
              posYStart;

        float crntPtSize;
        float charSize;
        string displayCharSize;

        string symSetId;

        //----------------------------------------------------------------//
        //                                                                //
        // Page initialisation.                                           //
        //                                                                //
        //----------------------------------------------------------------//

        int indBuf = 0;

        symSetId = PCLSymbolSets.TranslateKind1ToId(symSetKind1);
        charSize = PCLXLWriter.GetCharSize((float)pointSize);
        displayCharSize = charSize.ToString("F2");

        if (indxOrientation < PCLOrientations.GetCount())
        {
            PCLXLWriter.AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.Orientation,
                                     PCLOrientations.GetIdPCLXL(indxOrientation));
        }

        if (indxPaperSize < PCLPaperSizes.GetCount())
        {
            PCLXLWriter.AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.MediaSize,
                                     PCLPaperSizes.GetIdPCLXL(indxPaperSize));
        }

        if ((indxPaperType < PCLPaperTypes.GetCount()) &&
            (PCLPaperTypes.GetType(indxPaperType) != PCLPaperTypes.eEntryType.NotSet))
        {
            PCLXLWriter.AddAttrUbyteArray(ref buffer,
                                          ref indBuf,
                                          PCLXLAttributes.eTag.MediaType,
                                          PCLPaperTypes.GetName(indxPaperType));
        }

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.SimplexPageMode,
                                 (byte)PCLXLAttrEnums.eVal.eSimplexFrontSide);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.BeginPage);

        PCLXLWriter.AddAttrUint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.PageOrigin,
                                    0, 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetPageOrigin);

        //----------------------------------------------------------------//
        //                                                                //
        // Background image.                                              //
        //                                                                //
        //----------------------------------------------------------------//

        if (formAsMacro)
        {
            PCLXLWriter.AddAttrUbyteArray(ref buffer,
                                          ref indBuf,
                                          PCLXLAttributes.eTag.StreamName,
                                          _formName);

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.ExecStream);

            prnWriter.Write(buffer, 0, indBuf);
            indBuf = 0;
        }
        else
        {
            prnWriter.Write(buffer, 0, indBuf);
            indBuf = 0;

            GenerateOverlay(prnWriter, false, optGridVertical);
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Descriptive text.                                              //
        //                                                                //
        //----------------------------------------------------------------//

        crntPtSize = 12;

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.GrayLevel,
                                 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetBrushSource);

        prnWriter.Write(buffer, 0, indBuf);
        indBuf = 0;

        PCLXLWriter.Font(prnWriter, false, crntPtSize,
                         _symSet_19U, "Courier       Bd");

        posX = _marginX + ((_cellWidth * 7) / 2);
        posY = _posYDesc;

        if (downloadFont)
        {
            const int maxLen = 51;
            const int halfLen = (maxLen - 5) / 2;

            string tempId;

            int len = fontFilename.Length;

            if (len < maxLen)
                tempId = fontFilename;
            else
                tempId = fontFilename.Substring(0, halfLen) + " ... " +
                         fontFilename.Substring(len - halfLen, halfLen);

            PCLXLWriter.Text(prnWriter, false, false,
                             PCLXLWriter.advances_Courier, crntPtSize,
                             posX, posY, tempId);
        }
        else
        {
            PCLXLWriter.Text(prnWriter, false, false,
                             PCLXLWriter.advances_Courier, crntPtSize,
                             posX, posY, fontId);
        }

        posY += _lineSpacing;

        PCLXLWriter.Text(prnWriter, false, false,
                         PCLXLWriter.advances_Courier, crntPtSize,
                         posX, posY, pointSize.ToString() + " point");

        posY += _lineSpacing;

        if (sampleRangeOffset == 0)
        {
            PCLXLWriter.Text(prnWriter, false, false,
                             PCLXLWriter.advances_Courier, crntPtSize,
                             posX, posY, symSetId +
                                         " (" + symbolSetName + ")");
        }
        else
        {
            string offsetText;

            offsetText = ": Range offset 0x" +
                         sampleRangeOffset.ToString("X4");

            PCLXLWriter.Text(prnWriter, false, false,
                             PCLXLWriter.advances_Courier, crntPtSize,
                             posX, posY, symSetId +
                                         " (" + symbolSetName + ")" +
                                         offsetText);
        }

        crntPtSize = 8;

        PCLXLWriter.Font(prnWriter, false, crntPtSize,
                         _symSet_19U, "Courier       Bd");

        posY += _lineSpacing;

        PCLXLWriter.Text(prnWriter, false, false,
                         PCLXLWriter.advances_Courier, crntPtSize,
                         posX, posY, fontDesc);

        //----------------------------------------------------------------//

        if (symSetUserSet)
        {
            const int maxLen = 61;
            const int halfLen = (maxLen - 5) / 2;

            int len = symSetUserFile.Length;

            posY += _lineSpacing / 2;

            if (len < maxLen)
                PCLXLWriter.Text(prnWriter, false, false,
                                 PCLXLWriter.advances_Courier, crntPtSize,
                                 posX, posY,
                                 "Symbol set file: " + symSetUserFile);
            else
                PCLXLWriter.Text(prnWriter, false, false,
                                 PCLXLWriter.advances_Courier, crntPtSize,
                                 posX, posY,
                                 "Symbol set file: " +
                                 symSetUserFile.Substring(0, halfLen) +
                                 " ... " +
                                 symSetUserFile.Substring(len - halfLen,
                                                          halfLen));
        }

        //----------------------------------------------------------------//

        crntPtSize = 12;

        PCLXLWriter.Font(prnWriter, false, crntPtSize,
                         _symSet_19U, "Courier       Bd");

        posX = _marginX + (_cellWidth * 6);
        posY = _posYSelData + _cellHeight;

        PCLXLWriter.Text(prnWriter, false, false,
                         PCLXLWriter.advances_Courier, crntPtSize,
                         posX, posY, fontName);

        posY += _lineSpacing;

        PCLXLWriter.Text(prnWriter, false, false,
                         PCLXLWriter.advances_Courier, crntPtSize,
                         posX, posY, symSetKind1.ToString());

        posY += _lineSpacing;

        PCLXLWriter.Text(prnWriter, false, false,
                         PCLXLWriter.advances_Courier, crntPtSize,
                         posX, posY, displayCharSize);

        //----------------------------------------------------------------//
        //                                                                //
        // Grid.                                                          //
        //                                                                //
        //----------------------------------------------------------------//

        crntPtSize = (float)pointSize;

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.GrayLevel,
                                 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetBrushSource);

        prnWriter.Write(buffer, 0, indBuf);
        indBuf = 0;

        PCLXLWriter.Font(prnWriter, false, crntPtSize,
                         symSetKind1, fontName);

        //----------------------------------------------------------------//

        int startIndxMajor;
        int startRow;
        int startCol;
        short rowSize;

        if ((showC0Chars) || (sampleRangeOffset != 0))
            startIndxMajor = indxMajorC0Start;
        else
            startIndxMajor = indxMajorC0End;

        //----------------------------------------------------------------//

        if (optGridVertical)
        {
            startRow = 0;
            startCol = startIndxMajor;

            rowSize = (short)(_gridDim - startIndxMajor);

            posX = (short)(_marginX + (_cellWidth * (startIndxMajor + 1)) +
                                      (_cellWidth / 3));
            posY = _posYGrid + _cellHeight +
                                      (_cellHeight * 2 / 3);
        }
        else
        {
            startRow = startIndxMajor;
            startCol = 0;

            rowSize = _gridDim;

            posX = _marginX + _cellWidth + (_cellWidth / 3);
            posY = (short)(_posYGrid + (_cellHeight * (startIndxMajor + 1)) +
                                       (_cellHeight * 2 / 3));
        }

        posXStart = posX;
        posYStart = posY;

        short[] tmpAdvance = new short[rowSize];

        for (int i = 0; i < rowSize; i++)
        {
            tmpAdvance[i] = _cellWidth;
        }

        for (int row = startRow;
                   row < _gridDim;
                   row++)
        {
            int indxMajor;

            PCLXLWriter.AddAttrSint16XY(ref buffer,
                                        ref indBuf,
                                        PCLXLAttributes.eTag.Point,
                                        posX, posY);

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.SetCursor);

            indxMajor = startIndxMajor + (row - startRow);

            if ((sampleRangeOffset == 0) && (!symSetUserSet))
            {
                //--------------------------------------------------------//
                //                                                        //
                // No range offset specified.                             //
                //                                                        //
                //--------------------------------------------------------//

                byte[] codes8Bit = new byte[rowSize];

                if (optGridVertical)
                {
                    for (int col = 0;
                               col < rowSize;
                               col++)
                    {
                        codes8Bit[col] =
                            (byte)(((startIndxMajor + col) * _gridDim) +
                                   row);
                    }
                }
                else
                {
                    for (int col = 0;
                               col < rowSize;
                               col++)
                    {
                        codes8Bit[col] = (byte)((indxMajor * _gridDim) +
                                                col);
                    }
                }

                PCLXLWriter.AddAttrUbyteArray(
                    ref buffer,
                    ref indBuf,
                    PCLXLAttributes.eTag.TextData,
                    rowSize,
                    codes8Bit);
            }
            else
            {
                //--------------------------------------------------------//
                //                                                        //
                // Range offset specified (only for symbol set Unicode),  //
                // or a User-defined PCL Symbol Set definition is in use. //
                //                                                        //
                // The Unicode standard is such that the offset could  be //
                // up to 0x10ff00; but PCL XL only uses ubyte or uint16   //
                // arrays for text characters, hence we are limited to    //
                // offsets of 0xff00 or less.                             // 
                //                                                        //
                //--------------------------------------------------------//

                ushort[] codes16Bit = new ushort[rowSize];

                if (optGridVertical)
                {
                    for (int col = 0;
                               col < rowSize;
                               col++)
                    {
                        codes16Bit[col] =
                            (ushort)(sampleRangeOffset +
                                      ((startIndxMajor + col) * _gridDim) +
                                       row);
                    }
                }
                else
                {
                    for (int col = 0;
                               col < rowSize;
                               col++)
                    {
                        codes16Bit[col] =
                            (ushort)(sampleRangeOffset +
                                      (indxMajor * _gridDim) + col);
                    }
                }

                if (symSetUserSet)
                {
                    //----------------------------------------------------//
                    //                                                    //
                    // map the local code-points to the target            //
                    // code-points defined by the PCL User-Defined Symbol //
                    // Set file.                                          //
                    //                                                    //
                    //----------------------------------------------------//

                    int indx;

                    for (int i = 0; i < rowSize; i++)
                    {
                        indx = codes16Bit[i];

                        if (indx < symSetUserMapMax)
                        {
                            codes16Bit[i] = symSetUserMap[indx];
                        }
                        else
                        {
                            codes16Bit[i] = 0xffff; // not a character
                        }
                    }
                }

                PCLXLWriter.AddAttrUint16Array(
                    ref buffer,
                    ref indBuf,
                    PCLXLAttributes.eTag.TextData,
                    rowSize,
                    codes16Bit);
            }

            PCLXLWriter.AddAttrUbyteArray(
                ref buffer,
                ref indBuf,
                PCLXLAttributes.eTag.XSpacingData,
                rowSize,
                tmpAdvance);

            PCLXLWriter.AddOperator(ref buffer,
                                     ref indBuf,
                                     PCLXLOperators.eTag.Text);

            posY += _cellHeight;

            prnWriter.Write(buffer, 0, indBuf);
            indBuf = 0;
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Mapping of target code-points.                                 //
        //                                                                //
        //----------------------------------------------------------------//

        if (showMapCodesUCS2)
        {
            short posXStartCopy = posXStart;
            short posYStartCopy = posYStart;

            //------------------------------------------------------------//
            //                                                            //
            // Unicode target code-point value (print at top of cell).    //
            // Print target Unicode code-points referenced by the         //
            // user-defined symbol set file.                              //
            //                                                            //
            //------------------------------------------------------------//

            crntPtSize = 6;

            posXStart -= ((_cellWidth * 7) / 24);
            posYStart -= (_cellHeight / 2);

            posX = posXStart;
            posY = posYStart;

            PCLXLWriter.Font(prnWriter, false, crntPtSize,
                             _symSet_19U, "Arial           ");

            for (int indxMajor = startIndxMajor;
                       indxMajor < _gridDim;
                       indxMajor++)
            {
                ushort codeVal,
                       mapVal;

                for (int indxMinor = 0; indxMinor < _gridDim; indxMinor++)
                {
                    codeVal = (ushort)(((indxMajor * _gridDim) +
                                         indxMinor) + sampleRangeOffset);

                    if (symSetUserSet)
                    {
                        if (codeVal <= symSetUserMapMax)
                        {
                            mapVal = symSetUserMap[codeVal];
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
                        PCLXLWriter.Text(prnWriter, false, false,
                                         PCLXLWriter.advances_ArialRegular,
                                         crntPtSize,
                                         posX, posY,
                                         "U+" + mapVal.ToString("X4"));
                    }

                    if (optGridVertical)
                        posY += _cellHeight;
                    else
                        posX += _cellWidth;
                }

                if (optGridVertical)
                    posXStart += _cellWidth;
                else
                    posYStart += _cellHeight;

                posX = posXStart;
                posY = posYStart;
            }

            posXStart = posXStartCopy;
            posYStart = posYStartCopy;
        }

        if (showMapCodesUTF8)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Equivalent UTF-8 representation of target code-point       //
            // value (print at bottom of cell).                           //
            //                                                            //
            //------------------------------------------------------------//

            crntPtSize = 5;

            posXStart -= ((_cellWidth * 7) / 24);
            posYStart += ((_cellHeight * 3) / 10);

            posX = posXStart;
            posY = posYStart;

            PCLXLWriter.Font(prnWriter, false, crntPtSize,
                             _symSet_19U, "Arial           ");

            for (int indxMajor = startIndxMajor;
                       indxMajor < _gridDim;
                       indxMajor++)
            {
                ushort codeVal,
                       mapVal;

                for (int indxMinor = 0; indxMinor < _gridDim; indxMinor++)
                {
                    codeVal = (ushort)(((indxMajor * _gridDim) +
                                         indxMinor) + sampleRangeOffset);

                    if (symSetUserSet)
                    {
                        if (codeVal <= symSetUserMapMax)
                        {
                            mapVal = symSetUserMap[codeVal];
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

                        PrnParseDataUTF8.ConvertUTF32ToUTF8HexString(
                            mapVal,
                            true,
                            ref utf8Hex);

                        PCLXLWriter.Text(prnWriter, false, false,
                                         PCLXLWriter.advances_ArialRegular,
                                         crntPtSize,
                                         posX, posY,
                                         utf8Hex);
                    }

                    if (optGridVertical)
                        posY += _cellHeight;
                    else
                        posX += _cellWidth;
                }

                if (optGridVertical)
                    posXStart += _cellWidth;
                else
                    posYStart += _cellHeight;

                posX = posXStart;
                posY = posYStart;
            }
        }

        //----------------------------------------------------------------//

        PCLXLWriter.AddAttrUint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.PageCopies,
                                  1);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.EndPage);

        prnWriter.Write(buffer, 0, indBuf);
    }
}
