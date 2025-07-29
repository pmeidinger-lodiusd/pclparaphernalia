using System.IO;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides PCL XL support for the Text and Background element
/// of the Text Modification action of the MiscSamples tool.
/// 
/// © Chris Hutchinson 2014
/// 
/// </summary>

static class ToolMiscSamplesActTxtModPatPCLXL
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    const string _formName = "MiscTxtModPatForm";

    const int _symSet_19U = 629;
    const ushort _unitsPerInch = PCLXLWriter._sessionUPI;

    const short _pageOriginX = (_unitsPerInch * 1);
    const short _pageOriginY = (_unitsPerInch * 1);
    const short _incInch = (_unitsPerInch * 1);
    const short _lineInc = (_unitsPerInch * 5) / 6;

    const short _posXDesc = _pageOriginX;
    const short _posXData = _pageOriginX + (2 * _incInch);

    const short _posYHddr = _pageOriginY;
    const short _posYDesc = _pageOriginY + (2 * _incInch);
    const short _posYData = _pageOriginY + (2 * _incInch);

    const short _patternId_DarkGrey = 301;
    const short _patternId_LightGrey = 302;

    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Static variables.                                                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    static readonly short _fontIndexArial = PCLFonts.GetIndexForName("Arial");
    static readonly short _fontIndexCourier = PCLFonts.GetIndexForName("Courier");

    static readonly string _fontNameArial =
        PCLFonts.GetPCLXLName(_fontIndexArial,
                              PCLFonts.eVariant.Regular);
    static readonly string _fontNameCourier =
        PCLFonts.GetPCLXLName(_fontIndexCourier,
                              PCLFonts.eVariant.Regular);
    static readonly string _fontNameCourierBold =
        PCLFonts.GetPCLXLName(_fontIndexCourier,
                              PCLFonts.eVariant.Bold);

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

    public static void GenerateJob(BinaryWriter prnWriter,
                                   int indxPaperSize,
                                   int indxPaperType,
                                   int indxOrientation,
                                   bool formAsMacro)
    {
        GenerateJobHeader(prnWriter);

        if (formAsMacro)
            GenerateOverlay(prnWriter, true, indxPaperSize, indxOrientation);

        GeneratePage(prnWriter,
                     indxPaperSize,
                     indxPaperType,
                     indxOrientation,
                     formAsMacro);

        GenerateJobTrailer(prnWriter, formAsMacro);
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
    // Write tray map termination sequences to output file.               //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateJobTrailer(BinaryWriter prnWriter,
                                           bool formAsMacro)
    {
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
                                        int indxPaperSize,
                                        int indxOrientation)
    {
        const int lenBuf = 1024;

        byte[] buffer = new byte[lenBuf];

        short ptSize;

        int indBuf;

        short posX,
              posY;

        ushort boxX1,
               boxX2,
               boxY1,
               boxY2;

        short rectX,
              rectY,
              rectHeight,
              rectWidth;

        byte stroke = 1;

        //----------------------------------------------------------------//

        indBuf = 0;

        //----------------------------------------------------------------//
        //                                                                //
        // Header                                                         //
        // Parts of overlay use different brush and/or pen definitions,   //
        // so enclosed in a GS block.                                     //
        //                                                                //
        //----------------------------------------------------------------//

        if (formAsMacro)
            PCLXLWriter.StreamHeader(prnWriter, true, _formName);

        PCLXLWriter.AddOperator(ref buffer,
                          ref indBuf,
                          PCLXLOperators.eTag.PushGS);

        //----------------------------------------------------------------//
        //                                                                //
        // Colour space, pen & brush definitions.                         //
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
                           stroke);

        PCLXLWriter.AddOperator(ref buffer,
                          ref indBuf,
                          PCLXLOperators.eTag.SetPenWidth);

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                               buffer, ref indBuf);

        //----------------------------------------------------------------//
        //                                                                //
        // Box.                                                           //
        //                                                                //
        //----------------------------------------------------------------//

        boxX1 = _unitsPerInch / 2;  // half-inch left margin
        boxY1 = _unitsPerInch / 2;  // half-inch top-margin

        boxX2 = (ushort)(PCLPaperSizes.GetPaperWidth(
                                indxPaperSize, _unitsPerInch,
                                PCLOrientations.eAspect.Portrait) -
                          boxX1);

        boxY2 = (ushort)(PCLPaperSizes.GetPaperLength(
                                indxPaperSize, _unitsPerInch,
                                PCLOrientations.eAspect.Portrait) -
                          boxY1);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.eTag.TxMode,
                           (byte)PCLXLAttrEnums.eVal.eTransparent);

        PCLXLWriter.AddOperator(ref buffer,
                          ref indBuf,
                          PCLXLOperators.eTag.SetPatternTxMode);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.eTag.TxMode,
                           (byte)PCLXLAttrEnums.eVal.eTransparent);

        PCLXLWriter.AddOperator(ref buffer,
                          ref indBuf,
                          PCLXLOperators.eTag.SetSourceTxMode);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.eTag.GrayLevel,
                           100);

        PCLXLWriter.AddOperator(ref buffer,
                          ref indBuf,
                          PCLXLOperators.eTag.SetPenSource);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.eTag.PenWidth,
                           5);

        PCLXLWriter.AddOperator(ref buffer,
                          ref indBuf,
                          PCLXLOperators.eTag.SetPenWidth);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.eTag.NullBrush,
                           0);

        PCLXLWriter.AddOperator(ref buffer,
                          ref indBuf,
                          PCLXLOperators.eTag.SetBrushSource);

        PCLXLWriter.AddAttrUint16XY(ref buffer,
                              ref indBuf,
                              PCLXLAttributes.eTag.EllipseDimension,
                              100, 100);

        PCLXLWriter.AddAttrUint16Box(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.BoundingBox,
                               boxX1, boxY1, boxX2, boxY2);

        PCLXLWriter.AddOperator(ref buffer,
                          ref indBuf,
                          PCLXLOperators.eTag.RoundRectangle);

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                               buffer, ref indBuf);

        //----------------------------------------------------------------//
        //                                                                //
        // Text.                                                          //
        //                                                                //
        //----------------------------------------------------------------//

        PCLXLWriter.AddAttrUbyte(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.eTag.GrayLevel,
                           100);

        PCLXLWriter.AddOperator(ref buffer,
                          ref indBuf,
                          PCLXLOperators.eTag.SetBrushSource);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.eTag.NullPen,
                           0);

        PCLXLWriter.AddOperator(ref buffer,
                          ref indBuf,
                          PCLXLOperators.eTag.SetPenSource);

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                               buffer, ref indBuf);

        ptSize = 15;

        PCLXLWriter.Font(prnWriter, formAsMacro, ptSize,
                         _symSet_19U, _fontNameCourierBold);

        posX = _posXDesc;
        posY = _posYHddr;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                   "PCL XL Text & Background:");

        ptSize = 12;

        PCLXLWriter.Font(prnWriter, formAsMacro, ptSize,
                         _symSet_19U, _fontNameCourier);

        posY = _posYDesc;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                  "Black:");

        posY += _lineInc;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                   "Shade = " + "Dark Gray:");

        posY += _lineInc;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                  "Shade = " + "Light Gray:");

        posY += _lineInc;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                  "White:");

        //----------------------------------------------------------------//
        //                                                                //
        // Background shading.                                            //
        //                                                                //
        //----------------------------------------------------------------//

        PatternDefineDpi300(prnWriter, formAsMacro);

        rectHeight = (_lineInc * 3) / 5;
        rectWidth = (_unitsPerInch * 9) / 10;

        posX = _posXData;
        posY = _posYData - (_lineInc / 2);

        //----------------------------------------------------------------//

        PCLXLWriter.AddAttrUbyte(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.eTag.GrayLevel,
                           0);

        PCLXLWriter.AddOperator(ref buffer,
                                 ref indBuf,
                                 PCLXLOperators.eTag.SetBrushSource);

        rectX = posX;
        rectY = posY;

        for (int i = 0; i < 4; i++)
        {
            PCLXLWriter.AddAttrUint16Box(ref buffer,
                                         ref indBuf,
                                         PCLXLAttributes.eTag.BoundingBox,
                                         (ushort)rectX,
                                         (ushort)rectY,
                                         (ushort)(rectX + rectWidth),
                                         (ushort)(rectY + rectHeight));

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.Rectangle);

            rectY += _lineInc;
        }

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                               buffer, ref indBuf);

        //----------------------------------------------------------------//

        PCLXLWriter.AddAttrSint16(ref buffer,
                                   ref indBuf,
                                   PCLXLAttributes.eTag.PatternSelectID,
                                   _patternId_DarkGrey);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.PatternOrigin,
                                     0, 0);

        PCLXLWriter.AddOperator(ref buffer,
                                 ref indBuf,
                                 PCLXLOperators.eTag.SetBrushSource);

        rectX += rectWidth;
        rectY = posY;

        for (int i = 0; i < 4; i++)
        {
            PCLXLWriter.AddAttrUint16Box(ref buffer,
                                         ref indBuf,
                                         PCLXLAttributes.eTag.BoundingBox,
                                         (ushort)rectX,
                                         (ushort)rectY,
                                         (ushort)(rectX + rectWidth),
                                         (ushort)(rectY + rectHeight));

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.Rectangle);

            rectY += _lineInc;
        }

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                               buffer, ref indBuf);

        //----------------------------------------------------------------//

        PCLXLWriter.AddAttrSint16(ref buffer,
                                   ref indBuf,
                                   PCLXLAttributes.eTag.PatternSelectID,
                                   _patternId_LightGrey);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.PatternOrigin,
                                     0, 0);

        PCLXLWriter.AddOperator(ref buffer,
                                 ref indBuf,
                                 PCLXLOperators.eTag.SetBrushSource);

        rectX += rectWidth;
        rectY = posY;

        for (int i = 0; i < 4; i++)
        {
            PCLXLWriter.AddAttrUint16Box(ref buffer,
                                         ref indBuf,
                                         PCLXLAttributes.eTag.BoundingBox,
                                         (ushort)rectX,
                                         (ushort)rectY,
                                         (ushort)(rectX + rectWidth),
                                         (ushort)(rectY + rectHeight));

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.Rectangle);

            rectY += _lineInc;
        }

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                               buffer, ref indBuf);

        //----------------------------------------------------------------//
        //                                                                //
        // Overlay end.                                                   //
        //                                                                //
        //----------------------------------------------------------------//

        PCLXLWriter.AddOperator(ref buffer,
                          ref indBuf,
                          PCLXLOperators.eTag.PopGS);

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                               buffer, ref indBuf);

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

    private static void GeneratePage(BinaryWriter prnWriter,
                                     int indxPaperSize,
                                     int indxPaperType,
                                     int indxOrientation,
                                     bool formAsMacro)
    {
        const int sizeStd = 1024;

        byte[] bufStd = new byte[sizeStd];

        string sampleText = "000000000000000";

        short posX,
              posY;

        int indStd;

        short ptSize;

        indStd = 0;

        //----------------------------------------------------------------//

        if (indxOrientation < PCLOrientations.GetCount())
        {
            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.Orientation,
                               PCLOrientations.GetIdPCLXL(indxOrientation));
        }

        if (indxPaperSize < PCLPaperSizes.GetCount())
        {
            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.MediaSize,
                               PCLPaperSizes.GetIdPCLXL(indxPaperSize));
        }

        if ((indxPaperType < PCLPaperTypes.GetCount()) &&
            (PCLPaperTypes.GetType(indxPaperType) !=
                PCLPaperTypes.eEntryType.NotSet))
        {
            PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                                    ref indStd,
                                    PCLXLAttributes.eTag.MediaType,
                                    PCLPaperTypes.GetName(indxPaperType));
        }

        PCLXLWriter.AddAttrUbyte(ref bufStd,
                           ref indStd,
                           PCLXLAttributes.eTag.SimplexPageMode,
                           (byte)PCLXLAttrEnums.eVal.eSimplexFrontSide);

        PCLXLWriter.AddOperator(ref bufStd,
                          ref indStd,
                          PCLXLOperators.eTag.BeginPage);

        PCLXLWriter.AddAttrUint16XY(ref bufStd,
                              ref indStd,
                              PCLXLAttributes.eTag.PageOrigin,
                              0, 0);

        PCLXLWriter.AddOperator(ref bufStd,
                          ref indStd,
                          PCLXLOperators.eTag.SetPageOrigin);

        PCLXLWriter.AddAttrUbyte(ref bufStd,
                           ref indStd,
                           PCLXLAttributes.eTag.ColorSpace,
                           (byte)PCLXLAttrEnums.eVal.eGray);

        PCLXLWriter.AddOperator(ref bufStd,
                          ref indStd,
                          PCLXLOperators.eTag.SetColorSpace);

        prnWriter.Write(bufStd, 0, indStd);
        indStd = 0;

        //----------------------------------------------------------------//

        if (formAsMacro)
        {
            PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                                    ref indStd,
                                    PCLXLAttributes.eTag.StreamName,
                                    _formName);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.ExecStream);

            prnWriter.Write(bufStd, 0, indStd);
            indStd = 0;
        }
        else
        {
            GenerateOverlay(prnWriter, false,
                            indxPaperSize, indxOrientation);
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Text.                                                          //
        //                                                                //
        //----------------------------------------------------------------//

        ptSize = 34;

        PCLXLWriter.Font(prnWriter, false, ptSize,
                         _symSet_19U, _fontNameArial);

        PCLXLWriter.AddAttrUbyte(ref bufStd,
                           ref indStd,
                           PCLXLAttributes.eTag.NullPen,
                           0);

        PCLXLWriter.AddOperator(ref bufStd,
                          ref indStd,
                          PCLXLOperators.eTag.SetPenSource);

        //----------------------------------------------------------------//
        // Black                                                          //
        //----------------------------------------------------------------//

        posX = _posXData;
        posY = _posYData;

        PCLXLWriter.AddAttrUbyte(ref bufStd,
                           ref indStd,
                           PCLXLAttributes.eTag.GrayLevel,
                           0);

        PCLXLWriter.AddOperator(ref bufStd,
                          ref indStd,
                          PCLXLOperators.eTag.SetBrushSource);

        prnWriter.Write(bufStd, 0, indStd);
        indStd = 0;

        PCLXLWriter.Text(prnWriter, false, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                   sampleText);

        //----------------------------------------------------------------//
        // Shade 1                                                        //
        //----------------------------------------------------------------//

        posY += _lineInc;

        PCLXLWriter.AddAttrSint16(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.PatternSelectID,
                                   _patternId_DarkGrey);

        PCLXLWriter.AddAttrSint16XY(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.eTag.PatternOrigin,
                                     0, 0);

        PCLXLWriter.AddOperator(ref bufStd,
                                 ref indStd,
                                 PCLXLOperators.eTag.SetBrushSource);

        prnWriter.Write(bufStd, 0, indStd);
        indStd = 0;

        PCLXLWriter.Text(prnWriter, false, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                   sampleText);

        //----------------------------------------------------------------//
        // Shade 2                                                        //
        //----------------------------------------------------------------//

        posY += _lineInc;

        PCLXLWriter.AddAttrSint16(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.PatternSelectID,
                                   _patternId_LightGrey);

        PCLXLWriter.AddAttrSint16XY(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.eTag.PatternOrigin,
                                     0, 0);

        PCLXLWriter.AddOperator(ref bufStd,
                                 ref indStd,
                                 PCLXLOperators.eTag.SetBrushSource);

        prnWriter.Write(bufStd, 0, indStd);
        indStd = 0;

        PCLXLWriter.Text(prnWriter, false, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                   sampleText);

        //----------------------------------------------------------------//
        // White                                                          //
        //----------------------------------------------------------------//

        posY += _lineInc;

        PCLXLWriter.AddAttrUbyte(ref bufStd,
                           ref indStd,
                           PCLXLAttributes.eTag.GrayLevel,
                           255);

        PCLXLWriter.AddOperator(ref bufStd,
                          ref indStd,
                          PCLXLOperators.eTag.SetBrushSource);

        prnWriter.Write(bufStd, 0, indStd);
        indStd = 0;

        PCLXLWriter.Text(prnWriter, false, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                   sampleText);

        //----------------------------------------------------------------//

        PCLXLWriter.AddAttrUint16(ref bufStd,
                            ref indStd,
                            PCLXLAttributes.eTag.PageCopies,
                            1);

        PCLXLWriter.AddOperator(ref bufStd,
                          ref indStd,
                          PCLXLOperators.eTag.EndPage);

        prnWriter.Write(bufStd, 0, indStd);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p a t t e r n D e f i n e D p i 3 0 0                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Define 300 dots-per-inch user-defined patterns.                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void PatternDefineDpi300(BinaryWriter prnWriter,
                                             bool formAsMacro)
    {
        const ushort patWidth = 16;
        const ushort patHeight = 16;

        const ushort destWidth =
            (patWidth * _unitsPerInch) / 300;
        const ushort destHeight =
            (patHeight * _unitsPerInch) / 300;

        byte[] pattern_LightGrey =
                           { 0x00, 0x00, 0x60, 0x60,
                             0x60, 0x60, 0x00, 0x00,
                             0x00, 0x00, 0x06, 0x06,
                             0x06, 0x06, 0x00, 0x00,
                             0x00, 0x00, 0x60, 0x60,
                             0x60, 0x60, 0x00, 0x00,
                             0x00, 0x00, 0x06, 0x06,
                             0x06, 0x06, 0x00, 0x00 };

        byte[] pattern_DarkGrey =
                           { 0xC1, 0xC1, 0xEB, 0xEB,
                             0xC1, 0xC1, 0x88, 0x88,
                             0x1C, 0x1C, 0xBE, 0xBE,
                             0x1C, 0x1C, 0x88, 0x88,
                             0xC1, 0xC1, 0xEB, 0xEB,
                             0xC1, 0xC1, 0x88, 0x88,
                             0x1C, 0x1C, 0xBE, 0xBE,
                             0x1C, 0x1C, 0x88, 0x88 };

        PCLXLWriter.PatternDefine(prnWriter,
                                   formAsMacro,
                                   _patternId_LightGrey,
                                   patWidth,
                                   patHeight,
                                   destWidth,
                                   destHeight,
                                   PCLXLAttrEnums.eVal.eIndexedPixel,
                                   PCLXLAttrEnums.eVal.e1Bit,
                                   PCLXLAttrEnums.eVal.ePagePattern,
                                   PCLXLAttrEnums.eVal.eNoCompression,
                                   pattern_LightGrey);

        PCLXLWriter.PatternDefine(prnWriter,
                                   formAsMacro,
                                   _patternId_DarkGrey,
                                   patWidth,
                                   patHeight,
                                   destWidth,
                                   destHeight,
                                   PCLXLAttrEnums.eVal.eIndexedPixel,
                                   PCLXLAttrEnums.eVal.e1Bit,
                                   PCLXLAttrEnums.eVal.ePagePattern,
                                   PCLXLAttrEnums.eVal.eNoCompression,
                                   pattern_DarkGrey);
    }
}
