using Microsoft.Win32;
using System;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class manages persistent storage of Target options.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class TargetPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        const string _mainKey = MainForm._regMainKey;

        const string _subKeyTarget = "Target";
        const string _subKeyTargetFile = "File";
        const string _subKeyTargetPrinter = "Printer";
        const string _subKeyTargetNetPrinter = "NetPrinter";
        const string _subKeyTargetWinPrinter = "WinPrinter";
        const string _subKeyWorkFolder = "WorkFolder";

        const string _nameIndxTargetType = "IndxTargetType";
        const string _nameFilename = "Filename";
        const string _nameFoldername = "Foldername";
        const string _namePrintername = "Printername";
        const string _nameIPAddress = "IPAddress";
        const string _namePort = "Port";
        const string _nameTimeoutSend = "TimeoutMsecsSend";
        const string _nameTimeoutReceive = "TimeoutMsecsReceive";

        const string _defaultFilename = "ItemNoLongerUsed";
        const string _defaultPrintername = "<None>";
        const string _defaultIPAddress = "192.168.0.98";

        const int _indexZero = 0;
        const int _defaultNetPort = 9100;

        const int _defaultNetTimeoutSend = 15000;
        const int _defaultNetTimeoutReceive = 10000;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C o m m o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored common Target data.                                //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void loadDataCommon(ref int indxTargetType)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTarget;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxTargetType = (int)subKey.GetValue(_nameIndxTargetType,
                                                        _indexZero);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a N e t P r i n t e r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Target network printer data.                       //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void loadDataNetPrinter(ref string ipAddress,
                                               ref int port,
                                               ref int timeoutSend,
                                               ref int timeoutReceive)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTarget + "\\" + _subKeyTargetNetPrinter;

            //----------------------------------------------------------------//

            using (RegistryKey subKey = keyMain.CreateSubKey(_subKeyTarget))
            {
                if (Helper_RegKey.keyExists(subKey, _subKeyTargetPrinter))
                    // update from v2_5_0_0
                    Helper_RegKey.renameKey(subKey,
                                           _subKeyTargetPrinter,
                                           _subKeyTargetNetPrinter);
            }

            //----------------------------------------------------------------//

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                ipAddress = (string)subKey.GetValue(_nameIPAddress,
                                                     _defaultIPAddress);

                port = (int)subKey.GetValue(_namePort,
                                               _defaultNetPort);

                timeoutSend = (int)subKey.GetValue(_nameTimeoutSend,
                                               _defaultNetTimeoutSend);

                timeoutReceive = (int)subKey.GetValue(_nameTimeoutReceive,
                                               _defaultNetTimeoutReceive);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a W i n P r i n t e r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored Target windows printer data.                       //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void loadDataWinPrinter(ref string printerName)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTarget + "\\" + _subKeyTargetWinPrinter;

            //----------------------------------------------------------------//

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                printerName = (string)subKey.GetValue(_namePrintername,
                                                        _defaultPrintername);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a W o r k F ol d e r                                 //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored default working folder data.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void loadDataWorkFolder(ref string foldername)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTarget + "\\" + _subKeyWorkFolder;

            string defWorkFolder = Environment.GetEnvironmentVariable("TMP");

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                foldername = (string)subKey.GetValue(_nameFoldername,
                                                     defWorkFolder);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a C o m m o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current common Target data.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void saveDataCommon(int indxTargetType)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTarget;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxTargetType,
                                 indxTargetType,
                                 RegistryValueKind.DWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a N e t P r i n t e r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Target Network Printer data.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void saveDataNetPrinter(int indxTargetType,
                                               string ipAddress,
                                               int port,
                                               int timeoutSend,
                                               int timeoutReceive)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTarget;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxTargetType,
                                 indxTargetType,
                                 RegistryValueKind.DWord);
            }

            key = _subKeyTarget + "\\" + _subKeyTargetNetPrinter;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIPAddress,
                                 ipAddress,
                                 RegistryValueKind.String);

                subKey.SetValue(_namePort,
                                 port,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameTimeoutSend,
                                 timeoutSend,
                                 RegistryValueKind.DWord);

                subKey.SetValue(_nameTimeoutReceive,
                                 timeoutReceive,
                                 RegistryValueKind.DWord);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a W i n P r i n t e r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current Target Windows Printer data.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void saveDataWinPrinter(int indxTargetType,
                                               string printerName)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTarget;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxTargetType,
                                 indxTargetType,
                                 RegistryValueKind.DWord);
            }

            key = _subKeyTarget + "\\" + _subKeyTargetWinPrinter;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_namePrintername,
                                 printerName,
                                 RegistryValueKind.String);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a W o r k F o l d e r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current default working folder data.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void saveDataWorkFolder(string saveFoldername)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTarget + "\\" + _subKeyWorkFolder;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameFoldername,
                                saveFoldername,
                                RegistryValueKind.String);
            }
        }
    }
}
