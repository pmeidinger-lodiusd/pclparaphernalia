using System;
using System.IO;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides PCL support for the PrintArea tool.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

static class ToolPrintAreaPCL
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    const string _hexChars = "0123456789ABCDEF";

    const int _macroId = 1;
    const ushort _sessionUPI = PCLWriter.sessionUPI;

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
                                   bool formAsMacro)
    {
        PCLOrientations.eAspect aspect;

        ushort paperWidth,
               paperLength,
               logXOffset;

        ushort A4Length,
               A4Width;

        float scaleText,
               scaleTextLength,
               scaleTextWidth;

        bool customPaperSize;

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

        if (PCLPaperSizes.IsCustomSize(indxPaperSize))
            customPaperSize = true;
        //      else if (PCLPaperSizes.getIdPCL(indxPaperSize) == 0xff)
        //          customPaperSize = true;
        else
            customPaperSize = false;

        paperLength = PCLPaperSizes.GetPaperLength(indxPaperSize,
                                                   _sessionUPI, aspect);

        paperWidth = PCLPaperSizes.GetPaperWidth(indxPaperSize,
                                                 _sessionUPI, aspect);

        logXOffset = PCLPaperSizes.GetLogicalOffset(indxPaperSize,
                                                    _sessionUPI, aspect);

        scaleTextLength = (float)paperLength / A4Length;
        scaleTextWidth = (float)paperWidth / A4Width;

        if (scaleTextLength < scaleTextWidth)
            scaleText = scaleTextLength;
        else
            scaleText = scaleTextWidth;

        //----------------------------------------------------------------//

        GenerateJobHeader(prnWriter,
                          indxPaperSize,
                          indxPaperType,
                          indxOrientation,
                          indxPlexMode,
                          pjlCommand,
                          formAsMacro,
                          customPaperSize,
                          paperWidth,
                          paperLength,
                          logXOffset,
                          scaleText);

        GeneratePage(prnWriter,
                     indxPaperSize,
                     indxPaperType,
                     indxOrientation,
                     indxPlexMode,
                     pjlCommand,
                     formAsMacro,
                     customPaperSize,
                     false,
                     paperWidth,
                     paperLength,
                     logXOffset,
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
                         customPaperSize,
                         true,
                         paperWidth,
                         paperLength,
                         logXOffset,
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
                                          int indxPaperSize,
                                          int indxPaperType,
                                          int indxOrientation,
                                          int indxPlexMode,
                                          string pjlCommand,
                                          bool formAsMacro,
                                          bool customPaperSize,
                                          ushort paperWidth,
                                          ushort paperLength,
                                          ushort logXOffset,
                                          float scaleText)
    {
        PCLWriter.StdJobHeader(prnWriter, pjlCommand);

        if (formAsMacro)
            GenerateOverlay(prnWriter, true,
                            paperWidth, paperLength, logXOffset, scaleText);

        if (customPaperSize)
            PCLWriter.PageHeaderCustom(prnWriter,
                                        indxPaperType,
                                        indxOrientation,
                                        indxPlexMode,
                                        paperWidth,
                                        paperLength);
        else
            PCLWriter.PageHeader(prnWriter,
                                  indxPaperSize,
                                  indxPaperType,
                                  indxOrientation,
                                  indxPlexMode);
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
                                        ushort paperWidth,
                                        ushort paperLength,
                                        ushort logXOffset,
                                        float scaleText)
    {
        short rulerWidth;
        short rulerHeight;

        short rulerCellsX;
        short rulerCellsY;

        short posX,
              posY;

        short lineInc,
              ptSize;

        short stroke = 1;

        //----------------------------------------------------------------//

        rulerCellsX = (short)((paperWidth / _sessionUPI) - 1);
        rulerCellsY = (short)((paperLength / _sessionUPI) - 1);
        rulerWidth = (short)(rulerCellsX * _sessionUPI);
        rulerHeight = (short)(rulerCellsY * _sessionUPI);

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
        // Horizontal ruler.                                              //
        //                                                                //
        //----------------------------------------------------------------//

        posX = (short)(_rulerOriginX - logXOffset);
        posY = _rulerOriginY;

        PCLWriter.LineHorizontal(prnWriter, posX, posY, rulerWidth, stroke);

        posX += _rulerCell;

        for (int i = 0; i < rulerCellsX; i++)
        {
            PCLWriter.LineVertical(prnWriter, posX, posY, _rulerDiv, stroke);

            posX += _rulerCell;
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Vertical ruler.                                                //
        //                                                                //
        //----------------------------------------------------------------//

        posX = (short)(_rulerOriginX - logXOffset);
        posY = _rulerOriginY;

        PCLWriter.LineVertical(prnWriter, posX, posY, rulerHeight, stroke);

        posY += _rulerCell;

        for (int i = 0; i < rulerCellsY; i++)
        {
            PCLWriter.LineHorizontal(prnWriter, posX, posY,
                                     _rulerDiv, stroke);

            posY += _rulerCell;
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Left logical page margin - vertical line.                      //
        //                                                                //
        //----------------------------------------------------------------//

        PCLWriter.LineVertical(prnWriter, 0, _rulerOriginY,
                               rulerHeight, stroke);

        //----------------------------------------------------------------//
        //                                                                //
        // Sample marker box.                                             //
        //                                                                //
        //----------------------------------------------------------------//

        lineInc = (short)((_sessionUPI * scaleText) / 8);

        posX = (short)((_rulerCell * 5.5 * scaleText) - logXOffset);
        posY = (short)(_posYDesc - lineInc);

        GenerateSquare(prnWriter, posX, posY, true);

        //----------------------------------------------------------------//
        //                                                                //
        // Text.                                                          //
        //                                                                //
        //----------------------------------------------------------------//

        ptSize = (short)(15 * scaleText);

        PCLWriter.Font(prnWriter, true, "19U",
                  "s1p" + ptSize + "v0s0b16602T");

        posX = (short)(_posXHddr - logXOffset);
        posY = _posYHddr;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "PCL print area sample");

        ptSize = (short)(10 * scaleText);

        PCLWriter.Font(prnWriter, true, "19U",
                  "s1p" + ptSize + "v0s0b16602T");

        posX = (short)(_posXDesc - logXOffset);
        posY = _posYDesc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Paper size:");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Paper type:");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Orientation:");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Plex mode:");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Paper width:");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Paper length:");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "PJL option:");

        //----------------------------------------------------------------//

        posY = (short)(_posYDesc + (_rulerCell * scaleText));

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Black squares of side 3 units, each containing a" +
                  " central white square of side one");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "unit, and some directional markers, as per the" +
                  " half-size sample above,");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "demonstrate how objects are clipped by the" +
                  " boundaries of the printable area.");

        posY += lineInc;
        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "The four corner squares are (theoretically) positioned" +
                  " in the corners of the");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "physical sheet, except that the left edges of the top" +
                  " and bottom left-hand squares");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "are constrained to be positioned at the left margin" +
                  " of the PCL logical page,");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "which is inset from the sheet edge, and marked here" +
                  " with a vertical line.");

        posY += lineInc;
        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "The middle left-hand square is positioned relative" +
                  " to the bottom and right logical");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "page margins, and rotated 180 degrees.");

        posY += lineInc;
        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Fixed pitch (10 cpi) text characters are also clipped" +
                  " by the boundaries of the");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "printable area; one set is shown relative to the" +
                  " left logical page margin, and");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "another set (rotated 180 degrees) is shown" +
                  " relative to the right margin.");

        posY += lineInc;
        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "PJL options may move the logical page and/or" +
                  " unprintable area margins relative");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "to the physical sheet.");

        //----------------------------------------------------------------//
        //                                                                //
        // Overlay end.                                                   //
        //                                                                //
        //----------------------------------------------------------------//

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
                                     int indxPlexMode,
                                     string pjlCommand,
                                     bool formAsMacro,
                                     bool customPaperSize,
                                     bool rearFace,
                                     ushort paperWidth,
                                     ushort paperLength,
                                     ushort logXOffset,
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

        const short bottomMargin = (short)(0.5 * _sessionUPI);

        short squareRightX,
              squareBottomY;

        short posX,
              posY;

        short lineInc,
              ptSize;

        int ctA;

        //----------------------------------------------------------------//

        if (formAsMacro)
            PCLWriter.MacroControl(prnWriter, _macroId, PCLWriter.eMacroControl.Call);
        else
            GenerateOverlay(prnWriter, false,
                            paperWidth, paperLength, logXOffset, scaleText);

        //----------------------------------------------------------------//
        //                                                                //
        // Corner squares.                                                //
        //                                                                //
        //----------------------------------------------------------------//

        squareRightX = (short)(paperWidth - _boxOuterEdge - logXOffset);
        squareBottomY = (short)(paperLength - _boxOuterEdge);

        // Top-left.                                                      //

        posX = 0;
        posY = 0;

        GenerateSquare(prnWriter, posX, posY, false);

        // Top-right.                                                     //

        posX = squareRightX;
        posY = 0;

        GenerateSquare(prnWriter, posX, posY, false);

        // Bottom-left.                                                   //

        posX = 0;
        posY = squareBottomY;

        GenerateSquare(prnWriter, posX, posY, false);

        // Bottom-right.                                                  //

        posX = squareRightX;
        posY = squareBottomY;

        GenerateSquare(prnWriter, posX, posY, false);

        //----------------------------------------------------------------//
        //                                                                //
        // Paper description data.                                        //
        //                                                                //
        //----------------------------------------------------------------//

        ptSize = (short)(10 * scaleText);
        lineInc = (short)((_sessionUPI * scaleText) / 8);

        PCLWriter.Font(prnWriter, true, "19U",
                  "s0p" + (120 / ptSize) + "h0s3b4099T");

        posX = (short)((_posXDesc + (_rulerCell * scaleText)) - logXOffset);
        posY = _posYDesc;

        if (customPaperSize)
            PCLWriter.Text(prnWriter, posX, posY, 0,
                      PCLPaperSizes.GetNameAndDesc(indxPaperSize));
        else
            PCLWriter.Text(prnWriter, posX, posY, 0,
                      PCLPaperSizes.GetName(indxPaperSize));

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  PCLPaperTypes.GetName(indxPaperType));

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  PCLOrientations.GetName(indxOrientation));

        posY += lineInc;

        if (rearFace)
            PCLWriter.Text(prnWriter, posX, posY, 0,
                      PCLPlexModes.GetName(indxPlexMode) +
                        ": rear face");
        else
            PCLWriter.Text(prnWriter, posX, posY, 0,
                      PCLPlexModes.GetName(indxPlexMode));

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  (Math.Round((paperWidth *
                               unitsToMilliMetres), 2)).ToString("F1") +
                  " mm = " +
                  (Math.Round((paperWidth *
                               unitsToInches), 3)).ToString("F3") +
                  "\"");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  (Math.Round((paperLength *
                               unitsToMilliMetres), 2)).ToString("F1") +
                  " mm = " +
                  (Math.Round((paperLength *
                               unitsToInches), 3)).ToString("F3") +
                  "\"");

        posY += lineInc;

        if (pjlCommand == "")
            PCLWriter.Text(prnWriter, posX, posY, 0, "<none>");
        else
            PCLWriter.Text(prnWriter, posX, posY, 0, pjlCommand);

        //----------------------------------------------------------------//
        //                                                                //
        // Fixed-pitch 10cpi text - not rotated.                          //
        //                                                                //
        //----------------------------------------------------------------//

        PCLWriter.Font(prnWriter, true, "19U", "s0p10h0s0b4099T");

        posY = _posYText;

        ctA = (paperWidth * 10) / _sessionUPI;

        PCLWriter.Text(prnWriter, 0, posY, 0, digitsTextA.Substring(0, ctA));

        posY += _rulerDiv;

        PCLWriter.Text(prnWriter, 0, posY, 0, digitsTextB.Substring(0, ctA));

        //----------------------------------------------------------------//
        //                                                                //
        // Rotate print direction by 180-degrees.                         //
        //                                                                //
        //----------------------------------------------------------------//

        PCLWriter.PrintDirection(prnWriter, 180);

        //----------------------------------------------------------------//
        //                                                                //
        // Fixed-pitch 10cpi text - 180-degree rotated.                   //
        //                                                                //
        //----------------------------------------------------------------//

        posY = (short)(paperLength - _posYText - (2 * bottomMargin));

        ctA = (paperWidth * 10) / _sessionUPI;

        PCLWriter.Text(prnWriter, 0, posY, 0, digitsTextA.Substring(0, ctA));

        posY += _rulerDiv;

        PCLWriter.Text(prnWriter, 0, posY, 0, digitsTextB.Substring(0, ctA));

        //----------------------------------------------------------------//
        //                                                                //
        // Left box: rotated (180-degree) orientation.                    //
        //                                                                //
        //----------------------------------------------------------------//

        posX = squareRightX;
        posY = (short)(((paperLength - _boxOuterEdge) / 2) - bottomMargin);

        GenerateSquare(prnWriter, posX, posY, false);

        //----------------------------------------------------------------//
        //                                                                //
        // Reset print direction to 0-degrees.                            //
        //                                                                //
        //----------------------------------------------------------------//

        PCLWriter.PrintDirection(prnWriter, 0);

        //----------------------------------------------------------------//

        PCLWriter.FormFeed(prnWriter);
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
                                       bool halfSize)
    {
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
        //                                                                //
        // Outer square (black).                                          //
        //                                                                //
        //----------------------------------------------------------------//

        short posX = startX;
        short posY = startY;

        PCLWriter.RectangleSolid(prnWriter, posX, posY,
                                 boxOuterEdge, boxOuterEdge,
                                 false, false, false);

        //----------------------------------------------------------------//
        //                                                                //
        // Inner square (white).                                          //
        //                                                                //
        //----------------------------------------------------------------//

        posX += boxInnerOffset;
        posY += boxInnerOffset;

        PCLWriter.RectangleSolid(prnWriter, posX, posY,
                                 boxInnerEdge, boxInnerEdge,
                                 true, false, false);

        //----------------------------------------------------------------//
        //                                                                //
        // Top marker rectangle (white).                                  //
        //                                                                //
        //----------------------------------------------------------------//

        posX = (short)(startX + boxMarkerOffset);
        posY = startY;

        PCLWriter.RectangleSolid(prnWriter, posX, posY,
                                 boxInnerOffset,
                                 boxMarkerEdge,
                                 true, false, false);

        //----------------------------------------------------------------//
        //                                                                //
        // Left marker rectangle (white).                                 //
        //                                                                //
        //----------------------------------------------------------------//

        posX = startX;
        posY = (short)(startY + boxMarkerOffset);

        PCLWriter.RectangleSolid(prnWriter, posX, posY,
                                 boxMarkerEdge,
                                 boxInnerOffset,
                                 true, false, false);
    }
}
