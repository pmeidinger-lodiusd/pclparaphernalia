using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class manages persistent storage of options for the Make Overlay tool.
    /// 
    /// © Chris Hutchinson 2012
    /// 
    /// </summary>

    static class ToolMakeOverlayPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        const string _mainKey = MainForm._regMainKey;

        const string _subKeyTools = "Tools";
        const string _subKeyToolsMakeOverlay = "MakeOverlay";
        const string _subKeyPCL5 = "PCL5";
        const string _subKeyPCL6 = "PCL6";
        const string _subKeyPCL = "PCL";
        const string _subKeyPCLXL = "PCLXL";

        const string _namePrnFilename = "PrintFilename";
        const string _nameOvlFilename = "OverlayFilename";
        const string _nameMacroId = "MacroId";
        const string _nameStreamName = "StreamName";
        const string _nameFlagEncapsulated = "FlagEncapsulated";
        const string _nameFlagRestoreCursor = "FlagRestoreCursor";
        const string _nameFlagRestoreGS = "FlagRestoreGS";
        const string _nameIndxRptFileFmt = "IndxRptFileFmt";

        const string _defaultPrnFilename = "DefaultPrintFile.prn";
        const string _defaultOvlFilename = "DefaultOverlayFile";
        const string _defaultStreamName = "Stream 001";

        const int _defaultMacroId = 101;

        const int _flagFalse = 0;
        const int _flagTrue = 1;
        const int _indexZero = 0;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C o m m o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored common data.                                       //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCommon(ref string prnFilename)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsMakeOverlay;

            string defWorkFolder = ToolCommonData.DefWorkFolder;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (Helper_RegKey.KeyExists(subKey, _subKeyPCL5))
                    // update from v2_5_0_0
                    Helper_RegKey.RenameKey(subKey, _subKeyPCL5, _subKeyPCL);

                if (Helper_RegKey.KeyExists(subKey, _subKeyPCL6))
                    // update from v2_5_0_0
                    Helper_RegKey.RenameKey(subKey, _subKeyPCL6, _subKeyPCLXL);

                prnFilename = (string)subKey.GetValue(_namePrnFilename,
                                                      defWorkFolder + "\\" +
                                                      _defaultPrnFilename);
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

        public static void LoadDataPCL(ref string ovlFilename,
                                        ref bool flagRestoreCursor,
                                        ref bool flagEncapsulated,
                                        ref int macroId)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            int tmpInt;

            string key;

            string defWorkFolder = ToolCommonData.DefWorkFolder;

            key = _subKeyTools + "\\" + _subKeyToolsMakeOverlay +
                                 "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                ovlFilename = (string)subKey.GetValue(_nameOvlFilename,
                                                         defWorkFolder + "\\" +
                                                         _defaultOvlFilename + ".ovl");
                macroId = (int)subKey.GetValue(_nameMacroId,
                                                        _defaultMacroId);

                tmpInt = (int)subKey.GetValue(_nameFlagEncapsulated,
                                                  _flagTrue);

                flagEncapsulated = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagRestoreCursor,
                                                  _flagTrue);

                flagRestoreCursor = tmpInt != _flagFalse;
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

        public static void LoadDataPCLXL(ref string ovlFilename,
                                        ref bool flagRestoreGS,
                                        ref bool flagEncapsulated,
                                        ref string streamName)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            int tmpInt;

            string key;

            string defWorkFolder = ToolCommonData.DefWorkFolder;

            key = _subKeyTools + "\\" + _subKeyToolsMakeOverlay +
                                 "\\" + _subKeyPCLXL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                ovlFilename = (string)subKey.GetValue(_nameOvlFilename,
                                                        defWorkFolder + "\\" +
                                                        _defaultOvlFilename + ".ovx");
                streamName = (string)subKey.GetValue(_nameStreamName,
                                                        _defaultStreamName);

                tmpInt = (int)subKey.GetValue(_nameFlagEncapsulated,
                                                  _flagTrue);

                flagEncapsulated = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagRestoreGS,
                                                  _flagTrue);

                flagRestoreGS = tmpInt != _flagFalse;
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

            string key = _subKeyTools + "\\" + _subKeyToolsMakeOverlay;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxRptFileFmt = (int)subKey.GetValue(_nameIndxRptFileFmt,
                                                         _indexZero);
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
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsMakeOverlay;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (prnFilename != null)
                {
                    subKey.SetValue(_namePrnFilename,
                                    prnFilename,
                                    RegistryValueKind.String);
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
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsMakeOverlay +
                                 "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (ovlFilename != null)
                {
                    subKey.SetValue(_nameOvlFilename,
                                    ovlFilename,
                                    RegistryValueKind.String);
                }

                subKey.SetValue(_nameMacroId,
                                macroId,
                                RegistryValueKind.DWord);

                if (flagEncapsulated)
                    subKey.SetValue(_nameFlagEncapsulated,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagEncapsulated,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagRestoreCursor)
                    subKey.SetValue(_nameFlagRestoreCursor,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagRestoreCursor,
                                    _flagFalse,
                                    RegistryValueKind.DWord);
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
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsMakeOverlay +
                                 "\\" + _subKeyPCLXL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (ovlFilename != null)
                {
                    subKey.SetValue(_nameOvlFilename,
                                    ovlFilename,
                                    RegistryValueKind.String);
                }

                if (flagEncapsulated)
                    subKey.SetValue(_nameFlagEncapsulated,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagEncapsulated,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagRestoreGS)
                    subKey.SetValue(_nameFlagRestoreGS,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagRestoreGS,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (streamName != null)
                {
                    subKey.SetValue(_nameStreamName,
                                     streamName,
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

            string key = _subKeyTools + "\\" + _subKeyToolsMakeOverlay;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxRptFileFmt,
                                indxRptFileFmt,
                                RegistryValueKind.DWord);
            }
        }
    }
}
