using Microsoft.Win32;
using System;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class manages persistent storage of options for the ImageBitmap tool.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class ToolImageBitmapPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        const string _mainKey = MainForm._regMainKey;

        const string _subKeyTools = "Tools";
        const string _subKeyToolsImageBitmap = "ImageBitmap";
        const string _subKeyPCL5 = "PCL5";
        const string _subKeyPCL6 = "PCL6";
        const string _subKeyPCL = "PCL";
        const string _subKeyPCLXL = "PCLXL";

        const string _nameCaptureFile = "CaptureFile";
        const string _nameCoordX = "CoordX";
        const string _nameCoordY = "CoordY";
        const string _nameFilename = "Filename";
        const string _nameIndxOrientation = "IndxOrientation";
        const string _nameIndxPaperSize = "IndxPaperSize";
        const string _nameIndxPaperType = "IndxPaperType";
        const string _nameIndxPDL = "IndxPDL";
        const string _nameIndxRasterRes = "IndxRasterRes";
        const string _nameScaleX = "ScaleX";
        const string _nameScaleY = "ScaleY";

        const int _indexZero = 0;

        const string _defaultCaptureFile = "CaptureFile_ImageBitmap.prn";
        const string _defaultCaptureFilePCL = "CaptureFile_ImageBitmapPCL.prn";
        const string _defaultCaptureFilePCLXL = "CaptureFile_ImageBitmapPCLXL.prn";
        const string _defaultFilename = "DefaultImageFile.bmp";

        const int _defaultCoord = 300;
        const int _defaultScale = 100;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C a p t u r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Image Bitmap capture file data.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void loadDataCapture(ToolCommonData.ePrintLang crntPDL,
                                            ref string captureFile)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string oldKey = _subKeyTools + "\\" + _subKeyToolsImageBitmap;
            string oldFile;

            bool update_from_v2_5_0_0 = false;

            string defWorkFolder = ToolCommonData.DefWorkFolder;

            using (RegistryKey subKey = keyMain.OpenSubKey(oldKey, true))
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
                string keyPCL = _subKeyTools +
                                 "\\" + _subKeyToolsImageBitmap +
                                 "\\" + _subKeyPCL;

                using (RegistryKey subKey = keyMain.CreateSubKey(keyPCL))
                {
                    subKey.SetValue(_nameCaptureFile,
                                     oldFile,
                                     RegistryValueKind.String);
                }

                string keyPCLXL = _subKeyTools +
                                 "\\" + _subKeyToolsImageBitmap +
                                 "\\" + _subKeyPCLXL;

                using (RegistryKey subKey = keyMain.CreateSubKey(keyPCLXL))
                {
                    subKey.SetValue(_nameCaptureFile,
                                     oldFile,
                                     RegistryValueKind.String);
                }
            }

            if (crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                string key = _subKeyTools + "\\" + _subKeyToolsImageBitmap +
                                            "\\" + _subKeyPCL;

                using (RegistryKey subKey = keyMain.CreateSubKey(key))
                {
                    captureFile = (string)subKey.GetValue(
                        _nameCaptureFile,
                        defWorkFolder + "\\" + _defaultCaptureFilePCL);
                }
            }
            else if (crntPDL == ToolCommonData.ePrintLang.PCLXL)
            {
                string key = _subKeyTools + "\\" + _subKeyToolsImageBitmap +
                                            "\\" + _subKeyPCLXL;

                using (RegistryKey subKey = keyMain.CreateSubKey(key))
                {
                    captureFile = (string)subKey.GetValue(
                        _nameCaptureFile,
                        defWorkFolder + "\\" + _defaultCaptureFilePCLXL);
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

        public static void loadDataCommon(ref int indxPDL,
                                          ref string filename,
                                          ref int destPosX,
                                          ref int destPosY,
                                          ref int destScaleX,
                                          ref int destScaleY,
                                          ref int indxRasterRes)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsImageBitmap;

            string defWorkFolder = ToolCommonData.DefWorkFolder;

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

                filename = (string)subKey.GetValue(_nameFilename,
                                                        defWorkFolder + "\\" +
                                                        _defaultFilename);

                destPosX = (int)subKey.GetValue(_nameCoordX,
                                                       _defaultCoord);

                destPosY = (int)subKey.GetValue(_nameCoordY,
                                                       _defaultCoord);

                destScaleX = (int)subKey.GetValue(_nameScaleX,
                                                       _defaultScale);

                destScaleY = (int)subKey.GetValue(_nameScaleY,
                                                       _defaultScale);

                indxRasterRes = (int)subKey.GetValue(_nameIndxRasterRes,
                                                       _indexZero);
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

        public static void loadDataPCL(string pdlName,
                                       ref int indxOrientation,
                                       ref int indxPaperSize,
                                       ref int indxPaperType)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsImageBitmap +
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
        // s a v e D a t a C a p t u r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Image Bitmap capture file data.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void saveDataCapture(ToolCommonData.ePrintLang crntPDL,
                                            string captureFile)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            if (crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                string key = _subKeyTools + "\\" + _subKeyToolsImageBitmap +
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
                string key = _subKeyTools + "\\" + _subKeyToolsImageBitmap +
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
        // Store current Bitmap common data.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void saveDataCommon(int indxPDL,
                                          string filename,
                                          int destPosX,
                                          int destPosY,
                                          int destScaleX,
                                          int destScaleY,
                                          int indxRasterRes)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools + "\\" + _subKeyToolsImageBitmap;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxPDL,
                                indxPDL,
                                RegistryValueKind.DWord);

                if (filename != null)
                {
                    subKey.SetValue(_nameFilename,
                                    filename,
                                    RegistryValueKind.String);
                }

                subKey.SetValue(_nameCoordX,
                                destPosX,
                                RegistryValueKind.DWord);

                subKey.SetValue(_nameCoordY,
                                destPosY,
                                RegistryValueKind.DWord);

                subKey.SetValue(_nameScaleX,
                                destScaleX,
                                RegistryValueKind.DWord);

                subKey.SetValue(_nameScaleY,
                                destScaleY,
                                RegistryValueKind.DWord);

                subKey.SetValue(_nameIndxRasterRes,
                                indxRasterRes,
                                RegistryValueKind.DWord);
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

        public static void saveDataPCL(string pdlName,
                                       int indxOrientation,
                                       int indxPaperSize,
                                       int indxPaperType)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key;

            key = _subKeyTools + "\\" + _subKeyToolsImageBitmap +
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
    }
}
