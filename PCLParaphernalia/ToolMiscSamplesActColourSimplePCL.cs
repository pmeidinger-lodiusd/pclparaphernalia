using System.IO;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides PCL support for the PCL Simple element of the
    /// Colours action of the MiscSamples tool.
    /// 
    /// © Chris Hutchinson 2014
    /// 
    /// </summary>

    static class ToolMiscSamplesActColourSimplePCL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const int _macroId = 1;
        const ushort _unitsPerInch = PCLWriter.sessionUPI;

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

        static int _indxFontArial = PCLFonts.getIndexForName("Arial");
        static int _indxFontCourier = PCLFonts.getIndexForName("Courier");

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

            short rectX,
                  rectY,
                  rectHeight,
                  rectWidth;

            byte stroke = 1;

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
            boxY = _unitsPerInch / 2;

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
                      "PCL simple colour mode:");

            //----------------------------------------------------------------//

            ptSize = 12;

            PCLWriter.font(prnWriter, true, "19U",
                           PCLFonts.getPCLFontSelect(_indxFontCourier,
                                                      PCLFonts.eVariant.Regular,
                                                      ptSize, 0));

            //----------------------------------------------------------------//

            posX = (short)(_posXDesc1 - logXOffset);
            posY = _posYDesc1;

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "Palette");

            //----------------------------------------------------------------//

            posX = (short)(_posXDesc2 - logXOffset);
            posY = _posYDesc2;

            posX = (short)(_posXDesc2 - logXOffset);

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "Mono");

            posX += _colInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "RGB");

            posX += _colInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "CMY");

            //----------------------------------------------------------------//

            posX = (short)(_posXDesc3 - logXOffset);
            posY = _posYDesc3;

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "index");

            //----------------------------------------------------------------//

            posX = (short)(_posXDesc4 - logXOffset);
            posY = _posYDesc4;

            PCLWriter.text(prnWriter, posX, posY, 0, "0");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0, "1");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0, "2");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0, "3");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0, "4");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0, "5");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0, "6");

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0, "7");

            //----------------------------------------------------------------//
            //                                                                //
            // Background shade.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            rectX = (short)(_posXDesc2 - (_incInch / 4) - logXOffset);
            rectY = _posYDesc2 + (_incInch / 4);
            rectWidth = (_incInch * 17) / 4;
            rectHeight = _incInch * 7;

            PCLWriter.rectangleShaded(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, 5,
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
            short posX,
                  posY,
                  rectX,
                  rectY,
                  rectHeight,
                  rectWidth;

            //----------------------------------------------------------------//

            if (formAsMacro)
                PCLWriter.macroControl(prnWriter, _macroId,
                                       PCLWriter.eMacroControl.Call);
            else
                generateOverlay(prnWriter, false, logXOffset,
                                indxPaperSize, indxOrientation);

            rectHeight = _lineInc / 2;
            rectWidth = _lineInc;

            //----------------------------------------------------------------//
            //                                                                //
            // Set pattern transparency to Opaque so that white samples show  //
            // on the shaded background.                                      //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.patternTransparency(prnWriter,
                                           true);

            //----------------------------------------------------------------//
            //                                                                //
            // Monochrome palette.                                            //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.paletteSimple(prnWriter,
                                     PCLWriter.eSimplePalette.K);

            posX = (short)(_posXData - logXOffset);
            posY = _posYData;

            rectX = posX;
            rectY = posY;

            PCLWriter.setForegroundColour(prnWriter, 0);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 1);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            //----------------------------------------------------------------//
            //                                                                //
            // RGB palette.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.paletteSimple(prnWriter,
                                     PCLWriter.eSimplePalette.RGB);

            posX += _colInc;

            rectX = posX;
            rectY = posY;

            PCLWriter.setForegroundColour(prnWriter, 0);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 1);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 2);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 3);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 4);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 5);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 6);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 7);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            //----------------------------------------------------------------//
            //                                                                //
            // CMY palette.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.paletteSimple(prnWriter,
                                     PCLWriter.eSimplePalette.CMY);

            posX += _colInc;

            rectX = posX;
            rectY = posY;

            PCLWriter.setForegroundColour(prnWriter, 0);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, true,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 1);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 2);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 3);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 4);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 5);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 6);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            rectY += _lineInc;

            PCLWriter.setForegroundColour(prnWriter, 7);

            PCLWriter.rectangleSolid(prnWriter, rectX, rectY,
                                      rectHeight, rectWidth, false,
                                      false, false);

            //----------------------------------------------------------------//

            PCLWriter.formFeed(prnWriter);
        }
    }
}
