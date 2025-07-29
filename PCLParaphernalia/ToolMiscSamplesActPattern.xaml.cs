using System.Windows;

namespace PCLParaphernalia;

/// <summary>
/// Interaction logic for ToolMiscSamples.xaml
/// 
/// Class handles the MiscSamples: Patterns tab.
/// 
/// © Chris Hutchinson 2015
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

    private enum ePatternType : byte
    {
        Shading,
        XHatch
    }

    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Fields (class variables).                                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private ePatternType _indxPatternTypePCL;
    private ePatternType _indxPatternTypePCLXL;

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

        if (_crntPDL == ToolCommonData.ePrintLang.PCL)
        {
            if (_indxPatternTypePCL == ePatternType.Shading)
            {
                rbPatternTypeShading.IsChecked = true;
            }
            else if (_indxPatternTypePCL == ePatternType.XHatch)
            {
                rbPatternTypeXHatch.IsChecked = true;
            }
            else
            {
                _indxPatternTypePCL = ePatternType.Shading;

                rbPatternTypeShading.IsChecked = true;
            }

            chkOptFormAsMacro.IsChecked = _flagPatternFormAsMacroPCL;
        }
        else
        {
            if (_indxPatternTypePCLXL == ePatternType.Shading)
            {
                rbPatternTypeShading.IsChecked = true;
            }
            else if (_indxPatternTypePCLXL == ePatternType.XHatch)
            {
                rbPatternTypeXHatch.IsChecked = true;
            }
            else
            {
                _indxPatternTypePCLXL = ePatternType.Shading;

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
        if (_crntPDL == ToolCommonData.ePrintLang.PCL)
        {
            if (_indxPatternTypePCL == ePatternType.Shading)
                txtPatternDesc.Text = "Shows samples of built-in and user-defined Shading patterns.";
            else
                txtPatternDesc.Text = "Shows samples of built-in and user-defined Cross-Hatch patterns.";
        }
        else
        {
            if (_indxPatternTypePCLXL == ePatternType.Shading)
                txtPatternDesc.Text = "Shows samples of user-defined Shading patterns.";
            else
                txtPatternDesc.Text = "Shows samples of user-defined Cross-hatch patterns.";
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

        ToolMiscSamplesPersist.LoadDataTypePattern(
            "PCL",
            ref tmpInt,
            ref _flagPatternFormAsMacroPCL);

        if (tmpInt == (int)ePatternType.XHatch)
            _indxPatternTypePCL = ePatternType.XHatch;
        else
            _indxPatternTypePCL = ePatternType.Shading;

        ToolMiscSamplesPersist.LoadDataTypePattern(
            "PCLXL",
            ref tmpInt,
            ref _flagPatternFormAsMacroPCLXL);

        if (tmpInt == (int)ePatternType.XHatch)
            _indxPatternTypePCLXL = ePatternType.XHatch;
        else
            _indxPatternTypePCLXL = ePatternType.Shading;
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
        ToolMiscSamplesPersist.SaveDataTypePattern(
            "PCL",
            (int)_indxPatternTypePCL,
            _flagPatternFormAsMacroPCL);

        ToolMiscSamplesPersist.SaveDataTypePattern(
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

    private void rbPatternTypeShading_Click(object sender,
                                             RoutedEventArgs e)
    {
        if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            _indxPatternTypePCL = ePatternType.Shading;
        else
            _indxPatternTypePCLXL = ePatternType.Shading;

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

    private void rbPatternTypeXHatch_Click(object sender,
                                            RoutedEventArgs e)
    {
        if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            _indxPatternTypePCL = ePatternType.XHatch;
        else
            _indxPatternTypePCLXL = ePatternType.XHatch;

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
        ToolCommonData.ePrintLang crntPDL)
    {
        if (crntPDL == ToolCommonData.ePrintLang.PCL)
            _flagPatternFormAsMacroPCL = setFlag;
        else if (crntPDL == ToolCommonData.ePrintLang.PCLXL)
            _flagPatternFormAsMacroPCLXL = setFlag;
    }
}
