using System.IO;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides PCL XL support for the Shading element of the
    /// Patterns action of the MiscSamples tool.
    /// 
    /// © Chris Hutchinson 2014
    /// 
    /// </summary>

    static class ToolMiscSamplesActPatternShadePCLXL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const string _formName = "MiscSamplesForm";

        const int _symSet_19U = 629;
        const ushort _unitsPerInch = PCLXLWriter._sessionUPI;

        const short _pageOriginX = (_unitsPerInch * 1);
        const short _pageOriginY = (_unitsPerInch * 1);
        const short _incInch = (_unitsPerInch * 1);
        const short _lineInc = (_unitsPerInch * 5) / 6;

        const short _posXDesc = _pageOriginX;
        const short _posXData1 = _pageOriginX + ((7 * _incInch) / 3);
        const short _posXData2 = _posXData1 + ((3 * _incInch / 2));
        const short _posXData3 = _posXData2 + ((3 * _incInch / 2));

        const short _posYHddr = _pageOriginY;
        const short _posYDesc1 = _pageOriginY + (2 * _incInch);
        const short _posYDesc2 = _pageOriginY + ((3 * _incInch / 2));
        const short _posYData = _pageOriginY + (2 * _incInch);

        const short _shade_1_id = 1;
        const short _shade_2_id = 2;
        const short _shade_3_id = 3;
        const short _shade_4_id = 4;
        const short _shade_5_id = 5;
        const short _shade_6_id = 6;
        const short _shade_7_id = 7;

        const short _shade_1_minVal = 1;
        const short _shade_2_minVal = 3;
        const short _shade_3_minVal = 11;
        const short _shade_4_minVal = 21;
        const short _shade_5_minVal = 36;
        const short _shade_6_minVal = 56;
        const short _shade_7_minVal = 81;
        const short _shade_7_maxVal = 99;

        const short _patternBase_300 = 300;
        const short _patternBase_600 = 600;

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

        static int _patternsCt = 0;
        static ushort[] _patternIds;
        static ushort[] _patternHeights;
        static ushort[] _patternWidths;

        static string[] _patternDescs;

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
                                       bool formAsMacro)
        {
            getPatternData();

            generateJobHeader(prnWriter);

            if (formAsMacro)
                generateOverlay(prnWriter, true,
                                 indxPaperSize, indxOrientation);

            generatePage(prnWriter,
                         indxPaperSize,
                         indxPaperType,
                         indxOrientation,
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
                       "PCL XL shading patterns:");

            //----------------------------------------------------------------//

            ptSize = 12;

            PCLXLWriter.font(prnWriter, formAsMacro, ptSize,
                             _symSet_19U, _fontNameCourier);

            posY = _posYDesc1;

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "#" + _patternIds[i].ToString() + ": ");

                posY += _lineInc;
            }

            //----------------------------------------------------------------//

            ptSize = 10;

            PCLXLWriter.font(prnWriter, formAsMacro, ptSize,
                             _symSet_19U, _fontNameCourier);

            //----------------------------------------------------------------//

            posY = _posYDesc1 + (_lineInc / 4);

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLXLWriter.text(prnWriter, formAsMacro, false,
                           PCLXLWriter.advances_Courier, ptSize,
                           posX, posY,
                           _patternDescs[i] + ":");

                posY += _lineInc;
            }

            //----------------------------------------------------------------//

            ptSize = 8;

            PCLXLWriter.font(prnWriter, formAsMacro, ptSize,
                             _symSet_19U, _fontNameCourier);

            //----------------------------------------------------------------//

            posY = _posYDesc2;
            posX = _posXData1;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "Predefined");

            posX = _posXData2;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "User-defined 300 dpi");

            posX = _posXData3;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "User-defined 600 dpi");

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
                                         bool formAsMacro)
        {
            const int sizeStd = 1024;

            byte[] bufStd = new byte[sizeStd];

            short posX,
                  posY,
                  rectX,
                  rectY,
                  rectHeight,
                  rectWidth;

            int indStd;

            short ptSize;

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
            //                                                                //
            // Pre-defined shading - not present in PCL XL.                   //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = 15;

            PCLXLWriter.font(prnWriter, false, ptSize,
                             _symSet_19U, _fontNameArial);

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               0);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.NullPen,
                               0);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetPenSource);

            posX = _posXData1;
            posY = _posYDesc1 + (_lineInc / 4);

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLXLWriter.text(prnWriter, false, false,
                           PCLXLWriter.advances_Courier, ptSize,
                           posX, posY,
                           "n/a");

                posY += _lineInc;
            }

            prnWriter.Write(bufStd, 0, indStd);
            indStd = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // User-defined shading.                                          //
            //                                                                //
            //----------------------------------------------------------------//

            rectHeight = _lineInc / 2;
            rectWidth = _lineInc;

            PCLXLWriter.addAttrUbyte(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.eTag.ColorSpace,
                                     (byte)PCLXLAttrEnums.eVal.eGray);

            PCLXLWriter.addAttrUbyte(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.eTag.PaletteDepth,
                                     (byte)PCLXLAttrEnums.eVal.e8Bit);

            PCLXLWriter.addAttrUbyteArray(ref bufStd,
                                          ref indStd,
                                          PCLXLAttributes.eTag.PaletteData,
                                          2,
                                          PCLXLWriter.monoPalette);

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

            PCLXLWriter.addAttrUbyte(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.eTag.TxMode,
                                     (byte)PCLXLAttrEnums.eVal.eTransparent);

            PCLXLWriter.addOperator(ref bufStd,
                                    ref indStd,
                                    PCLXLOperators.eTag.SetPatternTxMode);

            prnWriter.Write(bufStd, 0, indStd);

            indStd = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // User-defined 300 dpi shading.                                  //
            //                                                                //
            //----------------------------------------------------------------//

            rectX = _posXData2;
            rectY = _posYData;

            patternDefineDpi300(prnWriter, _patternBase_300);

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLXLWriter.addAttrSint16(
                    ref bufStd,
                    ref indStd,
                    PCLXLAttributes.eTag.PatternSelectID,
                    (short)(_patternBase_300 + _patternIds[i]));

                PCLXLWriter.addAttrSint16XY(ref bufStd,
                                            ref indStd,
                                            PCLXLAttributes.eTag.PatternOrigin,
                                            0, 0);

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

                rectY += _lineInc;
            }

            prnWriter.Write(bufStd, 0, indStd);

            indStd = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // User-defined 600 dpi shading.                                  //
            //                                                                //
            //----------------------------------------------------------------//

            rectX = _posXData3;
            rectY = _posYData;

            patternDefineDpi600(prnWriter, _patternBase_600);

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLXLWriter.addAttrSint16(
                    ref bufStd,
                    ref indStd,
                    PCLXLAttributes.eTag.PatternSelectID,
                    (short)(_patternBase_600 + _patternIds[i]));

                PCLXLWriter.addAttrSint16XY(ref bufStd,
                                            ref indStd,
                                            PCLXLAttributes.eTag.PatternOrigin,
                                            0, 0);

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

                rectY += _lineInc;
            }

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
        // g e t P a t t e r n D a t a                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve information about the available cross-hatch patterns.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void getPatternData()
        {
            _patternsCt = PCLPatternDefs.getCount(
                PCLPatternDefs.eType.Shading);

            _patternIds = new ushort[_patternsCt];
            _patternHeights = new ushort[_patternsCt];
            _patternWidths = new ushort[_patternsCt];
            _patternDescs = new string[_patternsCt];

            for (int i = 0; i < _patternsCt; i++)
            {
                _patternIds[i] = PCLPatternDefs.getId(
                    PCLPatternDefs.eType.Shading, i);
                _patternHeights[i] = PCLPatternDefs.getHeight(
                    PCLPatternDefs.eType.Shading, i);
                _patternWidths[i] = PCLPatternDefs.getWidth(
                    PCLPatternDefs.eType.Shading, i);
                _patternDescs[i] = PCLPatternDefs.getDesc(
                    PCLPatternDefs.eType.Shading, i);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a t t e r n D e f i n e D p i 3 0 0                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Define 300 dots-per-inch user-defined patterns to match the        //
        // pre-defined patterns.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void patternDefineDpi300(BinaryWriter prnWriter,
                                                 int baseID)
        {
            const ushort dpi = 300;

            for (int i = 0; i < _patternsCt; i++)
            {
                ushort patWidth = _patternWidths[i];
                ushort patHeight = _patternHeights[i];

                ushort destWidth =
                         (ushort)((patWidth * _unitsPerInch) / dpi);
                ushort destHeight =
                         (ushort)((patHeight * _unitsPerInch) / dpi);

                PCLXLWriter.patternDefine(
                    prnWriter,
                    false,
                    (short)(baseID + _patternIds[i]),
                    patWidth,
                    patHeight,
                    destWidth,
                    destHeight,
                    PCLXLAttrEnums.eVal.eIndexedPixel,
                    PCLXLAttrEnums.eVal.e1Bit,
                    PCLXLAttrEnums.eVal.eTempPattern,
                    PCLXLAttrEnums.eVal.eNoCompression,
                    PCLPatternDefs.getBytes(
                        PCLPatternDefs.eType.Shading, i));
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a t t e r n D e f i n e D p i 6 0 0                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Define 600 dots-per-inch user-defined patterns to match the        //
        // pre-defined patterns.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void patternDefineDpi600(BinaryWriter prnWriter,
                                                int baseID)
        {
            const ushort dpi = 600;

            for (int i = 0; i < _patternsCt; i++)
            {
                ushort patWidth = _patternWidths[i];
                ushort patHeight = _patternHeights[i];

                ushort destWidth =
                         (ushort)((patWidth * _unitsPerInch) / dpi);
                ushort destHeight =
                         (ushort)((patHeight * _unitsPerInch) / dpi);

                PCLXLWriter.patternDefine(
                    prnWriter,
                    false,
                    (short)(baseID + _patternIds[i]),
                    patWidth,
                    patHeight,
                    destWidth,
                    destHeight,
                    PCLXLAttrEnums.eVal.eIndexedPixel,
                    PCLXLAttrEnums.eVal.e1Bit,
                    PCLXLAttrEnums.eVal.eTempPattern,
                    PCLXLAttrEnums.eVal.eNoCompression,
                    PCLPatternDefs.getBytes(
                        PCLPatternDefs.eType.Shading, i));
            }
        }
    }
}
