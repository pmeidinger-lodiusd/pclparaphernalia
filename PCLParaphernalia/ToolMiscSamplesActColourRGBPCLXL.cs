using System.IO;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides support for the PCLXL RGB element of the
/// Colour action of the MiscSamples tool.
/// 
/// © Chris Hutchinson 2014
/// 
/// </summary>

static class ToolMiscSamplesActColourRGBPCLXL
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    const string _formName = "MiscSamplesForm";

    const int _symSet_19U = 629;
    const ushort _unitsPerInch = PCLXLWriter._sessionUPI;
    const short _patternId_1 = 601;

    const short _pageOriginX = (_unitsPerInch * 1);
    const short _pageOriginY = (_unitsPerInch * 1);
    const short _incInch = (_unitsPerInch * 1);
    const short _lineInc = (_unitsPerInch * 5) / 6;
    const short _colInc = (_unitsPerInch * 3) / 2;

    const short _posXDesc = _pageOriginX;
    const short _posXDesc1 = _posXDesc + ((_incInch * 15) / 4);
    const short _posXDesc2 = _posXDesc + ((_incInch * 5) / 2);
    const short _posXDesc3 = _posXDesc;
    const short _posXDesc4 = _posXDesc;

    const short _posYHddr = _pageOriginY;
    const short _posYDesc1 = _pageOriginY + (_incInch);
    const short _posYDesc2 = _pageOriginY + ((_incInch * 5) / 4);
    const short _posYDesc3 = _pageOriginY + ((_incInch * 7) / 4);
    const short _posYDesc4 = _pageOriginY + (_incInch * 2);

    const short _posXData = _posXDesc + ((_incInch * 5) / 2);
    const short _posYData = _pageOriginY + ((_incInch * 7) / 4);

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
                                    int[] sampleDef,
                                    bool formAsMacro)
    {
        GenerateJobHeader(prnWriter);

        if (formAsMacro)
            GenerateOverlay(prnWriter, true,
                             indxPaperSize, indxOrientation);

        GeneratePage(prnWriter,
                     indxPaperSize,
                     indxPaperType,
                     indxOrientation,
                     sampleDef,
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
        //                                                                //
        //----------------------------------------------------------------//

        if (formAsMacro)
        {
            PCLXLWriter.StreamHeader(prnWriter, true, _formName);
        }

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
                   "PCL XL RGB colour mode:");

        //----------------------------------------------------------------//

        ptSize = 12;

        PCLXLWriter.Font(prnWriter, formAsMacro, ptSize,
                         _symSet_19U, _fontNameCourier);

        posY += _incInch / 2;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                   "Sample 4-colour palette:");

        //----------------------------------------------------------------//

        posX = _posXDesc2;
        posY = _posYDesc2;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                   "RGB");

        //----------------------------------------------------------------//

        posX = _posXDesc3;
        posY = _posYDesc3;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                   "index");

        posX += _incInch;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                       "value");

        //----------------------------------------------------------------//

        posX = _posXDesc4;
        posY = _posYDesc4;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                   "0");

        posY += _lineInc;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                   "1");

        posY += _lineInc;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                   "2");

        posY += _lineInc;

        PCLXLWriter.Text(prnWriter, formAsMacro, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                   "3");

        //----------------------------------------------------------------//
        //                                                                //
        // Background shade.                                              //
        //                                                                //
        //----------------------------------------------------------------//

        rectX = _posXDesc2 - (_incInch / 4);
        rectY = _posYDesc2 + (_incInch / 4);
        rectWidth = (_incInch * 13) / 10;
        rectHeight = (_incInch * 7) / 2;

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

        PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                               buffer, ref indBuf);

        PatternDefineDpi600(prnWriter, _patternId_1, formAsMacro);

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

        PCLXLWriter.AddAttrSint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.PatternSelectID,
                                  601);

        PCLXLWriter.AddAttrSint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.PatternOrigin,
                                    0, 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetBrushSource);

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
                                     int[] sampleDef,
                                     bool formAsMacro)
    {
        const int sizeStd = 1024;
        const int sizeRGB = 3;

        byte[] bufStd = new byte[sizeStd];

        short posX,
              posY,
              rectX,
              rectY,
              rectHeight,
              rectWidth;

        int indStd;

        short ptSize;

        int temp;

        byte[] palette_0 = new byte[sizeRGB],
                palette_1 = new byte[sizeRGB],
                palette_2 = new byte[sizeRGB],
                palette_3 = new byte[sizeRGB];

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

        rectHeight = _lineInc / 2;
        rectWidth = _lineInc;

        //----------------------------------------------------------------//
        //                                                                //
        // Colour definitions.                                            //
        //                                                                //
        //----------------------------------------------------------------//

        temp = sampleDef[0];

        palette_0[2] = (byte)(temp & 0xff);
        temp >>= 8;
        palette_0[1] = (byte)(temp & 0xff);
        temp >>= 8;
        palette_0[0] = (byte)(temp & 0xff);

        temp = sampleDef[1];

        palette_1[2] = (byte)(temp & 0xff);
        temp >>= 8;
        palette_1[1] = (byte)(temp & 0xff);
        temp >>= 8;
        palette_1[0] = (byte)(temp & 0xff);

        temp = sampleDef[2];

        palette_2[2] = (byte)(temp & 0xff);
        temp >>= 8;
        palette_2[1] = (byte)(temp & 0xff);
        temp >>= 8;
        palette_2[0] = (byte)(temp & 0xff);

        temp = sampleDef[3];

        palette_3[2] = (byte)(temp & 0xff);
        temp >>= 8;
        palette_3[1] = (byte)(temp & 0xff);
        temp >>= 8;
        palette_3[0] = (byte)(temp & 0xff);

        //----------------------------------------------------------------//
        //                                                                //
        // Write details.                                                 //
        //                                                                //
        //----------------------------------------------------------------//

        ptSize = 12;

        PCLXLWriter.Font(prnWriter, false, ptSize,
                         _symSet_19U, _fontNameCourier);

        posX = _posXDesc;
        posY = _posYHddr;

        //----------------------------------------------------------------//

        posX = _posXDesc4;
        posY = _posYDesc4;

        posX += _incInch;

        PCLXLWriter.Text(prnWriter, false, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                       "0x" +
                       palette_0[0].ToString("x2") +
                       palette_0[1].ToString("x2") +
                       palette_0[2].ToString("x2"));

        posY += _lineInc;

        PCLXLWriter.Text(prnWriter, false, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                       "0x" +
                       palette_1[0].ToString("x2") +
                       palette_1[1].ToString("x2") +
                       palette_1[2].ToString("x2"));

        posY += _lineInc;

        PCLXLWriter.Text(prnWriter, false, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                       "0x" +
                       palette_2[0].ToString("x2") +
                       palette_2[1].ToString("x2") +
                       palette_2[2].ToString("x2"));

        posY += _lineInc;

        PCLXLWriter.Text(prnWriter, false, false,
                   PCLXLWriter.advances_Courier, ptSize,
                   posX, posY,
                       "0x" +
                       palette_3[0].ToString("x2") +
                       palette_3[1].ToString("x2") +
                       palette_3[2].ToString("x2"));

        //----------------------------------------------------------------//
        //                                                                //
        // RGB colour space.                                              //
        //                                                                //
        //----------------------------------------------------------------//

        posX = _posXData;
        posY = _posYData;

        rectX = posX;
        rectY = posY;

        PCLXLWriter.AddAttrUbyte(ref bufStd,
                           ref indStd,
                           PCLXLAttributes.eTag.ColorSpace,
                           (byte)PCLXLAttrEnums.eVal.eRGB);

        PCLXLWriter.AddOperator(ref bufStd,
                          ref indStd,
                          PCLXLOperators.eTag.SetColorSpace);

        PCLXLWriter.AddAttrUbyte(ref bufStd,
                                 ref indStd,
                                 PCLXLAttributes.eTag.NullPen,
                                 0);

        PCLXLWriter.AddOperator(ref bufStd,
                          ref indStd,
                          PCLXLOperators.eTag.SetPenSource);

        PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                           ref indStd,
                           PCLXLAttributes.eTag.RGBColor,
                           sizeRGB,
                           palette_0);

        PCLXLWriter.AddOperator(ref bufStd,
                          ref indStd,
                          PCLXLOperators.eTag.SetBrushSource);

        PCLXLWriter.AddAttrUint16Box(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.eTag.BoundingBox,
                                     (ushort)rectX,
                                     (ushort)rectY,
                                     (ushort)(rectX + rectWidth),
                                     (ushort)(rectY + rectHeight));

        PCLXLWriter.AddOperator(ref bufStd,
                                ref indStd,
                                PCLXLOperators.eTag.Rectangle);

        prnWriter.Write(bufStd, 0, indStd);
        indStd = 0;

        //----------------------------------------------------------------//

        rectY += _lineInc;

        PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                           ref indStd,
                           PCLXLAttributes.eTag.RGBColor,
                           sizeRGB,
                           palette_1);

        PCLXLWriter.AddOperator(ref bufStd,
                          ref indStd,
                          PCLXLOperators.eTag.SetBrushSource);

        PCLXLWriter.AddAttrUint16Box(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.eTag.BoundingBox,
                                     (ushort)rectX,
                                     (ushort)rectY,
                                     (ushort)(rectX + rectWidth),
                                     (ushort)(rectY + rectHeight));

        PCLXLWriter.AddOperator(ref bufStd,
                                ref indStd,
                                PCLXLOperators.eTag.Rectangle);

        prnWriter.Write(bufStd, 0, indStd);
        indStd = 0;

        //----------------------------------------------------------------//

        rectY += _lineInc;

        PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                           ref indStd,
                           PCLXLAttributes.eTag.RGBColor,
                           sizeRGB,
                           palette_2);

        PCLXLWriter.AddOperator(ref bufStd,
                          ref indStd,
                          PCLXLOperators.eTag.SetBrushSource);

        PCLXLWriter.AddAttrUint16Box(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.eTag.BoundingBox,
                                     (ushort)rectX,
                                     (ushort)rectY,
                                     (ushort)(rectX + rectWidth),
                                     (ushort)(rectY + rectHeight));

        PCLXLWriter.AddOperator(ref bufStd,
                                ref indStd,
                                PCLXLOperators.eTag.Rectangle);

        prnWriter.Write(bufStd, 0, indStd);
        indStd = 0;

        //----------------------------------------------------------------//

        rectY += _lineInc;

        PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                           ref indStd,
                           PCLXLAttributes.eTag.RGBColor,
                           sizeRGB,
                           palette_3);

        PCLXLWriter.AddOperator(ref bufStd,
                          ref indStd,
                          PCLXLOperators.eTag.SetBrushSource);

        PCLXLWriter.AddAttrUint16Box(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.eTag.BoundingBox,
                                     (ushort)rectX,
                                     (ushort)rectY,
                                     (ushort)(rectX + rectWidth),
                                     (ushort)(rectY + rectHeight));

        PCLXLWriter.AddOperator(ref bufStd,
                                ref indStd,
                                PCLXLOperators.eTag.Rectangle);

        prnWriter.Write(bufStd, 0, indStd);
        indStd = 0;

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
    // p a t t e r n D e f i n e D p i 6 0 0                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Define 600 dots-per-inch user-defined pattern.                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void PatternDefineDpi600(BinaryWriter prnWriter,
                                             short patternId,
                                             bool embedded)
    {
        const ushort patWidth = 16;
        const ushort patHeight = 16;

        const ushort destWidth =
            (patWidth * _unitsPerInch) / 600;
        const ushort destHeight =
            (patHeight * _unitsPerInch) / 600;

        byte[] pattern = { 0x00, 0x00, 0x60, 0x60,
                           0x60, 0x60, 0x00, 0x00,
                           0x00, 0x00, 0x06, 0x06,
                           0x06, 0x06, 0x00, 0x00,
                           0x00, 0x00, 0x60, 0x60,
                           0x60, 0x60, 0x00, 0x00,
                           0x00, 0x00, 0x06, 0x06,
                           0x06, 0x06, 0x00, 0x00 };

        PCLXLWriter.PatternDefine(prnWriter,
                                   embedded,
                                   patternId,
                                   patWidth,
                                   patHeight,
                                   destWidth,
                                   destHeight,
                                   PCLXLAttrEnums.eVal.eIndexedPixel,
                                   PCLXLAttrEnums.eVal.e1Bit,
                                   PCLXLAttrEnums.eVal.eTempPattern,
                                   PCLXLAttrEnums.eVal.eNoCompression,
                                   pattern);
    }
}
