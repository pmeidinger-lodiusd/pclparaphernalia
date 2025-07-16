using System;
using System.Data;
using System.IO;
using System.Windows;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides PCL handling for the Soft Font Generate tool.
/// 
/// © Chris Hutchinson 2012
/// 
/// </summary>

class ToolSoftFontGenPCL
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    const int cSizeHddrFmt15Max = 0xffff;
    const int cSizeHddrDesc = 72;
    const int cSizeHddrTrail = 2;

    const int cSizeCharHddr = 4;
    const int cSizeCharGlyphHddr = 4;
    const int cSizeCharTrail = 2;

    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private readonly ToolSoftFontGenPCLCommon _baseHandler;

    private Stream _opStream = null;
    private BinaryWriter _binWriter = null;

    private readonly ToolSoftFontGenTTF _ttfHandler = null;

    private readonly DataTable _tableLog;

    private bool _symbolMapping = false;

    //--------------------------------------------------------------------//
    //                                              C o n s t r u c t o r //
    // T o o l S o f t G e n P C L                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public ToolSoftFontGenPCL(DataTable tableLog,
                               ToolSoftFontGenTTF ttfHandler)
    {
        _baseHandler = new ToolSoftFontGenPCLCommon();

        _tableLog = tableLog;

        _ttfHandler = ttfHandler;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e F o n t                                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Generate PCLETTO font.                                             //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool GenerateFont(ref string pclFilename,
                                 ref bool monoSpaced,
                                 bool symbolMapping,
                                 bool fmt16,
                                 bool segGTLast,
                                 bool usePCLT,
                                 bool symSetUnbound,
                                 bool tabvmtxPresent,
                                 bool flagVMetrics,
                                 byte symSetType,
                                 int sizeCharSet,
                                 ushort symSet,
                                 ushort style,
                                 sbyte strokeWeight,
                                 ushort typeface,
                                 ulong charCollComp,
                                 byte[] conversionText)
    {
        bool flagOK = true;
        bool useVMetrics;

        if (fmt16)
            useVMetrics = flagVMetrics;
        else
            useVMetrics = false;

        _baseHandler.Initialise(_ttfHandler);

        _symbolMapping = symbolMapping;

        //----------------------------------------------------------------//
        //                                                                //
        // Open print file and stream.                                    //
        //                                                                //
        //----------------------------------------------------------------//

        try
        {
            flagOK = _baseHandler.StreamOpen(ref pclFilename,
                                     false,
                                     ref _binWriter,
                                     ref _opStream);
        }
        catch (Exception exc)
        {
            flagOK = false;

            MessageBox.Show(exc.ToString(),
                            "Failure to open output font file",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
        }

        if (flagOK)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Generate font file contents.                               //
            //                                                            //
            //------------------------------------------------------------//

            ushort numChars = 0,
                   firstCode = 0,
                   lastCode = 0,
                   maxGlyphId = 0,
                   maxComponentDepth = 0,
                   unitsPerEm = 0;

            bool glyphZeroExists = false;

            _ttfHandler.GlyphReferencedUnmarkAll();

            _ttfHandler.GetBasicMetrics(ref numChars,
                                         ref firstCode,
                                         ref lastCode,
                                         ref maxGlyphId,
                                         ref maxComponentDepth,
                                         ref unitsPerEm,
                                         ref glyphZeroExists);

            try
            {
                //--------------------------------------------------------//
                //                                                        //
                // Write font header.                                     //
                //                                                        //
                //--------------------------------------------------------//

                WriteHddr(ref monoSpaced,
                           fmt16,
                           segGTLast,
                           usePCLT,
                           glyphZeroExists,
                           symSetUnbound,
                           tabvmtxPresent,
                           useVMetrics,
                           symSetType,
                           firstCode,
                           lastCode,
                           numChars,
                           unitsPerEm,
                           symSet,
                           style,
                           strokeWeight,
                           typeface,
                           charCollComp,
                           conversionText);

                //--------------------------------------------------------//
                //                                                        //
                // Write Galley Character and font characters.            //
                //                                                        //
                //--------------------------------------------------------//

                if (glyphZeroExists)
                    WriteChar(0xffff, 0, 0, 0, maxGlyphId);

                WriteCharSet(maxGlyphId, sizeCharSet, symSetUnbound);

                //--------------------------------------------------------//
                //                                                        //
                // Close streams and files.                               //
                //                                                        //
                //--------------------------------------------------------//

                _binWriter.Close();
                _opStream.Close();

                _ttfHandler.FontFileClose();
            }

            catch (Exception exc)
            {
                flagOK = false;

                MessageBox.Show(exc.ToString(),
                                "Failure to write font file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        return flagOK;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l s B y t e                                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return low (least-significant) byte from supplied unsigned 16-bit  //
    // integer.                                                           //
    //                                                                    //
    //--------------------------------------------------------------------//

    private byte LsByte(ushort value)
    {
        return (byte)(value & 0x00ff);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l s U I n t 1 6                                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return low (least-significant) unsigned 16-bit integer from        //
    // supplied unsigned 32-bit integer.                                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    private ushort LsUInt16(uint value)
    {
        return (ushort)(value & 0x0000ffff);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m s B y t e                                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return high (most-significant) byte from supplied unsigned 16-bit  //
    // integer.                                                           //
    //                                                                    //
    //--------------------------------------------------------------------//

    private byte MsByte(ushort value)
    {
        return (byte)((value & 0xff00) >> 8);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m s U I n t 1 6                                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return high (most-significant) unsigned 16-bit integer from        //
    // supplied unsigned 32-bit integer.                                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    private ushort MsUInt16(uint value)
    {
        return (ushort)((value & 0xffff0000) >> 16);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // w r i t e C h a r                                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write PCL format character data:                                   //
    //                                                                    //
    //    <esc>*c#E         Character Code:                               //
    //                      #      = decimal character code               //
    //    <esc>(s#W[data]   Character Descriptor / Data                   //
    //                      #      = number of bytes of data              //
    //                      [data] = font character data                  //
    //                                                                    //
    // Note that the function may be called recursively, if a glyph is    //
    // composite (i.e. made up of two or more components).                //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void WriteChar(ushort charCode,
                            ushort codepoint,
                            ushort glyphId,
                            ushort depth,
                            ushort maxGlyphId)
    {
        ushort glyphWidth = 0,
               glyphHeight = 0,
               charBlockSize = 0,
               charDataSize = 0;

        short glyphLSB = 0,
              glyphTSB = 0;

        uint glyphOffset = 0,
               glyphLength = 0;

        bool glyphComposite = false;

        byte checksumMod256;

        byte[] charHddr = new byte[cSizeCharHddr];
        byte[] charGlyphHddr = new byte[cSizeCharGlyphHddr];
        byte[] charTrail = new byte[cSizeCharTrail];
        byte[] glyphData = null;

        //----------------------------------------------------------------//
        //                                                                //
        // Mark glyph as used.                                            //
        // These markers are checked for composite sub-glyphs.            //
        //                                                                //
        //----------------------------------------------------------------//

        _ttfHandler.GlyphReferencedMark(glyphId);

        //----------------------------------------------------------------//
        //                                                                //
        // Get glyph details:                                             //
        //    advance width.                                              //
        //    left-side bearing.                                          //
        //    offset and length of the glyph data in the TTF file.        //
        //                                                                //
        //----------------------------------------------------------------//

        _ttfHandler.GetGlyphData(glyphId,
                                  ref glyphWidth,
                                  ref glyphHeight,  // not used here
                                  ref glyphLSB,
                                  ref glyphTSB,     // not used here
                                  ref glyphOffset,
                                  ref glyphLength,
                                  ref glyphComposite);

        //----------------------------------------------------------------//
        //                                                                //
        // Log character details.                                         //
        //                                                                //
        //----------------------------------------------------------------//

        ToolSoftFontGenLog.LogCharDetails(_tableLog,
                                           false,
                                           glyphComposite,
                                           charCode,
                                           codepoint,
                                           glyphId,
                                           depth,
                                           glyphWidth,
                                           glyphHeight,
                                           glyphLSB,
                                           glyphTSB,
                                           glyphOffset,
                                           glyphLength);

        //----------------------------------------------------------------//
        //                                                                //
        // Calculate total size of header.                                //
        //                                                                //
        // Write PCL 'Character Code' escape sequence.                    //
        // Write PCL 'Character Definition' escape sequence.              //
        //                                                                //
        //----------------------------------------------------------------//

        charBlockSize = (ushort)(cSizeCharHddr + cSizeCharGlyphHddr +
                                  glyphLength + cSizeCharTrail);

        PCLWriter.CharDownloadCode(_binWriter, charCode);

        PCLWriter.CharDownloadDesc(_binWriter, charBlockSize);

        //----------------------------------------------------------------//
        //                                                                //
        // Write Format 15 header.                                        //
        // This character format is used with both Format 15 and          //
        // Format 16 font headers.                                        //
        //                                                                //
        //----------------------------------------------------------------//

        charHddr[0] = 15;                  // Format = 15
        charHddr[1] = 0;                   // Continuation = false
        charHddr[2] = 2;                   // Descriptor size
        charHddr[3] = 15;                  // Class = 15

        _baseHandler.WriteBuffer(cSizeCharHddr, charHddr);

        //----------------------------------------------------------------//
        //                                                                //
        // Write glyph header.                                            //
        // This counts towards the checksum recorded in the trailer.      //
        //                                                                //
        //----------------------------------------------------------------//

        checksumMod256 = 0;

        charDataSize = (ushort)(cSizeCharGlyphHddr + glyphLength);

        charGlyphHddr[0] = MsByte(charDataSize);
        charGlyphHddr[1] = LsByte(charDataSize);
        charGlyphHddr[2] = MsByte(glyphId);
        charGlyphHddr[3] = LsByte(glyphId);

        _baseHandler.WriteCharFragment(cSizeCharGlyphHddr,
                                        charGlyphHddr,
                                        ref checksumMod256);

        //----------------------------------------------------------------//
        //                                                                //
        // Write TrueType glyph data (copied from TrueType font file).    //
        // The data is read into a dynamically allocated buffer because:  //
        //    -  This avoids the complication of having a fixed-length    //
        //       buffer and a loop to read the data in chunks.            //
        //    -  Not having a static buffer allows the function to be     //
        //       called recursively.                                      //
        //                                                                //
        //----------------------------------------------------------------//

        if (glyphLength > 0)
        {
            bool flagOK = true;

            glyphData = new byte[glyphLength];

            flagOK = _ttfHandler.ReadByteArray((int)glyphOffset,
                                                (int)glyphLength,
                                                ref glyphData);
            // TODO: what if flagOK = true (i.e. read fails?

            _baseHandler.WriteCharFragment((int)glyphLength,
                                            glyphData,
                                            ref checksumMod256);
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Write trailer (Reserved byte and Checksum byte).               //
        //                                                                //
        //----------------------------------------------------------------//

        checksumMod256 = (byte)((256 - checksumMod256) % 256);

        charTrail[0] = 0;                  // Reserved byte
        charTrail[1] = checksumMod256;     // Checksum byte

        _baseHandler.WriteBuffer(cSizeCharTrail, charTrail);

        //----------------------------------------------------------------//
        //                                                                //
        // Handler composite glyphs.                                      //
        //                                                                //
        //----------------------------------------------------------------//

        if (glyphComposite)
        {
            // if we move this to TTFHandler, do the maxGlyphId check there instead

            int indBuf;

            ushort glyphCompFlags,
                   glyphCompId;

            indBuf = 10; // point to first set of component data //

            do
            {
                glyphCompFlags = (ushort)((glyphData[indBuf] << 8) +
                                            glyphData[indBuf + 1]);
                glyphCompId = (ushort)((glyphData[indBuf + 2] << 8) +
                                            glyphData[indBuf + 3]);

                if (glyphCompId > maxGlyphId)
                {
                    // flagOK = false;

                    ToolSoftFontGenLog.LogError(
                        _tableLog, MessageBoxImage.Error,
                        "Composite glyph identifier " + glyphCompId +
                        " > maximum of " + maxGlyphId);
                }
                else
                {
                    if (_ttfHandler.GlyphReferencedCheck(glyphCompId))
                    {
                        ToolSoftFontGenLog.LogCharDetails(
                            _tableLog,
                            true,
                            _ttfHandler.GlyphCompositeCheck(glyphCompId),
                            0,
                            0,
                            glyphCompId,
                            depth,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0);
                    }
                    else
                    {
                        // flagOK = 
                        WriteChar(0xffff, 0, glyphCompId,
                                   (ushort)(depth + 1), maxGlyphId);
                    }
                }

                // if flagOK
                {
                    indBuf += 4;

                    if ((glyphCompFlags &
                        ToolSoftFontGenTTF.mask_glyf_compFlag_ARG_1_AND_2_ARE_WORDS) != 0)
                        indBuf += 4;
                    else
                        indBuf += 2;

                    if ((glyphCompFlags &
                        ToolSoftFontGenTTF.mask_glyf_compFlag_WE_HAVE_A_TWO_BY_TWO) != 0)
                        indBuf += 8;
                    else if ((glyphCompFlags &
                        ToolSoftFontGenTTF.mask_glyf_compFlag_WE_HAVE_AN_X_AND_Y_SCALE) != 0)
                        indBuf += 4;
                    else if ((glyphCompFlags &
                        ToolSoftFontGenTTF.mask_glyf_compFlag_WE_HAVE_A_SCALE) != 0)
                        indBuf += 2;
                }
            } while ((glyphCompFlags &
                        ToolSoftFontGenTTF.mask_glyf_compFlag_MORE_COMPONENTS) != 0);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // w r i t e C h a r S e t                                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write PCL format characters for the set of characters in the       //
    // selected symbol set.                                               //
    //                                                                    //
    // For each character, write Character Code and Character Definition  //
    // sequences.                                                         //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void WriteCharSet(ushort maxGlyphId,
                               int sizeCharSet,
                               bool symSetUnbound)
    {
        bool glyphExists = false;

        ushort startCode,
               endCode,
               glyphId = 0,
               codepoint = 0;

        //----------------------------------------------------------------//
        //                                                                //
        // Download individual character glyphs.                          //
        //                                                                //
        //----------------------------------------------------------------//

        startCode = 0;
        endCode = (ushort)(sizeCharSet - 1);

        for (int i = startCode; i <= endCode; i++)
        {
            ushort charCode = (ushort)i;

            glyphExists = _ttfHandler.GetCharData(charCode,
                                                   ref codepoint,
                                                   ref glyphId);

            if (glyphExists)
            {
                WriteChar(charCode, codepoint, glyphId, 0, maxGlyphId);
            }
            else if (!symSetUnbound)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Check whether or not all (graphic) characters in the   //
                // target character set have glyphs which are present.    //
                // Report any that don't.                                 //
                //                                                        //
                // For standard 'Unicode' encoded (usually text) fonts,   //
                // ignore characters for which the target code-point has  //
                // been set to 0xffff (indicating character not present)  //
                // and perhaps those with target code-points in the       //
                // 'control code' ranges (0x00->0x1f and 0x7f->0x9f).     //
                //                                                        //
                // For 'Symbol' encoded fonts, perhaps ignore characters  //
                // with target code-points in the 'control code' ranges   //
                // (0xf000->0xf01f and 0xf07f->0xf09f).                   //
                //                                                        //
                //--------------------------------------------------------//

                if (codepoint == 0xffff)
                {
                    // do nothing - character-code not mapped
                }
                /*
                else if ((!_symbolMapping) &&
                         (codepoint < 0x20) ||
                         ((codepoint >= 0x7f) && (codepoint <= 0x9f)))
                {
                    // do nothing //
                }
                else if ((_symbolMapping) &&
                         (codepoint < 0xf020) ||
                         ((codepoint >= 0xf07f) && (codepoint <= 0xf09f)))
                {
                    // do nothing //
                }
                */
                else
                {
                    ToolSoftFontGenLog.LogMissingChar(
                        _tableLog,
                        charCode,
                        codepoint);
                }
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // w r i t e H d d r                                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Generate font header descriptor, segmented data and checksum byte. //
    //                                                                    //
    //--------------------------------------------------------------------//

    private bool WriteHddr(ref bool monoSpaced,
                               bool fmt16,
                               bool segGTLast,
                               bool usePCLT,
                               bool glyphZeroExists,
                               bool symSetUnbound,
                               bool tabvmtxPresent,
                               bool flagVMetrics,
                               byte symSetType,
                               ushort firstCode,
                               ushort lastCode,
                               ushort numChars,
                               ushort unitsPerEm,
                               ushort symSet,
                               ushort style,
                               sbyte strokeWeight,
                               ushort typeface,
                               ulong charCollComp,
                               byte[] conversionText)
    {
        bool flagOK = true;

        ushort cellWidth = 0,
               cellHeight = 0,
               textWidth = 0,
               textHeight = 0,
               pitch = 0,
               xHeight = 0,
               capHeight = 0,
               mUlinePosU = 0,
               mUlineDep = 0;

        short mUlinePos = 0;

        uint fontNo = 0;

        int sum;
        int convTextLen;
        int hddrLen;

        byte mod256;
        byte serifStyle = 0;
        byte fontFormat;
        byte fontType;
        byte fontSpacing;

        sbyte widthType = 0;

        byte[] fontNamePCLT = new byte[ToolSoftFontGenTTF.cSizeFontname];
        byte[] panoseData = new byte[ToolSoftFontGenTTF.cSizePanose];
        byte[] hddrDesc = new byte[cSizeHddrDesc];

        //----------------------------------------------------------------//
        //                                                                //
        // Get relevant PCL data from TrueType font.                      //
        //                                                                //
        //----------------------------------------------------------------//

        monoSpaced = false;

        _ttfHandler.GetPCLFontHeaderData(usePCLT,
                                          ref monoSpaced,
                                          ref cellWidth,
                                          ref cellHeight,
                                          ref textWidth,
                                          ref textHeight,
                                          ref pitch,
                                          ref xHeight,
                                          ref capHeight,
                                          ref mUlinePos,
                                          ref mUlineDep,
                                          ref fontNo,
                                          ref serifStyle,
                                          ref widthType,
                                          ref fontNamePCLT,
                                          ref panoseData);

        mUlinePosU = (ushort)mUlinePos;

        //----------------------------------------------------------------//

        if (fmt16)
            fontFormat = 16;            // Format = Universal
        else
            fontFormat = 15;            // Format = TrueType scalable

        if (monoSpaced)
            fontSpacing = 0;            // Spacing = Fixed-pitch
        else
            fontSpacing = 1;            // Spacing = Proportional

        if (symSetUnbound)
        {
            fontType = 11;              // Type = unbound Unicode-indexed
            firstCode = 0;
            lastCode = numChars;
        }
        else
        {
            fontType = symSetType;      // Type = as per target symbol set
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Calculate total size of header.                                //
        // Write PCL 'download header' escape sequence.                   //
        //                                                                //
        //----------------------------------------------------------------//

        convTextLen = conversionText.Length;

        hddrLen = cSizeHddrDesc +
                  _baseHandler.GetHddrSegmentsLen(
                        false,
                        fmt16,
                        glyphZeroExists,
                        symSetUnbound,
                        tabvmtxPresent,
                        flagVMetrics,
                        convTextLen) +
                  cSizeHddrTrail;

        if ((hddrLen > cSizeHddrFmt15Max) && (!fmt16))
        {
            flagOK = false;

            MessageBox.Show("Header length of '" + hddrLen +
                             "' is incompatible with 'format 15'" +
                             " font.",
                             "Soft font header invalid",
                             MessageBoxButton.OK,
                             MessageBoxImage.Error);
        }
        else
        {

            PCLWriter.FontDownloadHddr(_binWriter, (uint)hddrLen);

            //------------------------------------------------------------//
            //                                                            //
            // Write font header descriptor.                              //
            //                                                            //
            //------------------------------------------------------------//

            hddrDesc[0] = MsByte(cSizeHddrDesc);
            hddrDesc[1] = LsByte(cSizeHddrDesc);

            hddrDesc[2] = fontFormat;          // Font Format
            hddrDesc[3] = fontType;            // Font Type
            hddrDesc[4] = MsByte(style);      // Style MSB
            hddrDesc[5] = 0;                   // Reserved
            hddrDesc[6] = 0;                   // Baseline Position MSB
            hddrDesc[7] = 0;                   // Baseline Position LSB
            hddrDesc[8] = MsByte(cellWidth);  // Cell width MSB
            hddrDesc[9] = LsByte(cellWidth);  // Cell Width LSB
            hddrDesc[10] = MsByte(cellHeight); // Cell Height MSB
            hddrDesc[11] = LsByte(cellHeight); // Cell Height LSB
            hddrDesc[12] = 0;                   // Orientation
            hddrDesc[13] = fontSpacing;         // Spacing
            hddrDesc[14] = MsByte(symSet);     // Symbol Set MSB
            hddrDesc[15] = LsByte(symSet);     // Symbol Set LSB
            hddrDesc[16] = MsByte(pitch);      // Pitch MSB
            hddrDesc[17] = LsByte(pitch);      // Pitch LSB
            hddrDesc[18] = 0;                   // Height MSB
            hddrDesc[19] = 0;                   // Height LSB
            hddrDesc[20] = MsByte(xHeight);    // xHeight MSB
            hddrDesc[21] = MsByte(xHeight);    // xHeight LSB
            hddrDesc[22] = (byte)widthType;    // Width Type
            hddrDesc[23] = LsByte(style);      // Style LSB
            hddrDesc[24] = (byte)strokeWeight; // Stroke Weight
            hddrDesc[25] = LsByte(typeface);   // Typeface LSB
            hddrDesc[26] = MsByte(typeface);   // Typeface MSB
            hddrDesc[27] = serifStyle;          // Serif Style
            hddrDesc[28] = 2;                   // Quality = Letter
            hddrDesc[29] = 0;                   // Placement
            hddrDesc[30] = 0;                   // Underline Position
            hddrDesc[31] = 0;                   // Underline Thickness
            hddrDesc[32] = MsByte(textHeight); // Text Height MSB
            hddrDesc[33] = LsByte(textHeight); // Text Height LSB
            hddrDesc[34] = MsByte(textWidth);  // Text Width MSB
            hddrDesc[35] = LsByte(textWidth);  // Text Width LSB
            hddrDesc[36] = MsByte(firstCode);  // First Code MSB
            hddrDesc[37] = LsByte(firstCode);  // First Code LSB
            hddrDesc[38] = MsByte(lastCode);   // Last Code MSB
            hddrDesc[39] = LsByte(lastCode);   // Last Code LSB
            hddrDesc[40] = 0;                   // Pitch Extended
            hddrDesc[41] = 0;                   // Height Extended
            hddrDesc[42] = MsByte(capHeight);  // Cap Height MSB
            hddrDesc[43] = LsByte(capHeight);  // Cap Height LSB
            hddrDesc[44] = MsByte(MsUInt16(fontNo));  // Font No. byte 0
            hddrDesc[45] = LsByte(MsUInt16(fontNo));  // Font No. byte 1
            hddrDesc[46] = MsByte(LsUInt16(fontNo));  // Font No. byte 2
            hddrDesc[47] = LsByte(LsUInt16(fontNo));  // Font No. byte 3
            hddrDesc[48] = fontNamePCLT[0];     // Font Name byte 0
            hddrDesc[49] = fontNamePCLT[1];     // Font Name byte 1
            hddrDesc[50] = fontNamePCLT[2];     // Font Name byte 2
            hddrDesc[51] = fontNamePCLT[3];     // Font Name byte 3
            hddrDesc[52] = fontNamePCLT[4];     // Font Name byte 4
            hddrDesc[53] = fontNamePCLT[5];     // Font Name byte 5
            hddrDesc[54] = fontNamePCLT[6];     // Font Name byte 6
            hddrDesc[55] = fontNamePCLT[7];     // Font Name byte 7
            hddrDesc[56] = fontNamePCLT[8];     // Font Name byte 8
            hddrDesc[57] = fontNamePCLT[9];     // Font Name byte 9
            hddrDesc[58] = fontNamePCLT[10];    // Font Name byte 10
            hddrDesc[59] = fontNamePCLT[11];    // Font Name byte 11
            hddrDesc[60] = fontNamePCLT[12];    // Font Name byte 12
            hddrDesc[61] = fontNamePCLT[13];    // Font Name byte 13
            hddrDesc[62] = fontNamePCLT[14];    // Font Name byte 14
            hddrDesc[63] = fontNamePCLT[15];    // Font Name byte 15
            hddrDesc[64] = MsByte(unitsPerEm); // Scale Factor MSB
            hddrDesc[65] = LsByte(unitsPerEm); // Scale Factor LSB
            hddrDesc[66] = MsByte(mUlinePosU); // Master U-line Pos. MSB
            hddrDesc[67] = LsByte(mUlinePosU); // Master U-line Pos. LSB
            hddrDesc[68] = MsByte(mUlineDep);  // Master U-line Dep. MSB
            hddrDesc[69] = LsByte(mUlineDep);  // Master U-line Dep. LSB
            hddrDesc[70] = 1;                   // Scaling Tech. = TrueType
            hddrDesc[71] = 0;                   // Variety

            _baseHandler.WriteBuffer(cSizeHddrDesc, hddrDesc);

            //------------------------------------------------------------//
            //                                                            //
            // Start calculating checksum byte from byte 64 onwards of    //
            // header.                                                    //
            //                                                            //
            //------------------------------------------------------------//

            sum = 0;

            for (int i = 64; i < cSizeHddrDesc; i++)
            {
                sum += hddrDesc[i];
            }

            mod256 = (byte)(sum % 256);

            //------------------------------------------------------------//
            //                                                            //
            // Write header segmented data.                               //
            //                                                            //
            //------------------------------------------------------------//

            flagOK = _baseHandler.WriteHddrSegments(false,
                                                     fmt16,
                                                     segGTLast,
                                                     glyphZeroExists,
                                                     symSetUnbound,
                                                     tabvmtxPresent,
                                                     flagVMetrics,
                                                     charCollComp,
                                                     conversionText,
                                                     panoseData,
                                                     ref mod256);

            if (flagOK)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Write 'reserved byte' and (calculated) checksum byte.  //
                //                                                        //
                //--------------------------------------------------------//

                mod256 = (byte)((256 - mod256) % 256);

                byte[] trailer = new byte[cSizeHddrTrail];

                trailer[0] = 0;
                trailer[1] = mod256;

                _baseHandler.WriteBuffer(cSizeHddrTrail, trailer);
            }
        }

        return flagOK;
    }
}
