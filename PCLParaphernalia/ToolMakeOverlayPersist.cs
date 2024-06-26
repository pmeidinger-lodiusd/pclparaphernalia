﻿using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class manages persistent storage of options for the Make Overlay tool.</para>
    /// <para>© Chris Hutchinson 2012</para>
    ///
    /// </summary>
    internal static class ToolMakeOverlayPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const string _mainKey = MainForm._regMainKey;
        private const string _subKeyTools = "Tools";
        private const string _subKeyToolsMakeOverlay = _subKeyTools + @"\MakeOverlay";
        private const string _subKeyPCL5 = "PCL5";
        private const string _subKeyPCL6 = "PCL6";
        private const string _subKeyPCL = "PCL";
        private const string _subKeyPCLXL = "PCLXL";
        private const string _namePrnFilename = "PrintFilename";
        private const string _nameOvlFilename = "OverlayFilename";
        private const string _nameMacroId = "MacroId";
        private const string _nameStreamName = "StreamName";
        private const string _nameFlagEncapsulated = "FlagEncapsulated";
        private const string _nameFlagRestoreCursor = "FlagRestoreCursor";
        private const string _nameFlagRestoreGS = "FlagRestoreGS";
        private const string _nameIndxRptFileFmt = "IndxRptFileFmt";
        private const string _defaultPrnFilename = "DefaultPrintFile.prn";
        private const string _defaultOvlFilename = "DefaultOverlayFile";
        private const string _defaultStreamName = "Stream 001";
        private const int _defaultMacroId = 101;
        private const int _flagFalse = 0;
        private const int _flagTrue = 1;
        private const int _indexZero = 0;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C o m m o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored common data.                                       //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCommon(out string prnFilename)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.CreateSubKey(_subKeyToolsMakeOverlay))
                {
                    prnFilename = (string)subKey.GetValue(_namePrnFilename, defWorkFolder + "\\" + _defaultPrnFilename);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a P C L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored PCL data.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataPCL(out string ovlFilename,
                                       out bool flagRestoreCursor,
                                       out bool flagEncapsulated,
                                       out int macroId)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsMakeOverlay + "\\" + _subKeyPCL;

                int tmpInt;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    ovlFilename = (string)subKey.GetValue(_nameOvlFilename, defWorkFolder + "\\" + _defaultOvlFilename + ".ovl");

                    macroId = (int)subKey.GetValue(_nameMacroId, _defaultMacroId);

                    tmpInt = (int)subKey.GetValue(_nameFlagEncapsulated, _flagTrue);

                    flagEncapsulated = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagRestoreCursor, _flagTrue);

                    flagRestoreCursor = tmpInt != _flagFalse;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a P C L X L                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored PCLXL data.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataPCLXL(out string ovlFilename,
                                            out bool flagRestoreGS,
                                            out bool flagEncapsulated,
                                            out string streamName)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsMakeOverlay + "\\" + _subKeyPCLXL;

                int tmpInt;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    ovlFilename = (string)subKey.GetValue(_nameOvlFilename, defWorkFolder + "\\" + _defaultOvlFilename + ".ovx");

                    streamName = (string)subKey.GetValue(_nameStreamName, _defaultStreamName);

                    tmpInt = (int)subKey.GetValue(_nameFlagEncapsulated, _flagTrue);

                    flagEncapsulated = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagRestoreGS, _flagTrue);

                    flagRestoreGS = tmpInt != _flagFalse;
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
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsMakeOverlay))
                {
                    indxRptFileFmt = (int)subKey.GetValue(_nameIndxRptFileFmt, _indexZero);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a C o m m o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current common data.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataCommon(string prnFilename)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsMakeOverlay))
                {
                    if (prnFilename != null)
                    {
                        subKey.SetValue(_namePrnFilename, prnFilename, RegistryValueKind.String);
                    }
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a P C L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store PCL data.                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataPCL(string ovlFilename,
                                        bool flagRestoreCursor,
                                        bool flagEncapsulated,
                                        int macroId)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsMakeOverlay + "\\" + _subKeyPCL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    if (ovlFilename != null)
                        subKey.SetValue(_nameOvlFilename, ovlFilename, RegistryValueKind.String);

                    subKey.SetValue(_nameMacroId, macroId, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagEncapsulated, flagEncapsulated ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagRestoreCursor, flagRestoreCursor ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a P C L X L                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store PCLXL data.                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataPCLXL(string ovlFilename,
                                        bool flagRestoreGS,
                                        bool flagEncapsulated,
                                        string streamName)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsMakeOverlay + "\\" + _subKeyPCLXL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    if (ovlFilename != null)
                        subKey.SetValue(_nameOvlFilename, ovlFilename, RegistryValueKind.String);

                    subKey.SetValue(_nameFlagEncapsulated, flagEncapsulated ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagRestoreGS, flagRestoreGS ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    if (streamName != null)
                        subKey.SetValue(_nameStreamName, streamName, RegistryValueKind.String);
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
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsMakeOverlay))
                {
                    subKey.SetValue(_nameIndxRptFileFmt, indxRptFileFmt, RegistryValueKind.DWord);
                }
            }
        }
    }
}