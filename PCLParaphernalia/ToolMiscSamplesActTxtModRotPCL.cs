using System;
using System.IO;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides PCL support for the Rotation element
/// of the Text Modification action of the MiscSamples tool.
/// 
/// © Chris Hutchinson 2014
/// 
/// </summary>

static class ToolMiscSamplesActTxtModRotPCL
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    const int _macroId = 1;
    const ushort _unitsPerInch = PCLWriter.sessionUPI;
    const ushort _plotUnitsPerInch = PCLWriter.plotterUnitsPerInchHPGL2;

    const short _pageOriginX = (_unitsPerInch * 1);
    const short _pageOriginY = (_unitsPerInch * 1);
    const short _incInch = (_unitsPerInch * 1);
    const short _lineInc = (_unitsPerInch * 5) / 6;

    const short _posXDesc = _pageOriginX;
    const short _posXData1 = _pageOriginX + ((9 * _incInch) / 2);
    const short _posXData2 = _pageOriginX + ((9 * _incInch) / 2);

    const short _posYHddr = _pageOriginY;
    const short _posYDesc1 = _pageOriginY + (1 * _incInch);
    const short _posYDesc2 = _pageOriginY + (5 * _incInch);
    const short _posYData1 = _pageOriginY + (3 * _incInch);
    const short _posYData2 = _pageOriginY + (7 * _incInch);

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

        GenerateJobHeader(prnWriter,
                          indxPaperSize,
                          indxPaperType,
                          indxOrientation,
                          formAsMacro,
                          logXOffset);

        GeneratePage(prnWriter,
                     indxPaperSize,
                     indxPaperType,
                     indxOrientation,
                     formAsMacro,
                     logXOffset);

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
            GenerateOverlay(prnWriter, true, logXOffset, indxPaperSize, indxOrientation);

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
            PCLWriter.MacroControl(prnWriter, _macroId, PCLWriter.eMacroControl.StartDef);

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
                  "PCL & HP-GL/2 Text Rotation:");

        ptSize = 12;

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontCourier,
                                                  PCLFonts.eVariant.Regular,
                                                  ptSize, 0));

        posY = _posYDesc1;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Orthogonal:");

        posY = _posYDesc2;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Non-orthogonal:");

        //----------------------------------------------------------------//
        //                                                                //
        // Overlay end.                                                   //
        //                                                                //
        //----------------------------------------------------------------//

        PCLWriter.PatternSet(prnWriter,
                              PCLWriter.ePatternType.SolidBlack,
                              0);

        if (formAsMacro)
            PCLWriter.MacroControl(prnWriter, 0, PCLWriter.eMacroControl.StopDef);
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
        string lbCRTerm = "\x0d" + "~";

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
               sinAngle,
               cosAngle;

        //----------------------------------------------------------------//

        if (formAsMacro)
            PCLWriter.MacroControl(prnWriter, _macroId, PCLWriter.eMacroControl.Call);
        else
            GenerateOverlay(prnWriter, false, logXOffset, indxPaperSize, indxOrientation);

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
        boxWidth = (short)(_logPageWidth);
        boxHeight = (short)(_logPageHeight);

        PCLWriter.PictureFrame(prnWriter,
                                boxX,
                                boxY,
                                boxHeight,
                                boxWidth);

        PCLWriter.ModeHPGL2(prnWriter, false, false);

        PCLWriter.CmdHPGL2(prnWriter, "IN", string.Empty, false);
        PCLWriter.CmdHPGL2(prnWriter, "SP", "1", true);
        PCLWriter.CmdHPGL2(prnWriter, "DT", "~", false);

        PCLWriter.CmdHPGL2(prnWriter, "SC",
                            "0," + scaleX.ToString("F4") +
                            ",0," + (-scaleY).ToString("F4") +
                            ",2",
                            false);

        PCLWriter.CmdHPGL2(prnWriter, "IR", "0,100,100,0", false);
        PCLWriter.CmdHPGL2(prnWriter, "PU", "0,0", true);

        PCLWriter.ModePCL(prnWriter, true);

        //----------------------------------------------------------------//
        //                                                                //
        // Rotated text.                                                  //
        //                                                                //
        //----------------------------------------------------------------//

        ptSize = 36;

        PCLWriter.Font(prnWriter, true, "19U",
                       PCLFonts.GetPCLFontSelect(_indxFontArial,
                                                  PCLFonts.eVariant.Regular,
                                                  ptSize, 0));

        //----------------------------------------------------------------//
        // Orthogonal text                                                //
        //----------------------------------------------------------------//

        posX = (short)(_posXData1 - logXOffset);
        posY = _posYData1;

        posX += _lineInc / 4;

        PCLWriter.PrintDirection(prnWriter, 0);

        PCLWriter.TextRotated(prnWriter, posX, posY, 0, 0, true,
                               "angle 0");

        posX -= _lineInc / 2;

        PCLWriter.TextRotated(prnWriter, posX, posY, 0, 180, true,
                               "ccw 180");

        posX += _lineInc / 4;
        posY -= _lineInc / 4;

        PCLWriter.TextRotated(prnWriter, posX, posY, 0, 90, true,
                               "ccw 90");

        posY += _lineInc / 2;

        PCLWriter.TextRotated(prnWriter, posX, posY, 0, 270, true,
                               "ccw 270");

        //----------------------------------------------------------------//
        // direction quadrants I and III                                  //
        //----------------------------------------------------------------//

        posX = (short)(_posXData2 - logXOffset);
        posY = _posYData2;

        PCLWriter.ModeHPGL2(prnWriter, false, false);

        PCLWriter.CmdHPGL2(prnWriter, "SD",
                            PCLFonts.GetHPGL2FontDef(_indxFontArial,
                                   PCLFonts.eVariant.Regular,
                                   14, ptSize, 0),
                            true);

        PCLWriter.CmdHPGL2(prnWriter, "PA",
                            posX.ToString() + "," +
                            posY.ToString(), false);

        degrees = 30;
        angle = Math.PI * degrees / 180.0;
        sinAngle = Math.Sin(angle);
        cosAngle = Math.Cos(angle);

        PCLWriter.CmdHPGL2(prnWriter, "DI",
                            cosAngle.ToString() + "," +
                            sinAngle.ToString(), false);

        PCLWriter.CmdHPGL2(prnWriter, "LB",
                           "angle +30" + lbCRTerm, true);

        PCLWriter.CmdHPGL2(prnWriter, "DI",
                            "-" + cosAngle.ToString() + "," +
                            "-" + sinAngle.ToString(), false);

        PCLWriter.CmdHPGL2(prnWriter, "LB",
                           "angle +210" + lbCRTerm, true);

        //----------------------------------------------------------------//
        // direction quadrants II and IV                                  //
        //----------------------------------------------------------------//

        posX = (short)(_posXData2 - logXOffset - ((1 * _incInch) / 4));

        PCLWriter.ModeHPGL2(prnWriter, false, false);

        PCLWriter.CmdHPGL2(prnWriter, "SD",
                            PCLFonts.GetHPGL2FontDef(_indxFontArial,
                                   PCLFonts.eVariant.Regular,
                                   14, ptSize, 0),
                            true);

        PCLWriter.CmdHPGL2(prnWriter, "PA",
                            posX.ToString() + "," +
                            posY.ToString(), false);

        degrees = 45;
        angle = Math.PI * degrees / 180.0;
        sinAngle = Math.Sin(angle);
        cosAngle = Math.Cos(angle);

        PCLWriter.CmdHPGL2(prnWriter, "DI",
                            "-" + cosAngle.ToString() + "," +
                            sinAngle.ToString(), false);

        PCLWriter.CmdHPGL2(prnWriter, "LB",
                           "angle +135" + lbCRTerm, true);

        posX += ((1 * _incInch) / 2);
        posY += ((_lineInc * 1) / 6);

        PCLWriter.CmdHPGL2(prnWriter, "PA",
                            posX.ToString() + "," +
                            posY.ToString(), false);

        PCLWriter.CmdHPGL2(prnWriter, "DI",
                            cosAngle.ToString() + "," +
                            "-" + sinAngle.ToString(), false);

        PCLWriter.CmdHPGL2(prnWriter, "LB",
                           "angle +315" + lbCRTerm, true);

        //----------------------------------------------------------------//
        // resets                                                         //
        //----------------------------------------------------------------//

        posX = (short)(_posXData2 - logXOffset);
        posY += ((_lineInc * 1) / 2);

        PCLWriter.CmdHPGL2(prnWriter, "DI", "1,0", false);

        //----------------------------------------------------------------//

        PCLWriter.FormFeed(prnWriter);
    }
}
