﻿using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class manages persistent storage of common Tool options.</para>
    /// <para>© Chris Hutchinson 2011</para>
    ///
    /// </summary>
    static class ToolCommonPersist
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        const string _mainKey = MainForm._regMainKey;

        const string _subKeyTools = "Tools";

        const string _nameIndxToolType = "IndxToolType";

        const int _indexZero = 0;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d D a t a                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve stored common Tool data.                                  //
        // Missing items are given default values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LoadData(ref int indxToolType)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyTools))
                {
                    indxToolType = (int)subKey.GetValue(_nameIndxToolType, _indexZero);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s a v e D a t a                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store current common Tool data.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SaveData(int indxToolType)
        {
            using (var keyMain = Registry.CurrentUser.CreateSubKey(_mainKey))
            {
                using (var subKey = keyMain.CreateSubKey(_subKeyTools))
                {
                    subKey.SetValue(_nameIndxToolType, indxToolType, RegistryValueKind.DWord);
                }
            }
        }
    }
}
