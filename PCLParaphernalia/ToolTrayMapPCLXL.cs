using System.IO;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides PCL XL support for the TrayMap tool.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class ToolTrayMapPCLXL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const string _rootNameFront = "TrayMapFormFront";
        const string _rootNameRear = "TrayMapFormRear";
        const int _noForm = -1;

        const int _trayIdAutoSelectPCLXL = 1;

        const ushort _unitsPerInch = PCLXLWriter._sessionUPI;

        const short _posXName = (_unitsPerInch * 1);
        const short _posXValue = (_unitsPerInch * 7) / 2;
        const short _posXIncSub = (_unitsPerInch / 3);

        const short _posYHddr = (_unitsPerInch * 1);
        const short _posYDesc = (_unitsPerInch * 21) / 10;
        const short _posYIncMain = (_unitsPerInch * 3) / 4;
        const short _posYIncSub = (_unitsPerInch / 3);

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

            string[] formNamesFront = new string[pageCount];
            string[] formNamesRear = new string[pageCount];

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
                (float)PCLPaperSizes.getPaperLength(
                            (int)PCLPaperSizes.eIndex.ISO_A4,
                            _unitsPerInch,
                            PCLOrientations.eAspect.Portrait);

            for (int i = 0; i < pageCount; i++)
            {
                scaleFactors[i] = (float)
                (PCLPaperSizes.getPaperLength(
                    indxPaperSize[i],
                    _unitsPerInch,
                    PCLOrientations.eAspect.Portrait) /
                 A4LengthPort);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Generate the print job.                                        //
            //                                                                //
            //----------------------------------------------------------------//

            generateJobHeader(prnWriter);

            if (formAsMacro)
                generateOverlaySet(prnWriter,
                                   pageCount,
                                   indxPaperSize,
                                   indxPlexMode,
                                   scaleFactors,
                                   ref formCountFront,
                                   ref formCountRear,
                                   ref indxFormsFront,
                                   ref indxFormsRear,
                                   ref formNamesFront,
                                   ref formNamesRear);

            generatePageSet(prnWriter,
                            pageCount,
                            indxPaperSize,
                            indxPaperType,
                            indxPaperTray,
                            indxPlexMode,
                            indxOrientFront,
                            indxOrientRear,
                            indxFormsFront,
                            indxFormsRear,
                            formNamesFront,
                            formNamesRear,
                            scaleFactors,
                            formAsMacro);

            if (formAsMacro)
                generateOverlayDeletes(prnWriter,
                                        formCountFront, formCountRear,
                                        formNamesFront, formNamesRear);

            generateJobTrailer(prnWriter);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e J o b H e a d e r                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write stream initialisation sequences to output file.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateJobHeader(BinaryWriter prnWriter)
        {
            PCLXLWriter.stdJobHeader(prnWriter, "");
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e J o b T r a i l e r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write tray map termination sequences to output file.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateJobTrailer(BinaryWriter prnWriter)
        {
            PCLXLWriter.stdJobTrailer(prnWriter, false, "");
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e O v e r l a y D e l e t e s                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Delete overlays.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateOverlayDeletes(BinaryWriter prnWriter,
                                                     int formCountFront,
                                                     int formCountRear,
                                                     string[] formNamesFront,
                                                     string[] formNamesRear)
        {
            for (int i = 0; i < formCountFront; i++)
            {
                PCLXLWriter.streamRemove(prnWriter, formNamesFront[i]);
            }

            for (int i = 0; i < formCountRear; i++)
            {
                PCLXLWriter.streamRemove(prnWriter, formNamesRear[i]);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e O v e r l a y F r o n t                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write background data sequences for front overlay to output file.  //
        // Optionally top and tail these with macro (user-defined stream)     //
        // definition sequences.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateOverlayFront(BinaryWriter prnWriter,
                                                  bool formAsMacro,
                                                  string formName,
                                                  float scaleFactor)
        {
            const int lenBuf = 1024;

            short rectHeight = (short)(scaleFactor * (_unitsPerInch / 2));
            short rectWidth = (short)(scaleFactor * ((_unitsPerInch * 7) / 2));
            short rectStroke = (short)(scaleFactor * (_unitsPerInch / 200));
            short rectCorner = (short)(scaleFactor * (_unitsPerInch / 3));

            short ptSizeHddr = (short)(scaleFactor * 24),
                  ptSizeMain = (short)(scaleFactor * 18),
                  ptSizeSub = (short)(scaleFactor * 8);

            byte[] buffer = new byte[lenBuf];

            int indBuf;

            short posX,
                  posY,
                  posYInc;

            indBuf = 0;

            if (formAsMacro)
            {
                PCLXLWriter.streamHeader(prnWriter, true, formName);
            }

            //----------------------------------------------------------------//

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.ColorSpace,
                               (byte)PCLXLAttrEnums.eVal.eGray);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetColorSpace);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.GrayLevel,
                               128);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
                                         buffer, ref indBuf);

            //----------------------------------------------------------------//

            PCLXLWriter.font(prnWriter, formAsMacro, ptSizeHddr,
                             629, "Arial         Bd");

            //----------------------------------------------------------------//

            posYInc = (short)(scaleFactor * _posYIncMain);
            posX = (short)(scaleFactor * _posXName);
            posY = (short)(scaleFactor * _posYHddr);

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialBold, ptSizeHddr,
                       posX, posY, "Tray map test (PCL XL)");

            //----------------------------------------------------------------//

            PCLXLWriter.font(prnWriter, formAsMacro, ptSizeMain,
                             629, "Arial         Bd");

            //----------------------------------------------------------------//

            posY = (short)(scaleFactor * _posYDesc);

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialBold, ptSizeMain,
                       posX, posY, "Page Number:");

            posY += posYInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialBold, ptSizeMain,
                       posX, posY, "Paper Size:");

            posY += posYInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialBold, ptSizeMain,
                       posX, posY, "Paper Type:");

            posY += posYInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialBold, ptSizeMain,
                       posX, posY, "Plex Mode:");

            posY += posYInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialBold, ptSizeMain,
                       posX, posY, "Orientation:");

            posY += posYInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialBold, ptSizeMain,
                       posX, posY, "PCL XL Tray ID:");

            posY += posYInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialBold, ptSizeMain,
                       posX, posY, "Printer Tray:");

            //----------------------------------------------------------------//

            PCLXLWriter.font(prnWriter, formAsMacro, ptSizeSub,
                             629, "Arial         Bd");

            //----------------------------------------------------------------//

            posX = (short)(scaleFactor * (_posXValue + _posXIncSub));
            posY += (short)(scaleFactor * _posYIncSub);

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialBold, ptSizeSub,
                       posX, posY,
                       "record the tray name/number used in this box");

            //----------------------------------------------------------------//

            posX = (short)(scaleFactor * _posXValue);
            posY -= (short)(scaleFactor * (_posXIncSub * 2));

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.PushGS);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.TxMode,
                               (byte)PCLXLAttrEnums.eVal.eTransparent);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPatternTxMode);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.TxMode,
                               (byte)PCLXLAttrEnums.eVal.eTransparent);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetSourceTxMode);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.GrayLevel,
                               0);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPenSource);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.PenWidth,
                               (byte)rectStroke);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPenWidth);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.NullBrush,
                               0);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.EllipseDimension,
                                  (ushort)rectCorner, (ushort)rectCorner);

            PCLXLWriter.addAttrUint16Box(ref buffer,
                                   ref indBuf,
                                   PCLXLAttributes.eTag.BoundingBox,
                                   (ushort)posX, (ushort)posY,
                                   (ushort)(posX + rectWidth),
                                   (ushort)(posY + rectHeight));

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.RoundRectangle);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.PopGS);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            //----------------------------------------------------------------//

            if (formAsMacro)
            {
                PCLXLWriter.addOperator(ref buffer,
                                  ref indBuf,
                                  PCLXLOperators.eTag.EndStream);

                prnWriter.Write(buffer, 0, indBuf);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e O v e r l a y R e a r                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write background data sequences for rear overlay to output file.   //
        // Optionally top and tail these with macro (user-defined stream)     //
        // definition sequences.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateOverlayRear(BinaryWriter prnWriter,
                                                 bool formAsMacro,
                                                 string formName,
                                                 float scaleFactor)
        {
            const int lenBuf = 1024;

            byte[] buffer = new byte[lenBuf];

            int indBuf;

            short posX,
                  posY,
                  posYInc;

            int ptSizeHddr = (int)(scaleFactor * 24),
                  ptSizeMain = (int)(scaleFactor * 18);

            indBuf = 0;

            if (formAsMacro)
            {
                PCLXLWriter.streamHeader(prnWriter, true, formName);
            }

            //----------------------------------------------------------------//

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.ColorSpace,
                               (byte)PCLXLAttrEnums.eVal.eGray);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetColorSpace);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.GrayLevel,
                               128);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
                                         buffer, ref indBuf);

            //----------------------------------------------------------------//

            PCLXLWriter.font(prnWriter, formAsMacro, ptSizeHddr,
                             629, "Arial         Bd");

            //----------------------------------------------------------------//

            posYInc = (short)(scaleFactor * _posYIncMain);

            posX = (short)(scaleFactor * _posXName);
            posY = (short)(scaleFactor * _posYHddr);

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialBold, ptSizeHddr,
                       posX, posY, "Tray map test (PCL XL)");

            //----------------------------------------------------------------//

            PCLXLWriter.font(prnWriter, formAsMacro, ptSizeMain,
                             629, "Arial         Bd");

            //----------------------------------------------------------------//

            posY = (short)(scaleFactor * _posYDesc);

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialBold, ptSizeMain,
                       posX, posY, "Page Number:");

            posY += (short)(posYInc * 4);

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_ArialBold, ptSizeMain,
                       posX, posY, "Orientation:");

            //----------------------------------------------------------------//

            if (formAsMacro)
            {
                PCLXLWriter.addOperator(ref buffer,
                                  ref indBuf,
                                  PCLXLOperators.eTag.EndStream);

                prnWriter.Write(buffer, 0, indBuf);
            }
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

        public static void generateOverlaySet(BinaryWriter prnWriter,
                                               int pageCount,
                                               int[] indxPaperSize,
                                               int[] indxPlexMode,
                                               float[] scaleFactors,
                                               ref int formCountFront,
                                               ref int formCountRear,
                                               ref int[] indxFormsFront,
                                               ref int[] indxFormsRear,
                                               ref string[] formNamesFront,
                                               ref string[] formNamesRear)
        {
            int crntFormFront,
                  crntFormRear;

            bool[] duplexSheet = new bool[pageCount];

            //----------------------------------------------------------------//
            //                                                                //
            // Which sheets are duplex?.                                      //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < pageCount; i++)
            {
                if (PCLPlexModes.isSimplex(indxPlexMode[i]))
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

            formNamesFront[crntFormFront] =
                _rootNameFront +
                PCLPaperSizes.getName(indxPaperSize[0]);

            generateOverlayFront(prnWriter, true,
                                  formNamesFront[crntFormFront],
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

                    formNamesFront[crntFormFront] =
                        _rootNameFront +
                        PCLPaperSizes.getName(indxPaperSize[i]);

                    generateOverlayFront(prnWriter, true,
                                          formNamesFront[crntFormFront],
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
                formNamesRear[crntFormRear] =
                    _rootNameRear +
                    PCLPaperSizes.getName(indxPaperSize[0]);

                generateOverlayRear(prnWriter, true,
                                     formNamesRear[crntFormRear],
                                     scaleFactors[0]);

                indxFormsRear[0] = crntFormRear++;
            }
            else
            {
                indxFormsRear[0] = _noForm;
            }

            //----------------------------------------------------------------//
            // Subsequent sheets.                                             //
            //----------------------------------------------------------------//

            for (int i = 1; i < pageCount; i++)
            {
                if (!duplexSheet[i])
                {
                    indxFormsRear[i] = _noForm;
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
                        formNamesRear[crntFormRear] =
                            _rootNameRear +
                            PCLPaperSizes.getName(indxPaperSize[i]);

                        generateOverlayRear(prnWriter, true,
                                             formNamesRear[crntFormRear],
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

        private static void generatePage(BinaryWriter prnWriter,
                                         int pageNo,
                                         int pageCount,
                                         int indxPaperSize,
                                         int indxPaperType,
                                         int indxPaperTray,
                                         int indxPlexMode,
                                         int indxOrientFront,
                                         int indxOrientRear,
                                         string formNameFront,
                                         string formNameRear,
                                         float scaleFactor,
                                         bool formAsMacro)
        {
            const int sizeStd = 1024;

            byte[] bufStd = new byte[sizeStd];

            int indStd;

            int ptSizeMain = (int)(scaleFactor * 20);

            short posX,
                  posY,
                  posYInc;

            string tmpStr;

            bool simplex = PCLPlexModes.isSimplex(indxPlexMode);

            indStd = 0;

            PCLXLWriter.pageBegin(prnWriter,
                                   indxPaperSize,
                                   indxPaperType,
                                   indxPaperTray,
                                   indxOrientFront,
                                   indxPlexMode,
                                   true,        // always true 'cos possible different Paper Type on each sheet
                                   true);

            //----------------------------------------------------------------//

            if (formAsMacro)
            {
                PCLXLWriter.addAttrUbyteArray(ref bufStd,
                                        ref indStd,
                                        PCLXLAttributes.eTag.StreamName,
                                        formNameFront);

                PCLXLWriter.addOperator(ref bufStd,
                                  ref indStd,
                                  PCLXLOperators.eTag.ExecStream);

                prnWriter.Write(bufStd, 0, indStd);
                indStd = 0;
            }
            else
            {
                prnWriter.Write(bufStd, 0, indStd);
                indStd = 0;

                generateOverlayFront(prnWriter, false,
                                      "", scaleFactor);
            }

            //----------------------------------------------------------------//

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.ColorSpace,
                               (byte)PCLXLAttrEnums.eVal.eGray);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetColorSpace);

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               0);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               0);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetPenSource);

            prnWriter.Write(bufStd, 0, indStd);
            indStd = 0;

            //----------------------------------------------------------------//

            PCLXLWriter.font(prnWriter, false, ptSizeMain,
                             629, "Courier       Bd");

            //----------------------------------------------------------------//

            posYInc = (short)(scaleFactor * _posYIncMain);

            posX = (short)(scaleFactor * _posXValue);
            posY = (short)(scaleFactor * _posYDesc);

            tmpStr = pageNo.ToString() + " of " + pageCount.ToString();

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSizeMain,
                       posX, posY, tmpStr);

            //----------------------------------------------------------------//

            posY += posYInc;

            if (indxPaperSize >= PCLPaperSizes.getCount())
                tmpStr = "*** unknown ***";
            else
                tmpStr = PCLPaperSizes.getName(indxPaperSize);

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSizeMain,
                       posX, posY, tmpStr);

            //----------------------------------------------------------------//

            posY += posYInc;

            if (indxPaperType >= PCLPaperTypes.getCount())
                tmpStr = "*** unknown ***";
            else if (PCLPaperTypes.getType(indxPaperType) ==
                    PCLPaperTypes.eEntryType.NotSet)
                tmpStr = "<not set>";
            else
                tmpStr = PCLPaperTypes.getName(indxPaperType);

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSizeMain,
                       posX, posY, tmpStr);

            //----------------------------------------------------------------//

            posY += posYInc;

            if (indxPlexMode >= PCLPlexModes.getCount())
                tmpStr = "*** unknown ***";
            else
                tmpStr = PCLPlexModes.getName(indxPlexMode);

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSizeMain,
                       posX, posY, tmpStr);

            //----------------------------------------------------------------//

            posY += posYInc;

            if (indxOrientFront >= PCLOrientations.getCount())
                tmpStr = "*** unknown ***";
            else
                tmpStr = PCLOrientations.getName(indxOrientFront);

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSizeMain,
                       posX, posY, tmpStr);

            //----------------------------------------------------------------//

            posY += posYInc;

            if (indxPaperTray < 0)
                tmpStr = "<not set>";
            else if (indxPaperTray == _trayIdAutoSelectPCLXL)
                tmpStr = indxPaperTray.ToString() + " (auto-select)";
            else
                tmpStr = indxPaperTray.ToString();

            PCLXLWriter.text(prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSizeMain,
                       posX, posY, tmpStr);

            //----------------------------------------------------------------//

            PCLXLWriter.addAttrUint16(ref bufStd,
                                ref indStd,
                                PCLXLAttributes.eTag.PageCopies,
                                1);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.EndPage);

            prnWriter.Write(bufStd, 0, indStd);

            //----------------------------------------------------------------//
            //                                                                // 
            // Rear face (if not simplex)                                     // 
            //                                                                // 
            //----------------------------------------------------------------//

            if (!simplex)
            {

                indStd = 0;

                PCLXLWriter.pageBegin(prnWriter,
                                       indxPaperSize,
                                       indxPaperType,
                                       indxPaperTray,
                                       indxOrientRear,
                                       indxPlexMode,
                                       true,        // always true 'cos possible different Paper Type on each sheet
                                       false);

                //----------------------------------------------------------------//

                if (formAsMacro)
                {
                    PCLXLWriter.addAttrUbyteArray(ref bufStd,
                                            ref indStd,
                                            PCLXLAttributes.eTag.StreamName,
                                            formNameRear);

                    PCLXLWriter.addOperator(ref bufStd,
                                      ref indStd,
                                      PCLXLOperators.eTag.ExecStream);

                    prnWriter.Write(bufStd, 0, indStd);
                    indStd = 0;
                }
                else
                {
                    prnWriter.Write(bufStd, 0, indStd);
                    indStd = 0;

                    generateOverlayRear(prnWriter, false,
                                         "", scaleFactor);

                }

                //----------------------------------------------------------------//

                PCLXLWriter.addAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.ColorSpace,
                                   (byte)PCLXLAttrEnums.eVal.eGray);

                PCLXLWriter.addOperator(ref bufStd,
                                  ref indStd,
                                  PCLXLOperators.eTag.SetColorSpace);

                PCLXLWriter.addAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.GrayLevel,
                                   0);

                PCLXLWriter.addOperator(ref bufStd,
                                  ref indStd,
                                  PCLXLOperators.eTag.SetBrushSource);

                PCLXLWriter.addAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.GrayLevel,
                                   0);

                PCLXLWriter.addOperator(ref bufStd,
                                  ref indStd,
                                  PCLXLOperators.eTag.SetPenSource);

                prnWriter.Write(bufStd, 0, indStd);
                indStd = 0;

                //----------------------------------------------------------------//

                PCLXLWriter.font(prnWriter, false, ptSizeMain,
                                 629, "Courier       Bd");

                //----------------------------------------------------------------//

                posX = (short)(scaleFactor * _posXValue);
                posY = (short)(scaleFactor * _posYDesc);

                tmpStr = pageNo.ToString() + " (rear) of " + pageCount.ToString();

                PCLXLWriter.text(prnWriter, false, false,
                           PCLXLWriter.advances_Courier, ptSizeMain,
                           posX, posY, tmpStr);

                //----------------------------------------------------------------//

                posY += (short)(posYInc * 4);

                if (indxOrientRear >= PCLOrientations.getCount())
                    tmpStr = "*** unknown ***";
                else
                    tmpStr = PCLOrientations.getName(indxOrientRear);

                PCLXLWriter.text(prnWriter, false, false,
                           PCLXLWriter.advances_Courier, ptSizeMain,
                           posX, posY, tmpStr);

                //----------------------------------------------------------------//

                PCLXLWriter.addAttrUint16(ref bufStd,
                                    ref indStd,
                                    PCLXLAttributes.eTag.PageCopies,
                                    1);

                PCLXLWriter.addOperator(ref bufStd,
                                  ref indStd,
                                  PCLXLOperators.eTag.EndPage);

                prnWriter.Write(bufStd, 0, indStd);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e P a g e S e t                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write set of test data pages to output file.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generatePageSet(BinaryWriter prnWriter,
                                             int pageCount,
                                             int[] indxPaperSize,
                                             int[] indxPaperType,
                                             int[] indxPaperTray,
                                             int[] indxPlexMode,
                                             int[] indxOrientFront,
                                             int[] indxOrientRear,
                                             int[] indxFormsFront,
                                             int[] indxFormsRear,
                                             string[] formNamesFront,
                                             string[] formNamesRear,
                                             float[] scaleFactors,
                                             bool formAsMacro)
        {
            for (int i = 0; i < pageCount; i++)
            {
                string formNameFront;
                string formNameRear;

                int index;

                if (formAsMacro)
                {
                    index = indxFormsFront[i];

                    formNameFront = formNamesFront[index];

                    index = indxFormsRear[i];

                    if (index == _noForm)
                        formNameRear = "";
                    else
                        formNameRear = formNamesRear[index];
                }
                else
                {
                    formNameFront = "";
                    formNameRear = "";
                }

                generatePage(prnWriter,
                              i + 1,
                              pageCount,
                              indxPaperSize[i],
                              indxPaperType[i],
                              indxPaperTray[i],
                              indxPlexMode[i],
                              indxOrientFront[i],
                              indxOrientRear[i],
                              formNameFront,
                              formNameRear,
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
                return _trayIdAutoSelectPCLXL;
            }
        }
    }
}
