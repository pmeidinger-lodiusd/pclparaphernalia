using System.IO;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides PCL XL support for the FormSample tool.
/// 
/// © Chris Hutchinson 2012
/// 
/// </summary>

static class ToolFormSamplePCLX
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    const int _symSet_19U = 629;

    public enum eStreamMethod : byte
    {
        ExecuteBegin,
        ExecuteEnd,
        Max
    }

    private static readonly string[] streamMethodNames =
    {
        "Execute stream (@ start of page)",
        "Execute stream (@ end of page)",
        "Max - invalid"
    };

    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

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

    public static void GenerateJob(
        BinaryWriter prnWriter,
        int indxPaperSize,
        int indxPaperType,
        int indxOrientation,
        int indxOrientRear,
        int indxPlexMode,
        int testPageCount,
        bool flagMainEncapsulated,
        bool flagRearEncapsulated,
        bool flagStreamRemove,
        bool flagMainForm,
        bool flagRearForm,
        bool flagRearBPlate,
        bool flagGSPushPop,
        bool flagPrintDescText,
        string formFileMain,
        string formFileRear,
        eStreamMethod indxMethod,
        string formNameMain,
        string formNameRear)
    {
        bool flagSimplexJob = PCLPlexModes.IsSimplex(indxPlexMode);

        GenerateJobHeader(prnWriter,
                          indxPaperSize,
                          indxPaperType,
                          indxOrientation,
                          indxPlexMode,
                          flagSimplexJob,
                          flagMainEncapsulated,
                          flagRearEncapsulated,
                          flagStreamRemove,
                          flagMainForm,
                          flagRearForm,
                          formFileMain,
                          formFileRear,
                          indxMethod,
                          formNameMain,
                          formNameRear);

        GeneratePageSet(prnWriter,
                         testPageCount,
                         indxPaperSize,
                         indxPaperType,
                         indxOrientation,
                         indxOrientRear,
                         indxPlexMode,
                         flagSimplexJob,
                         flagMainForm,
                         flagRearForm,
                         flagRearBPlate,
                         flagGSPushPop,
                         flagPrintDescText,
                         formFileMain,
                         formFileRear,
                         indxMethod,
                         formNameMain,
                         formNameRear);

        GenerateJobTrailer(prnWriter,
                            flagStreamRemove,
                            flagMainForm,
                            flagRearForm,
                            formNameMain,
                            formNameRear);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e J o b H e a d e r                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write stream initialisation sequences to output file.              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateJobHeader(
        BinaryWriter prnWriter,
        int indxPaperSize,
        int indxPaperType,
        int indxOrientation,
        int indxPlexMode,
        bool flagSimplexJob,
        bool flagMainEncapsulated,
        bool flagRearEncapsulated,
        bool flagStreamRemove,
        bool flagMainForm,
        bool flagRearForm,
        string formFileMain,
        string formFileRear,
        eStreamMethod indxMethod,
        string formNameMain,
        string formNameRear)
    {
        PCLXLWriter.StdJobHeader(prnWriter, string.Empty);

        if (flagMainForm)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Main (or only) form in use.                                //
            // Download contents of specified file.                       //
            //                                                            //
            //------------------------------------------------------------//

            PCLXLDownloadStream.StreamFileEmbed(prnWriter,
                                                formFileMain,
                                                formNameMain,
                                                flagMainEncapsulated);
        }

        if (!flagSimplexJob)
        {
            if (flagRearForm)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Rear form in use.                                      //
                // Download contents of specified file.                   //
                //                                                        //
                //--------------------------------------------------------//

                PCLXLDownloadStream.StreamFileEmbed(prnWriter,
                                                    formFileRear,
                                                    formNameRear,
                                                    flagRearEncapsulated);
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e J o b T r a i l e r                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write termination sequences to output file.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateJobTrailer(BinaryWriter prnWriter,
                                           bool flagStreamRemove,
                                           bool flagMainForm,
                                           bool flagRearForm,
                                           string formNameMain,
                                           string formNameRear)
    {
        if (flagStreamRemove)
        {
            if (flagMainForm)
                PCLXLWriter.StreamRemove(prnWriter, formNameMain);

            if (flagRearForm)
                PCLXLWriter.StreamRemove(prnWriter, formNameRear);
        }

        PCLXLWriter.StdJobTrailer(prnWriter, false, string.Empty);
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
                                     int indxOrientation,
                                     int indxOrientRear,
                                     int indxPlexMode,
                                     bool flagFrontFace,
                                     bool flagSimplexJob,
                                     bool flagMainForm,
                                     bool flagRearForm,
                                     bool flagRearBPlate,
                                     bool flagGSPushPop,
                                     bool flagPrintDescText,
                                     string formFileMain,
                                     string formFileRear,
                                     eStreamMethod indxMethod,
                                     string formNameMain,
                                     string formNameRear)
    {
        const int lenBuf = 1024;
        const short incPosY = 150;

        byte[] buffer = new byte[lenBuf];

        bool pageUsesForm;

        short posX,
              posY;

        string formName;
        int indxOrient;

        int indBuf = 0;
        int crntPtSize;

        bool altOrient = indxOrientation != indxOrientRear;
        bool firstPage = pageNo == 1;

        if (flagFrontFace)
        {
            indxOrient = indxOrientation;
            pageUsesForm = flagMainForm;
            formName = formNameMain;
        }
        else
        {
            indxOrient = indxOrientRear;

            if (flagRearForm)
            {
                pageUsesForm = flagRearForm;
                formName = formNameRear;
            }
            else
            {
                pageUsesForm = flagMainForm;
                formName = formNameMain;
            }
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Write 'BeginPage' operator and (if requested for begin page)   //
        // the required stream 'execute' operator.                        //
        //                                                                //
        //----------------------------------------------------------------//

        PCLXLWriter.PageBegin(prnWriter,
                               indxPaperSize,
                               indxPaperType,
                               -1,
                               indxOrient,
                               indxPlexMode,
                               firstPage,
                               flagFrontFace);

        if (pageUsesForm)
        {
            if (indxMethod == eStreamMethod.ExecuteBegin)
            {
                if (flagGSPushPop)
                {
                    PCLXLWriter.AddOperator(
                       ref buffer,
                       ref indBuf,
                       PCLXLOperators.eTag.PushGS);

                    prnWriter.Write(buffer, 0, indBuf);
                    indBuf = 0;
                }

                PCLXLWriter.StreamExec(prnWriter, false, formName);

                if (flagGSPushPop)
                {
                    PCLXLWriter.AddOperator(
                        ref buffer,
                        ref indBuf,
                        PCLXLOperators.eTag.PopGS);

                    prnWriter.Write(buffer, 0, indBuf);
                    indBuf = 0;
                }
            }
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Write descriptive text.                                        //
        //                                                                //
        //----------------------------------------------------------------//

        if (flagPrintDescText)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Headers.                                                   //
            //                                                            //
            //------------------------------------------------------------//

            crntPtSize = 10;

            PCLXLWriter.AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.ColorSpace,
                                     (byte)PCLXLAttrEnums.eVal.eGray);

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.SetColorSpace);

            PCLXLWriter.AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.GrayLevel,
                                     0);

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.SetBrushSource);

            prnWriter.Write(buffer, 0, indBuf);
            indBuf = 0;

            PCLXLWriter.Font(prnWriter, false, crntPtSize,
                             _symSet_19U, "Courier         ");

            posX = 600;
            posY = 1350;

            PCLXLWriter.Text(prnWriter, false, false,
                             PCLXLWriter.advances_Courier,
                              crntPtSize, posX, posY, "Page:");

            if (firstPage)
            {
                posY += incPosY;

                PCLXLWriter.Text(prnWriter, false, false,
                                  PCLXLWriter.advances_Courier, crntPtSize,
                                  posX, posY, "PaperSize:");

                posY += incPosY;

                PCLXLWriter.Text(prnWriter, false, false,
                                  PCLXLWriter.advances_Courier, crntPtSize,
                                  posX, posY, "PaperType:");

                posY += incPosY;

                PCLXLWriter.Text(prnWriter, false, false,
                                  PCLXLWriter.advances_Courier, crntPtSize,
                                  posX, posY, "Plex Mode:");

                posY += incPosY;

                PCLXLWriter.Text(prnWriter, false, false,
                                  PCLXLWriter.advances_Courier, crntPtSize,
                                  posX, posY, "Method:");

                posY += incPosY;

                PCLXLWriter.Text(prnWriter, false, false,
                                  PCLXLWriter.advances_Courier, crntPtSize,
                                  posX, posY, "Orientation:");

                posY += incPosY;

                PCLXLWriter.Text(prnWriter, false, false,
                                  PCLXLWriter.advances_Courier, crntPtSize,
                                  posX, posY, "Rear Orientation:");

                posY += incPosY;

                PCLXLWriter.Text(prnWriter, false, false,
                                  PCLXLWriter.advances_Courier, crntPtSize,
                                  posX, posY, "Main Form:");

                posY += incPosY;

                if (flagRearForm)
                    if (flagRearBPlate)
                        PCLXLWriter.Text(prnWriter, false, false,
                                          PCLXLWriter.advances_Courier,
                                          crntPtSize, posX, posY,
                                          "Rear Boilerplate:");
                    else
                        PCLXLWriter.Text(prnWriter, false, false,
                                          PCLXLWriter.advances_Courier,
                                          crntPtSize, posX, posY,
                                          "Rear Form:");
            }

            //------------------------------------------------------------//
            //                                                            //
            // Write variable data.                                       //
            //                                                            //
            //------------------------------------------------------------//

            crntPtSize = 10;

            PCLXLWriter.AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.GrayLevel,
                                     0);

            PCLXLWriter.AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.SetBrushSource);

            prnWriter.Write(buffer, 0, indBuf);
            indBuf = 0;

            PCLXLWriter.Font(prnWriter, false, crntPtSize,
                             _symSet_19U, "Courier       Bd");

            posX = 1800;
            posY = 1350;

            PCLXLWriter.Text(prnWriter, false, false,
                             PCLXLWriter.advances_Courier, crntPtSize,
                             posX, posY,
                             pageNo + " of " + pageCount);

            if (firstPage)
            {
                string textOrientRear;

                posY += incPosY;

                PCLXLWriter.Text(prnWriter, false, false,
                                  PCLXLWriter.advances_Courier, crntPtSize,
                                  posX, posY,
                                  PCLPaperSizes.GetName(indxPaperSize));

                posY += incPosY;

                PCLXLWriter.Text(prnWriter, false, false,
                                  PCLXLWriter.advances_Courier, crntPtSize,
                                  posX, posY,
                                  PCLPaperTypes.GetName(indxPaperType));

                posY += incPosY;

                PCLXLWriter.Text(prnWriter, false, false,
                                  PCLXLWriter.advances_Courier, crntPtSize,
                                  posX, posY,
                                  PCLPlexModes.GetName(indxPlexMode));

                posY += incPosY;

                PCLXLWriter.Text(prnWriter, false, false,
                                  PCLXLWriter.advances_Courier, crntPtSize,
                                  posX, posY,
                                  streamMethodNames[(int)indxMethod]);

                posY += incPosY;

                PCLXLWriter.Text(prnWriter, false, false,
                                  PCLXLWriter.advances_Courier, crntPtSize,
                                  posX, posY,
                                  PCLOrientations.GetName(indxOrientation));

                if (flagSimplexJob)
                    textOrientRear = "<not applicable>";
                else
                    textOrientRear = PCLOrientations.GetName(indxOrientRear);

                posY += incPosY;

                PCLXLWriter.Text(prnWriter, false, false,
                                  PCLXLWriter.advances_Courier, crntPtSize,
                                  posX, posY,
                                  textOrientRear);

                posY += incPosY;

                if (flagMainForm)
                {
                    const int maxLen = 51;
                    const int halfLen = (maxLen - 5) / 2;

                    int len = formFileMain.Length;

                    if (len < maxLen)
                        PCLXLWriter.Text(prnWriter, false, false,
                                          PCLXLWriter.advances_Courier,
                                          crntPtSize, posX, posY, formFileMain);
                    else
                        PCLXLWriter.Text(prnWriter, false, false,
                                          PCLXLWriter.advances_Courier,
                                          crntPtSize, posX, posY,
                                          formFileMain.Substring(0, halfLen) +
                                          " ... " +
                                          formFileMain.Substring(len - halfLen,
                                                                  halfLen));
                }

                posY += incPosY;

                if (flagRearForm)
                {
                    const int maxLen = 51;
                    const int halfLen = (maxLen - 5) / 2;

                    int len = formFileRear.Length;

                    if (len < maxLen)
                        PCLXLWriter.Text(prnWriter, false, false,
                                          PCLXLWriter.advances_Courier,
                                          crntPtSize, posX, posY, formFileRear);
                    else
                        PCLXLWriter.Text(prnWriter, false, false,
                                          PCLXLWriter.advances_Courier,
                                          crntPtSize, posX, posY,
                                          formFileRear.Substring(0, halfLen) +
                                          " ... " +
                                          formFileRear.Substring(len - halfLen,
                                                                  halfLen));
                }
            }
        }

        //----------------------------------------------------------------//
        //                                                                //
        // If requested for end page, write the required stream 'execute' //
        // operator.                                                      //
        //                                                                //
        //----------------------------------------------------------------//

        if (pageUsesForm)
        {
            if (indxMethod == eStreamMethod.ExecuteEnd)
            {
                if (flagGSPushPop)
                {
                    PCLXLWriter.AddOperator(
                       ref buffer,
                       ref indBuf,
                       PCLXLOperators.eTag.PushGS);

                    prnWriter.Write(buffer, 0, indBuf);
                    indBuf = 0;
                }

                PCLXLWriter.StreamExec(prnWriter, false, formName);

                if (flagGSPushPop)
                {
                    PCLXLWriter.AddOperator(
                        ref buffer,
                        ref indBuf,
                        PCLXLOperators.eTag.PopGS);

                    prnWriter.Write(buffer, 0, indBuf);
                    indBuf = 0;
                }
            }
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Write EndPage' operator and associated attribute list.         //
        //                                                                //
        //----------------------------------------------------------------//

        PCLXLWriter.PageEnd(prnWriter, 1);

        //------------------------------------------------------------//
        //                                                            //
        // Generate rear boilerplate side if necessary.               //
        //                                                            //
        //------------------------------------------------------------//

        if ((flagRearForm) && (flagRearBPlate))
        {
            PCLXLWriter.PageBegin(prnWriter,
                                   indxPaperSize,
                                   indxPaperType,
                                   -1,
                                   indxOrientRear,
                                   indxPlexMode,
                                   firstPage,
                                   false);

            PCLXLWriter.StreamExec(prnWriter, false, formNameRear);

            PCLXLWriter.PageEnd(prnWriter, 1);
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

    private static void GeneratePageSet(BinaryWriter prnWriter,
                                        int pageCount,
                                        int indxPaperSize,
                                        int indxPaperType,
                                        int indxOrientation,
                                        int indxOrientRear,
                                        int indxPlexMode,
                                        bool flagSimplexJob,
                                        bool flagMainForm,
                                        bool flagRearForm,
                                        bool flagRearBPlate,
                                        bool flagGSPushPop,
                                        bool flagPrintDescText,
                                        string formFileMain,
                                        string formFileRear,
                                        eStreamMethod indxMethod,
                                        string formNameMain,
                                        string formNameRear)
    {
        bool flagFrontFace = true;

        for (int pageNo = 1; pageNo <= pageCount; pageNo++)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Generate test pages.                                       //
            //                                                            //
            //------------------------------------------------------------//

            GeneratePage(prnWriter,
                          pageNo,
                          pageCount,
                          indxPaperSize,
                          indxPaperType,
                          indxOrientation,
                          indxOrientRear,
                          indxPlexMode,
                          flagFrontFace,
                          flagSimplexJob,
                          flagMainForm,
                          flagRearForm,
                          flagRearBPlate,
                          flagGSPushPop,
                          flagPrintDescText,
                          formFileMain,
                          formFileRear,
                          indxMethod,
                          formNameMain,
                          formNameRear);

            //------------------------------------------------------------//
            //                                                            //
            // Toggle front/rear face indicator.                          //
            //                                                            //
            //------------------------------------------------------------//

            if (!flagSimplexJob)
            {
                if (!flagRearForm)
                    flagFrontFace = !flagFrontFace;
                else if (!flagRearBPlate)
                    flagFrontFace = !flagFrontFace;
            }
        }
    }
}
