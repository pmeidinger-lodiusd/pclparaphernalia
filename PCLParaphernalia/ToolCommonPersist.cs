using Microsoft.Win32;
using System;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class manages persistent storage of common Tool options.
    /// 
    /// © Chris Hutchinson 2011
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

        public static void loadData(ref int indxToolType)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                indxToolType = (int)subKey.GetValue(_nameIndxToolType,
                                                      _indexZero);
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

        public static void saveData(int indxToolType)
        {
            RegistryKey keyMain =
                Registry.CurrentUser.CreateSubKey(_mainKey);

            string key = _subKeyTools;

            using (RegistryKey subKey = keyMain.CreateSubKey(key))
            {
                subKey.SetValue(_nameIndxToolType,
                                indxToolType,
                                RegistryValueKind.DWord);
            }
        }
    }
}
