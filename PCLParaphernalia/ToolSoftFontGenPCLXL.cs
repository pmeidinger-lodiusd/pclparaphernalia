﻿using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides PCL XL handling for the Soft Font Generate tool.
    /// 
    /// © Chris Hutchinson 2012
    /// 
    /// </summary>

    class ToolSoftFontGenPCLXL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const int cSizeFontname = 16;
        const int cSizeHddrDesc = 8;
        const int cSizeCharHddrClass0 = 6;
        const int cSizeCharHddrClass1 = 10;
        const int cSizeCharHddrClass2 = 12;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly ToolSoftFontGenPCLCommon _baseHandler;

        private readonly ASCIIEncoding _ascii = new ASCIIEncoding();

        private Stream _opStream = null;
        private BinaryWriter _binWriter = null;

        private readonly ToolSoftFontGenTTF _ttfHandler = null;

        private readonly DataTable _tableLog;

        private bool _symbolMapping = false;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T o o l S o f t G e n P C L X L                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolSoftFontGenPCLXL(DataTable tableLog,
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
                                     bool symbolMapping,
                                     bool symSetUnbound,
                                     bool tabvmtxPresent,
                                     bool flagVMetrics,
                                     byte[] fontName,
                                     int sizeCharSet,
                                     ushort symSet,
                                     byte[] conversionText)
        {
            bool flagOK = true;

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
                                         true,
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

                int len,
                      charClass;

                ushort numChars = 0,
                       firstCode = 0,
                       lastCode = 0,
                       maxGlyphId = 0,
                       maxComponentDepth = 0,
                       unitsPerEm = 0;

                bool glyphZeroExists = false;

                byte[] fontNameXL = new byte[cSizeFontname];

                _ttfHandler.GlyphReferencedUnmarkAll();

                _ttfHandler.GetBasicMetrics(ref numChars,
                                             ref firstCode,
                                             ref lastCode,
                                             ref maxGlyphId,
                                             ref maxComponentDepth,
                                             ref unitsPerEm,
                                             ref glyphZeroExists);

                len = fontName.Length;

                if (len > cSizeFontname)
                    len = cSizeFontname;

                for (int j = 0; j < len; j++)
                {
                    fontNameXL[j] = fontName[j];
                }

                for (int j = len; j < cSizeFontname; j++)
                {
                    fontNameXL[j] = 0x20;
                }

                try
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Write font header.                                     //
                    //                                                        //
                    //--------------------------------------------------------//

                    WriteHddr(glyphZeroExists,
                               symSetUnbound,
                               tabvmtxPresent,
                               flagVMetrics,
                               numChars,
                               symSet,
                               fontNameXL,
                               conversionText);

                    //--------------------------------------------------------//
                    //                                                        //
                    // Write BeginChar operator and associated Attribute List.//
                    //                                                        //
                    //--------------------------------------------------------//

                    PCLXLWriter.FontCharBegin(_binWriter,
                                               false,
                                               cSizeFontname,
                                               fontNameXL);

                    //--------------------------------------------------------//
                    //                                                        //
                    // Write Galley Character and font characters.            //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (symSetUnbound)
                        charClass = 0;
                    else if ((tabvmtxPresent) && (flagVMetrics))
                        charClass = 2;
                    else
                        charClass = 1;

                    if (glyphZeroExists)
                        WriteChar(charClass, 0xffff, 0, 0, 0, maxGlyphId);

                    WriteCharSet(maxGlyphId, sizeCharSet, charClass,
                                  symSetUnbound);

                    //--------------------------------------------------------//
                    //                                                        //
                    // Write EndChar operator.                                //
                    //                                                        //
                    //--------------------------------------------------------//

                    PCLXLWriter.FontCharEnd(_binWriter, false);

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
        // Write PCL XL format character:                                     //
        //                                                                    //
        //  ReadChar operator                                                 //
        //  Embedded Data header                                              //
        //  character data                                                    //
        //                                                                    //
        // Note that the function may be called recursively, if a glyph is    //
        // composite (i.e. made up of two or more components).                //
        //                                                                    //
        // Unbound fonts: write Class 0 characters; these rely on the GT      //
        //                              GT segment in the font header         //
        //                              containing the 'hhea' and 'htmx'      //
        //                              tables.                               //
        // Bound fonts:   write Class 1 characters, which include advance     //
        //                              width and LSB in each character       //
        //                              header.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void WriteChar(int charClass,
                                ushort charCode,
                                ushort codepoint,
                                ushort glyphId,
                                ushort depth,
                                ushort maxGlyphId)
        {
            ushort glyphWidth = 0,
                   glyphHeight = 0,
                   charDataSize = 0,
                   charSize,
                   hddrSize;

            short glyphLSB = 0,
                  glyphTSB = 0;

            uint glyphOffset = 0,
                   glyphLength = 0;

            bool glyphComposite = false;

            byte checksumMod256 = 0;

            byte[] glyphData = null;

            byte[] charHddr;

            if (charClass == 1)
                hddrSize = cSizeCharHddrClass1;
            else if (charClass == 2)
                hddrSize = cSizeCharHddrClass2;
            else
                hddrSize = cSizeCharHddrClass0;

            charHddr = new byte[hddrSize];

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
                                      ref glyphHeight,  // not used
                                      ref glyphLSB,
                                      ref glyphTSB,
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
            // Write ReadChar operator and associated Attribute List.         //
            // Write Embedded Data Introduction sequence.                     //
            //                                                                //
            //----------------------------------------------------------------//

            charDataSize = (ushort)(hddrSize + glyphLength);
            charSize = (ushort)(charDataSize - 2);

            PCLXLWriter.FontCharRead(_binWriter, false,
                                      charCode, charDataSize);

            PCLXLWriter.EmbedDataIntro(_binWriter,
                                        false,
                                        charDataSize);
            if (charClass == 0)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Write Format 1 Class 0 header.                             //
                //                                                            //
                //------------------------------------------------------------//

                charHddr[0] = 1;                   // Format
                charHddr[1] = 0;                   // Class
                charHddr[2] = MsByte(charSize);   // CharSize MSB
                charHddr[3] = LsByte(charSize);   // CharSize LSB
                charHddr[4] = MsByte(glyphId);    // Glyph Id MSB
                charHddr[5] = LsByte(glyphId);    // Glyph Id LSB
            }
            else if (charClass == 1)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Write Format 1 Class 1 header.                             //
                //                                                            //
                //------------------------------------------------------------//

                charHddr[0] = 1;                   // Format
                charHddr[1] = 1;                   // Class
                charHddr[2] = MsByte(charSize);   // CharSize MSB
                charHddr[3] = LsByte(charSize);   // CharSize LSB
                charHddr[4] = MsByte((ushort)glyphLSB);// Glyph Left Side Bearing
                charHddr[5] = LsByte((ushort)glyphLSB);// Glyph Left Side Bearing
                charHddr[6] = MsByte(glyphWidth); // Glyph Width MSB
                charHddr[7] = LsByte(glyphWidth); // Glyph Width LSB
                charHddr[8] = MsByte(glyphId);    // Glyph Id MSB
                charHddr[9] = LsByte(glyphId);    // Glyph Id LSB
            }
            else // charClass == 2
            {
                //------------------------------------------------------------//
                //                                                            //
                // Write Format 1 Class 2 header.                             //
                //                                                            //
                //------------------------------------------------------------//

                charHddr[0] = 1;                   // Format
                charHddr[1] = 2;                   // Class
                charHddr[2] = MsByte(charSize);   // CharSize MSB
                charHddr[3] = LsByte(charSize);   // CharSize LSB
                charHddr[4] = MsByte((ushort)glyphLSB);// Glyph Left Side Bearing
                charHddr[5] = LsByte((ushort)glyphLSB);// Glyph Left Side Bearing
                charHddr[6] = MsByte(glyphWidth); // Glyph Width MSB
                charHddr[7] = LsByte(glyphWidth); // Glyph Width LSB
                charHddr[8] = MsByte((ushort)glyphTSB);// Glyph Top Side Bearing
                charHddr[9] = LsByte((ushort)glyphTSB);// Glyph Top Side Bearing
                charHddr[10] = MsByte(glyphId);    // Glyph Id MSB
                charHddr[11] = LsByte(glyphId);    // Glyph Id LSB
            }

            _baseHandler.WriteBuffer(hddrSize, charHddr);

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
                            WriteChar(charClass, 0xffff, 0, glyphCompId,
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
        // Write PCL XL format characters for the set of characters in the    //
        // selected symbol set.                                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void WriteCharSet(ushort maxGlyphId,
                                   int sizeCharSet,
                                   int charClass,
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
                    WriteChar(charClass, charCode, codepoint,
                               glyphId, 0, maxGlyphId);
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

        private void WriteHddr(bool glyphZeroExists,
                                bool symSetUnbound,
                                bool tabvmtxPresent,
                                bool flagVMetrics,
                                ushort numChars,
                                ushort symSet,
                                byte[] fontNameXL,
                                byte[] conversionText)
        {
            byte mod256 = 0;

            byte[] hddrDesc = new byte[cSizeHddrDesc];
            byte[] panoseData = new byte[ToolSoftFontGenTTF.cSizePanose];

            //----------------------------------------------------------------//
            //                                                                //
            // Write BeginFontHeader Operator and associated Attribute List.  //
            //                                                                //
            //----------------------------------------------------------------//

            PCLXLWriter.FontHddrBegin(_binWriter,
                                       false,
                                       cSizeFontname,
                                       fontNameXL,
                                       0);

            //----------------------------------------------------------------//
            //                                                                //
            // Write Format 0 font header.                                    //
            //                                                                //
            //----------------------------------------------------------------//

            hddrDesc[0] = 0;                    // Format
            hddrDesc[1] = (byte)PCLXLAttrEnums.eVal.ePortraitOrientation;
            hddrDesc[2] = MsByte(symSet);      // Symbol set MSB
            hddrDesc[3] = LsByte(symSet);      // Symbol Set LSB
            hddrDesc[4] = 1;                    // Scaling = TrueType
            hddrDesc[5] = 0;                    // Variety
            hddrDesc[6] = MsByte(numChars);    // NumChars MSB
            hddrDesc[7] = LsByte(numChars);    // NumChars LSB

            _baseHandler.WriteHddrFragment(true,
                                           cSizeHddrDesc,
                                           hddrDesc,
                                           ref mod256);

            //----------------------------------------------------------------//
            //                                                                //
            // Write segmented data.                                          //
            //                                                                //
            //----------------------------------------------------------------//

            mod256 = 0; // not actually required for PCL XL

            panoseData = _ttfHandler.PanoseData;

            _baseHandler.WriteHddrSegments(true,
                                            true,
                                            false,
                                            glyphZeroExists,
                                            symSetUnbound,
                                            tabvmtxPresent,
                                            flagVMetrics,
                                            0,         // not used for PCL XL //
                                            conversionText,
                                            panoseData,
                                            ref mod256);

            //----------------------------------------------------------------//
            //                                                                //
            // Write BeginFontHeader Operator and associated Attribute List.  //
            //                                                                //
            //----------------------------------------------------------------//

            PCLXLWriter.FontHddrEnd(_binWriter,
                                     false);
        }
    }
}
