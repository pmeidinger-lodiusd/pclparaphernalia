using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class manages persistent storage of options for the FormSample tool.</para>
    /// <para>© Chris Hutchinson 2012</para>
    ///
    /// </summary>
    internal static class ToolFormSamplePersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const string _mainKey = MainForm._regMainKey;
        private const string _subKeyTools = "Tools";
        private const string _subKeyToolsFormSample = _subKeyTools + @"\FormSample";
        private const string _subKeyPCL5 = "PCL5";
        private const string _subKeyPCL6 = "PCL6";
        private const string _subKeyPCL = "PCL";
        private const string _subKeyPCLXL = "PCLXL";
        private const string _nameCaptureFile = "CaptureFile";
        private const string _nameFormNameMain = "FormNameMain";
        private const string _nameFormNameRear = "FormNameRear";
        private const string _nameFormFileMain = "FormFileMain";
        private const string _nameFormFileRear = "FormFileRear";
        private const string _namePrnDiskFileMain = "PrnDiskFileMain";
        private const string _namePrnDiskFileRear = "PrnDiskFileRear";
        private const string _nameMacroIdMain = "MacroIdMain";
        private const string _nameMacroIdRear = "MacroIdRear";
        private const string _nameFlagMacroRemove = "FlagMacroRemove";
        private const string _nameFlagMainForm = "FlagMainForm";
        private const string _nameFlagRearForm = "FlagRearForm";
        private const string _nameFlagMainOnPrnDisk = "FlagMainOnPrnDisk";
        private const string _nameFlagRearOnPrnDisk = "FlagRearOnPrnDisk";
        private const string _nameFlagRearBPlate = "FlagRearBPlate";
        private const string _nameFlagGSPushPop = "FlagGSPushPop";
        private const string _nameFlagPrintDescText = "FlagPrintDescText";
        private const string _nameTestPageCount = "TestPageCount";
        private const string _nameIndxMethod = "IndxMethod";
        private const string _nameIndxOrientation = "IndxOrientation";
        private const string _nameIndxOrientRear = "IndxOrientationRear";
        private const string _nameIndxPaperSize = "IndxPaperSize";
        private const string _nameIndxPaperType = "IndxPaperType";
        private const string _nameIndxPDL = "IndxPDL";
        private const string _nameIndxPlexMode = "IndxPlexMode";
        private const int _flagFalse = 0;
        private const int _flagTrue = 1;
        private const int _indexZero = 0;

        //const string _defaultCaptureFile = "Capture_FormSample.prn";
        private const string _defaultCaptureFilePCL = "CaptureFile_FormSamplePCL.prn";

        private const string _defaultCaptureFilePCLXL = "CaptureFile_FormSamplePCLXL.prn";
        private const string _defaultFilePCLMain = "DefaultFilePCLMain.ovl";
        private const string _defaultFilePCLRear = "DefaultFilePCLRear.ovl";
        private const string _defaultFilePCLXLMain = "DefaultFilePCLXLMain.ovx";
        private const string _defaultFilePCLXLRear = "DefaultFilePCLXLRear.ovx";
        private const string _defaultFormNameMain = "TestFormMain";
        private const string _defaultFormNameRear = "TestFormRear";
        private const int _defaultMacroIdMain = 32767;
        private const int _defaultMacroIdRear = 32766;
        private const int _defaultTestPageCount = 3;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C a p t u r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored FormSample capture file data.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCapture(ToolCommonData.PrintLang crntPDL, ref string captureFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string oldFile;

                bool update_from_v2_5_0_0 = false;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                //----------------------------------------------------------------//

                using (var subKey = keyMain.OpenSubKey(_subKeyToolsFormSample, true))
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
                    const string keyPCL = _subKeyToolsFormSample + "\\" + _subKeyPCL;

                    using (var subKey = keyMain.CreateSubKey(keyPCL))
                    {
                        subKey.SetValue(_nameCaptureFile, oldFile, RegistryValueKind.String);
                    }

                    const string keyPCLXL = _subKeyToolsFormSample + "\\" + _subKeyPCLXL;

                    using (var subKey = keyMain.CreateSubKey(keyPCLXL))
                    {
                        subKey.SetValue(_nameCaptureFile, oldFile, RegistryValueKind.String);
                    }
                }

                //----------------------------------------------------------------//

                if (crntPDL == ToolCommonData.PrintLang.PCL)
                {
                    const string key = _subKeyToolsFormSample + "\\" + _subKeyPCL;

                    using (var subKey = keyMain.CreateSubKey(key))
                    {
                        captureFile = (string)subKey.GetValue(_nameCaptureFile, defWorkFolder + "\\" + _defaultCaptureFilePCL);
                    }
                }
                else if (crntPDL == ToolCommonData.PrintLang.PCLXL)
                {
                    const string key = _subKeyToolsFormSample + "\\" + _subKeyPCLXL;

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
        // Retrieve stored FormSample common data.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCommon(ref int indxPDL)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsFormSample))
                {
                    indxPDL = (int)subKey.GetValue(_nameIndxPDL, _indexZero);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a G e n e r a l                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored FormSample PCL or PCL XL general data.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataGeneral(
            string pdlName,
            ref int indxPaperType,
            ref int indxPaperSize,
            ref int indxOrientation,
            ref int indxPlexMode,
            ref int indxOrientRear,
            ref int indxMethod,
            ref int testPageCount,
            ref bool flagMacroRemove,
            ref bool flagMainForm,
            ref bool flagRearForm,
            ref bool flagRearBPlate,
            ref bool flagPrintDescText)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string key = _subKeyToolsFormSample + "\\" + pdlName;

                int tmpInt;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    indxPaperType = (int)subKey.GetValue(_nameIndxPaperType, _indexZero);
                    indxPaperSize = (int)subKey.GetValue(_nameIndxPaperSize, _indexZero);
                    indxOrientation = (int)subKey.GetValue(_nameIndxOrientation, _indexZero);
                    indxPlexMode = (int)subKey.GetValue(_nameIndxPlexMode, _indexZero);
                    indxOrientRear = (int)subKey.GetValue(_nameIndxOrientRear, _indexZero);
                    indxMethod = (int)subKey.GetValue(_nameIndxMethod, _indexZero);
                    testPageCount = (int)subKey.GetValue(_nameTestPageCount, _defaultTestPageCount);

                    tmpInt = (int)subKey.GetValue(_nameFlagMacroRemove, _flagTrue);

                    flagMacroRemove = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagMainForm, _flagTrue);

                    flagMainForm = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagRearForm, _flagTrue);

                    flagRearForm = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagRearBPlate, _flagTrue);

                    flagRearBPlate = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagPrintDescText, _flagTrue);

                    flagPrintDescText = tmpInt != _flagFalse;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a P C L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored FormSample PCL form data.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataPCL(ref bool flagMainOnPrnDisk,
                                        ref bool flagRearOnPrnDisk,
                                        ref string formFileMain,
                                        ref string formFileRear,
                                        ref string prnDiskFileMain,
                                        ref string prnDiskFileRear,
                                        ref int macroIdMain,
                                        ref int macroIdRear)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsFormSample + "\\" + _subKeyPCL;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                int tmpInt;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    tmpInt = (int)subKey.GetValue(_nameFlagMainOnPrnDisk, _flagFalse);

                    flagMainOnPrnDisk = tmpInt != _flagFalse;

                    tmpInt = (int)subKey.GetValue(_nameFlagRearOnPrnDisk, _flagFalse);

                    flagRearOnPrnDisk = tmpInt != _flagFalse;

                    formFileMain = (string)subKey.GetValue(_nameFormFileMain, defWorkFolder + "\\" + _defaultFilePCLMain);
                    formFileRear = (string)subKey.GetValue(_nameFormFileRear, defWorkFolder + "\\" + _defaultFilePCLRear);

                    prnDiskFileMain = (string)subKey.GetValue(_namePrnDiskFileMain, _defaultFilePCLMain);
                    prnDiskFileRear = (string)subKey.GetValue(_namePrnDiskFileRear, _defaultFilePCLRear);

                    macroIdMain = (int)subKey.GetValue(_nameMacroIdMain, _defaultMacroIdMain);
                    macroIdRear = (int)subKey.GetValue(_nameMacroIdRear, _defaultMacroIdRear);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a P C L X L                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored FormSample PCL XL form data.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataPCLXL(ref string formFileMain,
                                          ref string formFileRear,
                                          ref string formNameMain,
                                          ref string formNameRear,
                                          ref bool flagGSPushPop)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsFormSample + "\\" + _subKeyPCLXL;

                int tmpInt;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    formFileMain = (string)subKey.GetValue(_nameFormFileMain, defWorkFolder + "\\" + _defaultFilePCLXLMain);
                    formFileRear = (string)subKey.GetValue(_nameFormFileRear, defWorkFolder + "\\" + _defaultFilePCLXLRear);
                    formNameMain = (string)subKey.GetValue(_nameFormNameMain, _defaultFormNameMain);
                    formNameRear = (string)subKey.GetValue(_nameFormNameRear, _defaultFormNameRear);

                    tmpInt = (int)subKey.GetValue(_nameFlagGSPushPop, _flagTrue);

                    flagGSPushPop = tmpInt != _flagFalse;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a C a p t u r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current FormSample capture file data.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataCapture(ToolCommonData.PrintLang crntPDL, string captureFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                if (crntPDL == ToolCommonData.PrintLang.PCL)
                {
                    const string key = _subKeyToolsFormSample + "\\" + _subKeyPCL;

                    using (var subKey = keyMain.CreateSubKey(key))
                    {
                        if (captureFile != null)
                            subKey.SetValue(_nameCaptureFile, captureFile, RegistryValueKind.String);
                    }
                }
                else if (crntPDL == ToolCommonData.PrintLang.PCLXL)
                {
                    const string key = _subKeyToolsFormSample + "\\" + _subKeyPCLXL;

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
        // Store current FormSample common data.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataCommon(int indxPDL)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsFormSample))
                {
                    subKey.SetValue(_nameIndxPDL, indxPDL, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a G e n e r a l                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current FormSample PCL or PCL XL general data.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataGeneral(string pdlName,
                                            int indxPaperType,
                                            int indxPaperSize,
                                            int indxOrientation,
                                            int indxPlexMode,
                                            int indxOrientRear,
                                            int indxMethod,
                                            int testPageCount,
                                            bool flagMacroRemove,
                                            bool flagMainForm,
                                            bool flagRearForm,
                                            bool flagRearBPlate,
                                            bool flagPrintDescText)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string key = _subKeyToolsFormSample + "\\" + pdlName;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameIndxPaperType, indxPaperType, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxPaperSize, indxPaperSize, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxOrientation, indxOrientation, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxPlexMode, indxPlexMode, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxOrientRear, indxOrientRear, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxMethod, indxMethod, RegistryValueKind.DWord);

                    subKey.SetValue(_nameTestPageCount, testPageCount, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagMacroRemove, flagMacroRemove ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagMainForm, flagMainForm ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagRearForm, flagRearForm ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagRearBPlate, flagRearBPlate ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagPrintDescText, flagPrintDescText ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a P C L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store FormSample PCL form data.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataPCL(bool flagMainOnPrnDisk,
                                        bool flagRearOnPrnDisk,
                                        string formFileMain,
                                        string formFileRear,
                                        string prnDiskFileMain,
                                        string prnDiskFileRear,
                                        int macroIdMain,
                                        int macroIdRear)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsFormSample + "\\" + _subKeyPCL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameFlagMainOnPrnDisk, flagMainOnPrnDisk ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    subKey.SetValue(_nameFlagRearOnPrnDisk, flagRearOnPrnDisk ? _flagTrue : _flagFalse, RegistryValueKind.DWord);

                    if (formFileMain != null)
                        subKey.SetValue(_nameFormFileMain, formFileMain, RegistryValueKind.String);

                    if (formFileRear != null)
                        subKey.SetValue(_nameFormFileRear, formFileRear, RegistryValueKind.String);

                    if (prnDiskFileMain != null)
                        subKey.SetValue(_namePrnDiskFileMain, prnDiskFileMain, RegistryValueKind.String);

                    if (prnDiskFileRear != null)
                        subKey.SetValue(_namePrnDiskFileRear, prnDiskFileRear, RegistryValueKind.String);

                    subKey.SetValue(_nameMacroIdMain, macroIdMain, RegistryValueKind.DWord);

                    subKey.SetValue(_nameMacroIdRear, macroIdRear, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a P C L X L                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store FormSample PCL XL form data.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataPCLXL(string formFileMain,
                                          string formFileRear,
                                          string formNameMain,
                                          string formNameRear,
                                          bool flagGSPushPop)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyToolsFormSample + "\\" + _subKeyPCLXL;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    if (formFileMain != null)
                        subKey.SetValue(_nameFormFileMain, formFileMain, RegistryValueKind.String);

                    if (formFileRear != null)
                        subKey.SetValue(_nameFormFileRear, formFileRear, RegistryValueKind.String);

                    if (formNameMain != null)
                        subKey.SetValue(_nameFormNameMain, formNameMain, RegistryValueKind.String);

                    if (formNameRear != null)
                        subKey.SetValue(_nameFormNameRear, formNameRear, RegistryValueKind.String);

                    subKey.SetValue(_nameFlagGSPushPop, flagGSPushPop ? _flagTrue : _flagFalse, RegistryValueKind.DWord);
                }
            }
        }
    }
}