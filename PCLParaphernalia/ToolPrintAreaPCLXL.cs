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

        public static void generateJob(BinaryWriter prnWriter,
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

            aspect = PCLOrientations.getAspect(indxOrientation);

            A4Length = PCLPaperSizes.getPaperLength(
                            (byte)PCLPaperSizes.eIndex.ISO_A4,
                            _sessionUPI,
                            aspect);

            A4Width = PCLPaperSizes.getPaperWidth(
                            (byte)PCLPaperSizes.eIndex.ISO_A4,
                            _sessionUPI,
                            aspect);

            paperLength = PCLPaperSizes.getPaperLength(indxPaperSize,
                                                       _sessionUPI, aspect);

            paperWidth = PCLPaperSizes.getPaperWidth(indxPaperSize,
                                                      _sessionUPI, aspect);

            scaleTextLength = (float)paperLength / A4Length;
            scaleTextWidth = (float)paperWidth / A4Width;

            if (scaleTextLength < scaleTextWidth)
                scaleText = scaleTextLength;
            else
                scaleText = scaleTextWidth;

            //----------------------------------------------------------------//

            generateJobHeader(prnWriter, pjlCommand);

            if (formAsMacro)
                generateOverlay(prnWriter, true, paperWidth, paperLength,
                                scaleText);

            generatePage(prnWriter,
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

            if (PCLPlexModes.getPlexType(indxPlexMode) !=
                PCLPlexModes.ePlexType.Simplex)
            {
                generatePage(prnWriter,
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

        private static void generateJobHeader(BinaryWriter prnWriter,
                                              string pjlCommand)
        {
            PCLXLWriter.stdJobHeader(prnWriter, pjlCommand);
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
                PCLXLWriter.streamHeader(prnWriter, true, _formName);
            }

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
            // Horizontal ruler.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            posX1 = _rulerOriginX;
            posY1 = _rulerOriginY;
            posX2 = rulerWidth;                 // relative draw X
            posY2 = 0;                          // relative draw Y

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.NewPath);

            PCLXLWriter.addAttrSint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.Point,
                                  posX1, posY1);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetCursor);

            PCLXLWriter.addAttrSint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.EndPoint,
                                  posX2, posY2);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.LineRelPath);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.PaintPath);

            //----------------------------------------------------------------//

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.NewPath);

            PCLXLWriter.addAttrSint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.Point,
                                  posX1, posY1);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetCursor);

            posX1 = _rulerCell;             // relative movement X
            posY1 = 0;                      // relative movement Y
            posX2 = 0;                      // relative draw X
            posY2 = _rulerDiv;              // relative draw Y

            for (int i = 0; i < rulerCellsX; i++)
            {

                PCLXLWriter.addAttrSint16XY(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.eTag.Point,
                                      posX1, posY1);

                PCLXLWriter.addOperator(ref buffer,
                                  ref indBuf,
                                  PCLXLOperators.eTag.SetCursorRel);

                PCLXLWriter.addAttrSint16XY(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.eTag.EndPoint,
                                      posX2, posY2);

                PCLXLWriter.addOperator(ref buffer,
                                  ref indBuf,
                                  PCLXLOperators.eTag.LineRelPath);

                if (i == 0)
                {
                    posY1 = -_rulerDiv;
                }
            }

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.PaintPath);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
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

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.NewPath);

            PCLXLWriter.addAttrSint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.Point,
                                  posX1, posY1);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetCursor);

            PCLXLWriter.addAttrSint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.EndPoint,
                                  posX2, posY2);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.LineRelPath);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.PaintPath);

            //----------------------------------------------------------------//

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.NewPath);

            PCLXLWriter.addAttrSint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.Point,
                                  posX1, posY1);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetCursor);

            posX1 = 0;                      // relative movement X
            posY1 = _rulerCell;             // relative movement Y
            posX2 = _rulerDiv;              // relative draw X
            posY2 = 0;                      // relative draw Y

            for (int i = 0; i < rulerCellsY; i++)
            {
                PCLXLWriter.addAttrSint16XY(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.eTag.Point,
                                      posX1, posY1);

                PCLXLWriter.addOperator(ref buffer,
                                  ref indBuf,
                                  PCLXLOperators.eTag.SetCursorRel);

                PCLXLWriter.addAttrSint16XY(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.eTag.EndPoint,
                                      posX2, posY2);

                PCLXLWriter.addOperator(ref buffer,
                                  ref indBuf,
                                  PCLXLOperators.eTag.LineRelPath);

                if (i == 0)
                {
                    posX1 = -_rulerDiv;
                }
            }

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.PaintPath);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            //----------------------------------------------------------------//
            //                                                                //
            // Sample marker box.                                             //
            //                                                                //
            //----------------------------------------------------------------//

            lineInc = (short)((_sessionUPI * scaleText) / 8);

            posX1 = (short)(_rulerCell * 5.5 * scaleText);
            posY1 = (short)(_posYDesc - lineInc);

            generateSquare(prnWriter, posX1, posY1, true, formAsMacro);

            //----------------------------------------------------------------//
            //                                                                //
            // Text.                                                          //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = (short)(15 * scaleText);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.GrayLevel,
                               0);

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

            PCLXLWriter.font(prnWriter, formAsMacro, ptSize,
                       _symSet_19U, "Arial           ");

            posX1 = _posXHddr;
            posY1 = _posYHddr;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "PCL XL print area sample");

            ptSize = (short)(10 * scaleText);
            lineInc = (short)((_sessionUPI * scaleText) / 8);

            PCLXLWriter.font(prnWriter, formAsMacro, ptSize,
                       _symSet_19U, "Arial           ");

            posX1 = _posXDesc;
            posY1 = _posYDesc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Paper size:");

            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Paper type:");

            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Orientation:");

            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Plex mode:");

            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Paper width:");

            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Paper length:");

            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "PJL option:");

            //----------------------------------------------------------------//

            posY1 = (short)(_posYDesc + (_rulerCell * scaleText));

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Black squares of side 3 units, each containing a" +
                       " central white square of side one");

            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "unit, and some directional markers, as per the" +
                       " half-size sample above,");

            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "demonstrate how objects are clipped by the" +
                       " boundaries of the printable area.");

            posY1 += lineInc;
            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                      "The four corner squares are (theoretically) positioned" +
                      " in the corners of the");

            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                      "physical sheet.");

            posY1 += lineInc;
            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "The middle left-hand square is positioned relative" +
                       " to the bottom and right edges");

            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "of the physical sheet, and rotated 180 degrees.");

            posY1 += lineInc;
            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "Fixed pitch (10 cpi) text characters are also clipped" +
                       " by the boundaries of the");

            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "printable area; one set is shown relative to the" +
                       " left paper edge, and another");

            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialRegular, ptSize,
                       posX1, posY1,
                       "set (rotated 180 degrees) is shown relative" +
                       " to the right paper edge.");

            posY1 += lineInc;
            posY1 += lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
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

            if (indxOrientation < PCLOrientations.getCount())
            {
                if (PCLOrientations.isLandscape(indxOrientation))
                    flagLandscape = true;

                PCLXLWriter.addAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.Orientation,
                                   PCLOrientations.getIdPCLXL(indxOrientation));
            }

            if (indxPaperSize < PCLPaperSizes.getCount())
            {
                customPaperSize = PCLPaperSizes.isCustomSize(indxPaperSize);

                if (customPaperSize)
                {
                    if (customUseMetric)
                    {
                        ushort width,
                               length;

                        width = (ushort)(Math.Round(paperWidth * unitsToMilliMetreTenths));
                        length = (ushort)(Math.Round(paperLength * unitsToMilliMetreTenths));

                        PCLXLWriter.addAttrUint16XY(
                            ref bufStd,
                            ref indStd,
                            PCLXLAttributes.eTag.CustomMediaSize,
                            width, length);

                        PCLXLWriter.addAttrUbyte(
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

                        PCLXLWriter.addAttrReal32XY(
                            ref bufStd,
                            ref indStd,
                            PCLXLAttributes.eTag.CustomMediaSize,
                            width, length);

                        PCLXLWriter.addAttrUbyte(
                            ref bufStd,
                            ref indStd,
                            PCLXLAttributes.eTag.CustomMediaSizeUnits,
                            (byte)PCLXLAttrEnums.eVal.eInch);
                    }
                }
                else if (trayIdUnknown)
                {
                    PCLXLWriter.addAttrUbyteArray(ref bufStd,
                                       ref indStd,
                                       PCLXLAttributes.eTag.MediaSize,
                                       PCLPaperSizes.getNamePCLXL(indxPaperSize));
                }
                else
                {
                    PCLXLWriter.addAttrUbyte(ref bufStd,
                                       ref indStd,
                                       PCLXLAttributes.eTag.MediaSize,
                                       PCLPaperSizes.getIdPCLXL(indxPaperSize));
                }
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

            if ((indxPlexMode < PCLPlexModes.getCount()) &&
                (!PCLPlexModes.isSimplex(indxPlexMode)))
            {
                PCLXLWriter.addAttrUbyte(ref bufStd,
                                 ref indStd,
                                 PCLXLAttributes.eTag.DuplexPageMode,
                                 PCLPlexModes.getIdPCLXL(indxPlexMode, flagLandscape));

                if (rearFace)
                {
                    PCLXLWriter.addAttrUbyte(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.eTag.DuplexPageSide,
                                     (byte)PCLXLAttrEnums.eVal.eBackMediaSide);
                }
                else
                {
                    PCLXLWriter.addAttrUbyte(ref bufStd,
                                     ref indStd,
                                     PCLXLAttributes.eTag.DuplexPageSide,
                                     (byte)PCLXLAttrEnums.eVal.eFrontMediaSide);
                }
            }
            else
            {
                PCLXLWriter.addAttrUbyte(ref bufStd,
                                 ref indStd,
                                 PCLXLAttributes.eTag.SimplexPageMode,
                                 (byte)PCLXLAttrEnums.eVal.eSimplexFrontSide);
            }

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
                generateOverlay(prnWriter, false, paperWidth, paperLength,
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

            generateSquare(prnWriter, posX, posY, false, false);

            // Top-right.                                                     //

            posX = squareRightX;
            posY = 0;

            generateSquare(prnWriter, posX, posY, false, false);

            // Bottom-left.                                                   //

            posX = 0;
            posY = squareBottomY;

            generateSquare(prnWriter, posX, posY, false, false);

            // Bottom-right.                                                  //

            posX = squareRightX;
            posY = squareBottomY;

            generateSquare(prnWriter, posX, posY, false, false);

            //----------------------------------------------------------------//
            //                                                                //
            // Paper description data.                                        //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = (short)(10 * scaleText);
            lineInc = (short)((_sessionUPI * scaleText) / 8);

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               0);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            prnWriter.Write(bufStd, 0, indStd);
            indStd = 0;

            PCLXLWriter.font(prnWriter, false, ptSize,
                       _symSet_19U, "Courier       Bd");

            posX = (short)(_posXDesc + (_rulerCell * scaleText));
            posY = _posYDesc;

            if (customPaperSize)
                PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       PCLPaperSizes.getNameAndDesc(indxPaperSize));
            else
                PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       PCLPaperSizes.getName(indxPaperSize));

            posY += lineInc;

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       PCLPaperTypes.getName(indxPaperType));

            posY += lineInc;

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       PCLOrientations.getName(indxOrientation));

            posY += lineInc;

            if (rearFace)
            {
                PCLXLWriter.text(prnWriter, false, false,
                           PCLXLWriter.advances_Courier, ptSize,
                           posX, posY,
                           PCLPlexModes.getName(indxPlexMode) +
                                ": rear face");
            }
            else
            {
                PCLXLWriter.text(prnWriter, false, false,
                           PCLXLWriter.advances_Courier, ptSize,
                           posX, posY,
                           PCLPlexModes.getName(indxPlexMode));
            }

            posY += lineInc;

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                      (Math.Round((paperWidth *
                                   unitsToMilliMetres), 2)).ToString("F1") +
                      " mm = " +
                      (Math.Round((paperWidth *
                                   unitsToInches), 3)).ToString("F3") +
                      "\"");

            posY += lineInc;

            PCLXLWriter.text(prnWriter, false, false,
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
                PCLXLWriter.text(prnWriter, false, false,
                           PCLXLWriter.advances_Courier, ptSize,
                           posX, posY, "<none>");
            else
                PCLXLWriter.text(prnWriter, false, false,
                           PCLXLWriter.advances_Courier, ptSize,
                           posX, posY, pjlCommand);

            //----------------------------------------------------------------//
            //                                                                //
            // Fixed-pitch 10cpi text - not rotated.                          //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = 12;     // = 10 cpi for Courier

            PCLXLWriter.font(prnWriter, false, ptSize,
                       _symSet_19U, "Courier         ");

            posY = _posYText;

            ctA = (paperWidth * 10) / _sessionUPI;

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       0, posY, digitsTextA.Substring(0, ctA));

            posY += _rulerDiv;

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       0, posY, digitsTextB.Substring(0, ctA));

            //----------------------------------------------------------------//
            //                                                                //
            // Rotate page coordinate system by 180-degrees, around bottom    //
            // right corner.                                                  //
            //                                                                //
            //----------------------------------------------------------------//

            PCLXLWriter.addAttrSint16XY(ref bufStd,
                                  ref indStd,
                                  PCLXLAttributes.eTag.PageOrigin,
                                  (short)paperWidth, (short)paperLength);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetPageOrigin);

            PCLXLWriter.addAttrSint16(ref bufStd,
                                ref indStd,
                                PCLXLAttributes.eTag.PageAngle,
                                180);

            PCLXLWriter.addOperator(ref bufStd,
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

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       0, posY, digitsTextA.Substring(0, ctA));

            posY += _rulerDiv;

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       0, posY, digitsTextB.Substring(0, ctA));

            //----------------------------------------------------------------//
            //                                                                //
            // Left box: rotated (180-degree) orientation.                    //
            //                                                                //
            //----------------------------------------------------------------//

            posX = squareRightX;
            posY = (short)((paperLength - _boxOuterEdge) / 2);

            generateSquare(prnWriter, posX, posY, false, false);

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
        // g e n e r a t e S q u a r e                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate box-in-box square.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateSquare(BinaryWriter prnWriter,
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

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.PushGS);

            //----------------------------------------------------------------//
            //                                                                //
            // Outer square (black).                                          //
            //                                                                //
            //----------------------------------------------------------------//

            short posX = startX;
            short posY = startY;

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               0);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUint16Box(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.BoundingBox,
                                   (ushort)posX,
                                   (ushort)posY,
                                   (ushort)(posX + boxOuterEdge),
                                   (ushort)(posY + boxOuterEdge));

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.Rectangle);

            //----------------------------------------------------------------//
            //                                                                //
            // Inner square (white).                                          //
            //                                                                //
            //----------------------------------------------------------------//

            posX += boxInnerOffset;
            posY += boxInnerOffset;

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               255);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUint16Box(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.BoundingBox,
                                   (ushort)posX,
                                   (ushort)posY,
                                   (ushort)(posX + boxInnerEdge),
                                   (ushort)(posY + boxInnerEdge));

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.Rectangle);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.PopGS);

            //----------------------------------------------------------------//
            //                                                                //
            // Top marker rectangle (white).                                  //
            //                                                                //
            //----------------------------------------------------------------//

            posX = (short)(startX + boxMarkerOffset);
            posY = startY;

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               255);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUint16Box(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.BoundingBox,
                                   (ushort)posX,
                                   (ushort)posY,
                                   (ushort)(posX + boxMarkerEdge),
                                   (ushort)(posY + boxInnerOffset));

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.Rectangle);

            //----------------------------------------------------------------//
            //                                                                //
            // Left marker rectangle (white).                                 //
            //                                                                //
            //----------------------------------------------------------------//

            posX = startX;
            posY = (short)(startY + boxMarkerOffset);

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               255);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUint16Box(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.BoundingBox,
                                   (ushort)posX,
                                   (ushort)posY,
                                   (ushort)(posX + boxInnerOffset),
                                   (ushort)(posY + boxMarkerEdge));

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.Rectangle);

            //----------------------------------------------------------------//

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.PopGS);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
                                   bufStd, ref indStd);
        }
    }
}
