using Microsoft.Win32;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class manages persistent storage of options for the FormSample tool.
/// 
/// © Chris Hutchinson 2012
/// 
/// </summary>

static class ToolFormSamplePersist
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    const string _mainKey = MainForm._regMainKey;

    const string _subKeyTools = "Tools";
    const string _subKeyToolsFormSample = "FormSample";
    const string _subKeyPCL5 = "PCL5";
    const string _subKeyPCL6 = "PCL6";
    const string _subKeyPCL = "PCL";
    const string _subKeyPCLXL = "PCLXL";

    const string _nameCaptureFile = "CaptureFile";
    const string _nameFormNameMain = "FormNameMain";
    const string _nameFormNameRear = "FormNameRear";
    const string _nameFormFileMain = "FormFileMain";
    const string _nameFormFileRear = "FormFileRear";
    const string _namePrnDiskFileMain = "PrnDiskFileMain";
    const string _namePrnDiskFileRear = "PrnDiskFileRear";
    const string _nameMacroIdMain = "MacroIdMain";
    const string _nameMacroIdRear = "MacroIdRear";
    const string _nameFlagMacroRemove = "FlagMacroRemove";
    const string _nameFlagMainForm = "FlagMainForm";
    const string _nameFlagRearForm = "FlagRearForm";
    const string _nameFlagMainOnPrnDisk = "FlagMainOnPrnDisk";
    const string _nameFlagRearOnPrnDisk = "FlagRearOnPrnDisk";
    const string _nameFlagRearBPlate = "FlagRearBPlate";
    const string _nameFlagGSPushPop = "FlagGSPushPop";
    const string _nameFlagPrintDescText = "FlagPrintDescText";
    const string _nameTestPageCount = "TestPageCount";
    const string _nameIndxMethod = "IndxMethod";
    const string _nameIndxOrientation = "IndxOrientation";
    const string _nameIndxOrientRear = "IndxOrientationRear";
    const string _nameIndxPaperSize = "IndxPaperSize";
    const string _nameIndxPaperType = "IndxPaperType";
    const string _nameIndxPDL = "IndxPDL";
    const string _nameIndxPlexMode = "IndxPlexMode";

    const int _flagFalse = 0;
    const int _flagTrue = 1;
    const int _indexZero = 0;

    const string _defaultCaptureFile = "Capture_FormSample.prn";
    const string _defaultCaptureFilePCL = "CaptureFile_FormSamplePCL.prn";
    const string _defaultCaptureFilePCLXL = "CaptureFile_FormSamplePCLXL.prn";
    const string _defaultFilePCLMain = "DefaultFilePCLMain.ovl";
    const string _defaultFilePCLRear = "DefaultFilePCLRear.ovl";
    const string _defaultFilePCLXLMain = "DefaultFilePCLXLMain.ovx";
    const string _defaultFilePCLXLRear = "DefaultFilePCLXLRear.ovx";
    const string _defaultFormNameMain = "TestFormMain";
    const string _defaultFormNameRear = "TestFormRear";

    const int _defaultMacroIdMain = 32767;
    const int _defaultMacroIdRear = 32766;
    const int _defaultTestPageCount = 3;

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a C a p t u r e                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored FormSample capture file data.                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataCapture(ToolCommonData.ePrintLang crntPDL,
                                        ref string captureFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string oldKey = _subKeyTools + "\\" + _subKeyToolsFormSample;
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
                            "\\" + _subKeyToolsFormSample +
                            "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(keyPCL))
            {
                subKey.SetValue(_nameCaptureFile,
                                 oldFile,
                                 RegistryValueKind.String);
            }

            string keyPCLXL = _subKeyTools +
                              "\\" + _subKeyToolsFormSample +
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
            string key = _subKeyTools + "\\" + _subKeyToolsFormSample +
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
            string key = _subKeyTools + "\\" + _subKeyToolsFormSample +
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
    // Retrieve stored FormSample common data.                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataCommon(ref int indxPDL)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsFormSample;

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
    // l o a d D a t a G e n e r a l                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored FormSample PCL or PCL XL general data.             //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataGeneral(
        string pdlName,
        ref int indxPaperType,
        ref int indxPaperSize,
        ref int indxOrientation,
        ref int indxPlexMode,
        ref int indxOrientRear,
        ref int indxMethod,
        ref int testPageCount,
        ref bool flagMacroRemove,
        ref bool flagMainForm,
        ref bool flagRearForm,
        ref bool flagRearBPlate,
        ref bool flagPrintDescText)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        int tmpInt;

        key = _subKeyTools + "\\" + _subKeyToolsFormSample +
                             "\\" + pdlName;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            indxPaperType = (int)subKey.GetValue(_nameIndxPaperType,
                                                     _indexZero);
            indxPaperSize = (int)subKey.GetValue(_nameIndxPaperSize,
                                                     _indexZero);
            indxOrientation = (int)subKey.GetValue(_nameIndxOrientation,
                                                     _indexZero);
            indxPlexMode = (int)subKey.GetValue(_nameIndxPlexMode,
                                                     _indexZero);
            indxOrientRear = (int)subKey.GetValue(_nameIndxOrientRear,
                                                     _indexZero);
            indxMethod = (int)subKey.GetValue(_nameIndxMethod,
                                                     _indexZero);
            testPageCount = (int)subKey.GetValue(_nameTestPageCount,
                                                     _defaultTestPageCount);

            tmpInt = (int)subKey.GetValue(_nameFlagMacroRemove,
                                              _flagTrue);

            flagMacroRemove = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagMainForm,
                                              _flagTrue);

            flagMainForm = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagRearForm,
                                              _flagTrue);

            flagRearForm = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagRearBPlate,
                                              _flagTrue);

            flagRearBPlate = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagPrintDescText,
                                              _flagTrue);

            flagPrintDescText = tmpInt != _flagFalse;
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a P C L                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored FormSample PCL form data.                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPCL(ref bool flagMainOnPrnDisk,
                                    ref bool flagRearOnPrnDisk,
                                    ref string formFileMain,
                                    ref string formFileRear,
                                    ref string prnDiskFileMain,
                                    ref string prnDiskFileRear,
                                    ref int macroIdMain,
                                    ref int macroIdRear)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

        int tmpInt;

        key = _subKeyTools + "\\" + _subKeyToolsFormSample +
                             "\\" + _subKeyPCL;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            tmpInt = (int)subKey.GetValue(_nameFlagMainOnPrnDisk,
                                            _flagFalse);

            flagMainOnPrnDisk = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagRearOnPrnDisk,
                                            _flagFalse);

            flagRearOnPrnDisk = tmpInt != _flagFalse;

            formFileMain = (string)subKey.GetValue(_nameFormFileMain,
                                                      defWorkFolder + "\\" +
                                                      _defaultFilePCLMain);
            formFileRear = (string)subKey.GetValue(_nameFormFileRear,
                                                      defWorkFolder + "\\" +
                                                      _defaultFilePCLRear);

            prnDiskFileMain = (string)subKey.GetValue(_namePrnDiskFileMain,
                                                        _defaultFilePCLMain);
            prnDiskFileRear = (string)subKey.GetValue(_namePrnDiskFileRear,
                                                        _defaultFilePCLRear);

            macroIdMain = (int)subKey.GetValue(_nameMacroIdMain,
                                                     _defaultMacroIdMain);
            macroIdRear = (int)subKey.GetValue(_nameMacroIdRear,
                                                     _defaultMacroIdRear);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a P C L X L                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored FormSample PCL XL form data.                       //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPCLXL(ref string formFileMain,
                                      ref string formFileRear,
                                      ref string formNameMain,
                                      ref string formNameRear,
                                      ref bool flagGSPushPop)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        int tmpInt;

        string key;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

        key = _subKeyTools + "\\" + _subKeyToolsFormSample +
                             "\\" + _subKeyPCLXL;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            formFileMain = (string)subKey.GetValue(_nameFormFileMain,
                                                     defWorkFolder + "\\" +
                                                     _defaultFilePCLXLMain);
            formFileRear = (string)subKey.GetValue(_nameFormFileRear,
                                                     defWorkFolder + "\\" +
                                                     _defaultFilePCLXLRear);
            formNameMain = (string)subKey.GetValue(_nameFormNameMain,
                                                     _defaultFormNameMain);
            formNameRear = (string)subKey.GetValue(_nameFormNameRear,
                                                     _defaultFormNameRear);

            tmpInt = (int)subKey.GetValue(_nameFlagGSPushPop,
                                              _flagTrue);

            flagGSPushPop = tmpInt != _flagFalse;
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a C a p t u r e                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current FormSample capture file data.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataCapture(ToolCommonData.ePrintLang crntPDL,
                                        string captureFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        if (crntPDL == ToolCommonData.ePrintLang.PCL)
        {
            string key = _subKeyTools + "\\" + _subKeyToolsFormSample +
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
            string key = _subKeyTools + "\\" + _subKeyToolsFormSample +
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
    // Store current FormSample common data.                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataCommon(int indxPDL)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsFormSample;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxPDL,
                            indxPDL,
                            RegistryValueKind.DWord);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a G e n e r a l                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current FormSample PCL or PCL XL general data.               //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataGeneral(string pdlName,
                                        int indxPaperType,
                                        int indxPaperSize,
                                        int indxOrientation,
                                        int indxPlexMode,
                                        int indxOrientRear,
                                        int indxMethod,
                                        int testPageCount,
                                        bool flagMacroRemove,
                                        bool flagMainForm,
                                        bool flagRearForm,
                                        bool flagRearBPlate,
                                        bool flagPrintDescText)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsFormSample +
                             "\\" + pdlName;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxPaperType,
                            indxPaperType,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxPaperSize,
                            indxPaperSize,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxOrientation,
                            indxOrientation,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxPlexMode,
                            indxPlexMode,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxOrientRear,
                            indxOrientRear,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxMethod,
                            indxMethod,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameTestPageCount,
                            testPageCount,
                            RegistryValueKind.DWord);

            if (flagMacroRemove)
                subKey.SetValue(_nameFlagMacroRemove, _flagTrue, RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagMacroRemove, _flagFalse, RegistryValueKind.DWord);

            if (flagMainForm)
                subKey.SetValue(_nameFlagMainForm, _flagTrue, RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagMainForm, _flagFalse, RegistryValueKind.DWord);

            if (flagRearForm)
                subKey.SetValue(_nameFlagRearForm, _flagTrue, RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagRearForm, _flagFalse, RegistryValueKind.DWord);

            if (flagRearBPlate)
                subKey.SetValue(_nameFlagRearBPlate, _flagTrue, RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagRearBPlate, _flagFalse, RegistryValueKind.DWord);

            if (flagPrintDescText)
                subKey.SetValue(_nameFlagPrintDescText, _flagTrue, RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagPrintDescText, _flagFalse, RegistryValueKind.DWord);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a P C L                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store FormSample PCL form data.                                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataPCL(bool flagMainOnPrnDisk,
                                    bool flagRearOnPrnDisk,
                                    string formFileMain,
                                    string formFileRear,
                                    string prnDiskFileMain,
                                    string prnDiskFileRear,
                                    int macroIdMain,
                                    int macroIdRear)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsFormSample +
                             "\\" + _subKeyPCL;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (flagMainOnPrnDisk)
                subKey.SetValue(_nameFlagMainOnPrnDisk, _flagTrue, RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagMainOnPrnDisk, _flagFalse, RegistryValueKind.DWord);

            if (flagRearOnPrnDisk)
                subKey.SetValue(_nameFlagRearOnPrnDisk, _flagTrue, RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagRearOnPrnDisk, _flagFalse, RegistryValueKind.DWord);

            if (formFileMain != null)
            {
                subKey.SetValue(_nameFormFileMain, formFileMain, RegistryValueKind.String);
            }

            if (formFileRear != null)
            {
                subKey.SetValue(_nameFormFileRear, formFileRear, RegistryValueKind.String);
            }

            if (prnDiskFileMain != null)
            {
                subKey.SetValue(_namePrnDiskFileMain, prnDiskFileMain, RegistryValueKind.String);
            }

            if (prnDiskFileRear != null)
            {
                subKey.SetValue(_namePrnDiskFileRear, prnDiskFileRear, RegistryValueKind.String);
            }

            subKey.SetValue(_nameMacroIdMain,
                            macroIdMain,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameMacroIdRear,
                            macroIdRear,
                            RegistryValueKind.DWord);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a P C L X L                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store FormSample PCL XL form data.                                 //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataPCLXL(string formFileMain,
                                      string formFileRear,
                                      string formNameMain,
                                      string formNameRear,
                                      bool flagGSPushPop)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key;

        key = _subKeyTools + "\\" + _subKeyToolsFormSample +
                             "\\" + _subKeyPCLXL;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            if (formFileMain != null)
            {
                subKey.SetValue(_nameFormFileMain,
                                formFileMain,
                                RegistryValueKind.String);
            }

            if (formFileRear != null)
            {
                subKey.SetValue(_nameFormFileRear,
                                formFileRear,
                                RegistryValueKind.String);
            }

            if (formNameMain != null)
            {
                subKey.SetValue(_nameFormNameMain,
                                formNameMain,
                                RegistryValueKind.String);
            }

            if (formNameRear != null)
            {
                subKey.SetValue(_nameFormNameRear,
                                formNameRear,
                                RegistryValueKind.String);
            }

            if (flagGSPushPop)
                subKey.SetValue(_nameFlagGSPushPop,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagGSPushPop,
                                _flagFalse,
                                RegistryValueKind.DWord);
        }
    }
}
