﻿using System;
using System.IO;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>
    /// Class provides PCL support for the Text and Background element
    /// of the Text Modification action of the MiscSamples tool.
    /// </para>
    /// <para>© Chris Hutchinson 2014</para>
    ///
    /// </summary>
    static class ToolMiscSamplesActTxtModPatPCL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const int _macroId           = 1;
        const ushort _unitsPerInch     = PCLWriter.sessionUPI;

        const short _pageOriginX       = (_unitsPerInch * 1);
        const short _pageOriginY       = (_unitsPerInch * 1);
        const short _incInch           = (_unitsPerInch * 1);
        const short _lineInc           = (_unitsPerInch * 5) / 6;

        const short _posXDesc  = _pageOriginX;
        const short _posXData  = _pageOriginX + (2 * _incInch);

        const short _posYHddr  = _pageOriginY;
        const short _posYDesc  = _pageOriginY + (2 * _incInch);
        const short _posYData  = _pageOriginY + (2 * _incInch);

        const short _shade_1 = 40;
        const short _shade_2 = 20;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Static variables.                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        static readonly int _indxFontArial = PCLFonts.getIndexForName("Arial");
        static readonly int _indxFontCourier = PCLFonts.getIndexForName("Courier");

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

            _logPageWidth = PCLPaperSizes.getLogPageWidth(indxPaperSize,
                                                           _unitsPerInch,
                                                           aspect);

            _logPageHeight = PCLPaperSizes.getLogPageLength(indxPaperSize,
                                                          _unitsPerInch,
                                                          aspect);

            _paperWidth = PCLPaperSizes.getPaperWidth(indxPaperSize,
                                                       _unitsPerInch,
                                                       aspect);

            _paperHeight = PCLPaperSizes.getPaperLength(indxPaperSize,
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
            PCLWriter.stdJobHeader(prnWriter, string.Empty);

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
                  boxWidth,
                  rectX,
                  rectY,
                  rectHeight,
                  rectWidth;

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

            boxX = (short)((_unitsPerInch / 2) - logXOffset);
            boxY = (short)(_unitsPerInch / 2);

            boxWidth = (short)(_paperWidth - _unitsPerInch);
            boxHeight = (short)(_paperHeight - _unitsPerInch);

            PCLWriter.rectangleOutline(prnWriter, boxX, boxY,
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

            ptSize = 15;

            PCLWriter.font(prnWriter, true, "19U",
                           PCLFonts.getPCLFontSelect(_indxFontCourier,
                                                      PCLFonts.eVariant.Bold,
                                                      ptSize, 0));

            posX = (short)(_posXDesc - logXOffset);
            posY = _posYHddr;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "PCL text & background:");

            ptSize = 12;

            PCLWriter.font(prnWriter, true, "19U",
                           PCLFonts.getPCLFontSelect(_indxFontCourier,
                                                      PCLFonts.eVariant.Regular,
                                                      ptSize, 0));

            posY = _posYDesc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "Black:");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "Shade " + _shade_1 + "%:");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "Shade " + _shade_2 + "%:");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "White:");

            //----------------------------------------------------------------//
            //                                                                //
            // Background shading.                                            //
            //                                                                //
            //----------------------------------------------------------------//

            posX = (short)(_posXData - logXOffset);
            posY = _posYData - (_lineInc / 2);

            rectX = posX;
            rectY = posY;

            rectHeight = (short)((_lineInc * 3) / 5);
            rectWidth  = (short)((_unitsPerInch * 9) / 10);

            PCLWriter.patternTransparency(prnWriter, false);

            PCLWriter.patternSet(prnWriter,
                                  PCLWriter.ePatternType.SolidBlack,
                                  0);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectX += rectWidth;

            PCLWriter.rectangleShaded(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, _shade_1,
                                      false, false);

            rectX += rectWidth;

            PCLWriter.rectangleShaded(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, _shade_2,
                                      false, false);

            rectX = posX;
            rectY += _lineInc;

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectX += rectWidth;

            PCLWriter.rectangleShaded(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, _shade_1,
                                      false, false);

            rectX += rectWidth;

            PCLWriter.rectangleShaded(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, _shade_2,
                                      false, false);

            rectX = posX;
            rectY += _lineInc;

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectX += rectWidth;

            PCLWriter.rectangleShaded(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, _shade_1,
                                      false, false);

            rectX += rectWidth;

            PCLWriter.rectangleShaded(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, _shade_2,
                                      false, false);

            rectX = posX;
            rectY += _lineInc;

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectX += rectWidth;

            PCLWriter.rectangleShaded(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, _shade_1,
                                      false, false);

            rectX += rectWidth;

            PCLWriter.rectangleShaded(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, _shade_2,
                                      false, false);

            //----------------------------------------------------------------//
            //                                                                //
            // Overlay end.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.patternSet(prnWriter,
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
            const string sampleText  = "000000000000000";

            short posX,
                  posY;

            short ptSize;

            //----------------------------------------------------------------//

            if (formAsMacro)
                PCLWriter.macroControl(prnWriter, _macroId,
                                       PCLWriter.eMacroControl.Call);
            else
                generateOverlay(prnWriter, false, logXOffset,
                                indxPaperSize, indxOrientation);

            //----------------------------------------------------------------//
            //                                                                //
            // Text.                                                          //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = 36;

            PCLWriter.font(prnWriter, true, "19U",
                           PCLFonts.getPCLFontSelect(_indxFontArial,
                                                      PCLFonts.eVariant.Regular,
                                                      ptSize, 0));

            //----------------------------------------------------------------//
            // Black                                                          //
            //----------------------------------------------------------------//

            posX = (short)(_posXData - logXOffset);
            posY = _posYData;

            PCLWriter.patternTransparency(prnWriter, false);

            PCLWriter.patternSet(prnWriter,
                                 PCLWriter.ePatternType.SolidBlack,
                                 0);

            PCLWriter.text(prnWriter, posX, posY, 0, sampleText);

            //----------------------------------------------------------------//
            // Shade 1                                                        //
            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLWriter.patternTransparency(prnWriter, true);

            PCLWriter.patternSet(prnWriter,
                                 PCLWriter.ePatternType.Shading,
                                 _shade_1);

            PCLWriter.text(prnWriter, posX, posY, 0, sampleText);

            //----------------------------------------------------------------//
            // Shade 2                                                        //
            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLWriter.patternTransparency(prnWriter, true);

            PCLWriter.patternSet(prnWriter,
                                 PCLWriter.ePatternType.Shading,
                                 _shade_2);

            PCLWriter.text(prnWriter, posX, posY, 0, sampleText);

            //----------------------------------------------------------------//
            // White                                                          //
            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLWriter.patternTransparency(prnWriter, true);

            PCLWriter.patternSet(prnWriter,
                                 PCLWriter.ePatternType.SolidWhite,
                                 0);

            PCLWriter.text(prnWriter, posX, posY, 0, sampleText);

            //----------------------------------------------------------------//

            PCLWriter.formFeed(prnWriter);
        }
    }
}
