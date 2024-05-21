﻿using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides TTF handling for the Soft Font Generate tool.
    /// 
    /// © Chris Hutchinson 2011
    /// 
    /// </summary>
    class ToolSoftFontGenTTF
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const uint cTabID_cmap  = 0x636D6170;
        const uint cTabID_head  = 0x68656164;
        const uint cTabID_hhea  = 0x68686561;
        const uint cTabID_hmtx  = 0x686D7478;
        const uint cTabID_maxp  = 0x6D617870;
        const uint cTabID_name  = 0x6E616D65;
        const uint cTabID_OS_2  = 0x4F532F32;
        const uint cTabID_post  = 0x706F7374;
        const uint cTabID_cvt   = 0x63767420;
        const uint cTabID_fpgm  = 0x6670676D;
        const uint cTabID_gdir  = 0x67646972;
        const uint cTabID_glyf  = 0x676C7966;
        const uint cTabID_loca  = 0x6C6F6361;
        const uint cTabID_prep  = 0x70726570;
        const uint cTabID_ttcf  = 0x74746366;
        const uint cTabID_CFF   = 0x43464620;
        const uint cTabID_VORG  = 0x564F5247;
        const uint cTabID_EBDT  = 0x45424454;
        const uint cTabID_EBLC  = 0x45424C43;
        const uint cTabID_EBSC  = 0x45425343;
        const uint cTabID_BASE  = 0x42415345;
        const uint cTabID_GDEF  = 0x47444546;
        const uint cTabID_GPOS  = 0x47504F53;
        const uint cTabID_GSUB  = 0x47535542;
        const uint cTabID_JSTF  = 0x4A535446;
        const uint cTabID_DSIG  = 0x44534947;
        const uint cTabID_gasp  = 0x67617370;
        const uint cTabID_hdmx  = 0x68646D78;
        const uint cTabID_kern  = 0x6B65726E;
        const uint cTabID_LTSH  = 0x4C545348;
        const uint cTabID_PCLT  = 0x50434C54;
        const uint cTabID_VDMX  = 0x56444D58;
        const uint cTabID_vhea  = 0x76686561;
        const uint cTabID_vmtx  = 0x766D7478;

        const uint cTabVer_sfnt       = 0x00010000;
        const uint cTabVer_head       = 0x00010000;
        const uint cTabVer_hhea       = 0x00010000;
        const uint cTabVer_maxp       = 0x00010000;
        const uint cTabVer_PCLT       = 0x00010000;
        const uint cTabVer_ttcf_1     = 0x00010000;
        const uint cTabVer_ttcf_2     = 0x00020000;
        const uint cTabVer_vhea_1_0   = 0x00010000;
        const uint cTabVer_vhea_1_1   = 0x00011000;

        const ushort cTabVer_OS_2_3 = 0x0003;
        const ushort cTabVer_OS_2_4 = 0x0004;

        const ushort mask_OS_2_fsSelection_ITALIC           = 0x0001;
        const ushort mask_OS_2_fsSelection_UNDERSCORE       = 0x0002;
        const ushort mask_OS_2_fsSelection_NEGATIVE         = 0x0004;
        const ushort mask_OS_2_fsSelection_OUTLINED         = 0x0008;
        const ushort mask_OS_2_fsSelection_STRIKEOUT        = 0x0010;
        const ushort mask_OS_2_fsSelection_BOLD             = 0x0020;
        const ushort mask_OS_2_fsSelection_REGULAR          = 0x0040;
        const ushort mask_OS_2_fsSelection_USE_TYPO_METRICS = 0x0080;
        const ushort mask_OS_2_fsSelection_WWS              = 0x0100;
        const ushort mask_OS_2_fsSelection_OBLIQUE          = 0x0200;
        const ushort mask_OS_2_fsSelection_Reserved         = 0xFC00;

        const ushort mask_OS_2_fsType_INSTALLABLE_EMBED        = 0x0000;
        const ushort mask_OS_2_fsType_Reserved_A               = 0x0001;
        const ushort mask_OS_2_fsType_RESTRICTED_LICENSE_EMBED = 0x0002;
        const ushort mask_OS_2_fsType_PREVIEW_AND_PRINT_EMBED  = 0x0004;
        const ushort mask_OS_2_fsType_EDITABLE_EMBED           = 0x0008;
        const ushort mask_OS_2_fsType_Reserved_B               = 0x00F0;
        const ushort mask_OS_2_fsType_NO_SUBSETTING            = 0x0100;
        const ushort mask_OS_2_fsType_BITMAP_EMBED_ONLY        = 0x0200;
        const ushort mask_OS_2_fsType_Reserved_C               = 0xFC00;

        const ushort mask_OS_2_usWidthClass_FWIDTH_ULTRA_CONDENSED = 1;
        const ushort mask_OS_2_usWidthClass_FWIDTH_EXTRA_CONDENSED = 2;
        const ushort mask_OS_2_usWidthClass_FWIDTH_CONDENSED       = 3;
        const ushort mask_OS_2_usWidthClass_FWIDTH_SEMI_CONDENSED  = 4;
        const ushort mask_OS_2_usWidthClass_FWIDTH_NORMAL          = 5;
        const ushort mask_OS_2_usWidthClass_FWIDTH_SEMI_EXPANDED   = 6;
        const ushort mask_OS_2_usWidthClass_FWIDTH_EXPANDED        = 7;
        const ushort mask_OS_2_usWidthClass_FWIDTH_EXTRA_EXPANDED  = 8;
        const ushort mask_OS_2_usWidthClass_FWIDTH_ULTRA_EXPANDED  = 9;

        const ushort mask_OS_2_usWeightClass_FW_THIN       = 100;
        const ushort mask_OS_2_usWeightClass_FW_EXTRALIGHT = 200;
        const ushort mask_OS_2_usWeightClass_FW_LIGHT      = 300;
        const ushort mask_OS_2_usWeightClass_FW_NORMAL     = 400;
        const ushort mask_OS_2_usWeightClass_FW_MEDIUM     = 500;
        const ushort mask_OS_2_usWeightClass_FW_SEMIBOLD   = 600;
        const ushort mask_OS_2_usWeightClass_FW_BOLD       = 700;
        const ushort mask_OS_2_usWeightClass_FW_EXTRABOLD  = 800;
        const ushort mask_OS_2_usWeightClass_FW_BLACK      = 900;

        const byte cSpaceCodePoint                       = 0x20;

        const byte cPanoseFamilyAny                      = 0;
        const byte cPanoseFamilyNoFit                    = 1;
        const byte cPanoseFamilyLatinText                = 2;
        const byte cPanoseFamilyLatinHandwritten         = 3;
        const byte cPanoseFamilyLatinDecorative          = 4;
        const byte cPanoseFamilyLatinSymbol              = 5;
    
        const byte cPanoseMonoLatinText                  = 9;
        const byte cPanoseMonoLatinHandwritten           = 3;
        const byte cPanoseMonoLatinDecorative            = 9;
        const byte cPanoseMonoLatinSymbol                = 3;

        public const int cSizePanose                   = 10;
        public const int cSizeFontname                 = 16;

        public const ushort mask_glyf_compFlag_ARG_1_AND_2_ARE_WORDS    = 0x0001;
        public const ushort mask_glyf_compFlag_WE_HAVE_A_SCALE          = 0x0008;
        public const ushort mask_glyf_compFlag_MORE_COMPONENTS          = 0x0020;
        public const ushort mask_glyf_compFlag_WE_HAVE_AN_X_AND_Y_SCALE = 0x0040;
        public const ushort mask_glyf_compFlag_WE_HAVE_A_TWO_BY_TWO     = 0x0080;

        public enum eLicenceType
        {
            Allowed,
            NotAllowed,
            OwnerOnly
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int _sizeCharSet;

        private readonly bool _logVerbose = true;

        private Stream _ipStream = null;
        private BinaryReader _binReader = null;

        private ushort[] _mappingTable;

        private readonly DataTable _tableDonor;
        private readonly DataTable _tableMapping;

        private long _fontFileSize = 0;
        
        private ushort _cmap_numChars = 0;
        private ushort _cmap_missChars = 0;
        private ushort _cmap_firstCode = 0;
        private ushort _cmap_lastCode = 0;

        private ushort _head_unitsPerEm = 0;
        private short _head_xMin = 0;
        private short _head_xMax = 0;
        private short _head_yMin = 0;
        private short _head_yMax = 0;
        private short _head_indxLocFmt = 0;

        private ushort _hhea_numHMetrics = 0;
        private short _hhea_ascender = 0;
        private short _hhea_descender = 0;
        private short _hhea_lineGap = 0;

        private ushort _vhea_numVMetrics = 0;
        
        private ushort _maxp_numGlyphs = 0;
        private ushort _maxp_maxCompDepth = 0;
        
        private short _OS_2_xAvgCharWidth = 0;
        private short _OS_2_sxHeight = 0;
        private short _OS_2_sTypoDescender = 0;

        private ushort _OS_2_usWidthClass = 0;
        private ushort _OS_2_usWeightClass = 0;
        private ushort _OS_2_fsType = 0;
        private ushort _OS_2_fsSelection = 0;

        private ushort _PCLT_symSet;
        private ushort _PCLT_typeFamily;
        private ushort _PCLT_style;
        private ushort _PCLT_pitch;
        private ushort _PCLT_xHeight;
        private ushort _PCLT_capHeight;
        private uint _PCLT_fontNo;
        private ulong _PCLT_charComp;

        private byte _PCLT_serifStyle;
        private sbyte _PCLT_widthType;
        private sbyte _PCLT_strokeWeight;

        private uint _post_isFixedPitch = 0;

        private string _name_fullFontnameStr = "";

        private readonly byte[] _name_fullFontname = new byte[cSizeFontname];
        private byte[] _PCLT_typeface = new byte[cSizeFontname];

        private byte[] _OS_2_panose = new byte[cSizePanose];
        
        private bool _glyphZeroExists = false;
        private bool _tabPCLTPresent = false;
        private bool _tabvmtxPresent = false;

        private readonly ToolSoftFontGenTTFTable _tab_OS_2;
        private readonly ToolSoftFontGenTTFTable _tab_PCLT;
        private readonly ToolSoftFontGenTTFTable _tab_cmap;
        private readonly ToolSoftFontGenTTFTable _tab_cvt;
        private readonly ToolSoftFontGenTTFTable _tab_fpgm;
        private readonly ToolSoftFontGenTTFTable _tab_gdir;
        private readonly ToolSoftFontGenTTFTable _tab_glyf;
        private readonly ToolSoftFontGenTTFTable _tab_head;
        private readonly ToolSoftFontGenTTFTable _tab_hhea;
        private readonly ToolSoftFontGenTTFTable _tab_hmtx;
        private readonly ToolSoftFontGenTTFTable _tab_loca;
        private readonly ToolSoftFontGenTTFTable _tab_maxp;
        private readonly ToolSoftFontGenTTFTable _tab_name;
        private readonly ToolSoftFontGenTTFTable _tab_post;
        private readonly ToolSoftFontGenTTFTable _tab_prep;
        private readonly ToolSoftFontGenTTFTable _tab_ttcf;
        private readonly ToolSoftFontGenTTFTable _tab_vhea;
        private readonly ToolSoftFontGenTTFTable _tab_vmtx;

        private GlyphDataEntry[] _glyphData;
        private readonly CharCodeEntry[]  _charData;

        private string _filenameTTF;

        //--------------------------------------------------------------------//
        //                                                  S t r u c t u r e //
        // C h a r C o d e E n t r y                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private struct CharCodeEntry
        {
            ushort _codepoint;     // Unicode codepoint
            ushort _glyphId;
            bool _glyphPresent;

            //----------------------------------------------------------------//

            public CharCodeEntry(ushort codepoint,
                                 ushort glyphId,
                                 bool glyphPresent)
            {
                _codepoint    = codepoint;
                _glyphId      = glyphId;
                _glyphPresent = glyphPresent;
            }

            //----------------------------------------------------------------//

            public bool getGlyphId (ref ushort glyphId)
            {
                glyphId = _glyphId;

                return _glyphPresent;
            }

            //----------------------------------------------------------------//

            public void getValues(ref ushort codepoint,
                                  ref ushort glyphId,
                                  ref bool glyphPresent)
            {
                codepoint    = _codepoint;
                glyphId      = _glyphId;
                glyphPresent = _glyphPresent;
            }

            //----------------------------------------------------------------//

            public bool glyphPresent()
            {
                if (_glyphPresent)
                    return true;
                else
                    return false;
            }

            //----------------------------------------------------------------//

            public void setValues(ushort codepoint,
                                  ushort glyphId,
                                  bool glyphPresent)
            {
                _codepoint    = codepoint;
                _glyphId      = glyphId;
                _glyphPresent = glyphPresent;
            }
        }

        //--------------------------------------------------------------------//
        //                                                  S t r u c t u r e //
        // G l y p h D a t a E n t r y                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private struct GlyphDataEntry
        {
            ushort _advanceWidth,
                   _advanceHeight;

            short _leftSideBearing,
                   _topSideBearing;

            uint _offset,
                   _length;

            bool _composite,
                    _referenced;

            //----------------------------------------------------------------//

            public GlyphDataEntry (ushort advanceWidth,
                                   ushort advanceHeight,
                                   short leftSideBearing,
                                   short topSideBearing,
                                   uint offset,
                                   uint length,
                                   bool composite)
            {
                _advanceWidth    = advanceWidth;
                _advanceHeight   = advanceHeight;
                _leftSideBearing = leftSideBearing;
                _topSideBearing  = topSideBearing;
                _offset          = offset;
                _length          = length;
                _composite       = composite;
                _referenced      = false;
            }

            //----------------------------------------------------------------//

            public bool checkComposite ()
            {
                return _composite;
            }

            //----------------------------------------------------------------//

            public bool checkReferenced ()
            {
                return _referenced;
            }

            //----------------------------------------------------------------//

            public void getAdvance (ref ushort advance)
            {
                advance = _advanceWidth;
            }

            //----------------------------------------------------------------//

            public void getFlags (ref bool composite)
            {
                composite = _composite;
            }

            //----------------------------------------------------------------//

            public void getLocation (ref uint offset,
                                     ref uint length)
            {
                offset = _offset;
                length = _length;
            }

            //----------------------------------------------------------------//

            public void getMetricsH (ref ushort advanceWidth,
                                     ref short leftSideBearing)
            {
                advanceWidth    = _advanceWidth;
                leftSideBearing = _leftSideBearing;
            }

            //----------------------------------------------------------------//

            public void getMetricsV (ref ushort advanceHeight,
                                     ref short topSideBearing)
            {
                advanceHeight  = _advanceHeight;
                topSideBearing = _topSideBearing;
            }

            //----------------------------------------------------------------//

            public void markReferenced ()
            {
                _referenced = true;
            }

            //----------------------------------------------------------------//

            public void setFlags (bool composite)
            {
                _composite = composite;
            }

            //----------------------------------------------------------------//

            public void setLocation (uint offset,
                                     uint length)
            {
                _offset = offset;
                _length = length;
            }

            //----------------------------------------------------------------//

            public void setMetricsH (ushort advanceWidth,
                                     short leftSideBearing)
            {
                _advanceWidth    = advanceWidth;
                _leftSideBearing = leftSideBearing;
            }

            //----------------------------------------------------------------//

            public void setMetricsV (ushort advanceHeight,
                                     short topSideBearing)
            {
                _advanceHeight  = advanceHeight;
                _topSideBearing = topSideBearing;
            }

            //----------------------------------------------------------------//

            public void unmarkReferenced ()
            {
                _referenced = false;
            }
        }
        
        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T o o l S o f t G e n T T F                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolSoftFontGenTTF(DataTable tableDonor,
                                  DataTable tableMapping,
                                  bool logVerbose,
                                  int sizeCharSet)
        {
            _tableDonor   = tableDonor;
            _tableMapping = tableMapping;
            _logVerbose   = logVerbose;

            _tab_OS_2 = new ToolSoftFontGenTTFTable (cTabID_OS_2);
            _tab_PCLT = new ToolSoftFontGenTTFTable (cTabID_PCLT);
            _tab_cmap = new ToolSoftFontGenTTFTable (cTabID_cmap);
            _tab_cvt  = new ToolSoftFontGenTTFTable (cTabID_cvt);
            _tab_fpgm = new ToolSoftFontGenTTFTable (cTabID_fpgm);
            _tab_gdir = new ToolSoftFontGenTTFTable (cTabID_gdir);  // empty //
            _tab_glyf = new ToolSoftFontGenTTFTable (cTabID_glyf);
            _tab_head = new ToolSoftFontGenTTFTable (cTabID_head);
            _tab_hhea = new ToolSoftFontGenTTFTable (cTabID_hhea);
            _tab_hmtx = new ToolSoftFontGenTTFTable (cTabID_hmtx);
            _tab_loca = new ToolSoftFontGenTTFTable (cTabID_loca);
            _tab_maxp = new ToolSoftFontGenTTFTable (cTabID_maxp);
            _tab_name = new ToolSoftFontGenTTFTable (cTabID_name);
            _tab_post = new ToolSoftFontGenTTFTable (cTabID_post);
            _tab_prep = new ToolSoftFontGenTTFTable (cTabID_prep);
            _tab_ttcf = new ToolSoftFontGenTTFTable (cTabID_ttcf);
            _tab_vhea = new ToolSoftFontGenTTFTable (cTabID_vhea);
            _tab_vmtx = new ToolSoftFontGenTTFTable (cTabID_vmtx);
            
            _sizeCharSet = sizeCharSet;

            _charData = new CharCodeEntry[sizeCharSet];

            for (int i = 0; i < sizeCharSet; i++)
            {
                _charData[i].setValues (0, 0, false);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b y t e A r r a y T o I n t 1 6                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns an Int16 value from the first two bytes of the supplied    //
        // byte array, which is assumed to contain a Big-Endian (Motorola)    //
        // format value.                                                      //
        //               [ BitConverter.IsLittleEndian ]                      //
        //--------------------------------------------------------------------//

        private short byteArrayToInt16(byte[] Buf)
        {
            const int sliceSize = 2;

            uint uiSub,
                   uiTot;

            uiTot = 0;

            for (int j = 0; j < sliceSize; j++)
            {
                uiSub = (byte) Buf[j];
                uiTot = (uiTot << 8) | uiSub;
            }

            return (short) uiTot;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b y t e A r r a y T o U I n t 1 6                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns a UInt16 value from the first two bytes of the supplied    //
        // byte array, which is assumed to contain a Big-Endian (Motorola)    //
        // format value.                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private ushort byteArrayToUInt16(byte[] Buf)
        {
            const int sliceSize = 2;

            uint uiSub,
                   uiTot;

            uiTot = 0;

            for (int j = 0; j < sliceSize; j++)
            {
                uiSub = (byte) Buf[j];
                uiTot = (uiTot << 8) | uiSub;
            }

            return (ushort) uiTot;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b y t e A r r a y T o U I n t 3 2                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns a UInt32 value from the first four bytes of the supplied   //
        // byte array, which is assumed to contain a Big-Endian (Motorola)    //
        // format value.                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private uint byteArrayToUInt32(byte[] Buf)
        {
            const int sliceSize = 4;

            uint uiSub,
                   uiTot;

            uiTot = 0;

            for (int j = 0; j < sliceSize; j++)
            {
                uiSub = (byte) Buf[j];
                uiTot = (uiTot << 8) | uiSub;
            }

            return uiTot;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b y t e A r r a y T o U I n t 6 4                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns a UInt64 value from the first eight bytes of the supplied  //
        // byte array, which is assumed to contain a Big-Endian (Motorola)    //
        // format value.                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private ulong byteArrayToUInt64 (byte[] Buf)
        {
            const int sliceSize = 8;

            ulong uiSub,
                   uiTot;

            uiTot = 0;

            for (int j = 0; j < sliceSize; j++)
            {
                uiSub = (byte) Buf[j];
                uiTot = (uiTot << 8) | uiSub;
            }

            return uiTot;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h a r R e f e r e n c e d C h e c k                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check whether the specified character code is associated with a    //
        // glyph which is present.                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool charReferencedCheck(ushort charCode)
        {
            return _charData[charCode].glyphPresent ();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k F o r T T C                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check whether or not the nominated font file starts with a valid   //
        // 'ttcf' table identifier (for .TTC TrueType Collection file).       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool checkForTTC (string fileName,
                                    ref bool typeTTC,
                                    ref uint numFonts)
        {
            string tabName = "ttcf";

            bool flagOK = false;
            bool fileOpen;

            _fontFileSize = 0;

            typeTTC = false;

            fileOpen = fontFileOpen (fileName, ref _fontFileSize);

            if (!fileOpen)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Error opening TrueType Font file " + fileName);
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // File successfully opened.                                  //
                // Read and check first four bytes to determine if this is a  //
                // TrueType Collection file.                                  //
                //                                                            //
                //------------------------------------------------------------//

                uint tabId = 0,
                       tabVersion = 0;

                typeTTC = false;

                flagOK = readBytesAsUInt32 (0, ref tabId);

                if (!flagOK)
                {
                    flagOK = false;

                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Error reading first four bytes of font file");
                }
                else if (tabId == cTabID_ttcf)
                {
                    typeTTC = true;

                    flagOK = readBytesAsUInt32 (-1, ref tabVersion);

                    if (flagOK)
                    {
                        flagOK = readBytesAsUInt32 (-1, ref numFonts);
                    }

                    if (!flagOK)
                        numFonts = 0;
                }

                //------------------------------------------------------------//
                //                                                            //
                // Diagnostics.                                               //
                //                                                            //
                //------------------------------------------------------------//

                if (_logVerbose && typeTTC)
                {
                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "version  = 0x" + tabVersion.ToString ("x8"));
                    
                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "numfonts = " + numFonts.ToString ());
                }

                if (fileOpen)
                    fontFileClose ();
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k L i c e n c e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Interpret the 'fsTYpe' value read from the OS/2 table, and return  //
        // an indication of whether the font may or may not be converted.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public eLicenceType checkLicence (ref string licenceText)
        {
            eLicenceType licenceType = eLicenceType.NotAllowed;

            if (_OS_2_fsType == mask_OS_2_fsType_INSTALLABLE_EMBED)
            {
                licenceText = "Installable Embedding";
                licenceType = eLicenceType.Allowed;
            }
            else if ((_OS_2_fsType & mask_OS_2_fsType_RESTRICTED_LICENSE_EMBED) != 0)
            {
                licenceText = "Restricted License embedding";
                licenceType = eLicenceType.NotAllowed;
            }
            else if ((_OS_2_fsType & mask_OS_2_fsType_PREVIEW_AND_PRINT_EMBED) != 0)
            {
                licenceText = "Preview & Print embedding";
                licenceType = eLicenceType.OwnerOnly;
            }
            else if ((_OS_2_fsType & mask_OS_2_fsType_EDITABLE_EMBED) != 0)
            {
                licenceText = "Editable embedding";
                licenceType = eLicenceType.OwnerOnly;
            }
            else if ((_OS_2_fsType & mask_OS_2_fsType_Reserved_A) != 0)
            {
                licenceText = "Reserved bit range A";
                licenceType = eLicenceType.NotAllowed;
            }
            else if ((_OS_2_fsType & mask_OS_2_fsType_Reserved_B) != 0)
            {
                licenceText = "Reserved bit range B";
                licenceType = eLicenceType.NotAllowed;
            }

            if ((_OS_2_fsType & mask_OS_2_fsType_NO_SUBSETTING) != 0)
            {
                licenceText += " | No subsetting";
                licenceType = eLicenceType.NotAllowed;
            }

            if ((_OS_2_fsType & mask_OS_2_fsType_BITMAP_EMBED_ONLY) != 0)
            {
                licenceText += " | Bitmap embedding only";
                licenceType = eLicenceType.NotAllowed;
            }

            if ((_OS_2_fsType & mask_OS_2_fsType_Reserved_C) != 0)
            {
                licenceText = " | Reserved bit range C";
                licenceType = eLicenceType.NotAllowed;
            }
            return licenceType;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t F i l e C l o s e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close input stream and file.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void fontFileClose()
        {
            _binReader.Close ();
            _ipStream.Close ();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t F i l e O p e n                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open read stream for specified font file.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool fontFileOpen(string filename,
                                    ref long fileSize)
        {
            bool open = false;

            _filenameTTF = filename;

            if ((filename == null) || (filename == ""))
            {
                MessageBox.Show ("Font file name is null.",
                                "Source (TrueType) font file selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            else if (!File.Exists (filename))
            {
                MessageBox.Show ("Font file '" + filename +
                                "' does not exist.",
                                "Source (TrueType) font file selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    _ipStream = File.Open (filename,
                                           FileMode.Open,
                                           FileAccess.Read,
                                           FileShare.None);

                    if (_ipStream != null)
                    {
                        FileInfo fi = new FileInfo (filename);

                        fileSize = fi.Length;

                        open = true;

                        _binReader = new BinaryReader (_ipStream);
                    }
                }
                catch
                {
                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Error opening font file " + filename);
                }
            }

            return open;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t F i l e R e O p e n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Re-open read stream for specified font file.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool fontFileReOpen()
        {
            bool flagOK = true;

            flagOK = fontFileOpen (_filenameTTF, ref _fontFileSize);

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t F i l e S e e k                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Seek to specified offset in font file.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool fontFileSeek(int offset)
        {
            bool flagOK = true;

            try
            {
                _ipStream.Seek (offset, SeekOrigin.Begin);
            }
            catch
            {
                flagOK = false;
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F o n t F u l l n a m e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the 'Full font name' string read from the 'name' table.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string FontFullname
        {
            get
            {
                return _name_fullFontnameStr;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t B a s i c M e t r i c s                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return basic metrics for the TTF font.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void getBasicMetrics(ref ushort numChars,
                                    ref ushort firstCode,
                                    ref ushort lastCode,
                                    ref ushort maxGlyphId,
                                    ref ushort maxComponentDepth,
                                    ref ushort unitsPerEm,
                                    ref bool glyphZeroExists)
        {
            numChars          = _cmap_numChars;
            firstCode         = _cmap_firstCode;
            lastCode          = _cmap_lastCode;
            maxGlyphId        = (ushort) (_maxp_numGlyphs - 1);
            maxComponentDepth = _maxp_maxCompDepth;
            unitsPerEm        = _head_unitsPerEm;
            glyphZeroExists   = _glyphZeroExists;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C h a r D a t a                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If a glyph is associated with the specified character code, the    //
        // glyph identifier associated with the specified character code in   //
        // the current symbol set is returned.                                //
        // The boolean return value indicates whether or not such a glyph     //
        // exists.                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool getCharData (ushort charCode,
                                    ref ushort codepoint, 
                                    ref ushort glyphId)
        {
            bool glyphPresent = false;

            _charData[charCode].getValues (ref codepoint,
                                           ref glyphId,
                                           ref glyphPresent);

            return glyphPresent;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t G l y p h D a t a                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return glyph data associated with the specified glyph identifier.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void getGlyphData(ushort identifier,
                                 ref ushort width,
                                 ref ushort height,
                                 ref short leftSideBearing,
                                 ref short topSideBearing,
                                 ref uint offset,
                                 ref uint length,
                                 ref bool composite)
        {
            _glyphData[identifier].getMetricsH (ref width,
                                                ref leftSideBearing);
            
            _glyphData[identifier].getMetricsV (ref height,
                                                ref topSideBearing);

            _glyphData[identifier].getLocation (ref offset, ref length);

            _glyphData[identifier].getFlags (ref composite);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O S 2 s T y p o D e s c e n d e r                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the sTypoDescender value obtained from the OS/2 table.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public short getOS2sTypoDescender ()
        {
            return _OS_2_sTypoDescender;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O u t p u t N u m T a b l e s                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the number of tables to be written to the output PCL font   //
        // file for the given Page Description Language.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public short getOutputNumTables (bool pdlIsPCLXL,
                                         bool symSetUnbound,
                                         bool flagVMetrics)
        {
            int numTables;

            if (pdlIsPCLXL)
            {
                if (symSetUnbound)
                {
                    numTables = 5;          // gdir, head, hhea, hmtx, maxp

                    if (flagVMetrics)
                    {
                        if (_tab_vhea.TableLength != 0)
                            numTables++;    // vhea

                        if (_tab_vmtx.TableLength != 0)
                            numTables++;    // vmtx
                    }
                }
                else
                {
                    numTables = 3;          // gdir, head, maxp
                }
            }
            else    // PCL
            {
                numTables = 5;              // gdir, head, hhea, hmtx, maxp 

                if (flagVMetrics)
                {
                    if (_tab_vhea.TableLength != 0)
                        numTables++;        // vhea

                    if (_tab_vmtx.TableLength != 0)
                        numTables++;        // vhea
                }
            }

            if (_tab_cvt.TableLength  != 0)
                numTables++;
            
            if (_tab_fpgm.TableLength != 0)
                numTables++;
            
            if (_tab_prep.TableLength != 0)
                numTables++;

            return (short) numTables;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L F o n t H e a d e r D a t a                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return PCL specific data.                                          //
        // ***** Only called from ToolSoftFontGenPCL.WriteHddr                //
        //                                                                    //
        //--------------------------------------------------------------------//
         
        public void getPCLFontHeaderData (bool usePCLT,
                                          ref bool monoSpaced, 
                                          ref ushort cellWidth,
                                          ref ushort cellHeight,
                                          ref ushort textWidth,
                                          ref ushort textHeight,
                                          ref ushort pitch,
                                          ref ushort xHeight,
                                          ref ushort capHeight,
                                          ref short mUlinePos,
                                          ref ushort mUlineDep,
                                          ref uint fontNo,
                                          ref byte serifStyle,
                                          ref sbyte widthType,
                                          ref byte[] fontName,
                                          ref byte[] panoseData)
        {
            sbyte defPCLWidthType = 0;

            byte panoseFamily,
                 panoseProportion;

            ushort defPCLPitch = 0,
                   advWidthThis = 0,
                   advWidthCommon = 0,
                   glyphId = 0;

            bool monoSpaced_panose,
                    monoSpaced_post,
                    monoSpaced_glyphs,
                    glyphWidthSet,
                    glyphPresent;

            //----------------------------------------------------------------//
            //                                                                //
            // Derive the following values from items (already read) from the //
            // 'OS/2' table:                                                  //
            //    Pitch         default (in absence of space glyph)           //
            //    Text Width                                                  //
            //    xHeight       default (in absence of PCLT table)            //
            //    Width Type    default (in absence of PCLT table); not 1:1   //
            //    Monospaced                                                  //
            //                                                                //
            //----------------------------------------------------------------//

            defPCLWidthType    = 0;
            defPCLPitch        = 0;

            textWidth = (ushort)_OS_2_xAvgCharWidth;
            xHeight = (ushort)_OS_2_sxHeight;
            
            glyphPresent = _charData [cSpaceCodePoint].getGlyphId (ref glyphId);

            if (glyphPresent)
            {
                _glyphData [glyphId].getAdvance (ref advWidthThis);
                if (advWidthThis > 0)
                    defPCLPitch = advWidthThis;
                else
                    defPCLPitch = (ushort) _OS_2_xAvgCharWidth;
            }
            else
            {
                defPCLPitch = (ushort)_OS_2_xAvgCharWidth;
            }

            //------------------------------------------------------------//

            switch (_OS_2_usWidthClass)
            {
                case mask_OS_2_usWidthClass_FWIDTH_ULTRA_CONDENSED:
                    defPCLWidthType = (sbyte)PCLFonts.eWidthType.UltraCompressed;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_EXTRA_CONDENSED:
                    defPCLWidthType = (sbyte)PCLFonts.eWidthType.ExtraCompressed;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_CONDENSED:
                    defPCLWidthType = (sbyte)PCLFonts.eWidthType.Compressed;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_SEMI_CONDENSED:
                    defPCLWidthType = (sbyte)PCLFonts.eWidthType.Condensed;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_NORMAL:
                    defPCLWidthType = (sbyte)PCLFonts.eWidthType.Normal;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_SEMI_EXPANDED:
                    defPCLWidthType = (sbyte)PCLFonts.eWidthType.Expanded;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_EXPANDED:
                    defPCLWidthType = (sbyte)PCLFonts.eWidthType.Expanded;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_EXTRA_EXPANDED:
                    defPCLWidthType = (sbyte)PCLFonts.eWidthType.ExtraExpanded;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_ULTRA_EXPANDED:
                    defPCLWidthType = (sbyte)PCLFonts.eWidthType.ExtraExpanded;
                    break;

                default:
                    defPCLWidthType = (sbyte)PCLFonts.eWidthType.Normal;
                    break;
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Get data from PCLT table (already read, if it exists).         //
            //                                                                //
            //----------------------------------------------------------------//
            
            if ((!_tabPCLTPresent) || (!usePCLT))
            {
                //--------------------------------------------------------//
                //                                                        //
                // Set default values for the cases where either the      //
                // table is not present, or the option has been set to    //
                // ignore it.                                             //
                //                                                        //
                //--------------------------------------------------------//

                pitch = defPCLPitch;
                capHeight = 0;
                fontNo = 0;
                widthType = defPCLWidthType;
                serifStyle = 0;

                fontName [0] = _name_fullFontname [0];
                fontName [1] = _name_fullFontname [1];
                fontName [2] = _name_fullFontname [2];
                fontName [3] = _name_fullFontname [3];
                fontName [4] = _name_fullFontname [4];
                fontName [5] = _name_fullFontname [5];
                fontName [6] = _name_fullFontname [6];
                fontName [7] = _name_fullFontname [7];
                fontName [8] = _name_fullFontname [8];
                fontName [9] = _name_fullFontname [9];
                fontName [10] = _name_fullFontname [10];
                fontName [11] = _name_fullFontname [11];
                fontName [12] = _name_fullFontname [12];
                fontName [13] = _name_fullFontname [13];
                fontName [14] = _name_fullFontname [14];
                fontName [15] = _name_fullFontname [15];
            }
            else
            {
                if (_PCLT_pitch == 0)
                    pitch = defPCLPitch;
                else
                    pitch = _PCLT_pitch;

                capHeight = _PCLT_capHeight;
                fontNo = _PCLT_fontNo;
                widthType = _PCLT_widthType;
                serifStyle = _PCLT_serifStyle;

                fontName [0] = _PCLT_typeface [0];
                fontName [1] = _PCLT_typeface [1];
                fontName [2] = _PCLT_typeface [2];
                fontName [3] = _PCLT_typeface [3];
                fontName [4] = _PCLT_typeface [4];
                fontName [5] = _PCLT_typeface [5];
                fontName [6] = _PCLT_typeface [6];
                fontName [7] = _PCLT_typeface [7];
                fontName [8] = _PCLT_typeface [8];
                fontName [9] = _PCLT_typeface [9];
                fontName [10] = _PCLT_typeface [10];
                fontName [11] = _PCLT_typeface [11];
                fontName [12] = _PCLT_typeface [12];
                fontName [13] = _PCLT_typeface [13];
                fontName [14] = _PCLT_typeface [14];
                fontName [15] = _PCLT_typeface [15];
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Derive the following values from items (already read) from the //
            // 'head' table:                                                  //
            //    Cell Height                                                 //
            //         Width                                                  //
            //    Master Underline Position                                   //
            //                     Thickness                                  //
            //                                                                //
            //----------------------------------------------------------------//

            cellWidth  = (ushort) (_head_xMax - _head_xMin);
            cellHeight = (ushort) (_head_yMax - _head_yMin);

            mUlinePos = (short)  (- (_head_unitsPerEm * 20) / 100);
            mUlineDep = (ushort) (_head_unitsPerEm * 5 / 100);

            //----------------------------------------------------------------//
            //                                                                //
            // Derive the following values from items (already read) from the //
            // 'hhea' table:                                                  //
            //    Text Height                                                 //
            //                                                                //
            //----------------------------------------------------------------//

            textHeight = (ushort) (_hhea_ascender - _hhea_descender +
                                   _hhea_lineGap);

            //----------------------------------------------------------------//
            //                                                                //
            // Check that the various 'monospaced' indicators are consistent  //
            // and determine wherher or not the font is 'monospaced'.         //
            //                                                                //
            // 'cmap' table:                                                  //
            //    derived CharCodeEntry[] array maps codepoints to glyphs     //
            // 'glyf' table:                                                  //
            //    derived GlyphDataEntry[] array gives glyph advance widths   //
            // 'OS/2' table:                                                  //
            //    Panose                                                      //
            //    xAvgCharWidth                                               //
            // 'post' table:                                                  //
            //    isFixedPitch                                                //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < cSizePanose; i++)
            {
                panoseData [i] = _OS_2_panose [i];
            }

            panoseFamily = panoseData [0];
            panoseProportion = panoseData [3];

            if ((panoseFamily == cPanoseFamilyLatinText)
                               &&
                (panoseProportion == cPanoseMonoLatinText))
            {
                monoSpaced_panose = true;
            }
            else if ((panoseFamily == cPanoseFamilyLatinHandwritten)
                                   &&
                    (panoseProportion == cPanoseMonoLatinHandwritten))
            {
                monoSpaced_panose = true;
            }
            else if ((panoseFamily == cPanoseFamilyLatinDecorative)
                                   &&
                    (panoseProportion == cPanoseMonoLatinDecorative))
            {
                monoSpaced_panose = true;
            }
            else if ((panoseFamily == cPanoseFamilyLatinSymbol)
                                   &&
                    (panoseProportion == cPanoseMonoLatinSymbol))
            {
                monoSpaced_panose = true;
            }
            else
            {
                monoSpaced_panose = false;
            }

            //----------------------------------------------------------------//

            if (_post_isFixedPitch == 0)
                monoSpaced_post = false;
            else
                monoSpaced_post = true;

            //----------------------------------------------------------------//

            glyphWidthSet = false;
            glyphPresent = false;

            monoSpaced_glyphs = true;
 
            for (int i = _cmap_firstCode; i <= _cmap_lastCode; i++)
            {
                glyphPresent = _charData [i].getGlyphId (ref glyphId);

                if (glyphPresent)
                {
                    _glyphData [glyphId].getAdvance (ref advWidthThis);

                    if (glyphWidthSet)
                    {
                        if (advWidthThis != advWidthCommon)
                        {
                            monoSpaced_glyphs = false;
                        }
                    }
                    else
                    {
                        glyphWidthSet = true;

                        advWidthCommon = advWidthThis;
                    }
                }
            }

            //----------------------------------------------------------------//

            if (! monoSpaced_glyphs)
            {
                monoSpaced = false;
            }
            else if (monoSpaced_panose && monoSpaced_post)
            {
                monoSpaced = true;
            }
            else
            {
                MessageBoxResult mbResult;

                string text_OS_2_panose,
                       text_OS_2_avgWidth,
                       text_post,
                       text_glyph,
                       text_common;

                text_common = "Inconsistency between indicators:";

                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, true, false,
                    "Monospacing check",
                    text_common);

                text_glyph = "Glyphs in chosen mapping all have advance" +
                             " width = " + advWidthCommon;

                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, false, false,
                    "",
                    text_glyph);

                text_OS_2_avgWidth = "OS/2 | xAvgCharWidth = " +
                                     _OS_2_xAvgCharWidth;

                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, false, false,
                    "",
                    text_OS_2_avgWidth);

                if (monoSpaced_panose)
                    text_OS_2_panose = "OS/2 | Panose: Family = " + panoseFamily +
                                       " & Proportion = " + panoseProportion +
                                       " indicates font is monospaced";
                else
                    text_OS_2_panose = "OS/2 | Panose: Family = " + panoseFamily +
                                       " & Proportion = " + panoseProportion +
                                       " indicates font is not monospaced";

                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, false, false,
                    "",
                    text_OS_2_panose);

                if (_post_isFixedPitch == 0)
                    text_post = "post | isFixedPitch = 0x" +
                                _post_isFixedPitch.ToString ("x8") +
                                " (= proportionally-spaced)";
                else
                    text_post = "post | isFixedPitch = 0x" +
                                _post_isFixedPitch.ToString ("x8") +
                                " (= fixed-pitch)";

                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, false, false,
                    "",
                    text_post);

                mbResult = MessageBox.Show (
                                text_common + "\r\n" +
                                text_glyph + "\r\n" +
                                text_OS_2_avgWidth + "\r\n" +
                                text_OS_2_panose + "\r\n" +
                                text_post + "\r\n\r\n" +
                                "Generate PCL font as fixed-pitch?",
                                "Monospacing inconsistency",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question);

                if (mbResult == MessageBoxResult.Yes)
                {
                    monoSpaced = true;

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "",
                        "User chooses to generate PCL font as" +
                        " fixed-pitch");
                }
                else
                {
                    monoSpaced = false;

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "",
                        "User chooses to generate PCL font as" +
                        " proportionally-spaced");
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L F o n t S e l e c t D a t a                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return PCL font select data.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void getPCLFontSelectData (ref ushort styleNonPCLT,
                                          ref sbyte strokeWeightNonPCLT,
                                          ref ushort symSetPCLT,
                                          ref ushort stylePCLT,
                                          ref sbyte strokeWeightPCLT,
                                          ref ushort typefaceNoPCLT,
                                          ref string typefacePCLT,
                                          ref ulong charCompPCLT)
        {
            byte stylePosture = 0,
                 styleWidth = 0;

            ushort styleStructure;

            //----------------------------------------------------------------//
            //                                                                //
            // Get values from PCLT table (if present).                       //
            // Derive values from other tables.                               //
            //                                                                //
            //----------------------------------------------------------------//

            if (_tabPCLTPresent)
            {
                symSetPCLT       = _PCLT_symSet;
                stylePCLT        = _PCLT_style;
                strokeWeightPCLT = _PCLT_strokeWeight;
                typefaceNoPCLT   = _PCLT_typeFamily;
                typefacePCLT     = Encoding.ASCII.GetString(_PCLT_typeface);
                charCompPCLT     = _PCLT_charComp; 
            }
            else
            {
                symSetPCLT       = 0;
                stylePCLT        = 0;
                strokeWeightPCLT = 0;
                typefaceNoPCLT   = 0;
                typefacePCLT     = null;
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Get values from other tables (for style and strokeweight).     //
            //                                                                //
            //----------------------------------------------------------------//

            if ((_OS_2_fsSelection & mask_OS_2_fsSelection_REGULAR) != 0)
                stylePosture = (byte)PCLFonts.eStylePosture.Upright;
            else if ((_OS_2_fsSelection & mask_OS_2_fsSelection_ITALIC) != 0)
                stylePosture = (byte)PCLFonts.eStylePosture.Italic;
            else if ((_OS_2_fsSelection & mask_OS_2_fsSelection_OBLIQUE) != 0)
                stylePosture = (byte)PCLFonts.eStylePosture.ItalicAlt;
            else
                stylePosture = (byte)PCLFonts.eStylePosture.Upright;

            if ((_OS_2_fsSelection & mask_OS_2_fsSelection_OUTLINED) != 0)
                styleStructure = (ushort)PCLFonts.eStyleStructure.Outline;
            else
                styleStructure = (ushort)PCLFonts.eStyleStructure.Solid;

            //------------------------------------------------------------//
            //                                                            //
            // Map the nine OS_2_usWidth Class values to the eight PCL    //
            // style Width values (not a 1:1 relationship).               //
            //                                                            //
            //------------------------------------------------------------//

            switch (_OS_2_usWidthClass)
            {
                case mask_OS_2_usWidthClass_FWIDTH_ULTRA_CONDENSED:
                    styleWidth = (byte)PCLFonts.eStyleWidth.UltraCompressed;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_EXTRA_CONDENSED:
                    styleWidth = (byte)PCLFonts.eStyleWidth.ExtraCompressed;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_CONDENSED:
                    styleWidth = (byte)PCLFonts.eStyleWidth.Compressed;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_SEMI_CONDENSED:
                    styleWidth = (byte)PCLFonts.eStyleWidth.Condensed;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_NORMAL:
                    styleWidth = (byte)PCLFonts.eStyleWidth.Normal;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_SEMI_EXPANDED:
                    styleWidth = (byte)PCLFonts.eStyleWidth.Expanded;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_EXPANDED:
                    styleWidth = (byte)PCLFonts.eStyleWidth.Expanded;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_EXTRA_EXPANDED:
                    styleWidth = (byte)PCLFonts.eStyleWidth.ExtraExpanded;
                    break;

                case mask_OS_2_usWidthClass_FWIDTH_ULTRA_EXPANDED:
                    styleWidth = (byte)PCLFonts.eStyleWidth.ExtraExpanded;
                    break;

                default:
                    styleWidth = (byte)PCLFonts.eStyleWidth.Normal;
                    break;
            }

            //------------------------------------------------------------//
            //                                                            //
            // Map the nine common OS_2_usWeightClass values to the       //
            // fifteen PCL strokeWeight values (not a 1:1 relationship).  //
            //                                                            //
            // The _OS_2_fsSelection.REGULAR or .BOLD flags are not       //
            // examined, but they should be consistent with the           //
            // _usWeightClass values.                                     // 
            //                                                            //
            //------------------------------------------------------------//

            switch (_OS_2_usWeightClass)
            {
                case mask_OS_2_usWeightClass_FW_THIN:
                    strokeWeightNonPCLT = (sbyte)PCLFonts.eStrokeWeight.Thin;
                    break;

                case mask_OS_2_usWeightClass_FW_EXTRALIGHT:
                    strokeWeightNonPCLT = (sbyte)PCLFonts.eStrokeWeight.ExtraLight;
                    break;

                case mask_OS_2_usWeightClass_FW_LIGHT:
                    strokeWeightNonPCLT = (sbyte)PCLFonts.eStrokeWeight.Light;
                    break;

                case mask_OS_2_usWeightClass_FW_NORMAL:
                    strokeWeightNonPCLT = (sbyte)PCLFonts.eStrokeWeight.Medium;
                    break;

                case mask_OS_2_usWeightClass_FW_MEDIUM:
                    strokeWeightNonPCLT = (sbyte)PCLFonts.eStrokeWeight.Medium;
                    break;

                case mask_OS_2_usWeightClass_FW_SEMIBOLD:
                    strokeWeightNonPCLT = (sbyte)PCLFonts.eStrokeWeight.SemiBold;
                    break;

                case mask_OS_2_usWeightClass_FW_BOLD:
                    strokeWeightNonPCLT = (sbyte)PCLFonts.eStrokeWeight.Bold;
                    break;

                case mask_OS_2_usWeightClass_FW_EXTRABOLD:
                    strokeWeightNonPCLT = (sbyte)PCLFonts.eStrokeWeight.ExtraBold;
                    break;

                case mask_OS_2_usWeightClass_FW_BLACK:
                    strokeWeightNonPCLT = (sbyte)PCLFonts.eStrokeWeight.Black;
                    break;

                default:
                    strokeWeightNonPCLT = (sbyte)PCLFonts.eStrokeWeight.Medium;
                    break;
            }

            //------------------------------------------------------------//

            styleNonPCLT = (ushort)(stylePosture +
                                    (4 * styleWidth) +
                                    (32 * styleStructure));
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L T S y m S e t                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the Symbol Set value obtained from the (optional) PCLT      //
        // table.                                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort getPCLTSymSet ()
        {
            return _PCLT_symSet;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L T T y p e F a m i l y                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the Type Family value obtained from the (optional) PCLT     //
        // table.                                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort getPCLTTypeFamily()
        {
            return _PCLT_typeFamily;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t S e g G T T a b l e s S i z e                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the overall size of the GT segment tables to be written to  //
        // the output PCL font file.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public uint getSegGTTablesSize (bool pdlIsPCLXL,
                                          bool symSetUnbound,
                                          bool flagVMetrics)
        {
            uint sizeTables = 0;

            sizeTables = _tab_cvt.TablePadLen +
                         _tab_fpgm.TablePadLen +
                         _tab_head.TablePadLen +
                         _tab_maxp.TablePadLen +
                         _tab_prep.TablePadLen;
            
            if ((! pdlIsPCLXL) || symSetUnbound)
            {
                sizeTables = sizeTables +
                             _tab_hhea.TablePadLen +
                             _tab_hmtx.TablePadLen;

                if (flagVMetrics)
                {
                    sizeTables = sizeTables +
                                 _tab_vhea.TablePadLen +
                                 _tab_vmtx.TablePadLen;
                }
            }

            return sizeTables;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T a b l e M e t r i c s                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return table metrics for the tables which may be written to the    //
        // PCL font file.                                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void getTableMetrics(ref ToolSoftFontGenTTFTable data_cvt,
                                    ref ToolSoftFontGenTTFTable data_gdir,
                                    ref ToolSoftFontGenTTFTable data_fpgm,
                                    ref ToolSoftFontGenTTFTable data_head,
                                    ref ToolSoftFontGenTTFTable data_hhea,
                                    ref ToolSoftFontGenTTFTable data_hmtx,
                                    ref ToolSoftFontGenTTFTable data_maxp,
                                    ref ToolSoftFontGenTTFTable data_prep,
                                    ref ToolSoftFontGenTTFTable data_vhea,
                                    ref ToolSoftFontGenTTFTable data_vmtx)
        {
            data_cvt  = _tab_cvt;
            data_gdir = _tab_gdir;
            data_fpgm = _tab_fpgm;
            data_head = _tab_head;
            data_hhea = _tab_hhea;
            data_hmtx = _tab_hmtx;
            data_maxp = _tab_maxp;
            data_prep = _tab_prep;
            data_vhea = _tab_vhea;
            data_vmtx = _tab_vmtx;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T T C D a t a                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read the 'ttcf' (TrueType Collection header) table to obtain:      //
        //    Number of fonts in TTC                                          //
        //    Array of offsets to the OffsetTable for each font in the        //
        //        collection                                                  //
        // The return value indicates success or failure of the reads.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool getTTCData (string fileName,
                                   uint numFonts, 
                                   ref uint[] fontOffsets,
                                   ref string[] fontNames)
        {
            string tabName = "ttcf";

            bool flagOK = true;
            bool fileOpen;

            int offset;

            fileOpen = fontFileOpen (fileName, ref _fontFileSize);

            if (!fileOpen)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Error opening TrueType Font file " + fileName);
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // File successfully opened.                                  //
                // Read and check first four bytes to determine if this is a  //
                // TrueType Collection file.                                  //
                //                                                            //
                //------------------------------------------------------------//

                uint checkNumFonts = 0;

                offset = 8;
                
                flagOK = readBytesAsUInt32 (offset, ref checkNumFonts);

                if (checkNumFonts != numFonts)
                {
                    flagOK = false;
                }
                else
                {
                    offset += 4;

                    for (int i = 0; i < numFonts; i++)
                    {
                        flagOK = readBytesAsUInt32 (offset, ref fontOffsets [i]);

                        if (!flagOK)
                        {
                            i = (int)numFonts;
                        }
                        else
                        {
                            offset += 4;

                            //------------------------------------------------//
                            //                                                //
                            // Diagnostics.                                   //
                            //                                                //
                            //------------------------------------------------//

                            if (_logVerbose)
                            {
                                ToolSoftFontGenLog.logNameAndValue (
                                    _tableDonor, true, false,
                                    "DIAG: " + tabName + " subTab " + i + ":",
                                    "offset = " +
                                    fontOffsets [i].ToString ());
                            }

                            flagOK = readTableDirectory ((int) fontOffsets[i],
                                                         true);

                            if (flagOK)
                            {
                                flagOK = readData_name (true, ref fontNames[i]);
                            }
                        }
                    }
                }

                if (fileOpen)
                    fontFileClose ();
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g l y p h C o m p o s i t e C h e c k                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check whether the specified glyph is a composite.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool glyphCompositeCheck(ushort glyphId)
        {
            return _glyphData[glyphId].checkComposite ();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g l y p h R e f e r e n c e d C h e c k                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check whether the specified glyph has already been referenced by   //
        // the output (printer) font generator.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool glyphReferencedCheck(ushort glyphId)
        {
            return _glyphData [glyphId].checkReferenced();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g l y p h R e f e r e n c e d M a r k                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set marker indicating that the specified glyph has been            //
        // referenced by the output (printer) font generator.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void glyphReferencedMark(ushort glyphId)
        {
            _glyphData[glyphId].markReferenced ();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g l y p h R e f e r e n c e d U n m a r k A l l                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set marker indicating that the specified glyph has been            //
        // referenced by the output (printer) font generator.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void glyphReferencedUnmarkAll()
        {
            for (int i = 0; i < _maxp_numGlyphs; i++)
            {
                _glyphData[i].unmarkReferenced ();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e F o n t D a t a                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Obtain and store basic data from the nominated TrueType font file. //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool initialiseFontData(string fileName,
                                          int sfntOffset,
                                          int symSetIndx,
                                          ref bool tabPCLTPresent,
                                          ref bool tabvmtxPresent,
                                          ref bool symbolMapping,
                                          bool symSetUnbound,
                                          bool symSetUserSet,
                                          bool symSetMapPCL)
        {
            bool flagOK = false;

            bool fileOpen;

            _fontFileSize = 0;
            _cmap_numChars = 0;
            _tabPCLTPresent = false;
            _tabvmtxPresent = false;

            fileOpen = fontFileOpen (fileName, ref _fontFileSize);

            if (! fileOpen)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Error opening TrueType Font file " + fileName);
            }
            else
            {
                uint tabVer_sfnt = 0;

                //------------------------------------------------------------//
                //                                                            //
                // Read and check TTF 'sfnt' version value.                   //
                //                                                            //
                //------------------------------------------------------------//

                flagOK = readBytesAsUInt32 (sfntOffset, ref tabVer_sfnt);

                if (!flagOK)
                {
                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Error reading sfntVersion");
                }
                else if (tabVer_sfnt != cTabVer_sfnt)
                {
                    flagOK = false;

                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Wrong sfntVersion = 0x" +
                        tabVer_sfnt.ToString ("x8") +
                        "\r\n\r\n" +
                        "Expected version = 0x" +
                        cTabVer_sfnt.ToString ("x8"));
                }

                //------------------------------------------------------------//
                //                                                            //
                // Read TTF Table Directory entries.                          //
                // These indicate which tables are present (not all are       //
                // mandatory) and provide the start offset (from beginning of //
                // file) and length of those tables which are present.        //
                //                                                            //
                //------------------------------------------------------------//

                if (flagOK)
                {
                    flagOK = readTableDirectory (sfntOffset, false);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Read required entries from the various tables.             //
                //                                                            //
                //------------------------------------------------------------//

                if (flagOK)
                {
                    string dummyString = "";

                    flagOK = readData_name (false, ref dummyString);
                }

                if (flagOK)
                {
                    flagOK = readData_OS_2 ();
                }

                if (flagOK)
                {
                    flagOK = readData_head ();
                }

                if (flagOK)
                {
                    flagOK = readData_hhea ();
                }

                if (flagOK && (_tab_vhea.TableLength != 0))
                {
                    flagOK = readData_vhea ();
                }

                if (flagOK)
                {
                    flagOK = readData_maxp ();
                }

                if (flagOK)
                {
                    flagOK = readData_PCLT ();
                }

                if (flagOK)
                {
                    flagOK = readData_post ();
                }

                //------------------------------------------------------------//
                //                                                            //
                // Construct a 'character-code to glyph-ID' index (for the    //
                // chosen symbol set), by reading entries from the 'cmap'     //
                // (character to glyph index mapping) table.                  //
                //                                                            //
                //------------------------------------------------------------//

                if (flagOK)
                {
                    flagOK = readData_cmap (symSetIndx,
                                            ref symbolMapping,
                                            symSetUnbound,
                                            symSetUserSet,
                                            symSetMapPCL);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Obtain the advance widths and left-side bearings for the   //
                // glyphs (and hence characters in the chosen symbol set) by  //
                // reading entries from the 'hmtx' (horizontal metrics) table.//
                //                                                            //
                //------------------------------------------------------------//

                if (flagOK)
                {
                    _glyphData = new GlyphDataEntry[_maxp_numGlyphs];

                    flagOK = readData_hmtx ();
                }

                //------------------------------------------------------------//
                //                                                            //
                // Obtain the advance heights and top-side bearings for the   //
                // glyphs (and hence characters in the chosen symbol set) by  //
                // reading entries from the 'vmtx' (vertical metrics) table.  //
                //                                                            //
                //------------------------------------------------------------//

                if (flagOK && (_tab_vmtx.TableLength != 0))
                {
                    flagOK = readData_vmtx ();
                }

                //------------------------------------------------------------//
                //                                                            //
                // Obtain the locations and composite flag indicators for the //
                // glyphs (and hence characters in the chosen symbol set) by  //
                // reading entries from the 'loca' (index to location) and    //
                // glypf (glyph data) tables.                                 //
                //                                                            //
                //------------------------------------------------------------//

                if (flagOK)
                {
                    flagOK = readData_loca_glyf ();
                }

                //------------------------------------------------------------//
                //                                                            //
                // Check whether glyph zero (the .notdef glyph) exists (it    //
                // usually does).                                             //
                //                                                            //
                //------------------------------------------------------------//

                if (flagOK)
                {
                    uint glyphOffset = 0,
                           glyphLength = 0;

                    _glyphData[0].getLocation (ref glyphOffset,
                                               ref glyphLength);

                    if (glyphLength == 0)
                        _glyphZeroExists = false;
                    else
                        _glyphZeroExists = true;
                }

                if (fileOpen)
                    fontFileClose ();
            }

            tabPCLTPresent = _tabPCLTPresent;
            tabvmtxPresent = _tabvmtxPresent;

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P a n o s e D a t a                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the 'Panose data' array read from the 'OS/2' table.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte[] PanoseData
        {
            get
            {
                return _OS_2_panose;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d B y t e A r r a y                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read a specified number of bytes from a specified offset in the    //
        // TrueType file, and write them to a specified buffer.               //
        // The return value indicates success or failure of the read.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool readByteArray(int offset,
                                      int length,
                                      ref byte[] target)
        {
            int readLen = 0;

            bool flagOK;

            flagOK = true;

            if (offset != -1)
                flagOK = fontFileSeek (offset);

            if (flagOK)
            {
                try
                {
                    readLen = _binReader.Read (target, 0, length);

                    if (readLen != length)
                        flagOK = false;
                }
                catch
                {
                    flagOK = false;
                }
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d B y t e A s S B y t e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read one byte from the TrueType file, and return the value as an   //
        // SByte.                                                             //
        // The return value indicates success or failure of the read.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readByteAsSByte(int offset,
                                        ref sbyte target)
        {
            const int sliceSize = 1;

            int readLen = 0;

            bool flagOK;

            byte[] slice = new byte[sliceSize];

            flagOK = true;

            if (offset != -1)
                flagOK = fontFileSeek (offset);

            if (flagOK)
            {
                try
                {
                    readLen = _binReader.Read (slice, 0, sliceSize);

                    if (readLen != sliceSize)
                        flagOK = false;
                }
                catch
                {
                    flagOK = false;
                }
            }

            if (flagOK)
            {
                target = (sbyte) slice[0];
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d B y t e A s U B y t e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read one byte from the TrueType file, and return the value as a    //
        // UByte.                                                             //
        // The return value indicates success or failure of the read.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readByteAsUByte(int offset,
                                        ref byte target)
        {
            const int sliceSize = 1;

            int readLen = 0;

            bool flagOK;

            byte[] slice = new byte[sliceSize];

            flagOK = true;

            if (offset != -1)
                flagOK = fontFileSeek (offset);

            if (flagOK)
            {
                try
                {
                    readLen = _binReader.Read (slice, 0, sliceSize);

                    if (readLen != sliceSize)
                        flagOK = false;
                }
                catch
                {
                    flagOK = false;
                }
            }

            if (flagOK)
            {
                target = slice[0];
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d B y t e s A s I n t 1 6                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read two bytes (assumed to contain a Motorola (Big-Endian) format  //
        // value) from the TrueType file, and convert the value to an Int16   //
        // integer.                                                           //
        // The return value indicates success or failure of the read.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readBytesAsInt16(int offset,
                                         ref short target)
        {
            const int sliceSize = 2;

            int readLen = 0;

            bool flagOK;

            byte[] slice = new byte[sliceSize];

            flagOK = true;

            if (offset != -1)
                flagOK = fontFileSeek (offset);

            if (flagOK)
            {
                try
                {
                    readLen = _binReader.Read (slice, 0, sliceSize);

                    if (readLen != sliceSize)
                        flagOK = false;
                }
                catch
                {
                    flagOK = false;
                }
            }

            if (flagOK)
            {
                target = byteArrayToInt16 (slice);
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d B y t e s A s U I n t 1 6                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read two bytes (assumed to contain a Motorola (Big-Endian) format  //
        // value) from the TrueType file, and convert the value to a UInt16   //
        // integer.                                                           //
        // The return value indicates success or failure of the read.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readBytesAsUInt16(int offset,
                                          ref ushort target)
        {
            const int sliceSize = 2;

            int readLen = 0;

            bool flagOK;

            byte[] slice = new byte[sliceSize];

            flagOK = true;

            if (offset != -1)
                flagOK = fontFileSeek (offset);

            if (flagOK)
            {
                try
                {
                    readLen = _binReader.Read (slice, 0, sliceSize);

                    if (readLen != sliceSize)
                        flagOK = false;
                }
                catch
                {
                    flagOK = false;
                }
            }

            if (flagOK)
            {
                target = byteArrayToUInt16 (slice);
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d B y t e s A s U I n t 3 2                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read four bytes (assumed to contain a Motorola (Big-Endian) format //
        // value) from the TrueType file, and convert the value to a UInt32   //
        // integer.                                                           //
        // The return value indicates success or failure of the read.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readBytesAsUInt32(int offset,
                                          ref uint target)
        {
            const int sliceSize = 4;

            int readLen = 0;

            bool flagOK;

            byte[] slice = new byte[sliceSize];

            flagOK = true;

            if (offset != -1)
                flagOK = fontFileSeek (offset);

            if (flagOK)
            {
                try
                {
                    readLen = _binReader.Read (slice, 0, sliceSize);

                    if (readLen != sliceSize)
                        flagOK = false;
                }
                catch
                {
                    flagOK = false;
                }
            }

            if (flagOK)
            {
                target = byteArrayToUInt32 (slice);
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d B y t e s A s U I n t 6 4                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read eight bytes (assumed to contain a Motorola (Big-Endian)       //
        // format value) from the TrueType file, and convert the value to a   //
        // UInt64 integer.                                                    //
        // The return value indicates success or failure of the read.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readBytesAsUInt64 (int offset,
                                           ref ulong target)
        {
            const int sliceSize = 8;

            int readLen = 0;

            bool flagOK;

            byte[] slice = new byte[sliceSize];

            flagOK = true;

            if (offset != -1)
                flagOK = fontFileSeek(offset);

            if (flagOK)
            {
                try
                {
                    readLen = _binReader.Read(slice, 0, sliceSize);

                    if (readLen != sliceSize)
                        flagOK = false;
                }
                catch
                {
                    flagOK = false;
                }
            }

            if (flagOK)
            {
                target = byteArrayToUInt64(slice);
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d D a t a _ c m a p                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        //                                                                    //
        // Read the (mandatory) 'cmap' (font header) table to obtain details  //
        // of (Unicode) index to glyph ID mapping; from this, derive.         //
        //    firstCode                                                       //
        //    lastCode                                                        //
        //    numChars                                                        //
        // The return value indicates success or failure of the reads.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readData_cmap (int symSetIndx,
                                       ref bool symbolMapping,
                                       bool symSetUnbound,
                                       bool symSetUserSet,
                                       bool symSetMapPCL)
        {
            string tabName = "cmap";

            bool flagOK = true;

            uint tabOffset = 0,
                   tabLength = 0,
                   reqLength;

            uint subTabOffset = 0,
                   subTabSymbol = 0,
                   subTabUnicode = 0;

            ushort subTabPlatform = 0,
                   subTabEncoding = 0,
                   subTabFormat = 0;

            ushort fmt4Length = 0,
                   fmt4Lang = 0,
                   fmt4SegCountx2 = 0,
                   fmt4SegCount = 0,
                   fmt4SearchRange = 0,
                   fmt4EntrySelector = 0,
                   fmt4RangeShift = 0;

            int baseEndCode = 0,
                   baseStartCode = 0,
                   baseIdDelta = 0,
                   baseIdRangeOffset = 0,
                   baseGlyphIdArray = 0,
                   sizeGlyphIdArray = 0,
                   subTabNo = -1;

            ushort tabVersion = 0,
                   tabNumTables = 0;

            bool encodingSymbol,
                    encodingUnicode;

            string mapSymSet     = "";
            string mapSymSetType = "";

            flagOK = true;

            encodingSymbol  = false;
            encodingUnicode = false;

            _tab_cmap.getByteRange (ref tabOffset, ref tabLength);

            //----------------------------------------------------------------//
            //                                                                //
            // Get table header details.                                      //
            // Format:                                                        //
            //    ushort   version                                            //
            //    ushort   numTables                                          //
            //                                                                //
            //----------------------------------------------------------------//

            reqLength = 4;

            if (tabLength < reqLength)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Length of '" + tabName + "' table too small: " +
                    tabLength + " < " + reqLength);
            }
            
            if (flagOK)
            {
                flagOK = readBytesAsUInt16 ((int)tabOffset, ref tabVersion);

                if (!flagOK)
                {
                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Error reading '" + tabName + "' table version");
                }
                else
                {
                    flagOK = readBytesAsUInt16 ((int) (tabOffset + 2),
                                                ref tabNumTables);
                    if (flagOK)
                    {
                        reqLength = 4 + (8 * (uint) tabNumTables);

                        if (reqLength > tabLength)
                        {
                            flagOK  = false;

                            ToolSoftFontGenLog.logError (
                                _tableDonor, MessageBoxImage.Error,
                                "Length of '" + tabName +
                                "' table too small for " +
                                tabNumTables + " encoding records: " +
                                tabLength + " < " + reqLength);
                        }
                    }
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Read array of encoding records, which provide details of the   //
            // sub-tables present; there should be 'tabNumTables' of these.   //
            // These entries follow the table header; format of each entry:   //
            //    UInt16   platformID                                         //
            //    UInt16   encodingID                                         //
            //    UInt32   offset                                             //
            //                                                                //
            //----------------------------------------------------------------//

            if (flagOK)
            {
                if (_logVerbose)
                {
                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, true, false,
                        "DIAG: table = " + tabName + ":",
                        "table has " +
                                 tabNumTables.ToString () + " sub-tables");
                }

                for (int i = 0; (i < tabNumTables) && flagOK; i++)
                {
                    flagOK = readBytesAsUInt16 (-1, ref subTabPlatform);

                    if (flagOK)
                        flagOK = readBytesAsUInt16 (-1, ref subTabEncoding);

                    if (flagOK)
                        flagOK = readBytesAsUInt32 (-1, ref subTabOffset);

                    if (flagOK)
                    {
                        if (_logVerbose)
                        {
                            string platformDesc;
                            string encDesc;

                            switch (subTabPlatform)
                            {
                                case 0:
                                    platformDesc = " (Unicode)";
                                    encDesc = "";
                                    break;

                                case 1:
                                    platformDesc = " (Macintosh)";
                                    encDesc = "";
                                    break;

                                case 2:
                                    platformDesc = " (ISO)";
                                    encDesc = "";
                                    break;

                                case 3:
                                    platformDesc = " (Windows)";

                                    switch (subTabEncoding)
                                    {
                                        case 0:
                                            encDesc = " (Symbol)";
                                            break;

                                        case 1:
                                            encDesc = " (Unicode BMP (UCS-2))";
                                            break;

                                        case 2:
                                            encDesc = " (ShiftJIS)";
                                            break;

                                        case 3:
                                            encDesc = " (PRC)";
                                            break;

                                        case 4:
                                            encDesc = " (BIG5)";
                                            break;

                                        case 5:
                                            encDesc = " (Wansung)";
                                            break;

                                        case 6:
                                            encDesc = " (Johab)";
                                            break;

                                        case 7:
                                            encDesc = " (Reserved)";
                                            break;

                                        case 8:
                                            encDesc = " (Reserved)";
                                            break;

                                        case 9:
                                            encDesc = " (Reserved)";
                                            break;

                                        case 10:
                                            encDesc = " (Unicode UCS-4)";
                                            break;

                                        default:
                                            encDesc = " (unknown)";
                                            break;
                                    }

                                    break;

                                case 4:
                                    platformDesc = " (Custom)";
                                    encDesc = "";
                                    break;

                                default:
                                    platformDesc = " (unknown)";
                                    encDesc = "";
                                    break;
                            }

                            ToolSoftFontGenLog.logNameAndValue (
                                _tableDonor, false, false,
                                "DIAG: " + tabName + " subTab " + i + ":",
                                "offset = " + subTabOffset +
                                "; platform = " + subTabPlatform + platformDesc +
                                "; encoding = " + subTabEncoding + encDesc);
                        }

                        reqLength = 4 + subTabOffset;

                        if (reqLength > tabLength)
                        {
                            flagOK = false;

                            ToolSoftFontGenLog.logError (
                                _tableDonor, MessageBoxImage.Error,
                                "'" + tabName + "' sub-table header at offset " +
                                reqLength + " past end of table of length " +
                                tabLength);
                        }
                    }

                    if (flagOK)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Check whether the sub-table is one of interest:    //
                        //    platform = 3 (Windows)                          //
                        //    encoding = 0 (Symbol)  or                       //
                        //               1 (Unicode)                          //
                        //                                                    //
                        //----------------------------------------------------//

                        if ((subTabPlatform == 3) && (subTabEncoding == 0))
                        {
                            encodingSymbol = true;
                            subTabSymbol   = subTabOffset;
                            subTabNo       = i;

                            _mappingTable =
                                PCLSymbolSets.getMapArraySymbol();

                            mapSymSet = "Symbol";
                            mapSymSetType = "Fixed";
                        }
                        else if ((subTabPlatform == 3) && (subTabEncoding == 1))
                        {
                            encodingUnicode = true;
                            subTabUnicode   = subTabOffset;
                            subTabNo        = i;

                            _mappingTable =
                                PCLSymbolSets.getMapArray (symSetIndx,
                                                           symSetMapPCL);

                            if (symSetUnbound)
                                mapSymSetType = "Unbound";
                            else if (symSetUserSet)
                                mapSymSetType = "User-defined";
                            else if (symSetMapPCL)
                                mapSymSetType = "LaserJet (PCL)";
                            else
                                mapSymSetType = "Strict (standard))";

                            mapSymSet = PCLSymbolSets.getId (symSetIndx) +
                                        " (Kind1 = "    +
                                        PCLSymbolSets.getKind1 (symSetIndx) +
                                        "): " +
                                        PCLSymbolSets.getName (symSetIndx);
                        }
                    }
                    else
                    {
                        flagOK = false;

                        ToolSoftFontGenLog.logError (
                            _tableDonor, MessageBoxImage.Error,
                            "Error reading '" + tabName + "' encoding records");
                    }
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Check that the sub-table is for Unicode (text) or Symbol       //
            // encoding, and that it is format 4 (segment mapping to delta    //
            // values).                                                       //
            // Format of format 4 sub-table:                                  //
            //    ushort   format                                             //
            //    ushort   length                                             //
            //    ushort   language                                           //
            //    ushort   segCountx2              represents (segCount * 2)  //
            //    ushort   searchRange                                        //
            //    ushort   entrySelector                                      //
            //    ushort   rangeShift                                         //
            //    ushort   endCode[segCount]       array size = segCount      //
            //    ushort   reservedPad                                        //
            //    ushort   startCode[segCount]     array size = segCount      //
            //    short    idDelta[segCount]       array size = segCount      //
            //    ushort   idRangeOffset[segCount] array size = segCount      //
            //    ushort   glyphIdArray[ ]         array size = remainder     //
            //                                                                //
            // The four parallel arrays (each of size segCount) describe      //
            // ranges of character codes (via endCode and startCode values)   //
            // supported by the font.                                         //
            // This allows the font to contain non-contiguous code ranges.    //
            // The arrays then map these code ranges to the glyph IDs, which  //
            // usually form a contiguous set; ID=0 is reserved for the        //
            // 'missing glyph'.                                               //
            // The segments are in ascending order of endCode value.          //
            //                                                                //
            //----------------------------------------------------------------//

            if (flagOK)
            {
                if (encodingUnicode)
                {
                    ToolSoftFontGenLog.logNameAndValue (
                        _tableMapping, false, false,
                        "Platform:", "Windows");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableMapping, false, false,
                        "Encoding:", "Unicode BMP (UCS-2)");

                    if (symSetUnbound)
                    {
                        ToolSoftFontGenLog.logNameAndValue (
                            _tableMapping, false, false,
                            "Mapping:", "Unicode");
                    }
                    else
                    {
                        ToolSoftFontGenLog.logNameAndValue (
                            _tableMapping, false, false,
                            "Symbol set:", mapSymSet);

                        ToolSoftFontGenLog.logNameAndValue (
                            _tableMapping, false, false,
                            "Type:", mapSymSetType);
                    }

                    subTabOffset = tabOffset + subTabUnicode;
                }
                else if (encodingSymbol)
                {
                    ToolSoftFontGenLog.logNameAndValue (
                        _tableMapping, false, false,
                        "Platform:", "Windows");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableMapping, false, false,
                        "Encoding:", "Symbol");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableMapping, false, false,
                        "Symbol set:", mapSymSet);

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableMapping, false, false,
                        "Type:", mapSymSetType);

                    subTabOffset = tabOffset + subTabSymbol;
                }
                else
                {
                    flagOK = false;

                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "No Unicode or Symbol encoding sub-table of " +
                        " format 4 found for Windows platform in '" +
                        tabName + "' table");
                }

          //    if (flagOK && _logVerbose && ! symSetUnbound)
                if (flagOK && ! symSetUnbound)
                {
                    //----------------------------------------------------//
                    //                                                    //
                    // Display the mapping between the symbol set         //
                    // code-point, and the (Unicode) code-point of the    //
                    // target glyph.                                      //
                    //                                                    //
                    //----------------------------------------------------//

                    int len = _mappingTable.Length;
                    int indx;

                    bool firstEntry = true;
                    bool addLine;

                    for (int i = 0; i < len; i += 8)
                    {
                        addLine = false;

                        for (int j  = 0; j < 8; j++)
                        {
                            indx = i + j;

                            if (indx < len)
                            {
                                if (_mappingTable [indx] != 0xffff)
                                {
                                    addLine = true;
                                    j = 8;          // end loop //
                                }
                            }
                        }

                        if (addLine)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // At least one code-point in the current subset  //
                            // of 8 values is mapped to a glyph.              //
                            //                                                //
                            //------------------------------------------------//

                            StringBuilder mData = new StringBuilder ();

                            mData.Append ("U+");

                            for (int j = 0; j < 8; j++)
                            {
                                indx = i + j;

                                if (indx < len)
                                {
                                    mData.Append (
                                        " " + _mappingTable [indx].ToString ("x4"));
                                }
                            }

                            ToolSoftFontGenLog.logNameAndValue (
                                _tableMapping, firstEntry, false,
                                "Map 0x" + i.ToString ("x2") + "-->",
                                mData.ToString ());

                            firstEntry = false;
                        }
                    }
                }

                if (flagOK)
                {
                    flagOK = readBytesAsUInt16 ((int) subTabOffset,
                                                ref subTabFormat);
                }

                if (flagOK)
                {
                    if (subTabFormat == 4)
                    {
                        flagOK = readBytesAsUInt16 (-1, ref fmt4Length);
                        
                        if (flagOK)
                        {
                            if (fmt4Length < 14)
                            {
                                flagOK = false;

                                ToolSoftFontGenLog.logError (
                                    _tableDonor, MessageBoxImage.Error,
                                    "'" + tabName +
                                    "' sub-table header not within table");
                            }
                            else if ((subTabOffset + fmt4Length) >
                                (tabOffset + tabLength))
                            {
                                flagOK = false;

                                ToolSoftFontGenLog.logError (
                                    _tableDonor, MessageBoxImage.Error,
                                    "'" + tabName +
                                    "' sub-table not within table");
                           }
                        }

                        if (flagOK)
                            flagOK = readBytesAsUInt16 (-1, ref fmt4Lang);

                        if (flagOK)
                            flagOK = readBytesAsUInt16 (-1, ref fmt4SegCountx2);

                        if (flagOK)
                            flagOK = readBytesAsUInt16 (-1,
                                                        ref fmt4SearchRange);

                        if (flagOK)
                            flagOK = readBytesAsUInt16 (-1,
                                                        ref fmt4EntrySelector);

                        if (flagOK)
                            flagOK = readBytesAsUInt16 (-1, ref fmt4RangeShift);

                        if (flagOK)
                        {
                            short x;

                            fmt4SegCount = (ushort) (fmt4SegCountx2 / 2);

                            x = (short) (16 + (8 * fmt4SegCount));

                            if (x > fmt4Length)
                            {
                                flagOK = false;

                                ToolSoftFontGenLog.logError (
                                    _tableDonor, MessageBoxImage.Error,
                                    "'" + tabName +
                                    "' sub-table internally inconsistent");
                            }
                            else
                            {
                                if (_logVerbose)
                                {
                                    string encDesc;

                                    if (encodingSymbol)
                                        encDesc = "0 (Symbol)";
                                    else
                                        encDesc = "1 (Unicode BMP (UCS-2))";

                                    ToolSoftFontGenLog.logNameAndValue(
                                        _tableDonor, true, false,
                                        "DIAG: " + tabName + " subTab " + subTabNo + ":",
                                        "Format = 4 (Segment mapping to delta values)" +
                                        " sub-table found for");

                                    ToolSoftFontGenLog.logNameAndValue(
                                        _tableDonor, false, false,
                                        "",
                                        " Platform = 3 (Windows)" +
                                        "; Encoding = " + encDesc);
                                }

                                baseEndCode =
                                    (int) (subTabOffset + 14);
                                baseStartCode =
                                    (int) (baseEndCode + fmt4SegCountx2 + 2);
                                baseIdDelta =
                                    (int) (baseStartCode + fmt4SegCountx2);
                                baseIdRangeOffset =
                                    (int) (baseIdDelta + fmt4SegCountx2);
                                baseGlyphIdArray =
                                    (int) (baseIdRangeOffset + fmt4SegCountx2);
                                sizeGlyphIdArray =
                                    (int) (fmt4Length - x);
                            }
                        }
                    }
                    else
                    {
                        flagOK = false;

                        if (encodingUnicode)
                            ToolSoftFontGenLog.logError (
                                _tableDonor, MessageBoxImage.Error,
                                "'" + tabName + "' sub-table " + subTabNo +
                                " for Unicode is not format 4");
                        else
                            ToolSoftFontGenLog.logError (
                                _tableDonor, MessageBoxImage.Error,
                                "'" + tabName + "' sub-table " + subTabNo +
                                " for Symbol is not format 4");
                    }
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Construct a character-code to glyph-ID table for the requested //
            // mapping.                                                       //
            //                                                                //
            //----------------------------------------------------------------//

            if (flagOK)
            {
                int totChars = 0;

                byte[] indexEndCode       = new byte[fmt4SegCountx2];
                byte[] indexStartCode     = new byte[fmt4SegCountx2];
                byte[] indexIdDelta       = new byte[fmt4SegCountx2];
                byte[] indexIdRangeOffset = new byte[fmt4SegCountx2];
                byte[] glyphIdArray       = new byte[sizeGlyphIdArray];

                if (symSetUnbound)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Construct a character-code to glyph-ID table for the   //
                    // 16-bit character code range (0x0000 to 0xffff).        //
                    //                                                        //
                    //--------------------------------------------------------//

                    for (int i = 0; i < _sizeCharSet; i++)
                    {
                        _charData [i].setValues ((ushort) i, 0, false);
                    }
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Construct a character-code to glyph-ID table for the   //
                    // character code range of the requested mapping.         //
                    //                                                        //
                    //--------------------------------------------------------//

                    for (int i = 0; i < _sizeCharSet; i++)
                    {
                        _charData [i].setValues (_mappingTable [i], 0, false);
                    }
                }

                flagOK = readByteArray (baseEndCode,
                                        fmt4SegCountx2,
                                        ref indexEndCode);
                if (flagOK)
                    flagOK = readByteArray (baseStartCode,
                                            fmt4SegCountx2,
                                            ref indexStartCode);

                if (flagOK)
                    flagOK = readByteArray (baseIdDelta,
                                            fmt4SegCountx2,
                                            ref indexIdDelta);

                if (flagOK)
                    flagOK = readByteArray (baseIdRangeOffset,
                                            fmt4SegCountx2,
                                            ref indexIdRangeOffset);

                if (flagOK)
                    flagOK = readByteArray (baseGlyphIdArray,
                                            sizeGlyphIdArray,
                                            ref glyphIdArray);

                if (flagOK)
                {
                    int segx2;

                    uint endCode,
                           startCode;

                    ushort charCode,
                           mapCode = 0,
                           idRangeOffset;

                    short idDelta;

                    bool reqCode;

                    if (_logVerbose)
                    {
                        ToolSoftFontGenLog.logNameAndValue (
                            _tableDonor, true, false,
                            "DIAG: Segments",
                            "Count = " + fmt4SegCount.ToString ());

                        ToolSoftFontGenLog.logNameAndValue(
                            _tableMapping, false, false,
                            "",
                            "");
                    }

                    for (segx2 = 0; segx2 < fmt4SegCountx2; segx2 += 2)
                    {
                        byte[] ba2 = new byte[2];

                        ba2[0] = indexEndCode[segx2];
                        ba2[1] = indexEndCode[segx2 + 1];

                        endCode = byteArrayToUInt16 (ba2);

                        ba2[0] = indexStartCode[segx2];
                        ba2[1] = indexStartCode[segx2 + 1];

                        startCode = byteArrayToUInt16 (ba2);

                        ba2[0] = indexIdDelta[segx2];
                        ba2[1] = indexIdDelta[segx2 + 1];

                        idDelta = byteArrayToInt16 (ba2);

                        ba2[0] = indexIdRangeOffset[segx2];
                        ba2[1] = indexIdRangeOffset[segx2 + 1];

                        idRangeOffset = byteArrayToUInt16 (ba2);

                        if (_logVerbose)
                        {
                            ToolSoftFontGenLog.logNameAndValue (
                                _tableDonor, false, false,
                                "DIAG: Segment",
                                segx2 / 2 +
                                ": U+" + startCode.ToString ("x4") +
                                "->U+" + endCode.ToString ("x4") +
                                " (" + startCode.ToString () +
                                "->" + endCode.ToString () + ")" +
                                "; d = " + idDelta +
                                "; r_O = " + idRangeOffset);
                        }

                        for (uint i = startCode; i <= endCode; i++)
                        {
                            totChars++;
                            charCode = (ushort) i;
                            reqCode = false;

                            if (symSetUnbound)
                            {
                                if (charCode != 0xffff)
                                {
                                    reqCode = true;
                                    mapCode = (ushort)i;
                                }
                            }
                            else
                            {
                                for (ushort j = 0; j < _sizeCharSet; j++)
                                {
                                    if ((charCode != 0xffff) &&
                                        (charCode == _mappingTable [j]))
                                    {
                                        reqCode = true;
                                        mapCode = j;
                                        j = (ushort)_sizeCharSet;
                                    }
                                }
                            }

                            if (reqCode)
                            {
                                //--------------------------------------------//
                                //                                            //
                                // The current character code in the segment  //
                                // is one that is part of the required set.   //
                                // Obtain the associated glyph ID, and store  //
                                // this in the 'character-code to glyph-index'//
                                // table for the 8-bit character code range.  //
                                //                                            //
                                //--------------------------------------------//

                                ushort glyphIndex = 0;

                                if (idRangeOffset == 0)
                                {
                                    //----------------------------------------//
                                    //                                        //
                                    // The idDelta value is added directly to //
                                    // the character code to obtain the glyph //
                                    // index (modulo 65536).                  //
                                    //                                        //
                                    //----------------------------------------//

                                    glyphIndex = (ushort)((idDelta + charCode)
                                                           % 65536);

                                    if (_logVerbose)
                                    {
                                        ToolSoftFontGenLog.logNameAndValue (
                                            _tableMapping, false, false,
                                            "DIAG: Codepoint",
                                            "U+" +
                                            charCode.ToString ("x4") +
                                            " (" + charCode.ToString () + ")" +
                                            "; glyph = " + glyphIndex +
                                            "; map to 0x" +
                                            mapCode.ToString ("x2") +
                                            " (" + mapCode.ToString () + ")");
                                    }
                                }
                                else
                                {
                                    //----------------------------------------//
                                    //                                        //
                                    // This bit is really quite complicated!  //
                                    // The character code offset from         //
                                    // startCode is added to the idRangeOffset//
                                    // value, and the sum then used as an     //
                                    // offset from the current location       //
                                    // within idRangeOffset itself to index   //
                                    // out the correct glyphIdArray value.    //
                                    //                                        //
                                    // This not very obvious indexing works   //
                                    // because glyphIdArray immediately       //
                                    // follows idRangeOffset in the sub-table //
                                    // structure.                             //
                                    //                                        //
                                    // However, as we've copied parts of this //
                                    // sub-table separately, we have to       //
                                    // modify this indexing somewhat.         //
                                    //                                        //
                                    //----------------------------------------//

                                    int j = (int)(idRangeOffset +
                                            ((charCode - startCode) * 2) -
                                            (fmt4SegCountx2 - segx2));

                                    if (j >= sizeGlyphIdArray)
                                    {
                                        flagOK = false;

                                        ToolSoftFontGenLog.logError (
                                            _tableDonor, MessageBoxImage.Error,
                                            "GlyphId index " + j +
                                            " >= array size " +
                                            sizeGlyphIdArray);
                                    }
                                    else
                                    {
                                        ba2 [0] = glyphIdArray [j];
                                        ba2 [1] = glyphIdArray [j + 1];

                                        glyphIndex = byteArrayToUInt16 (ba2);

                                        if (glyphIndex != 0)
                                            glyphIndex =
                                                (ushort)((idDelta + glyphIndex)
                                                          % 65536);

                                        if (_logVerbose)
                                        {
                                            ToolSoftFontGenLog.logNameAndValue (
                                                _tableMapping, false, false,
                                                "DIAG: Codepoint",
                                                "U+" +
                                                charCode.ToString ("x4") +
                                                " (" + charCode.ToString () +
                                                ")" +
                                                "; glyph = " + glyphIndex +
                                                "; map to 0x" +
                                                mapCode.ToString ("x2") +
                                                " (" + mapCode.ToString () +
                                                ")");
                                        }
                                    }
                                }

                                //--------------------------------------------//
                                //                                            //
                                // Update the referenced character in the     //
                                // target character set with the glyph        //
                                // details.                                   //
                                // Then check to see if any other characters  //
                                // have also been mapped to the same          //
                                // code-point (and hence the same glyph) and  //
                                // update these as well; note that this is    //
                                // unlikely to occur.                         //
                                //                                            //
                                //--------------------------------------------//

                                _charData [mapCode].setValues (charCode,
                                                             glyphIndex,
                                                             true);

                                if (! symSetUnbound)
                                {
                                    for (int k = mapCode + 1;
                                         k < _sizeCharSet;
                                         k++)
                                    {
                                        if (charCode == _mappingTable [k])
                                        {
                                            _charData [k].setValues (charCode,
                                                                    glyphIndex,
                                                                    true);
                                        }
                                    }
                                }
                            }                         // end of 'if(reqCode)' //

                            if (!flagOK) i = endCode + 1;   // force end loop //
                        }                             // end of inner 'for'   //

                        if (!flagOK) segx2 = fmt4SegCountx2;// force end loop //
                    }                                 // end of outer 'for'   //
                }                                     // end of 'if(flagOK)'  //

                if (flagOK)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Determine number of characters and first and last      //
                    // codes within the target character set range.           //
                    //                                                        //
                    //--------------------------------------------------------//

                    ushort codepoint = 0,
                           glyphId   = 0;

                    string hexCt = "";

                    bool glyphPresent = false,
                         firstFound   = false,
                         firstMissing = true;

                    _cmap_numChars = 0;
                    _cmap_missChars = 0;
                    _cmap_firstCode = 0;
                    _cmap_lastCode = 0;

                    if (_sizeCharSet < 257)
                        hexCt = "x2";
                    else
                        hexCt = "x4";

                    for (int i = 0; i < _sizeCharSet; i++)
                    {
                        _charData[i].getValues(ref codepoint,
                                               ref glyphId,
                                               ref glyphPresent);

                        if (glyphPresent)
                        {
                            _cmap_numChars++;
                            _cmap_lastCode = (ushort) i;

                            if (!firstFound)
                            {
                                firstFound = true;
                                _cmap_firstCode = (ushort) i;
                            }
                        }
                        else if (codepoint != 0xffff)
                        {
                            _cmap_missChars++;

                            if (!symSetUnbound)
                            {
                                ToolSoftFontGenLog.logNameAndValue(
                                _tableMapping, firstMissing, false,
                                "Character code",
                                "0x" + i.ToString(hexCt) +
                                " (" + i.ToString() +
                                ")" +
                                " mapped to Codepoint U+" +
                                codepoint.ToString("x4") +
                                " (" + codepoint.ToString() +
                                ")" +
                                " has no associated glyph");

                                if (firstMissing)
                                    firstMissing = false;
                            }
                        }
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Log details of character counts, etc.                  //
                    //                                                        //
                    //--------------------------------------------------------//

                    if ((_cmap_missChars != 0) && !symSetUnbound)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Output message if any of the expected graphic      //
                        // characters do not have a glyph.                    //
                        //                                                    //
                        //----------------------------------------------------//

                        ToolSoftFontGenLog.logError(
                            _tableMapping, MessageBoxImage.Warning,
                            _cmap_missChars.ToString() +
                            " of the characters in the selected" +
                            " character (symbol) set" +
                            " do not have a glyph associated with the" +
                            " mapped codepoint in the donor TrueType font");
                    }

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableMapping, true, false,
                        "Codepoints:",
                        "Total      = " +
                        totChars.ToString ("D").PadLeft(5, ' ') +
                        " (0x" + totChars.ToString ("x4") + ")");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableMapping, false, false,
                        "",
                        "Mapped     = " +
                        _cmap_numChars.ToString ("D").PadLeft (5, ' ') +
                        " (0x" + _cmap_numChars.ToString ("x4") + ")");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableMapping, false, false,
                        "",
                        "Missing    = " +
                        _cmap_missChars.ToString ("D").PadLeft (5, ' ') +
                        " (0x" + _cmap_missChars.ToString ("x4") + ")");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableMapping, false, false,
                        "Mapped Set:",
                        "First Code = " +
                        _cmap_firstCode.ToString ("D").PadLeft (5, ' ') +
                        " (0x" + _cmap_firstCode.ToString ("x4") + ")");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableMapping, false, false,
                        "",
                        "Last Code  = " +
                        _cmap_lastCode.ToString ("D").PadLeft (5, ' ') +
                        " (0x" + _cmap_lastCode.ToString ("x4") + ")");
                }
            }

            if (encodingSymbol)
                symbolMapping = true;
            else
                symbolMapping = false;

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d D a t a _ h e a d                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        //                                                                    //
        // Read the (mandatory) 'head' (font header) table to obtain:         //
        //    unitsPerEm                                                      //
        //    xMin                                                            //
        //    yMin                                                            //
        //    xMax                                                            //
        //    yMax                                                            //
        //    indexToLocFormat                                                //
        // The return value indicates success or failure of the reads.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readData_head()
        {
            string tabName = "head";

            bool flagOK = true;

            uint tabOffset = 0,
                   tabLength = 0,
                   reqLength;

            uint tabVersion = 0;

            flagOK = true;

            _tab_head.getByteRange (ref tabOffset, ref tabLength);
            reqLength = 54;

            if (tabLength < reqLength)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Length of '" + tabName + "' table too small: " +
                    tabLength + " < " + reqLength);
            }

            if (flagOK)
            {
                flagOK = readBytesAsUInt32 ((int) tabOffset, ref tabVersion);

                if (!flagOK)
                {
                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Error reading '" + tabName + "' table version");
                }
                else if (tabVersion != cTabVer_head)
                {
                    flagOK = false;

                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Wrong '" + tabName + "' table version = 0x" +
                        tabVersion.ToString ("x8") + "\r\n\r\n" +
                        "Expected version = 0x" +
                        cTabVer_head.ToString ("x8"));
                }
                else
                {
                    flagOK = readBytesAsUInt16 ((int) (tabOffset + 18),
                                                ref _head_unitsPerEm);

                    if (flagOK)
                    {
                        flagOK = readBytesAsInt16 ((int) (tabOffset + 36),
                                                   ref _head_xMin);
                    }

                    if (flagOK)
                    {
                        flagOK = readBytesAsInt16 (-1,
                                                   ref _head_yMin);
                    }

                    if (flagOK)
                    {
                        flagOK = readBytesAsInt16 (-1,
                                                   ref _head_xMax);
                    }

                    if (flagOK)
                    {
                        flagOK = readBytesAsInt16 (-1,
                                                   ref _head_yMax);
                    }

                    if (flagOK)
                    {
                        flagOK = readBytesAsInt16 ((int) (tabOffset + 50),
                                                   ref _head_indxLocFmt);
                    }
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Diagnostics.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            if (_logVerbose)
            {
                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, true, false,
                    "DIAG: table = " + tabName + ":",
                    "unitsPerEm = " + _head_unitsPerEm.ToString ());

                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, false, false,
                    "DIAG: table = " + tabName + ":",
                    "xMin = " + _head_xMin.ToString ());

                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, false, false,
                    "DIAG: table = " + tabName + ":",
                    "yMin = " + _head_yMin.ToString ());

                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, false, false,
                    "DIAG: table = " + tabName + ":",
                    "xMax = " + _head_xMax.ToString ());

                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, false, false,
                    "DIAG: table = " + tabName + ":",
                    "yMax = " + _head_yMax.ToString ());

                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, false, false,
                    "DIAG: table = " + tabName + ":",
                    "indexToLocFormat = " + _head_indxLocFmt.ToString ());
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d D a t a _ h h e a                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read the (mandatory) 'hhea' (horizontal header) table to obtain:   //
        //    Ascender                                                        //
        //    Descender                                                       //
        //    LineGap                                                         //
        //    numberofHMetrics                                                //
        // The return value indicates success or failure of the reads.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readData_hhea()
        {
            string tabName = "hhea";

            bool flagOK = true;

            uint tabOffset = 0,
                   tabLength = 0,
                   reqLength;

            uint tabVersion = 0;

            flagOK = true;

            _tab_hhea.getByteRange (ref tabOffset, ref tabLength);
            reqLength = 36;

            if (tabLength < reqLength)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Length of '" + tabName + "' table too small: " +
                    tabLength + " < " + reqLength);
            }

            if (flagOK)
            {
                flagOK = readBytesAsUInt32 ((int) tabOffset, ref tabVersion);

                if (!flagOK)
                {
                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Error reading '" + tabName + "' table version");
                }
                else if (tabVersion != cTabVer_hhea)
                {
                    flagOK = false;

                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Wrong '" + tabName + "' table version = 0x" +
                        tabVersion.ToString ("x8") + "\r\n\r\n" +
                        "Expected version = 0x" +
                        cTabVer_hhea.ToString ("x8"));
                }
                else
                {
                    flagOK = readBytesAsInt16 ((int) (tabOffset + 4),
                                               ref _hhea_ascender);
                    
                    if (flagOK)
                    {
                        flagOK = readBytesAsInt16 (-1,
                                                   ref _hhea_descender);
                    }

                    if (flagOK)
                    {
                        flagOK = readBytesAsInt16 (-1,
                                                   ref _hhea_lineGap);
                    }

                    if (flagOK)
                    {
                        flagOK = readBytesAsUInt16 ((int) (tabOffset + 34),
                                                    ref _hhea_numHMetrics);
                    }
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Diagnostics.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            if (_logVerbose)
            {
                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, true, false,
                    "DIAG: table = " + tabName + ":",
                    "Ascender = " + _hhea_ascender.ToString ());

                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, false, false,
                    "DIAG: table = " + tabName + ":",
                    "Descender = " + _hhea_descender.ToString ());

                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, false, false,
                    "DIAG: table = " + tabName + ":",
                    "LineGap = " + _hhea_lineGap.ToString ());

                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, false, false,
                    "DIAG: table = " + tabName + ":",
                    "numberOfHMetrics = " + _hhea_numHMetrics.ToString ());
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d D a t a _ h m t x                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read the (mandatory) 'hmtx' (horizontal metrics) table to obtain   //
        // advance widths and left-side bearings for each glyph in the glyph  //
        // table.                                                             //
        // The return value indicates success or failure of the reads.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readData_hmtx()
        {
            string tabName = "hmtx";

            bool flagOK = true;

            uint tabOffset = 0,
                   tabLength = 0,
                   reqLength;

            int hMetricsArrayLen = 0,
                  lsbArrayLen = 0;
            int hMetricsArraySize = 0,
                  lsbArraySize = 0;

            flagOK = true;

            _tab_hmtx.getByteRange (ref tabOffset, ref tabLength);

            hMetricsArrayLen = _hhea_numHMetrics;
            lsbArrayLen      = _maxp_numGlyphs - _hhea_numHMetrics;

            hMetricsArraySize = 4 * hMetricsArrayLen;
            lsbArraySize      = 2 * lsbArrayLen;

            reqLength = (uint) (hMetricsArraySize + lsbArraySize);

            if (_hhea_numHMetrics < 1)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Number of HMetrics " + _hhea_numHMetrics + " < 1");
            }
            else if ((_maxp_numGlyphs - _hhea_numHMetrics) < 0)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Number of glyphs " + _maxp_numGlyphs +
                    " < number of HMetrics " + _hhea_numHMetrics);
            }
            else if (tabLength < reqLength)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Length of '" + tabName + "' table too small: " +
                    tabLength + " < " + reqLength);
            }
             

            if (flagOK)
            {
                byte[] hMetricsArray = new byte[hMetricsArraySize];
                byte[] lsbArray      = new byte[lsbArraySize];

                flagOK = readByteArray ((int) tabOffset,
                                        hMetricsArraySize,
                                        ref hMetricsArray);

                if (flagOK)
                {
                    flagOK = readByteArray (-1,
                                            lsbArraySize,
                                            ref lsbArray);
                }

                if (flagOK)
                {
                    ushort glyphId = 0,
                           advance = 0;

                    short lsb;

                    uint advanceOffset,
                           lsbOffset;

                    for (int indx = 0; indx < hMetricsArrayLen; indx++)
                    {
                        advanceOffset = (uint) (indx * 4);
                        lsbOffset     = advanceOffset + 2;

                        advance = (ushort) ((hMetricsArray[advanceOffset] << 8) +
                                             hMetricsArray[advanceOffset + 1]);

                        lsb = (short) ((hMetricsArray[lsbOffset] << 8) +
                                        hMetricsArray[lsbOffset + 1]);

                        _glyphData[indx].setMetricsH (advance, lsb);
                    }

                    for (int indx = 0; indx < lsbArrayLen; indx++)
                    {
                        lsbOffset = (uint) (indx * 2);

                        lsb = (short) ((lsbArray[lsbOffset] << 8) +
                                        lsbArray[lsbOffset + 1]);
                        
                        glyphId = (ushort) (hMetricsArrayLen + indx);  
                        
                        _glyphData[glyphId].setMetricsH (advance, lsb);
                    }
                }
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d D a t a _ l o c a _ g l y f                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read the (mandatory) 'loca' (location) and 'glyf' (glyph data)     //
        // tables to obtain the offset and length of the glyph data, and an   //
        // indication of whether or not the glyph is composite, for each      //
        // glyph in the glyph table.                                          //
        // The return value indicates success or failure of the reads.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readData_loca_glyf()
        {
            bool flagOK = true;
            bool composite = false;

            uint locaOffset = 0,
                   locaLength = 0,
                   glyfOffset = 0,
                   glyfLength = 0;

            uint offsetThis = 0,
                   offsetNext = 0,
                   entryLen   = 0,
                   entryOffset = 0;

            ushort offsetTemp = 0;

            short numContours = 0;

            flagOK = true;

            //----------------------------------------------------------------//
            //                                                                //
            // For each glyph, obtain offset, length and composite indicator, //
            // and store these details in the glyph table.                    //
            //                                                                //
            //----------------------------------------------------------------//

            _tab_loca.getByteRange (ref locaOffset, ref locaLength);
            _tab_glyf.getByteRange (ref glyfOffset, ref glyfLength);

            if (_logVerbose)
            {
                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, true, false,
                    "DIAG: Glyphs",
                    "Count = " +
                    _maxp_numGlyphs.ToString ());
            }

            for (int glyphId = 0;
                 (glyphId < _maxp_numGlyphs) && flagOK;
                 glyphId++)
            {
                if (_head_indxLocFmt != 0)
                {
                    flagOK = readBytesAsUInt32 (
                        (int) (locaOffset + (4 * glyphId)),
                        ref offsetThis);

                    if (flagOK)
                    {
                        flagOK = readBytesAsUInt32 (-1, ref offsetNext);
                    }
                }
                else
                {
                    flagOK = readBytesAsUInt16 (
                        (int) (locaOffset + (2 * glyphId)),
                        ref offsetTemp);

                    if (flagOK)
                    {
                        offsetThis = (uint) (offsetTemp * 2);

                        flagOK = readBytesAsUInt16 (-1, ref offsetTemp);

                        if (flagOK)
                            offsetNext = (uint) (offsetTemp * 2);
                    }
                }

                if (flagOK)
                {
                    entryOffset = glyfOffset + offsetThis;
                    entryLen = offsetNext - offsetThis;
                }

                //------------------------------------------------------------//
                //                                                            //
                // Check if this is a composite glyph.                        //
                //                                                            //
                //------------------------------------------------------------//

                if (flagOK)
                {
                    if (entryLen == 0)
                    {
                        composite = false;
                    }
                    else
                    {
                        flagOK = readBytesAsInt16 ((int) entryOffset,
                                                   ref numContours);

                        if (numContours >= 0)
                            composite = false;
                        else
                            composite = true;
                    }
                }

                //------------------------------------------------------------//
                //                                                            //
                // Store data.                                                //
                //                                                            //
                //------------------------------------------------------------//

                _glyphData[glyphId].setLocation (entryOffset, entryLen);
                _glyphData[glyphId].setFlags (composite);

                //------------------------------------------------------------//
                //                                                            //
                // Diagnostics.                                               //
                //                                                            //
                //------------------------------------------------------------//

                if (_logVerbose)
                {
                    short leftSideBearing = 0,
                          topSideBearing = 0;
                    ushort advanceWidth = 0,
                           advanceHeight = 0;

                    _glyphData[glyphId].getMetricsH (ref advanceWidth,
                                                     ref leftSideBearing);

                    _glyphData[glyphId].getMetricsV (ref advanceHeight,
                                                     ref topSideBearing);
                    
                    if (composite)
                    {
                        ToolSoftFontGenLog.logNameAndValue (
                            _tableDonor, false, false,
                            "DIAG: Glyph",
                            glyphId.ToString () +
                            ": offset/len = " +
                            entryOffset.ToString () +
                            "/" + entryLen +
                            "; advW = " +
                            advanceWidth.ToString () +
                            "; lsb = " +
                            leftSideBearing.ToString () +
                            "; advH = " +
                            advanceHeight.ToString () +
                            "; tsb = " +
                            topSideBearing.ToString () +
                            "; cmp");
                    }
                    else
                    {
                        ToolSoftFontGenLog.logNameAndValue (
                            _tableDonor, false, false,
                            "DIAG: Glyph",
                            glyphId.ToString () +
                            ": offset/len = " +
                            entryOffset.ToString () +
                            "/" + entryLen +
                            "; advW = " +
                            advanceWidth.ToString () +
                            "; lsb = " +
                            leftSideBearing.ToString () +
                            "; advH = " +
                            advanceHeight.ToString () +
                            "; tsb = " +
                            topSideBearing.ToString ());
                    }
                }
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d D a t a _ m a x p                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read the (mandatory) 'maxp' (maximum profile) table to obtain:     //
        //    numGlyphs                                                       //
        //    maxComponentDepth                                               //
        // The return value indicates success or failure of the reads.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readData_maxp()
        {
            string tabName = "maxp";

            bool flagOK = true;

            uint tabOffset = 0,
                   tabLength = 0,
                   reqLength;

            uint tabVersion = 0;

            flagOK = true;

            _tab_maxp.getByteRange (ref tabOffset, ref tabLength);
            reqLength = 32;

            if (tabLength < reqLength)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Length of '" + tabName + "' table too small: " +
                    tabLength + " < " + reqLength);
            }

            if (flagOK)
            {
                flagOK = readBytesAsUInt32 ((int) tabOffset, ref tabVersion);

                if (!flagOK)
                {
                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Error reading '" + tabName + "' table version");
                }
                else if (tabVersion != cTabVer_maxp)
                {
                    flagOK = false;

                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Wrong '" + tabName + "' table version = 0x" +
                        tabVersion.ToString ("x8") + "\r\n\r\n" +
                        "Expected version = 0x" +
                        cTabVer_maxp.ToString ("x8"));
                }
                else
                {
                    flagOK = readBytesAsUInt16 ((int) (tabOffset + 4),
                                                ref _maxp_numGlyphs);
                    if (flagOK)
                    {
                        flagOK = readBytesAsUInt16 ((int) (tabOffset + 30),
                                                    ref _maxp_maxCompDepth);
                    }
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Diagnostics.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            if (_logVerbose)
            {
                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, true, false,
                    "DIAG: table = " + tabName + ":",
                    "numGlyphs = " + _maxp_numGlyphs.ToString ());

                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, false, false,
                    "DIAG: table = " + tabName + ":",
                    "maxComponentDepth = " +
                    _maxp_maxCompDepth.ToString ());
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d D a t a _ n a m e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read the (mandatory) 'name' (naming) table in order to obtain      //
        // descriptive details (e.g. copyright, font family name, vendor).    //
        // Note that none of these details are required for conversion        //
        // purposes (they are logged for information), except the             //
        // FullFontName which provides a default TypeFace name for PCL fonts  //
        // (in case the PCLT table is not present).                           //
        // The return value indicates success or failure of the reads.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readData_name(bool getTTCData,
                                      ref string fullFontName)
        {
            string tabName = "name";

            bool flagOK = true;

            uint tabOffset = 0,
                   tabLength = 0,
                   reqLength;

            ushort nameRecCount = 0,
                   nameRecPlatform = 0,
                   nameRecEncoding = 0,
                   nameRecLanguage = 0,
                   nameRecNameId = 0,
                   nameRecLength = 0,
                   nameRecOffset = 0;

            ushort tabFormat = 0,
                   stringsOffset = 0;

            flagOK = true;

            _tab_name.getByteRange (ref tabOffset, ref tabLength);

            //----------------------------------------------------------------//
            //                                                                //
            // Get table details.                                             //
            // Format:                                                        //
            //    ushort   format              = 0 or 1                       //
            //    ushort   count               Number of name records         //
            //    ushort   stringOffset        Offset from start of table to  //
            //                                start of string storage         //
            //    NameRec  nameRec[count]      Name records array             // 
            //    ushort   langCount           Number of language records     //
            //    LangRec  langRec[langCount]  Language records array         //
            //    Variable                     Storage for the string data    //                       
            //                                                                //
            // The language count and language array items are not present in //
            // the format 0 table.                                            //
            //                                                                //
            //----------------------------------------------------------------//

            reqLength = 6;

            if (tabLength < reqLength)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Length of '" + tabName + "' table too small: " +
                    tabLength + " < " + reqLength);
            }

            if (flagOK)
            {
                flagOK = readBytesAsUInt16 ((int) tabOffset, ref tabFormat);

                if (!flagOK)
                {
                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Error reading '" + tabName + "' table format");
                }
                else if ((tabFormat != 0) && (tabFormat != 1))
                {
                    flagOK = false;

                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "'" + tabName + "' table format " +
                        tabFormat.ToString ("x4") + " is not 0 or 1");
                }
                else
                {
                    flagOK = readBytesAsUInt16 ((int) (tabOffset + 2),
                                                ref nameRecCount);
                    if (flagOK)
                    {
                        flagOK = readBytesAsUInt16 ((int) (tabOffset + 4),
                                                    ref stringsOffset);
                    }
                    
                    if (flagOK)
                    {
                        uint minLen = 6 + (12 * (uint) nameRecCount);

                        if (minLen > tabLength)
                        {
                            flagOK  = false;

                            ToolSoftFontGenLog.logError (
                                _tableDonor, MessageBoxImage.Error,
                                "'" + tabName + "' table length " + tabLength +
                                " too short for " + nameRecCount  +
                                " encoding records");
                        }
                        else if ((uint)stringsOffset > tabLength)
                        {
                            flagOK  = false;

                            ToolSoftFontGenLog.logError (
                                _tableDonor, MessageBoxImage.Error,
                                "string storage offset " + stringsOffset +
                                "incompatible with '" + tabName +
                                "' table length " + tabLength);
                        }
                    }
                }
            }

            if (_logVerbose)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Log details of table header.                               //
                //                                                            //
                //------------------------------------------------------------//

                ToolSoftFontGenLog.logNameAndValue(
                    _tableDonor, false, false,
                    "DIAG: table = " + tabName + ":",
                    "format = " + tabFormat.ToString());

                ToolSoftFontGenLog.logNameAndValue(
                    _tableDonor, false, false,
                    "DIAG: table = " + tabName + ":",
                    "count = " +
                    nameRecCount.ToString());

                ToolSoftFontGenLog.logNameAndValue(
                    _tableDonor, false, false,
                    "DIAG: table = " + tabName + ":",
                    "stringOffset = " +
                    stringsOffset.ToString());

                if (!getTTCData)
                {
                    ToolSoftFontGenLog.logNameAndValue(
                        _tableDonor, true, false,
                        "DIAG: table = " + tabName + ":",
                        "Search nameRecord array for records associated with:");

                    ToolSoftFontGenLog.logNameAndValue(
                        _tableDonor, false, false,
                        "",
                        "Platform = 3 (Windows)" +
                        "; Encoding = 0 (Symbol) or 1 (Unicode BMP (UCS-2))");

                    ToolSoftFontGenLog.logNameAndValue(
                        _tableDonor, false, true,
                        "",
                        "Language = 0x0409 (English (US))");
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Read array of name records, which provide details of the       //
            // sub-tables present. There should be 'nameRecCount' of these.   //
            // These entries follow the table header.                         //
            // Format of each entry:                                          //
            //    ushort   platformID   Platform ID                           //
            //    ushort   encodingID   Platform-specific encoding ID         //
            //    ushort   languageID   Language ID                           //
            //    ushort   nameID       Name ID                               // 
            //    ushort   length       String length (in bytes)              // 
            //    ushort   offset       String offset from start of storage   //
            //                          area                                  //
            //                                                                //
            //----------------------------------------------------------------//

            if (flagOK)
            {
            //  if (logIdData)
                {
                }

                const int maxNameRecStrLen = 2048;

                byte[] tempBuf = new byte[maxNameRecStrLen];

                int textOffset;

                string nameText = "";

                bool logIdData;
                
                for (int i = 0; (i < nameRecCount) && flagOK; i++)
                {
                    flagOK = readBytesAsUInt16 (
                        (int) (tabOffset + 6 + (12 * i)),
                        ref nameRecPlatform);

                    if (flagOK)
                        flagOK = readBytesAsUInt16 (-1, ref nameRecEncoding);

                    if (flagOK)
                        flagOK = readBytesAsUInt16 (-1, ref nameRecLanguage);

                    if (flagOK)
                        flagOK = readBytesAsUInt16 (-1, ref nameRecNameId);

                    if (flagOK)
                        flagOK = readBytesAsUInt16 (-1, ref nameRecLength);

                    if (flagOK)
                        flagOK = readBytesAsUInt16 (-1, ref nameRecOffset);
            
                    if (flagOK)
                    {
                        if ((uint)(stringsOffset + nameRecOffset + nameRecLength)
                            > tabLength)
                        {
                            flagOK  = false;

                            ToolSoftFontGenLog.logError (
                                _tableDonor, MessageBoxImage.Error,
                                "record of length " + nameRecLength +
                                " at offset " + (stringsOffset + nameRecOffset) +
                                " extends past end of '" + tabName +
                                "' table of length " + tabLength);
                        }
                    }
                    else
                    {
                        ToolSoftFontGenLog.logError (
                            _tableDonor, MessageBoxImage.Error,
                            "Error reading '" + tabName + "' record");
                    }

                    if (flagOK)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Check whether the name record is of interest:      //
                        //    platform = 3 (Windows)                          //
                        //    encoding = 0 (Symbol)  or                       //
                        //               1 (Unicode)                          //
                        //    language = 1033 (0x0409 = English (US)          //
                        //                     is always expected)            //
                        //                                                    //
                        //----------------------------------------------------//

                        if ((nameRecPlatform == 3) &&
                            ((nameRecEncoding == 0) ||
                             (nameRecEncoding == 1)) &&
                            nameRecLanguage == 0x0409)
                        {
                            bool nameIsFullFontName = false;

                            textOffset = (int) (tabOffset + stringsOffset +
                                                  nameRecOffset);

                            logIdData = true;

                            switch (nameRecNameId)
                            {
                                case 0:
                                    nameText = "Copyright:";
                                    break;

                                case 1:
                                    nameText = "Font Family:";
                                    break;

                                case 2:
                                    nameText = "Font Subfamily:";
                                    break;

                                case 3:
                                    nameText = "Font ID:";
                                    break;

                                case 4:
                                    nameText = "Full font name:";
                                    nameIsFullFontName = true;
                                    break;

                                case 5:
                                    nameText = "Version:";
                                    break;

                                case 6:
                                    nameText = "Postscript name:";
                                    break;

                                case 7:
                                    nameText = "Trademark:";
                                    break;

                                case 8:
                                    nameText = "Manufacturer:";
                                    break;

                                case 9:
                                    nameText = "Designer:";
                                    break;

                                case 10:
                                    nameText = "Description:";
                                    break;

                                case 11:
                                    nameText = "Vendor URL:";
                                    break;

                                case 12:
                                    nameText = "Designer URL:";
                                    break;

                                case 13:
                                    nameText = "License:";
                                    break;

                                case 14:
                                    nameText = "License URL:";
                                    break;

                                case 15:
                                    nameText = "Reserved:";
                                    break;

                                case 16:
                                    nameText = "Typo. Family:";
                                    break;

                                case 17:
                                    nameText = "Typo. Subfamily:";
                                    break;

                                case 18:
                                    nameText = "Compatible Full:";
                                    break;

                                case 19:
                                    nameText = "Sample text:";
                                    break;

                                case 20:
                                    nameText = "Postscript CID:";
                                    break;

                                case 21:
                                    nameText = "WWS Family:";
                                    break;

                                case 22:
                                    nameText = "WWS Subfamily:";
                                    break;

                                case 23:
                                    nameText = "Lt. Back Palette:";
                                    break;

                                case 24:
                                    nameText = "Dk. Back Palette:";
                                    break;

                                default:
                                    logIdData = false;
                                    break;
                            }

                            if (logIdData)
                            {
                                string recVal;
                                
                                if (nameRecLength < maxNameRecStrLen)
                                {
                                    flagOK = readByteArray (textOffset,
                                                            nameRecLength,
                                                            ref tempBuf);

                                    if (tempBuf[0] == 0x00)
                                    {
                                        //------------------------------------//
                                        //                                    //
                                        // Value is (probably) held as a      //
                                        // Big-Endian wide (16-bit) character //
                                        // string.                            //
                                        // As this process (for now) runs on  //
                                        // Intel (Little-Endian), need to     //
                                        // reverse the bytes.                 //
                                        // Note: don't do this if on Motorola.//
                                        //                                    //
                                        //------------------------------------//

                                        byte byteMS;

                                        for (int j = 0;
                                             j < nameRecLength;
                                             j += 2)
                                        {
                                            byteMS = tempBuf[j];

                                            tempBuf[j] = tempBuf[j + 1];
                                            tempBuf[j + 1] = byteMS;
                                        }

                                        UnicodeEncoding unicode =
                                            new UnicodeEncoding ();

                                        recVal = unicode.GetString (
                                            tempBuf, 0, nameRecLength);
                                    }
                                    else
                                    {
                                        //------------------------------------//
                                        //                                    //
                                        // Value is (probably) held as a      //
                                        // single-byte character string.      //
                                        //                                    //
                                        //------------------------------------//

                                        ASCIIEncoding ascii =
                                            new ASCIIEncoding ();

                                        recVal = ascii.GetString (
                                            tempBuf, 0, nameRecLength);
                                    }

                                    if (!getTTCData)
                                    {
                                        ToolSoftFontGenLog.logNameAndValue (
                                            _tableDonor, false, false,
                                            nameText,
                                            recVal.ToString ());
                                    }
                                    else if (nameIsFullFontName && _logVerbose)
                                    {
                                        ToolSoftFontGenLog.logNameAndValue (
                                            _tableDonor, false, false,
                                            "DIAG: font name",
                                            recVal.ToString ());
                                    }

                                    if (nameIsFullFontName)
                                    {
                                        _name_fullFontnameStr = recVal;

                                        fullFontName = recVal;

                                        ASCIIEncoding ascii =
                                            new ASCIIEncoding ();

                                        byte[] temp = ascii.GetBytes (recVal);

                                        int len = temp.Length;

                                        if (len > cSizeFontname)
                                            len = cSizeFontname;

                                        for (int j =0; j<len; j++)
                                        {
                                            _name_fullFontname[j] = temp[j];
                                        }

                                        for (int j =len; j<cSizeFontname; j++)
                                        {
                                            _name_fullFontname[j] = 0x20;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d D a t a _ O S _ 2                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read the (mandatory) 'OS/2' (OS/2 and Windows Metrics) table to    //
        // obtain:                                                            //
        //    xAvgCharWidth                                                   //
        //    usWeightClass                                                   //
        //    usWidthClass                                                    //
        //    fsType                                                          //
        //    panose                                                          //
        //    sxHeight                                                        //
        //    sTypoDescender                                                  //
        //                                                                    //
        // If the fsType flags are such that embedding is restricted, log a   //
        // warning.                                                           //
        // The return value indicates success or failure of the reads.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readData_OS_2()
        {
            string tabName = "OS/2";

            bool flagOK = true;

            uint tabOffset = 0,
                   tabLength = 0,
                   minLength;

            ushort tabVersion = 0;

            flagOK = true;

            _tab_OS_2.getByteRange (ref tabOffset, ref tabLength);
            minLength = 78;         // for version 0

            if (tabLength < minLength)
            {
                //------------------------------------------------------------//
                //                                                            //
                // The length of the table varies according to the table      //
                // version (e.g. in v3 it is 96 bytes long).                  //
                //                                                            //
                //------------------------------------------------------------//

                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Length of '" + tabName + "' table too small: " +
                    tabLength + " < " + minLength);
            }
            else
            {
                flagOK = readBytesAsUInt16 ((int) tabOffset, ref tabVersion);
                
                if (flagOK)
                {
                    flagOK = readBytesAsInt16 (-1,
                                               ref _OS_2_xAvgCharWidth);
                }

                if (flagOK)
                {
                    flagOK = readBytesAsUInt16 (-1,
                                                ref _OS_2_usWeightClass);
                }

                if (flagOK)
                {
                    flagOK = readBytesAsUInt16 (-1,
                                                ref _OS_2_usWidthClass);
                }

                if (flagOK)
                {
                    flagOK = readBytesAsUInt16 (-1,
                                                ref _OS_2_fsType);
                }

                if (flagOK)
                {
                    flagOK = readByteArray ((int) (tabOffset + 32),
                                            cSizePanose,
                                            ref _OS_2_panose);
                }

                if (flagOK)
                {
                    flagOK = readBytesAsUInt16((int)(tabOffset + 62),
                                                ref _OS_2_fsSelection);
                }

                if (flagOK)
                {
                    _OS_2_sTypoDescender = 0;

                    flagOK = readBytesAsInt16 ((int)(tabOffset + 70),
                                               ref _OS_2_sTypoDescender);
                }

                if (flagOK)
                {
                    _OS_2_sxHeight = 0;

                    if (tabLength >= 88)
                        flagOK = readBytesAsInt16 ((int) (tabOffset + 86),
                                                   ref _OS_2_sxHeight);
                }

                if (!flagOK)
                {
                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Error reading '" + tabName + "' table");
                }

                //----------------------------------------------------------------//
                //                                                                //
                // Diagnostics.                                                   //
                //                                                                //
                //----------------------------------------------------------------//

                if (_logVerbose)
                {
                    string usWeightClassDesc = "";
                    string usWidthClassDesc  = "";

                    StringBuilder fsTypeDesc = new StringBuilder ();
                    StringBuilder fsSelDesc  = new StringBuilder ();

                    //------------------------------------------------------------//

                    if (_OS_2_usWeightClass == mask_OS_2_usWeightClass_FW_THIN)
                        usWeightClassDesc = "THIN";
                    else if (_OS_2_usWeightClass == mask_OS_2_usWeightClass_FW_EXTRALIGHT)
                        usWeightClassDesc = "EXTRALIGHT";
                    else if (_OS_2_usWeightClass == mask_OS_2_usWeightClass_FW_LIGHT)
                        usWeightClassDesc = "LIGHT";
                    else if (_OS_2_usWeightClass == mask_OS_2_usWeightClass_FW_NORMAL)
                        usWeightClassDesc = "NORMAL";
                    else if (_OS_2_usWeightClass == mask_OS_2_usWeightClass_FW_MEDIUM)
                        usWeightClassDesc = "MEDIUM";
                    else if (_OS_2_usWeightClass == mask_OS_2_usWeightClass_FW_SEMIBOLD)
                        usWeightClassDesc = "SEMIBOLD";
                    else if (_OS_2_usWeightClass == mask_OS_2_usWeightClass_FW_BOLD)
                        usWeightClassDesc = "BOLD";
                    else if (_OS_2_usWeightClass == mask_OS_2_usWeightClass_FW_EXTRABOLD)
                        usWeightClassDesc = "EXTRABOLD";
                    else if (_OS_2_usWeightClass == mask_OS_2_usWeightClass_FW_BLACK)
                        usWeightClassDesc = "BLACK";
                    else
                        usWeightClassDesc = "unknown";

                    //------------------------------------------------------------//

                    if (_OS_2_usWidthClass == mask_OS_2_usWidthClass_FWIDTH_ULTRA_CONDENSED)
                        usWidthClassDesc = "ULTRA_CONDENSED (50%)";
                    if (_OS_2_usWidthClass == mask_OS_2_usWidthClass_FWIDTH_EXTRA_CONDENSED)
                        usWidthClassDesc = "EXTRA_CONDENSED (62.5%)";
                    else if (_OS_2_usWidthClass == mask_OS_2_usWidthClass_FWIDTH_CONDENSED)
                        usWidthClassDesc = "CONDENSED (75%)";
                    else if (_OS_2_usWidthClass == mask_OS_2_usWidthClass_FWIDTH_SEMI_CONDENSED)
                        usWidthClassDesc = "SEMI_CONDENSED (87.5%)";
                    else if (_OS_2_usWidthClass == mask_OS_2_usWidthClass_FWIDTH_NORMAL)
                        usWidthClassDesc = "NORMAL (100%)";
                    else if (_OS_2_usWidthClass == mask_OS_2_usWidthClass_FWIDTH_SEMI_EXPANDED)
                        usWidthClassDesc = "SEMI_EXPANDED (112.5%)";
                    else if (_OS_2_usWidthClass == mask_OS_2_usWidthClass_FWIDTH_EXPANDED)
                        usWidthClassDesc = "EXPANDED (125%)";
                    else if (_OS_2_usWidthClass == mask_OS_2_usWidthClass_FWIDTH_EXTRA_EXPANDED)
                        usWidthClassDesc = "EXTRA_EXPANDED (150%)";
                    else if (_OS_2_usWidthClass == mask_OS_2_usWidthClass_FWIDTH_ULTRA_EXPANDED)
                        usWidthClassDesc = "ULTRA_EXPANDED (200%)";
                    else
                        usWidthClassDesc = "unknown";

                    //------------------------------------------------------------//

                    if ((_OS_2_fsSelection & mask_OS_2_fsSelection_OBLIQUE) != 0)
                        fsSelDesc.Append("OBLIQUE ");
                    if ((_OS_2_fsSelection & mask_OS_2_fsSelection_WWS) != 0)
                        fsSelDesc.Append("WWS ");
                    if ((_OS_2_fsSelection & mask_OS_2_fsSelection_USE_TYPO_METRICS) != 0)
                        fsSelDesc.Append("USE_TYPO_METRICS ");
                    if ((_OS_2_fsSelection & mask_OS_2_fsSelection_REGULAR) != 0)
                        fsSelDesc.Append("REGULAR ");
                    if ((_OS_2_fsSelection & mask_OS_2_fsSelection_BOLD) != 0)
                        fsSelDesc.Append("BOLD ");
                    if ((_OS_2_fsSelection & mask_OS_2_fsSelection_STRIKEOUT) != 0)
                        fsSelDesc.Append("STRIKEOUT ");
                    if ((_OS_2_fsSelection & mask_OS_2_fsSelection_OUTLINED) != 0)
                        fsSelDesc.Append("OUTLINED ");
                    if ((_OS_2_fsSelection & mask_OS_2_fsSelection_NEGATIVE) != 0)
                        fsSelDesc.Append("NEGATIVE ");
                    if ((_OS_2_fsSelection & mask_OS_2_fsSelection_UNDERSCORE) != 0)
                        fsSelDesc.Append("UNDERSCORE ");
                    if ((_OS_2_fsSelection & mask_OS_2_fsSelection_ITALIC) != 0)
                        fsSelDesc.Append("ITALIC ");
                    if ((_OS_2_fsSelection & mask_OS_2_fsSelection_Reserved) != 0)
                        fsSelDesc.Append("Reserved byte range ");

                    //------------------------------------------------------------//

                    if (_OS_2_fsType == mask_OS_2_fsType_INSTALLABLE_EMBED)
                    {
                        fsTypeDesc.Append("INSTALLABLE_EMBEDDING");
                    }
                    else
                    {
                        if ((_OS_2_fsType & mask_OS_2_fsType_RESTRICTED_LICENSE_EMBED) != 0)
                            fsTypeDesc.Append("RESTRICTED_EMBEDDING ");
                        if ((_OS_2_fsType & mask_OS_2_fsType_PREVIEW_AND_PRINT_EMBED) != 0)
                            fsTypeDesc.Append("PREVIEW&PRINT_EMBEDDING ");
                        if ((_OS_2_fsType & mask_OS_2_fsType_EDITABLE_EMBED) != 0)
                            fsTypeDesc.Append("EDITABLE_EMBEDDING ");

                        if ((_OS_2_fsType & mask_OS_2_fsType_NO_SUBSETTING) != 0)
                            fsTypeDesc.Append("NO_SUBSETTING");
                        if ((_OS_2_fsType & mask_OS_2_fsType_BITMAP_EMBED_ONLY) != 0)
                            fsTypeDesc.Append("BITMAP_EMBEDDING_ONLY");

                        if((_OS_2_fsType & mask_OS_2_fsType_Reserved_A) != 0)
                            fsTypeDesc.Append("*Reserved bit range A*");
                        if((_OS_2_fsType & mask_OS_2_fsType_Reserved_B) != 0)
                            fsTypeDesc.Append("*Reserved bit range B*");
                        if((_OS_2_fsType & mask_OS_2_fsType_Reserved_B) != 0)
                            fsTypeDesc.Append("*Reserved bit range C*");
                    }

                    //------------------------------------------------------------//

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, true, false,
                        "DIAG: table = " + tabName + ":",
                        "Version = 0x" + tabVersion.ToString ("x8"));

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "xAvgCharWidth = " +
                        _OS_2_xAvgCharWidth.ToString () +
                        " pels / em");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "usWeightClass = " +
                        _OS_2_usWeightClass.ToString () +
                        " (" + usWeightClassDesc + ")");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "usWidthClass = " +
                        _OS_2_usWidthClass.ToString () +
                        " (" + usWidthClassDesc + ")");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "sTypoDescender = " +
                        _OS_2_sTypoDescender.ToString ());

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "sxHeight = " +
                        _OS_2_sxHeight.ToString ());

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "fsSelection = " + _OS_2_fsSelection.ToString () +
                        " (" + fsSelDesc + ")");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "panose = '" +
                        _OS_2_panose[0] + "-" + _OS_2_panose[1] + "-" +
                        _OS_2_panose[2] + "-" + _OS_2_panose[3] + "-" +
                        _OS_2_panose[4] + "-" + _OS_2_panose[5] + "-" +
                        _OS_2_panose[6] + "-" + _OS_2_panose[7] + "-" +
                        _OS_2_panose[8] + "-" + _OS_2_panose[9] +
                        "'");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "fsType = " + _OS_2_fsType.ToString () +
                        " (" + fsTypeDesc + ")");
                }

                //----------------------------------------------------------------//
                //                                                                //
                // Licence data.                                                  //
                //                                                                //
                //----------------------------------------------------------------//

                if (flagOK)
                {
                    string licenceText = "";

                    eLicenceType licenceType = checkLicence (ref licenceText);

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "Licensing:", licenceText);

                    if (licenceType == eLicenceType.NotAllowed)
                    {
                        ToolSoftFontGenLog.logNameAndValue (
                            _tableDonor, false, false,
                            "",
                            "***** font conversion requires permission of legal owner *****");
                    }
                    else if (licenceType == eLicenceType.OwnerOnly)
                    {
                        ToolSoftFontGenLog.logNameAndValue (
                            _tableDonor, false, false,
                            "",
                            "***** temporary use of converted font *****");
                        
                        ToolSoftFontGenLog.logNameAndValue (
                            _tableDonor, false, false,
                            "",
                            "***** by TrueType font licensee only  *****");
                    }
                }
            }            

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d D a t a _ P C L T                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read the (optional) 'PCLT' (PCL Treatment) table in order to       //
        // details pertinent to a soft font header.                           //
        //   Font Number                                                      //
        //   Pitch                                                            //
        //   xHeight                                                          //
        //   Style                                                            //
        //   Type Family                                                      //
        //   CapHeight                                                        //
        //   Symbol Set                                                       //
        //   Typeface                                                         //
        //   CharComp     -  only required for bound fonts                    //
        //   FileName                                                         //
        //   StrokeWeight                                                     //
        //   WidthType                                                        //
        //   SerifStyle                                                       //
        // The return value indicates success or failure of the reads.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readData_PCLT ()
        {
            string tabName = "PCLT";

            bool flagOK = true;

            uint tabOffset = 0,
                   tabLength = 0,
                   reqLength;

            flagOK = true;

            _tab_PCLT.getByteRange (ref tabOffset, ref tabLength);
            reqLength = 54;

            _tabPCLTPresent = false;

            if (tabLength == 0)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Table not present.                                         //
                //                                                            //
                //------------------------------------------------------------//

                if (_logVerbose)
                {
                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, true, false,
                        "DIAG: table = " + tabName + ":",
                        "table not present");
                }
            }
            else if (tabLength < reqLength)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Length of '" + tabName + "' table too small: " +
                    tabLength + " < " + reqLength);
            }
            else
            {
                uint version = 0;

                byte[] charComp = new byte[8];
                byte[] fileName = new byte[6];

                _tabPCLTPresent = true;
                
                flagOK = readBytesAsUInt32 ((int) tabOffset,
                                            ref version);

                if (flagOK)
                {
                    flagOK = readBytesAsUInt32 (-1,
                                                ref _PCLT_fontNo);
                }

                if (flagOK)
                {
                    flagOK = readBytesAsUInt16 (-1,
                                                ref _PCLT_pitch);
                }

                if (flagOK)
                {
                    flagOK = readBytesAsUInt16 (-1,
                                                ref _PCLT_xHeight);
                }

                if (flagOK)
                {
                    flagOK = readBytesAsUInt16 (-1,
                                                ref _PCLT_style);
                }

                if (flagOK)
                {
                    flagOK = readBytesAsUInt16 (-1,
                                                ref _PCLT_typeFamily);
                }

                if (flagOK)
                {
                    flagOK = readBytesAsUInt16 (-1,
                                                ref _PCLT_capHeight);
                }

                if (flagOK)
                {
                    flagOK = readBytesAsUInt16 (-1,
                                                ref _PCLT_symSet);
                }

                if (flagOK)
                {
                    flagOK = readByteArray (-1, 16,
                                            ref _PCLT_typeface);
                }
                /*
                if (flagOK)
                {
                    flagOK = readBytesAsUInt32 (-1,
                                                ref charCompA);
                }

                if (flagOK)
                {
                    flagOK = readBytesAsUInt32 (-1,
                                                ref charCompB);
                }
                */

                if (flagOK)
                {
                    flagOK = readBytesAsUInt64 (-1,
                                                ref _PCLT_charComp);
                }

                if (flagOK)
                {
                    flagOK = readByteArray (-1, 6,
                                            ref fileName);
                }

                if (flagOK)
                {
                    flagOK = readByteAsSByte (-1, 
                                              ref _PCLT_strokeWeight);
                }

                if (flagOK)
                {
                    flagOK = readByteAsSByte (-1,
                                              ref _PCLT_widthType);
                }

                if (flagOK)
                {
                    flagOK = readByteAsUByte (-1,
                                              ref _PCLT_serifStyle);
                }

                if (!flagOK)
                {
                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Error reading '" + tabName + "' table");
                }

                //----------------------------------------------------------------//
                //                                                                //
                // Diagnostics.                                                   //
                //                                                                //
                //----------------------------------------------------------------//

                if (_logVerbose)
                {
                    ASCIIEncoding ascii =
                        new ASCIIEncoding ();

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, true, false,
                        "DIAG: table = " + tabName + ":",
                        "Version = 0x" + version.ToString("x8"));

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "fontNumber = 0x" + _PCLT_fontNo.ToString("x8"));

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "pitch = " + _PCLT_pitch.ToString ());

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "xHeight = " + _PCLT_xHeight.ToString ());

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "style = " + _PCLT_style.ToString ());

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "typeFamily = " + _PCLT_typeFamily.ToString ());

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "capHeight = " + _PCLT_capHeight.ToString ());

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "symbol set = " + _PCLT_symSet.ToString () +
                        " (= " +
                        PCLSymbolSets.translateKind1ToId(_PCLT_symSet) +
                        " )");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "typeface = '" +
                        ascii.GetString (_PCLT_typeface, 0, 16) + "'");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "character complement = 0x" +
                        _PCLT_charComp.ToString("x16"));

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "filename = '" +
                        ascii.GetString (fileName, 0, 6) + "'");

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "strokeWeight = " +
                        _PCLT_strokeWeight.ToString ());

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "widthType = " + _PCLT_widthType.ToString ());

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "serifStyle = 0x" +
                        _PCLT_serifStyle.ToString ("x2"));
                }
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d D a t a _ p o s t                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read the (mandatory) 'post' (PostScript information) table to      //
        // obtain:                                                            //
        //    isFixedPitch                                                    //
        //                                                                    //
        // The return value indicates success or failure of the reads.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readData_post ()
        {
            string tabName = "post";

            bool flagOK = true;

            uint tabOffset = 0,
                   tabLength = 0,
                   minLength;

            ushort tabVersion = 0;

            flagOK = true;

            _tab_post.getByteRange (ref tabOffset, ref tabLength);
            minLength = 32;         // for versions 0 and 3

            if (tabLength < minLength)
            {
                //------------------------------------------------------------//
                //                                                            //
                // The length of the table varies according to the table      //
                // version.                                                   //
                //                                                            //
                //------------------------------------------------------------//

                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Length of '" + tabName + "' table too small: " +
                    tabLength + " < " + minLength);
            }
            else
            {
                flagOK = readBytesAsUInt16 ((int)tabOffset, ref tabVersion);

                if (flagOK)
                {
                    flagOK = readBytesAsUInt32 ((int)(tabOffset + 12),
                                                ref _post_isFixedPitch);
                }

                //----------------------------------------------------------------//
                //                                                                //
                // Diagnostics.                                                   //
                //                                                                //
                //----------------------------------------------------------------//

                if (_logVerbose)
                {
                    string text;

                    if (_post_isFixedPitch == 0)
                        text = " (= proportionally-spaced)";
                    else 
                        text = " (= fixed-pitch)";

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, true, false,
                        "DIAG: table = " + tabName + ":",
                        "Version = 0x" + tabVersion.ToString ("x8"));

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "isFixedPitch = 0x" +
                        _post_isFixedPitch.ToString ("x8") +
                        text);
                }
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d D a t a _ t t c f                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read the 'ttcf' (TrueType Collection header) table to obtain:      //
        //    Number of fonts in TTC                                          //
        //    Array of offsets to the OffsetTable for each font in the        //
        //        collection                                                  //
        // The return value indicates success or failure of the reads.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readData_ttcf (ref bool typeTTC,
                                       ref uint numFonts)
        {
            string tabName = "ttcf";

            bool flagOK = true;

            //    UInt32 tabOffset = 0,
            //           tabLength = 0,
            //           reqLength;

            uint tabVersion = 0;
            uint tabId = 0;

            typeTTC = false;

            flagOK = readBytesAsUInt32 (0, ref tabId);

            if (!flagOK)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Error reading first four bytes of font file");
            }
            else if (tabId == cTabID_ttcf)
            {
                uint[] offsets;
                
                typeTTC = true;

                flagOK = readBytesAsUInt32 (-1, ref tabVersion);

                if (flagOK)
                {
                    flagOK = readBytesAsUInt32 (-1, ref numFonts);
                }

                if (flagOK)
                {
                    offsets = new uint[numFonts];

                    for (int i = 0; i < numFonts; i++)
                    {
                        flagOK = readBytesAsUInt32 (-1, ref offsets [i]);
                    }
                }
                else
                {
                    offsets = new uint[0];
                    numFonts = 0;
                }

                if (!flagOK)
                {
                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Error reading '" + tabName + "' table");
                }

                //------------------------------------------------------------//
                //                                                            //
                // Diagnostics.                                               //
                //                                                            //
                //------------------------------------------------------------//

                if (_logVerbose)
                {
                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, true, false,
                        "DIAG: table = " + tabName + ":",
                        "version  = 0x" + tabVersion.ToString ("x8"));

                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: table = " + tabName + ":",
                        "numFonts = " +
                        numFonts.ToString ());

                    for (int i = 0; i < numFonts; i++)
                    {
                        ToolSoftFontGenLog.logNameAndValue (
                            _tableDonor, false, false,
                            "DIAG: table = " + tabName + ":",
                            "offset " + i + " = " +
                            offsets[i].ToString ());
                    }
                }
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d D a t a _ v h e a                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read the (optional) 'vhea' (vertical header) table to obtain:      //
        //    numberofVMetrics                                                //
        // The return value indicates success or failure of the reads.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readData_vhea ()
        {
            string tabName = "vhea";

            bool flagOK = true;

            uint tabOffset = 0,
                   tabLength = 0,
                   reqLength;

            uint tabVersion = 0;

            flagOK = true;

            _tab_vhea.getByteRange (ref tabOffset, ref tabLength);
            reqLength = 36;

            if (tabLength < reqLength)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Length of '" + tabName + "' table too small: " +
                    tabLength + " < " + reqLength);
            }

            if (flagOK)
            {
                flagOK = readBytesAsUInt32 ((int)tabOffset, ref tabVersion);

                if (!flagOK)
                {
                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Error reading '" + tabName + "' table version");
                }
                else if ((tabVersion != cTabVer_vhea_1_0) && (tabVersion != cTabVer_vhea_1_1))
                {
                    flagOK = false;

                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Wrong '" + tabName + "' table version = 0x" +
                        tabVersion.ToString ("x8") + "\r\n\r\n" +
                        "Expected version = 0x" +
                        cTabVer_vhea_1_0.ToString ("x8") + " or 0x" +
                        cTabVer_vhea_1_1.ToString ("x8"));
                }
                else
                {
                    flagOK = readBytesAsUInt16 ((int)(tabOffset + 34),
                                               ref _vhea_numVMetrics);
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Diagnostics.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            if (_logVerbose)
            {
                ToolSoftFontGenLog.logNameAndValue (
                    _tableDonor, true, false,
                    "DIAG: table = " + tabName + ":",
                    "numberOfVMetrics = " + _vhea_numVMetrics.ToString ());
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d D a t a _ v m t x                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read the (optional) 'vmtx' (vertical metrics) table to obtain      //
        // advance heights and top-side bearings for each glyph in the glyph  //
        // table.                                                             //
        // The return value indicates success or failure of the reads.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readData_vmtx ()
        {
            string tabName = "vmtx";

            bool flagOK = true;

            uint tabOffset = 0,
                   tabLength = 0,
                   reqLength;

            int vMetricsArrayLen = 0,
                  tsbArrayLen = 0;
            int vMetricsArraySize = 0,
                  tsbArraySize = 0;

            flagOK = true;

            _tabvmtxPresent = false;

            _tab_vmtx.getByteRange (ref tabOffset, ref tabLength);

            vMetricsArrayLen = _vhea_numVMetrics;
            tsbArrayLen = _maxp_numGlyphs - _vhea_numVMetrics;

            vMetricsArraySize = 4 * vMetricsArrayLen;
            tsbArraySize = 2 * tsbArrayLen;

            reqLength = (uint)(vMetricsArraySize + tsbArraySize);

            if (_vhea_numVMetrics < 1)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Number of VMetrics " + _vhea_numVMetrics + " < 1");
            }
            else if ((_maxp_numGlyphs - _vhea_numVMetrics) < 0)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Number of glyphs " + _maxp_numGlyphs +
                    " < number of VMetrics " + _vhea_numVMetrics);
            }
            else if (tabLength < reqLength)
            {
                flagOK = false;

                ToolSoftFontGenLog.logError (
                    _tableDonor, MessageBoxImage.Error,
                    "Length of '" + tabName + "' table too small: " +
                    tabLength + " < " + reqLength);
            }

            if (flagOK)
            {
                byte[] vMetricsArray = new byte[vMetricsArraySize];
                byte[] tsbArray = new byte[tsbArraySize];

                flagOK = readByteArray ((int)tabOffset,
                                        vMetricsArraySize,
                                        ref vMetricsArray);

                _tabvmtxPresent = true;

                if (flagOK)
                {
                    flagOK = readByteArray (-1,
                                            tsbArraySize,
                                            ref tsbArray);
                }

                if (flagOK)
                {
                    ushort glyphId = 0,
                           advance = 0;

                    short tsb;

                    uint advanceOffset,
                           tsbOffset;

                    for (int indx = 0; indx < vMetricsArrayLen; indx++)
                    {
                        advanceOffset = (uint)(indx * 4);
                        tsbOffset = advanceOffset + 2;

                        advance = (ushort)((vMetricsArray [advanceOffset] << 8) +
                                             vMetricsArray [advanceOffset + 1]);

                        tsb = (short)((vMetricsArray [tsbOffset] << 8) +
                                        vMetricsArray [tsbOffset + 1]);

                        _glyphData [indx].setMetricsV (advance, tsb);
                    }

                    for (int indx = 0; indx < tsbArrayLen; indx++)
                    {
                        tsbOffset = (uint)(indx * 2);

                        tsb = (short)((tsbArray [tsbOffset] << 8) +
                                        tsbArray [tsbOffset + 1]);

                        glyphId = (ushort)(vMetricsArrayLen + indx);

                       _glyphData [glyphId].setMetricsV (advance, tsb);
                    }
                }
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d T a b l e D i r e c t o r y                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read the table directory from the TrueType file.                   //
        // Store offset and length details for each of the relevant tables.   //
        // The return value indicates success or failure of the read.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool readTableDirectory(int dirOffset,
                                           bool getTTCData)
        {
            bool flagOK = true;

            ushort numTables = 0;
            uint tabTag = 0,
                   tabChecksum = 0;
            uint tabOffset = 0,
                   tabLength = 0;
            int padBytes = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Initialise table directory table structure.                    //
            //                                                                //
            //----------------------------------------------------------------//

            _tab_OS_2.initialise ();
            _tab_PCLT.initialise ();
            _tab_cmap.initialise ();
            _tab_cvt.initialise ();
            _tab_fpgm.initialise ();
            _tab_gdir.initialise ();        // empty (place-holder) table //
            _tab_glyf.initialise ();
            _tab_head.initialise ();
            _tab_hhea.initialise ();
            _tab_hmtx.initialise ();
            _tab_loca.initialise ();
            _tab_maxp.initialise ();
            _tab_name.initialise ();
            _tab_prep.initialise ();
            _tab_vhea.initialise ();
            _tab_vmtx.initialise ();

            //----------------------------------------------------------------//
            //                                                                //
            // Read the 'number of tables' value.                             //
            //                                                                //
            //----------------------------------------------------------------//

            if (flagOK)
            {
                flagOK = readBytesAsUInt16 (dirOffset + 4, ref numTables);

                if (!flagOK)
                {
                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Error reading number of tables");
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Reposition to the start of the table directory, and read each  //
            // table entry.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            if (flagOK)
            {
                flagOK = fontFileSeek (dirOffset + 12);

                if (!flagOK)
                {
                    ToolSoftFontGenLog.logError (
                        _tableDonor, MessageBoxImage.Error,
                        "Error repositioning to start of table directory");
                }
            }

            if (flagOK)
            {
                byte[] tabName = new byte[4];

                if (_logVerbose && !getTTCData)
                {
                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "DIAG: Tables",
                        "Count = " + numTables);
                }

                for (int i = 0; (i < numTables) && flagOK; i++)
                {
                    flagOK = readByteArray(-1, 4, ref tabName);

                    if (flagOK)
                    {
                        tabTag = byteArrayToUInt32 (tabName);

                        flagOK = readBytesAsUInt32 (-1, ref tabChecksum);
                    }

                    if (flagOK)
                    {
                        flagOK = readBytesAsUInt32 (-1, ref tabOffset);
                    }

                    if (flagOK)
                    {
                        flagOK = readBytesAsUInt32 (-1, ref tabLength);
                    }

                    if (flagOK)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Store offset and length details for each table,    //
                        // provided that it is one we are interested in.      //
                        //                                                    //
                        //----------------------------------------------------//

                        padBytes = (int) (tabLength % 4);

                        if (padBytes != 0)
                            padBytes = 4 - padBytes;

                        switch (tabTag)
                        {
                            case cTabID_OS_2:
                                _tab_OS_2.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            case cTabID_PCLT:
                                _tab_PCLT.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            case cTabID_cmap:
                                _tab_cmap.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            case cTabID_cvt:
                                _tab_cvt.setMetrics (tabChecksum,
                                                     tabOffset,
                                                     tabLength,
                                                     padBytes);
                                break;

                            case cTabID_fpgm:
                                _tab_fpgm.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            case cTabID_glyf:
                                _tab_glyf.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            case cTabID_head:
                                _tab_head.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            case cTabID_hhea:
                                _tab_hhea.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            case cTabID_hmtx:
                                _tab_hmtx.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            case cTabID_loca:
                                _tab_loca.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            case cTabID_maxp:
                                _tab_maxp.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            case cTabID_name:
                                _tab_name.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            case cTabID_post:
                                _tab_post.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            case cTabID_prep:
                                _tab_prep.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            case cTabID_vhea:
                                _tab_vhea.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            case cTabID_vmtx:
                                _tab_vmtx.setMetrics (tabChecksum,
                                                      tabOffset,
                                                      tabLength,
                                                      padBytes);
                                break;

                            default:
                                break;
                        }

                        if (_logVerbose && !getTTCData)
                        {
                            ToolSoftFontGenLog.logNameAndValue (
                                _tableDonor, false, false,
                                "DIAG: table = " +
                                ((char) tabName[0]).ToString () +
                                ((char) tabName[1]).ToString () +
                                ((char) tabName[2]).ToString () +
                                ((char) tabName[3]).ToString () + ":",
                                "(0x" + tabTag.ToString ("x8") +
                                "); offset = " + tabOffset +
                                "; length = " + tabLength);
                        }
                    }
                    else
                    {
                        ToolSoftFontGenLog.logError (
                            _tableDonor, MessageBoxImage.Error,
                            "Error reading TTF table directory entries)");
                    }
                }

                if (_logVerbose && !getTTCData)
                {
                    ToolSoftFontGenLog.logNameAndValue (
                        _tableDonor, false, false,
                        "",
                        "");
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Check that details for the (mandatory) tables that are         //
            // required have been obtained.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            if (flagOK)
            {
                if (getTTCData)
                {
                    if (_tab_name.zeroLength ())
                    {
                        flagOK = false;
                    }
                }
                else
                {
                    if (_tab_cmap.zeroLength () ||
                        _tab_glyf.zeroLength () ||
                        _tab_head.zeroLength () ||
                        _tab_hhea.zeroLength () ||
                        _tab_hmtx.zeroLength () ||
                        _tab_loca.zeroLength () ||
                        _tab_maxp.zeroLength () ||
                        _tab_name.zeroLength ())
                    {
                        flagOK = false;
                    }
                }

                if (! flagOK)
                {
                    ToolSoftFontGenLog.logError (
                                _tableDonor, MessageBoxImage.Error,
                                "Mandatory TTF table not present");
                }
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S i z e C h a r S e t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return character set size (256 or 65536).                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int SizeCharSet
        {
            get { return _sizeCharSet; }
            set { _sizeCharSet = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T a b l e v m t x P r e s e n t                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return indication as to whether or not the (optional) 'vmtx' table //
        // is present.                                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool TablevmtxPresent
        {
            get { return _tabvmtxPresent; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T y p e f a c e N a m e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the 'Typeface' array read from the 'PCLT' table (if it      //
        // exists).                                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte[] TypefaceName
        {
            get
            {
                return _PCLT_typeface;
            }
        }
    }
}
