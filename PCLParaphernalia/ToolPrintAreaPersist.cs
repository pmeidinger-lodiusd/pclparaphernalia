﻿using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class manages persistent storage of options for the PrintArea tool.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class ToolPrintAreaPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const string _mainKey = MainForm._regMainKey;
        private const string _subKeyTools = "Tools";
        private const string _subKeyToolsPrintArea = _subKeyTools + @"\PrintArea";
        private const string _subKeyPCL5 = "PCL5";
        private const string _subKeyPCL6 = "PCL6";
        private const string _subKeyPCL = "PCL";
        private const string _subKeyPCLXL = "PCLXL";
        private const string _nameCaptureFile = "CaptureFile";
        private const string _nameFlagFormAsMacro = "FlagFormAsMacro";
        private const string _nameFlagCustomUseMetric = "FlagCustomUseMetric";
        private const string _nameCustomShortEdge = "CustomShortEdge";
        private const string _nameCustomLongEdge = "CustomLongEdge";
        private const string _nameIndxOrientation = "IndxOrientation";
        private const string _nameIndxPaperSize = "IndxPaperSize";
        private const string _nameIndxPaperType = "IndxPaperType";
        private const string _nameIndxPDL = "IndxPDL";
        private const string _nameIndxPJLCommand = "IndxPJLCommand";
        private const string _nameIndxPlexMode = "IndxPlexMode";
        private const int _flagFalse = 0;
        private const int _flagTrue = 1;
        private const int _indexZero = 0;
        private const int _customShortEdgeDefault = 4960;    // A4 dots @ 600 dpi
        private const int _customLongEdgeDefault = 7014;    // A4 dots @ 600 dpi

        private const string _defaultCaptureFilePCL = "CaptureFile_PrintAreaPCL.prn";
        private const string _defaultCaptureFilePCLXL = "CaptureFile_PrintAreaPCLXL.prn";

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C a p t u r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Print Area capture file data.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCapture(ToolCommonData.PrintLang crntPDL, ref string captureFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string oldKey = _subKeyToolsPrintArea;
                string oldFile;

                bool update_from_v2_5_0_0 = false;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.OpenSubKey(oldKey, true))
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
                    const string keyPCL = _subKeyToolsPrintArea + "\\" + _subKeyPCL;

                    using (var subKey = keyMain.CreateSubKey(keyPCL))
                    {
                        subKey.SetValue(_nameCaptureFile, oldFile, RegistryValueKind.String);
                    }

                    const string keyPCLXL = _subKeyToolsPrintArea + "\\" + _subKeyPCLXL;

                    using (var subKey = keyMain.CreateSubKey(keyPCLXL))
                    {
                        subKey.SetValue(_nameCaptureFile, oldFile, RegistryValueKind.String);
                    }
                }

                if (crntPDL == ToolCommonData.PrintLang.PCL)
                {
                    const string key = _subKeyToolsPrintArea + "\\" + _subKeyPCL;

                    using (var subKey = keyMain.CreateSubKey(key))
                    {
                        captureFile = (string)subKey.GetValue(_nameCaptureFile, defWorkFolder + "\\" + _defaultCaptureFilePCL);
                    }
                }
                else if (crntPDL == ToolCommonData.PrintLang.PCLXL)
                {
                    const string key = _subKeyToolsPrintArea + "\\" + _subKeyPCLXL;

                    using (var subKey = keyMain.CreateSubKey(key))
                    {
                        captureFile = (string)subKey.GetValue(_nameCaptureFile, defWorkFolder + "\\" + _defaultCaptureFilePCLXL);
                    }
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

        public static void LoadDataCommon(out int indxPDL)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsPrintArea))
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
        // Retrieve stored PrintArea PCL or PCLXL data.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataPCL(string pdlName,
                                       out int indxOrientation,
                                       out int indxPaperSize,
                                       out int indxPaperType,
                                       out int indxPlexMode,
                                       out int indxPJLCommand,
                                       out bool flagFormAsMacro,
                                       out bool flagCustomUseMetric,
                                       out ushort customShortEdge,
                                       out ushort customLongEdge)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string key = _subKeyToolsPrintArea + "\\" + pdlName;

                int tmpInt;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    indxOrientation = (int)subKey.GetValue(_nameIndxOrientation, _indexZero);

                    indxPaperSize = (int)subKey.GetValue(_nameIndxPaperSize, _indexZero);

                    indxPaperType = (int)subKey.GetValue(_nameIndxPaperType, _indexZero);

                    indxPlexMode = (int)subKey.GetValue(_nameIndxPlexMode, _indexZero);

                    indxPJLCommand = (int)subKey.GetValue(_nameIndxPJLCommand, _indexZero);

                    tmpInt = (int)subKey.GetValue(_nameFlagFormAsMacro, _flagTrue);

                    flagFormAsMacro = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagCustomUseMetric, _flagTrue);

                    flagCustomUseMetric = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameCustomShortEdge, _customShortEdgeDefault);

                    customShortEdge = (ushort)tmpInt;

                    tmpInt = (int)subKey.GetValue(_nameCustomLongEdge, _customLongEdgeDefault);

                    customLongEdge = (ushort)tmpInt;
                }
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

        public static void SaveDataCapture(ToolCommonData.PrintLang crntPDL, string captureFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                if (crntPDL == ToolCommonData.PrintLang.PCL)
                {
                    const string key = _subKeyToolsPrintArea + "\\" + _subKeyPCL;

                    using (var subKey = keyMain.CreateSubKey(key))
                    {
                        if (captureFile != null)
                            subKey.SetValue(_nameCaptureFile, captureFile, RegistryValueKind.String);
                    }
                }
                else if (crntPDL == ToolCommonData.PrintLang.PCLXL)
                {
                    const string key = _subKeyToolsPrintArea + "\\" + _subKeyPCLXL;

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
        // Store current PrintArea common data.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataCommon(int indxPDL)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsPrintArea))
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
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string key = _subKeyToolsPrintArea + "\\" + pdlName;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameIndxOrientation, indxOrientation, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxPaperSize, indxPaperSize, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxPaperType, indxPaperType, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxPlexMode, indxPlexMode, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxPJLCommand, indxPJLCommand, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagFormAsMacro, flagFormAsMacro ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagCustomUseMetric, flagCustomUseMetric ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameCustomShortEdge, customShortEdge, RegistryValueKind.DWord);

                    subKey.SetValue(_nameCustomLongEdge, customLongEdge, RegistryValueKind.DWord);
                }
            }
        }
    }
}