using System.IO;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides PCL support for the Define Logical Page action
/// of the MiscSamples tool.
/// 
/// © Chris Hutchinson 2014
/// 
/// </summary>

static class ToolMiscSamplesActLogPagePCL
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    const string _hexChars = "0123456789ABCDEF";

    const int _macroId = 1;
    const ushort _unitsPerInch = PCLWriter.sessionUPI;

    const short _rulerDivPerCell = 10;
    const short _rulerVOriginX = (_unitsPerInch * 6);
    const short _rulerHOriginY = (_unitsPerInch * 5);
    const short _rulerCell = (_unitsPerInch * 1);
    const short _rulerDiv = (_rulerCell / _rulerDivPerCell);

    const short _posOrigin = _rulerCell;
    const short _posXDesc = _posOrigin + (4 * _rulerDiv);
    const short _posYHddr = _posOrigin - (4 * _rulerDiv);
    const short _posYDesc = _posOrigin + (4 * _rulerDiv);

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
                                   short logLeftOffset,
                                   short logTopOffset,
                                   ushort logPageWidth,
                                   ushort logPageHeight,
                                   bool formAsMacro,
                                   bool incStdPage)
    {
        const PCLOrientations.eAspect aspectPort
                = PCLOrientations.eAspect.Portrait;

        PCLOrientations.eAspect aspect;

        ushort paperWidth,
               paperLength,
               paperLengthPort,
               logXOffset;

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

        GenerateJobHeader(prnWriter,
                          indxPaperSize,
                          indxPaperType,
                          indxOrientation,
                          formAsMacro,
                          paperWidth,
                          paperLength,
                          logXOffset);

        GeneratePageSet(prnWriter,
                         indxPaperSize,
                         indxPaperType,
                         indxOrientation,
                         formAsMacro,
                         incStdPage,
                         paperWidth,
                         paperLength,
                         logXOffset,
                         logLeftOffset,
                         logTopOffset,
                         logPageWidth,
                         logPageHeight);

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
                                          ushort paperWidth,
                                          ushort paperLength,
                                          ushort logXOffset)
    {
        PCLWriter.StdJobHeader(prnWriter, string.Empty);

        if (formAsMacro)
            GenerateOverlay(prnWriter, true,
                            paperWidth, paperLength, logXOffset);

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
                                        ushort paperWidth,
                                        ushort paperLength,
                                        ushort logXOffset)
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

        rulerCellsX = (short)((paperWidth / _unitsPerInch) + 1);
        rulerCellsY = (short)((paperLength / _unitsPerInch) + 1);
        rulerWidth = (short)(rulerCellsX * _unitsPerInch);
        rulerHeight = (short)(rulerCellsY * _unitsPerInch);

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

        posX = 0;
        posY = _rulerHOriginY;

        PCLWriter.LineHorizontal(prnWriter, posX, posY, rulerWidth, stroke);

        posY -= (_rulerDiv / 2);

        for (int i = 0; i < rulerCellsX; i++)
        {
            PCLWriter.LineVertical(prnWriter, posX, posY,
                                   _rulerDiv * 2, stroke);

            posX += _rulerDiv;

            for (int j = 1; j < _rulerDivPerCell; j++)
            {
                PCLWriter.LineVertical(prnWriter, posX, posY,
                                       _rulerDiv, stroke);

                posX += _rulerDiv;
            }
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Vertical ruler.                                                //
        //                                                                //
        //----------------------------------------------------------------//

        posX = _rulerVOriginX;
        posY = 0;

        PCLWriter.LineVertical(prnWriter, posX, posY, rulerHeight, stroke);

        posX -= (_rulerDiv / 2);

        for (int i = 0; i < rulerCellsY; i++)
        {
            PCLWriter.LineHorizontal(prnWriter, posX, posY,
                                     _rulerDiv * 2, stroke);

            posY += _rulerDiv;

            for (int j = 1; j < _rulerDivPerCell; j++)
            {
                PCLWriter.LineHorizontal(prnWriter, posX, posY,
                                         _rulerDiv, stroke);

                posY += _rulerDiv;
            }
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Left logical page margin - vertical line.                      //
        //                                                                //
        //----------------------------------------------------------------//

        PCLWriter.LineVertical(prnWriter, 0, 0,
                               rulerHeight, stroke);

        //----------------------------------------------------------------//
        //                                                                //
        // Text.                                                          //
        //                                                                //
        //----------------------------------------------------------------//

        ptSize = 10;
        lineInc = (_rulerDiv * 2);

        PCLWriter.Font(prnWriter, true, "19U",
                  "s1p" + ptSize + "v0s0b16602T");

        posX = (short)(_posXDesc - logXOffset);
        posY = _posYDesc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Paper size:");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Orientation:");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Paper width:");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Paper length:");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Logical page left offset:");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Logical page top offset:");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Logical page width:");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "Logical page height:");

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
                                     bool formAsMacro,
                                     bool stdPage,
                                     ushort paperWidth,
                                     ushort paperLength,
                                     ushort logXOffset,
                                     short logLeftOffset,
                                     short logTopOffset,
                                     ushort logPageWidth,
                                     ushort logPageHeight)
    {
        const ushort dcptsPerInch = 720;

        const double unitsToInches = (1.00 / _unitsPerInch);
        const double unitsToMilliMetres = (25.4 / _unitsPerInch);

        const double dcptsToInches = (1.00 / dcptsPerInch);
        const double dcptsToMilliMetres = (25.4 / dcptsPerInch);

        short posX,
              posY;

        short lineInc,
              ptSize;

        //----------------------------------------------------------------//

        if (formAsMacro)
            PCLWriter.MacroControl(prnWriter, _macroId,
                                   PCLWriter.eMacroControl.Call);
        else
            GenerateOverlay(prnWriter, false,
                            paperWidth, paperLength, logXOffset);

        //----------------------------------------------------------------//
        //                                                                //
        // Header.                                                        //
        //                                                                //
        //----------------------------------------------------------------//

        ptSize = 15;

        PCLWriter.Font(prnWriter, true, "19U",
                  "s1p" + ptSize + "v0s0b16602T");

        posX = (short)(_posXDesc - logXOffset);
        posY = _posYHddr;

        if (stdPage)
        {
            PCLWriter.Text(prnWriter, posX, posY, 0,
                      "PCL Standard Logical Page sample");
        }
        else
        {
            PCLWriter.Text(prnWriter, posX, posY, 0,
                      "PCL Define Logical Page sample");
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Paper description data.                                        //
        //                                                                //
        //----------------------------------------------------------------//

        ptSize = 10;
        lineInc = _rulerDiv * 2;

        PCLWriter.Font(prnWriter, true, "19U",
                  "s0p" + (120 / ptSize) + "h0s3b4099T");

        posX = (short)((_posXDesc + (_rulerCell * 2)) - logXOffset);
        posY = _posYDesc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  PCLPaperSizes.GetName(indxPaperSize));

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  PCLOrientations.GetName(indxOrientation));

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  (paperWidth * unitsToMilliMetres).ToString("F0") +
                  "mm = " +
                  (paperWidth * unitsToInches).ToString("F2") +
                  "\"");

        posY += lineInc;

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  (paperLength * unitsToMilliMetres).ToString("F0") +
                  "mm = " +
                  (paperLength * unitsToInches).ToString("F2") +
                  "\"");

        if (stdPage)
        {
            posY += lineInc;

            PCLWriter.Text(prnWriter, posX, posY, 0,
                      "standard");

            posY += lineInc;

            PCLWriter.Text(prnWriter, posX, posY, 0,
                      "standard");

            posY += lineInc;

            PCLWriter.Text(prnWriter, posX, posY, 0,
                      "standard");

            posY += lineInc;

            PCLWriter.Text(prnWriter, posX, posY, 0,
                      "standard");

            PCLWriter.FormFeed(prnWriter);
        }
        else
        {
            posY += lineInc;

            PCLWriter.Text(prnWriter, posX, posY, 0,
                      logLeftOffset.ToString("F0") +
                      " decipoints = " +
                      (logLeftOffset * dcptsToMilliMetres).ToString("F0") +
                      "mm = " +
                      (logLeftOffset * dcptsToInches).ToString("F2") +
                      "\"");

            posY += lineInc;

            PCLWriter.Text(prnWriter, posX, posY, 0,
                      logTopOffset.ToString("F0") +
                      " decipoints = " +
                      (logTopOffset * dcptsToMilliMetres).ToString("F0") +
                      "mm = " +
                      (logTopOffset * dcptsToInches).ToString("F2") +
                      "\"");

            posY += lineInc;

            PCLWriter.Text(prnWriter, posX, posY, 0,
                      logPageWidth.ToString("F0") +
                      " decipoints = " +
                      (logPageWidth * dcptsToMilliMetres).ToString("F0") +
                      "mm = " +
                      (logPageWidth * dcptsToInches).ToString("F2") +
                      "\"");

            posY += lineInc;

            PCLWriter.Text(prnWriter, posX, posY, 0,
                      logPageHeight.ToString("F0") +
                      " decipoints = " +
                      (logPageHeight * dcptsToMilliMetres).ToString("F0") +
                      "mm = " +
                      (logPageHeight * dcptsToInches).ToString("F2") +
                      "\"");
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
                                         int indxPaperSize,
                                         int indxPaperType,
                                         int indxOrientation,
                                         bool formAsMacro,
                                         bool incStdPage,
                                         ushort paperWidth,
                                         ushort paperLength,
                                         ushort logXOffset,
                                         short logLeftOffset,
                                         short logTopOffset,
                                         ushort logPageWidth,
                                         ushort logPageHeight)
    {
        if (incStdPage)
        {
            GeneratePage(prnWriter,
                          indxPaperSize,
                          indxPaperType,
                          indxOrientation,
                          formAsMacro,
                          true,
                          paperWidth,
                          paperLength,
                          logXOffset,
                          logLeftOffset,
                          logTopOffset,
                          logPageWidth,
                          logPageHeight);
        }

        //----------------------------------------------------------------//

        PCLWriter.DefLogPage(prnWriter,
                              indxOrientation,
                              logLeftOffset,
                              logTopOffset,
                              logPageWidth,
                              logPageHeight);

        PCLWriter.MarginLeft(prnWriter, 0);

        PCLWriter.MarginTop(prnWriter, 0);

        //----------------------------------------------------------------//

        GeneratePage(prnWriter,
              indxPaperSize,
              indxPaperType,
              indxOrientation,
              formAsMacro,
              false,
              paperWidth,
              paperLength,
              logXOffset,
              logLeftOffset,
              logTopOffset,
              logPageWidth,
              logPageHeight);
    }
}
