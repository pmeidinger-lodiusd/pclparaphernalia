using Microsoft.Win32;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class manages persistent storage of options for the StatusReadback tool.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

static class ToolStatusReadbackPersist
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    const string _mainKey = MainForm._regMainKey;

    const string _subKeyTools = "Tools";
    const string _subKeyToolsStatusReadback = "StatusReadback";
    const string _subKeyPCL = "PCL";
    const string _subKeyPJL = "PJL";
    const string _subKeyPJLFS = "PJLFS";

    const string _nameBinSrcFile = "BinSrcFile";
    const string _nameBinTgtFile = "BinTgtFile";
    const string _nameCaptureFile = "CaptureFile";
    const string _nameCustomCat = "CustomCat";
    const string _nameCustomVar = "CustomVar";
    const string _nameObjectPath = "ObjectPath";
    const string _nameReportFile = "ReportFile";
    const string _nameFlagPJLFS = "flagPJLFS";
    const string _nameFlagPJLFSSecJob = "flagPJLFSSecJob";
    const string _nameIndxCategory = "IndxCategory";
    const string _nameIndxCommand = "IndxCommand";
    const string _nameIndxEntityType = "IndxEntityType";
    const string _nameIndxLocationType = "IndxLocationType";
    const string _nameIndxPDL = "IndxPDL";
    const string _nameIndxRptFileFmt = "IndxRptFileFmt";
    const string _nameIndxVariable = "IndxVariable";

    const int _flagFalse = 0;
    const int _flagTrue = 1;
    const int _indexZero = 0;

    const string _defaultCaptureFilePCL = "CaptureFile_StatusReadbackPCL.prn";
    const string _defaultCaptureFilePJL = "CaptureFile_StatusReadbackPJL.prn";

    const string _defaultReportFilePCL = "ReportFile_StatusReadbackPCL.txt";
    const string _defaultReportFilePJL = "ReportFile_StatusReadbackPJL.txt";

    const string _defaultCustomCatPJL = "CUSTOM_CAT_1";
    const string _defaultCustomVarPJL = "CUSTOM_VAR_1";

    const string _defaultBinSrcFilePJLFS = "BinSrcFile_PJLFS.pcl";
    const string _defaultBinTgtFilePJLFS = "BinTgtFile_PJLFS.pcl";
    const string _defaultObjectPathPJLFS = "0:\\pcl\\macros\\macro1";

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a C a p t u r e                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored Status Readback capture file data.                 //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataCapture(ToolCommonData.ePrintLang crntPDL,
                                        ref string captureFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string oldKey = _subKeyTools + "\\" + _subKeyToolsStatusReadback;
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
                             "\\" + _subKeyToolsStatusReadback +
                             "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(keyPCL))
            {
                subKey.SetValue(_nameCaptureFile,
                                 oldFile,
                                 RegistryValueKind.String);
            }

            string keyPJL = _subKeyTools +
                             "\\" + _subKeyToolsStatusReadback +
                             "\\" + _subKeyPJL;

            using (RegistryKey subKey = keyMain.CreateSubKey(keyPJL))
            {
                subKey.SetValue(_nameCaptureFile,
                                 oldFile,
                                 RegistryValueKind.String);
            }
        }

        if (crntPDL == ToolCommonData.ePrintLang.PCL)
        {
            string key = _subKeyTools + "\\" + _subKeyToolsStatusReadback +
                                        "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                captureFile = (string)subKey.GetValue(
                    _nameCaptureFile,
                    defWorkFolder + "\\" + _defaultCaptureFilePCL);
            }
        }
        else if (crntPDL == ToolCommonData.ePrintLang.PJL)
        {
            string key = _subKeyTools + "\\" + _subKeyToolsStatusReadback +
                                        "\\" + _subKeyPJL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                captureFile = (string)subKey.GetValue(
                    _nameCaptureFile,
                    defWorkFolder + "\\" + _defaultCaptureFilePJL);
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a C o m m o n                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored StatusReadback common data.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataCommon(ref int indxPDL)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsStatusReadback;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            indxPDL = (int)subKey.GetValue(_nameIndxPDL,
                                             _indexZero);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a P C L                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored StatusReadback PCL data.                           //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPCL(ref int indxEntityType,
                                   ref int indxLocationType,
                                   ref string reportFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string oldKey = _subKeyTools + "\\" + _subKeyToolsStatusReadback;
        string oldFile;

        bool update_from_v2_5_0_0 = false;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

        using (RegistryKey subKey = keyMain.OpenSubKey(oldKey, true))
        {
            oldFile = (string)subKey.GetValue(_nameReportFile);

            if (oldFile != null)
            {
                update_from_v2_5_0_0 = true;

                subKey.DeleteValue(_nameReportFile);
            }
        }

        if (update_from_v2_5_0_0)
        {
            string keyPCL = _subKeyTools +
                             "\\" + _subKeyToolsStatusReadback +
                             "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(keyPCL))
            {
                subKey.SetValue(_nameReportFile,
                                 oldFile,
                                 RegistryValueKind.String);
            }

            string keyPJL = _subKeyTools +
                             "\\" + _subKeyToolsStatusReadback +
                             "\\" + _subKeyPJL;

            using (RegistryKey subKey = keyMain.CreateSubKey(keyPJL))
            {
                subKey.SetValue(_nameReportFile,
                                 oldFile,
                                 RegistryValueKind.String);
            }
        }

        string key = _subKeyTools + "\\" + _subKeyToolsStatusReadback +
                     "\\" + _subKeyPCL;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            indxEntityType = (int)subKey.GetValue(_nameIndxEntityType,
                                                    _indexZero);

            indxLocationType = (int)subKey.GetValue(_nameIndxLocationType,
                                                      _indexZero);

            reportFile = (string)subKey.GetValue(_nameReportFile,
                                                  defWorkFolder + "\\" +
                                                  _defaultReportFilePCL);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a P J L                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored StatusReadback PJL data.                           //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPJL(ref int indxCategory,
                                   ref int indxCommand,
                                   ref int indxVariable,
                                   ref string customCat,
                                   ref string customVar,
                                   ref string reportFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string oldKey = _subKeyTools + "\\" + _subKeyToolsStatusReadback;
        string oldFile;

        bool update_from_v2_5_0_0 = false;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

        using (RegistryKey subKey = keyMain.OpenSubKey(oldKey, true))
        {
            oldFile = (string)subKey.GetValue(_nameReportFile);

            if (oldFile != null)
            {
                update_from_v2_5_0_0 = true;

                subKey.DeleteValue(_nameCaptureFile);
            }
        }

        if (update_from_v2_5_0_0)
        {
            string keyPCL = _subKeyTools +
                             "\\" + _subKeyToolsStatusReadback +
                             "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(keyPCL))
            {
                subKey.SetValue(_nameReportFile,
                                 oldFile,
                                 RegistryValueKind.String);
            }

            string keyPJL = _subKeyTools +
                             "\\" + _subKeyToolsStatusReadback +
                             "\\" + _subKeyPJL;

            using (RegistryKey subKey = keyMain.CreateSubKey(keyPJL))
            {
                subKey.SetValue(_nameReportFile,
                                 oldFile,
                                 RegistryValueKind.String);
            }
        }

        string key = _subKeyTools + "\\" + _subKeyToolsStatusReadback +
                     "\\" + _subKeyPJL;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            indxCategory = (int)subKey.GetValue(_nameIndxCategory,
                                                  _indexZero);

            indxCommand = (int)subKey.GetValue(_nameIndxCommand,
                                                  _indexZero);

            indxVariable = (int)subKey.GetValue(_nameIndxVariable,
                                                  _indexZero);

            customCat = (string)subKey.GetValue(_nameCustomCat,
                                                 _defaultCustomCatPJL);

            customVar = (string)subKey.GetValue(_nameCustomVar,
                                                 _defaultCustomVarPJL);

            reportFile = (string)subKey.GetValue(_nameReportFile,
                                                  defWorkFolder + "\\" +
                                                  _defaultReportFilePJL);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o a d D a t a P J L F S                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored StatusReadback PJL File System data.               //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void LoadDataPJLFS(ref int indxCommand,
                                      ref string objectPath,
                                      ref string binSrcFile,
                                      ref string binTgtFile,
                                      ref bool flagPJLFS,
                                      ref bool flagPJLFSSecJob)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        int tmpInt;

        string defWorkFolder = ToolCommonData.DefWorkFolder;

        string key = _subKeyTools + "\\" + _subKeyToolsStatusReadback +
                     "\\" + _subKeyPJLFS;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            indxCommand = (int)subKey.GetValue(_nameIndxCommand,
                                                  _indexZero);

            objectPath = (string)subKey.GetValue(_nameObjectPath,
                                                   _defaultObjectPathPJLFS);

            binSrcFile = (string)subKey.GetValue(_nameBinSrcFile,
                                                   _defaultBinSrcFilePJLFS);

            binTgtFile = (string)subKey.GetValue(_nameBinTgtFile,
                                                   _defaultBinTgtFilePJLFS);

            tmpInt = (int)subKey.GetValue(_nameFlagPJLFS,
                                              _flagFalse);

            flagPJLFS = tmpInt != _flagFalse;

            tmpInt = (int)subKey.GetValue(_nameFlagPJLFSSecJob,
                                              _flagFalse);

            flagPJLFSSecJob = tmpInt != _flagFalse;
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

        string key = _subKeyTools + "\\" + _subKeyToolsStatusReadback;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            indxRptFileFmt = (int)subKey.GetValue(_nameIndxRptFileFmt,
                                                     _indexZero);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a C a p t u r e                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current Status Readback capture file data.                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataCapture(ToolCommonData.ePrintLang crntPDL,
                                        string captureFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        if (crntPDL == ToolCommonData.ePrintLang.PCL)
        {
            string key = _subKeyTools + "\\" + _subKeyToolsStatusReadback +
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
        else if (crntPDL == ToolCommonData.ePrintLang.PJL)
        {
            string key = _subKeyTools + "\\" + _subKeyToolsStatusReadback +
                                        "\\" + _subKeyPJL;

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
    // Store current StatusReadback common data.                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataCommon(int indxPDL)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsStatusReadback;

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
    // Store current StatusReadback PCL data.                             //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataPCL(int indxEntityType,
                                   int indxLocType,
                                   string reportFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsStatusReadback +
                             "\\" + _subKeyPCL;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxEntityType,
                            indxEntityType,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxLocationType,
                            indxLocType,
                            RegistryValueKind.DWord);

            if (reportFile != null)
            {
                subKey.SetValue(_nameReportFile,
                                reportFile,
                                RegistryValueKind.String);
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a P J L                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current StatusReadback PJL data.                             //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataPJL(int indxCategory,
                                   int indxCommand,
                                   int indxVariable,
                                   string customCat,
                                   string customVar,
                                   string reportFile)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsStatusReadback +
                             "\\" + _subKeyPJL;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxCategory,
                            indxCategory,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxCommand,
                            indxCommand,
                            RegistryValueKind.DWord);

            subKey.SetValue(_nameIndxVariable,
                            indxVariable,
                            RegistryValueKind.DWord);

            if (customCat != null)
            {
                subKey.SetValue(_nameCustomCat,
                                customCat,
                                RegistryValueKind.String);
            }

            if (customVar != null)
            {
                subKey.SetValue(_nameCustomVar,
                                customVar,
                                RegistryValueKind.String);
            }

            if (reportFile != null)
            {
                subKey.SetValue(_nameReportFile,
                                reportFile,
                                RegistryValueKind.String);
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s a v e D a t a P J L F S                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current StatusReadback PJL File System data.                 //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SaveDataPJLFS(int indxCommand,
                                      string objectPath,
                                      string binSrcFile,
                                      string binTgtFile,
                                      bool flagPJLFS,
                                      bool flagPJLFSSecJob)
    {
        RegistryKey keyMain =
            Registry.CurrentUser.CreateSubKey(_mainKey);

        string key = _subKeyTools + "\\" + _subKeyToolsStatusReadback +
                             "\\" + _subKeyPJLFS;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxCommand,
                            indxCommand,
                            RegistryValueKind.DWord);

            if (objectPath != null)
            {
                subKey.SetValue(_nameObjectPath,
                                objectPath,
                                RegistryValueKind.String);
            }

            if (binSrcFile != null)
            {
                subKey.SetValue(_nameBinSrcFile,
                                binSrcFile,
                                RegistryValueKind.String);
            }

            if (binTgtFile != null)
            {
                subKey.SetValue(_nameBinTgtFile,
                                binTgtFile,
                                RegistryValueKind.String);
            }

            if (flagPJLFS)
                subKey.SetValue(_nameFlagPJLFS,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagPJLFS,
                                _flagFalse,
                                RegistryValueKind.DWord);

            if (flagPJLFSSecJob)
                subKey.SetValue(_nameFlagPJLFSSecJob,
                                _flagTrue,
                                RegistryValueKind.DWord);
            else
                subKey.SetValue(_nameFlagPJLFSSecJob,
                                _flagFalse,
                                RegistryValueKind.DWord);
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

        string key = _subKeyTools + "\\" + _subKeyToolsStatusReadback;

        using (RegistryKey subKey = keyMain.CreateSubKey(key))
        {
            subKey.SetValue(_nameIndxRptFileFmt,
                            indxRptFileFmt,
                            RegistryValueKind.DWord);
        }
    }
}
