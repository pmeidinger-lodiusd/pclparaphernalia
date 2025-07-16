using Microsoft.Win32;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class manages persistent storage of options for the FontSample tool.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

static class ToolFontSamplePersist
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    const string _mainKey = MainForm._regMainKey;

    const string _subKeyTools = "Tools";
    const string _subKeyToolsFontSample = "FontSample";
    const string _subKeyPCL5 = "PCL5";
    const string _subKeyPCL6 = "PCL6";
    const string _subKeyPCL = "PCL";
    const string _subKeyPCLXL = "PCLXL";
    const string _subKeyCustom = "Custom";
    const string _subKeyDownload = "Download";
    const string _subKeyPreset = "Preset";
    const string _subKeyPrnDisk = "PrnMassStore";

    const string _nameCaptureFile = "CaptureFile";
    const string _nameDownloadFile = "DownloadFile";
    const string _nameDownloadId = "DownloadId";
    const string _nameMassStoreFontFile = "Filename";
    const string _nameMassStoreFontId = "AssociateFontId";
    const string _nameMassStoreMacroId = "AssociateMacroId";
    const string _nameFlagBound = "FlagBound";
    const string _nameFlagMassStoreViaMacro = "FlagLoadViaMacro";
    const string _nameFlagRamDataRemove = "FlagRamDataRemove";
    const string _nameFlagDownloadRemove = "FlagDownloadRemove";
    const string _nameFlagDetailsKnown = "FlagDetailsKnown";
    const string _nameFlagFormAsMacro = "FlagFormAsMacro";
    const string _nameFlagProportional = "FlagProportional";
    const string _nameFlagScalable = "FlagScalable";
    const string _nameFlagSelectById = "FlagSelectById";
    const string _nameFlagShowC0Chars = "FlagShowC0Chars";
    const string _nameFlagShowUserMapCodes = "FlagShowUserMapCodes";
    const string _nameFlagShowMapCodesUCS2 = "FlagShowMapCodesUCS2";
    const string _nameFlagShowMapCodesUTF8 = "FlagShowMapCodesUTF8";
    const string _nameFlagOptGridVertical = "FlagOptGridVertical";
    const string _nameFlagSymSetUserActEmbed = "FlagSymSetUserActEmbed";
    const string _nameFontName = "FontName";
    const string _nameHeight = "Height";
    const string _nameIndxFont = "IndxFont";
    const string _nameIndxOrientation = "IndxOrientation";
    const string _nameIndxPaperSize = "IndxPaperSize";
    const string _nameIndxPaperType = "IndxPaperType";
    const string _nameIndxPDL = "IndxPDL";
    const string _nameIndxSymSet = "IndxSymSet";
    const string _nameIndxVariant = "IndxVariant";
    const string _namePitch = "Pitch";
    const string _nameStyle = "Style";
    const string _nameSymSetNumber = "SymSetNumber";
    const string _nameSymSetUserFile = "SymSetUserFile";
    const string _nameTypeface = "Typeface";
    const string _nameWeight = "Weight";

    const int _flagFalse = 0;
    const int _flagTrue = 1;
    const int _indexZero = 0;

    const string _defaultCaptureFilePCL = "CaptureFile_FontSamplePCL.prn";
    const string _defaultCaptureFilePCLXL = "CaptureFile_FontSamplePCLXL.prn";
    const string _defaultFontFilePCL = "DefaultFontFile.sft";
    const string _defaultFontFilePCLXL = "DefaultFontFile.sfx";
    const string _defaultFontName = "Arial";
    const string _defaultSymSetUserFile = "DefaultSymSetFile.pcl";
    const string _defaultMassStoreFontFile = "DefaultFont.sfp";

    const int _sizeFactor = 1000;
    const int _defaultHeightPtsK = 15 * _sizeFactor;
    const int _defaultPitchPtsK = 8 * _sizeFactor;
    const int _defaultSymSetNo = 14;
    const int _defaultSoftFontIdPCL = 101;
    const int _defaultSoftFontIdMacroPCL = 201;
    const int _defaultIndxVariant = (int)(PCLFonts.eVariant.Regular);

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a C a p t u r e                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored FontSample capture file data.                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataCapture(ToolCommonData.ePrintLang crntPDL,
                                        ref string captureFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string oldKey = _subKeyTools + "\\" + _subKeyToolsFontSample;
        string oldFile;

        bool update_from_v2_5_0_0 = false;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

        //----------------------------------------------------------------//

        using (RegistryKey subKey = keyMain.OpenSubKey(oldKey, true))
        {
            oldFile = (string)subKey.GetValue(_nameCaptureFile);

            if (oldFile != null)
            {
                update_from_v2_5_0_0 = true;

                subKey.DeleteValue(_nameCaptureFile);
            }
        }

        if (update_from_v2_5_0_0)
        {
            string keyPCL = _subKeyTools +
                             "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(keyPCL))
            {
                subKey.SetValue(_nameCaptureFile,
                                 oldFile,
                                 RegistryValueKind.String);
            }

            string keyPCLXL = _subKeyTools +
                             "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCLXL;

            using (RegistryKey subKey = keyMain.CreateSubKey(keyPCLXL))
            {
                subKey.SetValue(_nameCaptureFile,
                                 oldFile,
                                 RegistryValueKind.String);
            }
        }

        //----------------------------------------------------------------//

        if (crntPDL == ToolCommonData.ePrintLang.PCL)
        {
            string key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                                        "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                captureFile = (string)subKey.GetValue(
                    _nameCaptureFile,
                    defWorkFolder + "\\" + _defaultCaptureFilePCL);
            }
        }
        else if (crntPDL == ToolCommonData.ePrintLang.PCLXL)
        {
            string key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                                        "\\" + _subKeyPCLXL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                captureFile = (string)subKey.GetValue(
                    _nameCaptureFile,
                    defWorkFolder + "\\" + _defaultCaptureFilePCLXL);
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a C o m m o n                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored FontSample common data.                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataCommon(ref int indxPDL,
                                       ref bool flagOptGridVertical)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        int tmpInt;

        string key = _subKeyTools + "\\" + _subKeyToolsFontSample;

        //----------------------------------------------------------------//

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (Helper_RegKey.KeyExists(subKey, _subKeyPCL5))
                // update from v2_5_0_0
                Helper_RegKey.RenameKey(subKey, _subKeyPCL5, _subKeyPCL);

            if (Helper_RegKey.KeyExists(subKey, _subKeyPCL6))
                // update from v2_5_0_0
                Helper_RegKey.RenameKey(subKey, _subKeyPCL6, _subKeyPCLXL);

            indxPDL = (int)subKey.GetValue(_nameIndxPDL,
                                             _indexZero);

            tmpInt = (int)subKey.GetValue(_nameFlagOptGridVertical,
                                                       _flagFalse);

            flagOptGridVertical = tmpInt != _flagFalse;
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a G e n e r a l                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored FontSample PCL or PCLXL general data.              //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataGeneral(string pdlName,
                                        ref int indxOrientation,
                                        ref int indxPaperSize,
                                        ref int indxPaperType,
                                        ref int indxFont,
                                        ref bool flagFormAsMacro,
                                        ref bool flagShowC0Chars,
                                        ref bool flagShowMapCodesUCS2,
                                        ref bool flagShowMapCodesUTF8,
                                        ref bool flagSymSetUserActEmbed)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        int tmpInt;

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + pdlName;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            //------------------------------------------------------------//
            // update from version 2.8.0 0                                //
            //------------------------------------------------------------//

            if (subKey.GetValue(_nameFlagShowUserMapCodes) != null)
            {
                if (subKey.GetValue(_nameFlagShowMapCodesUCS2) == null)
                {
                    subKey.SetValue(
                        _nameFlagShowMapCodesUCS2,
                        subKey.GetValue(_nameFlagShowUserMapCodes),
                        RegistryValueKind.DWord);
                }

                subKey.DeleteValue(_nameFlagShowUserMapCodes);
            }

            //------------------------------------------------------------//

            indxOrientation = (int)subKey.GetValue(_nameIndxOrientation,
                                                      _indexZero);
            indxPaperSize = (int)subKey.GetValue(_nameIndxPaperSize,
                                                      _indexZero);
            indxPaperType = (int)subKey.GetValue(_nameIndxPaperType,
                                                      _indexZero);
            indxFont = (int)subKey.GetValue(_nameIndxFont,
                                                      _indexZero);

            tmpInt = (int)subKey.GetValue(_nameFlagFormAsMacro,
                                                      _flagTrue);

            flagFormAsMacro = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagShowC0Chars,
                                                       _flagFalse);

            flagShowC0Chars = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagShowMapCodesUCS2,
                                            _flagFalse);

            flagShowMapCodesUCS2 = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagShowMapCodesUTF8,
                                            _flagFalse);

            flagShowMapCodesUTF8 = tmpInt != _flagFalse;

            if (pdlName == _subKeyPCL)
            {
                tmpInt = (int)subKey.GetValue(_nameFlagSymSetUserActEmbed,
                                                _flagTrue);

                flagSymSetUserActEmbed = tmpInt != _flagFalse;
            }
            else
            {
                flagSymSetUserActEmbed = false;
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a P C L C u s t o m                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored FontSample PCL data for 'custom' font.            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPCLCustom(ref bool flagProportional,
                                           ref bool flagScalable,
                                           ref bool flagBound,
                                           ref ushort style,
                                           ref ushort typeface,
                                           ref short weight,
                                           ref double height,
                                           ref double pitch,
                                           ref int indxSymSet,
                                           ref ushort symSetCustom,
                                           ref string symSetUserFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        int tmpInt;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

        //----------------------------------------------------------------//

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCL +
                             "\\" + _subKeyCustom;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            tmpInt = (int)subKey.GetValue(_nameFlagProportional,
                                            _flagTrue);

            flagProportional = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagScalable,
                                            _flagTrue);

            flagScalable = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagBound,
                                            _flagTrue);

            flagBound = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameStyle,
                                                     0);
            style = (ushort)tmpInt;

            tmpInt = (int)subKey.GetValue(_nameTypeface,
                                                     0);
            typeface = (ushort)tmpInt;

            tmpInt = (int)subKey.GetValue(_nameWeight,
                                                     0);
            weight = (short)tmpInt;

            tmpInt = (int)subKey.GetValue(_nameHeight,
                                                     _defaultHeightPtsK);
            height = tmpInt / _sizeFactor;

            tmpInt = (int)subKey.GetValue(_namePitch,
                                                     _defaultPitchPtsK);
            pitch = tmpInt / _sizeFactor;

            indxSymSet = (int)subKey.GetValue(_nameIndxSymSet,
                                                     _indexZero);

            tmpInt = (int)subKey.GetValue(_nameSymSetNumber,
                                                     _defaultSymSetNo);
            symSetCustom = (ushort)tmpInt;

            symSetUserFile = (string)subKey.GetValue(_nameSymSetUserFile,
                                                      defWorkFolder + "\\" +
                                                      _defaultSymSetUserFile);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a P C L D o w n l o a d                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored FontSample PCL data for 'download' font.          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPCLDownload(ref string downloadFile,
                                             ref ushort downloadId,
                                             ref bool flagDownloadRemove,
                                             ref bool flagSelectById,
                                             ref double height,
                                             ref double pitch,
                                             ref int indxSymSet,
                                             ref ushort symSetCustom,
                                             ref string symSetUserFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        int tmpInt;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

        //----------------------------------------------------------------//

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCL +
                             "\\" + _subKeyDownload;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            downloadFile = (string)subKey.GetValue(_nameDownloadFile,
                                                   defWorkFolder + "\\" +
                                                   _defaultFontFilePCL);

            tmpInt = (int)subKey.GetValue(_nameDownloadId,
                                                  1);
            downloadId = (ushort)tmpInt;

            tmpInt = (int)subKey.GetValue(_nameFlagSelectById,
                                                  _flagTrue);
            flagSelectById = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagDownloadRemove,
                                                  _flagTrue);
            flagDownloadRemove = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameHeight,
                                                  _defaultHeightPtsK);
            height = tmpInt / _sizeFactor;

            tmpInt = (int)subKey.GetValue(_namePitch,
                                                  _defaultPitchPtsK);
            pitch = tmpInt / _sizeFactor;

            indxSymSet = (int)subKey.GetValue(_nameIndxSymSet,
                                                  _indexZero);

            tmpInt = (int)subKey.GetValue(_nameSymSetNumber,
                                                  _defaultSymSetNo);
            symSetCustom = (ushort)tmpInt;

            symSetUserFile = (string)subKey.GetValue(_nameSymSetUserFile,
                                                      defWorkFolder + "\\" +
                                                      _defaultSymSetUserFile);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a P C L P r e s e t                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored FontSample PCL data for 'preset' font.            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPCLPreset(ref int indxFont,
                                           ref PCLFonts.eVariant variant,
                                           ref double height,
                                           ref double pitch,
                                           ref int indxSymSet,
                                           ref ushort symSetCustom,
                                           ref string symSetUserFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        int tmpInt;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

        //----------------------------------------------------------------//

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCL +
                             "\\" + _subKeyPreset;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            indxFont = (int)subKey.GetValue(_nameIndxFont,
                                                  _indexZero);

            tmpInt = (int)subKey.GetValue(_nameIndxVariant,
                                                  _defaultIndxVariant);
            variant = (PCLFonts.eVariant)tmpInt;

            tmpInt = (int)subKey.GetValue(_nameHeight,
                                                  _defaultHeightPtsK);
            height = tmpInt / _sizeFactor;

            tmpInt = (int)subKey.GetValue(_namePitch,
                                                  _defaultPitchPtsK);
            pitch = tmpInt / _sizeFactor;

            indxSymSet = (int)subKey.GetValue(_nameIndxSymSet,
                                                  _indexZero);

            tmpInt = (int)subKey.GetValue(_nameSymSetNumber,
                                                  _defaultSymSetNo);
            symSetCustom = (ushort)tmpInt;

            symSetUserFile = (string)subKey.GetValue(_nameSymSetUserFile,
                                                      defWorkFolder + "\\" +
                                                      _defaultSymSetUserFile);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a P C L P r n D i s k                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored FontSample PCL data for 'printer mass storage'     //
    // font.                                                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPCLPrnDisk(ref string fontFilename,
                                           ref ushort fontId,
                                           ref ushort macroId,
                                           ref bool flagRamDataRemove,
                                           ref bool flagSelById,
                                           ref bool flagLoadViaMacro,
                                           ref bool flagDetailsKnown,
                                           ref bool flagProportional,
                                           ref bool flagScalable,
                                           ref bool flagBound,
                                           ref ushort style,
                                           ref ushort typeface,
                                           ref short weight,
                                           ref double height,
                                           ref double pitch,
                                           ref int indxSymSet,
                                           ref ushort symSetCustom,
                                           ref string symSetUserFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        int tmpInt;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

        //----------------------------------------------------------------//

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCL +
                             "\\" + _subKeyPrnDisk;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            fontFilename = (string)subKey.GetValue(_nameMassStoreFontFile,
                                                   _defaultMassStoreFontFile);

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameMassStoreFontId,
                                            _defaultSoftFontIdPCL);
            fontId = (ushort)tmpInt;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameMassStoreMacroId,
                                            _defaultSoftFontIdMacroPCL);
            macroId = (ushort)tmpInt;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagRamDataRemove,
                                            _flagFalse);

            flagRamDataRemove = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagSelectById,
                                            _flagFalse);

            flagSelById = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagMassStoreViaMacro,
                                            _flagFalse);

            flagLoadViaMacro = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagDetailsKnown,
                                            _flagFalse);

            flagDetailsKnown = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagProportional,
                                            _flagFalse);

            flagProportional = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagScalable,
                                            _flagFalse);

            flagScalable = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameFlagBound,
                                            _flagTrue);

            flagBound = tmpInt != _flagFalse;

            //------------------------------------------------------------//

            tmpInt = (int)subKey.GetValue(_nameStyle, 0);
            style = (ushort)tmpInt;

            tmpInt = (int)subKey.GetValue(_nameTypeface, 3);
            typeface = (ushort)tmpInt;

            tmpInt = (int)subKey.GetValue(_nameWeight, 0);
            weight = (short)tmpInt;

            tmpInt = (int)subKey.GetValue(_nameHeight, _defaultHeightPtsK);
            height = tmpInt / _sizeFactor;

            tmpInt = (int)subKey.GetValue(_namePitch, _defaultPitchPtsK);
            pitch = tmpInt / _sizeFactor;

            //------------------------------------------------------------//

            indxSymSet = (int)subKey.GetValue(_nameIndxSymSet, _indexZero);

            tmpInt = (int)subKey.GetValue(_nameSymSetNumber, _defaultSymSetNo);
            symSetCustom = (ushort)tmpInt;

            symSetUserFile = (string)subKey.GetValue(_nameSymSetUserFile,
                                                      defWorkFolder + "\\" +
                                                      _defaultSymSetUserFile);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a P C L X L C u s t o m                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored FontSample PCLXL data for 'custom' font.            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPCLXLCustom(ref string fontName,
                                            ref double height,
                                            ref int indxSymSet,
                                            ref ushort symSetCustom,
                                            ref string symSetUserFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        int tmpInt;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCLXL +
                             "\\" + _subKeyCustom;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            fontName = (string)subKey.GetValue(_nameFontName,
                                                   _defaultFontName);

            tmpInt = (int)subKey.GetValue(_nameHeight,
                                                  _defaultHeightPtsK);
            height = tmpInt / _sizeFactor;

            indxSymSet = (int)subKey.GetValue(_nameIndxSymSet,
                                                  _indexZero);

            tmpInt = (int)subKey.GetValue(_nameSymSetNumber,
                                                  _defaultSymSetNo);
            symSetCustom = (ushort)tmpInt;

            symSetUserFile = (string)subKey.GetValue(_nameSymSetUserFile,
                                                      defWorkFolder + "\\" +
                                                      _defaultSymSetUserFile);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a P C L X L D o w n l o a d                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored FontSample PCLXL data for 'download' font.          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPCLXLDownload(ref string downloadFile,
                                              ref bool flagDownloadRemove,
                                              ref double height,
                                              ref int indxSymSet,
                                              ref ushort symSetCustom,
                                              ref string symSetUserFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        int tmpInt;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

        //----------------------------------------------------------------//

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCLXL +
                             "\\" + _subKeyDownload;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            downloadFile = (string)subKey.GetValue(_nameDownloadFile,
                                                   defWorkFolder + "\\" +
                                                   _defaultFontFilePCLXL);

            tmpInt = (int)subKey.GetValue(_nameFlagDownloadRemove,
                                                   _flagTrue);
            flagDownloadRemove = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameHeight,
                                                  _defaultHeightPtsK);
            height = tmpInt / _sizeFactor;

            indxSymSet = (int)subKey.GetValue(_nameIndxSymSet,
                                                  _indexZero);

            tmpInt = (int)subKey.GetValue(_nameSymSetNumber,
                                                  _defaultSymSetNo);
            symSetCustom = (ushort)tmpInt;

            symSetUserFile = (string)subKey.GetValue(_nameSymSetUserFile,
                                      defWorkFolder + "\\" +
                                      _defaultSymSetUserFile);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a P C L X L P r e s e t                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored FontSample PCLXL data for 'preset' font.            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPCLXLPreset(ref int indxFont,
                                            ref PCLFonts.eVariant variant,
                                            ref double height,
                                            ref int indxSymSet,
                                            ref ushort symSetCustom,
                                            ref string symSetUserFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        int tmpInt;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCLXL +
                             "\\" + _subKeyPreset;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            indxFont = (int)subKey.GetValue(_nameIndxFont,
                                                  _indexZero);

            tmpInt = (int)subKey.GetValue(_nameIndxVariant,
                                                  _defaultIndxVariant);
            variant = (PCLFonts.eVariant)tmpInt;

            tmpInt = (int)subKey.GetValue(_nameHeight,
                                                  _defaultHeightPtsK);
            height = tmpInt / _sizeFactor;

            indxSymSet = (int)subKey.GetValue(_nameIndxSymSet,
                                                  _indexZero);

            tmpInt = (int)subKey.GetValue(_nameSymSetNumber,
                                                  _defaultSymSetNo);
            symSetCustom = (ushort)tmpInt;

            symSetUserFile = (string)subKey.GetValue(_nameSymSetUserFile,
                                                      defWorkFolder + "\\" +
                                                      _defaultSymSetUserFile);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a C a p t u r e                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current FontSample capture file data.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataCapture(ToolCommonData.ePrintLang crntPDL,
                                        string captureFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        if (crntPDL == ToolCommonData.ePrintLang.PCL)
        {
            string key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                                        "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (captureFile != null)
                {
                    subKey.SetValue(_nameCaptureFile,
                                     captureFile,
                                     RegistryValueKind.String);
                }
            }
        }
        else if (crntPDL == ToolCommonData.ePrintLang.PCLXL)
        {
            string key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                                        "\\" + _subKeyPCLXL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (captureFile != null)
                {
                    subKey.SetValue(_nameCaptureFile,
                                     captureFile,
                                     RegistryValueKind.String);
                }
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a C o m m o n                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current FontSample common data.                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataCommon(int indxPDL,
                                       bool flagOptGridVertical)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsFontSample;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxPDL,
                            indxPDL,
                            RegistryValueKind.DWord);

            if (flagOptGridVertical)
                subKey.SetValue(_nameFlagOptGridVertical,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagOptGridVertical,
                                _flagFalse,
                                RegistryValueKind.DWord);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a G e n e r a l                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current FontSample PCL or PCLXL general data.                //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataGeneral(string pdlName,
                                        int indxOrientation,
                                        int indxPaperSize,
                                        int indxPaperType,
                                        int indxFont,
                                        bool flagFormAsMacro,
                                        bool flagShowC0Chars,
                                        bool flagShowMapCodesUCS2,
                                        bool flagShowMapCodesUTF8,
                                        bool flagSymSetUserActEmbed)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + pdlName;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxOrientation,
                            indxOrientation,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxPaperSize,
                            indxPaperSize,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxPaperType,
                            indxPaperType,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxFont,
                            indxFont,
                            RegistryValueKind.DWord);

            if (flagFormAsMacro)
                subKey.SetValue(_nameFlagFormAsMacro,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagFormAsMacro,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagShowC0Chars)
                subKey.SetValue(_nameFlagShowC0Chars,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagShowC0Chars,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagShowMapCodesUCS2)
                subKey.SetValue(_nameFlagShowMapCodesUCS2,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagShowMapCodesUCS2,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagShowMapCodesUTF8)
                subKey.SetValue(_nameFlagShowMapCodesUTF8,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagShowMapCodesUTF8,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (pdlName == _subKeyPCL)
            {
                if (flagSymSetUserActEmbed)
                    subKey.SetValue(_nameFlagSymSetUserActEmbed,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagSymSetUserActEmbed,
                                    _flagFalse,
                                    RegistryValueKind.DWord);
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a P C L C u s t o m                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store FontSample PCL data for 'custom' font.                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataPCLCustom(bool flagProportional,
                                          bool flagScalable,
                                          bool flagBound,
                                          ushort style,
                                          ushort typeface,
                                          short weight,
                                          double height,
                                          double pitch,
                                          int indxSymSet,
                                          ushort symSetCustom,
                                          string symSetUserFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCL +
                             "\\" + _subKeyCustom;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (flagProportional)
                subKey.SetValue(_nameFlagProportional,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagProportional,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagScalable)
                subKey.SetValue(_nameFlagScalable,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagScalable,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagBound)
                subKey.SetValue(_nameFlagBound,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagBound,
                                _flagFalse,
                                RegistryValueKind.DWord);

            subKey.SetValue(_nameStyle,
                            style,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameTypeface,
                            typeface,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameWeight,
                            weight,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameHeight,
                            height * _sizeFactor,
                            RegistryValueKind.DWord);

            subKey.SetValue(_namePitch,
                            pitch * _sizeFactor,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxSymSet,
                            indxSymSet,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameSymSetNumber,
                            symSetCustom,
                            RegistryValueKind.DWord);

            if (symSetUserFile != null)
            {
                subKey.SetValue(_nameSymSetUserFile,
                                 symSetUserFile,
                                 RegistryValueKind.String);
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a P C L D o w n l o a d                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store FontSample PCL data for 'download' font.                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataPCLDownload(string downloadFile,
                                            ushort downloadId,
                                            bool flagDownloadRemove,
                                            bool flagSelectById,
                                            double height,
                                            double pitch,
                                            int indxSymSet,
                                            ushort symSetCustom,
                                            string symSetUserFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCL +
                             "\\" + _subKeyDownload;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (downloadFile != null)
            {
                subKey.SetValue(_nameDownloadFile,
                                downloadFile,
                                RegistryValueKind.String);
            }

            subKey.SetValue(_nameDownloadId,
                            downloadId,
                            RegistryValueKind.DWord);

            if (flagDownloadRemove)
                subKey.SetValue(_nameFlagDownloadRemove,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagDownloadRemove,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagSelectById)
                subKey.SetValue(_nameFlagSelectById,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagSelectById,
                                _flagFalse,
                                RegistryValueKind.DWord);

            subKey.SetValue(_nameHeight,
                            height * _sizeFactor,
                            RegistryValueKind.DWord);

            subKey.SetValue(_namePitch,
                            pitch * _sizeFactor,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxSymSet,
                            indxSymSet,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameSymSetNumber,
                            symSetCustom,
                            RegistryValueKind.DWord);

            if (symSetUserFile != null)
            {
                subKey.SetValue(_nameSymSetUserFile,
                                 symSetUserFile,
                                 RegistryValueKind.String);
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a P C L P r e s e t                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store FontSample PCL data for 'preset' font.                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataPCLPreset(int indxFont,
                                          PCLFonts.eVariant variant,
                                          double height,
                                          double pitch,
                                          int indxSymSet,
                                          ushort symSetCustom,
                                          string symSetUserFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCL +
                             "\\" + _subKeyPreset;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxFont,
                            indxFont,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxVariant,
                            (int)variant,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameHeight,
                            height * _sizeFactor,
                            RegistryValueKind.DWord);

            subKey.SetValue(_namePitch,
                            pitch * _sizeFactor,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxSymSet,
                            indxSymSet,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameSymSetNumber,
                            symSetCustom,
                            RegistryValueKind.DWord);

            if (symSetUserFile != null)
            {
                subKey.SetValue(_nameSymSetUserFile,
                                 symSetUserFile,
                                 RegistryValueKind.String);
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a P C L P r n D i s k                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store FontSample PCL data for 'printer mass storage' font.         //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataPCLPrnDisk(string fontFilename,
                                           ushort fontId,
                                           ushort macroId,
                                           bool flagRamDataRemove,
                                           bool flagSelById,
                                           bool flagLoadViaMacro,
                                           bool flagDetailsKnown,
                                           bool flagProportional,
                                           bool flagScalable,
                                           bool flagBound,
                                           ushort style,
                                           ushort typeface,
                                           short weight,
                                           double height,
                                           double pitch,
                                           int indxSymSet,
                                           ushort symSetCustom,
                                           string symSetUserFile)
    {

        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCL +
                             "\\" + _subKeyPrnDisk;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (fontFilename != null)
            {
                subKey.SetValue(_nameMassStoreFontFile,
                                 fontFilename,
                                 RegistryValueKind.String);
            }

            subKey.SetValue(_nameMassStoreFontId,
                            fontId,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameMassStoreMacroId,
                            macroId,
                            RegistryValueKind.DWord);

            if (flagRamDataRemove)
                subKey.SetValue(_nameFlagRamDataRemove,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagRamDataRemove,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagSelById)
                subKey.SetValue(_nameFlagSelectById,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagSelectById,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagLoadViaMacro)
                subKey.SetValue(_nameFlagMassStoreViaMacro,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagMassStoreViaMacro,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagDetailsKnown)
                subKey.SetValue(_nameFlagDetailsKnown,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagDetailsKnown,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagProportional)
                subKey.SetValue(_nameFlagProportional,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagProportional,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagScalable)
                subKey.SetValue(_nameFlagScalable,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagScalable,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagBound)
                subKey.SetValue(_nameFlagBound,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagBound,
                                _flagFalse,
                                RegistryValueKind.DWord);

            subKey.SetValue(_nameStyle,
                            style,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameTypeface,
                            typeface,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameWeight,
                            weight,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameHeight,
                            height * _sizeFactor,
                            RegistryValueKind.DWord);

            subKey.SetValue(_namePitch,
                            pitch * _sizeFactor,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxSymSet,
                            indxSymSet,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameSymSetNumber,
                            symSetCustom,
                            RegistryValueKind.DWord);

            if (symSetUserFile != null)
            {
                subKey.SetValue(_nameSymSetUserFile,
                                 symSetUserFile,
                                 RegistryValueKind.String);
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a P C L X L C u s t o m                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store FontSample PCLXL data for 'custom' font.                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataPCLXLCustom(string fontName,
                                            double height,
                                            int indxSymSet,
                                            ushort symSetCustom,
                                            string symSetUserFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCLXL +
                             "\\" + _subKeyCustom;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (fontName != null)
            {
                subKey.SetValue(_nameFontName,
                                fontName,
                                RegistryValueKind.String);
            }

            subKey.SetValue(_nameHeight,
                            height * _sizeFactor,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxSymSet,
                            indxSymSet,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameSymSetNumber,
                            symSetCustom,
                            RegistryValueKind.DWord);

            if (symSetUserFile != null)
            {
                subKey.SetValue(_nameSymSetUserFile,
                                 symSetUserFile,
                                 RegistryValueKind.String);
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a P C L X L D o w n l o a d                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store FontSample PCLXL data for 'download' font.                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataPCLXLDownload(string downloadFile,
                                              bool flagDownloadRemove,
                                              double height,
                                              int indxSymSet,
                                              ushort symSetCustom,
                                              string symSetUserFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCLXL +
                             "\\" + _subKeyDownload;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (downloadFile != null)
            {
                subKey.SetValue(_nameDownloadFile,
                                downloadFile,
                                RegistryValueKind.String);
            }

            if (flagDownloadRemove)
                subKey.SetValue(_nameFlagDownloadRemove,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagDownloadRemove,
                                _flagFalse,
                                RegistryValueKind.DWord);

            subKey.SetValue(_nameHeight,
                            height * _sizeFactor,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxSymSet,
                            indxSymSet,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameSymSetNumber,
                            symSetCustom,
                            RegistryValueKind.DWord);

            if (symSetUserFile != null)
            {
                subKey.SetValue(_nameSymSetUserFile,
                                 symSetUserFile,
                                 RegistryValueKind.String);
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a P C L X L P r e s e t                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store FontSample PCLXL data for 'preset' font.                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataPCLXLPreset(int indxFont,
                                            PCLFonts.eVariant variant,
                                            double height,
                                            int indxSymSet,
                                            ushort symSetCustom,
                                            string symSetUserFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsFontSample +
                             "\\" + _subKeyPCLXL +
                             "\\" + _subKeyPreset;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxFont,
                            indxFont,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxVariant,
                            (int)variant,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameHeight,
                            height * _sizeFactor,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxSymSet,
                            indxSymSet,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameSymSetNumber,
                            symSetCustom,
                            RegistryValueKind.DWord);

            if (symSetUserFile != null)
            {
                subKey.SetValue(_nameSymSetUserFile,
                                 symSetUserFile,
                                 RegistryValueKind.String);
            }
        }
    }
}
