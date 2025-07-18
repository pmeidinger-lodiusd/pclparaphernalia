﻿namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines constants used by print file 'parse' mechanisms.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    [System.Reflection.Obfuscation(Feature = "properties renaming")]

    public static class PrnParseConstants
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum eContType
        {
            None = 0,
            Abort,
            Reset,
            Unknown,
            Special,
            PCLAlphaNumericID,
            PCLColourLookup,
            PCLConfigurationIO,
            PCLConfigurationIOVal,
            PCLConfigureImageData,
            PCLConfigureRasterData,
            PCLComplex,
            PCLDitherMatrix,
            PCLDitherMatrixPlane,
            PCLDitherMatrixPlaneData,
            PCLDownload,
            PCLDownloadCombo,
            PCLDriverConfiguration,
            PCLEmbeddedData,
            PCLEmbeddedPML,
            PCLEscEncText,
            PCLEscEncTextData,
            PCLExtension,
            PCLFontChar,
            PCLFontHddr,
            PCLLogicalPageData,
            PCLDefineSymbolSet,
            PCLDefineSymbolSetMap,
            PCLMultiByteData,
            PCLPaletteConfiguration,
            PCLUserDefPatternHddr,
            PCLUserDefPatternData,
            PCLViewIlluminant,
            HPGL2,
            HPGL2Binary,
            HPGL2Label,
            HPGL2Long,
            HPGL2LongQuote,
            PJL,
            PostScript,
            PCLXL,
            PCLXLEmbed,
            PCLXLFontChar,
            PCLXLFontHddr,
            Prescribe
        }

        public enum eActPCL
        {
            None,
            AlphaNumericID,
            CheckForPML,
            ColourLookup,
            ConfigurationIO,
            ConfigureImageData,
            ConfigureRasterData,
            DefineSymbolSet,
            DitherMatrix,
            DriverConfiguration,
            EmbeddedData,
            EscEncText,
            FontChar,
            FontHddr,
            LogicalPageData,
            MacroStart,
            MacroStop,
            PaletteConfiguration,
            StyleData,
            TextParsing,
            SwitchToHPGL2,
            SwitchToPJL,
            UserDefinedPattern,
            ViewIlluminant
        }

        public enum eActPCLXL
        {
            None,
            CharSize,
            Measure,
            UnitsPerMeasure
        }
        public enum eOvlAct
        {
            None = 0,
            Adjust,         // PCL    only
            Download,       // PCL    only
            DownloadDelete, // PCL    only
            EndOfFile,      // PCL    only ?
            IdFont,         // PCL    only
            IdPalette,      // PCL    only
            IdPattern,      // PCL    only
            IdSymSet,       // PCL    only
            IdMacro,        // PCL    only
            IgnorePage,
            Illegal,        // PCL XL only ?
            Insert,
            PageBegin,      // PCL XL only  or PCL as well
            PageEnd,        // PCL XL only  or PCL as well
            PageBoundary,   // PCL    only
            PageChange,     // PCL    only
            PageMark,       // PCL    only
            PushGS,         // PCL XL only
            Remove,
            Replace_0x77,   // PCL XL only
            Reset,          // PCL    only 
            Terminate
        }

        public enum eOvlPos
        {
            BeforeFirstPage = 0,    // keep these entries in this logical order
            WithinFirstPage,
            BetweenPages,
            WithinOtherPages,
            AfterPages
        }

        [System.Reflection.Obfuscation(Exclude = true)]

        public enum eOvlShow
        {
            None = 0,
            Insert,
            Modify,
            Remove,
            Illegal,
            Terminate
        }

        public enum eSeqGrp
        {
            Unknown = 0,
            Colour,
            CursorPositioning,
            FontManagement,
            FontSelection,
            JobControl,
            Macros,
            Misc,
            PageControl,
            PictureFrame,
            PrintModel,
            RasterGraphics,
            RectangularAreaFill,
            SoftFontCreation,
            StatusReadback,
            UserPattern
        }

        public enum ePCLXLBinding
        {
            Unknown = 0,
            BinaryLSFirst,
            BinaryMSFirst,
            ASCII
        }

        public enum eOptCharSets
        {
            ASCII = 0,
            ISO_8859_1,
            Win_ANSI,
            Max     // limit 
        }

        public enum eOptCharSetSubActs
        {
            Mnemonics = 0,
            MnemonicsIncSpace,
            Hex,
            Dots,
            Spaces,
            Substitute,
            Max     // limit
        }

        public enum eOptOffsetFormats
        {
            Hexadecimal = 0,
            Decimal,
            Max     // limit
        }

        public enum eOptStatsLevel
        {
            ReferencedOnly = 0,
            All,
            Max     // limit
        }

        public enum ePMLSeqType
        {
            Hddr = 0,
            RequestAction,
            RequestTypeLen,
            RequestData,
            ReplyAction,
            ReplyOutcome,
            ReplyData
        }

        public enum eOffsetPosition
        {
            Unknown = -1,
            StartOfFile = -2,
            EndOfFile = -3,
            CrntPosition = -4
        }

        public enum eStdClrs
        {
            AliceBlue = 0,
            AntiqueWhite,
            Aqua,
            Aquamarine,
            Azure,
            Beige,
            Bisque,
            Black,
            BlanchedAlmond,
            Blue,
            BlueViolet,
            Brown,
            BurlyWood,
            CadetBlue,
            Chartreuse,
            Chocolate,
            Coral,
            CornflowerBlue,
            Cornsilk,
            Crimson,
            Cyan,
            DarkBlue,
            DarkCyan,
            DarkGoldenrod,
            DarkGray,
            DarkGreen,
            DarkKhaki,
            DarkMagenta,
            DarkOliveGreen,
            DarkOrange,
            DarkOrchid,
            DarkRed,
            DarkSalmon,
            DarkSeaGreen,
            DarkSlateBlue,
            DarkSlateGray,
            DarkTurquoise,
            DarkViolet,
            DeepPink,
            DeepSkyBlue,
            DimGray,
            DodgerBlue,
            Firebrick,
            FloralWhite,
            ForestGreen,
            Fuchsia,
            Gainsboro,
            GhostWhite,
            Gold,
            Goldenrod,
            Gray,
            Green,
            GreenYellow,
            Honeydew,
            HotPink,
            IndianRed,
            Indigo,
            Ivory,
            Khaki,
            Lavender,
            LavenderBlush,
            LawnGreen,
            LemonChiffon,
            LightBlue,
            LightCoral,
            LightCyan,
            LightGoldenrodYellow,
            LightGray,
            LightGreen,
            LightPink,
            LightSalmon,
            LightSeaGreen,
            LightSkyBlue,
            LightSlateGray,
            LightSteelBlue,
            LightYellow,
            Lime,
            LimeGreen,
            Linen,
            Magenta,
            Maroon,
            MediumAquamarine,
            MediumBlue,
            MediumOrchid,
            MediumPurple,
            MediumSeaGreen,
            MediumSlateBlue,
            MediumSpringGreen,
            MediumTurquoise,
            MediumVioletRed,
            MidnightBlue,
            MintCream,
            MistyRose,
            Moccasin,
            NavajoWhite,
            Navy,
            OldLace,
            Olive,
            OliveDrab,
            Orange,
            OrangeRed,
            Orchid,
            PaleGoldenrod,
            PaleGreen,
            PaleTurquoise,
            PaleVioletRed,
            PapayaWhip,
            PeachPuff,
            Peru,
            Pink,
            Plum,
            PowderBlue,
            Purple,
            Red,
            RosyBrown,
            RoyalBlue,
            SaddleBrown,
            Salmon,
            SandyBrown,
            SeaGreen,
            SeaShell,
            Sienna,
            Silver,
            SkyBlue,
            SlateBlue,
            SlateGray,
            Snow,
            SpringGreen,
            SteelBlue,
            Tan,
            Teal,
            Thistle,
            Tomato,
            Transparent,
            Turquoise,
            Violet,
            Wheat,
            White,
            WhiteSmoke,
            Yellow,
            YellowGreen
        }

        public const int bufSize = 2048;        // multiple of 16         
        public const int viewBytesPerLine = 16; // divisor of 1024

        public const byte asciiAngleLeft = 0x3c;
        public const byte asciiAngleRight = 0x3e;
        public const byte asciiAlphaLCMax = 0x7a;
        public const byte asciiAlphaLCMin = 0x61;
        public const byte asciiAlphaUCMax = 0x5a;
        public const byte asciiAlphaUCMin = 0x41;
        public const byte asciiApostrophe = 0x27;
        public const byte asciiAtSign = 0x40;
        public const byte asciiBEL = 0x07;
        public const byte asciiCR = 0x0d;
        public const byte asciiDEL = 0x7f;
        public const byte asciiDigit0 = 0x30;
        public const byte asciiGraphicMin = 0x21;
        public const byte asciiDigit1 = 0x31;
        public const byte asciiDigit9 = 0x39;
        public const byte asciiEquals = 0x3d;
        public const byte asciiEsc = 0x1b;
        public const byte asciiETX = 0x03;
        public const byte asciiExclamationMark = 0x21;
        public const byte asciiFF = 0x0c;
        public const byte asciiHT = 0x09;
        public const byte asciiLF = 0x0a;
        public const byte asciiMax8bit = 0xff;
        public const byte asciiMinus = 0x2d;
        public const byte asciiNUL = 0x00;
        public const byte asciiPeriod = 0x2e;
        public const byte asciiPipe = 0x7c;
        public const byte asciiPlus = 0x2b;
        public const byte asciiQuote = 0x22;
        public const byte asciiSemiColon = 0x3b;
        public const byte asciiSpace = 0x20;
        public const byte asciiSUB = 0x1f;
        public const byte asciiSubDefault = 0xbf;

        public const byte pclSimpleICharLow = 0x30;
        public const byte pclSimpleICharHigh = 0x7e;

        public const byte pclComplexICharLow = 0x21;
        public const byte pclComplexICharHigh = 0x2f;

        public const byte pclComplexGCharLow = 0x60;
        public const byte pclComplexGCharHigh = 0x7e;

        public const byte pclComplexPCharLow = 0x60;
        public const byte pclComplexPCharHigh = 0x7e;

        public const byte pclComplexTCharLow = 0x40;
        public const byte pclComplexTCharHigh = 0x5e;

        public const byte pclxlAttrUbyte = 0xf8;
        public const byte pclxlAttrUint16 = 0xf9;
        public const byte pclxlEmbedData = 0xfa;
        public const byte pclxlEmbedDataByte = 0xfb;
        public const byte pclxlDataTypeLow = 0xc0;
        public const byte pclxlDataTypeHigh = 0xef;
        public const byte pclxlDataTypeUbyte = 0xc0;
        public const byte pclxlDataTypeUint16 = 0xc1;
        public const byte pclxlOperatorLow = 0x41;
        public const byte pclxlOperatorHigh = 0xbf;

        public const byte prescribeSCRCDefault = 0x52;  // R //
        public const byte prescribeSCRCDelimiter = 0x21;  // ! //

        public const string cRptA_colName_RowType = "RowType";  // not displayed
        public const string cRptA_colName_Action = "Action";   // MakeOverlay only
        public const string cRptA_colName_Offset = "Offset";
        public const string cRptA_colName_Type = "Type";
        public const string cRptA_colName_Seq = "Sequence";
        public const string cRptA_colName_Desc = "Description";

        public const string cRptC_colName_Offset = "Offset";
        public const string cRptC_colName_Hex = "Hexadecimal";
        public const string cRptC_colName_Text = "Text";

        public const string cRptS_colName_Seq = "Sequence";
        public const string cRptS_colName_Desc = "Description";
        public const string cRptS_colName_CtP = "Parent";
        public const string cRptS_colName_CtE = "Embedded";
        public const string cRptS_colName_CtT = "Total";

        public const int cRptA_colMax_RowType = -1;   // not displayed
        public const int cRptA_colMax_Action = 10;   // MakeOverlay only
        public const int cRptA_colMax_Offset = 13;
        public const int cRptA_colMax_Type = 21;
        public const int cRptA_colMax_Seq = 16;
        public const int cRptA_colMax_Desc = 52;

        public const int cRptC_colMax_Offset = 13;
        public const int cRptC_colMax_Hex = 48;
        public const int cRptC_colMax_Text = 16;

        public const int cRptS_colMax_Seq = 19;
        public const int cRptS_colMax_Desc = 52;
        public const int cRptS_colMax_CtP = 8;
        public const int cRptS_colMax_CtE = 8;
        public const int cRptS_colMax_CtT = 8;

        public const int cColSeparatorLen = 2;

        public const int pclDotResDefault = 300;

        public static byte[] cHexBytes = {0x30, 0x31, 0x32, 0x33,  // 0123
                                          0x34, 0x35, 0x36, 0x37,  // 4567
                                          0x38, 0x39, 0x61, 0x62,  // 89ab
                                          0x63, 0x64, 0x65, 0x66}; // cdef;

        public static char[] cHexChars = {'0', '1', '2', '3',
                                          '4', '5', '6', '7',
                                          '8', '9', 'a', 'b',
                                          'c', 'd', 'e', 'f'};
    }
}