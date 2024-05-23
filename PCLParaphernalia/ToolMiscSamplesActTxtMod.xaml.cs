using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// <para>Interaction logic for ToolMiscSamples.xaml</para>
    /// <para>Class handles the MiscSamples Character Embellishments tab.</para>
    /// <para>© Chris Hutchinson 2015</para>
    ///
    /// </summary>
    [System.Reflection.Obfuscation(Feature = "renaming", ApplyToMembers = true)]

    public partial class ToolMiscSamples : Window
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private enum TxtModType : byte
        {
            Chr,
            Pat,
            Rot
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private TxtModType _indxTxtModTypePCL;
        private TxtModType _indxTxtModTypePCLXL;

        private bool _flagTxtModFormAsMacroPCL;
        private bool _flagTxtModFormAsMacroPCLXL;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e D a t a T x t M o d                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialise 'Text modification' data.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void InitialiseDataTxtMod()
        {
            lbOrientation.Visibility = Visibility.Hidden;
            cbOrientation.Visibility = Visibility.Hidden;

            if (_crntPDL == ToolCommonData.PrintLang.PCL)
            {
                if (_indxTxtModTypePCL == TxtModType.Chr)
                {
                    rbTxtModTypeChr.IsChecked = true;
                }
                else if (_indxTxtModTypePCL == TxtModType.Pat)
                {
                    rbTxtModTypePat.IsChecked = true;
                }
                else if (_indxTxtModTypePCL == TxtModType.Rot)
                {
                    rbTxtModTypeRot.IsChecked = true;
                }
                else
                {
                    _indxTxtModTypePCL = TxtModType.Chr;

                    rbTxtModTypeChr.IsChecked = true;
                }

                chkOptFormAsMacro.IsChecked = _flagTxtModFormAsMacroPCL;
            }
            else
            {
                if (_indxTxtModTypePCLXL == TxtModType.Chr)
                {
                    rbTxtModTypeChr.IsChecked = true;
                }
                else if (_indxTxtModTypePCLXL == TxtModType.Pat)
                {
                    rbTxtModTypePat.IsChecked = true;
                }
                else if (_indxTxtModTypePCLXL == TxtModType.Rot)
                {
                    rbTxtModTypeRot.IsChecked = true;
                }
                else
                {
                    _indxTxtModTypePCLXL = TxtModType.Chr;

                    rbTxtModTypeChr.IsChecked = true;
                }

                chkOptFormAsMacro.IsChecked = _flagTxtModFormAsMacroPCL;
            }

            InitialiseDescTxtMod();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e D e s c T x t M o d                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialise 'Text modification' description.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void InitialiseDescTxtMod()
        {
            if (_crntPDL == ToolCommonData.PrintLang.PCL)
            {
                if (_indxTxtModTypePCL == TxtModType.Chr)
                {
                    txtTxtModDesc.Text =
                        "Most font embellishments, apart from the standard" +
                        " style (e.g. italic) and weight (e.g. bold)" +
                        " variations, are provided using HP-GL/2 commands:" +
                        "\r\n\r\n" +
                        "This sample shows:\r\n" +
                        "    X and Y sizing\r\n" +
                        "    character slant - left and right\r\n" +
                        "    spacing: extra space / less space\r\n" +
                        "\r\n" +
                        "Some LaserJet printers support PCL pseudo-bold and" +
                        " pseudo-italic character enhancements." +
                        "\r\n" +
                        "Such enhancements can only be applied to certain" +
                        " fonts, for example the internal MS-Mincho and" +
                        " MS-Gothic fonts, or downloaded PCLETTO fonts which" +
                        " contain a 'Character Enhancement' segment." +
                        "\r\n" +
                        "These pseudo-enhancements are not demonstrated here";
                }
                else if (_indxTxtModTypePCL == TxtModType.Pat)
                {
                    txtTxtModDesc.Text = "Shows samples of Black, White and Patterned text, each printed on Black, White and Patterned backgrounds.";
                }
                else
                {
                    txtTxtModDesc.Text =
                        "Shows samples of Rotated text.\r\n" +
                        "Text rotated by other than 0, 90, 180  or 270 degrees in PCL is achieved via use of HP-GL/2 commands.";
                }
            }
            else
            {
                if (_indxTxtModTypePCLXL == TxtModType.Chr)
                {
                    txtTxtModDesc.Text =
                        "PCL XL supports several different character" +
                        " embellishments:\r\n" +
                        " - Angle\r\n" +
                        " - Pseudo-Bold\r\n" +
                        " - Scale (separate X and Y values)\r\n" +
                        " - Shear (separate X and Y values)";
                }
                else if (_indxTxtModTypePCLXL == TxtModType.Pat)
                {
                    txtTxtModDesc.Text = "Shows samples of Black, White and Patterned text, each printed on Black, White and Patterned backgrounds.";
                }
                else
                {
                    txtTxtModDesc.Text =
                        "Shows samples of Rotated text.\r\n" +
                        "Text rotated by other than 0, 90, 180  or 270" +
                        " degrees in PCL XL is achieved via use of the" +
                        " CharAngle attribute of the SetCharangle operator," +
                        " in conjunction with appropriate XSpacingData and" +
                        " YSpacingData attributes of the Text operator.";
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d D a t a T x t M o d                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load current metrics from persistent storage.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void MetricsLoadDataTxtMod()
        {
            int tmpInt = 0;

            ToolMiscSamplesPersist.LoadDataTypeTxtMod("PCL", ref tmpInt, ref _flagTxtModFormAsMacroPCL);

            if (tmpInt == (int)TxtModType.Pat)
                _indxTxtModTypePCL = TxtModType.Pat;
            else if (tmpInt == (int)TxtModType.Rot)
                _indxTxtModTypePCL = TxtModType.Rot;
            else
                _indxTxtModTypePCL = TxtModType.Chr;

            ToolMiscSamplesPersist.LoadDataTypeTxtMod("PCLXL", ref tmpInt, ref _flagTxtModFormAsMacroPCLXL);

            if (tmpInt == (int)TxtModType.Pat)
                _indxTxtModTypePCLXL = TxtModType.Pat;
            else if (tmpInt == (int)TxtModType.Rot)
                _indxTxtModTypePCLXL = TxtModType.Rot;
            else
                _indxTxtModTypePCLXL = TxtModType.Chr;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s S a v e D a t a T x t M o d                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Save current metrics to persistent storage.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void MetricsSaveDataTxtMod()
        {
            ToolMiscSamplesPersist.SaveDataTypeTxtMod("PCL", (int)_indxTxtModTypePCL, _flagTxtModFormAsMacroPCL);

            ToolMiscSamplesPersist.SaveDataTypeTxtMod("PCLXL", (int)_indxPatternTypePCLXL, _flagTxtModFormAsMacroPCLXL);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b T x t M o d T y p e C h r _ C l i c k                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting 'Character enhancement' text modification.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbTxtModTypeChr_Click(object sender, RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.PrintLang.PCL)
                _indxTxtModTypePCL = TxtModType.Chr;
            else
                _indxTxtModTypePCLXL = TxtModType.Chr;

            InitialiseDescTxtMod();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b T x t M o d T y p e P a t _ C l i c k                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting 'Text & background' text modification.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbTxtModTypePat_Click(object sender, RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.PrintLang.PCL)
                _indxTxtModTypePCL = TxtModType.Pat;
            else
                _indxTxtModTypePCLXL = TxtModType.Pat;

            InitialiseDescTxtMod();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b T x t M o d T y p e R o t _ C l i c k                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting 'Rotation' text modification.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbTxtModTypeRot_Click(object sender, RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.PrintLang.PCL)
                _indxTxtModTypePCL = TxtModType.Rot;
            else
                _indxTxtModTypePCLXL = TxtModType.Rot;

            InitialiseDescTxtMod();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F l a g T x t M o d F o r m A s M a c r o                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set or unset 'Render fixed text as overlay' flag.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetFlagTxtModFormAsMacro(bool setFlag, ToolCommonData.PrintLang crntPDL)
        {
            if (crntPDL == ToolCommonData.PrintLang.PCL)
            {
                _flagTxtModFormAsMacroPCL = setFlag;
            }
            else if (crntPDL == ToolCommonData.PrintLang.PCLXL)
            {
                _flagTxtModFormAsMacroPCLXL = setFlag;
            }
        }
    }
}
