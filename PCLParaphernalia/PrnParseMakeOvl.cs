﻿using System.Data;
using System.IO;
using System.Text;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class provides common routines associated with the Make Overlay tool.</para>
    /// <para>© Chris Hutchinson 2012</para>
    ///
    /// </summary>
    internal static class PrnParseMakeOvl
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

            PrnParseConstants.OvlAct action = linkData.MakeOvlAct;

            //----------------------------------------------------------------//
            //                                                                //
            // Check whether file update required.                            //
            //                                                                //
            //----------------------------------------------------------------//

            if (action == PrnParseConstants.OvlAct.Terminate)
            {
                terminate = true;
            }
            else if (action == PrnParseConstants.OvlAct.EndOfFile)
            {
                update = true;
            }
            else if ((action == PrnParseConstants.OvlAct.Download)
                                          ||
                     (action == PrnParseConstants.OvlAct.DownloadDelete)
                                          ||
                     (action == PrnParseConstants.OvlAct.IdFont)
                                          ||
                     (action == PrnParseConstants.OvlAct.IdPalette)
                                          ||
                     (action == PrnParseConstants.OvlAct.IdPattern)
                                          ||
                     (action == PrnParseConstants.OvlAct.IdSymSet)
                                          ||
                     (action == PrnParseConstants.OvlAct.IdMacro))
            {
                //------------------------------------------------------------//
                //                                                            //
                // No action - actions defined for future enhancement         //
                //                                                            //
                //------------------------------------------------------------//
            }
            else if (action != PrnParseConstants.OvlAct.None)
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

                long skipBegin,
                      skipEnd,
                      comboStart = 0,
                      crntPos,
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

                crntPos = linkData.MakeOvlOffset;
                skipBegin = linkData.MakeOvlSkipBegin;
                skipEnd = linkData.MakeOvlSkipEnd;

                if (skipBegin >= 0)
                {
                    if (ovlXL)
                    {
                        syncLen = skipBegin - crntPos;
                        fragLen = 0;
                    }
                    else if (comboSeq && (!comboFirst))
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
                else if (action == PrnParseConstants.OvlAct.EndOfFile)
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
                        MessageBox.Show($"IO Exception:\r\n{e.Message}\r\n\r\nSeeking to offset {crntPos}.",
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
                            MessageBox.Show($"IO Exception:\r\n{e.Message}\r\n\r\nCopying " + readLen + " bytes.",
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
                            MessageBox.Show($"IO Exception:\r\n{e.Message}\r\n\r\nCopying " + readLen + " bytes.",
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

                if (action == PrnParseConstants.OvlAct.PageBoundary)
                {
                    linkData.MakeOvlSkipBegin = skipEnd;
                }
                else if (action == PrnParseConstants.OvlAct.PageBegin)
                {
                    // keep current SkipBegin position
                }
                else if (action == PrnParseConstants.OvlAct.PageEnd)
                {
                    // keep current SkipBegin position
                }
                else if (action == PrnParseConstants.OvlAct.Reset)
                {
                    // keep current SkipBegin position
                }
                else
                {
                    linkData.MakeOvlSkipBegin =
                        (int)PrnParseConstants.OffsetPosition.Unknown;
                }

                if (action == PrnParseConstants.OvlAct.Replace_0x77)
                {
                    PCLXLWriter.WriteOperator(binWriter,
                                               PCLXLOperators.Tag.SetPageScale,
                                               encapsulate);
                }
                else if (action == PrnParseConstants.OvlAct.PushGS)
                {
                    PCLXLWriter.WriteOperator(binWriter,
                                               PCLXLOperators.Tag.PushGS,
                                               encapsulate);
                }

                action = PrnParseConstants.OvlAct.None;
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
            PrnParseConstants.OptOffsetFormats indxOffsetFormat)
        {
            bool breakpoint = false;
            bool comboModified;

            int analysisLevel;

            long seqBegin,
                  seqEnd;

            PrnParseConstants.OvlPos makeOvlPosCrnt = linkData.MakeOvlPos;
            PrnParseConstants.OvlAct makeOvlActCrnt = linkData.MakeOvlAct;
            PrnParseConstants.OvlShow makeOvlShowCrnt = linkData.MakeOvlShow;

            PrnParseConstants.OvlPos makeOvlPosNew = linkData.MakeOvlPos;
            PrnParseConstants.OvlAct makeOvlActNew = linkData.MakeOvlAct;
            PrnParseConstants.OvlShow makeOvlShowNew = linkData.MakeOvlShow;

            analysisLevel = linkData.AnalysisLevel;

            comboModified = linkData.PclComboModified;

            seqBegin = fileOffset + seqStart;
            seqEnd = seqBegin + fragLen;

            if (makeOvlPosCrnt == PrnParseConstants.OvlPos.BeforeFirstPage)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Current position is BeforeFirstPage                        //
                //                                                            //
                //------------------------------------------------------------//

                if ((makeOvlActCrnt == PrnParseConstants.OvlAct.IdMacro) &&
                         (vInt == linkData.MakeOvlMacroId))
                {
                    breakpoint = true;

                    makeOvlActNew = PrnParseConstants.OvlAct.Terminate;
                    makeOvlShowNew = PrnParseConstants.OvlShow.Terminate;

                    CheckActionPCLMacroClash(vInt, table);
                }
                else if (makeOvlActCrnt == PrnParseConstants.OvlAct.PageChange)
                {
                    breakpoint = true;

                    makeOvlActNew = PrnParseConstants.OvlAct.Remove;
                    makeOvlShowNew = PrnParseConstants.OvlShow.Remove;
                }
                else if (makeOvlActCrnt == PrnParseConstants.OvlAct.PageMark)
                {
                    makeOvlPosNew = PrnParseConstants.OvlPos.WithinFirstPage;
                }
                else if (makeOvlActCrnt == PrnParseConstants.OvlAct.Remove)
                {
                    breakpoint = true;
                    makeOvlShowNew = PrnParseConstants.OvlShow.Remove;
                }
                else if (makeOvlActCrnt == PrnParseConstants.OvlAct.Reset)
                {
                    breakpoint = true;

                    makeOvlActNew = PrnParseConstants.OvlAct.Remove;
                    makeOvlShowNew = PrnParseConstants.OvlShow.Remove;
                }
                else if (comboSeq && comboModified)
                {
                    breakpoint = true;

                    makeOvlActNew = PrnParseConstants.OvlAct.Adjust;
                    makeOvlShowNew = PrnParseConstants.OvlShow.None;
                }
                else
                {
                    makeOvlShowNew = PrnParseConstants.OvlShow.None;
                }
            }
            else if (makeOvlPosCrnt == PrnParseConstants.OvlPos.WithinFirstPage)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Current position is WithinFirstPage                        //
                //                                                            //
                //------------------------------------------------------------//

                if ((makeOvlActCrnt == PrnParseConstants.OvlAct.IdMacro) &&
                         (vInt == linkData.MakeOvlMacroId))
                {
                    breakpoint = true;

                    makeOvlActNew = PrnParseConstants.OvlAct.Terminate;
                    makeOvlShowNew = PrnParseConstants.OvlShow.Terminate;

                    CheckActionPCLMacroClash(vInt, table);
                }
                else if (makeOvlActCrnt == PrnParseConstants.OvlAct.PageChange)
                {
                    breakpoint = true;

                    makeOvlActNew = PrnParseConstants.OvlAct.PageBegin;
                    makeOvlPosNew = PrnParseConstants.OvlPos.WithinOtherPages;
                    makeOvlShowNew = PrnParseConstants.OvlShow.None;
                }
                else if (makeOvlActCrnt == PrnParseConstants.OvlAct.Remove)
                {
                    breakpoint = true;
                    makeOvlShowNew = PrnParseConstants.OvlShow.Remove;
                }
                else if (makeOvlActCrnt == PrnParseConstants.OvlAct.Reset)
                {
                    breakpoint = true;

                    makeOvlPosNew = PrnParseConstants.OvlPos.AfterPages;
                    makeOvlShowNew = PrnParseConstants.OvlShow.Remove;
                }
                else if (comboSeq && comboModified)
                {
                    breakpoint = true;

                    makeOvlActNew = PrnParseConstants.OvlAct.Adjust;
                    makeOvlShowNew = PrnParseConstants.OvlShow.None;
                }
                else
                {
                    makeOvlShowNew = PrnParseConstants.OvlShow.None;
                }
            }
            else if (makeOvlPosCrnt == PrnParseConstants.OvlPos.WithinOtherPages)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Current position is WithinOtherPages                       //
                //                                                            //
                //------------------------------------------------------------//

                if (makeOvlActCrnt == PrnParseConstants.OvlAct.PageChange)
                {
                    breakpoint = true;

                    makeOvlActNew = PrnParseConstants.OvlAct.PageBoundary;
                    makeOvlShowNew = PrnParseConstants.OvlShow.None;

                    if (linkData.MakeOvlSkipBegin > 0)
                    {
                        long pageStart = linkData.MakeOvlSkipBegin;
                        long pageLen = seqBegin - pageStart;

                        PrnParseCommon.AddDataRow(
                            PrnParseRowTypes.Type.MsgComment,
                            table,
                            PrnParseConstants.OvlShow.Remove,
                            indxOffsetFormat,
                            (int)pageStart,
                            analysisLevel,
                            string.Empty,
                            "[" + pageLen + " bytes]",
                            "Subsequent page");
                    }
                }
                else if (makeOvlActCrnt == PrnParseConstants.OvlAct.Reset)
                {
                    breakpoint = true;

                    makeOvlPosNew = PrnParseConstants.OvlPos.AfterPages;
                    makeOvlShowNew = PrnParseConstants.OvlShow.Remove;

                    if (linkData.MakeOvlSkipBegin > 0)
                    {
                        long pageStart = linkData.MakeOvlSkipBegin;
                        long pageLen = seqBegin - pageStart;

                        PrnParseCommon.AddDataRow(
                            PrnParseRowTypes.Type.MsgComment,
                            table,
                            PrnParseConstants.OvlShow.Remove,
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

                if (makeOvlActCrnt == PrnParseConstants.OvlAct.Reset)
                {
                    breakpoint = true;

                    makeOvlActNew = PrnParseConstants.OvlAct.Remove;
                    makeOvlShowNew = PrnParseConstants.OvlShow.Remove;
                }
                else if (makeOvlActCrnt != PrnParseConstants.OvlAct.None)
                {
                    breakpoint = true;

                    makeOvlActNew = PrnParseConstants.OvlAct.Terminate;
                    makeOvlShowNew = PrnParseConstants.OvlShow.Terminate;
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
                PrnParseConstants.ContType contType;
                byte iChar = 0x20,
                     gChar = 0x20;

                int prefixLen = 0;

                linkData.GetPrefixData(ref prefixLen, ref iChar, ref gChar);

                if (comboSeq)
                    comboModified = true;

                if (seqComplete)
                    contType = PrnParseConstants.ContType.Reset;
                else
                    contType = PrnParseConstants.ContType.PCLComplex;

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

                if (makeOvlActNew == PrnParseConstants.OvlAct.PageBegin)
                {
                    linkData.MakeOvlSkipBegin = seqBegin;
                    linkData.MakeOvlSkipEnd = seqBegin;
                }
                else if (makeOvlActNew == PrnParseConstants.OvlAct.PageBoundary)
                {
                    linkData.MakeOvlSkipEnd = seqBegin;
                }
                else if (makeOvlActNew == PrnParseConstants.OvlAct.PageEnd)
                {
                    linkData.MakeOvlSkipEnd = seqBegin;
                }
                else if (makeOvlActNew == PrnParseConstants.OvlAct.Adjust)
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
                PrnParseRowTypes.Type.MsgError,
                table,
                PrnParseConstants.OvlShow.Terminate,
                string.Empty,
                "Error",
                string.Empty,
                "Macro identifier " + vInt +
                " is the specified overlay identifier");

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.Type.MsgError,
                table,
                PrnParseConstants.OvlShow.Terminate,
                string.Empty,
                "Error",
                string.Empty,
               "Run aborted");

            MessageBox.Show($"The specified overlay identifier {vInt} is already used within the source print file.",
                "Overlay Identifier Conflict",
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
            PrnParseConstants.OvlAct attrOvlAct,
            PrnParseConstants.OvlShow operOvlShow,
            int attrDataStart,
            int attrPos,
            long fileOffset,
            PrnParseLinkData linkData,
            DataTable table,
            PrnParseConstants.OptOffsetFormats indxOffsetFormat)
        {
            bool breakpoint = false;

            int analysisLevel;

            long seqBegin,
                  seqEnd;

            PrnParseConstants.OvlPos makeOvlPos = linkData.MakeOvlPos;
            PrnParseConstants.OvlAct makeOvlAct = linkData.MakeOvlAct;

            analysisLevel = linkData.AnalysisLevel;

            seqBegin = fileOffset + attrDataStart;
            seqEnd = fileOffset + attrPos + 2;    // what about 2-byte tags?

            if (makeOvlPos == PrnParseConstants.OvlPos.WithinOtherPages)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Current position is WithinOtherPages.                      //
                // Don't want to report individual tag removal.               //
                //                                                            //
                //------------------------------------------------------------//

                linkData.MakeOvlShow = PrnParseConstants.OvlShow.None;
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

                if (attrOvlAct == PrnParseConstants.OvlAct.Remove)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Remove;

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
                if (firstPass && linkData.MakeOvlEncapsulate)
                {
                    long offset = linkData.MakeOvlOffset;
                    long copyLen = seqBegin - offset;

                    if (copyLen > 0)
                    {
                        PrnParseCommon.AddDataRow(
                            PrnParseRowTypes.Type.MsgComment,
                            table,
                            PrnParseConstants.OvlShow.Modify,
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
            PrnParseConstants.OvlAct operOvlAct,
            int operDataStart,
            int operPos,
            long fileOffset,
            PrnParseLinkData linkData,
            DataTable table,
            PrnParseConstants.OptOffsetFormats indxOffsetFormat)
        {
            bool breakpoint = false;

            int analysisLevel;

            long seqBegin,
                  seqEnd;

            PrnParseConstants.OvlPos makeOvlPos = linkData.MakeOvlPos;
            PrnParseConstants.OvlAct makeOvlAct = linkData.MakeOvlAct;

            analysisLevel = linkData.AnalysisLevel;

            seqBegin = fileOffset + operDataStart;
            seqEnd = fileOffset + operPos + 1;

            if (makeOvlPos == PrnParseConstants.OvlPos.BeforeFirstPage)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Current position is BeforeFirstPage.                       //
                //                                                            //
                //------------------------------------------------------------//

                if (operOvlAct == PrnParseConstants.OvlAct.Illegal)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Illegal;

                    linkData.MakeOvlAct = PrnParseConstants.OvlAct.Terminate;
                    linkData.MakeOvlSkipBegin = seqEnd - 1;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.PageBegin)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Remove;

                    linkData.MakeOvlAct = PrnParseConstants.OvlAct.Remove;
                    linkData.MakeOvlSkipBegin = seqBegin;
                    linkData.MakeOvlSkipEnd = seqEnd;

                    if (firstPass)
                        linkData.MakeOvlPos = PrnParseConstants.OvlPos.WithinFirstPage;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.PageEnd)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Illegal;

                    linkData.MakeOvlPos = PrnParseConstants.OvlPos.BetweenPages;
                    linkData.MakeOvlAct = PrnParseConstants.OvlAct.Remove;
                    linkData.MakeOvlSkipBegin = seqBegin;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.Remove)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Remove;

                    linkData.MakeOvlAct = operOvlAct;
                    linkData.MakeOvlSkipBegin = seqBegin;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.Replace_0x77)
                {
                    if (firstPass)
                    {
                        breakpoint = false;
                        linkData.MakeOvlShow = PrnParseConstants.OvlShow.None;
                    }
                    else
                    {
                        breakpoint = true;
                        linkData.MakeOvlShow = PrnParseConstants.OvlShow.Remove;
                    }

                    linkData.MakeOvlAct = operOvlAct;
                    linkData.MakeOvlSkipBegin = seqEnd - 1;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else
                {
                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.None;
                }
            }
            else if (makeOvlPos == PrnParseConstants.OvlPos.WithinFirstPage)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Current position is WithinFirstPage.                       //
                //                                                            //
                //------------------------------------------------------------//

                if (operOvlAct == PrnParseConstants.OvlAct.Illegal)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Illegal;

                    linkData.MakeOvlAct = PrnParseConstants.OvlAct.Terminate;
                    linkData.MakeOvlSkipBegin = seqEnd - 1;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.PageBegin)
                {
                    breakpoint = true;

                    if (firstPass)
                    {
                        linkData.MakeOvlShow = PrnParseConstants.OvlShow.None;
                        linkData.MakeOvlPos = PrnParseConstants.OvlPos.WithinOtherPages;
                    }
                    else
                    {
                        linkData.MakeOvlShow = PrnParseConstants.OvlShow.Remove;
                    }

                    linkData.MakeOvlAct = PrnParseConstants.OvlAct.Remove;
                    linkData.MakeOvlSkipBegin = seqBegin;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.PageEnd)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Remove;

                    if (!firstPass)
                        linkData.MakeOvlPos = PrnParseConstants.OvlPos.BetweenPages;

                    linkData.MakeOvlAct = PrnParseConstants.OvlAct.Remove;
                    linkData.MakeOvlSkipBegin = seqBegin;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.Remove)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Remove;

                    linkData.MakeOvlAct = operOvlAct;
                    linkData.MakeOvlSkipBegin = seqBegin;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.Replace_0x77)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Illegal;

                    linkData.MakeOvlAct = PrnParseConstants.OvlAct.Terminate;
                    linkData.MakeOvlSkipBegin = seqEnd - 1;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else
                {
                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.None;
                }
            }
            else if (makeOvlPos == PrnParseConstants.OvlPos.BetweenPages)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Current position is BetweenPages.                          //
                //                                                            //
                //------------------------------------------------------------//

                if (firstPass)
                {
                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.None;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.Illegal)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Illegal;

                    linkData.MakeOvlAct = PrnParseConstants.OvlAct.Terminate;
                    linkData.MakeOvlSkipBegin = seqEnd - 1;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.PageBegin)
                {
                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.None;

                    linkData.MakeOvlPos = PrnParseConstants.OvlPos.WithinOtherPages;
                    linkData.MakeOvlSkipBegin = seqBegin;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.Remove)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Remove;

                    linkData.MakeOvlAct = operOvlAct;
                    linkData.MakeOvlSkipBegin = seqBegin;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.Replace_0x77)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Illegal;

                    linkData.MakeOvlAct = PrnParseConstants.OvlAct.Terminate;
                    linkData.MakeOvlSkipBegin = seqEnd - 1;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
            }
            else if (makeOvlPos == PrnParseConstants.OvlPos.WithinOtherPages)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Current position is WithinOtherPages.                      //
                //                                                            //
                //------------------------------------------------------------//

                if (firstPass)
                {
                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.None;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.Illegal)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Illegal;

                    linkData.MakeOvlAct = PrnParseConstants.OvlAct.Terminate;
                    linkData.MakeOvlSkipBegin = seqEnd - 1;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.PageEnd)
                {
                    long pageStart = linkData.MakeOvlSkipBegin;
                    long pageLen = seqEnd - pageStart;

                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.None;
                    linkData.MakeOvlPos = PrnParseConstants.OvlPos.BetweenPages;

                    linkData.MakeOvlAct = PrnParseConstants.OvlAct.Remove;
                    linkData.MakeOvlSkipEnd = seqEnd;

                    PrnParseCommon.AddDataRow(
                        PrnParseRowTypes.Type.MsgComment,
                        table,
                        PrnParseConstants.OvlShow.Remove,
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
                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.None;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.Illegal)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Illegal;

                    linkData.MakeOvlAct = PrnParseConstants.OvlAct.Terminate;
                    linkData.MakeOvlSkipBegin = seqEnd - 1;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.PageBegin)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Remove;

                    linkData.MakeOvlAct = PrnParseConstants.OvlAct.Remove;
                    linkData.MakeOvlSkipBegin = seqBegin;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.PageEnd)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Remove;

                    linkData.MakeOvlPos = PrnParseConstants.OvlPos.BetweenPages;
                    linkData.MakeOvlAct = PrnParseConstants.OvlAct.Remove;
                    linkData.MakeOvlSkipBegin = seqBegin;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.Remove)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Remove;

                    linkData.MakeOvlAct = operOvlAct;
                    linkData.MakeOvlSkipBegin = seqBegin;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else if (operOvlAct == PrnParseConstants.OvlAct.Replace_0x77)
                {
                    breakpoint = true;

                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.Remove;

                    linkData.MakeOvlAct = operOvlAct;
                    linkData.MakeOvlSkipBegin = seqEnd - 1;
                    linkData.MakeOvlSkipEnd = seqEnd;
                }
                else
                {
                    linkData.MakeOvlShow = PrnParseConstants.OvlShow.None;
                }
            }

            if (breakpoint)
            {
                if (linkData.MakeOvlEncapsulate &&
                    (firstPass || (!operHasAttrList)))
                {
                    long offset = linkData.MakeOvlOffset;
                    long copyLen = seqBegin - offset;

                    if (copyLen > 0)
                    {
                        PrnParseCommon.AddDataRow(
                            PrnParseRowTypes.Type.MsgComment,
                            table,
                            PrnParseConstants.OvlShow.Modify,
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
            PrnParseConstants.OptOffsetFormats indxOffsetFormat)
        {
            bool breakpoint = false;

            bool encapsulate = linkData.MakeOvlEncapsulate;

            int analysisLevel;

            long seqBegin,
                  seqEnd;

            PrnParseConstants.OvlPos makeOvlPos = linkData.MakeOvlPos;
            PrnParseConstants.OvlAct makeOvlAct = linkData.MakeOvlAct;

            analysisLevel = linkData.AnalysisLevel;

            seqBegin = fileOffset + hddrEnd;
            seqEnd = fileOffset + hddrEnd;

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
                            PrnParseRowTypes.Type.MsgComment,
                            table,
                            PrnParseConstants.OvlShow.Modify,
                            indxOffsetFormat,
                            (int)offset,
                            analysisLevel,
                            "PCLXL embedding",
                            "[" + copyLen + " bytes]",
                            "Encapsulated within ReadStream structure(s)");
                    }
                }

                linkData.MakeOvlAct = PrnParseConstants.OvlAct.PushGS;
                linkData.MakeOvlSkipBegin = seqEnd;
                linkData.MakeOvlSkipEnd = seqEnd;

                if (encapsulate)
                {
                    descText = "PushGS (encapsulated within" +
                                               " ReadStream structure)";
                }
                else
                {
                    descText = "PushGS";
                }

                PrnParseCommon.AddDataRow(
                    PrnParseRowTypes.Type.PCLXLOperator,
                    table,
                    PrnParseConstants.OvlShow.Insert,
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
            PrnParseConstants.OffsetPosition crntPos;

            parserPCL.SetTable(table);

            crntPos = PrnParseConstants.OffsetPosition.StartOfFile;

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

                crntPos = PrnParseConstants.OffsetPosition.CrntPosition;

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
            PrnParseConstants.OffsetPosition position)
        {
            PrnParseConstants.ActPCL actType = PrnParseConstants.ActPCL.None;

            PrnParseConstants.OvlAct makeOvlAct = PrnParseConstants.OvlAct.None;

            bool seqKnown;

            bool optObsolete = false,
                    optResetHPGL2 = false,
                    optNilGChar = false,
                    optNilValue = false,
                    optValueIsLen = false,
                    optDisplayHexVal = false;

            short vInt16;
            string descComplex = string.Empty;

            //----------------------------------------------------------------//
            //                                                                //
            // Check sequence against entries in standard Complex             //
            // (Parameterised) Sequence table.                                //
            //                                                                //
            //----------------------------------------------------------------//

            string typeText = "PCL Parameterised";
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

            int prefixLen;
            string seq;
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
                PrnParseConstants.OvlShow.Insert,
                typeText,
                seq,
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
                MessageBox.Show("IO Exception:\r\n" + e.Message,
                                "Inserting Sequences",
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
            PrnParseConstants.OffsetPosition crntPos;

            parserPCL.SetTable(table);

            crntPos = PrnParseConstants.OffsetPosition.EndOfFile;

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

                crntPos = PrnParseConstants.OffsetPosition.CrntPosition;
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
}