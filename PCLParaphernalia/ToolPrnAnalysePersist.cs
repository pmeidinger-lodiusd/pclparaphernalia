using Microsoft.Win32;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class manages persistent storage of options for the AnalysePRN tool.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

static class ToolPrnAnalysePersist
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    const string _mainKey = MainForm._regMainKey;

    const string _subKeyTools = "Tools";
    const string _subKeyToolsAnalyse = "PrnAnalyse";
    const string _subKeyOptCharSet = "OptCharSet";
    const string _subKeyOptClrMap = "OptClrMap";
    const string _subKeyOptGeneral = "OptGeneral";
    const string _subKeyOptHPGL2 = "OptHPGL2";
    const string _subKeyOptPCL = "OptPCL";
    const string _subKeyOptPCLXL = "OptPCLXL";
    const string _subKeyOptPML = "OptPML";
    const string _subKeyOptStats = "OptStats";

    const string _subKeyCrnt = "Crnt";
    const string _subKeyUserThemeRoot = "UserTheme_";

    const string _nameFilename = "Filename";
    const string _nameTheme = "Name";
    const string _nameFlagAutoAnalyse = "FlagAutoAnalyse";
    const string _nameFlagDiagFileAccess = "FlagDiagFileAccess";
    const string _nameFlagBinData = "FlagBinData";

    const string _nameFlagAlphaNumId = "FlagAlphaNumId";
    const string _nameFlagColourLookup = "FlagColourLookup";
    const string _nameFlagConfIO = "FlagConfIO";
    const string _nameFlagConfImageData = "FlagCIData";
    const string _nameFlagConfRasterData = "FlagCRData";
    const string _nameFlagDefLogPage = "FlagLogPageData";
    const string _nameFlagDefSymSet = "FlagDefSymSet";
    const string _nameFlagDitherMatrix = "FlagDitherMatrix";
    const string _nameFlagDriverConf = "FlagDriverConf";
    const string _nameFlagEscEncText = "FlagEscEncText";
    const string _nameFlagPaletteConf = "FlagPaletteConf";
    const string _nameFlagUserPattern = "FlagUserPattern";
    const string _nameFlagViewIlluminant = "FlagViewIlluminant";

    const string _nameFlagClrMapUseClr = "FlagUseClr";
    const string _nameFlagMacroDisplay = "FlagMacroDisplay";
    const string _nameFlagOperPos = "FlagOperPos";
    const string _nameFlagPCLFontSelect = "FlagPCLFontSelect";
    const string _nameFlagPCLPassThrough = "FlagPCLPassThrough";
    const string _nameFlagWithinPCL = "FlagWithinPCL";
    const string _nameFlagWithinPJL = "FlagWithinPJL";
    const string _nameFlagFontChar = "FlagFontChar";
    const string _nameFlagFontHddr = "FlagFontHddr";
    const string _nameFlagFontDraw = "FlagFontDraw";
    const string _nameFlagExcUnusedPCLObs = "FlagExcUnusedPCLObs";
    const string _nameFlagExcUnusedPCLXLRes = "FlagExcUnusedPCLXLRes";
    const string _nameFlagStyleData = "FlagStyleData";
    const string _nameFlagUserStream = "FlagUserStream";
    const string _nameFlagVerbose = "FlagVerbose";

    const string _nameFontDrawHeight = "FontDrawHeight";
    const string _nameFontDrawWidth = "FontDrawWidth";
    const string _nameIndxClrMapRootBack = "IndxBack_";
    const string _nameIndxClrMapRootFore = "IndxFore_";
    const string _nameIndxName = "IndxName";
    const string _nameIndxOffsetType = "IndxOffsetType";
    const string _nameIndxRptFileFmt = "IndxRptFileFmt";
    const string _nameIndxStatsLevel = "IndxStatsLevel";
    const string _nameIndxSubAct = "IndxSubAct";
    const string _nameIndxSubCode = "IndxSubCode";

    const string _defaultFilename = "DefaultPrintFile.prn";
    const string _defaultThemeName = "NoName";

    const int _defaultSubCode = 191;

    const int _flagFalse = 0;
    const int _flagTrue = 1;
    const int _indexZero = 0;

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a                                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored print file data.                                   //
    // Missing items are given default values.                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadData(ref string filename)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            filename = (string)subKey.GetValue(_nameFilename,
                                                    defWorkFolder + "\\" +
                                                    _defaultFilename);
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

    public static void LoadDataRpt(ref int indxRptFileFmt)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            indxRptFileFmt = (int)subKey.GetValue(_nameIndxRptFileFmt,
                                                     _indexZero);
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

    public static void LoadOptCharSet(ref int indxName,
                                      ref int indxSubAct,
                                      ref int indxSubCode)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                                    "\\" + _subKeyOptCharSet;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            indxName = (int)subKey.GetValue(_nameIndxName,
                                                       _indexZero);
            indxSubAct = (int)subKey.GetValue(_nameIndxSubAct,
                                                       _indexZero);
            indxSubCode = (int)subKey.GetValue(_nameIndxSubCode,
                                                       _defaultSubCode);
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

    public static void LoadOptClrMap(ref bool flagClrMapUseClr)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                                    "\\" + _subKeyOptClrMap;

        int tmpInt;

        //----------------------------------------------------------------//

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            tmpInt = (int)subKey.GetValue(_nameFlagClrMapUseClr,
                                             _flagTrue);

            flagClrMapUseClr = tmpInt != _flagFalse;
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

    public static void LoadOptClrMapCrnt(ref int[] indxClrMapBack,
                                          ref int[] indxClrMapFore)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                                    "\\" + _subKeyOptClrMap +
                                    "\\" + _subKeyCrnt;

        int tmpInt;

        int ctIndx = PrnParseRowTypes.GetCount();

        //----------------------------------------------------------------//

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            for (int i = 0; i < ctIndx; i++)
            {
                tmpInt = indxClrMapBack[i]; // current or initial value;

                indxClrMapBack[i] =
                    (int)subKey.GetValue(
                                _nameIndxClrMapRootBack + i.ToString("D2"),
                                tmpInt);

                tmpInt = indxClrMapFore[i]; // current or initial value;

                indxClrMapFore[i] =
                    (int)subKey.GetValue(
                                _nameIndxClrMapRootFore + i.ToString("D2"),
                                tmpInt);
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

    public static void LoadOptClrMapTheme(int number,
                                           ref int[] indxClrMapBack,
                                           ref int[] indxClrMapFore)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string themeNo = number.ToString("D2");

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                                    "\\" + _subKeyOptClrMap +
                                    "\\" + _subKeyUserThemeRoot + themeNo;

        int tmpInt;

        int ctIndx = PrnParseRowTypes.GetCount();

        //----------------------------------------------------------------//

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            for (int i = 0; i < ctIndx; i++)
            {
                tmpInt = indxClrMapBack[i]; // current or initial value;

                indxClrMapBack[i] =
                    (int)subKey.GetValue(
                                _nameIndxClrMapRootBack + i.ToString("D2"),
                                tmpInt);

                tmpInt = indxClrMapFore[i]; // current or initial value;

                indxClrMapFore[i] =
                    (int)subKey.GetValue(
                                _nameIndxClrMapRootFore + i.ToString("D2"),
                                tmpInt);
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
                                               ref string name)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string themeNo = number.ToString("D2");

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                                    "\\" + _subKeyOptClrMap +
                                    "\\" + _subKeyUserThemeRoot + themeNo;

        //----------------------------------------------------------------//

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            name = (string)subKey.GetValue(_nameTheme,
                                            _defaultThemeName);
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

    public static void LoadOptGeneral(ref int indxOffsetType,
                                      ref bool flagMiscAutoAnalyse,
                                      ref bool flagDiagFileAccess)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        int tmpInt;

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                                    "\\" + _subKeyOptGeneral;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            indxOffsetType = (int)subKey.GetValue(_nameIndxOffsetType,
                                                       _indexZero);

            tmpInt = (int)subKey.GetValue(_nameFlagAutoAnalyse,
                                                       _flagTrue);

            flagMiscAutoAnalyse = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagDiagFileAccess,
                                                       _flagFalse);

            flagDiagFileAccess = tmpInt != _flagFalse;
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

    public static void LoadOptHPGL2(ref bool flagMiscBinData)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        int tmpInt;

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                                    "\\" + _subKeyOptHPGL2;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            tmpInt = (int)subKey.GetValue(_nameFlagBinData,
                                              _flagFalse);

            flagMiscBinData = tmpInt != _flagFalse;
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

    public static void LoadOptPCL(ref bool flagFontHddr,
                                  ref bool flagFontChar,
                                  ref bool flagFontDraw,
                                  ref int valFontDrawHeight,
                                  ref int valFontDrawWidth,
                                  ref bool flagMacroDisplay,
                                  ref bool flagMiscStyleData,
                                  ref bool flagMiscBinData,
                                  ref bool flagTransAlphaNumId,
                                  ref bool flagTransColourLookup,
                                  ref bool flagTransConfIO,
                                  ref bool flagTransConfImageData,
                                  ref bool flagTransConfRasterData,
                                  ref bool flagTransDefLogPage,
                                  ref bool flagTransDefSymSet,
                                  ref bool flagTransDitherMatrix,
                                  ref bool flagTransDriverConf,
                                  ref bool flagTransEscEncText,
                                  ref bool flagTransPaletteConf,
                                  ref bool flagTransUserPattern,
                                  ref bool flagTransViewIlluminant)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        int tmpInt;

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                                    "\\" + _subKeyOptPCL;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            tmpInt = (int)subKey.GetValue(_nameFlagFontHddr,
                                              _flagTrue);

            flagFontHddr = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagFontChar,
                                              _flagFalse);

            flagFontChar = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagFontDraw,
                                              _flagFalse);

            flagFontDraw = tmpInt != _flagFalse;

            valFontDrawHeight =
                (int)subKey.GetValue(_nameFontDrawHeight,
                                         75);

            valFontDrawWidth =
                (int)subKey.GetValue(_nameFontDrawWidth,
                                         50);

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagMacroDisplay,
                                              _flagTrue);

            flagMacroDisplay = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagStyleData,
                                              _flagFalse);

            flagMiscStyleData = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagBinData,
                                              _flagFalse);

            flagMiscBinData = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagAlphaNumId,
                                              _flagFalse);

            flagTransAlphaNumId = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagColourLookup,
                                              _flagFalse);

            flagTransColourLookup = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagConfIO,
                                              _flagFalse);

            flagTransConfIO = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagConfImageData,
                                              _flagFalse);

            flagTransConfImageData = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagConfRasterData,
                                              _flagFalse);

            flagTransConfRasterData = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagDefLogPage,
                                              _flagFalse);

            flagTransDefLogPage = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagDefSymSet,
                                              _flagFalse);

            flagTransDefSymSet = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagDitherMatrix,
                                              _flagFalse);

            flagTransDitherMatrix = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagDriverConf,
                                              _flagFalse);

            flagTransDriverConf = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagEscEncText,
                                              _flagFalse);

            flagTransEscEncText = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagPaletteConf,
                                              _flagFalse);

            flagTransPaletteConf = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagUserPattern,
                                              _flagFalse);

            flagTransUserPattern = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagViewIlluminant,
                                              _flagFalse);

            flagTransViewIlluminant = tmpInt != _flagFalse;
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

    public static void LoadOptPCLXL(ref bool flagFontHddr,
                                    ref bool flagFontChar,
                                    ref bool flagFontDraw,
                                    ref int valFontDrawHeight,
                                    ref int valFontDrawWidth,
                                    ref bool flagEncUserStream,
                                    ref bool flagEncPCLPassThrough,
                                    ref bool flagEncPCLFontSelect,
                                    ref bool flagMiscOperPos,
                                    ref bool flagMiscBinData,
                                    ref bool flagMiscVerbose)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        int tmpInt;

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                                    "\\" + _subKeyOptPCLXL;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            tmpInt = (int)subKey.GetValue(_nameFlagFontHddr,
                                              _flagTrue);

            flagFontHddr = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagFontChar,
                                              _flagFalse);

            flagFontChar = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagFontDraw,
                                              _flagFalse);

            flagFontDraw = tmpInt != _flagFalse;

            valFontDrawHeight =
                (int)subKey.GetValue(_nameFontDrawHeight,
                                         75);

            valFontDrawWidth =
                (int)subKey.GetValue(_nameFontDrawWidth,
                                         50);

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagUserStream,
                                              _flagTrue);

            flagEncUserStream = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagPCLPassThrough,
                                              _flagFalse);

            flagEncPCLPassThrough = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagPCLFontSelect,
                                              _flagFalse);

            flagEncPCLFontSelect = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagPCLFontSelect,
                                              _flagFalse);

            flagEncPCLFontSelect = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagOperPos,
                                              _flagTrue);

            flagMiscOperPos = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagBinData,
                                              _flagFalse);

            flagMiscBinData = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagVerbose,
                                              _flagFalse);

            flagMiscVerbose = tmpInt != _flagFalse;
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

    public static void LoadOptPML(ref bool flagWithinPCL,
                                  ref bool flagWithinPJL,
                                  ref bool flagVerbose)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        int tmpInt;

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                                    "\\" + _subKeyOptPML;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            tmpInt = (int)subKey.GetValue(_nameFlagWithinPCL,
                                              _flagFalse);

            flagWithinPCL = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagWithinPJL,
                                              _flagFalse);

            flagWithinPJL = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagVerbose,
                                              _flagFalse);

            flagVerbose = tmpInt != _flagFalse;

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

    public static void LoadOptStats(
        ref int indxLevel,
        ref bool flagExcUnusedPCLObs,
        ref bool flagExcUnusedPCLXLRes)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        int tmpInt;

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                                    "\\" + _subKeyOptStats;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            indxLevel = (int)subKey.GetValue(_nameIndxStatsLevel,
                                                       _indexZero);

            tmpInt = (int)subKey.GetValue(_nameFlagExcUnusedPCLObs,
                                              _flagFalse);

            flagExcUnusedPCLObs = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagExcUnusedPCLXLRes,
                                              _flagFalse);

            flagExcUnusedPCLXLRes = tmpInt != _flagFalse;
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
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (filename != null)
            {
                subKey.SetValue(_nameFilename,
                                filename,
                                RegistryValueKind.String);
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
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsAnalyse;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxRptFileFmt,
                            indxRptFileFmt,
                            RegistryValueKind.DWord);
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
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                             "\\" + _subKeyOptCharSet;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxName,
                            indxName,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxSubAct,
                             indxSubAct,
                             RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxSubCode,
                            indxSubCode,
                            RegistryValueKind.DWord);
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
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                             "\\" + _subKeyOptClrMap;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (flagClrMapUseClr)
                subKey.SetValue(_nameFlagClrMapUseClr,
                                 _flagTrue,
                                 RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagClrMapUseClr,
                                 _flagFalse,
                                 RegistryValueKind.DWord);
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

    public static void SaveOptClrMapCrnt(int[] indxClrMapBack,
                                          int[] indxClrMapFore)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                             "\\" + _subKeyOptClrMap +
                             "\\" + _subKeyCrnt;

        int ctIndx;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            ctIndx = indxClrMapBack.Length;

            for (int i = 0; i < ctIndx; i++)
            {
                subKey.SetValue(_nameIndxClrMapRootBack + i.ToString("D2"),
                                indxClrMapBack[i],
                                RegistryValueKind.DWord);
            }

            ctIndx = indxClrMapFore.Length;

            for (int i = 0; i < ctIndx; i++)
            {
                subKey.SetValue(_nameIndxClrMapRootFore + i.ToString("D2"),
                                indxClrMapFore[i],
                                RegistryValueKind.DWord);
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

    public static void SaveOptClrMapTheme(int number,
                                           int[] indxClrMapBack,
                                           int[] indxClrMapFore,
                                           string name)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string themeNo = number.ToString("D2");

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                             "\\" + _subKeyOptClrMap +
                             "\\" + _subKeyUserThemeRoot + themeNo;

        int ctIndx;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            ctIndx = indxClrMapBack.Length;

            for (int i = 0; i < ctIndx; i++)
            {
                subKey.SetValue(_nameIndxClrMapRootBack + i.ToString("D2"),
                                indxClrMapBack[i],
                                RegistryValueKind.DWord);
            }

            ctIndx = indxClrMapFore.Length;

            for (int i = 0; i < ctIndx; i++)
            {
                subKey.SetValue(_nameIndxClrMapRootFore + i.ToString("D2"),
                                indxClrMapFore[i],
                                RegistryValueKind.DWord);
            }

            if (name != null)
            {
                subKey.SetValue(_nameTheme,
                                 name,
                                 RegistryValueKind.String);
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

    public static void SaveOptGeneral(int indxOffsetType,
                                      bool flagMiscAutoAnalyse,
                                      bool flagDiagFileAccess)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                             "\\" + _subKeyOptGeneral;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxOffsetType,
                            indxOffsetType,
                            RegistryValueKind.DWord);

            if (flagMiscAutoAnalyse)
                subKey.SetValue(_nameFlagAutoAnalyse,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagAutoAnalyse,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagDiagFileAccess)
                subKey.SetValue(_nameFlagDiagFileAccess,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagDiagFileAccess,
                                _flagFalse,
                                RegistryValueKind.DWord);
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
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                             "\\" + _subKeyOptHPGL2;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (flagMiscBinData)
                subKey.SetValue(_nameFlagBinData,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagBinData,
                                _flagFalse,
                                RegistryValueKind.DWord);
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
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                             "\\" + _subKeyOptPCL;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (flagFontHddr)
                subKey.SetValue(_nameFlagFontHddr,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagFontHddr,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagFontChar)
                subKey.SetValue(_nameFlagFontChar,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagFontChar,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagFontDraw)
                subKey.SetValue(_nameFlagFontDraw,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagFontDraw,
                                _flagFalse,
                                RegistryValueKind.DWord);

            subKey.SetValue(_nameFontDrawHeight,
                             valFontDrawHeight,
                             RegistryValueKind.DWord);

            subKey.SetValue(_nameFontDrawWidth,
                             valFontDrawWidth,
                             RegistryValueKind.DWord);

            if (flagMacroDisplay)
                subKey.SetValue(_nameFlagMacroDisplay,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagMacroDisplay,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagMiscStyleData)
                subKey.SetValue(_nameFlagStyleData,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagStyleData,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagMiscBinData)
                subKey.SetValue(_nameFlagBinData,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagBinData,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagTransAlphaNumId)
                subKey.SetValue(_nameFlagAlphaNumId,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagAlphaNumId,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagTransColourLookup)
                subKey.SetValue(_nameFlagColourLookup,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagColourLookup,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagTransConfIO)
                subKey.SetValue(_nameFlagConfIO,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagConfIO,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagTransConfImageData)
                subKey.SetValue(_nameFlagConfImageData,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagConfImageData,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagTransConfRasterData)
                subKey.SetValue(_nameFlagConfRasterData,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagConfRasterData,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagTransDefLogPage)
                subKey.SetValue(_nameFlagDefLogPage,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagDefLogPage,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagTransDefSymSet)
                subKey.SetValue(_nameFlagDefSymSet,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagDefSymSet,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagTransDitherMatrix)
                subKey.SetValue(_nameFlagDitherMatrix,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagDitherMatrix,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagTransDriverConf)
                subKey.SetValue(_nameFlagDriverConf,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagDriverConf,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagTransEscEncText)
                subKey.SetValue(_nameFlagEscEncText,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagEscEncText,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagTransPaletteConf)
                subKey.SetValue(_nameFlagPaletteConf,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagPaletteConf,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagTransUserPattern)
                subKey.SetValue(_nameFlagUserPattern,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagUserPattern,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagTransViewIlluminant)
                subKey.SetValue(_nameFlagViewIlluminant,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagViewIlluminant,
                                _flagFalse,
                                RegistryValueKind.DWord);
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
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                             "\\" + _subKeyOptPCLXL;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (flagFontHddr)
                subKey.SetValue(_nameFlagFontHddr,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagFontHddr,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagFontChar)
                subKey.SetValue(_nameFlagFontChar,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagFontChar,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagFontDraw)
                subKey.SetValue(_nameFlagFontDraw,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagFontDraw,
                                _flagFalse,
                                RegistryValueKind.DWord);

            subKey.SetValue(_nameFontDrawHeight,
                             valFontDrawHeight,
                             RegistryValueKind.DWord);

            subKey.SetValue(_nameFontDrawWidth,
                             valFontDrawWidth,
                             RegistryValueKind.DWord);

            if (flagEncUserStream)
                subKey.SetValue(_nameFlagUserStream,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagUserStream,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagEncPCLPassThrough)
                subKey.SetValue(_nameFlagPCLPassThrough,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagPCLPassThrough,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagEncPCLFontSelect)
                subKey.SetValue(_nameFlagPCLFontSelect,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagPCLFontSelect,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagMiscOperPos)
                subKey.SetValue(_nameFlagOperPos,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagOperPos,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagMiscBinData)
                subKey.SetValue(_nameFlagBinData,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagBinData,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagMiscVerbose)
                subKey.SetValue(_nameFlagVerbose,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagVerbose,
                                _flagFalse,
                                RegistryValueKind.DWord);
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

    public static void SaveOptPML(bool flagWithinPCL,
                                  bool flagWithinPJL,
                                  bool flagVerbose)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                             "\\" + _subKeyOptPML;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (flagWithinPCL)
                subKey.SetValue(_nameFlagWithinPCL,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagWithinPCL,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagWithinPJL)
                subKey.SetValue(_nameFlagWithinPJL,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagWithinPJL,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagVerbose)
                subKey.SetValue(_nameFlagVerbose,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagVerbose,
                                _flagFalse,
                                RegistryValueKind.DWord);
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

    public static void SaveOptStats(int indxLevel,
                                     bool flagExcUnusedPCLObs,
                                     bool flagExcUnusedPCLXLRes)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsAnalyse +
                             "\\" + _subKeyOptStats;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxStatsLevel,
                            indxLevel,
                            RegistryValueKind.DWord);

            if (flagExcUnusedPCLObs)
                subKey.SetValue(_nameFlagExcUnusedPCLObs,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagExcUnusedPCLObs,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagExcUnusedPCLXLRes)
                subKey.SetValue(_nameFlagExcUnusedPCLXLRes,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagExcUnusedPCLXLRes,
                                _flagFalse,
                                RegistryValueKind.DWord);
        }
    }
}
