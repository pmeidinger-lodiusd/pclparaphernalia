using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class manages persistent storage of options for the PrinterInfo tool.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class ToolPrintLangPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const string _mainKey = MainForm._regMainKey;
        private const string _subKeyTools = "Tools";
        private const string _subKeyToolsPDLData = _subKeyTools + @"\PdlData";
        private const string _subKeyToolsPrintLang = "PrintLang";
        private const string _subKeyPCL = "PCL";
        private const string _subKeyPCLXL = "PCLXL";
        private const string _subKeyPML = "PML";
        private const string _subKeySymSets = "SymSets";
        private const string _subKeyFonts = "Fonts";
        private const string _nameIndxInfoType = "IndxInfoType";
        private const string _nameReportFile = "ReportFile";
        private const string _nameFlagOptDiscrete = "FlagOptDiscrete";
        private const string _nameFlagOptMapping = "FlagOptMapping";

        //const string _nameFlagOptMapDuo = "FlagOptMapDuo";
        private const string _nameFlagOptObsolete = "FlagOptObsolete";

        private const string _nameFlagOptReserved = "FlagOptReserved";
        private const string _nameFlagOptRptWrap = "FlagOptRptWrap";
        private const string _nameFlagSeqControl = "FlagSeqControl";
        private const string _nameFlagSeqComplex = "FlagSeqComplex";
        private const string _nameFlagSeqSimple = "FlagSeqSimple";
        private const string _nameFlagTagAction = "FlagTagAction";
        private const string _nameFlagTagAttrDefiner = "FlagTagAttrDefiner";
        private const string _nameFlagTagAttribute = "FlagTagAttribute";
        private const string _nameFlagTagDataType = "FlagTagDataType";
        private const string _nameFlagTagEmbedDataDef = "FlagTagEmbedDataDef";
        private const string _nameFlagTagOperator = "FlagTagOperator";
        private const string _nameFlagTagOutcome = "FlagTagOutcome";
        private const string _nameFlagTagWhitespace = "FlagTagWhitespace";
        private const string _nameIndxRptFileFmt = "IndxRptFileFmt";
        private const string _nameIndxRptChkMarks = "IndxRptChkMarks";
        private const string _nameSymSetMapType = "SymSetMapType";
        private const int _flagFalse = 0;
        private const int _flagTrue = 1;
        private const int _indexZero = 0;
        private const string _defaultFilename = "DefaultPDLReportFile.txt";

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C o m m o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored PDLData common data.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCommon(out int indxInfoType, out string reportFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.CreateSubKey(_subKeyToolsPrintLang))
                {
                    indxInfoType = (int)subKey.GetValue(_nameIndxInfoType, _indexZero);

                    reportFile = (string)subKey.GetValue(_nameReportFile, defWorkFolder + "\\" + _defaultFilename);
                }
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

        public static void LoadDataFonts(out bool flagOptMap)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsPrintLang + "\\" + _subKeyFonts;

                int tmpInt;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    tmpInt = (int)subKey.GetValue(_nameFlagOptMapping, _flagTrue);

                    flagOptMap = tmpInt != _flagFalse;
                }
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

        public static void LoadDataPCL(out bool flagSeqControl,
                                       out bool flagSeqSimple,
                                       out bool flagSeqComplex,
                                       out bool flagOptObsolete,
                                       out bool flagOptDiscrete)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsPrintLang + "\\" + _subKeyPCL;

                int tmpInt;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    tmpInt = (int)subKey.GetValue(_nameFlagSeqControl, _flagTrue);

                    flagSeqControl = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagSeqSimple, _flagTrue);

                    flagSeqSimple = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagSeqComplex, _flagTrue);

                    flagSeqComplex = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagOptObsolete, _flagTrue);

                    flagOptObsolete = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagOptDiscrete, _flagTrue);

                    flagOptDiscrete = tmpInt != _flagFalse;
                }
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

        public static void LoadDataPCLXL(out bool flagTagDataType,
                                         out bool flagTagAttribute,
                                         out bool flagTagOperator,
                                         out bool flagTagAttrDefiner,
                                         out bool flagTagEmbedDataDef,
                                         out bool flagTagWhitespace,
                                         out bool flagOptReserved)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsPrintLang + "\\" + _subKeyPCLXL;

                int tmpInt;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    tmpInt = (int)subKey.GetValue(_nameFlagTagDataType, _flagTrue);

                    flagTagDataType = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagTagAttribute, _flagTrue);

                    flagTagAttribute = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagTagOperator, _flagTrue);

                    flagTagOperator = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagTagAttrDefiner, _flagTrue);

                    flagTagAttrDefiner = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagTagEmbedDataDef, _flagTrue);

                    flagTagEmbedDataDef = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagTagWhitespace, _flagTrue);

                    flagTagWhitespace = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagOptReserved, _flagTrue);

                    flagOptReserved = tmpInt != _flagFalse;
                }
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

        public static void LoadDataPML(out bool flagTagDataType, out bool flagTagAction, out bool flagTagOutcome)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsPrintLang + "\\" + _subKeyPML;

                int tmpInt;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    tmpInt = (int)subKey.GetValue(_nameFlagTagDataType, _flagTrue);

                    flagTagDataType = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagTagAction, _flagTrue);

                    flagTagAction = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagTagOutcome, _flagTrue);

                    flagTagOutcome = tmpInt != _flagFalse;
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

        public static void LoadDataRpt(out int indxRptFileFmt, out int indxRptChkMarks, out bool flagOptRptWrap)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                int tmpInt;

                using (var subKey = keyMain.CreateSubKey(_subKeyToolsPrintLang))
                {
                    indxRptFileFmt = (int)subKey.GetValue(_nameIndxRptFileFmt, _indexZero);

                    indxRptChkMarks = (int)subKey.GetValue(_nameIndxRptChkMarks, _indexZero);

                    tmpInt = (int)subKey.GetValue(_nameFlagOptRptWrap, _flagTrue);

                    flagOptRptWrap = tmpInt != _flagFalse;
                }
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

        public static void LoadDataSymSets(out bool flagOptMap, out int mapType)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsPrintLang + "\\" + _subKeySymSets;

                int tmpInt;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    tmpInt = (int)subKey.GetValue(_nameFlagOptMapping, _flagTrue);

                    flagOptMap = tmpInt != _flagFalse;

                    mapType = (int)subKey.GetValue(_nameSymSetMapType, _indexZero);
                }
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

        public static void SaveDataCommon(int indxInfoType, string reportFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsPrintLang))
                {
                    subKey.SetValue(_nameIndxInfoType, indxInfoType, RegistryValueKind.DWord);

                    if (reportFile != null)
                        subKey.SetValue(_nameReportFile, reportFile, RegistryValueKind.String);
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
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsPrintLang + "\\" + _subKeyFonts;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameFlagOptMapping, flagOptMap ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
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
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsPrintLang + "\\" + _subKeyPCL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameFlagSeqControl, flagSeqControl ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagSeqSimple, flagSeqSimple ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagSeqComplex, flagSeqComplex ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagOptObsolete, flagOptObsolete ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagOptDiscrete, flagOptDiscrete ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
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
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsPrintLang + "\\" + _subKeyPCLXL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameFlagTagDataType, flagTagDataType ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagTagAttribute, flagTagAttribute ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagTagOperator, flagTagOperator ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagTagAttrDefiner, flagTagAttrDefiner ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagTagEmbedDataDef, flagTagEmbedDataDef ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagTagWhitespace, flagTagWhitespace ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagOptReserved, flagOptReserved ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
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

        public static void SaveDataPML(bool flagTagDataType, bool flagTagAction, bool flagTagOutcome)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsPrintLang + "\\" + _subKeyPML;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameFlagTagDataType, flagTagDataType ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagTagAction, flagTagAction ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagTagOutcome, flagTagOutcome ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
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

        public static void SaveDataRpt(int indxRptFileFmt, int indxRptChkMarks, bool flagOptRptWrap)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsPrintLang))
                {
                    subKey.SetValue(_nameIndxRptFileFmt, indxRptFileFmt, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxRptChkMarks, indxRptChkMarks, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagOptRptWrap, flagOptRptWrap ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
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

        public static void SaveDataSymSets(bool flagOptMap, int mapType)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsPrintLang + "\\" + _subKeySymSets;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameFlagOptMapping, flagOptMap ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameSymSetMapType, mapType, RegistryValueKind.DWord);
                }
            }
        }
    }
}