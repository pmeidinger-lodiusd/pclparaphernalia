using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class manages persistent storage of options for the Soft Font
    ///	Generate tool.
    /// 
    /// © Chris Hutchinson 2012
    /// 
    /// </summary>

    static class ToolSoftFontGenPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const string _mainKey = MainForm._regMainKey;

        const string _subKeyTools = "Tools";
        const string _subKeyToolsSoftFontGen = "SoftFontGen";
        const string _subKeyMapping = "Mapping";
        const string _subKeyPCL5 = "PCL5";
        const string _subKeyPCL6 = "PCL6";
        const string _subKeyPCL = "PCL";
        const string _subKeyPCLXL = "PCLXL";
        const string _subKeyTTF = "TTF";

        const string _nameTargetFolder = "TargetFolder";
        const string _nameAdhocFontFile = "AdhocFontFile";
        const string _nameSymSetUserFile = "SymSetUserFile";
        const string _nameFlagLogVerbose = "FlagLogVerbose";
        const string _nameFlagFormat16 = "FlagFormat16";
        const string _nameFlagSegGTLast = "FlagSegGTLast";
        const string _nameFlagSymSetMapPCL = "FlagSymSetMapPCL";
        const string _nameFlagSymSetUnbound = "FlagSymSetUnbound";
        const string _nameFlagSymSetUserSet = "FlagSymSetUserSet";
        const string _nameFlagCharCompSpecific = "FlagCharCompSpecific";
        const string _nameFlagUsePCLT = "FlagUsePCLT";
        const string _nameCharCompUnicode = "CharCompUnicode";
        const string _nameFlagVMetrics = "FlagVMetrics";
        const string _nameIndxFont = "IndxFont";
        const string _nameIndxPDL = "IndxPDL";
        const string _nameIndxRptFileFmt = "IndxRptFileFmt";
        const string _nameIndxRptChkMarks = "indxRptChkMarks";
        const string _nameIndxSymSet = "IndxSymSet";
        const string _nameIndxUsePCLT = "IndxUsePCLT";

        const int _flagFalse = 0;
        const int _flagTrue = 1;
        const int _indexZero = 0;
        const long _defaultCompUnicode = -2;    // 0xfffffffffffffffe //

        const string _defaultSymSetUserFile = "DefaultSymSetFile.pcl";
        const string _defaultFontFileTTF = "DefaultFontFile.ttf";

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C o m m o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored FontSample common data.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void loadDataCommon(ref int indxPDL,
                                           ref bool flagLogVerbose)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsSoftFontGen;

            int tmpInt;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (Helper_RegKey.keyExists(subKey, _subKeyPCL5))
                    // update from v2_5_0_0
                    Helper_RegKey.renameKey(subKey, _subKeyPCL5, _subKeyPCL);

                if (Helper_RegKey.keyExists(subKey, _subKeyPCL6))
                    // update from v2_5_0_0
                    Helper_RegKey.renameKey(subKey, _subKeyPCL6, _subKeyPCLXL);

                indxPDL = (int)subKey.GetValue(_nameIndxPDL,
                                                 _indexZero);

                tmpInt = (int)subKey.GetValue(_nameFlagLogVerbose,
                                                         _flagTrue);

                if (tmpInt == _flagFalse)
                    flagLogVerbose = false;
                else
                    flagLogVerbose = true;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a M a p p i n g                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Soft Font Generate mapping data.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void loadDataMapping(ref int indxSymSet,
                                            ref bool flagSymSetMapPCL,
                                            ref bool flagSymSetUnbound,
                                            ref bool flagSymSetUserSet,
                                            ref string symSetUserFile)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;

            string defWorkFolder = ToolCommonData.DefWorkFolder;

            key = _subKeyTools + "\\" + _subKeyToolsSoftFontGen +
                                 "\\" + _subKeyMapping;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxSymSet = (int)subKey.GetValue(_nameIndxSymSet,
                                                         _indexZero);

                tmpInt = (int)subKey.GetValue(_nameFlagSymSetMapPCL,
                                                  _flagFalse);

                if (tmpInt == _flagFalse)
                    flagSymSetMapPCL = false;
                else
                    flagSymSetMapPCL = true;

                tmpInt = (int)subKey.GetValue(_nameFlagSymSetUnbound,
                                                  _flagFalse);

                if (tmpInt == _flagFalse)
                    flagSymSetUnbound = false;
                else
                    flagSymSetUnbound = true;

                tmpInt = (int)subKey.GetValue(_nameFlagSymSetUserSet,
                                                  _flagFalse);

                if (tmpInt == _flagFalse)
                    flagSymSetUserSet = false;
                else
                    flagSymSetUserSet = true;

                symSetUserFile = (string)subKey.GetValue(_nameSymSetUserFile,
                                                          defWorkFolder + "\\" +
                                                          _defaultSymSetUserFile);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a P C L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Soft Font Generate PCL data.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void loadDataPCL(ref string targetFolder,
                                        ref bool flagFormat16,
                                        ref bool flagCharCompSpecific,
                                        ref bool flagVMetrics,
                                        ref bool flagSegGTLast,
                                        ref ulong charCompUnicode)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;
            long tmpInt64;

            string defWorkFolder = ToolCommonData.DefWorkFolder;

            key = _subKeyTools + "\\" + _subKeyToolsSoftFontGen +
                                 "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                // update from v2_8_0_0 begin //

                if (subKey.GetValue(_nameIndxUsePCLT) != null)
                    subKey.DeleteValue(_nameIndxUsePCLT);

                // update from v2_8_0_0 end   //

                targetFolder = (string)subKey.GetValue(_nameTargetFolder,
                                                       defWorkFolder);

                tmpInt = (int)subKey.GetValue(_nameFlagLogVerbose,
                                                         _flagTrue);

                if (tmpInt == _flagFalse)
                    flagFormat16 = false;
                else
                    flagFormat16 = true;

                tmpInt = (int)subKey.GetValue(_nameFlagCharCompSpecific,
                                                 _flagFalse);

                if (tmpInt == _flagFalse)
                    flagCharCompSpecific = false;
                else
                    flagCharCompSpecific = true;

                tmpInt = (int)subKey.GetValue(_nameFlagVMetrics,
                                                  _flagTrue);

                if (tmpInt == _flagFalse)
                    flagVMetrics = false;
                else
                    flagVMetrics = true;

                tmpInt = (int)subKey.GetValue(_nameFlagSegGTLast,
                                                  _flagFalse);

                if (tmpInt == _flagFalse)
                    flagSegGTLast = false;
                else
                    flagSegGTLast = true;

                tmpInt64 = (long)subKey.GetValue(_nameCharCompUnicode,
                                                   _defaultCompUnicode);

                charCompUnicode = (ulong)tmpInt64;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a P C L X L                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Soft Font Generate PCLXL data.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void loadDataPCLXL(ref string targetFolder,
                                          ref bool flagVMetrics)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;

            string defWorkFolder = ToolCommonData.DefWorkFolder;

            key = _subKeyTools + "\\" + _subKeyToolsSoftFontGen +
                                 "\\" + _subKeyPCLXL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                // update from v2_8_0_0 begin //

                if (subKey.GetValue(_nameIndxUsePCLT) != null)
                    subKey.DeleteValue(_nameIndxUsePCLT);

                // update from v2_8_0_0 end   //

                targetFolder = (string)subKey.GetValue(_nameTargetFolder,
                                                         defWorkFolder);

                tmpInt = (int)subKey.GetValue(_nameFlagVMetrics,
                                                  _flagTrue);

                if (tmpInt == _flagFalse)
                    flagVMetrics = false;
                else
                    flagVMetrics = true;
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

        public static void loadDataRpt(ref int indxRptFileFmt,
                                        ref int indxRptChkMarks)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsSoftFontGen;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxRptFileFmt = (int)subKey.GetValue(_nameIndxRptFileFmt,
                                                         _indexZero);

                indxRptChkMarks = (int)subKey.GetValue(_nameIndxRptChkMarks,
                                                          _indexZero);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a T T F                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Soft Font Generate TTF data.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void loadDataTTF(ref int indxFont,
                                        ref bool flagUsePCLT,
                                        ref string adhocFontFile)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;

            string defWorkFolder = ToolCommonData.DefWorkFolder;

            key = _subKeyTools + "\\" + _subKeyToolsSoftFontGen +
                                 "\\" + _subKeyTTF;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxFont = (int)subKey.GetValue(_nameIndxFont,
                                                      _indexZero);

                tmpInt = (int)subKey.GetValue(_nameFlagUsePCLT,
                                                _flagTrue);

                if (tmpInt == _flagFalse)
                    flagUsePCLT = false;
                else
                    flagUsePCLT = true;

                adhocFontFile = (string)subKey.GetValue(_nameAdhocFontFile,
                                                         defWorkFolder + "\\" +
                                                         _defaultFontFileTTF);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a C o m m o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Soft Font Generate common data.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void saveDataCommon(int indxPDL,
                                          bool flagLogVerbose)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsSoftFontGen;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxPDL,
                                indxPDL,
                                RegistryValueKind.DWord);

                if (flagLogVerbose)
                    subKey.SetValue(_nameFlagLogVerbose,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagLogVerbose,
                                    _flagFalse,
                                    RegistryValueKind.DWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a M a p p i n g                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Soft Font Generate mapping data.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void saveDataMapping(int indxSymSet,
                                            bool flagSymSetMapPCL,
                                            bool flagSymSetUnbound,
                                            bool flagSymSetUserSet,
                                            string symSetUserFile)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsSoftFontGen +
                                 "\\" + _subKeyMapping;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxSymSet,
                                indxSymSet,
                                RegistryValueKind.DWord);

                if (flagSymSetMapPCL)
                    subKey.SetValue(_nameFlagSymSetMapPCL,
                                     _flagTrue,
                                     RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagSymSetMapPCL,
                                     _flagFalse,
                                     RegistryValueKind.DWord);

                if (flagSymSetUnbound)
                    subKey.SetValue(_nameFlagSymSetUnbound,
                                     _flagTrue,
                                     RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagSymSetUnbound,
                                     _flagFalse,
                                     RegistryValueKind.DWord);

                if (flagSymSetUserSet)
                    subKey.SetValue(_nameFlagSymSetUserSet,
                                     _flagTrue,
                                     RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagSymSetUserSet,
                                     _flagFalse,
                                     RegistryValueKind.DWord);

                if (symSetUserFile != null)
                {
                    subKey.SetValue(_nameSymSetUserFile,
                                     symSetUserFile,
                                     RegistryValueKind.String);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a P C L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store Soft Font Generate PCL data.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void saveDataPCL(string targetFolder,
                                        bool flagFormat16,
                                        bool flagCharCompSpecific,
                                        bool flagVMetrics,
                                        bool flagSegGTLast,
                                        ulong charCompUnicode)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            long tmpInt64;

            key = _subKeyTools + "\\" + _subKeyToolsSoftFontGen +
                                 "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (targetFolder != null)
                {
                    subKey.SetValue(_nameTargetFolder,
                                    targetFolder,
                                    RegistryValueKind.String);
                }

                if (flagFormat16)
                    subKey.SetValue(_nameFlagFormat16,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagFormat16,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagCharCompSpecific)
                    subKey.SetValue(_nameFlagCharCompSpecific,
                                     _flagTrue,
                                     RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagCharCompSpecific,
                                     _flagFalse,
                                     RegistryValueKind.DWord);

                if (flagVMetrics)
                    subKey.SetValue(_nameFlagVMetrics,
                                     _flagTrue,
                                     RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagVMetrics,
                                     _flagFalse,
                                     RegistryValueKind.DWord);

                if (flagSegGTLast)
                    subKey.SetValue(_nameFlagSegGTLast,
                                     _flagTrue,
                                     RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagSegGTLast,
                                     _flagFalse,
                                     RegistryValueKind.DWord);

                tmpInt64 = (long)charCompUnicode;

                subKey.SetValue(_nameCharCompUnicode,
                                 tmpInt64,
                                 RegistryValueKind.QWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a P C L X L                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store Soft Font Generate PCLXL data.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void saveDataPCLXL(string targetFolder,
                                         bool flagVMetrics)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsSoftFontGen +
                                 "\\" + _subKeyPCLXL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (targetFolder != null)
                {
                    subKey.SetValue(_nameTargetFolder,
                                    targetFolder,
                                    RegistryValueKind.String);
                }

                if (flagVMetrics)
                    subKey.SetValue(_nameFlagVMetrics,
                                     _flagTrue,
                                     RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagVMetrics,
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

        public static void saveDataRpt(int indxRptFileFmt,
                                        int indxRptChkMarks)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsSoftFontGen;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxRptFileFmt,
                                indxRptFileFmt,
                                RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxRptChkMarks,
                                indxRptChkMarks,
                                RegistryValueKind.DWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a T T F                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store Soft Font Generate TTF data.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void saveDataTTF(int indxFont,
                                        bool flagUsePCLT,
                                        string adhocFontFile)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsSoftFontGen +
                                 "\\" + _subKeyTTF;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxFont,
                                indxFont,
                                RegistryValueKind.DWord);

                if (flagUsePCLT)
                    subKey.SetValue(_nameFlagUsePCLT,
                                     _flagTrue,
                                     RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagUsePCLT,
                                     _flagFalse,
                                     RegistryValueKind.DWord);

                if (adhocFontFile != null)
                {
                    subKey.SetValue(_nameAdhocFontFile,
                                     adhocFontFile,
                                     RegistryValueKind.String);
                }
            }
        }
    }
}
