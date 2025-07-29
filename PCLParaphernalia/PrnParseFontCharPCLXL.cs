using System.Data;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class handles PCL XL downloadable soft font characters.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

class PrnParseFontCharPCLXL
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private enum eStage
    {
        Start = 0,
        CheckDataHddr,
        ShowDataHddr,
        ShowDataBody,
        ShowDataRem,
        EndOK,
        BadSeqA,
        BadSeqB
    }

    private enum ePCLXLCharFormat
    {
        Bitmap = 0,
        TrueType = 1
    }

    private enum ePCLXLCharClass
    {
        Bitmap = 0,
        TTFClass0 = 0,
        TTFClass1 = 1,
        TTFClass2 = 2
    }

    private const int _blockHddrLen = 2;

    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private PrnParseLinkData _linkData;
    private PrnParseOptions _options;

    private DataTable _table;

    private eStage _nextStage;
    private ePCLXLCharFormat _charFormat;
    private ePCLXLCharClass _charClass;

    private byte[] _buf;

    private int _fileOffset;
    private int _analysisLevel;

    private int _charLen;
    private int _charRem;
    private int _charPos;
    private int _charHddrLen;
    private int _charDataLen;
    private int _charHeight;
    private int _charWidth;

    private int _charDataSize;

    private int _drawCharMaxHeight;
    private int _drawCharMaxWidth;

    private bool _showBinData;
    private bool _validChar;

    private bool _drawCharShape;

    private bool _bitmapFont;
    private bool _truetypeFont;

    private PrnParseConstants.eOptOffsetFormats _indxOffsetFormat;

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // a n a l y s e F o n t C h a r                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Provide an interpretation of the contents of a PCL soft font       //
    // character description/data block.                                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool AnalyseFontChar(int charLen,
                                   byte[] buf,
                                   int fileOffset,
                                   ref int bufRem,
                                   ref int bufOffset,
                                   PrnParseLinkData linkData,
                                   PrnParseOptions options,
                                   DataTable table)
    {
        int binDataLen;

        PrnParseConstants.eContType contType;

        //----------------------------------------------------------------//
        //                                                                //
        // Initialise.                                                    //
        //                                                                //
        //----------------------------------------------------------------//

        _table = table;
        _buf = buf;
        _fileOffset = fileOffset;

        _linkData = linkData;
        _options = options;

        contType = _linkData.GetContType();
        _analysisLevel = _linkData.AnalysisLevel;

        _indxOffsetFormat = _options.IndxGenOffsetFormat;
        _showBinData = _options.FlagPCLXLMiscBinData;

        _options.GetOptPCLXLDraw(ref _drawCharShape,
                                  ref _drawCharMaxHeight,
                                  ref _drawCharMaxWidth);

        if (contType == PrnParseConstants.eContType.None)
        {
            _nextStage = eStage.Start;
            _validChar = true;

            _charLen = charLen;
            _charRem = charLen;
            _charPos = fileOffset + bufOffset;

            _charHeight = -1;
            _charWidth = -1;
        }
        else
        {
            contType = PrnParseConstants.eContType.None;
            _linkData.ResetContData();
        }

        if (_nextStage == eStage.Start)
        {
            if (bufRem < (_blockHddrLen))
            {
                //--------------------------------------------------------//
                //                                                        //
                // First two bytes (Format & Class) are not in buffer.    //
                // Initiate continuation.                                 //
                //                                                        //
                //--------------------------------------------------------//

                contType = PrnParseConstants.eContType.PCLXLFontChar;

                _linkData.SetBacktrack(contType, -bufRem);
            }
            else
            {
                ProcessBlockHeader(ref bufRem,
                                    ref bufOffset);
            }
        }

        if (_nextStage == eStage.CheckDataHddr)
        {
            if (_charHddrLen > bufRem)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Data Header is not entirely within buffer.             //
                // Initiate (back-tracking) continuation.                 //
                //                                                        //
                //--------------------------------------------------------//

                contType = PrnParseConstants.eContType.PCLXLFontChar;

                _linkData.SetBacktrack(contType, -bufRem);
            }
            else
            {
                _nextStage = eStage.ShowDataHddr;
            }
        }

        if (_nextStage == eStage.ShowDataHddr)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Process character data header.                             //
            //                                                            //
            //------------------------------------------------------------//

            ProcessDataHeader(ref bufRem,
                               ref bufOffset);
        }

        if ((_nextStage == eStage.ShowDataBody) && (_charRem != 0))
        {
            //------------------------------------------------------------//
            //                                                            //
            // Output details of (first part of) variable data.           //
            //                                                            //
            //------------------------------------------------------------//

            if (_charFormat == ePCLXLCharFormat.Bitmap)
                ProcessRasterDataBody(ref bufRem, ref bufOffset);
            else if (_charFormat == ePCLXLCharFormat.TrueType)
                ProcessTrueTypeDataBody(ref bufRem, ref bufOffset);
            else
                _nextStage = eStage.BadSeqA;
        }

        if ((_nextStage == eStage.ShowDataRem) && (_charRem != 0))
        {
            //------------------------------------------------------------//
            //                                                            //
            // Output details of (remainder of) variable data.            //
            //                                                            //
            //------------------------------------------------------------//

            if (_charFormat == ePCLXLCharFormat.Bitmap)
                ProcessRasterDataRem(ref bufRem, ref bufOffset);
            else if (_charFormat == ePCLXLCharFormat.TrueType)
                ProcessTrueTypeDataBody(ref bufRem, ref bufOffset);
            else
                _nextStage = eStage.BadSeqA;
        }

        if (_nextStage == eStage.EndOK)
        {
            //------------------------------------------------------------//
            //                                                            //
            // End of processing of valid header.                         //
            //                                                            //
            //------------------------------------------------------------//

            return _validChar;
        }

        if (_nextStage == eStage.BadSeqA)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Inconsistency found.                                       //
            //                                                            //
            //------------------------------------------------------------//

            _nextStage = eStage.BadSeqB;

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgError,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                string.Empty,
                string.Empty,
                "Processing of header abandoned");
        }

        if ((_nextStage == eStage.BadSeqB) && (_charRem != 0))
        {
            //------------------------------------------------------------//
            //                                                            //
            // Header does not appear to be valid.                        //
            // Treat remainder of header as a binary sequence without     //
            // interpretation.                                            //
            // Check if remainder of download sequence is within the      //
            // buffer.                                                    //
            //                                                            //
            //------------------------------------------------------------//

            if (_charRem > bufRem)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Remainder of sequence is not in buffer.                //
                // Initiate continuation.                                 //
                //                                                        //
                //--------------------------------------------------------//

                contType = PrnParseConstants.eContType.PCLXLFontChar;

                binDataLen = bufRem;
                _charRem -= bufRem;

                _linkData.SetContinuation(contType);
            }
            else
            {
                contType = PrnParseConstants.eContType.None;
                _linkData.ResetContData();

                binDataLen = _charRem;
                _charRem = 0;
            }

            if ((binDataLen) != 0)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Some, or all, of the download data is contained within //
                // the current 'block'.                                   //
                //                                                        //
                //--------------------------------------------------------//

                PrnParseCommon.AddDataRow(
                    PrnParseRowTypes.eType.DataBinary,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _indxOffsetFormat,
                    fileOffset + bufOffset,
                    _analysisLevel,
                    "PCLXL Binary",
                    "[ " + binDataLen + " bytes ]",
                    string.Empty);

                bufRem -= binDataLen;
                bufOffset += binDataLen;
            }
        }

        return _validChar;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s B l o c k H e a d e r                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Provide an interpretation of the contents of the Block header.     //
    //                                                                    //
    //    byte 0   Format                                                 //
    //         1   Class                                                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessBlockHeader(ref int bufRem,
                                     ref int bufOffset)
    {
        string itemDesc;

        //----------------------------------------------------------------//
        //                                                                //
        // Show size and (optionally) data.                               //
        //                                                                //
        //----------------------------------------------------------------//

        PrnParseCommon.AddDataRow(
            PrnParseRowTypes.eType.DataBinary,
            _table,
            PrnParseConstants.eOvlShow.None,
            _indxOffsetFormat,
            _fileOffset + bufOffset,
            _analysisLevel,
            "PCLXL Binary",
            "[ " + _blockHddrLen + " bytes ]",
            "Block header");

        if (_showBinData)
        {
            PrnParseData.ProcessBinary(
                _table,
                PrnParseConstants.eOvlShow.None,
                _buf,
                _fileOffset,
                bufOffset,
                _blockHddrLen,
                string.Empty,
                true,
                false,
                true,
                _indxOffsetFormat,
                _analysisLevel);
        }

        _charFormat = (ePCLXLCharFormat)_buf[bufOffset];
        _charClass = (ePCLXLCharClass)_buf[bufOffset + 1];
        _validChar = true;

        //----------------------------------------------------------------//
        //                                                                //
        // Format (byte 0).                                               //
        //                                                                //
        //----------------------------------------------------------------//

        switch (_charFormat)
        {
            case ePCLXLCharFormat.Bitmap:
                itemDesc = "0: Bitmap";
                _bitmapFont = true;
                _truetypeFont = false;

                break;

            case ePCLXLCharFormat.TrueType:
                itemDesc = "1: Truetype";
                _bitmapFont = false;
                _truetypeFont = true;

                break;

            default:
                itemDesc = _charFormat.ToString() + ": Unknown";
                _bitmapFont = false;
                _truetypeFont = false;
                _validChar = false;
                break;
        }

        PrnParseCommon.AddTextRow(
            PrnParseRowTypes.eType.PCLXLFontChar,
            _table,
            PrnParseConstants.eOvlShow.None,
            string.Empty,
            "Format:",
            string.Empty,
            itemDesc);

        //----------------------------------------------------------------//
        //                                                                //
        // Class (byte 1).                                                //
        //                                                                //
        //----------------------------------------------------------------//

        itemDesc = _charClass.ToString() + ": Unknown";

        if (_bitmapFont)
        {
            if (_charClass == ePCLXLCharClass.Bitmap)
            {
                itemDesc = "0: Bitmap";
                _charHddrLen = 8;
            }
            else
                _validChar = false;
        }
        else if (_truetypeFont)
        {
            if (_charClass == ePCLXLCharClass.TTFClass0)
            {
                itemDesc = "0: Dense";
                _charHddrLen = 4;
            }
            else if (_charClass == ePCLXLCharClass.TTFClass1)
            {
                itemDesc = "1: Sparse";
                _charHddrLen = 8;
            }
            else if (_charClass == ePCLXLCharClass.TTFClass2)
            {
                itemDesc = "2: Sparse - vertical rotated";
                _charHddrLen = 10;
            }
            else
                _validChar = false;
        }

        PrnParseCommon.AddTextRow(
            PrnParseRowTypes.eType.PCLXLFontChar,
            _table,
            PrnParseConstants.eOvlShow.None,
            string.Empty,
            "Class:",
            string.Empty,
            itemDesc);

        //----------------------------------------------------------------//
        //                                                                //
        // Adjust pointers & check validity.                              //
        //                                                                //
        //----------------------------------------------------------------//

        bufRem -= _blockHddrLen;
        bufOffset += _blockHddrLen;
        _charRem -= _blockHddrLen;

        if (_validChar)
        {
            _nextStage = eStage.CheckDataHddr;
        }
        else
        {
            _nextStage = eStage.BadSeqA;

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgWarning,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                string.Empty,
                string.Empty,
                "Format and/or Class invalid");
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s D a t a H e a d e r                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Provide an interpretation of the contents of the Character Data    //
    // header bytes.                                                      //
    // The content of the header depends on the Format and Class          //
    // attributes (found in the block header).                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessDataHeader(ref int bufRem,
                                    ref int bufOffset)
    {
        ushort ui16a;

        uint ui32a;

        short si16a;

        //----------------------------------------------------------------//
        //                                                                //
        // Show size (calculated when block header processed) and         //
        // (optionally) the binary header data.                           //
        //                                                                //
        //----------------------------------------------------------------//

        PrnParseCommon.AddDataRow(
            PrnParseRowTypes.eType.DataBinary,
            _table,
            PrnParseConstants.eOvlShow.None,
            _indxOffsetFormat,
            _fileOffset + bufOffset,
            _analysisLevel,
            "PCLXL Binary",
            "[ " + _charHddrLen + " bytes ]",
            "Character data header");

        if (_showBinData)
        {
            PrnParseData.ProcessBinary(
                _table,
                PrnParseConstants.eOvlShow.None,
                _buf,
                _fileOffset,
                bufOffset,
                _charHddrLen,
                string.Empty,
                true,
                false,
                true,
                _indxOffsetFormat,
                _analysisLevel);
        }

        if (_charFormat == ePCLXLCharFormat.Bitmap)
        {
            ushort bytesPerRow;

            //------------------------------------------------------------//
            //                                                            //
            // Bitmap font.                                               //
            //------------------------------------------------------------//
            //                                                            //
            // Left offset (bytes 0 & 1).                                 //
            //                                                            //
            // Distance from the 'reference point' to the left side of    //
            // the character.                                             //
            //                                                            //
            //------------------------------------------------------------//

            si16a = (short)((_buf[bufOffset] * 256) +
                              _buf[bufOffset + 1]);

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.PCLXLFontChar,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Left Offset:",
                string.Empty,
                si16a.ToString() + " dots");

            //------------------------------------------------------------//
            //                                                            //
            // Top offset (bytes 2 & 3).                                  //
            //                                                            //
            // Distance from the 'reference point' to the top of the      //
            // character.                                                 //
            //                                                            //
            //------------------------------------------------------------//

            si16a = (short)((_buf[bufOffset + 2] * 256) +
                              _buf[bufOffset + 3]);

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.PCLXLFontChar,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Top Offset:",
                string.Empty,
                si16a.ToString() + " dots");

            //------------------------------------------------------------//
            //                                                            //
            // Character Width (bytes 4 & 5).                             //
            //                                                            //
            //------------------------------------------------------------//

            _charWidth = (ushort)((_buf[bufOffset + 4] * 256) +
                               _buf[bufOffset + 5]);

            bytesPerRow = (ushort)((_charWidth / 8));

            if ((_charWidth % 8) != 0)
                bytesPerRow++;

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.PCLXLFontChar,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Character Width:",
                string.Empty,
                _charWidth + " dots (requires " +
                bytesPerRow + " padded bytes per row)");

            //------------------------------------------------------------//
            //                                                            //
            // Character Height (bytes 6 & 7).                            //
            //                                                            //
            //------------------------------------------------------------//

            _charHeight = (ushort)((_buf[bufOffset + 6] * 256) +
                                     _buf[bufOffset + 7]);

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.PCLXLFontChar,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Character Height:",
                string.Empty,
                _charHeight.ToString() + " dots");

            //------------------------------------------------------------//
            //                                                            //
            // Estimated character data size:                             //
            //    ((paddedCharWidth) / 8) * charHeight) bytes.            //
            //                                                            //
            //------------------------------------------------------------//

            ui32a = (uint)(bytesPerRow * _charHeight);

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.PCLXLFontChar,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Raster data size:",
                string.Empty,
                ui32a + " bytes (assuming " +
                _charHeight + " rows of " +
                bytesPerRow + " bytes)");

            if (ui32a != (_charLen - _blockHddrLen - _charHddrLen))
            {
                _validChar = false;

                _nextStage = eStage.BadSeqA;

                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.eType.MsgWarning,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    string.Empty,
                    "*** Warning ***",
                    string.Empty,
                    "Estimated data size (" + ui32a + " bytes)" +
                    " inconsistent with");

                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.eType.MsgWarning,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    "download size = " + _charLen +
                    ", block header = " + _blockHddrLen + " and ");

                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.eType.MsgWarning,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    "data header = " + _charHddrLen + " bytes");
            }
        }
        else if (_truetypeFont)
        {
            //------------------------------------------------------------//
            // TrueType font.                                             //
            //------------------------------------------------------------//
            //                                                            //
            // Character Data Size (bytes 0 & 1).                         //
            // This gives the size of the remainder of the header         //
            // (including these two bytes) plus the glyph data size.      //
            // It should hence be equal to the total CharacterLength      //
            // minus the block header length (the size of the Format and  //
            // Class bytes).                                              //
            //                                                            //
            //------------------------------------------------------------//

            _charDataSize = (short)((_buf[bufOffset] * 256) +
                                      _buf[bufOffset + 1]);

            ui16a = (ushort)(_charDataSize - _charHddrLen);

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.PCLXLFontChar,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Character Data Size:",
                string.Empty,
                _charDataSize.ToString() + " bytes " +
                " (header = " + _charHddrLen +
                "; glyph data = " + ui16a + ")");

            if ((_charDataSize + _blockHddrLen) != _charLen)
            {
                _validChar = false;

                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.eType.MsgWarning,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    string.Empty,
                    "*** Warning ***",
                    string.Empty,
                    "Character Data Size (" + _charDataSize + " bytes)" +
                    " inconsistent with");

                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.eType.MsgWarning,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    "download size = " + _charLen +
                    " and block header = " + _blockHddrLen + " bytes");
            }
            else
            {
                //--------------------------------------------------------//
                //                                                        //
                // Left Side Bearing.                                     //
                // For class 0 - not present.                             //
                //           1 - bytes 2 & 3.                             //
                //           2 - bytes 2 & 3.                             //
                //                                                        //
                //--------------------------------------------------------//

                if ((_charClass == ePCLXLCharClass.TTFClass1) ||
                    (_charClass == ePCLXLCharClass.TTFClass2))
                {
                    si16a = (short)((_buf[bufOffset + 2] * 256) +
                                      _buf[bufOffset + 3]);

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.eType.PCLXLFontChar,
                        _table,
                        PrnParseConstants.eOvlShow.None,
                        string.Empty,
                        "Left Side Bearing:",
                        string.Empty,
                        si16a.ToString() + " font units");
                }

                //--------------------------------------------------------//
                //                                                        //
                // Advance Width.                                         //
                // For class 0 - not present.                             //
                //           1 - bytes 4 & 5.                             //
                //           2 - bytes 4 & 5.                             //
                //                                                        //
                //--------------------------------------------------------//

                if ((_charClass == ePCLXLCharClass.TTFClass1) ||
                    (_charClass == ePCLXLCharClass.TTFClass2))
                {
                    si16a = (short)((_buf[bufOffset + 4] * 256) +
                                      _buf[bufOffset + 5]);

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.eType.PCLXLFontChar,
                        _table,
                        PrnParseConstants.eOvlShow.None,
                        string.Empty,
                        "Advance Width:",
                        string.Empty,
                        si16a.ToString() + " font units");
                }

                //--------------------------------------------------------//
                //                                                        //
                // Top Side Bearing.                                      //
                // For class 0 - not present.                             //
                //           1 - not present.                             //
                //           2 - bytes 6 & 7.                             //
                //                                                        //
                //--------------------------------------------------------//

                if (_charClass == ePCLXLCharClass.TTFClass2)
                {
                    si16a = (short)((_buf[bufOffset + 6] * 256) +
                                      _buf[bufOffset + 7]);

                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.eType.PCLXLFontChar,
                        _table,
                        PrnParseConstants.eOvlShow.None,
                        string.Empty,
                        "Top Side Bearing:",
                        string.Empty,
                        si16a.ToString() + " font units");
                }

                //--------------------------------------------------------//
                //                                                        //
                // TrueType Glyph ID.                                     //
                // For class 0 - bytes  2 &  3.                           //
                //           1 - bytes  6 &  7.                           //
                //           2 - bytes  8 &  9.                           //
                //                                                        //
                //--------------------------------------------------------//

                ui16a = 0;

                if (_charClass == ePCLXLCharClass.TTFClass0)
                {
                    ui16a = (ushort)((_buf[bufOffset + 2] * 256) +
                                       _buf[bufOffset + 3]);
                }
                else if (_charClass == ePCLXLCharClass.TTFClass1)
                {
                    ui16a = (ushort)((_buf[bufOffset + 6] * 256) +
                                       _buf[bufOffset + 7]);
                }
                else if (_charClass == ePCLXLCharClass.TTFClass2)
                {
                    ui16a = (ushort)((_buf[bufOffset + 8] * 256) +
                                       _buf[bufOffset + 9]);
                }

                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.eType.PCLXLFontChar,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    string.Empty,
                    "TrueType Glyph ID:",
                    string.Empty,
                    ui16a.ToString());
            }
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Adjust pointers.                                               //
        //                                                                //
        //----------------------------------------------------------------//

        bufRem -= _charHddrLen;
        bufOffset += _charHddrLen;
        _charRem -= _charHddrLen;
        _charDataLen = _charRem;

        if (_validChar)
            _nextStage = eStage.ShowDataBody;
        else
            _nextStage = eStage.BadSeqA;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s R a s t e r D a t a B o d y                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Process Raster character data.                                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessRasterDataBody(ref int bufRem,
                                        ref int bufOffset)
    {
        PrnParseConstants.eContType contType;

        int binDataLen;

        bool shapeTooLarge = false;

        if (_drawCharShape)
        {
            if ((_charHeight > _drawCharMaxHeight)
                            ||
                (_charWidth > _drawCharMaxWidth)
                            ||
                (_charDataLen > PrnParseConstants.bufSize))
            {
                shapeTooLarge = true;
            }
            else
            {
                shapeTooLarge = false;
            }
        }

        if (_charRem > bufRem)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Remainder of sequence is not in buffer.                    //
            // Initiate continuation (back-tracking if drawing character  //
            // shape).                                                    //
            //                                                            //
            //------------------------------------------------------------//

            if ((_drawCharShape) && (!shapeTooLarge))
            {
                contType = PrnParseConstants.eContType.PCLFontChar;

                _linkData.SetBacktrack(contType, -bufRem);

                bufOffset += bufRem;
                bufRem = 0;
                binDataLen = 0;
            }
            else
            {
                _nextStage = eStage.ShowDataRem;

                contType = PrnParseConstants.eContType.PCLXLFontChar;

                binDataLen = bufRem;
                _charRem -= bufRem;

                _linkData.SetContinuation(contType);
            }
        }
        else
        {
            contType = PrnParseConstants.eContType.None;
            _linkData.ResetContData();

            binDataLen = _charRem;
            _charRem = 0;

            _nextStage = eStage.EndOK;
        }

        if ((binDataLen) != 0)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Some, or all, of the download data is contained within the //
            // current 'block'.                                           //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                PrnParseRowTypes.eType.DataBinary,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                _fileOffset + bufOffset,
                _analysisLevel,
                "PCLXL Binary",
                "[ " + binDataLen + " bytes ]",
                "Raster character data");

            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    binDataLen,
                    string.Empty,
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_drawCharShape)
            {
                RasterDraw(bufOffset, binDataLen, shapeTooLarge);
            }

            //------------------------------------------------------------//
            //                                                            //
            // Adjust pointers.                                           //
            //                                                            //
            //------------------------------------------------------------//

            bufRem -= binDataLen;
            bufOffset += binDataLen;
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s R a s t e r D a t a R e m                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Process remainder of Raster character data.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessRasterDataRem(ref int bufRem,
                                      ref int bufOffset)
    {
        PrnParseConstants.eContType contType;

        int binDataLen;

        if (_charRem > bufRem)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Remainder of sequence is not in buffer.                    //
            // Initiate (non back-tracking) continuation.                 //
            //                                                            //
            //------------------------------------------------------------//

            contType = PrnParseConstants.eContType.PCLXLFontChar;

            binDataLen = bufRem;
            _charRem -= bufRem;

            _linkData.SetContinuation(contType);
        }
        else
        {
            contType = PrnParseConstants.eContType.None;
            _linkData.ResetContData();

            binDataLen = _charRem;
            _charRem = 0;

            _nextStage = eStage.EndOK;
        }

        if ((binDataLen) != 0)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Some, or all, of the download data is contained within the //
            // current 'block'.                                           //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                PrnParseRowTypes.eType.DataBinary,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                _fileOffset + bufOffset,
                _analysisLevel,
                "PCLXL Binary",
                "[ " + binDataLen + " bytes ]",
                "Raster character data");

            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    binDataLen,
                    string.Empty,
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            //------------------------------------------------------------//
            //                                                            //
            // Adjust pointers.                                           //
            //                                                            //
            //------------------------------------------------------------//

            bufRem -= binDataLen;
            bufOffset += binDataLen;
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s T r u e T y p e D a t a B o d y                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Process TrueType character data.                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessTrueTypeDataBody(ref int bufRem,
                                          ref int bufOffset)
    {
        PrnParseConstants.eContType contType;

        int binDataLen;

        if (_charRem > bufRem)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Remainder of sequence is not in buffer.                    //
            // Initiate continuation.                                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = PrnParseConstants.eContType.PCLFontChar;

            binDataLen = bufRem;
            _charRem -= bufRem;

            _linkData.SetContinuation(contType);
        }
        else
        {
            contType = PrnParseConstants.eContType.None;
            _linkData.ResetContData();

            binDataLen = _charRem;
            _charRem = 0;
        }

        if ((binDataLen) != 0)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Some, or all, of the download data is contained within the //
            // current 'block'.                                           //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                PrnParseRowTypes.eType.DataBinary,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                _fileOffset + bufOffset,
                _analysisLevel,
                "PCLXL Binary",
                "[ " + binDataLen + " bytes ]",
                "TrueType glyph data");

            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    binDataLen,
                    string.Empty,
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            bufRem -= binDataLen;
            bufOffset += binDataLen;
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r a s t e r D r a w                                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Draw raster character.                                             //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void RasterDraw(int bufOffset,
                             int dataLen,
                             bool shapeTooLarge)
    {
        if (shapeTooLarge)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Shape exceeds size constraints.                            //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgComment,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                "Comment",
                "Shape",
                "***** Too large to display *****");

            if (_charHeight > _drawCharMaxHeight)
            {
                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.eType.MsgComment,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    string.Empty,
                    "Comment",
                    "Shape",
                    "Height (" + _charHeight +
                    ") > " + _drawCharMaxHeight +
                    " dots");
            }

            if (_charWidth > _drawCharMaxWidth)
            {
                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.eType.MsgComment,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    string.Empty,
                    "Comment",
                    "Shape",
                    "Width (" + _charWidth +
                    ") > " + _drawCharMaxWidth +
                    " dots");
            }

            if (_charDataLen > PrnParseConstants.bufSize)
            {
                PrnParseCommon.AddTextRow(
                    PrnParseRowTypes.eType.MsgComment,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    string.Empty,
                    "Comment",
                    string.Empty,
                    "Data   (" + _charDataLen +
                    ") > " + PrnParseConstants.bufSize +
                    " bytes");
            }
        }
        else if (_charClass == ePCLXLCharClass.Bitmap)
        {
            //----------------------------------------------------//
            //                                                    //
            // Display shape of Bitmap character.                 //
            // Note that we don't expect that this function will  //
            // have been invoked for any other character class!   //
            //                                                    //
            //----------------------------------------------------//

            bool firstLine;

            int bytesPerRow,
                  sliceLen,
                  crntOffset,
                  sub;

            string rowImage;

            bytesPerRow = (_charWidth / 8);

            if (_charWidth - (bytesPerRow * 8) != 0)
                bytesPerRow++;

            firstLine = true;
            crntOffset = bufOffset;

            for (int i = 0; i < dataLen; i += bytesPerRow)
            {
                //------------------------------------------------//
                //                                                //
                // Calculate slice length.                        //
                //                                                //
                //------------------------------------------------//

                if ((i + bytesPerRow) > dataLen)
                    sliceLen = dataLen - i;
                else
                    sliceLen = bytesPerRow;

                //------------------------------------------------//
                //                                                //
                // Extract required details from current slice.   //
                //                                                //
                //------------------------------------------------//

                rowImage = string.Empty;

                for (int j = crntOffset;
                     j < (crntOffset + sliceLen);
                     j++)
                {
                    sub = (_buf[j]);

                    for (int k = 0; k < 8; k++)
                    {
                        if ((sub & 0x80) != 0)
                            rowImage += "@";
                        else
                            rowImage += " ";

                        sub <<= 1;
                    }
                }

                //------------------------------------------------//
                //                                                //
                // Add row (line) to grid.                        //
                //                                                //
                //------------------------------------------------//

                if (firstLine)
                {
                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.eType.PCLXLFontChar,
                        _table,
                        PrnParseConstants.eOvlShow.None,
                        string.Empty,
                        string.Empty,
                        "Shape",
                        rowImage);
                }
                else
                {
                    PrnParseCommon.AddTextRow(
                        PrnParseRowTypes.eType.PCLXLFontChar,
                        _table,
                        PrnParseConstants.eOvlShow.None,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        rowImage);
                }

                firstLine = false;
                crntOffset += sliceLen;
            }
        }
    }
}