using System.IO;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides PCL support for the PCL Imaging element of the
    /// Colours action of the MiscSamples tool.
    /// 
    /// © Chris Hutchinson 2014
    /// 
    /// </summary>

    static class ToolMiscSamplesActColourImagingPCL
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
                                       int[] sampleDef,
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
                         sampleDef,
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
                      "PCL imaging colour mode:");

            //----------------------------------------------------------------//

            ptSize = 12;

            PCLWriter.font(prnWriter, true, "19U",
                           PCLFonts.getPCLFontSelect(_indxFontCourier,
                                                      PCLFonts.eVariant.Regular,
                                                      ptSize, 0));
            posY += _incInch / 2;

            PCLWriter.text(prnWriter, posX, posY, 0,
                      "Sample 4-colour palette:");

            //----------------------------------------------------------------//

            posX = (short)(_posXDesc1 - logXOffset);
            posY = _posYDesc1;

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "Colour space");

            //----------------------------------------------------------------//

            posX = (short)(_posXDesc2 - logXOffset);
            posY = _posYDesc2;

            posX = (short)(_posXDesc2 - logXOffset);

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "RGB");

            posX += _colInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "CMY");

            posX += _colInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "SRGB");

            //----------------------------------------------------------------//

            posX = (short)(_posXDesc3 - logXOffset);
            posY = _posYDesc3;

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "index");

            posX += _incInch;

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "value");

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

            //----------------------------------------------------------------//
            //                                                                //
            // Background shade.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            rectX = (short)(_posXDesc2 - (_incInch / 4) - logXOffset);
            rectY = _posYDesc2 + (_incInch / 4);
            rectWidth = (_incInch * 17) / 4;
            rectHeight = (_incInch * 7) / 2;

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
                                         int[] sampleDef,
                                         bool formAsMacro,
                                         ushort logXOffset)
        {
            short posX,
                  posY,
                  rectX,
                  rectY,
                  rectHeight,
                  rectWidth;

            short ptSize;

            int temp;

            byte[] palette_0 = new byte[3],
                    palette_1 = new byte[3],
                    palette_2 = new byte[3],
                    palette_3 = new byte[3];

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

            PCLWriter.font(prnWriter, true, "19U",
                           PCLFonts.getPCLFontSelect(_indxFontCourier,
                                                      PCLFonts.eVariant.Regular,
                                                      ptSize, 0));

            //----------------------------------------------------------------//

            posX = (short)(_posXDesc4 - logXOffset);
            posY = _posYDesc4;

            posX += _incInch;

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "0x" +
                           palette_0[0].ToString("x2") +
                           palette_0[1].ToString("x2") +
                           palette_0[2].ToString("x2"));

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "0x" +
                           palette_1[0].ToString("x2") +
                           palette_1[1].ToString("x2") +
                           palette_1[2].ToString("x2"));

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "0x" +
                           palette_2[0].ToString("x2") +
                           palette_2[1].ToString("x2") +
                           palette_2[2].ToString("x2"));

            posY += _lineInc;

            PCLWriter.text(prnWriter, posX, posY, 0,
                           "0x" +
                           palette_3[0].ToString("x2") +
                           palette_3[1].ToString("x2") +
                           palette_3[2].ToString("x2"));

            //----------------------------------------------------------------//
            //                                                                //
            // RGB colour space.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.configureImageData(prnWriter,
                                          0,    // colour space = RGB
                                          1,    // PEM = Direct-by-pixel
                                          2,    // 2 bitsPerindex -> 4 colours
                                          8,    // bits per component - ignored
                                          8,    // bits per component - ignored
                                          8);   // bits per component - ignored

            PCLWriter.paletteEntry(prnWriter,
                                    0,
                                    palette_0[0], palette_0[1], palette_0[2]);

            PCLWriter.paletteEntry(prnWriter,
                                    1,
                                    palette_1[0], palette_1[1], palette_1[2]);

            PCLWriter.paletteEntry(prnWriter,
                                    2,
                                    palette_2[0], palette_2[1], palette_2[2]);

            PCLWriter.paletteEntry(prnWriter,
                                    3,
                                    palette_3[0], palette_3[1], palette_3[2]);

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

            //----------------------------------------------------------------//
            //                                                                //
            // CMY colour space.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.configureImageData(prnWriter,
                                          1,    // colour space = CMY
                                          1,    // PEM = Direct-by-pixel
                                          2,    // 2 bitsPerindex -> 4 colours
                                          8,    // bits per component - ignored
                                          8,    // bits per component - ignored
                                          8);   // bits per component - ignored

            PCLWriter.paletteEntry(prnWriter,
                                    0,
                                    palette_0[0], palette_0[1], palette_0[2]);

            PCLWriter.paletteEntry(prnWriter,
                                    1,
                                    palette_1[0], palette_1[1], palette_1[2]);

            PCLWriter.paletteEntry(prnWriter,
                                    2,
                                    palette_2[0], palette_2[1], palette_2[2]);

            PCLWriter.paletteEntry(prnWriter,
                                    3,
                                    palette_3[0], palette_3[1], palette_3[2]);

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

            //----------------------------------------------------------------//
            //                                                                //
            // SRGB colour space.                                             //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.configureImageData(prnWriter,
                                          2,    // colour space = SRGB
                                          1,    // PEM = Direct-by-pixel
                                          2,    // 2 bitsPerindex -> 4 colours
                                          8,    // bits per component - ignored
                                          8,    // bits per component - ignored
                                          8);   // bits per component - ignored

            PCLWriter.paletteEntry(prnWriter,
                                    0,
                                    palette_0[0], palette_0[1], palette_0[2]);

            PCLWriter.paletteEntry(prnWriter,
                                    1,
                                    palette_1[0], palette_1[1], palette_1[2]);

            PCLWriter.paletteEntry(prnWriter,
                                    2,
                                    palette_2[0], palette_2[1], palette_2[2]);

            PCLWriter.paletteEntry(prnWriter,
                                    3,
                                    palette_3[0], palette_3[1], palette_3[2]);

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

            //----------------------------------------------------------------//

            PCLWriter.formFeed(prnWriter);
        }
    }
}
