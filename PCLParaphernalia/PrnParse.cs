﻿using Microsoft.Win32;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Navigation;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles 'parsing' of print file.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    [System.Reflection.Obfuscation(Feature = "properties renaming")]
    internal class PrnParse
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum ParseType
        {
            Analyse = 0,
            MakeOverlay,
            ScanForPDL
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private Stream _ipStream;
        private Stream _opStream;
        private Stream _subStream;

        private BinaryReader _binReader;
        private BinaryWriter _binWriter;
        private BinaryWriter _subWriter;

        private readonly ParseType _parseType;
        private PrnParseOptions _options;
        private DataTable _table;

        private ToolCommonData.PrintLang _crntPDL;
        //   private Int32 _perCentMax;

        private long _fileSize;

        private readonly int _analysisLevel;

        private readonly PrnParsePCL _parsePCL;
        private readonly PrnParsePCLXL _parsePCLXL;
        private readonly PrnParseHPGL2 _parseHPGL2;
        private readonly PrnParsePJL _parsePJL;
        private readonly PrnParsePrescribe _parsePrescribe;

        private readonly PrnParseLinkData _linkData;

        private bool _flagDiagFileAccess;
        private bool _PCLXLFirstCall;
        private bool _subFileOpen;
        private bool _subFileCreated;

        private string _prnFilename;
        private string _subFilename;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P r n P a r s e                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParse(ParseType parseType, int analysisLevel)
        {
            _parseType = parseType;

            _analysisLevel = analysisLevel;

            _parseHPGL2 = new PrnParseHPGL2();
            _parsePCL = new PrnParsePCL(_parseType, _parseHPGL2);
            _parsePCLXL = new PrnParsePCLXL(_parseType);
            _parsePJL = new PrnParsePJL();
            _parsePrescribe = new PrnParsePrescribe();

            PrnParseCommon.InitialiseRunType(_parseType);
            PrnParseData.InitialiseRunType(_parseType);

            _linkData = new PrnParseLinkData(this, analysisLevel, 0, PCLXLOperators.EmbedDataType.None);

            _PCLXLFirstCall = true;
            _subFileCreated = false;
            _subFileOpen = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a n a l y s e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Analyse print file.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool Analyse(string prnFilename,
                               PrnParseOptions options,
                               DataTable table)
        //                 ToolPrnAnalyse   owner,
        //                 BackgroundWorker bkWk)
        {
            _options = options;
            _table = table;
            _prnFilename = prnFilename;

            _flagDiagFileAccess = _options.FlagGenDiagFileAccess;

            //  _perCentMax = 0;

            if (!PrnFileOpen(prnFilename, ref _fileSize))
                return false;

            //  analyseAction (bkWk);

            _linkData.FileSize = _fileSize;

            AnalyseAction(PCLXLOperators.EmbedDataType.None);

            PrnFileClose();

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a n a l y s e A c t i o n                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Analyse print file.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void AnalyseAction(PCLXLOperators.EmbedDataType pclxlEmbedType)
        {
            int blockLen,
                  contDataLen = 0,
                  bufRem,
                  bufOffset = 0,
                  blockStart = 0,
                  fileOffset = 0;

            //   Int32 perCent = 0;

            ToolCommonData.PrintLang newPDL;

            PrnParseConstants.OptCharSetSubActs indxCharSetSubAct = 0;
            PrnParseConstants.OptCharSets indxCharSetName = 0;
            int valCharSetSubCode = 0;

            bool backTrack = false;
            const bool rowLimitReached = false;
            bool endReached = false;
            bool invalidSeqFound = false;

            byte[] buf = new byte[PrnParseConstants.bufSize];

            //   bkWk.ReportProgress (perCent);

            //----------------------------------------------------------------//

            _linkData.PclxlEmbedType = pclxlEmbedType;

            PrnParseConstants.OptOffsetFormats indxOffsetFormat = _options.IndxGenOffsetFormat;
            _options.GetOptCharSet(ref indxCharSetName, ref indxCharSetSubAct, ref valCharSetSubCode);

            if (_parseType == ParseType.Analyse)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Action 'current file' start conditions; this:              //
                // - Sets initial PDL (to override default of PCL); _crntPDL  //
                // - Sets PCL XL initial state (for PDL = PCL XL)             //
                // - Returns requested start offset.                          //
                //                                                            //
                //------------------------------------------------------------//

                blockStart = AnalyseActionStart();
            }

            if (_fileSize == 0)
                endReached = true;
            else if (blockStart != 0)
                _ipStream.Seek(blockStart, SeekOrigin.Begin);

            //----------------------------------------------------------------//

            while (!endReached && !rowLimitReached)
            {
                //  if (_analysisLevel == 0)
                //      _progBar->Position = _blockStart;

                //------------------------------------------------------------//
                //                                                            //
                // Read next 'block' of file.                                 //
                // If end-of-file detected, block will be less than full.     //
                //                                                            //
                //------------------------------------------------------------//

                blockLen = _binReader.Read(buf, 0, PrnParseConstants.bufSize);

                if (blockLen == 0)
                {
                    endReached = true;

                    _linkData.SetEof(true);
                }
                else
                {
                    if (blockLen < PrnParseConstants.bufSize)
                    {
                        _linkData.SetEof(true);
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Analyse the current 'block'.                           //
                    //                                                        //
                    //--------------------------------------------------------//

                    fileOffset = blockStart;
                    bufOffset = 0;
                    bufRem = blockLen;
                    contDataLen = 0;

                    while ((bufRem > 0) && (!endReached))
                    {
                        newPDL = _crntPDL;

                        bool badSeq;
                        switch (_crntPDL)
                        {
                            case ToolCommonData.PrintLang.PCL:

                                badSeq = _parsePCL.ParseBuffer(
                                    buf,
                                    ref fileOffset,
                                    ref bufRem,
                                    ref bufOffset,
                                    ref newPDL,
                                    ref endReached,
                                    _linkData,
                                    _options,
                                    _table);
                                break;

                            case ToolCommonData.PrintLang.PCL3GUI:

                                badSeq = _parsePCL.ParseBuffer(
                                    buf,
                                    ref fileOffset,
                                    ref bufRem,
                                    ref bufOffset,
                                    ref newPDL,
                                    ref endReached,
                                    _linkData,
                                    _options,
                                    _table);
                                break;

                            case ToolCommonData.PrintLang.HPGL2:

                                badSeq = _parseHPGL2.ParseBuffer(
                                    buf,
                                    ref fileOffset,
                                    ref bufRem,
                                    ref bufOffset,
                                    ref newPDL,
                                    ref endReached,
                                    _linkData,
                                    _options,
                                    _table);
                                break;

                            case ToolCommonData.PrintLang.PJL:

                                badSeq = _parsePJL.ParseBuffer(
                                    buf,
                                    ref fileOffset,
                                    ref bufRem,
                                    ref bufOffset,
                                    ref newPDL,
                                    ref endReached,
                                    _linkData,
                                    _options,
                                    _table);
                                break;

                            case ToolCommonData.PrintLang.PCLXL:

                                badSeq = _parsePCLXL.ParseBuffer(
                                    buf,
                                    ref fileOffset,
                                    ref bufRem,
                                    ref bufOffset,
                                    ref newPDL,
                                    ref endReached,
                                    _linkData,
                                    _options,
                                    _table,
                                    _PCLXLFirstCall);

                                _PCLXLFirstCall = false;

                                break;

                            case ToolCommonData.PrintLang.XL2HB:

                                badSeq = _parsePCLXL.ParseBuffer(
                                    buf,
                                    ref fileOffset,
                                    ref bufRem,
                                    ref bufOffset,
                                    ref newPDL,
                                    ref endReached,
                                    _linkData,
                                    _options,
                                    _table,
                                    _PCLXLFirstCall);

                                _PCLXLFirstCall = false;

                                break;

                            case ToolCommonData.PrintLang.Prescribe:

                                badSeq = _parsePrescribe.ParseBuffer(
                                    buf,
                                    ref fileOffset,
                                    ref bufRem,
                                    ref bufOffset,
                                    ref newPDL,
                                    ref endReached,
                                    _linkData,
                                    _options,
                                    _table);

                                break;

                            default:

                                badSeq = true;

                                PrnParseCommon.AddTextRow(
                                    PrnParseRowTypes.Type.MsgWarning,
                                    _table,
                                    PrnParseConstants.OvlShow.None,
                                    string.Empty,
                                    "*** Warning ***",
                                    string.Empty,
                                    "Unknown language; revert to PCL");

                                newPDL = ToolCommonData.PrintLang.PCL;
                                endReached = true;      // TEMP ??????????
                                break;
                        }

                        //----------------------------------------------------//
                        //                                                    //
                        // Check for and report on language switch.           //
                        //                                                    //
                        //----------------------------------------------------//

                        if (newPDL != _crntPDL)
                        {
                            bool makeMacroScan,
                                    makeMacroRun;

                            string langName;

                            makeMacroScan = _parseType == ParseType.ScanForPDL;
                            makeMacroRun = _parseType == ParseType.MakeOverlay;

                            if (makeMacroScan)
                                endReached = true;

                            switch (newPDL)
                            {
                                case ToolCommonData.PrintLang.PCL:
                                    langName = "PCL";

                                    if (makeMacroScan || makeMacroRun)
                                    {
                                        if (_crntPDL == ToolCommonData.PrintLang.Prescribe)
                                            endReached = false;
                                    }

                                    break;

                                case ToolCommonData.PrintLang.PCL3GUI:
                                    langName = "PCL3GUI";
                                    break;

                                case ToolCommonData.PrintLang.PCLXL:
                                    langName = "PCLXL";
                                    break;

                                case ToolCommonData.PrintLang.HPGL2:
                                    langName = "HP-GL/2";
                                    break;

                                case ToolCommonData.PrintLang.PJL:
                                    langName = "PJL";

                                    if (makeMacroScan || makeMacroRun)
                                    {
                                        if (_linkData.IsEofSet && (bufRem == 0))
                                            newPDL = ToolCommonData.PrintLang.PCL;
                                        else
                                            endReached = false;
                                    }

                                    break;

                                case ToolCommonData.PrintLang.PostScript:
                                    langName = "PostScript";
                                    break;

                                case ToolCommonData.PrintLang.Prescribe:
                                    langName = "Prescribe";

                                    if (makeMacroScan || makeMacroRun)
                                    {
                                        if (bufOffset == 0)
                                            endReached = false;
                                    }

                                    break;

                                case ToolCommonData.PrintLang.XL2HB:
                                    langName = "XL2HB (Brother GDI (PCL XL based))";
                                    break;

                                default:
                                    langName = "Unknown";
                                    break;
                            }

                            PrnParseCommon.AddTextRow(
                                PrnParseRowTypes.Type.MsgComment,
                                _table,
                                PrnParseConstants.OvlShow.None,
                                string.Empty,
                                "Comment",
                                string.Empty,
                                "Switch language to " + langName);

                            _crntPDL = newPDL;
                        }

                        //----------------------------------------------------//
                        //                                                    //
                        // Check if invalid sequence found, or 'continuation' //
                        // action necessary.                                  //
                        //                                                    //
                        //----------------------------------------------------//

                        if (badSeq)
                            invalidSeqFound = true;

                        backTrack = false;

                        if ((_parseType == ParseType.MakeOverlay)
                                      &&
                            (_linkData.MakeOvlAct !=
                                PrnParseConstants.OvlAct.None))
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Make Overlay run break-point.                  //
                            // Update the output file accordingly.            //
                            // Then reset the current read point ready for    //
                            // continuing analysis.                           //
                            //                                                //
                            //------------------------------------------------//

                            endReached = PrnParseMakeOvl.Breakpoint(
                                _linkData,
                                _ipStream,
                                _binReader,
                                _binWriter);

                            _ipStream.Seek(blockStart + blockLen, SeekOrigin.Begin);
                        }

                        if (_linkData.IsContinuation())
                        {
                            if (_linkData.GetContType() == PrnParseConstants.ContType.Abort)
                            {
                                endReached = true;

                                PrnParseCommon.AddTextRow(
                                    PrnParseRowTypes.Type.MsgError,
                                    _table,
                                    PrnParseConstants.OvlShow.Terminate,
                                    string.Empty,
                                    "*** Error ***",
                                    string.Empty,
                                    "Invalid sequences force abort");

                                _linkData.SetContinuation(PrnParseConstants.ContType.None);
                            }
                            else
                            {
                                //--------------------------------------------//
                                //                                            //
                                // Continuation situation.                    //
                                // Set the file pointer back to the start of  //
                                // the interrupted sequence.                  //
                                //                                            //
                                //--------------------------------------------//

                                backTrack = _linkData.BackTrack;

                                if (backTrack)
                                {
                                    _linkData.SetEof(false);

                                    contDataLen = _linkData.DataLen;

                                    if (contDataLen > 0)
                                        _ipStream.Seek(contDataLen, SeekOrigin.Begin);
                                    else
                                        _ipStream.Seek(contDataLen, SeekOrigin.Current);

                                    bufRem = 0;
                                }
                            }
                        }
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Increment 'block' offset value.                        //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (backTrack)
                    {
                        if (contDataLen > 0)
                            blockStart = contDataLen;
                        else if ((blockLen + contDataLen) == 0)
                            _linkData.SetEof(true);
                        else
                            blockStart = blockStart + blockLen + contDataLen;
                    }
                    else
                    {
                        blockStart += blockLen;
                    }

                    if (_linkData.IsEofSet)
                        endReached = true;
                }

                //------------------------------------------------------------//
                //                                                            //
                // Check whether (unsatisfied) continuation action signalled. //
                //                                                            //
                //------------------------------------------------------------//

                if (endReached && (_parseType == ParseType.Analyse))
                {
                    if (_linkData.IsContinuation())
                    {
                        bool dummyBool = false;

                        invalidSeqFound = true;

                        PrnParseCommon.AddTextRow(
                            PrnParseRowTypes.Type.MsgWarning,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            string.Empty,
                            "*** Warning ***",
                            string.Empty,
                            "Continuation signalled, but end-of-file");

                        bufRem = contDataLen;
                        if (bufRem < 0)
                            bufRem = -bufRem;

                        while (bufRem > 0)
                        {
                            PrnParseData.ProcessLines(
                                _table,
                                PrnParseConstants.OvlShow.None,
                                _linkData,
                                _crntPDL,
                                buf,
                                fileOffset,
                                bufRem,
                                ref bufRem,
                                ref bufOffset,
                                ref dummyBool,
                                true,
                                true,
                                false,
                                PrnParseConstants.asciiEsc,
                                "Unknown",
                                0,
                                indxCharSetSubAct,
                                (byte)valCharSetSubCode,
                                indxCharSetName,
                                indxOffsetFormat,
                                _analysisLevel);
                        }
                    }
                    else
                    {
                        if (_crntPDL == ToolCommonData.PrintLang.PCLXL)
                        {
                            _parsePCLXL.ProcessStoredEmbeddedData(true);
                        }
                    }
                }

                //------------------------------------------------------------//
                //                                                            //
                // Hide progress bar and display analysis details.            //
                //                                                            //
                //------------------------------------------------------------//

                /*
                if (_analysisLevel == 0)
                    _progBar->Hide();

                _stdAnalysis->ReadyRows();
                */
            }

            //----------------------------------------------------------------//
            //                                                                //
            // End-point (end-of-file or row-limit) reached.                  //
            //                                                                //
            //----------------------------------------------------------------//

            if (_parseType == ParseType.MakeOverlay)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Make Overlay run end-of-file action.                       //
                // Update the output file accordingly.                        //
                //                                                            //
                //------------------------------------------------------------//

                _linkData.MakeOvlAct = PrnParseConstants.OvlAct.EndOfFile;

                PrnParseMakeOvl.Breakpoint(_linkData,
                                            _ipStream,
                                            _binReader,
                                            _binWriter);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Display pop-up if invalid sequence(s) detected.                //
            //                                                                //
            //----------------------------------------------------------------//

            if (invalidSeqFound)
            {
                MessageBox.Show($"Invalid sequence(s) detected during level {_analysisLevel} analysis.\r\nFile {_prnFilename}\r\nSize {_fileSize} bytes.",
                                 "Invalid Sequence(s)",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a n a l y s e A c t i o n S t a r t                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check for start conditions specific to current file.               //
        // Return Requested start offset.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private int AnalyseActionStart()
        {
            int offsetStart = 0,
                  offsetEnd = -1;

            PrnParseConstants.PCLXLBinding indxXLBinding = PrnParseConstants.PCLXLBinding.Unknown;

            //----------------------------------------------------------------//

            _options.GetOptCurFBasic(ref _crntPDL,
                                      ref indxXLBinding,
                                      ref offsetStart,
                                      ref offsetEnd);

            if (_analysisLevel == 0)
            {
                if (_crntPDL == ToolCommonData.PrintLang.PCL)
                {
                    // do nothing
                }
                else if (_crntPDL == ToolCommonData.PrintLang.PCLXL)
                {
                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgComment,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Comment",
                        string.Empty,
                        "Start Language = PCLXL requested");

                    if (indxXLBinding == PrnParseConstants.PCLXLBinding.Unknown)
                    {
                        PrnParseCommon.AddTextRow(
                            PrnParseRowTypes.Type.MsgComment,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            string.Empty,
                            "Comment",
                            string.Empty,
                            "Stream Header not yet read");
                    }
                    else if (indxXLBinding == PrnParseConstants.PCLXLBinding.BinaryLSFirst)
                    {
                        PrnParseCommon.AddTextRow(
                            PrnParseRowTypes.Type.MsgComment,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            string.Empty,
                            "Comment",
                            string.Empty,
                            "Stream Header assumed: binary low-byte first");
                    }
                    else if (indxXLBinding == PrnParseConstants.PCLXLBinding.BinaryMSFirst)
                    {
                        PrnParseCommon.AddTextRow(
                            PrnParseRowTypes.Type.MsgComment,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            string.Empty,
                            "Comment",
                            string.Empty,
                            "Stream Header assumed: binary high-byte first");
                    }
                }
                else if (_crntPDL == ToolCommonData.PrintLang.HPGL2)
                {
                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgComment,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Comment",
                        string.Empty,
                        "Start Language = HP-GL/2 requested");
                }
                else if (_crntPDL == ToolCommonData.PrintLang.PJL)
                {
                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgComment,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Comment",
                        string.Empty,
                        "Start Language = PJL requested");
                }
                else if (_crntPDL == ToolCommonData.PrintLang.PCLXL)
                {
                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgComment,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Comment",
                        string.Empty,
                        "Start Language = PostScript requested");
                }
                else
                {
                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgComment,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Comment",
                        string.Empty,
                        "Unknown Start Language requested");
                }

                if (offsetStart != 0)
                {
                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgComment,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Comment",
                        string.Empty,
                        $"Start Offset   = {offsetStart} (0x{offsetStart:X8}) requested");
                }

                if (offsetEnd != -1)
                {
                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgComment,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Comment",
                        string.Empty,
                        $"End   Offset   = {offsetEnd} (0x{offsetEnd:X8}) requested");
                }
            }

            if (_fileSize == 0)
            {
                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.Type.MsgComment,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Comment",
                    string.Empty,
                    "File is zero size");
            }

            if (_analysisLevel == 0)
                return offsetStart;
            else
                return 0;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a n a l y s e E m b e d d e d P C L X L                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Analyse embedded PCL XL in work file.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool AnalyseEmbeddedPCLXL(string prnFilename,
                                        ToolCommonData.PrintLang newPDL,
                                        PCLXLOperators.EmbedDataType type,
                                        PrnParseOptions options,
                                        DataTable table)
        {
            _options = options;
            _table = table;
            _prnFilename = prnFilename;

            _flagDiagFileAccess = _options.FlagGenDiagFileAccess;

            if (!PrnFileOpen(prnFilename, ref _fileSize))
                return false;

            string typeText;
            string detailTextA;

            if (type == PCLXLOperators.EmbedDataType.PassThrough)
                typeText = "PCL PassThrough";
            else if (type == PCLXLOperators.EmbedDataType.Stream)
                typeText = "User-Defined Stream";
            else if (type == PCLXLOperators.EmbedDataType.FontHeader)
                typeText = "Font Header";
            else if (type == PCLXLOperators.EmbedDataType.FontChar)
                typeText = "Font Character";
            else
                typeText = "unknown entity";

            detailTextA = $"Embedding level = {_analysisLevel}; size = {_fileSize} bytes";

            if (type == PCLXLOperators.EmbedDataType.FontHeader)
            {
                _linkData.SetBacktrack(  // not really Backtrack but works!
                    PrnParseConstants.ContType.PCLXLFontHddr,
                    (int)_fileSize);
            }
            else if (type == PCLXLOperators.EmbedDataType.FontChar)
            {
                _linkData.SetBacktrack(  // not really Backtrack but works!
                    PrnParseConstants.ContType.PCLXLFontChar,
                    (int)_fileSize);
            }

            //------------------------------------------------------------//

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.Type.MsgComment,
                _table,
                PrnParseConstants.OvlShow.None,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);

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
                "Start analysis of embedded " + typeText);

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.Type.MsgComment,
                _table,
                PrnParseConstants.OvlShow.None,
                string.Empty,
                "Comment",
                string.Empty,
                detailTextA);

            //------------------------------------------------------------//

            AnalyseAction(type);

            //------------------------------------------------------------//

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.Type.MsgComment,
                _table,
                PrnParseConstants.OvlShow.None,
                string.Empty,
                "Comment",
                string.Empty,
                "End analysis of embedded " + typeText);

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.Type.MsgComment,
                _table,
                PrnParseConstants.OvlShow.None,
                string.Empty,
                "Comment",
                string.Empty,
                detailTextA);

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.Type.MsgComment,
                _table,
                PrnParseConstants.OvlShow.None,
                string.Empty,
                "<<<<<<<<<<<<<<<<<<<<",
                string.Empty,
                "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.Type.MsgComment,
                _table,
                PrnParseConstants.OvlShow.None,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);

            //------------------------------------------------------------//

            PrnFileClose();
            

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // e m b e d d e d D a t a S t o r e                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void EmbeddedDataStore(byte[] buf, int seqOffset, int seqLen)
        {
            if (!_subFileCreated)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Create and open temporary workfile for Write access.       //
                // The file is created in the folder associated with the TMP  //
                // environment variable.                                      //
                //                                                            //
                //------------------------------------------------------------//

                _subFilename = Path.Combine(Environment.GetEnvironmentVariable("TMP"),
                                 $"{DateTime.Now:yyyyMMdd_HHmmss_fff}_{_analysisLevel}.tmp");

                try
                {
                    _subStream = File.Create(_subFilename);
                }
                catch (IOException e)
                {
                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgError,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Error",
                        string.Empty,
                        "Failed to create embedded data store file:");

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgError,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        _subFilename);

                    MessageBox.Show($"IO Exception:\n\n{e.Message}\n\nCreating temporary file '{_subFilename}'.",
                                     "Embedded Data Store",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);

                    return;
                }

                if (_subStream == null)
                    return;

                _subFileCreated = true;

                _subWriter = new BinaryWriter(_subStream);

                _subFileOpen = true;

                if (_flagDiagFileAccess)
                {
                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgDiag,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Diagnostic",
                        string.Empty,
                        "Create file:");

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgDiag,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        _subFilename);
                }
            }

            if (_subFileOpen)
            {
                _subStream.Write(buf, seqOffset, seqLen);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // e m b e d d e d P C L A n a l y s e                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Analyse embedded PCL.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool EmbeddedPCLAnalyse(byte[] buf,
                                        ref int fileOffset,
                                        ref int bufRem,
                                        ref int bufOffset,
                                        PrnParseLinkData linkData,
                                        PrnParseOptions options,
                                        DataTable table)
        {
            bool badSeq;

            var crntPDL = ToolCommonData.PrintLang.PCL;

            bool endReached = false;

            linkData.MacroLevelAdjust(true);

            badSeq = _parsePCL.ParseBuffer(buf,
                                            ref fileOffset,
                                            ref bufRem,
                                            ref bufOffset,
                                            ref crntPDL,
                                            ref endReached,
                                            linkData,
                                            options,
                                            table);

            linkData.MacroLevelAdjust(false);

            return badSeq;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // e m b e d d e d P C L X L A n a l y s e                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Analyse embedded PCL XL data, stored in a temporary file, by       //
        // (recursively) setting up a new 'PrnParse' object.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void EmbeddedPCLXLAnalyse(ToolCommonData.PrintLang newPDL, PCLXLOperators.EmbedDataType type)
        {
            if (_subFileOpen && (_parseType == ParseType.Analyse))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Close temporary workfile for Write access.                 //
                //                                                            //
                //------------------------------------------------------------//

                string fName = ((FileStream)_subStream).Name;

                _subFileOpen = false;

                _subWriter.Close();
                _subStream.Close();

                if (_flagDiagFileAccess)
                {
                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgDiag,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Diagnostic",
                        string.Empty,
                        "Close file:");

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgDiag,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        fName);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Set up recursive analysis environment.                     //
                //                                                            //
                //------------------------------------------------------------//

                PrnParseOptions options = new PrnParseOptions(_options);

                options.SetOptCurF(
                    newPDL,
                    PrnParseConstants.PCLXLBinding.Unknown,
                    PrnParseConstants.OptOffsetFormats.Decimal,
                    0,
                    -1);

                PrnParse subParse = new PrnParse(_parseType, _analysisLevel + 1);

                subParse.AnalyseEmbeddedPCLXL(_subFilename,
                                               newPDL,
                                               type,
                                               options,
                                               _table);
                try
                {
                    File.Delete(_subFilename);
                }
                catch (IOException e)
                {
                    MessageBox.Show($"IO Exception:\n\n{e.Message}\n\nDeleting temporary file '{_subFilename}'.",
                                     "Embedded PCL XL Analysis",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);
                }

                if (_flagDiagFileAccess)
                {
                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgDiag,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Diagnostic",
                        string.Empty,
                        "Delete file:");

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgDiag,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        _subFilename);
                }

                _subFileCreated = false;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m a k e O v e r l a y P C L                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate PCL overlay by reading & modifying input print file.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool MakeOverlayPCL(string prnFilename,
                                       ref string ovlFilename,
                                       PrnParseOptions options,
                                       DataTable table,
                                       bool restoreCursor,
                                       bool encapsulate,
                                       int macroId)
        {
            _options = options;
            _table = table;

            if (!PrnFileOpen(prnFilename, ref _fileSize))
                return false;

            _linkData.MakeOvlXL = false;
            _linkData.MakeOvlEncapsulate = encapsulate;

            _linkData.FileSize = _fileSize;

            OvlFileOpen(false, ref ovlFilename);

            if (encapsulate)
                _linkData.MakeOvlMacroId = macroId;
            else
                _linkData.MakeOvlMacroId = -1;

            PrnParseMakeOvl.InsertHeaderPCL(_parsePCL,
                                                _table,
                                                _binWriter,
                                                encapsulate,
                                                restoreCursor,
                                                macroId);

            AnalyseAction(PCLXLOperators.EmbedDataType.None);

            if (_linkData.MakeOvlAct != PrnParseConstants.OvlAct.Terminate)
            {
                PrnParseMakeOvl.InsertTrailerPCL(_parsePCL,
                                                    _table,
                                                    _binWriter,
                                                    encapsulate,
                                                    restoreCursor);
            }

            PrnFileClose();
            OvlFileClose();
            

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m a k e O v e r l a y P C L X L                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate PCL XL overlay by reading & modifying input print file.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool MakeOverlayPCLXL(string prnFilename,
                                         ref string ovlFilename,
                                         PrnParseOptions options,
                                         DataTable table,
                                         bool restoreGS,
                                         bool encapsulate,
                                         string streamName)
        {
            _options = options;
            _table = table;

            if (!PrnFileOpen(prnFilename, ref _fileSize))
                return false;

            _linkData.MakeOvlXL = true;
            _linkData.MakeOvlEncapsulate = encapsulate;
            _linkData.MakeOvlRestoreStateXL = restoreGS;

            _linkData.FileSize = _fileSize;

            OvlFileOpen(true, ref ovlFilename);

            if (encapsulate)
                _linkData.MakeOvlStreamName = streamName;
            else
                _linkData.MakeOvlStreamName = string.Empty;

            _parsePCLXL.MakeOverlayInsertHeader(_binWriter,
                                                    encapsulate,
                                                    streamName,
                                                    _table);

            AnalyseAction(PCLXLOperators.EmbedDataType.None);

            if (_linkData.MakeOvlAct != PrnParseConstants.OvlAct.Terminate)
            {
                _parsePCLXL.MakeOverlayInsertTrailer(_binWriter,
                                                        restoreGS,
                                                        encapsulate,
                                                        _table);
            }

            PrnFileClose();
            OvlFileClose();
            

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m a k e O v e r l a y S c a n                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Scan input print file to determine if PCL or PCL XL or other PDL.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool MakeOverlayScan(string prnFilename, PrnParseOptions options, ref ToolCommonData.PrintLang pdl)
        {
            _options = options;

            if (!PrnFileOpen(prnFilename, ref _fileSize))
            {
                pdl = ToolCommonData.PrintLang.Unknown;

                return false;
            }

            _linkData.FileSize = _fileSize;

            AnalyseAction(PCLXLOperators.EmbedDataType.None);

            pdl = _crntPDL;

            PrnFileClose();

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // o v l F i l e C l o s e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close output stream and file (only for macro generation).          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void OvlFileClose()
        {
            string fName = ((FileStream)_opStream).Name;

            _binWriter.Close();
            _opStream.Close();

            if (_flagDiagFileAccess)
            {
                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.Type.MsgDiag,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Diagnostic",
                    string.Empty,
                    "Close file:");

                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.Type.MsgDiag,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    fName);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // o v l F i l e O p e n                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open write stream for overlay file.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void OvlFileOpen(bool makeOvlXL, ref string ovlFilename)
        {
            var saveDirectory = Path.GetDirectoryName(ovlFilename);
            var tmpFilename = Path.GetFileName(ovlFilename);

            //----------------------------------------------------------------//
            //                                                                //
            // Invoke 'Save As' dialogue.                                     //
            //                                                                //
            //----------------------------------------------------------------//

            var saveDialog = new SaveFileDialog();

            if (makeOvlXL)
            {
                saveDialog.Filter = "PCLXL Overlay File|*.ovx";
                saveDialog.DefaultExt = "ovx";
            }
            else
            {
                saveDialog.Filter = "PCL Overlay File|*.ovl";
                saveDialog.DefaultExt = "ovl";
            }

            saveDialog.RestoreDirectory = true;
            saveDialog.InitialDirectory = saveDirectory;
            saveDialog.OverwritePrompt = true;
            saveDialog.FileName = tmpFilename;

            if (saveDialog.ShowDialog() == true)
            {
                ovlFilename = saveDialog.FileName;
                tmpFilename = ovlFilename;
            }

            try
            {
                _opStream = File.Create(tmpFilename);
            }
            catch (IOException e)
            {
                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.Type.MsgError,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Error",
                    string.Empty,
                    "Create/open overlay file:");

                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.Type.MsgError,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    tmpFilename);

                MessageBox.Show($"IO Exception:\n\n{e.Message}\n\nCreating temporary file '{tmpFilename}'.",
                                 "Open Overlay File",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);

                return;
            }

            if (_opStream == null)
                return;

            _binWriter = new BinaryWriter(_opStream);

            if (_flagDiagFileAccess)
            {
                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.Type.MsgDiag,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Diagnostic",
                    string.Empty,
                    "Create file:");

                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.Type.MsgDiag,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    tmpFilename);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r n F i l e C l o s e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close input stream and file.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void PrnFileClose()
        {
            string fName = ((FileStream)_ipStream).Name;

            _binReader.Close();

            try             // not sure if try needed for void return function
            {
                _ipStream.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show($"IO Exception:\n\n{ex.Message}\n\nClosing file.",
                                 "Input Stream/File Close",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }

            if (_flagDiagFileAccess)
            {
                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.Type.MsgDiag,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Diagnostic",
                    string.Empty,
                    "Close file:");

                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.Type.MsgDiag,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    fName);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r n F i l e O p e n                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open read stream for specified print file.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool PrnFileOpen(string fileName, ref long fileSize)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Print file name is null.",
                                "Print File Selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            if (!File.Exists(fileName))
            {
                MessageBox.Show($"Print file '{fileName}' does not exist.",
                                "Print File Selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            try
            {
                _ipStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"IO Exception:\r\n{ex.Message}\n\nOpening print file '{fileName}'",
                                "Print File Selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            if (_ipStream == null)
                return false;

            fileSize = new FileInfo(fileName).Length;

            _binReader = new BinaryReader(_ipStream);

            if (_flagDiagFileAccess)
            {
                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.Type.MsgDiag,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Diagnostic",
                    string.Empty,
                    "Open (Read) file:");

                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.Type.MsgDiag,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    fileName);
            }

            return true;
        }
    }
}