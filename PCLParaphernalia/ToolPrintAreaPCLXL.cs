using System;
using System.IO;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides PCL XL support for the PrintArea tool.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class ToolPrintAreaPCLXL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const string _formName = "FontSampleForm";
        const string _hexChars = "0123456789ABCDEF";

        const int _symSet_19U = 629;
        const ushort _sessionUPI = PCLXLWriter._sessionUPI;

        const short _boxOuterEdge = (_sessionUPI * 1);
        const short _rulerOriginX = (_sessionUPI * 1);
        const short _rulerOriginY = (_sessionUPI * 1);
        const short _rulerCell = (_sessionUPI * 1);
        const short _rulerDiv = (_rulerCell / 5);

        const short _posXHddr = _rulerOriginX + (2 * _rulerDiv);
        const short _posXDesc = _rulerOriginX + (1 * _rulerDiv);
        const short _posYHddr = _rulerOriginY - (2 * _rulerDiv);
        const short _posYText = _rulerOriginY + (2 * _rulerDiv);
        const short _posYDesc = _rulerOriginY + (6 * _rulerDiv);

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
                                       int indxPlexMode,
                                       string pjlCommand,
                                       bool formAsMacro,
                                       bool trayIdUnknown,
                                       bool customUseMetric)
        {
            PCLOrientations.eAspect aspect;

            ushort paperWidth,
                   paperLength;

            ushort A4Length,
                   A4Width;

            float scaleText,
                   scaleTextLength,
                   scaleTextWidth;

            //----------------------------------------------------------------//

            aspect = PCLOrientations.GetAspect(indxOrientation);

            A4Length = PCLPaperSizes.GetPaperLength(
                            (byte)PCLPaperSizes.eIndex.ISO_A4,
                            _sessionUPI,
                            aspect);

            A4Width = PCLPaperSizes.GetPaperWidth(
                            (byte)PCLPaperSizes.eIndex.ISO_A4,
                            _sessionUPI,
                            aspect);

            paperLength = PCLPaperSizes.GetPaperLength(indxPaperSize,
                                                       _sessionUPI, aspect);

            paperWidth = PCLPaperSizes.GetPaperWidth(indxPaperSize,
                                                      _sessionUPI, aspect);

            scaleTextLength = (float)paperLength / A4Length;
            scaleTextWidth = (float)paperWidth / A4Width;

            if (scaleTextLength < scaleTextWidth)
                scaleText = scaleTextLength;
            else
                scaleText = scaleTextWidth;

            //----------------------------------------------------------------//

            GenerateJobHeader(prnWriter, pjlCommand);

            if (formAsMacro)
                GenerateOverlay(prnWriter, true, paperWidth, paperLength,
                                scaleText);

            GeneratePage(prnWriter,
                         indxPaperSize,
                         indxPaperType,
                         indxOrientation,
                         indxPlexMode,
                         pjlCommand,
                         formAsMacro,
                         trayIdUnknown,
                         customUseMetric,
                         false,
                         paperWidth,
                         paperLength,
                         scaleText);

            if (PCLPlexModes.GetPlexType(indxPlexMode) !=
                PCLPlexModes.ePlexType.Simplex)
            {
                GeneratePage(prnWriter,
                             indxPaperSize,
                             indxPaperType,
                             indxOrientation,
                             indxPlexMode,
                             pjlCommand,
                             formAsMacro,
                             trayIdUnknown,
                             customUseMetric,
                             true,
                             paperWidth,
                             paperLength,
                             scaleText);
            }

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

        private static void GenerateJobHeader(BinaryWriter prnWriter,
                                              string pjlCommand)
        {
            PCLXLWriter.StdJobHeader(prnWriter, pjlCommand);
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
                                            ushort paperWidth,
                                            ushort paperLength,
                                            float scaleText)
        {
            const int lenBuf = 1024;

            byte[] buffer = new byte[lenBuf];

            short rulerWidth;
            short rulerHeight;

            short rulerCellsX;
            short rulerCellsY;

            short lineInc,
                  ptSize;

            byte stroke = 1;

            int indBuf;

            short posX1,
                  posX2,
                  posY1,
                  posY2;

            //----------------------------------------------------------------//

            rulerCellsX = (short)((paperWidth / _sessionUPI) - 1);
            rulerCellsY = (short)((paperLength / _sessionUPI) - 1);
            rulerWidth = (short)(rulerCellsX * _sessionUPI);
            rulerHeight = (short)(rulerCellsY * _sessionUPI);

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
            // Horizontal ruler.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            posX1 = _rulerOriginX;
            posY1 = _rulerOriginY;
            posX2 = rulerWidth;                 // relative draw X
            posY2 = 0;                          // relative draw Y

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.NewPath);

            PCLXLWriter.AddAttrSint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.Point,
                                  posX1, posY1);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetCursor);

            PCLXLWriter.AddAttrSint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.EndPoint,
                                  posX2, posY2);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.LineRelPath);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.PaintPath);

            //----------------------------------------------------------------//

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.NewPath);

            PCLXLWriter.AddAttrSint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.Point,
                                  posX1, posY1);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetCursor);

            posX1 = _rulerCell;             // relative movement X
            posY1 = 0;                      // relative movement Y
            posX2 = 0;                      // relative draw X
            posY2 = _rulerDiv;              // relative draw Y

            for (int i = 0; i < rulerCellsX; i++)
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

                if (i == 0)
                {
                    posY1 = -_rulerDiv;
                }
            }

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.PaintPath);

            PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            //----------------------------------------------------------------//
            //                                                                //
            // Vertical ruler.                                                //
            //                                                                //
            //----------------------------------------------------------------//

            posX1 = _rulerOriginX;
            posY1 = _rulerOriginY;
            posX2 = 0;                          // relative draw X
            posY2 = rulerHeight;                // relative draw Y

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.NewPath);

            PCLXLWriter.AddAttrSint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.Point,
                                  posX1, posY1);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetCursor);

            PCLXLWriter.AddAttrSint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.EndPoint,
                                  posX2, posY2);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.LineRelPath);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.PaintPath);

            //----------------------------------------------------------------//

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.NewPath);

            PCLXLWriter.AddAttrSint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.Point,
                                  posX1, posY1);

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetCursor);

            posX1 = 0;                      // relative movement X
            posY1 = _rulerCell;             // relative movement Y
            posX2 = _rulerDiv;              // relative draw X
            posY2 = 0;                      // relative draw Y

            for (int i = 0; i < rulerCellsY; i++)
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

                if (i == 0)
                {
                    posX1 = -_rulerDiv;
                }
            }

            PCLXLWriter.AddOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.PaintPath);

            PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            //----------------------------------------------------------------//
            //                                                                //
            // Sample marker box.                                             //
            //                                                                //
            //----------------------------------------------------------------//

            lineInc = (short)((_sessionUPI * scaleText) / 8);

            posX1 = (short)(_rulerCell * 5.5 * scaleText);
            posY1 = (short)(_posYDesc - lineInc);

            GenerateSquare(prnWriter, posX1, posY1, true, formAsMacro);

            //----------------------------------------------------------------//
            //                                                                //
            // Text.                                                          //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = (short)(15 * scaleText);

            PCLXLWriter.AddAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.GrayLevel,
                               0);

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

            PCLXLWriter.Font(prnWriter, formAsMacro, ptSize,
                       _symSet_19U, "Arial           ");

            posX1 = _posXHddr;
            posY1 = _posYHddr;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "PCL XL print area sample");

            ptSize = (short)(10 * scaleText);
            lineInc = (short)((_sessionUPI * scaleText) / 8);

            PCLXLWriter.Font(prnWriter, formAsMacro, ptSize,
                       _symSet_19U, "Arial           ");

            posX1 = _posXDesc;
            posY1 = _posYDesc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Paper size:");

            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Paper type:");

            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Orientation:");

            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Plex mode:");

            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Paper width:");

            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Paper length:");

            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "PJL option:");

            //----------------------------------------------------------------//

            posY1 = (short)(_posYDesc + (_rulerCell * scaleText));

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Black squares of side 3 units, each containing a" +
                       " central white square of side one");

            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "unit, and some directional markers, as per the" +
                       " half-size sample above,");

            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "demonstrate how objects are clipped by the" +
                       " boundaries of the printable area.");

            posY1 += lineInc;
            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                      "The four corner squares are (theoretically) positioned" +
                      " in the corners of the");

            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                      "physical sheet.");

            posY1 += lineInc;
            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "The middle left-hand square is positioned relative" +
                       " to the bottom and right edges");

            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "of the physical sheet, and rotated 180 degrees.");

            posY1 += lineInc;
            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Fixed pitch (10 cpi) text characters are also clipped" +
                       " by the boundaries of the");

            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "printable area; one set is shown relative to the" +
                       " left paper edge, and another");

            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "set (rotated 180 degrees) is shown relative" +
                       " to the right paper edge.");

            posY1 += lineInc;
            posY1 += lineInc;

            PCLXLWriter.Text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "PJL options may move the unprintable area margins" +
                       " relative to the physical sheet.");

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

        private static void GeneratePage(BinaryWriter prnWriter,
                                         int indxPaperSize,
                                         int indxPaperType,
                                         int indxOrientation,
                                         int indxPlexMode,
                                         string pjlCommand,
                                         bool formAsMacro,
                                         bool trayIdUnknown,
                                         bool customUseMetric,
                                         bool rearFace,
                                         ushort paperWidth,
                                         ushort paperLength,
                                         float scaleText)
        {
            const string digitsTextA = "         1         2" +
                                       "         3         4" +
                                       "         5         6" +
                                       "         7         8" +
                                       "         9        10" +
                                       "        11        12" +
                                       "        13        14" +
                                       "        15        16" +
                                       "        17        18";

            const string digitsTextB = "12345678901234567890" +
                                       "12345678901234567890" +
                                       "12345678901234567890" +
                                       "12345678901234567890" +
                                       "12345678901234567890" +
                                       "12345678901234567890" +
                                       "12345678901234567890" +
                                       "12345678901234567890" +
                                       "12345678901234567890";

            const double unitsToInches = (1.00 / _sessionUPI);
            const double unitsToMilliMetres = (25.4 / _sessionUPI);
            const double unitsToMilliMetreTenths = (254.0 / _sessionUPI);

            const int sizeStd = 1024;

            bool customPaperSize = false;

            byte[] bufStd = new byte[sizeStd];

            bool flagLandscape;

            short squareRightX,
                  squareBottomY;

            short posX,
                  posY;

            int indStd,
                  ctA;

            short lineInc,
                  ptSize;

            indStd = 0;

            //----------------------------------------------------------------//

            flagLandscape = false;

            if (indxOrientation < PCLOrientations.GetCount())
            {
                if (PCLOrientations.IsLandscape(indxOrientation))
                    flagLandscape = true;

                PCLXLWriter.AddAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.Orientation,
                                   PCLOrientations.GetIdPCLXL(indxOrientation));
            }

            if (indxPaperSize < PCLPaperSizes.GetCount())
            {
                customPaperSize = PCLPaperSizes.IsCustomSize(indxPaperSize);

                if (customPaperSize)
                {
                    if (customUseMetric)
                    {
                        ushort width,
                               length;

                        width = (ushort)(Math.Round(paperWidth * unitsToMilliMetreTenths));
                        length = (ushort)(Math.Round(paperLength * unitsToMilliMetreTenths));

                        PCLXLWriter.AddAttrUint16XY(
                            ref bufStd,
                            ref indStd,
                            PCLXLAttributes.eTag.CustomMediaSize,
                            width, length);

                        PCLXLWriter.AddAttrUbyte(
                            ref bufStd,
                            ref indStd,
                            PCLXLAttributes.eTag.CustomMediaSizeUnits,
                            (byte)PCLXLAttrEnums.eVal.eTenthsOfAMillimeter);
                    }
                    else
                    {
                        float width,
                               length;

                        width = (float)(paperWidth * unitsToInches);
                        length = (float)(paperLength * unitsToInches);

                        PCLXLWriter.AddAttrReal32XY(
                            ref bufStd,
                            ref indStd,
                            PCLXLAttributes.eTag.CustomMediaSize,
                            width, length);

                        PCLXLWriter.AddAttrUbyte(
                            ref bufStd,
                            ref indStd,
                            PCLXLAttributes.eTag.CustomMediaSizeUnits,
                            (byte)PCLXLAttrEnums.eVal.eInch);
                    }
                }
                else if (trayIdUnknown)
                {
                    PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                                       ref indStd,
                                       PCLXLAttributes.eTag.MediaSize,
                                       PCLPaperSizes.GetNamePCLXL(indxPaperSize));
                }
                else
                {
                    PCLXLWriter.AddAttrUbyte(ref bufStd,
                                       ref indStd,
                                       PCLXLAttributes.eTag.MediaSize,
                                       PCLPaperSizes.GetIdPCLXL(indxPaperSize));
                }
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

            if ((indxPlexMode < PCLPlexModes.GetCount()) &&
                (!PCLPlexModes.IsSimplex(indxPlexMode)))
            {
                PCLXLWriter.AddAttrUbyte(ref bufStd,
                                 ref indStd,
                                 PCLXLAttributes.eTag.DuplexPageMode,
                                 PCLPlexModes.GetIdPCLXL(indxPlexMode, flagLandscape));

                if (rearFace)
                {
                    PCLXLWriter.AddAttrUbyte(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.eTag.DuplexPageSide,
                                     (byte)PCLXLAttrEnums.eVal.eBackMediaSide);
                }
                else
                {
                    PCLXLWriter.AddAttrUbyte(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.eTag.DuplexPageSide,
                                     (byte)PCLXLAttrEnums.eVal.eFrontMediaSide);
                }
            }
            else
            {
                PCLXLWriter.AddAttrUbyte(ref bufStd,
                                 ref indStd,
                                 PCLXLAttributes.eTag.SimplexPageMode,
                                 (byte)PCLXLAttrEnums.eVal.eSimplexFrontSide);
            }

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
                GenerateOverlay(prnWriter, false, paperWidth, paperLength,
                                scaleText);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Corner squares.                                                //
            //                                                                //
            //----------------------------------------------------------------//

            squareRightX = (short)(paperWidth - _boxOuterEdge);
            squareBottomY = (short)(paperLength - _boxOuterEdge);

            // Top-left.                                                      //

            posX = 0;
            posY = 0;

            GenerateSquare(prnWriter, posX, posY, false, false);

            // Top-right.                                                     //

            posX = squareRightX;
            posY = 0;

            GenerateSquare(prnWriter, posX, posY, false, false);

            // Bottom-left.                                                   //

            posX = 0;
            posY = squareBottomY;

            GenerateSquare(prnWriter, posX, posY, false, false);

            // Bottom-right.                                                  //

            posX = squareRightX;
            posY = squareBottomY;

            GenerateSquare(prnWriter, posX, posY, false, false);

            //----------------------------------------------------------------//
            //                                                                //
            // Paper description data.                                        //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = (short)(10 * scaleText);
            lineInc = (short)((_sessionUPI * scaleText) / 8);

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               0);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            prnWriter.Write(bufStd, 0, indStd);
            indStd = 0;

            PCLXLWriter.Font(prnWriter, false, ptSize,
                       _symSet_19U, "Courier       Bd");

            posX = (short)(_posXDesc + (_rulerCell * scaleText));
            posY = _posYDesc;

            if (customPaperSize)
                PCLXLWriter.Text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       PCLPaperSizes.GetNameAndDesc(indxPaperSize));
            else
                PCLXLWriter.Text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       PCLPaperSizes.GetName(indxPaperSize));

            posY += lineInc;

            PCLXLWriter.Text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       PCLPaperTypes.GetName(indxPaperType));

            posY += lineInc;

            PCLXLWriter.Text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       PCLOrientations.GetName(indxOrientation));

            posY += lineInc;

            if (rearFace)
            {
                PCLXLWriter.Text(prnWriter, false, false,
                           PCLXLWriter.advances_Courier, ptSize,
                           posX, posY,
                           PCLPlexModes.GetName(indxPlexMode) +
                                ": rear face");
            }
            else
            {
                PCLXLWriter.Text(prnWriter, false, false,
                           PCLXLWriter.advances_Courier, ptSize,
                           posX, posY,
                           PCLPlexModes.GetName(indxPlexMode));
            }

            posY += lineInc;

            PCLXLWriter.Text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                      (Math.Round((paperWidth *
                                   unitsToMilliMetres), 2)).ToString("F1") +
                      " mm = " +
                      (Math.Round((paperWidth *
                                   unitsToInches), 3)).ToString("F3") +
                      "\"");

            posY += lineInc;

            PCLXLWriter.Text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                      (Math.Round((paperLength *
                                   unitsToMilliMetres), 2)).ToString("F1") +
                      " mm = " +
                      (Math.Round((paperLength *
                                   unitsToInches), 3)).ToString("F3") +
                      "\"");

            posY += lineInc;

            if (pjlCommand == string.Empty)
                PCLXLWriter.Text(prnWriter, false, false,
                           PCLXLWriter.advances_Courier, ptSize,
                           posX, posY, "<none>");
            else
                PCLXLWriter.Text(prnWriter, false, false,
                           PCLXLWriter.advances_Courier, ptSize,
                           posX, posY, pjlCommand);

            //----------------------------------------------------------------//
            //                                                                //
            // Fixed-pitch 10cpi text - not rotated.                          //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = 12;     // = 10 cpi for Courier

            PCLXLWriter.Font(prnWriter, false, ptSize,
                       _symSet_19U, "Courier         ");

            posY = _posYText;

            ctA = (paperWidth * 10) / _sessionUPI;

            PCLXLWriter.Text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       0, posY, digitsTextA.Substring(0, ctA));

            posY += _rulerDiv;

            PCLXLWriter.Text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       0, posY, digitsTextB.Substring(0, ctA));

            //----------------------------------------------------------------//
            //                                                                //
            // Rotate page coordinate system by 180-degrees, around bottom    //
            // right corner.                                                  //
            //                                                                //
            //----------------------------------------------------------------//

            PCLXLWriter.AddAttrSint16XY(ref bufStd,
                                  ref indStd,
                                  PCLXLAttributes.eTag.PageOrigin,
                                  (short)paperWidth, (short)paperLength);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetPageOrigin);

            PCLXLWriter.AddAttrSint16(ref bufStd,
                                ref indStd,
                                PCLXLAttributes.eTag.PageAngle,
                                180);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetPageRotation);

            prnWriter.Write(bufStd, 0, indStd);
            indStd = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Fixed-pitch 10cpi text - 180-degree rotated.                   //
            //                                                                //
            //----------------------------------------------------------------//

            posY = (short)(paperLength - _posYText - (_rulerDiv * 2.5));

            ctA = (paperWidth * 10) / _sessionUPI;

            PCLXLWriter.Text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       0, posY, digitsTextA.Substring(0, ctA));

            posY += _rulerDiv;

            PCLXLWriter.Text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       0, posY, digitsTextB.Substring(0, ctA));

            //----------------------------------------------------------------//
            //                                                                //
            // Left box: rotated (180-degree) orientation.                    //
            //                                                                //
            //----------------------------------------------------------------//

            posX = squareRightX;
            posY = (short)((paperLength - _boxOuterEdge) / 2);

            GenerateSquare(prnWriter, posX, posY, false, false);

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
        // g e n e r a t e S q u a r e                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate box-in-box square.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void GenerateSquare(BinaryWriter prnWriter,
                                           short startX,
                                           short startY,
                                           bool halfSize,
                                           bool formAsMacro)
        {
            const int sizeStd = 128;

            byte[] bufStd = new byte[sizeStd];

            int indStd = 0;

            int scaler;

            short boxOuterEdge,
                  boxInnerEdge,
                  boxInnerOffset,
                  boxMarkerEdge,
                  boxMarkerOffset;

            //----------------------------------------------------------------//
            //                                                                //
            // Set dimensions for full size or half-size image.               //
            //                                                                //
            //----------------------------------------------------------------//

            if (halfSize)
                scaler = 2;
            else
                scaler = 1;

            boxOuterEdge = (short)(_boxOuterEdge / scaler);
            boxInnerEdge = (short)((_boxOuterEdge / 3) / scaler);
            boxInnerOffset = (short)((_boxOuterEdge / 3) / scaler);
            boxMarkerEdge = (short)((_boxOuterEdge / 15) / scaler);
            boxMarkerOffset = (short)(((_boxOuterEdge -
                                        boxMarkerEdge) / 2) / scaler);

            //----------------------------------------------------------------//

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.PushGS);

            //----------------------------------------------------------------//
            //                                                                //
            // Outer square (black).                                          //
            //                                                                //
            //----------------------------------------------------------------//

            short posX = startX;
            short posY = startY;

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               0);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.AddAttrUint16Box(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.BoundingBox,
                                   (ushort)posX,
                                   (ushort)posY,
                                   (ushort)(posX + boxOuterEdge),
                                   (ushort)(posY + boxOuterEdge));

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.Rectangle);

            //----------------------------------------------------------------//
            //                                                                //
            // Inner square (white).                                          //
            //                                                                //
            //----------------------------------------------------------------//

            posX += boxInnerOffset;
            posY += boxInnerOffset;

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               255);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.AddAttrUint16Box(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.BoundingBox,
                                   (ushort)posX,
                                   (ushort)posY,
                                   (ushort)(posX + boxInnerEdge),
                                   (ushort)(posY + boxInnerEdge));

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.Rectangle);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.PopGS);

            //----------------------------------------------------------------//
            //                                                                //
            // Top marker rectangle (white).                                  //
            //                                                                //
            //----------------------------------------------------------------//

            posX = (short)(startX + boxMarkerOffset);
            posY = startY;

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               255);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.AddAttrUint16Box(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.BoundingBox,
                                   (ushort)posX,
                                   (ushort)posY,
                                   (ushort)(posX + boxMarkerEdge),
                                   (ushort)(posY + boxInnerOffset));

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.Rectangle);

            //----------------------------------------------------------------//
            //                                                                //
            // Left marker rectangle (white).                                 //
            //                                                                //
            //----------------------------------------------------------------//

            posX = startX;
            posY = (short)(startY + boxMarkerOffset);

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               255);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.AddAttrUint16Box(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.BoundingBox,
                                   (ushort)posX,
                                   (ushort)posY,
                                   (ushort)(posX + boxInnerOffset),
                                   (ushort)(posY + boxMarkerEdge));

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.Rectangle);

            //----------------------------------------------------------------//

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.PopGS);

            PCLXLWriter.WriteStreamBlock(prnWriter, formAsMacro,
                                   bufStd, ref indStd);
        }
    }
}
