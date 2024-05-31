using Microsoft.Win32;
using System;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class manages persistent storage of Target options.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class TargetPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const string _mainKey = MainForm._regMainKey;
        private const string _subKeyTarget = "Target";

        //const string _subKeyTargetFile = "File";
        private const string _subKeyTargetPrinter = "Printer";

        private const string _subKeyTargetNetPrinter = "NetPrinter";
        private const string _subKeyTargetWinPrinter = "WinPrinter";
        private const string _subKeyWorkFolder = "WorkFolder";
        private const string _nameIndxTargetType = "IndxTargetType";

        //const string _nameFilename = "Filename";
        private const string _nameFoldername = "Foldername";

        private const string _namePrintername = "Printername";
        private const string _nameIPAddress = "IPAddress";
        private const string _namePort = "Port";
        private const string _nameTimeoutSend = "TimeoutMsecsSend";
        private const string _nameTimeoutReceive = "TimeoutMsecsReceive";

        //const string _defaultFilename = "ItemNoLongerUsed";
        private const string _defaultPrintername = "<None>";

        private const string _defaultIPAddress = "192.168.0.98";
        private const int _indexZero = 0;
        private const int _defaultNetPort = 9100;
        private const int _defaultNetTimeoutSend = 15000;
        private const int _defaultNetTimeoutReceive = 10000;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a C o m m o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored common Target data.                                //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadDataCommon(ref int indxTargetType)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyTarget))
                {
                    indxTargetType = (int)subKey.GetValue(_nameIndxTargetType, _indexZero);
                }
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

        public static void LoadDataNetPrinter(ref string ipAddress,
                                               ref int port,
                                               ref int timeoutSend,
                                               ref int timeoutReceive)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyTarget + "\\" + _subKeyTargetNetPrinter;

                //----------------------------------------------------------------//

                using (var subKey = keyMain.CreateSubKey(_subKeyTarget))
                {
                    if (Helper_RegKey.KeyExists(subKey, _subKeyTargetPrinter))
                    {
                        // update from v2_5_0_0
                        Helper_RegKey.RenameKey(subKey, _subKeyTargetPrinter, _subKeyTargetNetPrinter);
                    }
                }

                //----------------------------------------------------------------//

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    ipAddress = (string)subKey.GetValue(_nameIPAddress, _defaultIPAddress);

                    port = (int)subKey.GetValue(_namePort, _defaultNetPort);

                    timeoutSend = (int)subKey.GetValue(_nameTimeoutSend, _defaultNetTimeoutSend);

                    timeoutReceive = (int)subKey.GetValue(_nameTimeoutReceive, _defaultNetTimeoutReceive);
                }
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

        public static void LoadDataWinPrinter(ref string printerName)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyTarget + "\\" + _subKeyTargetWinPrinter;

                //----------------------------------------------------------------//

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    printerName = (string)subKey.GetValue(_namePrintername, _defaultPrintername);
                }
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

        public static void LoadDataWorkFolder(ref string foldername)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyTarget + "\\" + _subKeyWorkFolder;

                string defWorkFolder = Environment.GetEnvironmentVariable("TMP");

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    foldername = (string)subKey.GetValue(_nameFoldername, defWorkFolder);
                }
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

        public static void SaveDataCommon(int indxTargetType)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyTarget))
                {
                    subKey.SetValue(_nameIndxTargetType, indxTargetType, RegistryValueKind.DWord);
                }
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

        public static void SaveDataNetPrinter(int indxTargetType,
                                               string ipAddress,
                                               int port,
                                               int timeoutSend,
                                               int timeoutReceive)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyTarget))
                {
                    subKey.SetValue(_nameIndxTargetType, indxTargetType, RegistryValueKind.DWord);
                }

                const string key = _subKeyTarget + "\\" + _subKeyTargetNetPrinter;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameIPAddress, ipAddress, RegistryValueKind.String);

                    subKey.SetValue(_namePort, port, RegistryValueKind.DWord);

                    subKey.SetValue(_nameTimeoutSend, timeoutSend, RegistryValueKind.DWord);

                    subKey.SetValue(_nameTimeoutReceive, timeoutReceive, RegistryValueKind.DWord);
                }
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

        public static void SaveDataWinPrinter(int indxTargetType, string printerName)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyTarget))
                {
                    subKey.SetValue(_nameIndxTargetType, indxTargetType, RegistryValueKind.DWord);
                }

                const string key = _subKeyTarget + "\\" + _subKeyTargetWinPrinter;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_namePrintername, printerName, RegistryValueKind.String);
                }
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

        public static void SaveDataWorkFolder(string saveFoldername)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                const string key = _subKeyTarget + "\\" + _subKeyWorkFolder;

                using (var subKey = keyMain.CreateSubKey(key))
                {
                    subKey.SetValue(_nameFoldername, saveFoldername, RegistryValueKind.String);
                }
            }
        }
    }
}