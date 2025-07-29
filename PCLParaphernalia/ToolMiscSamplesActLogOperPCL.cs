using System.IO;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides PCL support for the Logical Operations action
/// of the MiscSamples tool.
/// 
/// © Chris Hutchinson 2014
/// 
/// </summary>

static class ToolMiscSamplesActLogOperPCL
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    const ushort _unitsPerInch = PCLWriter.sessionUPI;

    const short _rasterRes = 600;
    const byte _defaultROP = 252;

    const int _macroIdDestBox = 101;
    const int _macroIdDestBoxRow = 111;
    const int _macroIdDestBoxRowHddr = 112;
    const int _macroIdDestBoxPage = 121;
    const int _macroIdSrcBoxRasterPos = 201;
    const int _macroIdSrcBoxRasterNeg = 202;
    const int _macroIdSrcBoxRasters = 211;
    const int _macroIdSrcBoxText = 212;
    const int _macroIdSrcBox = 221;
    const int _macroIdSrcBoxRow = 231;

    const int _patternId = 101;

    const short _incInch = (_unitsPerInch * 1);
    const short _pageOriginX = (_incInch * 1);
    const short _pageOriginY = (_incInch * 1) / 2;
    const short _rowInc = (_incInch * 5) / 4;
    const short _colInc = (_incInch * 5) / 4;
    const short _lineInc = (_incInch / 6);

    const short _posXPage_1_Hddr = _pageOriginX;
    const short _posYPage_1_Hddr = _pageOriginY + (_incInch * 1) / 2;
    const short _posYPage_1_Data1 = _pageOriginY + (_incInch * 9) / 4;
    const short _posYPage_1_Data2 = _pageOriginY + (_incInch * 13) / 2;

    const short _posXPage_n_Hddr = _pageOriginX;
    const short _posYPage_n_Hddr = _pageOriginY;

    const short _posXPage_n_Data = _pageOriginX;
    const short _posYPage_n_Data = _pageOriginY + (_incInch / 3);

    const short _destBoxSide = _incInch;

    const short _sourceImagePixelsWidth = 192;
    const short _sourceImagePixelsHeight = 192;

    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    static readonly int _indxFontArial = PCLFonts.GetIndexForName("Arial");
    static readonly int _indxFontCourier = PCLFonts.GetIndexForName("Courier");

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
                                   int indxPalette,
                                   int indxClrD1,
                                   int indxClrD2,
                                   int indxClrS1,
                                   int indxClrS2,
                                   int indxClrT1,
                                   int indxClrT2,
                                   int minROP,
                                   int maxROP,
                                   bool flagUseMacros)
    {
        const PCLOrientations.eAspect aspectPort
                = PCLOrientations.eAspect.Portrait;

        PCLOrientations.eAspect aspect;

        ushort paperWidth,
               paperLength,
               paperLengthPort,
               logXOffset;

        bool flagOptColour;

        byte idClrD1 = 0,
             idClrD2 = 0,
             idClrS1 = 0,
             idClrS2 = 0,
             idClrT1 = 0,
             idClrT2 = 0,
             idClrBlack = 0,
             idClrWhite = 0;

        string nameClrD1,
               nameClrD2,
               nameClrS1,
               nameClrS2,
               nameClrT1,
               nameClrT2;

        //----------------------------------------------------------------//

        aspect = PCLOrientations.GetAspect(indxOrientation);

        paperLength = PCLPaperSizes.GetPaperLength(indxPaperSize,
                                                   _unitsPerInch, aspect);

        paperWidth = PCLPaperSizes.GetPaperWidth(indxPaperSize,
                                                 _unitsPerInch, aspect);

        logXOffset = PCLPaperSizes.GetLogicalOffset(indxPaperSize,
                                                    _unitsPerInch, aspect);

        paperLengthPort = PCLPaperSizes.GetPaperLength(indxPaperSize,
                                                       _unitsPerInch,
                                                       aspectPort);

        //----------------------------------------------------------------//

        aspect = PCLOrientations.GetAspect(indxOrientation);

        logXOffset = PCLPaperSizes.GetLogicalOffset(indxPaperSize,
                                                    _unitsPerInch, aspect);

        //----------------------------------------------------------------//

        idClrBlack = PCLPalettes.GetColourId(
                        indxPalette,
                        PCLPalettes.GetClrItemBlack(indxPalette));
        idClrWhite = PCLPalettes.GetColourId(
                        indxPalette,
                        PCLPalettes.GetClrItemWhite(indxPalette));

        idClrD1 = PCLPalettes.GetColourId(indxPalette, indxClrD1);
        idClrS1 = PCLPalettes.GetColourId(indxPalette, indxClrS1);
        idClrT1 = PCLPalettes.GetColourId(indxPalette, indxClrT1);

        idClrD2 = PCLPalettes.GetColourId(indxPalette, indxClrD2);
        idClrS2 = PCLPalettes.GetColourId(indxPalette, indxClrS2);
        idClrT2 = PCLPalettes.GetColourId(indxPalette, indxClrT2);

        nameClrD1 = PCLPalettes.GetColourName(indxPalette, indxClrD1);
        nameClrD2 = PCLPalettes.GetColourName(indxPalette, indxClrD2);
        nameClrS1 = PCLPalettes.GetColourName(indxPalette, indxClrS1);
        nameClrS2 = PCLPalettes.GetColourName(indxPalette, indxClrS2);
        nameClrT1 = PCLPalettes.GetColourName(indxPalette, indxClrT1);
        nameClrT2 = PCLPalettes.GetColourName(indxPalette, indxClrT2);

        if (PCLPalettes.IsMonochrome(indxPalette))
            flagOptColour = false;
        else
            flagOptColour = true;

        //----------------------------------------------------------------//

        GenerateJobHeader(prnWriter,
                          indxPaperSize,
                          indxPaperType,
                          indxOrientation,
                          logXOffset);

        //----------------------------------------------------------------//

        PCLWriter.PaletteSimple(prnWriter,
                                 PCLPalettes.GetPaletteId(indxPalette));

        PCLWriter.RasterResolution(prnWriter, _rasterRes, false);

        if (flagOptColour)
        {
            if (idClrT1 == idClrBlack)                                   // ***** DO WE NEED TO DISTINGUISH THIS *****
                WritePattern(prnWriter, _patternId, idClrT1, idClrT2, false);
            else
                WritePattern(prnWriter, _patternId, idClrT1, idClrT2, true);
        }
        else
        {
            WritePattern(prnWriter, _patternId, idClrT1, idClrT2,
                          false);
        }

        if (flagUseMacros)
        {
            WriteDestBox(prnWriter, idClrD1, idClrD2, idClrBlack,
                          flagOptColour, true);

            WriteDestBoxRow(prnWriter, idClrD1, idClrD2, idClrBlack,
                             flagOptColour, true);

            WriteDestBoxRowHddr(prnWriter, true);

            WriteDestBoxPage(prnWriter, logXOffset, idClrD1, idClrD2,
                              idClrBlack, flagOptColour, true);

            WriteSrcBoxRaster(prnWriter, idClrS1, idClrS2,
                               false, flagOptColour, true);
            WriteSrcBoxRaster(prnWriter, idClrS1, idClrS2,
                               true, flagOptColour, true);

            WriteSrcBoxText(prnWriter, idClrS1, idClrS2, idClrBlack,
                             flagOptColour, true);

            WriteSrcBox(prnWriter, idClrS1, idClrS2, idClrBlack,
                         flagOptColour, true);

            WriteSrcBoxRow(prnWriter, idClrS1, idClrS2, idClrBlack,
                            flagOptColour, true);
        }

        GeneratePageSet(prnWriter,
                         logXOffset,
                         indxPalette,
                         idClrD1,
                         idClrD2,
                         idClrS1,
                         idClrS2,
                         idClrBlack,
                         idClrWhite,
                         nameClrD1,
                         nameClrD2,
                         nameClrS1,
                         nameClrS2,
                         nameClrT1,
                         nameClrT2,
                         minROP,
                         maxROP,
                         flagOptColour,
                         flagUseMacros);

        GenerateJobTrailer(prnWriter, flagUseMacros);
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
                                          ushort logXOffset)
    {
        PCLWriter.StdJobHeader(prnWriter, string.Empty);

        PCLWriter.PageHeader(prnWriter,
                             indxPaperSize,
                             indxPaperType,
                             indxOrientation,
                             (int)PCLPlexModes.eIndex.DuplexLongEdge);
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
                                            bool flagUseMacros)
    {
        PCLWriter.PatternDelete(prnWriter, _patternId);

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, _macroIdSrcBoxRow,
                                    PCLWriter.eMacroControl.Delete);

            PCLWriter.MacroControl(prnWriter, _macroIdSrcBox,
                                    PCLWriter.eMacroControl.Delete);

            PCLWriter.MacroControl(prnWriter, _macroIdSrcBoxText,
                                    PCLWriter.eMacroControl.Delete);

            PCLWriter.MacroControl(prnWriter, _macroIdSrcBoxRasterPos,
                                    PCLWriter.eMacroControl.Delete);

            PCLWriter.MacroControl(prnWriter, _macroIdSrcBoxRasterNeg,
                                    PCLWriter.eMacroControl.Delete);

            PCLWriter.MacroControl(prnWriter, _macroIdDestBoxPage,
                                    PCLWriter.eMacroControl.Delete);

            PCLWriter.MacroControl(prnWriter, _macroIdDestBoxRowHddr,
                                    PCLWriter.eMacroControl.Delete);

            PCLWriter.MacroControl(prnWriter, _macroIdDestBoxRow,
                                    PCLWriter.eMacroControl.Delete);

            PCLWriter.MacroControl(prnWriter, _macroIdDestBox,
                                    PCLWriter.eMacroControl.Delete);
        }

        PCLWriter.StdJobTrailer(prnWriter, false, 0);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e P a g e _ 1                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write introductory page sequences to output file.                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GeneratePage_1(BinaryWriter prnWriter,
                                        ushort logXOffset,
                                        int indxPalette,
                                        byte minROP,
                                        byte maxROP,
                                        byte idClrD1,
                                        byte idClrD2,
                                        byte idClrS1,
                                        byte idClrS2,
                                        byte idClrBlack,
                                        byte idClrWhite,
                                        string nameClrD1,
                                        string nameClrD2,
                                        string nameClrS1,
                                        string nameClrS2,
                                        string nameClrT1,
                                        string nameClrT2,
                                        bool flagOptColour,
                                        bool flagUseMacros)
    {
        short posX,
              posY;

        short ptSize,
              srcOffsetX,
              srcOffsetY;

        string nameClrSpace;

        //----------------------------------------------------------------//

        srcOffsetX = ((_destBoxSide / 2) -
                                _sourceImagePixelsWidth) / 2;
        srcOffsetY = (_destBoxSide -
                               _sourceImagePixelsHeight) / 2;

        //----------------------------------------------------------------//
        //                                                                //
        // Heading and introductory texts.                                //
        //                                                                //
        //----------------------------------------------------------------//

        nameClrSpace = PCLPalettes.GetPaletteName(indxPalette);

        if (flagOptColour)
            PCLWriter.SetForegroundColour(prnWriter, idClrBlack);

        ptSize = 15;

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontArial,
                                                  PCLFonts.eVariant.Bold,
                                                  ptSize, 0));

        posX = (short)(_posXPage_1_Hddr - logXOffset);
        posY = _posYPage_1_Hddr;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "PCL Logical Operations samples:");

        ptSize = 12;

        posY += (_lineInc * 3);

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontArial,
                                                  PCLFonts.eVariant.Regular,
                                                  ptSize, 0));

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Palette = " + nameClrSpace);

        posY += (_lineInc * 2);

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Shows how a Source image, in conjunction with a" +
                  " Texture (a Pattern and colour");

        posY += _lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "combination) interacts with a Destination image" +
                  " (i.e. what is already on the page),");

        posY += _lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "and the effect of the different Logical Operation" +
                  " (ROP) values, together with Source");

        posY += _lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "and Texture (pattern) transparency settings.");

        //----------------------------------------------------------------//

        ptSize = 12;

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontCourier,
                                                  PCLFonts.eVariant.Regular,
                                                  ptSize, 0));
        posY = _posYPage_1_Data1;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "(D)estination:");

        posY += _rowInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "(S)ource:");

        posY += _rowInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "(T)exture (pattern):");

        //----------------------------------------------------------------//
        //                                                                //
        // Destination image.                                             //
        //                                                                //
        //----------------------------------------------------------------//

        posX += (_colInc * 2);
        posY = _posYPage_1_Data1;

        PCLWriter.CursorPosition(prnWriter,
                                  posX,
                                  posY);
        if (flagUseMacros)
            PCLWriter.MacroControl(prnWriter, _macroIdDestBox,
                                    PCLWriter.eMacroControl.Call);
        else
            WriteDestBox(prnWriter, idClrD1, idClrD2, idClrBlack,
                          flagOptColour, false);

        if (flagOptColour)
            PCLWriter.SetForegroundColour(prnWriter, idClrBlack);

        //----------------------------------------------------------------//
        //                                                                //
        // Source image.                                                  //
        //                                                                //
        //----------------------------------------------------------------//

        posY += _rowInc;

        posX += srcOffsetX;
        posY += srcOffsetY;

        PCLWriter.CursorPosition(prnWriter,
                                  posX,
                                  posY);

        ptSize = 28;

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontArial,
                                                  PCLFonts.eVariant.Regular,
                                                  ptSize, 0));

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, _macroIdSrcBox,
                                    PCLWriter.eMacroControl.Call);
        }
        else
        {
            WriteSrcBox(prnWriter, idClrS1, idClrS2, idClrBlack,
                         flagOptColour, false);
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Texture (pattern).                                             //
        //                                                                //
        //----------------------------------------------------------------//

        posY += _rowInc;

        posX -= srcOffsetX;
        posY -= srcOffsetY;

        PCLWriter.RectangleUserFill(prnWriter,
                                     posX,
                                     posY,
                                     _destBoxSide,
                                     _destBoxSide,
                                     _patternId,
                                     false,
                                     false);

        //----------------------------------------------------------------//
        //                                                                //
        // Image explanatory texts.                                       //
        //                                                                //
        //----------------------------------------------------------------//

        posX = (short)(_posXPage_1_Hddr - logXOffset);
        posX += (_rowInc * 3);

        posY = _posYPage_1_Data1;

        ptSize = 8;

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontCourier,
                                                  PCLFonts.eVariant.Regular,
                                                  ptSize, 0));

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "colours = " + nameClrD1 + " / " + nameClrD2);

        //----------------------------------------------------------------//

        posY = _posYPage_1_Data1;
        posY += _rowInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "colours = " + nameClrS1 + " / " + nameClrS2);

        posY += _lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "includes:");

        posY += _lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  " - small square raster image");

        posY += _lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  " - inverse copy of raster image");

        posY += _lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  " - text (the letter 'O' in each colour)");

        //----------------------------------------------------------------//

        posY = _posYPage_1_Data1;
        posY += (_rowInc * 2);

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "colours = " + nameClrT1 + " / " + nameClrT2);

        //----------------------------------------------------------------//
        //                                                                //
        // Sample with default ROP.                                       //
        //                                                                //
        //----------------------------------------------------------------//

        ptSize = 12;

        posX = (short)(_posXPage_1_Hddr - logXOffset);
        posY = _posYPage_1_Data2;

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontCourier,
                                                  PCLFonts.eVariant.Bold,
                                                  ptSize, 0));

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Sample (using default ROP):");

        posY += _rowInc / 2;

        PCLWriter.CursorPosition(prnWriter,
                                  posX,
                                  posY);

        if (flagUseMacros)
            PCLWriter.MacroControl(prnWriter, _macroIdDestBoxRowHddr,
                                    PCLWriter.eMacroControl.Call);
        else
            WriteDestBoxRowHddr(prnWriter, false);

        posY += _incInch / 3;

        ptSize = 10;

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontCourier,
                                                  PCLFonts.eVariant.Regular,
                                                  ptSize, 0));

        PCLWriter.Text(prnWriter, posX, posY, 0,
                        PCLLogicalOperations.GetDescShort(_defaultROP));

        posY += _lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                        PCLLogicalOperations.ActInfix(_defaultROP));

        posY -= _lineInc;
        posX += _colInc;

        PCLWriter.CursorPosition(prnWriter,
                                  posX,
                                  posY);

        if (flagUseMacros)
            PCLWriter.MacroControl(prnWriter, _macroIdDestBoxRow,
                                    PCLWriter.eMacroControl.Call);
        else
            WriteDestBoxRow(prnWriter, idClrD1, idClrD2, idClrBlack,
                             flagOptColour, false);

        //----------------------------------------------------------------//

        ptSize = 28;

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontArial,
                                                  PCLFonts.eVariant.Regular,
                                                  ptSize, 0));

        //----------------------------------------------------------------//

        posX += srcOffsetX;
        posY += srcOffsetY;

        PCLWriter.PatternSet(prnWriter,
                              PCLWriter.ePatternType.UserDefined,
                              _patternId);

        PCLWriter.SetROP(prnWriter, _defaultROP);

        PCLWriter.CursorPosition(prnWriter,
                                  posX,
                                  posY);

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, _macroIdSrcBoxRow,
                                    PCLWriter.eMacroControl.Call);
        }
        else
            WriteSrcBoxRow(prnWriter, idClrS1, idClrS2, idClrBlack,
                            flagOptColour, false);

        //----------------------------------------------------------------//
        //                                                                //
        // Explanatory text for following pages.                          //
        //                                                                //
        //----------------------------------------------------------------//

        if (flagOptColour)
            PCLWriter.SetForegroundColour(prnWriter, idClrBlack);

        PCLWriter.PatternSet(prnWriter,
                              PCLWriter.ePatternType.SolidBlack,
                              -1);

        ptSize = 12;

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontArial,
                                                  PCLFonts.eVariant.Bold,
                                                  ptSize, 0));
        posX -= _colInc;
        posY += _rowInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "The following pages show the effects of the various" +
                  " Logical Operation (ROP)");

        posY += _lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "values (in the range " +
                  PCLLogicalOperations.GetDescShort(minROP) +
                  " - " +
                  PCLLogicalOperations.GetDescShort(maxROP) +
                  "), when combined with");

        posY += _lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "different Source and Texture (pattern) transparency" +
                  " settings:");
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e P a g e _ n                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write individual test data page sequences to output file.          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GeneratePage_n(BinaryWriter prnWriter,
                                        ushort logXOffset,
                                        byte startROP,
                                        byte idClrD1,
                                        byte idClrD2,
                                        byte idClrS1,
                                        byte idClrS2,
                                        byte idClrBlack,
                                        byte idClrWhite,
                                        bool flagOptColour,
                                        bool flagUseMacros)
    {
        short posX,
              posY;

        short ptSize,
              srcOffsetX,
              srcOffsetY;

        //----------------------------------------------------------------//

        srcOffsetX = ((_destBoxSide / 2) -
                                _sourceImagePixelsWidth) / 2;
        srcOffsetY = (_destBoxSide -
                               _sourceImagePixelsHeight) / 2;

        //----------------------------------------------------------------//

        PCLWriter.PatternSet(prnWriter,
                              PCLWriter.ePatternType.SolidBlack,
                              -1);

        PCLWriter.SetROP(prnWriter, _defaultROP);

        ptSize = 10;

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontCourier,
                                                  PCLFonts.eVariant.Regular,
                                                  ptSize, 0));

        posX = (short)(_posXPage_n_Data - logXOffset);
        posY = _posYPage_n_Data;

        for (int i = 0; i < 8; i++)
        {
            PCLWriter.Text(prnWriter, posX, posY, 0,
                            PCLLogicalOperations.GetDescShort(startROP + i));

            posY += _lineInc;

            PCLWriter.Text(prnWriter, posX, posY, 0,
                            PCLLogicalOperations.ActInfix(startROP + i));

            posY -= _lineInc;

            posY += _rowInc;
        }

        //----------------------------------------------------------------//

        if (flagUseMacros)
            PCLWriter.MacroControl(prnWriter, _macroIdDestBoxPage,
                                    PCLWriter.eMacroControl.Call);
        else
            WriteDestBoxPage(prnWriter, logXOffset, idClrD1, idClrD2,
                              idClrBlack, flagOptColour, false);

        //----------------------------------------------------------------//

        if (flagOptColour)
            PCLWriter.SetForegroundColour(prnWriter, idClrBlack);

        //----------------------------------------------------------------//

        ptSize = 28;

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontArial,
                                                  PCLFonts.eVariant.Regular,
                                                  ptSize, 0));

        PCLWriter.PatternSet(prnWriter,
                              PCLWriter.ePatternType.UserDefined,
                              _patternId);

        //----------------------------------------------------------------//

        posX = (short)(_posXPage_n_Data + _colInc - logXOffset);
        posY = _posYPage_n_Data;

        posX += srcOffsetX;
        posY += srcOffsetY;

        PCLWriter.CursorPosition(prnWriter,
                                  posX,
                                  posY);

        //----------------------------------------------------------------//

        for (int i = 0; i < 8; i++)
        {
            PCLWriter.SetROP(prnWriter, (byte)(startROP + i));

            if (flagUseMacros)
            {
                PCLWriter.MacroControl(prnWriter, _macroIdSrcBoxRow,
                                        PCLWriter.eMacroControl.Call);
            }
            else
                WriteSrcBoxRow(prnWriter, idClrS1, idClrS2, idClrBlack,
                                flagOptColour, false);

            posY += +_rowInc;

            PCLWriter.CursorPosition(prnWriter,
                                      posX,
                                      posY);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e P a g e S e t                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write test data page(s) to output file.                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GeneratePageSet(BinaryWriter prnWriter,
                                         ushort logXOffset,
                                         int indxPalette,
                                         byte idClrD1,
                                         byte idClrD2,
                                         byte idClrS1,
                                         byte idClrS2,
                                         byte idClrBlack,
                                         byte idClrWhite,
                                         string nameClrD1,
                                         string nameClrD2,
                                         string nameClrS1,
                                         string nameClrS2,
                                         string nameClrT1,
                                         string nameClrT2,
                                         int minROP,
                                         int maxROP,
                                         bool flagOptColour,
                                         bool flagUseMacros)
    {
        GeneratePage_1(prnWriter,
                        logXOffset,
                        indxPalette,
                        (byte)minROP,
                        (byte)maxROP,
                        idClrD1,
                        idClrD2,
                        idClrS1,
                        idClrS2,
                        idClrBlack,
                        idClrWhite,
                        nameClrD1,
                        nameClrD2,
                        nameClrS1,
                        nameClrS2,
                        nameClrT1,
                        nameClrT2,
                        flagOptColour,
                        flagUseMacros);

        PCLWriter.FormFeed(prnWriter);

        for (int i = minROP; i < maxROP; i += 8)
        {
            GeneratePage_n(prnWriter,
                            logXOffset,
                            (byte)i,
                            idClrD1,
                            idClrD2,
                            idClrS1,
                            idClrS2,
                            idClrBlack,
                            idClrWhite,
                            flagOptColour,
                            flagUseMacros);

            PCLWriter.FormFeed(prnWriter);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // w r i t e D e s t B o x                                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write sequences (either directly, or as a macro definition) for    //
    // the 'destination' box.                                             //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void WriteDestBox(BinaryWriter prnWriter,
                                      byte idClrD1,
                                      byte idClrD2,
                                      byte idClrBlack,
                                      bool flagOptColour,
                                      bool flagUseMacros)
    {
        const short macroId = _macroIdDestBox;
        const short halfBox = _destBoxSide / 2;

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.StartDef);
        }

        PCLWriter.SetForegroundColour(prnWriter, idClrD1);

        PCLWriter.PatternSet(prnWriter,
                              PCLWriter.ePatternType.SolidBlack,
                              -1);

        PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

        PCLWriter.RectangleSolid(prnWriter,
                                  0,
                                  0,
                                  halfBox,
                                  halfBox,
                                  false,
                                  true,
                                  false);

        PCLWriter.CursorRelative(prnWriter,
                                  halfBox,
                                  halfBox);

        PCLWriter.RectangleSolid(prnWriter,
                                  0,
                                  0,
                                  halfBox,
                                  halfBox,
                                  false,
                                  true,
                                  false);

        if (flagOptColour)
        {
            PCLWriter.SetForegroundColour(prnWriter, idClrD2);

            PCLWriter.CursorRelative(prnWriter,
                                      -halfBox,
                                      0);

            PCLWriter.RectangleSolid(prnWriter,
                                      0,
                                      0,
                                      halfBox,
                                      halfBox,
                                      false,
                                      true,
                                      false);

            PCLWriter.CursorRelative(prnWriter,
                                      halfBox,
                                      -halfBox);

            PCLWriter.RectangleSolid(prnWriter,
                                      0,
                                      0,
                                      halfBox,
                                      halfBox,
                                      false,
                                      true,
                                      false);
        }

        PCLWriter.SetForegroundColour(prnWriter, idClrBlack);

        PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                PCLWriter.eMacroControl.StopDef);

            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.MakePermanent);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // w r i t e D e s t B o x P a g e                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write sequences (either directly, or as a macro definition) for    //
    // page of 8 rows of the 4 'destination' boxes, plus column headings. //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void WriteDestBoxPage(BinaryWriter prnWriter,
                                          ushort logXOffset,
                                          byte idClrD1,
                                          byte idClrD2,
                                          byte idClrBlack,
                                          bool flagOptColour,
                                          bool flagUseMacros)
    {
        const short macroId = _macroIdDestBoxPage;

        short posX,
              posY;

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.StartDef);
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Headers.                                                        //
        //                                                                //
        //----------------------------------------------------------------//

        posX = (short)(_posXPage_n_Hddr - logXOffset);
        posY = _posYPage_n_Hddr;

        PCLWriter.CursorPosition(prnWriter,
                                  posX,
                                  posY);

        if (flagUseMacros)
            PCLWriter.MacroControl(prnWriter, _macroIdDestBoxRowHddr,
                                    PCLWriter.eMacroControl.Call);
        else
            WriteDestBoxRowHddr(prnWriter, false);

        //----------------------------------------------------------------//
        //                                                                //
        // Rows of destination boxes.                                     //
        //                                                                //
        //----------------------------------------------------------------//

        posX = (short)(_posXPage_n_Data + _colInc - logXOffset);
        posY = _posYPage_n_Data;

        PCLWriter.CursorPosition(prnWriter,
                                  posX,
                                  posY);

        for (int i = 0; i < 8; i++)
        {
            if (flagUseMacros)
                PCLWriter.MacroControl(prnWriter, _macroIdDestBoxRow,
                                        PCLWriter.eMacroControl.Call);
            else
                WriteDestBoxRow(prnWriter, idClrD1, idClrD2, idClrBlack,
                                 flagOptColour, false);

            PCLWriter.CursorRelative(prnWriter,
                                      0,
                                      _rowInc);
        }

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                PCLWriter.eMacroControl.StopDef);

            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.MakePermanent);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // w r i t e D e s t B o x R o w                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write sequences (either directly, or as a macro definition) for    //
    // row of 4 'destination' boxes.                                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void WriteDestBoxRow(BinaryWriter prnWriter,
                                         byte idClrD1,
                                         byte idClrD2,
                                         byte idClrBlack,
                                         bool flagOptColour,
                                         bool flagUseMacros)
    {
        const short macroId = _macroIdDestBoxRow;

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                PCLWriter.eMacroControl.StartDef);
        }

        PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

        if (flagUseMacros)
            PCLWriter.MacroControl(prnWriter, _macroIdDestBox,     // box 1
                                    PCLWriter.eMacroControl.Call);
        else
            WriteDestBox(prnWriter, idClrD1, idClrD2, idClrBlack,
                          flagOptColour, false);

        PCLWriter.CursorRelative(prnWriter,
                                  _colInc,
                                  0);

        if (flagUseMacros)
            PCLWriter.MacroControl(prnWriter, _macroIdDestBox,     // box 2
                                    PCLWriter.eMacroControl.Call);
        else
            WriteDestBox(prnWriter, idClrD1, idClrD2, idClrBlack,
                          flagOptColour, false);

        PCLWriter.CursorRelative(prnWriter,
                                  _colInc,
                                  0);

        if (flagUseMacros)
            PCLWriter.MacroControl(prnWriter, _macroIdDestBox,     // box 3
                                    PCLWriter.eMacroControl.Call);
        else
            WriteDestBox(prnWriter, idClrD1, idClrD2, idClrBlack,
                          flagOptColour, false);

        PCLWriter.CursorRelative(prnWriter,
                                  _colInc,
                                  0);

        if (flagUseMacros)
            PCLWriter.MacroControl(prnWriter, _macroIdDestBox,     // box 4
                                    PCLWriter.eMacroControl.Call);
        else
            WriteDestBox(prnWriter, idClrD1, idClrD2, idClrBlack,
                          flagOptColour, false);

        PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.StopDef);

            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.MakePermanent);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // w r i t e D e s t B o x R o w H d d r                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write sequences (either directly, or as a macro definition) for    //
    // column headers for samples.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void WriteDestBoxRowHddr(BinaryWriter prnWriter,
                                             bool flagUseMacros)
    {
        const short macroId = _macroIdDestBoxRowHddr;

        short ptSize;

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.StartDef);
        }

        ptSize = 10;

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontArial,
                                                  PCLFonts.eVariant.Regular,
                                                  ptSize, 0));

        //----------------------------------------------------------------//

        PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

        prnWriter.Write("ROP".ToCharArray());

        PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);

        PCLWriter.CursorRelative(prnWriter, _colInc, 0);

        PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

        prnWriter.Write("Source = transparent".ToCharArray());

        PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);
        PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

        PCLWriter.CursorRelative(prnWriter, (_colInc * 2), 0);

        prnWriter.Write("Source = opaque".ToCharArray());

        PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);

        PCLWriter.CursorRelative(prnWriter, 0, (_incInch / 6));

        ptSize = 8;

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontArial,
                                                  PCLFonts.eVariant.Regular,
                                                  ptSize, 0));

        for (int i = 0; i < 2; i++)
        {
            PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

            prnWriter.Write("Pattern=transparent".ToCharArray());

            PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);

            PCLWriter.CursorRelative(prnWriter, _colInc, 0);

            PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

            prnWriter.Write("Pattern=opaque".ToCharArray());

            PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);

            PCLWriter.CursorRelative(prnWriter, _colInc, 0);
        }

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.StopDef);

            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.MakePermanent);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // w r i t e P a t t e r n                                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Define user-defined sample pattern.                                //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void WritePattern(BinaryWriter prnWriter,
                                      short patternId,
                                      byte idClrT1,
                                      byte idClrT2,
                                      bool flagOptColour)
    {
        const ushort patWidth = 16; // multiple of 8
        const ushort patHeight = 16; // multiple of 8

        byte patWidthMS = (patWidth >> 8) & 0xff;
        byte patWidthLS = patWidth & 0xff;
        byte patHeightMS = (patHeight >> 8) & 0xff;
        byte patHeightLS = patHeight & 0xff;

        byte[] patternBase = { 0xC0, 0x01,      // row 00
                                0xE0, 0x00,      //     01
                                0x70, 0x00,      //     02
                                0x38, 0x00,      //     03
                                0x1C, 0x00,      //     04
                                0x0E, 0x00,      //     05
                                0x07, 0x00,      //     06
                                0x03, 0x80,      //     07
                                0x01, 0xC0,      //     08
                                0x00, 0xE0,      //     09
                                0x00, 0x70,      //     10
                                0x00, 0x38,      //     11
                                0x00, 0x1C,      //     12
                                0x00, 0x0E,      //     13
                                0x00, 0x07,      //     14
                                0x80, 0x03 };    //     15

        if (flagOptColour)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Set up format 01 colour pattern, using 8 bits-per-pixel    //
            // elements to index into the current palette (which, using   //
            // simple colour mode, is limited to 8 entries).              //
            //                                                            //
            // Note that there is not a resolution-specified equivalent   //
            // of this format, so the pattern will have the (default)     //
            // resolution of 300 dpi.                                     //
            //                                                            //
            //------------------------------------------------------------//

            const byte format = 1;
            const byte bitsPerPixel = 8;

            int patSize = (patWidth * patHeight * bitsPerPixel) / 8;

            int rowBytes = patWidth / 8;

            int rowVal;

            uint mask;

            int indexIp,
                  indexOp;

            byte[] hddrFmt_01 = { format,
                                  0x00,
                                  bitsPerPixel,
                                  0x00,
                                  patHeightMS,
                                  patHeightLS,
                                  patWidthMS,
                                  patWidthLS };

            byte[] pattern = new byte[patSize];

            for (int i = 0; i < patHeight; i++)
            {
                mask = 0x01 << (patWidth - 1);

                indexIp = i * rowBytes;
                indexOp = i * patWidth;

                rowVal = 0;

                for (int k = 0; k < rowBytes; k++)
                {
                    rowVal = (rowVal * 256) +
                             patternBase[indexIp + k];
                }

                for (int j = 0; j < patWidth; j++)
                {
                    if ((rowVal & mask) != 0)
                        pattern[indexOp + j] = idClrT1;
                    else
                        pattern[indexOp + j] = idClrT2;

                    mask >>= 1;
                }
            }

            PCLWriter.PatternDefine(prnWriter, patternId,
                                     hddrFmt_01, pattern);
        }
        else
        {
            //------------------------------------------------------------//
            //                                                            //
            // Set up format 20 (resolution-specified) monochrome         //
            // pattern.                                                   //
            // But use 300 dpi resolution, to match the colour one (which //
            // always uses the default resolution, similar to format 0).  //
            //                                                            //
            //------------------------------------------------------------//

            const short rasterRes = 300;

            const byte format = 20;
            const byte bitsPerPixel = 1;

            byte rasterResMS = (rasterRes >> 8) & 0xff;
            byte rasterResLS = rasterRes & 0xff;

            byte[] hddrFmt_20 = { format,
                                  0x00,
                                  bitsPerPixel,
                                  0x00,
                                  patHeightMS,
                                  patHeightLS,
                                  patWidthMS,
                                  patWidthLS,
                                  rasterResMS,
                                  rasterResLS,
                                  rasterResMS,
                                  rasterResLS };

            PCLWriter.PatternDefine(prnWriter, patternId,
                                     hddrFmt_20, patternBase);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // w r i t e S r c B o x                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write sequences (either directly, or as a macro definition) for    //
    // source image.                                                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void WriteSrcBox(BinaryWriter prnWriter,
                                     byte idClrS1,
                                     byte idClrS2,
                                     byte idClrBlack,
                                     bool flagOptColour,
                                     bool flagUseMacros)
    {
        const short macroId = _macroIdSrcBox;

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.StartDef);
        }

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, _macroIdSrcBoxText,
                                    PCLWriter.eMacroControl.Call);

            PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

            PCLWriter.MacroControl(prnWriter, _macroIdSrcBoxRasterPos,
                                    PCLWriter.eMacroControl.Call);

            PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);
            PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

            PCLWriter.CursorRelative(prnWriter, _destBoxSide / 2, 0);

            PCLWriter.MacroControl(prnWriter, _macroIdSrcBoxRasterNeg,
                                    PCLWriter.eMacroControl.Call);

            PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);
        }
        else
        {
            WriteSrcBoxText(prnWriter, idClrS1, idClrS2, idClrBlack,
                             flagOptColour, false);

            PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

            WriteSrcBoxRaster(prnWriter, idClrS1, idClrS2,
                               false, flagOptColour, false);

            PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);
            PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

            PCLWriter.CursorRelative(prnWriter, _destBoxSide / 2, 0);

            WriteSrcBoxRaster(prnWriter, idClrS1, idClrS2,
                               true, flagOptColour, false);

            PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);
        }

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.StopDef);

            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.MakePermanent);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // w r i t e S r c B o x R a s t e r                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write sequences (either directly, or as a macro definition) for    //
    // 'source' raster image.                                             //
    //                                                                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void WriteSrcBoxRaster(BinaryWriter prnWriter,
                                           byte idClrS1,
                                           byte idClrS2,
                                           bool inverse,
                                           bool flagOptColour,
                                           bool flagUseMacros)
    {
        const short macroIdPos = _macroIdSrcBoxRasterPos;
        const short macroIdNeg = _macroIdSrcBoxRasterNeg;

        const short blockCt = 7;    // A + B + C + D + C + B + A

        const short compressModeRLE = 1;
        const short compressModeDeltaRow = 3;
        const short compressModeAdaptive = 5;

        const short rowCtA = 16;
        const short rowCtB = 16;
        const short rowCtC = 32;
        const short rowCtD = 64;

        byte[] maskRowAPos = { 0x03, 0xff,
                               0x03, 0x00,
                               0x07, 0xff,
                               0x03, 0x00,
                               0x03, 0xff };
        byte[] maskRowANeg = { 0x03, 0x00,
                               0x03, 0xff,
                               0x07, 0x00,
                               0x03, 0xff,
                               0x03, 0x00 };

        byte[] maskRowBPos = { 0x01, 0xff,
                               0x13, 0x00,
                               0x01, 0xff };
        byte[] maskRowBNeg = { 0x01, 0x00,
                               0x13, 0xff,
                               0x01, 0x00 };

        byte[] maskRowCPos = { 0x03, 0x00,
                               0x03, 0xff,
                               0x07, 0x00,
                               0x03, 0xff,
                               0x03, 0x00 };
        byte[] maskRowCNeg = { 0x03, 0xff,
                               0x03, 0x00,
                               0x07, 0xff,
                               0x03, 0x00,
                               0x03, 0xff };

        byte[] maskRowDPos = { 0x01, 0xff,
                               0x05, 0x00,
                               0x07, 0xff,
                               0x05, 0x00,
                               0x01, 0xff };
        byte[] maskRowDNeg = { 0x01, 0x00,
                               0x05, 0xff,
                               0x07, 0x00,
                               0x05, 0xff,
                               0x01, 0x00 };

        byte[] maskRowCrnt;

        int maskLen;

        short rowCtCrnt;
        short macroId;

        int blockSize,
              rowSize;

        //----------------------------------------------------------------//

        if (inverse)
            macroId = macroIdNeg;
        else
            macroId = macroIdPos;

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.StartDef);
        }

        if (flagOptColour)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Colour mode is Simple Colour mode; this uses a predefined  //
            // palette consisting of eight colours.                       //
            //                                                            //   
            // When using this mode, the pixel encoding mode is always    //
            // Indexed Planar.                                            //
            // A palette of eight values implies three planes (since      //
            // 8 = 2 to power 3).                                         //
            //                                                            //
            // This image uses a mix of compression modes:                //
            //      1 - Run-Length encoded                                //
            //      3 - delta row; the latter is used to repeat rows.     //
            //                                                            //
            //------------------------------------------------------------//

            const int planeCt = 3;

            int indxClr1,
                  indxClr2;

            PCLWriter.RasterBegin(prnWriter,
                                   _sourceImagePixelsWidth,
                                   _sourceImagePixelsHeight,
                                   compressModeRLE);

            //------------------------------------------------------------//

            for (int blockNo = 0; blockNo < blockCt; blockNo++)
            {
                if ((blockNo == 0) || (blockNo == 6))
                {
                    rowCtCrnt = rowCtA;

                    if (inverse)
                        maskRowCrnt = maskRowANeg;
                    else
                        maskRowCrnt = maskRowAPos;
                }
                else if ((blockNo == 1) || (blockNo == 5))
                {
                    rowCtCrnt = rowCtB;

                    if (inverse)
                        maskRowCrnt = maskRowBNeg;
                    else
                        maskRowCrnt = maskRowBPos;
                }
                else if ((blockNo == 2) || (blockNo == 4))
                {
                    rowCtCrnt = rowCtC;

                    if (inverse)
                        maskRowCrnt = maskRowCNeg;
                    else
                        maskRowCrnt = maskRowCPos;
                }
                else
                {
                    rowCtCrnt = rowCtD;

                    if (inverse)
                        maskRowCrnt = maskRowDNeg;
                    else
                        maskRowCrnt = maskRowDPos;
                }

                indxClr1 = idClrS1;
                indxClr2 = idClrS2;

                maskLen = maskRowCrnt.Length;

                byte[] opRow = new byte[maskLen];

                for (int plane = 0; plane < planeCt; plane++)
                {
                    for (int j = 0; j < maskLen; j++)
                    {
                        if ((j & 1) == 0)
                        {
                            // odd bytes are RLE repeat count bytes
                            opRow[j] = maskRowCrnt[j];
                        }
                        else
                        {
                            // even bytes are the RLE bit pattern bytes
                            int opByte = 0;

                            int ipByte = maskRowCrnt[j];

                            for (int k = 0; k < 8; k++)
                            {
                                if (k != 0)
                                {
                                    ipByte <<= 1;
                                    opByte <<= 1;
                                }

                                if ((ipByte & 0x80) != 0)
                                {
                                    if ((indxClr1 & 0x01) != 0)
                                        opByte += 1;
                                }
                                else
                                {
                                    if ((indxClr2 & 0x01) != 0)
                                        opByte += 1;
                                }
                            }

                            opRow[j] = (byte)opByte;
                        }
                    }

                    if (plane < (planeCt - 1))
                        PCLWriter.RasterTransferPlane(prnWriter,
                                                       maskLen,
                                                       opRow);
                    else
                        PCLWriter.RasterTransferRow(prnWriter,
                                                     maskLen,
                                                     opRow);

                    indxClr1 >>= 1;   // next plane of colour
                    indxClr2 >>= 1;   // next plane of colour
                }

                PCLWriter.RasterCompressionMode(prnWriter,
                                                 compressModeDeltaRow);

                for (int j = 1; j < rowCtCrnt; j++)
                {
                    PCLWriter.RasterTransferRow(prnWriter, 0, null);
                }

                if (blockNo < 6)
                {
                    PCLWriter.RasterCompressionMode(prnWriter,
                                                     compressModeRLE);
                }
            }

            //------------------------------------------------------------//

            PCLWriter.RasterEnd(prnWriter);
        }
        else
        {
            //------------------------------------------------------------//
            //                                                            //
            // Monochrome mode is a simple colour mode which uses a       //
            // predefined palette consisting of two entries (black and    //
            // white).                                                    //
            //                                                            //
            // This image uses compression mode 5 ("adaptive              //
            // compression").                                             //
            // Elements use TLLD format:                                  //
            //  Type 01 - data uses Run-length encoding method            //
            //       05 - repeat count; data is null                      //
            //                                                            //
            // This compression mode cannot be used with 'planar'         //
            // pixel-encoding modes - note that Simple Colour mode sets   //
            // 'indexed planar'.                                          //
            //                                                            //
            // Monochrome mode is actually a simple colour mode, and DOES //
            // use indexed planar pixel encoding - but as there is only   //
            // one plane, it works OK.                                    //
            //                                                            //
            //------------------------------------------------------------//

            const int sizeTLL = 3;

            PCLWriter.RasterBegin(prnWriter,
                                   _sourceImagePixelsWidth,
                                   _sourceImagePixelsHeight,
                                   compressModeAdaptive);

            for (int blockNo = 0; blockNo < blockCt; blockNo++)
            {
                byte[] block;

                int offset;

                if ((blockNo == 0) || (blockNo == 6))
                {
                    rowCtCrnt = rowCtA;

                    if (inverse)
                        maskRowCrnt = maskRowANeg;
                    else
                        maskRowCrnt = maskRowAPos;
                }
                else if ((blockNo == 1) || (blockNo == 5))
                {
                    rowCtCrnt = rowCtB;

                    if (inverse)
                        maskRowCrnt = maskRowBNeg;
                    else
                        maskRowCrnt = maskRowBPos;
                }
                else if ((blockNo == 2) || (blockNo == 4))
                {
                    rowCtCrnt = rowCtC;

                    if (inverse)
                        maskRowCrnt = maskRowCNeg;
                    else
                        maskRowCrnt = maskRowCPos;
                }
                else
                {
                    rowCtCrnt = rowCtD;

                    if (inverse)
                        maskRowCrnt = maskRowDNeg;
                    else
                        maskRowCrnt = maskRowDPos;
                }

                rowSize = maskRowCrnt.Length;

                blockSize = sizeTLL + rowSize + sizeTLL;

                block = new byte[blockSize];

                block[0] = 0x01;        // type 01 - RLE
                block[1] = 0x00;        // length - assume < 256;
                block[2] = (byte)rowSize;

                for (int byteNo = 0; byteNo < rowSize; byteNo++)
                {
                    block[3 + byteNo] = maskRowCrnt[byteNo];
                }

                offset = rowSize + 3;

                block[offset] = 0x05;   // type 05 - repeat
                block[offset + 1] = 0x00;   // repeat count - assume < 256
                block[offset + 2] = (byte)(rowCtCrnt - 1);

                PCLWriter.RasterTransferRow(prnWriter,
                                             blockSize,
                                             block);
            }

            PCLWriter.RasterEnd(prnWriter);
        }

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.StopDef);

            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.MakePermanent);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // w r i t e S r c B o x R o w                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write sequences (either directly, or as a macro definition) for    //
    // source image.                                                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void WriteSrcBoxRow(BinaryWriter prnWriter,
                                        byte idClrS1,
                                        byte idClrS2,
                                        byte idClrBlack,
                                        bool flagOptColour,
                                        bool flagUseMacros)
    {
        const short macroId = _macroIdSrcBoxRow;

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.StartDef);
        }

        PCLWriter.SourceTransparency(prnWriter, false);
        PCLWriter.PatternTransparency(prnWriter, false);

        if (flagUseMacros)
            PCLWriter.MacroControl(prnWriter, _macroIdSrcBox,
                                    PCLWriter.eMacroControl.Call);
        else
            WriteSrcBox(prnWriter, idClrS1, idClrS2, idClrBlack,
                         flagOptColour, false);

        //----------------------------------------------------------------//

        PCLWriter.CursorRelative(prnWriter,
                                  _colInc,
                                  0);

        PCLWriter.SourceTransparency(prnWriter, false);
        PCLWriter.PatternTransparency(prnWriter, true);

        if (flagUseMacros)
            PCLWriter.MacroControl(prnWriter, _macroIdSrcBox,
                                     PCLWriter.eMacroControl.Call);
        else
            WriteSrcBox(prnWriter, idClrS1, idClrS2, idClrBlack,
                         flagOptColour, false);

        //----------------------------------------------------------------//

        PCLWriter.CursorRelative(prnWriter,
                                  _colInc,
                                  0);

        PCLWriter.SourceTransparency(prnWriter, true);
        PCLWriter.PatternTransparency(prnWriter, false);

        if (flagUseMacros)
            PCLWriter.MacroControl(prnWriter, _macroIdSrcBox,
                                    PCLWriter.eMacroControl.Call);
        else
            WriteSrcBox(prnWriter, idClrS1, idClrS2, idClrBlack,
                         flagOptColour, false);

        //----------------------------------------------------------------//

        PCLWriter.CursorRelative(prnWriter,
                                  _colInc,
                                  0);

        PCLWriter.SourceTransparency(prnWriter, true);
        PCLWriter.PatternTransparency(prnWriter, true);

        if (flagUseMacros)
            PCLWriter.MacroControl(prnWriter, _macroIdSrcBox,
                                    PCLWriter.eMacroControl.Call);
        else
            WriteSrcBox(prnWriter, idClrS1, idClrS2, idClrBlack,
                         flagOptColour, false);

        //----------------------------------------------------------------//

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.StopDef);

            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.MakePermanent);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // w r i t e S r c B o x T e x t                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write sequences (either directly, or as a macro definition) for    //
    // sample, consisting of multiple copies of 'source' text characters. //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void WriteSrcBoxText(BinaryWriter prnWriter,
                                         byte idClrS1,
                                         byte idClrS2,
                                         byte idClrBlack,
                                         bool flagOptColour,
                                         bool flagUseMacros)
    {
        const short macroId = _macroIdSrcBoxText;

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.StartDef);
        }

        PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

        PCLWriter.CursorRelative(prnWriter, _destBoxSide / 4, 0);

        PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Push);

        PCLWriter.SetForegroundColour(prnWriter, idClrS1);

        prnWriter.Write("O".ToCharArray());

        PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);

        PCLWriter.CursorRelative(prnWriter, 0, (_destBoxSide * 5) / 8);

        PCLWriter.SetForegroundColour(prnWriter, idClrS2);

        prnWriter.Write("O".ToCharArray());

        PCLWriter.SetForegroundColour(prnWriter, idClrBlack);

        PCLWriter.CursorPushPop(prnWriter, PCLWriter.ePushPop.Pop);

        if (flagUseMacros)
        {
            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.StopDef);

            PCLWriter.MacroControl(prnWriter, macroId,
                                    PCLWriter.eMacroControl.MakePermanent);
        }
    }
}
