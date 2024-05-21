﻿using System;
using System.Data;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles PCL downloadable soft font characters.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    class PrnParseFontCharPCL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private enum Stage
        {
            Start = 0,
            CheckDesc,
            ShowDesc,
            ShowData,
            ShowDataHddr,
            ShowDataBody,
            ShowDataRem,
            ShowChecksum,
            EndOK,
            BadSeqA,
            BadSeqB
        }

        private enum PCLCharFormat
        {
            Raster = 4,
            Intellifont = 10,
            TrueType = 15
        }

        private enum PCLCharClass
        {
            Bitmap = 1,
            BitmapCompressed = 2,
            Intellifont = 3,
            IntellifontCompound = 4,
            TrueType = 15,
            Unknown
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

        private Stage _nextStage;
        private PCLCharFormat _charFormat;
        private PCLCharClass _charClass;

        private byte[] _buf;

        private byte _charDescLen;

        private int _fileOffset;
        private int _analysisLevel;

        private int _charLen;
        private int _charRem;
        private int _charPos;
        private int _charDataLen;
        private int _charDataRem;
        private int _charDataBlockRem;
        private int _charHeight;
        private int _charWidth;

        private int _charChksLen;
        //        private Int32 _charChksPos;
        private int _charChksVal;

        private int _charResvLen;
        //        private Int32 _charResvPos;

        private int _drawCharMaxHeight;
        private int _drawCharMaxWidth;

        private bool _showBinData;
        private bool _validChar;
        private bool _contChar;
        //      private Boolean _contCharExpected;

        private bool _drawCharShape;

        //      private Boolean _bitmapFont;
        //      private Boolean _intelliFont;
        //      private Boolean _truetypeFont;
        //      private Boolean _bitmapCompressed;

        private PrnParseConstants.OptOffsetFormats _indxOffsetFormat;

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
                                       int fileOffset,
                                       byte[] buf,
                                       ref int bufRem,
                                       ref int bufOffset,
                                       PrnParseLinkData linkData,
                                       PrnParseOptions options,
                                       DataTable table)
        {
            int binDataLen;

            PrnParseConstants.ContType contType;

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

            contType = _linkData.getContType();
            _analysisLevel = _linkData.AnalysisLevel;

            _indxOffsetFormat = _options.IndxGenOffsetFormat;
            _showBinData = _options.FlagPCLMiscBinData;

            _options.getOptPCLDraw(ref _drawCharShape,
                                    ref _drawCharMaxHeight,
                                    ref _drawCharMaxWidth);

            if (contType == PrnParseConstants.ContType.None)
            {
                _nextStage = Stage.Start;
                _validChar = true;

                _charLen = charLen;
                _charRem = charLen;
                _charPos = fileOffset + bufOffset;

                _charHeight = -1;
                _charWidth = -1;
            }
            else
            {
                contType = PrnParseConstants.ContType.None;
                _linkData.resetContData();
            }

            if (_nextStage == Stage.Start)
            {
                if (bufRem > _blockHddrLen)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Block header (Format Identifier & Continuation Marker) //
                    // and at least one more byte (Descriptor Size, unless    //
                    // continuation block) are in buffer.                     //
                    // Process block header and determine next stage (either  //
                    // CheckDesc, or ShowData (if Continuation block).        //
                    //                                                        //
                    //--------------------------------------------------------//

                    ProcessBlockHeader(ref bufRem,
                                        ref bufOffset);

                    if (_contChar)
                    {
                        _nextStage = Stage.ShowData;
                        _charDataBlockRem = _charDataRem;
                    }
                    else
                    {
                        _nextStage = Stage.CheckDesc;
                        _charChksVal = 0;
                    }
                }
                else
                {
                    contType = PrnParseConstants.ContType.PCLFontChar;

                    _linkData.setBacktrack(contType, -bufRem);
                }
            }

            if (_nextStage == Stage.CheckDesc)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Check if character data descriptor is in buffer.           //
                //                                                            //
                //------------------------------------------------------------//

                _charDescLen = buf[bufOffset];

                if ((_charDescLen + _blockHddrLen) > _charLen)
                {
                    _validChar = false;
                    _nextStage = Stage.BadSeqA;

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.MsgWarning,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "*** Warning ***",
                        string.Empty,
                        "Descriptor size (" + _charDescLen + " bytes)" +
                        " inconsistent with");

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.MsgWarning,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        "download size = " + _charLen +
                        " and block header = " + _blockHddrLen + " bytes");
                }
                else if (_charDescLen > bufRem)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Header/Descriptor is not entirely within buffer.       //
                    // Initiate (back-tracking) continuation.                 //
                    //                                                        //
                    //--------------------------------------------------------//

                    contType = PrnParseConstants.ContType.PCLFontChar;

                    _linkData.setBacktrack(contType, -bufRem);
                }
                else
                {
                    _nextStage = Stage.ShowDesc;
                }
            }

            if (_nextStage == Stage.ShowDesc)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Process character descriptor.                              //
                //                                                            //
                //------------------------------------------------------------//

                ProcessDescriptor(ref bufRem,
                                  ref bufOffset);

                bufRem -= _charDescLen;
                _charRem -= _charDescLen;
                bufOffset += _charDescLen;

                if (_validChar)
                    _nextStage = Stage.ShowData;
                else
                    _nextStage = Stage.BadSeqA;
            }

            if (_nextStage == Stage.ShowData)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Process character data.                                    //
                //                                                            //
                //------------------------------------------------------------//

                if (_charFormat == PCLCharFormat.Raster)
                {
                    _charDataLen = _charRem;
                    _charDataRem = _charDataLen;

                    if (_contChar)
                        _nextStage = Stage.ShowDataRem;
                    else
                        _nextStage = Stage.ShowDataBody;
                }
                else
                {
                    if (_contChar)
                        _nextStage = Stage.ShowDataBody;
                    else
                        _nextStage = Stage.ShowDataHddr;
                }
            }

            if ((_nextStage == Stage.ShowDataHddr) && (_charRem != 0))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Output details of (scalable font) character data header.   //
                //                                                            //
                //------------------------------------------------------------//

                if (_charFormat == PCLCharFormat.Intellifont)
                {
                    ProcessIntellifontDataHddr(ref bufRem,
                                                ref bufOffset);
                }
                else if (_charFormat == PCLCharFormat.TrueType)
                {
                    ProcessTrueTypeDataHddr(ref bufRem,
                                             ref bufOffset);
                }
                else
                {
                    _nextStage = Stage.BadSeqA;
                }
            }

            if ((_nextStage == Stage.ShowDataBody) && (_charRem != 0))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Output details of character data.                          //
                //                                                            //
                //------------------------------------------------------------//

                if (_charFormat == PCLCharFormat.Raster)
                {
                    ProcessRasterDataBody(ref bufRem,
                                           ref bufOffset);
                }
                else if (_charFormat == PCLCharFormat.Intellifont)
                {
                    ProcessIntellifontDataBody(ref bufRem,
                                                ref bufOffset);
                }
                else if (_charFormat == PCLCharFormat.TrueType)
                {
                    ProcessTrueTypeDataBody(ref bufRem,
                                             ref bufOffset);
                }
                else
                {
                    _nextStage = Stage.BadSeqA;
                }
            }

            if ((_nextStage == Stage.ShowDataRem) && (_charRem != 0))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Output details of remainder of (Raster) character data.    //
                //                                                            //
                //------------------------------------------------------------//

                if (_charFormat == PCLCharFormat.Raster)
                {
                    ProcessRasterDataRem(ref bufRem,
                                          ref bufOffset);
                }
                else
                {
                    _nextStage = Stage.BadSeqA;
                }
            }

            if ((_nextStage == Stage.ShowChecksum) && (_charRem != 0))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Output details of Reserved byte and Checksum fields.       //
                // Not present for Raster (bitmap) format.                    //
                //                                                            //
                //------------------------------------------------------------//

                ProcessChecksum(ref bufRem,
                                ref bufOffset);
            }

            if (_nextStage == Stage.EndOK)
            {
                //------------------------------------------------------------//
                //                                                            //
                // End of processing of valid header.                         //
                //                                                            //
                //------------------------------------------------------------//

                return _validChar;
            }

            if (_nextStage == Stage.BadSeqA)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Inconsistency found.                                       //
                //                                                            //
                //------------------------------------------------------------//

                _nextStage = Stage.BadSeqB;

                PrnParseCommon.addTextRow(
                    PrnParseRowTypes.Type.MsgError,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    "Processing of character abandoned");
            }

            if ((_nextStage == Stage.BadSeqB) && (_charRem != 0))
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

                    contType = PrnParseConstants.ContType.PCLFontChar;

                    binDataLen = bufRem;
                    _charRem -= bufRem;

                    _linkData.setContinuation(contType);
                }
                else
                {
                    contType = PrnParseConstants.ContType.None;
                    _linkData.resetContData();

                    binDataLen = _charRem;
                    _charRem = 0;
                }

                if (binDataLen != 0)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Some, or all, of the download data is contained within //
                    // the current 'block'.                                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    PrnParseCommon.addDataRow(
                        PrnParseRowTypes.Type.DataBinary,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        _indxOffsetFormat,
                        fileOffset + bufOffset,
                        _analysisLevel,
                        "PCL Binary",
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
        //         1   Continuation flag                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ProcessBlockHeader(ref int bufRem,
                                         ref int bufOffset)
        {
            string itemDesc;

            PrnParseCommon.addDataRow(
                PrnParseRowTypes.Type.DataBinary,
                _table,
                PrnParseConstants.OvlShow.None,
                _indxOffsetFormat,
                _fileOffset + bufOffset,
                _analysisLevel,
                "PCL Binary",
                "[ " + _blockHddrLen + " bytes ]",
                "Character data block header");

            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.OvlShow.None,
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

            //--------------------------------------------------------//
            //                                                        //
            // Format (byte 0):                                       //
            //                                                        //
            //--------------------------------------------------------//

            _charFormat = (PCLCharFormat)_buf[bufOffset];

            switch (_charFormat)
            {
                case PCLCharFormat.Raster:
                    itemDesc = "4: Raster";
                    break;

                case PCLCharFormat.Intellifont:
                    itemDesc = "10: Intellifont Scalable";
                    break;

                case PCLCharFormat.TrueType:
                    itemDesc = "15: Truetype Scalable";
                    break;

                default:
                    itemDesc = _charFormat.ToString() + ": Unknown";
                    break;
            }

            PrnParseCommon.addTextRow(
                PrnParseRowTypes.Type.PCLFontChar,
                _table,
                PrnParseConstants.OvlShow.None,
                string.Empty,
                "Header Format:",
                string.Empty,
                itemDesc);

            //--------------------------------------------------------//
            //                                                        //
            // Continuation flag (byte 1).                            //
            //                                                        //
            //--------------------------------------------------------//

            if (_buf[bufOffset + 1] == 0)
            {
                _contChar = false;
                _charClass = PCLCharClass.Unknown; // Set in desc.
            }
            else
            {
                _contChar = true;

                PrnParseCommon.addTextRow(
                    PrnParseRowTypes.Type.PCLFontChar,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    "Continuation block");
            }

            //--------------------------------------------------------//
            //                                                        //
            // Adjust pointers.                                       //
            //                                                        //
            //--------------------------------------------------------//

            bufRem -= _blockHddrLen;
            _charRem -= _blockHddrLen;
            bufOffset += _blockHddrLen;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s C h e c k s u m                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Output details of Reserved byte and Checksum byte.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ProcessChecksum(ref int bufRem,
                                      ref int bufOffset)
        {
            PrnParseConstants.ContType contType;

            _charChksLen = 1; // TEMP
            _charResvLen = 1; // TEMP

            if (_charChksLen == 0)
            {
                if (_charRem != 0)
                {
                    _validChar = false;
                    _nextStage = Stage.BadSeqA;

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.MsgWarning,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "*** Warning ***",
                        string.Empty,
                        "Header is  internally inconsistent");
                }
            }
            else
            {
                if (_charRem != (_charResvLen + _charChksLen))
                {
                    _validChar = false;
                    _nextStage = Stage.BadSeqA;

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.MsgWarning,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "*** Warning ***",
                        string.Empty,
                        "Either Character Data Size is incorrect or");

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.MsgWarning,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        "Reserved byte and/or Checksum byte are missing");
                }
                else
                {
                    if (_charRem > bufRem)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Remainder of sequence is not in buffer.            //
                        // Initiate (back-tracking) continuation.             //
                        //                                                    //
                        //----------------------------------------------------//

                        contType = PrnParseConstants.ContType.PCLFontChar;

                        _linkData.setBacktrack(contType, -bufRem);
                    }
                    else
                    {
                        byte crntByte;

                        contType = PrnParseConstants.ContType.None;
                        _linkData.resetContData();

                        //----------------------------------------------------//
                        //                                                    //
                        // Display Reserved byte (should always be            //
                        // (binary) zero).                                    //
                        //                                                    //
                        //----------------------------------------------------//

                        crntByte = _buf[bufOffset];

                        PrnParseCommon.addDataRow(
                            PrnParseRowTypes.Type.PCLFontChar,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            _indxOffsetFormat,
                            _fileOffset + bufOffset,
                            _analysisLevel,
                            "Reserved byte",
                            "[ 1 byte ]",
                            "0x" +
                            PrnParseCommon.byteToHexString(crntByte));

                        _charChksVal += crntByte;

                        //----------------------------------------------------//
                        //                                                    //
                        // Display Checksum byte.                             //
                        // Verify that it matches the calculated value.       //
                        //                                                    //
                        //----------------------------------------------------//

                        crntByte = _buf[bufOffset + 1];

                        PrnParseCommon.addDataRow(
                            PrnParseRowTypes.Type.PCLFontChar,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            _indxOffsetFormat,
                            _fileOffset + bufOffset + 1,
                            _analysisLevel,
                            "Checksum",
                            "[ 1 byte ]",
                            "0x" +
                            PrnParseCommon.byteToHexString(crntByte));

                        _charChksVal = (256 - (_charChksVal % 256)) % 256;

                        if (_charChksVal != crntByte)
                        {
                            crntByte = (byte)_charChksVal;

                            PrnParseCommon.addTextRow(
                                PrnParseRowTypes.Type.MsgWarning,
                                _table,
                                PrnParseConstants.OvlShow.None,
                                string.Empty,
                                "*** Warning ***",
                                string.Empty,
                                "Calculated checksum is 0x" +
                                PrnParseCommon.byteToHexString(crntByte));

                            _validChar = false;
                        }

                        //----------------------------------------------------//
                        //                                                    //
                        // Adjust pointers.                                   //
                        //                                                    //
                        //----------------------------------------------------//

                        bufRem -= _charRem;
                        bufOffset += _charRem;

                        _charRem = 0;
                        _nextStage = Stage.EndOK;
                    }
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s D e s c r i p t o r                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Provide an interpretation of the contents of the Character         //
        // Descriptor part of the header.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ProcessDescriptor(ref int bufRem,
                                       ref int bufOffset)
        {
            uint ui32a;
            ushort ui16a;

            short si16a;

            string itemDesc;

            //----------------------------------------------------------------//
            //                                                                //
            // Show size and (optionally) data.                               //
            //                                                                //
            //----------------------------------------------------------------//

            PrnParseCommon.addDataRow(
                PrnParseRowTypes.Type.DataBinary,
                _table,
                PrnParseConstants.OvlShow.None,
                _indxOffsetFormat,
                _fileOffset + bufOffset,
                _analysisLevel,
                "PCL Binary",
                "[ " + _charDescLen + " bytes ]",
                "Character descriptor");

            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.OvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    _charDescLen,
                    string.Empty,
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            // _bitmapFont       = false;
            // _intelliFont      = false;
            // _truetypeFont     = false;

            //----------------------------------------------------------------//
            //                                                                //
            // Character Descriptor size.                                     //
            //                                                                //
            //----------------------------------------------------------------//

            PrnParseCommon.addTextRow(
                PrnParseRowTypes.Type.PCLFontChar,
                _table,
                PrnParseConstants.OvlShow.None,
                string.Empty,
                "Descriptor Size:",
                string.Empty,
                _charDescLen.ToString());

            //----------------------------------------------------------------//
            //                                                                //
            // Class (byte 1).                                                //
            //                                                                //
            //----------------------------------------------------------------//

            _charClass = (PCLCharClass)_buf[bufOffset + 1];

            switch (_charClass)
            {
                case PCLCharClass.Bitmap:
                    itemDesc = "1: Bitmap";
                    //_bitmapFont = true;
                    break;

                case PCLCharClass.BitmapCompressed:
                    itemDesc = "2: Compressed Bitmap";
                    //_bitmapFont       = true;
                    //_bitmapCompressed = true;
                    break;

                case PCLCharClass.Intellifont:
                    itemDesc = "3: Intellifont Scalable: Contour";
                    //_intelliFont = true;
                    break;

                case PCLCharClass.IntellifontCompound:
                    itemDesc = "4: Intellifont Scalable: Compound Contour";
                    //_intelliFont = true;
                    break;

                case PCLCharClass.TrueType:
                    itemDesc = "15: Truetype Scalable";
                    //_truetypeFont = true;
                    break;

                default:
                    itemDesc = _charClass + ": Unknown";
                    break;
            }

            PrnParseCommon.addTextRow(
                PrnParseRowTypes.Type.PCLFontChar,
                _table,
                PrnParseConstants.OvlShow.None,
                string.Empty,
                "Class:",
                string.Empty,
                itemDesc);

            if (_charFormat == PCLCharFormat.Raster)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Format 4 - Raster (bitmap).                                //
                //                                                            //
                //------------------------------------------------------------//

                ushort bytesPerRow;

                //------------------------------------------------------------//
                //                                                            //
                // Orientation (byte 2).                                      //
                //                                                            //
                //    0 = Portrait                                            //
                //    1 = Landscape                                           //
                //    2 = Reverse Portrait                                    //
                //    3 = Reverse Landscape                                   //
                //                                                            //
                //------------------------------------------------------------//

                ui16a = _buf[bufOffset + 2];

                switch (ui16a)
                {
                    case 0:
                        itemDesc = "0: Portrait";
                        break;

                    case 1:
                        itemDesc = "1: Landscape";
                        break;

                    case 2:
                        itemDesc = "2: Reverse Portrait";
                        break;

                    case 3:
                        itemDesc = "3: Reverse Landscape";
                        break;

                    default:
                        itemDesc = ui16a + ": Unknown";
                        break;
                }

                PrnParseCommon.addTextRow(
                    PrnParseRowTypes.Type.PCLFontChar,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Orientation:",
                    string.Empty,
                    itemDesc);

                //------------------------------------------------------------//
                //                                                            //
                // Left offset (bytes 4 & 5).                                 //
                //                                                            //
                // Distance from the 'reference point' to the left side of    //
                // the character.                                             //
                //                                                            //
                //------------------------------------------------------------//

                si16a = (short)((_buf[bufOffset + 4] * 256) +
                                  _buf[bufOffset + 5]);

                PrnParseCommon.addTextRow(
                    PrnParseRowTypes.Type.PCLFontChar,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Left Offset:",
                    string.Empty,
                    si16a + " dots");

                //------------------------------------------------------------//
                //                                                            //
                // Top offset (bytes 6 & 7).                                  //
                //                                                            //
                // Distance from the 'reference point' to the top of the      //
                // character.                                                 //
                //                                                            //
                //------------------------------------------------------------//

                si16a = (short)((_buf[bufOffset + 6] * 256) +
                                  _buf[bufOffset + 7]);

                PrnParseCommon.addTextRow(
                    PrnParseRowTypes.Type.PCLFontChar,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Top Offset:",
                    string.Empty,
                    si16a + " dots");

                //------------------------------------------------------------//
                //                                                            //
                // Character Width (bytes 8 & 9).                             //
                //                                                            //
                //------------------------------------------------------------//

                _charWidth = (ushort)((_buf[bufOffset + 8] * 256) +
                                        _buf[bufOffset + 9]);

                bytesPerRow = (ushort)(_charWidth / 8);

                if ((_charWidth % 8) != 0)
                    bytesPerRow++;

                PrnParseCommon.addTextRow(
                    PrnParseRowTypes.Type.PCLFontChar,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Character Width:",
                    string.Empty,
                    _charWidth + " dots (requires " +
                    bytesPerRow + " padded bytes per row)");

                //------------------------------------------------------------//
                //                                                            //
                // Character Height (bytes 10 & 11).                          //
                //                                                            //
                //------------------------------------------------------------//

                _charHeight = (ushort)((_buf[bufOffset + 10] * 256) +
                                         _buf[bufOffset + 11]);

                PrnParseCommon.addTextRow(
                    PrnParseRowTypes.Type.PCLFontChar,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Character Height:",
                    string.Empty,
                    _charHeight + " dots");

                //------------------------------------------------------------//
                //                                                            //
                // Delta-X (bytes 12 & 13).                                   //
                //                                                            //
                // Number of radix (quarter) dots to advance cursor after     //
                // printing character; only relevant to proportionally-spaced //
                // fonts.                                                     //
                //                                                            //
                //------------------------------------------------------------//

                si16a = (short)((_buf[bufOffset + 12] * 256) +
                                  _buf[bufOffset + 13]);

                PrnParseCommon.addTextRow(
                    PrnParseRowTypes.Type.PCLFontChar,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Delta-X:",
                    string.Empty,
                    si16a + " quarter-dots");

                //------------------------------------------------------------//
                //                                                            //
                // Estimated character data size:                             //
                //    (bytesPerRow * charHeight) bytes.                       //
                //                                                            //
                //------------------------------------------------------------//

                if (_charClass == PCLCharClass.Bitmap)
                {
                    ui32a = (uint)(bytesPerRow * _charHeight);

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.PCLFontChar,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Raster data size:",
                        string.Empty,
                        ui32a + " bytes (assuming " +
                        _charHeight + " rows of " +
                        bytesPerRow + " bytes)");

                    if (ui32a != (_charLen - _blockHddrLen - _charDescLen))
                    {
                        _validChar = false;

                        _nextStage = Stage.BadSeqA;

                        PrnParseCommon.addTextRow(
                            PrnParseRowTypes.Type.MsgWarning,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            string.Empty,
                            "*** Warning ***",
                            string.Empty,
                            "Estimated data size (" + ui32a + " bytes)" +
                            " inconsistent with");

                        PrnParseCommon.addTextRow(
                            PrnParseRowTypes.Type.MsgWarning,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            "download size = " + _charLen +
                            ", block header = " + _blockHddrLen + " and ");

                        PrnParseCommon.addTextRow(
                            PrnParseRowTypes.Type.MsgWarning,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            "descriptor = " + _charDescLen + " bytes");
                    }
                }
            }
            else if (_charFormat == PCLCharFormat.Intellifont)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Expected Descriptor size is only two bytes (the Format &   //
                // Class bytes).                                              //
                // Report any extra bytes as unexpected.                      //
                //                                                            //
                //------------------------------------------------------------//

                int charDescExtra = _charDescLen - 2;

                if (charDescExtra > 0)
                {
                    _validChar = false;
                    _nextStage = Stage.BadSeqA;

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.MsgWarning,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "***Warning***",
                        string.Empty,
                        "Descriptor size (" + _charDescLen + " bytes)" +
                        " larger than expected (2 bytes)");

                    PrnParseData.ProcessBinary(
                        _table,
                        PrnParseConstants.OvlShow.None,
                        _buf,
                        _fileOffset,
                        bufOffset + 2,
                        charDescExtra,
                        "Additional data:",
                        _showBinData,
                        false,
                        true,
                        _indxOffsetFormat,
                        _analysisLevel);
                }
            }
            else if (_charFormat == PCLCharFormat.TrueType)
            {
                int charDescExtra = _charDescLen - 2;

                if (charDescExtra > 0)
                {
                    PrnParseData.ProcessBinary(
                        _table,
                        PrnParseConstants.OvlShow.None,
                        _buf,
                        _fileOffset,
                        bufOffset + 2,
                        charDescExtra,
                        "Additional data:",
                        _showBinData,
                        false,
                        true,
                        _indxOffsetFormat,
                        _analysisLevel);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s I n t e l l i f o n t D a t a B o d y                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process Intellifont character data.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ProcessIntellifontDataBody(ref int bufRem,
                                                 ref int bufOffset)
        {
            PrnParseConstants.ContType contType;

            int binDataLen;

            if (_charDataBlockRem > bufRem)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Remainder of sequence is not in buffer.                    //
                // Initiate continuation.                                     //
                //                                                            //
                //------------------------------------------------------------//

                contType = PrnParseConstants.ContType.PCLFontChar;

                binDataLen = bufRem;
                _charRem -= bufRem;
                _charDataRem -= bufRem;
                _charDataBlockRem -= bufRem;

                _linkData.setContinuation(contType);
            }
            else
            {
                contType = PrnParseConstants.ContType.None;

                _linkData.resetContData();

                binDataLen = _charDataBlockRem;
                _charRem -= _charDataBlockRem;
                _charDataRem -= _charDataBlockRem;
                _charDataBlockRem = 0;

                _nextStage = Stage.ShowChecksum;
            }

            if (binDataLen != 0)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Some, or all, of the download data is contained within the //
                // current 'block'.                                           //
                //                                                            //
                //------------------------------------------------------------//

                PrnParseCommon.addDataRow(
                    PrnParseRowTypes.Type.DataBinary,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    _indxOffsetFormat,
                    _fileOffset + bufOffset,
                    _analysisLevel,
                    "PCL Binary",
                    "[ " + binDataLen + " bytes ]",
                    "Intellifont character data");

                if (_showBinData)
                {
                    PrnParseData.ProcessBinary(
                        _table,
                        PrnParseConstants.OvlShow.None,
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

                for (int i = 0; i < binDataLen; i++)
                {
                    _charChksVal += _buf[bufOffset + i];
                }

                bufRem -= binDataLen;
                bufOffset += binDataLen;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s I n t e l l i f o n t D a t a H d d r                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process Intellifont character data header.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ProcessIntellifontDataHddr(ref int bufRem,
                                                ref int bufOffset)
        {
            PrnParseConstants.ContType contType;

            int hddrLen;

            ushort contourDataSize;

            short metricDataOffset,
                  charDataOffset,
                  contourTreeOffset,
                  xyDataOffset,
                  compEscapement;

            byte compCount;

            if (_charClass == PCLCharClass.Intellifont)
                hddrLen = 10;
            else
                hddrLen = 4;

            if (bufRem < hddrLen)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Header is not in buffer.                                   //
                // Initiate (back-tracking) continuation.                     //
                //                                                            //
                //------------------------------------------------------------//

                contType = PrnParseConstants.ContType.PCLFontChar;

                _linkData.setBacktrack(contType, -bufRem);
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Show size and (optionally) data.                           //
                //                                                            //
                //------------------------------------------------------------//

                PrnParseCommon.addDataRow(
                    PrnParseRowTypes.Type.DataBinary,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    _indxOffsetFormat,
                    _fileOffset + bufOffset,
                    _analysisLevel,
                    "PCL Binary",
                    "[ " + hddrLen + " bytes ]",
                    "Intellifont character data header");

                if (_showBinData)
                {
                    PrnParseData.ProcessBinary(
                        _table,
                        PrnParseConstants.OvlShow.None,
                        _buf,
                        _fileOffset,
                        bufOffset,
                        hddrLen,
                        string.Empty,
                        true,
                        false,
                        true,
                        _indxOffsetFormat,
                        _analysisLevel);
                }

                for (int i = 0; i < hddrLen; i++)
                {
                    _charChksVal += _buf[bufOffset + i];
                }

                if (_charClass == PCLCharClass.Intellifont)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Contour Data Size.                                     //
                    //                                                        //
                    //--------------------------------------------------------//

                    contourDataSize = (ushort)((_buf[bufOffset] * 256) +
                                                 _buf[bufOffset + 1]);
                    _charDataLen = (ushort)(contourDataSize - hddrLen);

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.PCLFontChar,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Contour Data Size:",
                        string.Empty,
                        contourDataSize + " bytes" +
                        "(header = " + hddrLen +
                        "; glyph data = " + _charDataLen + ")");

                    //--------------------------------------------------------//
                    //                                                        //
                    // Metric Data Offset.                                    //
                    //                                                        //
                    //--------------------------------------------------------//

                    metricDataOffset = (short)((_buf[bufOffset + 2] * 256) +
                                                 _buf[bufOffset + 3]);

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.PCLFontChar,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Metric Data Offset:",
                        string.Empty,
                        metricDataOffset + " bytes");

                    //--------------------------------------------------------//
                    //                                                        //
                    // Character Intellifont Data Offset.                     //
                    //                                                        //
                    //--------------------------------------------------------//

                    charDataOffset = (short)((_buf[bufOffset + 4] * 256) +
                                               _buf[bufOffset + 5]);

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.PCLFontChar,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Character Data Offset:",
                        string.Empty,
                        charDataOffset + " bytes");

                    //--------------------------------------------------------//
                    //                                                        //
                    // Contour Tree Offset.                                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    contourTreeOffset = (short)((_buf[bufOffset + 6] * 256) +
                                                  _buf[bufOffset + 7]);

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.PCLFontChar,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Contour Tree Offset:",
                        string.Empty,
                        contourTreeOffset + " bytes");

                    //--------------------------------------------------------//
                    //                                                        //
                    // XY Data Offset.                                        //
                    //                                                        //
                    //--------------------------------------------------------//

                    xyDataOffset = (short)((_buf[bufOffset + 8] * 256) +
                                             _buf[bufOffset + 9]);

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.PCLFontChar,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "XY Data Offset:",
                        string.Empty,
                        xyDataOffset + " bytes");
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Compound Character Escapement.                         //
                    //                                                        //
                    //--------------------------------------------------------//

                    compEscapement = (short)((_buf[bufOffset] * 256) +
                                               _buf[bufOffset + 1]);

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.PCLFontChar,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Compound Escapement:",
                        string.Empty,
                        compEscapement + " design units");

                    //--------------------------------------------------------//
                    //                                                        //
                    // Number of Components.                                  //
                    //                                                        //
                    //--------------------------------------------------------//

                    compCount = _buf[bufOffset + 2];

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.PCLFontChar,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Number of Components:",
                        string.Empty,
                        compCount.ToString());
                }

                //------------------------------------------------------------//
                //                                                            //
                // Adjust pointers to reference start of variable data.       //
                //                                                            //
                //------------------------------------------------------------//

                contType = PrnParseConstants.ContType.None;
                _linkData.resetContData();

                _charRem -= hddrLen;
                bufRem -= hddrLen;
                bufOffset += hddrLen;
                _charDataRem = _charDataLen;
                _charDataBlockRem = _charDataLen;

                if ((_charDataLen + 2) > _charRem)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Character data must be spread over two or more blocks. //
                    //                                                        //
                    //--------------------------------------------------------//

                    _charDataBlockRem = _charRem;

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.MsgComment,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Comment",
                        string.Empty,
                        "Continuation block expected");
                }

                _nextStage = Stage.ShowDataBody;
            }
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
            PrnParseConstants.ContType contType;

            int binDataLen;

            bool shapeTooLarge = false;

            if (_drawCharShape)
            {
                shapeTooLarge = (_charHeight > _drawCharMaxHeight)
                                ||
                    (_charWidth > _drawCharMaxWidth)
                                ||
                    (_charDataLen > PrnParseConstants.bufSize);
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

                if (_drawCharShape && (!shapeTooLarge))
                {
                    contType = PrnParseConstants.ContType.PCLFontChar;

                    _linkData.setBacktrack(contType, -bufRem);

                    bufOffset += bufRem;
                    bufRem = 0;
                    binDataLen = 0;
                }
                else
                {
                    _nextStage = Stage.ShowDataRem;

                    contType = PrnParseConstants.ContType.PCLFontChar;

                    binDataLen = bufRem;
                    _charRem -= bufRem;

                    _linkData.setContinuation(contType);
                }
            }
            else
            {
                contType = PrnParseConstants.ContType.None;
                _linkData.resetContData();

                binDataLen = _charRem;
                _charRem = 0;

                _nextStage = Stage.EndOK;
            }

            if (binDataLen != 0)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Some, or all, of the download data is contained within the //
                // current 'block'.                                           //
                //                                                            //
                //------------------------------------------------------------//

                PrnParseCommon.addDataRow(
                    PrnParseRowTypes.Type.DataBinary,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    _indxOffsetFormat,
                    _fileOffset + bufOffset,
                    _analysisLevel,
                    "PCL Binary",
                    "[ " + binDataLen + " bytes ]",
                    "Raster character data");

                if (_showBinData)
                {
                    PrnParseData.ProcessBinary(
                        _table,
                        PrnParseConstants.OvlShow.None,
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
            PrnParseConstants.ContType contType;

            int binDataLen;

            if (_charRem > bufRem)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Remainder of sequence is not in buffer.                    //
                // Initiate (non back-tracking) continuation.                 //
                //                                                            //
                //------------------------------------------------------------//

                contType = PrnParseConstants.ContType.PCLFontChar;

                binDataLen = bufRem;
                _charRem -= bufRem;

                _linkData.setContinuation(contType);
            }
            else
            {
                contType = PrnParseConstants.ContType.None;
                _linkData.resetContData();

                binDataLen = _charRem;
                _charRem = 0;

                _nextStage = Stage.EndOK;
            }

            if (binDataLen != 0)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Some, or all, of the download data is contained within the //
                // current 'block'.                                           //
                //                                                            //
                //------------------------------------------------------------//

                PrnParseCommon.addDataRow(
                    PrnParseRowTypes.Type.DataBinary,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    _indxOffsetFormat,
                    _fileOffset + bufOffset,
                    _analysisLevel,
                    "PCL Binary",
                    "[ " + binDataLen + " bytes ]",
                    "Raster character data");

                if (_showBinData)
                {
                    PrnParseData.ProcessBinary(
                        _table,
                        PrnParseConstants.OvlShow.None,
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
        // Process TrueType glyph data.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ProcessTrueTypeDataBody(ref int bufRem,
                                              ref int bufOffset)
        {
            PrnParseConstants.ContType contType;

            int binDataLen;

            if (_charDataBlockRem > bufRem)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Remainder of sequence is not in buffer.                    //
                // Initiate continuation.                                     //
                //                                                            //
                //------------------------------------------------------------//

                contType = PrnParseConstants.ContType.PCLFontChar;

                binDataLen = bufRem;
                _charRem -= bufRem;
                _charDataRem -= bufRem;
                _charDataBlockRem -= bufRem;

                _linkData.setContinuation(contType);
            }
            else
            {
                contType = PrnParseConstants.ContType.None;
                _linkData.resetContData();

                binDataLen = _charDataBlockRem;
                _charRem -= _charDataBlockRem;
                _charDataRem -= _charDataBlockRem;
                _charDataBlockRem = 0;

                _nextStage = Stage.ShowChecksum;
            }

            if (binDataLen != 0)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Some, or all, of the download data is contained within the //
                // current 'block'.                                           //
                //                                                            //
                //------------------------------------------------------------//

                PrnParseCommon.addDataRow(
                    PrnParseRowTypes.Type.DataBinary,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    _indxOffsetFormat,
                    _fileOffset + bufOffset,
                    _analysisLevel,
                    "PCL Binary",
                    "[ " + binDataLen + " bytes ]",
                    "TrueType glyph data");

                if (_showBinData)
                {
                    PrnParseData.ProcessBinary(
                        _table,
                        PrnParseConstants.OvlShow.None,
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

                for (int i = 0; i < binDataLen; i++)
                {
                    _charChksVal += _buf[bufOffset + i];
                }

                bufRem -= binDataLen;
                bufOffset += binDataLen;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r o c e s s T r u e T y p e D a t a H d d r                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process TrueType character data header.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ProcessTrueTypeDataHddr(ref int bufRem,
                                             ref int bufOffset)
        {
            PrnParseConstants.ContType contType;

            int hddrLen;

            ushort charDataSize;

            short glyphID;

            hddrLen = 4;

            if (bufRem < hddrLen)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Header is not in buffer.                                   //
                // Initiate (back-tracking) continuation.                     //
                //                                                            //
                //------------------------------------------------------------//

                contType = PrnParseConstants.ContType.PCLFontChar;

                _linkData.setBacktrack(contType, -bufRem);
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Show size and (optionally) data.                           //
                //                                                            //
                //------------------------------------------------------------//

                PrnParseCommon.addDataRow(
                    PrnParseRowTypes.Type.DataBinary,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    _indxOffsetFormat,
                    _fileOffset + bufOffset,
                    _analysisLevel,
                    "PCL Binary",
                    "[ " + hddrLen + " bytes ]",
                    "TrueType character data header");

                if (_showBinData)
                {
                    PrnParseData.ProcessBinary(
                        _table,
                        PrnParseConstants.OvlShow.None,
                        _buf,
                        _fileOffset,
                        bufOffset,
                        hddrLen,
                        string.Empty,
                        true,
                        false,
                        true,
                        _indxOffsetFormat,
                        _analysisLevel);
                }

                for (int i = 0; i < hddrLen; i++)
                {
                    _charChksVal += _buf[bufOffset + i];
                }

                //------------------------------------------------------------//
                //                                                            //
                // Character Data Size.                                       //
                //                                                            //
                //------------------------------------------------------------//

                charDataSize = (ushort)((_buf[bufOffset] * 256) +
                                          _buf[bufOffset + 1]);
                _charDataLen = (ushort)(charDataSize - hddrLen);

                PrnParseCommon.addTextRow(
                    PrnParseRowTypes.Type.PCLFontChar,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Character Data Size:",
                    string.Empty,
                    charDataSize + " bytes" +
                    " (header = " + hddrLen +
                    "; glyph data = " + _charDataLen + ")");

                //------------------------------------------------------------//
                //                                                            //
                // Glyph ID.                                                  //
                //                                                            //
                //------------------------------------------------------------//

                glyphID = (short)((_buf[bufOffset + 2] * 256) +
                                    _buf[bufOffset + 3]);

                PrnParseCommon.addTextRow(
                    PrnParseRowTypes.Type.PCLFontChar,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Glyph ID:",
                    string.Empty,
                    glyphID.ToString());

                //------------------------------------------------------------//
                //                                                            //
                // Adjust pointers to reference start of variable data.       //
                //                                                            //
                //------------------------------------------------------------//

                contType = PrnParseConstants.ContType.None;
                _linkData.resetContData();

                _charRem -= hddrLen;
                bufRem -= hddrLen;
                bufOffset += hddrLen;

                _charDataRem = _charDataLen;
                _charDataBlockRem = _charDataLen;

                if ((_charDataLen + 2) > _charRem)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Character data must be spread over two or more blocks. //
                    //                                                        //
                    //--------------------------------------------------------//

                    _charDataBlockRem = _charRem;

                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.MsgComment,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Comment",
                        string.Empty,
                        "Continuation block expected");
                }

                _nextStage = Stage.ShowDataBody;
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

                PrnParseCommon.addTextRow(
                    PrnParseRowTypes.Type.MsgComment,
                    _table,
                    PrnParseConstants.OvlShow.None,
                    string.Empty,
                    "Comment",
                    "Shape",
                    "***** Too large to display *****");

                if (_charHeight > _drawCharMaxHeight)
                {
                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.MsgComment,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Comment",
                        "Shape",
                        "Height (" + _charHeight +
                        ") > " + _drawCharMaxHeight +
                        " dots");
                }

                if (_charWidth > _drawCharMaxWidth)
                {
                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.MsgComment,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Comment",
                        "Shape",
                        "Width (" + _charWidth +
                        ") > " + _drawCharMaxWidth +
                        " dots");
                }

                if (_charDataLen > PrnParseConstants.bufSize)
                {
                    PrnParseCommon.addTextRow(
                        PrnParseRowTypes.Type.MsgComment,
                        _table,
                        PrnParseConstants.OvlShow.None,
                        string.Empty,
                        "Comment",
                        string.Empty,
                        "Data   (" + _charDataLen +
                        ") > " + PrnParseConstants.bufSize +
                        " bytes");
                }
            }
            else if (_charClass == PCLCharClass.Bitmap)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Display shape of bitmap character.                         //
                //                                                            //
                //------------------------------------------------------------//

                bool firstLine;

                int bytesPerRow,
                      sliceLen,
                      crntOffset,
                      sub;

                string rowImage;

                bytesPerRow = _charWidth / 8;

                if (_charWidth - (bytesPerRow * 8) != 0)
                    bytesPerRow++;

                firstLine = true;
                crntOffset = bufOffset;

                for (int i = 0; i < dataLen; i += bytesPerRow)
                {
                    if ((i + bytesPerRow) > dataLen)
                    {
                        //--------------------------------------------//
                        //                                            //
                        // Last slice of data is less than full.      //
                        //                                            //
                        //--------------------------------------------//

                        sliceLen = dataLen - i;
                    }
                    else
                    {
                        sliceLen = bytesPerRow;
                    }

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
                        sub = _buf[j];

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
                        PrnParseCommon.addTextRow(
                            PrnParseRowTypes.Type.PCLFontChar,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            string.Empty,
                            string.Empty,
                            "Shape",
                            rowImage);
                    }
                    else
                    {
                        PrnParseCommon.addTextRow(
                            PrnParseRowTypes.Type.PCLFontChar,
                            _table,
                            PrnParseConstants.OvlShow.None,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            rowImage);
                    }

                    firstLine = false;
                    crntOffset += sliceLen;
                }
            }
            else if (_charClass == PCLCharClass.BitmapCompressed)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Display shape of compressed bitmap character.              //
                //                                                            //
                //------------------------------------------------------------//

                bool firstLine,
                        blackDot;

                int pos,
                      crntOffset,
                      colCt,
                      dotCt,
                      rptCt;

                string rowImage;

                firstLine = true;
                crntOffset = bufOffset;
                pos = 0;

                while (pos < dataLen)
                {
                    colCt = 0;
                    rowImage = string.Empty;

                    rptCt = _buf[crntOffset + pos];
                    pos++;

                    blackDot = false;

                    while ((pos < dataLen) && (colCt < _charWidth))
                    {
                        dotCt = _buf[crntOffset + pos];
                        colCt += dotCt;

                        for (int i = 0; i < dotCt; i++)
                        {
                            if (blackDot)
                                rowImage += "@";
                            else
                                rowImage += " ";
                        }

                        blackDot = !blackDot;
                        pos++;
                    }

                    for (int j = 0; j <= rptCt; j++)
                    {
                        if (firstLine)
                        {
                            PrnParseCommon.addTextRow(
                                PrnParseRowTypes.Type.PCLFontChar,
                                _table,
                                PrnParseConstants.OvlShow.None,
                                string.Empty,
                                string.Empty,
                                "Shape",
                                rowImage);
                        }
                        else
                        {
                            PrnParseCommon.addTextRow(
                                PrnParseRowTypes.Type.PCLFontChar,
                                _table,
                                PrnParseConstants.OvlShow.None,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                rowImage);
                        }

                        firstLine = false;
                    }
                }
            }
        }
    }
}