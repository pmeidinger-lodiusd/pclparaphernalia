using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class manages persistent storage of options for the main form.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class MainFormPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        const string _mainKey = MainForm._regMainKey;

        const string _subKeyVersionData = "VersionData";
        const string _subKeyWindowState = "WindowState";

        const string _nameVersionBuild = "Build";
        const string _nameVersionMajor = "Major";
        const string _nameVersionMinor = "Minor";
        const string _nameVersionRevision = "Revision";

        const string _nameMainWindowLeft = "MainWindowLeft";
        const string _nameMainWindowTop = "MainWindowTop";
        const string _nameMainWindowHeight = "MainWindowHeight";
        const string _nameMainWindowWidth = "MainWindowWidth";
        const string _nameMainWindowScale = "MainWindowScale";

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d V e r s i o n D a t a                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored version data; first used after version 2.5.0.0     //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void loadVersionData(ref int major,
                                            ref int minor,
                                            ref int build,
                                            ref int revision)

        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyVersionData;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                major = (int)subKey.GetValue(_nameVersionMajor, -1);
                minor = (int)subKey.GetValue(_nameVersionMinor, -1);
                build = (int)subKey.GetValue(_nameVersionBuild, -1);
                revision = (int)subKey.GetValue(_nameVersionRevision, -1);
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

        public static void loadWindowData(ref int left,
                                           ref int top,
                                           ref int height,
                                           ref int width,
                                           ref int scale)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyWindowState;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                left = (int)subKey.GetValue(_nameMainWindowLeft, -1);
                top = (int)subKey.GetValue(_nameMainWindowTop, -1);
                height = (int)subKey.GetValue(_nameMainWindowHeight, -1);
                width = (int)subKey.GetValue(_nameMainWindowWidth, -1);
                scale = (int)subKey.GetValue(_nameMainWindowScale, 100);
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

        public static void saveVersionData(int major,
                                            int minor,
                                            int build,
                                            int revision)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyVersionData;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameVersionMajor, major,
                                RegistryValueKind.DWord);
                subKey.SetValue(_nameVersionMinor, minor,
                                RegistryValueKind.DWord);
                subKey.SetValue(_nameVersionBuild, build,
                                RegistryValueKind.DWord);
                subKey.SetValue(_nameVersionRevision, revision,
                                RegistryValueKind.DWord);
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

        public static void saveWindowData(int mwLeft,
                                          int mwTop,
                                          int mwHeight,
                                          int mwWidth,
                                          int mwScale)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyWindowState;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameMainWindowLeft, mwLeft,
                                                    RegistryValueKind.DWord);
                subKey.SetValue(_nameMainWindowTop, mwTop,
                                                    RegistryValueKind.DWord);
                subKey.SetValue(_nameMainWindowHeight, mwHeight,
                                                    RegistryValueKind.DWord);
                subKey.SetValue(_nameMainWindowWidth, mwWidth,
                                                    RegistryValueKind.DWord);
                subKey.SetValue(_nameMainWindowScale, mwScale,
                                                    RegistryValueKind.DWord);
            }
        }
    }
}
