using System;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

// using System.Windows.Media;

namespace PCLParaphernalia
{
    /// <summary>
    /// <para>Interaction logic for ToolMiscSamples.xaml</para>
    /// <para>Class handles the MiscSamples tool form.</para>
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

        private const ushort _unitsPerInch = 600;
        private const ushort _dPtsPerInch = 720;
        private const double _unitsToDPts = (1.00 * _dPtsPerInch / _unitsPerInch);
        private const double _unitsToInches = (1.00 / _unitsPerInch);
        private const double _unitsToMilliMetres = (25.4 / _unitsPerInch);
        private const double _dPtsToInches = (1.00 / 720);
        private const double _dPtsToMilliMetres = (25.4 / 720);

        private enum SampleType : byte
        {
            // must be in same order as _subsetTypes array

            Colour,
            LogOper,
            LogPage,     // PCL only
            Pattern,
            TxtMod,
            Unicode
        };

        private readonly string[] sSampleNames =
        {
            // must be in same order as _subsetTypes array

            "Colour",
            "Logical operations",
            "Logical page definition",     // PCL only
            "Patterns",
            "Text modification",
            "Unicode characters"
        };

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly int[] _subsetPDLs =
        {
            (int) ToolCommonData.PrintLang.PCL,
            (int) ToolCommonData.PrintLang.PCLXL
        };

        private static readonly int[] _subsetSampleTypes =
        {
            // must be in same order as eSampleTypes enumeration

            (int) ToolCommonData.ToolSubIds.Colour,
            (int) ToolCommonData.ToolSubIds.LogOper,
            (int) ToolCommonData.ToolSubIds.LogPage,
            (int) ToolCommonData.ToolSubIds.Pattern,
            (int) ToolCommonData.ToolSubIds.TxtMod,
            (int) ToolCommonData.ToolSubIds.Unicode
        };

        private static readonly int[] _subsetOrientations =
        {
            (int) PCLOrientations.Index.Portrait,
            (int) PCLOrientations.Index.Landscape,
            (int) PCLOrientations.Index.ReversePortrait,
            (int) PCLOrientations.Index.ReverseLandscape
        };

        private static readonly int[] _subsetPaperSizes =
        {
            (int) PCLPaperSizes.Index.ISO_A4,
            (int) PCLPaperSizes.Index.ANSI_A_Letter,
        };

        private static readonly int[] _subsetPaperTypes =
        {
            (int) PCLPaperTypes.Index.NotSet,
            (int) PCLPaperTypes.Index.Plain
        };

        private int _indxPDL;
        private int _indxSampleType;

        private ToolCommonData.PrintLang _crntPDL;

        private int _ctPDLs;
        private int _ctSampleTypes;
        private int _ctOrientations;
        private int _ctPaperSizes;
        private int _ctPaperTypes;

        private int _indxOrientationPCL;
        private int _indxOrientationPCLXL;
        private int _indxPaperSizePCL;
        private int _indxPaperSizePCLXL;
        private int _indxPaperTypePCL;
        private int _indxPaperTypePCLXL;

        private ushort _paperSizeShortEdge;
        private ushort _paperSizeLongEdge;
        private ushort _paperMarginsLogicalLand;
        private ushort _paperMarginsLogicalPort;
        private ushort _paperMarginsLogicalLeft;
        private ushort _paperMarginsLogicalTop;
        private ushort _paperMarginsUnprintable;

        private ushort _paperWidthPrintable;
        private ushort _paperWidthLogical;
        private ushort _paperWidthPhysical;
        private ushort _paperLengthPrintable;
        private ushort _paperLengthLogical;
        private ushort _paperLengthPhysical;

        private bool _initialised;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T o o l M i s c S a m p l e s                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolMiscSamples(ref ToolCommonData.PrintLang crntPDL, ref ToolCommonData.ToolSubIds crntType)
        {
            InitializeComponent();

            _initialised = false;

            Initialise();

            _initialised = true;

            crntPDL = _crntPDL;
            crntType = (ToolCommonData.ToolSubIds)_subsetSampleTypes[_indxSampleType];
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n G e n e r a t e _ C l i c k                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Generate Test Data' button is clicked.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            //----------------------------------------------------------------//
            //                                                                //
            // Get current test metrics.                                      //
            // Note that the relevant (PDL-specific) stored option values     //
            // SHOULD already be up-to-date, since the fields all have        //
            // associated 'OnChange' actions. ***** Not with WPF ????? *****  //
            // But we'll save them all anyway, to make sure.                  //
            //                                                                //
            //----------------------------------------------------------------//

            //     btnGenerate.BorderBrush = Brushes.Red;

            _indxPDL = cbPDL.SelectedIndex;
            _crntPDL = (ToolCommonData.PrintLang)_subsetPDLs[_indxPDL];

            PdlOptionsStore();

            //----------------------------------------------------------------//
            //                                                                //
            // Generate test print file.                                      //
            //                                                                //
            //----------------------------------------------------------------//

            try
            {
                BinaryWriter binWriter = null;

                ToolCommonData.ToolSubIds sampleType = (ToolCommonData.ToolSubIds)_subsetSampleTypes[_indxSampleType];

                TargetCore.MetricsLoadFileCapt(
                    ToolCommonData.ToolIds.MiscSamples,
                    sampleType,
                    _crntPDL);

                TargetCore.RequestStreamOpen(
                    ref binWriter,
                    ToolCommonData.ToolIds.MiscSamples,
                    sampleType,
                    _crntPDL);

                if (_crntPDL == ToolCommonData.PrintLang.PCL)
                {
                    switch (_indxSampleType)
                    {
                        case (int)SampleType.Colour:

                            if (_indxColourTypePCL == ColourType.PCL_Simple)
                            {
                                ToolMiscSamplesActColourSimplePCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    _subsetOrientations[_indxOrientationPCL],
                                    _flagColourFormAsMacroPCL);
                            }
                            else if (_indxColourTypePCL == ColourType.PCL_Imaging)
                            {
                                ToolMiscSamplesActColourImagingPCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    _subsetOrientations[_indxOrientationPCL],
                                    _samplesPCL_CID,
                                    _flagColourFormAsMacroPCL);
                            }
                            break;

                        case (int)SampleType.LogOper:

                            int indxMode =
                                _subsetLogOperModesPCL[_indxLogOperModePCL];

                            if (PCLPalettes.IsMonochrome(indxMode))
                            {
                                ToolMiscSamplesActLogOperPCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    _subsetOrientations[_indxOrientationPCL],
                                    indxMode,
                                    _indxLogOperMonoD1PCL,
                                    _indxLogOperMonoD2PCL,
                                    _indxLogOperMonoS1PCL,
                                    _indxLogOperMonoS2PCL,
                                    _indxLogOperMonoT1PCL,
                                    _indxLogOperMonoT2PCL,
                                    _indxLogOperROPFromPCL * _logOperROPInc,
                                    ((_indxLogOperROPToPCL + 1) * _logOperROPInc) - 1,
                                    _flagLogOperUseMacrosPCL);
                            }
                            else
                            {
                                ToolMiscSamplesActLogOperPCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    _subsetOrientations[_indxOrientationPCL],
                                    indxMode,
                                    _indxLogOperClrD1PCL,
                                    _indxLogOperClrD2PCL,
                                    _indxLogOperClrS1PCL,
                                    _indxLogOperClrS2PCL,
                                    _indxLogOperClrT1PCL,
                                    _indxLogOperClrT2PCL,
                                    _indxLogOperROPFromPCL * _logOperROPInc,
                                    ((_indxLogOperROPToPCL + 1) * _logOperROPInc) - 1,
                                    _flagLogOperUseMacrosPCL);
                            }

                            break;

                        case (int)SampleType.LogPage:

                            ToolMiscSamplesActLogPagePCL.GenerateJob(
                                binWriter,
                                _subsetPaperSizes[_indxPaperSizePCL],
                                _subsetPaperTypes[_indxPaperTypePCL],
                                _subsetOrientations[_indxOrientationPCL],
                                (short)_logPageOffLeftDPt,
                                (short)_logPageOffTopDPt,
                                (ushort)_logPageWidthDPt,
                                (ushort)_logPageHeightDPt,
                                _flagLogPageFormAsMacroPCL,
                                _flagLogPageOptStdPagePCL);
                            break;

                        case (int)SampleType.Pattern:

                            if (_indxPatternTypePCL == PatternType.Shading)
                            {
                                ToolMiscSamplesActPatternShadePCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    (int)PCLOrientations.Index.Portrait,
                                    _flagPatternFormAsMacroPCL);
                            }
                            else if (_indxPatternTypePCL == PatternType.XHatch)
                            {
                                ToolMiscSamplesActPatternXHatchPCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    (int)PCLOrientations.Index.Portrait,
                                    _flagPatternFormAsMacroPCL);
                            }
                            break;

                        case (int)SampleType.TxtMod:

                            if (_indxTxtModTypePCL == TxtModType.Chr)
                            {
                                ToolMiscSamplesActTxtModChrPCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    (int)PCLOrientations.Index.Portrait,
                                    _flagTxtModFormAsMacroPCL);
                            }
                            else if (_indxTxtModTypePCL == TxtModType.Pat)
                            {
                                ToolMiscSamplesActTxtModPatPCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    (int)PCLOrientations.Index.Portrait,
                                    _flagTxtModFormAsMacroPCL);
                            }
                            else if (_indxTxtModTypePCL == TxtModType.Rot)
                            {
                                ToolMiscSamplesActTxtModRotPCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    (int)PCLOrientations.Index.Portrait,
                                    _flagTxtModFormAsMacroPCL);
                            }

                            break;

                        case (int)SampleType.Unicode:

                            ToolMiscSamplesActUnicodePCL.GenerateJob(
                                binWriter,
                                _subsetPaperSizes[_indxPaperSizePCL],
                                _subsetPaperTypes[_indxPaperTypePCL],
                                (int)PCLOrientations.Index.Portrait,
                                _flagUnicodeFormAsMacroPCL,
                                _unicodeUCS2PCL,
                                _subsetUnicodeFonts[_indxUnicodeFontPCL],
                                _unicodeFontVarPCL);
                            break;
                    }
                }
                else   // if (_crntPDL == ToolCommonData.ePrintLang.PCLXL)
                {
                    switch (_indxSampleType)
                    {
                        case (int)SampleType.Colour:

                            if (_indxColourTypePCLXL == ColourType.PCLXL_Gray)
                            {
                                ToolMiscSamplesActColourGrayPCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    _subsetOrientations[_indxOrientationPCLXL],
                                    _samplesPCLXL_Gray,
                                    _flagColourFormAsMacroPCLXL);
                            }
                            else if (_indxColourTypePCLXL == ColourType.PCLXL_RGB)
                            {
                                ToolMiscSamplesActColourRGBPCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    _subsetOrientations[_indxOrientationPCLXL],
                                    _samplesPCLXL_RGB,
                                    _flagColourFormAsMacroPCLXL);
                            }
                            break;

                        case (int)SampleType.LogOper:

                            int indxMode =
                                _subsetLogOperModesPCLXL[_indxLogOperModePCLXL];

                            if (PCLXLPalettes.IsMonochrome(indxMode))
                            {
                                ToolMiscSamplesActLogOperPCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    _subsetOrientations[_indxOrientationPCLXL],
                                    indxMode,
                                    _indxLogOperGrayD1PCLXL,
                                    _indxLogOperGrayD2PCLXL,
                                    _indxLogOperGrayS1PCLXL,
                                    _indxLogOperGrayS2PCLXL,
                                    _indxLogOperGrayT1PCLXL,
                                    _indxLogOperGrayT2PCLXL,
                                    _indxLogOperROPFromPCLXL * _logOperROPInc,
                                    ((_indxLogOperROPToPCLXL + 1) * _logOperROPInc) - 1,
                                    _flagLogOperUseMacrosPCLXL,
                                    _flagLogOperOptSrcTextPatPCLXL);
                            }
                            else
                            {
                                ToolMiscSamplesActLogOperPCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    _subsetOrientations[_indxOrientationPCLXL],
                                    indxMode,
                                    _indxLogOperClrD1PCLXL,
                                    _indxLogOperClrD2PCLXL,
                                    _indxLogOperClrS1PCLXL,
                                    _indxLogOperClrS2PCLXL,
                                    _indxLogOperClrT1PCLXL,
                                    _indxLogOperClrT2PCLXL,
                                    _indxLogOperROPFromPCLXL * _logOperROPInc,
                                    ((_indxLogOperROPToPCLXL + 1) * _logOperROPInc) - 1,
                                    _flagLogOperUseMacrosPCLXL,
                                    _flagLogOperOptSrcTextPatPCLXL);
                            }

                            break;

                        case (int)SampleType.LogPage:

                            // can't select this option for PCLXL
                            // so should never reach here
                            break;

                        case (int)SampleType.Pattern:

                            if (_indxPatternTypePCLXL == PatternType.Shading)
                            {
                                ToolMiscSamplesActPatternShadePCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    (int)PCLOrientations.Index.Portrait,
                                    _flagPatternFormAsMacroPCLXL);
                            }
                            else if (_indxPatternTypePCLXL == PatternType.XHatch)
                            {
                                ToolMiscSamplesActPatternXHatchPCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    (int)PCLOrientations.Index.Portrait,
                                    _flagPatternFormAsMacroPCLXL);
                            }
                            break;

                        case (int)SampleType.TxtMod:

                            if (_indxTxtModTypePCLXL == TxtModType.Chr)
                            {
                                ToolMiscSamplesActTxtModChrPCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    (int)PCLOrientations.Index.Portrait,
                                    _flagPatternFormAsMacroPCLXL);
                            }
                            else if (_indxTxtModTypePCLXL == TxtModType.Pat)
                            {
                                ToolMiscSamplesActTxtModPatPCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    (int)PCLOrientations.Index.Portrait,
                                    _flagTxtModFormAsMacroPCLXL);
                            }
                            else if (_indxTxtModTypePCLXL == TxtModType.Rot)
                            {
                                ToolMiscSamplesActTxtModRotPCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    (int)PCLOrientations.Index.Portrait,
                                    _flagTxtModFormAsMacroPCLXL);
                            }

                            break;

                        case (int)SampleType.Unicode:

                            ToolMiscSamplesActUnicodePCLXL.GenerateJob(
                                binWriter,
                                _subsetPaperSizes[_indxPaperSizePCLXL],
                                _subsetPaperTypes[_indxPaperTypePCLXL],
                                (int)PCLOrientations.Index.Portrait,
                                _flagUnicodeFormAsMacroPCLXL,
                                _unicodeUCS2PCLXL,
                                _subsetUnicodeFonts[_indxUnicodeFontPCLXL],
                                _unicodeFontVarPCLXL);
                            break;
                    }
                }

                TargetCore.RequestStreamWrite(false);
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"SocketException:\r\n\r\nMessage: {ex.Message}\r\n\r\nErrorCode: {ex.ErrorCode}\r\n\r\nSocketErrorCode: {ex.SocketErrorCode}",
                                "Generate Test Data",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception:\r\n" + ex.Message,
                                "Generate Test Data",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            //      btnGenerate.BorderBrush = Brushes.Transparent;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b O r i e n t a t i o n _ S e l e c t i o n C h a n g e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Orientation item has changed.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbOrientation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initialised && cbOrientation.HasItems)
            {
                if (_crntPDL == ToolCommonData.PrintLang.PCL)
                    _indxOrientationPCL = cbOrientation.SelectedIndex;
                else
                    _indxOrientationPCLXL = cbOrientation.SelectedIndex;

                SetPaperMetrics();
                SetPaperMetricsLogPage();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P a p e r S i z e _ S e l e c t i o n C h a n g e d            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Paper Size item has changed.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPaperSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initialised && cbPaperSize.HasItems)
            {
                if (_crntPDL == ToolCommonData.PrintLang.PCL)
                    _indxPaperSizePCL = cbPaperSize.SelectedIndex;
                else
                    _indxPaperSizePCLXL = cbPaperSize.SelectedIndex;

                SetPaperMetrics();
                SetPaperMetricsLogPage();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P a p e r T y p e _ S e l e c t i o n C h a n g e d            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Paper Type item has changed.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPaperType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initialised && cbPaperType.HasItems)
            {
                if (_crntPDL == ToolCommonData.PrintLang.PCL)
                    _indxPaperTypePCL = cbPaperType.SelectedIndex;
                else
                    _indxPaperTypePCLXL = cbPaperType.SelectedIndex;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P D L _ S e l e c t i o n C h a n g e d                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Print Language item has changed.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPDL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initialised)
            {
                PdlOptionsStore();

                _indxPDL = cbPDL.SelectedIndex;
                _crntPDL = (ToolCommonData.PrintLang)_subsetPDLs[_indxPDL];

                PdlOptionsRestore();

                InitialiseData(false);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b S a m p l e T y p e _ S e l e c t i o n C h a n g e d          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Sample type item has changed.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbSampleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initialised && cbSampleType.HasItems)
            {
                _indxSampleType = cbSampleType.SelectedIndex;

                InitialiseData(true);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k O p t F o r m A s M a c r o _ C h e c k e d                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Option 'Render fixed text as overlay' checked.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkOptFormAsMacro_Checked(object sender, RoutedEventArgs e)
        {
            switch (_indxSampleType)
            {
                case (int)SampleType.Colour:

                    SetFlagColourFormAsMacro(true, _crntPDL);
                    break;

                case (int)SampleType.LogOper:

                    SetFlagLogOperFormAsMacro(true, _crntPDL);
                    break;

                case (int)SampleType.LogPage:

                    SetFlagLogPageFormAsMacro(true, _crntPDL);
                    break;

                case (int)SampleType.Pattern:

                    SetFlagPatternFormAsMacro(true, _crntPDL);
                    break;

                case (int)SampleType.TxtMod:

                    SetFlagTxtModFormAsMacro(true, _crntPDL);
                    break;

                case (int)SampleType.Unicode:

                    SetFlagUnicodeFormAsMacro(true, _crntPDL);
                    break;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k O p t F o r m A s M a c r o _ U n c h e c k e d              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Option 'Render fixed text as overlay' unchecked.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ChkOptFormAsMacro_Unchecked(object sender, RoutedEventArgs e)
        {
            switch (_indxSampleType)
            {
                case (int)SampleType.Colour:

                    SetFlagColourFormAsMacro(false, _crntPDL);
                    break;

                case (int)SampleType.LogOper:

                    SetFlagLogOperFormAsMacro(false, _crntPDL);
                    break;

                case (int)SampleType.LogPage:

                    SetFlagLogPageFormAsMacro(false, _crntPDL);
                    break;

                case (int)SampleType.Pattern:

                    SetFlagPatternFormAsMacro(false, _crntPDL);
                    break;

                case (int)SampleType.TxtMod:

                    SetFlagTxtModFormAsMacro(false, _crntPDL);
                    break;

                case (int)SampleType.Unicode:

                    SetFlagUnicodeFormAsMacro(false, _crntPDL);
                    break;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g i v e C r n t P D L                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GiveCrntPDL(ref ToolCommonData.PrintLang crntPDL)
        {
            crntPDL = _crntPDL;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g i v e C r n t T y p e                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GiveCrntType(ref ToolCommonData.ToolSubIds crntType)
        {
            crntType = (ToolCommonData.ToolSubIds)_subsetSampleTypes[_indxSampleType];
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialisation.                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void Initialise()
        {
            int index;

            //----------------------------------------------------------------//
            //                                                                //
            // Populate form.                                                 //
            //                                                                //
            //----------------------------------------------------------------//

            cbPDL.Items.Clear();

            _ctPDLs = _subsetPDLs.Length;

            for (int i = 0; i < _ctPDLs; i++)
            {
                //  index = _subsetPDLs[i];

                cbPDL.Items.Add(Enum.GetName(
                    typeof(ToolCommonData.PrintLang), i));
            }

            //----------------------------------------------------------------//

            cbOrientation.Items.Clear();

            _ctOrientations = _subsetOrientations.Length;

            for (int i = 0; i < _ctOrientations; i++)
            {
                index = _subsetOrientations[i];

                cbOrientation.Items.Add(PCLOrientations.GetName(index));
            }

            //----------------------------------------------------------------//

            cbPaperSize.Items.Clear();

            _ctPaperSizes = _subsetPaperSizes.Length;

            for (int i = 0; i < _ctPaperSizes; i++)
            {
                index = _subsetPaperSizes[i];

                cbPaperSize.Items.Add(PCLPaperSizes.GetName(index));
            }

            //----------------------------------------------------------------//

            cbPaperType.Items.Clear();

            _ctPaperTypes = _subsetPaperTypes.Length;

            for (int i = 0; i < _ctPaperTypes; i++)
            {
                index = _subsetPaperTypes[i];

                cbPaperType.Items.Add(PCLPaperTypes.GetName(index));
            }

            //----------------------------------------------------------------//

            _ctSampleTypes = _subsetSampleTypes.Length;

            cbSampleType.Items.Clear();

            _ctSampleTypes = _subsetSampleTypes.Length;

            for (int i = 0; i < _ctSampleTypes; i++)
            {
                cbSampleType.Items.Add(sSampleNames[i]);
            }

            //----------------------------------------------------------------//

            ResetTarget();

            //----------------------------------------------------------------//
            //                                                                //
            // Reinstate settings from persistent storage.                    //
            //                                                                //
            //----------------------------------------------------------------//

            MetricsLoad();

            PdlOptionsRestore();

            cbPDL.SelectedIndex = (byte)_indxPDL;

            InitialiseData(true);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e D a t a                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialise data for selected sample type.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void InitialiseData(bool typeChange)
        {
            SetPaperMetrics();

            //----------------------------------------------------------------//

            btnGenerate.IsEnabled = true;

            chkOptFormAsMacro.Visibility = Visibility.Visible;
            chkOptFormAsMacro.Content = "Render fixed text as overlay";

            if (_indxSampleType == (int)SampleType.Colour)
            {
                InitialiseDataColour();

                cbSampleType.SelectedIndex = (int)SampleType.Colour;

                tabSampleType.SelectedItem = tabColour;
            }
            else if (_indxSampleType == (int)SampleType.LogOper)
            {
                if (_crntPDL == ToolCommonData.PrintLang.PCL)
                    chkOptFormAsMacro.Content = "Use macros";
                else if (_crntPDL == ToolCommonData.PrintLang.PCLXL)
                    chkOptFormAsMacro.Content = "Use user-defined streams";

                InitialiseDataLogOper(typeChange);

                cbSampleType.SelectedIndex = (int)SampleType.LogOper;

                tabSampleType.SelectedItem = tabLogOper;
            }
            else if (_indxSampleType == (int)SampleType.LogPage)
            {
                if (_crntPDL == ToolCommonData.PrintLang.PCLXL)
                    chkOptFormAsMacro.Visibility = Visibility.Hidden;

                InitialiseDataLogPage();

                cbSampleType.SelectedIndex = (int)SampleType.LogPage;

                tabSampleType.SelectedItem = tabLogPage;
            }
            else if (_indxSampleType == (int)SampleType.Pattern)
            {
                InitialiseDataPattern();

                cbSampleType.SelectedIndex = (int)SampleType.Pattern;

                tabSampleType.SelectedItem = tabPattern;
            }
            else if (_indxSampleType == (int)SampleType.TxtMod)
            {
                InitialiseDataTxtMod();

                cbSampleType.SelectedIndex = (int)SampleType.TxtMod;

                tabSampleType.SelectedItem = tabTxtMod;
            }
            else if (_indxSampleType == (int)SampleType.Unicode)
            {
                InitialiseDataUnicode();

                cbSampleType.SelectedIndex = (int)SampleType.Unicode;

                tabSampleType.SelectedItem = tabUnicode;
            }
            else
            {
                _indxSampleType = (int)SampleType.TxtMod;

                InitialiseDataTxtMod();

                cbSampleType.SelectedIndex = (int)SampleType.TxtMod;

                tabSampleType.SelectedItem = tabTxtMod;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load metrics from persistent storage.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void MetricsLoad()
        {
            ToolMiscSamplesPersist.LoadDataCommon(out _indxPDL, out _indxSampleType);

            ToolMiscSamplesPersist.LoadDataCommonPDL(
                "PCL",
                out _indxOrientationPCL,
                out _indxPaperSizePCL,
                out _indxPaperTypePCL);

            ToolMiscSamplesPersist.LoadDataCommonPDL(
                "PCLXL",
                out _indxOrientationPCLXL,
                out _indxPaperSizePCLXL,
                out _indxPaperTypePCLXL);

            //----------------------------------------------------------------//

            if ((_indxPDL < 0) || (_indxPDL >= _ctPDLs))
                _indxPDL = 0;

            _crntPDL = (ToolCommonData.PrintLang)_subsetPDLs[_indxPDL];

            //----------------------------------------------------------------//

            if ((_indxSampleType < 0) || (_indxSampleType >= _ctSampleTypes))
                _indxSampleType = (int)SampleType.TxtMod;

            //----------------------------------------------------------------//

            MetricsLoadDataColour();
            MetricsLoadDataLogOper();
            MetricsLoadDataLogPage();
            MetricsLoadDataPattern();
            MetricsLoadDataTxtMod();
            MetricsLoadDataUnicode();

            //----------------------------------------------------------------//

            if ((_indxOrientationPCL < 0) || (_indxOrientationPCL >= _ctOrientations))
                _indxOrientationPCL = 0;

            if ((_indxPaperSizePCL < 0) || (_indxPaperSizePCL >= _ctPaperSizes))
                _indxPaperSizePCL = 0;

            if ((_indxPaperTypePCL < 0) || (_indxPaperTypePCL >= _ctPaperTypes))
                _indxPaperTypePCL = 0;

            //----------------------------------------------------------------//

            if ((_indxOrientationPCLXL < 0) || (_indxOrientationPCLXL >= _ctOrientations))
                _indxOrientationPCLXL = 0;

            if ((_indxPaperSizePCLXL < 0) || (_indxPaperSizePCLXL >= _ctPaperSizes))
                _indxPaperSizePCLXL = 0;

            if ((_indxPaperTypePCLXL < 0) || (_indxPaperTypePCLXL >= _ctPaperTypes))
                _indxPaperTypePCLXL = 0;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s S a v e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Save current metrics to persistent storage.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void MetricsSave()
        {
            PdlOptionsStore();

            ToolMiscSamplesPersist.SaveDataCommon(_indxPDL, _indxSampleType);

            ToolMiscSamplesPersist.SaveDataCommonPDL(
                "PCL",
                _indxOrientationPCL,
                _indxPaperSizePCL,
                _indxPaperTypePCL);

            ToolMiscSamplesPersist.SaveDataCommonPDL(
                "PCLXL",
                _indxOrientationPCLXL,
                _indxPaperSizePCLXL,
                _indxPaperTypePCLXL);

            MetricsSaveDataColour();
            MetricsSaveDataLogOper();
            MetricsSaveDataLogPage();
            MetricsSaveDataPattern();
            MetricsSaveDataTxtMod();
            MetricsSaveDataUnicode();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p d l O p t i o n s R e s t o r e                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Restore the test metrics options for the current PDL.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void PdlOptionsRestore()
        {
            if (_crntPDL == ToolCommonData.PrintLang.PCL)
            {
                cbOrientation.SelectedIndex = _indxOrientationPCL;
                cbPaperSize.SelectedIndex = _indxPaperSizePCL;
                cbPaperType.SelectedIndex = _indxPaperTypePCL;
            }
            else
            {
                cbOrientation.SelectedIndex = _indxOrientationPCLXL;
                cbPaperSize.SelectedIndex = _indxPaperSizePCLXL;
                cbPaperType.SelectedIndex = _indxPaperTypePCLXL;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p d l O p t i o n s S t o r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store the test metrics options for the current PDL.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void PdlOptionsStore()
        {
            if (_crntPDL == ToolCommonData.PrintLang.PCL)
            {
                _indxOrientationPCL = cbOrientation.SelectedIndex;
                _indxPaperSizePCL = cbPaperSize.SelectedIndex;
                _indxPaperTypePCL = cbPaperType.SelectedIndex;
            }
            else
            {
                _indxOrientationPCLXL = cbOrientation.SelectedIndex;
                _indxPaperSizePCLXL = cbPaperSize.SelectedIndex;
                _indxPaperTypePCLXL = cbPaperType.SelectedIndex;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s e t T a r g e t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset the text on the 'Generate' button.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void ResetTarget()
        {
            TargetCore.Target targetType = TargetCore.GetTargetType();

            if (targetType == TargetCore.Target.File)
            {
                btnGenerate.Content = "Generate & send test data to file";
            }
            else if (targetType == TargetCore.Target.NetPrinter)
            {
                string netPrnAddress = string.Empty;
                int netPrnPort = 0;

                int netTimeoutSend = 0;
                int netTimeoutReceive = 0;

                TargetCore.MetricsLoadNetPrinter(out netPrnAddress,
                                                  out netPrnPort,
                                                  out netTimeoutSend,
                                                  out netTimeoutReceive);

                btnGenerate.Content = "Generate & send test data to\r\n" + netPrnAddress + " : " + netPrnPort.ToString();
            }
            else if (targetType == TargetCore.Target.WinPrinter)
            {
                string winPrintername = string.Empty;

                TargetCore.MetricsLoadWinPrinter(out winPrintername);

                btnGenerate.Content = "Generate & send test data to printer\r\n" + winPrintername;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t P a p e r M e t r i c s                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set the contents of the Paper metrics fields.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetPaperMetrics()
        {
            PCLOrientations.Aspect aspect;

            int indxOrientation,
                  indxPaperSize,
                  indxPaperType;

            if (_crntPDL == ToolCommonData.PrintLang.PCL)
            {
                indxOrientation = _subsetOrientations[_indxOrientationPCL];
                indxPaperSize = _subsetPaperSizes[_indxPaperSizePCL];
            }
            else
            {
                indxOrientation = _subsetOrientations[_indxOrientationPCLXL];
                indxPaperSize = _subsetPaperSizes[_indxPaperSizePCLXL];
            }

            aspect = PCLOrientations.GetAspect(indxOrientation);

            _paperSizeLongEdge = PCLPaperSizes.GetSizeLongEdge(indxPaperSize, _unitsPerInch);

            _paperSizeShortEdge = PCLPaperSizes.GetSizeShortEdge(indxPaperSize, _unitsPerInch);

            _paperMarginsUnprintable = PCLPaperSizes.GetMarginsUnprintable(indxPaperSize, _unitsPerInch);

            _paperMarginsLogicalLand = PCLPaperSizes.GetMarginsLogicalLand(indxPaperSize, _unitsPerInch);

            _paperMarginsLogicalPort = PCLPaperSizes.GetMarginsLogicalPort(indxPaperSize, _unitsPerInch);

            if (aspect == PCLOrientations.Aspect.Portrait)
            {
                _paperMarginsLogicalLeft = _paperMarginsLogicalPort;
                _paperMarginsLogicalTop = 0;

                _paperWidthPhysical = _paperSizeShortEdge;

                _paperLengthPhysical = _paperSizeLongEdge;

                _paperWidthPrintable = (ushort)(_paperSizeShortEdge - (_paperMarginsUnprintable * 2));

                _paperLengthPrintable = (ushort)(_paperSizeLongEdge - (_paperMarginsUnprintable * 2));

                _paperWidthLogical = (ushort)(_paperSizeShortEdge - (_paperMarginsLogicalPort * 2));

                _paperLengthLogical = _paperSizeLongEdge;
            }
            else
            {
                _paperMarginsLogicalLeft = _paperMarginsLogicalLand;
                _paperMarginsLogicalTop = 0;

                _paperWidthPhysical = _paperSizeLongEdge;

                _paperLengthPhysical = _paperSizeShortEdge;

                _paperWidthPrintable = (ushort)(_paperSizeLongEdge - (_paperMarginsUnprintable * 2));

                _paperLengthPrintable = (ushort)(_paperSizeShortEdge - (_paperMarginsUnprintable * 2));

                _paperWidthLogical = (ushort)(_paperSizeLongEdge - (_paperMarginsLogicalLand * 2));

                _paperLengthLogical = _paperSizeShortEdge;
            }
        }
    }
}