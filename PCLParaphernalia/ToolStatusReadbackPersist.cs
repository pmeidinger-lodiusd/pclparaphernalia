﻿using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class manages persistent storage of options for the StatusReadback tool.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class ToolStatusReadbackPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const string _mainKey = MainForm._regMainKey;
        private const string _subKeyTools = "Tools";
        private const string _subKeyToolsStatusReadback = _subKeyTools + @"\StatusReadback";
        private const string _subKeyPCL = "PCL";
        private const string _subKeyPJL = "PJL";
        private const string _subKeyPJLFS = "PJLFS";
        private const string _nameBinSrcFile = "BinSrcFile";
        private const string _nameBinTgtFile = "BinTgtFile";
        private const string _nameCaptureFile = "CaptureFile";
        private const string _nameCustomCat = "CustomCat";
        private const string _nameCustomVar = "CustomVar";
        private const string _nameObjectPath = "ObjectPath";
        private const string _nameReportFile = "ReportFile";
        private const string _nameFlagPJLFS = "flagPJLFS";
        private const string _nameFlagPJLFSSecJob = "flagPJLFSSecJob";
        private const string _nameIndxCategory = "IndxCategory";
        private const string _nameIndxCommand = "IndxCommand";
        private const string _nameIndxEntityType = "IndxEntityType";
        private const string _nameIndxLocationType = "IndxLocationType";
        private const string _nameIndxPDL = "IndxPDL";
        private const string _nameIndxRptFileFmt = "IndxRptFileFmt";
        private const string _nameIndxVariable = "IndxVariable";
        private const int _flagFalse = 0;
        private const int _flagTrue = 1;
        private const int _indexZero = 0;
        private const string _defaultCaptureFilePCL = "CaptureFile_StatusReadbackPCL.prn";
        private const string _defaultCaptureFilePJL = "CaptureFile_StatusReadbackPJL.prn";
        private const string _defaultReportFilePCL = "ReportFile_StatusReadbackPCL.txt";
        private const string _defaultReportFilePJL = "ReportFile_StatusReadbackPJL.txt";
        private const string _defaultCustomCatPJL = "CUSTOM_CAT_1";
        private const string _defaultCustomVarPJL = "CUSTOM_VAR_1";
        private const string _defaultBinSrcFilePJLFS = "BinSrcFile_PJLFS.pcl";
        private const string _defaultBinTgtFilePJLFS = "BinTgtFile_PJLFS.pcl";
        private const string _defaultObjectPathPJLFS = "0:\\pcl\\macros\\macro1";

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C a p t u r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Status Readback capture file data.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCapture(ToolCommonData.PrintLang crntPDL, ref string captureFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string oldFile;

                bool update_from_v2_5_0_0 = false;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.OpenSubKey(_subKeyToolsStatusReadback, true))
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
                    const string keyPCL = _subKeyToolsStatusReadback + "\\" + _subKeyPCL;

                    using (var subKey = keyMain.CreateSubKey(keyPCL))
                    {
                        subKey.SetValue(_nameCaptureFile, oldFile, RegistryValueKind.String);
                    }

                    const string keyPJL = _subKeyToolsStatusReadback + "\\" + _subKeyPJL;

                    using (var subKey = keyMain.CreateSubKey(keyPJL))
                    {
                        subKey.SetValue(_nameCaptureFile, oldFile, RegistryValueKind.String);
                    }
                }

                if (crntPDL == ToolCommonData.PrintLang.PCL)
                {
                    const string key = _subKeyToolsStatusReadback + "\\" + _subKeyPCL;

                    using (var subKey = keyMain.CreateSubKey(key))
                    {
                        captureFile = (string)subKey.GetValue(_nameCaptureFile, defWorkFolder + "\\" + _defaultCaptureFilePCL);
                    }
                }
                else if (crntPDL == ToolCommonData.PrintLang.PJL)
                {
                    const string key = _subKeyToolsStatusReadback + "\\" + _subKeyPJL;

                    using (var subKey = keyMain.CreateSubKey(key))
                    {
                        captureFile = (string)subKey.GetValue(_nameCaptureFile, defWorkFolder + "\\" + _defaultCaptureFilePJL);
                    }
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

        public static void LoadDataCommon(out int indxPDL)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsStatusReadback))
                {
                    indxPDL = (int)subKey.GetValue(_nameIndxPDL, _indexZero);
                }
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

        public static void LoadDataPCL(out int indxEntityType,
                                       out int indxLocationType,
                                       out string reportFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string key;

                string oldFile;

                bool update_from_v2_5_0_0 = false;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.OpenSubKey(_subKeyToolsStatusReadback, true))
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
                    const string keyPCL = _subKeyToolsStatusReadback + "\\" + _subKeyPCL;

                    using (var subKey = keyMain.CreateSubKey(keyPCL))
                    {
                        subKey.SetValue(_nameReportFile, oldFile, RegistryValueKind.String);
                    }

                    const string keyPJL = _subKeyToolsStatusReadback + "\\" + _subKeyPJL;

                    using (var subKey = keyMain.CreateSubKey(keyPJL))
                    {
                        subKey.SetValue(_nameReportFile, oldFile, RegistryValueKind.String);
                    }
                }

                key = _subKeyToolsStatusReadback + "\\" + _subKeyPCL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    indxEntityType = (int)subKey.GetValue(_nameIndxEntityType, _indexZero);

                    indxLocationType = (int)subKey.GetValue(_nameIndxLocationType, _indexZero);

                    reportFile = (string)subKey.GetValue(_nameReportFile, defWorkFolder + "\\" + _defaultReportFilePCL);
                }
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

        public static void LoadDataPJL(out int indxCategory,
                                       out int indxCommand,
                                       out int indxVariable,
                                       out string customCat,
                                       out string customVar,
                                       out string reportFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string key;

                string oldFile;

                bool update_from_v2_5_0_0 = false;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.OpenSubKey(_subKeyToolsStatusReadback, true))
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
                    const string keyPCL = _subKeyToolsStatusReadback + "\\" + _subKeyPCL;

                    using (var subKey = keyMain.CreateSubKey(keyPCL))
                    {
                        subKey.SetValue(_nameReportFile, oldFile, RegistryValueKind.String);
                    }

                    const string keyPJL = _subKeyToolsStatusReadback + "\\" + _subKeyPJL;

                    using (var subKey = keyMain.CreateSubKey(keyPJL))
                    {
                        subKey.SetValue(_nameReportFile, oldFile, RegistryValueKind.String);
                    }
                }

                key = _subKeyToolsStatusReadback + "\\" + _subKeyPJL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    indxCategory = (int)subKey.GetValue(_nameIndxCategory, _indexZero);

                    indxCommand = (int)subKey.GetValue(_nameIndxCommand, _indexZero);

                    indxVariable = (int)subKey.GetValue(_nameIndxVariable, _indexZero);

                    customCat = (string)subKey.GetValue(_nameCustomCat, _defaultCustomCatPJL);

                    customVar = (string)subKey.GetValue(_nameCustomVar, _defaultCustomVarPJL);

                    reportFile = (string)subKey.GetValue(_nameReportFile, defWorkFolder + "\\" + _defaultReportFilePJL);
                }
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

        public static void LoadDataPJLFS(out int indxCommand,
                                         out string objectPath,
                                         out string binSrcFile,
                                         out string binTgtFile,
                                         out bool flagPJLFS,
                                         out bool flagPJLFSSecJob)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsStatusReadback + "\\" + _subKeyPJLFS;

                int tmpInt;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    indxCommand = (int)subKey.GetValue(_nameIndxCommand, _indexZero);

                    objectPath = (string)subKey.GetValue(_nameObjectPath, _defaultObjectPathPJLFS);

                    binSrcFile = (string)subKey.GetValue(_nameBinSrcFile, _defaultBinSrcFilePJLFS);

                    binTgtFile = (string)subKey.GetValue(_nameBinTgtFile, _defaultBinTgtFilePJLFS);

                    tmpInt = (int)subKey.GetValue(_nameFlagPJLFS, _flagFalse);

                    flagPJLFS = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagPJLFSSecJob, _flagFalse);

                    flagPJLFSSecJob = tmpInt != _flagFalse;
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
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsStatusReadback))
                {
                    indxRptFileFmt = (int)subKey.GetValue(_nameIndxRptFileFmt, _indexZero);
                }
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

        public static void SaveDataCapture(ToolCommonData.PrintLang crntPDL, string captureFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                if (crntPDL == ToolCommonData.PrintLang.PCL)
                {
                    const string key = _subKeyToolsStatusReadback + "\\" + _subKeyPCL;

                    using (var subKey = keyMain.CreateSubKey(key))
                    {
                        if (captureFile != null)
                            subKey.SetValue(_nameCaptureFile, captureFile, RegistryValueKind.String);
                    }
                }
                else if (crntPDL == ToolCommonData.PrintLang.PJL)
                {
                    const string key = _subKeyToolsStatusReadback + "\\" + _subKeyPJL;

                    using (var subKey = keyMain.CreateSubKey(key))
                    {
                        if (captureFile != null)
                            subKey.SetValue(_nameCaptureFile, captureFile, RegistryValueKind.String);
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
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsStatusReadback))
                {
                    subKey.SetValue(_nameIndxPDL, indxPDL, RegistryValueKind.DWord);
                }
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

        public static void SaveDataPCL(int indxEntityType, int indxLocType, string reportFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsStatusReadback + "\\" + _subKeyPCL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameIndxEntityType, indxEntityType, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxLocationType, indxLocType, RegistryValueKind.DWord);

                    if (reportFile != null)
                        subKey.SetValue(_nameReportFile, reportFile, RegistryValueKind.String);
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
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsStatusReadback + "\\" + _subKeyPJL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameIndxCategory, indxCategory, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxCommand, indxCommand, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxVariable, indxVariable, RegistryValueKind.DWord);

                    if (customCat != null)
                        subKey.SetValue(_nameCustomCat, customCat, RegistryValueKind.String);

                    if (customVar != null)
                        subKey.SetValue(_nameCustomVar, customVar, RegistryValueKind.String);

                    if (reportFile != null)
                        subKey.SetValue(_nameReportFile, reportFile, RegistryValueKind.String);
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
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsStatusReadback + "\\" + _subKeyPJLFS;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameIndxCommand, indxCommand, RegistryValueKind.DWord);

                    if (objectPath != null)
                        subKey.SetValue(_nameObjectPath, objectPath, RegistryValueKind.String);

                    if (binSrcFile != null)
                        subKey.SetValue(_nameBinSrcFile, binSrcFile, RegistryValueKind.String);

                    if (binTgtFile != null)
                        subKey.SetValue(_nameBinTgtFile, binTgtFile, RegistryValueKind.String);

                    subKey.SetValue(_nameFlagPJLFS, flagPJLFS ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagPJLFSSecJob, flagPJLFSSecJob ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
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
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsStatusReadback))
                {
                    subKey.SetValue(_nameIndxRptFileFmt, indxRptFileFmt, RegistryValueKind.DWord);
                }
            }
        }
    }
}