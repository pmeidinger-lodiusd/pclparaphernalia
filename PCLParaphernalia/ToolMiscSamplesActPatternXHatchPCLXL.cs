using System.IO;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>
    /// Class provides PCL XL support for the Cross-hatch element of the
    /// Patterns action of the MiscSamples tool.
    /// </para>
    /// <para>© Chris Hutchinson 2014</para>
    ///
    /// </summary>
    internal static class ToolMiscSamplesActPatternXHatchPCLXL
    {
        private const string _formName = "MiscSamplesForm";
        private const int _symSet_19U = 629;
        private const ushort _unitsPerInch = PCLXLWriter._sessionUPI;
        private const short _pageOriginX = (_unitsPerInch * 1);
        private const short _pageOriginY = (_unitsPerInch * 1);
        private const short _incInch = (_unitsPerInch * 1);
        private const short _lineInc = (_unitsPerInch * 5) / 6;
        private const short _posXDesc = _pageOriginX;
        private const short _posXData1 = _pageOriginX + ((7 * _incInch) / 3);
        private const short _posXData2 = _posXData1 + (3 * _incInch / 2);
        private const short _posXData3 = _posXData2 + (3 * _incInch / 2);
        private const short _posYHddr = _pageOriginY;
        private const short _posYDesc1 = _pageOriginY + (2 * _incInch);
        private const short _posYDesc2 = _pageOriginY + (3 * _incInch / 2);
        private const short _posYData = _pageOriginY + (2 * _incInch);
        private const short _patternBase_300 = 300;
        private const short _patternBase_600 = 600;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Static variables.                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly short _fontIndexArial = PCLFonts.GetIndexForName("Arial");
        private static readonly short _fontIndexCourier = PCLFonts.GetIndexForName("Courier");
        private static readonly string _fontNameArial = PCLFonts.GetPCLXLName(_fontIndexArial, PCLFonts.Variant.Regular);
        private static readonly string _fontNameCourier = PCLFonts.GetPCLXLName(_fontIndexCourier, PCLFonts.Variant.Regular);
        private static readonly string _fontNameCourierBold = PCLFonts.GetPCLXLName(_fontIndexCourier, PCLFonts.Variant.Bold);
        private static int _patternsCt = 0;
        private static ushort[] _patternIds;
        private static ushort[] _patternHeights;
        private static ushort[] _patternWidths;
        private static string[] _patternDescs;

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
            GetPatternData();

            GenerateJobHeader(prnWriter);

            if (formAsMacro)
            {
                GenerateOverlay(prnWriter, true, indxPaperSize, indxOrientation);
            }

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

        private static void GenerateJobTrailer(BinaryWriter prnWriter, bool formAsMacro)
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

            const byte stroke = 1;

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
                              PCLXLOperators.Tag.PushGS);

            //----------------------------------------------------------------//
            //                                                                //
            // Colour space, pen & brush definitions.                         //
            //                                                                //
            //----------------------------------------------------------------//

            PCLXLWriter.AddAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.ColorSpace,
                               (byte)PCLXLAttrEnums.Val.Gray);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.Tag.SetColorSpace);

            PCLXLWriter.AddAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.NullBrush,
                               0);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.Tag.SetBrushSource);

            PCLXLWriter.AddAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.GrayLevel,
                               0);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.Tag.SetPenSource);

            PCLXLWriter.AddAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.PenWidth,
                               stroke);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.Tag.SetPenWidth);

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
                                    PCLOrientations.Aspect.Portrait) -
                              boxX1);

            boxY2 = (ushort)(PCLPaperSizes.GetPaperLength(
                                    indxPaperSize, _unitsPerInch,
                                    PCLOrientations.Aspect.Portrait) -
                              boxY1);

            PCLXLWriter.AddAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.TxMode,
                               (byte)PCLXLAttrEnums.Val.Transparent);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.Tag.SetPatternTxMode);

            PCLXLWriter.AddAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.TxMode,
                               (byte)PCLXLAttrEnums.Val.Transparent);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.Tag.SetSourceTxMode);

            PCLXLWriter.AddAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.GrayLevel,
                               100);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.Tag.SetPenSource);

            PCLXLWriter.AddAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.PenWidth,
                               5);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.Tag.SetPenWidth);

            PCLXLWriter.AddAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.NullBrush,
                               0);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.Tag.SetBrushSource);

            PCLXLWriter.AddAttrUint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.Tag.EllipseDimension,
                                  100, 100);

            PCLXLWriter.AddAttrUint16Box(ref buffer,
                                   ref indBuf,
                                   PCLXLAttributes.Tag.BoundingBox,
                                   boxX1, boxY1, boxX2, boxY2);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.Tag.RoundRectangle);

            PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            //----------------------------------------------------------------//
            //                                                                //
            // Text.                                                          //
            //                                                                //
            //----------------------------------------------------------------//

            PCLXLWriter.AddAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.GrayLevel,
                               100);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.Tag.SetBrushSource);

            PCLXLWriter.AddAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.NullPen,
                               0);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.Tag.SetPenSource);

            PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            ptSize = 15;

            PCLXLWriter.Font(prnWriter, formAsMacro, ptSize, _symSet_19U, _fontNameCourierBold);

            posX = _posXDesc;
            posY = _posYHddr;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "PCL XL cross-hatch patterns:");

            //----------------------------------------------------------------//

            ptSize = 12;

            PCLXLWriter.Font(prnWriter, formAsMacro, ptSize, _symSet_19U, _fontNameCourier);

            posY = _posYDesc1;

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "#" + _patternIds[i].ToString() + ": ");

                posY += _lineInc;
            }

            //----------------------------------------------------------------//

            ptSize = 10;

            PCLXLWriter.Font(prnWriter, formAsMacro, ptSize, _symSet_19U, _fontNameCourier);

            //----------------------------------------------------------------//

            posY = _posYDesc1 + (_lineInc / 4);

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLXLWriter.Text(prnWriter, formAsMacro, false,
                           PCLXLWriter.advances_Courier, ptSize,
                           posX, posY,
                           _patternDescs[i] + ":");

                posY += _lineInc;
            }

            //----------------------------------------------------------------//

            ptSize = 8;

            PCLXLWriter.Font(prnWriter, formAsMacro, ptSize, _symSet_19U, _fontNameCourier);

            //----------------------------------------------------------------//

            posY = _posYDesc2;
            posX = _posXData1;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "Predefined");

            posX = _posXData2;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "User-defined 300 dpi");

            posX = _posXData3;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "User-defined 600 dpi");

            //----------------------------------------------------------------//
            //                                                                //
            // Overlay end.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.Tag.PopGS);

            PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro, buffer, ref indBuf);

            if (formAsMacro)
            {
                PCLXLWriter.AddOperator(ref buffer,
                                  ref indBuf,
                                  PCLXLOperators.Tag.EndStream);

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

            if (indxOrientation < PCLOrientations.GetCount())
            {
                PCLXLWriter.AddAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.Tag.Orientation,
                                   PCLOrientations.GetIdPCLXL(indxOrientation));
            }

            if (indxPaperSize < PCLPaperSizes.GetCount())
            {
                PCLXLWriter.AddAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.Tag.MediaSize,
                                   PCLPaperSizes.GetIdPCLXL(indxPaperSize));
            }

            if ((indxPaperType < PCLPaperTypes.GetCount()) &&
                (PCLPaperTypes.GetType(indxPaperType) !=
                    PCLPaperTypes.EntryType.NotSet))
            {
                PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                                        ref indStd,
                                        PCLXLAttributes.Tag.MediaType,
                                        PCLPaperTypes.GetName(indxPaperType));
            }

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.Tag.SimplexPageMode,
                               (byte)PCLXLAttrEnums.Val.SimplexFrontSide);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.BeginPage);

            PCLXLWriter.AddAttrUint16XY(ref bufStd,
                                  ref indStd,
                                  PCLXLAttributes.Tag.PageOrigin,
                                  0, 0);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.SetPageOrigin);

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.Tag.ColorSpace,
                               (byte)PCLXLAttrEnums.Val.Gray);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.SetColorSpace);

            prnWriter.Write(bufStd, 0, indStd);
            indStd = 0;

            //----------------------------------------------------------------//

            if (formAsMacro)
            {
                PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                                        ref indStd,
                                        PCLXLAttributes.Tag.StreamName,
                                        _formName);

                PCLXLWriter.AddOperator(ref bufStd,
                                  ref indStd,
                                  PCLXLOperators.Tag.ExecStream);

                prnWriter.Write(bufStd, 0, indStd);
                indStd = 0;
            }
            else
            {
                GenerateOverlay(prnWriter, false, indxPaperSize, indxOrientation);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Pre-defined shading - not present in PCL XL.                   //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = 15;

            PCLXLWriter.Font(prnWriter, false, ptSize, _symSet_19U, _fontNameArial);

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.Tag.GrayLevel,
                               0);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.SetBrushSource);

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.Tag.NullPen,
                               0);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.SetPenSource);

            posX = _posXData1;
            posY = _posYDesc1 + (_lineInc / 4);

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLXLWriter.Text(prnWriter, false, false,
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

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.Tag.ColorSpace,
                                     (byte)PCLXLAttrEnums.Val.Gray);

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.Tag.PaletteDepth,
                                     (byte)PCLXLAttrEnums.Val.e8Bit);

            PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                                          ref indStd,
                                          PCLXLAttributes.Tag.PaletteData,
                                          2,
                                          PCLXLWriter.monoPalette);

            PCLXLWriter.AddOperator(ref bufStd,
                                    ref indStd,
                                    PCLXLOperators.Tag.SetColorSpace);

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.Tag.NullPen,
                                     0);

            PCLXLWriter.AddOperator(ref bufStd,
                                    ref indStd,
                                    PCLXLOperators.Tag.SetPenSource);

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.Tag.TxMode,
                                     (byte)PCLXLAttrEnums.Val.Transparent);

            PCLXLWriter.AddOperator(ref bufStd,
                                    ref indStd,
                                    PCLXLOperators.Tag.SetPatternTxMode);

            prnWriter.Write(bufStd, 0, indStd);

            indStd = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // User-defined 300 dpi shading.                                  //
            //                                                                //
            //----------------------------------------------------------------//

            rectX = _posXData2;
            rectY = _posYData;

            PatternDefineDpi300(prnWriter, _patternBase_300);

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLXLWriter.AddAttrSint16(
                    ref bufStd,
                    ref indStd,
                    PCLXLAttributes.Tag.PatternSelectID,
                    (short)(_patternBase_300 + _patternIds[i]));

                PCLXLWriter.AddAttrSint16XY(ref bufStd,
                                            ref indStd,
                                            PCLXLAttributes.Tag.PatternOrigin,
                                            0, 0);

                PCLXLWriter.AddOperator(ref bufStd,
                                        ref indStd,
                                        PCLXLOperators.Tag.SetBrushSource);

                PCLXLWriter.AddAttrUint16Box(ref bufStd,
                                             ref indStd,
                                             PCLXLAttributes.Tag.BoundingBox,
                                             (ushort)rectX,
                                             (ushort)rectY,
                                             (ushort)(rectX + rectWidth),
                                             (ushort)(rectY + rectHeight));

                PCLXLWriter.AddOperator(ref bufStd,
                                        ref indStd,
                                        PCLXLOperators.Tag.Rectangle);

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

            PatternDefineDpi600(prnWriter, _patternBase_600);

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLXLWriter.AddAttrSint16(
                    ref bufStd,
                    ref indStd,
                    PCLXLAttributes.Tag.PatternSelectID,
                    (short)(_patternBase_600 + _patternIds[i]));

                PCLXLWriter.AddAttrSint16XY(ref bufStd,
                                            ref indStd,
                                            PCLXLAttributes.Tag.PatternOrigin,
                                            0, 0);

                PCLXLWriter.AddOperator(ref bufStd,
                                        ref indStd,
                                        PCLXLOperators.Tag.SetBrushSource);

                PCLXLWriter.AddAttrUint16Box(ref bufStd,
                                             ref indStd,
                                             PCLXLAttributes.Tag.BoundingBox,
                                             (ushort)rectX,
                                             (ushort)rectY,
                                             (ushort)(rectX + rectWidth),
                                             (ushort)(rectY + rectHeight));

                PCLXLWriter.AddOperator(ref bufStd,
                                        ref indStd,
                                        PCLXLOperators.Tag.Rectangle);

                rectY += _lineInc;
            }

            prnWriter.Write(bufStd, 0, indStd);

            indStd = 0;

            //----------------------------------------------------------------//

            PCLXLWriter.AddAttrUint16(ref bufStd,
                                ref indStd,
                                PCLXLAttributes.Tag.PageCopies,
                                1);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.EndPage);

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

        private static void GetPatternData()
        {
            _patternsCt = PCLPatternDefs.GetCount(PCLPatternDefs.Type.CrossHatch);

            _patternIds = new ushort[_patternsCt];
            _patternHeights = new ushort[_patternsCt];
            _patternWidths = new ushort[_patternsCt];
            _patternDescs = new string[_patternsCt];

            for (int i = 0; i < _patternsCt; i++)
            {
                _patternIds[i] = PCLPatternDefs.GetId(PCLPatternDefs.Type.CrossHatch, i);
                _patternHeights[i] = PCLPatternDefs.GetHeight(PCLPatternDefs.Type.CrossHatch, i);
                _patternWidths[i] = PCLPatternDefs.GetWidth(PCLPatternDefs.Type.CrossHatch, i);
                _patternDescs[i] = PCLPatternDefs.GetDesc(PCLPatternDefs.Type.CrossHatch, i);
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

        private static void PatternDefineDpi300(BinaryWriter prnWriter, int baseID)
        {
            const ushort dpi = 300;

            for (int i = 0; i < _patternsCt; i++)
            {
                ushort patWidth = _patternWidths[i];
                ushort patHeight = _patternHeights[i];

                ushort destWidth = (ushort)((patWidth * _unitsPerInch) / dpi);
                ushort destHeight = (ushort)((patHeight * _unitsPerInch) / dpi);

                PCLXLWriter.PatternDefine(
                    prnWriter,
                    false,
                    (short)(baseID + _patternIds[i]),
                    patWidth,
                    patHeight,
                    destWidth,
                    destHeight,
                    PCLXLAttrEnums.Val.IndexedPixel,
                    PCLXLAttrEnums.Val.e1Bit,
                    PCLXLAttrEnums.Val.TempPattern,
                    PCLXLAttrEnums.Val.NoCompression,
                    PCLPatternDefs.GetBytes(PCLPatternDefs.Type.CrossHatch, i));
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

        private static void PatternDefineDpi600(BinaryWriter prnWriter, int baseID)
        {
            const ushort dpi = 600;

            for (int i = 0; i < _patternsCt; i++)
            {
                ushort patWidth = _patternWidths[i];
                ushort patHeight = _patternHeights[i];

                ushort destWidth = (ushort)((patWidth * _unitsPerInch) / dpi);
                ushort destHeight = (ushort)((patHeight * _unitsPerInch) / dpi);

                PCLXLWriter.PatternDefine(
                    prnWriter,
                    false,
                    (short)(baseID + _patternIds[i]),
                    patWidth,
                    patHeight,
                    destWidth,
                    destHeight,
                    PCLXLAttrEnums.Val.IndexedPixel,
                    PCLXLAttrEnums.Val.e1Bit,
                    PCLXLAttrEnums.Val.TempPattern,
                    PCLXLAttrEnums.Val.NoCompression,
                    PCLPatternDefs.GetBytes(PCLPatternDefs.Type.CrossHatch, i));
            }
        }
    }
}