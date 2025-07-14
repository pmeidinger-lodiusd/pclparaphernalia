using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class manages persistent storage of options for the PrinterInfo tool.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class ToolPrintLangPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        const string _mainKey = MainForm._regMainKey;

        const string _subKeyTools = "Tools";
        const string _subKeyToolsPDLData = "PdlData";
        const string _subKeyToolsPrintLang = "PrintLang";
        const string _subKeyPCL = "PCL";
        const string _subKeyPCLXL = "PCLXL";
        const string _subKeyPML = "PML";
        const string _subKeySymSets = "SymSets";
        const string _subKeyFonts = "Fonts";

        const string _nameIndxInfoType = "IndxInfoType";
        const string _nameReportFile = "ReportFile";
        const string _nameFlagOptDiscrete = "FlagOptDiscrete";
        const string _nameFlagOptMapping = "FlagOptMapping";
        const string _nameFlagOptMapDuo = "FlagOptMapDuo";
        const string _nameFlagOptObsolete = "FlagOptObsolete";
        const string _nameFlagOptReserved = "FlagOptReserved";
        const string _nameFlagOptRptWrap = "FlagOptRptWrap";
        const string _nameFlagSeqControl = "FlagSeqControl";
        const string _nameFlagSeqComplex = "FlagSeqComplex";
        const string _nameFlagSeqSimple = "FlagSeqSimple";
        const string _nameFlagTagAction = "FlagTagAction";
        const string _nameFlagTagAttrDefiner = "FlagTagAttrDefiner";
        const string _nameFlagTagAttribute = "FlagTagAttribute";
        const string _nameFlagTagDataType = "FlagTagDataType";
        const string _nameFlagTagEmbedDataDef = "FlagTagEmbedDataDef";
        const string _nameFlagTagOperator = "FlagTagOperator";
        const string _nameFlagTagOutcome = "FlagTagOutcome";
        const string _nameFlagTagWhitespace = "FlagTagWhitespace";
        const string _nameIndxRptFileFmt = "IndxRptFileFmt";
        const string _nameIndxRptChkMarks = "IndxRptChkMarks";
        const string _nameSymSetMapType = "SymSetMapType";

        const int _flagFalse = 0;
        const int _flagTrue = 1;
        const int _indexZero = 0;

        const string _defaultFilename = "DefaultPDLReportFile.txt";

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C o m m o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored PDLData common data.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCommon(ref int indxInfoType,
                                          ref string reportFile)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsPrintLang;

            string defWorkFolder = ToolCommonData.DefWorkFolder;

            using (RegistryKey subKey = keyMain.CreateSubKey(_subKeyTools))
            {
                if (Helper_RegKey.KeyExists(subKey, _subKeyToolsPDLData))
                    // update from v2_5_0_0
                    Helper_RegKey.RenameKey(subKey,
                                           _subKeyToolsPDLData,
                                           _subKeyToolsPrintLang);
            }

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxInfoType = (int)subKey.GetValue(_nameIndxInfoType,
                                                      _indexZero);

                reportFile = (string)subKey.GetValue(_nameReportFile,
                                                      defWorkFolder + "\\" +
                                                      _defaultFilename);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a F o n t s                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored PDLData Fonts data.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataFonts(ref bool flagOptMap)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;

            key = _subKeyTools + "\\" + _subKeyToolsPrintLang +
                                 "\\" + _subKeyFonts;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                tmpInt = (int)subKey.GetValue(_nameFlagOptMapping,
                                                  _flagTrue);

                flagOptMap = tmpInt != _flagFalse;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a P C L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored PDLData PCL data.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataPCL(ref bool flagSeqControl,
                                       ref bool flagSeqSimple,
                                       ref bool flagSeqComplex,
                                       ref bool flagOptObsolete,
                                       ref bool flagOptDiscrete)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;

            key = _subKeyTools + "\\" + _subKeyToolsPrintLang +
                                 "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                tmpInt = (int)subKey.GetValue(_nameFlagSeqControl,
                                                _flagTrue);

                flagSeqControl = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagSeqSimple,
                                                _flagTrue);

                flagSeqSimple = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagSeqComplex,
                                                _flagTrue);

                flagSeqComplex = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagOptObsolete,
                                                _flagTrue);

                flagOptObsolete = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagOptDiscrete,
                                                _flagTrue);

                flagOptDiscrete = tmpInt != _flagFalse;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a P C L X L                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored PDLData PCLXL state.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataPCLXL(ref bool flagTagDataType,
                                         ref bool flagTagAttribute,
                                         ref bool flagTagOperator,
                                         ref bool flagTagAttrDefiner,
                                         ref bool flagTagEmbedDataDef,
                                         ref bool flagTagWhitespace,
                                         ref bool flagOptReserved)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;

            key = _subKeyTools + "\\" + _subKeyToolsPrintLang +
                                 "\\" + _subKeyPCLXL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                tmpInt = (int)subKey.GetValue(_nameFlagTagDataType,
                                                _flagTrue);

                flagTagDataType = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagTagAttribute,
                                                _flagTrue);

                flagTagAttribute = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagTagOperator,
                                                _flagTrue);

                flagTagOperator = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagTagAttrDefiner,
                                                _flagTrue);

                flagTagAttrDefiner = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagTagEmbedDataDef,
                                                _flagTrue);

                flagTagEmbedDataDef = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagTagWhitespace,
                                                _flagTrue);

                flagTagWhitespace = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagOptReserved,
                                                _flagTrue);

                flagOptReserved = tmpInt != _flagFalse;

            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a P M L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored PDLData PML state.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataPML(ref bool flagTagDataType,
                                        ref bool flagTagAction,
                                        ref bool flagTagOutcome)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;

            key = _subKeyTools + "\\" + _subKeyToolsPrintLang +
                                 "\\" + _subKeyPML;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                tmpInt = (int)subKey.GetValue(_nameFlagTagDataType,
                                                  _flagTrue);

                flagTagDataType = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagTagAction,
                                                  _flagTrue);

                flagTagAction = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagTagOutcome,
                                                  _flagTrue);

                flagTagOutcome = tmpInt != _flagFalse;
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

        public static void LoadDataRpt(ref int indxRptFileFmt,
                                        ref int indxRptChkMarks,
                                        ref bool flagOptRptWrap)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsPrintLang;

            int tmpInt;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxRptFileFmt = (int)subKey.GetValue(_nameIndxRptFileFmt,
                                                         _indexZero);

                indxRptChkMarks = (int)subKey.GetValue(_nameIndxRptChkMarks,
                                                          _indexZero);

                tmpInt = (int)subKey.GetValue(_nameFlagOptRptWrap,
                                _flagTrue);

                flagOptRptWrap = tmpInt != _flagFalse;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a S y m S e t s                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored PDLData Symbol Set data.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataSymSets(ref bool flagOptMap,
                                           ref int mapType)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;

            key = _subKeyTools + "\\" + _subKeyToolsPrintLang +
                                 "\\" + _subKeySymSets;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                tmpInt = (int)subKey.GetValue(_nameFlagOptMapping,
                                                  _flagTrue);

                flagOptMap = tmpInt != _flagFalse;

                mapType = (int)subKey.GetValue(_nameSymSetMapType,
                                                   _indexZero);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a C o m m o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current PDLData common data.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataCommon(int indxInfoType,
                                          string reportFile)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsPrintLang;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxInfoType,
                                indxInfoType,
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
        // s a v e D a t a F o n t s                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current PDLData Fonts data.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataFonts(bool flagOptMap)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsPrintLang +
                                 "\\" + _subKeyFonts;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (flagOptMap)
                    subKey.SetValue(_nameFlagOptMapping,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagOptMapping,
                                    _flagFalse,
                                    RegistryValueKind.DWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a P C L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current PDLData PCL data.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataPCL(bool flagSeqControl,
                                       bool flagSeqSimple,
                                       bool flagSeqComplex,
                                       bool flagOptObsolete,
                                       bool flagOptDiscrete)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsPrintLang +
                                 "\\" + _subKeyPCL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (flagSeqControl)
                    subKey.SetValue(_nameFlagSeqControl,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagSeqControl,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagSeqSimple)
                    subKey.SetValue(_nameFlagSeqSimple,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagSeqSimple,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagSeqComplex)
                    subKey.SetValue(_nameFlagSeqComplex,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagSeqComplex,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagOptObsolete)
                    subKey.SetValue(_nameFlagOptObsolete,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagOptObsolete,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagOptDiscrete)
                    subKey.SetValue(_nameFlagOptDiscrete,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagOptDiscrete,
                                    _flagFalse,
                                    RegistryValueKind.DWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a P C L X L                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current PDLData PCLXL data.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataPCLXL(bool flagTagDataType,
                                         bool flagTagAttribute,
                                         bool flagTagOperator,
                                         bool flagTagAttrDefiner,
                                         bool flagTagEmbedDataDef,
                                         bool flagTagWhitespace,
                                         bool flagOptReserved)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsPrintLang +
                                 "\\" + _subKeyPCLXL;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (flagTagDataType)
                    subKey.SetValue(_nameFlagTagDataType,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagTagDataType,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagTagAttribute)
                    subKey.SetValue(_nameFlagTagAttribute,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagTagAttribute,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagTagOperator)
                    subKey.SetValue(_nameFlagTagOperator,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagTagOperator,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagTagAttrDefiner)
                    subKey.SetValue(_nameFlagTagAttrDefiner,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagTagAttrDefiner,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagTagEmbedDataDef)
                    subKey.SetValue(_nameFlagTagEmbedDataDef,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagTagEmbedDataDef,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagTagWhitespace)
                    subKey.SetValue(_nameFlagTagWhitespace,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagTagWhitespace,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagOptReserved)
                    subKey.SetValue(_nameFlagOptReserved,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagOptReserved,
                                    _flagFalse,
                                    RegistryValueKind.DWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a P M L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current PDLData PML data.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataPML(bool flagTagDataType,
                                        bool flagTagAction,
                                        bool flagTagOutcome)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsPrintLang +
                                 "\\" + _subKeyPML;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (flagTagDataType)
                    subKey.SetValue(_nameFlagTagDataType,
                                     _flagTrue,
                                     RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagTagDataType,
                                     _flagFalse,
                                     RegistryValueKind.DWord);

                if (flagTagAction)
                    subKey.SetValue(_nameFlagTagAction,
                                     _flagTrue,
                                     RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagTagAction,
                                     _flagFalse,
                                     RegistryValueKind.DWord);

                if (flagTagOutcome)
                    subKey.SetValue(_nameFlagTagOutcome,
                                     _flagTrue,
                                     RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagTagOutcome,
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

        public static void SaveDataRpt(int indxRptFileFmt,
                                        int indxRptChkMarks,
                                        bool flagOptRptWrap)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsPrintLang;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxRptFileFmt,
                                indxRptFileFmt,
                                RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxRptChkMarks,
                                indxRptChkMarks,
                                RegistryValueKind.DWord);

                if (flagOptRptWrap)
                    subKey.SetValue(_nameFlagOptRptWrap,
                                     _flagTrue,
                                     RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagOptRptWrap,
                                     _flagFalse,
                                     RegistryValueKind.DWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a S y m S e t s                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current PDLData Symbol Set data.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataSymSets(bool flagOptMap,
                                           int mapType)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsPrintLang +
                                 "\\" + _subKeySymSets;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                if (flagOptMap)
                    subKey.SetValue(_nameFlagOptMapping,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagOptMapping,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                subKey.SetValue(_nameSymSetMapType,
                                 mapType,
                                 RegistryValueKind.DWord);
            }
        }
    }
}
