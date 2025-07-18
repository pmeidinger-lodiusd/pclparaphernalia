﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
// using System.Windows.Media;

namespace PCLParaphernalia
{
    /// <summary>
    /// Interaction logic for ToolMiscSamples.xaml
    /// 
    /// Class handles the MiscSamples tool form.
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

        const ushort _unitsPerInch = 600;
        const ushort _dPtsPerInch = 720;

        const double _unitsToDPts = (1.00 * _dPtsPerInch / _unitsPerInch);
        const double _unitsToInches = (1.00 / _unitsPerInch);
        const double _unitsToMilliMetres = (25.4 / _unitsPerInch);

        const double _dPtsToInches = (1.00 / 720);
        const double _dPtsToMilliMetres = (25.4 / 720);

        private enum eSampleType : byte
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
            (int) ToolCommonData.ePrintLang.PCL,
            (int) ToolCommonData.ePrintLang.PCLXL
        };

        private static readonly int[] _subsetSampleTypes =
        {
            // must be in same order as eSampleTypes enumeration

            (int) ToolCommonData.eToolSubIds.Colour,
            (int) ToolCommonData.eToolSubIds.LogOper,
            (int) ToolCommonData.eToolSubIds.LogPage,
            (int) ToolCommonData.eToolSubIds.Pattern,
            (int) ToolCommonData.eToolSubIds.TxtMod,
            (int) ToolCommonData.eToolSubIds.Unicode
        };

        private static readonly int[] _subsetOrientations =
        {
            (int) PCLOrientations.eIndex.Portrait,
            (int) PCLOrientations.eIndex.Landscape,
            (int) PCLOrientations.eIndex.ReversePortrait,
            (int) PCLOrientations.eIndex.ReverseLandscape
        };

        private static readonly int[] _subsetPaperSizes =
        {
            (int) PCLPaperSizes.eIndex.ISO_A4,
            (int) PCLPaperSizes.eIndex.ANSI_A_Letter,
        };

        private static readonly int[] _subsetPaperTypes =
        {
            (int) PCLPaperTypes.eIndex.NotSet,
            (int) PCLPaperTypes.eIndex.Plain
        };

        private int _indxPDL;
        private int _indxSampleType;

        private ToolCommonData.ePrintLang _crntPDL;

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

        public ToolMiscSamples(ref ToolCommonData.ePrintLang crntPDL,
                                ref ToolCommonData.eToolSubIds crntType)
        {
            InitializeComponent();

            _initialised = false;

            Initialise();

            _initialised = true;

            crntPDL = _crntPDL;
            crntType =
                (ToolCommonData.eToolSubIds)_subsetSampleTypes[_indxSampleType];
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
            _crntPDL = (ToolCommonData.ePrintLang)_subsetPDLs[_indxPDL];

            PdlOptionsStore();

            //----------------------------------------------------------------//
            //                                                                //
            // Generate test print file.                                      //
            //                                                                //
            //----------------------------------------------------------------//

            try
            {
                BinaryWriter binWriter = null;

                ToolCommonData.eToolSubIds sampleType =
                    (ToolCommonData.eToolSubIds)_subsetSampleTypes[_indxSampleType];

                TargetCore.MetricsLoadFileCapt(
                    ToolCommonData.eToolIds.MiscSamples,
                    sampleType,
                    _crntPDL);

                TargetCore.RequestStreamOpen(
                    ref binWriter,
                    ToolCommonData.eToolIds.MiscSamples,
                    sampleType,
                    _crntPDL);

                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                {
                    switch (_indxSampleType)
                    {
                        case (int)eSampleType.Colour:

                            if (_indxColourTypePCL == eColourType.PCL_Simple)
                            {
                                ToolMiscSamplesActColourSimplePCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    _subsetOrientations[_indxOrientationPCL],
                                    _flagColourFormAsMacroPCL);
                            }
                            else if (_indxColourTypePCL == eColourType.PCL_Imaging)
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

                        case (int)eSampleType.LogOper:

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
                                    (_indxLogOperROPFromPCL * _logOperROPInc),
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
                                    (_indxLogOperROPFromPCL * _logOperROPInc),
                                    ((_indxLogOperROPToPCL + 1) * _logOperROPInc) - 1,
                                    _flagLogOperUseMacrosPCL);
                            }

                            break;

                        case (int)eSampleType.LogPage:

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

                        case (int)eSampleType.Pattern:

                            if (_indxPatternTypePCL == ePatternType.Shading)
                            {
                                ToolMiscSamplesActPatternShadePCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    (int)PCLOrientations.eIndex.Portrait,
                                    _flagPatternFormAsMacroPCL);
                            }
                            else if (_indxPatternTypePCL == ePatternType.XHatch)
                            {
                                ToolMiscSamplesActPatternXHatchPCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    (int)PCLOrientations.eIndex.Portrait,
                                    _flagPatternFormAsMacroPCL);
                            }

                            break;

                        case (int)eSampleType.TxtMod:

                            if (_indxTxtModTypePCL == eTxtModType.Chr)
                            {
                                ToolMiscSamplesActTxtModChrPCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    (int)PCLOrientations.eIndex.Portrait,
                                    _flagTxtModFormAsMacroPCL);
                            }
                            else if (_indxTxtModTypePCL == eTxtModType.Pat)
                            {
                                ToolMiscSamplesActTxtModPatPCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    (int)PCLOrientations.eIndex.Portrait,
                                    _flagTxtModFormAsMacroPCL);
                            }
                            else if (_indxTxtModTypePCL == eTxtModType.Rot)
                            {
                                ToolMiscSamplesActTxtModRotPCL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCL],
                                    _subsetPaperTypes[_indxPaperTypePCL],
                                    (int)PCLOrientations.eIndex.Portrait,
                                    _flagTxtModFormAsMacroPCL);
                            }

                            break;

                        case (int)eSampleType.Unicode:

                            ToolMiscSamplesActUnicodePCL.generateJob(
                                binWriter,
                                _subsetPaperSizes[_indxPaperSizePCL],
                                _subsetPaperTypes[_indxPaperTypePCL],
                                (int)PCLOrientations.eIndex.Portrait,
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
                        case (int)eSampleType.Colour:

                            if (_indxColourTypePCLXL == eColourType.PCLXL_Gray)
                            {
                                ToolMiscSamplesActColourGrayPCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    _subsetOrientations[_indxOrientationPCLXL],
                                    _samplesPCLXL_Gray,
                                    _flagColourFormAsMacroPCLXL);
                            }
                            else if (_indxColourTypePCLXL == eColourType.PCLXL_RGB)
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

                        case (int)eSampleType.LogOper:

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
                                    (_indxLogOperROPFromPCLXL * _logOperROPInc),
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
                                    (_indxLogOperROPFromPCLXL * _logOperROPInc),
                                    ((_indxLogOperROPToPCLXL + 1) * _logOperROPInc) - 1,
                                    _flagLogOperUseMacrosPCLXL,
                                    _flagLogOperOptSrcTextPatPCLXL);
                            }

                            break;

                        case (int)eSampleType.LogPage:

                            // can't select this option for PCLXL
                            // so should never reach here
                            break;

                        case (int)eSampleType.Pattern:

                            if (_indxPatternTypePCLXL == ePatternType.Shading)
                            {
                                ToolMiscSamplesActPatternShadePCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    (int)PCLOrientations.eIndex.Portrait,
                                    _flagPatternFormAsMacroPCLXL);
                            }
                            else if (_indxPatternTypePCLXL == ePatternType.XHatch)
                            {
                                ToolMiscSamplesActPatternXHatchPCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    (int)PCLOrientations.eIndex.Portrait,
                                    _flagPatternFormAsMacroPCLXL);
                            }

                            break;

                        case (int)eSampleType.TxtMod:

                            if (_indxTxtModTypePCLXL == eTxtModType.Chr)
                            {
                                ToolMiscSamplesActTxtModChrPCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    (int)PCLOrientations.eIndex.Portrait,
                                    _flagPatternFormAsMacroPCLXL);
                            }
                            else if (_indxTxtModTypePCLXL == eTxtModType.Pat)
                            {
                                ToolMiscSamplesActTxtModPatPCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    (int)PCLOrientations.eIndex.Portrait,
                                    _flagTxtModFormAsMacroPCLXL);
                            }
                            else if (_indxTxtModTypePCLXL == eTxtModType.Rot)
                            {
                                ToolMiscSamplesActTxtModRotPCLXL.GenerateJob(
                                    binWriter,
                                    _subsetPaperSizes[_indxPaperSizePCLXL],
                                    _subsetPaperTypes[_indxPaperTypePCLXL],
                                    (int)PCLOrientations.eIndex.Portrait,
                                    _flagTxtModFormAsMacroPCLXL);
                            }

                            break;

                        case (int)eSampleType.Unicode:

                            ToolMiscSamplesActUnicodePCLXL.generateJob(
                                binWriter,
                                _subsetPaperSizes[_indxPaperSizePCLXL],
                                _subsetPaperTypes[_indxPaperTypePCLXL],
                                (int)PCLOrientations.eIndex.Portrait,
                                _flagUnicodeFormAsMacroPCLXL,
                                _unicodeUCS2PCLXL,
                                _subsetUnicodeFonts[_indxUnicodeFontPCLXL],
                                _unicodeFontVarPCLXL);
                            break;
                    }
                }

                TargetCore.RequestStreamWrite(false);
            }

            catch (SocketException sockExc)
            {
                MessageBox.Show(sockExc.ToString(),
                                "Socket exception",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString(),
                                "Unknown exception",
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

        private void cbOrientation_SelectionChanged(object sender,
                                                    SelectionChangedEventArgs e)
        {
            if (_initialised && cbOrientation.HasItems)
            {
                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
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

        private void cbPaperSize_SelectionChanged(object sender,
                                                  SelectionChangedEventArgs e)
        {
            if (_initialised && cbPaperSize.HasItems)
            {
                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
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

        private void cbPaperType_SelectionChanged(object sender,
                                                  SelectionChangedEventArgs e)
        {
            if (_initialised && cbPaperType.HasItems)
            {
                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
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

        private void cbPDL_SelectionChanged(object sender,
                                            SelectionChangedEventArgs e)
        {
            if (_initialised)
            {
                PdlOptionsStore();

                _indxPDL = cbPDL.SelectedIndex;
                _crntPDL = (ToolCommonData.ePrintLang)_subsetPDLs[_indxPDL];

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

        private void cbSampleType_SelectionChanged(object sender,
                                                    SelectionChangedEventArgs e)
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

        private void chkOptFormAsMacro_Checked(object sender,
                                                RoutedEventArgs e)
        {
            switch (_indxSampleType)
            {
                case (int)eSampleType.Colour:

                    SetFlagColourFormAsMacro(true, _crntPDL);
                    break;

                case (int)eSampleType.LogOper:

                    SetFlagLogOperFormAsMacro(true, _crntPDL);
                    break;

                case (int)eSampleType.LogPage:

                    SetFlagLogPageFormAsMacro(true, _crntPDL);
                    break;

                case (int)eSampleType.Pattern:

                    SetFlagPatternFormAsMacro(true, _crntPDL);
                    break;

                case (int)eSampleType.TxtMod:

                    SetFlagTxtModFormAsMacro(true, _crntPDL);
                    break;

                case (int)eSampleType.Unicode:

                    setFlagUnicodeFormAsMacro(true, _crntPDL);
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

        private void chkOptFormAsMacro_Unchecked(object sender,
                                                  RoutedEventArgs e)
        {
            switch (_indxSampleType)
            {
                case (int)eSampleType.Colour:

                    SetFlagColourFormAsMacro(false, _crntPDL);
                    break;

                case (int)eSampleType.LogOper:

                    SetFlagLogOperFormAsMacro(false, _crntPDL);
                    break;

                case (int)eSampleType.LogPage:

                    SetFlagLogPageFormAsMacro(false, _crntPDL);
                    break;

                case (int)eSampleType.Pattern:

                    SetFlagPatternFormAsMacro(false, _crntPDL);
                    break;

                case (int)eSampleType.TxtMod:

                    SetFlagTxtModFormAsMacro(false, _crntPDL);
                    break;

                case (int)eSampleType.Unicode:

                    setFlagUnicodeFormAsMacro(false, _crntPDL);
                    break;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g i v e C r n t P D L                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GiveCrntPDL(ref ToolCommonData.ePrintLang crntPDL)
        {
            crntPDL = _crntPDL;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g i v e C r n t T y p e                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GiveCrntType(ref ToolCommonData.eToolSubIds crntType)
        {
            crntType =
                (ToolCommonData.eToolSubIds)_subsetSampleTypes[_indxSampleType];
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
                    typeof(ToolCommonData.ePrintLang), i));
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

            if (_indxSampleType == (int)eSampleType.Colour)
            {
                InitialiseDataColour();

                cbSampleType.SelectedIndex = (int)eSampleType.Colour;

                tabSampleType.SelectedItem = tabColour;
            }
            else if (_indxSampleType == (int)eSampleType.LogOper)
            {
                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                    chkOptFormAsMacro.Content = "Use macros";
                else if (_crntPDL == ToolCommonData.ePrintLang.PCLXL)
                    chkOptFormAsMacro.Content = "Use user-defined streams";

                InitialiseDataLogOper(typeChange);

                cbSampleType.SelectedIndex = (int)eSampleType.LogOper;

                tabSampleType.SelectedItem = tabLogOper;
            }
            else if (_indxSampleType == (int)eSampleType.LogPage)
            {
                if (_crntPDL == ToolCommonData.ePrintLang.PCLXL)
                    chkOptFormAsMacro.Visibility = Visibility.Hidden;

                InitialiseDataLogPage();

                cbSampleType.SelectedIndex = (int)eSampleType.LogPage;

                tabSampleType.SelectedItem = tabLogPage;
            }
            else if (_indxSampleType == (int)eSampleType.Pattern)
            {
                InitialiseDataPattern();

                cbSampleType.SelectedIndex = (int)eSampleType.Pattern;

                tabSampleType.SelectedItem = tabPattern;
            }
            else if (_indxSampleType == (int)eSampleType.TxtMod)
            {
                InitialiseDataTxtMod();

                cbSampleType.SelectedIndex = (int)eSampleType.TxtMod;

                tabSampleType.SelectedItem = tabTxtMod;
            }
            else if (_indxSampleType == (int)eSampleType.Unicode)
            {
                initialiseDataUnicode();

                cbSampleType.SelectedIndex = (int)eSampleType.Unicode;

                tabSampleType.SelectedItem = tabUnicode;
            }
            else
            {
                _indxSampleType = (int)eSampleType.TxtMod;

                InitialiseDataTxtMod();

                cbSampleType.SelectedIndex = (int)eSampleType.TxtMod;

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
            ToolMiscSamplesPersist.LoadDataCommon(ref _indxPDL,
                                                  ref _indxSampleType);

            ToolMiscSamplesPersist.LoadDataCommonPDL(
                "PCL",
                ref _indxOrientationPCL,
                ref _indxPaperSizePCL,
                ref _indxPaperTypePCL);

            ToolMiscSamplesPersist.LoadDataCommonPDL(
                "PCLXL",
                ref _indxOrientationPCLXL,
                ref _indxPaperSizePCLXL,
                ref _indxPaperTypePCLXL);

            //----------------------------------------------------------------//

            if ((_indxPDL < 0) || (_indxPDL >= _ctPDLs))
                _indxPDL = 0;

            _crntPDL = (ToolCommonData.ePrintLang)_subsetPDLs[_indxPDL];

            //----------------------------------------------------------------//

            if ((_indxSampleType < 0) || (_indxSampleType >= _ctSampleTypes))
                _indxSampleType = (int)eSampleType.TxtMod;

            //----------------------------------------------------------------//

            MetricsLoadDataColour();
            MetricsLoadDataLogOper();
            MetricsLoadDataLogPage();
            MetricsLoadDataPattern();
            MetricsLoadDataTxtMod();
            metricsLoadDataUnicode();

            //----------------------------------------------------------------//

            if ((_indxOrientationPCL < 0) ||
                (_indxOrientationPCL >= _ctOrientations))
                _indxOrientationPCL = 0;

            if ((_indxPaperSizePCL < 0) ||
                (_indxPaperSizePCL >= _ctPaperSizes))
                _indxPaperSizePCL = 0;

            if ((_indxPaperTypePCL < 0) ||
                (_indxPaperTypePCL >= _ctPaperTypes))
                _indxPaperTypePCL = 0;

            //----------------------------------------------------------------//

            if ((_indxOrientationPCLXL < 0) ||
                (_indxOrientationPCLXL >= _ctOrientations))
                _indxOrientationPCLXL = 0;

            if ((_indxPaperSizePCLXL < 0) ||
                (_indxPaperSizePCLXL >= _ctPaperSizes))
                _indxPaperSizePCLXL = 0;

            if ((_indxPaperTypePCLXL < 0) ||
                (_indxPaperTypePCLXL >= _ctPaperTypes))
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

            ToolMiscSamplesPersist.SaveDataCommon(_indxPDL,
                                                  _indxSampleType);

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
            metricsSaveDataUnicode();
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
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
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
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
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
            TargetCore.eTarget targetType = TargetCore.getType();

            if (targetType == TargetCore.eTarget.File)
            {
                btnGenerate.Content = "Generate & send test data to file";
            }
            else if (targetType == TargetCore.eTarget.NetPrinter)
            {
                string netPrnAddress = string.Empty;
                int netPrnPort = 0;

                int netTimeoutSend = 0;
                int netTimeoutReceive = 0;

                TargetCore.MetricsLoadNetPrinter(ref netPrnAddress,
                                                  ref netPrnPort,
                                                  ref netTimeoutSend,
                                                  ref netTimeoutReceive);

                btnGenerate.Content = "Generate & send test data to " +
                                      "\r\n" +
                                      netPrnAddress + " : " +
                                      netPrnPort.ToString();
            }
            else if (targetType == TargetCore.eTarget.WinPrinter)
            {
                string winPrintername = string.Empty;

                TargetCore.MetricsLoadWinPrinter(ref winPrintername);

                btnGenerate.Content = "Generate & send test data to printer " +
                                      "\r\n" +
                                      winPrintername;
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
            PCLOrientations.eAspect aspect;

            int indxOrientation,
                  indxPaperSize,
                  indxPaperType;

            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                indxOrientation = _subsetOrientations[_indxOrientationPCL];
                indxPaperSize = _subsetPaperSizes[_indxPaperSizePCL];
                indxPaperType = _indxPaperTypePCL;
            }
            else
            {
                indxOrientation = _subsetOrientations[_indxOrientationPCLXL];
                indxPaperSize = _subsetPaperSizes[_indxPaperSizePCLXL];
                indxPaperType = _indxPaperTypePCLXL;
            }

            aspect = PCLOrientations.GetAspect(indxOrientation);

            _paperSizeLongEdge =
                PCLPaperSizes.GetSizeLongEdge(indxPaperSize,
                                              _unitsPerInch);

            _paperSizeShortEdge =
                PCLPaperSizes.GetSizeShortEdge(indxPaperSize,
                                               _unitsPerInch);

            _paperMarginsUnprintable =
                PCLPaperSizes.GetMarginsUnprintable(indxPaperSize,
                                                    _unitsPerInch);

            _paperMarginsLogicalLand =
                PCLPaperSizes.GetMarginsLogicalLand(indxPaperSize,
                                                    _unitsPerInch);

            _paperMarginsLogicalPort =
                PCLPaperSizes.GetMarginsLogicalPort(indxPaperSize,
                                                    _unitsPerInch);

            if (aspect == PCLOrientations.eAspect.Portrait)
            {
                _paperMarginsLogicalLeft = _paperMarginsLogicalPort;
                _paperMarginsLogicalTop = 0;

                _paperWidthPhysical =
                    _paperSizeShortEdge;

                _paperLengthPhysical =
                    _paperSizeLongEdge;

                _paperWidthPrintable =
                    (ushort)(_paperSizeShortEdge -
                             (_paperMarginsUnprintable * 2));

                _paperLengthPrintable =
                    (ushort)(_paperSizeLongEdge -
                             (_paperMarginsUnprintable * 2));

                _paperWidthLogical =
                    (ushort)(_paperSizeShortEdge -
                             (_paperMarginsLogicalPort * 2));

                _paperLengthLogical =
                    _paperSizeLongEdge;
            }
            else
            {
                _paperMarginsLogicalLeft = _paperMarginsLogicalLand;
                _paperMarginsLogicalTop = 0;

                _paperWidthPhysical =
                    _paperSizeLongEdge;

                _paperLengthPhysical =
                    _paperSizeShortEdge;

                _paperWidthPrintable =
                    (ushort)(_paperSizeLongEdge -
                             (_paperMarginsUnprintable * 2));

                _paperLengthPrintable =
                    (ushort)(_paperSizeShortEdge -
                             (_paperMarginsUnprintable * 2));

                _paperWidthLogical =
                    (ushort)(_paperSizeLongEdge -
                             (_paperMarginsLogicalLand * 2));

                _paperLengthLogical =
                    _paperSizeShortEdge;
            }
        }
    }
}
