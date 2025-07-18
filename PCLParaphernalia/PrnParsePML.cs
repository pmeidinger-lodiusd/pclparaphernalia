﻿using System;
using System.Data;
using System.Text;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines functions to parse PML commands.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    class PrnParsePML
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const int _decodeAreaMax = 48;
        const int _decodeSliceMax = 4;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private DataTable _table;

        private byte[] _buf;

        private int _fileOffset;

        private int _analysisLevel;

        private PrnParseConstants.eOptOffsetFormats _indxOffsetFormat;

        private bool _verboseMode;

        private PrnParseConstants.eOptCharSetSubActs _indxCharSetSubAct;
        private PrnParseConstants.eOptCharSets _indxCharSetName;
        private int _valCharSetSubCode;

        private readonly ASCIIEncoding _ascii = new ASCIIEncoding();

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P r n P a r s e P M L                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParsePML()
        {
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s P M L A S C I I H e x                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process hexadecimal PML sequence, presented in ASCII               //
        // representation (as is the case with PML embedded within PJL DMINFO //
        // and DMCMD statements).                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool processPMLASCIIHex(byte[] buf,
                                           int fileOffset,
                                           int seqDataLen,
                                           int bufOffset,
                                           PrnParseLinkData linkData,
                                           PrnParseOptions options,
                                           DataTable table)
        {
            const byte digitZero = 0x30;
            const byte digitNine = 0x39;
            const byte upperA = 0x41;
            const byte upperF = 0x46;
            const byte lowerA = 0x61;
            const byte lowerF = 0x66;

            bool invalidSeqFound = false;

            int seqLen = 0,
                  offset;

            //----------------------------------------------------------------//

            _table = table;
            _fileOffset = fileOffset;

            _analysisLevel = linkData.AnalysisLevel + 1;    // always embedded

            _verboseMode = options.FlagPMLVerbose;

            _indxOffsetFormat = options.IndxGenOffsetFormat;

            options.GetOptCharSet(ref _indxCharSetName,
                                   ref _indxCharSetSubAct,
                                   ref _valCharSetSubCode);

            seqLen = seqDataLen / 2;

            //----------------------------------------------------------------//

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgComment,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                ">>>>>>>>>>>>>>>>>>>>",
                string.Empty,
                ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgComment,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Comment",
                string.Empty,
                "Start analysis of embedded PML string");

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgComment,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Comment",
                string.Empty,
                "of size " + seqDataLen + " ASCIIHEX characters (" +
                             seqLen + " bytes)");

            //----------------------------------------------------------------//

            if ((seqDataLen == 0) || (seqDataLen % 2 != 0))
            {
                invalidSeqFound = true;

                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.eType.MsgWarning,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    string.Empty,
                    "*** Warning ***",
                    string.Empty,
                    "ASCII HEX not multiple of 2 bytes");
            }
            else
            {
                byte crntByte;

                int hexVal = 0;

                _buf = new byte[seqLen];

                offset = bufOffset;

                for (int i = 0; (i < seqLen) && (!invalidSeqFound); i++)
                {
                    crntByte = buf[offset];

                    if ((crntByte >= digitZero) && (crntByte <= digitNine))
                        hexVal = (crntByte - digitZero);
                    else if ((crntByte >= upperA) && (crntByte <= upperF))
                        hexVal = (crntByte - upperA + 10);
                    else if ((crntByte >= lowerA) && (crntByte <= lowerF))
                        hexVal = (crntByte - lowerA + 10);
                    else
                        invalidSeqFound = true;

                    if (!invalidSeqFound)
                    {
                        hexVal *= 16;

                        crntByte = buf[offset + 1];

                        if ((crntByte >= digitZero) && (crntByte <= digitNine))
                            hexVal += (crntByte - digitZero);
                        else if ((crntByte >= upperA) && (crntByte <= upperF))
                            hexVal += (crntByte - upperA + 10);
                        else if ((crntByte >= lowerA) && (crntByte <= lowerF))
                            hexVal += (crntByte - lowerA + 10);
                        else
                            invalidSeqFound = true;
                    }

                    if (!invalidSeqFound)
                    {
                        _buf[i] = (byte)hexVal;
                        offset += 2;
                    }
                }

                if (invalidSeqFound)
                {
                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.eType.MsgWarning,
                        _table,
                        PrnParseConstants.eOvlShow.None,
                        string.Empty,
                        "*** Warning ***",
                        string.Empty,
                        "Not ASCII HEX");
                }
                else
                {
                    invalidSeqFound = processPMLSeq(seqLen, 0, false);

                }
            }

            //----------------------------------------------------------------//

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgComment,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Comment",
                string.Empty,
                "End analysis of embedded PML string");

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgComment,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Comment",
                string.Empty,
                "of size " + seqDataLen + " ASCIIHEX characters (" +
                             seqLen + " bytes)");

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgComment,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "<<<<<<<<<<<<<<<<<<<<",
                string.Empty,
                "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

            return invalidSeqFound;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s P M L E m b e d d e d                                //
        //                                                                    //
        // Process hexadecimal PML sequence, presented in hexadecimal         //
        // representation (as is the case with PML embedded within PCL I/O    //
        // Configuration sequences).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool processPMLEmbedded(byte[] buf,
                                          int fileOffset,
                                          int dataLen,
                                          int bufOffset,
                                          PrnParseLinkData linkData,
                                          PrnParseOptions options,
                                          DataTable table)
        {
            bool seqOK = true;

            _table = table;
            _buf = buf;
            _fileOffset = fileOffset;

            _analysisLevel = linkData.AnalysisLevel + 1;    // always embedded

            _verboseMode = options.FlagPMLVerbose;

            //----------------------------------------------------------------//

            _indxOffsetFormat = options.IndxGenOffsetFormat;

            options.GetOptCharSet(ref _indxCharSetName,
                                   ref _indxCharSetSubAct,
                                   ref _valCharSetSubCode);

            //----------------------------------------------------------------//

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgComment,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                ">>>>>>>>>>>>>>>>>>>>",
                string.Empty,
                ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgComment,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Comment",
                string.Empty,
                "Start analysis of embedded PML string");

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgComment,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Comment",
                string.Empty,
                "of size " + dataLen + " bytes");

            //----------------------------------------------------------------//

            seqOK = !processPMLSeq(dataLen, bufOffset, true);

            //----------------------------------------------------------------//

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgComment,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Comment",
                string.Empty,
                "End analysis of embedded PML string");

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgComment,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Comment",
                string.Empty,
                "of size " + dataLen + " bytes");

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgComment,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "<<<<<<<<<<<<<<<<<<<<",
                string.Empty,
                "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

            return seqOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s P M L S e q                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process hexadecimal PML sequence.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool processPMLSeq(int seqLen,
                                       int seqOffset,
                                       bool hddrExpected)
        {
            const byte cPMLReplyTypeMin = 0x80;
            //  const Byte cPMLErrorTypeMin = 0x80;

            bool invalidSeqFound = false;

            int partOffset,
                  partLen,
                  dataLen,
                  remLen;

            byte crntByte,
                 dataType;

            string tagDesc = string.Empty;

            invalidSeqFound = false;
            partOffset = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // First four bytes may be the 'PML ' introducer.                 //
            //                                                                //
            //----------------------------------------------------------------//

            if (hddrExpected)
            {
                partLen = 4;

                showElement(PrnParseConstants.ePMLSeqType.Hddr,
                             PMLDataTypes.eTag.String,
                             seqOffset + partOffset,
                             partLen,
                             "PML Intro",
                             string.Empty);

                partOffset += partLen;
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Next character should be an action (Request or Reply)          //
            // identifier.                                                    //
            //                                                                //
            //----------------------------------------------------------------//

            partLen = 1;

            crntByte = _buf[seqOffset + partOffset];

            PMLActions.CheckTag(crntByte, ref tagDesc);

            if (crntByte < cPMLReplyTypeMin)
            {
                //------------------------------------------------------------//
                //                                                            //
                // PML Request.                                               //
                // Should be followed by a ParameterList containing zero or   //
                // more Parameter values.                                     //
                // Each Parameter is expected to have the format:             //
                //   DataType  -  6 bits                                      //
                //   Length    - 10 bits                                      //
                //   Data      -  0 or more bytes                             //
                //                                                            //
                //------------------------------------------------------------//

                showElement(PrnParseConstants.ePMLSeqType.RequestAction,
                             PMLDataTypes.eTag.Unknown,
                             seqOffset + partOffset,
                             partLen,
                             "PML Request",
                             tagDesc);

                partOffset += partLen;

                while (partOffset < seqLen)
                {
                    remLen = seqLen - partOffset;

                    partLen = 2;

                    if (partLen > remLen)
                    {
                        invalidSeqFound = true;
                    }
                    else
                    {
                        crntByte = _buf[seqOffset + partOffset];

                        dataType = (byte)(crntByte / 4);
                        dataLen = ((crntByte & 0x03) * 256) +
                                    _buf[seqOffset + partOffset + 1];

                        PMLDataTypes.CheckTag(dataType, ref tagDesc);

                        tagDesc = tagDesc + " / " + dataLen;

                        showElement(PrnParseConstants.ePMLSeqType.RequestTypeLen,
                                     PMLDataTypes.eTag.Unknown,
                                     seqOffset + partOffset,
                                     partLen,
                                     "PML Data Type/Length",
                                     tagDesc);

                        partOffset += partLen;
                        remLen -= partLen;

                        if (dataLen > remLen)
                        {
                            invalidSeqFound = true;
                        }
                        else
                        {
                            if (dataType == (byte)PMLDataTypes.eTag.String)
                            {
                                partLen = 2;

                                showElement(PrnParseConstants.ePMLSeqType.RequestData,
                                             PMLDataTypes.eTag.StringHddr,
                                             seqOffset + partOffset,
                                             partLen,
                                             "         Symbol Set",
                                             string.Empty);

                                partOffset += partLen;
                                dataLen -= partLen;
                            }

                            partLen = dataLen;

                            showElement(PrnParseConstants.ePMLSeqType.RequestData,
                                         (PMLDataTypes.eTag)dataType,
                                         seqOffset + partOffset,
                                         partLen,
                                         "         Value",
                                         string.Empty);

                            partOffset += partLen;
                        }
                    }

                    if (invalidSeqFound)
                    {
                        partOffset = seqLen + 1;

                        PrnParseCommon.AddTextRow(
                            PrnParseRowTypes.eType.MsgWarning,
                            _table,
                            PrnParseConstants.eOvlShow.None,
                            string.Empty,
                            "*** Warning ***",
                            string.Empty,
                            "Structure suspect");
                    }
                }
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // PML Reply.                                                 //
                // Should be followed by an ExecutionOutcome byte, then a     //
                // Results list containing zero or more Result elements.      //
                // Each Result is expected to have the format:                //
                //   DataType  -  6 bits                                      //
                //   Length    - 10 bits                                      //
                //   Data      -  0 or more bytes                             //
                //                                                            //
                // The first of the Result elements is normally expected to   //
                // be an ObjectIdentifier datatype, but this may be preceded  //
                // by an ErrorCode datatype if the original request was not   //
                // satisified completely.                                     //
                //                                                            //
                //------------------------------------------------------------//

                showElement(PrnParseConstants.ePMLSeqType.ReplyAction,
                             PMLDataTypes.eTag.Unknown,
                             seqOffset + partOffset,
                             partLen,
                             "PML Reply",
                             tagDesc);

                partOffset += partLen;

                remLen = seqLen - partOffset;

                partLen = 1;

                if (partLen > remLen)
                {
                    invalidSeqFound = true;
                    partOffset = seqLen + 1;
                }
                else
                {
                    crntByte = _buf[seqOffset + partOffset];

                    PMLOutcomes.CheckTag(crntByte, ref tagDesc);

                    showElement(PrnParseConstants.ePMLSeqType.ReplyOutcome,
                                 PMLDataTypes.eTag.Unknown,
                                 seqOffset + partOffset,
                                 partLen,
                                 "PML Outcome",
                                 tagDesc);

                    partOffset += partLen;
                }

                while (partOffset < seqLen)
                {
                    remLen = seqLen - partOffset;

                    partLen = 2;

                    if (partLen > remLen)
                    {
                        invalidSeqFound = true;
                    }
                    else
                    {
                        crntByte = _buf[seqOffset + partOffset];

                        dataType = (byte)(crntByte / 4);
                        dataLen = ((crntByte & 0x03) * 256) +
                                    _buf[seqOffset + partOffset + 1];

                        PMLDataTypes.CheckTag(dataType, ref tagDesc);

                        tagDesc = tagDesc + " / " + dataLen;

                        showElement(PrnParseConstants.ePMLSeqType.RequestTypeLen,
                                     PMLDataTypes.eTag.Unknown,
                                     seqOffset + partOffset,
                                     partLen,
                                     "PML Data Type/Length",
                                     tagDesc);

                        partOffset += partLen;
                        remLen -= partLen;

                        if (dataLen > remLen)
                        {
                            invalidSeqFound = true;
                        }
                        else if (dataLen != 0)
                        {
                            if (dataType == (byte)PMLDataTypes.eTag.String)
                            {
                                partLen = 2;

                                showElement(PrnParseConstants.ePMLSeqType.RequestData,
                                             PMLDataTypes.eTag.StringHddr,
                                             seqOffset + partOffset,
                                             partLen,
                                             "         Symbol Set",
                                             string.Empty);

                                partOffset += partLen;
                                dataLen -= partLen;
                            }

                            partLen = dataLen;

                            showElement(PrnParseConstants.ePMLSeqType.RequestData,
                                         (PMLDataTypes.eTag)dataType,
                                         seqOffset + partOffset,
                                         partLen,
                                         "         Value",
                                         string.Empty);

                            partOffset += partLen;
                        }
                    }

                    if (invalidSeqFound)
                    {
                        partOffset = seqLen + 1;

                        PrnParseCommon.AddTextRow(
                            PrnParseRowTypes.eType.MsgWarning,
                            _table,
                            PrnParseConstants.eOvlShow.None,
                            string.Empty,
                            "*** Warning ***",
                            string.Empty,
                            "Structure suspect");
                    }
                }
            }

            return invalidSeqFound;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s h o w E l e m e n t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display details of a supplied PML sequence.                        //
        //                                                                    //
        // Note that we cannot interpret some values (e.g. Object             //
        // Identifiers, Enumerations, Collections), because this would        //
        // require access to the PML Object Specification (alternatively      //
        // known as the Management Information Base (MIB) definition) for the //
        // target printer.                                                    //
        //                                                                    //
        // Not only are some MIBs very difficult to obtain, but they vary     //
        // from device to device - and (even if we bothered to attempt to     //
        // ascertain this from the contents of PJL comments) we don't always  //
        // know which device is the target.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void showElement(PrnParseConstants.ePMLSeqType seqType,
                                  PMLDataTypes.eTag dataType,
                                  int seqOffset,
                                  int seqLen,
                                  string typeText,
                                  string descText)
        {
            const int maxSlice = 4;

            int sliceLen,
                  chunkIpLen,
                  chunkOpLen,
                  chunkOffset,
                  sliceOffset,
                  ipPtr,
                  heldItem,
                  heldItemLen;

            bool firstLine,
                    firstSlice,
                    lastSlice,
                    chunkComplete,
                    seqError;

            string seq,
                   decode;

            StringBuilder chunkOp = new StringBuilder();

            //----------------------------------------------------------------//
            //                                                                //
            // Initialise.                                                    //
            //                                                                //
            //----------------------------------------------------------------//

            firstLine = true;
            firstSlice = true;
            lastSlice = false;
            chunkComplete = false;
            seqError = false;

            sliceOffset = seqOffset;

            //----------------------------------------------------------------//
            //                                                                //
            // Set slice length according to sequence type.                   //
            //                                                                //
            //----------------------------------------------------------------//

            if (seqType == PrnParseConstants.ePMLSeqType.RequestAction)
                sliceLen = 1;           // PML Request action type
            else if (seqType == PrnParseConstants.ePMLSeqType.RequestTypeLen)
                sliceLen = 2;           // PML Request data type/length bytes
            else if (seqType == PrnParseConstants.ePMLSeqType.RequestData)
                sliceLen = maxSlice;    // PML Request data
            else if (seqType == PrnParseConstants.ePMLSeqType.ReplyAction)
                sliceLen = 1;           // PML Reply action type
            else if (seqType == PrnParseConstants.ePMLSeqType.Hddr)
                sliceLen = 4;           // PML Header (within embedded PCL)
            else
                sliceLen = 1;

            //----------------------------------------------------------------//
            //                                                                //
            // Loop round, interpreting the data slice by slice.              //
            //                                                                //
            // The input data is examined in 'slices' (of a size suitable for //
            // details to be shown in the Seq column of the display window),  //
            // and is output in 'chunks' (of a size suitable for details to   //
            // be shown in the Interpretation column).                        //
            //                                                                //
            //----------------------------------------------------------------//

            ipPtr = 0;
            chunkIpLen = 0;
            chunkOffset = sliceOffset;
            chunkOpLen = 0;
            heldItem = 0;
            heldItemLen = 0;

            while (ipPtr < seqLen)
            {
                decode = string.Empty;

                //------------------------------------------------------------//
                //                                                            //
                // Loop step 1:                                               //
                // Select next input slice.                                   //
                //                                                            //
                //------------------------------------------------------------//

                if ((ipPtr + sliceLen) >= seqLen)
                {
                    sliceLen = seqLen - ipPtr;
                    lastSlice = true;
                }

                //------------------------------------------------------------//
                //                                                            //
                // Loop step 2:                                               //
                // Process slice according to data type.                      //
                //                                                            //
                //------------------------------------------------------------//

                if ((seqType == PrnParseConstants.ePMLSeqType.RequestAction) ||
                    (seqType == PrnParseConstants.ePMLSeqType.RequestTypeLen) ||
                    (seqType == PrnParseConstants.ePMLSeqType.ReplyAction) ||
                    (seqType == PrnParseConstants.ePMLSeqType.ReplyOutcome))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Interpretation text has been supplied.                 //
                    // Output this with the last slice of the sequence.       //
                    //                                                        //
                    //--------------------------------------------------------//

                    chunkIpLen += sliceLen;

                    if (lastSlice)
                        decode = descText;

                }
                else
                {
                    decode = showElementDecodeData(ref chunkOp,
                                                    ref chunkIpLen,
                                                    ref chunkOpLen,
                                                    ref heldItem,
                                                    ref heldItemLen,
                                                    ref chunkComplete,
                                                    ref seqError,
                                                    sliceOffset,
                                                    sliceLen,
                                                    chunkOffset,
                                                    firstLine,
                                                    firstSlice,
                                                    lastSlice,
                                                    dataType);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Loop step 3:                                               //
                // Convert slice of supplied sequence data to hexadecimal     //
                // notation in the sequence buffer.                           //
                // Do this only when the current chunk is complete, unless    //
                // verbose mode is set, or an error has been detected.        //
                //                                                            //
                //------------------------------------------------------------//

                seq = showElementSeqData(sliceLen,
                                          sliceOffset,
                                          chunkIpLen,
                                          chunkOffset,
                                          lastSlice,
                                          chunkComplete,
                                          seqError);

                //------------------------------------------------------------//
                //                                                            //
                // Loop step 4:                                               //
                // Output details of current slice (if verbose mode or error  //
                //                                  found)                    //
                //                   current chunk (otherwise)                //
                //                                                            //
                //------------------------------------------------------------//

                if (lastSlice)
                {
                    chunkComplete = true;
                }

                if (_verboseMode || seqError || chunkComplete)
                {
                    if (firstLine)
                    {
                        PrnParseCommon.AddDataRow(
                            PrnParseRowTypes.eType.PMLSeq,
                            _table,
                            PrnParseConstants.eOvlShow.None,
                            _indxOffsetFormat,
                            _fileOffset + chunkOffset,
                            _analysisLevel,
                            typeText,
                            seq,
                            decode.ToString());
                    }
                    else
                    {
                        PrnParseCommon.AddDataRow(
                            PrnParseRowTypes.eType.PMLSeq,
                            _table,
                            PrnParseConstants.eOvlShow.None,
                            _indxOffsetFormat,
                            _fileOffset + chunkOffset,
                            _analysisLevel,
                            string.Empty,
                            seq,
                            decode.ToString());
                    }

                    firstLine = false;

                    if (chunkComplete)
                    {
                        chunkComplete = false;
                        chunkOffset += chunkIpLen;
                        chunkIpLen = 0;
                        chunkOpLen = 0;
                        chunkOp.Clear();
                    }
                }

                //------------------------------------------------------------//
                //                                                            //
                // Loop step 5:                                               //
                // Adjust pointers to next (if any) slice.                    //
                //                                                            //
                //------------------------------------------------------------//

                firstSlice = false;

                ipPtr += sliceLen;
                sliceOffset += sliceLen;
            }  // end of While loop
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s h o w E l e m e n t D e c o d e D a t a                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process slice according to data type, and return decode.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private string showElementDecodeData(
            ref StringBuilder chunkOp,
            ref int chunkIpLen,
            ref int chunkOpLen,
            ref int heldItem,
            ref int heldItemLen,
            ref bool chunkComplete,
            ref bool seqError,
            int sliceOffset,
            int sliceLen,
            int chunkOffset,
            bool firstLine,
            bool firstSlice,
            bool lastSlice,
            PMLDataTypes.eTag dataType)
        {
            StringBuilder decode = new StringBuilder();

            int decodeMax = _decodeAreaMax;

            int itemLen = 0;

            if (dataType == PMLDataTypes.eTag.ObjectID)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Object Identifier.                                         //
                //                                                            //
                // We cannot interpret the OIDs, because this would require   //
                // access to the Management Information Base (MIB) definition //
                // for the target printer.                                    //
                //                                                            //
                // So we just interpret the hexadecimal byte values as        //
                // decimal elements, separated by "." characters; the         //
                // elements can take two forms:                               //
                //  - If the value is less than 128 (i.e. 0x00->0x7f), this   //
                //    value is used directly; this is the short form.         //
                //  - If the value is 128 or more (i.e. 0x80 ->0xff), the     //
                //    low-order bits (1-7) are treated as a length,           //
                //    indicating how many bytes following this one are used   //
                //    to hold the value; some of these bytes may themselves   //
                //    have their high-order bit set.                          //
                //                                                            //
                //------------------------------------------------------------//

                int j,
                      k,
                      thisByte,
                      item;

                item = heldItem;
                itemLen = heldItemLen;

                for (j = 0; j < sliceLen; j++)
                {
                    k = chunkOffset + chunkIpLen + j;
                    thisByte = _buf[k];

                    if (itemLen != 0)
                    {
                        // continuation

                        item *= 256;
                        item += thisByte;
                    }
                    else if (thisByte > 127)
                    {
                        itemLen = (thisByte & 0x7f) + 1;
                        item = 0;
                    }
                    else
                    {
                        itemLen = 1;
                        item = thisByte;
                    }

                    itemLen -= 1;

                    if (itemLen == 0)
                    {
                        if ((!firstLine) || (chunkOp.Length != 0))
                            chunkOp.Append(".");

                        chunkOp.Append(item.ToString());
                    }
                }

                if (itemLen != 0)
                    heldItem = item;
                else
                    heldItem = 0;

                heldItemLen = itemLen;
            }
            else if (dataType == PMLDataTypes.eTag.Enumeration)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Enumeration.                                               //
                //                                                            //
                // We cannot interpret the enumerations, because this would   //
                // require access to the Management Information Base (MIB)    //
                // definition for the target printer.                         //
                //                                                            //
                // Hence we just treat the value as an integer.               //
                // The hexadecimal value is converted, byte by byte, to a     //
                // signed integer (using Big-Endian byte-ordering).           //
                // The integer value is then converted to a string            //
                // representation by a standard string function.              //
                //                                                            //
                //------------------------------------------------------------//

                if (sliceLen > 4)
                {
                    if (lastSlice)
                        chunkOp.Append("***** large integer value *****");
                }
                else
                {
                    int iSub,
                          iTot;

                    bool msByte;

                    string tempStr;

                    iTot = 0;
                    msByte = true;

                    for (int j = 0; j < sliceLen; j++)
                    {
                        iSub = _buf[sliceOffset + j];

                        if (msByte && (iSub >= 0x80))
                            iTot = iSub - 256;
                        else
                            iTot = (iTot * 256) + iSub;

                        msByte = false;
                    }

                    tempStr = iTot.ToString();
                    chunkOp.Append(tempStr);
                }
            }
            else if (dataType == PMLDataTypes.eTag.Sint)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Signed integer value.                                      //
                //                                                            //
                // The (signed) value can (theoretically) occupy up to 43     //
                // bytes (!).                                                 //
                // We won't attempt to interpret any value larger than 4      //
                // bytes.                                                     //
                //                                                            //
                // Decode the value as follows:                               //
                //                                                            //
                //  - The value is converted, byte by byte, to a signed       //
                //    integer ((using Big-Endian byte-ordering).              //
                //                                                            //
                //  - This integer value is then converted to its string      //
                //    representation.                                         //
                //                                                            //
                //------------------------------------------------------------//

                if (sliceLen > 4)
                {
                    if (lastSlice)
                        chunkOp.Append("***** large integer value *****");
                }
                else
                {
                    int iSub,
                          iTot;

                    bool msByte;

                    string tempStr;

                    iTot = 0;
                    msByte = true;

                    for (int j = 0; j < sliceLen; j++)
                    {
                        iSub = _buf[sliceOffset + j];

                        if (msByte && (iSub > 0x80))
                            iTot = iSub - 256;
                        else
                            iTot = (iTot * 256) + iSub;

                        msByte = false;
                    }

                    tempStr = iTot.ToString();
                    chunkOp.Append(tempStr);
                }
            }
            else if (dataType == PMLDataTypes.eTag.Real)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Real (IEEE 32-bit single-precision floating-point) value.  //
                //                                                            //
                // Decode the value as follows:                               //
                //                                                            //
                //  - The 4-byte value is converted, byte by byte, to an      //
                //    unsigned integer.                                       //
                //                                                            //
                //  - The resultant integer is then converted back to a byte  //
                //    array (using the host byte ordering).                   //
                //                                                            //
                //  - This new byte array is then converted to a floating     //
                //    floating point value.                                   //
                //                                                            //
                //  - This value is then converted to its string              //
                //    representation.                                         //
                //                                                            //
                //------------------------------------------------------------//

                if (sliceLen != 4)
                {
                    if (lastSlice)
                        chunkOp.Append("***** invalid real value *****");
                }
                else
                {
                    uint uiSub,
                           uiTot;

                    byte[] byteArray;

                    float f;

                    string tempStr;

                    uiTot = 0;

                    for (int j = 0; j < sliceLen; j++)
                    {
                        uiSub = _buf[sliceOffset + j];
                        uiTot = (uiTot * 256) + uiSub;
                    }

                    byteArray = BitConverter.GetBytes(uiTot);

                    f = BitConverter.ToSingle(byteArray, 0);

                    tempStr = f.ToString("F6");
                    chunkOp.Append(tempStr);
                }
            }
            else if (dataType == PMLDataTypes.eTag.StringHddr)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Not a true data type; just the few two bytes of a      //
                // String data type.                                      //
                // These bytes define the Symbol Set with which the       //
                // remaining characters of the String are encoded.        //
                //                                                        //
                // The two bytes are interpreted as a 'Kind1' short       //
                // integer value, from which the more familiar HP Symbol  //
                // Set identifier can be derived.                         //
                //                                                        //
                // Two part: 11-bit number (binary).                      //
                //            5-bit letter-code: add 64 to this to obtain //
                //                  the (ASCII) character-code of the     //
                //                  letter.                               //
                //                                                        //
                // e.g. value of  0x000E --> 0N                           //
                //                0x0115 --> 8U                           //
                //                0x0155 --> 10U                          //
                //                0x01F1 --> 15Q                          //
                //                                                        //
                //--------------------------------------------------------//

                int ix1,
                      ix2,
                      ix3;

                ix1 = (_buf[sliceOffset] * 256) + _buf[sliceOffset + 1];

                ix2 = ix1 >> 5;
                ix3 = (ix1 & 0x1f) + 64;

                chunkOp.Append(ix1 + " (= " +
                                         ix2.ToString() +
                                         (char)ix3 + ")");
            }
            else if (dataType == PMLDataTypes.eTag.String)
            {
                //--------------------------------------------------------//
                //                                                        //
                // String of ASCII character(s).                          //
                // First two characters are the Symbol Set Kind1 value,   //
                // which will have been processed by a previous call,     //
                // using the pseudo data type of ePMLDtStringHddr.        //
                // Interpret the value(s) and output the resultant        //
                // character(s) within quotes.                            //
                //                                                        //
                //--------------------------------------------------------//

                int k;

                for (int j = 0; j < sliceLen; j++)
                {
                    k = chunkOffset + chunkIpLen + j;

                    chunkOp.Append(PrnParseData.ProcessByte(
                        _buf[k],
                        _indxCharSetSubAct,
                        (byte)_valCharSetSubCode,
                        _indxCharSetName));
                }
            }
            else if (dataType == PMLDataTypes.eTag.Binary)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Binary.                                                //
                // This is just a sequence of bytes, the interpretation   //
                // of which is dependent on the associated Object         //
                // Identifier.                                            //
                // All we can do is display the value as hex.             //
                //                                                        //
                //--------------------------------------------------------//

                string seq;

                seq = PrnParseCommon.ByteArrayToHexString(_buf,
                                                           sliceOffset,
                                                           sliceLen);

                if (chunkOp.Length == 0)
                    chunkOp.Append("[ ");

                chunkOp.Append(seq);
            }
            else if (dataType == PMLDataTypes.eTag.ErrorCode)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Error Code.                                            //
                // Should be a single byte value; look this up in the     //
                // table.                                                 //
                //                                                        //
                //--------------------------------------------------------//

                string tempStr = string.Empty;

                if (sliceLen != 1)
                {
                    if (lastSlice)
                        chunkOp.Append("***** large Error Code value *****");
                }
                else
                {
                    PMLOutcomes.CheckTag(_buf[sliceOffset],
                                          ref tempStr);

                    chunkOp.Append(tempStr);
                }
            }
            else if (dataType == PMLDataTypes.eTag.NullValue)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Null value.                                            //
                // Should never occur, since the NullValue datatype       //
                // should always have a length of zero.                   //
                //                                                        //
                //--------------------------------------------------------//

                seqError = true;
                chunkOp.Append("*** non-zero-length Null Value ***");
            }
            else if (dataType == PMLDataTypes.eTag.Collection)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Collection.                                            //
                // This is just a value, representing a series of flag    //
                // bits.                                                  //
                // All we can do is display the value as hex, since       //
                // interpretation of the bits requires the MIB definition.//
                //                                                        //
                //--------------------------------------------------------//

                string seq;

                seq = PrnParseCommon.ByteArrayToHexString(_buf,
                                                           sliceOffset,
                                                           sliceLen);

                if (chunkOp.Length == 0)
                    chunkOp.Append("[ ");

                chunkOp.Append(seq);
            }
            else
            {
                //--------------------------------------------------------//
                //                                                        //
                // Unknown type.                                          //
                //                                                        //
                //--------------------------------------------------------//

                seqError = true;
                chunkOp.Append("*** unknown type ***");
            }

            chunkIpLen += sliceLen;
            chunkOpLen = chunkOp.Length;

            if (_verboseMode || lastSlice || seqError ||
                ((chunkOpLen + 2) >= decodeMax))
            {
                chunkComplete = true;

                if ((dataType == PMLDataTypes.eTag.Binary) ||
                    (dataType == PMLDataTypes.eTag.Collection))
                    chunkOp.Append("]");

                decode.Append(chunkOp);
            }

            return decode.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s h o w E l e m e n t S e q D a t a                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Convert slice of supplied sequence data to hexadecimal notation in //
        // the sequence buffer.                                               //
        // Do this only when the current chunk is complete, unless verbose    //
        // mode is set, or an error has been detected.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private string showElementSeqData(int sliceLen,
                                           int sliceOffset,
                                           int chunkIpLen,
                                           int chunkOffset,
                                           bool lastSlice,
                                           bool chunkComplete,
                                           bool seqError)
        {
            StringBuilder seq = new StringBuilder();

            byte crntByte;

            int hexPtr,
                  hexStart = 0,
                  hexEnd = 0,
                  sub;

            bool useEllipsis,
                    displaySlice;

            char[] hexBuf = new char[(_decodeSliceMax * 2) + 1];

            useEllipsis = false;
            displaySlice = false;
            seq.Clear();

            if (_verboseMode || seqError)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Convert current slice.                                     //
                //                                                            //
                //------------------------------------------------------------//

                displaySlice = true;
                hexStart = sliceOffset;
                hexEnd = sliceOffset + sliceLen;
            }
            else if ((!_verboseMode) && (chunkComplete || lastSlice))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Convert first few characters of current chunk.             //
                //                                                            //
                //------------------------------------------------------------//

                displaySlice = true;

                if (chunkIpLen > _decodeSliceMax)
                {
                    hexStart = chunkOffset;
                    hexEnd = chunkOffset + _decodeSliceMax - 1;
                    useEllipsis = true;
                }
                else
                {
                    hexStart = chunkOffset;
                    hexEnd = chunkOffset + chunkIpLen;
                }
            }

            if (displaySlice)
            {
                hexPtr = 0;

                for (int j = hexStart; j < hexEnd; j++)
                {
                    sub = (_buf[j]);
                    sub >>= 4;
                    crntByte = PrnParseConstants.cHexBytes[sub];
                    hexBuf[hexPtr++] = (char)crntByte;

                    sub = (_buf[j] & 0x0f);
                    crntByte = PrnParseConstants.cHexBytes[sub];
                    hexBuf[hexPtr++] = (char)crntByte;
                }

                seq.Append("0x");
                seq.Append(hexBuf, 0, hexPtr);

                if (useEllipsis)
                    seq.Append("..");
            }

            return seq.ToString();
        }
    }
}