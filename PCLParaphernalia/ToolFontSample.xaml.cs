﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// Interaction logic for ToolFontSample.xaml
    /// 
    /// Class handles the FontSample tool form.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    [System.Reflection.Obfuscation(Feature = "renaming",
                                            ApplyToMembers = true)]

    public partial class ToolFontSample : Window
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const string _defaultFontNamePCLXL = "Arial           ";
        private const string _defaultPrnDiskNamePCL = "MassStoreFont.sfp";

        private const char _defaultSymSetIdAlpha = 'N';
        private const ushort _defaultSymSetIdNum = 0;
        private const ushort _defaultSymSetNo = 14;

        private const double _defaultFontHeightPCLXL = 15.00;
        private const double _defaultFontHeightPCL = 15.00;
        private const double _defaultFontPitchPCL = 8.00;

        private const ushort _defaultSoftFontIdPCL = 32767;
        private const ushort _defaultSoftFontIdMacroPCL = 1001;

        private const ushort _defaultFontStylePCL = 0;
        private const ushort _defaultFontTypefacePCL = 16602;
        private const short _defaultFontWeightPCL = 0;

        private static readonly int[] _subsetPDLs =
        {
            (int) ToolCommonData.ePrintLang.PCL,
            (int) ToolCommonData.ePrintLang.PCLXL,
        };

        private static readonly int[] _subsetOrientations =
        {
            (int) PCLOrientations.eIndex.Portrait,
            (int) PCLOrientations.eIndex.ReversePortrait
        };

        private static readonly int[] _subsetPaperSizes =
        {
            (int) PCLPaperSizes.eIndex.ISO_A4,
            (int) PCLPaperSizes.eIndex.ANSI_A_Letter
        };

        private static readonly int[] _subsetPaperTypes =
        {
            (int) PCLPaperTypes.eIndex.NotSet,
            (int) PCLPaperTypes.eIndex.Plain
        };

        private static readonly int[] _subsetParseMethodsPCLAll =
        {
            (int) PCLTextParsingMethods.eIndex.not_specified,
            (int) PCLTextParsingMethods.eIndex.m0_1_byte_default,
            (int) PCLTextParsingMethods.eIndex.m1_1_byte_alt,
            (int) PCLTextParsingMethods.eIndex.m2_2_byte,
            (int) PCLTextParsingMethods.eIndex.m21_1_or_2_byte_Asian7bit,
            (int) PCLTextParsingMethods.eIndex.m31_1_or_2_byte_ShiftJIS,
            (int) PCLTextParsingMethods.eIndex.m38_1_or_2_byte_Asian8bit,
            (int) PCLTextParsingMethods.eIndex.m83_UTF8,
            (int) PCLTextParsingMethods.eIndex.m1008_UTF8_alt
        };

        private static readonly int[] _subsetParseMethodsPCLDirect =
        {
            (int) PCLTextParsingMethods.eIndex.m83_UTF8,
            (int) PCLTextParsingMethods.eIndex.m1008_UTF8_alt
        };

        private static readonly int[] _subsetParseMethodsPCLXLAll =
        {
            (int) PCLTextParsingMethods.eIndex.m0_1_byte_default,
            (int) PCLTextParsingMethods.eIndex.m2_2_byte,
            (int) PCLTextParsingMethods.eIndex.m21_1_or_2_byte_Asian7bit,
            (int) PCLTextParsingMethods.eIndex.m31_1_or_2_byte_ShiftJIS,
            (int) PCLTextParsingMethods.eIndex.m38_1_or_2_byte_Asian8bit
        };

        private static readonly int[] _subsetParseMethodsPCLXLDirect =
        {
            (int) PCLTextParsingMethods.eIndex.m2_2_byte,
        };

        private static readonly int _ctFonts = PCLFonts.GetCountUnique();

        private static readonly int[] _subsetFonts = new int[_ctFonts];

        private static readonly int _ctSymSets = PCLSymbolSets.GetCountStd() +
                                          PCLSymbolSets.GetCountUserSet();

        private static readonly int[] _subsetSymSets = new int[_ctSymSets];

        private static int[] _subsetParseMethods;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private ushort _fontDownloadIdPCL = _defaultSoftFontIdPCL;
        private ushort _fontPrnDiskIdPCL = _defaultSoftFontIdPCL;
        private ushort _fontPrnDiskMacroIdPCL = _defaultSoftFontIdMacroPCL;

        private readonly ushort[] _sampleOffsetBlocks = new ushort[256];

        private FontCustomPCL _fontCustomPCL;
        private FontDownloadPCL _fontDownloadPCL;
        private FontPresetPCL _fontPresetPCL;
        private FontPrnDiskPCL _fontPrnDiskPCL;

        private FontCustomPCLXL _fontCustomPCLXL;
        private FontDownloadPCLXL _fontDownloadPCLXL;
        private FontPresetPCLXL _fontPresetPCLXL;

        private ToolCommonData.ePrintLang _crntPDL;

        private int _ctPDLs;
        private int _ctOrientations;
        private int _ctPaperSizes;
        private int _ctPaperTypes;
        private int _ctSampleOffsetBlocks;
        private int _ctSamplePages;
        private int _ctParseMethods;

        private int _indxPDL;
        private int _indxFontPCL;
        private int _indxFontPCLXL;
        private int _indxSymSetPCL;
        private int _indxSymSetPCLXL;

        private int _indxParseMethod;

        private int _indxOrientationPCL;
        private int _indxOrientationPCLXL;
        private int _indxPaperSizePCL;
        private int _indxPaperSizePCLXL;
        private int _indxPaperTypePCL;
        private int _indxPaperTypePCLXL;

        private bool _prnDiskLoadViaMacro;

        private bool _fontBound;
        private bool _fontProportional;
        private bool _fontScalable;

        private bool _formAsMacroPCL;
        private bool _formAsMacroPCLXL;

        private bool _showC0CharsPCL;
        private bool _showC0CharsPCLXL;

        private bool _mapCodesRelevant;
        private bool _showMapCodesUCS2PCL;
        private bool _showMapCodesUCS2PCLXL;
        private bool _showMapCodesUTF8PCL;
        private bool _showMapCodesUTF8PCLXL;

        private bool _downloadSelByIdPCL;
        private bool _downloadRemovePCL;
        private bool _downloadRemovePCLXL;

        private bool _prnDiskSelByIdPCL;
        private bool _prnDiskRemovePCL;
        private bool _prnDiskFontPCL;
        private bool _prnDiskFontDataKnownPCL;

        private bool _optGridVertical;

        private bool _initialised;
        private bool _settingSymSetAttributes;
        private bool _useSampleBlocks;
        private bool _symSetUserFileValid;
        private bool _symSetUserActEmbedPCL;
        private bool _symSetUserActEmbedPCLXL;   // dummy

        private ushort _symSetNo;
        private ushort _symSetNoUserSet;
        private ushort _symSetUserFirstCode = 0;
        private ushort _symSetUserLastCode = 0;

        private string _symSetId;

        private string _symSetName;
        private string _fontDesc;
        private string _fontNamePCLXL;

        private string _fontPrnDiskNamePCL;

        private string _fontFilenamePCL;
        private string _fontFilenamePCLXL;

        private string _fontSelSeqPCL;
        private string _fontSelDescPCL;
        private string _fontLoadDescPCL;
        private string _symSetUserFile;

        private ushort _fontTypefacePCL;

        private double _fontHeightPCL;
        private double _fontPitchPCL;
        private double _fontHeightPCLXL;

        private ushort _fontStylePCL;
        private short _fontWeightPCL;

        private PCLFonts.eVariant _fontVar;
        private PCLFonts.eFontType _fontType;

        private PCLSymbolSets.eSymSetGroup _symSetGroup;
        private PCLSymSetTypes.eIndex _symSetType;
        private PCLSymSetTypes.eIndex _fontSymSetTypePCL;

        //--------------------------------------------------------------------//
        //                                                  S t r u c t u r e //
        // F o n t C u s t o m P C L                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private struct FontCustomPCL
        {
            bool _proportional;
            bool _scalable;
            bool _bound;
            ushort _style;
            ushort _typeface;
            short _weight;
            double _height;
            double _pitch;
            int _symSetIndex;
            ushort _symSetCustom;
            string _symSetUserFile;

            //----------------------------------------------------------------//

            public FontCustomPCL(bool proportional,
                                  bool scalable,
                                  bool bound,
                                  ushort style,
                                  ushort typeface,
                                  short weight,
                                  double height,
                                  double pitch,
                                  int symSetIndex,
                                  ushort symSetCustom,
                                  string symSetUserFile)
            {
                _proportional = proportional;
                _scalable = scalable;
                _bound = bound;
                _style = style;
                _typeface = typeface;
                _weight = weight;
                _height = height;
                _pitch = pitch;
                _symSetIndex = symSetIndex;
                _symSetCustom = symSetCustom;
                _symSetUserFile = symSetUserFile;
            }

            //----------------------------------------------------------------//

            public void Restore(ref bool proportional,
                                ref bool scalable,
                                ref bool bound,
                                ref ushort style,
                                ref ushort typeface,
                                ref short weight,
                                ref double height,
                                ref double pitch,
                                ref int symSetIndex,
                                ref ushort symSetCustom,
                                ref string symSetUserFile)
            {
                proportional = _proportional;
                scalable = _scalable;
                bound = _bound;
                style = _style;
                typeface = _typeface;
                weight = _weight;
                height = _height;
                pitch = _pitch;
                symSetIndex = _symSetIndex;
                symSetCustom = _symSetCustom;
                symSetUserFile = _symSetUserFile;
            }

            //----------------------------------------------------------------//

            public void Store(bool proportional,
                              bool scalable,
                              bool bound,
                              ushort style,
                              ushort typeface,
                              short weight,
                              double height,
                              double pitch,
                              int symSetIndex,
                              ushort symSetCustom,
                              string symSetUserFile)
            {
                _proportional = proportional;
                _scalable = scalable;
                _bound = bound;
                _style = style;
                _typeface = typeface;
                _weight = weight;
                _height = height;
                _pitch = pitch;
                _symSetIndex = symSetIndex;
                _symSetCustom = symSetCustom;
                _symSetUserFile = symSetUserFile;
            }
        }

        //--------------------------------------------------------------------//
        //                                                  S t r u c t u r e //
        // F o n t C u s t o m P C L X L                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private struct FontCustomPCLXL
        {
            string _fontName;
            double _height;
            int _symSetIndex;
            ushort _symSetCustom;
            string _symSetUserFile;

            //----------------------------------------------------------------//

            public FontCustomPCLXL(string fontName,
                                    double height,
                                    int symSetIndex,
                                    ushort symSetCustom,
                                    string symSetUserFile)
            {
                _fontName = fontName;
                _height = height;
                _symSetIndex = symSetIndex;
                _symSetCustom = symSetCustom;
                _symSetUserFile = symSetUserFile;
            }

            //----------------------------------------------------------------//

            public void Restore(ref string fontName,
                                 ref double height,
                                 ref int symSetIndex,
                                 ref ushort symSetCustom,
                                 ref string symSetUserFile)
            {
                fontName = _fontName;
                height = _height;
                symSetIndex = _symSetIndex;
                symSetCustom = _symSetCustom;
                symSetUserFile = _symSetUserFile;
            }

            //----------------------------------------------------------------//

            public void Store(string fontName,
                               double height,
                               int symSetIndex,
                               ushort symSetCustom,
                               string symSetUserFile)
            {
                _fontName = fontName;
                _height = height;
                _symSetIndex = symSetIndex;
                _symSetCustom = symSetCustom;
                _symSetUserFile = symSetUserFile;
            }
        }

        //--------------------------------------------------------------------//
        //                                                  S t r u c t u r e //
        // F o n t D o w n l o a d P C L                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private struct FontDownloadPCL
        {
            string _downloadFile;
            ushort _downloadId;
            bool _selectById;
            bool _downloadRemove;
            double _height;
            double _pitch;
            int _symSetIndex;
            ushort _symSetCustom;
            string _symSetUserFile;

            //----------------------------------------------------------------//

            public FontDownloadPCL(string downloadFile,
                                    ushort downloadId,
                                    bool downloadRemove,
                                    bool selectById,
                                    double height,
                                    double pitch,
                                    int symSetIndex,
                                    ushort symSetCustom,
                                    string symSetUserFile)
            {
                _downloadFile = downloadFile;
                _downloadId = downloadId;
                _downloadRemove = downloadRemove;
                _selectById = selectById;
                _height = height;
                _pitch = pitch;
                _symSetIndex = symSetIndex;
                _symSetCustom = symSetCustom;
                _symSetUserFile = symSetUserFile;
            }

            //----------------------------------------------------------------//

            public void Restore(ref string downloadFile,
                                ref ushort downloadId,
                                ref bool downloadRemove,
                                ref bool selectById,
                                ref double height,
                                ref double pitch,
                                ref int symSetIndex,
                                ref ushort symSetCustom,
                                ref string symSetUserFile)
            {
                downloadFile = _downloadFile;
                downloadId = _downloadId;
                downloadRemove = _downloadRemove;
                selectById = _selectById;
                height = _height;
                pitch = _pitch;
                symSetIndex = _symSetIndex;
                symSetCustom = _symSetCustom;
                symSetUserFile = _symSetUserFile;
            }

            //----------------------------------------------------------------//

            public void Store(string downloadFile,
                              ushort downloadId,
                              bool downloadRemove,
                              bool selectById,
                              double height,
                              double pitch,
                              int symSetIndex,
                              ushort symSetCustom,
                              string symSetUserFile)
            {
                _downloadFile = downloadFile;
                _downloadId = downloadId;
                _downloadRemove = downloadRemove;
                _selectById = selectById;
                _height = height;
                _pitch = pitch;
                _symSetIndex = symSetIndex;
                _symSetCustom = symSetCustom;
                _symSetUserFile = symSetUserFile;
            }
        }

        //--------------------------------------------------------------------//
        //                                                  S t r u c t u r e //
        // F o n t D o w n l o a d P C L X L                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private struct FontDownloadPCLXL
        {
            string _downloadFile;
            bool _downloadRemove;
            double _height;
            int _symSetIndex;
            ushort _symSetCustom;
            string _symSetUserFile;

            //----------------------------------------------------------------//

            public FontDownloadPCLXL(string downloadFile,
                                      bool downloadRemove,
                                      double height,
                                      int symSetIndex,
                                      ushort symSetCustom,
                                      string symSetUserFile)
            {
                _downloadFile = downloadFile;
                _downloadRemove = downloadRemove;
                _height = height;
                _symSetIndex = symSetIndex;
                _symSetCustom = symSetCustom;
                _symSetUserFile = symSetUserFile;
            }

            //----------------------------------------------------------------//

            public void Restore(ref string downloadFile,
                                ref bool downloadRemove,
                                ref double height,
                                ref int symSetIndex,
                                ref ushort symSetCustom,
                                ref string symSetUserFile)
            {
                downloadFile = _downloadFile;
                downloadRemove = _downloadRemove;
                height = _height;
                symSetIndex = _symSetIndex;
                symSetCustom = _symSetCustom;
                symSetUserFile = _symSetUserFile;
            }

            //----------------------------------------------------------------//

            public void Store(string downloadFile,
                              bool downloadRemove,
                              double height,
                              int symSetIndex,
                              ushort symSetCustom,
                              string symSetUserFile)
            {
                _downloadFile = downloadFile;
                _downloadRemove = downloadRemove;
                _height = height;
                _symSetIndex = symSetIndex;
                _symSetCustom = symSetCustom;
                _symSetUserFile = symSetUserFile;
            }
        }

        //--------------------------------------------------------------------//
        //                                                  S t r u c t u r e //
        // F o n t P r e s e t P C L                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private struct FontPresetPCL
        {
            int _fontIndex;
            PCLFonts.eVariant _variant;
            double _height;
            double _pitch;
            int _symSetIndex;
            ushort _symSetCustom;
            string _symSetUserFile;

            //----------------------------------------------------------------//

            public FontPresetPCL(int fontIndex,
                                  PCLFonts.eVariant variant,
                                  double height,
                                  double pitch,
                                  int symSetIndex,
                                  ushort symSetCustom,
                                  string symSetUserFile)
            {
                _fontIndex = fontIndex;
                _variant = variant;
                _height = height;
                _pitch = pitch;
                _symSetIndex = symSetIndex;
                _symSetCustom = symSetCustom;
                _symSetUserFile = symSetUserFile;
            }

            //----------------------------------------------------------------//

            public void Restore(ref int fontIndex,
                                ref PCLFonts.eVariant variant,
                                ref double height,
                                ref double pitch,
                                ref int symSetIndex,
                                ref ushort symSetCustom,
                                ref string symSetUserFile)
            {
                fontIndex = _fontIndex;
                variant = _variant;
                height = _height;
                pitch = _pitch;
                symSetIndex = _symSetIndex;
                symSetCustom = _symSetCustom;
                symSetUserFile = _symSetUserFile;
            }

            //----------------------------------------------------------------//

            public void Store(int fontIndex,
                              bool fontScalable,
                              bool fontBound,
                              PCLFonts.eVariant variant,
                              double height,
                              double pitch,
                              int symSetIndex,
                              ushort symSetCustom,
                              string symSetUserFile)
            {
                _fontIndex = fontIndex;
                _variant = variant;

                //     if (!fontBound)
                //     {
                _symSetIndex = symSetIndex;
                _symSetCustom = symSetCustom;
                _symSetUserFile = symSetUserFile;
                //     }

                if (fontScalable)
                {
                    _height = height;
                    _pitch = pitch;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                  S t r u c t u r e //
        // F o n t P r e s e t P C L X L                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private struct FontPresetPCLXL
        {
            int _fontIndex;
            PCLFonts.eVariant _variant;
            double _height;
            int _symSetIndex;
            ushort _symSetCustom;
            string _symSetUserFile;

            //--------------------------------------------------------------------//

            public FontPresetPCLXL(int fontIndex,
                                    PCLFonts.eVariant variant,
                                    double height,
                                    int symSetIndex,
                                    ushort symSetCustom,
                                    string symSetUserFile)
            {
                _fontIndex = fontIndex;
                _variant = variant;
                _height = height;
                _symSetIndex = symSetIndex;
                _symSetCustom = symSetCustom;
                _symSetUserFile = symSetUserFile;
            }

            //--------------------------------------------------------------------//

            public void Restore(ref int fontIndex,
                                ref PCLFonts.eVariant variant,
                                ref double height,
                                ref int symSetIndex,
                                ref ushort symSetCustom,
                                ref string symSetUserFile)
            {
                fontIndex = _fontIndex;
                variant = _variant;
                height = _height;
                symSetIndex = _symSetIndex;
                symSetCustom = _symSetCustom;
                symSetUserFile = _symSetUserFile;
            }

            //--------------------------------------------------------------------//

            public void Store(int fontIndex,
                              bool fontScalable,
                              bool fontBound,
                              PCLFonts.eVariant variant,
                              double height,
                              int symSetIndex,
                              ushort symSetCustom,
                              string symSetUserFile)
            {
                _fontIndex = fontIndex;
                _variant = variant;

                //      if (!fontBound)
                //      {
                _symSetIndex = symSetIndex;
                _symSetCustom = symSetCustom;
                _symSetUserFile = symSetUserFile;
                //      }

                if (fontScalable)
                {
                    _height = height;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                  S t r u c t u r e //
        // F o n t P r n D i s k P C L                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private struct FontPrnDiskPCL
        {
            string _fontName;
            ushort _fontId;
            ushort _macroId;
            bool _ramDataRemove;
            bool _selectById;
            bool _loadViaMacro;
            bool _characteristicsKnown;
            bool _proportional;
            bool _scalable;
            bool _bound;
            ushort _style;
            ushort _typeface;
            short _weight;
            double _height;
            double _pitch;
            int _symSetIndex;
            ushort _symSetCustom;
            string _symSetUserFile;

            //----------------------------------------------------------------//

            public FontPrnDiskPCL(string fontName,
                                   ushort fontId,
                                   ushort macroId,
                                   bool ramDataRemove,
                                   bool selectById,
                                   bool loadViaMacro,
                                   bool characteristicsKnown,
                                   bool proportional,
                                   bool scalable,
                                   bool bound,
                                   ushort style,
                                   ushort typeface,
                                   short weight,
                                   double height,
                                   double pitch,
                                   int symSetIndex,
                                   ushort symSetCustom,
                                   string symSetUserFile)
            {
                _fontName = fontName;
                _fontId = fontId;
                _macroId = macroId;
                _ramDataRemove = ramDataRemove;
                _selectById = selectById;
                _loadViaMacro = loadViaMacro;
                _characteristicsKnown = characteristicsKnown;
                _proportional = proportional;
                _scalable = scalable;
                _bound = bound;
                _style = style;
                _typeface = typeface;
                _weight = weight;
                _height = height;
                _pitch = pitch;
                _symSetIndex = symSetIndex;
                _symSetCustom = symSetCustom;
                _symSetUserFile = symSetUserFile;
            }

            //----------------------------------------------------------------//

            public void Restore(ref string fontName,
                                 ref ushort fontId,
                                 ref ushort macroId,
                                 ref bool ramDataRemove,
                                 ref bool selectById,
                                 ref bool loadViaMacro,
                                 ref bool characteristicsKnown,
                                 ref bool proportional,
                                 ref bool scalable,
                                 ref bool bound,
                                 ref ushort style,
                                 ref ushort typeface,
                                 ref short weight,
                                 ref double height,
                                 ref double pitch,
                                 ref int symSetIndex,
                                 ref ushort symSetCustom,
                                 ref string symSetUserFile)
            {
                fontName = _fontName;
                fontId = _fontId;
                macroId = _macroId;
                ramDataRemove = _ramDataRemove;
                selectById = _selectById;
                loadViaMacro = _loadViaMacro;
                characteristicsKnown = _characteristicsKnown;
                proportional = _proportional;
                scalable = _scalable;
                bound = _bound;
                style = _style;
                typeface = _typeface;
                weight = _weight;
                height = _height;
                pitch = _pitch;
                symSetIndex = _symSetIndex;
                symSetCustom = _symSetCustom;
                symSetUserFile = _symSetUserFile;
            }

            //----------------------------------------------------------------//

            public void Store(string fontName,
                               ushort fontId,
                               ushort macroId,
                               bool ramDataRemove,
                               bool selectById,
                               bool loadViaMacro,
                               bool characteristicsKnown,
                               bool proportional,
                               bool scalable,
                               bool bound,
                               ushort style,
                               ushort typeface,
                               short weight,
                               double height,
                               double pitch,
                               int symSetIndex,
                               ushort symSetCustom,
                               string symSetUserFile)
            {
                _fontName = fontName;
                _fontId = fontId;
                _macroId = macroId;
                _ramDataRemove = ramDataRemove;
                _selectById = selectById;
                _loadViaMacro = loadViaMacro;
                _characteristicsKnown = characteristicsKnown;
                _proportional = proportional;
                _scalable = scalable;
                _bound = bound;
                _style = style;
                _typeface = typeface;
                _weight = weight;
                _height = height;
                _pitch = pitch;
                _symSetIndex = symSetIndex;
                _symSetCustom = symSetCustom;
                _symSetUserFile = symSetUserFile;
            }
        }

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T o o l F o n t S a m p l e                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolFontSample(ref ToolCommonData.ePrintLang crntPDL)
        {
            InitializeComponent();

            Initialise();

            crntPDL = _crntPDL;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n G e n e r a t e _ C l i c k                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Generate Test Data' button is clicked.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            bool flagOK = true;

            _indxPDL = cbPDL.SelectedIndex;
            _crntPDL = (ToolCommonData.ePrintLang)_subsetPDLs[_indxPDL];

            if ((_symSetGroup == PCLSymbolSets.eSymSetGroup.UserSet) &&
               (!_symSetUserFileValid))
            {
                flagOK = CheckPCLSymSetFile();
            }

            if (flagOK)
            {
                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                {
                    if (_fontType == PCLFonts.eFontType.Download)
                    {
                        flagOK = CheckPCLSoftFontFile();
                    }
                }
                else
                {
                    if (_fontType == PCLFonts.eFontType.Download)
                    {
                        flagOK = CheckPCLXLFontFile();
                    }
                }
            }

            if (flagOK)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Generate test print file.                                  //
                //                                                            //
                //------------------------------------------------------------//

                try
                {
                    bool downloadFont = false;

                    BinaryWriter binWriter = null;

                    ushort[] sampleBlocks;

                    TargetCore.RequestStreamOpen(
                        ref binWriter,
                        ToolCommonData.eToolIds.FontSample,
                        ToolCommonData.eToolSubIds.None,
                        _crntPDL);

                    //--------------------------------------------------------//

                    if ((_ctSamplePages == 0) || (!_useSampleBlocks))
                    {
                        sampleBlocks = new ushort[1];

                        sampleBlocks[0] = 0;
                    }
                    else
                    {
                        CheckBox chk;

                        int pageNo = 0;

                        sampleBlocks = new ushort[_ctSamplePages];

                        for (int i = 0; i < _ctSampleOffsetBlocks; i++)
                        {
                            chk = (CheckBox)lstSampleOffsets.Items[i];

                            if (chk.IsChecked == true)
                            {
                                sampleBlocks[pageNo++] = _sampleOffsetBlocks[i];
                            }
                        }
                    }

                    //--------------------------------------------------------//

                    if (_fontType == PCLFonts.eFontType.Download)
                        downloadFont = true;

                    //--------------------------------------------------------//

                    if ((_symSetGroup == PCLSymbolSets.eSymSetGroup.Preset) ||
                        (_symSetGroup == PCLSymbolSets.eSymSetGroup.Unicode))
                    {
                        int index;

                        if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                            index = _subsetSymSets[_indxSymSetPCL];
                        else
                            index = _subsetSymSets[_indxSymSetPCLXL];

                        _symSetName = PCLSymbolSets.GetName(index);
                    }
                    else if (_symSetGroup == PCLSymbolSets.eSymSetGroup.UserSet)
                    {
                        if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                        {
                            if (_symSetUserActEmbedPCL)
                            {
                                _symSetName = "<user-defined via file>";
                            }
                            else
                            {
                                _symSetName = "<Unicode-indexed via file>";

                                _symSetId = PCLSymbolSets.GetId(
                                    PCLSymbolSets.IndexUnicode);
                            }
                        }
                        else if (_crntPDL == ToolCommonData.ePrintLang.PCLXL)
                        {
                            if (_symSetUserActEmbedPCLXL)
                            {
                                //never true!!
                                _symSetName = "<user-defined via file>";
                            }
                            else
                            {
                                _symSetName = "<Unicode-indexed via file>";

                                _symSetId = PCLSymbolSets.GetId(
                                    PCLSymbolSets.IndexUnicode);
                            }
                        }
                    }
                    else if (_symSetGroup == PCLSymbolSets.eSymSetGroup.Custom)
                    {
                        _symSetName = "<custom>";
                    }

                    //--------------------------------------------------------//

                    if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // PCL                                                //
                        //                                                    //
                        //----------------------------------------------------//

                        bool symSetUserSet,
                                 fontSelById;

                        double fontSize;

                        ushort fontIdNo;

                        string parseMethodText = string.Empty,
                               fontFilename;

                        PCLTextParsingMethods.eIndex parseMethod;

                        //----------------------------------------------------//

                        parseMethod =
                            (PCLTextParsingMethods.eIndex)_indxParseMethod;

                        if (parseMethod != PCLTextParsingMethods.eIndex.not_specified)
                        {
                            parseMethodText =
                                "; parse method " +
                                PCLTextParsingMethods.GetValue(_indxParseMethod);
                        }

                        //----------------------------------------------------//

                        if (_symSetGroup == PCLSymbolSets.eSymSetGroup.UserSet)
                            symSetUserSet = true;
                        else
                            symSetUserSet = false;

                        //----------------------------------------------------//

                        if (_fontType == PCLFonts.eFontType.Download)
                        {
                            fontFilename = _fontFilenamePCL;
                            fontIdNo = _fontDownloadIdPCL;
                            fontSelById = _downloadSelByIdPCL;
                        }
                        else if (_fontType == PCLFonts.eFontType.PrnDisk)
                        {
                            fontFilename = _fontPrnDiskNamePCL;
                            fontIdNo = _fontPrnDiskIdPCL;
                            fontSelById = true;
                        }
                        else
                        {
                            fontFilename = string.Empty;
                            fontIdNo = 0;
                            fontSelById = false;
                        }

                        //----------------------------------------------------//

                        if (_fontType != PCLFonts.eFontType.PrnDisk)
                        {
                            if (ValidatePCLFontCharacteristics())
                                SetFontSelectData();
                        }
                        else if (!_prnDiskSelByIdPCL)
                        {
                            if (ValidatePCLFontCharacteristics())
                                SetFontSelectData();
                        }

                        if (_fontProportional)
                            fontSize = _fontHeightPCL;
                        else
                            fontSize = _fontPitchPCL;

                        //----------------------------------------------------//

                        ToolFontSamplePCL.generateJob(
                            binWriter,
                            _fontType,
                            _subsetPaperSizes[_indxPaperSizePCL],
                            _subsetPaperTypes[_indxPaperTypePCL],
                            _subsetOrientations[_indxOrientationPCL],
                            _formAsMacroPCL,
                            _showC0CharsPCL,
                            _optGridVertical,
                            _fontBound,
                            SetFontTitle(_indxFontPCL),
                            _fontDesc + parseMethodText,
                            _symSetId,
                            _fontLoadDescPCL,
                            _fontSelDescPCL,
                            _fontSelSeqPCL,
                            _symSetName,
                            sampleBlocks,
                            parseMethod,
                            fontSize,
                            _fontProportional,
                            _downloadRemovePCL,
                            fontSelById,
                            _prnDiskFontDataKnownPCL,
                            _prnDiskLoadViaMacro,
                            fontIdNo,
                            _fontPrnDiskMacroIdPCL,
                            fontFilename,
                            symSetUserSet,
                            (_mapCodesRelevant == true) ? _showMapCodesUCS2PCL : false,
                            (_mapCodesRelevant == true) ? _showMapCodesUTF8PCL : false,
                            _symSetUserActEmbedPCL,
                            _symSetUserFile);
                    }
                    else    // if (_crntPDL == ToolCommonData.ePrintLang.PCLXL)
                    {
                        bool symSetUserSet;

                        if (_symSetGroup == PCLSymbolSets.eSymSetGroup.UserSet)
                            symSetUserSet = true;
                        else
                            symSetUserSet = false;

                        if (ValidatePCLXLFontCharacteristics())
                            SetFontSelectData();

                        ToolFontSamplePCLXL.generateJob(
                            binWriter,
                            _subsetPaperSizes[_indxPaperSizePCLXL],
                            _subsetPaperTypes[_indxPaperTypePCLXL],
                            _subsetOrientations[_indxOrientationPCLXL],
                            _formAsMacroPCLXL,
                            _showC0CharsPCLXL,
                            _optGridVertical,
                            SetFontTitle(_indxFontPCLXL),
                            _fontDesc,
                            _symSetNo,
                            _fontNamePCLXL,
                            _symSetName,
                            sampleBlocks,
                            _fontHeightPCLXL,
                            downloadFont,
                            _downloadRemovePCLXL,
                            _fontFilenamePCLXL,
                            symSetUserSet,
                            (_mapCodesRelevant == true) ? _showMapCodesUCS2PCLXL : false,
                            (_mapCodesRelevant == true) ? _showMapCodesUTF8PCLXL : false,
                            _symSetUserFile);
                    }

                    TargetCore.RequestStreamWrite(false);
                }

                catch (SocketException sockExc)
                {
                    MessageBox.Show(sockExc.ToString(),
                                    "Socket exception",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }

                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString(),
                                    "Unknown exception",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n P C L S o f t F o n t F i l e B r o w s e _ C l i c k        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Browse' button is clicked for a soft (download)   //
        // font.                                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnPCLSoftFontFileBrowse_Click(object sender,
                                                     RoutedEventArgs e)
        {
            bool selected;

            string filename = _fontFilenamePCL;

            selected = SelectPCLFontFile(ref filename);

            if (selected)
            {
                _fontFilenamePCL = filename;
                txtPCLSoftFontFile.Text = _fontFilenamePCL;

                SetFontOptions(_indxFontPCL, false, false);
                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n P C L X L F o n t F i l e B r o w s e _ C l i c k            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Browse' button is clicked for a 'download' font.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnPCLXLFontFileBrowse_Click(object sender,
                                                 RoutedEventArgs e)
        {
            bool selected;

            string filename = _fontFilenamePCLXL;

            selected = SelectPCLXLFontFile(ref filename);

            if (selected)
            {
                _fontFilenamePCLXL = filename;
                txtPCLXLSoftFontFile.Text = _fontFilenamePCLXL;

                SetFontOptions(_indxFontPCLXL, false, false);
                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n S a m p l e O f f s e t C l e a r A l l _ C l i c k          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Clear all' button associated with the range       //
        // offset values is clicked.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnSampleOffsetClearAll_Click(object sender,
                                                    RoutedEventArgs e)
        {
            CheckBox chk;

            for (int i = 0; i < _ctSampleOffsetBlocks; i++)
            {
                chk = (CheckBox)lstSampleOffsets.Items[i];
                chk.IsChecked = false;
            }

            _ctSamplePages = 0;
            txtSamplePageCt.Text = _ctSamplePages.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n S a m p l e O f f s e t S e l e c t A l l _ C l i c k        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Select all' button associated with the range      //
        // offset values is clicked.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnSampleOffsetSelectAll_Click(object sender,
                                                     RoutedEventArgs e)
        {
            CheckBox chk;

            for (int i = 0; i < _ctSampleOffsetBlocks; i++)
            {
                chk = (CheckBox)lstSampleOffsets.Items[i];
                chk.IsChecked = true;
            }

            _ctSamplePages = _ctSampleOffsetBlocks;
            txtSamplePageCt.Text = _ctSamplePages.ToString();
        }
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n S y m S e t F i l e B r o w s e _ C l i c k                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Activated when the Browse button on the User-defined symbol set    //
        // file panel is clicked.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnSymSetFileBrowse_Click(object sender,
                                                RoutedEventArgs e)
        {
            //----------------------------------------------------------------//
            //                                                                //
            // Invoke File/Open dialogue to select target Symbol set file.    //
            //                                                                //
            //----------------------------------------------------------------//

            bool selected;
            bool flagOK = true;

            string filename = _symSetUserFile;

            selected = SelectSymSetFile(ref filename);

            if (selected)
            {
                _symSetUserFile = filename;
                txtSymSetFile.Text = _symSetUserFile;

                flagOK = CheckPCLSymSetFile();

                SetSymSetAttributes();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b F o n t _ S e l e c t i o n C h a n g e d                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Font item has changed.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbFont_SelectionChanged(object sender,
                                             SelectionChangedEventArgs e)
        {
            if (_initialised && cbFont.HasItems)
            {
                FontChoicesStore();

                FontChoicesRestore();

                if (_fontType != PCLFonts.eFontType.PrnDisk)
                {
                    CheckFontSupportsSymSet();
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b O r i e n t a t i o n _ S e l e c t i o n C h a n g e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Orientation item has changed.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbOrientation_SelectionChanged(object sender,
                                                    SelectionChangedEventArgs e)
        {
            if (_initialised && cbOrientation.HasItems)
            {
                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                    _indxOrientationPCL = cbOrientation.SelectedIndex;
                else
                    _indxOrientationPCLXL = cbOrientation.SelectedIndex;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P a p e r S i z e _ S e l e c t i o n C h a n g e d            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Paper Size item has changed.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPaperSize_SelectionChanged(object sender,
                                                  SelectionChangedEventArgs e)
        {
            if (_initialised && cbPaperSize.HasItems)
            {
                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                    _indxPaperSizePCL = cbPaperSize.SelectedIndex;
                else
                    _indxPaperSizePCLXL = cbPaperSize.SelectedIndex;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P a p e r T y p e _ S e l e c t i o n C h a n g e d            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Paper Type item has changed.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPaperType_SelectionChanged(object sender,
                                                  SelectionChangedEventArgs e)
        {
            if (_initialised && cbPaperType.HasItems)
            {
                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                    _indxPaperTypePCL = cbPaperType.SelectedIndex;
                else
                    _indxPaperTypePCLXL = cbPaperType.SelectedIndex;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P a r s e M e t h o d _ S e l e c t i o n C h a n g e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text Parsing Method item has changed.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbParseMethod_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (_initialised && cbParseMethod.HasItems)
            {
                int index;

                index = cbParseMethod.SelectedIndex;

                _indxParseMethod = _subsetParseMethods[index];

                SetSymSetOffsetRanges(_symSetGroup, _symSetType,
                                      _indxParseMethod);

                SetShowCodesOptions();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P D L _ S e l e c t i o n C h a n g e d                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Print Language item has changed.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPDL_SelectionChanged(object sender,
                                            SelectionChangedEventArgs e)
        {
            if (_initialised)
            {
                PdlOptionsStore();
                FontChoicesStore();

                _indxPDL = cbPDL.SelectedIndex;
                _crntPDL = (ToolCommonData.ePrintLang)_subsetPDLs[_indxPDL];

                PdlOptionsRestore();
                FontChoicesRestore();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b S y m S e t _ S e l e c t i o n C h a n g e d                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Symbol Set item has changed.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbSymSet_SelectionChanged(object sender,
                                               SelectionChangedEventArgs e)
        {
            if (_initialised && cbSymSet.HasItems)
            {
                CheckSymSetType();

                SetSymSetAttributes();

                SetShowCodesOptions();

                CheckFontSupportsSymSet();

                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k F o n t S u p p o r t s S y m S e t                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check to see if selected symbol set is (probably) supported by the //
        // selected font.                                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void CheckFontSupportsSymSet()
        {
            bool check = true;

            int indxFont = cbFont.SelectedIndex;

            PCLFonts.eFontType fontType =
                PCLFonts.GetType(_subsetFonts[indxFont]);

            if (fontType != PCLFonts.eFontType.PrnDisk)
            {
                check = true;
            }

            if (check)
            {
                if ((PCLFonts.IsPresetFont(_subsetFonts[indxFont])) &&
                    (!PCLFonts.IsSymSetInList(_subsetFonts[indxFont],
                                             _symSetNo)))
                {
                    int symSetIndx = PCLSymbolSets.GetIndexForId(_symSetNo);

                    string symSetName,
                           symSetId;

                    if (symSetIndx == -1)
                        symSetName = "(<unknown>)";
                    else
                        symSetName = "(" + PCLSymbolSets.GetName(symSetIndx) + ")";

                    symSetId = PCLSymbolSets.TranslateKind1ToId(_symSetNo);

                    MessageBox.Show("Symbol set '" +
                                     symSetId + " " + symSetName +
                                    "' may not be supported by the '" +
                                    PCLFonts.GetName(_subsetFonts[indxFont]) +
                                    "' font",
                                     "Symbol Set / Font",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Warning);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k P C L S o f t F o n t F i l e                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check the contents of the PCL soft font (download) file.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool CheckPCLSoftFontFile()
        {
            bool flagOK = true;

            bool selected = true;

            string filename = _fontFilenamePCL;

            if (!File.Exists(filename))
            {
                selected = false;

                MessageBox.Show(
                    "File '" + filename + "' does not exist",
                    "PCL soft font download file",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                selected = SelectPCLFontFile(ref filename);
            }

            if (selected)
            {
                _fontFilenamePCL = filename;
                txtPCLSoftFontFile.Text = _fontFilenamePCL;

                flagOK = PCLDownloadFont.GetFontCharacteristics(
                            _fontFilenamePCL,
                            ref _fontProportional,
                            ref _fontScalable,
                            ref _fontBound,
                            ref _fontPitchPCL,
                            ref _fontHeightPCL,
                            ref _fontStylePCL,
                            ref _fontWeightPCL,
                            ref _fontTypefacePCL,
                            ref _symSetNo,
                            ref _fontSymSetTypePCL);
            }

            if ((!selected) || (!flagOK))
            {
                flagOK = false;

                MessageBox.Show("Either select a valid soft font file," +
                                 " or choose a standard font",
                                "PCL soft font download file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                Helper_WPFFocusFix.Focus(txtPCLSoftFontFile);   // need this to focus
                txtPCLSoftFontFile.SelectAll();
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k P C L S y m S e t F i l e                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check the contents of the PCL (download) symbol set file.          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool CheckPCLSymSetFile()
        {
            bool flagOK = true;

            bool selected = true;

            string filename = _symSetUserFile;

            if (!File.Exists(filename))
            {
                selected = false;

                MessageBox.Show(
                    "File '" + filename + "' does not exist",
                    "Symbol Set definition file",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                selected = SelectSymSetFile(ref filename);
            }

            if (selected)
            {
                _symSetUserFile = filename;
                txtSymSetFile.Text = _symSetUserFile;

                flagOK = PCLDownloadSymSet.CheckSymSetFile(
                    _symSetUserFile,
                    ref _symSetNoUserSet,
                    ref _symSetUserFirstCode,
                    ref _symSetUserLastCode,
                    ref _symSetType);

                if (flagOK)
                {
                    _symSetUserFileValid = true;

                    //    setSymSetAttributes (); // called separately
                }
                else
                {
                    _symSetUserFileValid = false;

                    PCLSymbolSets.SetDataUserSetDefault(_defaultSymSetNo);

                    _symSetNoUserSet = _defaultSymSetNo;

                    SetSymSetAttributes();
                }
            }

            if ((!selected) || (!flagOK))
            {
                flagOK = false;

                MessageBox.Show("Either select a valid symbol set file," +
                                 " or choose a specific symbol set",
                                "PCL user-defined symbol set",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                Helper_WPFFocusFix.Focus(txtSymSetFile);   // need this to focus
                txtSymSetFile.SelectAll();
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k P C L X L F o n t F i l e                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check the contents of the PCLXL (download) font file.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool CheckPCLXLFontFile()
        {
            bool flagOK = true;

            bool selected = true;

            string filename = _fontFilenamePCLXL;

            if (!File.Exists(filename))
            {
                selected = false;

                MessageBox.Show(
                    "File '" + filename + "' does not exist",
                    "PCLXL soft font download file",
                     MessageBoxButton.OK,
                     MessageBoxImage.Information);

                selected = SelectPCLXLFontFile(ref filename);
            }

            if (selected)
            {
                _fontFilenamePCLXL = filename;
                txtPCLXLSoftFontFile.Text = _fontFilenamePCLXL;

                flagOK = PCLXLDownloadFont.GetFontCharacteristics(
                                        _fontFilenamePCLXL,
                                        ref _fontNamePCLXL,
                                        ref _fontScalable,
                                        ref _fontBound,
                                        ref _symSetNo);
            }

            if ((!selected) || (!flagOK))
            {
                flagOK = false;

                MessageBox.Show("Either select a valid soft font file," +
                                 " or choose a standard font",
                                "PCLXL soft font download file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                Helper_WPFFocusFix.Focus(txtPCLXLSoftFontFile);   // need this to focus
                txtPCLXLSoftFontFile.SelectAll();
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k S y m S e t T y p e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check to see if selected symbol set is the special <user-defined>  //
        // value; if so, validate the nominated file.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void CheckSymSetType()
        {
            int indxSymSet;

            grpSymSetFile.Visibility = Visibility.Hidden;

            indxSymSet = cbSymSet.SelectedIndex;

            if (indxSymSet != -1)
            {
                int indxList = _subsetSymSets[indxSymSet];

                _symSetGroup = PCLSymbolSets.GetGroup(indxList);

                if (_symSetGroup == PCLSymbolSets.eSymSetGroup.UserSet)
                {
                    bool flagOK;

                    grpSymSetFile.Visibility = Visibility.Visible;

                    txtSymSetFile.Text = _symSetUserFile;

                    flagOK = CheckPCLSymSetFile();
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k O f f s e t _ C h e c k e d                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when a sample offset checkbox is checked.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkOffset_Checked(object sender, RoutedEventArgs e)
        {
            _ctSamplePages++;

            txtSamplePageCt.Text = _ctSamplePages.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k O f f s e t _ U n c h e c k e d                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when a sample offset checkbox is unchecked.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkOffset_Unchecked(object sender, RoutedEventArgs e)
        {
            _ctSamplePages--;

            txtSamplePageCt.Text = _ctSamplePages.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k O p t F o r m A s M a c r o _ C h e c k e d                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Option 'Render fixed text as overlay' checked.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkOptFormAsMacro_Checked(object sender,
                                                RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _formAsMacroPCL = true;
            else
                _formAsMacroPCLXL = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k O p t F o r m A s M a c r o _ U n c h e c k e d              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Option 'Render fixed text as overlay' unchecked.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkOptFormAsMacro_Unchecked(object sender,
                                                  RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _formAsMacroPCL = false;
            else
                _formAsMacroPCLXL = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k O p t S h o w C 0 C h a r s _ C h e c k e d                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Option 'Render fixed text as overlay' checked.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkOptShowC0Chars_Checked(object sender,
                                                RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _showC0CharsPCL = true;
            else
                _showC0CharsPCLXL = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k O p t S h o w C 0 C h a r s _ U n c h e c k e d              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Option 'Render fixed text as overlay' unchecked.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkOptShowC0Chars_Unchecked(object sender,
                                                  RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _showC0CharsPCL = false;
            else
                _showC0CharsPCLXL = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k O p t S h o w M a p C o d e s U C S 2 _ C h e c k e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Option 'Show UCS-2' checked.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkOptShowMapCodesUCS2_Checked(object sender,
                                                     RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _showMapCodesUCS2PCL = true;
            else
                _showMapCodesUCS2PCLXL = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k O p t S h o w M a p C o d e s U C S 2 _ U n c h e c k e d    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Option 'Show UCS-2' unchecked.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkOptShowMapCodesUCS2_Unchecked(object sender,
                                                       RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _showMapCodesUCS2PCL = false;
            else
                _showMapCodesUCS2PCLXL = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k O p t S h o w M a p C o d e s U T F 8 _ C h e c k e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Option 'Show UTF-8' checked.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkOptShowMapCodesUTF8_Checked(object sender,
                                                     RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _showMapCodesUTF8PCL = true;
            else
                _showMapCodesUTF8PCLXL = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k O p t S h o w M a p C o d e s U T F 8 _ U n c h e c k e d    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Option 'Show UTF-8' unchecked.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkOptShowMapCodesUTF8_Unchecked(object sender,
                                                       RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _showMapCodesUTF8PCL = false;
            else
                _showMapCodesUTF8PCLXL = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L P r n D i s k D a t a K n o w n _ C h e c k e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL option 'printer mass storage characteristics known' checked.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLPrnDiskDataKnown_Checked(object sender,
                                                     RoutedEventArgs e)
        {
            if (_initialised)
            {
                _prnDiskFontDataKnownPCL = true;

                rbPCLSelectByChar.IsEnabled = true;

                SetFontDesc();
                SetFontOptionsPCL(_indxFontPCL);
                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L P r n D i s k D a t a K n o w n _ U n c h e c k e d    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL option 'printer mass storage characteristics known' unchecked. //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLPrnDiskDataKnown_Unchecked(object sender,
                                                       RoutedEventArgs e)
        {
            if (_initialised)
            {
                _prnDiskFontDataKnownPCL = false;

                rbPCLSelectByChar.IsEnabled = false;

                SetFontDesc();
                SetFontOptionsPCL(_indxFontPCL);
                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L S o f t F o n t R e m o v e _ C h e c k e d            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL Option 'Remove soft font at end of job' checked.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLSoftFontRemove_Checked(object sender,
                                                   RoutedEventArgs e)
        {
            if (_prnDiskFontPCL)
                _prnDiskRemovePCL = true;
            else
                _downloadRemovePCL = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L S o f t F o n t R e m o v e _ U n c h e c k e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL Option 'Remove soft font at end of job' unchecked.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLSoftFontRemove_Unchecked(object sender,
                                                     RoutedEventArgs e)
        {
            if (_prnDiskFontPCL)
                _prnDiskRemovePCL = false;
            else
                _downloadRemovePCL = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L S o f t F o n t R e m o v e _ C h e c k e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL Option 'Remove soft font at end of job' checked.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLFontRemove_Checked(object sender,
                                                   RoutedEventArgs e)
        {
            _downloadRemovePCLXL = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L S o f t F o n t R e m o v e _ U n c h e c k e d    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL Option 'Remove soft font at end of job' unchecked.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLFontRemove_Unchecked(object sender,
                                                     RoutedEventArgs e)
        {
            _downloadRemovePCLXL = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t C h o i c e s R e s t o r e                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Restore font choice data.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void FontChoicesRestore()
        {
            bool samePreset = false;

            int indxFont = 0;

            indxFont = cbFont.SelectedIndex;

            _fontType = PCLFonts.GetType(_subsetFonts[indxFont]);

            chkPCLPrnDiskDataKnown.Visibility = Visibility.Hidden;
            grpSample.Visibility = Visibility.Hidden;

            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                if ((_fontType == PCLFonts.eFontType.PresetTypeface) ||
                    (_fontType == PCLFonts.eFontType.PresetFamilyMember))
                {
                    //--------------------------------------------------------//
                    //                                                        // 
                    // PCL font chosen from presented list.                  // 
                    //                                                        // 
                    //--------------------------------------------------------//

                    _fontPresetPCL.Restore(ref _indxFontPCL,
                                            ref _fontVar,
                                            ref _fontHeightPCL,
                                            ref _fontPitchPCL,
                                            ref _indxSymSetPCL,
                                            ref _symSetNo,
                                            ref _symSetUserFile);

                    //--------------------------------------------------------//

                    if (indxFont == _indxFontPCL)
                        samePreset = true;
                    else
                        _indxFontPCL = indxFont;
                }
                else if (_fontType == PCLFonts.eFontType.Download)
                {
                    //--------------------------------------------------------//
                    //                                                        // 
                    // PCL download font.                                     // 
                    //                                                        // 
                    //--------------------------------------------------------//

                    _fontDownloadPCL.Restore(ref _fontFilenamePCL,
                                              ref _fontDownloadIdPCL,
                                              ref _downloadRemovePCL,
                                              ref _downloadSelByIdPCL,
                                              ref _fontHeightPCL,
                                              ref _fontPitchPCL,
                                              ref _indxSymSetPCL,
                                              ref _symSetNo,
                                              ref _symSetUserFile);
                }
                else if (_fontType == PCLFonts.eFontType.PrnDisk)
                {
                    //--------------------------------------------------------//
                    //                                                        // 
                    // PCL printer mass storage font.                         // 
                    //                                                        // 
                    //--------------------------------------------------------//

                    _fontPrnDiskPCL.Restore(ref _fontPrnDiskNamePCL,
                                            ref _fontPrnDiskIdPCL,
                                            ref _fontPrnDiskMacroIdPCL,
                                            ref _prnDiskRemovePCL,
                                            ref _prnDiskSelByIdPCL,
                                            ref _prnDiskLoadViaMacro,
                                            ref _prnDiskFontDataKnownPCL,
                                            ref _fontProportional,
                                            ref _fontScalable,
                                            ref _fontBound,
                                            ref _fontStylePCL,
                                            ref _fontTypefacePCL,
                                            ref _fontWeightPCL,
                                            ref _fontHeightPCL,
                                            ref _fontPitchPCL,
                                            ref _indxSymSetPCL,
                                            ref _symSetNo,
                                            ref _symSetUserFile);

                    chkPCLPrnDiskDataKnown.Visibility = Visibility.Visible;

                    if (_prnDiskFontDataKnownPCL)
                        chkPCLPrnDiskDataKnown.IsChecked = true;
                    else
                        chkPCLPrnDiskDataKnown.IsChecked = false;
                }
                else // if (_fontType == PCLFonts.eFontType.Custom)
                {
                    //--------------------------------------------------------//
                    //                                                        // 
                    // PCL font to be selected via specified characteristics.// 
                    //                                                        // 
                    //--------------------------------------------------------//

                    _fontCustomPCL.Restore(ref _fontProportional,
                                            ref _fontScalable,
                                            ref _fontBound,
                                            ref _fontStylePCL,
                                            ref _fontTypefacePCL,
                                            ref _fontWeightPCL,
                                            ref _fontHeightPCL,
                                            ref _fontPitchPCL,
                                            ref _indxSymSetPCL,
                                            ref _symSetNo,
                                            ref _symSetUserFile);
                }
            }
            else // _indxPDL == ToolCommonData.ePrintLang.PCLXL
            {
                if ((_fontType == PCLFonts.eFontType.PresetTypeface) ||
                    (_fontType == PCLFonts.eFontType.PresetFamilyMember))
                {
                    //--------------------------------------------------------//
                    //                                                        // 
                    // PCLXL font chosen from presented list.                  // 
                    //                                                        // 
                    //--------------------------------------------------------//

                    _fontPresetPCLXL.Restore(ref _indxFontPCLXL,
                                              ref _fontVar,
                                              ref _fontHeightPCLXL,
                                              ref _indxSymSetPCLXL,
                                              ref _symSetNo,
                                              ref _symSetUserFile);

                    if (indxFont == _indxFontPCLXL)
                        samePreset = true;
                    else
                        _indxFontPCLXL = indxFont;
                }
                else if (_fontType == PCLFonts.eFontType.Download)
                {
                    //--------------------------------------------------------//
                    //                                                        // 
                    // PCLXL download font.                                   // 
                    //                                                        // 
                    //--------------------------------------------------------//

                    _fontDownloadPCLXL.Restore(ref _fontFilenamePCLXL,
                                                ref _downloadRemovePCLXL,
                                                ref _fontHeightPCLXL,
                                                ref _indxSymSetPCLXL,
                                                ref _symSetNo,
                                                ref _symSetUserFile);
                }
                else // if (_fontType == PCLFonts.eFontType.Custom)
                {
                    //--------------------------------------------------------//
                    //                                                        // 
                    // PCLXL font to be selected via specified characteristics.// 
                    //                                                        // 
                    //--------------------------------------------------------//

                    _fontCustomPCLXL.Restore(ref _fontNamePCLXL,
                                              ref _fontHeightPCLXL,
                                              ref _indxSymSetPCLXL,
                                              ref _symSetNo,
                                              ref _symSetUserFile);
                }
            }

            SetFontOptions(indxFont, true, samePreset);

            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t C h o i c e s S t o r e                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store font choice data.                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void FontChoicesStore()
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                if ((_fontType == PCLFonts.eFontType.PresetTypeface) ||
                    (_fontType == PCLFonts.eFontType.PresetFamilyMember))
                {
                    _fontPresetPCL.Store(_indxFontPCL,
                                           _fontScalable,
                                           _fontBound,
                                          _fontVar,
                                          _fontHeightPCL,
                                          _fontPitchPCL,
                                          _indxSymSetPCL,
                                          _symSetNo,
                                          _symSetUserFile);
                }
                else if (_fontType == PCLFonts.eFontType.Download)
                {
                    _fontDownloadPCL.Store(_fontFilenamePCL,
                                            _fontDownloadIdPCL,
                                            _downloadRemovePCL,
                                            _downloadSelByIdPCL,
                                            _fontHeightPCL,
                                            _fontPitchPCL,
                                            _indxSymSetPCL,
                                            _symSetNo,
                                            _symSetUserFile);
                }
                else if (_fontType == PCLFonts.eFontType.PrnDisk)
                {
                    _fontPrnDiskPCL.Store(_fontPrnDiskNamePCL,
                                          _fontPrnDiskIdPCL,
                                          _fontPrnDiskMacroIdPCL,
                                          _prnDiskRemovePCL,
                                          _prnDiskSelByIdPCL,
                                          _prnDiskLoadViaMacro,
                                          _prnDiskFontDataKnownPCL,
                                          _fontProportional,
                                          _fontScalable,
                                          _fontBound,
                                          _fontStylePCL,
                                          _fontTypefacePCL,
                                          _fontWeightPCL,
                                          _fontHeightPCL,
                                          _fontPitchPCL,
                                          _indxSymSetPCL,
                                          _symSetNo,
                                          _symSetUserFile);
                }
                else // if (_fontType == PCLFonts.eFontType.Custom)
                {
                    _fontCustomPCL.Store(_fontProportional,
                                          _fontScalable,
                                          _fontBound,
                                          _fontStylePCL,
                                          _fontTypefacePCL,
                                          _fontWeightPCL,
                                          _fontHeightPCL,
                                          _fontPitchPCL,
                                          _indxSymSetPCL,
                                          _symSetNo,
                                          _symSetUserFile);
                }
            }
            else // _indxPDL == ToolCommonData.ePrintLang.PCLXL
            {
                if ((_fontType == PCLFonts.eFontType.PresetTypeface) ||
                    (_fontType == PCLFonts.eFontType.PresetFamilyMember))
                {
                    _fontPresetPCLXL.Store(_indxFontPCLXL,
                                            _fontScalable,
                                            _fontBound,
                                            _fontVar,
                                            _fontHeightPCLXL,
                                            _indxSymSetPCLXL,
                                            _symSetNo,
                                            _symSetUserFile);
                }
                else if (_fontType == PCLFonts.eFontType.Download)
                {
                    _fontDownloadPCLXL.Store(_fontFilenamePCLXL,
                                              _downloadRemovePCLXL,
                                              _fontHeightPCLXL,
                                              _indxSymSetPCLXL,
                                              _symSetNo,
                                              _symSetUserFile);
                }
                else // if (_fontType == PCLFonts.eFontType.Custom)
                {
                    _fontCustomPCLXL.Store(_fontNamePCLXL,
                                            _fontHeightPCLXL,
                                            _indxSymSetPCLXL,
                                            _symSetNo,
                                            _symSetUserFile);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g i v e C r n t P D L                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GiveCrntPDL(ref ToolCommonData.ePrintLang crntPDL)
        {
            crntPDL = _crntPDL;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialisation.                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void Initialise()
        {
            int index,
                  ctr;

            PCLSymbolSets.eSymSetGroup symSetGroup;

            _initialised = false;

            //----------------------------------------------------------------//
            //                                                                //
            // Populate form.                                                 //
            //                                                                //
            //----------------------------------------------------------------//

            cbPDL.Items.Clear();

            _ctPDLs = _subsetPDLs.Length;

            for (int i = 0; i < _ctPDLs; i++)
            {
                index = _subsetPDLs[i];

                cbPDL.Items.Add(Enum.GetName(
                    typeof(ToolCommonData.ePrintLang), i));
            }

            //----------------------------------------------------------------//

            cbOrientation.Items.Clear();

            _ctOrientations = _subsetOrientations.Length;

            for (int i = 0; i < _ctOrientations; i++)
            {
                index = _subsetOrientations[i];

                cbOrientation.Items.Add(PCLOrientations.GetName(index));
            }

            //----------------------------------------------------------------//

            cbPaperSize.Items.Clear();

            _ctPaperSizes = _subsetPaperSizes.Length;

            for (int i = 0; i < _ctPaperSizes; i++)
            {
                index = _subsetPaperSizes[i];

                cbPaperSize.Items.Add(PCLPaperSizes.GetName(index));
            }

            //----------------------------------------------------------------//

            cbPaperType.Items.Clear();

            _ctPaperTypes = _subsetPaperTypes.Length;

            for (int i = 0; i < _ctPaperTypes; i++)
            {
                index = _subsetPaperTypes[i];

                cbPaperType.Items.Add(PCLPaperTypes.GetName(index));
            }

            //----------------------------------------------------------------//

            cbFont.Items.Clear();

            ctr = PCLFonts.GetCount();
            index = 0;

            for (int i = 0; i < ctr; i++)
            {
                if (PCLFonts.GetType(i) != PCLFonts.eFontType.PresetFamily)
                {
                    _subsetFonts[index++] = i;
                    cbFont.Items.Add(PCLFonts.GetName(i));
                }
            }

            //----------------------------------------------------------------//

            cbSymSet.Items.Clear();

            ctr = PCLSymbolSets.GetCount();
            index = 0;

            for (int i = 0; i < ctr; i++)
            {
                symSetGroup = PCLSymbolSets.GetGroup(i);

                if ((symSetGroup == PCLSymbolSets.eSymSetGroup.Custom) ||
                    (symSetGroup == PCLSymbolSets.eSymSetGroup.Preset) ||
                    (symSetGroup == PCLSymbolSets.eSymSetGroup.Unicode) ||
                    (symSetGroup == PCLSymbolSets.eSymSetGroup.UserSet))
                {
                    _subsetSymSets[index++] = i;
                    cbSymSet.Items.Add(PCLSymbolSets.GetName(i));
                }
            }

            //----------------------------------------------------------------//

            ResetTarget();

            //----------------------------------------------------------------//
            //                                                                //
            // Reinstate settings from persistent storage.                    //
            //                                                                //
            //----------------------------------------------------------------//

            MetricsLoad();

            PdlOptionsRestore();
            FontChoicesRestore();

            cbPDL.SelectedIndex = (byte)_indxPDL;

            if (_optGridVertical)
                rbOptGridV.IsChecked = true;
            else
                rbOptGridH.IsChecked = true;

            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                if (_showMapCodesUCS2PCL)
                    chkOptShowMapCodesUCS2.IsChecked = true;
                else
                    chkOptShowMapCodesUCS2.IsChecked = false;

                if (_showMapCodesUTF8PCL)
                    chkOptShowMapCodesUTF8.IsChecked = true;
                else
                    chkOptShowMapCodesUTF8.IsChecked = false;

                if (_symSetUserActEmbedPCL)
                    rbSymSetUserActEmbed.IsChecked = true;
                else
                    rbSymSetUserActIndex.IsChecked = true;
            }
            else if (_crntPDL == ToolCommonData.ePrintLang.PCLXL)
            {
                if (_showMapCodesUCS2PCLXL)
                    chkOptShowMapCodesUCS2.IsChecked = true;
                else
                    chkOptShowMapCodesUCS2.IsChecked = false;

                if (_showMapCodesUTF8PCLXL)
                    chkOptShowMapCodesUTF8.IsChecked = true;
                else
                    chkOptShowMapCodesUTF8.IsChecked = false;

                rbSymSetUserActIndex.IsChecked = true;
            }

            _symSetUserFileValid = false;
            _settingSymSetAttributes = false;

            CheckSymSetType();

            SetSymSetAttributes();

            SetShowCodesOptions();

            CheckFontSupportsSymSet();

            SetFontSelectData();

            _initialised = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load metrics from persistent storage.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void MetricsLoad()
        {
            int indxFontTemp = 0;
            int indxFont = 0;

            ToolFontSamplePersist.LoadDataCommon(ref _indxPDL,
                                                 ref _optGridVertical);

            ToolFontSamplePersist.LoadDataGeneral("PCL",
                                                  ref _indxOrientationPCL,
                                                  ref _indxPaperSizePCL,
                                                  ref _indxPaperTypePCL,
                                                  ref indxFontTemp,
                                                  ref _formAsMacroPCL,
                                                  ref _showC0CharsPCL,
                                                  ref _showMapCodesUCS2PCL,
                                                  ref _showMapCodesUTF8PCL,
                                                  ref _symSetUserActEmbedPCL);

            ToolFontSamplePersist.LoadDataPCLCustom(ref _fontProportional,
                                                     ref _fontScalable,
                                                     ref _fontBound,
                                                     ref _fontStylePCL,
                                                     ref _fontTypefacePCL,
                                                     ref _fontWeightPCL,
                                                     ref _fontHeightPCL,
                                                     ref _fontPitchPCL,
                                                     ref _indxSymSetPCL,
                                                     ref _symSetNo,
                                                     ref _symSetUserFile);

            _fontCustomPCL.Store(_fontProportional,
                                  _fontScalable,
                                  _fontBound,
                                  _fontStylePCL,
                                  _fontTypefacePCL,
                                  _fontWeightPCL,
                                  _fontHeightPCL,
                                  _fontPitchPCL,
                                  _indxSymSetPCL,
                                  _symSetNo,
                                  _symSetUserFile);

            ToolFontSamplePersist.LoadDataPCLDownload(ref _fontFilenamePCL,
                                                       ref _fontDownloadIdPCL,
                                                       ref _downloadRemovePCL,
                                                       ref _downloadSelByIdPCL,
                                                       ref _fontHeightPCL,
                                                       ref _fontPitchPCL,
                                                       ref _indxSymSetPCL,
                                                       ref _symSetNo,
                                                       ref _symSetUserFile);

            _fontDownloadPCL.Store(_fontFilenamePCL,
                                    _fontDownloadIdPCL,
                                    _downloadRemovePCL,
                                    _downloadSelByIdPCL,
                                    _fontHeightPCL,
                                    _fontPitchPCL,
                                    _indxSymSetPCL,
                                    _symSetNo,
                                    _symSetUserFile);

            ToolFontSamplePersist.LoadDataPCLPrnDisk(ref _fontPrnDiskNamePCL,
                                                     ref _fontPrnDiskIdPCL,
                                                     ref _fontPrnDiskMacroIdPCL,
                                                     ref _prnDiskRemovePCL,
                                                     ref _prnDiskSelByIdPCL,
                                                     ref _prnDiskLoadViaMacro,
                                                     ref _prnDiskFontDataKnownPCL,
                                                     ref _fontProportional,
                                                     ref _fontScalable,
                                                     ref _fontBound,
                                                     ref _fontStylePCL,
                                                     ref _fontTypefacePCL,
                                                     ref _fontWeightPCL,
                                                     ref _fontHeightPCL,
                                                     ref _fontPitchPCL,
                                                     ref _indxSymSetPCL,
                                                     ref _symSetNo,
                                                     ref _symSetUserFile);

            _fontPrnDiskPCL.Store(_fontPrnDiskNamePCL,
                                  _fontPrnDiskIdPCL,
                                  _fontPrnDiskMacroIdPCL,
                                  _prnDiskRemovePCL,
                                  _prnDiskSelByIdPCL,
                                  _prnDiskLoadViaMacro,
                                  _prnDiskFontDataKnownPCL,
                                  _fontProportional,
                                  _fontScalable,
                                  _fontBound,
                                  _fontStylePCL,
                                  _fontTypefacePCL,
                                  _fontWeightPCL,
                                  _fontHeightPCL,
                                  _fontPitchPCL,
                                  _indxSymSetPCL,
                                  _symSetNo,
                                  _symSetUserFile);

            ToolFontSamplePersist.LoadDataPCLPreset(ref _indxFontPCL,
                                                     ref _fontVar,
                                                     ref _fontHeightPCL,
                                                     ref _fontPitchPCL,
                                                     ref _indxSymSetPCL,
                                                     ref _symSetNo,
                                                     ref _symSetUserFile);

            indxFont = _subsetFonts[_indxFontPCL];

            _fontBound = PCLFonts.IsBoundFont(indxFont);
            _fontScalable = PCLFonts.IsScalableFont(indxFont);
            _fontProportional = PCLFonts.IsProportionalFont(indxFont);

            _fontPresetPCL.Store(_indxFontPCL,
                                  _fontScalable,
                                  _fontBound,
                                  _fontVar,
                                  _fontHeightPCL,
                                  _fontPitchPCL,
                                  _indxSymSetPCL,
                                  _symSetNo,
                                  _symSetUserFile);

            _indxFontPCL = indxFontTemp;

            //----------------------------------------------------------------//

            ToolFontSamplePersist.LoadDataGeneral("PCLXL",
                                                  ref _indxOrientationPCLXL,
                                                  ref _indxPaperSizePCLXL,
                                                  ref _indxPaperTypePCLXL,
                                                  ref indxFontTemp,
                                                  ref _formAsMacroPCLXL,
                                                  ref _showC0CharsPCLXL,
                                                  ref _showMapCodesUCS2PCLXL,
                                                  ref _showMapCodesUTF8PCLXL,
                                                  ref _symSetUserActEmbedPCLXL);

            ToolFontSamplePersist.LoadDataPCLXLCustom(ref _fontNamePCLXL,
                                                     ref _fontHeightPCLXL,
                                                     ref _indxSymSetPCLXL,
                                                     ref _symSetNo,
                                                     ref _symSetUserFile);

            _fontCustomPCLXL.Store(_fontNamePCLXL,
                                    _fontHeightPCLXL,
                                    _indxSymSetPCLXL,
                                    _symSetNo,
                                    _symSetUserFile);

            ToolFontSamplePersist.LoadDataPCLXLDownload(ref _fontFilenamePCLXL,
                                                       ref _downloadRemovePCLXL,
                                                       ref _fontHeightPCLXL,
                                                       ref _indxSymSetPCLXL,
                                                       ref _symSetNo,
                                                       ref _symSetUserFile);

            _fontDownloadPCLXL.Store(_fontFilenamePCLXL,
                                      _downloadRemovePCLXL,
                                      _fontHeightPCLXL,
                                      _indxSymSetPCLXL,
                                      _symSetNo,
                                      _symSetUserFile);

            ToolFontSamplePersist.LoadDataPCLXLPreset(ref _indxFontPCLXL,
                                                     ref _fontVar,
                                                     ref _fontHeightPCLXL,
                                                     ref _indxSymSetPCLXL,
                                                     ref _symSetNo,
                                                     ref _symSetUserFile);

            indxFont = _subsetFonts[_indxFontPCLXL];

            _fontBound = PCLFonts.IsBoundFont(indxFont);
            _fontScalable = PCLFonts.IsScalableFont(indxFont);
            _fontProportional = PCLFonts.IsProportionalFont(indxFont);

            _fontPresetPCLXL.Store(_indxFontPCLXL,
                                    _fontScalable,
                                    _fontBound,
                                    _fontVar,
                                    _fontHeightPCLXL,
                                    _indxSymSetPCLXL,
                                    _symSetNo,
                                    _symSetUserFile);

            _indxFontPCLXL = indxFontTemp;

            //----------------------------------------------------------------//

            if ((_indxPDL < 0) || (_indxPDL >= _ctPDLs))
                _indxPDL = 0;

            _crntPDL = (ToolCommonData.ePrintLang)_subsetPDLs[_indxPDL];

            //----------------------------------------------------------------//

            if ((_indxOrientationPCL < 0) ||
                (_indxOrientationPCL >= _ctOrientations))
                _indxOrientationPCL = 0;

            if ((_indxPaperSizePCL < 0) ||
                (_indxPaperSizePCL >= _ctPaperSizes))
                _indxPaperSizePCL = 0;

            if ((_indxPaperTypePCL < 0) ||
                (_indxPaperTypePCL >= _ctPaperTypes))
                _indxPaperTypePCL = 0;

            if ((_indxFontPCL < 0) ||
                (_indxFontPCL >= _ctFonts))
                _indxFontPCL = 0;

            if ((_indxSymSetPCL < 0) ||
                (_indxSymSetPCL >= _ctSymSets))
                _indxSymSetPCL = 0;

            //----------------------------------------------------------------//

            if ((_indxOrientationPCLXL < 0) ||
                (_indxOrientationPCLXL >= _ctOrientations))
                _indxOrientationPCLXL = 0;

            if ((_indxPaperSizePCLXL < 0) ||
                (_indxPaperSizePCLXL >= _ctPaperSizes))
                _indxPaperSizePCLXL = 0;

            if ((_indxPaperTypePCLXL < 0) ||
                (_indxPaperTypePCLXL >= _ctPaperTypes))
                _indxPaperTypePCLXL = 0;

            if ((_indxFontPCLXL < 0) ||
                (_indxFontPCLXL >= _ctFonts))
                _indxFontPCLXL = 0;

            if ((_indxSymSetPCLXL < 0) ||
                (_indxSymSetPCLXL >= _ctSymSets))
                _indxSymSetPCLXL = 0;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s S a v e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Save current metrics to persistent storage.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void MetricsSave()
        {
            PdlOptionsStore();
            FontChoicesStore();

            ToolFontSamplePersist.SaveDataCommon(_indxPDL,
                                                  _optGridVertical);

            ToolFontSamplePersist.SaveDataGeneral("PCL",
                                                  _indxOrientationPCL,
                                                  _indxPaperSizePCL,
                                                  _indxPaperTypePCL,
                                                  _indxFontPCL,
                                                  _formAsMacroPCL,
                                                  _showC0CharsPCL,
                                                  _showMapCodesUCS2PCL,
                                                  _showMapCodesUTF8PCL,
                                                  _symSetUserActEmbedPCL);

            _fontCustomPCL.Restore(ref _fontProportional,
                                    ref _fontScalable,
                                    ref _fontBound,
                                    ref _fontStylePCL,
                                    ref _fontTypefacePCL,
                                    ref _fontWeightPCL,
                                    ref _fontHeightPCL,
                                    ref _fontPitchPCL,
                                    ref _indxSymSetPCL,
                                    ref _symSetNo,
                                    ref _symSetUserFile);

            ToolFontSamplePersist.SaveDataPCLCustom(_fontProportional,
                                                     _fontScalable,
                                                     _fontBound,
                                                     _fontStylePCL,
                                                     _fontTypefacePCL,
                                                     _fontWeightPCL,
                                                     _fontHeightPCL,
                                                     _fontPitchPCL,
                                                     _indxSymSetPCL,
                                                     _symSetNo,
                                                     _symSetUserFile);

            _fontDownloadPCL.Restore(ref _fontFilenamePCL,
                                      ref _fontDownloadIdPCL,
                                      ref _downloadRemovePCL,
                                      ref _downloadSelByIdPCL,
                                      ref _fontHeightPCL,
                                      ref _fontPitchPCL,
                                      ref _indxSymSetPCL,
                                      ref _symSetNo,
                                      ref _symSetUserFile);

            ToolFontSamplePersist.SaveDataPCLDownload(_fontFilenamePCL,
                                                       _fontDownloadIdPCL,
                                                       _downloadRemovePCL,
                                                       _downloadSelByIdPCL,
                                                       _fontHeightPCL,
                                                       _fontPitchPCL,
                                                       _indxSymSetPCL,
                                                       _symSetNo,
                                                       _symSetUserFile);

            _fontPrnDiskPCL.Restore(ref _fontPrnDiskNamePCL,
                                    ref _fontPrnDiskIdPCL,
                                    ref _fontPrnDiskMacroIdPCL,
                                    ref _prnDiskRemovePCL,
                                    ref _prnDiskSelByIdPCL,
                                    ref _prnDiskLoadViaMacro,
                                    ref _prnDiskFontDataKnownPCL,
                                    ref _fontProportional,
                                    ref _fontScalable,
                                    ref _fontBound,
                                    ref _fontStylePCL,
                                    ref _fontTypefacePCL,
                                    ref _fontWeightPCL,
                                    ref _fontHeightPCL,
                                    ref _fontPitchPCL,
                                    ref _indxSymSetPCL,
                                    ref _symSetNo,
                                    ref _symSetUserFile);

            ToolFontSamplePersist.SaveDataPCLPrnDisk(_fontPrnDiskNamePCL,
                                                     _fontPrnDiskIdPCL,
                                                     _fontPrnDiskMacroIdPCL,
                                                     _prnDiskRemovePCL,
                                                     _prnDiskSelByIdPCL,
                                                     _prnDiskLoadViaMacro,
                                                     _prnDiskFontDataKnownPCL,
                                                     _fontProportional,
                                                     _fontScalable,
                                                     _fontBound,
                                                     _fontStylePCL,
                                                     _fontTypefacePCL,
                                                     _fontWeightPCL,
                                                     _fontHeightPCL,
                                                     _fontPitchPCL,
                                                     _indxSymSetPCL,
                                                     _symSetNo,
                                                     _symSetUserFile);

            _fontPresetPCL.Restore(ref _indxFontPCL,
                                    ref _fontVar,
                                    ref _fontHeightPCL,
                                    ref _fontPitchPCL,
                                    ref _indxSymSetPCL,
                                    ref _symSetNo,
                                    ref _symSetUserFile);

            ToolFontSamplePersist.SaveDataPCLPreset(_indxFontPCL,
                                                     _fontVar,
                                                     _fontHeightPCL,
                                                     _fontPitchPCL,
                                                     _indxSymSetPCL,
                                                     _symSetNo,
                                                     _symSetUserFile);

            //----------------------------------------------------------------//

            ToolFontSamplePersist.SaveDataGeneral("PCLXL",
                                                  _indxOrientationPCLXL,
                                                  _indxPaperSizePCLXL,
                                                  _indxPaperTypePCLXL,
                                                  _indxFontPCLXL,
                                                  _formAsMacroPCLXL,
                                                  _showC0CharsPCLXL,
                                                  _showMapCodesUCS2PCLXL,
                                                  _showMapCodesUTF8PCLXL,
                                                  _symSetUserActEmbedPCLXL);

            _fontCustomPCLXL.Restore(ref _fontNamePCLXL,
                                      ref _fontHeightPCLXL,
                                      ref _indxSymSetPCLXL,
                                      ref _symSetNo,
                                      ref _symSetUserFile);

            ToolFontSamplePersist.SaveDataPCLXLCustom(_fontNamePCLXL,
                                                       _fontHeightPCLXL,
                                                       _indxSymSetPCLXL,
                                                       _symSetNo,
                                                       _symSetUserFile);

            _fontDownloadPCLXL.Restore(ref _fontFilenamePCLXL,
                                        ref _downloadRemovePCLXL,
                                        ref _fontHeightPCLXL,
                                        ref _indxSymSetPCLXL,
                                        ref _symSetNo,
                                        ref _symSetUserFile);

            ToolFontSamplePersist.SaveDataPCLXLDownload(_fontFilenamePCLXL,
                                                         _downloadRemovePCLXL,
                                                         _fontHeightPCLXL,
                                                         _indxSymSetPCLXL,
                                                         _symSetNo,
                                                         _symSetUserFile);

            _fontPresetPCLXL.Restore(ref _indxFontPCLXL,
                                      ref _fontVar,
                                      ref _fontHeightPCLXL,
                                      ref _indxSymSetPCLXL,
                                      ref _symSetNo,
                                      ref _symSetUserFile);

            ToolFontSamplePersist.SaveDataPCLXLPreset(_indxFontPCLXL,
                                                       _fontVar,
                                                       _fontHeightPCLXL,
                                                       _indxSymSetPCLXL,
                                                       _symSetNo,
                                                       _symSetUserFile);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p d l O p t i o n s R e s t o r e                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Restore the test metrics options for the current PDL.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void PdlOptionsRestore()
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                cbOrientation.SelectedIndex = _indxOrientationPCL;
                cbPaperSize.SelectedIndex = _indxPaperSizePCL;
                cbPaperType.SelectedIndex = _indxPaperTypePCL;
                cbFont.SelectedIndex = _indxFontPCL;
                chkOptFormAsMacro.IsChecked = _formAsMacroPCL;
                chkOptShowC0Chars.IsChecked = _showC0CharsPCL;
                chkOptShowMapCodesUCS2.IsChecked = _showMapCodesUCS2PCL;
                rbSymSetUserActEmbed.IsChecked = _symSetUserActEmbedPCL;

                rbSymSetUserActEmbed.IsEnabled = true;
                tabPCL.IsEnabled = true;
                tabPCLXL.IsEnabled = false;
                tabPCL.IsSelected = true;
            }
            else
            {
                cbOrientation.SelectedIndex = _indxOrientationPCLXL;
                cbPaperSize.SelectedIndex = _indxPaperSizePCLXL;
                cbPaperType.SelectedIndex = _indxPaperTypePCLXL;
                cbFont.SelectedIndex = _indxFontPCLXL;
                chkOptFormAsMacro.IsChecked = _formAsMacroPCLXL;
                chkOptShowC0Chars.IsChecked = _showC0CharsPCLXL;
                chkOptShowMapCodesUCS2.IsChecked = _showMapCodesUCS2PCLXL;
                rbSymSetUserActIndex.IsChecked = true;

                rbSymSetUserActEmbed.IsEnabled = false;
                tabPCL.IsEnabled = false;
                tabPCLXL.IsEnabled = true;
                tabPCLXL.IsSelected = true;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p d l O p t i o n s S t o r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store the test metrics options for the current PDL and font type.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void PdlOptionsStore()
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                _indxOrientationPCL = cbOrientation.SelectedIndex;
                _indxPaperSizePCL = cbPaperSize.SelectedIndex;
                _indxPaperTypePCL = cbPaperType.SelectedIndex;

                _indxFontPCL = cbFont.SelectedIndex;

                if (chkOptFormAsMacro.IsChecked == true)
                    _formAsMacroPCL = true;
                else
                    _formAsMacroPCL = false;

                if (chkOptShowC0Chars.IsChecked == true)
                    _showC0CharsPCL = true;
                else
                    _showC0CharsPCL = false;

                if (chkOptShowMapCodesUCS2.IsChecked == true)
                    _showMapCodesUCS2PCL = true;
                else
                    _showMapCodesUCS2PCL = false;

                if (chkOptShowMapCodesUTF8.IsChecked == true)
                    _showMapCodesUTF8PCL = true;
                else
                    _showMapCodesUTF8PCL = false;

                if (rbSymSetUserActEmbed.IsChecked == true)
                    _symSetUserActEmbedPCL = true;
                else
                    _symSetUserActEmbedPCL = false;
            }
            else
            {
                _indxOrientationPCLXL = cbOrientation.SelectedIndex;
                _indxPaperSizePCLXL = cbPaperSize.SelectedIndex;
                _indxPaperTypePCLXL = cbPaperType.SelectedIndex;

                _indxFontPCLXL = cbFont.SelectedIndex;

                if (chkOptFormAsMacro.IsChecked == true)
                    _formAsMacroPCLXL = true;
                else
                    _formAsMacroPCLXL = false;

                if (chkOptShowC0Chars.IsChecked == true)
                    _showC0CharsPCLXL = true;
                else
                    _showC0CharsPCLXL = false;

                if (chkOptShowMapCodesUCS2.IsChecked == true)
                    _showMapCodesUCS2PCLXL = true;
                else
                    _showMapCodesUCS2PCLXL = false;

                if (chkOptShowMapCodesUTF8.IsChecked == true)
                    _showMapCodesUTF8PCLXL = true;
                else
                    _showMapCodesUTF8PCLXL = false;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b F o n t V a r B _ C l i c k                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Bold' font variant radio button is selected.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbFontVarB_Click(object sender, RoutedEventArgs e)
        {
            _fontVar = PCLFonts.eVariant.Bold;

            SetFontOptions(cbFont.SelectedIndex, false, true);

            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b F o n t V a r B I _ C l i c k                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Bold Italic' font variant radio button is         //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbFontVarBI_Click(object sender, RoutedEventArgs e)
        {
            _fontVar = PCLFonts.eVariant.BoldItalic;

            SetFontOptions(cbFont.SelectedIndex, false, true);

            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b F o n t V a r I _ C l i c k                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Italic' font variant radio button is selected.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbFontVarI_Click(object sender, RoutedEventArgs e)
        {
            _fontVar = PCLFonts.eVariant.Italic;

            SetFontOptions(cbFont.SelectedIndex, false, true);

            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b F o n t V a r R _ C l i c k                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Regular' font variant radio button is selected.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbFontVarR_Click(object sender, RoutedEventArgs e)
        {
            _fontVar = PCLFonts.eVariant.Regular;

            SetFontOptions(cbFont.SelectedIndex, false, true);

            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b O p t G r i d H _ C l i c k                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Horizontal' grid radio button is selected.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbOptGridH_Click(object sender, RoutedEventArgs e)
        {
            _optGridVertical = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b O p t G r i d V _ C l i c k                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Vertical' grid radio button is selected.          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbOptGridV_Click(object sender, RoutedEventArgs e)
        {
            _optGridVertical = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L S c a l a b l e _ C l i c k                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL 'Scalable' radio button is selected.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLScalable_Click(object sender, RoutedEventArgs e)
        {
            _fontScalable = true;

            SetFontDesc();
            SetFontOptionsPCLSize(_prnDiskFontPCL, _prnDiskFontDataKnownPCL);
            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L S c B i t m a p _ C l i c k                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL 'Bitmap' radio button is selected.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLScBitmap_Click(object sender, RoutedEventArgs e)
        {
            _fontScalable = false;

            SetFontDesc();
            SetFontOptionsPCLSize(_prnDiskFontPCL, _prnDiskFontDataKnownPCL);
            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L S e l e c t B y C h a r _ C l i c k                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL download 'Select by characteristics' radio     //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLSelectByChar_Click(object sender,
                                              RoutedEventArgs e)
        {
            if (_fontType == PCLFonts.eFontType.Download)
                _downloadSelByIdPCL = false;
            else // if (_fontType == PCLFonts.eFontType.PrnDisk)
                _prnDiskSelByIdPCL = false;

            SetFontDesc();
            SetFontOptionsPCLSize(_prnDiskFontPCL, _prnDiskFontDataKnownPCL);
            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L S e l e c t B y I D _ C l i c k                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL download 'Select by ID' radio button is        //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLSelectById_Click(object sender,
                                            RoutedEventArgs e)
        {
            if (_fontType == PCLFonts.eFontType.Download)
                _downloadSelByIdPCL = true;
            else // if (_fontType == PCLFonts.eFontType.PrnDisk)
                _prnDiskSelByIdPCL = true;

            SetFontDesc();
            SetFontOptionsPCLSize(_prnDiskFontPCL, _prnDiskFontDataKnownPCL);
            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L S o f t F o n t S r c H o s t _ C l i c k                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL soft font source = host workstation' radio    //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLSoftFontSrcHost_Click(object sender,
                                                 RoutedEventArgs e)
        {
            rbPCLSelectByChar.IsEnabled = true;

            lbPCLSoftFontMacroId.Visibility = Visibility.Hidden;
            txtPCLSoftFontMacroId.Visibility = Visibility.Hidden;

            SetFontDesc();
            SetFontOptionsPCLSize(_prnDiskFontPCL, _prnDiskFontDataKnownPCL);
            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L S o f t F o n t S r c P r n D i s k F o n t _ C l i c k  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL soft font source = printer hard disk' radio   //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLSoftFontSrcPrnDiskFont_Click(object sender,
                                                        RoutedEventArgs e)
        {
            rbPCLSelectByChar.IsEnabled = false;
            rbPCLSelectById.IsChecked = true;

            _prnDiskLoadViaMacro = false;

            lbPCLSoftFontMacroId.Visibility = Visibility.Hidden;
            txtPCLSoftFontMacroId.Visibility = Visibility.Hidden;

            SetFontDesc();
            SetFontOptionsPCLSize(_prnDiskFontPCL, _prnDiskFontDataKnownPCL);
            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L S o f t F o n t S r c P r n D i s k M a c r o _ C l i c k//
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL soft font source = printer hard disk' radio   //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLSoftFontSrcPrnDiskMacro_Click(object sender,
                                                         RoutedEventArgs e)
        {
            rbPCLSelectByChar.IsEnabled = false;
            rbPCLSelectById.IsChecked = true;

            _prnDiskLoadViaMacro = true;

            lbPCLSoftFontMacroId.Visibility = Visibility.Visible;
            txtPCLSoftFontMacroId.Visibility = Visibility.Visible;

            txtPCLSoftFontMacroId.Text = _fontPrnDiskMacroIdPCL.ToString();

            SetFontDesc();
            SetFontOptionsPCLSize(_prnDiskFontPCL, _prnDiskFontDataKnownPCL);
            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L S p a c e F i x P _ C l i c k                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Fixed-pitch' spacing radio button is selected.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLSpaceFixP_Click(object sender, RoutedEventArgs e)
        {
            _fontProportional = false;

            SetFontDesc();
            SetFontOptionsPCLSize(_prnDiskFontPCL, _prnDiskFontDataKnownPCL);
            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L S p a c e P r o p _ C l i c k                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Proportional' spacing radio button is selected.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLSpaceProp_Click(object sender, RoutedEventArgs e)
        {
            _fontProportional = true;

            SetFontDesc();
            SetFontOptionsPCLSize(_prnDiskFontPCL, _prnDiskFontDataKnownPCL);
            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L S y m s e t B o u n d _ C l i c k                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL 'Symbol set is bound' radio button is          //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLSymSetBound_Click(object sender,
                                             RoutedEventArgs e)
        {
            _fontBound = true;

            SetFontDesc();
            cbSymSet.IsEnabled = false;
            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L S y m s e t U n b o u n d _ C l i c k                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL 'Symbol set is unbound' radio button is        //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLSymSetUnbound_Click(object sender,
                                               RoutedEventArgs e)
        {
            _fontBound = false;

            SetFontDesc();
            cbSymSet.IsEnabled = true;
            SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L X L S o f t F o n t S r c H o s t _ C l i c k            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL XL soft font source = host workstation' radio //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLXLSoftFontSrcHost_Click(object sender,
                                                   RoutedEventArgs e)
        {
            // dummy, since no other options permitted
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S y m S e t U s e r A c t E m b e d _ C l i c k                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the User-defined symbol set option 'Embed in job'      //
        // radio button is selected.                                          //
        // Only relevant for PCL - not PCL XL.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSymSetUserActEmbed_Click(object sender,
                                                 RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                _symSetUserActEmbedPCL = true;

                SetSymSetAttributes();

                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S y m S e t U s e r A c t I n d e x _ C l i c k                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the User-defined symbol set option 'Use as Unicode     //
        // index' radio button is selected.                                   //
        // Always set for PCL XL - embed option not relevant.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSymSetUserActIndex_Click(object sender,
                                                 RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                _symSetUserActEmbedPCL = false;

                SetSymSetAttributes();

                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s e t T a r g e t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset the text on the 'Generate' button.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void ResetTarget()
        {
            TargetCore.eTarget targetType = TargetCore.getType();

            if (targetType == TargetCore.eTarget.File)
            {
                btnGenerate.Content = "Generate & send test data to file";
            }
            else if (targetType == TargetCore.eTarget.NetPrinter)
            {
                string netPrnAddress = string.Empty;
                int netPrnPort = 0;

                int netTimeoutSend = 0;
                int netTimeoutReceive = 0;

                TargetCore.MetricsLoadNetPrinter(ref netPrnAddress,
                                                  ref netPrnPort,
                                                  ref netTimeoutSend,
                                                  ref netTimeoutReceive);

                btnGenerate.Content = "Generate & send test data to " +
                                      "\r\n" +
                                      netPrnAddress + " : " +
                                      netPrnPort.ToString();
            }
            else if (targetType == TargetCore.eTarget.WinPrinter)
            {
                string winPrintername = string.Empty;

                TargetCore.MetricsLoadWinPrinter(ref winPrintername);

                btnGenerate.Content = "Generate & send test data to printer " +
                                      "\r\n" +
                                      winPrintername;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e l e c t P C L F o n t F i l e                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initiate 'open file' dialogue for PCL font file.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool SelectPCLFontFile(ref string fontFilename)
        {
            OpenFileDialog openDialog = ToolCommonFunctions.CreateOpenFileDialog(fontFilename);

            openDialog.Filter = "PCL Font files|*.sfp; *.sfs; *.sft; " +
                                               "*.SFP; *.SFS; *.SFT;" +
                                "|All files|*.*";

            bool? dialogResult = openDialog.ShowDialog();

            if (dialogResult == true)
                fontFilename = openDialog.FileName;

            return dialogResult == true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e l e c t P C L X L F o n t F i l e                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initiate 'open file' dialogue for PCLXL font file.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool SelectPCLXLFontFile(ref string fontFilename)
        {
            OpenFileDialog openDialog = ToolCommonFunctions.CreateOpenFileDialog(fontFilename);

            openDialog.Filter = "PCLXL Font files|*.sfx; *.SFX" +
                                "|All files|*.*";

            bool? dialogResult = openDialog.ShowDialog();

            if (dialogResult == true)
                fontFilename = openDialog.FileName;

            return dialogResult == true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e l e c t S y m S e t F i l e                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initiate 'open file' dialogue for user-defined symbol set file.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool SelectSymSetFile(ref string symSetFile)
        {
            OpenFileDialog openDialog = ToolCommonFunctions.CreateOpenFileDialog(symSetFile);

            openDialog.Filter = "PCL files|*.pcl; *.PCL;" +
                                "|All files|*.*";

            bool? dialogResult = openDialog.ShowDialog();

            if (dialogResult == true)
                symSetFile = openDialog.FileName;

            return dialogResult == true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F o n t D e s c                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate font description string and display in relevant box.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetFontDesc()
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                bool showDetails = true;

                if (_fontType == PCLFonts.eFontType.Download)
                {
                    _fontDesc = "download (id=" + _fontDownloadIdPCL + "); ";
                }
                else if (_fontType == PCLFonts.eFontType.PrnDisk)
                {
                    if (!_prnDiskFontDataKnownPCL)
                        showDetails = false;

                    if (_prnDiskLoadViaMacro)
                        _fontDesc = "prn disk load (id=" +
                                   _fontPrnDiskIdPCL + "); " +
                                    "via macro (id=" +
                                    _fontPrnDiskMacroIdPCL + "); ";
                    else
                        _fontDesc = "prn disk load (id=" +
                                    _fontPrnDiskIdPCL + "); ";
                }
                else
                {
                    _fontDesc = string.Empty;
                }

                if (showDetails)
                {
                    if (_fontProportional)
                        _fontDesc += "proportional";
                    else
                        _fontDesc += "fixed-pitch";

                    if (_fontScalable)
                        _fontDesc += "; scalable";
                    else
                        _fontDesc += "; bitmap";

                    if (_fontBound)
                        _fontDesc += "; bound (id=" + _symSetId + ")";
                }
            }
            else // if (_crntPDL == ToolCommonData.ePrintLang.PCLXL)
            {
                if (_fontType == PCLFonts.eFontType.Download)
                {
                    _fontDesc = "download";

                    if (_fontScalable)
                        _fontDesc += "; scalable";
                    else
                        _fontDesc += "; bitmap";

                    if (_fontBound)
                        _fontDesc += "; bound (id=" + _symSetId + ")";
                }
                else if ((_fontType == PCLFonts.eFontType.PresetTypeface) ||
                         (_fontType == PCLFonts.eFontType.PresetFamilyMember))
                {
                    if (_fontProportional)
                        _fontDesc = "proportional";
                    else
                        _fontDesc = "fixed-pitch";

                    if (_fontScalable)
                        _fontDesc += "; scalable";
                    else
                        _fontDesc += "; bitmap";

                    if (_fontBound)
                        _fontDesc += "; bound (id=" + _symSetId + ")";
                }
                else // if (_fontType == PCLFonts.eFontType.Custom)
                {
                    _fontDesc = "custom";
                }
            }

            txtFontDesc.Text = _fontDesc;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F o n t O p t i o n s                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set options relevant to selected font.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetFontOptions(int indxFont,
                                    bool restoreSymSet,
                                    bool samePreset)
        {
            SetFontOptionsVariants(indxFont, samePreset);

            //----------------------------------------------------------------//

            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                SetFontOptionsPCL(indxFont);
            else
                SetFontOptionsPCLXL(indxFont);

            //----------------------------------------------------------------//

            if ((_fontType == PCLFonts.eFontType.PrnDisk) &&
                (!_prnDiskFontDataKnownPCL))
            {
                cbSymSet.IsEnabled = false;
            }
            else if (_fontBound)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Font is bound to a specific symbol set.                    //
                //                                                            //
                //------------------------------------------------------------//

                cbSymSet.IsEnabled = false;

                int index,
                      indxSymSet;

                string idNum = string.Empty,
                       idAlpha = string.Empty;

                PCLSymbolSets.TranslateKind1ToId(_symSetNo,
                                                  ref idNum,
                                                  ref idAlpha);

                index = PCLSymbolSets.GetIndexForId(_symSetNo);
                indxSymSet = 0;

                for (int i = 0; i < _ctSymSets; i++)
                {
                    if (_subsetSymSets[i] == index)
                    {
                        indxSymSet = i;
                        i = _ctSymSets; // end loop
                    }
                }

                cbSymSet.SelectedIndex = indxSymSet;

                SetSymSetAttributes();
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Font is unbound (not bound to a specific symbol set).      //
                //                                                            //
                //------------------------------------------------------------//

                cbSymSet.IsEnabled = true;

                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                {
                    if (restoreSymSet)
                    {
                        if (_indxSymSetPCL >= _ctSymSets)
                            _indxSymSetPCL = 0;

                        cbSymSet.SelectedIndex = _indxSymSetPCL;

                        CheckSymSetType();
                    }

                    SetSymSetAttributes();

                    SetFontSelectData();
                }
                else
                {
                    if (restoreSymSet)
                    {
                        if (_indxSymSetPCLXL >= _ctSymSets)
                            _indxSymSetPCLXL = 0;

                        cbSymSet.SelectedIndex = _indxSymSetPCLXL;

                        CheckSymSetType();
                    }

                    SetSymSetAttributes();

                    SetFontSelectData();
                }
            }

            SetFontDesc();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F o n t O p t i o n s P C L                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set PCL options relevant to selected font.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetFontOptionsPCL(int indxFont)
        {
            _indxFontPCL = indxFont;

            grpPCLBasic.Visibility = Visibility.Visible;
            grpPCLDesign.Visibility = Visibility.Visible;
            grpSymSet.Visibility = Visibility.Visible;

            grpPCLSoftFont.Visibility = Visibility.Hidden;

            chkPCLPrnDiskDataKnown.Visibility = Visibility.Hidden;

            if (_fontType == PCLFonts.eFontType.Download)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Download details required.                                 //
                //                                                            //
                //------------------------------------------------------------//

                grpPCLSoftFont.Visibility = Visibility.Visible;

                rbPCLSoftFontSrcHost.IsEnabled = true;
                rbPCLSoftFontSrcPrnDiskFont.IsEnabled = false;
                rbPCLSoftFontSrcPrnDiskMacro.IsEnabled = false;

                rbPCLSoftFontSrcHost.IsChecked = true;

                btnPCLSoftFontFileBrowse.IsEnabled = true;

                lbPCLSoftFontMacroId.Visibility = Visibility.Hidden;
                txtPCLSoftFontMacroId.Visibility = Visibility.Hidden;

                txtPCLSoftFontFile.Text = _fontFilenamePCL;
                txtPCLSoftFontId.Text = _fontDownloadIdPCL.ToString();

                if ((_fontFilenamePCL == string.Empty) && (!_initialised))
                {
                    // wait till later to select file
                }
                else
                {
                    CheckPCLSoftFontFile();
                }

                _prnDiskFontPCL = false;

                rbPCLSelectByChar.IsEnabled = true;

                if (_downloadSelByIdPCL)
                    rbPCLSelectById.IsChecked = true;
                else
                    rbPCLSelectByChar.IsChecked = true;

                if (_downloadRemovePCL)
                    chkPCLSoftFontRemove.IsChecked = true;
                else
                    chkPCLSoftFontRemove.IsChecked = false;

                SetFontOptionsPCLBasic(true, false);
                SetFontOptionsPCLDesign(true, false);
            }
            else if (_fontType == PCLFonts.eFontType.PrnDisk)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Font is expected to be found in printer file storage,      //
                // usually on a hard disk.                                    //
                // So we'll possibly have no idea of its characteristics.     //
                //                                                            //
                //------------------------------------------------------------//

                grpPCLSoftFont.Visibility = Visibility.Visible;

                rbPCLSoftFontSrcHost.IsEnabled = false;
                rbPCLSoftFontSrcPrnDiskFont.IsEnabled = true;
                rbPCLSoftFontSrcPrnDiskMacro.IsEnabled = true;

                btnPCLSoftFontFileBrowse.IsEnabled = false;

                if (_prnDiskLoadViaMacro)
                {
                    rbPCLSoftFontSrcPrnDiskMacro.IsChecked = true;

                    lbPCLSoftFontMacroId.Visibility = Visibility.Visible;
                    txtPCLSoftFontMacroId.Visibility = Visibility.Visible;

                    txtPCLSoftFontMacroId.Text = _fontPrnDiskMacroIdPCL.ToString();
                }
                else
                {
                    rbPCLSoftFontSrcPrnDiskFont.IsChecked = true;

                    lbPCLSoftFontMacroId.Visibility = Visibility.Hidden;
                    txtPCLSoftFontMacroId.Visibility = Visibility.Hidden;
                }

                chkPCLPrnDiskDataKnown.Visibility = Visibility.Visible;

                txtPCLSoftFontFile.Text = _fontPrnDiskNamePCL;
                txtPCLSoftFontId.Text = _fontPrnDiskIdPCL.ToString();

                _prnDiskFontPCL = true;

                if (_prnDiskFontDataKnownPCL)
                {
                    grpPCLBasic.Visibility = Visibility.Visible;
                    grpPCLDesign.Visibility = Visibility.Visible;
                    grpSymSet.Visibility = Visibility.Visible;

                    SetFontOptionsPCLBasic(true, true);
                    SetFontOptionsPCLDesign(true, true);

                    rbPCLSelectByChar.IsEnabled = true;

                    if (_prnDiskSelByIdPCL)
                        rbPCLSelectById.IsChecked = true;
                    else
                        rbPCLSelectByChar.IsChecked = true;
                }
                else
                {
                    grpPCLBasic.Visibility = Visibility.Hidden;
                    grpPCLDesign.Visibility = Visibility.Hidden;
                    grpSymSet.Visibility = Visibility.Hidden;

                    _prnDiskSelByIdPCL = true;

                    rbPCLSelectById.IsChecked = true;

                    rbPCLSelectByChar.IsEnabled = false;
                }

                if (_prnDiskRemovePCL)
                    chkPCLSoftFontRemove.IsChecked = true;
                else
                    chkPCLSoftFontRemove.IsChecked = false;
            }
            else if (_fontType == PCLFonts.eFontType.Custom)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Characteristics required.                                  //
                // We've already restored these values ????                   //
                //                                                            //
                //------------------------------------------------------------//

                _prnDiskFontPCL = false;

                SetFontOptionsPCLBasic(true, true);
                SetFontOptionsPCLDesign(true, true);
            }
            else  //  if (_fontType == PCLFonts.eFontType.Preset)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Font chosen from preset list.                              //
                //                                                            //
                // If the font definition specifies a value for the Height    //
                // and Pitch values (only if Bitmap Fixed-Pitch?), use these  //
                // values, otherwise leave the values as restored earlier on  //
                // font switch.                                               //
                //                                                            //
                //------------------------------------------------------------//

                _prnDiskFontPCL = false;

                double fontHeight;
                double fontPitch;

                int fontIndx = _subsetFonts[indxFont];

                _fontBound = PCLFonts.IsBoundFont(fontIndx);
                _fontScalable = PCLFonts.IsScalableFont(fontIndx);
                _fontProportional = PCLFonts.IsProportionalFont(fontIndx);
                _fontStylePCL = PCLFonts.GetPCLStyle(fontIndx, _fontVar);
                _fontWeightPCL = PCLFonts.GetPCLWeight(fontIndx, _fontVar);
                _fontTypefacePCL = PCLFonts.GetPCLTypeface(fontIndx);

                if (_fontBound)
                    _symSetNo = PCLFonts.GetSymbolSetNumber(fontIndx);

                fontHeight = PCLFonts.GetPCLHeight(fontIndx);
                fontPitch = PCLFonts.GetPCLPitch(fontIndx);

                if (fontHeight != 0)
                    _fontHeightPCL = fontHeight;

                if (fontPitch != 0)
                    _fontPitchPCL = fontPitch;

                SetFontOptionsPCLBasic(true, false);
                SetFontOptionsPCLDesign(true, false);
            }

            SetFontOptionsPCLSize(_prnDiskFontPCL, _prnDiskFontDataKnownPCL);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F o n t O p t i o n s P C L B a s i c                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set PCL basic characteristics options relevant to selected font.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetFontOptionsPCLBasic(bool visible,
                                             bool enabled)
        {
            if (!visible)
            {
                lbPCLSpacing.Visibility = Visibility.Hidden;
                lbPCLScaling.Visibility = Visibility.Hidden;
                lbPCLSymset.Visibility = Visibility.Hidden;

                rbPCLScBitmap.Visibility = Visibility.Hidden;
                rbPCLScalable.Visibility = Visibility.Hidden;

                rbPCLSpaceFixP.Visibility = Visibility.Hidden;
                rbPCLSpaceProp.Visibility = Visibility.Hidden;

                rbPCLSymSetBound.Visibility = Visibility.Hidden;
                rbPCLSymSetUnbound.Visibility = Visibility.Hidden;
            }
            else
            {
                lbPCLSpacing.Visibility = Visibility.Visible;
                lbPCLScaling.Visibility = Visibility.Visible;
                lbPCLSymset.Visibility = Visibility.Visible;

                rbPCLScBitmap.Visibility = Visibility.Visible;
                rbPCLScalable.Visibility = Visibility.Visible;

                rbPCLSpaceFixP.Visibility = Visibility.Visible;
                rbPCLSpaceProp.Visibility = Visibility.Visible;

                rbPCLSymSetBound.Visibility = Visibility.Visible;
                rbPCLSymSetUnbound.Visibility = Visibility.Visible;

                if (enabled)
                {
                    rbPCLScBitmap.IsEnabled = true;
                    rbPCLScalable.IsEnabled = true;

                    rbPCLSpaceFixP.IsEnabled = true;
                    rbPCLSpaceProp.IsEnabled = true;

                    rbPCLSymSetBound.IsEnabled = true;
                    rbPCLSymSetUnbound.IsEnabled = true;
                }
                else
                {
                    rbPCLScBitmap.IsEnabled = false;
                    rbPCLScalable.IsEnabled = false;

                    rbPCLSpaceFixP.IsEnabled = false;
                    rbPCLSpaceProp.IsEnabled = false;

                    rbPCLSymSetBound.IsEnabled = false;
                    rbPCLSymSetUnbound.IsEnabled = false;
                }

                if (_fontScalable)
                    rbPCLScalable.IsChecked = true;
                else
                    rbPCLScBitmap.IsChecked = true;

                if (_fontProportional)
                    rbPCLSpaceProp.IsChecked = true;
                else
                    rbPCLSpaceFixP.IsChecked = true;

                if (_fontBound)
                    rbPCLSymSetBound.IsChecked = true;
                else
                    rbPCLSymSetUnbound.IsChecked = true;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F o n t O p t i o n s P C L D e s i g n                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set PCL design characteristics options relevant to selected font.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetFontOptionsPCLDesign(bool visible,
                                              bool enabled)
        {
            if (!visible)
            {
                lbPCLStyle.Visibility = Visibility.Hidden;
                lbPCLWeight.Visibility = Visibility.Hidden;
                lbPCLTypeface.Visibility = Visibility.Hidden;

                txtPCLStyle.Visibility = Visibility.Hidden;
                txtPCLWeight.Visibility = Visibility.Hidden;
                txtPCLTypeface.Visibility = Visibility.Hidden;
            }
            else
            {
                lbPCLStyle.Visibility = Visibility.Visible;
                lbPCLWeight.Visibility = Visibility.Visible;
                lbPCLTypeface.Visibility = Visibility.Visible;

                txtPCLStyle.Visibility = Visibility.Visible;
                txtPCLWeight.Visibility = Visibility.Visible;
                txtPCLTypeface.Visibility = Visibility.Visible;

                if (enabled)
                {
                    txtPCLStyle.IsEnabled = true;
                    txtPCLWeight.IsEnabled = true;
                    txtPCLTypeface.IsEnabled = true;
                }
                else
                {
                    txtPCLStyle.IsEnabled = false;
                    txtPCLWeight.IsEnabled = false;
                    txtPCLTypeface.IsEnabled = false;
                }

                txtPCLStyle.Text = _fontStylePCL.ToString();
                txtPCLWeight.Text = _fontWeightPCL.ToString();
                txtPCLTypeface.Text = _fontTypefacePCL.ToString();
            }
        }
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F o n t O p t i o n s P C L S i z e                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set PCL size options relevant to selected font.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetFontOptionsPCLSize(bool prnDiskFont,
                                           bool prnDiskFontDataKnown)
        {
            if (prnDiskFont && !prnDiskFontDataKnown)
            {
                lbPCLHeight.Visibility = Visibility.Hidden;
                lbPCLPitch.Visibility = Visibility.Hidden;
                txtPCLHeight.Visibility = Visibility.Hidden;
                txtPCLPitch.Visibility = Visibility.Hidden;
            }
            else
            {
                bool setHeight = false;
                bool setPitch = false;

                if (_fontProportional)
                {
                    // proportionally-spaced (bitmap OR scalable)

                    txtPCLHeight.IsEnabled = true;
                    txtPCLPitch.IsEnabled = false;

                    setHeight = true;
                }
                else if (_fontScalable)
                {
                    // fixed-pitch scalable

                    txtPCLHeight.IsEnabled = false;
                    txtPCLPitch.IsEnabled = true;

                    setPitch = true;
                }
                else
                {
                    // fixed-pitch bitmap

                    setHeight = true;
                    setPitch = true;

                    if ((_fontType == PCLFonts.eFontType.Custom) ||
                        (_fontType == PCLFonts.eFontType.PrnDisk))
                    {
                        // user choice

                        txtPCLHeight.IsEnabled = true;
                        txtPCLPitch.IsEnabled = true;
                    }
                    else
                    {
                        // from defined list or download soft font

                        txtPCLHeight.IsEnabled = false;
                        txtPCLPitch.IsEnabled = false;
                    }
                }

                if (_fontHeightPCL == 0)
                    _fontHeightPCL = _defaultFontHeightPCL;

                txtPCLHeight.Text = _fontHeightPCL.ToString("F2");

                if (_fontPitchPCL == 0)
                    _fontPitchPCL = _defaultFontPitchPCL;

                txtPCLPitch.Text = _fontPitchPCL.ToString("F2");

                if (setHeight)
                {
                    lbPCLHeight.Visibility = Visibility.Visible;
                    txtPCLHeight.Visibility = Visibility.Visible;
                }
                else
                {
                    lbPCLHeight.Visibility = Visibility.Hidden;
                    txtPCLHeight.Visibility = Visibility.Hidden;
                }

                if (setPitch)
                {
                    lbPCLPitch.Visibility = Visibility.Visible;
                    txtPCLPitch.Visibility = Visibility.Visible;
                }
                else
                {
                    lbPCLPitch.Visibility = Visibility.Hidden;
                    txtPCLPitch.Visibility = Visibility.Hidden;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F o n t O p t i o n s P C L X L                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set PCLXL options relevant to selected font.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetFontOptionsPCLXL(int indxFont)
        {
            _indxFontPCLXL = indxFont;

            grpPCLXLSoftFont.Visibility = Visibility.Hidden;

            if (_fontType == PCLFonts.eFontType.Download)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Download details required.                                 //
                //                                                            //
                //------------------------------------------------------------//

                grpPCLXLSoftFont.Visibility = Visibility.Visible;

                rbPCLXLSoftFontSrcHost.IsEnabled = true;

                rbPCLXLSoftFontSrcHost.IsChecked = true;

                txtPCLXLFontName.IsEnabled = false;

                if ((_fontFilenamePCLXL == string.Empty) && (!_initialised))
                {
                    // wait till later to select file
                }
                else
                {
                    CheckPCLXLFontFile();
                }

                if (_fontScalable)
                {
                    txtPCLXLHeight.IsEnabled = true;
                    lbPCLXLHeightComment.Visibility = Visibility.Hidden;
                }
                else
                {
                    txtPCLXLHeight.IsEnabled = false;
                    lbPCLXLHeightComment.Visibility = Visibility.Visible;
                }

                if (_downloadRemovePCLXL)
                    chkPCLXLSoftFontRemove.IsChecked = true;
                else
                    chkPCLXLSoftFontRemove.IsChecked = false;
            }
            else if (_fontType == PCLFonts.eFontType.PrnDisk)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Printer mass storage font.                                 //
                // Not supported by PCL XL.                                   //
                //                                                            //
                //------------------------------------------------------------//

                MessageBox.Show("Loading the font from a printer" +
                                " mass storage device is not supported" +
                                " with the currently selected printer" +
                                " language",
                                "Option not supported",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            else if (_fontType == PCLFonts.eFontType.Custom)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Characteristics required.                                  //
                // We've already restored these values.                       //
                //                                                            //
                //------------------------------------------------------------//

                txtPCLXLFontName.IsEnabled = true;
                txtPCLXLHeight.IsEnabled = true;
                lbPCLXLHeightComment.Visibility = Visibility.Visible;
                _fontBound = false;
            }
            else  //  if (_fontType == PCLFonts.eFontType.Preset)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Font chosen from preset list.                              //
                //                                                            //
                // If the font definition specifies a value for the Height    //
                // value (only if Bitmap Fixed-Pitch?), use this value,       //
                // otherwise leave the value as restored earlier on font      //
                // switch.                                                    //
                //                                                            //
                //------------------------------------------------------------//

                double fontHeight;

                int fontIndx = _subsetFonts[indxFont];

                txtPCLXLFontName.IsEnabled = false;

                _fontBound = PCLFonts.IsBoundFont(fontIndx);
                _fontScalable = PCLFonts.IsScalableFont(fontIndx);
                _fontProportional = PCLFonts.IsProportionalFont(fontIndx);
                _fontNamePCLXL = PCLFonts.GetPCLXLName(fontIndx, _fontVar);

                if (_fontBound)
                    _symSetNo = PCLFonts.GetSymbolSetNumber(fontIndx);

                fontHeight = PCLFonts.GetPCLXLHeight(fontIndx);

                if (fontHeight != 0)
                    _fontHeightPCLXL = fontHeight;

                if (_fontScalable)
                {
                    txtPCLXLHeight.IsEnabled = true;
                    lbPCLXLHeightComment.Visibility = Visibility.Hidden;
                }
                else
                {
                    txtPCLXLHeight.IsEnabled = false;
                    lbPCLXLHeightComment.Visibility = Visibility.Visible;
                }
            }

            //----------------------------------------------------------------//

            if (_fontHeightPCLXL == 0)
                _fontHeightPCLXL = _defaultFontHeightPCLXL;

            txtPCLXLSoftFontFile.Text = _fontFilenamePCLXL;
            txtPCLXLFontName.Text = _fontNamePCLXL;
            txtPCLXLHeight.Text = _fontHeightPCLXL.ToString("F2");
            txtPCLXLSymSet.Text = _symSetNo.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F o n t O p t i o n s V a r i a n t s                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set variant options relevant to selected font.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetFontOptionsVariants(int indxFont,
                                            bool samePreset)
        {
            bool varB,
                    varBI,
                    varI,
                    varR,
                    varSet;

            int fontIndx = _subsetFonts[indxFont];

            //----------------------------------------------------------------//

            rbVarB.Visibility = Visibility.Hidden;
            rbVarBI.Visibility = Visibility.Hidden;
            rbVarI.Visibility = Visibility.Hidden;
            rbVarR.Visibility = Visibility.Hidden;

            rbVarB.IsChecked = false;
            rbVarBI.IsChecked = false;
            rbVarI.IsChecked = false;
            rbVarR.IsChecked = false;

            varSet = false;

            //----------------------------------------------------------------//

            if ((_fontType == PCLFonts.eFontType.PresetTypeface) ||
                (_fontType == PCLFonts.eFontType.PresetFamilyMember))
            {
                varR = PCLFonts.VariantExists(fontIndx,
                                              PCLFonts.eVariant.Regular);

                varI = PCLFonts.VariantExists(fontIndx,
                                              PCLFonts.eVariant.Italic);

                varB = PCLFonts.VariantExists(fontIndx,
                                              PCLFonts.eVariant.Bold);

                varBI = PCLFonts.VariantExists(fontIndx,
                                               PCLFonts.eVariant.BoldItalic);

                //------------------------------------------------------------//

                if (varR)
                    rbVarR.Visibility = Visibility.Visible;

                if (varI)
                    rbVarI.Visibility = Visibility.Visible;

                if (varB)
                    rbVarB.Visibility = Visibility.Visible;

                if (varBI)
                    rbVarBI.Visibility = Visibility.Visible;

                //------------------------------------------------------------//

                if (samePreset)
                {
                    if ((varR) && (_fontVar == PCLFonts.eVariant.Regular))
                    {
                        rbVarR.IsChecked = true;
                        varSet = true;
                    }

                    if ((varI) && (_fontVar == PCLFonts.eVariant.Italic))
                    {
                        rbVarI.IsChecked = true;
                        varSet = true;
                    }

                    if ((varB) && (_fontVar == PCLFonts.eVariant.Bold))
                    {
                        rbVarB.IsChecked = true;
                        varSet = true;
                    }

                    if ((varBI) && (_fontVar == PCLFonts.eVariant.BoldItalic))
                    {
                        rbVarBI.IsChecked = true;
                        varSet = true;
                    }
                }

                if (!varSet)
                {
                    if (varR)
                    {
                        rbVarR.IsChecked = true;
                        _fontVar = PCLFonts.eVariant.Regular;
                    }
                    else if (varI)
                    {
                        rbVarI.IsChecked = true;
                        _fontVar = PCLFonts.eVariant.Italic;
                    }
                    else if (varB)
                    {
                        rbVarB.IsChecked = true;
                        _fontVar = PCLFonts.eVariant.Bold;
                    }
                    else if (varBI)
                    {
                        rbVarBI.IsChecked = true;
                        _fontVar = PCLFonts.eVariant.BoldItalic;
                    }
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F o n t S e l e c t D a t a                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set the contents of the 'Font Select' fields.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetFontSelectData()
        {
            int indxFont = cbFont.SelectedIndex;

            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                SetFontSelectDataPCL(indxFont);
            else // if (_indxPDL == ToolCommonData.ePrintLang.PCL)
                SetFontSelectDataPCLXL(indxFont);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F o n t S e l e c t D a t a P C L                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set the contents of the PCL 'Font Select' fields.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetFontSelectDataPCL(int indxFont)
        {
            string symSetId,
                   selSeqAttr = string.Empty;

            bool selById = false;

            _indxFontPCL = indxFont;

            if (_fontType == PCLFonts.eFontType.Download)
                selById = _downloadSelByIdPCL;
            else if (_fontType == PCLFonts.eFontType.PrnDisk)
                selById = _prnDiskSelByIdPCL;

            _fontSelSeqPCL = SetPCLFontSelectSeq(_subsetFonts[indxFont],
                                                   _fontVar,
                                                   _prnDiskFontDataKnownPCL,
                                                   selById,
                                                   _fontProportional,
                                                   _fontScalable,
                                                   _fontHeightPCL,
                                                   _fontPitchPCL,
                                                   _fontStylePCL,
                                                   _fontWeightPCL,
                                                   _fontTypefacePCL);

            //--------------------------------------------------------------------//

            if (_fontSelSeqPCL == string.Empty)
                selSeqAttr = string.Empty;
            else
                selSeqAttr = "<Esc>(" + _fontSelSeqPCL;

            if ((_symSetGroup == PCLSymbolSets.eSymSetGroup.UserSet) &&
                (!_symSetUserActEmbedPCL))
                symSetId = PCLSymbolSets.GetId(
                                    PCLSymbolSets.IndexUnicode);
            else
                symSetId = _symSetId;

            //--------------------------------------------------------------------//

            if (_fontType == PCLFonts.eFontType.Download)
            {
                _fontLoadDescPCL = "<Esc>*c" + _fontDownloadIdPCL + "D" +
                                   "<font download>";

                if (_downloadSelByIdPCL)
                {
                    if (_fontBound)
                    {
                        _fontSelDescPCL = "<Esc>(" + _fontDownloadIdPCL + "X" +
                                          selSeqAttr;
                    }
                    else
                    {
                        _fontSelDescPCL = "<Esc>(" + symSetId +
                                          "<Esc>(" + _fontDownloadIdPCL + "X" +
                                          selSeqAttr;
                    }
                }
                else
                {
                    _fontSelDescPCL = selSeqAttr;
                }
            }
            else if (_fontType == PCLFonts.eFontType.PrnDisk)
            {
                int binLen = _fontPrnDiskNamePCL.Length + 1;

                if (_prnDiskLoadViaMacro)
                {
                    _fontLoadDescPCL = "<Esc>*c" + _fontPrnDiskIdPCL + "D" +
                                       "<Esc>&f" + _fontPrnDiskMacroIdPCL + "Y" +
                                       "<Esc>(&n" + binLen.ToString() + "W" +
                                       "<05>" + _fontPrnDiskNamePCL + // "<filename>"
                                       "<Esc>&f3X";
                }
                else
                {
                    _fontLoadDescPCL = "<Esc>*c" + _fontPrnDiskIdPCL + "D" +
                                       "<Esc>(&n" + binLen.ToString() + "W" +
                                       "<01>" + _fontPrnDiskNamePCL;  // "<filename>"
                }

                if (_prnDiskFontDataKnownPCL)
                {
                    if (!_fontBound)
                        _fontSelDescPCL = "<Esc>(" + symSetId;
                    else
                        _fontSelDescPCL = string.Empty;

                    if (_prnDiskSelByIdPCL)
                    {
                        _fontSelDescPCL += "<Esc>(" + _fontPrnDiskIdPCL + "X";
                    }

                    if (_fontSelSeqPCL != string.Empty)
                    {
                        _fontSelDescPCL += "<Esc>(" + _fontSelSeqPCL;
                    }
                }
                else
                {
                    _fontSelDescPCL = "<Esc>(" + _fontPrnDiskIdPCL + "X";
                }
            }
            else
            {
                _fontLoadDescPCL = string.Empty;

                _fontSelDescPCL = "<Esc>(" + symSetId +
                                  selSeqAttr;
            }

            if (_fontLoadDescPCL != string.Empty)
                txtPCLSelSeq.Text = _fontLoadDescPCL + " .-. " +
                                    _fontSelDescPCL;
            else
                txtPCLSelSeq.Text = _fontSelDescPCL;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F o n t S e l e c t D a t a P C L X L                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set the contents of the PCLXL 'Font Select' fields.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetFontSelectDataPCLXL(int indxFont)
        {
            if ((_symSetGroup == PCLSymbolSets.eSymSetGroup.UserSet) &&
                (!_symSetUserActEmbedPCLXL)) // this should always be true
            {
                _symSetNo = PCLSymbolSets.GetKind1(
                                    PCLSymbolSets.IndexUnicode);
            }

            txtPCLXLFontName.Text = _fontNamePCLXL;

            txtPCLXLSymSet.Text = _symSetNo.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F o n t T i t l e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate font title string.                                        //
        // For download and mass storage load fonts, the title is left blank; //
        // the name of the file will be used instead in the language-specific //
        // generation code.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private string SetFontTitle(int indxFont)
        {
            string fontTitle;

            if (_fontType == PCLFonts.eFontType.Download)
            {
                fontTitle = string.Empty;
            }
            else if (_fontType == PCLFonts.eFontType.PrnDisk)
            {
                fontTitle = string.Empty;
            }
            else if (_fontType == PCLFonts.eFontType.Custom)
            {
                fontTitle = "<custom - selected via characteristics>";
            }
            else // if (_fontType == PCLFonts.eFontType.Preset)
            {
                fontTitle = PCLFonts.GetName(_subsetFonts[indxFont]);

                if (_fontVar == PCLFonts.eVariant.Italic)
                    fontTitle += " Italic";
                else if (_fontVar == PCLFonts.eVariant.Bold)
                    fontTitle += " Bold";
                else if (_fontVar == PCLFonts.eVariant.BoldItalic)
                    fontTitle += " Bold Italic";
            }

            return fontTitle;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t P C L F o n t S e l e c t S e q                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL font selection sequence appropriate to the          //
        // supplied font attributes.                                          //
        // ... except for the root '<esc>(' prefix.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private string SetPCLFontSelectSeq(
            int indxFont,
            PCLFonts.eVariant variant,
            bool prnDiskFontDataKnown,
            bool selectById,
            bool proportional,
            bool scalable,
            double height,
            double pitch,
            ushort style,
            short weight,
            ushort typeface)
        {
            PCLFonts.eFontType fontType;

            string seq = string.Empty;

            bool sizeSelect = false,
                    fullSelect = false;

            fontType = PCLFonts.GetType(indxFont);

            if (fontType == PCLFonts.eFontType.Download)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Download details required.                                 //
                //                                                            //
                //------------------------------------------------------------//

                if (selectById)
                    sizeSelect = true;
                else
                    fullSelect = true;
            }
            else if (fontType == PCLFonts.eFontType.PrnDisk)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Printer mass storage font details required.                //
                //                                                            //
                //------------------------------------------------------------//

                if (prnDiskFontDataKnown)
                {
                    if (selectById)
                        sizeSelect = true;
                    else
                        fullSelect = true;
                }
                else
                {
                    seq = string.Empty;
                }
            }
            else if (fontType == PCLFonts.eFontType.Custom)
            {
                fullSelect = true;
            }
            else  //  if (_fontType == PCLFonts.eFontTypes.Preset)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Font chosen from preset list.                              //
                //                                                            //
                //------------------------------------------------------------//

                seq = PCLFonts.GetPCLFontSelect(indxFont,
                                                 variant,
                                                 height,
                                                 pitch);
            }

            if (fullSelect)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Full characteristics required.                             //
                //                                                            //
                //------------------------------------------------------------//

                if (proportional)
                    seq = "s1p";
                else
                    seq = "s0p";

                if (scalable)
                {
                    if (proportional)
                    {
                        // Scalable; proportionally-spaced
                        seq += height.ToString("F2") + "v";
                    }
                    else
                    {
                        // Scalable; fixed-pitch
                        seq += pitch.ToString("F2") + "h";
                    }
                }
                else
                {
                    if (proportional)
                        // Bitmap; proportionally-spaced
                        seq += height.ToString("F2") + "v";
                    else
                        // Bitmap; fixed-pitch
                        seq += height.ToString("F2") + "v" +
                               pitch.ToString("F2") + "h";
                }

                seq += style.ToString() + "s" +
                       weight.ToString() + "b";

                seq += typeface + "T";
            }
            else if (sizeSelect)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Just size characteristics required.                        //
                //                                                            //
                //------------------------------------------------------------//

                if (scalable)
                {
                    if (proportional)
                    {
                        // Scalable; proportionally-spaced
                        seq = "s" + height.ToString("F2") + "V";
                    }
                    else
                    {
                        // Scalable; fixed-pitch
                        seq = "s" + pitch.ToString("F2") + "H";
                    }
                }
                else
                {
                    seq = string.Empty;
                }
            }

            return seq;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t S a m p l e A t t r i b u t e s                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // if relevant, set Parse Method & Offset data for the Sample group   //
        // box.                                                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetSampleAttributes(int indxSymSetEntry)
        {
            int tmpIndex;

            bool found;

            grpSample.Visibility = Visibility.Hidden;

            if ((_symSetGroup == PCLSymbolSets.eSymSetGroup.UserSet) &&
                     (((_crntPDL == ToolCommonData.ePrintLang.PCL) &&
                       (!_symSetUserActEmbedPCL)) ||
                      ((_crntPDL == ToolCommonData.ePrintLang.PCLXL) &&
                       (!_symSetUserActEmbedPCLXL)))) // this should always be true
            {
                //------------------------------------------------------------//
                //                                                            //
                // The Offset and Text Parsing Method fields (in the Sample   //
                // group box) may be relevant.                                //
                //                                                            //
                //------------------------------------------------------------//

                _useSampleBlocks = true;

                grpSample.Visibility = Visibility.Visible;

                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                {
                    _subsetParseMethods = _subsetParseMethodsPCLDirect;
                    _indxParseMethod = (int)PCLTextParsingMethods.eIndex.m83_UTF8;
                }
                else // if (_crntPDL == ToolCommonData.ePrintLang.PCLXL)
                {
                    _subsetParseMethods = _subsetParseMethodsPCLXLDirect;
                    _indxParseMethod = (int)PCLTextParsingMethods.eIndex.m2_2_byte;
                }

                _ctParseMethods = _subsetParseMethods.Length;

                cbParseMethod.Items.Clear();

                for (int i = 0; i < _ctParseMethods; i++)
                {
                    tmpIndex = _subsetParseMethods[i];

                    cbParseMethod.Items.Add(PCLTextParsingMethods.GetDescLong(tmpIndex));
                }

                //------------------------------------------------------------//

                found = false;

                for (int i = 0;
                     (i < _ctParseMethods) && (found == false);
                     i++)
                {
                    if (_subsetParseMethods[i] == _indxParseMethod)
                    {
                        found = true;

                        cbParseMethod.SelectedIndex = i;
                    }
                }

                if (!found)
                    cbParseMethod.SelectedIndex = 0;

                //------------------------------------------------------------//

                SetSymSetOffsetRanges(_symSetGroup, _symSetType,
                                      _indxParseMethod);
            }
            else if ((_symSetGroup == PCLSymbolSets.eSymSetGroup.Unicode) ||
                     (_symSetType == PCLSymSetTypes.eIndex.Bound_16bit) ||
                     (_symSetType == PCLSymSetTypes.eIndex.Unknown))
            {
                //------------------------------------------------------------//
                //                                                            //
                // The Offset and Text Parsing Method fields (in the Sample   //
                // group box) may be relevant.                                //
                //                                                            //
                //------------------------------------------------------------//

                _useSampleBlocks = true;

                grpSample.Visibility = Visibility.Visible;

                _indxParseMethod =
                    (int)PCLSymbolSets.GetParsingMethod(indxSymSetEntry);

                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                {
                    _subsetParseMethods = _subsetParseMethodsPCLAll;
                }
                else // if (_crntPDL == ToolCommonData.ePrintLang.PCLXL)
                {
                    _subsetParseMethods = _subsetParseMethodsPCLXLAll;
                }

                _ctParseMethods = _subsetParseMethods.Length;

                cbParseMethod.Items.Clear();

                for (int i = 0; i < _ctParseMethods; i++)
                {
                    tmpIndex = _subsetParseMethods[i];

                    cbParseMethod.Items.Add(PCLTextParsingMethods.GetDescLong(tmpIndex));
                }

                //------------------------------------------------------------//

                found = false;

                for (int i = 0;
                     (i < _ctParseMethods) && (found == false);
                     i++)
                {
                    if (_subsetParseMethods[i] == _indxParseMethod)
                    {
                        found = true;

                        cbParseMethod.SelectedIndex = i;
                    }
                }

                if (!found)
                    cbParseMethod.SelectedIndex = 0;

                //------------------------------------------------------------//

                SetSymSetOffsetRanges(_symSetGroup, _symSetType,
                                      _indxParseMethod);
            }
            else
            {
                _useSampleBlocks = false;

                grpSample.Visibility = Visibility.Hidden;
            }

            if ((_symSetGroup == PCLSymbolSets.eSymSetGroup.UserSet) &&
                (_symSetType == PCLSymSetTypes.eIndex.Bound_16bit) &&
                (_crntPDL == ToolCommonData.ePrintLang.PCL) &&
                (_symSetUserActEmbedPCL))
            {
                MessageBox.Show("Few (if any) printers support" +
                                 " user-defined 16-bit symbol sets",
                                 "Symbol Set type",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Warning);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t S h o w C o d e s O p t i o n s                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If relevant, set the 'show codes' options.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetShowCodesOptions()
        {
            chkOptShowMapCodesUCS2.Visibility = Visibility.Hidden;
            chkOptShowMapCodesUTF8.Visibility = Visibility.Hidden;

            _mapCodesRelevant = false;

            if (_symSetGroup == PCLSymbolSets.eSymSetGroup.UserSet)
            {
                chkOptShowMapCodesUCS2.Visibility = Visibility.Visible;
                chkOptShowMapCodesUTF8.Visibility = Visibility.Visible;

                _mapCodesRelevant = true;
            }
            else if (_symSetGroup == PCLSymbolSets.eSymSetGroup.Unicode)
            {
                chkOptShowMapCodesUCS2.Visibility = Visibility.Visible;
                chkOptShowMapCodesUTF8.Visibility = Visibility.Visible;

                _mapCodesRelevant = true;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t S y m S e t A t t r i b u t e s                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set the attributes of the selected symbol set.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetSymSetAttributes()
        {
            bool setIdText = false;

            int indxSymSetEntry,
                  indxSymSet;

            _settingSymSetAttributes = true;

            indxSymSet = cbSymSet.SelectedIndex;

            if (indxSymSet != -1)
            {

                indxSymSetEntry = _subsetSymSets[indxSymSet];

                _symSetGroup = PCLSymbolSets.GetGroup(indxSymSetEntry);
                _symSetType = PCLSymbolSets.GetType(indxSymSetEntry);

                _indxParseMethod =
                    (int)PCLTextParsingMethods.eIndex.not_specified;

                if (_symSetGroup == PCLSymbolSets.eSymSetGroup.UserSet)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // User-defined symbol set, held in a nominated file.     //
                    // At this stage, the file should already have been       //
                    // examined and validated, and details of the symbol set  //
                    // extracted.                                             //
                    // Obtain the number and equivalent ID values, and set    //
                    // these values in the number and ID boxes (which should  //
                    // be disabled to prevent user input).                    //
                    //                                                        //
                    //--------------------------------------------------------//

                    txtSymSetIdNum.IsEnabled = false;
                    txtSymSetIdAlpha.IsEnabled = false;

                    _symSetNo = PCLSymbolSets.GetKind1(indxSymSetEntry);

                    txtSymSetNo.Text = _symSetNo.ToString();

                    txtSymSetUserFirstCode.Text =
                        _symSetUserFirstCode.ToString("x4");
                    txtSymSetUserLastCode.Text =
                        _symSetUserLastCode.ToString("x4");

                    _symSetId = PCLSymbolSets.GetId(indxSymSetEntry);

                    setIdText = true;
                }
                else if ((_symSetGroup == PCLSymbolSets.eSymSetGroup.Preset) ||
                         (_symSetGroup == PCLSymbolSets.eSymSetGroup.Unicode))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Pre-defined symbol set, including the special case of  //
                    // Unicode.                                               //
                    // Obtain the number and equivalent ID values, and set    //
                    // these values in the number and ID boxes (which should  //
                    // be disabled to prevent user input).                    //
                    //                                                        //
                    //--------------------------------------------------------//

                    txtSymSetIdNum.IsEnabled = false;
                    txtSymSetIdAlpha.IsEnabled = false;

                    _symSetNo = PCLSymbolSets.GetKind1(indxSymSetEntry);

                    txtSymSetNo.Text = _symSetNo.ToString();

                    _symSetId = PCLSymbolSets.GetId(indxSymSetEntry);

                    setIdText = true;
                }
                else if (_fontBound)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Not a pre-defined symbol set, and the selected font is //
                    // bound to a specified symbol set.                       //
                    // Calculate the Id values from the stored symbol set     //
                    // number, and set these values in the ID boxes (which    //
                    // should be disabled to prevent user input).             //
                    //                                                        //
                    //--------------------------------------------------------//

                    txtSymSetIdNum.IsEnabled = false;
                    txtSymSetIdAlpha.IsEnabled = false;

                    txtSymSetNo.Text = _symSetNo.ToString();

                    _symSetId = PCLSymbolSets.TranslateKind1ToId(_symSetNo);

                    if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                        _symSetType = _fontSymSetTypePCL;

                    setIdText = true;
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Not a pre-defined symbol set, and the selected font is //
                    // not bound to a symbol set.                             //
                    // Symbol set is defined by user values entered into the  //
                    // ID boxes (which should be enabled to allow user input).//
                    // Calculate the symbol set number from the ID values.    //
                    //                                                        //
                    //--------------------------------------------------------//

                    int indxTemp;

                    txtSymSetIdNum.IsEnabled = true;
                    txtSymSetIdAlpha.IsEnabled = true;

                    if (_initialised)
                    {
                        _symSetId = txtSymSetIdNum.Text + txtSymSetIdAlpha.Text;
                        _symSetNo = PCLSymbolSets.TranslateIdToKind1(_symSetId);
                    }
                    else
                    {
                        string idNum = _defaultSymSetIdNum.ToString(),
                               idAlpha = _defaultSymSetIdAlpha.ToString();

                        PCLSymbolSets.TranslateKind1ToId(_symSetNo, ref idNum, ref idAlpha);

                        txtSymSetIdNum.Text = idNum;
                        txtSymSetIdAlpha.Text = idAlpha;

                        _symSetId = idNum + idAlpha;
                    }

                    txtSymSetNo.Text = _symSetNo.ToString();

                    indxTemp = PCLSymbolSets.GetIndexForId(_symSetNo);

                    if (indxTemp != -1)
                        _symSetType = PCLSymbolSets.GetType(indxTemp);
                }

                if (setIdText)
                {
                    string symSetId = _symSetId;

                    int len = _symSetId.Length;

                    if (len > 1)
                    {
                        txtSymSetIdNum.Text = symSetId.Substring(0, len - 1);
                        txtSymSetIdAlpha.Text = symSetId.Substring(len - 1, 1);
                    }
                    else
                    {
                        txtSymSetIdNum.Text = _defaultSymSetIdNum.ToString();
                        txtSymSetIdAlpha.Text = _defaultSymSetIdAlpha.ToString();
                    }
                }

                txtSymSetType.Text =
                    PCLSymSetTypes.GetDescShort((int)_symSetType);

                SetSampleAttributes(indxSymSetEntry);

                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                    _indxSymSetPCL = indxSymSet;
                else
                    _indxSymSetPCLXL = indxSymSet;
            }

            _settingSymSetAttributes = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t S y m S e t O f f s e t R a n g e s                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Populate the range offset drop-down box for multi-byte sets.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetSymSetOffsetRanges(
            PCLSymbolSets.eSymSetGroup symSetGroup,
            PCLSymSetTypes.eIndex symSetType,
            int indxParseMethod)
        {
            ushort[] rangesStd;
            ushort[] rangesSingle;
            ushort[] rangesDouble;

            int blockMin,
                  blockMax;

            int ctRangesStd,
                  ctRangesDouble;

            CheckBox chkOffset;

            //----------------------------------------------------------------//
            //                                                                //
            // Ascertain relevant code-point ranges.                          //
            //                                                                //
            //----------------------------------------------------------------//

            rangesDouble = null;

            if (symSetGroup == PCLSymbolSets.eSymSetGroup.UserSet)
            {
                rangesStd = new ushort[2];

                rangesStd[0] = _symSetUserFirstCode;
                rangesStd[1] = _symSetUserLastCode;
            }
            else if ((symSetType == PCLSymSetTypes.eIndex.Bound_7bit) ||
                     (symSetType == PCLSymSetTypes.eIndex.Bound_8bit) ||
                     (symSetType == PCLSymSetTypes.eIndex.Bound_PC8))
            {
                rangesStd = new ushort[2];

                rangesStd[0] = 0x00;
                rangesStd[1] = 0xff;
            }
            else
            {
                rangesSingle =
                    PCLTextParsingMethods.GetRangeDataSingle(indxParseMethod);

                if (rangesSingle == null)
                {
                    rangesStd = null;
                }
                else
                {
                    rangesStd = new ushort[2];

                    rangesStd[0] = 0x00;
                    rangesStd[1] = 0xff;
                }

                rangesDouble =
                    PCLTextParsingMethods.GetRangeDataDouble(indxParseMethod);
            }

            //----------------------------------------------------------------//

            if (rangesStd != null)
                ctRangesStd = (rangesStd.Length / 2);
            else
                ctRangesStd = -1;

            if (rangesDouble != null)
                ctRangesDouble = (rangesDouble.Length / 2);
            else
                ctRangesDouble = -1;

            lstSampleOffsets.Items.Clear();

            _ctSampleOffsetBlocks = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Convert code-point ranges to list of offset blocks.            //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i <= ctRangesStd; i += 2)
            {
                blockMin = rangesStd[i] >> 8;
                blockMax = rangesStd[i + 1] >> 8;

                for (int j = blockMin; j <= blockMax; j++)
                {
                    chkOffset = new CheckBox();
                    chkOffset.Margin = new Thickness(1, 1, 1, 1);
                    chkOffset.Content = j.ToString("x2") + "00";
                    chkOffset.IsEnabled = true;

                    lstSampleOffsets.Items.Add(chkOffset);

                    chkOffset.IsChecked = false;
                    chkOffset.Checked += new RoutedEventHandler(chkOffset_Checked);
                    chkOffset.Unchecked += new RoutedEventHandler(chkOffset_Unchecked);

                    _sampleOffsetBlocks[_ctSampleOffsetBlocks++] = (ushort)(j << 8);
                }
            }

            for (int i = 0; i <= ctRangesDouble; i += 2)
            {
                blockMin = rangesDouble[i] >> 8;
                blockMax = rangesDouble[i + 1] >> 8;

                for (int j = blockMin; j <= blockMax; j++)
                {
                    chkOffset = new CheckBox();
                    chkOffset.Margin = new Thickness(1, 1, 1, 1);
                    chkOffset.Content = j.ToString("x2") + "00";
                    chkOffset.IsEnabled = true;

                    lstSampleOffsets.Items.Add(chkOffset);

                    chkOffset.IsChecked = false;
                    chkOffset.Checked += new RoutedEventHandler(chkOffset_Checked);
                    chkOffset.Unchecked += new RoutedEventHandler(chkOffset_Unchecked);

                    _sampleOffsetBlocks[_ctSampleOffsetBlocks++] = (ushort)(j << 8);
                }
            }

            lstSampleOffsets.SelectedIndex = 0;

            chkOffset = (CheckBox)lstSampleOffsets.Items[0];
            chkOffset.IsChecked = true;

            lstSampleOffsets.ScrollIntoView(chkOffset);

            _ctSamplePages = 1;

            txtSamplePageCt.Text = _ctSamplePages.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L H e i g h t _ L o s t F o c u s                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL height (point size) item has lost focus.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLHeight_LostFocus(object sender,
                                             RoutedEventArgs e)
        {
            if (ValidatePCLFontHeight(true))
                SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L H e i g h t _ T e x t C h a n g e d                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL height (point size) item has changed.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLHeight_TextChanged(object sender,
                                               TextChangedEventArgs e)
        {
            if (ValidatePCLFontHeight(false))
                SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L P i t c h _ L o s t F o c u s                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL pitch (characters-per-inch) item has lost focus.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLPitch_LostFocus(object sender,
                                            RoutedEventArgs e)
        {
            if (ValidatePCLFontPitch(true))
                SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L P i t c h _ T e x t C h a n g e d                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL pitch (characters-per-inch)item has changed.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLPitch_TextChanged(object sender,
                                              TextChangedEventArgs e)
        {
            if (ValidatePCLFontPitch(false))
                SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L S o f t F o n t f i l e _ L o s t F o c u s            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL soft font filename item has lost focus.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLSoftFontFile_LostFocus(object sender,
                                                   RoutedEventArgs e)
        {
            bool selected = true;

            string filename = txtPCLSoftFontFile.Text;

            if (_fontType == PCLFonts.eFontType.Download)
            {
                _fontFilenamePCL = filename;

                if (!File.Exists(filename))
                {
                    selected = false;

                    MessageBox.Show("Font file '" + filename +
                                     "' does not exist.\r\n\r\n" +
                                     "Please select an appropriate file",
                                     "PCL font file invalid",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);

                    selected = SelectPCLFontFile(ref filename);
                }

                if (selected)
                {
                    _fontFilenamePCL = filename;
                    txtPCLSoftFontFile.Text = _fontFilenamePCL;

                    SetFontOptions(_indxFontPCL, false, false);
                    SetFontSelectData();
                }
            }
            else if (_fontType == PCLFonts.eFontType.PrnDisk)
            {
                _fontPrnDiskNamePCL = filename;

                SetFontOptions(_indxFontPCL, false, false);
                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L S o f t F o n t I d _ L o s t F o c u s                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL soft font Id item has lost focus.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLSoftFontId_LostFocus(object sender,
                                                 RoutedEventArgs e)
        {
            if (ValidatePCLSoftFontId(true))
            {
                SetFontDesc();
                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L S o f t F o n t I d _ T e x t C h a n g e d            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL soft font Id item has changed.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLSoftFontId_TextChanged(object sender,
                                                   TextChangedEventArgs e)
        {
            if (ValidatePCLSoftFontId(false))
            {
                SetFontDesc();
                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L S o f t F o n t M a c r o I d _ L o s t F o c u s      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL soft font macro Id item has lost focus.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLSoftFontMacroId_LostFocus(object sender,
                                                      RoutedEventArgs e)
        {
            if (ValidatePCLSoftFontMacroId(true))
            {
                SetFontDesc();
                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L S o f t F o n t M a c r o I d _ T e x t C h a n g e d  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL soft font macro Id item has changed.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLSoftFontMacroId_TextChanged(object sender,
                                                        TextChangedEventArgs e)
        {
            if (ValidatePCLSoftFontMacroId(false))
            {
                SetFontDesc();
                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L S t y l e _ L o s t F o c u s                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL style item has lost focus.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLStyle_LostFocus(object sender,
                                            RoutedEventArgs e)
        {
            if (ValidatePCLFontStyle(true))
                SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L S t y l e _ T e x t C h a n g e d                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL style item has changed.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLStyle_TextChanged(object sender,
                                              TextChangedEventArgs e)
        {
            if (ValidatePCLFontStyle(false))
                SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L T y p e f a c e _ L o s t F o c u s                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL typeface item has lost focus.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLTypeface_LostFocus(object sender,
                                               RoutedEventArgs e)
        {
            if (ValidatePCLFontTypeface(true))
                SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L T y p e f a c e _ T e x t C h a n g e d                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL typeface item has changed.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLTypeface_TextChanged(object sender,
                                                 TextChangedEventArgs e)
        {
            if (ValidatePCLFontTypeface(false))
                SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L W e i g h t _ L o s t F o c u s                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL stroke weight item has lost focus.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLWeight_LostFocus(object sender,
                                             RoutedEventArgs e)
        {
            if (ValidatePCLFontWeight(true))
                SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L W e i g h t _ T e x t C h a n g e d                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL stroke weight item has changed.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLWeight_TextChanged(object sender,
                                               TextChangedEventArgs e)
        {
            if (ValidatePCLFontWeight(false))
                SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L X L F o n t F i l e _ L o s t F o c u s                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL (download) font filename item has lost focus.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLXLFontFile_LostFocus(object sender,
                                               RoutedEventArgs e)
        {
            bool selected = true;

            string filename = txtPCLXLSoftFontFile.Text;

            _fontFilenamePCLXL = filename;

            if (!File.Exists(filename))
            {
                selected = false;

                MessageBox.Show("Font file '" + filename +
                                 "' does not exist.\r\n\r\n" +
                                 "Please select an appropriate file",
                                 "PCLXL font file invalid",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);

                selected = SelectPCLXLFontFile(ref filename);
            }

            if (selected)
            {
                _fontFilenamePCLXL = filename;
                txtPCLXLSoftFontFile.Text = _fontFilenamePCLXL;

                SetFontOptions(_indxFontPCLXL, false, false);
                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L X L F o n t N a m e _ L o s t F o c u s                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL font name item has lost focus.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLXLFontName_LostFocus(object sender,
                                               RoutedEventArgs e)
        {
            _fontNamePCLXL = txtPCLXLFontName.Text;

            if (ValidatePCLXLFontName(false))
                SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L X L H e i g h t _ T e x t C h a n g e d                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL height (point size) item has changed.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLXLHeight_TextChanged(object sender,
                                               TextChangedEventArgs e)
        {
            if (ValidatePCLXLFontHeight(false))
                SetFontSelectData();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t S y m S e t F i l e _ L o s t F o c u s                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // User-defined symbol set filename item has lost focus.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtSymSetFile_LostFocus(object sender,
                                              RoutedEventArgs e)
        {
            bool selected = true;

            string filename = txtSymSetFile.Text;

            if (!File.Exists(filename))
            {
                selected = false;

                MessageBox.Show("Symbol Set file '" + filename +
                                 "' does not exist.\r\n\r\n" +
                                 "Please select an appropriate file",
                                 "Symbol Set file invalid",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);

                selected = SelectSymSetFile(ref filename);
            }

            if (selected)
            {
                _symSetUserFile = filename;
                txtSymSetFile.Text = _symSetUserFile;

                SetSymSetAttributes();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t S y m S e t I d A l p h a _ G o t F o c u s                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for symbol set identifier (alphabetic part) has focus.    //
        // Select all text in the box, so that it can be over-written easily, //
        // without inadvertently causing validation failures.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtSymSetIdAlpha_GotFocus(object sender,
                                                RoutedEventArgs e)
        {
            txtSymSetIdAlpha.SelectAll();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t S y m S e t I d A l p h a _ L o s t F o c u s                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Alphabetic part of Symbol Set identifier has lost focus.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtSymSetIdAlpha_LostFocus(object sender,
                                                 RoutedEventArgs e)
        {
            if (ValidateSymSetIdAlpha(true))
            {
                SetSymSetAttributes();

                CheckFontSupportsSymSet();

                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t S y m S e t I d A l p h a _ T e x t C h a n g e d            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Alphabetic part of Symbol Set identifier changed.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtSymSetIdAlpha_TextChanged(object sender,
                                                  TextChangedEventArgs e)
        {
            if (!_settingSymSetAttributes)
            {
                if (ValidateSymSetIdAlpha(false))
                {
                    SetSymSetAttributes();

                    SetFontSelectData();
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t S y m S e t I d N u m _ G o t F o c u s                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for symbol set identifier (numeric part) has focus.       //
        // Select all text in the box, so that it can be over-written easily, //
        // without inadvertently causing validation failures.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtSymSetIdNum_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSymSetIdNum.SelectAll();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t S y m S e t I d N u m _ L o s t F o c u s                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Numeric part of Symbol Set identifier has lost focus.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtSymSetIdNum_LostFocus(object sender,
                                              RoutedEventArgs e)
        {
            if (ValidateSymSetIdNum(true))
            {
                SetSymSetAttributes();

                CheckFontSupportsSymSet();

                SetFontSelectData();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t S y m S e t I d N u m _ T e x t C h a n g e d                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Numeric part of Symbol Set identifier changed.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtSymSetIdNum_TextChanged(object sender,
                                                TextChangedEventArgs e)
        {
            if (!_settingSymSetAttributes)
            {
                if (ValidateSymSetIdNum(false))
                {
                    SetSymSetAttributes();

                    SetFontSelectData();
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L F o n t C h a r a c t e r i s t i c s        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCL font selection characteristics.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLFontCharacteristics()
        {
            bool OK = true;

            OK = ValidatePCLFontHeight(false);

            if (OK)
                OK = ValidatePCLFontPitch(false);

            if (OK)
                OK = ValidatePCLFontStyle(false);

            if (OK)
                OK = ValidatePCLFontWeight(false);

            if (OK)
                OK = ValidatePCLFontTypeface(false);

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L F o n t H e i g h t                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCL font Height value.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLFontHeight(bool lostFocusEvent)
        {
            const double minVal = 0.25;
            const double maxVal = 999.75;
            const double defVal = _defaultFontHeightPCL;

            double value = 0;

            bool OK = true;

            string crntText = txtPCLHeight.Text;

            if (crntText == string.Empty)
            {
                if ((_fontType == PCLFonts.eFontType.Download) &&
                    (_downloadSelByIdPCL) && (!_fontScalable))
                    value = 0;
                else if ((_fontScalable) && (!_fontProportional))
                    value = 0;
                else
                    OK = false;
            }
            else
            {
                OK = double.TryParse(crntText, out value);

                if ((value < minVal) || (value > maxVal))
                    OK = false;
            }

            if (OK)
            {
                _fontHeightPCL = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString("F2");

                    MessageBox.Show("Height value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    newText + "'",
                                    "PCL font selection attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    _fontHeightPCL = defVal;

                    txtPCLHeight.Text = newText;
                }
                else
                {
                    MessageBox.Show("Height value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Valid range is :\n\t" +
                                    minVal + " <= value <= " + maxVal + "\n" +
                                    "or\n" +
                                    "\t<null> to represent <not applicable>",
                                    "PCL font selection attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtPCLHeight.Focus();
                    txtPCLHeight.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L F o n t P i t c h                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCL font Pitch value.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLFontPitch(bool lostFocusEvent)
        {
            const double minVal = 0.10;
            const double maxVal = 576;
            const double defVal = _defaultFontPitchPCL;

            double value = 0;

            bool OK = true;

            string crntText = txtPCLPitch.Text;

            if (crntText == string.Empty)
            {
                if ((_fontType == PCLFonts.eFontType.Download) &&
                     (_downloadSelByIdPCL) && (!_fontScalable))
                    value = 0;
                else if (_fontProportional)
                    value = 0;
                else
                    OK = false;
            }
            else
            {
                OK = double.TryParse(crntText, out value);

                if ((value < minVal) || (value > maxVal))
                    OK = false;
            }

            if (OK)
            {
                _fontPitchPCL = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString("F2");

                    MessageBox.Show("Pitch value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    newText + "'",
                                    "PCL font selection attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    _fontPitchPCL = defVal;

                    txtPCLPitch.Text = newText;
                }
                else
                {
                    MessageBox.Show("Pitch value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Valid range is :\n\t" +
                                    minVal + " <= value <= " + maxVal + "\n" +
                                    "or\n" +
                                    "\t<null> to represent <not applicable>",
                                    "PCL font selection attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtPCLPitch.Focus();
                    txtPCLPitch.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L F o n t S t y l e                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCL font Style value.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLFontStyle(bool lostFocusEvent)
        {
            const ushort minVal = 0;
            const ushort maxVal = 32767;
            const ushort defVal = _defaultFontStylePCL;

            ushort value;

            bool OK = true;

            string crntText = txtPCLStyle.Text;

            OK = ushort.TryParse(crntText, out value);

            if (OK)
                if ((value < minVal) || (value > maxVal))
                    OK = false;

            if (OK)
            {
                _fontStylePCL = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString();

                    MessageBox.Show("Style value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    newText + "'",
                                    "PCL font selection attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    _fontStylePCL = defVal;

                    txtPCLStyle.Text = newText;
                }
                else
                {
                    MessageBox.Show("Style value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Valid range is :\n\t" +
                                    minVal + " <= value <= " + maxVal + "\n" +
                                    "or\n" +
                                    "\t<null> to represent <not applicable>",
                                    "PCL font selection attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtPCLStyle.Focus();
                    txtPCLStyle.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L F o n t T y p e f a c e                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCL font Typeface value.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLFontTypeface(bool lostFocusEvent)
        {
            const ushort minVal = 0;
            const ushort maxVal = 65535;
            const ushort defVal = _defaultFontTypefacePCL;

            ushort value;

            bool OK = true;

            string crntText = txtPCLTypeface.Text;

            OK = ushort.TryParse(crntText, out value);

            if (OK)
                if ((value < minVal) || (value > maxVal))
                    OK = false;

            if (OK)
            {
                _fontTypefacePCL = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString();

                    MessageBox.Show("Typeface value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    newText + "'",
                                    "PCL font selection attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    _fontTypefacePCL = defVal;

                    txtPCLTypeface.Text = newText;
                }
                else
                {
                    MessageBox.Show("Typeface value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Valid range is :\n\t" +
                                    minVal + " <= value <= " + maxVal + "\n" +
                                    "or\n" +
                                    "\t<null> to represent <not applicable>",
                                    "PCL font selection attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtPCLTypeface.Focus();
                    txtPCLTypeface.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L F o n t W e i g h t                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCL font Weight value.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLFontWeight(bool lostFocusEvent)
        {
            const short minVal = -7;
            const short maxVal = 7;
            const short defVal = _defaultFontWeightPCL;

            short value;

            bool OK = true;

            string crntText = txtPCLWeight.Text;

            OK = short.TryParse(crntText, out value);

            if (OK)
                if ((value < minVal) || (value > maxVal))
                    OK = false;

            if (OK)
            {
                _fontWeightPCL = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString();

                    MessageBox.Show("Weight value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    newText + "'",
                                    "PCL font selection attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    _fontWeightPCL = defVal;

                    txtPCLWeight.Text = newText;
                }
                else
                {
                    MessageBox.Show("Weight value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Valid range is :\n\t" +
                                    minVal + " <= value <= " + maxVal + "\n" +
                                    "or\n" +
                                    "\t<null> to represent <not applicable>",
                                    "PCL font selection attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtPCLWeight.Focus();
                    txtPCLWeight.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L S o f t F o n t I d                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCL soft font Id value.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLSoftFontId(bool lostFocusEvent)
        {
            const ushort minVal = 0;
            const ushort maxVal = 32767;
            const ushort defVal = _defaultSoftFontIdPCL;

            ushort value;

            bool OK = true;

            string crntText = txtPCLSoftFontId.Text;

            OK = ushort.TryParse(crntText, out value);

            if (OK)
                if ((value < minVal) || (value > maxVal))
                    OK = false;

            if (OK)
            {
                if (_fontType == PCLFonts.eFontType.Download)
                    _fontDownloadIdPCL = value;
                else // if (_fontType == PCLFonts.eFontType.PrnDisk)
                    _fontPrnDiskIdPCL = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString();

                    MessageBox.Show("Font Id value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    newText + "'",
                                    "PCL soft font (down)load identifier invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    if (_fontType == PCLFonts.eFontType.Download)
                        _fontDownloadIdPCL = defVal;
                    else // if (_fontType == PCLFonts.eFontType.PrnDisk)
                        _fontPrnDiskIdPCL = defVal;

                    txtPCLSoftFontId.Text = newText;
                }
                else
                {
                    MessageBox.Show("Font Id value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Valid range is :\n\t" +
                                    minVal + " <= value <= " + maxVal + "\n" +
                                    "or\n" +
                                    "\t<null> to represent <not applicable>",
                                    "PCL soft font (down)load identifier invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtPCLSoftFontId.Focus();
                    txtPCLSoftFontId.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L S o f t F o n t M a c r o I d                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCL soft font macro Id value.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLSoftFontMacroId(bool lostFocusEvent)
        {
            const ushort minVal = 0;
            const ushort maxVal = 32767;
            const ushort defVal = _defaultSoftFontIdMacroPCL;

            ushort value;

            bool OK = true;

            string crntText = txtPCLSoftFontMacroId.Text;

            OK = ushort.TryParse(crntText, out value);

            if (OK)
                if ((value < minVal) || (value > maxVal))
                    OK = false;

            if (OK)
            {
                _fontPrnDiskMacroIdPCL = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString();

                    MessageBox.Show("Macro Id value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    newText + "'",
                                    "PCL soft font load identifier invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    _fontPrnDiskMacroIdPCL = defVal;

                    txtPCLSoftFontMacroId.Text = newText;
                }
                else
                {
                    MessageBox.Show("Macro Id value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Valid range is :\n\t" +
                                    minVal + " <= value <= " + maxVal + "\n" +
                                    "or\n" +
                                    "\t<null> to represent <not applicable>",
                                    "PCL soft font load identifier invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtPCLSoftFontMacroId.Focus();
                    txtPCLSoftFontMacroId.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L X L F o n t C h a r a c t e r i s t i c s    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCLXL font selection characteristics.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLXLFontCharacteristics()
        {
            bool OK = true;

            OK = ValidatePCLXLFontName(false);

            if (OK)
                OK = ValidatePCLXLFontHeight(false);

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L X L F o n t H e i g h t                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCLXL font Height value.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLXLFontHeight(bool lostFocusEvent)
        {
            const double minVal = 0.25;
            const double maxVal = 999.75;
            const double defVal = _defaultFontHeightPCL;

            double value = 0;

            bool OK = true;

            string crntText = txtPCLXLHeight.Text;

            if (crntText == string.Empty)
            {
                if (_fontScalable)
                    OK = false;
            }
            else
            {
                OK = double.TryParse(crntText, out value);

                if ((value < minVal) || (value > maxVal))
                    OK = false;
            }

            if (OK)
            {
                _fontHeightPCLXL = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString("F2");

                    MessageBox.Show("Height value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    newText + "'",
                                    "PCLXL font selection attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    _fontHeightPCLXL = defVal;

                    txtPCLXLHeight.Text = newText;
                }
                else
                {
                    MessageBox.Show("Height value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Valid range is :\n\t" +
                                    minVal + " <= value <= " + maxVal + "\n" +
                                    "or\n" +
                                    "\t<null> to represent <not applicable>",
                                    "PCLXL font selection attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtPCLXLHeight.Focus();
                    txtPCLXLHeight.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L X L F o n t N a m e                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCLXL font name value.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLXLFontName(bool lostFocusEvent)
        {
            //          const Int32 minLen = 14;
            const int maxLen = 20;
            const string defVal = _defaultFontNamePCLXL;

            int len = 0;

            bool OK = true;

            string crntText = txtPCLXLFontName.Text;

            len = crntText.Length;

            if (crntText == string.Empty)
            {
                OK = false;
            }
            else if (len > maxLen)
            {
                OK = false;
            }

            if (!OK)
            {
                if (lostFocusEvent)
                {
                    MessageBox.Show("Font name value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    defVal + "'",
                                    "PCLXL font selection attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    _fontNamePCLXL = defVal;

                    txtPCLXLFontName.Text = _fontNamePCLXL;
                }
                else
                {
                    MessageBox.Show("Font name value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Valid length is <= " + maxLen,
                                    "PCLXL font selection attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtPCLXLFontName.Text = crntText.Substring(0, maxLen);
                    txtPCLXLFontName.Focus();
                    txtPCLXLFontName.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e S y m S e t I d A l p h a                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate alphabetic part of Symbol Set identifier.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidateSymSetIdAlpha(bool lostFocusEvent)
        {
            const char minVal = 'A';
            const char maxVal = 'Z';
            const char badVal = 'X';
            const char defVal = _defaultSymSetIdAlpha;

            int value = 0,
                  len;

            bool OK = true;

            string crntText = txtSymSetIdAlpha.Text;

            len = crntText.Length;

            if (len != 1)
            {
                OK = false;
            }
            else
            {
                value = char.ConvertToUtf32(crntText, 0);

                if ((value < minVal) || (value > maxVal) || (value == badVal))
                    OK = false;
            }

            if (!OK)
            {
                if (lostFocusEvent)
                {
                    MessageBox.Show("Alphabetic part '" + crntText +
                                    "' of identifier is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    defVal + "'",
                                    "Symbol Set identifier invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    txtSymSetIdAlpha.Text = defVal.ToString();

                    OK = true;
                }
                else
                {
                    MessageBox.Show("Alphabetic part '" + crntText +
                                    "' of identifier is invalid.\n\n" +
                                    "Valid range is :\n\t" +
                                    minVal + " <= value <= " + maxVal +
                                    " excluding " + badVal,
                                    "Symbol Set identifier invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtSymSetIdAlpha.Focus();
                    txtSymSetIdAlpha.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e S y m S e t I d N u m                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate numeric part of Symbol Set identifier.                    //
        //                                                                    //
        // Note that the maximum value is 2047 (which may be present in some  //
        // custom soft font headers), but only 1023 in symbol set definitions.//
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidateSymSetIdNum(bool lostFocusEvent)
        {
            const ushort minVal = 0;
            const ushort maxVal = 2047;
            const ushort defVal = _defaultSymSetIdNum;

            ushort value;

            bool OK = true;

            string crntText = txtSymSetIdNum.Text;

            OK = ushort.TryParse(crntText, out value);

            if (OK)
                if ((value < minVal) || (value > maxVal))
                    OK = false;

            if (!OK)
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString();

                    MessageBox.Show("Numeric part '" + crntText +
                                    "' of identifier is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    newText + "'",
                                    "Symbol Set identifier invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    txtSymSetIdNum.Text = newText;

                    OK = true;
                }
                else
                {
                    MessageBox.Show("Numeric part '" + crntText +
                                    "' of identifier is invalid.\n\n" +
                                    "Valid range is :\n\t" +
                                    minVal + " <= value <= " + maxVal,
                                    "Symbol Set identifier invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtSymSetIdNum.Focus();
                    txtSymSetIdNum.SelectAll();
                }
            }

            return OK;
        }
    }
}