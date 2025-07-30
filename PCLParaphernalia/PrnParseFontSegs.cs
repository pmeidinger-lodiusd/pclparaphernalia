using System.Data;
using System.Text;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class handles segmented data elements of downloadable soft fonts.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

class PrnParseFontSegs
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

    private PrnParseConstants.eContType _contType;

    private DataTable _table;

    private byte[] _buf;

    private int _fileOffset;
    private int _analysisLevel;
    private int _segRem;

    private bool _validSegs;
    private bool _showBinData;
    private bool _PCL;
    private PrnParseOptions _options;

    private PrnParseConstants.eOptOffsetFormats _indxOffsetFormat;
    private PrnParseRowTypes.eType _rowType;

    private readonly ASCIIEncoding _ascii = new ASCIIEncoding();

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // d e c o d e C h a r C o m p R e q                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Provide an interpretation of the contents of:                      //
    //  -   Character Complement   array (part of font header)            //
    //  -   Character Requirements array (part of Define Symbol Set)      //
    // One is the complement of the other.                                //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool DecodeCharCompReq(bool complement,
                                     bool format_MSL,
                                     bool PCL,
                                     int fileOffset,
                                     byte[] buf,
                                     int bufOffset,
                                     PrnParseLinkData linkData,
                                     PrnParseOptions options,
                                     DataTable table)
    {
        const int arrayBytes = 8;
        const int bitsPerByte = 8;
        const int arrayBits = arrayBytes * bitsPerByte;

        PCLCharCollections.eBitType bitType;
        PrnParseRowTypes.eType rowType;

        bool dataOK = true;

        ulong bitVal;

        string codeDesc,
               textDesc;

        int listIndex;

        bool bitSet,
                bitSig;

        int analysisLevel = linkData.AnalysisLevel;
        bool showBinData = options.FlagPCLMiscBinData;

        PrnParseConstants.eOptOffsetFormats indxOffsetFormat = options.IndxGenOffsetFormat;

        int offset = bufOffset;

        if (complement)
            textDesc = "Char. Complement";
        else
            textDesc = "Char. Requirements";

        if (PCL)
            rowType = PrnParseRowTypes.eType.PCLFontHddr;
        else
            rowType = PrnParseRowTypes.eType.PCLXLFontHddr;

        //----------------------------------------------------------------//
        //                                                                //
        // Obtain character collection bit array.                         //
        //                                                                //
        //----------------------------------------------------------------//

        ulong charCollArray = 0;

        for (int i = 0; i < arrayBytes; i++)
        {
            charCollArray = (charCollArray << 8) + buf[offset + i];
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Obtain aggregate values for the symbol index and collection    //
        // bits which are set.                                            //
        //                                                                //
        //----------------------------------------------------------------//

        ulong charCollVal = 0;
        ulong charCollIndex = 0;

        for (int i = 0; i < arrayBits; i++)
        {
            bitVal = ((ulong)0x01) << i;

            if ((charCollArray & bitVal) != 0)
            {
                // bit is set //

                listIndex = PCLCharCollections.GetindexForKey(i);

                bitType = PCLCharCollections.GetBitType(listIndex);

                if (bitType == PCLCharCollections.eBitType.Collection)
                    charCollVal += bitVal;
                else
                    charCollIndex += bitVal;
            }
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Display details of symbol index identifier bits.               //
        //                                                                //
        //----------------------------------------------------------------//

        if (format_MSL)
        {
            if ((complement) && (charCollIndex == 0x07))
                codeDesc = "'111' = MSL";
            else if ((!complement) && (charCollIndex == 0x00))
                codeDesc = "'000' = MSL";
            else
                codeDesc = "'" + charCollIndex.ToString() + "' not MSL value!";
        }
        else
        {
            if ((complement) && (charCollIndex == 0x06))
                codeDesc = "'110' = Unicode";
            else if ((!complement) && (charCollIndex == 0x01))
                codeDesc = "'001' = Unicode";
            else
                codeDesc = "'" + charCollIndex.ToString() + "' not Unicode value!";
        }

        PrnParseCommon.AddTextRow(
            rowType,
            table,
            PrnParseConstants.eOvlShow.None,
            string.Empty,
            textDesc,
            "Symbol index",
            codeDesc);

        //----------------------------------------------------------------//
        //                                                                //
        // Display details of collection bits.                            //
        //                                                                //
        //----------------------------------------------------------------//

        if (charCollVal == 0)
        {
            if (complement)
                codeDesc = "All bits unset - compatible with any character set";
            else
                codeDesc = "All bits unset - compatible with any typeface";

            PrnParseCommon.AddTextRow(
                rowType,
                table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                textDesc,
                "Collection",
                codeDesc);
        }
        else
        {
            for (int i = 0; i < arrayBits; i++)
            {
                bitVal = ((ulong)0x01) << i;

                if ((charCollVal & bitVal) != 0)
                    bitSet = true;
                else
                    bitSet = false;

                if (bitSet && (!complement))
                    bitSig = true;
                else if ((!bitSet) & (complement))
                    bitSig = true;
                else
                    bitSig = false;

                if (bitSig)
                {
                    listIndex = PCLCharCollections.GetindexForKey(i);

                    if (format_MSL)
                        codeDesc = PCLCharCollections.GetDescMSL(listIndex);
                    else
                        codeDesc = PCLCharCollections.GetDescUnicode(listIndex);

                    PrnParseCommon.AddTextRow(
                        rowType,
                        table,
                        PrnParseConstants.eOvlShow.None,
                        string.Empty,
                        textDesc,
                        "Collection",
                        codeDesc);
                }
            }
        }

        return dataOK;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g D a t a                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Process Segmented Data.                                            //
    //                                                                    //
    // PCL Format 15 (TrueType): the data consists of one or more         //
    // segments, each in TTLLD format:                                    //
    //                                                                    //
    //    bytes 0 -> 1         segment identifier                         //
    //          2 -> 3         segment data length 'n' (may be zero)      //
    //          4 -> 4 + (n-1) segment data                               //
    //                                                                    //
    // PCL Format 16 (Universal): the data consists of one or more        //
    // segments, each in TTLLLLD format:                                  //
    //                                                                    //
    //    bytes 0 -> 1         segment identifier                         //
    //          2 -> 5         segment data length 'n' (may be zero)      //
    //          6 -> 6 + (n-1) segment data                               //
    //                                                                    //
    // PCL XL segments have the same basic structure as PCL Universal     //
    // segments.                                                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool ProcessSegData(byte[] buf,
                                  int fileOffset,
                                  bool PCL,
                                  bool firstSeg,
                                  bool largeSegs,
                                  ref int bufRem,
                                  ref int bufOffset,
                                  ref int hddrDataRem,
                                  ref int hddrRem,
                                  ref int hddrChksVal,
                                  ref bool valid,
                                  PrnParseLinkData linkData,
                                  PrnParseOptions options,
                                  DataTable table)
    {
        PrnParseConstants.eContType contType;

        bool continuation = false;

        int binDataLen;
        int segHddrLen;
        int segSize;
        int segType;

        _buf = buf;
        _fileOffset = fileOffset;
        _linkData = linkData;
        _table = table;
        _PCL = PCL;
        _options = options;

        _analysisLevel = _linkData.AnalysisLevel;

        if (PCL)
        {
            _contType = PrnParseConstants.eContType.PCLFontHddr;
            _rowType = PrnParseRowTypes.eType.PCLFontHddr;
            _showBinData = options.FlagPCLMiscBinData;
        }
        else
        {
            _contType = PrnParseConstants.eContType.PCLXLFontHddr;
            _rowType = PrnParseRowTypes.eType.PCLXLFontHddr;
            _showBinData = options.FlagPCLXLMiscBinData;
        }

        if (largeSegs)
            segHddrLen = 6;
        else
            segHddrLen = 4;

        _indxOffsetFormat = options.IndxGenOffsetFormat;

        //----------------------------------------------------------------//
        //                                                                //
        // Reset continuation data.                                       //
        //                                                                //
        //----------------------------------------------------------------//

        contType = PrnParseConstants.eContType.None;

        _linkData.ResetContData();

        if (firstSeg)
        {
            //------------------------------------------------------------//
            //                                                            //
            // First segment.                                             //
            //                                                            //
            //------------------------------------------------------------//

            string text;

            segSize = 0;
            _segRem = 0;
            _validSegs = true;

            if (PCL)
                text = "PCL Binary";
            else
                text = "PCL XL Binary";

            PrnParseCommon.AddDataRow(
                PrnParseRowTypes.eType.DataBinary,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                _fileOffset + bufOffset,
                _analysisLevel,
                text,
                "[ " + hddrDataRem + " bytes ]",
                "Font header segmented data");
        }

        while ((hddrDataRem != 0) &&
               (contType == PrnParseConstants.eContType.None) &&
               (_validSegs))
        {
            if (_segRem == 0)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Either first segment, or previous segment fully        //
                // processed.                                             //
                // Output details of segment type and data length.        //
                //                                                        //
                //--------------------------------------------------------//

                if (bufRem < segHddrLen)
                {
                    //----------------------------------------------------//
                    //                                                    //
                    // Segment Type and Length data is not in buffer.     //
                    // Initiate (back-tracking) continuation.             //
                    //                                                    //
                    //----------------------------------------------------//

                    continuation = true;

                    contType = _contType;

                    _linkData.SetBacktrack(contType, -bufRem);
                }
                else
                {
                    //----------------------------------------------------//
                    //                                                    //
                    // Segment Type and Length data is in buffer.         //
                    //                                                    //
                    //----------------------------------------------------//

                    segType = (_buf[bufOffset] * 256) +
                               _buf[bufOffset + 1];

                    if (largeSegs)
                    {
                        segSize = (_buf[bufOffset + 2] * 65536 * 256) +
                                  (_buf[bufOffset + 3] * 65536) +
                                  (_buf[bufOffset + 4] * 256) +
                                   _buf[bufOffset + 5];
                    }
                    else
                    {
                        segSize = (_buf[bufOffset + 2] * 256) +
                                   _buf[bufOffset + 3];
                    }

                    switch (segType)
                    {
                        case 0x4150:
                            ProcessSeg_AP(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x4252:
                            ProcessSeg_BR(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x4343:
                            ProcessSeg_CC(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x4345:
                            ProcessSeg_CE(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x4350:
                            ProcessSeg_CP(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x4743:
                            ProcessSeg_GC(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x4749:
                            ProcessSeg_GI(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x4754:
                            ProcessSeg_GT(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x4946:
                            ProcessSeg_IF(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x5041:
                            ProcessSeg_PA(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x5046:
                            ProcessSeg_PF(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x5446:
                            ProcessSeg_TF(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x5645:
                            ProcessSeg_VE(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x5649:
                            ProcessSeg_VI(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x5652:
                            ProcessSeg_VR(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x5654:
                            ProcessSeg_VT(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x5857:
                            ProcessSeg_XW(segSize,
                                           segHddrLen,
                                           ref bufRem,
                                           ref bufOffset,
                                           ref hddrDataRem,
                                           ref hddrRem,
                                           ref hddrChksVal);
                            break;

                        case 0x00ffff:
                            ProcessSegNull(segSize,
                                            segHddrLen,
                                            ref bufRem,
                                            ref bufOffset,
                                            ref hddrDataRem,
                                            ref hddrRem,
                                            ref hddrChksVal);
                            break;

                        default:
                            ProcessSegUnknown(segType,
                                               segSize,
                                               segHddrLen,
                                               ref bufRem,
                                               ref bufOffset,
                                               ref hddrDataRem,
                                               ref hddrRem,
                                               ref hddrChksVal);
                            break;
                    }

                    contType = _linkData.GetContType();

                    if (contType != PrnParseConstants.eContType.None)
                        continuation = true;
                }
            }

            if (_segRem > bufRem)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Remainder of segment is not in buffer.                 //
                // Initiate (non back-tracking) continuation.             //
                //                                                        //
                //--------------------------------------------------------//

                continuation = true;

                contType = _contType;

                binDataLen = bufRem;
                _segRem -= bufRem;
                hddrDataRem -= bufRem;
                hddrRem -= bufRem;

                _linkData.SetContinuation(contType);
            }
            else
            {
                //--------------------------------------------------------//
                //                                                        //
                // Remainder of segment is in buffer.                     //
                //                                                        //
                //--------------------------------------------------------//

                binDataLen = _segRem;
                _segRem = 0;
                hddrDataRem -= binDataLen;
                hddrRem -= binDataLen;
            }

            if (binDataLen != 0)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Some, or all, of the segmented data is contained       //
                // within the current 'block'.                            //
                //                                                        //
                //--------------------------------------------------------//

                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    binDataLen,
                    "Segment data",
                    _showBinData,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);

                if (_PCL)
                {
                    for (int i = 0; i < binDataLen; i++)
                    {
                        hddrChksVal += _buf[bufOffset + i];
                    }
                }

                bufRem -= binDataLen;
                bufOffset += binDataLen;
            }
        }

        if ((hddrDataRem == 0) && (_validSegs))
        {
            //------------------------------------------------------------//
            //                                                            //
            // All segmented data processed.                              //
            //                                                            //
            //------------------------------------------------------------//

            valid = true;
        }

        return continuation;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g N u l l                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Null                                                 //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSegNull(int segSize,
                                int segHddrLen,
                                ref int bufRem,
                                ref int bufOffset,
                                ref int hddrDataRem,
                                ref int hddrRem,
                                ref int hddrChksVal)
    {
        string segTypeDesc = "Null";

        PrnParseConstants.eContType contType;

        int baseOffset,
              dataOffset,
              minSegSize,
              minSegLen;

        minSegSize = 0;
        minSegLen = segHddrLen + minSegSize;
        baseOffset = bufOffset + _fileOffset;
        dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields))");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;

                if (_segRem != 0)
                {
                    ReportError(
                        "Segment remainder " + _segRem + " non-zero",
                        string.Empty, string.Empty);
                }
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g U n k n o w n                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Unknown                                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSegUnknown(int segType,
                                   int segSize,
                                   int segHddrLen,
                                   ref int bufRem,
                                   ref int bufOffset,
                                   ref int hddrDataRem,
                                   ref int hddrRem,
                                   ref int hddrChksVal)
    {
        string segTypeDesc = "0x" + segType.ToString("X4") + ": Unknown";

        PrnParseConstants.eContType contType;

        int minSegSize = 0;
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ A P                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Application Support                                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_AP(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "AP: Application Support";

        PrnParseConstants.eContType contType;

        int minSegSize = 0;   // TODO when we discover the segment format     //
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//
            //                                                            //
            // TODO if we ever discover the segment format                //
            //                                                            //
            //------------------------------------------------------------//

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ B R                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Bitmap Resolution                                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_BR(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "BR: Bitmap Resolution";

        PrnParseConstants.eContType contType;

        ushort ui16a;

        int minSegSize = 4;
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//
            //                                                            //
            // bytes x  -x+1     X Resolution                             //
            //       x+2-x+3     Y Resolution                             //
            //                                                            //
            //------------------------------------------------------------//

            ui16a = (ushort)((_buf[dataOffset] * 256) +
                              _buf[dataOffset + 1]);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "        data:",
                "X Resolution:",
                ui16a + " dots per inch");

            ui16a = (ushort)((_buf[dataOffset + 2] * 256) +
                              _buf[dataOffset + 3]);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        data:",
                "Y Resolution:",
                ui16a + " dots per inch");

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;

                if (_segRem != 0)
                {
                    ReportError(
                        "Segment remainder " + _segRem + " non-zero",
                        string.Empty, string.Empty);
                }
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ C C                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Character Complement                                 //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_CC(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "CC: Character Complement";

        PrnParseConstants.eContType contType;

        int minSegSize = 8;
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//
            //                                                            //
            // Segment format is the standard Character Complement data.  //
            //                                                            //
            //------------------------------------------------------------//

            DecodeCharCompReq(
                true,
                false,
                _PCL,
                _fileOffset,
                _buf,
                dataOffset,
                _linkData,
                _options,
                _table);

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ C E                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Character Enhancement                                //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_CE(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "CE: Character Enhancement";

        PrnParseConstants.eContType contType;

        int baseOffset,
              dataOffset,
              minSegSize,
              minSegLen;

        minSegSize = 0;   // TODO if we ever discover the segment format  //
        minSegLen = segHddrLen + minSegSize;
        baseOffset = bufOffset + _fileOffset;
        dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//
            //                                                            //
            // TODO if we ever discover the segment format                //
            //                                                            //
            //------------------------------------------------------------//

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ C P                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Copyright                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_CP(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "CP: Copyright";

        const int sliceMax = 50;

        PrnParseConstants.eContType contType;

        bool firstLine;

        string textA,
               textB;

        int cpyRem,
              sliceLen,
              cpyOffset;

        int minSegSize = segSize;
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//
            //                                                            //
            //        x -  ..    Copyright data; treat as an array of     //
            //                   ASCII characters.                        //
            //                                                            //
            //------------------------------------------------------------//

            firstLine = true;
            cpyRem = segSize;
            cpyOffset = 0;

            while (cpyRem > 0)
            {
                if (cpyRem > sliceMax)
                    sliceLen = sliceMax;
                else
                    sliceLen = cpyRem;

                if (firstLine)
                {
                    textA = "        data:";
                    textB = "Copyright:";
                }
                else
                {
                    textA = string.Empty;
                    textB = string.Empty;
                }

                PrnParseCommon.AddDataRow(
                    _rowType,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _indxOffsetFormat,
                    baseOffset + cpyOffset,
                    _analysisLevel,
                    textA,
                    textB,
                    _ascii.GetString(_buf,
                                      dataOffset + cpyOffset,
                                      sliceLen));

                cpyRem -= sliceLen;
                cpyOffset += sliceLen;
                firstLine = false;
            }

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ G C                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Galley Character                                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_GC(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "GC: Galley Character";

        PrnParseConstants.eContType contType;

        ushort ui16a,
               numRegions = 0;

        bool numRegionsOK = false;

        int minSegSize = 6;
        int varSegSize = 0;
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            //------------------------------------------------------------//
            //                                                            //
            // Obtain 'Number of Regions' value from header.              //
            //                                                            //
            //------------------------------------------------------------//

            numRegions = (ushort)((_buf[dataOffset + 4] * 256) +
                                   _buf[dataOffset + 5]);

            varSegSize = 6 * numRegions;

            if ((minSegLen + varSegSize) <= PrnParseConstants.bufSize)
            {
                minSegSize += varSegSize;
                minSegLen += varSegSize;
                numRegionsOK = true;
            }
            else
            {
                numRegionsOK = false;
                _validSegs = false;
            }
        }

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//
            //                                                            //
            // bytes x  -x+1     Format (should be zero)                  //
            //                                                            //
            //------------------------------------------------------------//

            ui16a = (ushort)((_buf[dataOffset] * 256) +
                              _buf[dataOffset + 1]);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "        data:",
                "Format:",
                ui16a.ToString());

            //------------------------------------------------------------//
            //                                                            //
            // bytes x+2-x+3     Default Galley Character                 //
            //                                                            //
            //------------------------------------------------------------//

            ui16a = (ushort)((_buf[dataOffset + 2] * 256) +
                              _buf[dataOffset + 3]);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        data:",
                "Default Galley:",
                "0x" + ui16a.ToString("X4"));

            //------------------------------------------------------------//
            //                                                            //
            // bytes x+4-x+5     Number of Regions                        //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 4,
                _analysisLevel,
                "        data:",
                "Region Count:",
                numRegions.ToString());

            if (numRegionsOK)
            {

                //--------------------------------------------------------//
                //                                                        //
                // bytes x+6- ..     Region N definitions                 //
                //                   numRegions entries, each of format:  //
                //                   bytes  0 -  1     Region Start Code  //
                //                   bytes  2 -  3     Region End Code    //
                //                   bytes  4 -  5     Region Galley Char //
                //                                                        //
                //--------------------------------------------------------//

                for (int i = 0; i < numRegions; i++)
                {
                    int j = i * 6;

                    ui16a = (ushort)((_buf[dataOffset + 6 + j] * 256) +
                                      _buf[dataOffset + 7 + j]);

                    PrnParseCommon.AddDataRow(
                        _rowType,
                        _table,
                        PrnParseConstants.eOvlShow.None,
                        _indxOffsetFormat,
                        baseOffset + 6 + j,
                        _analysisLevel,
                        "        data:",
                        "Region Start:",
                        "0x" + ui16a.ToString("X4"));

                    ui16a = (ushort)((_buf[dataOffset + 8 + j] * 256) +
                                      _buf[dataOffset + 9 + j]);

                    PrnParseCommon.AddDataRow(
                        _rowType,
                        _table,
                        PrnParseConstants.eOvlShow.None,
                        _indxOffsetFormat,
                        baseOffset + 8 + j,
                        _analysisLevel,
                        "        data:",
                        "       End:",
                        "0x" + ui16a.ToString("X4"));

                    ui16a = (ushort)((_buf[dataOffset + 10 + j] * 256) +
                                      _buf[dataOffset + 11 + j]);

                    PrnParseCommon.AddDataRow(
                        _rowType,
                        _table,
                        PrnParseConstants.eOvlShow.None,
                        _indxOffsetFormat,
                        baseOffset + 10 + j,
                        _analysisLevel,
                        "        data:",
                        "       Galley:",
                        "0x" + ui16a.ToString("X4"));
                }
            }
            else
            {
                ReportError(
                    "Possibly corrupt: 'Region Count' value " + numRegions,
                    "makes minimum segment header size " +
                        (minSegLen + varSegSize) + " bytes",
                    "This is larger than application buffer size of " +
                        PrnParseConstants.bufSize + " bytes");
            }

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;

                if (_segRem != 0)
                {
                    ReportError(
                        "Segment remainder " + _segRem + " non-zero",
                        string.Empty, string.Empty);
                }
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ G I                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Global Intellifont                               //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_GI(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "GI: Global Intellifont";

        PrnParseConstants.eContType contType;

        int minSegSize = 0;   // TODO if we ever discover the segment format  //
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ G T                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Global TrueType                                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_GT(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "GT: Global TrueType";

        PrnParseConstants.eContType contType;

        int tableOffset,
              padBytes;

        uint ui32a,
               ui32b;

        uint offset,
               size,
               padSize;

        ushort ui16a,
               numTables = 0;

        bool numTablesOK = false;

        int minSegSize = 12;
        int varSegSize = 0;
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            //------------------------------------------------------------//
            //                                                            //
            // Obtain 'Number of Tables' value from SFNT Directory Header //
            //                                                            //
            //------------------------------------------------------------//

            numTables = (ushort)((_buf[dataOffset + 4] * 256) +
                                  _buf[dataOffset + 5]);

            varSegSize = 16 * numTables;

            if ((minSegLen + varSegSize) <= PrnParseConstants.bufSize)
            {
                minSegSize += varSegSize;
                minSegLen += varSegSize;
                numTablesOK = true;
            }
            else
            {
                numTablesOK = false;
                _validSegs = false;
            }
        }

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//
            //                                                            //
            // Interpret SFNT Directory Header.                           //
            // bytes  x -  x+3   SFNT Version                             //
            //                                                            //
            //------------------------------------------------------------//

            tableOffset = baseOffset;

            ui32a = (uint)((_buf[dataOffset] * 65536 * 256) +
                             (_buf[dataOffset + 1] * 65536) +
                             (_buf[dataOffset + 2] * 256) +
                              _buf[dataOffset + 3]);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "        data:",
                "SFNT version:",
                "0x" + ui32a.ToString("X8"));

            //------------------------------------------------------------//
            //                                                            //
            // bytes x+4-x+5     Number of Tables                         //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 4,
                _analysisLevel,
                "        data:",
                "Table Count:",
                numTables.ToString());

            //------------------------------------------------------------//
            //                                                            //
            // bytes x+6-x+7     Search Range                             //
            //                   This should be:                          //
            //                   (max. power of 2 <= numTables) * 16      //
            //                                                            //
            //------------------------------------------------------------//

            ui16a = (ushort)((_buf[dataOffset + 6] * 256) +
                              _buf[dataOffset + 7]);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 6,
                _analysisLevel,
                "        data:",
                "Search Range:",
                ui16a.ToString());

            //------------------------------------------------------------//
            //                                                            //
            // bytes x+8-x+9     Entry Selector                           //
            //                   This should be:                          //
            //                   Log-base-2 (max. power of 2 <= numTables)//
            //                                                            //
            //------------------------------------------------------------//

            ui16a = (ushort)((_buf[dataOffset + 8] * 256) +
                              _buf[dataOffset + 9]);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 8,
                _analysisLevel,
                "        data:",
                "Entry Selector:",
                ui16a.ToString());

            //------------------------------------------------------------//
            //                                                            //
            // bytes x+10-x+11   Range Shift                              //
            //                   This should be:                          //
            //                   (numTables * 16) - SearchRange           //
            //                                                            //
            //------------------------------------------------------------//

            ui16a = (ushort)((_buf[dataOffset + 10] * 256) +
                              _buf[dataOffset + 11]);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 10,
                _analysisLevel,
                "        data:",
                "Range Shift:",
                ui16a.ToString());

            if (numTablesOK)
            {
                //--------------------------------------------------------//
                //                                                        //
                // bytes x+12- ..    Table directory entries              //
                //                   numTables entries, each of format:   //
                //                   bytes  0 -  3     Tag                //
                //                   bytes  4 -  7     CheckSum           //
                //                   bytes  8 - 11     Offset             //
                //                   bytes 12 - 15     Size               //
                //                                                        //
                //--------------------------------------------------------//

                for (int i = 0; i < numTables; i++)
                {
                    int j = i * 16;

                    //----------------------------------------------------//
                    //                                                    //
                    // Tag                                                //
                    //                                                    //
                    //----------------------------------------------------//

                    PrnParseCommon.AddDataRow(
                        _rowType,
                        _table,
                        PrnParseConstants.eOvlShow.None,
                        _indxOffsetFormat,
                        baseOffset + 12 + j,
                        _analysisLevel,
                        "        data:",
                        "Table Tag:",
                        _ascii.GetString(_buf, dataOffset + 12 + j, 4));

                    //----------------------------------------------------//
                    //                                                    //
                    // Checksum                                           //
                    //                                                    //
                    //----------------------------------------------------//

                    ui32a = (uint)((_buf[dataOffset + 16 + j] * 65536 * 256) +
                                     (_buf[dataOffset + 17 + j] * 65536) +
                                     (_buf[dataOffset + 18 + j] * 256) +
                                      _buf[dataOffset + 19 + j]);

                    PrnParseCommon.AddDataRow(
                        _rowType,
                        _table,
                        PrnParseConstants.eOvlShow.None,
                        _indxOffsetFormat,
                        baseOffset + 16 + j,
                        _analysisLevel,
                        "        data:",
                        "      Checksum:",
                        "0x" + ui32a.ToString("X8"));

                    //----------------------------------------------------//
                    //                                                    //
                    // Offset                                             //
                    //                                                    //
                    //----------------------------------------------------//

                    offset = (uint)((_buf[dataOffset + 20 + j] * 65536 * 256) +
                                       (_buf[dataOffset + 21 + j] * 65536) +
                                       (_buf[dataOffset + 22 + j] * 256) +
                                        _buf[dataOffset + 23 + j]);

                    ui32b = (uint)(tableOffset + offset);

                    if (offset == 0)
                        PrnParseCommon.AddDataRow(
                            _rowType,
                            _table,
                            PrnParseConstants.eOvlShow.None,
                            _indxOffsetFormat,
                            baseOffset + 20 + j,
                            _analysisLevel,
                            "        data:",
                            "      Offset:",
                            "0");
                    else
                    {
                        PrnParseCommon.AddDataRow(
                            _rowType,
                            _table,
                            PrnParseConstants.eOvlShow.None,
                            _indxOffsetFormat,
                            baseOffset + 20 + j,
                            _analysisLevel,
                            "        data:",
                            "      Offset:",
                            offset.ToString() + " relative (= " +
                            ui32b.ToString() + " absolute)");
                    }

                    //----------------------------------------------------//
                    //                                                    //
                    // Size                                               //
                    // Where the size is not a multiple of 4 bytes, the   //
                    // table should be padded to the next multiple.       //
                    //                                                    //
                    //----------------------------------------------------//

                    size = (uint)((_buf[dataOffset + 24 + j] * 65536 * 256) +
                                     (_buf[dataOffset + 25 + j] * 65536) +
                                     (_buf[dataOffset + 26 + j] * 256) +
                                      _buf[dataOffset + 27 + j]);

                    padBytes = (int)(size % 4);

                    if (padBytes == 0)
                    {
                        PrnParseCommon.AddDataRow(
                            _rowType,
                            _table,
                            PrnParseConstants.eOvlShow.None,
                            _indxOffsetFormat,
                            baseOffset + 24 + j,
                            _analysisLevel,
                            "        data:",
                            "      Size:",
                            size.ToString());
                    }
                    else
                    {
                        padBytes = 4 - padBytes;
                        padSize = (uint)(size + padBytes);

                        PrnParseCommon.AddDataRow(
                            _rowType,
                            _table,
                            PrnParseConstants.eOvlShow.None,
                            _indxOffsetFormat,
                            baseOffset + 24 + j,
                            _analysisLevel,
                            "        data:",
                            "      Size:",
                            size.ToString() + " (padded size = " +
                            padSize.ToString() + ")");
                    }

                    if ((offset > segSize) || ((offset + size) > segSize))
                    {
                        ReportError(
                            "Offset and/or size incompatible with" +
                                " segment size",
                            string.Empty, string.Empty);
                    }
                }
            }
            else
            {
                ReportError(
                    "Possibly corrupt: 'Table Count' value " + numTables,
                    "makes minimum segment header size " +
                        (minSegLen + varSegSize) + " bytes",
                    "This is larger than application buffer size of " +
                        PrnParseConstants.bufSize + " bytes");
            }

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ I F                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Intellifont Face                                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_IF(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "IF: Intellifont Face";

        PrnParseConstants.eContType contType;

        int baseOffset,
              dataOffset,
              minSegSize,
              minSegLen;

        minSegSize = 0;   // TODO if we ever discover the segment format  //
        minSegLen = segHddrLen + minSegSize;
        baseOffset = bufOffset + _fileOffset;
        dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ P A                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Panose Description                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_PA(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "PA: Panose Description";

        PrnParseConstants.eContType contType;

        byte b;

        int baseOffset,
              dataOffset,
              minSegSize,
              minSegLen;

        string panoseSet;

        minSegSize = 10;
        minSegLen = segHddrLen + minSegSize;
        baseOffset = bufOffset + _fileOffset;
        dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//
            //                                                            //
            // bytes x  -x+9     Panose classification numbers            //
            //                                                            //
            //------------------------------------------------------------//

            panoseSet = string.Empty;

            for (int i = 0; i < 10; i++)
            {
                b = _buf[dataOffset + i];

                if (panoseSet.Length != 0)
                    panoseSet += "-";

                panoseSet += b;
            }

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "        data:",
                "Panose Class:",
                panoseSet);

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ P F                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: PostScript Font                                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_PF(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "PF: PostScript Font";

        PrnParseConstants.eContType contType;

        int minSegSize = 0;   // TODO if we ever discover the segment format  //
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ T F                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Type Face String                                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_TF(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "TF: Type Face String";

        PrnParseConstants.eContType contType;

        int minSegSize = 0;   // TODO if we ever discover the segment format  //
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ V E                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Vertical Exclude                                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_VE(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "VE: Vertical Exclude";

        PrnParseConstants.eContType contType;

        ushort ui16a,
               numRanges = 0;

        bool numRangesOK = false;

        int minSegSize = 2;
        int varSegSize = 0;
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            //------------------------------------------------------------//
            //                                                            //
            // Obtain 'Number of Ranges' value from header.               //
            //                                                            //
            //------------------------------------------------------------//

            numRanges = _buf[bufOffset + 7];

            varSegSize = 4 * numRanges;

            if ((minSegLen + varSegSize) <= PrnParseConstants.bufSize)
            {
                minSegSize += varSegSize;
                minSegLen += varSegSize;
                numRangesOK = true;
            }
            else
            {
                numRangesOK = false;
                _validSegs = false;
            }
        }

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//
            //                                                            //
            // bytes  x          Format (should be zero)                  //
            //                                                            //
            //------------------------------------------------------------//

            ui16a = _buf[bufOffset + 6];

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 6,
                _analysisLevel,
                "        data:",
                "Format:",
                ui16a.ToString());

            //------------------------------------------------------------//
            //                                                            //
            // bytes  x+1        Number of Ranges                         //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 7,
                _analysisLevel,
                "        data:",
                "Range Count:",
                numRanges.ToString());

            if (numRangesOK)
            {
                //--------------------------------------------------------//
                //                                                        //
                // bytes x+2 - ..    Range N definitions                  //
                //                   numRanges entries, each of format:   //
                //                   bytes  0 -  1     Range First Code   //
                //                   bytes  2 -  3     Range Last Code    //
                //                                                        //
                //--------------------------------------------------------//

                for (int i = 0; i < numRanges; i++)
                {
                    int j = i * 4;

                    ui16a = (ushort)((_buf[bufOffset + 8 + j] * 256) +
                                       _buf[bufOffset + 9 + j]);

                    PrnParseCommon.AddDataRow(
                        _rowType,
                        _table,
                        PrnParseConstants.eOvlShow.None,
                        _indxOffsetFormat,
                        baseOffset + 8 + j,
                        _analysisLevel,
                        "        data:",
                        "Range FirstCode:",
                        "0x" + ui16a.ToString("X4"));

                    j += 2;

                    ui16a = (ushort)((_buf[bufOffset + 8 + j] * 256) +
                                       _buf[bufOffset + 9 + j]);

                    PrnParseCommon.AddDataRow(
                        _rowType,
                        _table,
                        PrnParseConstants.eOvlShow.None,
                        _indxOffsetFormat,
                        baseOffset + 8 + j,
                        _analysisLevel,
                        "        data:",
                        "      LastCode:",
                        "0x" + ui16a.ToString("X4"));
                }
            }
            else
            {
                ReportError(
                    "Possibly corrupt: 'Range Count' value " + numRanges,
                    "makes minimum segment header size " + (minSegLen + varSegSize) + " bytes",
                    "This is larger than application buffer size of " + PrnParseConstants.bufSize + " bytes");
            }

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;

                if (_segRem != 0)
                {
                    ReportError(
                        "Segment remainder " + _segRem + " non-zero",
                        string.Empty, string.Empty);
                }
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ V I                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Vendor Information                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_VI(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "VI: Vendor Information";

        const int sliceMax = 50;

        PrnParseConstants.eContType contType;

        bool firstLine;

        string textA,
               textB;

        int sliceLen,
              infRem,
              infOffset;

        int minSegSize = segSize;
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//
            //                                                            //
            //        x -  ..    Vendor information; treat as an array of //
            //                   ASCII characters.                        //
            //                                                            //
            //------------------------------------------------------------//

            firstLine = true;
            infRem = segSize;
            infOffset = 0;

            while (infRem > 0)
            {
                if (infRem > sliceMax)
                    sliceLen = sliceMax;
                else
                    sliceLen = infRem;

                if (firstLine)
                {
                    textA = "        data:";
                    textB = "Information:";
                }
                else
                {
                    textA = string.Empty;
                    textB = string.Empty;
                }

                PrnParseCommon.AddDataRow(
                    _rowType,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _indxOffsetFormat,
                    baseOffset + infOffset,
                    _analysisLevel,
                    textA,
                    textB,
                    _ascii.GetString(_buf,
                                      dataOffset + infOffset,
                                      sliceLen));

                infRem -= sliceLen;
                infOffset += sliceLen;
                firstLine = false;
            }

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ V R                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Vertical Rotation                                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_VR(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "VR: Vertical Rotation";

        PrnParseConstants.eContType contType;

        short si16a;

        ushort ui16a;

        int minSegSize = 4;
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//
            //                                                            //
            // bytes x  -x+1     Format (should be zero)                  //
            //                                                            //
            //------------------------------------------------------------//

            ui16a = (ushort)((_buf[dataOffset] * 256) +
                              _buf[dataOffset + 1]);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "        data:",
                "Format:",
                ui16a.ToString());

            //------------------------------------------------------------//
            //                                                            //
            // bytes x+2-x+3     sTypoDescender                           //
            //                                                            //
            //------------------------------------------------------------//

            si16a = (short)((_buf[dataOffset + 2] * 256) +
                             _buf[dataOffset + 3]);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        data:",
                "sTypoDescender:",
                si16a.ToString());

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;

                if (_segRem != 0)
                {
                    ReportError(
                        "Segment remainder " + _segRem + " non-zero",
                        string.Empty, string.Empty);
                }
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ V T                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: Vertical Transformation                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_VT(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "VT: Vertical Transformation";

        PrnParseConstants.eContType contType;

        int eoTMOffset;

        ushort ui16a,
               numSubs;

        int minSegSize = segSize;
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//
            //                                                            //
            // Derive 'Number of Substitutes' value from Segment Size.    //
            //                                                            //
            //------------------------------------------------------------//

            numSubs = (ushort)((segSize - 4) / 4);

            //------------------------------------------------------------//
            //                                                            //
            // bytes x  - ..     Substitute N definitions                 //
            //                   numSubs entries, each of format:         //
            //                   bytes  0 -  1     Horizontal Glyph ID    //
            //                   bytes  2 -  3     Vertical Sub. Glyph ID //
            //                                                            //
            //------------------------------------------------------------//

            for (int i = 0; i < numSubs; i++)
            {
                int j = i * 4;

                ui16a = (ushort)((_buf[dataOffset + j] * 256) +
                                  _buf[dataOffset + j + 1]);

                PrnParseCommon.AddDataRow(
                    _rowType,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _indxOffsetFormat,
                    baseOffset + j,
                    _analysisLevel,
                    "        data:",
                    "Glyph ID:",
                    ui16a.ToString());

                ui16a = (ushort)((_buf[dataOffset + j + 2] * 256) +
                                  _buf[dataOffset + j + 3]);

                PrnParseCommon.AddDataRow(
                    _rowType,
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _indxOffsetFormat,
                    baseOffset + j + 2,
                    _analysisLevel,
                    "        data:",
                    "      Sub. ID:",
                    ui16a.ToString());
            }

            //------------------------------------------------------------//
            //                                                            //
            // bytes z  - z+1    End of Table Marker 1 (should be 0xFFFF) //
            // bytes z+2- z+3    End of Table Marker 2 (should be 0xFFFF) //
            //                                                            //
            //------------------------------------------------------------//

            eoTMOffset = segHddrLen + (numSubs * 4);
            minSegLen = eoTMOffset + 4;

            ui16a = (ushort)((_buf[dataOffset + eoTMOffset] * 256) +
                              _buf[dataOffset + eoTMOffset + 1]);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + eoTMOffset,
                _analysisLevel,
                "        data:",
                "EoT Marker 1:",
                "0x" + ui16a.ToString("X4"));

            ui16a = (ushort)((_buf[dataOffset + eoTMOffset + 2] * 256) +
                              _buf[dataOffset + eoTMOffset + 3]);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + eoTMOffset + 2,
                _analysisLevel,
                "        data:",
                "EoT Marker 2:",
                "0x" + ui16a.ToString("X4"));

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;

                if (_segRem != 0)
                {
                    ReportError(
                        "Segment remainder " + _segRem + " non-zero",
                        string.Empty, string.Empty);

                }
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p r o c e s s S e g _ X W                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Segment type: X-Window Font                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ProcessSeg_XW(int segSize,
                               int segHddrLen,
                               ref int bufRem,
                               ref int bufOffset,
                               ref int hddrDataRem,
                               ref int hddrRem,
                               ref int hddrChksVal)
    {
        string segTypeDesc = "XW: X-Window Font";

        PrnParseConstants.eContType contType;

        int minSegSize = 0;   // TODO if we ever discover the segment format  //
        int minSegLen = segHddrLen + minSegSize;
        int baseOffset = bufOffset + _fileOffset;
        int dataOffset = bufOffset + segHddrLen;

        if (bufRem < minSegLen)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Minimum required size not in buffer.                       //
            // Initiate (back-tracking) continuation.                     //
            //                                                            //
            //------------------------------------------------------------//

            contType = _contType;

            _linkData.SetBacktrack(contType, -bufRem);
        }
        else
        {
            if (_showBinData)
            {
                PrnParseData.ProcessBinary(
                    _table,
                    PrnParseConstants.eOvlShow.None,
                    _buf,
                    _fileOffset,
                    bufOffset,
                    minSegLen,
                    "Segment header",
                    true,
                    false,
                    true,
                    _indxOffsetFormat,
                    _analysisLevel);
            }

            if (_PCL)
            {
                for (int i = 0; i < minSegLen; i++)
                {
                    hddrChksVal += _buf[bufOffset + i];
                }
            }

            //------------------------------------------------------------//
            //                                                            //
            // Interpret segment header bytes:                            //
            // bytes  0 -  1     segment type                             //
            //        2 -  x-1   segment size  (format 15: x=4;           //
            //                                  format 16: x=6)           //
            //        x -  ..    segment data                             //
            //                                                            //
            //------------------------------------------------------------//

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset,
                _analysisLevel,
                "Segment type:",
                string.Empty,
                segTypeDesc);

            PrnParseCommon.AddDataRow(
                _rowType,
                _table,
                PrnParseConstants.eOvlShow.None,
                _indxOffsetFormat,
                baseOffset + 2,
                _analysisLevel,
                "        size:",
                string.Empty,
                segSize + " (" +
                (segSize + segHddrLen) +
                " including type & size fields)");

            baseOffset += segHddrLen;

            //------------------------------------------------------------//

            //------------------------------------------------------------//
            //                                                            //
            // Adjust offsets and remainders.                             //
            //                                                            //
            //------------------------------------------------------------//

            bufOffset += minSegLen;
            bufRem -= minSegLen;
            hddrDataRem -= minSegLen;
            hddrRem -= minSegLen;

            //------------------------------------------------------------//
            //                                                            //
            // Remaining binary segment data.                             //
            //                                                            //
            //------------------------------------------------------------//

            if ((segSize - minSegLen) > hddrDataRem)
            {
                ReportError(
                    "Segment (size " + segSize + ") larger than",
                    "remainder (" + hddrDataRem + ") of segmented data",
                    "Header is  internally inconsistent");

                _segRem = hddrDataRem;
            }
            else
            {
                _segRem = segSize - minSegSize;
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r e p o r t E r r o r                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Report error.                                                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void ReportError(string line1,
                              string line2,
                              string line3)
    {
        _validSegs = false;

        PrnParseCommon.AddTextRow(
            PrnParseRowTypes.eType.MsgWarning,
            _table,
            PrnParseConstants.eOvlShow.None,
            string.Empty,
            "*** Warning ***",
            string.Empty,
            line1);

        if (line2 != string.Empty)
            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgWarning,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                string.Empty,
                string.Empty,
                line2);

        if (line3 != string.Empty)
            PrnParseCommon.AddTextRow(
                PrnParseRowTypes.eType.MsgWarning,
                _table,
                PrnParseConstants.eOvlShow.None,
                string.Empty,
                string.Empty,
                string.Empty,
                line3);
    }
}