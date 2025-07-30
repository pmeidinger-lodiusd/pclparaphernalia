using System.Data;
using System.IO;
using System.Text;
using System.Windows;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides common routines associated with the Make Overlay tool.
/// 
/// © Chris Hutchinson 2012
/// 
/// </summary>

static class PrnParseMakeOvl
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static readonly ASCIIEncoding _ascii = new ASCIIEncoding();

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // b r e a k p o i n t                                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Process Make Overlay run break-point action.                       //
    // If break-point action is indicated:                                //
    //  - Copy/skip parts of input .prn file to output overlay file       //
    //  - Adjust break-point data                                         //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static bool Breakpoint(PrnParseLinkData linkData,
                                    Stream ipStream,
                                    BinaryReader binReader,
                                    BinaryWriter binWriter)
    {
        bool terminate = false;
        bool update = false;

        bool ovlXL = linkData.MakeOvlXL;
        bool encapsulate = linkData.MakeOvlEncapsulate;

        PrnParseConstants.eOvlAct action = linkData.MakeOvlAct;

        //----------------------------------------------------------------//
        //                                                                //
        // Check whether file update required.                            //
        //                                                                //
        //----------------------------------------------------------------//

        if (action == PrnParseConstants.eOvlAct.Terminate)
        {
            terminate = true;
        }
        else if (action == PrnParseConstants.eOvlAct.EndOfFile)
        {
            update = true;
        }
        else if ((action == PrnParseConstants.eOvlAct.Download)
                                      ||
                 (action == PrnParseConstants.eOvlAct.DownloadDelete)
                                      ||
                 (action == PrnParseConstants.eOvlAct.IdFont)
                                      ||
                 (action == PrnParseConstants.eOvlAct.IdPalette)
                                      ||
                 (action == PrnParseConstants.eOvlAct.IdPattern)
                                      ||
                 (action == PrnParseConstants.eOvlAct.IdSymSet)
                                      ||
                 (action == PrnParseConstants.eOvlAct.IdMacro))
        {
            //------------------------------------------------------------//
            //                                                            //
            // No action - actions defined for future enhancement         //
            //                                                            //
            //------------------------------------------------------------//
        }
        else if (action != PrnParseConstants.eOvlAct.None)
        {
            //------------------------------------------------------------//
            //                                                            //
            //   (action == PrnParseConstants.eOvlAct.PushGS)             // 
            //                        ||                                  //
            //   (action == PrnParseConstants.eOvlAct.Remove)             //
            //                        ||                                  //
            //   (action == PrnParseConstants.eOvlAct.Replace_0x77)       //
            //                        ||                                  //
            //   (action == PrnParseConstants.eOvlAct.Reset)              //
            //                        ||                                  //
            //   (action == PrnParseConstants.eOvlAct.IgnorePage)         //
            //                        ||                                  //
            //   (action == PrnParseConstants.eOvlAct.Adjust))            //
            //                                                   etc...   //
            //------------------------------------------------------------//

            update = true;
        }

        if (update)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Update output overlay file according to break-point data.  //
            //                                                            //
            //------------------------------------------------------------//

            const int bufSize = 1024;

            long comboStart = 0,
                  syncLen,
                  fragLen;

            int readLen;

            byte[] buf = new byte[bufSize];

            bool comboSeq = false,
                    comboFirst = false,
                    comboLast = false,
                    comboModified = false;

            linkData.GetPCLComboData(ref comboSeq,
                                      ref comboFirst,
                                      ref comboLast,
                                      ref comboModified,
                                      ref comboStart);

            long crntPos = linkData.MakeOvlOffset;
            long skipBegin = linkData.MakeOvlSkipBegin;
            long skipEnd = linkData.MakeOvlSkipEnd;

            if (skipBegin >= 0)
            {
                if (ovlXL)
                {
                    syncLen = skipBegin - crntPos;
                    fragLen = 0;
                }
                else if ((comboSeq) && (!comboFirst))
                {
                    if (comboStart > crntPos)
                    {
                        syncLen = comboStart - crntPos;

                        if (comboStart < skipBegin)
                            fragLen = skipBegin - comboStart;
                        else
                            fragLen = 0;
                    }
                    else if (comboStart < crntPos)
                    {
                        syncLen = 0;

                        if (crntPos < skipBegin)
                            fragLen = skipBegin - crntPos;
                        else
                            fragLen = 0;
                    }
                    else
                    {
                        syncLen = skipBegin - crntPos;
                        fragLen = 0;
                    }
                }
                else
                {
                    syncLen = skipBegin - crntPos;
                    fragLen = 0;
                }
            }
            else if (action == PrnParseConstants.eOvlAct.EndOfFile)
            {
                syncLen = linkData.FileSize - crntPos;
                fragLen = -1;
            }
            else
            {
                syncLen = -1;
                fragLen = -1;
            }

            if ((syncLen > 0) || (fragLen > 0))
            {
                try
                {
                    ipStream.Seek(crntPos, SeekOrigin.Begin);
                }

                catch (IOException e)
                {
                    MessageBox.Show("' IO Exception:\r\n" +
                                     e.Message +
                                     "\r\n\r\nSeeking to offset " + crntPos,
                                     "Synchronising",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);
                }

                //--------------------------------------------------------//

                if (syncLen > 0)
                {
                    //----------------------------------------------------//
                    //                                                    //
                    // Copy from last 'current' synchronisation position  //
                    // up to the 'skip begin' position.                   //
                    //                                                    //
                    //----------------------------------------------------//

                    readLen = 0;

                    try
                    {
                        while (syncLen > 0)
                        {
                            if (syncLen > bufSize)
                                readLen = bufSize;
                            else
                                readLen = (int)syncLen;

                            syncLen -= readLen;

                            binReader.Read(buf, 0, readLen);

                            if (ovlXL)
                            {
                                PCLXLWriter.WriteStreamBlock(binWriter,
                                                              encapsulate,
                                                              buf,
                                                              ref readLen);
                            }
                            else
                            {
                                binWriter.Write(buf, 0, readLen);
                            }
                        }
                    }

                    catch (IOException e)
                    {
                        MessageBox.Show("' IO Exception:\r\n" +
                                         e.Message +
                                         "\r\n\r\nCopying " + readLen + " bytes",
                                         "Synchronising",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Error);
                    }
                }

                //--------------------------------------------------------//

                if (fragLen > 0)
                {
                    //----------------------------------------------------//
                    //                                                    //
                    // Handle PCL complex sequences where one (or more)   //
                    // elements have been removed.                        //
                    //                                                    //
                    //----------------------------------------------------//

                    readLen = 0;

                    try
                    {

                        byte crntByte;

                        byte[] termByte = new byte[1];

                        readLen = (int)fragLen;

                        binReader.Read(buf, 0, readLen);

                        if (buf[0] != PrnParseConstants.asciiEsc)
                        {
                            // add in root

                            int prefixLen = 0;

                            byte prefixA = 0x20,
                                 prefixB = 0x20;

                            byte[] rootBuf = new byte[3];

                            linkData.GetPrefixData(ref prefixLen,
                                                    ref prefixA,
                                                    ref prefixB);

                            rootBuf[0] = 0x1b;
                            rootBuf[1] = prefixA;
                            rootBuf[2] = prefixB;

                            binWriter.Write(rootBuf, 0, prefixLen + 1);
                        }

                        crntByte = buf[readLen - 1];

                        if ((crntByte >= PrnParseConstants.pclComplexPCharLow)
                                               &&
                            (crntByte <= PrnParseConstants.pclComplexPCharHigh))
                        {
                            crntByte = (byte)(crntByte - 0x20);
                        }

                        termByte[0] = crntByte;

                        binWriter.Write(buf, 0, readLen - 1);

                        binWriter.Write(termByte, 0, 1);
                    }

                    catch (IOException e)
                    {
                        MessageBox.Show("' IO Exception:\r\n" +
                                         e.Message +
                                         "\r\n\r\nCopying " + readLen + " bytes",
                                         "Synchronising",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Error);
                    }
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Adjust break-point data.                                   //
            //                                                            //
            //------------------------------------------------------------//

            linkData.MakeOvlOffset = skipEnd;

            if (action == PrnParseConstants.eOvlAct.PageBoundary)
            {
                linkData.MakeOvlSkipBegin = skipEnd;
            }
            else if (action == PrnParseConstants.eOvlAct.PageBegin)
            {
                // keep current SkipBegin position
            }
            else if (action == PrnParseConstants.eOvlAct.PageEnd)
            {
                // keep current SkipBegin position
            }
            else if (action == PrnParseConstants.eOvlAct.Reset)
            {
                // keep current SkipBegin position
            }
            else
            {
                linkData.MakeOvlSkipBegin =
                    (int)PrnParseConstants.eOffsetPosition.Unknown;
            }

            if (action == PrnParseConstants.eOvlAct.Replace_0x77)
            {
                PCLXLWriter.WriteOperator(binWriter,
                                           PCLXLOperators.eTag.SetPageScale,
                                           encapsulate);
            }
            else if (action == PrnParseConstants.eOvlAct.PushGS)
            {
                PCLXLWriter.WriteOperator(binWriter,
                                           PCLXLOperators.eTag.PushGS,
                                           encapsulate);
            }

            action = PrnParseConstants.eOvlAct.None;
        }

        linkData.MakeOvlAct = action;

        return terminate;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h e c k A c t i o n P C L                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Check what (if any) Make Overlay action is required with current   //
    // PCL escape sequence and set up appropriate break-point data.       //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static bool CheckActionPCL(
        bool comboSeq,
        bool seqComplete,
        int vInt,
        int seqStart,
        int fragLen,
        long fileOffset,
        PrnParseLinkData linkData,
        DataTable table,
        PrnParseConstants.eOptOffsetFormats indxOffsetFormat)
    {
        bool breakpoint = false;
        bool comboModified;

        int analysisLevel;

        PrnParseConstants.eOvlPos makeOvlPosCrnt = linkData.MakeOvlPos;
        PrnParseConstants.eOvlAct makeOvlActCrnt = linkData.MakeOvlAct;
        PrnParseConstants.eOvlShow makeOvlShowCrnt = linkData.MakeOvlShow;

        PrnParseConstants.eOvlPos makeOvlPosNew = linkData.MakeOvlPos;
        PrnParseConstants.eOvlAct makeOvlActNew = linkData.MakeOvlAct;
        PrnParseConstants.eOvlShow makeOvlShowNew = linkData.MakeOvlShow;

        analysisLevel = linkData.AnalysisLevel;

        comboModified = linkData.PclComboModified;

        long seqBegin = fileOffset + seqStart;
        long seqEnd = seqBegin + fragLen;

        if (makeOvlPosCrnt == PrnParseConstants.eOvlPos.BeforeFirstPage)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Current position is BeforeFirstPage                        //
            //                                                            //
            //------------------------------------------------------------//

            if ((makeOvlActCrnt == PrnParseConstants.eOvlAct.IdMacro) &&
                     (vInt == linkData.MakeOvlMacroId))
            {
                breakpoint = true;

                makeOvlActNew = PrnParseConstants.eOvlAct.Terminate;
                makeOvlShowNew = PrnParseConstants.eOvlShow.Terminate;

                CheckActionPCLMacroClash(vInt, table);
            }
            else if (makeOvlActCrnt == PrnParseConstants.eOvlAct.PageChange)
            {
                breakpoint = true;

                makeOvlActNew = PrnParseConstants.eOvlAct.Remove;
                makeOvlShowNew = PrnParseConstants.eOvlShow.Remove;
            }
            else if (makeOvlActCrnt == PrnParseConstants.eOvlAct.PageMark)
            {
                makeOvlPosNew = PrnParseConstants.eOvlPos.WithinFirstPage;
            }
            else if (makeOvlActCrnt == PrnParseConstants.eOvlAct.Remove)
            {
                breakpoint = true;
                makeOvlShowNew = PrnParseConstants.eOvlShow.Remove;
            }
            else if (makeOvlActCrnt == PrnParseConstants.eOvlAct.Reset)
            {
                breakpoint = true;

                makeOvlActNew = PrnParseConstants.eOvlAct.Remove;
                makeOvlShowNew = PrnParseConstants.eOvlShow.Remove;
            }
            else if ((comboSeq) && (comboModified))
            {
                breakpoint = true;

                makeOvlActNew = PrnParseConstants.eOvlAct.Adjust;
                makeOvlShowNew = PrnParseConstants.eOvlShow.None;
            }
            else
            {
                makeOvlShowNew = PrnParseConstants.eOvlShow.None;
            }
        }
        else if (makeOvlPosCrnt == PrnParseConstants.eOvlPos.WithinFirstPage)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Current position is WithinFirstPage                        //
            //                                                            //
            //------------------------------------------------------------//

            if ((makeOvlActCrnt == PrnParseConstants.eOvlAct.IdMacro) &&
                     (vInt == linkData.MakeOvlMacroId))
            {
                breakpoint = true;

                makeOvlActNew = PrnParseConstants.eOvlAct.Terminate;
                makeOvlShowNew = PrnParseConstants.eOvlShow.Terminate;

                CheckActionPCLMacroClash(vInt, table);
            }
            else if (makeOvlActCrnt == PrnParseConstants.eOvlAct.PageChange)
            {
                breakpoint = true;

                makeOvlActNew = PrnParseConstants.eOvlAct.PageBegin;
                makeOvlPosNew = PrnParseConstants.eOvlPos.WithinOtherPages;
                makeOvlShowNew = PrnParseConstants.eOvlShow.None;
            }
            else if (makeOvlActCrnt == PrnParseConstants.eOvlAct.Remove)
            {
                breakpoint = true;
                makeOvlShowNew = PrnParseConstants.eOvlShow.Remove;
            }
            else if (makeOvlActCrnt == PrnParseConstants.eOvlAct.Reset)
            {
                breakpoint = true;

                makeOvlPosNew = PrnParseConstants.eOvlPos.AfterPages;
                makeOvlShowNew = PrnParseConstants.eOvlShow.Remove;
            }
            else if ((comboSeq) && (comboModified))
            {
                breakpoint = true;

                makeOvlActNew = PrnParseConstants.eOvlAct.Adjust;
                makeOvlShowNew = PrnParseConstants.eOvlShow.None;
            }
            else
            {
                makeOvlShowNew = PrnParseConstants.eOvlShow.None;
            }
        }
        else if (makeOvlPosCrnt == PrnParseConstants.eOvlPos.WithinOtherPages)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Current position is WithinOtherPages                       //
            //                                                            //
            //------------------------------------------------------------//

            if (makeOvlActCrnt == PrnParseConstants.eOvlAct.PageChange)
            {
                breakpoint = true;

                makeOvlActNew = PrnParseConstants.eOvlAct.PageBoundary;
                makeOvlShowNew = PrnParseConstants.eOvlShow.None;

                if (linkData.MakeOvlSkipBegin > 0)
                {
                    long pageStart = linkData.MakeOvlSkipBegin;
                    long pageLen = seqBegin - pageStart;

                    PrnParseCommon.AddDataRow(
                        PrnParseRowTypes.eType.MsgComment,
                        table,
                        PrnParseConstants.eOvlShow.Remove,
                        indxOffsetFormat,
                        (int)pageStart,
                        analysisLevel,
                        string.Empty,
                        "[" + pageLen + " bytes]",
                        "Subsequent page");
                }
            }
            else if (makeOvlActCrnt == PrnParseConstants.eOvlAct.Reset)
            {
                breakpoint = true;

                makeOvlPosNew = PrnParseConstants.eOvlPos.AfterPages;
                makeOvlShowNew = PrnParseConstants.eOvlShow.Remove;

                if (linkData.MakeOvlSkipBegin > 0)
                {
                    long pageStart = linkData.MakeOvlSkipBegin;
                    long pageLen = seqBegin - pageStart;

                    PrnParseCommon.AddDataRow(
                        PrnParseRowTypes.eType.MsgComment,
                        table,
                        PrnParseConstants.eOvlShow.Remove,
                        indxOffsetFormat,
                        (int)pageStart,
                        analysisLevel,
                        string.Empty,
                        "[" + pageLen + " bytes]",
                        "Subsequent page");
                }
            }
        }
        else
        {
            //------------------------------------------------------------//
            //                                                            //
            // Current position is AfterPages                             //
            //                                                            //
            //------------------------------------------------------------//

            if (makeOvlActCrnt == PrnParseConstants.eOvlAct.Reset)
            {
                breakpoint = true;

                makeOvlActNew = PrnParseConstants.eOvlAct.Remove;
                makeOvlShowNew = PrnParseConstants.eOvlShow.Remove;
            }
            else if (makeOvlActCrnt != PrnParseConstants.eOvlAct.None)
            {
                breakpoint = true;

                makeOvlActNew = PrnParseConstants.eOvlAct.Terminate;
                makeOvlShowNew = PrnParseConstants.eOvlShow.Terminate;
            }
        }

        linkData.MakeOvlPos = makeOvlPosNew;

        //----------------------------------------------------------------//
        //                                                                //
        // Set up breakpoint data if Make Overlay action is indicated.    //
        //                                                                //
        //----------------------------------------------------------------//

        if (breakpoint)
        {
            PrnParseConstants.eContType contType;
            byte iChar = 0x20,
                 gChar = 0x20;

            int prefixLen = 0;

            linkData.GetPrefixData(ref prefixLen,
                                     ref iChar,
                                     ref gChar);

            if (comboSeq)
                comboModified = true;

            if (seqComplete)
                contType = PrnParseConstants.eContType.Reset;
            else
                contType = PrnParseConstants.eContType.PCLComplex;

            linkData.PclComboModified = comboModified;

            linkData.SetContData(contType,
                                  prefixLen,
                                  (int)seqEnd,
                                  0,
                                  true,
                                  iChar,
                                  gChar);

            linkData.MakeOvlAct = makeOvlActNew;
            linkData.MakeOvlShow = makeOvlShowNew;

            if (makeOvlActNew == PrnParseConstants.eOvlAct.PageBegin)
            {
                linkData.MakeOvlSkipBegin = seqBegin;
                linkData.MakeOvlSkipEnd = seqBegin;
            }
            else if (makeOvlActNew == PrnParseConstants.eOvlAct.PageBoundary)
            {
                linkData.MakeOvlSkipEnd = seqBegin;
            }
            else if (makeOvlActNew == PrnParseConstants.eOvlAct.PageEnd)
            {
                linkData.MakeOvlSkipEnd = seqBegin;
            }
            else if (makeOvlActNew == PrnParseConstants.eOvlAct.Adjust)
            {
                linkData.MakeOvlSkipBegin = seqEnd;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else
            {
                if (linkData.MakeOvlSkipBegin < 0)
                {
                    linkData.MakeOvlSkipBegin = seqBegin;
                }

                linkData.MakeOvlSkipEnd = seqEnd;
            }
        }
        else
        {
            linkData.MakeOvlAct = makeOvlActNew;
            linkData.MakeOvlShow = makeOvlShowNew;
        }

        return breakpoint;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h e c k A c t i o n P C L M a c r o C l a s h                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Macro identifier for Make Overlay run is already used within the   //
    // source print file.                                                 //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void CheckActionPCLMacroClash(
        int vInt,
        DataTable table)
    {
        PrnParseCommon.AddTextRow(
            PrnParseRowTypes.eType.MsgError,
            table,
            PrnParseConstants.eOvlShow.Terminate,
            string.Empty,
            "Error",
            string.Empty,
            "Macro identifier " + vInt +
            " is the specified overlay identifier");

        PrnParseCommon.AddTextRow(
            PrnParseRowTypes.eType.MsgError,
            table,
            PrnParseConstants.eOvlShow.Terminate,
            string.Empty,
            "Error",
            string.Empty,
           "Run aborted");

        MessageBox.Show(
            "The specified overlay identifier " + vInt +
            " is already used within the" +
            " source print file",
            "Overlay identifier conflict",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h e c k A c t i o n P C L X L A t t r                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // First pass: check what (if any) Make Overlay action is required    //
    // with current PCL XL Attribute data.                                //
    // Second pass: check what (if any) Make Overlay action is required   //
    // with current PCL XL Attribute.                                     //
    // If action indicated, set up appropriate break-point data.          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static bool CheckActionPCLXLAttr(
        bool firstPass,
        PrnParseConstants.eOvlAct attrOvlAct,
        PrnParseConstants.eOvlShow operOvlShow,
        int attrDataStart,
        int attrPos,
        long fileOffset,
        PrnParseLinkData linkData,
        DataTable table,
        PrnParseConstants.eOptOffsetFormats indxOffsetFormat)
    {
        bool breakpoint = false;

        PrnParseConstants.eOvlPos makeOvlPos = linkData.MakeOvlPos;
        PrnParseConstants.eOvlAct makeOvlAct = linkData.MakeOvlAct;

        int analysisLevel = linkData.AnalysisLevel;

        long seqBegin = fileOffset + attrDataStart;
        long seqEnd = fileOffset + attrPos + 2;    // what about 2-byte tags?

        if (makeOvlPos == PrnParseConstants.eOvlPos.WithinOtherPages)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Current position is WithinOtherPages.                      //
            // Don't want to report individual tag removal.               //
            //                                                            //
            //------------------------------------------------------------//

            linkData.MakeOvlShow = PrnParseConstants.eOvlShow.None;
        }
        else
        {
            //------------------------------------------------------------//
            //                                                            //
            // Current position is not WithinOtherPages.                  //
            // The only action expected for individual attributes is      //
            // Remove; otherwise use the action specified for the         //
            // associated Operator.                                       //
            //                                                            //
            //------------------------------------------------------------//

            if (attrOvlAct == PrnParseConstants.eOvlAct.Remove)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Remove;

                linkData.MakeOvlAct = attrOvlAct;
                linkData.MakeOvlSkipBegin = fileOffset + attrDataStart;
                linkData.MakeOvlSkipEnd = fileOffset + attrPos;
            }
            else
            {
                linkData.MakeOvlShow = operOvlShow;
            }
        }

        if (breakpoint)
        {
            if ((firstPass) && (linkData.MakeOvlEncapsulate))
            {
                long offset = linkData.MakeOvlOffset;
                long copyLen = seqBegin - offset;

                if (copyLen > 0)
                {
                    PrnParseCommon.AddDataRow(
                        PrnParseRowTypes.eType.MsgComment,
                        table,
                        PrnParseConstants.eOvlShow.Modify,
                        indxOffsetFormat,
                        (int)offset,
                        analysisLevel,
                        "PCLXL embedding",
                        "[" + copyLen + " bytes]",
                        "Encapsulated within ReadStream structure(s)");
                }
            }
            else
            {
                linkData.MakeOvlAct = attrOvlAct;
                linkData.MakeOvlSkipBegin = seqBegin;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
        }

        return breakpoint;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h e c k A c t i o n P C L X L O p e r                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // First pass: check what (if any) Make Overlay action is required    //
    // with current PCL XL operator attribute list.                       //
    // Second pass: check what (if any) Make Overlay action is required   //
    // with current PCL XL operator.                                      //
    // If action indicated, set up appropriate break-point data.          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static bool CheckActionPCLXLOper(
        bool firstPass,
        bool operHasAttrList,
        PrnParseConstants.eOvlAct operOvlAct,
        int operDataStart,
        int operPos,
        long fileOffset,
        PrnParseLinkData linkData,
        DataTable table,
        PrnParseConstants.eOptOffsetFormats indxOffsetFormat)
    {
        bool breakpoint = false;

        PrnParseConstants.eOvlPos makeOvlPos = linkData.MakeOvlPos;
        PrnParseConstants.eOvlAct makeOvlAct = linkData.MakeOvlAct;

        int analysisLevel = linkData.AnalysisLevel;

        long seqBegin = fileOffset + operDataStart;
        long seqEnd = fileOffset + operPos + 1;

        if (makeOvlPos == PrnParseConstants.eOvlPos.BeforeFirstPage)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Current position is BeforeFirstPage.                       //
            //                                                            //
            //------------------------------------------------------------//

            if (operOvlAct == PrnParseConstants.eOvlAct.Illegal)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Illegal;

                linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Terminate;
                linkData.MakeOvlSkipBegin = seqEnd - 1;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.PageBegin)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Remove;

                linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Remove;
                linkData.MakeOvlSkipBegin = seqBegin;
                linkData.MakeOvlSkipEnd = seqEnd;

                if (firstPass)
                    linkData.MakeOvlPos = PrnParseConstants.eOvlPos.WithinFirstPage;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.PageEnd)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Illegal;

                linkData.MakeOvlPos = PrnParseConstants.eOvlPos.BetweenPages;
                linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Remove;
                linkData.MakeOvlSkipBegin = seqBegin;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.Remove)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Remove;

                linkData.MakeOvlAct = operOvlAct;
                linkData.MakeOvlSkipBegin = seqBegin;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.Replace_0x77)
            {
                if (firstPass)
                {
                    breakpoint = false;
                    linkData.MakeOvlShow = PrnParseConstants.eOvlShow.None;
                }
                else
                {
                    breakpoint = true;
                    linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Remove;
                }

                linkData.MakeOvlAct = operOvlAct;
                linkData.MakeOvlSkipBegin = seqEnd - 1;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else
            {
                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.None;
            }
        }
        else if (makeOvlPos == PrnParseConstants.eOvlPos.WithinFirstPage)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Current position is WithinFirstPage.                       //
            //                                                            //
            //------------------------------------------------------------//

            if (operOvlAct == PrnParseConstants.eOvlAct.Illegal)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Illegal;

                linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Terminate;
                linkData.MakeOvlSkipBegin = seqEnd - 1;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.PageBegin)
            {
                breakpoint = true;

                if (firstPass)
                {
                    linkData.MakeOvlShow = PrnParseConstants.eOvlShow.None;
                    linkData.MakeOvlPos = PrnParseConstants.eOvlPos.WithinOtherPages;
                }
                else
                {
                    linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Remove;
                }

                linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Remove;
                linkData.MakeOvlSkipBegin = seqBegin;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.PageEnd)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Remove;

                if (!firstPass)
                    linkData.MakeOvlPos = PrnParseConstants.eOvlPos.BetweenPages;

                linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Remove;
                linkData.MakeOvlSkipBegin = seqBegin;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.Remove)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Remove;

                linkData.MakeOvlAct = operOvlAct;
                linkData.MakeOvlSkipBegin = seqBegin;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.Replace_0x77)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Illegal;

                linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Terminate;
                linkData.MakeOvlSkipBegin = seqEnd - 1;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else
            {
                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.None;
            }
        }
        else if (makeOvlPos == PrnParseConstants.eOvlPos.BetweenPages)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Current position is BetweenPages.                          //
            //                                                            //
            //------------------------------------------------------------//

            if (firstPass)
            {
                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.None;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.Illegal)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Illegal;

                linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Terminate;
                linkData.MakeOvlSkipBegin = seqEnd - 1;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.PageBegin)
            {
                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.None;

                linkData.MakeOvlPos = PrnParseConstants.eOvlPos.WithinOtherPages;
                linkData.MakeOvlSkipBegin = seqBegin;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.Remove)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Remove;

                linkData.MakeOvlAct = operOvlAct;
                linkData.MakeOvlSkipBegin = seqBegin;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.Replace_0x77)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Illegal;

                linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Terminate;
                linkData.MakeOvlSkipBegin = seqEnd - 1;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
        }
        else if (makeOvlPos == PrnParseConstants.eOvlPos.WithinOtherPages)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Current position is WithinOtherPages.                      //
            //                                                            //
            //------------------------------------------------------------//

            if (firstPass)
            {
                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.None;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.Illegal)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Illegal;

                linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Terminate;
                linkData.MakeOvlSkipBegin = seqEnd - 1;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.PageEnd)
            {
                long pageStart = linkData.MakeOvlSkipBegin;
                long pageLen = seqEnd - pageStart;

                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.None;
                linkData.MakeOvlPos = PrnParseConstants.eOvlPos.BetweenPages;

                linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Remove;
                linkData.MakeOvlSkipEnd = seqEnd;

                PrnParseCommon.AddDataRow(
                    PrnParseRowTypes.eType.MsgComment,
                    table,
                    PrnParseConstants.eOvlShow.Remove,
                    indxOffsetFormat,
                    (int)linkData.MakeOvlSkipBegin,
                    analysisLevel,
                    string.Empty,
                    "[" + pageLen + " bytes]",
                    "Subsequent page");
            }
        }
        else
        {
            //------------------------------------------------------------//
            //                                                            //
            // Current position is AfterPages.                            //
            // (not sure if valid/required).                              //
            //                                                            //
            //------------------------------------------------------------//

            if (firstPass)
            {
                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.None;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.Illegal)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Illegal;

                linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Terminate;
                linkData.MakeOvlSkipBegin = seqEnd - 1;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.PageBegin)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Remove;

                linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Remove;
                linkData.MakeOvlSkipBegin = seqBegin;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.PageEnd)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Remove;

                linkData.MakeOvlPos = PrnParseConstants.eOvlPos.BetweenPages;
                linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Remove;
                linkData.MakeOvlSkipBegin = seqBegin;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.Remove)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Remove;

                linkData.MakeOvlAct = operOvlAct;
                linkData.MakeOvlSkipBegin = seqBegin;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else if (operOvlAct == PrnParseConstants.eOvlAct.Replace_0x77)
            {
                breakpoint = true;

                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.Remove;

                linkData.MakeOvlAct = operOvlAct;
                linkData.MakeOvlSkipBegin = seqEnd - 1;
                linkData.MakeOvlSkipEnd = seqEnd;
            }
            else
            {
                linkData.MakeOvlShow = PrnParseConstants.eOvlShow.None;
            }
        }

        if (breakpoint)
        {
            if ((linkData.MakeOvlEncapsulate) &&
                ((firstPass) || (!operHasAttrList)))
            {
                long offset = linkData.MakeOvlOffset;
                long copyLen = seqBegin - offset;

                if (copyLen > 0)
                {
                    PrnParseCommon.AddDataRow(
                        PrnParseRowTypes.eType.MsgComment,
                        table,
                        PrnParseConstants.eOvlShow.Modify,
                        indxOffsetFormat,
                        (int)offset,
                        analysisLevel,
                        "PCLXL embedding",
                        "[" + copyLen + " bytes]",
                        "Encapsulated within ReadStream structure(s)");
                }
            }
        }

        return breakpoint;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h e c k A c t i o n P C L X L P u s h G S                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Check if Make Overlay action is required to insert command to save //
    // the current PCL XL Graphics State.                                 //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static bool CheckActionPCLXLPushGS(
        int hddrEnd,
        long fileOffset,
        PrnParseLinkData linkData,
        DataTable table,
        PrnParseConstants.eOptOffsetFormats indxOffsetFormat)
    {
        bool breakpoint = false;

        bool encapsulate = linkData.MakeOvlEncapsulate;

        PrnParseConstants.eOvlPos makeOvlPos = linkData.MakeOvlPos;
        PrnParseConstants.eOvlAct makeOvlAct = linkData.MakeOvlAct;

        int analysisLevel = linkData.AnalysisLevel;

        long seqBegin = fileOffset + hddrEnd;
        long seqEnd = fileOffset + hddrEnd;

        if (linkData.MakeOvlRestoreStateXL)
        {
            string descText;

            breakpoint = true;

            if (encapsulate)
            {
                long offset = linkData.MakeOvlOffset;
                long copyLen = seqBegin - offset;

                if (copyLen > 0)
                {
                    PrnParseCommon.AddDataRow(
                        PrnParseRowTypes.eType.MsgComment,
                        table,
                        PrnParseConstants.eOvlShow.Modify,
                        indxOffsetFormat,
                        (int)offset,
                        analysisLevel,
                        "PCLXL embedding",
                        "[" + copyLen + " bytes]",
                        "Encapsulated within ReadStream structure(s)");
                }
            }

            linkData.MakeOvlAct = PrnParseConstants.eOvlAct.PushGS;
            linkData.MakeOvlSkipBegin = seqEnd;
            linkData.MakeOvlSkipEnd = seqEnd;

            if (encapsulate)
                descText = "PushGS (encapsulated within" +
                           " ReadStream structure)";
            else
                descText = "PushGS";

            PrnParseCommon.AddDataRow(
                PrnParseRowTypes.eType.PCLXLOperator,
                table,
                PrnParseConstants.eOvlShow.Insert,
                indxOffsetFormat,
                (int)seqEnd,
                analysisLevel,
                "PCLXL Operator",
                "0x61",
                descText);
        }

        return breakpoint;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i n s e r t H e a d e r P C L                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Make Overlay insert header action.                                 //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void InsertHeaderPCL(PrnParsePCL parserPCL,
                                       DataTable table,
                                       BinaryWriter binWriter,
                                       bool encapsulate,
                                       bool restoreCursor,
                                       int macroId)
    {
        PrnParseConstants.eOffsetPosition crntPos;

        parserPCL.SetTable(table);

        crntPos = PrnParseConstants.eOffsetPosition.StartOfFile;

        if (encapsulate)
        {
            InsertSequencePCL(
                parserPCL,
                binWriter,
                0x26, // & //
                0x66, // f //
                0x59, // Y //
                macroId.ToString(),
                crntPos);

            crntPos = PrnParseConstants.eOffsetPosition.CrntPosition;

            InsertSequencePCL(
                parserPCL,
                binWriter,
                0x26, // & //
                0x66, // f //
                0x58, // X //
                "0",
                crntPos);
        }

        if (restoreCursor)
        {
            InsertSequencePCL(
                parserPCL,
                binWriter,
                0x26, // & //
                0x66, // f //
                0x53, // S //
                "0",
                crntPos);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i n s e r t S e q u e n c e P C L                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Make Overlay insert sequence.                                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void InsertSequencePCL(
        PrnParsePCL parserPCL,
        BinaryWriter binWriter,
        byte iChar,
        byte gChar,
        byte tChar,
        string value,
        PrnParseConstants.eOffsetPosition position)
    {
        PrnParseConstants.eActPCL actType =
            PrnParseConstants.eActPCL.None;

        PrnParseConstants.eOvlAct makeOvlAct =
            PrnParseConstants.eOvlAct.None;

        bool seqKnown;

        bool optObsolete = false,
                optResetHPGL2 = false,
                optNilGChar = false,
                optNilValue = false,
                optValueIsLen = false,
                optDisplayHexVal = false;

        int prefixLen = 0;

        string descComplex = string.Empty;

        string seq = string.Empty;

        //----------------------------------------------------------------//
        //                                                                //
        // Check sequence against entries in standard Complex             //
        // (Parameterised) Sequence table.                                //
        //                                                                //
        //----------------------------------------------------------------//

        string typeText = "PCL Parameterised";

        short vInt16 = 0;

        bool vCheck = short.TryParse(value, out vInt16);

        seqKnown = PCLComplexSeqs.CheckComplexSeq(
                        0,
                        iChar,
                        gChar,
                        tChar,
                        vCheck,
                        vInt16,
                        ref optObsolete,
                        ref optResetHPGL2,
                        ref optNilGChar,
                        ref optNilValue,
                        ref optValueIsLen,
                        ref optDisplayHexVal,
                        ref actType,
                        ref makeOvlAct,
                        ref descComplex);

        //----------------------------------------------------------------//
        //                                                                //
        // Display details of sequence in report.                         //
        //                                                                //
        //----------------------------------------------------------------//

        byte[] seqBuf = new byte[4];

        seqBuf[0] = 0x1b;
        seqBuf[1] = iChar;
        seqBuf[2] = gChar;
        seqBuf[3] = tChar;

        if (gChar == 0x20)
        {
            prefixLen = 1;

            seq = _ascii.GetString(seqBuf, 1, 1) + value +
                  _ascii.GetString(seqBuf, 3, 1);
        }
        else
        {
            prefixLen = 2;

            seq = _ascii.GetString(seqBuf, 1, 2) + value +
                  _ascii.GetString(seqBuf, 3, 1);
        }

        parserPCL.ParseSequenceComplexDisplay(
            (int)position,
            prefixLen,
            true,
            PrnParseConstants.eOvlShow.Insert,
            typeText,
            seq.ToString(),
            descComplex,
            value);

        //----------------------------------------------------------------//
        //                                                                //
        // Write sequence to overlay file.                                //
        //                                                                //
        //----------------------------------------------------------------//
        try
        {
            int len;

            binWriter.Write(seqBuf, 0, prefixLen + 1);

            len = _ascii.GetByteCount(value);

            binWriter.Write(_ascii.GetBytes(value), 0, len);

            binWriter.Write(seqBuf, 3, 1);
        }

        catch (IOException e)
        {
            MessageBox.Show("' IO Exception:\r\n" +
                             e.Message,
                             "Inserting sequences",
                             MessageBoxButton.OK,
                             MessageBoxImage.Error);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i n s e r t T r a i l e r P C L                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Make Overlay insert trailer action.                                //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void InsertTrailerPCL(PrnParsePCL parserPCL,
                                        DataTable table,
                                        BinaryWriter binWriter,
                                        bool encapsulate,
                                        bool restoreCursor)
    {
        parserPCL.SetTable(table);

        PrnParseConstants.eOffsetPosition crntPos = PrnParseConstants.eOffsetPosition.EndOfFile;

        if (restoreCursor)
        {
            InsertSequencePCL(
                parserPCL,
                binWriter,
                0x26, // & //
                0x66, // f //
                0x53, // S //
                "1",
                crntPos);

            crntPos = PrnParseConstants.eOffsetPosition.CrntPosition;
        }

        if (encapsulate)
        {
            InsertSequencePCL(
                parserPCL,
                binWriter,
                0x26, // & //
                0x66, // f //
                0x58, // X //
                "1",
                crntPos);
        }
    }
}