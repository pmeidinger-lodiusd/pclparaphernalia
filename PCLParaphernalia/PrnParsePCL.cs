﻿using System.Data;
using System.Text;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines functions to parse PCL escape sequences.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal class PrnParsePCL
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

        private PrnParseLinkData _linkData;

        private PrnParseOptions _options;

        private readonly PrnParse.ParseType _parseType;

        private readonly PrnParseHPGL2 _parseHPGL2;

        private readonly PrnParseFontHddrPCL _parseFontHddrPCL;
        private readonly PrnParseFontCharPCL _parseFontCharPCL;
        private readonly PrnParsePCLBinary _parsePCLBinary;

        private DataTable _table;

        private byte[] _buf;

        private int _fileOffset;
        private int _endOffset;

        private PrnParseConstants.OptOffsetFormats _indxOffsetFormat;

        private PrnParseConstants.OptCharSetSubActs _indxCharSetSubAct;
        private PrnParseConstants.OptCharSets _indxCharSetName;
        private int _valCharSetSubCode;

        private int _textParsingMethod;
        private int _analysisLevel;
        private int _macroLevel;

        private bool _analyseFontHddr;
        private bool _analyseFontChar;
        private bool _interpretStyle;

        private bool _transAlphaNumId;
        private bool _transColourLookup;
        private bool _transConfIO;
        private bool _transConfImageData;
        private bool _transConfRasterData;
        private bool _transDefLogPage;
        private bool _transDefSymSet;
        private bool _transDitherMatrix;
        private bool _transDriverConf;
        private bool _transEscEncText;
        private bool _transPaletteConf;
        private bool _transUserPattern;
        private bool _transViewIlluminant;

        private bool _showBinData;
        private bool _showMacroData;

        private bool _analysePML;

        private readonly ASCIIEncoding _ascii = new ASCIIEncoding();

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P r n P a r s e P C L                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParsePCL(PrnParse.ParseType parseType,
                           PrnParseHPGL2 parseHPGL2)
        {
            _parseType = parseType;
            _parseHPGL2 = parseHPGL2;

            _textParsingMethod = (int)PCLTextParsingMethods.PCLVal.m0_1_byte_default;

            _parseFontHddrPCL = new PrnParseFontHddrPCL();
            _parseFontCharPCL = new PrnParseFontCharPCL();
            _parsePCLBinary = new PrnParsePCLBinary(_parseFontHddrPCL);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a r s e B u f f e r                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Parse provided buffer, assuming that the current print language is //
        // PCL.                                                               //
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

            //-------------------------------------------------------------------------//
            //                                                                         //
            // Initialise.                                                             //
            //                                                                         //
            //-------------------------------------------------------------------------//

            _buf = buf;
            _linkData = linkData;
            _options = options;
            _table = table;
            _fileOffset = fileOffset;

            _analysisLevel = linkData.AnalysisLevel;

            //----------------------------------------------------------------//

            _indxOffsetFormat = options.IndxGenOffsetFormat;

            _options.GetOptCharSet(ref _indxCharSetName,
                                    ref _indxCharSetSubAct,
                                    ref _valCharSetSubCode);

            _options.GetOptPCLBasic(ref _analyseFontHddr,
                                     ref _analyseFontChar,
                                     ref _showMacroData,
                                     ref _interpretStyle,
                                     ref _showBinData,
                                     ref _transAlphaNumId,
                                     ref _transColourLookup,
                                     ref _transConfIO,
                                     ref _transConfImageData,
                                     ref _transConfRasterData,
                                     ref _transDefLogPage,
                                     ref _transDefSymSet,
                                     ref _transDitherMatrix,
                                     ref _transDriverConf,
                                     ref _transEscEncText,
                                     ref _transPaletteConf,
                                     ref _transUserPattern,
                                     ref _transViewIlluminant);

            _endOffset = _options.ValCurFOffsetEnd;

            _analysePML = _options.FlagPMLWithinPCL;

            //----------------------------------------------------------------//

            if (linkData.IsContinuation())
            {
                seqInvalid = ParseContinuation(ref bufRem, ref bufOffset, ref crntPDL, ref endReached);
            }
            else
            {
                seqInvalid = ParseSequence(ref bufRem, ref bufOffset, ref crntPDL, ref endReached);
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
            PrnParseConstants.ContType contType;

            int prefixLen = 0,
                  contDataLen = 0,
                  downloadRem = 0,
                  binDataLen;
            bool hddrOK,
                    charOK,
                    dataOK,
                    continuation = false,
                    breakpoint = false,
                    backTrack = false,
                    dummyBool = false;

            bool invalidSeqFound = false;

            byte prefixA = 0x00,
                 prefixB = 0x00;

            contType = PrnParseConstants.ContType.None;

            _linkData.GetContData(ref contType,
                                   ref prefixLen,
                                   ref contDataLen,
                                   ref downloadRem,
                                   ref backTrack,
                                   ref prefixA,
                                   ref prefixB);
            bool badSeq;
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
                _linkData.ResetPCLComboData();
            }
            else if ((contType == PrnParseConstants.ContType.PCLDownload)
                               ||
                (contType == PrnParseConstants.ContType.PCLDownloadCombo))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended with an escape sequence with        //
                // following (usually binary) data download which is not      //
                // being specifically decoded.                                //
                // But the whole sequence was not completely contained in     //
                // that block.                                                //
                // Output the remaining 'download' characters (or the whole   //
                // buffer and initiate another continuation) before           //
                // continuing with the analysis.                              //
                //                                                            //
                //------------------------------------------------------------//

                if (downloadRem > bufRem)
                {
                    binDataLen = bufRem;
                    downloadRem -= bufRem;
                }
                else
                {
                    binDataLen = downloadRem;
                    downloadRem = 0;
                }

                //------------------------------------------------------------//
                //                                                            //
                // Some, or all, of the download data is contained within the //
                // current 'block'.                                           //
                //                                                            //
                //------------------------------------------------------------//

                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.OvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    binDataLen,
                    "PCL Binary",
                    _showBinData,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);

                //------------------------------------------------------------//
                //                                                            //
                // Adjust continuation data and pointers.                     //
                //                                                            //
                //------------------------------------------------------------//

                if (downloadRem == 0)
                {
                    if (contType == PrnParseConstants.ContType.PCLDownloadCombo)
                    {
                        _linkData.SetContData(
                            PrnParseConstants.ContType.PCLComplex,
                            prefixLen,
                            0,
                            0,
                            false,
                            prefixA,
                            prefixB);
                    }
                    else
                    {
                        _linkData.ResetContData();
                    }
                }
                else
                {
                    _linkData.SetContData(contType,
                                           prefixLen,
                                           contDataLen,
                                           downloadRem, // this is the value to update
                                           backTrack,
                                           prefixA,
                                           prefixB);
                }

                bufRem -= binDataLen;
                bufOffset += binDataLen;
            }
            else if (contType == PrnParseConstants.ContType.PCLFontHddr)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a font header  //
                // download sequence.                                         //
                //                                                            //
                //------------------------------------------------------------//

                hddrOK = _parseFontHddrPCL.AnalyseFontHddr(-1,
                                                            _fileOffset,
                                                            _buf,
                                                            ref bufRem,
                                                            ref bufOffset,
                                                            _linkData,
                                                            _options,
                                                            _table);

                if (!hddrOK)
                    invalidSeqFound = true;
            }
            else if (contType == PrnParseConstants.ContType.PCLFontChar)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a font         //
                // character download sequence.                               //
                //                                                            //
                //------------------------------------------------------------//

                charOK = _parseFontCharPCL.AnalyseFontChar(-1,
                                                            _fileOffset,
                                                            _buf,
                                                            ref bufRem,
                                                            ref bufOffset,
                                                            _linkData,
                                                            _options,
                                                            _table);

                if (!charOK)
                    invalidSeqFound = true;
            }
            else if (contType == PrnParseConstants.ContType.PCLAlphaNumericID)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of an             //
                // AlphaNumeric ID sequence.                                  //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeAlphaNumericID(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;
                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLColourLookup)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a              //
                // Colour Lookup Tables sequence.                             //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeColourLookup(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;
                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLConfigurationIO)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a              //
                // Configuration (I/O) sequence.                              //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeConfigurationIO(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLConfigureImageData)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a              //
                // Configure Image Data sequence.                             //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeConfigureImageData(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLConfigureRasterData)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a              //
                // Configure Raster Data sequence.                            //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeConfigureRasterData(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLLogicalPageData)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a              //
                // Define Logical Page sequence.                              //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeDefineLogicalPage(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLDefineSymbolSet)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a              //
                // Define Symbol Set header sequence.                         //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeDefineSymbolSet(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLDefineSymbolSetMap)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a              //
                // Define Symbol Set mapping data sequence.                   //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeDefineSymbolSetMap(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;
                /*
                if (downloadRem == 0)
                {
                    continuation = false;
                    _linkData.resetContData();
                }
                */
            }
            else if (contType == PrnParseConstants.ContType.PCLDitherMatrix)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a Download     //
                // Dither Matrix sequence.                                    //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeDitherMatrix(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                if (_linkData.GetContType() == PrnParseConstants.ContType.None)
                {
                    _linkData.ResetContData();
                }
            }
            else if (contType == PrnParseConstants.ContType.PCLDitherMatrixPlane)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of Download       //
                // Dither Matrix plane data header.                           //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeDitherMatrixPlane(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                if (_linkData.GetContType() == PrnParseConstants.ContType.None)
                {
                    _linkData.ResetContData();
                }
            }
            else if (contType == PrnParseConstants.ContType.PCLDitherMatrixPlaneData)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of Download       //
                // Dither Matrix plane data.                                  //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeDitherMatrixPlaneData(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                if (_linkData.GetContType() == PrnParseConstants.ContType.None)
                {
                    _linkData.ResetContData();
                }
            }
            else if (contType == PrnParseConstants.ContType.PCLDriverConfiguration)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a              //
                // Driver Configuration sequence.                             //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeDriverConfiguration(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLEscEncText)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of an             //
                // Escapement Encapsulated Text sequence.                     //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeEscEncText(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLEscEncTextData)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of an             //
                // Escapement Encapsulated Text data sequence.                //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeEscEncTextData(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLPaletteConfiguration)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a              //
                // Palette Configuration sequence.                            //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodePaletteConfiguration(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                _linkData.ResetContData();
            } else if (contType ==
                PrnParseConstants.ContType.PCLUserDefPatternHddr)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of an             //
                // User Defined Pattern sequence header.                      //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeUserDefinedPattern(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLUserDefPatternData)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of an             //
                // User Defined Pattern sequence pattern data.                //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeUserDefinedPatternData(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                if (_linkData.GetContType() == PrnParseConstants.ContType.None)
                {
                    _linkData.ResetContData();
                }
            }
            else if (contType == PrnParseConstants.ContType.PCLViewIlluminant)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a              //
                // Viewing Illuminant sequence.                               //
                //                                                            //
                //------------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeViewIlluminant(
                    downloadRem,
                    _fileOffset,
                    _buf,
                    ref bufRem,
                    ref bufOffset,
                    _linkData,
                    _options,
                    _table);

                if (!dataOK)
                    invalidSeqFound = true;

                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLEmbeddedPML)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Download contains data which may include an embedded   //
                // PML sequence.                                          //
                //                                                        //
                //--------------------------------------------------------//

                dataOK = _parsePCLBinary.DecodeEmbeddedPML(downloadRem,
                                            _fileOffset,
                                            _buf,
                                            ref bufRem,
                                            ref bufOffset,
                                            _linkData,
                                            _options,
                                            _table);

                if (!dataOK)
                    invalidSeqFound = true;

                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLMultiByteData)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a multi-byte   //
                // text parsing method sequence.                              //
                //                                                            //
                //------------------------------------------------------------//

                string typeText;

                if ((_textParsingMethod == (int)PCLTextParsingMethods.PCLVal.m83_UTF8) 
                        ||
                    (_textParsingMethod == (int)PCLTextParsingMethods.PCLVal.m1008_UTF8_alt))
                {
                    typeText = "UTF-8 data";
                }
                else
                {
                    typeText = "Data";
                }

                PrnParseData.ProcessLines(
                    _table,
                    PrnParseConstants.OvlShow.None,
                    _linkData,
                    ToolCommonData.PrintLang.PCL,
                    _buf,
                    _fileOffset,
                    bufRem,
                    ref bufRem,
                    ref bufOffset,
                    ref continuation,
                    true,
                    false,
                    false,
                    PrnParseConstants.asciiEsc,
                    typeText,
                    _textParsingMethod,
                    _indxCharSetSubAct,
                    (byte)_valCharSetSubCode,
                    _indxCharSetName,
                    _indxOffsetFormat,
                    _analysisLevel);

                if (continuation)
                {
                    contType = PrnParseConstants.ContType.PCLMultiByteData;

                    _linkData.SetBacktrack(contType, -bufRem);
                }
                else
                {
                    contType = PrnParseConstants.ContType.None;

                    _linkData.ResetContData();
                }
            }
            else if (contType == PrnParseConstants.ContType.PCLEmbeddedData)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of an embedded    //
                // data sequence (e.g. the data associated with the           //
                // Transparent Print command).                                //
                //                                                            //
                //------------------------------------------------------------//

                PrnParseData.ProcessLines(
                    _table,
                    PrnParseConstants.OvlShow.None,
                    _linkData,
                    ToolCommonData.PrintLang.Unknown,
                    _buf,
                    _fileOffset,
                    downloadRem,
                    ref bufRem,
                    ref bufOffset,
                    ref dummyBool,
                    true,
                    false,
                    false,
                    PrnParseConstants.asciiEsc,
                    "Embedded data",
                    0,
                    _indxCharSetSubAct,
                    (byte)_valCharSetSubCode,
                    _indxCharSetName,
                    _indxOffsetFormat,
                    _analysisLevel);

                continuation = false;
                _linkData.ResetContData();
            }
            else if (contType == PrnParseConstants.ContType.PCLComplex)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended during processing of a complex      //
                // (parameterised) escape sequence.                           //
                //                                                            //
                //------------------------------------------------------------//

                badSeq = ParseSequenceComplex(ref bufRem,
                                               ref bufOffset,
                                               ref continuation,
                                               ref breakpoint,
                                               ref crntPDL,
                                               //   true, // // CheckExtensionTable //
                                               true);

                if (badSeq)
                    invalidSeqFound = true;
            }
            else if (contType == PrnParseConstants.ContType.PCLExtension)
            {
                //-------------------------------------------------------------------//
                //                                                                   //
                // Previous 'block' ended during processing of a complex escape      //
                // sequence which has been identified as a vendor-defined extension  //
                // sequence.                                                         //
                //                                                                   //
                //-------------------------------------------------------------------//

                badSeq = ParseSequenceComplex(ref bufRem,
                                               ref bufOffset,
                                               ref continuation,
                                               ref breakpoint,
                                               ref crntPDL,
                                               //   true, // // CheckExtensionTable //
                                               false);

                if (badSeq)
                    invalidSeqFound = true;
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Previous 'block' ended with a partial match of a Special   //
                // Escape sequence, or with insufficient characters to        //
                // identify the type of sequence.                             //
                // The continuation action has already reset the buffer, so   //
                // now unset the markers.                                     //
                //                                                            //
                //------------------------------------------------------------//

                continuation = false;
                _linkData.ResetContData();
            }

            if ((_endOffset != -1) && ((_fileOffset + bufOffset) > _endOffset))
                endReached = true;

            return invalidSeqFound;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a r s e S e q u e n c e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process sequences until end-point reached.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ParseSequence(
            ref int bufRem,
            ref int bufOffset,
            ref ToolCommonData.PrintLang crntPDL,
            ref bool endReached)
        {
            bool breakpoint = false;
            bool continuation = false;
            bool langSwitch = false;
            bool invalidSeqFound = false;
            bool dummyBool = false;

            while (!continuation && !breakpoint && !langSwitch && !endReached && (bufRem > 0))
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
                else if (_buf[bufOffset] != PrnParseConstants.asciiEsc)
                {
                    if (_buf[bufOffset] == PrnParseConstants.prescribeSCRCDelimiter)
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

                            continuation = true;

                            _linkData.SetBacktrack(
                                PrnParseConstants.ContType.Unknown,
                                -bufRem);
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
                                _linkData.PrescribeCallerPDL = ToolCommonData.PrintLang.PCL;
                            }
                        }
                    }

                    if ((!continuation) && (!langSwitch))
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Data.                                              //
                        // Output <LF> terminated lines until next Escape     //
                        // character found, or end of buffer reached.         //
                        //                                                    //
                        //----------------------------------------------------//

                        string typeText;

                        if ((_textParsingMethod == (int)PCLTextParsingMethods.PCLVal.m83_UTF8) 
                                                ||
                            (_textParsingMethod == (int)PCLTextParsingMethods.PCLVal.m1008_UTF8_alt))
                        {
                            typeText = "UTF-8 data";
                        }
                        else
                        {
                            typeText = "Data";
                        }

                        PrnParseData.ProcessLines(
                            _table,
                            PrnParseConstants.OvlShow.None,
                            _linkData,
                            ToolCommonData.PrintLang.PCL,
                            _buf,
                            _fileOffset,
                            bufRem,
                            ref bufRem,
                            ref bufOffset,
                            ref continuation,
                            true,
                            false,
                            false,
                            PrnParseConstants.asciiEsc,
                            typeText,
                            _textParsingMethod,
                            _indxCharSetSubAct,
                            (byte)_valCharSetSubCode,
                            _indxCharSetName,
                            _indxOffsetFormat,
                            _analysisLevel);

                        if (_parseType == PrnParse.ParseType.MakeOverlay && _linkData.MakeOvlPageMark)
                        {
                            PrnParseConstants.OvlPos makeOvlPos = _linkData.MakeOvlPos;

                            if (makeOvlPos == PrnParseConstants.OvlPos.BeforeFirstPage)
                            {
                                makeOvlPos = PrnParseConstants.OvlPos.WithinFirstPage;
                            }
                            else if (makeOvlPos != PrnParseConstants.OvlPos.WithinFirstPage)
                            {
                                makeOvlPos = PrnParseConstants.OvlPos.WithinOtherPages;
                            }

                            _linkData.MakeOvlPos = makeOvlPos;
                        }

                        if (_linkData.MakeOvlAct != PrnParseConstants.OvlAct.None)
                        {
                            breakpoint = true;
                        }
                        else if (continuation)
                        {
                            _linkData.SetBacktrack(
                                PrnParseConstants.ContType.PCLMultiByteData,
                                -bufRem);
                        }
                        else
                        {
                            _linkData.ResetContData();
                        }
                    }
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Escape character found.                                //
                    //                                                        //
                    //--------------------------------------------------------//

                    _linkData.ResetPCLComboData();

                    bool badSeq;
                    if ((bufRem >= 2) &&
                        (_buf[bufOffset + 1] >= PrnParseConstants.pclSimpleICharLow)
                                    &&
                        (_buf[bufOffset + 1] <= PrnParseConstants.pclSimpleICharHigh))
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // A Simple (two-character) escape sequence.          //
                        // Or the start of a (vendor-defined) Extension       //
                        // Complex escape sequence which doesn't follow the   //
                        // rules.                                             //
                        // We could check for the latter first; we could do   //
                        // this because:                                      //
                        //  - The only known Extension sequences with the     //
                        //    I_CHAR character in this range are the Data     //
                        //    Products ones, with I_CHAR="|".                 //
                        //  - There are no standard Simple sequences with "|" //
                        //    as the second character (out of permitted       //
                        //    range).                                         //
                        //                                                    //
                        //----------------------------------------------------//

                        if (_buf[bufOffset + 1] == PrnParseConstants.asciiPipe)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // DataProducts (Typhoon series) Extension        //
                            // sequence?                                      //
                            // Ideally, this I_CHAR should be checked against //
                            // a set of values in a table, but so far, we     //
                            // only have this one value.                      //
                            //                                                //
                            //------------------------------------------------//

                            badSeq = ParseSequenceComplex(ref bufRem,
                                                           ref bufOffset,
                                                           ref continuation,
                                                           ref breakpoint,
                                                           ref crntPDL,
                                                           //   true, // // CheckExtensionTable //
                                                           false);

                            if (crntPDL != ToolCommonData.PrintLang.PCL)
                                langSwitch = true;

                            if (badSeq)
                                invalidSeqFound = true;
                        }
                        else
                        {
                            bool seqKnown = ParseSequenceSimple(ref bufRem, ref bufOffset, ref breakpoint);

                            if (!seqKnown)
                            {
                                //--------------------------------------------//
                                //                                            //
                                // Unrecognised data.                         //
                                // Output <LF> terminated lines until next    //
                                // Escape character found, or end of buffer   //
                                // reached.                                   //
                                //                                            //
                                //--------------------------------------------//

                                PrnParseData.ProcessLines(
                                    _table,
                                    PrnParseConstants.OvlShow.None,
                                    _linkData,
                                    ToolCommonData.PrintLang.PCL,
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
                        }
                    }
                    else if ((bufRem >= 3)     // min = 3 if nil-G with no value
                                           &&
                             (_buf[bufOffset + 1] >= PrnParseConstants.pclComplexICharLow)
                                           &&
                             (_buf[bufOffset + 1] <= PrnParseConstants.pclComplexICharHigh))
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Complex (parameterised) PCL sequence.              //
                        //                                                    //
                        //----------------------------------------------------//

                        badSeq = ParseSequenceComplex(ref bufRem,
                                                       ref bufOffset,
                                                       ref continuation,
                                                       ref breakpoint,
                                                       ref crntPDL,
                                                       //   true, // // CheckExtensionTable //
                                                       true);

                        if (crntPDL != ToolCommonData.PrintLang.PCL)
                            langSwitch = true;

                        if (badSeq)
                            invalidSeqFound = true;
                    }
                    else if (bufRem < 4)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Remaining data in buffer too short to determine    //
                        // type.                                              //
                        // Initiate continuation action.                      //
                        //                                                    //
                        //----------------------------------------------------//

                        continuation = true;

                        _linkData.SetBacktrack(PrnParseConstants.ContType.Unknown, -bufRem);
                    }
                    else
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Unrecognised data.                                 //
                        // Output <LF> terminated lines until next Escape     //
                        // character found, or end of buffer reached.         //
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
                            "Unexpected sequence found:");

                        PrnParseData.ProcessLines(
                            _table,
                            PrnParseConstants.OvlShow.None,
                            _linkData,
                            ToolCommonData.PrintLang.PCL,
                            _buf,
                            _fileOffset,
                            bufRem,
                            ref bufRem,
                            ref bufOffset,
                            ref continuation,
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
                }
            }

            return invalidSeqFound;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a r s e S e q u e n c e C o m p l e x                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // This function processes a sequence (already identified by its      //
        // opening characters) as a Complex (Parameterised) Escape Sequence.  //
        //                                                                    //
        // Continuation action is invoked if the sequence is not terminated   //
        // in the current 'block'.                                            //
        //                                                                    //
        // Format of Complex Escape Sequences:                                //
        //                                                                    //
        //      byte     Escape character (x'1B)                              //
        //      byte     Parameterised Indicator character (I_CHAR)           //
        //      byte     Group character                   (G_CHAR)           //
        //      n-byte   Value field                       (V_FIELD)          //
        //      byte     Termination character             (T_CHAR)           //
        //                                                                    //
        // Sequences may be combined, where the I/G_CHAR prefix is the same,  //
        // by omitting this prefix (and the Escape character) from the second //
        // and subsequent sequence. In these cases, the Termination Character //
        // for all but the last in the combination becomes a Parameter        //
        // Character (P_CHAR) which is the lower-case equivalent of the       //
        // upper-case Termination Character (T_CHAR).                         //
        //                                                                    //
        // Some sequences have no Group character (in which case they cannot  //
        // be part of a Combination sequence).                                //
        // Some sequences do not have a value field.                          //
        //                                                                    //
        // Each table entry matches I/G/T_CHAR values against descriptions.   //
        // Options indicate special treatment is required for some sequences. //
        //                                                                    //
        // I_CHAR should be within the (ASCII) range 0x21 - 0x2f.             //
        // However, vendor-dependent extensions may utilise other values      //
        // (e.g. 0x7C by DataProducts devices).                               //
        // We don't currently cater for any of these, but could if they were  //
        // tabulated in an Extension Sequence table.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ParseSequenceComplex(
            ref int bufRem,
            ref int bufOffset,
            ref bool continuation,
            ref bool breakpoint,
            ref ToolCommonData.PrintLang crntPDL,
            //  Boolean CheckExtensionTable,
            bool CheckStandardTable)
        {
            PrnParseConstants.ContType contType = PrnParseConstants.ContType.None;

            PrnParseConstants.ActPCL actType = PrnParseConstants.ActPCL.None;

            PrnParseConstants.OvlAct makeOvlAct = PrnParseConstants.OvlAct.None;

            PrnParseConstants.OvlShow makeOvlShow = PrnParseConstants.OvlShow.None;

            byte gChar = 0x20,
                 iChar = 0x20,
                 p_or_TChar,
                 crntByte;
            int vtLen = 0,
                  processedLen,
                  seqLen,
                  seqOffset,
                  seqPos,
                  seqStart,
                  prefixLen = 0,
                  binDataLen,
                  contDataLen = 0,
                  downloadRem = 0,
                  endPos,
                  vInt,
                  vPosFirst,
                  vPosNext,
                  vPosCrnt;
            //   Int32 vendorCode;

            long comboStart = 0;

            bool seqKnown,
                    seqComplete,
                    seqProprietary = false,
                    comboSeq = false,
                    comboFirst = false,
                    comboLast = false,
                    comboModified = false,
                    backTrack = false,
                    p_or_TCharFound = false;

            bool optObsolete = false,
                    optResetHPGL2 = false,
                    optNilGChar = false,
                    optNilValue = false,
                    optValueIsLen = false,
                    optDisplayHexVal = false;
            // Boolean optValueAngleQuoted;

            bool vCheck,
                    vInvalid,
                    vCharInvalid,
                    vNegative,
                    vFractional,
                    vSignFound,
                    vStarted,
                    vQuotedStart,
                    vQuotedEnd,
                    vNumberStarted;

            string descComplex = string.Empty,
                   typeText,
                   vendorName;

            bool invalidSeq = false;
            seqComplete = false;
            continuation = false;
            vInvalid = false;
            vNegative = false;
            vFractional = false;
            vSignFound = false;
            vStarted = false;
            vQuotedStart = false;
            vQuotedEnd = false;
            vNumberStarted = false;

            vInt = 0;
            binDataLen = 0;

            p_or_TChar = 0x20;

            //----------------------------------------------------------------//
            //                                                                //
            // Determine whether continuation call or not.                    //
            //                                                                //
            //----------------------------------------------------------------//

            if (_linkData.IsContinuation())
            {
                //------------------------------------------------------------//
                //                                                            //
                // Continuation situation.                                    //
                // This will only occur if the sequence is a combination      //
                // sequence, and at least one (value + parameter character)   //
                // pair has already been processed.                           //
                //                                                            //
                //------------------------------------------------------------//

                contType = PrnParseConstants.ContType.None;

                _linkData.GetContData(ref contType,
                                       ref prefixLen,
                                       ref contDataLen,
                                       ref downloadRem,
                                       ref backTrack,
                                       ref iChar,
                                       ref gChar);

                _linkData.GetPCLComboData(ref comboSeq,
                                           ref comboFirst,
                                           ref comboLast,
                                           ref comboModified,
                                           ref comboStart);

                comboFirst = false;
                comboLast = false;

                seqOffset = 0;
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Not a continuation situation.                              //
                // Read and store details of I and G characters, and hence    //
                // determine sequence prefix.                                 //
                //                                                            //
                //------------------------------------------------------------//

                iChar = _buf[bufOffset + 1];
                gChar = _buf[bufOffset + 2];

                if ((gChar < PrnParseConstants.pclComplexGCharLow)
                              ||
                    (gChar > PrnParseConstants.pclComplexGCharHigh))
                {
                    gChar = 0x20;
                    prefixLen = 1;
                }
                else
                {
                    prefixLen = 2;
                }

                seqOffset = prefixLen + 1; // references first V_field //
                comboSeq = false;
                comboFirst = true;  // initial state if combo detected //
                comboLast = false; // initial state if combo detected //
                comboModified = false; // initial state if combo detected //
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Process bytes from the buffer until the (upper-case)           //
            // Termination character is found, or the end of buffer is        //
            // reached, or (in a 'Make Overlay' run) a breakpoint is reached. //
            //                                                                //
            //----------------------------------------------------------------//

            vPosFirst = bufOffset + seqOffset;
            vPosNext = vPosFirst;
            vPosCrnt = vPosFirst;
            endPos = bufOffset + bufRem;

            for (int i = vPosFirst; (i < endPos) && !continuation && !breakpoint && !seqComplete; i++)
            {
                crntByte = _buf[i];

                //------------------------------------------------------------//
                //                                                            //
                // Check next character.                                      //
                //                                                            //
                //------------------------------------------------------------//

                if (crntByte == PrnParseConstants.asciiEsc)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Invalid - sequence terminated before previous (part)   //
                    // sequence terminated!                                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    string text;

                    seqComplete = true;
                    invalidSeq = true;

                    if (p_or_TCharFound)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Previous part sequence already processed and       //
                        // displayed.                                         //
                        // We don't want to repeat the display.               //
                        //                                                    //
                        //----------------------------------------------------//

                        p_or_TCharFound = false;

                        text = "previous sequence";
                    }
                    else
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Previous part sequence not yet complete.           //
                        // We want to display what we have up to now.         //
                        //                                                    //
                        //----------------------------------------------------//

                        if (vtLen != 0)
                            p_or_TCharFound = true;

                        if (comboSeq)
                            text = "next (part of) sequence";
                        else
                            text = "next sequence";
                    }

                    PrnParseCommon.AddDataRow(
                        PrnParseRowTypes.Type.MsgWarning,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        _indxOffsetFormat,
                        _fileOffset + i,
                        _analysisLevel,
                        "*** Warning ***",
                        string.Empty,
                        $"<Esc> found before termination of {text}");

                    p_or_TChar = PrnParseConstants.asciiSUB;    // will not match any table entries //

                    i--;                                  // point back to byte before <Esc>  //
                }
                else
                {
                    p_or_TCharFound = false;
                }

                if (seqComplete)
                {
                    // already processed above
                }
                else if ((i == vPosNext) && (crntByte == PrnParseConstants.asciiAngleLeft))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // First character of value field is "<".                 //
                    // This could be the start of a (non-standard)            //
                    // 'angle-quoted' value as used in proprietary Oce        //
                    // VarioPrint sequences, and in the (obsolete) HP 'Large  //
                    // Character Print Data' sequence; note that these do not //
                    // adhere to the standard PCL syntax rules.               //
                    // Search for equivalent ">" character; we expect it to   //
                    // be within the next 60 bytes.                           //
                    //                                                        //
                    //--------------------------------------------------------//

                    vStarted = true;
                    vQuotedStart = true;
                    vQuotedEnd = false;
                }
                else if (vQuotedStart && (!vQuotedEnd))
                {
                    if (crntByte == PrnParseConstants.asciiAngleRight)
                    {
                        vQuotedEnd = true;
                    }
                }
                else if ((crntByte >= PrnParseConstants.pclComplexTCharLow)
                                        &&
                         (crntByte <= PrnParseConstants.pclComplexTCharHigh))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence Terminator Character found.                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    seqComplete = true;
                    p_or_TCharFound = true;
                    p_or_TChar = crntByte;
                    comboLast = true;

                    if (comboSeq)
                        comboFirst = false;
                }
                else if ((crntByte >= PrnParseConstants.pclComplexPCharLow)
                                      &&
                         (crntByte <= PrnParseConstants.pclComplexPCharHigh))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence Parameter Character found.                    //
                    //                                                        //
                    //--------------------------------------------------------//

                    p_or_TCharFound = true;
                    p_or_TChar = (byte)(crntByte - 0x20);
                    comboLast = false;

                    comboFirst = !comboSeq;

                    comboSeq = true;
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Character must be part of the value field.             //
                    // Check that the character is valid, and calculate the   //
                    // aggregate value (at least, of the integer part).       //
                    //                                                        //
                    //--------------------------------------------------------//

                    vStarted = true;
                    vCharInvalid = false;
                    vQuotedStart = false;
                    vQuotedEnd = false;

                    if (vFractional)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Decimal point already found.                       //
                        // Only allow digits.                                 //
                        //                                                    //
                        //----------------------------------------------------//

                        if ((crntByte >= PrnParseConstants.asciiDigit0)
                                   &&
                            (crntByte <= PrnParseConstants.asciiDigit9))
                        {
                            // do nothing //
                        }
                        else
                        {
                            vCharInvalid = true;
                        }
                    }
                    else if (vNumberStarted)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Digit already found, but not fractional.           //
                        // Only allow digits or decimal point.                //
                        //                                                    //
                        //----------------------------------------------------//

                        if (crntByte == PrnParseConstants.asciiPeriod)
                        {
                            vFractional = true;
                        }
                        else if ((crntByte >= PrnParseConstants.asciiDigit0)
                                         &&
                                 (crntByte <= PrnParseConstants.asciiDigit9))
                        {
                            vInt = (vInt * 10) +
                                   (crntByte - PrnParseConstants.asciiDigit0);
                        }
                        else
                        {
                            vCharInvalid = true;
                        }
                    }
                    else if (vSignFound)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Sign already found, but no digit or decimal point. //
                        // Only allow spaces or digits or decimal point.      //
                        //                                                    //
                        //----------------------------------------------------//

                        if (crntByte == PrnParseConstants.asciiSpace)
                        {
                            // do nothing - spaces allowed before/after sign //
                        }
                        else if (crntByte == PrnParseConstants.asciiPeriod)
                        {
                            vFractional = true;
                        }
                        else if ((crntByte >= PrnParseConstants.asciiDigit0)
                                         &&
                                 (crntByte <= PrnParseConstants.asciiDigit9))
                        {
                            vNumberStarted = true;

                            vInt = crntByte - PrnParseConstants.asciiDigit0;
                        }
                        else
                        {
                            vCharInvalid = true;
                        }
                    }
                    else
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Only spaces (if anything) found so far.            //
                        // Allow spaces or signs or digits or decimal point.  //
                        //                                                    //
                        //----------------------------------------------------//

                        if (crntByte == PrnParseConstants.asciiSpace)
                        {
                            // do nothing //
                        }
                        else if (crntByte == PrnParseConstants.asciiPlus)
                        {
                            vSignFound = true;
                        }
                        else if (crntByte == PrnParseConstants.asciiMinus)
                        {
                            vSignFound = true;
                            vNegative = true;
                        }
                        else if (crntByte == PrnParseConstants.asciiPeriod)
                        {
                            vFractional = true;
                        }
                        else if ((crntByte >= PrnParseConstants.asciiDigit0)
                                        &&
                                 (crntByte <= PrnParseConstants.asciiDigit9))
                        {
                            vNumberStarted = true;
                            vInt = crntByte - PrnParseConstants.asciiDigit0;
                        }
                        else
                        {
                            vCharInvalid = true;
                        }
                    }

                    if (vCharInvalid)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Invalid value value !                              //
                        //                                                    //
                        //----------------------------------------------------//

                        string seq;
                        string text;

                        if (crntByte < PrnParseConstants.asciiGraphicMin)
                            seq = $"0x{crntByte.ToString("x2")}";
                        else
                            seq = ((char)crntByte).ToString();

                        if (comboSeq)
                            text = "next (part of) combination sequence";
                        else
                            text = "next sequence";

                        vInvalid = true;
                        invalidSeq = true;

                        PrnParseCommon.AddDataRow(
                            PrnParseRowTypes.Type.MsgWarning,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            _indxOffsetFormat,
                            _fileOffset + i,
                            _analysisLevel,
                            "*** Warning ***",
                            seq,
                            $"Invalid value in {text}");
                    }
                }

                //------------------------------------------------------------//
                //                                                            //
                // Check whether Sequence Terminator Character or Parameter   //
                // Character Has been found.                                  //
                //                                                            //
                //------------------------------------------------------------//

                if (p_or_TCharFound)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence Terminator Character or Parameter Character   //
                    // found.                                                 //
                    // Search table for description of (this part of) the     //
                    // sequence.                                              //
                    //                                                        //
                    //--------------------------------------------------------//

                    string val;
                    string seq;

                    seqKnown = false;
                    seqProprietary = false;

                    if (vNegative)
                    {
                        vInt = -vInt;
                    }

                    int vLen = i - vPosCrnt;
                    vtLen = vLen + 1;

                    vCheck = !vInvalid && !vFractional;

                    /*
                    if (CheckExtensionTable)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Check sequence against entries in special          //
                        // Extension Sequence table.                          //
                        //                                                    //
                        //----------------------------------------------------//

                        seqKnown = PCLExtensionSeqs.checkComplexSeq(
                            (_analysisLevel +
                             _macroLevel),
                            iChar,
                            gChar,
                            p_or_TChar,
                            vCheck,
                            vInt,
                            ref optObsolete,
                            ref optResetHPGL2,
                            ref optNilGChar,
                            ref optNilValue,
                            ref optValueIsLen,
                            ref optValueAngleQuoted,
                            ref actType,
                            ref makeOvlAct,
                            ref descComplex,
                            ref vendorCode);

                        seqKnown = false; // TEMPORARY //
                        if (seqKnown)
                            seqProprietary = true;
                    }
                    */

                    if ((!seqProprietary) && CheckStandardTable)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Check sequence against entries in standard Complex //
                        // (Parameterised) Sequence table.                    //
                        //                                                    //
                        //----------------------------------------------------//

                        seqKnown = PCLComplexSeqs.CheckComplexSeq(
                            _analysisLevel +
                             _macroLevel,
                            iChar,
                            gChar,
                            p_or_TChar,
                            vCheck,
                            vInt,
                            ref optObsolete,
                            ref optResetHPGL2,
                            ref optNilGChar,
                            ref optNilValue,
                            ref optValueIsLen,
                            ref optDisplayHexVal,
                            ref actType,
                            ref makeOvlAct,
                            ref descComplex);
                    }

                    if (seqKnown)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Check sequence validity against option flags.      //
                        //                                                    //
                        //----------------------------------------------------//

                        if (optNilValue && vStarted)
                        {
                            invalidSeq = true;

                            PrnParseCommon.AddTextRow(
                                PrnParseRowTypes.Type.MsgWarning,
                                _table,
                                PrnParseConstants.OvlShow.None,
                                string.Empty,
                                "*** Warning ***",
                                string.Empty,
                                "Unexpected value field in next sequence");
                        }

                        if (optNilGChar && (prefixLen != 1))
                        {
                            //------------------------------------------------//
                            //                                                //
                            // This should never occur?                       //
                            // It implies that we have a sequence with a      //
                            // G_CHAR component which has matched an entry in //
                            // the table for a nil-G_CHAR sequence!           //
                            //                                                //
                            //------------------------------------------------//

                            invalidSeq = true;

                            PrnParseCommon.AddTextRow(
                                PrnParseRowTypes.Type.MsgError,
                                _table,
                                PrnParseConstants.OvlShow.None,
                                string.Empty,
                                "*** Error ***",
                                string.Empty,
                                "Unexpected nil-G sequence match");
                        }
                        else if (!optNilGChar && (prefixLen != 2))
                        {
                            //------------------------------------------------//
                            //                                                //
                            // This should never occur?                       //
                            // It implies that we have a sequence without a   //
                            // G_CHAR component which has matched an entry in //
                            // the table for a standard (with G_CHAR)         //
                            // sequence!                                      //
                            //                                                //
                            //------------------------------------------------//

                            invalidSeq = true;

                            PrnParseCommon.AddTextRow(
                                PrnParseRowTypes.Type.MsgError,
                                _table,
                                PrnParseConstants.OvlShow.None,
                                string.Empty,
                                "*** Error ***",
                                string.Empty,
                                "Unexpected sequence match");
                        }

                        //----------------------------------------------------//
                        //                                                    //
                        // Action 'active' option flags.                      //
                        //                                                    //
                        //----------------------------------------------------//

                        if (optResetHPGL2)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Sequence resets HP-GL/2 state variables.       //
                            //                                                //
                            //------------------------------------------------//

                            _parseHPGL2.ResetHPGL2();
                        }

                        if (actType == PrnParseConstants.ActPCL.SwitchToHPGL2)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Switch to HP-GL/2 language processing.         //
                            //                                                //
                            //------------------------------------------------//

                            crntPDL = ToolCommonData.PrintLang.HPGL2;
                            seqComplete = true;
                        }
                        else if (actType == PrnParseConstants.ActPCL.SwitchToPJL)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Switch to PJL language processing.             //
                            //                                                //
                            //------------------------------------------------//

                            crntPDL = ToolCommonData.PrintLang.PJL;
                            seqComplete = true;
                        }
                        else if (actType == PrnParseConstants.ActPCL.TextParsing)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Sequence sets TextParsingMethod.               //
                            //                                                //
                            //------------------------------------------------//

                            _textParsingMethod = vInt;
                        }

                        if (optObsolete)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Sequence marked as obsolete.                   //
                            //                                                //
                            //------------------------------------------------//

                            PrnParseCommon.AddTextRow(
                                PrnParseRowTypes.Type.MsgComment,
                                _table,
                                PrnParseConstants.OvlShow.None,
                                string.Empty,
                                "Comment",
                                string.Empty,
                                "The following sequence is considered to be obsolete:");
                        }
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Check for 'macro stop' sequence.                       //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (actType == PrnParseConstants.ActPCL.MacroStop)
                    {
                        if (_macroLevel > 0)
                            _macroLevel--;

                        PrnParseCommon.SetDisplayCriteria(_showMacroData, _macroLevel);
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Update sequence metrics.                               //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (comboFirst)
                    {
                        seqPos = vPosCrnt - prefixLen;
                        seqLen = vtLen + prefixLen;
                        seqStart = bufOffset;
                        comboStart = _fileOffset + seqStart;

                        if (seqProprietary)
                            typeText = "PCL Extension";
                        else
                            typeText = "PCL Parameterised";
                    }
                    else
                    {
                        seqPos = vPosCrnt;
                        seqLen = vtLen;
                        seqStart = vPosCrnt;

                        typeText = string.Empty;
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // If a 'Make Overlay' run, check what action (if any) is //
                    // required.                                              //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (_parseType == PrnParse.ParseType.MakeOverlay)
                    {
                        int fragLen;

                        if (comboFirst)
                            fragLen = vtLen + prefixLen + 1;
                        else
                            fragLen = vtLen;

                        _linkData.MakeOvlAct = makeOvlAct;
                        _linkData.PclComboModified = comboModified;

                        _linkData.SetPrefixData(prefixLen, iChar, gChar);

                        breakpoint = PrnParseMakeOvl.CheckActionPCL(
                                        comboSeq,
                                        seqComplete,
                                        vInt,
                                        seqStart,
                                        fragLen,
                                        _fileOffset,
                                        _linkData,
                                        _table,
                                        _indxOffsetFormat);

                        makeOvlAct = _linkData.MakeOvlAct;
                        makeOvlShow = _linkData.MakeOvlShow;

                        if (makeOvlAct == PrnParseConstants.OvlAct.Adjust)
                            makeOvlAct = PrnParseConstants.OvlAct.None;

                        if (breakpoint)
                        {
                            contType = _linkData.GetContType();

                            comboModified = _linkData.PclComboModified;

                            _linkData.SetPCLComboData(comboSeq,
                                                       comboFirst,
                                                       comboLast,
                                                       comboModified,
                                                       comboStart);
                        }
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Output details and interpretation of (this part of)    //
                    // the sequence.                                          //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (seqProprietary)
                    {
                        /*
                        switch (vendorCode)
                        {
                            case eVendorDataProducts:
                                vendorName = "Data Products";
                                break;

                            case eVendorOce:
                                vendorName = "Oce";
                                break;

                            default:
                                vendorName = "Unknown";
                        }
                        */
                        vendorName = "Unknown"; // TEMPORARY //

                        PrnParseCommon.AddTextRow(
                            PrnParseRowTypes.Type.MsgComment,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            string.Empty,
                            "Comment",
                            string.Empty,
                            $"The following sequence is proprietary to {vendorName}:");
                    }

                    if (vLen > 0)
                    {
                        if (optDisplayHexVal &&
                            vCheck) // vCheck ensures we don't do this for invalid or fractional values
                        {
                            val = _ascii.GetString(_buf, vPosCrnt, vLen) + $" (0x{vInt.ToString("x")})";
                        }
                        else
                        {
                            val = _ascii.GetString(_buf, vPosCrnt, vLen);
                        }
                    }
                    else
                    {
                        val = string.Empty;
                    }

                    if (seqLen > 0)
                        seq = _ascii.GetString(_buf, seqPos, seqLen);
                    else
                        seq = string.Empty;

                    if (seqProprietary)
                    {
                        ParseSequenceComplexDisplay(_fileOffset + seqStart,
                                                     prefixLen,
                                                     comboFirst,
                                                     makeOvlShow,
                                                     typeText,
                                                     seq,
                                                     descComplex,
                                                     val);
                    }
                    else
                    {
                        if (seqKnown)
                        {
                            if ((makeOvlAct == PrnParseConstants.OvlAct.IdMacro) 
                                            &&
                                (vInt != _linkData.MakeOvlMacroId))
                            {
                                makeOvlAct = PrnParseConstants.OvlAct.None;
                            }

                            ParseSequenceComplexDisplay(_fileOffset + seqStart,
                                                         prefixLen,
                                                         comboFirst,
                                                         makeOvlShow,
                                                         typeText,
                                                         seq,
                                                         descComplex,
                                                         val);

                            if ((actType == PrnParseConstants.ActPCL.StyleData) && _interpretStyle)
                            {
                                ProcessStyleData(vInt);
                            }
                        }
                        else
                        {
                            ParseSequenceComplexDisplay(
                                _fileOffset + seqStart,
                                prefixLen,
                                comboFirst,
                                makeOvlShow,
                                typeText,
                                seq,
                                "***** Unknown sequence *****",
                                val);
                        }
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Adjust pointer to start of next (part) sequence.       //
                    // If the sequence is recognised as being a 'download'    //
                    // type of sequence, the pointer will reference the start //
                    // of 'download' characters.                              //
                    // Otherwise, the pointer will reference either the start //
                    // of the next escape sequence (if the T_CHAR has been    //
                    // found), or the start of the 'value' field of the next  //
                    // part of a combination sequence.                        //
                    //                                                        //
                    //--------------------------------------------------------//

                    vPosCrnt += vtLen;

                    if (comboSeq)
                        vPosNext = vPosCrnt;

                    if (!seqComplete)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Terminator character has not yet been found, so a  //
                        // combination sequence is being processed.           //
                        //                                                    //
                        //----------------------------------------------------//

                        comboSeq = true;
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Check for 'macro start' sequence.                      //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (actType == PrnParseConstants.ActPCL.MacroStart)
                    {
                        if (!_showMacroData)
                        {
                            PrnParseCommon.AddTextRow(
                                PrnParseRowTypes.Type.MsgComment,
                                _table,
                                PrnParseConstants.OvlShow.None,
                                string.Empty,
                                "Comment",
                                string.Empty,
                                "Preference options inhibit display of macro contents");
                        }

                        _macroLevel++;

                        PrnParseCommon.SetDisplayCriteria(_showMacroData, _macroLevel);
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Check for 'download' sequence.                         //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (seqKnown && optValueIsLen)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Download sequence.                                 //
                        // Terminator character is expected to be followed by //
                        // download data bytes (the number of which is        //
                        // defined by the Value field preceding the T_CHAR    //
                        // (Terminator) or P_CHAR (Parameter) character.      //
                        //                                                    //
                        //----------------------------------------------------//

                        if (vInvalid || vFractional || vNegative)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Invalid bytecount for download sequence.       //
                            // Abort processing of the current sequence.      //
                            //                                                //
                            //------------------------------------------------//

                            invalidSeq = true;

                            PrnParseCommon.AddTextRow(
                                PrnParseRowTypes.Type.MsgWarning,
                                _table,
                                PrnParseConstants.OvlShow.None,
                                string.Empty,
                                "*** Warning ***",
                                string.Empty,
                                "Invalid bytecount in previous sequence");

                            PrnParseCommon.AddTextRow(
                                PrnParseRowTypes.Type.MsgWarning,
                                _table,
                                PrnParseConstants.OvlShow.None,
                                string.Empty,
                                "*** Warning ***",
                                string.Empty,
                                "Processing of current sequence abandoned");
                        }
                        else
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Valid download sequence introduction.          //
                            // Adjust external pointers to start of the       //
                            // download data (after the sequence termination  //
                            // character).                                    //
                            // Adjust internal pointers so that the           //
                            // adjustment after the end of the 'for' loop     //
                            // does not 'double count'.                       //
                            //                                                //
                            //------------------------------------------------//

                            seqLen = seqOffset + (vPosCrnt - vPosFirst);
                            bufRem -= seqLen;
                            bufOffset += seqLen;

                            seqOffset = 0;
                            vPosFirst = vPosCrnt;
                            binDataLen = vInt;

                            //------------------------------------------------//
                            //                                                //
                            // Process download data.                         //
                            //                                                //
                            //------------------------------------------------//

                            if ((_parseType == PrnParse.ParseType.MakeOverlay)
                                                  &&
                                (makeOvlAct == PrnParseConstants.OvlAct.Remove))
                            {
                                _linkData.MakeOvlSkipEnd += vInt;
                                _linkData.DataLen += vInt;
                            }

                            i += binDataLen;
                            vPosCrnt += binDataLen;

                            continuation = ParseSequenceEmbeddedData(
                                                ref bufRem,
                                                ref bufOffset,
                                                ref binDataLen,
                                                actType,
                                                vPosFirst,
                                                endPos,
                                                prefixLen,
                                                iChar,
                                                gChar,
                                                seqComplete,
                                                ref invalidSeq);

                            vPosFirst = vPosCrnt;
                            bufRem -= binDataLen;
                            bufOffset += binDataLen;
                            /*
                            i = i + binDataLen;
                            vPosCrnt = vPosCrnt + binDataLen;
                            vPosFirst = vPosCrnt;
                            */
                        }  // end of download sequence processing
                    }  // end of 'if download sequence'

                    if (comboSeq)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Reset flags for next part of combination sequence. //
                        //                                                    //
                        //----------------------------------------------------//

                        vInvalid = false;
                        vNegative = false;
                        vFractional = false;
                        vSignFound = false;
                        vStarted = false;
                        vNumberStarted = false;
                        vInt = 0;
                        binDataLen = 0;
                        vtLen = 0;
                    }
                }  // end of 'if termination character found'
            }  // end of 'for' loop

            //----------------------------------------------------------------//
            //                                                                //
            // Either the complete sequence has been found and processed, or  //
            // the end of buffer has been reached, or a continuation (for a   //
            // download sequence) has already been signalled.                 //
            // Calculate how much has been processed in this iteration, and   //
            // then adjust external buffer pointers and flags.                //
            //                                                                //
            //----------------------------------------------------------------//

            processedLen = seqOffset + (vPosCrnt - vPosFirst);
            bufRem -= processedLen;
            bufOffset += processedLen;

            if (continuation)
            {
                //------------------------------------------------------------//
                //                                                            //
                // End of buffer reached during processing of a 'download'    //
                // sequence.                                                  //
                // Do nothing - action already invoked.                       //
                //                                                            //
                //------------------------------------------------------------//
            }
            else if (breakpoint)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Breakpoint (indicating sequence removal required) found    //
                // during MakeMacro run.                                      //
                // Do nothing - action already invoked.                       //
                //                                                            //
                //------------------------------------------------------------//
            }
            else if (seqComplete)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Sequence Terminator character found - no continuation      //
                // necessary.                                                 //
                //                                                            //
                //------------------------------------------------------------//

                _linkData.ResetContData();
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // (Upper-case) sequence Terminator character not found       //
                // before end of buffer.                                      //
                // Initiate continuation action.                              //
                //                                                            //
                //------------------------------------------------------------//

                int dataLen;

                if (p_or_TCharFound)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // (Lower-case) Parameter character just encountered;     //
                    // buffer exhausted before any value bytes for the next   //
                    // part of the combination sequence.                      //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (seqProprietary)
                    {
                        contType = PrnParseConstants.ContType.PCLExtension;
                        dataLen = bufRem;
                    }
                    else
                    {
                        contType = PrnParseConstants.ContType.PCLComplex;
                        dataLen = bufRem;
                    }
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // At least one value byte, for the first or next part of //
                    // the complex sequence, has been encountered.            //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (!comboSeq)
                    {
                        contType = PrnParseConstants.ContType.Unknown;
                        dataLen = bufRem + processedLen;
                    }
                    else
                    {
                        contType = PrnParseConstants.ContType.PCLComplex;
                        dataLen = bufRem;
                    }
                }

                _linkData.SetContData(contType,
                                       prefixLen,
                                       -dataLen,
                                       0,
                                       true,
                                       iChar,
                                       gChar);

                _linkData.PclComboSeq = comboSeq;

                bufRem = 0;
            }

            return invalidSeq;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a r s e S e q u e n c e C o m p l e x D i s p l a y              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display details of the supplied sequence, in slices if necessary.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void ParseSequenceComplexDisplay(
            int offset,
            int prefixLen,
            bool firstPart,
            PrnParseConstants.OvlShow makeOvlShow,
            string type,
            string sequence,
            string description,
            string value)
        {
            const string escText = "<Esc>";

            bool firstSlice;

            int len,
                  opSeqFixLen,
                  opSeqOffset,
                  sliceLen,
                  sliceMax,
                  sliceOffset,
                  crntOffset;

            string opFixed,
                   typeText,
                   descText,
                   seqSlice;

            //----------------------------------------------------------------//
            //                                                                //
            // Display sequence (in slices if necessary).                     //
            //                                                                //
            //----------------------------------------------------------------//

            len = sequence.Length;
            firstSlice = true;
            sliceOffset = 0;
            opSeqFixLen = escText.Length;
            descText = string.Empty;

            if (firstPart)
            {
                opSeqOffset = opSeqFixLen;
                opFixed = escText;
            }
            else
            {
                opSeqOffset = opSeqFixLen + prefixLen;
                opFixed = new string(' ', opSeqOffset);
            }

            sliceMax = PrnParseConstants.cRptA_colMax_Seq - opSeqOffset;

            while (len > 0)
            {
                if (len > sliceMax)
                //--------------------------------------------------------//
                //                                                        //
                // Not last slice.                                        //
                //                                                        //
                //--------------------------------------------------------//
                {
                    sliceLen = sliceMax;
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Last, or only, slice.                                  //
                    //                                                        //
                    //--------------------------------------------------------//

                    int ptr = description.IndexOf("#");

                    sliceLen = len;

                    if (ptr == -1)
                    {
                        descText = description;
                    }
                    else
                    {
                        if (value.Length == 0)
                        {
                            descText = description.Substring(0, ptr) +
                                      "0" +
                                      description.Substring(ptr + 1);
                        }
                        else
                        {
                            descText = description.Substring(0, ptr) +
                                       value +
                                       description.Substring(ptr + 1);
                        }
                    }
                }

                seqSlice = sequence.Substring(sliceOffset, sliceLen);

                if (firstSlice)
                {
                    crntOffset = offset + sliceOffset;
                    typeText = type;
                }
                else
                {
                    crntOffset = offset + sliceOffset + 1;  // WHY ????????????????//
                    typeText = string.Empty;
                }

                PrnParseCommon.AddDataRow(
                    PrnParseRowTypes.Type.PCLSeqComplex,
                    _table,
                    makeOvlShow,
                    _indxOffsetFormat,
                    crntOffset,
                    _analysisLevel,
                    typeText,
                    opFixed + seqSlice,
                    descText);

                len -= sliceLen;
                sliceOffset += sliceLen;
                opSeqOffset = opSeqFixLen + prefixLen;

                sliceMax = PrnParseConstants.cRptA_colMax_Seq - opSeqOffset;

                opFixed = new string(' ', opSeqOffset);
                firstSlice = false;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a r s e S e q u e n c e E m b e d d e d D a t a                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process data associated with a PCL parameterised sequence where    //
        // value field indicates the length of the (often binary) data        //
        // associated with, and following, the sequence parameter (or         //
        // terminator) character.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ParseSequenceEmbeddedData(
            ref int bufRem,
            ref int bufOffset,
            ref int binDataLen,
            PrnParseConstants.ActPCL actType,
            int startPos,
            int endPos,
            int prefixLen,
            byte iChar,
            byte gChar,
            bool seqComplete,
            ref bool invalidSeqFound)
        {
            bool continuation,
                    hddrOK,
                    charOK,
                    dataOK;

            bool dummyBool = false;

            bool analyseRun;
            continuation = false;
            invalidSeqFound = false;

            if (binDataLen == 0)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Download sequence with zero length.                        //
                // (this does occur sometimes, especially with raster         //
                // graphics data).                                            //
                //                                                            //
                //------------------------------------------------------------//
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Download sequence with non-zero length.                    //
                //                                                            //
                //------------------------------------------------------------//

                analyseRun = _parseType == PrnParse.ParseType.Analyse;

                PrnParseConstants.OvlShow ovlShow;

                if (_parseType == PrnParse.ParseType.MakeOverlay)
                    ovlShow = _linkData.MakeOvlShow;
                else
                    ovlShow = PrnParseConstants.OvlShow.None;

                if (analyseRun &&
                    (actType == PrnParseConstants.ActPCL.FontHddr) &&
                    _analyseFontHddr)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is a PCL font header download, and the option //
                    // to analyse it has been invoked.                        //
                    //                                                        //
                    //--------------------------------------------------------//

                    hddrOK = _parseFontHddrPCL.AnalyseFontHddr(
                                binDataLen,
                                 _fileOffset,
                                _buf,
                                ref bufRem,
                                ref bufOffset,
                                _linkData,
                                _options,
                                _table);

                    if (!hddrOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.FontChar) &&
                         _analyseFontChar)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is a PCL font character download, and the     //
                    // option to analyse it has been invoked.                 //
                    //                                                        //
                    //--------------------------------------------------------//

                    charOK = _parseFontCharPCL.AnalyseFontChar(binDataLen,
                                                               _fileOffset,
                                                               _buf,
                                                               ref bufRem,
                                                               ref bufOffset,
                                                               _linkData,
                                                               _options,
                                                               _table);

                    if (!charOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.AlphaNumericID) &&
                         _transAlphaNumId)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is the AlphaNumericID command:                //
                    //      <esc>&n#W[op-char][string-data]                   //
                    // We must have, at least, the op-code character (as the  //
                    // binary data length is non-zero).                       //
                    //                                                        //
                    //--------------------------------------------------------//

                    dataOK = _parsePCLBinary.DecodeAlphaNumericID(
                        binDataLen,
                        _fileOffset,
                        _buf,
                        ref bufRem,
                        ref bufOffset,
                        _linkData,
                        _options,
                        _table);

                    if (!dataOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.ColourLookup) &&
                         _transColourLookup)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is the Colour Lookup Tables command:          //
                    //      <esc>*l#W[binary]                                 //
                    //                                                        //
                    //--------------------------------------------------------//

                    dataOK = _parsePCLBinary.DecodeColourLookup(
                        binDataLen,
                        _fileOffset,
                        _buf,
                        ref bufRem,
                        ref bufOffset,
                        _linkData,
                        _options,
                        _table);

                    if (!dataOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.ConfigurationIO) &&
                         _transConfIO)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is the Configuration (I/O) command:           //
                    //      <esc>&b#W[binary]                                 //
                    //                                                        //
                    //--------------------------------------------------------//

                    dataOK = _parsePCLBinary.DecodeConfigurationIO(
                        binDataLen,
                        _fileOffset,
                        _buf,
                        ref bufRem,
                        ref bufOffset,
                        _linkData,
                        _options,
                        _table);

                    if (!dataOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.ConfigureImageData) &&
                         _transConfImageData)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is the ConfigureImageData command:            //
                    //      <esc>*v#W[binary-data]                            //
                    // The common 'short form' uses 6 bytes of binary data;   //
                    // there are various long forms.                          //
                    //                                                        //
                    //--------------------------------------------------------//

                    dataOK = _parsePCLBinary.DecodeConfigureImageData(
                        binDataLen,
                        _fileOffset,
                        _buf,
                        ref bufRem,
                        ref bufOffset,
                        _linkData,
                        _options,
                        _table);

                    if (!dataOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.ConfigureRasterData) &&
                         _transConfRasterData)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is the ConfigureRasterData command:           //
                    //      <esc>*g#W[binary-data]                            //
                    // Similar purpose to the Configure Image Data sequence,  //
                    // but used with inkjet devices (PCL3 enhanced languages).//
                    // There are various formats 9not all of which are known  //
                    // here).                                                 //
                    //                                                        //
                    //--------------------------------------------------------//

                    dataOK = _parsePCLBinary.DecodeConfigureRasterData(
                        binDataLen,
                        _fileOffset,
                        _buf,
                        ref bufRem,
                        ref bufOffset,
                        _linkData,
                        _options,
                        _table);

                    if (!dataOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.LogicalPageData) &&
                         _transDefLogPage)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is the Define Logical Page command:           //
                    //      <esc>&a#W[binary-data]                            //
                    // Data length should be 4 or 10 bytes.                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    dataOK = _parsePCLBinary.DecodeDefineLogicalPage(
                        binDataLen,
                        _fileOffset,
                        _buf,
                        ref bufRem,
                        ref bufOffset,
                        _linkData,
                        _options,
                        _table);

                    if (!dataOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.DefineSymbolSet) &&
                         _transDefSymSet)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is the Define Symbol Set command:             //
                    //      <esc>(f#W[binary-data]                            //
                    //                                                        //
                    //--------------------------------------------------------//

                    dataOK = _parsePCLBinary.DecodeDefineSymbolSet(
                        binDataLen,
                        _fileOffset,
                        _buf,
                        ref bufRem,
                        ref bufOffset,
                        _linkData,
                        _options,
                        _table);

                    if (!dataOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.DitherMatrix) &&
                         _transDitherMatrix)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is the Download Dither Matrix command:        //
                    //      <esc>*m#W[binary]                                 //
                    //                                                        //
                    //--------------------------------------------------------//

                    dataOK = _parsePCLBinary.DecodeDitherMatrix(
                        binDataLen,
                        _fileOffset,
                        _buf,
                        ref bufRem,
                        ref bufOffset,
                        _linkData,
                        _options,
                        _table);

                    if (!dataOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.DriverConfiguration) &&
                         _transDriverConf)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is the Driver Configuration command:          //
                    //      <esc>*o#W[binary-data]                            //
                    //                                                        //
                    //--------------------------------------------------------//

                    dataOK = _parsePCLBinary.DecodeDriverConfiguration(
                        binDataLen,
                        _fileOffset,
                        _buf,
                        ref bufRem,
                        ref bufOffset,
                        _linkData,
                        _options,
                        _table);

                    if (!dataOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.EscEncText) &&
                         _transEscEncText)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is the Escapement Encapsulated Text command:  //
                    //      <esc>&p#W[binary]                                 //
                    //                                                        //
                    //--------------------------------------------------------//

                    dataOK = _parsePCLBinary.DecodeEscEncText(
                        binDataLen,
                        _fileOffset,
                        _buf,
                        ref bufRem,
                        ref bufOffset,
                        _linkData,
                        _options,
                        _table);

                    if (!dataOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.PaletteConfiguration) &&
                         _transPaletteConf)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is the Palette Configuration command:         //
                    //      <esc>&a#W[binary-data]                            //
                    // Data length is variable.                               //
                    //                                                        //
                    //--------------------------------------------------------//

                    dataOK = _parsePCLBinary.DecodePaletteConfiguration(
                        binDataLen,
                        _fileOffset,
                        _buf,
                        ref bufRem,
                        ref bufOffset,
                        _linkData,
                        _options,
                        _table);

                    if (!dataOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.UserDefinedPattern) &&
                         _transUserPattern)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is the User Defined Pattern command:          //
                    //      <esc>*c#W[data]                                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    dataOK = _parsePCLBinary.DecodeUserDefinedPattern(
                        binDataLen,
                        _fileOffset,
                        _buf,
                        ref bufRem,
                        ref bufOffset,
                        _linkData,
                        _options,
                        _table);

                    if (!dataOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.ViewIlluminant) &&
                         _transViewIlluminant)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence is the Viewing Illuminant command:            //
                    //      <esc>*i#W[binary]                                 //
                    //                                                        //
                    //--------------------------------------------------------//

                    dataOK = _parsePCLBinary.DecodeViewIlluminant(
                        binDataLen,
                        _fileOffset,
                        _buf,
                        ref bufRem,
                        ref bufOffset,
                        _linkData,
                        _options,
                        _table);

                    if (!dataOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.ConfigurationIO) &&
                         _analysePML)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Download contains data which may include an embedded   //
                    // PML sequence.                                          //
                    //                                                        //
                    //--------------------------------------------------------//

                    dataOK = _parsePCLBinary.DecodeEmbeddedPML(
                        binDataLen,
                        _fileOffset,
                        _buf,
                        ref bufRem,
                        ref bufOffset,
                        _linkData,
                        _options,
                        _table);

                    if (!dataOK)
                        invalidSeqFound = true;

                    binDataLen = 0;

                    continuation = _linkData.IsContinuation();
                }
                else if (analyseRun &&
                         (actType == PrnParseConstants.ActPCL.EmbeddedData))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Download contains data which is to be displayed as     //
                    // text, as far as possible.                              //
                    // A typical example of this is the data associated with  //
                    // the Transparent Print command:                         //
                    //      <esc>&p#X[data]                                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (binDataLen > bufRem)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Not all of string identifier is in current buffer. //
                        // Initiate continuation action.                      //
                        // We should only do this simple continuation if the  //
                        // length of the data will fit within a 'read block'. //
                        // TODO - *** need to cater for data longer than ***  //
                        //        *** buffer ?? check this out           ***  //
                        //                                                    //
                        //----------------------------------------------------//

                        continuation = true;

                        _linkData.SetContData(
                            PrnParseConstants.ContType.PCLEmbeddedData,
                            0,
                            -bufRem,
                            binDataLen,
                            true,
                            0x20,
                            0x20);
                    }
                    else
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // All of string identifier is in current buffer.     //
                        // Display details.                                   //
                        //                                                    //
                        //----------------------------------------------------//

                        PrnParseData.ProcessLines(
                            _table,
                            ovlShow,
                            _linkData,
                            ToolCommonData.PrintLang.Unknown,
                            _buf,
                            _fileOffset,
                            binDataLen,
                            ref bufRem,
                            ref bufOffset,
                            ref dummyBool,
                            true,
                            false,
                            true,
                            PrnParseConstants.asciiEsc,
                            "Embedded data",
                            0,
                            _indxCharSetSubAct,
                            (byte)_valCharSetSubCode,
                            _indxCharSetName,
                            _indxOffsetFormat,
                            _analysisLevel);

                        binDataLen = 0;
                    }
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // No special sequence processing invoked, or not an      //
                    // Analyse run.                                           //
                    //                                                        //
                    //--------------------------------------------------------//

                    if ((startPos + binDataLen) > endPos)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // No special sequence processing invoked.            //
                        // Sequence straddles 'block' boundary.               //
                        // Initiate continuation action.                      //
                        //                                                    //
                        //----------------------------------------------------//

                        continuation = true;

                        int downloadRem = binDataLen;
                        binDataLen = endPos - startPos;
                        downloadRem -= binDataLen;

                        if (!seqComplete)
                        {
                            _linkData.SetContData(
                                PrnParseConstants.ContType.PCLDownloadCombo,
                                prefixLen,
                                0,
                                downloadRem,
                                false,
                                iChar,
                                gChar);

                            _linkData.PclComboSeq = true;
                        }
                        else
                        {
                            _linkData.SetContData(
                                PrnParseConstants.ContType.PCLDownload,
                                0,
                                0,
                                downloadRem,
                                false,
                                0x20,
                                0x20);

                            _linkData.PclComboSeq = false;
                        }
                    }

                    if (binDataLen == 0)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // None of the download data is contained within the  //
                        // current 'read block'.                              //
                        // The necessary Continuation processing has already  //
                        // been signalled.                                    //
                        //                                                    //
                        //----------------------------------------------------//
                    }
                    else
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Some, or all, of the download data is contained    //
                        // within the current 'read block'.                   //
                        //                                                    //
                        //----------------------------------------------------//

                        PrnParseData.ProcessBinary(
                            _table,
                            ovlShow,
                            _buf,
                            _fileOffset,
                            bufOffset,
                            binDataLen,
                            "PCL Binary",
                            _showBinData,
                            false,
                            true,
                            _indxOffsetFormat,
                            _analysisLevel);
                    }
                }
            }

            return continuation;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a r s e S e q u e n c e S i m p l e                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Format of simple sequences:                                        //
        //                                                                    //
        //      byte     Escape character (x'1B')                             //
        //      byte     Indicator character               (I_CHAR)           //
        //                                                                    //
        // Each table entry matches an I_CHAR value against a description     //
        // Note that any 'switch_language' sequences may be processed as      //
        // Special Escape Sequences (provided the current language allows)    //
        // rather than as Simple Escape Sequences.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ParseSequenceSimple(
            ref int bufRem,
            ref int bufOffset,
            ref bool breakpoint)
        {
            byte iChar;
            bool optObsolete = false;
            bool optResetHPGL2 = false;

            var makeOvlAct = PrnParseConstants.OvlAct.None;

            var makeOvlShow = PrnParseConstants.OvlShow.None;

            string descSimple = string.Empty;

            iChar = _buf[bufOffset + 1];

            bool seqKnown = PCLSimpleSeqs.CheckSimpleSeq(
                _analysisLevel + _macroLevel,
                iChar,
                ref optObsolete,
                ref optResetHPGL2,
                ref makeOvlAct,
                ref descSimple);

            if (seqKnown)
            {
                if (optResetHPGL2)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence resets HP-GL/2 state variables.               //
                    //                                                        //
                    //--------------------------------------------------------//

                    //        _HPGL2Analysis->ResetHPGL2();
                }

                if (optObsolete)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Sequence marked as obsolete.                           //
                    //                                                        //
                    //--------------------------------------------------------//

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.Type.MsgComment,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Comment",
                        string.Empty,
                        "The following sequence is considered to be obsolete:");
                }

                if (_parseType == PrnParse.ParseType.MakeOverlay)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // MakeMacro action indicated.                            //
                    //                                                        //
                    //--------------------------------------------------------//

                    _linkData.MakeOvlAct = makeOvlAct;

                    breakpoint = PrnParseMakeOvl.CheckActionPCL(
                                    false,
                                    true,
                                    -1,
                                    bufOffset,
                                    2,
                                    _fileOffset,
                                    _linkData,
                                    _table,
                                    _indxOffsetFormat);

                    makeOvlAct = _linkData.MakeOvlAct;
                    makeOvlShow = _linkData.MakeOvlShow;
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Output details and interpretation of the sequence.             //
            //                                                                //
            //----------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                PrnParseRowTypes.Type.PCLSeqSimple,
                _table,
                makeOvlShow,
                _indxOffsetFormat,
                _fileOffset + bufOffset,
                _analysisLevel,
                "PCL Simple",
                "<Esc>" + (char)iChar,
                descSimple);

            bufRem -= 2;
            bufOffset += 2;

            return seqKnown;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s S t y l e D a t a                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Dispays an interpretation of the components of the style value.    //
        //                                                                    //
        // Bit numbers are zero-indexed from (left) Most Significant:         //
        //                                                                    //
        //    bits  0  -  5   Reserved                                        //
        //          6  - 10   Structure  (e.g. Solid)                         //
        //         11  - 13   Width      (e.g. Condensed)                     //
        //         14  - 15   Posture    (e.g. Italic)                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ProcessStyleData(int style)
        {
            int index;

            string itemDesc;

            index = (style >> 5) & 0x1f;

            switch (index)
            {
                case 0:
                    itemDesc = "0: Solid (Normal, Black)";
                    break;

                case 1:
                    itemDesc = "32: Outline (Hollow)";
                    break;

                case 2:
                    itemDesc = "64: Inline (Incised, Engraved)";
                    break;

                case 3:
                    itemDesc = "96: Contour, Edged (Antique, Distressed)";
                    break;

                case 4:
                    itemDesc = "128: Solid with Shadow";
                    break;

                case 5:
                    itemDesc = "160: Outline with Shadow";
                    break;

                case 6:
                    itemDesc = "192: Inline with Shadow";
                    break;

                case 7:
                    itemDesc = "224: Contour, or Edged, with Shadow";
                    break;

                case 8:
                    itemDesc = "256: Pattern Filled";
                    break;

                case 9:
                    itemDesc = "288: Pattern Filled 1";
                    break;

                case 10:
                    itemDesc = "320: Pattern Filled 2";
                    break;

                case 11:
                    itemDesc = "352: Pattern Filled 3";
                    break;

                case 12:
                    itemDesc = "384: Pattern Filled with Shadow";
                    break;

                case 13:
                    itemDesc = "416: Pattern Filled with Shadow 1";
                    break;

                case 14:
                    itemDesc = "448: Pattern Filled with Shadow 2";
                    break;

                case 15:
                    itemDesc = "480: Pattern Filled with Shadow 3";
                    break;

                case 16:
                    itemDesc = "512: Inverse";
                    break;

                case 17:
                    itemDesc = "544: Inverse with Border";
                    break;

                default:
                    itemDesc = ">=576: Unknown (Reserved)";
                    break;
            }

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.Type.PCLDecode,
                _table,
                PrnParseConstants.OvlShow.None,
                string.Empty,
                "     ----> Structure",
                string.Empty,
                itemDesc);

            index = (style >> 2) & 0x07;

            switch (index)
            {
                case 0:
                    itemDesc = "0: Normal";
                    break;

                case 1:
                    itemDesc = "4: Condensed";
                    break;

                case 2:
                    itemDesc = "8: Compressed (Extra Condensed)";
                    break;

                case 3:
                    itemDesc = "12: Extra Compressed";
                    break;

                case 4:
                    itemDesc = "16: Ultra Compressed";
                    break;

                case 5:
                    itemDesc = "20: Unknown (Reserved)";
                    break;

                case 6:
                    itemDesc = "24: Expanded (Extended)";
                    break;

                case 7:
                    itemDesc = "28: Extra Expanded (Extra Extended)";
                    break;

                default:
                    itemDesc = ">=32: Impossible?";
                    break;
            }

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.Type.PCLDecode,
                _table,
                PrnParseConstants.OvlShow.None,
                string.Empty,
                "     ----> Width",
                string.Empty,
                itemDesc);

            index = style & 0x03;

            switch (index)
            {
                case 0:
                    itemDesc = "0: Upright";
                    break;

                case 1:
                    itemDesc = "1: Oblique, Italic";
                    break;

                case 2:
                    itemDesc = "2: Alternate Italic (Backslanted, Cursive, Swash)";
                    break;

                case 3:
                    itemDesc = "3: Unknown (Reserved)";
                    break;

                default:
                    itemDesc = ">=4: Impossible?";
                    break;
            }

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.Type.PCLDecode,
                _table,
                PrnParseConstants.OvlShow.None,
                string.Empty,
                "     ----> Posture",
                string.Empty,
                itemDesc);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s e t P C L                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Resets PCL state variables.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        //private void ResetPCL()
        //{
        //    _textParsingMethod = (int)PCLTextParsingMethods.PCLVal.m0_1_byte_default;
        //    _macroLevel = 0;
        //}

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t T a b l e                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set Datatable target (used by Make Overlay inserts).               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetTable(DataTable table) => _table = table;
    }
}