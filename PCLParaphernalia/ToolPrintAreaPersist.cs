using Microsoft.Win32;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class manages persistent storage of options for the PrintArea tool.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

static class ToolPrintAreaPersist
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    const string _mainKey = MainForm._regMainKey;

    const string _subKeyTools = "Tools";
    const string _subKeyToolsPrintArea = "PrintArea";
    const string _subKeyPCL5 = "PCL5";
    const string _subKeyPCL6 = "PCL6";
    const string _subKeyPCL = "PCL";
    const string _subKeyPCLXL = "PCLXL";

    const string _nameCaptureFile = "CaptureFile";
    const string _nameFlagFormAsMacro = "FlagFormAsMacro";
    const string _nameFlagCustomUseMetric = "FlagCustomUseMetric";
    const string _nameCustomShortEdge = "CustomShortEdge";
    const string _nameCustomLongEdge = "CustomLongEdge";
    const string _nameIndxOrientation = "IndxOrientation";
    const string _nameIndxPaperSize = "IndxPaperSize";
    const string _nameIndxPaperType = "IndxPaperType";
    const string _nameIndxPDL = "IndxPDL";
    const string _nameIndxPJLCommand = "IndxPJLCommand";
    const string _nameIndxPlexMode = "IndxPlexMode";

    const int _flagFalse = 0;
    const int _flagTrue = 1;
    const int _indexZero = 0;

    const int _customShortEdgeDefault = 4960;    // A4 dots @ 600 dpi 
    const int _customLongEdgeDefault = 7014;    // A4 dots @ 600 dpi 

    const string _defaultCaptureFilePCL = "CaptureFile_PrintAreaPCL.prn";
    const string _defaultCaptureFilePCLXL = "CaptureFile_PrintAreaPCLXL.prn";

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a C a p t u r e                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored Print Area capture file data.                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataCapture(ToolCommonData.ePrintLang crntPDL,
                                        ref string captureFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string oldKey = _subKeyTools + "\\" + _subKeyToolsPrintArea;
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
                             "\\" + _subKeyToolsPrintArea +
                             "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(keyPCL))
            {
                subKey.SetValue(_nameCaptureFile,
                                 oldFile,
                                 RegistryValueKind.String);
            }

            string keyPCLXL = _subKeyTools +
                             "\\" + _subKeyToolsPrintArea +
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
            string key = _subKeyTools + "\\" + _subKeyToolsPrintArea +
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
            string key = _subKeyTools + "\\" + _subKeyToolsPrintArea +
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
    // Retrieve stored PrintArea common data.                             //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataCommon(ref int indxPDL)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsPrintArea;

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
    // l o a d D a t a P C L                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored PrintArea PCL or PCLXL data.                       //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPCL(string pdlName,
                                   ref int indxOrientation,
                                   ref int indxPaperSize,
                                   ref int indxPaperType,
                                   ref int indxPlexMode,
                                   ref int indxPJLCommand,
                                   ref bool flagFormAsMacro,
                                   ref bool flagCustomUseMetric,
                                   ref ushort customShortEdge,
                                   ref ushort customLongEdge)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        int tmpInt;

        byte[] buffer = { 0x00 };

        key = _subKeyTools + "\\" + _subKeyToolsPrintArea +
                             "\\" + pdlName;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            indxOrientation = (int)subKey.GetValue(_nameIndxOrientation,
                                                     _indexZero);

            indxPaperSize = (int)subKey.GetValue(_nameIndxPaperSize,
                                                     _indexZero);

            indxPaperType = (int)subKey.GetValue(_nameIndxPaperType,
                                                     _indexZero);

            indxPlexMode = (int)subKey.GetValue(_nameIndxPlexMode,
                                                      _indexZero);

            indxPJLCommand = (int)subKey.GetValue(_nameIndxPJLCommand,
                                                     _indexZero);

            tmpInt = (int)subKey.GetValue(_nameFlagFormAsMacro,
                                                     _flagTrue);

            flagFormAsMacro = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagCustomUseMetric,
                                                     _flagTrue);

            flagCustomUseMetric = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameCustomShortEdge,
                                            _customShortEdgeDefault);

            customShortEdge = (ushort)tmpInt;

            tmpInt = (int)subKey.GetValue(_nameCustomLongEdge,
                                            _customLongEdgeDefault);

            customLongEdge = (ushort)tmpInt;
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a C a p t u r e                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current Print Area capture file data.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataCapture(ToolCommonData.ePrintLang crntPDL,
                                        string captureFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        if (crntPDL == ToolCommonData.ePrintLang.PCL)
        {
            string key = _subKeyTools + "\\" + _subKeyToolsPrintArea +
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
            string key = _subKeyTools + "\\" + _subKeyToolsPrintArea +
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
    // Store current PrintArea common data.                               //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataCommon(int indxPDL)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsPrintArea;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxPDL,
                            indxPDL,
                            RegistryValueKind.DWord);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a P C L                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current PrintArea PCL or PCLXL data.                         //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataPCL(string pdlName,
                                   int indxOrientation,
                                   int indxPaperSize,
                                   int indxPaperType,
                                   int indxPlexMode,
                                   int indxPJLCommand,
                                   bool flagFormAsMacro,
                                   bool flagCustomUseMetric,
                                   ushort customShortEdge,
                                   ushort customLongEdge)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsPrintArea +
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

            subKey.SetValue(_nameIndxPlexMode,
                            indxPlexMode,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxPJLCommand,
                            indxPJLCommand,
                            RegistryValueKind.DWord);

            if (flagFormAsMacro)
                subKey.SetValue(_nameFlagFormAsMacro, _flagTrue, RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagFormAsMacro, _flagFalse, RegistryValueKind.DWord);

            if (flagCustomUseMetric)
                subKey.SetValue(_nameFlagCustomUseMetric, _flagTrue, RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagCustomUseMetric, _flagFalse, RegistryValueKind.DWord);

            subKey.SetValue(_nameCustomShortEdge,
                            customShortEdge,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameCustomLongEdge,
                            customLongEdge,
                            RegistryValueKind.DWord);
        }
    }
}
