using Microsoft.Win32;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class manages persistent storage of options for the TrayMap tool.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

static class ToolTrayMapPersist
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    const string _mainKey = MainForm._regMainKey;

    const string _subKeyTools = "Tools";
    const string _subKeyToolsTrayMap = "TrayMap";
    const string _subKeyPCL5 = "PCL5";
    const string _subKeyPCL6 = "PCL6";
    const string _subKeyPCL = "PCL";
    const string _subKeyPCLXL = "PCLXL";
    const string _subKeySheetRoot = "Sheet_";

    const string _nameCaptureFile = "CaptureFile";
    const string _nameFlagFormAsMacro = "FlagFormAsMacro";
    const string _nameIndxOrientation = "IndxOrientation"; // pre v2.8 //
    const string _nameIndxOrientFront = "IndxOrientFront";
    const string _nameIndxOrientRear = "IndxOrientRear";
    const string _nameIndxPaperSize = "IndxPaperSize";
    const string _nameIndxPaperType = "IndxPaperType";
    const string _nameIndxPaperTray = "IndxPaperTray";
    const string _nameIndxPDL = "IndxPDL";
    const string _nameIndxPlexMode = "IndxPlexMode";
    const string _nameIndxTrayIdOpt = "IndxTrayIdOpt"; // pre v2.8 //
    const string _nameSheetCt = "SheetCt";
    const string _nameTrayIdList = "TrayIdList"; // pre v2.8 //

    const int _flagFalse = 0;
    const int _flagTrue = 1;
    const int _indexZero = 0;
    const int _indexOne = 1;

    const string _defaultCaptureFilePCL = "CaptureFile_TrayMapPCL.prn";
    const string _defaultCaptureFilePCLXL = "CaptureFile_TrayMapPCLXL.prn";

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a C a p t u r e                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored Tray Map capture file data.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataCapture(ToolCommonData.ePrintLang crntPDL,
                                        ref string captureFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string oldKey = _subKeyTools + "\\" + _subKeyToolsTrayMap;
        string oldFile;

        bool update_from_v2_5_0_0 = false;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

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
                             "\\" + _subKeyToolsTrayMap +
                             "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(keyPCL))
            {
                subKey.SetValue(_nameCaptureFile,
                                 oldFile,
                                 RegistryValueKind.String);
            }

            string keyPCLXL = _subKeyTools +
                             "\\" + _subKeyToolsTrayMap +
                             "\\" + _subKeyPCLXL;

            using (RegistryKey subKey = keyMain.CreateSubKey(keyPCLXL))
            {
                subKey.SetValue(_nameCaptureFile,
                                 oldFile,
                                 RegistryValueKind.String);
            }
        }

        if (crntPDL == ToolCommonData.ePrintLang.PCL)
        {
            string key = _subKeyTools + "\\" + _subKeyToolsTrayMap +
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
            string key = _subKeyTools + "\\" + _subKeyToolsTrayMap +
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
    // Retrieve stored TrayMap common data.                               //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataCommon(ref int indxPDL)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsTrayMap;

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
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a P C L O p t                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored TrayMap PCL options.                               //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPCLOpt(ref bool flagFormAsMacro,
                                       ref int sheetCt)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        int tmpInt;

        byte[] buffer = { 0x00 };

        key = _subKeyTools + "\\" + _subKeyToolsTrayMap +
                             "\\" + _subKeyPCL;

        if (MainFormData.VersionChange)
        {
            bool update_2_8 = false;

            int vMaj = -1,
                  vMin = -1,
                  vBui = -1,
                  vRev = -1;

            MainFormData.GetVersionData(false,
                                         ref vMaj, ref vMin,
                                         ref vBui, ref vRev);

            if ((vMaj == 2) && (vMin < 8))
                update_2_8 = true;

            if (update_2_8)
            {
                using (RegistryKey subKey = keyMain.CreateSubKey(key))
                {
                    subKey.DeleteValue(_nameIndxOrientation);
                    subKey.DeleteValue(_nameIndxPaperSize);
                    subKey.DeleteValue(_nameIndxPaperType);
                    subKey.DeleteValue(_nameIndxTrayIdOpt);
                    subKey.DeleteValue(_nameTrayIdList);
                }
            }
        }

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            tmpInt = (int)subKey.GetValue(_nameFlagFormAsMacro,
                                                     _flagTrue);

            flagFormAsMacro = tmpInt != _flagFalse;

            sheetCt = (int)subKey.GetValue(_nameSheetCt,
                                             _indexOne);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a P C L X L O p t                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored TrayMap PCL XL options.                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPCLXLOpt(ref bool flagFormAsMacro,
                                         ref int sheetCt)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        int tmpInt;

        byte[] buffer = { 0x00 };

        key = _subKeyTools + "\\" + _subKeyToolsTrayMap +
                             "\\" + _subKeyPCLXL;

        if (MainFormData.VersionChange)
        {
            bool update_2_8 = false;

            int vMaj = -1,
                  vMin = -1,
                  vBui = -1,
                  vRev = -1;

            MainFormData.GetVersionData(false,
                                         ref vMaj, ref vMin,
                                         ref vBui, ref vRev);

            if ((vMaj == 2) && (vMin < 8))
                update_2_8 = true;

            if (update_2_8)
            {
                using (RegistryKey subKey = keyMain.CreateSubKey(key))
                {
                    subKey.DeleteValue(_nameIndxOrientation);
                    subKey.DeleteValue(_nameIndxPaperSize);
                    subKey.DeleteValue(_nameIndxPaperType);
                    subKey.DeleteValue(_nameIndxTrayIdOpt);
                    subKey.DeleteValue(_nameTrayIdList);
                }
            }
        }

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            tmpInt = (int)subKey.GetValue(_nameFlagFormAsMacro,
                                                     _flagTrue);

            flagFormAsMacro = tmpInt != _flagFalse;

            sheetCt = (int)subKey.GetValue(_nameSheetCt,
                                             _indexOne);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a S h e e t O p t                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored TrayMap Sheet option data for PCL or PCLXL.        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataSheetOpt(string pdlName,
                                         int sheetNo,
                                         ref int indxPaperSize,
                                         ref int indxPaperType,
                                         ref int indxPaperTray,
                                         ref int indxPlexMode,
                                         ref int indxOrient_F,
                                         ref int indxOrient_R)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsTrayMap +
                             "\\" + pdlName +
                             "\\" + _subKeySheetRoot +
                             sheetNo.ToString("D2");

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            indxPaperSize = (int)subKey.GetValue(_nameIndxPaperSize,
                                                     _indexZero);

            indxPaperType = (int)subKey.GetValue(_nameIndxPaperType,
                                                     _indexZero);

            indxPaperTray = (int)subKey.GetValue(_nameIndxPaperTray,
                                                   _indexZero);

            indxPlexMode = (int)subKey.GetValue(_nameIndxPlexMode,
                                                  _indexZero);

            indxOrient_F = (int)subKey.GetValue(_nameIndxOrientFront,
                                                  _indexZero);

            indxOrient_R = (int)subKey.GetValue(_nameIndxOrientRear,
                                                  _indexZero);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a C a p t u r e                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current Tray Map capture file data.                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataCapture(ToolCommonData.ePrintLang crntPDL,
                                        string captureFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        if (crntPDL == ToolCommonData.ePrintLang.PCL)
        {
            string key = _subKeyTools + "\\" + _subKeyToolsTrayMap +
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
            string key = _subKeyTools + "\\" + _subKeyToolsTrayMap +
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
    // Store current TrayMap common data.                                 //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataCommon(int indxPDL)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsTrayMap;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxPDL,
                            indxPDL,
                            RegistryValueKind.DWord);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a P C L O p t                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current TrayMap PCL options.                                 //
    //--------------------------------------------------------------------//

    public static void SaveDataPCLOpt(bool flagFormAsMacro,
                                       int sheetCt)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsTrayMap +
                             "\\" + _subKeyPCL;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (flagFormAsMacro)
                subKey.SetValue(_nameFlagFormAsMacro,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagFormAsMacro,
                                _flagFalse,
                                RegistryValueKind.DWord);

            subKey.SetValue(_nameSheetCt,
                            sheetCt,
                            RegistryValueKind.DWord);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a P C L X L O p t                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current TrayMap PCL XL options.                              //
    //--------------------------------------------------------------------//

    public static void SaveDataPCLXLOpt(bool flagFormAsMacro,
                                         int sheetCt)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsTrayMap +
                             "\\" + _subKeyPCLXL;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (flagFormAsMacro)
                subKey.SetValue(_nameFlagFormAsMacro,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagFormAsMacro,
                                _flagFalse,
                                RegistryValueKind.DWord);

            subKey.SetValue(_nameSheetCt,
                            sheetCt,
                            RegistryValueKind.DWord);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a S h e e t O p t                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current TrayMap Sheet option data for PCL or PCLXL.          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataSheetOpt(string pdlName,
                                         int sheetNo,
                                         int indxPaperSize,
                                         int indxPaperType,
                                         int indxPaperTray,
                                         int indxPlexMode,
                                         int indxOrientFront,
                                         int indxOrientRear)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsTrayMap +
                             "\\" + pdlName +
                             "\\" + _subKeySheetRoot +
                             sheetNo.ToString("D2");

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxPaperSize,
                            indxPaperSize,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxPaperType,
                            indxPaperType,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxPaperTray,
                            indxPaperTray,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxPlexMode,
                            indxPlexMode,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxOrientFront,
                            indxOrientFront,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxOrientRear,
                            indxOrientRear,
                            RegistryValueKind.DWord);
        }
    }
}
