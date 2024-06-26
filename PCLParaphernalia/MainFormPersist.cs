﻿using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class manages persistent storage of options for the main form.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class MainFormPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const string _mainKey = MainForm._regMainKey;
        private const string _subKeyVersionData = "VersionData";
        private const string _subKeyWindowState = "WindowState";
        private const string _nameVersionBuild = "Build";
        private const string _nameVersionMajor = "Major";
        private const string _nameVersionMinor = "Minor";
        private const string _nameVersionRevision = "Revision";
        private const string _nameMainWindowLeft = "MainWindowLeft";
        private const string _nameMainWindowTop = "MainWindowTop";
        private const string _nameMainWindowHeight = "MainWindowHeight";
        private const string _nameMainWindowWidth = "MainWindowWidth";
        private const string _nameMainWindowScale = "MainWindowScale";

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d V e r s i o n D a t a                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored version data; first used after version 2.5.0.0     //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadVersionData(out int major, out int minor, out int build, out int revision)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyVersionData))
                {
                    major = (int)subKey.GetValue(_nameVersionMajor, -1);
                    minor = (int)subKey.GetValue(_nameVersionMinor, -1);
                    build = (int)subKey.GetValue(_nameVersionBuild, -1);
                    revision = (int)subKey.GetValue(_nameVersionRevision, -1);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d W i n d o w D a t a                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored windows state data.                                //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadWindowData(out int left, out int top, out int height, out int width, out int scale)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyWindowState))
                {
                    left = (int)subKey.GetValue(_nameMainWindowLeft, -1);
                    top = (int)subKey.GetValue(_nameMainWindowTop, -1);
                    height = (int)subKey.GetValue(_nameMainWindowHeight, -1);
                    width = (int)subKey.GetValue(_nameMainWindowWidth, -1);
                    scale = (int)subKey.GetValue(_nameMainWindowScale, 100);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e V e r s i o n D a t a                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current version data.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveVersionData(int major, int minor, int build, int revision)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyVersionData))
                {
                    subKey.SetValue(_nameVersionMajor, major, RegistryValueKind.DWord);
                    subKey.SetValue(_nameVersionMinor, minor, RegistryValueKind.DWord);
                    subKey.SetValue(_nameVersionBuild, build, RegistryValueKind.DWord);
                    subKey.SetValue(_nameVersionRevision, revision, RegistryValueKind.DWord);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e W i n d o w D a t a                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current window state.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveWindowData(int mwLeft, int mwTop, int mwHeight, int mwWidth, int mwScale)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyWindowState))
                {
                    subKey.SetValue(_nameMainWindowLeft, mwLeft, RegistryValueKind.DWord);
                    subKey.SetValue(_nameMainWindowTop, mwTop, RegistryValueKind.DWord);
                    subKey.SetValue(_nameMainWindowHeight, mwHeight, RegistryValueKind.DWord);
                    subKey.SetValue(_nameMainWindowWidth, mwWidth, RegistryValueKind.DWord);
                    subKey.SetValue(_nameMainWindowScale, mwScale, RegistryValueKind.DWord);
                }
            }
        }
    }
}