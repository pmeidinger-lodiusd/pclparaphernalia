using System.IO;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>
    /// Class provides PCL XL support for the Rotation element
    /// of the Text Modification action of the MiscSamples tool.
    /// </para>
    /// <para>© Chris Hutchinson 2014</para>
    ///
    /// </summary>
    internal static class ToolMiscSamplesActTxtModRotPCLXL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const string _formName = "MiscTxtModRotForm";
        private const int _symSet_19U = 629;
        private const ushort _unitsPerInch = PCLXLWriter._sessionUPI;
        private const short _pageOriginX = (_unitsPerInch * 1);
        private const short _pageOriginY = (_unitsPerInch * 1);
        private const short _incInch = (_unitsPerInch * 1);
        private const short _lineInc = (_unitsPerInch * 5) / 6;
        private const short _posXDesc = _pageOriginX;
        private const short _posXData1 = _pageOriginX + ((3 * _incInch) / 2);
        private const short _posXData2 = _pageOriginX + ((7 * _incInch) / 2);
        private const short _posYDesc = _pageOriginY;
        private const short _posYData = _pageOriginY;

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
                               (byte)PCLXLAttrEnums.Val.eGray);

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
            // This uses different brush and/or pen definitions, so enclosed  //
            // in a GS block.                                                 //
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
                               (byte)PCLXLAttrEnums.Val.eTransparent);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.Tag.SetPatternTxMode);

            PCLXLWriter.AddAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.TxMode,
                               (byte)PCLXLAttrEnums.Val.eTransparent);

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

            PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro, buffer, ref indBuf);

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

            PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro, buffer, ref indBuf);

            ptSize = 15;

            PCLXLWriter.Font(prnWriter, formAsMacro, ptSize, _symSet_19U, _fontNameCourierBold);

            posX = _posXDesc;
            posY = _posYDesc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "PCL XL Text Rotation:");

            ptSize = 12;

            PCLXLWriter.Font(prnWriter, formAsMacro, ptSize, _symSet_19U, _fontNameCourier);

            posY += _lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "Font");

            posY += _lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "Angle | scale | X & Y spacing");

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

            const string sampleText = "0123456789";
            const string sampleTextA = "01234";
            const string sampleTextB = "56789";

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
                               (byte)PCLXLAttrEnums.Val.eSimplexFrontSide);

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
                               (byte)PCLXLAttrEnums.Val.eGray);

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
            // Descriptive text.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = 18;

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

            PCLXLWriter.Font(prnWriter, false, ptSize,
                             _symSet_19U, _fontNameCourier);

            posX = _posXData1;
            posY = _posYData;

            posY += _lineInc;

            PCLXLWriter.Text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "Arial");
            prnWriter.Write(bufStd, 0, indStd);
            indStd = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Embellished text.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = 36;

            PCLXLWriter.Font(prnWriter, false, ptSize, _symSet_19U, _fontNameArial);

            posX = _posXData2;
            posY = _posYData;

            posY += _lineInc;

            PCLXLWriter.CharAngle(prnWriter, false, 0);
            PCLXLWriter.CharBold(prnWriter, false, 0);
            PCLXLWriter.CharScale(prnWriter, false,
                                   (float)1.0, (float)1.0);
            PCLXLWriter.CharShear(prnWriter, false,
                                   (float)0.0, (float)0.0);

            PCLXLWriter.Text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       sampleText);

            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLXLWriter.CharAngle(prnWriter, false, -30);

            PCLXLWriter.Text(prnWriter, false, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX, posY,
                       sampleText);

            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLXLWriter.CharShear(prnWriter, false, (float)0.0, (float)0.0);

            PCLXLWriter.CharScale(prnWriter, false, (float)2.0, (float)1.0);

            PCLXLWriter.CharAngle(prnWriter, false, -45);

            PCLXLWriter.TextAngled(prnWriter, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX, posY, -45,
                       sampleTextA);

            posX = (short)(posX + ((7 * _incInch) / 4));
            posY += _lineInc;

            PCLXLWriter.CharScale(prnWriter, false, (float)1.0, (float)2.0);

            PCLXLWriter.CharAngle(prnWriter, false, 30);

            PCLXLWriter.TextAngled(prnWriter, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX, posY, 30,
                       sampleTextB);

            //----------------------------------------------------------------//

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
    }
}