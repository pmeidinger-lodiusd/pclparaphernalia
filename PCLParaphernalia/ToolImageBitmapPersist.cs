using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class manages persistent storage of options for the ImageBitmap tool.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class ToolImageBitmapPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const string _mainKey = MainForm._regMainKey;
        private const string _subKeyTools = "Tools";
        private const string _subKeyToolsImageBitmap = _subKeyTools + @"\ImageBitmap";
        private const string _subKeyPCL5 = "PCL5";
        private const string _subKeyPCL6 = "PCL6";
        private const string _subKeyPCL = "PCL";
        private const string _subKeyPCLXL = "PCLXL";
        private const string _nameCaptureFile = "CaptureFile";
        private const string _nameCoordX = "CoordX";
        private const string _nameCoordY = "CoordY";
        private const string _nameFilename = "Filename";
        private const string _nameIndxOrientation = "IndxOrientation";
        private const string _nameIndxPaperSize = "IndxPaperSize";
        private const string _nameIndxPaperType = "IndxPaperType";
        private const string _nameIndxPDL = "IndxPDL";
        private const string _nameIndxRasterRes = "IndxRasterRes";
        private const string _nameScaleX = "ScaleX";
        private const string _nameScaleY = "ScaleY";
        private const int _indexZero = 0;

        //const string _defaultCaptureFile = "CaptureFile_ImageBitmap.prn";
        private const string _defaultCaptureFilePCL = "CaptureFile_ImageBitmapPCL.prn";

        private const string _defaultCaptureFilePCLXL = "CaptureFile_ImageBitmapPCLXL.prn";
        private const string _defaultFilename = "DefaultImageFile.bmp";
        private const int _defaultCoord = 300;
        private const int _defaultScale = 100;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C a p t u r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Image Bitmap capture file data.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCapture(ToolCommonData.PrintLang crntPDL, ref string captureFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string oldKey = _subKeyToolsImageBitmap;

                string oldFile;

                bool update_from_v2_5_0_0 = false;

                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.OpenSubKey(oldKey, true))
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
                    const string keyPCL = _subKeyToolsImageBitmap + "\\" + _subKeyPCL;

                    using (var subKey = keyMain.CreateSubKey(keyPCL))
                    {
                        subKey.SetValue(_nameCaptureFile, oldFile, RegistryValueKind.String);
                    }

                    const string keyPCLXL = _subKeyToolsImageBitmap + "\\" + _subKeyPCLXL;

                    using (var subKey = keyMain.CreateSubKey(keyPCLXL))
                    {
                        subKey.SetValue(_nameCaptureFile, oldFile, RegistryValueKind.String);
                    }
                }

                if (crntPDL == ToolCommonData.PrintLang.PCL)
                {
                    const string key = _subKeyToolsImageBitmap + "\\" + _subKeyPCL;

                    using (var subKey = keyMain.CreateSubKey(key))
                    {
                        captureFile = (string)subKey.GetValue(_nameCaptureFile, defWorkFolder + "\\" + _defaultCaptureFilePCL);
                    }
                }
                else if (crntPDL == ToolCommonData.PrintLang.PCLXL)
                {
                    const string key = _subKeyToolsImageBitmap + "\\" + _subKeyPCLXL;

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
        // Retrieve stored Bitmap common data.                                //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCommon(ref int indxPDL,
                                          ref string filename,
                                          ref int destPosX,
                                          ref int destPosY,
                                          ref int destScaleX,
                                          ref int destScaleY,
                                          ref int indxRasterRes)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string defWorkFolder = ToolCommonData.DefWorkFolder;

                using (var subKey = keyMain.CreateSubKey(_subKeyToolsImageBitmap))
                {
                    indxPDL = (int)subKey.GetValue(_nameIndxPDL, _indexZero);

                    filename = (string)subKey.GetValue(_nameFilename, defWorkFolder + "\\" + _defaultFilename);

                    destPosX = (int)subKey.GetValue(_nameCoordX, _defaultCoord);

                    destPosY = (int)subKey.GetValue(_nameCoordY, _defaultCoord);

                    destScaleX = (int)subKey.GetValue(_nameScaleX, _defaultScale);

                    destScaleY = (int)subKey.GetValue(_nameScaleY, _defaultScale);

                    indxRasterRes = (int)subKey.GetValue(_nameIndxRasterRes, _indexZero);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a P C L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Bitmap PCL or PCLXL data.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataPCL(string pdlName,
                                       ref int indxOrientation,
                                       ref int indxPaperSize,
                                       ref int indxPaperType)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string key = _subKeyToolsImageBitmap + "\\" + pdlName;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    indxOrientation = (int)subKey.GetValue(_nameIndxOrientation, _indexZero);
                    indxPaperSize = (int)subKey.GetValue(_nameIndxPaperSize, _indexZero);
                    indxPaperType = (int)subKey.GetValue(_nameIndxPaperType, _indexZero);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a C a p t u r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Image Bitmap capture file data.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataCapture(ToolCommonData.PrintLang crntPDL, string captureFile)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                if (crntPDL == ToolCommonData.PrintLang.PCL)
                {
                    const string key = _subKeyToolsImageBitmap + "\\" + _subKeyPCL;

                    using (var subKey = keyMain.CreateSubKey(key))
                    {
                        if (captureFile != null)
                            subKey.SetValue(_nameCaptureFile, captureFile, RegistryValueKind.String);
                    }
                }
                else if (crntPDL == ToolCommonData.PrintLang.PCLXL)
                {
                    const string key = _subKeyToolsImageBitmap + "\\" + _subKeyPCLXL;

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
        // Store current Bitmap common data.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataCommon(int indxPDL,
                                          string filename,
                                          int destPosX,
                                          int destPosY,
                                          int destScaleX,
                                          int destScaleY,
                                          int indxRasterRes)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyToolsImageBitmap))
                {
                    subKey.SetValue(_nameIndxPDL, indxPDL, RegistryValueKind.DWord);

                    if (filename != null)
                        subKey.SetValue(_nameFilename, filename, RegistryValueKind.String);

                    subKey.SetValue(_nameCoordX, destPosX, RegistryValueKind.DWord);

                    subKey.SetValue(_nameCoordY, destPosY, RegistryValueKind.DWord);

                    subKey.SetValue(_nameScaleX, destScaleX, RegistryValueKind.DWord);

                    subKey.SetValue(_nameScaleY, destScaleY, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxRasterRes, indxRasterRes, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a P C L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Bitmap PCL or PCLXL data.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveDataPCL(string pdlName,
                                       int indxOrientation,
                                       int indxPaperSize,
                                       int indxPaperType)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                string key = _subKeyToolsImageBitmap + "\\" + pdlName;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameIndxOrientation, indxOrientation, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxPaperSize, indxPaperSize, RegistryValueKind.DWord);

                    subKey.SetValue(_nameIndxPaperType, indxPaperType, RegistryValueKind.DWord);
                }
            }
        }
    }
}