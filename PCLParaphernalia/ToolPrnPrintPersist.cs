using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class manages persistent storage of options for the Prn Print tool.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    static class ToolPrnPrintPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        const string _mainKey = MainForm._regMainKey;

        const string _subKeyTools = "Tools";
        const string _subKeyToolsPrnPrint = _subKeyTools + @"\PrnPrint";

        const string _nameCaptureFile = "CaptureFile";
        const string _nameFilename = "Filename";

        const string _defaultCaptureFile = "CaptureFile_PrnPrint.prn";
        const string _defaultFilename = "DefaultPrintFile.prn";

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C a p t u r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Prn Print capture file data.                       //
        // The 'current PDL' parameter is not relevant and is ignored.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCapture(ToolCommonData.PrintLang crntPDL, ref string captureFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsPrnPrint))
                {
                    captureFile = (string)subKey.GetValue(_nameCaptureFile, ToolCommonData.DefWorkFolder + "\\" + _defaultCaptureFile);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a G e n e r a l                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored print file data.                                   //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataGeneral(ref string filename)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsPrnPrint))
                {
                    filename = (string)subKey.GetValue(_nameFilename, ToolCommonData.DefWorkFolder + "\\" + _defaultFilename);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a C a p t u r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Prn Print capture file data.                         //
        // The 'current PDL' parameter is not relevant and is ignored.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataCapture(ToolCommonData.PrintLang crntPDL, string captureFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsPrnPrint))
                {
                    if (captureFile != null)
                        subKey.SetValue(_nameCaptureFile, captureFile, RegistryValueKind.String);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a G e n e r a l                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current print file data.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataGeneral(string filename)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsPrnPrint))
                {
                    if (filename != null)
                        subKey.SetValue(_nameFilename, filename, RegistryValueKind.String);
                }
            }
        }
    }
}
