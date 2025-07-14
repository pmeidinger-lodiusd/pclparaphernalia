using System.IO;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides PCL support for the Shading element of the
    /// Patterns action of the MiscSamples tool.
    /// 
    /// © Chris Hutchinson 2014
    /// 
    /// </summary>

    static class ToolMiscSamplesActPatternShadePCL
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

        const short _posXDesc = _pageOriginX;
        const short _posXData1 = _pageOriginX + ((7 * _incInch) / 3);
        const short _posXData2 = _posXData1 + ((3 * _incInch / 2));
        const short _posXData3 = _posXData2 + ((3 * _incInch / 2));

        const short _posYHddr = _pageOriginY;
        const short _posYDesc1 = _pageOriginY + (2 * _incInch);
        const short _posYDesc2 = _pageOriginY + ((3 * _incInch / 2));
        const short _posYData = _pageOriginY + (2 * _incInch);

        const short _patternBase_300 = 300;
        const short _patternBase_600 = 600;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Static variables.                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        static readonly int _indxFontArial = PCLFonts.GetIndexForName("Arial");
        static readonly int _indxFontCourier = PCLFonts.GetIndexForName("Courier");

        static int _logPageWidth;
        static int _logPageHeight;
        static int _paperWidth;
        static int _paperHeight;

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

        public static void GenerateJob(BinaryWriter prnWriter,
                                       int indxPaperSize,
                                       int indxPaperType,
                                       int indxOrientation,
                                       bool formAsMacro)
        {
            PCLOrientations.eAspect aspect;

            ushort logXOffset;

            //----------------------------------------------------------------//

            aspect = PCLOrientations.GetAspect(indxOrientation);

            logXOffset = PCLPaperSizes.GetLogicalOffset(indxPaperSize,
                                                        _unitsPerInch, aspect);

            _logPageWidth = PCLPaperSizes.GetLogPageWidth(indxPaperSize,
                                                           _unitsPerInch,
                                                           aspect);

            _logPageHeight = PCLPaperSizes.GetLogPageLength(indxPaperSize,
                                                          _unitsPerInch,
                                                          aspect);

            _paperWidth = PCLPaperSizes.GetPaperWidth(indxPaperSize,
                                                       _unitsPerInch,
                                                       aspect);

            _paperHeight = PCLPaperSizes.GetPaperLength(indxPaperSize,
                                                         _unitsPerInch,
                                                         aspect);

            //----------------------------------------------------------------//

            GetPatternData();

            GenerateJobHeader(prnWriter,
                              indxPaperSize,
                              indxPaperType,
                              indxOrientation,
                              formAsMacro,
                              logXOffset);

            PatternDefineDpi300(prnWriter, _patternBase_300);

            PatternDefineDpi600(prnWriter, _patternBase_600);

            GeneratePage(prnWriter,
                         indxPaperSize,
                         indxPaperType,
                         indxOrientation,
                         formAsMacro,
                         logXOffset);

            PatternDeleteSet(prnWriter, _patternBase_300);

            PatternDeleteSet(prnWriter, _patternBase_600);

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
                                              int indxPaperSize,
                                              int indxPaperType,
                                              int indxOrientation,
                                              bool formAsMacro,
                                              ushort logXOffset)
        {
            PCLWriter.StdJobHeader(prnWriter, string.Empty);

            if (formAsMacro)
                GenerateOverlay(prnWriter, true, logXOffset,
                                indxPaperSize, indxOrientation);

            PCLWriter.PageHeader(prnWriter,
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

        private static void GenerateJobTrailer(BinaryWriter prnWriter,
                                               bool formAsMacro)
        {
            PCLWriter.StdJobTrailer(prnWriter, formAsMacro, _macroId);
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

            byte stroke = 1;

            //----------------------------------------------------------------//
            //                                                                //
            // Header                                                         //
            //                                                                //
            //----------------------------------------------------------------//

            if (formAsMacro)
                PCLWriter.MacroControl(prnWriter, _macroId,
                                  PCLWriter.eMacroControl.StartDef);

            //----------------------------------------------------------------//
            //                                                                //
            // Box.                                                           //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.PatternSet(prnWriter,
                                  PCLWriter.ePatternType.Shading,
                                  60);

            boxX = (short)((_unitsPerInch / 2) - logXOffset);
            boxY = _unitsPerInch / 2;

            boxWidth = (short)(_paperWidth - _unitsPerInch);
            boxHeight = (short)(_paperHeight - _unitsPerInch);

            PCLWriter.RectangleOutline(prnWriter, boxX, boxY,
                                        boxHeight, boxWidth, stroke,
                                        false, false);

            //----------------------------------------------------------------//
            //                                                                //
            // Text.                                                          //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.PatternSet(prnWriter,
                                  PCLWriter.ePatternType.SolidBlack,
                                  0);

            ptSize = 15;

            PCLWriter.Font(prnWriter, true, "19U",
                           PCLFonts.GetPCLFontSelect(_indxFontCourier,
                                                      PCLFonts.eVariant.Bold,
                                                      ptSize, 0));

            posX = (short)(_posXDesc - logXOffset);
            posY = _posYHddr;

            PCLWriter.Text(prnWriter, posX, posY, 0,
                      "PCL shading patterns:");

            //----------------------------------------------------------------//

            ptSize = 12;

            PCLWriter.Font(prnWriter, true, "19U",
                           PCLFonts.GetPCLFontSelect(_indxFontCourier,
                                                      PCLFonts.eVariant.Regular,
                                                      ptSize, 0));

            //----------------------------------------------------------------//

            posY = _posYDesc1;

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLWriter.Text(prnWriter, posX, posY, 0,
                               "#" + (i + 1).ToString() + ": ");

                posY += _lineInc;
            }

            //----------------------------------------------------------------//

            ptSize = 10;

            PCLWriter.Font(prnWriter, true, "19U",
                           PCLFonts.GetPCLFontSelect(_indxFontCourier,
                                                      PCLFonts.eVariant.Regular,
                                                      ptSize, 0));

            //----------------------------------------------------------------//

            posY = _posYDesc1 + (_lineInc / 4);

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLWriter.Text(prnWriter, posX, posY, 0,
                               _patternDescs[i] + ":");

                posY += _lineInc;
            }

            //----------------------------------------------------------------//

            ptSize = 8;

            PCLWriter.Font(prnWriter, true, "19U",
                           PCLFonts.GetPCLFontSelect(_indxFontCourier,
                                                      PCLFonts.eVariant.Regular,
                                                      ptSize, 0));

            //----------------------------------------------------------------//

            posY = _posYDesc2;
            posX = (short)(_posXData1 - logXOffset);

            PCLWriter.Text(prnWriter, posX, posY, 0,
                      "Predefined");

            posX = (short)(_posXData2 - logXOffset);

            PCLWriter.Text(prnWriter, posX, posY, 0,
                      "User-defined 300 dpi");

            posX = (short)(_posXData3 - logXOffset);

            PCLWriter.Text(prnWriter, posX, posY, 0,
                      "User-defined 600 dpi");

            //----------------------------------------------------------------//
            //                                                                //
            // Overlay end.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.PatternSet(prnWriter,
                                  PCLWriter.ePatternType.SolidBlack,
                                  0);

            if (formAsMacro)
                PCLWriter.MacroControl(prnWriter, 0,
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

        private static void GeneratePage(BinaryWriter prnWriter,
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
                PCLWriter.MacroControl(prnWriter, _macroId,
                                       PCLWriter.eMacroControl.Call);
            else
                GenerateOverlay(prnWriter, false, logXOffset,
                                indxPaperSize, indxOrientation);

            rectHeight = _lineInc / 2;
            rectWidth = _lineInc;

            //----------------------------------------------------------------//
            //                                                                //
            // Pre-defined shading.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            posX = (short)(_posXData1 - logXOffset);
            posY = _posYData;

            rectX = posX;
            rectY = posY;

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLWriter.RectangleShaded(prnWriter, rectX, rectY,
                                          rectHeight, rectWidth,
                                          (short)_patternIds[i],
                                          false, false);

                rectY += _lineInc;
            }

            //----------------------------------------------------------------//
            //                                                                //
            // User-defined 300 dpi shading.                                  //
            //                                                                //
            //----------------------------------------------------------------//

            posX = (short)(_posXData2 - logXOffset);
            posY = _posYData;

            rectX = posX;
            rectY = posY;

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLWriter.RectangleUserFill(
                    prnWriter, rectX, rectY,
                    rectHeight, rectWidth,
                    (short)(_patternBase_300 + _patternIds[i]),
                    false, false);

                rectY += _lineInc;
            }

            //----------------------------------------------------------------//
            //                                                                //
            // User-defined 600 dpi shading.                                  //
            //                                                                //
            //----------------------------------------------------------------//

            posX = (short)(_posXData3 - logXOffset);
            posY = _posYData;

            rectX = posX;
            rectY = posY;

            for (int i = 0; i < _patternsCt; i++)
            {
                PCLWriter.RectangleUserFill(
                    prnWriter, rectX, rectY,
                    rectHeight, rectWidth,
                    (short)(_patternBase_600 + _patternIds[i]),
                    false, false);

                rectY += _lineInc;
            }

            //----------------------------------------------------------------//

            PCLWriter.FormFeed(prnWriter);
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
            _patternsCt = PCLPatternDefs.GetCount(
                PCLPatternDefs.eType.Shading);

            _patternIds = new ushort[_patternsCt];
            _patternHeights = new ushort[_patternsCt];
            _patternWidths = new ushort[_patternsCt];
            _patternDescs = new string[_patternsCt];

            for (int i = 0; i < _patternsCt; i++)
            {
                _patternIds[i] = PCLPatternDefs.GetId(
                    PCLPatternDefs.eType.Shading, i);
                _patternHeights[i] = PCLPatternDefs.GetHeight(
                    PCLPatternDefs.eType.Shading, i);
                _patternWidths[i] = PCLPatternDefs.GetWidth(
                    PCLPatternDefs.eType.Shading, i);
                _patternDescs[i] = PCLPatternDefs.GetDesc(
                    PCLPatternDefs.eType.Shading, i);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a t t e r n D e f i n e D p i 3 0 0                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Define default user-defined patterns to match the pre-defined      //
        // patterns.                                                          //
        // The format 0 pattern header does not define a resolution, so (we   //
        // assume) that the pattern will use the default 300 dots-per-inch.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PatternDefineDpi300(BinaryWriter prnWriter,
                                                 int baseID)
        {
            byte[] hddrFmt_0 = { 0x00, 0x00, 0x01, 0x00,
                                 0x00, 0x10, 0x00, 0x10 };

            for (int i = 0; i < _patternsCt; i++)
            {
                hddrFmt_0[4] = (byte)((_patternHeights[i] & 0xff00) >> 8);
                hddrFmt_0[5] = (byte)(_patternHeights[i] & 0x00ff);

                hddrFmt_0[6] = (byte)((_patternWidths[i] & 0xff00) >> 8);
                hddrFmt_0[7] = (byte)(_patternWidths[i] & 0x00ff);

                PCLWriter.PatternDefine(
                    prnWriter, (short)(baseID + _patternIds[i]),
                    hddrFmt_0,
                    PCLPatternDefs.GetBytes(
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
        // The format 20 pattern header defines X & Y resolutions, which we   //
        // are setting to 600 dots-per-inch.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PatternDefineDpi600(BinaryWriter prnWriter,
                                                int baseID)
        {
            byte[] hddrFmt_20 = { 0x14, 0x00, 0x01, 0x00,
                                  0x00, 0x10, 0x00, 0x10,
                                  0x02, 0x58, 0x02, 0x58 };

            for (int i = 0; i < _patternsCt; i++)
            {
                hddrFmt_20[4] = (byte)((_patternHeights[i] & 0xff00) >> 8);
                hddrFmt_20[5] = (byte)(_patternHeights[i] & 0x00ff);

                hddrFmt_20[6] = (byte)((_patternWidths[i] & 0xff00) >> 8);
                hddrFmt_20[7] = (byte)(_patternWidths[i] & 0x00ff);

                PCLWriter.PatternDefine(
                    prnWriter, (short)(baseID + _patternIds[i]),
                    hddrFmt_20,
                    PCLPatternDefs.GetBytes(
                        PCLPatternDefs.eType.Shading, i));
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a t t e r n D e l e t e S e t                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Delete user-defined patterns.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PatternDeleteSet(BinaryWriter prnWriter,
                                             int baseID)
        {
            for (int i = 0; i < _patternsCt; i++)
            {
                PCLWriter.PatternDelete(
                    prnWriter, (short)(baseID + _patternIds[i]));
            }
        }
    }
}
