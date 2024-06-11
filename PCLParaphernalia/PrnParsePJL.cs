using System.Data;
using System.Text;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines functions to parse PJL commands.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal class PrnParsePJL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const int _lenPJLIntro = 4;       // @PJL //
        private const int _maxPJLCmdLen = 1024;
        private const int _maxPJLLineLen = 50;

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

        private PrnParseConstants.OptOffsetFormats _indxOffsetFormat;

        private bool _showPML;

        private PrnParseConstants.OptCharSetSubActs _indxCharSetSubAct;
        private PrnParseConstants.OptCharSets _indxCharSetName;
        private int _valCharSetSubCode;

        private readonly ASCIIEncoding _ascii = new ASCIIEncoding();

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P r n P a r s e P J L                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a r s e B u f f e r                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Parse provided buffer, assuming that the current print language is //
        // PJL.                                                               //
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

            //----------------------------------------------------------------//

            _indxOffsetFormat = _options.IndxGenOffsetFormat;

            _options.GetOptCharSet(ref _indxCharSetName, ref _indxCharSetSubAct, ref _valCharSetSubCode);

            _endOffset = _options.ValCurFOffsetEnd;

            _showPML = _options.FlagPMLWithinPJL;

            //----------------------------------------------------------------//

            if (linkData.IsContinuation())
            {
                seqInvalid = ParseContinuation(ref bufRem, ref bufOffset, ref crntPDL, ref endReached);
            }
            else
            {
                seqInvalid = ParseSequences(ref bufRem, ref bufOffset, ref crntPDL, ref endReached);
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

        private bool ParseContinuation(
            ref int bufRem,
            ref int bufOffset,
            ref ToolCommonData.PrintLang crntPDL,
            ref bool endReached)
        {
            PrnParseConstants.ContType contType = PrnParseConstants.ContType.None;

            int prefixLen = 0,
                  contDataLen = 0,
                  downloadRem = 0;

            bool backTrack = false;

            const bool invalidSeqFound = false;

            byte prefixA = 0x00,
                 prefixB = 0x00;

            _linkData.GetContData(ref contType,
                                   ref prefixLen,
                                   ref contDataLen,
                                   ref downloadRem,
                                   ref backTrack,
                                   ref prefixA,
                                   ref prefixB);

            if ((contType == PrnParseConstants.ContType.PJL)
                             ||
                (contType == PrnParseConstants.ContType.Special)
                             ||
                (contType == PrnParseConstants.ContType.Unknown)
                             ||
                (contType == PrnParseConstants.ContType.Reset))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended with a partial match of a PJL       //
                // sequence, or with insufficient characters to identify      //
                // the type of sequence.                                      //
                // The continuation action has already reset the buffer, so   //
                // now unset the markers.                                     //
                //                                                            //
                //------------------------------------------------------------//

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
            ref bool endReached)
        {
            long startPos;
            bool langSwitch = false;
            bool invalidSeqFound = false;
            bool dummyBool = false;

            bool continuation = false;
            startPos = _fileOffset + bufOffset;

            while (!continuation && !langSwitch && !endReached && (bufRem > 0))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Process data until language-switch or end of buffer, or    //
                // specified end point.                                       //
                //                                                            //
                //------------------------------------------------------------//

                if ((_endOffset != -1) && ((_fileOffset + bufOffset) > _endOffset))
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
                    // PJL.                                                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    langSwitch = true;

                    crntPDL = ToolCommonData.PrintLang.PCL;
                }
                else if (_buf[bufOffset] != PrnParseConstants.asciiAtSign)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Next character is NOT an @ symbol, so it can't be a    //
                    // PJL command.                                           //
                    // Switch to PCL language processing.                     //
                    //                                                        //
                    //--------------------------------------------------------//

                    langSwitch = true;

                    crntPDL = ToolCommonData.PrintLang.PCL;
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence does not start with an Escape character, so   //
                    // it should be a PJL command.                            //
                    // PJL commands should start with @PJL and end with a     //
                    // LineFeed (0x0a) character.                             //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (bufRem < 5)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Insufficient characters remain in buffer to        //
                        // identify the sequence as PJL, so initiate a        //
                        // continuation action.                               //
                        //                                                    //
                        //----------------------------------------------------//

                        continuation = true;

                        PrnParseConstants.ContType contType = PrnParseConstants.ContType.PJL;
                        _linkData.SetBacktrack(contType, -bufRem);
                    }
                    else if (_ascii.GetString(_buf, bufOffset, _lenPJLIntro) != "@PJL")
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Not a PJL sequence.                                //
                        // Display the unexpected sequence up to the next     //
                        // Escape character.                                  //
                        //                                                    //
                        //----------------------------------------------------//

                        invalidSeqFound = true;

                        PrnParseCommon.AddTextRow(
                            PrnParseRowTypes.Type.MsgWarning,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            string.Empty,
                            "*** Warning ***",
                            string.Empty,
                            "Unexpected sequence found");

                        PrnParseData.ProcessLines(
                            _table,
                            PrnParseConstants.OvlShow.None,
                            _linkData,
                            ToolCommonData.PrintLang.PJL,
                            _buf,
                            _fileOffset,
                            bufRem,
                            ref bufRem,
                            ref bufOffset,
                            ref dummyBool,
                            true,
                            true,
                            false,
                            PrnParseConstants.asciiEsc,
                            "Data",
                            0,
                            _indxCharSetSubAct,
                            (byte)_valCharSetSubCode,
                            _indxCharSetName,
                            _indxOffsetFormat,
                            _analysisLevel);
                    }
                    else
                    {
                        //-------------------------------------------------------------//
                        //                                                             //
                        // PJL sequence detected.                                      //
                        //                                                             //
                        //-------------------------------------------------------------//

                        bool badSeq = ProcessPJLCommand(ref bufRem, ref bufOffset, ref continuation, ref langSwitch, ref crntPDL);

                        if (badSeq)
                            invalidSeqFound = true;
                    }
                }
            }

            _linkData.MakeOvlAct = PrnParseConstants.OvlAct.Remove;
            _linkData.MakeOvlSkipBegin = startPos;
            _linkData.MakeOvlSkipEnd = _fileOffset + bufOffset;

            return invalidSeqFound;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s P J L C o m m a n d                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process current PJL command in buffer.                             //
        // Command format is one of:                                          //
        //                                                                    //
        //    @PJL [<CR>]<LF>                                                 //
        //                                                                    //
        //    Can be used to separate real commands, and add clarity to long  //
        //    sets of commands.                                               //
        //                                                                    //
        //    @PJL command [<words>] [<CR>}<LF>                               //
        //                                                                    //
        //    For the COMMENT and ECHO commands only.                         //
        //    <words>               Any string of printable characters        //
        //                          (range 0x21-0xff) or whitespace           //
        //                          (space (0x20) or horizontal tab (0x09)),  //
        //                          starting with a printable character.      //
        //                          The string may be enclosed in quote       //
        //                          characters (0x22), in which case it       //
        //                          cannot include a quote character.         //
        //                                                                    //
        //    @PJL command [modifier : value] [option [= value]] [<CR>}<LF>   //
        //                                                                    //
        //    command               One of the defined set of command names.  //
        //    modifier:value        is present for some commands to indicate  //
        //                          particular personality, or port, etc.     //
        //    option                Present for most commands.                //
        //    value                 Associated with 'option' name or (if      //
        //                          that is not present, the command).        //
        //                          There may be more than one option=value   //
        //                          pair.                                     //
        //                                                                    //
        // Whitespace (space or horizontal tab) characters MUST be present    //
        // between the @PJL introducer and the 'command', and between the     //
        // command and modifier names, and between the modifier value and the //
        // option name.                                                       //
        //                                                                    //
        // Interrupt process if an <Esc> character is encountered.            //
        // Long lines are split into shorter slices.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ProcessPJLCommand(
            ref int bufRem,
            ref int bufOffset,
            ref bool continuation,
            ref bool langSwitch,
            ref ToolCommonData.PrintLang crntPDL)
        {
            byte crntByte;

            char crntChar,
                 normChar;
            int len,
                  cmdLen,
                  cmdRem,
                  langLen,
                  offset,
                  lineStart;

            int quoteStart = 0,
                  quoteEnd = 0;

            bool invalidSeqFound,
                    endLoop,
                    foundTerm,
                    firstLine;

            bool foundStartQuote;
            bool noWhitespace = false;

            string lang,
                   commandName,
                   commandParams,
                   line,
                   showChar,
                   desc = string.Empty;

            StringBuilder seq = new StringBuilder();

            StringBuilder cmd = new StringBuilder();

            StringBuilder cmdPart1 = new StringBuilder();
            StringBuilder cmdPart2 = new StringBuilder();

            cmdPart1.Append("@PJL");

            invalidSeqFound = false;
            foundStartQuote = false;
            firstLine = true;
            langSwitch = false;
            continuation = false;
            foundTerm = false;

            cmdRem = bufRem - _lenPJLIntro;
            offset = bufOffset + _lenPJLIntro;
            cmdLen = _lenPJLIntro;

            //----------------------------------------------------------------//
            //                                                                //
            // Search for termination character.                              //
            // This should be a LineFeed (<LF>, 0x0a) character, but may be   //
            // an Escape character (<Esc>, 0x1b) signalling return to PCL.    //
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

            while ((!foundTerm) && (cmdRem > 0) && (cmdLen < _maxPJLCmdLen))
            {
                crntByte = _buf[offset];

                if (crntByte == PrnParseConstants.asciiLF)
                {
                    foundTerm = true;
                    offset++;
                    cmdLen++;
                    cmdRem--;
                }
                else if (crntByte == PrnParseConstants.asciiEsc)
                {
                    foundTerm = true;
                }
                else
                {
                    offset++;
                    cmdLen++;
                    cmdRem--;
                }
            }

            if ((!foundTerm) && (cmdLen != _maxPJLCmdLen))              // ***** Should this be < rather than != ? ***** //
            {
                //------------------------------------------------------------//
                //                                                            //
                // Termination character not found before buffer exhausted,   //
                // or maximum command length exceeded.                        //
                // Initiate (backtracking) continuation action.               //
                //                                                            //
                //------------------------------------------------------------//

                continuation = true;

                _linkData.SetBacktrack(PrnParseConstants.ContType.PJL, -bufRem);
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Process command.                                           //
                // At this point, we have in the buffer one of:               //
                //  - characters terminated by <LF> (counted in length).      //
                //  - characters terminated by <Esc> (not counted in length). //
                //  - characters not terminated, but maxmimum length.         //
                //                                                            //
                //------------------------------------------------------------//

                cmdRem = cmdLen - _lenPJLIntro;
                offset = bufOffset + _lenPJLIntro;

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
                        showChar = PrnParseData.ProcessByte(
                                        crntByte,
                                        _indxCharSetSubAct,
                                        (byte)_valCharSetSubCode,
                                        _indxCharSetName);

                        cmdPart1.Append(showChar);

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

                    if ((crntByte == PrnParseConstants.asciiSpace)
                                         ||
                        (crntByte == PrnParseConstants.asciiHT))
                    {
                        // nextstage = Whitespace;
                        endLoop = true;
                    }
                    else if ((cmdRem == 2) && (crntByte == PrnParseConstants.asciiCR))
                    {
                        // nextstage = Terminator;
                        endLoop = true;
                    }
                    else if ((cmdRem == 1) && (crntByte == PrnParseConstants.asciiLF))
                    {
                        // nextstage = Terminator;
                        endLoop = true;
                    }
                    else if (crntByte == PrnParseConstants.asciiEquals)
                    {
                        // nextstage = Part2;
                        endLoop = true;
                        noWhitespace = true;
                    }
                    else
                    {
                        crntChar = (char)crntByte;
                        normChar = char.ToUpper(crntChar);
                        cmd.Append(normChar);

                        showChar = PrnParseData.ProcessByte(
                                        crntByte,
                                        _indxCharSetSubAct,
                                        (byte)_valCharSetSubCode,
                                        _indxCharSetName);

                        cmdPart1.Append(showChar);

                        offset++;
                        cmdRem--;
                    }
                }

                //------------------------------------------------------------//
                //                                                            //
                // Check whether command name known.                          //
                //                                                            //
                //------------------------------------------------------------//

                commandName = cmd.ToString();

                bool seqKnown;
                if (commandName?.Length == 0)
                {
                    seqKnown = PJLCommands.CheckCmd(PJLCommands.nullCmdKey, ref desc, _analysisLevel);
                }
                else
                {
                    seqKnown = PJLCommands.CheckCmd(cmd.ToString(), ref desc, _analysisLevel);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Stage 3: look for command remainder.                       //
                //          TODO : split this up into component parts?        //
                //                                                            //
                //------------------------------------------------------------//

                endLoop = false;

                while ((!endLoop) && (cmdRem > 0))
                {
                    crntByte = _buf[offset];

                    if (crntByte == PrnParseConstants.asciiQuote)
                    {
                        if (!foundStartQuote)
                        {
                            foundStartQuote = true;
                            quoteStart = offset;
                        }
                        else
                        {
                            quoteEnd = offset;
                        }
                    }

                    crntChar = (char)crntByte;
                    normChar = char.ToUpper(crntChar);

                    showChar = PrnParseData.ProcessByte(
                                    crntByte,
                                    _indxCharSetSubAct,
                                    (byte)_valCharSetSubCode,
                                    _indxCharSetName);

                    cmdPart2.Append(showChar);

                    if ((crntByte == PrnParseConstants.asciiSpace)
                                         ||
                        (crntByte == PrnParseConstants.asciiHT))
                    {
                    }
                    else if ((crntByte == PrnParseConstants.asciiDEL)
                                         ||
                             (crntByte < PrnParseConstants.asciiSpace))
                    {
                        seq.Append((char)PrnParseConstants.asciiPeriod);
                    }
                    else
                    {
                        seq.Append(normChar);
                    }

                    offset++;
                    cmdRem--;
                }

                //--------------------------------------------------------//
                //                                                        //
                // Stage 4: Output details of sequence.                   //
                //                                                        //
                //--------------------------------------------------------//

                commandParams = cmdPart2.ToString();

                len = commandParams.Length;
                lineStart = 0;

                while (firstLine || (len > 0))
                {
                    if (len > _maxPJLLineLen)
                    {
                        line = commandParams.Substring(lineStart, _maxPJLLineLen);
                        len -= _maxPJLLineLen;
                        lineStart += _maxPJLLineLen;
                    }
                    else
                    {
                        line = commandParams.Substring(lineStart, len);
                        len = 0;
                    }

                    if (firstLine)
                    {
                        firstLine = false;

                        if (!seqKnown)
                        {
                            PrnParseCommon.AddTextRow(
                                PrnParseRowTypes.Type.MsgWarning,
                                _table,
                                PrnParseConstants.OvlShow.None,
                                string.Empty,
                                "*** Warning ***",
                                string.Empty,
                                "Following PJL commmand name not recognised:");
                        }

                        if (noWhitespace)
                        {
                            PrnParseCommon.AddTextRow(
                                PrnParseRowTypes.Type.MsgWarning,
                                _table,
                                PrnParseConstants.OvlShow.None,
                                string.Empty,
                                "*** Warning ***",
                                string.Empty,
                                "Following PJL command name not terminated by space or tab character:");
                        }

                        PrnParseCommon.AddDataRow(
                            PrnParseRowTypes.Type.PJLCommand,
                            _table,
                            PrnParseConstants.OvlShow.Remove,
                            _indxOffsetFormat,
                            _fileOffset + bufOffset,
                            _analysisLevel,
                            "PJL Command",
                            cmdPart1.ToString(),
                            line);
                    }
                    else
                    {
                        PrnParseCommon.AddTextRow(
                            PrnParseRowTypes.Type.PJLCommand,
                            _table,
                            PrnParseConstants.OvlShow.Remove,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            line);
                    }
                }

                //--------------------------------------------------------//
                //                                                        //
                // Stage 5: Do any special processing.                    //
                //                                                        //
                //--------------------------------------------------------//

                commandParams = seq.ToString();
                int seqLen;
                if ((commandName.Length == 5)
                                            &&
                                    (commandName.Substring(0, 5) == "ENTER")
                                            &&
                                    (commandParams.Length > 9)
                                            &&
                                    (commandParams.Substring(0, 9) == "LANGUAGE="))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Enter Language command encountered.                    //
                    //                                                        //
                    //--------------------------------------------------------//

                    langSwitch = true;

                    seqLen = seq.Length;

                    lang = commandParams.Substring(9, seqLen - 9);

                    langLen = lang.Length;

                    if ((langLen >= 5) && (lang.Substring(0, 5) == "PCLXL"))
                    {
                        crntPDL = ToolCommonData.PrintLang.PCLXL;
                    }
                    else if ((langLen >= 7) && (lang.Substring(0, 7) == "PCL3GUI"))
                    {
                        crntPDL = ToolCommonData.PrintLang.PCL3GUI;
                    }
                    else if ((langLen >= 3) && (lang.Substring(0, 3) == "PCL"))
                    {
                        crntPDL = ToolCommonData.PrintLang.PCL;
                    }
                    else if ((langLen >= 10) && (lang.Substring(0, 10) == "POSTSCRIPT"))
                    {
                        crntPDL = ToolCommonData.PrintLang.PostScript;
                    }
                    else if ((langLen >= 4) && (lang.Substring(0, 4) == "HPGL"))
                    {
                        crntPDL = ToolCommonData.PrintLang.HPGL2;
                    }
                    else if ((langLen >= 5) && (lang.Substring(0, 5) == "XL2HB"))
                    {
                        crntPDL = ToolCommonData.PrintLang.XL2HB;
                    }
                    else
                    {
                        crntPDL = ToolCommonData.PrintLang.Unknown;
                    }
                }
                else if (_showPML &&
                         (((commandName.Length == 5) && (commandName.Substring(0, 5) == "DMCMD"))
                            ||
                          ((commandName.Length == 6) && (commandName.Substring(0, 6) == "DMINFO"))))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // PML Device Management                                  //
                    //                                                        //
                    //--------------------------------------------------------//

                    if ((commandParams.Length > 9) && (commandParams.Substring(0, 9) == "ASCIIHEX="))
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // PML sequence; encoded as ASCII HEX.                //
                        // Expected to be enclosed in " quotes and followed   //
                        // by <CR><LF> or <LF> PJL terminator characters.     //
                        // Note that the normalised sequence will have "."    //
                        // characters in place of the <CR> and <LF> control   //
                        // codes.                                             //
                        //                                                    //
                        //----------------------------------------------------//

                        PrnParsePML parsePML = new PrnParsePML();

                        seqLen = quoteEnd - quoteStart - 1;

                        if (seqLen > 0)
                        {
                            invalidSeqFound =
                                parsePML.ProcessPMLASCIIHex(_buf,
                                                            _fileOffset,
                                                            seqLen,
                                                            quoteStart + 1,
                                                            _linkData,
                                                            _options,
                                                            _table);
                        }
                    }
                }

                bufOffset = offset;
                bufRem -= cmdLen;
            }

            return invalidSeqFound;
        }
    }
}