﻿using System;
using System.Data;
using System.IO;
using System.Text;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines functions to parse PCLXL sequences.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal class PrnParsePCLXL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const int _decodeIndentNone = 0;
        private const int _decodeIndentStd = 4;
        private const int _decodeAreaMax = 48;
        private const int _decodeSliceMax = 4;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private PrnParseLinkData _linkData;
        private PrnParse _analysisOwner;
        private PrnParseOptions _options;

        private readonly PrnParse.ParseType _parseType;

        private PrnParseConstants.OvlShow _operOvlShow;

        private DataTable _table;

        private readonly PrnParseFontHddrPCLXL _parseFontHddrPCLXL;
        private readonly PrnParseFontCharPCLXL _parseFontCharPCLXL;

        private byte[] _buf;

        private int _fileOffset;
        private int _endOffset;

        private PrnParseConstants.OptOffsetFormats _indxOffsetFormat;

        private PrnParseConstants.OptCharSetSubActs _indxCharSetSubAct;
        private PrnParseConstants.OptCharSets _indxCharSetName;
        private int _valCharSetSubCode;

        private PrnParseConstants.PCLXLBinding _startMode;
        private PrnParseConstants.PCLXLBinding _bindType;

        private PCLXLOperators.EmbedDataType _crntOperEmbedType;
        private PCLXLOperators.EmbedDataType _prevOperEmbedType;

        private PCLXLOperators.EmbedDataType _crntEmbedType;

        private readonly PrnParsePCLXLElementMetrics _displayMetricsCrnt;
        private readonly PrnParsePCLXLElementMetrics _displayMetricsEmbedByte;
        private readonly PrnParsePCLXLElementMetrics _displayMetricsEmbedWord;
        private readonly PrnParsePCLXLElementMetrics _displayMetricsHddr;
        private readonly PrnParsePCLXLElementMetrics _displayMetricsNil;
        private readonly PrnParsePCLXLElementMetrics _displayMetricsString;
        private readonly PrnParsePCLXLElementMetrics _displayMetricsUbyte;
        private readonly PrnParsePCLXLElementMetrics _displayMetricsUint16;

        private int _analysisLevel;

        private byte _attrID1,
                     _attrID2,
                     _crntOperID;

        private int _attrIDLen,
                      _attrDataStart,
                      _operDataStart,
                      _operNum,
                      _embedDataLen,
                      _embedDataRem,
                      _attrDataVal;

        private bool _streamActive,
                        _hddrRead,
                        _attrIDFound,
                        _operIDFound,
                        _attrDataStarted,
                        _operDataStarted,
                        _attrEnumerated,
                        _attrOperEnumeration,
                        _attrUbyteAsAscii,
                        _attrUint16AsUnicode,
                        _attrValueIsEmbedLength,
                        _attrValueIsPCLArray,
                        _rawDataAfterOpTag,
                        _analyseStreams,
                        _analyseFontHddr,
                        _analyseFontChar,
                        _analysePCLFontData,
                        _analysePassThrough,
                        _verboseMode,
                        _showOperPos,
                        _showBinData,
                        _continuation,
                        _breakpoint;

        private readonly ASCIIEncoding _ascii = new ASCIIEncoding();

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P r n P a r s e P C L X L                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParsePCLXL(PrnParse.ParseType parseType)
        {
            const bool flagNone = false;
            const bool flagArrayType = true;
            const bool flagUbyteAsAscii = true;
            //   const Boolean flagUint16AsUnicode = true;

            const int sizeSingle = 1;
            const int sizeDouble = 2;
            const int sizeQuad = 4;

            _parseType = parseType;

            _parseFontHddrPCLXL = new PrnParseFontHddrPCLXL();
            _parseFontCharPCLXL = new PrnParseFontCharPCLXL();

            _displayMetricsCrnt = new PrnParsePCLXLElementMetrics(
                flagNone,
                flagNone,
                flagNone,
                _decodeIndentStd,
                sizeSingle,
                sizeSingle,
                PCLXLDataTypes.BaseType.Unknown);

            _displayMetricsEmbedByte = new PrnParsePCLXLElementMetrics(
                flagNone,
                flagNone,
                flagNone,
                _decodeIndentStd,
                sizeSingle,
                sizeSingle,
                PCLXLDataTypes.BaseType.Ubyte);

            _displayMetricsEmbedWord = new PrnParsePCLXLElementMetrics(
                flagNone,
                flagNone,
                flagNone,
                _decodeIndentStd,
                sizeSingle,
                sizeQuad,
                PCLXLDataTypes.BaseType.Sint32);

            _displayMetricsHddr = new PrnParsePCLXLElementMetrics(
                flagUbyteAsAscii,
                flagNone,
                flagArrayType,
                _decodeIndentNone,
                sizeSingle,
                sizeSingle,
                PCLXLDataTypes.BaseType.Ubyte);

            _displayMetricsNil = new PrnParsePCLXLElementMetrics(
                flagNone,
                flagNone,
                flagNone,
                _decodeIndentStd,
                sizeSingle,
                sizeSingle,
                PCLXLDataTypes.BaseType.Unknown);

            _displayMetricsString = new PrnParsePCLXLElementMetrics(
                flagUbyteAsAscii,
                flagNone,
                flagArrayType,
                _decodeIndentStd,
                sizeSingle,
                sizeSingle,
                PCLXLDataTypes.BaseType.Ubyte);

            _displayMetricsUbyte = new PrnParsePCLXLElementMetrics(
                flagNone,
                flagNone,
                flagNone,
                _decodeIndentStd,
                sizeSingle,
                sizeSingle,
                PCLXLDataTypes.BaseType.Ubyte);

            _displayMetricsUint16 = new PrnParsePCLXLElementMetrics(
                flagNone,
                flagNone,
                flagNone,
                _decodeIndentStd,
                sizeSingle,
                sizeDouble,
                PCLXLDataTypes.BaseType.Uint16);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i s W h i t e s p a c e T a g                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check if supplied tag is a Whitespace tag.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool IsWhitespaceTag(byte crntByte)
        {
            return PCLXLWhitespaces.IsKnownTag(crntByte);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m a k e O v e r l a y I n s e r t H e a d e r                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Make Overlay insert header action.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void MakeOverlayInsertHeader(BinaryWriter binWriter,
                                            bool encapsulate,
                                            string streamName,
                                            DataTable table)
        {
            if (encapsulate)
            {
                PCLXLWriter.StreamBegin(binWriter, streamName);

                PrnParseCommon.AddDataRow(
                    PrnParseRowTypes.Type.MsgComment,
                    table,
                    PrnParseConstants.OvlShow.Insert,
                    _indxOffsetFormat,
                    (int)PrnParseConstants.OffsetPosition.StartOfFile,
                    _analysisLevel,
                    "PCLXL structure",
                    "0xc8c1....",
                    "BeginStream structure for stream '" + streamName + "'");
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m a k e O v e r l a y I n s e r t T r a i l e r                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Make Overlay insert trailer action.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void MakeOverlayInsertTrailer(BinaryWriter binWriter,
                                             bool flagRestoreGS,
                                             bool encapsulate,
                                             DataTable table)
        {
            if (flagRestoreGS)
            {
                string descText;

                PCLXLWriter.WriteOperator(binWriter, PCLXLOperators.Tag.PopGS, encapsulate);

                if (encapsulate)
                    descText = "PopGS (encapsulated within ReadStream structure)";
                else
                    descText = "PopGS";

                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.Type.PCLXLOperator,
                     table,
                     PrnParseConstants.OvlShow.Insert,
                     string.Empty,
                     "PCLXL Operator",
                     "0x60",
                     descText);
            }

            if (encapsulate)
            {
                PCLXLWriter.StreamEnd(binWriter);

                PrnParseCommon.AddDataRow(
                    PrnParseRowTypes.Type.PCLXLOperator,
                    table,
                    PrnParseConstants.OvlShow.Insert,
                    _indxOffsetFormat,
                    (int)PrnParseConstants.OffsetPosition.EndOfFile,
                    _analysisLevel,
                    "PCLXL Operator",
                    "0x5d",
                    "EndStream");
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a r s e B u f f e r                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Parse provided buffer, assuming that the current print language is //
        // PCL XL.                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool ParseBuffer(
            byte[] buf,
            ref int fileOffset,
            ref int bufRem,
            ref int bufOffset,
            ref ToolCommonData.PrintLang crntPDL,
            ref bool endReached,
            PrnParseLinkData linkData,
            PrnParseOptions options,
            DataTable table,
            bool firstCall)
        {
            bool seqInvalid;

            //----------------------------------------------------------------//
            //                                                                //
            // Initialise.                                                    //
            //                                                                //
            //----------------------------------------------------------------//

            _buf = buf;
            _linkData = linkData;
            _options = options;
            _table = table;
            _fileOffset = fileOffset;
            _rawDataAfterOpTag = false;

            _analysisOwner = _linkData.AnalysisOwner;
            _analysisLevel = _linkData.AnalysisLevel;
            _crntEmbedType = _linkData.PclxlEmbedType;

            //----------------------------------------------------------------//

            _indxOffsetFormat = _options.IndxGenOffsetFormat;

            _options.GetOptCharSet(ref _indxCharSetName,
                                    ref _indxCharSetSubAct,
                                    ref _valCharSetSubCode);

            _endOffset = _options.ValCurFOffsetEnd;

            _options.GetOptPCLXLBasic(ref _analyseFontHddr,
                                       ref _analyseFontChar,
                                       ref _analyseStreams,
                                       ref _analysePassThrough,
                                       ref _analysePCLFontData,
                                       ref _showOperPos,
                                       ref _showBinData,
                                       ref _verboseMode);

            _startMode = _options.IndxCurFXLBinding;

            //----------------------------------------------------------------//

            if (firstCall)
            {
                _crntOperEmbedType = PCLXLOperators.EmbedDataType.None;
                _prevOperEmbedType = PCLXLOperators.EmbedDataType.None;
                _crntOperID = 0;

                _streamActive = true;
                _hddrRead = false;
                _operNum = 0;

                _linkData.MakeOvlPos =
                    PrnParseConstants.OvlPos.BeforeFirstPage;

                if ((_startMode == PrnParseConstants.PCLXLBinding.BinaryLSFirst) ||
                    (_startMode == PrnParseConstants.PCLXLBinding.BinaryMSFirst))
                {
                    _hddrRead = true;
                    _bindType = _startMode;
                }
            }

            //----------------------------------------------------------------//

            if (linkData.IsContinuation())
                seqInvalid = ParseContinuation(ref bufRem, ref bufOffset, ref crntPDL, ref endReached, firstCall);
            else
                seqInvalid = ParseSequences(ref bufRem, ref bufOffset, ref crntPDL, ref endReached, firstCall);

            return seqInvalid;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a r s e C o n t i n u a t i o n                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Handle continuation situation signalled on last pass.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ParseContinuation(
            ref int bufRem,
            ref int bufOffset,
            ref ToolCommonData.PrintLang crntPDL,
            ref bool endReached,
            bool firstCall)
        {
            PrnParseConstants.ContType contType = PrnParseConstants.ContType.None;
            int prefixLen = 0,
                  contDataLen = 0,
                  downloadRem = 0;
            bool backTrack = false;

            bool invalidSeqFound = false;

            byte prefixA = 0x00,
                 prefixB = 0x00;

            _linkData.GetContData(ref contType,
                                   ref prefixLen,
                                   ref contDataLen,
                                   ref downloadRem,
                                   ref backTrack,
                                   ref prefixA,
                                   ref prefixB);

            if (contType == PrnParseConstants.ContType.Reset)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Continuation action was to take some action (initially to  //
                // support the 'Make Macro' breakpoint on the last (or only)  //
                // element in a PCL combination sequence) and then perform    //
                // the necesaary backtracking, but without any further action.//
                //                                                            //
                //------------------------------------------------------------//

                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLXLEmbed)
            {
                int binDataLen;
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended with incomplete PCLXL embedded data //
                // sequence; the whole sequence was not completely contained  //
                // in that block.                                             //
                // Output the remaining 'download' characters (or the whole   //
                // buffer and initiate another continuation) before           //
                // continuing with the analysis.                              //
                //                                                            //
                //------------------------------------------------------------//

                if (_embedDataRem > bufRem)
                {
                    binDataLen = bufRem;
                    _embedDataRem -= bufRem;
                }
                else
                {
                    binDataLen = _embedDataRem;
                    _embedDataRem = 0;
                }

                //------------------------------------------------------------//
                //                                                            //
                // Some, or all, of the download data is contained within the //
                // current 'block'.                                           //
                // Some types of download may require (optional) further      //
                // processing, by capturing (and then later analysing) the    //
                // embedded data.                                             //
                //                                                            //
                //------------------------------------------------------------//

                if (_crntOperEmbedType != PCLXLOperators.EmbedDataType.None)
                {
                    if (((_crntOperEmbedType == PCLXLOperators.EmbedDataType.PassThrough) && _analysePassThrough)
                                                 ||
                        ((_crntOperEmbedType == PCLXLOperators.EmbedDataType.Stream) && _analyseStreams)
                                                 ||
                        ((_crntOperEmbedType == PCLXLOperators.EmbedDataType.FontHeader) && _analyseFontHddr)
                                                 ||
                        ((_crntOperEmbedType == PCLXLOperators.EmbedDataType.FontChar) && _analyseFontChar))
                    {
                        _analysisOwner.EmbeddedDataStore(_buf, bufOffset, binDataLen);
                    }
                }

                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.OvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    binDataLen,
                    "               Data",
                    _showBinData,
                    true,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);

                //------------------------------------------------------------//
                //                                                            //
                // Adjust continuation data and pointers.                     //
                //                                                            //
                //------------------------------------------------------------//

                if (_embedDataRem == 0)
                {
                    _continuation = false;
                    _linkData.ResetContData();
                }

                bufRem -= binDataLen;
                bufOffset += binDataLen;
            }
            else if (contType == PrnParseConstants.ContType.PCLXLFontHddr)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a font header  //
                // download sequence.                                         //
                //                                                            //
                //------------------------------------------------------------//

                if (firstCall)
                {
                    contType = PrnParseConstants.ContType.None;
                    _linkData.ResetContData();
                }

                bool hddrOK = _parseFontHddrPCLXL.AnalyseFontHddr(contDataLen,
                                                  _buf,
                                                  _fileOffset,
                                                  ref bufRem,
                                                  ref bufOffset,
                                                  _linkData,
                                                  _options,
                                                  _table);

                if (!hddrOK)
                    invalidSeqFound = true;
            }
            else if (contType == PrnParseConstants.ContType.PCLXLFontChar)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a font         //
                // character download sequence.                               //
                //                                                            //
                //------------------------------------------------------------//

                if (firstCall)
                {
                    contType = PrnParseConstants.ContType.None;
                    _linkData.ResetContData();
                }

                bool charOK = _parseFontCharPCLXL.AnalyseFontChar(contDataLen,
                                                  _buf,
                                                  _fileOffset,
                                                  ref bufRem,
                                                  ref bufOffset,
                                                  _linkData,
                                                  _options,
                                                  _table);
                if (!charOK)
                    invalidSeqFound = true;
            }
            else if ((contType == PrnParseConstants.ContType.PCLXL)
                                  ||
                     (contType == PrnParseConstants.ContType.Special)
                                  ||
                     (contType == PrnParseConstants.ContType.Unknown))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended with a partial match of a PCLXL     //
                // sequence, or with insufficient characters to identify      //
                // the type of sequence.                                      //
                // The continuation action has already reset the buffer, so   //
                // now unset the markers.                                     //
                //                                                            //
                //------------------------------------------------------------//

                _continuation = false;
                _linkData.ResetContData();
            }

            if ((_endOffset != -1) && ((_fileOffset + bufOffset) > _endOffset))
                endReached = true;

            return invalidSeqFound;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a r s e S e q u e n c e s                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process sequences until end-point reached.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ParseSequences(
            ref int bufRem,
            ref int bufOffset,
            ref ToolCommonData.PrintLang crntPDL,
            ref bool endReached,
            bool firstCall)
        {
            byte crntByte;

            bool langSwitch = false;
            bool invalidSeqFound = false;

            _continuation = false;
            _breakpoint = false;

            while (!_continuation && !_breakpoint && !langSwitch &&
                   !invalidSeqFound && !endReached && (bufRem > 0))
            {
                crntByte = _buf[bufOffset];

                PrnParseConstants.ContType contType;
                bool badSeq;
                //------------------------------------------------------------//
                //                                                            //
                // Process data until language-switch or end of buffer, or    //
                // specified end-point.                                       //
                //                                                            //
                //------------------------------------------------------------//

                if ((_endOffset != -1) &&
                    ((_fileOffset + bufOffset) > _endOffset))
                {
                    endReached = true;
                }
                else if (crntByte == PrnParseConstants.asciiEsc)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Escape character found.                                //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (_streamActive)
                    {
                        invalidSeqFound = true;

                        PrnParseCommon.AddTextRow(
                            PrnParseRowTypes.Type.MsgWarning,
                            _table,
                            PrnParseConstants.OvlShow.Illegal,
                            string.Empty,
                            "*** Warning ***",
                            string.Empty,
                            "Stream still active at language-switch");
                    }

                    langSwitch = true;
                    crntPDL = ToolCommonData.PrintLang.PCL;
                    _linkData.MakeOvlPos = PrnParseConstants.OvlPos.AfterPages;
                }
                else if (!_hddrRead)
                {
                    if (crntByte == PrnParseConstants.prescribeSCRCDelimiter)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // "!" character found; could be the start of an      //
                        // embedded Prescribe command.                        //
                        // Need at least 3 bytes to check for the Prescribe   //
                        // start sequence (which is !R! by default).          //
                        //                                                    //
                        //----------------------------------------------------//

                        if (bufRem < 3)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Initiate continuation action.                  //
                            //                                                //
                            //------------------------------------------------//

                            _continuation = true;

                            contType = PrnParseConstants.ContType.PCLXL;

                            _linkData.SetBacktrack(contType, -bufRem);
                        }
                        else
                        {
                            if ((_buf[bufOffset + 1] == _linkData.PrescribeSCRC)
                                                  &&
                                (_buf[bufOffset + 2] == PrnParseConstants.prescribeSCRCDelimiter))
                            {
                                langSwitch = true;
                                crntPDL = ToolCommonData.PrintLang.Prescribe;
                                //  _linkData.MakeOvlPos =
                                //      PrnParseConstants.eOvlPos.AfterPages;
                                _linkData.PrescribeCallerPDL = ToolCommonData.PrintLang.PCLXL;
                            }
                        }
                    }

                    if (!_continuation && !langSwitch)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Process stream Header.                             //
                        //                                                    //
                        //----------------------------------------------------//

                        badSeq = ProcessHeader(ref bufRem, ref bufOffset);

                        if (badSeq)
                        {
                            invalidSeqFound = true;
                        }
                        else if (_hddrRead && (_parseType == PrnParse.ParseType.MakeOverlay))
                        {
                            _breakpoint =
                                PrnParseMakeOvl.CheckActionPCLXLPushGS(
                                            bufOffset,
                                            _fileOffset,
                                            _linkData,
                                            _table,
                                            _indxOffsetFormat);
                        }
                    }
                }
                else if ((crntByte == PrnParseConstants.pclxlAttrUbyte)
                                      ||
                         (crntByte == PrnParseConstants.pclxlAttrUint16))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Attribute definer.                                     //
                    //                                                        //
                    //--------------------------------------------------------//

                    ProcessAttributeTag(ref bufRem, ref bufOffset);
                }
                else if ((crntByte >= PrnParseConstants.pclxlDataTypeLow)
                                      &&
                         (crntByte <= PrnParseConstants.pclxlDataTypeHigh))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Data Type tag.                                         //
                    //                                                        //
                    //--------------------------------------------------------//

                    badSeq = ProcessDataTypeTag(ref bufRem, ref bufOffset);

                    if (badSeq)
                        invalidSeqFound = true;
                }
                else if (IsWhitespaceTag(crntByte))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // WhiteSpace (separator) value.                          //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (_attrDataStarted)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Attribute data has started, but attribute          //
                        // identifier tag has not yet been encountered; skip  //
                        // past the whitespace tag.                           //
                        // The data and anything up to the attribute tag (or  //
                        // other significant tag) will be processed when that //
                        // tag has been found.                                //
                        //                                                    //
                        //----------------------------------------------------//

                        bufOffset++;
                        bufRem--;
                    }
                    else
                    {
                        ProcessWhiteSpaceTag(ref bufRem, ref bufOffset);
                    }
                }
                else if (_attrDataStarted)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Attribute data has been started, but the expected      //
                    // Attribute tag has not been encountered.                //
                    // Revert to stored start point to display orphan         //
                    // attribute data.                                        //
                    //                                                        //
                    //--------------------------------------------------------//

                    int tempLen;

                    invalidSeqFound = true;

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgWarning,
                        _table,
                        PrnParseConstants.OvlShow.Illegal,
                        string.Empty,
                        "*** Warning ***",
                        string.Empty,
                        "The following attribute data appears to be orphaned:");

                    _attrIDFound = true;
                    _attrEnumerated = false;
                    _attrUbyteAsAscii = false;
                    _attrUint16AsUnicode = false;

                    tempLen = bufOffset - _attrDataStart;
                    bufRem += tempLen;
                    bufOffset = _attrDataStart;

                    _attrDataStarted = false;
                }
                else if ((crntByte >= PrnParseConstants.pclxlOperatorLow)
                                      &&
                         (crntByte <= PrnParseConstants.pclxlOperatorHigh))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Operator tag.                                          //
                    //                                                        //
                    //--------------------------------------------------------//

                    ProcessOperatorTag(ref bufRem,
                                        ref bufOffset);

                    if (_rawDataAfterOpTag)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // One of the special attributes which defines the    //
                        // length of (binary) data to follow the Operator tag //
                        // was encountered.                                   //
                        //                                                    //
                        //----------------------------------------------------//

                        _rawDataAfterOpTag = false;
                        _embedDataRem = _attrDataVal;

                        if (_embedDataRem < 0)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Invalid data length.                           //
                            //                                                //
                            //------------------------------------------------//

                            PrnParseCommon.AddTextRow(
                                PrnParseRowTypes.Type.MsgWarning,
                                _table,
                                PrnParseConstants.OvlShow.Illegal,
                                string.Empty,
                                "*** Warning ***",
                                string.Empty,
                                "Invalid raw data length value");

                            _embedDataLen = 0;
                            _embedDataRem = 0;
                        }
                        else if (_embedDataRem > bufRem)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Embedded data is not all contained in the      //
                            // current buffer.                                //
                            // Set markers for continuation action.           //
                            //                                                //
                            //------------------------------------------------//

                            _continuation = true;

                            _embedDataRem -= bufRem;
                            _embedDataLen = bufRem;

                            contType = PrnParseConstants.ContType.PCLXLEmbed;

                            _linkData.SetContinuation(contType);
                        }
                        else
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Embedded data is all contained in the          //
                            // current buffer.                                //
                            //                                                //
                            //------------------------------------------------//

                            _embedDataLen = _embedDataRem;
                            _embedDataRem = 0;
                        }

                        //----------------------------------------------------//
                        //                                                    //
                        // Display (first or only part of) embedded data.     //
                        //                                                    //
                        //----------------------------------------------------//

                        PrnParseData.ProcessBinary(
                            _table,
                            PrnParseConstants.OvlShow.None,
                            _buf,
                            _fileOffset,
                            bufOffset,
                            _embedDataLen,
                            "PCLXL Embedded Data",
                            _showBinData,
                            true,
                            true,
                            _indxOffsetFormat,
                            _analysisLevel);

                        bufOffset += _embedDataLen;
                        bufRem -= _embedDataLen;
                    }
                }
                else if ((crntByte == PrnParseConstants.pclxlEmbedData)
                                    ||
                         (crntByte == PrnParseConstants.pclxlEmbedDataByte))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Data embedded in stream.                               //
                    //                                                        //
                    //--------------------------------------------------------//

                    badSeq = ProcessEmbeddedDataTag(ref bufRem,
                                                     ref bufOffset);
                    if (badSeq)
                        invalidSeqFound = true;
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Unknown identifier.                                    //
                    //                                                        //
                    //--------------------------------------------------------//

                    invalidSeqFound = true;

                    ShowElement(bufOffset,
                                 1,
                                 "PCLXL",
                                 "*** Unidentified data ***",
                                 true,
                                 _displayMetricsNil,
                                 PrnParseConstants.OvlShow.Illegal,
                                 PrnParseRowTypes.Type.MsgWarning);

                    bufOffset++;
                    bufRem--;
                }
            }

            if (langSwitch)
            {
                _operNum = 0;
                _hddrRead = false;
                _streamActive = false;
            }

            return invalidSeqFound;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s A t t r i b u t e T a g                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process Attribute definer and tag.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ProcessAttributeTag(ref int bufRem, ref int bufOffset)
        {
            PrnParseConstants.ContType contType;

            PrnParseConstants.OvlAct attrOvlAct = PrnParseConstants.OvlAct.None;

            byte crntByte;

            int attrPos = bufOffset;

            const string descPrefix = "  ";

            string desc = string.Empty;

            //----------------------------------------------------------------//
            //                                                                //
            // Initialise.                                                    //
            //                                                                //
            //----------------------------------------------------------------//

            crntByte = _buf[bufOffset];

            //----------------------------------------------------------------//
            //                                                                //
            // The definer is either:                                         //
            //    attr_ubyte     this indicates that a single-byte attribute  //
            //                   identifier tag follows;                      //
            //    attr_uint16    this indicates that a double-byte attribute  //
            //                   identifier tag follows.                      //
            //                                                                //
            // Note that no double-byte attribute identifiers have yet been   //
            // defined (as at Class/Revision 3.0 of the protocol).            //
            //                                                                //
            //----------------------------------------------------------------//

            if (crntByte == PrnParseConstants.pclxlAttrUbyte)
                _attrIDLen = 1;
            else
                _attrIDLen = 2;

            if (bufRem < (_attrIDLen + 2))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Initiate continuation action, because there is             //
                // insufficient data left in the buffer to determine the next //
                // Attribute tag and the next character (which may be the     //
                // Operator tag (needed to interpret some attribute data      //
                // enumerations), or may be the DataType introduction for the //
                // next attribute).                                           //
                //                                                            //
                // If the Attribute definer was preceded by a DataType        //
                // value/identity sequence (the normal case), take this into  //
                // account in determining the 'backtrack' position ( where to //
                // restart the analysis after further data has been read from //
                // the file).                                                 //
                //                                                            //
                //------------------------------------------------------------//

                int backLen;

                _continuation = true;

                contType = PrnParseConstants.ContType.PCLXL;

                if (_operDataStarted)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // We haven't yet determined the Operator; revert to the  //
                    // beginning of the data for the first attribute.         //
                    //                                                        //
                    //--------------------------------------------------------//

                    _operDataStarted = false;
                    _attrDataStarted = false;

                    backLen = (bufOffset + bufRem) - _operDataStart;
                }
                else if (_attrDataStarted)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Revert to the beginning of the data for the current    //
                    // attribute.                                             //
                    //                                                        //
                    //--------------------------------------------------------//

                    _attrDataStarted = false;

                    backLen = (bufOffset + bufRem) - _attrDataStart;
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Attribute not preceded by DataType value/identity      //
                    // sequence?                                              //
                    // This should only be possible with malformed PCLXL (?)  //
                    //                                                        //
                    //--------------------------------------------------------//

                    backLen = bufRem;
                }

                _linkData.SetBacktrack(contType, -backLen);
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Attribute identifier found.                                //
                //                                                            //
                // Obtain description from Attribute table (or use 'Unknown'  //
                // entry).                                                    //
                //                                                            //
                // Then check whether there is associated Attribute data      //
                // (which may need to know the attribute identity (and, in    //
                // some cases, the operator identity as well) in order for    //
                // enumerated values to be interpreted correctly) to be       //
                // displayed first.                                           //
                //                                                            //
                //------------------------------------------------------------//

                _attrIDFound = true;
                if (_attrIDLen == 1)
                {
                    _attrID1 = _buf[bufOffset + 1];
                    _attrID2 = 0x00;
                }
                else
                {
                    _attrID1 = _buf[bufOffset + 1];
                    _attrID1 = _buf[bufOffset + 2];
                }

                PCLXLAttributes.CheckTag(_attrIDLen,
                                          _attrID1,
                                          _attrID2,
                                          out _,
                                          out _attrEnumerated,
                                          out _attrOperEnumeration,
                                          out _attrUbyteAsAscii,
                                          out _attrUint16AsUnicode,
                                          out _attrValueIsEmbedLength,
                                          out _attrValueIsPCLArray,
                                          out attrOvlAct,
                                          out desc);

                if (!_operIDFound)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // We've not yet reached the Operator identifier tag.     //
                    //                                                        //
                    // With some Attributes, enumerated values depend on the  //
                    // Operator identifier as well as the Attribute           //
                    // identifier, so we have to read the Operator tag before //
                    // processing the Attribute list.                         //
                    //                                                        //
                    // Another reason for reading forward to the Operator     //
                    // tag, before processing the Attribute list, is that     //
                    // some Embedded Data 'chunks' (e.g. those associated     //
                    // with the PassThrough operator, or the ReadFontHeader   //
                    // operator) need to be 'stitched together' if they are   //
                    // to be separately analysed; but we only do this if the  //
                    // chunks are for the same Attribute and Operator,        //
                    // otherwise we need to invoke analysis of any such       //
                    // chunks aggregated so far for an Attribute/Operator.    //
                    //                                                        //
                    // If not already done for this operator, store the start //
                    // point of the first attribute, for use on a subsequent  //
                    // pass.                                                  //
                    //                                                        //
                    // Then adjust pointers, so that we carry on processing   //
                    // (without reporting details) until we find the operator //
                    // tag.                                                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (!_operDataStarted)
                    {
                        _operDataStarted = true;
                        _operDataStart = _attrDataStart;
                    }

                    _attrDataStarted = false;
                    _attrIDFound = false;

                    bufOffset += (_attrIDLen + 1);
                    bufRem -= (_attrIDLen + 1);
                }
                else if (_attrDataStarted)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Revert to stored start point to display attribute data //
                    // (now that we know the attribute identity).             //
                    //                                                        //
                    //--------------------------------------------------------//

                    int tempLen = bufOffset - _attrDataStart;
                    bufRem += tempLen;
                    bufOffset = _attrDataStart;

                    if (_parseType == PrnParse.ParseType.MakeOverlay)
                    {
                        //--------------------------------------------------------//
                        //                                                        //
                        // Set any 'Make Overlay' action required that will       //
                        // affect how the preceding Attribute data will be        //
                        // processed.                                             //
                        //                                                        //
                        //--------------------------------------------------------//

                        PrnParseMakeOvl.CheckActionPCLXLAttr(
                            true,
                            attrOvlAct,
                            _operOvlShow,
                            _attrDataStart,
                            attrPos,
                            _fileOffset,
                            _linkData,
                            _table,
                            _indxOffsetFormat);
                    }
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sufficient information in buffer, so the attribute     //
                    // details can now be analysed and displayed.             //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (_parseType == PrnParse.ParseType.MakeOverlay)
                    {
                        //--------------------------------------------------------//
                        //                                                        //
                        // Set any 'Make Overlay' action required that will       //
                        // affect how the preceding Attribute data will be        //
                        // processed.                                             //
                        //                                                        //
                        //--------------------------------------------------------//

                        _breakpoint = PrnParseMakeOvl.CheckActionPCLXLAttr(
                            false,
                            attrOvlAct,
                            _operOvlShow,
                            _attrDataStart,
                            attrPos,
                            _fileOffset,
                            _linkData,
                            _table,
                            _indxOffsetFormat);
                    }

                    PrnParseConstants.OvlShow makeOvlShow =
                        _linkData.MakeOvlShow;

                    if (_attrValueIsEmbedLength)
                        _rawDataAfterOpTag = true;

                    if (_verboseMode)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Display details of the Attribute Definer tag.      //
                        //                                                    //
                        //----------------------------------------------------//

                        if (crntByte == PrnParseConstants.pclxlAttrUbyte)
                        {
                            ShowElement(bufOffset,
                                         1,
                                         "PCLXL Attribute Def.",
                                         "  attr_ubyte",
                                         true,
                                         _displayMetricsNil,
                                         makeOvlShow,
                                         PrnParseRowTypes.Type.PCLXLAttribute);
                        }
                        else
                        {
                            ShowElement(bufOffset,
                                         1,
                                         "PCLXL Attribute Def.",
                                         "  attr_uint16",
                                         true,
                                         _displayMetricsNil,
                                         makeOvlShow,
                                         PrnParseRowTypes.Type.PCLXLAttribute);
                        }

                        //----------------------------------------------------//
                        //                                                    //
                        // Display details of the Attribute (name) identifier.//
                        //                                                    //
                        //----------------------------------------------------//

                        ShowElement(bufOffset + 1,
                                     _attrIDLen,
                                     "                Name",
                                     descPrefix + desc,
                                     true,
                                     _displayMetricsNil,
                                     makeOvlShow,
                                     PrnParseRowTypes.Type.PCLXLAttribute);
                    }
                    else
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Display details of the Attribute Definer tag and   //
                        // (name) identifier.                                 //
                        //                                                    //
                        //----------------------------------------------------//

                        ShowElement(bufOffset,
                                     _attrIDLen + 1,
                                     "PCLXL Attribute",
                                     descPrefix + desc,
                                     true,
                                     _displayMetricsNil,
                                     makeOvlShow,
                                     PrnParseRowTypes.Type.PCLXLAttribute);
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Update statistics.                                     //
                    //                                                        //
                    //--------------------------------------------------------//

                    PCLXLAttributes.IncrementStatsCount(_attrIDLen,
                                                         _attrID1,
                                                         _attrID2,
                                                         _analysisLevel);

                    //--------------------------------------------------------//
                    //                                                        //
                    // Adjust pointers & flags.                               //
                    //                                                        //
                    //--------------------------------------------------------//

                    _attrIDFound = false;
                    _attrDataStarted = false;

                    bufOffset += (_attrIDLen + 1);
                    bufRem -= (_attrIDLen + 1);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s D a t a T y p e T a g                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Data Type tag.                                                     //
        //                                                                    //
        // The value associated with some data types may form an enumerated   //
        // set, but the interpretation of individual values is dependent on   //
        // the associated attribute.                                          //
        //                                                                    //
        // Since the Attribute tag follows the Data Type and value, full      //
        // processing of a Data Type is delayed until the Attribute identity  //
        // is known.                                                          //
        // Hence processing of Attribute data is performed in two passes.     //
        //                                                                    //
        // Where continuation action is found necessary (because the buffer   //
        // has been exhausted before all expected data has been found), the   //
        // action will take into account any stored start point for the       //
        // Attribute data.                                                    //
        //                                                                    //
        // Note that continuation action should not occur on the second pass, //
        // because the appropriate action has already been invoked on the     //
        // first pass.                                                        //
        // (If it does, then either the design is incorrect, or the Attribute //
        // data cannot all be fitted in the buffer at one time).              //
        //                                                                    //
        // At protocol version 3.0, some enumerated values are dependent on   //
        // the associated Operator identifier, as well as the Attribute       //
        // identifier, so the 'two-phase' analysis is extended to three       //
        // phases to cater for this situation.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ProcessDataTypeTag(ref int bufRem, ref int bufOffset)
        {
            PrnParseConstants.ContType contType;

            PCLXLDataTypes.BaseType baseType = PCLXLDataTypes.BaseType.Unknown;

            PrnParseConstants.OvlShow makeOvlShow;

            byte crntByte;

            int groupSize = 1,
                  unitSize = 1,
                  arraySize = 1,
                  seqHddrLen = 0,
                  opSeqLen = 0;
            bool seqKnown,
                    arrayType = false,
                    invalidArray,
                    flagReserved = false;

            const string descPrefix = "    ";
            string desc = string.Empty;

            //----------------------------------------------------------------//
            //                                                                //
            // Initialise.                                                    //
            //                                                                //
            //----------------------------------------------------------------//

            bool invalidSeqFound = false;
            invalidArray = false;

            makeOvlShow = _linkData.MakeOvlShow;

            crntByte = _buf[bufOffset];

            if (!_attrDataStarted)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Store pointer to start of Attribute data, so that it can   //
                // be re-processed, in the second pass, when the Attribute    //
                // identity is known.                                         //
                //                                                            //
                //------------------------------------------------------------//

                _attrDataStarted = true;
                _attrDataStart = bufOffset;
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Obtain description from Data Type tag table (or use 'Unknown'  //
            // entry).                                                        //
            //                                                                //
            //----------------------------------------------------------------//

            seqKnown = PCLXLDataTypes.CheckTag(crntByte, ref flagReserved, ref arrayType, ref groupSize, ref unitSize, ref baseType, ref desc);

            if (!seqKnown)
            {
                arraySize = 1;
                opSeqLen = 2;

                if (bufRem < opSeqLen)
                    _continuation = true;
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Entry found in Data Type table.                            //
                //                                                            //
                //------------------------------------------------------------//

                if (!arrayType)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Non-array data type.                                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    arraySize = 1;
                    seqHddrLen = 1;
                    opSeqLen = seqHddrLen + (unitSize * groupSize);

                    if (bufRem < opSeqLen)
                    {
                        _continuation = true;
                    }
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Array data type.                                       //
                    //                                                        //
                    // Next byte is expected to be another Data Type          //
                    // identifier, introducing a value representing the       //
                    // number of elements in the array.                       //
                    // This included identifier is expected be of type ubyte  //
                    // or uint16 only.                                        //
                    //                                                        //
                    // Extract this value (checking at each stage that there  //
                    // is sufficient data in the buffer, and invoking a       //
                    // continuation action if necessary).                     //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (bufRem < 2)
                    {
                        _continuation = true;
                    }
                    else
                    {
                        if (_buf[bufOffset + 1] == PrnParseConstants.pclxlDataTypeUbyte)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Count of elements in the array is given by an  //
                            // 8-bit value in the next byte.                  //
                            //                                                //
                            //------------------------------------------------//

                            seqHddrLen = 3;

                            if (bufRem < seqHddrLen)
                                _continuation = true;
                            else
                                arraySize = _buf[bufOffset + 2];
                        }
                        else if (_buf[bufOffset + 1] == PrnParseConstants.pclxlDataTypeUint16)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Count of elements in the array is given by a   //
                            // 16-bit value in the next two bytes.            //
                            //                                                //
                            //------------------------------------------------//

                            seqHddrLen = 4;

                            if (bufRem < seqHddrLen)
                            {
                                _continuation = true;
                            }
                            else
                            {
                                if (_bindType == PrnParseConstants.PCLXLBinding.BinaryMSFirst)
                                {
                                    arraySize = (_buf[bufOffset + 2] * 256) +
                                                 _buf[bufOffset + 3];
                                }
                                else
                                {
                                    arraySize = (_buf[bufOffset + 3] * 256) +
                                                 _buf[bufOffset + 2];
                                }
                            }
                        }
                        else
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Invalid tag.                                   //
                            //                                                //
                            //------------------------------------------------//

                            invalidSeqFound = true;
                            invalidArray = true;

                            seqHddrLen = 2;
                            arraySize = 0;

                            if (bufRem < opSeqLen)
                                _continuation = true;
                        }

                        if (arraySize < 0)
                        {
                            PrnParseCommon.AddTextRow(
                                PrnParseRowTypes.Type.MsgWarning,
                                _table,
                                PrnParseConstants.OvlShow.Illegal,
                                string.Empty,
                                "*** Warning ***",
                                string.Empty,
                                "Invalid array size");

                            arraySize = 0;
                        }

                        opSeqLen = seqHddrLen + (arraySize * groupSize * unitSize);

                        if (bufRem < opSeqLen)
                            _continuation = true;
                    }
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Data type characteristics established.                         //
            // If sufficient data has been read, and this is the 'second      //
            // pass' (i.e. the Attribute identity is known) then process the  //
            // data type and value.                                           //
            //                                                                //
            //----------------------------------------------------------------//

            if (_continuation)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Continuation action required.                              //
                // No further processing this time round the loop.            //
                //                                                            //
                //------------------------------------------------------------//
            }
            else if (_attrIDFound)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Attribute identity is known, so this is the second pass,   //
                // where the attribute data is being reprocessed.             //
                // Display details of the attribute data.                     //
                //                                                            //
                //------------------------------------------------------------//

                //------------------------------------------------------------//
                //                                                            //
                // Update statistics.                                         //
                //                                                            //
                //------------------------------------------------------------//

                PCLXLDataTypes.IncrementStatsCount(crntByte,
                                                  _analysisLevel);

                //------------------------------------------------------------//
                //                                                            //
                // Set display metrics.                                       //
                //                                                            //
                //------------------------------------------------------------//

                _attrDataStarted = false;

                _displayMetricsCrnt.SetData(_attrUbyteAsAscii,
                                             _attrUint16AsUnicode,
                                             arrayType,
                                             _decodeIndentStd,
                                             groupSize,
                                             unitSize,
                                             baseType);

                //------------------------------------------------------------//
                //                                                            //
                // First display details of the Data Type tag.                //
                //                                                            //
                //------------------------------------------------------------//

                if (!arrayType)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Not array type.                                        //
                    // Display details of the Data Type tag.                  //
                    //                                                        //
                    //--------------------------------------------------------//

                    ShowElement(bufOffset,
                                 1,
                                 "PCLXL Data Type",
                                 descPrefix + desc,
                                 true,
                                 _displayMetricsNil,
                                 makeOvlShow,
                                 PrnParseRowTypes.Type.PCLXLDataType);
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Array type.                                            //
                    // Display details of the Data Type tag and the array     //
                    // length definer.                                        //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (!_verboseMode)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Display Data type tag and length definer together. //
                        //                                                    //
                        //----------------------------------------------------//

                        if (_buf[bufOffset + 1] == PrnParseConstants.pclxlDataTypeUbyte)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Length definer is ubyte.                       //
                            //                                                //
                            //------------------------------------------------//

                            ShowElement(bufOffset,
                                         3,
                                         "PCLXL Data Type",
                                         descPrefix + desc,
                                         true,
                                         _displayMetricsNil,
                                         makeOvlShow,
                                         PrnParseRowTypes.Type.PCLXLDataType);
                        }
                        else if (_buf[bufOffset + 1] ==
                            PrnParseConstants.pclxlDataTypeUint16)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Length definer is uint16.                      //
                            //                                                //
                            //------------------------------------------------//

                            ShowElement(bufOffset,
                                         4,
                                         "PCLXL Data Type",
                                         descPrefix + desc,
                                         true,
                                         _displayMetricsNil,
                                         makeOvlShow,
                                         PrnParseRowTypes.Type.PCLXLDataType);
                        }
                        else
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Length definer is invalid.                     //
                            //                                                //
                            //------------------------------------------------//

                            ShowElement(bufOffset,
                                         2,
                                         "PCLXL Data Type",
                                         descPrefix + desc,
                                         true,
                                         _displayMetricsNil,
                                         PrnParseConstants.OvlShow.Illegal,
                                         PrnParseRowTypes.Type.PCLXLDataType);

                            PrnParseCommon.AddTextRow(
                                PrnParseRowTypes.Type.MsgWarning,
                                _table,
                                PrnParseConstants.OvlShow.Illegal,
                                string.Empty,
                                "*** Warning ***",
                                string.Empty,
                                "Invalid array length tag");
                        }
                    }
                    else
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Display Data type tag and length definer           //
                        // separately.                                        //
                        //                                                    //
                        //----------------------------------------------------//

                        ShowElement(bufOffset,
                                     1,
                                     "PCLXL Data Type",
                                     descPrefix + desc,
                                     true,
                                     _displayMetricsNil,
                                     makeOvlShow,
                                     PrnParseRowTypes.Type.PCLXLDataType);

                        if (_buf[bufOffset + 1] == PrnParseConstants.pclxlDataTypeUbyte)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Length definer is ubyte.                       //
                            //                                                //
                            //------------------------------------------------//

                            ShowElement(bufOffset + 1,
                                         1,
                                         "PCLXL Data Type",
                                         "    ubyte",
                                         true,
                                         _displayMetricsNil,
                                         makeOvlShow,
                                         PrnParseRowTypes.Type.PCLXLDataType);

                            ShowElement(bufOffset + 2,
                                         1,
                                         "           Elements",
                                         string.Empty,
                                         false,
                                         _displayMetricsUbyte,
                                         makeOvlShow,
                                         PrnParseRowTypes.Type.PCLXLDataType);
                        }
                        else if (_buf[bufOffset + 1] == PrnParseConstants.pclxlDataTypeUint16)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Length definer is uint16.                      //
                            //                                                //
                            //------------------------------------------------//

                            ShowElement(bufOffset + 1,
                                         1,
                                         "PCLXL Data Type",
                                         "    uint16",
                                         true,
                                         _displayMetricsNil,
                                         makeOvlShow,
                                         PrnParseRowTypes.Type.PCLXLDataType);

                            ShowElement(bufOffset + 2,
                                         2,
                                         "           Elements",
                                         string.Empty,
                                         false,
                                         _displayMetricsUint16,
                                         makeOvlShow,
                                         PrnParseRowTypes.Type.PCLXLDataType);
                        }
                        else
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Length definer is invalid.                     //
                            //                                                //
                            //------------------------------------------------//

                            ShowElement(bufOffset + 1,
                                         1,
                                         "PCLXL Data Type",
                                         "    invalid",
                                         true,
                                         _displayMetricsNil,
                                         PrnParseConstants.OvlShow.Illegal,
                                         PrnParseRowTypes.Type.PCLXLDataType);

                            PrnParseCommon.AddTextRow(
                                PrnParseRowTypes.Type.MsgWarning,
                                _table,
                                PrnParseConstants.OvlShow.Illegal,
                                string.Empty,
                                "*** Warning ***",
                                string.Empty,
                                "Invalid array length tag");
                        }
                    }
                }

                //------------------------------------------------------------//
                //                                                            //
                // Now display details of the Data Type value.                //
                //                                                            //
                //------------------------------------------------------------//

                if (invalidArray)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Array type, but length definer is unrecognised.        //
                    // Hence we can't evaluate the array length.              //
                    //                                                        //
                    //--------------------------------------------------------//
                }
                else if (_attrEnumerated &&
                         (groupSize == 1) &&
                         (!arrayType) &&
                         ((baseType == PCLXLDataTypes.BaseType.Ubyte) ||
                          (baseType == PCLXLDataTypes.BaseType.Sint16) ||
                          (baseType == PCLXLDataTypes.BaseType.Sint32) ||
                          (baseType == PCLXLDataTypes.BaseType.Uint16) ||
                          (baseType == PCLXLDataTypes.BaseType.Uint32)))
                {
                    string enumDesc = string.Empty;

                    uint uiVal;

                    int valLen;
                    if (baseType == PCLXLDataTypes.BaseType.Ubyte)
                    {
                        uiVal = _buf[bufOffset + seqHddrLen];
                        valLen = 1;
                    }
                    else
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Signed or unsigned integer value.                  //
                        //                                                    //
                        // The (2-byte or 4-byte) hexadecimal value is        //
                        // converted, byte by byte, to an unsigned integer    //
                        // (taking into account the current byte-ordering).   //
                        //                                                    //
                        //----------------------------------------------------//

                        uint uiSub;

                        uiVal = 0;

                        if ((baseType == PCLXLDataTypes.BaseType.Sint16) ||
                            (baseType == PCLXLDataTypes.BaseType.Uint16))
                        {
                            valLen = 2;
                        }
                        else
                        {
                            valLen = 4;
                        }

                        if (_bindType == PrnParseConstants.PCLXLBinding.BinaryMSFirst)
                        {
                            for (int j = 0; j < valLen; j++)
                            {
                                uiSub = _buf[bufOffset + seqHddrLen + j];
                                uiVal = (uiVal * 256) + uiSub;
                            }
                        }
                        else
                        {
                            for (int j = valLen - 1; j >= 0; j--)
                            {
                                uiSub = _buf[bufOffset + seqHddrLen + j];
                                uiVal = (uiVal * 256) + uiSub;
                            }
                        }
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Check if the value is in the enumerations table.       //
                    // If the value is known, use the description associated  //
                    // with the value.                                        //
                    // Otherwise, output the value directly.                  //
                    //                                                        //
                    //--------------------------------------------------------//

                    bool flagValIsTxt = false;

                    //--------------------------------------------------------//
                    //                                                        //
                    // An integer (but not array) value can be an enumeration //
                    // value (rather than an analogue quantity), depending on //
                    // the associated Attribute identifier.                   //
                    // Search the enumerated value table.                     //
                    //                                                        //
                    //--------------------------------------------------------//

                    bool valKnown = PCLXLAttrEnums.CheckValue(_analysisLevel,
                                      _crntOperID,
                                      _attrIDLen,
                                      _attrID1,
                                      _attrID2,
                                      uiVal,
                                      _attrOperEnumeration,
                                      ref flagValIsTxt,
                                      ref enumDesc);
                    if (valKnown)
                    {
                        ShowElement(bufOffset + seqHddrLen,
                                     valLen,
                                     "           Value",
                                     descPrefix + enumDesc,
                                     true,
                                     _displayMetricsNil,
                                     makeOvlShow,
                                     PrnParseRowTypes.Type.PCLXLDataValue);
                    }
                    else
                    {
                        int decodeIndent = _displayMetricsCrnt.DecodeIndent;
                        string text;

                        ShowElement(bufOffset + seqHddrLen,
                                     valLen,
                                     "           Value",
                                     string.Empty,
                                     false,
                                     _displayMetricsCrnt,
                                     makeOvlShow,
                                     PrnParseRowTypes.Type.PCLXLDataValue);

                        if (decodeIndent != 0)
                        {
                            text = new string(' ', decodeIndent) + "Enumerated value not recognised";
                        }
                        else
                        {
                            text = "Enumerated value not recognised";
                        }

                        PrnParseCommon.AddTextRow(
                            PrnParseRowTypes.Type.MsgWarning,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            string.Empty,
                            "*** Warning ***",
                            string.Empty,
                            text);
                    }
                }
                else if (baseType == PCLXLDataTypes.BaseType.Ubyte)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Byte: unitSize will always be 1.                       //
                    //                                                        //
                    // Note that only the unsigned byte type has been defined.//
                    //                                                        //
                    //--------------------------------------------------------//

                    if (groupSize == 1)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Not grouped (i.e. not ubyte_xy , ubyte_box, etc.). //
                        //                                                    //
                        //----------------------------------------------------//

                        if (!arrayType)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Single-byte value.                             //
                            // Output value directly.                         //
                            //                                                //
                            //------------------------------------------------//

                            ShowElement(bufOffset + seqHddrLen,
                                         1,
                                         "           Value",
                                         string.Empty,
                                         false,
                                         _displayMetricsCrnt,
                                         makeOvlShow,
                                         PrnParseRowTypes.Type.PCLXLDataValue);
                        }
                        else
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Array of single-byte values.                   //
                            //                                                //
                            //------------------------------------------------//

                            if (!_attrValueIsPCLArray)
                            {
                                ShowElement(bufOffset + seqHddrLen,
                                             arraySize * groupSize,
                                             "           Value",
                                             string.Empty,
                                             false,
                                             _displayMetricsCrnt,
                                             makeOvlShow,
                                             PrnParseRowTypes.Type.PCLXLDataValue);
                            }
                            else
                            {
                                //--------------------------------------------//
                                //                                            //
                                // ubyte_array which is to be interpreted as  //
                                // a PCL string.                              //
                                //                                            //
                                //--------------------------------------------//

                                int tempOffset,
                                      tempLen;

                                string tempText;

                                tempOffset = bufOffset + seqHddrLen;
                                tempLen = arraySize * groupSize;

                                tempText = "of size " + tempLen + " bytes";

                                if (!_analysePCLFontData)
                                {
                                    ShowElement(tempOffset,
                                                 tempLen,
                                                 "           Value",
                                                 string.Empty,
                                                 false,
                                                 _displayMetricsCrnt,
                                                 makeOvlShow,
                                                 PrnParseRowTypes.Type.PCLXLDataValue);
                                }
                                else
                                {
                                    bool badSeq;

                                    PrnParseData.ProcessBinary(
                                        _table,
                                        PrnParseConstants.OvlShow.None,
                                        _buf,
                                        _fileOffset,
                                        tempOffset,
                                        tempLen,
                                        "           Value",
                                        _showBinData,
                                        true,
                                        true,
                                        _indxOffsetFormat,
                                        _analysisLevel);

                                    PrnParseCommon.AddTextRow(
                                        PrnParseRowTypes.Type.MsgComment,
                                        _table,
                                        PrnParseConstants.OvlShow.None,
                                        string.Empty,
                                        ">>>>>>>>>>>>>>>>>>>>",
                                        string.Empty,
                                        ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

                                    PrnParseCommon.AddTextRow(
                                        PrnParseRowTypes.Type.MsgComment,
                                        _table,
                                        PrnParseConstants.OvlShow.None,
                                        string.Empty,
                                        "Comment",
                                        string.Empty,
                                        "Start analysis of embedded PCL string");

                                    PrnParseCommon.AddTextRow(
                                        PrnParseRowTypes.Type.MsgComment,
                                        _table,
                                        PrnParseConstants.OvlShow.None,
                                        string.Empty,
                                        "Comment",
                                        string.Empty,
                                        tempText);

                                    badSeq = _analysisOwner.EmbeddedPCLAnalyse(
                                        _buf,
                                        ref _fileOffset,
                                        ref tempLen,
                                        ref tempOffset,
                                        _linkData,
                                        _options,
                                        _table);

                                    if (badSeq)
                                        invalidSeqFound = true;

                                    PrnParseCommon.AddTextRow(
                                        PrnParseRowTypes.Type.MsgComment,
                                        _table,
                                        PrnParseConstants.OvlShow.None,
                                        string.Empty,
                                        "Comment",
                                        string.Empty,
                                        "End analysis of " +
                                        "embedded PCL string");

                                    PrnParseCommon.AddTextRow(
                                        PrnParseRowTypes.Type.MsgComment,
                                        _table,
                                        PrnParseConstants.OvlShow.None,
                                        string.Empty,
                                        "Comment",
                                        string.Empty,
                                        tempText);

                                    PrnParseCommon.AddTextRow(
                                        PrnParseRowTypes.Type.MsgComment,
                                        _table,
                                        PrnParseConstants.OvlShow.None,
                                        string.Empty,
                                        "<<<<<<<<<<<<<<<<<<<<",
                                        string.Empty,
                                        "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                                }
                            }
                        }
                    }
                    else
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Data is for a grouped item (e.g. ubyte_xy,         //
                        // ubyte_box).                                        //
                        //                                                    //
                        // Note that the current (version 3.0) protocol does  //
                        // NOT define any arrays of grouped values (e.g. an   //
                        // array of ubyte_box elements).                      //
                        //                                                    //
                        //----------------------------------------------------//

                        ShowElement(bufOffset + seqHddrLen,
                                     arraySize * groupSize,
                                     "           Value",
                                     string.Empty,
                                     false,
                                     _displayMetricsCrnt,
                                     makeOvlShow,
                                     PrnParseRowTypes.Type.PCLXLDataValue);
                    }
                }
                else if ((baseType == PCLXLDataTypes.BaseType.Uint16)
                                     &&
                         (groupSize == 1)
                                     &&
                         arrayType && _attrUint16AsUnicode)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Array of uint16 values is to be treated as an array of //
                    // Unicode characters.                                    //
                    //                                                        //
                    //--------------------------------------------------------//

                    ShowElement(bufOffset + seqHddrLen,
                                 arraySize * unitSize,
                                 "           Value (U+)",
                                 string.Empty,
                                 false,
                                 _displayMetricsCrnt,
                                 makeOvlShow,
                                 PrnParseRowTypes.Type.PCLXLDataValue);
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Data is one (or an array of (single or grouped)):      //
                    //    integer (signed or unsigned)                        //
                    //    real    (always signed)                             //
                    //    unknown                                             //
                    //                                                        //
                    //--------------------------------------------------------//

                    ShowElement(bufOffset + seqHddrLen,
                                 arraySize * groupSize * unitSize,
                                 "           Value",
                                 string.Empty,
                                 false,
                                 _displayMetricsCrnt,
                                 makeOvlShow,
                                 PrnParseRowTypes.Type.PCLXLDataValue);
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // If sufficient data has been read, adjust pointers to reference //
            // the next tag (which should be an Attribute tag).               //
            //                                                                //
            //----------------------------------------------------------------//

            if (!_continuation)
            {
                bufOffset += opSeqLen;
                bufRem -= opSeqLen;

                if ((bufRem == 0) && (!_attrIDFound))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // No data remains in buffer, but attribute identifier    //
                    // has not yet been found. Continuation action required.  //
                    //                                                        //
                    //--------------------------------------------------------//

                    _continuation = true;
                }
            }
            if (_continuation)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Reset pointer back to start of attribute data and invoke   //
                // continuation action.                                       //
                //                                                            //
                //------------------------------------------------------------//

                int bufLen = bufOffset + bufRem;
                int backLen;

                contType = PrnParseConstants.ContType.PCLXL;

                if (_operDataStarted)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // We haven't yet determined the Operator; revert to the  //
                    // beginning of the data for the first attribute.         //
                    //                                                        //
                    //--------------------------------------------------------//

                    _operDataStarted = false;
                    _attrDataStarted = false;

                    backLen = bufLen - _operDataStart;
                }
                else if (_attrDataStarted)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // We've already determined the Operator in a previous    //
                    // pass and are re-processing the attribute list from the //
                    // beginning; revert to the beginning of the data for the //
                    // current attribute.                                     //
                    //                                                        //
                    //--------------------------------------------------------//

                    _attrDataStarted = false;

                    backLen = bufLen - _attrDataStart;
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Attribute not preceded by DataType value/identity      //
                    // sequence?                                              //
                    // This should only be possible with malformed PCLXL (?)  //
                    //                                                        //
                    //--------------------------------------------------------//

                    backLen = bufRem;
                }

                _linkData.SetBacktrack(contType, -backLen);
            }

            return invalidSeqFound;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s E m b e d d e d D a t a T a g                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process Embedded Data Tag.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ProcessEmbeddedDataTag(ref int bufRem, ref int bufOffset)
        {
            PrnParseConstants.ContType contType;

            PrnParseConstants.OvlShow makeOvlShow;

            byte crntByte;

            int dataLenSize;

            //----------------------------------------------------------------//
            //                                                                //
            // Initialise.                                                    //
            //                                                                //
            //----------------------------------------------------------------//

            bool invalidSeqFound = false;

            makeOvlShow = _linkData.MakeOvlShow;

            crntByte = _buf[bufOffset];

            if (crntByte == PrnParseConstants.pclxlEmbedDataByte)
                dataLenSize = 1;
            else
                dataLenSize = 4;

            if (bufRem < (dataLenSize + 1))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Initiate continuation action.                              //
                //                                                            //
                //------------------------------------------------------------//

                _continuation = true;

                contType = PrnParseConstants.ContType.PCLXL;

                _linkData.SetBacktrack(contType, -bufRem);
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Interpret length field.                                    //
                //                                                            //
                //------------------------------------------------------------//

                if (dataLenSize == 1)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Single-byte length field.                              //
                    //                                                        //
                    //--------------------------------------------------------//

                    _embedDataRem = _buf[bufOffset + 1];
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Quadruple-byte length field.                           //
                    // Interpret according to stream binding.                 //
                    //                                                        //
                    //--------------------------------------------------------//

                    int x;

                    _embedDataRem = 0;

                    if (_bindType ==
                        PrnParseConstants.PCLXLBinding.BinaryMSFirst)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Binary value with Most Significant byte first.     //
                        //                                                    //
                        //----------------------------------------------------//

                        for (int i = 0; i < 4; i++)
                        {
                            x = _buf[bufOffset + 1 + i];
                            _embedDataRem = (_embedDataRem * 256) + x;
                        }
                    }
                    else if (_bindType == PrnParseConstants.PCLXLBinding.BinaryLSFirst)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Binary value with Least Significant byte first.    //
                        //                                                    //
                        //----------------------------------------------------//

                        for (int i = 3; i >= 0; i--)
                        {
                            x = _buf[bufOffset + 1 + i];
                            _embedDataRem = (_embedDataRem * 256) + x;
                        }
                    }
                    else
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // ASCII binding.                                     //
                        // Not supported in this code (not sure of format,    //
                        // hence not sure how to interpret).                  //
                        // Should not get to here, since ASCII binding should //
                        // be rejected at header analysis stage.              //
                        //                                                    //
                        //----------------------------------------------------//

                        invalidSeqFound = true;

                        PrnParseCommon.AddTextRow(
                            PrnParseRowTypes.Type.MsgWarning,
                            _table,
                            PrnParseConstants.OvlShow.Illegal,
                            string.Empty,
                            "*** Warning ***",
                            string.Empty,
                            "ASCII Binding not supported");

                        _embedDataRem = _buf[bufOffset + 1];
                    }
                }

                //------------------------------------------------------------//
                //                                                            //
                // Display details of embedded data introductory fields.      //
                //                                                            //
                //------------------------------------------------------------//

                if (dataLenSize == 1)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Embedded data (length value is single-byte).           //
                    //                                                        //
                    //--------------------------------------------------------//

                    ShowElement(bufOffset,
                                 1,
                                 "PCLXL Data Type",
                                 "    embedded_data_byte",
                                 true,
                                 _displayMetricsNil,
                                 makeOvlShow,
                                 PrnParseRowTypes.Type.PCLXLDataType);

                    ShowElement(bufOffset + 1,
                                 1,
                                 "PCLXL Embedded Len.",
                                 string.Empty,
                                 false,
                                 _displayMetricsEmbedByte,
                                 makeOvlShow,
                                 PrnParseRowTypes.Type.PCLXLDataValue);
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Embedded data (length value is quadruple-byte).        //
                    //                                                        //
                    //--------------------------------------------------------//

                    ShowElement(bufOffset,
                                 1,
                                 "PCLXL Data Type",
                                 "    embedded_data",
                                 true,
                                 _displayMetricsNil,
                                 makeOvlShow,
                                 PrnParseRowTypes.Type.PCLXLDataType);

                    ShowElement(bufOffset + 1,
                                 4,
                                 "PCLXL Embedded Len.",
                                 string.Empty,
                                 false,
                                 _displayMetricsEmbedWord,
                                 makeOvlShow,
                                 PrnParseRowTypes.Type.PCLXLDataValue);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Adjust pointers to start of embedded data value.           //
                // Then check whether the embedded data is all contained in   //
                // the current buffer.                                        //
                //                                                            //
                //------------------------------------------------------------//

                bufOffset += dataLenSize + 1;
                bufRem -= dataLenSize + 1;

                if (_embedDataRem < 0)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Invalid data length.                                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgWarning,
                        _table,
                        PrnParseConstants.OvlShow.Illegal,
                        string.Empty,
                        "*** Warning ***",
                        string.Empty,
                        "Invalid data length value");

                    _embedDataLen = 0;
                    _embedDataRem = 0;
                }
                else if (_embedDataRem > bufRem)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Embedded data is not all contained in the current      //
                    // buffer.                                                //
                    // Set markers for continuation action.                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    _continuation = true;

                    _embedDataRem -= bufRem;
                    _embedDataLen = bufRem;

                    contType = PrnParseConstants.ContType.PCLXLEmbed;

                    _linkData.SetContinuation(contType);
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Embedded data is all contained in the current buffer.  //
                    //                                                        //
                    //--------------------------------------------------------//

                    _embedDataLen = _embedDataRem;
                    _embedDataRem = 0;
                }

                //------------------------------------------------------------//
                //                                                            //
                // First check whether we need to store this embedded data    //
                // for later (embedded) analysis.                             //
                // Then display (first or only part of) embedded data as a    //
                // binary array.                                              //
                //                                                            //
                //------------------------------------------------------------//

                if (_crntOperEmbedType != PCLXLOperators.EmbedDataType.None)
                {
                    if (((_crntOperEmbedType == PCLXLOperators.EmbedDataType.PassThrough) && _analysePassThrough)
                                                 ||
                        ((_crntOperEmbedType == PCLXLOperators.EmbedDataType.Stream) && _analyseStreams)
                                                 ||
                        ((_crntOperEmbedType == PCLXLOperators.EmbedDataType.FontHeader) && _analyseFontHddr)
                                                 ||
                        ((_crntOperEmbedType == PCLXLOperators.EmbedDataType.FontChar) && _analyseFontChar))
                    {
                        _analysisOwner.EmbeddedDataStore(_buf, bufOffset, _embedDataLen);
                    }
                }

                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.OvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    _embedDataLen,
                    "               Data",
                    _showBinData,
                    true,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);

                bufOffset += _embedDataLen;
                bufRem -= _embedDataLen;
            }

            return invalidSeqFound;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s H e a d e r                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Format of the stream header is:                                    //
        //                                                                    //
        // byte 0         Binding Format Identifier                           //
        //                ' (0x27) = ASCII (doubt this is allowed)            //
        //                ( (0x28) = binary; high byte first                  //
        //                ) (0x29) = binary; low byte first                   //
        //      1         Reserved: should be space (0x20)                    //
        //      2   -> n  Stream Descriptor String                            //
        //      n+1 ....  Start of PCLXL stream body                          //
        //                                                                    //
        // The Stream Descriptor String is an ASCII string, terminated by a   //
        // LineFeed (0x0a) character. The string should contain at least      //
        // three fields                                                       //
        // (separated by semi-colon (0x3b) characters); only the first three  //
        // fields are mandatory:                                              //
        //                                                                    //
        // field  1        Stream Class Name        (HP-PCL XL)               //
        // field  2        Protocol Class Number    (latest = 3)              //
        // field  3        Protocol Class Revision  (         0)              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ProcessHeader(ref int bufRem, ref int bufOffset)
        {
            bool invalidSeqFound = false;

            PrnParseConstants.ContType contType;

            PrnParseConstants.OvlShow makeOvlShow;

            byte crntByte;
            int termPos = 0;

            for (int i = 0; i < bufRem; i++)
            {
                crntByte = _buf[bufOffset + i];

                if (crntByte == PrnParseConstants.asciiLF)
                {
                    termPos = i;
                    break;
                }
            }

            if (termPos == 0)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Line feed terminator character not found.                  //
                // Initiate continuation action.                              //
                //                                                            //
                //------------------------------------------------------------//

                if (bufRem > 200)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Terminator not found within expected maximum length.   //
                    //                                                        //
                    //--------------------------------------------------------//

                    invalidSeqFound = true;

                    PrnParseCommon.AddDataRow(
                        PrnParseRowTypes.Type.MsgWarning,
                         _table,
                         PrnParseConstants.OvlShow.Illegal,
                         _indxOffsetFormat,
                         _fileOffset + bufOffset,
                         _analysisLevel,
                         "*** Warning ***",
                         string.Empty,
                         "Invalid data length value");

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgWarning,
                         _table,
                         PrnParseConstants.OvlShow.Illegal,
                         string.Empty,
                         "*** Warning ***",
                         string.Empty,
                         "Header terminator not found within 200 bytes");

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgWarning,
                         _table,
                         PrnParseConstants.OvlShow.Illegal,
                         string.Empty,
                         "*** Warning ***",
                         string.Empty,
                         "Assume fragment with no header");

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgWarning,
                         _table,
                         PrnParseConstants.OvlShow.Illegal,
                         string.Empty,
                         "*** Warning ***",
                         string.Empty,
                         "Assume binding is binary (low-byte first)");

                    _bindType = PrnParseConstants.PCLXLBinding.BinaryLSFirst;

                    _hddrRead = true;
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Initiate continuation action.                          //
                    //                                                        //
                    //--------------------------------------------------------//

                    _continuation = true;

                    contType = PrnParseConstants.ContType.PCLXL;

                    _linkData.SetBacktrack(contType, -bufRem);
                }
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Linefeed terminator character found.                       //
                //                                                            //
                //------------------------------------------------------------//

                _hddrRead = true;
                int hddrLen = termPos + 1;
                if (hddrLen < 9)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Header too small.                                      //
                    // Must consist of at least 9 bytes:                      //
                    //    Binding Format Identifier;                          //
                    //    Reserved byte;                                      //
                    //    Descriptor String, as minimum three fields(each     //
                    //    minimum two bytes including terminators);           //
                    //    Linefeed terminator character.                      //
                    //                                                        //
                    //--------------------------------------------------------//

                    invalidSeqFound = true;

                    PrnParseCommon.AddDataRow(
                        PrnParseRowTypes.Type.MsgWarning,
                         _table,
                         PrnParseConstants.OvlShow.Illegal,
                         _indxOffsetFormat,
                         _fileOffset + bufOffset,
                         _analysisLevel,
                         "*** Warning ***",
                         string.Empty,
                         "Header too small");
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Display Stream Header details.                         //
                    //    -  Binding Format Identifier                        //
                    //    -  Reserved byte                                    //
                    //    -  Stream Descriptor String                         //
                    //                                                        //
                    //--------------------------------------------------------//

                    string bindText;

                    if (_buf[bufOffset] == 0x27)
                    {
                        _bindType = PrnParseConstants.PCLXLBinding.ASCII;
                        bindText = "' = ASCII (not supported by analyser)";

                        invalidSeqFound = true;
                    }
                    else if (_buf[bufOffset] == 0x28)
                    {
                        _bindType = PrnParseConstants.PCLXLBinding.BinaryMSFirst;
                        bindText = "( = binary: most significant byte first";

                        if (_parseType == PrnParse.ParseType.MakeOverlay)
                        {
                            invalidSeqFound = true;
                        }
                    }
                    else if (_buf[bufOffset] == 0x29)
                    {
                        _bindType = PrnParseConstants.PCLXLBinding.BinaryLSFirst;
                        bindText = ") = binary: least significant byte first";
                    }
                    else
                    {
                        _bindType = PrnParseConstants.PCLXLBinding.Unknown;
                        bindText = "*** unknown ***";

                        invalidSeqFound = true;
                    }

                    if (!_verboseMode && !invalidSeqFound)
                    {
                        ShowElement(bufOffset,
                                     hddrLen,
                                     "PCLXL Stream Header",
                                     string.Empty,
                                     false,
                                     _displayMetricsHddr,
                                     PrnParseConstants.OvlShow.None,
                                     PrnParseRowTypes.Type.PCLXLStreamHddr);
                    }
                    else
                    {
                        if (invalidSeqFound)
                            makeOvlShow = PrnParseConstants.OvlShow.Illegal;
                        else
                            makeOvlShow = PrnParseConstants.OvlShow.None;

                        PrnParseCommon.AddDataRow(
                            PrnParseRowTypes.Type.PCLXLStreamHddr,
                             _table,
                             PrnParseConstants.OvlShow.None,
                             _indxOffsetFormat,
                             _fileOffset + bufOffset,
                             _analysisLevel,
                             "PCLXL Stream Header",
                             string.Empty,
                             string.Empty);

                        ShowElement(bufOffset,
                                     1,
                                     "      Binding",
                                     bindText,
                                     true,
                                     _displayMetricsNil,
                                     makeOvlShow,
                                     PrnParseRowTypes.Type.PCLXLStreamHddr);

                        ShowElement(bufOffset + 1,
                                     1,
                                     "      Reserved",
                                     string.Empty,
                                     true,
                                     _displayMetricsNil,
                                     PrnParseConstants.OvlShow.None,
                                     PrnParseRowTypes.Type.PCLXLStreamHddr);

                        ShowElement(bufOffset + 2,
                                     hddrLen - 2,
                                     "      Descriptor",
                                     string.Empty,
                                     false,
                                     _displayMetricsHddr,
                                     PrnParseConstants.OvlShow.None,
                                     PrnParseRowTypes.Type.PCLXLStreamHddr);
                    }

                    if ((_bindType == PrnParseConstants.PCLXLBinding.Unknown)
                                              ||
                        (_bindType == PrnParseConstants.PCLXLBinding.ASCII))
                    {
                        PrnParseCommon.AddTextRow(
                            PrnParseRowTypes.Type.MsgComment,
                             _table,
                             PrnParseConstants.OvlShow.Illegal,
                             string.Empty,
                             "Comment",
                             string.Empty,
                             "Assume binding is binary (low-byte first)");

                        _bindType = PrnParseConstants.PCLXLBinding.BinaryLSFirst;
                    }
                }

                bufOffset += hddrLen;
                bufRem -= hddrLen;
            }

            if (invalidSeqFound)
                _linkData.SetContinuation(PrnParseConstants.ContType.Abort);

            return invalidSeqFound;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s O p e r a t o r T a g                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process Operator tag.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ProcessOperatorTag(ref int bufRem, ref int bufOffset)
        {
            bool seqKnown,
                    dummyBool = false,
                    endSession = false;

            int operPos = bufOffset;

            string desc = string.Empty;

            var operOvlAct = PrnParseConstants.OvlAct.None;

            //----------------------------------------------------------------//
            //                                                                //
            // Initialise.                                                    //
            //                                                                //
            //----------------------------------------------------------------//

            _operIDFound = true;
            _crntOperID = _buf[bufOffset];

            _prevOperEmbedType = _crntOperEmbedType;

            //----------------------------------------------------------------//
            //                                                                //
            // Obtain description and flags from Operator tag table (or use   //
            // 'Unknown' entry).                                              //
            //                                                                //
            //----------------------------------------------------------------//

            seqKnown = PCLXLOperators.CheckTag(_crntOperID,
                                                ref endSession,
                                                ref dummyBool,
                                                ref _crntOperEmbedType,
                                                ref operOvlAct,
                                                ref desc);

            if (endSession)
                _streamActive = false;

            if (_prevOperEmbedType != PCLXLOperators.EmbedDataType.None)
            {
                //------------------------------------------------------------//
                //                                                            //
                // The previous operator was of a type (e.g. PCLPassThrough,  //
                // ReadFontHeader, ReadChar) which is followed by embedded    //
                // data.                                                      //
                // Process that embedded data.                                //
                //                                                            //
                //------------------------------------------------------------//

                ProcessStoredEmbeddedData(false);
            }

            if (_operDataStarted)
            {
                //------------------------------------------------------------//
                //                                                            //
                // This operator has a (preceding) Attribute List, and this   //
                // is the first time we've encountered the Operator related   //
                // to that list.                                              //
                //                                                            //
                // Revert to the stored start point to display the attribute  //
                // list (now that we know the operator identity).             //
                //                                                            //
                // This is relevant to the cases where an attribute           //
                // enumeration is dependent on the operator identifier as     //
                // well as the attribute identifier (although we process all  //
                // lists/operators in this way now anyway, for other reasons).//
                //                                                            //
                //------------------------------------------------------------//

                int tempLen;

                _operDataStarted = false;
                _attrDataStarted = false;
                _attrIDFound = false;

                tempLen = bufOffset - _operDataStart;
                bufRem += tempLen;
                bufOffset = _operDataStart;

                if (_parseType == PrnParse.ParseType.MakeOverlay)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Set any 'Make Overlay' action required that will       //
                    // affect how the preceding Attribute List will be        //
                    // processed.                                             //
                    //                                                        //
                    //--------------------------------------------------------//

                    PrnParseMakeOvl.CheckActionPCLXLOper(
                        true,
                        true,
                        operOvlAct,
                        _operDataStart,
                        operPos,
                        _fileOffset,
                        _linkData,
                        _table,
                        _indxOffsetFormat);

                    _operOvlShow = _linkData.MakeOvlShow;
                }
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Either we've displayed the attribute list preceding this   //
                // operator, or the operator (e.g. EndChar) has no attribute  //
                // list.                                                      //
                //                                                            //
                //------------------------------------------------------------//

                if (_parseType == PrnParse.ParseType.MakeOverlay)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Check if 'Make Overlay' action required.               //
                    //                                                        //
                    //--------------------------------------------------------//

                    bool operHasAttrList = bufOffset != _attrDataStart;

                    _breakpoint = PrnParseMakeOvl.CheckActionPCLXLOper(
                                    false,
                                    operHasAttrList,
                                    operOvlAct,
                                    _operDataStart,
                                    operPos,
                                    _fileOffset,
                                    _linkData,
                                    _table,
                                    _indxOffsetFormat);

                    _operOvlShow = _linkData.MakeOvlShow;
                }

                //------------------------------------------------------------//
                //                                                            //
                // Display details of the Operator tag.                       //
                //                                                            //
                //------------------------------------------------------------//

                if ((_parseType == PrnParse.ParseType.MakeOverlay) &&
                    (operOvlAct == PrnParseConstants.OvlAct.Replace_0x77))
                {
                    string descText;

                    ShowElement(bufOffset,
                                 1,
                                 "PCLXL Operator",
                                 desc,
                                 true,
                                 _displayMetricsNil,
                                 PrnParseConstants.OvlShow.Remove,
                                 PrnParseRowTypes.Type.PCLXLOperator);

                    if (_linkData.MakeOvlEncapsulate)
                        descText = "SetPageScale (encapsulated within ReadStream structure)";
                    else
                        descText = "SetPageScale";

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.PCLXLOperator,
                         _table,
                         PrnParseConstants.OvlShow.Insert,
                         string.Empty,
                         "PCLXL Operator",
                         "0x77",
                         descText);
                }
                else
                {
                    ShowElement(bufOffset,
                                 1,
                                 "PCLXL Operator",
                                 desc,
                                 true,
                                 _displayMetricsNil,
                                 _operOvlShow,
                                 PrnParseRowTypes.Type.PCLXLOperator);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Display Operator tag position (if option set).             //
                //                                                            //
                //------------------------------------------------------------//

                _operNum++;

                if (_showOperPos)
                {
                    string text;

                    if (_analysisLevel == 0)
                    {
                        text = _operNum.ToString();
                    }
                    else
                    {
                        text = $"{_operNum.ToString()} (within embedded {_crntEmbedType.ToString()})";
                        //   " at level " +_analysisLevel.ToString ();
                    }

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgComment,
                         _table,
                         PrnParseConstants.OvlShow.None,
                         string.Empty,
                         "               No.",
                         string.Empty,
                         text);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Update statistics.                                         //
                //                                                            //
                //------------------------------------------------------------//

                PCLXLOperators.IncrementStatsCount(_crntOperID, _analysisLevel);

                //------------------------------------------------------------//
                //                                                            //
                // Adjust pointers & flags.                                   //
                //                                                            //
                //------------------------------------------------------------//

                _operIDFound = false;
                _operDataStarted = false;
                _attrDataStarted = false;
                _attrDataStart = bufOffset + 1;
                _operDataStart = bufOffset + 1;

                bufOffset++;
                bufRem--;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // P r o c e s s S t o r e d E m b e d d e d D a t a                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // The previous operator was of a type (e.g. PCLPassThrough,          //
        // ReadFontHeader, ReadChar) which is followed by embedded data.      //
        //                                                                    //
        // Determine if the embedded data collected so far:                   //
        //  - Should be processed immediately, now that we've encountered     //
        //    another operator tag (usually preceded by an attribute list),   //
        //    regardless of whether the new operator tag is the same as the   //
        //    previous one or not.                                            //
        //    This applies, for example, to the ReadChar operator.            //
        //  or:                                                               //
        //  - Should be accumulated, until an operator of a different type is //
        //    encountered, before being analysed, as several successive       //
        //    operators of the same type may be needed to describe the data.  //
        //    This applies, for example, to the PCLPassThrough and            //
        //    ReadFontHeader operators.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void ProcessStoredEmbeddedData(bool endReached)
        {
            if (endReached)
            {
                _prevOperEmbedType = _crntOperEmbedType;
                _crntOperEmbedType = PCLXLOperators.EmbedDataType.None;
            }

            if (_prevOperEmbedType == _crntOperEmbedType)
            {
                if ((_prevOperEmbedType == PCLXLOperators.EmbedDataType.FontChar) &&
                    _analyseFontChar)
                {
                    _analysisOwner.EmbeddedPCLXLAnalyse(
                        ToolCommonData.PrintLang.PCLXL,
                        PCLXLOperators.EmbedDataType.FontChar);
                }
            }
            else
            {
                if ((_prevOperEmbedType == PCLXLOperators.EmbedDataType.PassThrough) &&
                    _analysePassThrough)
                {
                    _analysisOwner.EmbeddedPCLXLAnalyse(
                        ToolCommonData.PrintLang.PCL,
                        PCLXLOperators.EmbedDataType.PassThrough);
                }
                else if ((_prevOperEmbedType == PCLXLOperators.EmbedDataType.Stream) &&
                    _analyseStreams)
                {
                    _analysisOwner.EmbeddedPCLXLAnalyse(
                        ToolCommonData.PrintLang.PCLXL,
                        PCLXLOperators.EmbedDataType.Stream);
                }
                else if ((_prevOperEmbedType == PCLXLOperators.EmbedDataType.FontHeader) &&
                    _analyseFontHddr)
                {
                    _analysisOwner.EmbeddedPCLXLAnalyse(
                        ToolCommonData.PrintLang.PCLXL,
                        PCLXLOperators.EmbedDataType.FontHeader);
                }
                else if ((_prevOperEmbedType == PCLXLOperators.EmbedDataType.FontChar) &&
                    _analyseFontChar)
                {
                    _analysisOwner.EmbeddedPCLXLAnalyse(
                        ToolCommonData.PrintLang.PCLXL,
                        PCLXLOperators.EmbedDataType.FontChar);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s W h i t e S p a c e T a g                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process ...                                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ProcessWhiteSpaceTag(ref int bufRem, ref int bufOffset)
        {
            byte crntByte;

            string desc = string.Empty,
                   mnemonic = string.Empty;

            bool seqKnown;

            //----------------------------------------------------------------//
            //                                                                //
            // Initialise.                                                    //
            //                                                                //
            //----------------------------------------------------------------//

            crntByte = _buf[bufOffset];

            //----------------------------------------------------------------//
            //                                                                //
            // Obtain description from WhiteSpace tag table.                  //
            //                                                                //
            //----------------------------------------------------------------//

            seqKnown = PCLXLWhitespaces.CheckTag(crntByte,
                                                  ref mnemonic,
                                                  ref desc);
            if (seqKnown)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Update statistics.                                         //
                //                                                            //
                //------------------------------------------------------------//

                PCLXLWhitespaces.IncrementStatsCount(crntByte,
                                                      _analysisLevel);

                //------------------------------------------------------------//
                //                                                            //
                // Display details of the WhiteSpace tag.                     //
                //                                                            //
                //------------------------------------------------------------//

                ShowElement(bufOffset,
                             1,
                             "PCLXL Whitespace",
                             mnemonic + ": " + desc,
                             true,
                             _displayMetricsNil,
                             _linkData.MakeOvlShow,
                             PrnParseRowTypes.Type.PCLXLWhiteSpace);

                //------------------------------------------------------------//
                //                                                            //
                // Adjust pointers & flags.                                   //
                //                                                            //
                //------------------------------------------------------------//

                bufOffset++;
                bufRem--;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s h o w E l e m e n t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display details of current element                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ShowElement(int bufOffset,
                                  int dataLen,
                                  string typeText,
                                  string descText,
                                  bool useDesc,
                                  PrnParsePCLXLElementMetrics metrics,
                                  PrnParseConstants.OvlShow makeOvlShow,
                                  PrnParseRowTypes.Type rowType)
        {
            PCLXLDataTypes.BaseType baseType = PCLXLDataTypes.BaseType.Unknown;
            int sliceLen,
                  chunkIpLen,
                  chunkOpLen,
                  chunkOffset,
                  sliceOffset,
                  groupSize = 0,
                  unitSize = 0,
                  decodeIndent = 0,
                  ipPtr;
            bool deferItem = false,
                    arrayType = false,
                    treatUbyteAsAscii = false,
                    treatUint16AsUnicode = false;
            StringBuilder chunkOp = new StringBuilder();

            //----------------------------------------------------------------//
            //                                                                //
            // Initialise.                                                    //
            //                                                                //
            //----------------------------------------------------------------//

            bool firstLine = true;
            bool firstSlice = true;
            bool lastSlice = false;
            bool chunkComplete = false;
            bool stringAscii = false;
            bool stringUnicode = false;
            bool seqError = false;
            sliceOffset = bufOffset;

            if (useDesc)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Interpretation text supplied.                              //
                //                                                            //
                //------------------------------------------------------------//

                sliceLen = _decodeSliceMax;
                arrayType = false;
                baseType = PCLXLDataTypes.BaseType.Unknown;
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Interpretation text not supplied.                          //
                // Use metrics data to determine how to process sequence.     //
                //                                                            //
                //------------------------------------------------------------//

                metrics.GetData(ref treatUbyteAsAscii,
                                 ref treatUint16AsUnicode,
                                 ref arrayType,
                                 ref decodeIndent,
                                 ref groupSize,
                                 ref unitSize,
                                 ref baseType);

                sliceLen = unitSize;
                int decodeMax = _decodeAreaMax - decodeIndent;
                if ((baseType == PCLXLDataTypes.BaseType.Ubyte) && treatUbyteAsAscii)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // ubyte (single or array) to be treated as ASCII         //
                    // character(s).                                          //
                    //                                                        //
                    //--------------------------------------------------------//

                    stringAscii = true;
                    sliceLen = _decodeSliceMax;
                }
                else if ((baseType == PCLXLDataTypes.BaseType.Uint16) && treatUint16AsUnicode)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // uint16 (single or array) to be treated as Unicode      //
                    // UCS-2 character(s).                                    //
                    //                                                        //
                    //--------------------------------------------------------//

                    stringUnicode = true;
                    sliceLen = _decodeSliceMax;
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Loop round, interpreting the data slice by slice.              //
            //                                                                //
            // The input data is examined in 'slices' (of a size suitable for //
            // details to be shown in the Seq column of the display window),  //
            // and is output in 'chunks' (of a size suitable for details to   //
            // be shown in the Desc/Interpretation column).                   //
            //                                                                //
            //----------------------------------------------------------------//

            ipPtr = 0;
            chunkIpLen = 0;
            chunkOffset = sliceOffset;
            chunkOpLen = 0;

            while (ipPtr < dataLen)
            {
                string decode = string.Empty;
                deferItem = false;

                //------------------------------------------------------------//
                //                                                            //
                // Loop step 1:                                               //
                // Select next input slice.                                   //
                //                                                            //
                //------------------------------------------------------------//

                if ((ipPtr + sliceLen) >= dataLen)
                {
                    sliceLen = dataLen - ipPtr;
                    lastSlice = true;
                }

                //------------------------------------------------------------//
                //                                                            //
                // Loop step 2:                                               //
                // Process slice according to data type.                      //
                //                                                            //
                //------------------------------------------------------------//

                if (useDesc)
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
                    //--------------------------------------------------------//
                    //                                                        //
                    // Process slice according to data type, and return       //
                    // decode.                                                //
                    //                                                        //
                    //--------------------------------------------------------//

                    decode = ShowElementDecodeData(ref chunkOp,
                                                    ref chunkIpLen,
                                                    ref chunkOpLen,
                                                    ref chunkComplete,
                                                    ref deferItem,
                                                    ref seqError,
                                                    sliceOffset,
                                                    sliceLen,
                                                    decodeIndent,
                                                    chunkOffset,
                                                    firstSlice,
                                                    lastSlice,
                                                    arrayType,
                                                    stringAscii,
                                                    stringUnicode,
                                                    baseType);
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

                string seq = ShowElementSeqData(sliceLen,
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
                            rowType,
                            _table,
                            makeOvlShow,
                            _indxOffsetFormat,
                            _fileOffset + chunkOffset,
                            _analysisLevel,
                            typeText,
                            seq,
                            decode);
                    }
                    else
                    {
                        PrnParseCommon.AddDataRow(
                            rowType,
                            _table,
                            makeOvlShow,
                            _indxOffsetFormat,
                            _fileOffset + chunkOffset,
                            _analysisLevel,
                            string.Empty,
                            seq,
                            decode);
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

                if (!deferItem)
                {
                    ipPtr += sliceLen;
                    sliceOffset += sliceLen;
                }
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

        private string ShowElementDecodeData(
            ref StringBuilder chunkOp,
            ref int chunkIpLen,
            ref int chunkOpLen,
            ref bool chunkComplete,
            ref bool deferItem,
            ref bool seqError,
            int sliceOffset,
            int sliceLen,
            int decodeIndent,
            int chunkOffset,
            bool firstSlice,
            bool lastSlice,
            bool arrayType,
            bool stringAscii,
            bool stringUnicode,
            PCLXLDataTypes.BaseType baseType)
        {
            StringBuilder decode = new StringBuilder();

            int decodeMax = _decodeAreaMax - decodeIndent;

            int chunkOpRem,
                  itemLen = 0;

            if (stringUnicode)
            {
                //------------------------------------------------------------//
                //                                                            //
                // (Array of) uint16 value(s) is to be treated as (an array   //
                // of) Unicode character(s).                                  //
                // Interpret the value(s) and output the resultant            //
                // character(s) within quotes.                                //
                // Take account of byte ordering before translating.          //
                // Unicode values of U+00FF or less translate directly to     //
                // their ISO-8859-1 equivalent.                               //
                //                                                            //
                //------------------------------------------------------------//

                string showChar;

                int k;

                for (int j = 0; j < sliceLen; j += 2)
                {
                    k = chunkOffset + chunkIpLen + j;

                    showChar = PrnParseData.ProcessBytePair(
                        _buf[k],
                        _buf[k + 1],
                        _bindType == PrnParseConstants.PCLXLBinding.BinaryMSFirst,
                        _indxCharSetSubAct,
                        (byte)_valCharSetSubCode,
                        _indxCharSetName);

                    chunkOp.Append(showChar);
                }

                chunkIpLen += sliceLen;
                chunkOpLen = chunkOp.Length;

                if (_verboseMode || lastSlice ||
                    ((chunkOpLen + 2) >= decodeMax))
                {
                    chunkComplete = true;

                    decode.Clear();

                    if (decodeIndent != 0)
                    {
                        string indent = new string(' ', decodeIndent);

                        decode.Append(indent);
                    }

                    decode.Append('"' + chunkOp.ToString() + '"');
                }
            }
            else if (stringAscii)
            {
                //------------------------------------------------------------//
                //                                                            //
                // (Array of) ubyte value(s) is to be treated as (an array    //
                // of) ASCII character(s).                                    //
                // Interpret the value(s) and output the resultant            //
                // character(s) within quotes.                                //
                //                                                            //
                //------------------------------------------------------------//

                string showChar;

                int k;

                for (int j = 0; j < sliceLen; j++)
                {
                    k = chunkOffset + chunkIpLen + j;

                    showChar = PrnParseData.ProcessByte(
                        _buf[k],
                        _indxCharSetSubAct,
                        (byte)_valCharSetSubCode,
                        _indxCharSetName);

                    chunkOp.Append(showChar);
                }

                chunkIpLen += sliceLen;
                chunkOpLen = chunkOp.Length;

                if (_verboseMode || lastSlice ||
                    ((chunkOpLen + 2) >= decodeMax))
                {
                    chunkComplete = true;

                    decode.Clear();

                    if (decodeIndent != 0)
                    {
                        string indent = new string(' ', decodeIndent);

                        decode.Append(indent);
                    }

                    decode.Append('"' + chunkOp.ToString() + '"');
                }
            }
            else if (baseType == PCLXLDataTypes.BaseType.Ubyte)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Array, group or single ubyte value(s) to be treated as     //
                // numeric value(s).                                          //
                // For arrays or grouped items, separate items with spaces.   //
                //                                                            //
                //------------------------------------------------------------//

                if (arrayType)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Display arrays within () brackets.                     //
                    // Second and subsequent lines of array are indented to   //
                    // to align with first line.                              //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (firstSlice)
                    {
                        chunkOp.Clear();
                        chunkOp.Append("(");
                        chunkOpLen = 1;
                    }
                    else if (chunkOpLen == 0)
                    {
                        chunkOp.Clear();
                        chunkOp.Append(" ");
                        chunkOpLen = 1;
                    }
                }

                chunkOpRem = decodeMax - chunkOpLen;

                if (chunkOpRem <= 0)
                {
                    deferItem = true;
                }
                else
                {
                    if (chunkOpLen != 0)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Insert separation character between elements of    //
                        // array or group.                                    //
                        //                                                    //
                        //----------------------------------------------------//

                        chunkOp.Append(" ");
                        chunkOpLen++;
                        chunkOpRem--;
                    }

                    if (lastSlice && arrayType)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Reserve space for terminating " )" characters.     //
                        //                                                    //
                        //----------------------------------------------------//

                        chunkOpRem -= 2;
                    }

                    if (chunkOpRem <= 0)
                    {
                        deferItem = true;
                    }
                    else
                    {
                        byte b = _buf[sliceOffset];

                        string tempStr = b.ToString();

                        itemLen = tempStr.Length;

                        if (itemLen > chunkOpRem)
                            deferItem = true;
                        else
                            chunkOp.Append(tempStr);
                    }
                }

                if (deferItem)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Insufficient space in current line.                    //
                    // Output accumulated details so far.                     //
                    // 'Defer' flag will cause current item to be             //
                    // re-processed next time round the loop.                 //
                    //                                                        //
                    //--------------------------------------------------------//

                    decode.Append("    " + chunkOp);
                    chunkComplete = true;
                }
                else
                {
                    chunkIpLen += sliceLen;
                    chunkOpLen += itemLen;

                    if (lastSlice && arrayType)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Add in terminating " )" characters.                //
                        //                                                    //
                        //----------------------------------------------------//

                        chunkOp.Append(" )");
                        chunkOpLen += 2;
                    }

                    if (_verboseMode || lastSlice)
                    {
                        //-------------------------------------------------------------//
                        //                                                             //
                        // Copy output line from work buffer to target.                //
                        //                                                             //
                        //-------------------------------------------------------------//

                        decode.Clear();

                        if (decodeIndent != 0)
                        {
                            string indent = new string(' ', decodeIndent);

                            decode.Append(indent);
                        }

                        decode.Append(chunkOp);

                        chunkComplete = true;
                    }
                }
            }
            else if ((baseType == PCLXLDataTypes.BaseType.Uint16)
                                ||
                     (baseType == PCLXLDataTypes.BaseType.Uint32)
                                ||
                     (baseType == PCLXLDataTypes.BaseType.Sint16)
                                ||
                     (baseType == PCLXLDataTypes.BaseType.Sint32)
                                ||
                     (baseType == PCLXLDataTypes.BaseType.Real32))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Integer or floating-point value.                           //
                // Take account of byte ordering before translating.          //
                //                                                            //
                //------------------------------------------------------------//

                if (arrayType)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Display arrays within () brackets.                     //
                    // Second and subsequent lines of array are indented to   //
                    // to align with first line.                              //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (firstSlice)
                    {
                        chunkOp.Clear();
                        chunkOp.Append("(");
                        chunkOpLen = 1;
                    }
                    else if (chunkOpLen == 0)
                    {
                        chunkOp.Clear();
                        chunkOp.Append(" ");
                        chunkOpLen = 1;
                    }
                }

                chunkOpRem = decodeMax - chunkOpLen;

                if (chunkOpRem <= 0)
                {
                    deferItem = true;
                }
                else
                {
                    if (chunkOpLen != 0)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Insert separation character between elements of    //
                        // array or group.                                    //
                        //                                                    //
                        //----------------------------------------------------//

                        chunkOp.Append(" ");
                        chunkOpLen++;
                        chunkOpRem--;
                    }

                    if (lastSlice && arrayType)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Reserve space for terminating " )" characters.     //
                        //                                                    //
                        //----------------------------------------------------//

                        chunkOpRem -= 2;
                    }

                    if (chunkOpRem <= 0)
                    {
                        deferItem = true;
                    }
                    else if (baseType == PCLXLDataTypes.BaseType.Real32)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Real (IEEE 32-bit single-precision floating-point) //
                        // value.                                             //
                        //                                                    //
                        // Decode the value as follows:                       //
                        //                                                    //
                        //  - The 4-byte value is converted, byte by byte, to //
                        //    an unsigned integer (taking into account the    //
                        //    byte-ordering specified by the PCL XL binding). //
                        //                                                    //
                        //  - The resultant integer is then converted back to //
                        //    a byte array (using the host byte ordering).    //
                        //                                                    //
                        //  - This new byte array is then converted to a      //
                        //    floating point value.                           //
                        //                                                    //
                        //  - This value is then converted to its string      //
                        //    representation.                                 //
                        //                                                    //
                        //----------------------------------------------------//

                        uint uiSub,
                               uiTot;

                        byte[] byteArray;

                        float f;

                        string tempStr;

                        uiTot = 0;

                        if (_bindType == PrnParseConstants.PCLXLBinding.BinaryMSFirst)
                        {
                            for (int j = 0; j < sliceLen; j++)
                            {
                                uiSub = _buf[sliceOffset + j];
                                uiTot = (uiTot * 256) + uiSub;
                            }
                        }
                        else
                        {
                            for (int j = sliceLen - 1; j >= 0; j--)
                            {
                                uiSub = _buf[sliceOffset + j];
                                uiTot = (uiTot * 256) + uiSub;
                            }
                        }

                        byteArray = BitConverter.GetBytes(uiTot);

                        f = BitConverter.ToSingle(byteArray, 0);

                        tempStr = f.ToString("F6");

                        itemLen = tempStr.Length;

                        if (itemLen > chunkOpRem)
                            deferItem = true;
                        else
                            chunkOp.Append(tempStr);
                    }
                    else if ((baseType == PCLXLDataTypes.BaseType.Uint16) ||
                             (baseType == PCLXLDataTypes.BaseType.Uint32))
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Unsigned integer value.                            //
                        //                                                    //
                        // Decode the value as follows:                       //
                        //                                                    //
                        //  - The (2-byte or 4-byte) value is converted, byte //
                        //    by byte, to an unsigned integer (taking into    //
                        //    account the byte-ordering specified by the      //
                        //    PCL XL binding).                                //
                        //                                                    //
                        //  - This integer value is then converted to its     //
                        //    string representation.                          //
                        //                                                    //
                        //----------------------------------------------------//

                        uint uiSub,
                               uiTot;

                        string tempStr;

                        uiTot = 0;

                        if (_bindType == PrnParseConstants.PCLXLBinding.BinaryMSFirst)
                        {
                            for (int j = 0; j < sliceLen; j++)
                            {
                                uiSub = _buf[sliceOffset + j];
                                uiTot = (uiTot * 256) + uiSub;
                            }
                        }
                        else
                        {
                            for (int j = sliceLen - 1; j >= 0; j--)
                            {
                                uiSub = _buf[sliceOffset + j];
                                uiTot = (uiTot * 256) + uiSub;
                            }
                        }

                        tempStr = uiTot.ToString();

                        itemLen = tempStr.Length;

                        if (itemLen > chunkOpRem)
                            deferItem = true;
                        else
                            chunkOp.Append(tempStr);

                        if (_attrValueIsEmbedLength)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // One of the special attributes which defines    //
                            // the length of (binary) data to follow the      //
                            // Operator tag.                                  //
                            // Used with host-based streams.                  //
                            //                                                //
                            //------------------------------------------------//

                            _attrDataVal = (int)uiTot;
                        }
                    }
                    else
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Signed integer value.                              //
                        //                                                    //
                        // Decode the value as follows:                       //
                        //                                                    //
                        //  - The (2-byte or 4-byte) value is converted, byte //
                        //    by byte, to a signed integer (taking into       //
                        //    account the byte-ordering specified by the      //
                        //    PCL XL binding).                                //
                        //                                                    //
                        //  - This integer value is then converted to its     //
                        //    string representation.                          //
                        //                                                    //
                        //----------------------------------------------------//

                        int iSub,
                              iTot;

                        bool msByte;

                        string tempStr;

                        iTot = 0;
                        msByte = true;

                        if (_bindType == PrnParseConstants.PCLXLBinding.BinaryMSFirst)
                        {
                            for (int j = 0; j < sliceLen; j++)
                            {
                                iSub = _buf[sliceOffset + j];

                                if (msByte && (iSub > 0x80))
                                    iTot = iSub - 256;
                                else
                                    iTot = (iTot * 256) + iSub;

                                msByte = false;
                            }
                        }
                        else
                        {
                            for (int j = sliceLen - 1; j >= 0; j--)
                            {
                                iSub = _buf[sliceOffset + j];

                                if (msByte && (iSub > 0x80))
                                    iTot = iSub - 256;
                                else
                                    iTot = (iTot * 256) + iSub;

                                msByte = false;
                            }
                        }

                        tempStr = iTot.ToString();
                        itemLen = tempStr.Length;

                        if (itemLen > chunkOpRem)
                            deferItem = true;
                        else
                            chunkOp.Append(tempStr);

                        if (_attrValueIsEmbedLength)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // One of the special attributes which defines    //
                            // the length of (binary) data to follow the      //
                            // Operator tag.                                  //
                            // Used with host-based streams.                  //
                            //                                                //
                            //------------------------------------------------//

                            _attrDataVal = iTot;
                        }
                    }
                }

                if (deferItem)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Insufficient space in current line.                    //
                    // Output accumulated details so far.                     //
                    // 'Defer' flag will cause current item to be             //
                    // re-processed next time round the loop.                 //
                    //                                                        //
                    //--------------------------------------------------------//

                    decode.Clear();

                    if (decodeIndent != 0)
                    {
                        string indent = new string(' ', decodeIndent);

                        decode.Append(indent);
                    }

                    decode.Append(chunkOp);

                    chunkComplete = true;
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Interpretation of current item has already been added  //
                    // to output work area.                                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    chunkIpLen += sliceLen;
                    chunkOpLen += itemLen;

                    if (lastSlice && arrayType)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Add in terminating " )" characters.                //
                        //                                                    //
                        //----------------------------------------------------//

                        chunkOp.Append(" )");
                        chunkOpLen += 2;
                    }

                    if (_verboseMode || lastSlice)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Copy output line from work buffer to target.       //
                        //                                                    //
                        //----------------------------------------------------//

                        decode.Clear();

                        if (decodeIndent != 0)
                        {
                            string indent = new string(' ', decodeIndent);

                            decode.Append(indent);
                        }

                        decode.Append(chunkOp);

                        chunkComplete = true;
                    }
                }
            }
            else
            {
                seqError = true;

                decode.Clear();
                decode.Append("*** unknown type ***");

                chunkComplete = true;
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

        private string ShowElementSeqData(int sliceLen,
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

            if (_verboseMode || seqError)
            {
                //-------------------------------------------------------------------//
                //                                                                   //
                // Convert current slice.                                            //
                //                                                                   //
                //-------------------------------------------------------------------//

                displaySlice = true;
                hexStart = sliceOffset;
                hexEnd = sliceOffset + sliceLen;
            }
            else if ((!_verboseMode) && (chunkComplete || lastSlice))
            {
                //-------------------------------------------------------------------//
                //                                                                   //
                // Convert first few characters of current chunk.                    //
                //                                                                   //
                //-------------------------------------------------------------------//

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
                    sub = _buf[j];
                    sub >>= 4;
                    crntByte = PrnParseConstants.cHexBytes[sub];
                    hexBuf[hexPtr++] = (char)crntByte;

                    sub = _buf[j] & 0x0f;
                    crntByte = PrnParseConstants.cHexBytes[sub];
                    //    hexBuf[hexPtr++] = crntByte;
                    hexBuf[hexPtr++] = (char)crntByte;
                }

                //       hexBuf[hexPtr] = 0x00;
                //       hexBuf2[hexPtr] = (Char) 0x00;

                seq.Append("0x")
                    .Append(hexBuf, 0, hexPtr);

                if (useEllipsis)
                    seq.Append("..");
            }

            return seq.ToString();
        }
    }
}