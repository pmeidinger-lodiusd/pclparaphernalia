using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class manages persistent storage of options for the AnalysePRN tool.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class ToolPrnAnalysePersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const string _mainKey = MainForm._regMainKey;
        private const string _subKeyTools = "Tools";
        private const string _subKeyToolsAnalyse = _subKeyTools + @"\PrnAnalyse";
        private const string _subKeyOptCharSet = "OptCharSet";
        private const string _subKeyOptClrMap = "OptClrMap";
        private const string _subKeyOptGeneral = "OptGeneral";
        private const string _subKeyOptHPGL2 = "OptHPGL2";
        private const string _subKeyOptPCL = "OptPCL";
        private const string _subKeyOptPCLXL = "OptPCLXL";
        private const string _subKeyOptPML = "OptPML";
        private const string _subKeyOptStats = "OptStats";
        private const string _subKeyCrnt = "Crnt";
        private const string _subKeyUserThemeRoot = "UserTheme_";
        private const string _nameFilename = "Filename";
        private const string _nameTheme = "Name";
        private const string _nameFlagAutoAnalyse = "FlagAutoAnalyse";
        private const string _nameFlagDiagFileAccess = "FlagDiagFileAccess";
        private const string _nameFlagBinData = "FlagBinData";
        private const string _nameFlagAlphaNumId = "FlagAlphaNumId";
        private const string _nameFlagColourLookup = "FlagColourLookup";
        private const string _nameFlagConfIO = "FlagConfIO";
        private const string _nameFlagConfImageData = "FlagCIData";
        private const string _nameFlagConfRasterData = "FlagCRData";
        private const string _nameFlagDefLogPage = "FlagLogPageData";
        private const string _nameFlagDefSymSet = "FlagDefSymSet";
        private const string _nameFlagDitherMatrix = "FlagDitherMatrix";
        private const string _nameFlagDriverConf = "FlagDriverConf";
        private const string _nameFlagEscEncText = "FlagEscEncText";
        private const string _nameFlagPaletteConf = "FlagPaletteConf";
        private const string _nameFlagUserPattern = "FlagUserPattern";
        private const string _nameFlagViewIlluminant = "FlagViewIlluminant";
        private const string _nameFlagClrMapUseClr = "FlagUseClr";
        private const string _nameFlagMacroDisplay = "FlagMacroDisplay";
        private const string _nameFlagOperPos = "FlagOperPos";
        private const string _nameFlagPCLFontSelect = "FlagPCLFontSelect";
        private const string _nameFlagPCLPassThrough = "FlagPCLPassThrough";
        private const string _nameFlagWithinPCL = "FlagWithinPCL";
        private const string _nameFlagWithinPJL = "FlagWithinPJL";
        private const string _nameFlagFontChar = "FlagFontChar";
        private const string _nameFlagFontHddr = "FlagFontHddr";
        private const string _nameFlagFontDraw = "FlagFontDraw";
        private const string _nameFlagExcUnusedPCLObs = "FlagExcUnusedPCLObs";
        private const string _nameFlagExcUnusedPCLXLRes = "FlagExcUnusedPCLXLRes";
        private const string _nameFlagStyleData = "FlagStyleData";
        private const string _nameFlagUserStream = "FlagUserStream";
        private const string _nameFlagVerbose = "FlagVerbose";
        private const string _nameFontDrawHeight = "FontDrawHeight";
        private const string _nameFontDrawWidth = "FontDrawWidth";
        private const string _nameIndxClrMapRootBack = "IndxBack_";
        private const string _nameIndxClrMapRootFore = "IndxFore_";
        private const string _nameIndxName = "IndxName";
        private const string _nameIndxOffsetType = "IndxOffsetType";
        private const string _nameIndxRptFileFmt = "IndxRptFileFmt";
        private const string _nameIndxStatsLevel = "IndxStatsLevel";
        private const string _nameIndxSubAct = "IndxSubAct";
        private const string _nameIndxSubCode = "IndxSubCode";
        private const string _defaultFilename = "DefaultPrintFile.prn";
        private const string _defaultThemeName = "NoName";
        private const int _defaultSubCode = 191;
        private const int _flagFalse = 0;
        private const int _flagTrue = 1;
        private const int _indexZero = 0;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored print file data.                                   //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadData(out string filename)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.CreateSubKey(_subKeyToolsAnalyse))
                {
                    filename = (string)subKey.GetValue(_nameFilename, defWorkFolder + "\\" + _defaultFilename);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a R p t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored report file data.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataRpt(out int indxRptFileFmt)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsAnalyse))
                {
                    indxRptFileFmt = (int)subKey.GetValue(_nameIndxRptFileFmt, _indexZero);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d O p t C h a r S e t                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored 'Character Set' options.                           //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadOptCharSet(out int indxName,
                                          out int indxSubAct,
                                          out int indxSubCode)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptCharSet;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    indxName = (int)subKey.GetValue(_nameIndxName, _indexZero);
                    indxSubAct = (int)subKey.GetValue(_nameIndxSubAct, _indexZero);
                    indxSubCode = (int)subKey.GetValue(_nameIndxSubCode, _defaultSubCode);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d O p t C l r M a p                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored 'colour map' options.                              //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadOptClrMap(out bool flagClrMapUseClr)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptClrMap;

                int tmpInt;

                //----------------------------------------------------------------//

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    tmpInt = (int)subKey.GetValue(_nameFlagClrMapUseClr, _flagTrue);

                    flagClrMapUseClr = tmpInt != _flagFalse;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d O p t C l r M a p C r n t                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored 'colour map' current colours.                      //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadOptClrMapCrnt(ref int[] indxClrMapBack, ref int[] indxClrMapFore)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsAnalyse +
                                    "\\" + _subKeyOptClrMap +
                                    "\\" + _subKeyCrnt;

                int tmpInt;

                int ctIndx = PrnParseRowTypes.GetCount();

                //----------------------------------------------------------------//

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    for (int i = 0; i < ctIndx; i++)
                    {
                        tmpInt = indxClrMapBack[i]; // current or initial value;

                        indxClrMapBack[i] = (int)subKey.GetValue(_nameIndxClrMapRootBack + i.ToString("D2"), tmpInt);

                        tmpInt = indxClrMapFore[i]; // current or initial value;

                        indxClrMapFore[i] = (int)subKey.GetValue(_nameIndxClrMapRootFore + i.ToString("D2"), tmpInt);
                    }
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d O p t C l r M a p T h e m e                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored 'colour map' user theme values for specified theme //
        // number.                                                            //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadOptClrMapTheme(int number, ref int[] indxClrMapBack, ref int[] indxClrMapFore)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string themeNo = number.ToString("D2");

                string key = _subKeyToolsAnalyse +
                                "\\" + _subKeyOptClrMap +
                                "\\" + _subKeyUserThemeRoot + themeNo;

                int tmpInt;

                int ctIndx = PrnParseRowTypes.GetCount();

                //----------------------------------------------------------------//

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    for (int i = 0; i < ctIndx; i++)
                    {
                        tmpInt = indxClrMapBack[i]; // current or initial value;

                        indxClrMapBack[i] = (int)subKey.GetValue(_nameIndxClrMapRootBack + i.ToString("D2"), tmpInt);

                        tmpInt = indxClrMapFore[i]; // current or initial value;

                        indxClrMapFore[i] = (int)subKey.GetValue(_nameIndxClrMapRootFore + i.ToString("D2"), tmpInt);
                    }
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d O p t C l r M a p T h e m e N a m e                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored 'colour map' user theme name for specified theme   //
        // number.                                                            //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadOptClrMapThemeName(int number,
                                                   out string name)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string themeNo = number.ToString("D2");

                string key = _subKeyToolsAnalyse +
                                "\\" + _subKeyOptClrMap +
                                "\\" + _subKeyUserThemeRoot + themeNo;

                //----------------------------------------------------------------//

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    name = (string)subKey.GetValue(_nameTheme, _defaultThemeName);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d O p t G e n e r a l                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored 'General' options.                                 //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadOptGeneral(out int indxOffsetType,
                                          out bool flagMiscAutoAnalyse,
                                          out bool flagDiagFileAccess)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                int tmpInt;

                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptGeneral;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    indxOffsetType = (int)subKey.GetValue(_nameIndxOffsetType, _indexZero);

                    tmpInt = (int)subKey.GetValue(_nameFlagAutoAnalyse, _flagTrue);

                    flagMiscAutoAnalyse = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagDiagFileAccess, _flagFalse);

                    flagDiagFileAccess = tmpInt != _flagFalse;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d O p t H P G L 2                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored 'HP-GL/2' options.                                 //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadOptHPGL2(out bool flagMiscBinData)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                int tmpInt;

                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptHPGL2;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    tmpInt = (int)subKey.GetValue(_nameFlagBinData, _flagFalse);

                    flagMiscBinData = tmpInt != _flagFalse;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d O p t P C L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored 'PCL' options.                                     //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadOptPCL(out bool flagFontHddr,
                                      out bool flagFontChar,
                                      out bool flagFontDraw,
                                      out int valFontDrawHeight,
                                      out int valFontDrawWidth,
                                      out bool flagMacroDisplay,
                                      out bool flagMiscStyleData,
                                      out bool flagMiscBinData,
                                      out bool flagTransAlphaNumId,
                                      out bool flagTransColourLookup,
                                      out bool flagTransConfIO,
                                      out bool flagTransConfImageData,
                                      out bool flagTransConfRasterData,
                                      out bool flagTransDefLogPage,
                                      out bool flagTransDefSymSet,
                                      out bool flagTransDitherMatrix,
                                      out bool flagTransDriverConf,
                                      out bool flagTransEscEncText,
                                      out bool flagTransPaletteConf,
                                      out bool flagTransUserPattern,
                                      out bool flagTransViewIlluminant)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                int tmpInt;

                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptPCL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    tmpInt = (int)subKey.GetValue(_nameFlagFontHddr, _flagTrue);

                    flagFontHddr = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagFontChar, _flagFalse);

                    flagFontChar = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagFontDraw, _flagFalse);

                    flagFontDraw = tmpInt != _flagFalse;

                    valFontDrawHeight = (int)subKey.GetValue(_nameFontDrawHeight, 75);

                    valFontDrawWidth = (int)subKey.GetValue(_nameFontDrawWidth, 50);

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagMacroDisplay, _flagTrue);

                    flagMacroDisplay = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagStyleData, _flagFalse);

                    flagMiscStyleData = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagBinData, _flagFalse);

                    flagMiscBinData = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagAlphaNumId, _flagFalse);

                    flagTransAlphaNumId = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagColourLookup, _flagFalse);

                    flagTransColourLookup = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagConfIO, _flagFalse);

                    flagTransConfIO = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagConfImageData, _flagFalse);

                    flagTransConfImageData = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagConfRasterData, _flagFalse);

                    flagTransConfRasterData = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagDefLogPage, _flagFalse);

                    flagTransDefLogPage = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagDefSymSet, _flagFalse);

                    flagTransDefSymSet = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagDitherMatrix, _flagFalse);

                    flagTransDitherMatrix = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagDriverConf, _flagFalse);

                    flagTransDriverConf = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagEscEncText, _flagFalse);

                    flagTransEscEncText = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagPaletteConf, _flagFalse);

                    flagTransPaletteConf = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagUserPattern, _flagFalse);

                    flagTransUserPattern = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagViewIlluminant, _flagFalse);

                    flagTransViewIlluminant = tmpInt != _flagFalse;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d O p t P C L X L                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored 'PCL XL' options.                                  //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadOptPCLXL(out bool flagFontHddr,
                                        out bool flagFontChar,
                                        out bool flagFontDraw,
                                        out int valFontDrawHeight,
                                        out int valFontDrawWidth,
                                        out bool flagEncUserStream,
                                        out bool flagEncPCLPassThrough,
                                        out bool flagEncPCLFontSelect,
                                        out bool flagMiscOperPos,
                                        out bool flagMiscBinData,
                                        out bool flagMiscVerbose)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                int tmpInt;

                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptPCLXL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    tmpInt = (int)subKey.GetValue(_nameFlagFontHddr, _flagTrue);

                    flagFontHddr = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagFontChar, _flagFalse);

                    flagFontChar = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagFontDraw, _flagFalse);

                    flagFontDraw = tmpInt != _flagFalse;

                    valFontDrawHeight = (int)subKey.GetValue(_nameFontDrawHeight, 75);

                    valFontDrawWidth = (int)subKey.GetValue(_nameFontDrawWidth, 50);

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagUserStream, _flagTrue);

                    flagEncUserStream = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagPCLPassThrough, _flagFalse);

                    flagEncPCLPassThrough = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagPCLFontSelect, _flagFalse);

                    flagEncPCLFontSelect = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagPCLFontSelect, _flagFalse);

                    flagEncPCLFontSelect = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagOperPos, _flagTrue);

                    flagMiscOperPos = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagBinData, _flagFalse);

                    flagMiscBinData = tmpInt != _flagFalse;

                    //------------------------------------------------------------//

                    tmpInt = (int)subKey.GetValue(_nameFlagVerbose, _flagFalse);

                    flagMiscVerbose = tmpInt != _flagFalse;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d O p t P M L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored 'PML' options.                                     //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadOptPML(out bool flagWithinPCL,
                                      out bool flagWithinPJL,
                                      out bool flagVerbose)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                int tmpInt;

                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptPML;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    tmpInt = (int)subKey.GetValue(_nameFlagWithinPCL, _flagFalse);

                    flagWithinPCL = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagWithinPJL, _flagFalse);

                    flagWithinPJL = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagVerbose, _flagFalse);

                    flagVerbose = tmpInt != _flagFalse;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d O p t S t a t s                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored 'Statistics' options.                              //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadOptStats(out int indxLevel,
                                        out bool flagExcUnusedPCLObs,
                                        out bool flagExcUnusedPCLXLRes)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                int tmpInt;

                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptStats;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    indxLevel = (int)subKey.GetValue(_nameIndxStatsLevel, _indexZero);

                    tmpInt = (int)subKey.GetValue(_nameFlagExcUnusedPCLObs, _flagFalse);

                    flagExcUnusedPCLObs = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagExcUnusedPCLXLRes, _flagFalse);

                    flagExcUnusedPCLXLRes = tmpInt != _flagFalse;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current print file data.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveData(string filename)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsAnalyse))
                {
                    if (filename != null)
                        subKey.SetValue(_nameFilename, filename, RegistryValueKind.String);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a R p t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current report file data.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataRpt(int indxRptFileFmt)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsAnalyse))
                {
                    subKey.SetValue(_nameIndxRptFileFmt, indxRptFileFmt, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e O p t C h a r S e t                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current 'Character Set' option data.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveOptCharSet(int indxName,
                                          int indxSubAct,
                                          int indxSubCode)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptCharSet;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameIndxName, indxName, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxSubAct, indxSubAct, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxSubCode, indxSubCode, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e O p t C l r M a p                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current 'colour map' option data.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveOptClrMap(bool flagClrMapUseClr)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptClrMap;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameFlagClrMapUseClr, flagClrMapUseClr ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e O p t C l r M a p C r n t                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current 'colour map' current colour data.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveOptClrMapCrnt(int[] indxClrMapBack, int[] indxClrMapFore)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsAnalyse +
                                    "\\" + _subKeyOptClrMap +
                                    "\\" + _subKeyCrnt;

                int ctIndx;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    ctIndx = indxClrMapBack.Length;

                    for (int i = 0; i < ctIndx; i++)
                    {
                        subKey.SetValue(_nameIndxClrMapRootBack + i.ToString("D2"), indxClrMapBack[i], RegistryValueKind.DWord);
                    }

                    ctIndx = indxClrMapFore.Length;

                    for (int i = 0; i < ctIndx; i++)
                    {
                        subKey.SetValue(_nameIndxClrMapRootFore + i.ToString("D2"), indxClrMapFore[i], RegistryValueKind.DWord);
                    }
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e O p t C l r M a p T h e m e                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current 'colour map' user theme values for specified theme   //
        // number.                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveOptClrMapTheme(int number, int[] indxClrMapBack, int[] indxClrMapFore, string name)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string themeNo = number.ToString("D2");

                string key = _subKeyToolsAnalyse +
                                "\\" + _subKeyOptClrMap +
                                "\\" + _subKeyUserThemeRoot +
                                themeNo;

                int ctIndx;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    ctIndx = indxClrMapBack.Length;

                    for (int i = 0; i < ctIndx; i++)
                    {
                        subKey.SetValue(_nameIndxClrMapRootBack + i.ToString("D2"), indxClrMapBack[i], RegistryValueKind.DWord);
                    }

                    ctIndx = indxClrMapFore.Length;

                    for (int i = 0; i < ctIndx; i++)
                    {
                        subKey.SetValue(_nameIndxClrMapRootFore + i.ToString("D2"), indxClrMapFore[i], RegistryValueKind.DWord);
                    }

                    if (name != null)
                        subKey.SetValue(_nameTheme, name, RegistryValueKind.String);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e O p t G e n e r a l                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current 'General' option data.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveOptGeneral(int indxOffsetType, bool flagMiscAutoAnalyse, bool flagDiagFileAccess)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptGeneral;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameIndxOffsetType, indxOffsetType, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagAutoAnalyse, flagMiscAutoAnalyse ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagDiagFileAccess, flagDiagFileAccess ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e O p t H P G L 2                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current 'HP-GL/2' option data.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveOptHPGL2(bool flagMiscBinData)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptHPGL2;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameFlagBinData, flagMiscBinData ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e O p t P C L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current 'PCL' option data.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveOptPCL(bool flagFontHddr,
                                      bool flagFontChar,
                                      bool flagFontDraw,
                                      int valFontDrawHeight,
                                      int valFontDrawWidth,
                                      bool flagMacroDisplay,
                                      bool flagMiscStyleData,
                                      bool flagMiscBinData,
                                      bool flagTransAlphaNumId,
                                      bool flagTransColourLookup,
                                      bool flagTransConfIO,
                                      bool flagTransConfImageData,
                                      bool flagTransConfRasterData,
                                      bool flagTransDefLogPage,
                                      bool flagTransDefSymSet,
                                      bool flagTransDitherMatrix,
                                      bool flagTransDriverConf,
                                      bool flagTransEscEncText,
                                      bool flagTransPaletteConf,
                                      bool flagTransUserPattern,
                                      bool flagTransViewIlluminant)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptPCL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameFlagFontHddr, flagFontHddr ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagFontChar, flagFontChar ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagFontDraw, flagFontDraw ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFontDrawHeight, valFontDrawHeight, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFontDrawWidth, valFontDrawWidth, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagMacroDisplay, flagMacroDisplay ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagStyleData, flagMiscStyleData ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagBinData, flagMiscBinData ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagAlphaNumId, flagTransAlphaNumId ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagColourLookup, flagTransColourLookup ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagConfIO, flagTransConfIO ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagConfImageData, flagTransConfImageData ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagConfRasterData, flagTransConfRasterData ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagDefLogPage, flagTransDefLogPage ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagDefSymSet, flagTransDefSymSet ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagDitherMatrix, flagTransDitherMatrix ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagDriverConf, flagTransDriverConf ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagEscEncText, flagTransEscEncText ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagPaletteConf, flagTransPaletteConf ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagUserPattern, flagTransUserPattern ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagViewIlluminant, flagTransViewIlluminant ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e O p t P C L X L                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current 'PCL XL' option data.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveOptPCLXL(bool flagFontHddr,
                                        bool flagFontChar,
                                        bool flagFontDraw,
                                        int valFontDrawHeight,
                                        int valFontDrawWidth,
                                        bool flagEncUserStream,
                                        bool flagEncPCLPassThrough,
                                        bool flagEncPCLFontSelect,
                                        bool flagMiscOperPos,
                                        bool flagMiscBinData,
                                        bool flagMiscVerbose)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptPCLXL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameFlagFontHddr, flagFontHddr ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagFontChar, flagFontChar ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagFontDraw, flagFontDraw ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFontDrawHeight, valFontDrawHeight, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFontDrawWidth, valFontDrawWidth, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagUserStream, flagEncUserStream ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagPCLPassThrough, flagEncPCLPassThrough ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagPCLFontSelect, flagEncPCLFontSelect ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagOperPos, flagMiscOperPos ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagBinData, flagMiscBinData ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagVerbose, flagMiscVerbose ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e O p t P M L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current 'PML' option data.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveOptPML(bool flagWithinPCL, bool flagWithinPJL, bool flagVerbose)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptPML;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameFlagWithinPCL, flagWithinPCL ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagWithinPJL, flagWithinPJL ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagVerbose, flagVerbose ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e O p t S t a t s                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current 'Statistics' option data.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveOptStats(int indxLevel, bool flagExcUnusedPCLObs, bool flagExcUnusedPCLXLRes)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsAnalyse + "\\" + _subKeyOptStats;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameIndxStatsLevel, indxLevel, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagExcUnusedPCLObs, flagExcUnusedPCLObs ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagExcUnusedPCLXLRes, flagExcUnusedPCLXLRes ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
            }
        }
    }
}