﻿using System;
using System.IO;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>
    /// Class provides PCL support for the Character Modification element
    /// of the Text Modification action of the MiscSamples tool.
    /// </para>
    /// <para>© Chris Hutchinson 2014</para>
    ///
    /// </summary>
    static class ToolMiscSamplesActTxtModChrPCL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const int _macroId            = 1;
        const ushort _unitsPerInch      = PCLWriter.sessionUPI;
        const ushort _plotUnitsPerInch  = PCLWriter.plotterUnitsPerInchHPGL2;

        const short _pageOriginX        = (_unitsPerInch * 1);
        const short _pageOriginY        = (_unitsPerInch * 1);
        const short _incInch            = (_unitsPerInch * 1);
        const short _lineInc            = (_unitsPerInch * 5) / 6;

        const short _posXDesc           = _pageOriginX;
        const short _posXData1          = _pageOriginX + ((3 * _incInch) / 2);
        const short _posXData2          = _pageOriginX + ((7 * _incInch) / 2);

        const short _posYDesc           = _pageOriginY;
        const short _posYData           = _pageOriginY;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Static variables.                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        static readonly int _indxFontArial     = PCLFonts.getIndexForName("Arial");
        static readonly int _indxFontCourier   = PCLFonts.getIndexForName("Courier");

        static int _logPageWidth;
        static int _logPageHeight;
        static int _paperWidth;
        static int _paperHeight;

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
            PCLOrientations.eAspect aspect;

            ushort logXOffset;

            //----------------------------------------------------------------//

            aspect = PCLOrientations.getAspect(indxOrientation);

            logXOffset = PCLPaperSizes.getLogicalOffset(indxPaperSize,
                                                        _unitsPerInch, aspect);

            _logPageWidth = PCLPaperSizes.getLogPageWidth (indxPaperSize,
                                                           _unitsPerInch,
                                                           aspect);

            _logPageHeight = PCLPaperSizes.getLogPageLength (indxPaperSize,
                                                          _unitsPerInch,
                                                          aspect);

            _paperWidth = PCLPaperSizes.getPaperWidth (indxPaperSize,
                                                       _unitsPerInch,
                                                       aspect);

            _paperHeight = PCLPaperSizes.getPaperLength (indxPaperSize,
                                                         _unitsPerInch,
                                                         aspect);

            //----------------------------------------------------------------//

            generateJobHeader(prnWriter,
                              indxPaperSize,
                              indxPaperType,
                              indxOrientation,
                              formAsMacro,
                              logXOffset);

            generatePage(prnWriter,
                         indxPaperSize,
                         indxPaperType,
                         indxOrientation,
                         formAsMacro,
                         logXOffset);

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
                                              int indxPaperSize,
                                              int indxPaperType,
                                              int indxOrientation,
                                              bool formAsMacro,
                                              ushort logXOffset)
        {
            PCLWriter.stdJobHeader(prnWriter, "");

            if (formAsMacro)
                generateOverlay(prnWriter, true, logXOffset,
                                indxPaperSize, indxOrientation);

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
        // Write tray map termination sequences to output file.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateJobTrailer(BinaryWriter prnWriter,
                                               bool formAsMacro)
        {
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
                                            ushort logXOffset,
                                            int indxPaperSize,
                                            int indxOrientation)
        {
            short posX,
                  posY;

            short ptSize;

            short boxX,
                  boxY,
                  boxHeight,
                  boxWidth;

            const byte stroke = 1;

            //----------------------------------------------------------------//
            //                                                                //
            // Header                                                         //
            //                                                                //
            //----------------------------------------------------------------//

            if (formAsMacro)
                PCLWriter.macroControl(prnWriter, _macroId,
                                  PCLWriter.eMacroControl.StartDef);

            //----------------------------------------------------------------//
            //                                                                //
            // Box.                                                           //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.patternSet(prnWriter,
                                  PCLWriter.ePatternType.Shading,
                                  60);

            boxX = (short) ((_unitsPerInch / 2) - logXOffset);
            boxY = (short) (_unitsPerInch / 2);

            boxWidth  = (short) (_paperWidth  - _unitsPerInch);
            boxHeight = (short) (_paperHeight - _unitsPerInch);

            PCLWriter.rectangleOutline (prnWriter, boxX, boxY,
                                        boxHeight, boxWidth, stroke,
                                        false, false);

            //----------------------------------------------------------------//
            //                                                                //
            // Text.                                                          //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.patternSet(prnWriter,
                                  PCLWriter.ePatternType.SolidBlack,
                                  0);

            ptSize  = 15;

            PCLWriter.font(prnWriter, true, "19U",
                           PCLFonts.getPCLFontSelect(_indxFontCourier,
                                                      PCLFonts.eVariant.Bold,
                                                      ptSize, 0));

            posX = (short)(_posXDesc - logXOffset);
            posY = _posYDesc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "PCL & HP-GL/2 Font Embellishments:");

            ptSize  = 12;

            PCLWriter.font(prnWriter, true, "19U",
                           PCLFonts.getPCLFontSelect(_indxFontCourier,
                                                      PCLFonts.eVariant.Regular,
                                                      ptSize, 0));

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "Font:");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "Size X:");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "Size Y:");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "Slant X:");

            posY += _lineInc;
            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "Extra space:");

            //----------------------------------------------------------------//
            //                                                                //
            // Overlay end.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.patternSet (prnWriter,
                                  PCLWriter.ePatternType.SolidBlack,
                                  0);

            if (formAsMacro)
                PCLWriter.macroControl(prnWriter, 0,
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

        private static void generatePage(BinaryWriter prnWriter,
                                         int indxPaperSize,
                                         int indxPaperType,
                                         int indxOrientation,
                                         bool formAsMacro,
                                         ushort logXOffset)
        {
            const string sampleText  = "0123456789";
            const string lbTerm      = "~";

            short posX,
                  posY;

            short ptSize,
                  degrees;

            short boxX,
                  boxY,
                  boxHeight,
                  boxWidth;

            double scaleX,
                   scaleY;

            double angle,
                   tanAngle;

            //----------------------------------------------------------------//

            if (formAsMacro)
                PCLWriter.macroControl(prnWriter, _macroId,
                                       PCLWriter.eMacroControl.Call);
            else
                generateOverlay(prnWriter, false, logXOffset,
                                indxPaperSize, indxOrientation);

            //----------------------------------------------------------------//
            //                                                                //
            // HP-GL/2 picture frame and initialisation.                      //
            //                                                                //
            // Plotter units are always absolute at 0.025mm (1/1016 inch),    //
            // but many HP-GL/2 commands use (definable) user units.          //
            // It makes the code clearer if we use the same units in HP-GL/2  //
            // as we do in PCL, so the SC (scale) command is used to set      //
            // user-units to 600 units-per-inch.                              //
            //                                                                //
            // Note that the default HP-GL/2 Y-axis has its origin at         //
            // lower-left of the picture frame, and Y-coordinate values       //
            // increase UP the page, whilst the PCL Y-axis has its origin at  //
            // the top margin and Y-coordinate values increase DOWN the page. // 
            //                                                                //
            // It is possible to use the same (600 upi) coordinates as PCL by //
            // using:                                                         //
            //  SC0,1.6933,0,-1.6933,2                                        // 
            //  IR0,100,100,0                                                 //   
            // Note that the IR coordinates shown in the example in the "PCL  //
            // Technical Reference" manual are different and are incorrect!   //
            // One drawback to using the same origin and axis direction is    //
            // that some commands (such as SR) then have to use negative      //
            // Y-values to avoid mirroring.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            scaleX = (double)_plotUnitsPerInch / _unitsPerInch;
            scaleY = (double)_plotUnitsPerInch / _unitsPerInch;

            boxX = 0;
            boxY = 0;
            boxWidth  = (short)_logPageWidth;
            boxHeight = (short)_logPageHeight;

            PCLWriter.pictureFrame (prnWriter,
                                    boxX,
                                    boxY,
                                    boxHeight,
                                    boxWidth);

            PCLWriter.modeHPGL2 (prnWriter, false, false);

            PCLWriter.cmdHPGL2 (prnWriter, "IN", "", false);
            PCLWriter.cmdHPGL2 (prnWriter, "SP", "1", true);
            PCLWriter.cmdHPGL2 (prnWriter, "DT", "~", false);

            PCLWriter.cmdHPGL2(prnWriter, "SC",
                                "0," + scaleX.ToString("F4") +
                                ",0," + (-scaleY).ToString("F4") +
                                ",2",
                                false);

            PCLWriter.cmdHPGL2 (prnWriter, "IR", "0,100,100,0", false);
            PCLWriter.cmdHPGL2 (prnWriter, "PU", "0,0", true);

            PCLWriter.modePCL (prnWriter, true);

            //----------------------------------------------------------------//
            //                                                                //
            // Descriptive text.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = 18;

            PCLWriter.font(prnWriter, true, "19U",
                           PCLFonts.getPCLFontSelect(_indxFontCourier,
                                                      PCLFonts.eVariant.Regular,
                                                      ptSize, 0));

            posX = (short)(_posXData1 - logXOffset);
            posY = _posYDesc;

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "Arial");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "+30");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "-45");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "+ve");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "-ve");

            //----------------------------------------------------------------//
            //                                                                //
            // Embellished text.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = 36;

            posX = (short)(_posXData2 - logXOffset);
            posY = _posYData;

            //----------------------------------------------------------------//
            // standard                                                       //
            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLWriter.modeHPGL2(prnWriter, false, false);

            PCLWriter.cmdHPGL2(prnWriter, "SD",
                                PCLFonts.getHPGL2FontDef(_indxFontArial,
                                       PCLFonts.eVariant.Regular,
                                       14, ptSize, 0),
                                true);

            PCLWriter.cmdHPGL2(prnWriter, "PA",
                                posX.ToString() + "," +
                                posY.ToString(), false);

            PCLWriter.cmdHPGL2(prnWriter, "LB", sampleText + lbTerm, true);

            //----------------------------------------------------------------//
            // size X                                                         //
            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLWriter.cmdHPGL2(prnWriter, "SD",
                                PCLFonts.getHPGL2FontDef(_indxFontArial,
                                       PCLFonts.eVariant.Regular,
                                       14, ptSize, 0),
                                true);

            PCLWriter.cmdHPGL2(prnWriter, "PA",
                                posX.ToString() + "," +
                                posY.ToString(), false);

            PCLWriter.cmdHPGL2(prnWriter, "SR", "4,-3", true);

            PCLWriter.cmdHPGL2(prnWriter, "LB", sampleText + lbTerm, true);

            PCLWriter.cmdHPGL2(prnWriter, "SR", "", true);

            //----------------------------------------------------------------//
            // size Y                                                         //
            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLWriter.cmdHPGL2(prnWriter, "SD",
                                PCLFonts.getHPGL2FontDef(_indxFontArial,
                                       PCLFonts.eVariant.Regular,
                                       14, ptSize, 0),
                                true);

            PCLWriter.cmdHPGL2(prnWriter, "PA",
                                posX.ToString() + "," +
                                posY.ToString(), false);

            PCLWriter.cmdHPGL2(prnWriter, "SR", "3.2,-6", true);

            PCLWriter.cmdHPGL2(prnWriter, "LB", sampleText + lbTerm, true);

            PCLWriter.cmdHPGL2(prnWriter, "SI", "", true);

            //----------------------------------------------------------------//
            // slant X positive                                               //
            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLWriter.cmdHPGL2(prnWriter, "SD",
                                PCLFonts.getHPGL2FontDef(_indxFontArial,
                                       PCLFonts.eVariant.Regular,
                                       14, ptSize, 0),
                                true);

            PCLWriter.cmdHPGL2(prnWriter, "PA",
                                posX.ToString() + "," +
                                posY.ToString(), false);

            degrees = 30;
            angle = Math.PI * degrees / 180.0;
            tanAngle = Math.Tan(angle);

            PCLWriter.cmdHPGL2 (prnWriter, "SL",
                                tanAngle.ToString(), false);

            PCLWriter.cmdHPGL2(prnWriter, "LB", sampleText + lbTerm, true);

            //----------------------------------------------------------------//
            // slant X negative                                               //
            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLWriter.cmdHPGL2(prnWriter, "SD",
                                PCLFonts.getHPGL2FontDef(_indxFontArial,
                                       PCLFonts.eVariant.Regular,
                                       14, ptSize, 0),
                                true);

            PCLWriter.cmdHPGL2(prnWriter, "PA",
                                posX.ToString() + "," +
                                posY.ToString(), false);

            degrees = 45;
            angle = Math.PI * degrees / 180.0;
            tanAngle = Math.Tan(angle);

            PCLWriter.cmdHPGL2(prnWriter, "SL",
                                "-" + tanAngle.ToString(), false);

            PCLWriter.cmdHPGL2(prnWriter, "LB", sampleText + lbTerm, true);

            PCLWriter.cmdHPGL2(prnWriter, "SL","0", false);

            //----------------------------------------------------------------//
            // extra space X positive                                         //
            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLWriter.cmdHPGL2(prnWriter, "SD",
                                PCLFonts.getHPGL2FontDef(_indxFontArial,
                                       PCLFonts.eVariant.Regular,
                                       14, ptSize, 0),
                                true);

            PCLWriter.cmdHPGL2(prnWriter, "PA",
                                posX.ToString() + "," +
                                posY.ToString(), false);

            PCLWriter.cmdHPGL2(prnWriter, "ES", ".2,0", false);

            PCLWriter.cmdHPGL2(prnWriter, "LB", sampleText + lbTerm, true);

            //----------------------------------------------------------------//
            // extra space X negative                                         //
            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLWriter.cmdHPGL2(prnWriter, "SD",
                                PCLFonts.getHPGL2FontDef(_indxFontArial,
                                       PCLFonts.eVariant.Regular,
                                       14, ptSize, 0),
                                true);

            PCLWriter.cmdHPGL2(prnWriter, "PA",
                                posX.ToString() + "," +
                                posY.ToString(), false);

            PCLWriter.cmdHPGL2(prnWriter, "ES", "-.1,0", false);

            PCLWriter.cmdHPGL2(prnWriter, "LB", sampleText + lbTerm, true);

            //----------------------------------------------------------------//

            PCLWriter.modePCL(prnWriter, true);

            PCLWriter.formFeed(prnWriter);
        }
    }
}
