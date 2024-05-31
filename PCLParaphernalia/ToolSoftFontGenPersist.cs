using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>
    /// Class manages persistent storage of options for the Soft Font
    ///	Generate tool.
    /// </para>
    /// <para>© Chris Hutchinson 2012</para>
    ///
    /// </summary>
    internal static class ToolSoftFontGenPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const string _mainKey = MainForm._regMainKey;
        private const string _subKeyTools = "Tools";
        private const string _subKeyToolsSoftFontGen = _subKeyTools + @"\SoftFontGen";
        private const string _subKeyMapping = "Mapping";
        private const string _subKeyPCL = "PCL";
        private const string _subKeyPCLXL = "PCLXL";
        private const string _subKeyTTF = "TTF";
        private const string _nameTargetFolder = "TargetFolder";
        private const string _nameAdhocFontFile = "AdhocFontFile";
        private const string _nameSymSetUserFile = "SymSetUserFile";
        private const string _nameFlagLogVerbose = "FlagLogVerbose";
        private const string _nameFlagFormat16 = "FlagFormat16";
        private const string _nameFlagSegGTLast = "FlagSegGTLast";
        private const string _nameFlagSymSetMapPCL = "FlagSymSetMapPCL";
        private const string _nameFlagSymSetUnbound = "FlagSymSetUnbound";
        private const string _nameFlagSymSetUserSet = "FlagSymSetUserSet";
        private const string _nameFlagCharCompSpecific = "FlagCharCompSpecific";
        private const string _nameFlagUsePCLT = "FlagUsePCLT";
        private const string _nameCharCompUnicode = "CharCompUnicode";
        private const string _nameFlagVMetrics = "FlagVMetrics";
        private const string _nameIndxFont = "IndxFont";
        private const string _nameIndxPDL = "IndxPDL";
        private const string _nameIndxRptFileFmt = "IndxRptFileFmt";
        private const string _nameIndxRptChkMarks = "indxRptChkMarks";
        private const string _nameIndxSymSet = "IndxSymSet";
        private const string _nameIndxUsePCLT = "IndxUsePCLT";
        private const int _flagFalse = 0;
        private const int _flagTrue = 1;
        private const int _indexZero = 0;
        private const long _defaultCompUnicode = -2;    // 0xfffffffffffffffe //

        private const string _defaultSymSetUserFile = "DefaultSymSetFile.pcl";
        private const string _defaultFontFileTTF = "DefaultFontFile.ttf";

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C o m m o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored FontSample common data.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCommon(out int indxPDL, out bool flagLogVerbose)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                int tmpInt;

                using (var subKey = keyMain.CreateSubKey(_subKeyToolsSoftFontGen))
                {
                    indxPDL = (int)subKey.GetValue(_nameIndxPDL, _indexZero);

                    tmpInt = (int)subKey.GetValue(_nameFlagLogVerbose, _flagTrue);

                    flagLogVerbose = tmpInt != _flagFalse;
                }
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

        public static void LoadDataMapping(out int indxSymSet,
                                            out bool flagSymSetMapPCL,
                                            out bool flagSymSetUnbound,
                                            out bool flagSymSetUserSet,
                                            out string symSetUserFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsSoftFontGen + "\\" + _subKeyMapping;

                int tmpInt;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    indxSymSet = (int)subKey.GetValue(_nameIndxSymSet, _indexZero);

                    tmpInt = (int)subKey.GetValue(_nameFlagSymSetMapPCL, _flagFalse);

                    flagSymSetMapPCL = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagSymSetUnbound, _flagFalse);

                    flagSymSetUnbound = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagSymSetUserSet, _flagFalse);

                    flagSymSetUserSet = tmpInt != _flagFalse;

                    symSetUserFile = (string)subKey.GetValue(_nameSymSetUserFile, defWorkFolder + "\\" + _defaultSymSetUserFile);
                }
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

        public static void LoadDataPCL(out string targetFolder,
                                       out bool flagFormat16,
                                       out bool flagCharCompSpecific,
                                       out bool flagVMetrics,
                                       out bool flagSegGTLast,
                                       out ulong charCompUnicode)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsSoftFontGen + "\\" + _subKeyPCL;

                int tmpInt;
                long tmpInt64;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    // update from v2_8_0_0 begin //

                    if (subKey.GetValue(_nameIndxUsePCLT) != null)
                        subKey.DeleteValue(_nameIndxUsePCLT);

                    // update from v2_8_0_0 end   //

                    targetFolder = (string)subKey.GetValue(_nameTargetFolder, defWorkFolder);

                    tmpInt = (int)subKey.GetValue(_nameFlagLogVerbose, _flagTrue);

                    flagFormat16 = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagCharCompSpecific, _flagFalse);

                    flagCharCompSpecific = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagVMetrics, _flagTrue);

                    flagVMetrics = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagSegGTLast, _flagFalse);

                    flagSegGTLast = tmpInt != _flagFalse;

                    tmpInt64 = (long)subKey.GetValue(_nameCharCompUnicode, _defaultCompUnicode);

                    charCompUnicode = (ulong)tmpInt64;
                }
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

        public static void LoadDataPCLXL(out string targetFolder,
                                          out bool flagVMetrics)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsSoftFontGen + "\\" + _subKeyPCLXL;

                int tmpInt;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    // update from v2_8_0_0 begin //

                    if (subKey.GetValue(_nameIndxUsePCLT) != null)
                        subKey.DeleteValue(_nameIndxUsePCLT);

                    // update from v2_8_0_0 end   //

                    targetFolder = (string)subKey.GetValue(_nameTargetFolder, defWorkFolder);

                    tmpInt = (int)subKey.GetValue(_nameFlagVMetrics, _flagTrue);

                    flagVMetrics = tmpInt != _flagFalse;
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

        public static void LoadDataRpt(out int indxRptFileFmt, out int indxRptChkMarks)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsSoftFontGen))
                {
                    indxRptFileFmt = (int)subKey.GetValue(_nameIndxRptFileFmt, _indexZero);

                    indxRptChkMarks = (int)subKey.GetValue(_nameIndxRptChkMarks, _indexZero);
                }
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

        public static void LoadDataTTF(out int indxFont, out bool flagUsePCLT, out string adhocFontFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsSoftFontGen + "\\" + _subKeyTTF;

                int tmpInt;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    indxFont = (int)subKey.GetValue(_nameIndxFont, _indexZero);

                    tmpInt = (int)subKey.GetValue(_nameFlagUsePCLT, _flagTrue);

                    flagUsePCLT = tmpInt != _flagFalse;

                    adhocFontFile = (string)subKey.GetValue(_nameAdhocFontFile, defWorkFolder + "\\" + _defaultFontFileTTF);
                }
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

        public static void SaveDataCommon(int indxPDL, bool flagLogVerbose)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsSoftFontGen))
                {
                    subKey.SetValue(_nameIndxPDL, indxPDL, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagLogVerbose, flagLogVerbose ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
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

        public static void SaveDataMapping(int indxSymSet,
                                            bool flagSymSetMapPCL,
                                            bool flagSymSetUnbound,
                                            bool flagSymSetUserSet,
                                            string symSetUserFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsSoftFontGen + "\\" + _subKeyMapping;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameIndxSymSet, indxSymSet, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagSymSetMapPCL, flagSymSetMapPCL ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagSymSetUnbound, flagSymSetUnbound ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagSymSetUserSet, flagSymSetUserSet ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    if (symSetUserFile != null)
                        subKey.SetValue(_nameSymSetUserFile, symSetUserFile, RegistryValueKind.String);
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

        public static void SaveDataPCL(string targetFolder,
                                        bool flagFormat16,
                                        bool flagCharCompSpecific,
                                        bool flagVMetrics,
                                        bool flagSegGTLast,
                                        ulong charCompUnicode)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsSoftFontGen + "\\" + _subKeyPCL;

                long tmpInt64;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    if (targetFolder != null)
                        subKey.SetValue(_nameTargetFolder, targetFolder, RegistryValueKind.String);

                    subKey.SetValue(_nameFlagFormat16, flagFormat16 ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagCharCompSpecific, flagCharCompSpecific ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagVMetrics, flagVMetrics ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagSegGTLast, flagSegGTLast ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    tmpInt64 = (long)charCompUnicode;

                    subKey.SetValue(_nameCharCompUnicode, tmpInt64, RegistryValueKind.QWord);
                }
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

        public static void SaveDataPCLXL(string targetFolder, bool flagVMetrics)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsSoftFontGen + "\\" + _subKeyPCLXL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    if (targetFolder != null)
                        subKey.SetValue(_nameTargetFolder, targetFolder, RegistryValueKind.String);

                    subKey.SetValue(_nameFlagVMetrics, flagVMetrics ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
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

        public static void SaveDataRpt(int indxRptFileFmt, int indxRptChkMarks)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsSoftFontGen))
                {
                    subKey.SetValue(_nameIndxRptFileFmt, indxRptFileFmt, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxRptChkMarks, indxRptChkMarks, RegistryValueKind.DWord);
                }
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

        public static void SaveDataTTF(int indxFont, bool flagUsePCLT, string adhocFontFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsSoftFontGen + "\\" + _subKeyTTF;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameIndxFont, indxFont, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagUsePCLT, flagUsePCLT ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    if (adhocFontFile != null)
                        subKey.SetValue(_nameAdhocFontFile, adhocFontFile, RegistryValueKind.String);
                }
            }
        }
    }
}