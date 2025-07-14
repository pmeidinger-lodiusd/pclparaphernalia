using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class manages persistent storage of options for the MiscSamples tool.
    /// 
    /// © Chris Hutchinson 2014
    /// 
    /// </summary>

    static class ToolMiscSamplesPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        const string _mainKey = MainForm._regMainKey;

        const string _subKeyTools = "Tools";
        const string _subKeyToolsMiscSamples = "MiscSamples";
        const string _subKeyPCL = "PCL";
        const string _subKeyPCLXL = "PCLXL";

        const string _subKeyCommon = "Common";

        const string _subKeyColour = "Colour";
        const string _subKeyLogOper = "LogOper";
        const string _subKeyLogPage = "LogPage";
        const string _subKeyPattern = "Pattern";
        const string _subKeyTxtMod = "TxtMod";
        const string _subKeyUnicode = "Unicode";

        const string _nameCaptureFile = "CaptureFile";
        const string _nameFlagAddStdPage = "FlagAddStdPage";
        const string _nameFlagFormAsMacro = "FlagFormAsMacro";
        const string _nameFlagMapHex = "FlagMapHex";
        const string _nameFlagSrcTextPat = "FlagSrcTextPat";
        const string _nameFlagUseColour = "FlagUseColour";
        const string _nameFlagUseMacros = "FlagUseMacros";

        const string _nameIndxColourMode = "IndxColourMode";
        const string _nameIndxColourD1 = "IndxColourD1";
        const string _nameIndxColourD2 = "IndxColourD2";
        const string _nameIndxColourS1 = "IndxColourS1";
        const string _nameIndxColourS2 = "IndxColourS2";
        const string _nameIndxColourT1 = "IndxColourT1";
        const string _nameIndxColourT2 = "IndxColourT2";
        const string _nameIndxMonoD1 = "IndxMonoD1";
        const string _nameIndxMonoD2 = "IndxMonoD2";
        const string _nameIndxMonoS1 = "IndxMonoS1";
        const string _nameIndxMonoS2 = "IndxMonoS2";
        const string _nameIndxMonoT1 = "IndxMonoT1";
        const string _nameIndxMonoT2 = "IndxMonoT2";
        const string _nameIndxFont = "IndxFont";
        const string _nameIndxOrientation = "IndxOrientation";
        const string _nameIndxPaperSize = "IndxPaperSize";
        const string _nameIndxPaperType = "IndxPaperType";
        const string _nameIndxPDL = "IndxPDL";
        const string _nameIndxPatternType = "IndxPatternType";
        const string _nameIndxROPFrom = "IndxROPFrom";
        const string _nameIndxROPTo = "IndxROPTo";
        const string _nameIndxSampleType = "IndxSampleType";
        const string _nameIndxTxtModType = "IndxTxtModType";
        const string _nameIndxVariant = "IndxVariant";

        const string _nameValueRoot = "_Value_";
        const string _nameCodePoint = "CodePoint";
        const string _nameOffsetLeft = "OffsetLeft";
        const string _nameOffsetTop = "OffsetTop";
        const string _namePageHeight = "PageHeight";
        const string _namePageWidth = "PageWidth";

        const int _flagFalse = 0;
        const int _flagTrue = 1;
        const int _indexZero = 0;
        const int _indexNeg = -1;

        const string _defaultCaptureFileRoot = "CaptureFile_MiscSamples_";

        const int _defaultColour_0 = 0xff0000;
        const int _defaultColour_1 = 0x00ff00;
        const int _defaultColour_2 = 0x0000ff;
        const int _defaultColour_3 = 0xffb450;
        const int _defaultShade_0 = 0x20;
        const int _defaultShade_1 = 0x40;
        const int _defaultShade_2 = 0x80;
        const int _defaultShade_3 = 0xc0;
        const int _defaultCodePoint = 0x20ac;
        const int _defaultIndxVariant = (int)(PCLFonts.eVariant.Regular);

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C a p t u r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Misc Samples capture file data.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCapture(
            ToolCommonData.eToolSubIds crntToolSubId,
            ToolCommonData.ePrintLang crntPDL,
            ref string captureFile)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string subKeyType = string.Empty;
            string defFileBase = string.Empty;

            string defWorkFolder = ToolCommonData.DefWorkFolder;

            if (crntToolSubId == ToolCommonData.eToolSubIds.Colour)
            {
                subKeyType = "\\" + _subKeyColour;
                defFileBase = _defaultCaptureFileRoot + _subKeyColour;
            }
            else if (crntToolSubId == ToolCommonData.eToolSubIds.LogOper)
            {
                subKeyType = "\\" + _subKeyLogOper;
                defFileBase = _defaultCaptureFileRoot + _subKeyLogOper;
            }
            else if (crntToolSubId == ToolCommonData.eToolSubIds.LogPage)
            {
                subKeyType = "\\" + _subKeyLogPage;
                defFileBase = _defaultCaptureFileRoot + _subKeyLogPage;
            }
            else if (crntToolSubId == ToolCommonData.eToolSubIds.Pattern)
            {
                subKeyType = "\\" + _subKeyPattern;
                defFileBase = _defaultCaptureFileRoot + _subKeyPattern;
            }
            else if (crntToolSubId == ToolCommonData.eToolSubIds.TxtMod)
            {
                subKeyType = "\\" + _subKeyTxtMod;
                defFileBase = _defaultCaptureFileRoot + _subKeyTxtMod;
            }
            else if (crntToolSubId == ToolCommonData.eToolSubIds.Unicode)
            {
                subKeyType = "\\" + _subKeyUnicode;
                defFileBase = _defaultCaptureFileRoot + _subKeyUnicode;
            }

            if (crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                string key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                            "\\" + subKeyType +
                                            "\\" + _subKeyPCL;

                using (RegistryKey subKey = keyMain.CreateSubKey(key))
                {
                    captureFile = (string)subKey.GetValue(
                        _nameCaptureFile,
                        defWorkFolder + "\\" + defFileBase + _subKeyPCL + ".prn");
                }
            }
            else if (crntPDL == ToolCommonData.ePrintLang.PCLXL)
            {
                string key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                            "\\" + subKeyType +
                                            "\\" + _subKeyPCLXL;

                using (RegistryKey subKey = keyMain.CreateSubKey(key))
                {
                    captureFile = (string)subKey.GetValue(
                        _nameCaptureFile,
                        defWorkFolder + "\\" + defFileBase + _subKeyPCLXL + ".prn");
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C o m m o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Misc Samples common data.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCommon(ref int indxPDL,
                                          ref int indxSampleType)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsMiscSamples;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxPDL = (int)subKey.GetValue(_nameIndxPDL,
                                                 _indexZero);

                indxSampleType = (int)subKey.GetValue(_nameIndxSampleType,
                                                         _indexZero);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C o m m o n P D L                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Misc Samples PCL or PCL XL common data.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCommonPDL(string pdlName,
                                              ref int indxOrientation,
                                              ref int indxPaperSize,
                                              ref int indxPaperType)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyCommon +
                                 "\\" + pdlName;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxOrientation = (int)subKey.GetValue(_nameIndxOrientation,
                                                         _indexZero);

                indxPaperSize = (int)subKey.GetValue(_nameIndxPaperSize,
                                                         _indexZero);

                indxPaperType = (int)subKey.GetValue(_nameIndxPaperType,
                                                         _indexZero);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a T y p e C o l o u r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Misc Samples - Colour data.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataTypeColour(string pdlName,
                                               ref int indxColourMode,
                                               ref bool flagFormAsMacro,
                                               ref bool flagMapHex)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyColour +
                                 "\\" + pdlName;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxColourMode = (int)subKey.GetValue(_nameIndxColourMode,
                                                          _indexZero);

                tmpInt = (int)subKey.GetValue(_nameFlagFormAsMacro,
                                                  _flagTrue);

                flagFormAsMacro = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagMapHex,
                                                  _flagTrue);

                flagMapHex = tmpInt != _flagFalse;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a T y p e C o l o u r S a m p l e                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Misc Samples - Colour data sample.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataTypeColourSample(string pdlName,
                                                     string sampleName,
                                                     int sampleCt,
                                                     ref int[] values,
                                                     bool monochrome)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyColour +
                                 "\\" + pdlName;

            if (monochrome)
            {
                using (RegistryKey subKey = keyMain.CreateSubKey(key))
                {
                    int defVal;

                    for (int i = 0; i < sampleCt; i++)
                    {
                        if (i == 0)
                            defVal = _defaultShade_0;
                        else if (i == 1)
                            defVal = _defaultShade_1;
                        else if (i == 2)
                            defVal = _defaultShade_2;
                        else
                            defVal = _defaultShade_3;

                        values[i] = (int)subKey.GetValue(
                            sampleName + _nameValueRoot + i, defVal);
                    }
                }
            }
            else
            {
                using (RegistryKey subKey = keyMain.CreateSubKey(key))
                {
                    for (int i = 0; i < sampleCt; i++)
                    {
                        int defVal;

                        if (i == 0)
                            defVal = _defaultColour_0;
                        else if (i == 1)
                            defVal = _defaultColour_1;
                        else if (i == 2)
                            defVal = _defaultColour_2;
                        else
                            defVal = _defaultColour_3;

                        values[i] = (int)subKey.GetValue(
                            sampleName + _nameValueRoot + i, defVal);
                    }
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a T y p e L o g O p e r                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Misc Samples - Logical Operations data.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataTypeLogOper(string pdlName,
                                                ref int indxMode,
                                                ref int indxROPFrom,
                                                ref int indxROPTo,
                                                ref int indxClrD1,
                                                ref int indxClrD2,
                                                ref int indxClrS1,
                                                ref int indxClrS2,
                                                ref int indxClrT1,
                                                ref int indxClrT2,
                                                ref int indxMonoD1,
                                                ref int indxMonoD2,
                                                ref int indxMonoS1,
                                                ref int indxMonoS2,
                                                ref int indxMonoT1,
                                                ref int indxMonoT2,
                                                ref bool flagUseMacros,
                                                ref bool flagSrcTextPat)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;
            int indxClrBlack,
                  indxClrWhite,
                  indxMonoBlack,
                  indxMonoWhite;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyLogOper +
                                 "\\" + pdlName;

            if (pdlName == _subKeyPCL)
            {
                byte indxPalCMY =
                    (byte)PCLPalettes.eIndex.PCLSimpleColourCMY;

                indxClrBlack = PCLPalettes.GetClrItemBlack(indxPalCMY);
                indxClrWhite = PCLPalettes.GetClrItemWhite(indxPalCMY);

                byte indxPalMono =
                    (byte)PCLPalettes.eIndex.PCLMonochrome;

                indxMonoBlack = PCLPalettes.GetClrItemBlack(indxPalMono);
                indxMonoWhite = PCLPalettes.GetClrItemWhite(indxPalMono);
            }
            else // if (pdlName == _subKeyPCLXL)
            {
                byte indxPalRGB =
                    (byte)PCLXLPalettes.eIndex.PCLXLRGB;

                indxClrBlack = PCLXLPalettes.GetClrItemBlack(indxPalRGB);
                indxClrWhite = PCLXLPalettes.GetClrItemWhite(indxPalRGB);

                indxMonoBlack = 0;
                indxMonoWhite = 255;
            }

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxMode = (int)subKey.GetValue(_nameIndxColourMode,
                                                    _indexZero);

                indxROPFrom = (int)subKey.GetValue(_nameIndxROPFrom,
                                                       _indexZero);

                indxROPTo = (int)subKey.GetValue(_nameIndxROPTo,
                                                     _indexNeg);

                indxClrD1 = (int)subKey.GetValue(_nameIndxColourD1,
                                                     indxClrBlack);

                indxClrD2 = (int)subKey.GetValue(_nameIndxColourD2,
                                                     indxClrWhite);

                indxClrS1 = (int)subKey.GetValue(_nameIndxColourS1,
                                                     indxClrBlack);

                indxClrS2 = (int)subKey.GetValue(_nameIndxColourS2,
                                                     indxClrWhite);

                indxClrT1 = (int)subKey.GetValue(_nameIndxColourT1,
                                                     indxClrBlack);

                indxClrT2 = (int)subKey.GetValue(_nameIndxColourT2,
                                                     indxClrWhite);

                indxMonoD1 = (int)subKey.GetValue(_nameIndxMonoD1,
                                                     indxMonoBlack);

                indxMonoD2 = (int)subKey.GetValue(_nameIndxMonoD2,
                                                     indxMonoWhite);

                indxMonoS1 = (int)subKey.GetValue(_nameIndxMonoS1,
                                                     indxMonoBlack);

                indxMonoS2 = (int)subKey.GetValue(_nameIndxMonoS2,
                                                     indxMonoWhite);

                indxMonoT1 = (int)subKey.GetValue(_nameIndxMonoT1,
                                                     indxMonoBlack);

                indxMonoT2 = (int)subKey.GetValue(_nameIndxMonoT2,
                                                     indxMonoWhite);

                tmpInt = (int)subKey.GetValue(_nameFlagUseMacros,
                                                  _flagTrue);

                flagUseMacros = tmpInt != _flagFalse;

                if (pdlName == _subKeyPCLXL)
                {
                    tmpInt = (int)subKey.GetValue(_nameFlagSrcTextPat,
                                                      _flagTrue);

                    flagSrcTextPat = tmpInt != _flagFalse;
                }
                else
                {
                    flagSrcTextPat = true;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a T y p e L o g P a g e                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Misc Samples - Logical Page data.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataTypeLogPage(string pdlName,
                                                ref int offsetLeft,
                                                ref int offsetTop,
                                                ref int pageHeight,
                                                ref int pageWidth,
                                                ref bool flagFormAsMacro,
                                                ref bool flagAddStdPage)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyLogPage +
                                 "\\" + pdlName;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                offsetLeft = (int)subKey.GetValue(_nameOffsetLeft,
                                                         _indexZero);

                offsetTop = (int)subKey.GetValue(_nameOffsetTop,
                                                         _indexZero);

                pageHeight = (int)subKey.GetValue(_namePageHeight,
                                                         _indexZero);

                pageWidth = (int)subKey.GetValue(_namePageWidth,
                                                         _indexZero);

                tmpInt = (int)subKey.GetValue(_nameFlagFormAsMacro,
                                                  _flagTrue);

                flagFormAsMacro = tmpInt != _flagFalse;

                tmpInt = (int)subKey.GetValue(_nameFlagAddStdPage,
                                                         _flagTrue);

                flagAddStdPage = tmpInt != _flagFalse;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a T y p e P a t t e r n                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Misc Samples - Pattern data.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataTypePattern(string pdlName,
                                               ref int indxPatternType,
                                               ref bool flagFormAsMacro)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyPattern +
                                 "\\" + pdlName;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxPatternType = (int)subKey.GetValue(_nameIndxPatternType,
                                                         _indexZero);

                tmpInt = (int)subKey.GetValue(_nameFlagFormAsMacro,
                                                  _flagTrue);

                flagFormAsMacro = tmpInt != _flagFalse;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a T y p e T x t M o d                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Misc Samples - Txt Mod data.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataTypeTxtMod(string pdlName,
                                               ref int indxTxtModType,
                                               ref bool flagFormAsMacro)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyTxtMod +
                                 "\\" + pdlName;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxTxtModType = (int)subKey.GetValue(_nameIndxTxtModType,
                                                         _indexZero);

                tmpInt = (int)subKey.GetValue(_nameFlagFormAsMacro,
                                                  _flagTrue);

                flagFormAsMacro = tmpInt != _flagFalse;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a T y p e U n i c o d e                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Misc Samples - Unicode data.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataTypeUnicode(string pdlName,
                                               ref int indxFont,
                                               ref PCLFonts.eVariant variant,
                                               ref int codePoint,
                                               ref bool flagFormAsMacro)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            int tmpInt;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyUnicode +
                                 "\\" + pdlName;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxFont = (int)subKey.GetValue(_nameIndxFont,
                                                  _indexZero);

                tmpInt = (int)subKey.GetValue(_nameIndxVariant,
                                                      _defaultIndxVariant);
                variant = (PCLFonts.eVariant)tmpInt;

                codePoint = (int)subKey.GetValue(_nameCodePoint,
                                                   _defaultCodePoint);

                tmpInt = (int)subKey.GetValue(_nameFlagFormAsMacro,
                                                  _flagTrue);

                flagFormAsMacro = tmpInt != _flagFalse;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a C a p t u r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Misc Samples capture file data.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataCapture(
            ToolCommonData.eToolSubIds crntToolSubId,
            ToolCommonData.ePrintLang crntPDL,
            string captureFile)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string subKeyType = string.Empty;

            if (crntToolSubId == ToolCommonData.eToolSubIds.Colour)
                subKeyType = "\\" + _subKeyColour;
            else if (crntToolSubId == ToolCommonData.eToolSubIds.LogOper)
                subKeyType = "\\" + _subKeyLogOper;
            else if (crntToolSubId == ToolCommonData.eToolSubIds.LogPage)
                subKeyType = "\\" + _subKeyLogPage;
            else if (crntToolSubId == ToolCommonData.eToolSubIds.Pattern)
                subKeyType = "\\" + _subKeyPattern;
            else if (crntToolSubId == ToolCommonData.eToolSubIds.TxtMod)
                subKeyType = "\\" + _subKeyTxtMod;
            else if (crntToolSubId == ToolCommonData.eToolSubIds.Unicode)
                subKeyType = "\\" + _subKeyUnicode;

            if (crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                string key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                            "\\" + subKeyType +
                                            "\\" + _subKeyPCL;

                using (RegistryKey subKey = keyMain.CreateSubKey(key))
                {
                    if (captureFile != null)
                    {
                        subKey.SetValue(_nameCaptureFile,
                                         captureFile,
                                         RegistryValueKind.String);
                    }
                }
            }
            else if (crntPDL == ToolCommonData.ePrintLang.PCLXL)
            {
                string key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                            "\\" + subKeyType +
                                            "\\" + _subKeyPCLXL;

                using (RegistryKey subKey = keyMain.CreateSubKey(key))
                {
                    if (captureFile != null)
                    {
                        subKey.SetValue(_nameCaptureFile,
                                         captureFile,
                                         RegistryValueKind.String);
                    }
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a C o m m o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Misc Samples common data.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataCommon(int indxPDL,
                                          int indxSampleType)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsMiscSamples;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxPDL,
                                indxPDL,
                                RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxSampleType,
                                indxSampleType,
                                RegistryValueKind.DWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a C o m m o n P D L                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Misc Samples PCL or PCL XL common data.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataCommonPDL(string pdlName,
                                             int indxOrientation,
                                             int indxPaperSize,
                                             int indxPaperType)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyCommon +
                                 "\\" + pdlName;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxOrientation,
                                indxOrientation,
                                RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxPaperSize,
                                indxPaperSize,
                                RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxPaperType,
                                indxPaperType,
                                RegistryValueKind.DWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a T y p e C o l o u r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Misc Samples - Colour data.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataTypeColour(string pdlName,
                                               int indxColourMode,
                                               bool flagFormAsMacro,
                                               bool flagMapHex)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyColour +
                                 "\\" + pdlName;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxColourMode,
                                indxColourMode,
                                RegistryValueKind.DWord);

                if (flagFormAsMacro)
                    subKey.SetValue(_nameFlagFormAsMacro,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagFormAsMacro,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagMapHex)
                    subKey.SetValue(_nameFlagMapHex,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagMapHex,
                                    _flagFalse,
                                    RegistryValueKind.DWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a T y p e C o l o u r S a m p l e                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Misc Samples - Colour data sample.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataTypeColourSample(string pdlName,
                                                     string sampleName,
                                                     int sampleCt,
                                                     int[] values)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyColour +
                                 "\\" + pdlName;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                for (int i = 0; i < sampleCt; i++)
                {
                    subKey.SetValue(sampleName + _nameValueRoot + i,
                                    values[i],
                                    RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a T y p e L o g O p e r                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Misc Samples - Logical Operations data.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataTypeLogOper(string pdlName,
                                                int indxMode,
                                                int indxROPFrom,
                                                int indxROPTo,
                                                int indxClrD1,
                                                int indxClrD2,
                                                int indxClrS1,
                                                int indxClrS2,
                                                int indxClrT1,
                                                int indxClrT2,
                                                int indxMonoD1,
                                                int indxMonoD2,
                                                int indxMonoS1,
                                                int indxMonoS2,
                                                int indxMonoT1,
                                                int indxMonoT2,
                                                bool flagUseMacros,
                                                bool flagSrcTextPat)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyLogOper +
                                 "\\" + pdlName;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxColourMode,
                                 indxMode,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxROPFrom,
                                 indxROPFrom,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxROPTo,
                                 indxROPTo,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxColourD1,
                                 indxClrD1,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxColourD2,
                                 indxClrD2,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxColourS1,
                                 indxClrS1,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxColourS2,
                                 indxClrS2,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxColourT1,
                                 indxClrT1,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxColourT2,
                                 indxClrT2,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxMonoD1,
                                 indxMonoD1,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxMonoD2,
                                 indxMonoD2,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxMonoS1,
                                 indxMonoS1,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxMonoS2,
                                 indxMonoS2,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxMonoT1,
                                 indxMonoT1,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxMonoT2,
                                 indxMonoT2,
                                 RegistryValueKind.DWord);

                if (flagUseMacros)
                    subKey.SetValue(_nameFlagUseMacros,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagUseMacros,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (pdlName == _subKeyPCLXL)
                {
                    if (flagSrcTextPat)
                        subKey.SetValue(_nameFlagSrcTextPat,
                                        _flagTrue,
                                        RegistryValueKind.DWord);
                    else
                        subKey.SetValue(_nameFlagSrcTextPat,
                                        _flagFalse,
                                        RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a T y p e L o g P a g e                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Misc Samples - Log Page data.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataTypeLogPage(string pdlName,
                                               int offsetLeft,
                                               int offsetTop,
                                               int pageHeight,
                                               int pageWidth,
                                               bool flagFormAsMacro,
                                               bool flagAddStdPage)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyLogPage +
                                 "\\" + pdlName;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameOffsetLeft,
                                offsetLeft,
                                RegistryValueKind.DWord);

                subKey.SetValue(_nameOffsetTop,
                                offsetTop,
                                RegistryValueKind.DWord);

                subKey.SetValue(_namePageHeight,
                                pageHeight,
                                RegistryValueKind.DWord);

                subKey.SetValue(_namePageWidth,
                                pageWidth,
                                RegistryValueKind.DWord);

                if (flagFormAsMacro)
                    subKey.SetValue(_nameFlagFormAsMacro,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagFormAsMacro,
                                    _flagFalse,
                                    RegistryValueKind.DWord);

                if (flagAddStdPage)
                    subKey.SetValue(_nameFlagAddStdPage,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagAddStdPage,
                                    _flagFalse,
                                    RegistryValueKind.DWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a T y p e P a t t e r n                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Misc Samples - Pattern data.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataTypePattern(string pdlName,
                                               int indxPatternType,
                                               bool flagFormAsMacro)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyPattern +
                                 "\\" + pdlName;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxPatternType,
                                indxPatternType,
                                RegistryValueKind.DWord);

                if (flagFormAsMacro)
                    subKey.SetValue(_nameFlagFormAsMacro,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagFormAsMacro,
                                    _flagFalse,
                                    RegistryValueKind.DWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a T y p e T x t M o d                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Misc Samples - Txt Mod data.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataTypeTxtMod(string pdlName,
                                              int indxTxtModType,
                                              bool flagFormAsMacro)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyTxtMod +
                                 "\\" + pdlName;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxTxtModType,
                                indxTxtModType,
                                RegistryValueKind.DWord);

                if (flagFormAsMacro)
                    subKey.SetValue(_nameFlagFormAsMacro,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagFormAsMacro,
                                    _flagFalse,
                                    RegistryValueKind.DWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a T y p e U n i c o d e                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Misc Samples - Unicode data.                         //
        //                                                                    //
        //--------------------------------------------------------------------//


        public static void SaveDataTypeUnicode(string pdlName,
                                               int indxFont,
                                               PCLFonts.eVariant variant,
                                               int codePoint,
                                               bool flagFormAsMacro)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsMiscSamples +
                                 "\\" + _subKeyUnicode +
                                 "\\" + pdlName;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxFont,
                                indxFont,
                                RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxVariant,
                                (int)variant,
                                RegistryValueKind.DWord);

                subKey.SetValue(_nameCodePoint,
                                codePoint,
                                RegistryValueKind.DWord);

                if (flagFormAsMacro)
                    subKey.SetValue(_nameFlagFormAsMacro,
                                    _flagTrue,
                                    RegistryValueKind.DWord);
                else
                    subKey.SetValue(_nameFlagFormAsMacro,
                                    _flagFalse,
                                    RegistryValueKind.DWord);
            }
        }
    }
}
