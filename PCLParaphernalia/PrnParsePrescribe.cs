using System.Data;
using System.Text;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides routines associated with 'parsing' of
	/// Kyocera Prescribe commands.
    /// 
    /// © Chris Hutchinson 2017
    /// 
    /// </summary>

    class PrnParsePrescribe
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const int _maxCmdLen = 256;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private PrnParseLinkData _linkData;

        private PrnParseOptions _options;

        private DataTable _table;

        private byte[] _buf;

        private int _analysisLevel;

        private int _fileOffset;
        private int _endOffset;

        private PrnParseConstants.eOptOffsetFormats _indxOffsetFormat;

        private PrnParseConstants.eOptCharSetSubActs _indxCharSetSubAct;
        private PrnParseConstants.eOptCharSets _indxCharSetName;
        private int _valCharSetSubCode;

        private readonly ASCIIEncoding _ascii = new ASCIIEncoding();

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P r n P a r s e P r e s c r i b e                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParsePrescribe()
        {
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f i n d C o m m a n d T e r m i n a t o r                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Search for Prescribe command terminator character.                 //
        // This is a semi-colon (0x3b) character.                             //
        // But we may encounter an Escape character (<Esc>, 0x1b) signalling  //
        // return to PCL.                                                     //
        //                                                                    //
        // This is to make sure that the termination character is in the      //
        // buffer before processing the command, so that we don't have to     //
        // cater for doing a 'continuation' read part way through processing  //
        // the command.                                                       //
        //                                                                    //
        // Initiate continuation action if terminator is not found in buffer, //
        // subject to a maximum command length (to prevent recursive          //
        // continuation actions).                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool findCommandTerminator(
            int bufRem,
            int bufOffset,
            ref int commandLen,
            ref bool continuation)
        {
            PrnParseConstants.eContType contType =
                PrnParseConstants.eContType.None;

            byte crntByte;

            int cmdLen,
                  rem,
                  offset;

            bool foundEnd,
                    foundTerm;

            continuation = false;
            foundEnd = false;
            foundTerm = false;

            rem = bufRem;
            offset = bufOffset;
            cmdLen = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Search for termination character.                              //
            //                                                                //
            //----------------------------------------------------------------//

            while ((!foundEnd) && (rem > 0) && (cmdLen < _maxCmdLen))
            {
                crntByte = _buf[offset];

                if (crntByte == PrnParseConstants.asciiSemiColon)
                {
                    foundTerm = true;
                    foundEnd = true;
                    offset++;
                    cmdLen++;
                    rem--;
                }
                else if (crntByte == PrnParseConstants.asciiEsc)
                {
                    foundEnd = true;
                }
                else
                {
                    offset++;
                    cmdLen++;
                    rem--;
                }
            }

            if ((!foundEnd) && (cmdLen < _maxCmdLen))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Termination character not found before buffer exhausted,   //
                // or maximum command length exceeded.                        //
                // Initiate (backtracking) continuation action.               //
                //                                                            //
                //------------------------------------------------------------//

                continuation = true;

                contType = PrnParseConstants.eContType.Prescribe;

                _linkData.setBacktrack(contType, -bufRem);

                commandLen = 0;
            }
            else
            {
                commandLen = cmdLen;
            }

            return foundTerm;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a r s e B u f f e r                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Parse provided buffer, assuming that the current print language is //
        // Prescribe.                                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool parseBuffer(
            byte[] buf,
            ref int fileOffset,
            ref int bufRem,
            ref int bufOffset,
            ref ToolCommonData.ePrintLang crntPDL,
            ref bool endReached,
            PrnParseLinkData linkData,
            PrnParseOptions options,
            DataTable table)
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

            _analysisLevel = _linkData.AnalysisLevel;

            seqInvalid = false;

            //----------------------------------------------------------------//

            _indxOffsetFormat = _options.IndxGenOffsetFormat;

            _options.getOptCharSet(ref _indxCharSetName,
                                    ref _indxCharSetSubAct,
                                    ref _valCharSetSubCode);

            _endOffset = _options.ValCurFOffsetEnd;

            //----------------------------------------------------------------//

            if (linkData.isContinuation())
                seqInvalid = parseContinuation(ref bufRem,
                                                ref bufOffset,
                                                ref crntPDL,
                                                ref endReached);
            else
                seqInvalid = parseSequences(ref bufRem,
                                             ref bufOffset,
                                             ref crntPDL,
                                             ref endReached);

            if (endReached)
            {
                crntPDL = _linkData.PrescribeCallerPDL;
            }

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

        private bool parseContinuation(
            ref int bufRem,
            ref int bufOffset,
            ref ToolCommonData.ePrintLang crntPDL,
            ref bool endReached)
        {
            PrnParseConstants.eContType contType;

            contType = PrnParseConstants.eContType.None;

            int prefixLen = 0,
                  contDataLen = 0,
                  downloadRem = 0;

            bool backTrack = false;

            bool invalidSeqFound = false;

            byte prefixA = 0x00,
                 prefixB = 0x00;

            _linkData.getContData(ref contType,
                                   ref prefixLen,
                                   ref contDataLen,
                                   ref downloadRem,
                                   ref backTrack,
                                   ref prefixA,
                                   ref prefixB);

            if ((contType == PrnParseConstants.eContType.Prescribe)
                             ||
                (contType == PrnParseConstants.eContType.Reset))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended with a partial match of a Prescribe //
                // command, or with insufficient characters to identify       //
                // the type of sequence.                                      //
                // The continuation action has already reset the buffer, so   //
                // now unset the markers.                                     //
                //                                                            //
                //------------------------------------------------------------//

                _linkData.resetContData();
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

        private bool parseSequences(
            ref int bufRem,
            ref int bufOffset,
            ref ToolCommonData.ePrintLang crntPDL,
            ref bool endReached)
        {
            long startPos;

            bool continuation = false;
            bool langSwitch = false;
            bool badSeq = false;
            bool invalidSeqFound = false;

            continuation = false;
            startPos = _fileOffset + bufOffset;

            if (!_linkData.PrescribeIntroRead)
            {
                if ((_buf[bufOffset] ==
                        PrnParseConstants.prescribeSCRCDelimiter)
                                      &&
                    (_buf[bufOffset + 1] == _linkData.PrescribeSCRC)
                                      &&
                    (_buf[bufOffset + 2] ==
                        PrnParseConstants.prescribeSCRCDelimiter))
                {
                    string seq = _ascii.GetString(_buf, bufOffset, 3);
                    //  String desc = PrescribeCommands.getDescCmdIntro();
                    string desc = string.Empty;

                    PrescribeCommands.checkCmdIntro(ref desc,
                                                     _analysisLevel);

                    PrnParseCommon.addDataRow(
                        PrnParseRowTypes.eType.PrescribeCommand,
                        _table,
                        PrnParseConstants.eOvlShow.Remove,
                        _indxOffsetFormat,
                        _fileOffset + bufOffset,
                        _analysisLevel,
                        "Prescribe",
                        seq,
                        desc);

                    bufOffset += 3;
                    bufRem -= 3;

                    _linkData.PrescribeIntroRead = true;
                }
                else
                {
                    // internal error ??
                }
            }

            while (!continuation && !langSwitch &&
                   !endReached && (bufRem > 0))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Process data until language-switch or end of buffer, or    //
                // specified end point.                                       //
                //                                                            //
                //------------------------------------------------------------//

                if ((_endOffset != -1) &&
                    ((_fileOffset + bufOffset) > _endOffset))
                {
                    endReached = true;
                }
                else if (_buf[bufOffset] == PrnParseConstants.asciiEsc)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Escape character found.                                //
                    // Switch to PCL language processing.                     //
                    //                                                        //
                    // Note that, in theory, only a few escape sequences are  //
                    // expected:                                              //
                    //      <Esc>E          Printer Reset                     //
                    //      <Esc>%-12345X   Universal Exit Language           //
                    // but if we find an escape sequence, it's certainly not  //
                    // Prescribe.                                             //
                    //                                                        //
                    //--------------------------------------------------------//

                    langSwitch = true;

                    crntPDL = ToolCommonData.ePrintLang.PCL;
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence does not start with an Escape character, so   //
                    // it should be a Prescribe command.                      //
                    //                                                        //
                    //--------------------------------------------------------//

                    badSeq = processCommand(ref bufRem,
                                             ref bufOffset,
                                             ref continuation,
                                             ref langSwitch,
                                             ref crntPDL);

                    if (badSeq)
                        invalidSeqFound = true;
                }
            }

            _linkData.MakeOvlAct = PrnParseConstants.eOvlAct.Remove;
            _linkData.MakeOvlSkipBegin = startPos;
            _linkData.MakeOvlSkipEnd = _fileOffset + bufOffset;

            return invalidSeqFound;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s C o m m a n d                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process current Prescribe command in buffer.                       //
        //                                                                    //
        // Interrupt process if an <Esc> character is encountered.            //
        // Long commands are split into shorter slices.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool processCommand(
            ref int bufRem,
            ref int bufOffset,
            ref bool continuation,
            ref bool langSwitch,
            ref ToolCommonData.ePrintLang crntPDL)
        {
            PrnParseConstants.eContType contType =
                PrnParseConstants.eContType.None;

            byte crntByte,
                 cmdParaByte1 = 0x3f;

            char crntChar,
                 normChar;

            int len,
                  cmdLen,
                  cmdRem,
                  cmdStart,
                  offset,
                  lineStart;

            int quoteStart = 0,
                  quoteEnd = 0;

            bool invalidSeqFound,
                    cmdParaByte1Found,
                    endLoop,
                    foundTerm;

            //  Boolean flagWithinQuote;
            bool flagWithinQuoteDouble;
            bool flagWithinQuoteSingle;
            bool cmdKnown = false;
            bool flagCmdExit = false;
            bool flagCmdSetCRC = false;

            string command,
                   commandName,
                   commandDesc = string.Empty;

            StringBuilder cmd = new StringBuilder();

            invalidSeqFound = false;
            foundTerm = false;
            langSwitch = false;

            lineStart = bufOffset;
            foundTerm = false;

            len = bufRem;
            offset = bufOffset;

            continuation = false;
            foundTerm = false;

            cmdRem = bufRem;
            cmdStart = offset;
            cmdLen = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Search for termination character.                              //
            // This should be a Semi-colon (0x3b) character.                  //
            // But we may encounter an Escape character (<Esc>, 0x1b)         //
            // signalling return to PCL?                                      //
            //                                                                //
            // This is to make sure that the termination character is in the  //
            // buffer before processing the command, so that we don't have to //
            // cater for doing a 'continuation' read part way through         //
            // processing the command.                                        //
            //                                                                //
            // Initiate continuation action if terminator is not found in     //
            // buffer, subject to a maximum command length (to prevent        //
            // recursive continuation actions).                               //
            //                                                                //
            //----------------------------------------------------------------//

            //  flagWithinQuote = false;
            flagWithinQuoteDouble = false;
            flagWithinQuoteSingle = false;

            while ((!foundTerm) && (cmdRem > 0) && (cmdLen < _maxCmdLen))
            {
                crntByte = _buf[offset];

                if (crntByte == PrnParseConstants.asciiEsc)
                {
                    foundTerm = true;
                }
                else
                {
                    if (flagWithinQuoteDouble)
                    {
                        if (crntByte == PrnParseConstants.asciiQuote)
                        {
                            flagWithinQuoteDouble = false;
                            //  flagWithinQuote = false;
                            quoteEnd = offset;
                        }
                    }
                    else if (flagWithinQuoteSingle)
                    {
                        if (crntByte == PrnParseConstants.asciiApostrophe)
                        {
                            flagWithinQuoteSingle = false;
                            //  flagWithinQuote = false;
                            quoteEnd = offset;
                        }
                    }
                    else if (crntByte == PrnParseConstants.asciiQuote)
                    {
                        flagWithinQuoteDouble = true;
                        //  flagWithinQuote = true;
                        quoteStart = offset;
                    }
                    else if (crntByte == PrnParseConstants.asciiApostrophe)
                    {
                        flagWithinQuoteSingle = true;
                        //  flagWithinQuote = true;
                        quoteStart = offset;
                    }
                    else if (crntByte == PrnParseConstants.asciiSemiColon)
                    {
                        foundTerm = true;
                    }

                    offset++;
                    cmdLen++;
                    cmdRem--;
                }
            }

            if ((!foundTerm) && (cmdLen < _maxCmdLen))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Termination character not found before buffer exhausted,   //
                // or maximum command length exceeded.                        //
                // Initiate (backtracking) continuation action.               //
                //                                                            //
                //------------------------------------------------------------//

                continuation = true;

                contType = PrnParseConstants.eContType.Prescribe;

                _linkData.setBacktrack(contType, -bufRem);

            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Process command.                                           //
                // At this point, we have in the buffer one of:               //
                //  - characters terminated by <semi-colon> (counted in       //
                //    length).                                                //
                //  - characters terminated by <Esc> (not counted in length). //
                //  - characters not terminated, but maxmimum length.         //
                //                                                            //
                //------------------------------------------------------------//

                cmdRem = cmdLen;
                offset = bufOffset;

                //------------------------------------------------------------//
                //                                                            //
                // Stage 1: look for & skip past whitespace.                  //
                //                                                            //
                //------------------------------------------------------------//

                endLoop = false;

                while ((!endLoop) && (cmdRem > 0))
                {
                    crntByte = _buf[offset];

                    if ((crntByte == PrnParseConstants.asciiSpace)
                                         ||
                        (crntByte == PrnParseConstants.asciiHT))
                    {
                        offset++;
                        cmdRem--;
                    }
                    else if ((crntByte == PrnParseConstants.asciiCR)
                                         ||
                        (crntByte == PrnParseConstants.asciiLF))
                    {
                        offset++;
                        cmdRem--;
                    }
                    else
                    {
                        endLoop = true;
                    }
                }

                //------------------------------------------------------------//
                //                                                            //
                // Stage 2: look for command name.                            //
                //                                                            //
                //------------------------------------------------------------//

                endLoop = false;

                while ((!endLoop) && (cmdRem > 0))
                {
                    crntByte = _buf[offset];

                    //--------------------------------------------------------//
                    //                                                        //
                    // Check for special characters first.                    //
                    //                                                        //
                    //--------------------------------------------------------//

                    if ((cmdRem == 1) &&
                             (crntByte == PrnParseConstants.asciiSemiColon))

                    {
                        // nextstage = Parameters or Terminator;
                        endLoop = true;
                    }
                    else if (((crntByte >= PrnParseConstants.asciiAlphaUCMin)
                                                &&
                              (crntByte <= PrnParseConstants.asciiAlphaUCMax))
                                                ||
                             ((crntByte >= PrnParseConstants.asciiAlphaLCMin)
                                                &&
                              (crntByte <= PrnParseConstants.asciiAlphaLCMax)))
                    {
                        crntChar = (char)crntByte;
                        normChar = char.ToUpper(crntChar);
                        cmd.Append(normChar);

                        offset++;
                        cmdRem--;
                    }
                    else
                    {
                        // nextstage = Parameters or Terminator;
                        endLoop = true;
                    }
                }

                //------------------------------------------------------------//
                //                                                            //
                // Check whether command name known.                          //
                //                                                            //
                //------------------------------------------------------------//

                commandName = cmd.ToString();

                cmdKnown = PrescribeCommands.checkCmd(cmd.ToString(),
                                                       ref commandDesc,
                                                       ref flagCmdExit,
                                                       ref flagCmdSetCRC,
                                                       _analysisLevel);

                //------------------------------------------------------------//
                //                                                            //
                // Stage 3: look for command remainder parameters, or the     //
                // terminator character.                                      //
                //                                                            //
                //------------------------------------------------------------//

                endLoop = false;
                cmdParaByte1Found = false;

                while ((!endLoop) && (cmdRem > 0))
                {
                    crntByte = _buf[offset];

                    if (!cmdParaByte1Found)
                    {
                        if ((crntByte != PrnParseConstants.asciiSpace)
                                             &&
                            (crntByte != PrnParseConstants.asciiHT))
                        {
                            cmdParaByte1 = crntByte;
                            cmdParaByte1Found = true;
                        }
                    }

                    offset++;
                    cmdRem--;
                }

                //------------------------------------------------------------//
                //                                                            //
                // Stage 4: Output details of command.                        //
                // Display sequence (in slices if necessary).                 //
                //                                                            //
                //------------------------------------------------------------//

                command = Encoding.ASCII.GetString(_buf, cmdStart, cmdLen);

                const int indent = 2;

                lineStart = 0;
                len = cmdLen;       // or length of string? //

                int sliceLen,
                      sliceLenMax,
                      sliceStart,
                      sliceOffset,
                      ccAdjust;

                bool firstSlice;

                string seq = string.Empty;

                byte[] seqBuf = new byte[PrnParseConstants.cRptA_colMax_Seq];

                firstSlice = true;
                sliceOffset = 0;

                if (firstSlice)
                    sliceLenMax = PrnParseConstants.cRptA_colMax_Seq;
                else
                    sliceLenMax = PrnParseConstants.cRptA_colMax_Seq - indent;

                sliceStart = bufOffset + sliceOffset;

                while (len > sliceLenMax)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is too large to fit on one output line.       //
                    //                                                        //
                    //--------------------------------------------------------//

                    sliceLen = sliceLenMax;
                    ccAdjust = 0;

                    if (firstSlice)
                    {
                        seq = command.Substring(sliceOffset, sliceLen);

                        PrnParseCommon.addDataRow(
                            PrnParseRowTypes.eType.PrescribeCommand,
                            _table,
                            PrnParseConstants.eOvlShow.Remove,
                            _indxOffsetFormat,
                            _fileOffset + bufOffset + sliceOffset,
                            _analysisLevel,
                            "Prescribe command",
                            seq.ToString(),
                            string.Empty);
                    }
                    else
                    {
                        seq = "  " + // indent number of spaces
                              command.Substring(sliceOffset, sliceLen);

                        PrnParseCommon.addDataRow(
                            PrnParseRowTypes.eType.PrescribeCommand,
                            _table,
                            PrnParseConstants.eOvlShow.Remove,
                            _indxOffsetFormat,
                            _fileOffset + bufOffset + sliceOffset,
                            _analysisLevel,
                            string.Empty,
                            seq.ToString(),
                            string.Empty);
                    }

                    len = len - sliceLen - ccAdjust;
                    sliceOffset = sliceOffset + sliceLen + ccAdjust;
                    sliceStart += (sliceLen + ccAdjust);
                    sliceLenMax = PrnParseConstants.cRptA_colMax_Seq - indent;

                    firstSlice = false;
                }

                //------------------------------------------------------------//
                //                                                            //
                // Display last (or only) slice of sequence.                  //
                //                                                            //
                //------------------------------------------------------------//

                sliceLen = len;
                ccAdjust = 0;

                if (len > 0)
                {
                    if (firstSlice)
                    {
                        seq = command.Substring(sliceOffset, sliceLen);

                        PrnParseCommon.addDataRow(
                            PrnParseRowTypes.eType.PrescribeCommand,
                            _table,
                            PrnParseConstants.eOvlShow.Remove,
                            _indxOffsetFormat,
                            _fileOffset + bufOffset + sliceOffset,
                            _analysisLevel,
                            "Prescribe Command",
                            seq,
                            commandDesc);
                    }
                    else
                    {
                        seq = "  " + // indent number of spaces
                              command.Substring(sliceOffset, sliceLen);

                        PrnParseCommon.addDataRow(
                            PrnParseRowTypes.eType.PrescribeCommand,
                            _table,
                            PrnParseConstants.eOvlShow.Remove,
                            _indxOffsetFormat,
                            _fileOffset + bufOffset + sliceOffset,
                            _analysisLevel,
                            string.Empty,
                            seq,
                            commandDesc);
                    }
                }

                //------------------------------------------------------------//
                //                                                            //
                // Stage 5: Do any special processing.                        //
                //                                                            //
                //------------------------------------------------------------//

                bufOffset = offset;
                bufRem -= cmdLen;

                if (flagCmdExit)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Exit command found and processed.                      //
                    //                                                        //
                    //--------------------------------------------------------//

                    langSwitch = true;

                    crntPDL = _linkData.PrescribeCallerPDL;
                    _linkData.PrescribeIntroRead = false;
                }
                else if (flagCmdSetCRC)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Set Command Recognition Character command found and    //
                    // processed.                                             //
                    //                                                        //
                    //--------------------------------------------------------//

                    _linkData.PrescribeSCRC = cmdParaByte1;

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.eType.MsgComment,
                        _table,
                        PrnParseConstants.eOvlShow.None,
                        string.Empty,
                        "Comment",
                        string.Empty,
                        "Set Prescribe CRC = " + (char)cmdParaByte1);
                }
            }

            return invalidSeqFound;
        }
    }
}