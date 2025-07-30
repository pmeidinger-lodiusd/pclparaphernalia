using System.IO;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides PCL support for the TrayMap tool.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

static class ToolTrayMapPCL
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    const int _macroIdBaseFront = 1;
    const int _macroIdBaseRear = 11;
    const int _noForm = -1;

    const int _trayIdAutoSelectPCL = 7;

    const ushort _unitsPerInch = PCLWriter.sessionUPI;

    const short _posXName = (_unitsPerInch * 1);
    const short _posXValue = (_unitsPerInch * 7) / 2;
    const short _posXIncSub = (_unitsPerInch / 3);

    const short _posYHddr = (_unitsPerInch * 1);
    const short _posYDesc = (_unitsPerInch * 21) / 10;
    const short _posYIncMain = (_unitsPerInch * 3) / 4;
    const short _posYIncSub = (_unitsPerInch / 3);

    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static int _logPageOffset;

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e J o b                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Generate test data.                                                //
    //                                                                    //
    // Most sequences are built up as (Unicode) strings, then converted   //
    // to byte arrays before writing out - this works OK because all the  //
    // characters we're using are within the ASCII range (0x00-0x7f) and  //
    // are hence represented using a single byte in the UTF-8 encoding.   //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void GenerateJob(BinaryWriter prnWriter,
                                   int pageCount,
                                   int[] indxPaperSize,
                                   int[] indxPaperType,
                                   int[] indxPaperTray,
                                   int[] indxPlexMode,
                                   int[] indxOrientFront,
                                   int[] indxOrientRear,
                                   bool formAsMacro)
    {
        int[] indxFormsFront = new int[pageCount];
        int[] indxFormsRear = new int[pageCount];

        short[] macroIdsFront = new short[pageCount];
        short[] macroIdsRear = new short[pageCount];

        float[] scaleFactors = new float[pageCount];

        int formCountFront = 0;
        int formCountRear = 0;

        //----------------------------------------------------------------//
        //                                                                //
        // Set up the scaling data for each sheet, relative to the A4     //
        // paper size dimensions.                                         //
        //                                                                //
        //----------------------------------------------------------------//

        float A4LengthPort =
            PCLPaperSizes.GetPaperLength(
                        (int)PCLPaperSizes.eIndex.ISO_A4,
                        _unitsPerInch,
                        PCLOrientations.eAspect.Portrait);

        for (int i = 0; i < pageCount; i++)
        {
            scaleFactors[i] =
            PCLPaperSizes.GetPaperLength(
                indxPaperSize[i],
                _unitsPerInch,
                PCLOrientations.eAspect.Portrait) /
             A4LengthPort;
        }

        //----------------------------------------------------------------//
        //                                                                //
        // We'll also set the logical page offset value to be used on all //
        // pages (and all overlays) from the value for the front of the   //
        // first sheet.                                                   //
        // This may be inaccurate for subsequent sheets (but only if they //
        // use different page sizes and/or orientations), but the error   //
        // will be minimal (at most, about 30 'dots', or 0.05 inch).      //
        //                                                                //
        //----------------------------------------------------------------//

        _logPageOffset = PCLPaperSizes.GetLogicalOffset(
            indxPaperSize[0],
            _unitsPerInch,
            PCLOrientations.GetAspect(indxOrientFront[0]));

        //----------------------------------------------------------------//
        //                                                                //
        // Generate the print job.                                        //
        //                                                                //
        //----------------------------------------------------------------//

        GenerateJobHeader(prnWriter);

        if (formAsMacro)
            GenerateOverlaySet(prnWriter,
                               pageCount,
                               indxPaperSize,
                               indxPlexMode,
                               scaleFactors,
                               ref formCountFront,
                               ref formCountRear,
                               ref indxFormsFront,
                               ref indxFormsRear,
                               ref macroIdsFront,
                               ref macroIdsRear);

        GeneratePageSet(prnWriter,
                        pageCount,
                        indxPaperSize,
                        indxPaperType,
                        indxPaperTray,
                        indxPlexMode,
                        indxOrientFront,
                        indxOrientRear,
                        indxFormsFront,
                        indxFormsRear,
                        macroIdsFront,
                        macroIdsRear,
                        scaleFactors,
                        formAsMacro);

        if (formAsMacro)
            GenerateOverlayDeletes(prnWriter,
                                    formCountFront, formCountRear,
                                    macroIdsFront, macroIdsRear);

        GenerateJobTrailer(prnWriter);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e J o b H e a d e r                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write stream initialisation sequences to output file.              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateJobHeader(BinaryWriter prnWriter)
    {
        PCLWriter.StdJobHeader(prnWriter, string.Empty);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e J o b T r a i l e r                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write termination sequences to output file.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateJobTrailer(BinaryWriter prnWriter)
    {
        PCLWriter.StdJobTrailer(prnWriter, false, 0);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e O v e r l a y D e l e t e s                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Delete overlays.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateOverlayDeletes(BinaryWriter prnWriter,
                                                int formCountFront,
                                                int formCountRear,
                                                short[] macroIdsFront,
                                                short[] macroIdsRear)
    {
        for (int i = 0; i < formCountFront; i++)
        {
            PCLWriter.MacroControl(prnWriter, macroIdsFront[i],
                                    PCLWriter.eMacroControl.Delete);
        }

        for (int i = 0; i < formCountRear; i++)
        {
            PCLWriter.MacroControl(prnWriter, macroIdsRear[i],
                                    PCLWriter.eMacroControl.Delete);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e O v e r l a y F r o n t                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write background data sequences for front overlay to output file.  //
    // Optionally top and tail these with macro definition sequences.     //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateOverlayFront(BinaryWriter prnWriter,
                                              bool formAsMacro,
                                              short macroId,
                                              float scaleFactor)
    {
        short rectHeight = (short)(scaleFactor * (_unitsPerInch / 2));
        short rectWidth = (short)(scaleFactor * ((_unitsPerInch * 7) / 2));
        short rectStroke = (short)(scaleFactor * (_unitsPerInch / 200));

        int ptSizeHddr = (int)(scaleFactor * 24),
              ptSizeMain = (int)(scaleFactor * 18),
              ptSizeSub = (int)(scaleFactor * 8);

        if (formAsMacro)
            PCLWriter.MacroControl(prnWriter, macroId,
                                   PCLWriter.eMacroControl.StartDef);

        //----------------------------------------------------------------//

        short posYInc = (short)(scaleFactor * _posYIncMain);
        short posX = (short)((scaleFactor * _posXName) - _logPageOffset);
        short posY = (short)(scaleFactor * _posYHddr);

        PCLWriter.Font(prnWriter, true,
                        "19U", "s1p" + ptSizeHddr + "v0s3b16602T");

        PCLWriter.Text(prnWriter, posX, posY, 0, "Tray map test (PCL)");

        //----------------------------------------------------------------//

        posY = (short)(scaleFactor * _posYDesc);

        PCLWriter.Font(prnWriter, true, string.Empty, "s" + ptSizeMain + "V");

        PCLWriter.Text(prnWriter, posX, posY, 0, "Page Number:");

        posY += posYInc;
        PCLWriter.Text(prnWriter, posX, posY, 0, "Paper Size:");

        posY += posYInc;
        PCLWriter.Text(prnWriter, posX, posY, 0, "Paper Type:");

        posY += posYInc;
        PCLWriter.Text(prnWriter, posX, posY, 0, "Plex Mode:");

        posY += posYInc;
        PCLWriter.Text(prnWriter, posX, posY, 0, "Orientation: ");

        posY += posYInc;
        PCLWriter.Text(prnWriter, posX, posY, 0, "PCL Tray ID:");

        posY += posYInc;
        PCLWriter.Text(prnWriter, posX, posY, 0, "Printer Tray:");

        //----------------------------------------------------------------//

        posX = (short)((scaleFactor * (_posXValue + _posXIncSub)) -
                         _logPageOffset);
        posY += (short)(scaleFactor * _posYIncSub);

        PCLWriter.Font(prnWriter, true,
                       "19U", "s1p" + ptSizeSub + "v0s3b16602T");

        PCLWriter.Text(prnWriter, posX, posY, 0,
                  "record the tray name/number used in this box");

        //----------------------------------------------------------------//

        posX = (short)(((scaleFactor * _posXValue) - _logPageOffset));
        posY -= (short)(scaleFactor * (_posXIncSub * 2));

        PCLWriter.RectangleOutline(prnWriter, posX, posY,
                                   rectHeight, rectWidth, rectStroke,
                                   false, false);

        //----------------------------------------------------------------//

        if (formAsMacro)
            PCLWriter.MacroControl(prnWriter, macroId,
                                   PCLWriter.eMacroControl.StopDef);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e O v e r l a y R e a r                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write background data sequences fopr rear overlay to output file.  //
    // Optionally top and tail these with macro definition sequences.     //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateOverlayRear(BinaryWriter prnWriter,
                                              bool formAsMacro,
                                              short macroId,
                                              float scaleFactor)
    {
        int ptSizeHddr = (int)(scaleFactor * 24),
              ptSizeMain = (int)(scaleFactor * 18);

        if (formAsMacro)
            PCLWriter.MacroControl(prnWriter, macroId,
                                   PCLWriter.eMacroControl.StartDef);

        //----------------------------------------------------------------//

        short posYInc = (short)(scaleFactor * _posYIncMain);
        short posX = (short)((scaleFactor * _posXName) - _logPageOffset);
        short posY = (short)(scaleFactor * _posYHddr);

        PCLWriter.Font(prnWriter, true,
                        "19U", "s1p" + ptSizeHddr + "v0s3b16602T");

        PCLWriter.Text(prnWriter, posX, posY, 0, "Tray map test (PCL)");

        //----------------------------------------------------------------//

        posY = (short)(scaleFactor * _posYDesc);

        PCLWriter.Font(prnWriter, true, string.Empty, "s" + ptSizeMain + "V");

        PCLWriter.Text(prnWriter, posX, posY, 0, "Page Number:");

        posY += (short)(posYInc * 4);

        PCLWriter.Text(prnWriter, posX, posY, 0, "Orientation: ");

        //----------------------------------------------------------------//

        if (formAsMacro)
            PCLWriter.MacroControl(prnWriter, macroId,
                                   PCLWriter.eMacroControl.StopDef);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e O v e r l a y S e t                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Because each sheet may be a different size, the information to be  //
    // printed may need to be scaled to fit the individual sheets, and    //
    // separate (scaled) macros may also be required.                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void GenerateOverlaySet(BinaryWriter prnWriter,
                                           int pageCount,
                                           int[] indxPaperSize,
                                           int[] indxPlexMode,
                                           float[] scaleFactors,
                                           ref int formCountFront,
                                           ref int formCountRear,
                                           ref int[] indxFormsFront,
                                           ref int[] indxFormsRear,
                                           ref short[] macroIdsFront,
                                           ref short[] macroIdsRear)
    {
        const int noForm = -1;

        short crntFormFront,
              crntFormRear;

        bool[] duplexSheet = new bool[pageCount];

        //----------------------------------------------------------------//
        //                                                                //
        // Which sheets are duplex?.                                      //
        //                                                                //
        //----------------------------------------------------------------//

        for (int i = 0; i < pageCount; i++)
        {
            if (PCLPlexModes.IsSimplex(indxPlexMode[i]))
                duplexSheet[i] = false;
            else
                duplexSheet[i] = true;
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Establish the forms required for the front side of the sheets. //
        // A different one is required for each paper size.               //
        //                                                                //
        //----------------------------------------------------------------//
        //----------------------------------------------------------------//
        // First sheet.                                                   //
        //----------------------------------------------------------------//

        crntFormFront = 0;

        macroIdsFront[crntFormFront] =
            (short)(_macroIdBaseFront + crntFormFront);

        GenerateOverlayFront(prnWriter, true,
                             macroIdsFront[crntFormFront],
                              scaleFactors[0]);

        indxFormsFront[0] = crntFormFront++;

        //----------------------------------------------------------------//
        // Subsequent sheets.                                             //
        //----------------------------------------------------------------//

        for (int i = 1; i < pageCount; i++)
        {
            bool matchFound = false;

            for (int j = 0; j < i; j++)
            {
                if (indxPaperSize[i] == indxPaperSize[j])
                {
                    //----------------------------------------------------//
                    // Same paper size as a previous sheet.               //
                    //----------------------------------------------------//

                    matchFound = true;

                    indxFormsFront[i] = indxFormsFront[j];

                    j = i; // force end loop //
                }
            }

            if (!matchFound)
            {
                //----------------------------------------------------//
                // New paper size.                                    //
                //----------------------------------------------------//

                macroIdsFront[crntFormFront] =
                    (short)(_macroIdBaseFront + crntFormFront);

                GenerateOverlayFront(prnWriter, true,
                                      macroIdsFront[crntFormFront],
                                      scaleFactors[i]);

                indxFormsFront[i] = crntFormFront++;
            }
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Establish the forms required for the rear side of the sheets.  //
        // A different one is required for each paper size.               //
        //                                                                //
        //----------------------------------------------------------------//
        //----------------------------------------------------------------//
        // First sheet.                                                   //
        //----------------------------------------------------------------//

        crntFormRear = 0;

        if (duplexSheet[0])
        {
            macroIdsRear[crntFormRear] =
                (short)(_macroIdBaseRear + crntFormRear);

            GenerateOverlayRear(prnWriter, true,
                                 macroIdsRear[crntFormRear],
                                 scaleFactors[0]);

            indxFormsRear[0] = crntFormRear++;
        }
        else
        {
            indxFormsRear[0] = noForm;
        }

        //----------------------------------------------------------------//
        // Subsequent sheets.                                             //
        //----------------------------------------------------------------//

        for (int i = 1; i < pageCount; i++)
        {
            if (!duplexSheet[i])
            {
                indxFormsRear[i] = noForm;
            }
            else
            {
                bool matchFound = false;

                for (int j = 0; j < i; j++)
                {
                    if (indxPaperSize[i] == indxPaperSize[j] &&
                        duplexSheet[j])
                    {
                        //------------------------------------------------//
                        // Same paper size as a previous duplex sheet.    //
                        //------------------------------------------------//

                        matchFound = true;

                        indxFormsRear[i] = indxFormsRear[j];

                        j = i; // force end loop //
                    }
                }

                //----------------------------------------------------//
                // New paper size.                                    //
                //----------------------------------------------------//

                if (!matchFound)
                {
                    macroIdsRear[crntFormRear] =
                        (short)(_macroIdBaseRear + crntFormRear);

                    GenerateOverlayRear(prnWriter, true,
                                         macroIdsRear[crntFormRear],
                                         scaleFactors[i]);

                    indxFormsRear[i] = crntFormRear++;
                }
            }
        }

        formCountFront = crntFormFront;
        formCountRear = crntFormRear;
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
                                     int pageNo,
                                     int pageCount,
                                     int indxPaperSize,
                                     int indxPaperType,
                                     int indxPaperTray,
                                     int indxPlexMode,
                                     int indxOrientFront,
                                     int indxOrientRear,
                                     short macroIdFront,
                                     short macroIdRear,
                                     float scaleFactor,
                                     bool formAsMacro)
    {
        int pitchMain = (int)(6 / scaleFactor);

        bool simplex = PCLPlexModes.IsSimplex(indxPlexMode);

        PCLWriter.PageHeader(prnWriter,
                             indxPaperSize,
                             indxPaperType,
                             indxOrientFront,
                             indxPlexMode);

        if (indxPaperTray != -1)
            PCLWriter.PaperSource(prnWriter, (short)indxPaperTray);

        if (!simplex)
        {
            PCLWriter.PageFace(prnWriter, true);
        }

        if (formAsMacro)
            PCLWriter.MacroControl(prnWriter, macroIdFront, PCLWriter.eMacroControl.Call);
        else
            GenerateOverlayFront(prnWriter, false, _noForm, scaleFactor);

        //----------------------------------------------------------------//

        short posYInc = (short)(scaleFactor * _posYIncMain);
        short posX = (short)((scaleFactor * _posXValue) - _logPageOffset);
        short posY = (short)((scaleFactor * _posYDesc));

        PCLWriter.Font(prnWriter, true,
                        "19U", "s0p" + pitchMain + "h0s3b4099T");

        // TODO: Why are these the same? -- PMM
        if (simplex)
        {
            PCLWriter.Text(prnWriter, posX, posY, 0, pageNo.ToString() + " of " + pageCount.ToString());
        }
        else
        {
            PCLWriter.Text(prnWriter, posX, posY, 0, pageNo.ToString() + " of " + pageCount.ToString());
        }

        //----------------------------------------------------------------//

        posY += posYInc;

        if (indxPaperSize >= PCLPaperSizes.GetCount())
            PCLWriter.Text(prnWriter, posX, posY, 0, "*** unknown ***");
        else
            PCLWriter.Text(prnWriter, posX, posY, 0, PCLPaperSizes.GetName(indxPaperSize));

        //----------------------------------------------------------------//

        posY += posYInc;

        if (indxPaperType >= PCLPaperTypes.GetCount())
            PCLWriter.Text(prnWriter, posX, posY, 0, "*** unknown ***");
        else if (PCLPaperTypes.GetType(indxPaperType) == PCLPaperTypes.eEntryType.NotSet)
            PCLWriter.Text(prnWriter, posX, posY, 0, "<not set>");
        else
            PCLWriter.Text(prnWriter, posX, posY, 0, PCLPaperTypes.GetName(indxPaperType));

        //----------------------------------------------------------------//

        posY += posYInc;

        if (indxPlexMode >= PCLPlexModes.GetCount())
            PCLWriter.Text(prnWriter, posX, posY, 0, "*** unknown ***");
        else
            PCLWriter.Text(prnWriter, posX, posY, 0, PCLPlexModes.GetName(indxPlexMode));

        //----------------------------------------------------------------//

        posY += posYInc;

        if (indxOrientFront >= PCLOrientations.GetCount())
            PCLWriter.Text(prnWriter, posX, posY, 0, "*** unknown ***");
        else
            PCLWriter.Text(prnWriter, posX, posY, 0, PCLOrientations.GetName(indxOrientFront));

        //----------------------------------------------------------------//

        posY += posYInc;

        if (indxPaperTray == PCLTrayDatas.GetIdNotSetPCL())
            PCLWriter.Text(prnWriter, posX, posY, 0, "<not set>");
        else if (indxPaperTray == _trayIdAutoSelectPCL)
            PCLWriter.Text(prnWriter, posX, posY, 0, indxPaperTray.ToString() + " (auto-select)");
        else
            PCLWriter.Text(prnWriter, posX, posY, 0, indxPaperTray.ToString());

        //----------------------------------------------------------------//
        //                                                                // 
        // Rear face (if not simplex)                                     // 
        //                                                                // 
        //----------------------------------------------------------------//

        if (!simplex)
        {
            if (indxOrientRear != indxOrientFront)
            {
                PCLWriter.PageOrientation(
                    prnWriter,
                    PCLOrientations.GetIdPCL(indxOrientRear).ToString());
            }

            PCLWriter.PageFace(prnWriter, false);

            if (formAsMacro)
                PCLWriter.MacroControl(prnWriter, macroIdRear, PCLWriter.eMacroControl.Call);
            else
                GenerateOverlayRear(prnWriter, false, _noForm, scaleFactor);

            //----------------------------------------------------------------//

            posX = (short)((scaleFactor * _posXValue) - _logPageOffset);
            posY = (short)(scaleFactor * _posYDesc);

            PCLWriter.Font(prnWriter, true,
                            "19U", "s0p" + pitchMain + "h0s3b4099T");

            PCLWriter.Text(prnWriter, posX, posY, 0, pageNo.ToString() +
                                                " (rear) of " +
                                                pageCount.ToString());

            //----------------------------------------------------------------//

            posY += (short)(posYInc * 4);

            if (indxOrientRear >= PCLOrientations.GetCount())
                PCLWriter.Text(prnWriter, posX, posY, 0, "*** unknown ***");
            else
                PCLWriter.Text(prnWriter, posX, posY, 0, PCLOrientations.GetName(indxOrientRear));
        }

        PCLWriter.FormFeed(prnWriter);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e P a g e S e t                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write set of test data pages to output file.                       //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GeneratePageSet(BinaryWriter prnWriter,
                                         int pageCount,
                                         int[] indxPaperSize,
                                         int[] indxPaperType,
                                         int[] indxPaperTray,
                                         int[] indxPlexMode,
                                         int[] indxOrientFront,
                                         int[] indxOrientRear,
                                         int[] indxFormsFront,
                                         int[] indxFormsRear,
                                         short[] macroIdsFront,
                                         short[] macroIdsRear,
                                         float[] scaleFactors,
                                         bool formAsMacro)
    {
        for (int i = 0; i < pageCount; i++)
        {
            short macroIdFront;
            short macroIdRear;

            int index;

            if (formAsMacro)
            {
                index = indxFormsFront[i];

                macroIdFront = macroIdsFront[index];

                index = indxFormsRear[i];

                if (index == _noForm)
                    macroIdRear = _noForm;
                else
                    macroIdRear = macroIdsRear[index];
            }
            else
            {
                macroIdFront = _noForm;
                macroIdRear = _noForm;
            }

            GeneratePage(prnWriter,
                          i + 1,
                          pageCount,
                          indxPaperSize[i],
                          indxPaperType[i],
                          indxPaperTray[i],
                          indxPlexMode[i],
                          indxOrientFront[i],
                          indxOrientRear[i],
                          macroIdFront,
                          macroIdRear,
                          scaleFactors[i],
                          formAsMacro);
        }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // T r a y I d A u t o S e l e c t                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return the 'auto-select' tray identifier.                         //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static int TrayIdAutoSelect
    {
        get
        {
            return _trayIdAutoSelectPCL;
        }
    }
}
