using Microsoft.Win32;
using System;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class manages persistent storage of options for the SymbolSetGenerate
    /// tool.
    /// 
    /// © Chris Hutchinson 2013
    /// 
    /// </summary>

    static class ToolSymbolSetGenPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const string _mainKey = MainForm._regMainKey;

        const string _subKeyTools = "Tools";
        const string _subKeyToolsSymSetGen = "SymSetGen";
        const string _subKeyDonor = "Donor";
        const string _subKeyTarget = "Target";

        const string _nameFlagSymSetUserSet = "FlagSymSetUserSet";
        const string _nameFlagSymSetMapPCL = "FlagSymSetMapPCL";
        const string _nameFlagIgnoreC0 = "FlagIgnoreC0";
        const string _nameFlagIgnoreC1 = "FlagIgnoreC1";
        const string _nameFlagMapHex = "FlagMapHex";
        const string _nameFlagIndexUnicode = "FlagIndexUnicode";
        const string _nameFlagCharReqSpecific = "FlagCharReqSpecific";
        const string _nameCharReqMSL = "CharReqMSL";
        const string _nameCharReqUnicode = "CharReqUnicode";
        const string _nameIndxRptFileFmt = "IndxRptFileFmt";
        const string _nameIndxSymSet = "IndxSymSet";
        const string _nameSymSetFile = "SymSetFile";
        const string _nameSymSetFolder = "SymSetFolder";

        const int _flagFalse = 0;
        const int _flagTrue = 1;
        const int _indexZero = 0;
        const long _defaultReqMSL = 0;
        const long _defaultReqUnicode = 1;

        const string _defaultSymSetFile = "DefaultSymSetFile.pcl";

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a D o n o r                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored SymbolSetGenerate data for donor.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void loadDataDonor(ref int indxSymSet,
                                          ref bool flagSymSetUserSet,
                                          ref bool flagSymSetMapPCL,
                                          ref string symSetFile)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;

            string defWorkFolder = ToolCommonData.DefWorkFolder;

            key = _subKeyTools + "\\" + _subKeyToolsSymSetGen +
                                 "\\" + _subKeyDonor;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxSymSet = (int)subKey.GetValue(_nameIndxSymSet,
                                                       _indexZero);

                tmpInt = (int)subKey.GetValue(_nameFlagSymSetUserSet,
                                                       _flagFalse);

                if (tmpInt == _flagFalse)
                    flagSymSetUserSet = false;
                else
                    flagSymSetUserSet = true;

                tmpInt = (int)subKey.GetValue(_nameFlagSymSetMapPCL,
                                                       _flagFalse);

                if (tmpInt == _flagFalse)
                    flagSymSetMapPCL = false;
                else
                    flagSymSetMapPCL = true;

                symSetFile = (string)subKey.GetValue(_nameSymSetFile,
                                                      defWorkFolder + "\\" +
                                                      _defaultSymSetFile);
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

        public static void loadDataRpt(ref int indxRptFileFmt)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsSymSetGen;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxRptFileFmt = (int)subKey.GetValue(_nameIndxRptFileFmt,
                                                         _indexZero);
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

        public static void loadDataTarget(ref bool flagMapHex,
                                           ref bool flagIgnoreC0,
                                           ref bool flagIgnoreC1,
                                           ref bool flagIndexUnicode,
                                           ref bool flagCharReqSpecific,
                                           ref ulong charReqUnicode,
                                           ref ulong charReqMSL,
                                           ref string symSetFolder)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;
            long tmpInt64;

            string defWorkFolder = ToolCommonData.DefWorkFolder;

            key = _subKeyTools + "\\" + _subKeyToolsSymSetGen +
                                 "\\" + _subKeyTarget;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                tmpInt = (int)subKey.GetValue(_nameFlagMapHex,
                                                       _flagTrue);

                if (tmpInt == _flagFalse)
                    flagMapHex = false;
                else
                    flagMapHex = true;

                tmpInt = (int)subKey.GetValue(_nameFlagIgnoreC0,
                                                       _flagTrue);

                if (tmpInt == _flagFalse)
                    flagIgnoreC0 = false;
                else
                    flagIgnoreC0 = true;

                tmpInt = (int)subKey.GetValue(_nameFlagIgnoreC1,
                                                       _flagFalse);

                if (tmpInt == _flagFalse)
                    flagIgnoreC1 = false;
                else
                    flagIgnoreC1 = true;

                tmpInt = (int)subKey.GetValue(_nameFlagIndexUnicode,
                                                       _flagTrue);

                if (tmpInt == _flagFalse)
                    flagIndexUnicode = false;
                else
                    flagIndexUnicode = true;

                tmpInt = (int)subKey.GetValue(_nameFlagCharReqSpecific,
                                                       _flagFalse);

                if (tmpInt == _flagFalse)
                    flagCharReqSpecific = false;
                else
                    flagCharReqSpecific = true;

                tmpInt64 = (long)subKey.GetValue(_nameCharReqUnicode,
                                                        _defaultReqUnicode);

                charReqUnicode = (ulong)tmpInt64;

                tmpInt64 = (long)subKey.GetValue(_nameCharReqMSL,
                                                        _defaultReqMSL);
                charReqMSL = (ulong)tmpInt64;

                symSetFolder = (string)subKey.GetValue(_nameSymSetFolder,
                                                        defWorkFolder);
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

        public static void saveDataDonor(int indxSymSet,
                                          bool flagSymSetUserSet,
                                          bool flagSymSetMapPCL,
                                          string symSetFile)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsSymSetGen +
                                 "\\" + _subKeyDonor;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxSymSet,
                                indxSymSet,
                                RegistryValueKind.DWord);

                if (flagSymSetUserSet)
                    subKey.SetValue(_nameFlagSymSetUserSet,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagSymSetUserSet,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagSymSetMapPCL)
                    subKey.SetValue(_nameFlagSymSetMapPCL,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagSymSetMapPCL,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (symSetFile != null)
                {
                    subKey.SetValue(_nameSymSetFile,
                                     symSetFile,
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

        public static void saveDataRpt(int indxRptFileFmt)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsSymSetGen;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxRptFileFmt,
                                indxRptFileFmt,
                                RegistryValueKind.DWord);
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

        public static void saveDataTarget(bool flagMapHex,
                                           bool flagIgnoreC0,
                                           bool flagIgnoreC1,
                                           bool flagIndexUnicode,
                                           bool flagCharReqSpecific,
                                           ulong charReqUnicode,
                                           ulong charReqMSL,
                                           string symSetFolder)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            long tmpInt64;

            key = _subKeyTools + "\\" + _subKeyToolsSymSetGen +
                                 "\\" + _subKeyTarget;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (flagMapHex)
                    subKey.SetValue(_nameFlagMapHex,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagMapHex,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagIgnoreC0)
                    subKey.SetValue(_nameFlagIgnoreC0,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagIgnoreC0,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagIgnoreC1)
                    subKey.SetValue(_nameFlagIgnoreC1,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagIgnoreC1,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagIndexUnicode)
                    subKey.SetValue(_nameFlagIndexUnicode,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagIndexUnicode,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagCharReqSpecific)
                    subKey.SetValue(_nameFlagCharReqSpecific,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagCharReqSpecific,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                tmpInt64 = (long)charReqUnicode;

                subKey.SetValue(_nameCharReqUnicode,
                                 tmpInt64,
                                 RegistryValueKind.QWord);

                tmpInt64 = (long)charReqMSL;

                subKey.SetValue(_nameCharReqMSL,
                                 tmpInt64,
                                 RegistryValueKind.QWord);

                if (symSetFolder != null)
                {
                    subKey.SetValue(_nameSymSetFolder,
                                     symSetFolder,
                                     RegistryValueKind.String);
                }
            }
        }
    }
}
