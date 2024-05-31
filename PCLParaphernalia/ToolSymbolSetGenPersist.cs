using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>
    /// Class manages persistent storage of options for the SymbolSetGenerate
    /// tool.
    /// </para>
    /// <para>© Chris Hutchinson 2013</para>
    ///
    /// </summary>
    internal static class ToolSymbolSetGenPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const string _mainKey = MainForm._regMainKey;
        private const string _subKeyTools = "Tools";
        private const string _subKeyToolsSymSetGen = _subKeyTools + @"\SymSetGen";
        private const string _subKeyDonor = "Donor";
        private const string _subKeyTarget = "Target";
        private const string _nameFlagSymSetUserSet = "FlagSymSetUserSet";
        private const string _nameFlagSymSetMapPCL = "FlagSymSetMapPCL";
        private const string _nameFlagIgnoreC0 = "FlagIgnoreC0";
        private const string _nameFlagIgnoreC1 = "FlagIgnoreC1";
        private const string _nameFlagMapHex = "FlagMapHex";
        private const string _nameFlagIndexUnicode = "FlagIndexUnicode";
        private const string _nameFlagCharReqSpecific = "FlagCharReqSpecific";
        private const string _nameCharReqMSL = "CharReqMSL";
        private const string _nameCharReqUnicode = "CharReqUnicode";
        private const string _nameIndxRptFileFmt = "IndxRptFileFmt";
        private const string _nameIndxSymSet = "IndxSymSet";
        private const string _nameSymSetFile = "SymSetFile";
        private const string _nameSymSetFolder = "SymSetFolder";
        private const int _flagFalse = 0;
        private const int _flagTrue = 1;
        private const int _indexZero = 0;
        private const long _defaultReqMSL = 0;
        private const long _defaultReqUnicode = 1;
        private const string _defaultSymSetFile = "DefaultSymSetFile.pcl";

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a D o n o r                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored SymbolSetGenerate data for donor.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataDonor(ref int indxSymSet,
                                          ref bool flagSymSetUserSet,
                                          ref bool flagSymSetMapPCL,
                                          ref string symSetFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsSymSetGen + "\\" + _subKeyDonor;

                int tmpInt;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    indxSymSet = (int)subKey.GetValue(_nameIndxSymSet, _indexZero);

                    tmpInt = (int)subKey.GetValue(_nameFlagSymSetUserSet, _flagFalse);

                    flagSymSetUserSet = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagSymSetMapPCL, _flagFalse);

                    flagSymSetMapPCL = tmpInt != _flagFalse;

                    symSetFile = (string)subKey.GetValue(_nameSymSetFile, defWorkFolder + "\\" + _defaultSymSetFile);
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

        public static void LoadDataRpt(ref int indxRptFileFmt)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsSymSetGen))
                {
                    indxRptFileFmt = (int)subKey.GetValue(_nameIndxRptFileFmt, _indexZero);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a T a r g e t                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored SymbolSetGenerate data for target.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataTarget(ref bool flagMapHex,
                                           ref bool flagIgnoreC0,
                                           ref bool flagIgnoreC1,
                                           ref bool flagIndexUnicode,
                                           ref bool flagCharReqSpecific,
                                           ref ulong charReqUnicode,
                                           ref ulong charReqMSL,
                                           ref string symSetFolder)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsSymSetGen + "\\" + _subKeyTarget;

                int tmpInt;
                long tmpInt64;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    tmpInt = (int)subKey.GetValue(_nameFlagMapHex, _flagTrue);

                    flagMapHex = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagIgnoreC0, _flagTrue);

                    flagIgnoreC0 = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagIgnoreC1, _flagFalse);

                    flagIgnoreC1 = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagIndexUnicode, _flagTrue);

                    flagIndexUnicode = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagCharReqSpecific, _flagFalse);

                    flagCharReqSpecific = tmpInt != _flagFalse;

                    tmpInt64 = (long)subKey.GetValue(_nameCharReqUnicode, _defaultReqUnicode);

                    charReqUnicode = (ulong)tmpInt64;

                    tmpInt64 = (long)subKey.GetValue(_nameCharReqMSL, _defaultReqMSL);
                    charReqMSL = (ulong)tmpInt64;

                    symSetFolder = (string)subKey.GetValue(_nameSymSetFolder, defWorkFolder);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a D o n o r                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current SymbolSetGenerate data for donor.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataDonor(int indxSymSet,
                                          bool flagSymSetUserSet,
                                          bool flagSymSetMapPCL,
                                          string symSetFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsSymSetGen + "\\" + _subKeyDonor;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameIndxSymSet, indxSymSet, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagSymSetUserSet, flagSymSetUserSet ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagSymSetMapPCL, flagSymSetMapPCL ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    if (symSetFile != null)
                        subKey.SetValue(_nameSymSetFile, symSetFile, RegistryValueKind.String);
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
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsSymSetGen))
                {
                    subKey.SetValue(_nameIndxRptFileFmt, indxRptFileFmt, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a T a r g e t                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current SymbolSetGenerate data for target.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataTarget(bool flagMapHex,
                                           bool flagIgnoreC0,
                                           bool flagIgnoreC1,
                                           bool flagIndexUnicode,
                                           bool flagCharReqSpecific,
                                           ulong charReqUnicode,
                                           ulong charReqMSL,
                                           string symSetFolder)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsSymSetGen + "\\" + _subKeyTarget;

                long tmpInt64;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameFlagMapHex, flagMapHex ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagIgnoreC0, flagIgnoreC0 ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagIgnoreC1, flagIgnoreC1 ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagIndexUnicode, flagIndexUnicode ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagCharReqSpecific, flagCharReqSpecific ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    tmpInt64 = (long)charReqUnicode;

                    subKey.SetValue(_nameCharReqUnicode, tmpInt64, RegistryValueKind.QWord);

                    tmpInt64 = (long)charReqMSL;

                    subKey.SetValue(_nameCharReqMSL, tmpInt64, RegistryValueKind.QWord);

                    if (symSetFolder != null)
                        subKey.SetValue(_nameSymSetFolder, symSetFolder, RegistryValueKind.String);
                }
            }
        }
    }
}