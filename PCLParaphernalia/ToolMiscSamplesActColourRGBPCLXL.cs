using System.IO;

namespace PCLParaphernalia
{
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

        static readonly short _fontIndexArial = PCLFonts.getIndexForName("Arial");
        static readonly short _fontIndexCourier = PCLFonts.getIndexForName("Courier");

        static readonly string _fontNameArial =
            PCLFonts.getPCLXLName(_fontIndexArial,
                                  PCLFonts.eVariant.Regular);
        static readonly string _fontNameCourier =
            PCLFonts.getPCLXLName(_fontIndexCourier,
                                  PCLFonts.eVariant.Regular);
        static readonly string _fontNameCourierBold =
            PCLFonts.getPCLXLName(_fontIndexCourier,
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

        public static void generateJob(BinaryWriter prnWriter,
                                        int indxPaperSize,
                                        int indxPaperType,
                                        int indxOrientation,
                                        int[] sampleDef,
                                        bool formAsMacro)
        {
            generateJobHeader(prnWriter);

            if (formAsMacro)
                generateOverlay(prnWriter, true,
                                 indxPaperSize, indxOrientation);

            generatePage(prnWriter,
                         indxPaperSize,
                         indxPaperType,
                         indxOrientation,
                         sampleDef,
                         formAsMacro);

            generateJobTrailer(prnWriter, formAsMacro);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e J o b H e a d e r                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write stream initialisation sequences to output file.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateJobHeader(BinaryWriter prnWriter)
        {
            PCLXLWriter.stdJobHeader(prnWriter, string.Empty);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e J o b T r a i l e r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write tray map termination sequences to output file.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateJobTrailer(BinaryWriter prnWriter,
                                               bool formAsMacro)
        {
            PCLXLWriter.stdJobTrailer(prnWriter, formAsMacro, _formName);
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
                PCLXLWriter.streamHeader(prnWriter, true, _formName);
            }

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.PushGS);

            //----------------------------------------------------------------//
            //                                                                //
            // Colour space, pen & brush definitions.                         //
            //                                                                //
            //----------------------------------------------------------------//

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.ColorSpace,
                               (byte)PCLXLAttrEnums.eVal.eGray);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetColorSpace);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.NullBrush,
                               0);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.GrayLevel,
                               0);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPenSource);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.PenWidth,
                               stroke);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPenWidth);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            //----------------------------------------------------------------//
            //                                                                //
            // Box.                                                           //
            //                                                                //
            //----------------------------------------------------------------//

            boxX1 = _unitsPerInch / 2;  // half-inch left margin
            boxY1 = _unitsPerInch / 2;  // half-inch top-margin

            boxX2 = (ushort)(PCLPaperSizes.getPaperWidth(
                                    indxPaperSize, _unitsPerInch,
                                    PCLOrientations.eAspect.Portrait) -
                              boxX1);

            boxY2 = (ushort)(PCLPaperSizes.getPaperLength(
                                    indxPaperSize, _unitsPerInch,
                                    PCLOrientations.eAspect.Portrait) -
                              boxY1);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.TxMode,
                               (byte)PCLXLAttrEnums.eVal.eTransparent);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPatternTxMode);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.TxMode,
                               (byte)PCLXLAttrEnums.eVal.eTransparent);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetSourceTxMode);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.GrayLevel,
                               100);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPenSource);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.PenWidth,
                               5);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPenWidth);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.NullBrush,
                               0);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.EllipseDimension,
                                  100, 100);

            PCLXLWriter.addAttrUint16Box(ref buffer,
                                   ref indBuf,
                                   PCLXLAttributes.eTag.BoundingBox,
                                   boxX1, boxY1, boxX2, boxY2);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.RoundRectangle);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            //----------------------------------------------------------------//
            //                                                                //
            // Text.                                                          //
            //                                                                //
            //----------------------------------------------------------------//

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.GrayLevel,
                               100);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.NullPen,
                               0);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPenSource);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            ptSize = 15;

            PCLXLWriter.font(prnWriter, formAsMacro, ptSize,
                             _symSet_19U, _fontNameCourierBold);

            posX = _posXDesc;
            posY = _posYHddr;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "PCL XL RGB colour mode:");

            //----------------------------------------------------------------//

            ptSize = 12;

            PCLXLWriter.font(prnWriter, formAsMacro, ptSize,
                             _symSet_19U, _fontNameCourier);

            posY += _incInch / 2;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "Sample 4-colour palette:");

            //----------------------------------------------------------------//

            posX = _posXDesc2;
            posY = _posYDesc2;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "RGB");

            //----------------------------------------------------------------//

            posX = _posXDesc3;
            posY = _posYDesc3;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "index");

            posX += _incInch;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                           "value");

            //----------------------------------------------------------------//

            posX = _posXDesc4;
            posY = _posYDesc4;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "0");

            posY += _lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "1");

            posY += _lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "2");

            posY += _lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
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

            PCLXLWriter.addAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.ColorSpace,
                                     (byte)PCLXLAttrEnums.eVal.eGray);

            PCLXLWriter.addAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.PaletteDepth,
                                     (byte)PCLXLAttrEnums.eVal.e8Bit);

            PCLXLWriter.addAttrUbyteArray(ref buffer,
                                          ref indBuf,
                                          PCLXLAttributes.eTag.PaletteData,
                                          2,
                                          PCLXLWriter.monoPalette);

            PCLXLWriter.addOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.SetColorSpace);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            patternDefineDpi600(prnWriter, _patternId_1, formAsMacro);

            PCLXLWriter.addAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.NullPen,
                                     0);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPenSource);

            PCLXLWriter.addAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.TxMode,
                                     (byte)PCLXLAttrEnums.eVal.eTransparent);

            PCLXLWriter.addOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.SetPatternTxMode);

            PCLXLWriter.addAttrSint16(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.eTag.PatternSelectID,
                                      601);

            PCLXLWriter.addAttrSint16XY(ref buffer,
                                        ref indBuf,
                                        PCLXLAttributes.eTag.PatternOrigin,
                                        0, 0);

            PCLXLWriter.addOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUint16Box(ref buffer,
                                         ref indBuf,
                                         PCLXLAttributes.eTag.BoundingBox,
                                         (ushort)rectX,
                                         (ushort)rectY,
                                         (ushort)(rectX + rectWidth),
                                         (ushort)(rectY + rectHeight));

            PCLXLWriter.addOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.Rectangle);

            //----------------------------------------------------------------//
            //                                                                //
            // Overlay end.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.PopGS);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            if (formAsMacro)
            {
                PCLXLWriter.addOperator(ref buffer,
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

            if (indxOrientation < PCLOrientations.getCount())
            {
                PCLXLWriter.addAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.Orientation,
                                   PCLOrientations.getIdPCLXL(indxOrientation));
            }

            if (indxPaperSize < PCLPaperSizes.getCount())
            {
                PCLXLWriter.addAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.MediaSize,
                                   PCLPaperSizes.getIdPCLXL(indxPaperSize));
            }

            if ((indxPaperType < PCLPaperTypes.getCount()) &&
                (PCLPaperTypes.getType(indxPaperType) !=
                    PCLPaperTypes.eEntryType.NotSet))
            {
                PCLXLWriter.addAttrUbyteArray(ref bufStd,
                                        ref indStd,
                                        PCLXLAttributes.eTag.MediaType,
                                        PCLPaperTypes.getName(indxPaperType));
            }

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.SimplexPageMode,
                               (byte)PCLXLAttrEnums.eVal.eSimplexFrontSide);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.BeginPage);

            PCLXLWriter.addAttrUint16XY(ref bufStd,
                                  ref indStd,
                                  PCLXLAttributes.eTag.PageOrigin,
                                  0, 0);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetPageOrigin);

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.ColorSpace,
                               (byte)PCLXLAttrEnums.eVal.eGray);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetColorSpace);

            prnWriter.Write(bufStd, 0, indStd);
            indStd = 0;

            //----------------------------------------------------------------//

            if (formAsMacro)
            {
                PCLXLWriter.addAttrUbyteArray(ref bufStd,
                                        ref indStd,
                                        PCLXLAttributes.eTag.StreamName,
                                        _formName);

                PCLXLWriter.addOperator(ref bufStd,
                                  ref indStd,
                                  PCLXLOperators.eTag.ExecStream);

                prnWriter.Write(bufStd, 0, indStd);
                indStd = 0;
            }
            else
            {
                generateOverlay(prnWriter, false,
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

            PCLXLWriter.font(prnWriter, false, ptSize,
                             _symSet_19U, _fontNameCourier);

            posX = _posXDesc;
            posY = _posYHddr;

            //----------------------------------------------------------------//

            posX = _posXDesc4;
            posY = _posYDesc4;

            posX += _incInch;

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                           "0x" +
                           palette_0[0].ToString("x2") +
                           palette_0[1].ToString("x2") +
                           palette_0[2].ToString("x2"));

            posY += _lineInc;

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                           "0x" +
                           palette_1[0].ToString("x2") +
                           palette_1[1].ToString("x2") +
                           palette_1[2].ToString("x2"));

            posY += _lineInc;

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                           "0x" +
                           palette_2[0].ToString("x2") +
                           palette_2[1].ToString("x2") +
                           palette_2[2].ToString("x2"));

            posY += _lineInc;

            PCLXLWriter.text(prnWriter, false, false,
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

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.ColorSpace,
                               (byte)PCLXLAttrEnums.eVal.eRGB);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetColorSpace);

            PCLXLWriter.addAttrUbyte(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.eTag.NullPen,
                                     0);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetPenSource);

            PCLXLWriter.addAttrUbyteArray(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.RGBColor,
                               sizeRGB,
                               palette_0);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUint16Box(ref bufStd,
                                         ref indStd,
                                         PCLXLAttributes.eTag.BoundingBox,
                                         (ushort)rectX,
                                         (ushort)rectY,
                                         (ushort)(rectX + rectWidth),
                                         (ushort)(rectY + rectHeight));

            PCLXLWriter.addOperator(ref bufStd,
                                    ref indStd,
                                    PCLXLOperators.eTag.Rectangle);

            prnWriter.Write(bufStd, 0, indStd);
            indStd = 0;

            //----------------------------------------------------------------//

            rectY += _lineInc;

            PCLXLWriter.addAttrUbyteArray(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.RGBColor,
                               sizeRGB,
                               palette_1);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUint16Box(ref bufStd,
                                         ref indStd,
                                         PCLXLAttributes.eTag.BoundingBox,
                                         (ushort)rectX,
                                         (ushort)rectY,
                                         (ushort)(rectX + rectWidth),
                                         (ushort)(rectY + rectHeight));

            PCLXLWriter.addOperator(ref bufStd,
                                    ref indStd,
                                    PCLXLOperators.eTag.Rectangle);

            prnWriter.Write(bufStd, 0, indStd);
            indStd = 0;

            //----------------------------------------------------------------//

            rectY += _lineInc;

            PCLXLWriter.addAttrUbyteArray(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.RGBColor,
                               sizeRGB,
                               palette_2);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUint16Box(ref bufStd,
                                         ref indStd,
                                         PCLXLAttributes.eTag.BoundingBox,
                                         (ushort)rectX,
                                         (ushort)rectY,
                                         (ushort)(rectX + rectWidth),
                                         (ushort)(rectY + rectHeight));

            PCLXLWriter.addOperator(ref bufStd,
                                    ref indStd,
                                    PCLXLOperators.eTag.Rectangle);

            prnWriter.Write(bufStd, 0, indStd);
            indStd = 0;

            //----------------------------------------------------------------//

            rectY += _lineInc;

            PCLXLWriter.addAttrUbyteArray(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.RGBColor,
                               sizeRGB,
                               palette_3);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUint16Box(ref bufStd,
                                         ref indStd,
                                         PCLXLAttributes.eTag.BoundingBox,
                                         (ushort)rectX,
                                         (ushort)rectY,
                                         (ushort)(rectX + rectWidth),
                                         (ushort)(rectY + rectHeight));

            PCLXLWriter.addOperator(ref bufStd,
                                    ref indStd,
                                    PCLXLOperators.eTag.Rectangle);

            prnWriter.Write(bufStd, 0, indStd);
            indStd = 0;

            //----------------------------------------------------------------//

            PCLXLWriter.addAttrUint16(ref bufStd,
                                ref indStd,
                                PCLXLAttributes.eTag.PageCopies,
                                1);

            PCLXLWriter.addOperator(ref bufStd,
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

        private static void patternDefineDpi600(BinaryWriter prnWriter,
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

            PCLXLWriter.patternDefine(prnWriter,
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
}
