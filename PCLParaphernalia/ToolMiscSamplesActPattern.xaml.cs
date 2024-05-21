using System;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// <para>Interaction logic for ToolMiscSamples.xaml</para>
    /// <para>Class handles the MiscSamples: Patterns tab.</para>
    /// <para>© Chris Hutchinson 2015</para>
    ///
    /// </summary>
    [System.Reflection.Obfuscation(Feature = "renaming",
                                            ApplyToMembers = true)]

    public partial class ToolMiscSamples : Window
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private enum PatternType : byte
        {
            Shading,
            XHatch
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private PatternType _indxPatternTypePCL;
        private PatternType _indxPatternTypePCLXL;

        private bool _flagPatternFormAsMacroPCL;
        private bool _flagPatternFormAsMacroPCLXL;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e D a t a P a t t e r n                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialise 'Patterns' data.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void InitialiseDataPattern()
        {
            lbOrientation.Visibility = Visibility.Hidden;
            cbOrientation.Visibility = Visibility.Hidden;

            if (_crntPDL == ToolCommonData.PrintLang.PCL)
            {
                if (_indxPatternTypePCL == PatternType.Shading)
                {
                    rbPatternTypeShading.IsChecked = true;
                }
                else if (_indxPatternTypePCL == PatternType.XHatch)
                {
                    rbPatternTypeXHatch.IsChecked = true;
                }
                else
                {
                    _indxPatternTypePCL = PatternType.Shading;

                    rbPatternTypeShading.IsChecked = true;
                }

                chkOptFormAsMacro.IsChecked = _flagPatternFormAsMacroPCL;
            }
            else
            {
                if (_indxPatternTypePCLXL == PatternType.Shading)
                {
                    rbPatternTypeShading.IsChecked = true;
                }
                else if (_indxPatternTypePCLXL == PatternType.XHatch)
                {
                    rbPatternTypeXHatch.IsChecked = true;
                }
                else
                {
                    _indxPatternTypePCLXL = PatternType.Shading;

                    rbPatternTypeShading.IsChecked = true;
                }

                chkOptFormAsMacro.IsChecked = _flagPatternFormAsMacroPCLXL;
            }

            InitialiseDescPattern();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e D e s c P a t t e r n                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialise 'Patterns' description.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void InitialiseDescPattern()
        {
            if (_crntPDL == ToolCommonData.PrintLang.PCL)
            {
                if (_indxPatternTypePCL == PatternType.Shading)
                {
                    txtPatternDesc.Text =
                        "Shows samples of built-in and user-defined" +
                        " Shading patterns.";
                }
                else
                {
                    txtPatternDesc.Text =
                        "Shows samples of built-in and user-defined" +
                        " Cross-Hatch patterns.";
                }
            }
            else
            {
                if (_indxPatternTypePCLXL == PatternType.Shading)
                {
                    txtPatternDesc.Text =
                        "Shows samples of user-defined Shading patterns.";
                }
                else
                {
                    txtPatternDesc.Text =
                        "Shows samples of user-defined Cross-hatch patterns.";
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d D a t a P a t t e r n                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load current metrics from persistent storage.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void MetricsLoadDataPattern()
        {
            int tmpInt = 0;

            ToolMiscSamplesPersist.loadDataTypePattern(
                "PCL",
                ref tmpInt,
                ref _flagPatternFormAsMacroPCL);

            if (tmpInt == (int)PatternType.XHatch)
                _indxPatternTypePCL = PatternType.XHatch;
            else
                _indxPatternTypePCL = PatternType.Shading;

            ToolMiscSamplesPersist.loadDataTypePattern(
                "PCLXL",
                ref tmpInt,
                ref _flagPatternFormAsMacroPCLXL);

            if (tmpInt == (int)PatternType.XHatch)
                _indxPatternTypePCLXL = PatternType.XHatch;
            else
                _indxPatternTypePCLXL = PatternType.Shading;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s S a v e D a t a P a t t e r n                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Save current metrics to persistent storage.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void MetricsSaveDataPattern()
        {
            ToolMiscSamplesPersist.saveDataTypePattern(
                "PCL",
                (int)_indxPatternTypePCL,
                _flagPatternFormAsMacroPCL);

            ToolMiscSamplesPersist.saveDataTypePattern(
                "PCLXL",
                (int)_indxPatternTypePCLXL,
                _flagPatternFormAsMacroPCLXL);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P a t t e r n T y p e S h a d i n g _ C l i c k                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting 'Shading' patterns.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void RbPatternTypeShading_Click(object sender,
                                                 RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.PrintLang.PCL)
                _indxPatternTypePCL = PatternType.Shading;
            else
                _indxPatternTypePCLXL = PatternType.Shading;

            InitialiseDescPattern();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P a t t e r n T y p e X H a t c h _ C l i c k                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting 'Cross-hatch' patterns.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void RbPatternTypeXHatch_Click(object sender,
                                                RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.PrintLang.PCL)
                _indxPatternTypePCL = PatternType.XHatch;
            else
                _indxPatternTypePCLXL = PatternType.XHatch;

            InitialiseDescPattern();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F l a g P a t t e r n F o r m A s M a c r o                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set or unset 'Render fixed text as overlay' flag.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetFlagPatternFormAsMacro(
            bool setFlag,
            ToolCommonData.PrintLang crntPDL)
        {
            if (crntPDL == ToolCommonData.PrintLang.PCL)
            {
                _flagPatternFormAsMacroPCL = setFlag;
            }
            else if (crntPDL == ToolCommonData.PrintLang.PCLXL)
            {
                _flagPatternFormAsMacroPCLXL = setFlag;
            }
        }
    }
}
