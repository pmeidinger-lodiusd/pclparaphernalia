﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// Interaction logic for ToolPrintArea.xaml
    /// 
    /// Class handles the PrintArea tool form.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    [System.Reflection.Obfuscation(Feature = "renaming",
                                            ApplyToMembers = true)]

    public partial class ToolPrintArea : Window
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const ushort _sessionUPI = 600;

        const double _unitsToInches = (1.00 / _sessionUPI);
        const double _unitsToMilliMetres = (25.4 / _sessionUPI);

        private static readonly int[] _subsetPDLs =
        {
            (int) ToolCommonData.ePrintLang.PCL,
            (int) ToolCommonData.ePrintLang.PCLXL,
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
            (int) PCLPaperSizes.eIndex.ISO_A3,
            (int) PCLPaperSizes.eIndex.ISO_A5,
            (int) PCLPaperSizes.eIndex.ISO_A6,
            (int) PCLPaperSizes.eIndex.RA4,
            (int) PCLPaperSizes.eIndex.ANSI_A_Letter,
            (int) PCLPaperSizes.eIndex.ANSI_B_Ledger_Tabloid,
            (int) PCLPaperSizes.eIndex.Legal,
            (int) PCLPaperSizes.eIndex.Executive,
            (int) PCLPaperSizes.eIndex.Foolscap,
            (int) PCLPaperSizes.eIndex.Statement,
            (int) PCLPaperSizes.eIndex.Env_Monarch,
            (int) PCLPaperSizes.eIndex.Env_Com9,
            (int) PCLPaperSizes.eIndex.Env_Com10,
            (int) PCLPaperSizes.eIndex.Env_Intl_DL,
            (int) PCLPaperSizes.eIndex.Env_Intl_B5,
            (int) PCLPaperSizes.eIndex.Env_Intl_C5,
            (int) PCLPaperSizes.eIndex.Env_Intl_C6,
            (int) PCLPaperSizes.eIndex.JIS_B5,
            (int) PCLPaperSizes.eIndex.JIS_B6,
            (int) PCLPaperSizes.eIndex.JP_Postcard,
            (int) PCLPaperSizes.eIndex.JP_PostcardDouble,
            (int) PCLPaperSizes.eIndex.CN_16K,
            (int) PCLPaperSizes.eIndex.CN_16K_184x260,
            (int) PCLPaperSizes.eIndex.CN_16K_195x270,
            (int) PCLPaperSizes.eIndex.Oficio_216x340,
            (int) PCLPaperSizes.eIndex.Card_10x15cm,
            (int) PCLPaperSizes.eIndex.Card_3x5,
            (int) PCLPaperSizes.eIndex.Card_4x6,
            (int) PCLPaperSizes.eIndex.Card_5x7,
            (int) PCLPaperSizes.eIndex.Card_5x8,
            (int) PCLPaperSizes.eIndex.Custom,
        };

        private static readonly int[] _subsetPlexModes =
        {
            (int) PCLPlexModes.eIndex.Simplex,
            (int) PCLPlexModes.eIndex.DuplexLongEdge,
            (int) PCLPlexModes.eIndex.DuplexShortEdge
        };

        private static readonly int[] _subsetPaperTypes =
        {
            (int) PCLPaperTypes.eIndex.NotSet,
            (int) PCLPaperTypes.eIndex.Plain
        };

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private ToolCommonData.ePrintLang _crntPDL;

        private int _ctPDLs;
        private int _ctOrientations;
        private int _ctPaperSizes;
        private int _ctPaperTypes;
        private int _ctPlexModes;
        private int _ctPJLCommands;

        private int _indxPDL;
        private int _indxOrientationPCL;
        private int _indxOrientationPCLXL;
        private int _indxPaperSizePCL;
        private int _indxPaperSizePCLXL;
        private int _indxPaperTypePCL;
        private int _indxPaperTypePCLXL;
        private int _indxPlexModePCL;
        private int _indxPlexModePCLXL;
        private int _indxPJLCommandPCL;
        private int _indxPJLCommandPCLXL;

        private ushort _customShortEdgeDots;
        private ushort _customShortEdgeDotsPCL;
        private ushort _customShortEdgeDotsPCLXL;
        private ushort _customLongEdgeDots;
        private ushort _customLongEdgeDotsPCL;
        private ushort _customLongEdgeDotsPCLXL;

        private bool _formAsMacroPCL;
        private bool _formAsMacroPCLXL;
        private bool _flagCustomPaperSize;
        private bool _flagTrayIdUnknown;
        private bool _flagForceCustomPaperSize;
        private bool _flagCustomUseMetric;
        private bool _flagCustomUseMetricPCL;
        private bool _flagCustomUseMetricPCLXL;

        private bool _initialised;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T o o l P r i n t A r e a                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolPrintArea(ref ToolCommonData.ePrintLang crntPDL)
        {
            InitializeComponent();

            Initialise();

            crntPDL = _crntPDL;
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
                string pjlCommand;

                int indxPaperSize;

                BinaryWriter binWriter = null;

                TargetCore.RequestStreamOpen(
                    ref binWriter,
                    ToolCommonData.eToolIds.PrintArea,
                    ToolCommonData.eToolSubIds.None,
                    _crntPDL);

                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                {
                    if (_indxPJLCommandPCL == 0)
                        pjlCommand = string.Empty;
                    else
                        pjlCommand = cbPJLCommand.Text;

                    indxPaperSize = _subsetPaperSizes[_indxPaperSizePCL];

                    if (_flagForceCustomPaperSize)
                    {
                        PCLPaperSizes.CustomDataCopy(indxPaperSize);

                        indxPaperSize = (int)PCLPaperSizes.eIndex.Custom;
                    }

                    ToolPrintAreaPCL.GenerateJob(
                        binWriter,
                        indxPaperSize,
                        _subsetPaperTypes[_indxPaperTypePCL],
                        _subsetOrientations[_indxOrientationPCL],
                        _subsetPlexModes[_indxPlexModePCL],
                        pjlCommand,
                        _formAsMacroPCL);
                }
                else
                {
                    if (_indxPJLCommandPCLXL == 0)
                        pjlCommand = string.Empty;
                    else
                        pjlCommand = cbPJLCommand.Text;

                    indxPaperSize = _subsetPaperSizes[_indxPaperSizePCLXL];

                    if (_flagForceCustomPaperSize)
                    {
                        PCLPaperSizes.CustomDataCopy(indxPaperSize);

                        indxPaperSize = (int)PCLPaperSizes.eIndex.Custom;
                    }

                    ToolPrintAreaPCLXL.GenerateJob(
                        binWriter,
                        indxPaperSize,
                        _subsetPaperTypes[_indxPaperTypePCLXL],
                        _subsetOrientations[_indxOrientationPCLXL],
                        _subsetPlexModes[_indxPlexModePCLXL],
                        pjlCommand,
                        _formAsMacroPCLXL,
                        _flagTrayIdUnknown,
                        _flagCustomUseMetricPCLXL);
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

                SetPaperMetrics(true);
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

                SetCustomPaperControls();

                SetPaperIdentifiers();
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
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P J L C o m m a n d _ S e l e c t i o n C h a n g e d          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PJL Command item has changed.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPJLCommand_SelectionChanged(object sender,
                                                   SelectionChangedEventArgs e)
        {
            if (_initialised && cbPJLCommand.HasItems)
            {
                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                    _indxPJLCommandPCL = cbPJLCommand.SelectedIndex;
                else
                    _indxPJLCommandPCLXL = cbPJLCommand.SelectedIndex;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P l e x M o d e _ S e l e c t i o n C h a n g e d              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Plex mode item has changed.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPlexMode_SelectionChanged(object sender,
                                                SelectionChangedEventArgs e)
        {
            if (_initialised && cbPlexMode.HasItems)
            {
                int plexMode = cbPlexMode.SelectedIndex;

                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                    _indxPlexModePCL = plexMode;
                else
                    _indxPlexModePCLXL = plexMode;
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
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _formAsMacroPCL = true;
            else
                _formAsMacroPCLXL = true;
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
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _formAsMacroPCL = false;
            else
                _formAsMacroPCLXL = false;
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
        // i n i t i a l i s e                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialisation.                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void Initialise()
        {
            int index;

            _initialised = false;

            //----------------------------------------------------------------//
            //                                                                //
            // Populate form.                                                 //
            //                                                                //
            //----------------------------------------------------------------//

            cbPDL.Items.Clear();

            _ctPDLs = _subsetPDLs.Length;

            for (int i = 0; i < _ctPDLs; i++)
            {
                index = _subsetPDLs[i];

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

            cbPlexMode.Items.Clear();

            _ctPlexModes = _subsetPlexModes.Length;

            for (int i = 0; i < _ctPlexModes; i++)
            {
                index = _subsetPlexModes[i];

                cbPlexMode.Items.Add(PCLPlexModes.GetName(index));
            }

            //----------------------------------------------------------------//

            cbPJLCommand.Items.Clear();

            _ctPJLCommands = 5;

            cbPJLCommand.Items.Add("<none>");
            cbPJLCommand.Items.Add("@PJL SET WIDEA4 = YES");
            cbPJLCommand.Items.Add("@PJL SET WIDEA4 = NO");
            cbPJLCommand.Items.Add("@PJL SET EDGETOEDGE = YES");
            cbPJLCommand.Items.Add("@PJL SET EDGETOEDGE = NO");

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

            if (_flagCustomUseMetric)
                rbCustomUseMetric.IsChecked = true;
            else
                rbCustomUseMetric.IsChecked = false;

            _initialised = true;
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
            ToolPrintAreaPersist.LoadDataCommon(ref _indxPDL);

            ToolPrintAreaPersist.LoadDataPCL("PCL",
                                             ref _indxOrientationPCL,
                                             ref _indxPaperSizePCL,
                                             ref _indxPaperTypePCL,
                                             ref _indxPlexModePCL,
                                             ref _indxPJLCommandPCL,
                                             ref _formAsMacroPCL,
                                             ref _flagCustomUseMetricPCL,
                                             ref _customShortEdgeDotsPCL,
                                             ref _customLongEdgeDotsPCL);

            ToolPrintAreaPersist.LoadDataPCL("PCLXL",
                                             ref _indxOrientationPCLXL,
                                             ref _indxPaperSizePCLXL,
                                             ref _indxPaperTypePCLXL,
                                             ref _indxPlexModePCLXL,
                                             ref _indxPJLCommandPCLXL,
                                             ref _formAsMacroPCLXL,
                                             ref _flagCustomUseMetricPCLXL,
                                             ref _customShortEdgeDotsPCLXL,
                                             ref _customLongEdgeDotsPCLXL);

            //----------------------------------------------------------------//

            if ((_indxPDL < 0) || (_indxPDL >= _ctPDLs))
                _indxPDL = 0;

            _crntPDL = (ToolCommonData.ePrintLang)_subsetPDLs[_indxPDL];

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

            if ((_indxPJLCommandPCL < 0) ||
                (_indxPJLCommandPCL >= _ctPJLCommands))
                _indxPJLCommandPCL = 0;

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

            if ((_indxPJLCommandPCLXL < 0) ||
                (_indxPJLCommandPCLXL >= _ctPJLCommands))
                _indxPJLCommandPCLXL = 0;
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

            ToolPrintAreaPersist.SaveDataCommon(_indxPDL);

            ToolPrintAreaPersist.SaveDataPCL("PCL",
                                             _indxOrientationPCL,
                                             _indxPaperSizePCL,
                                             _indxPaperTypePCL,
                                             _indxPlexModePCL,
                                             _indxPJLCommandPCL,
                                             _formAsMacroPCL,
                                             _flagCustomUseMetricPCL,
                                             _customShortEdgeDotsPCL,
                                             _customLongEdgeDotsPCL);

            ToolPrintAreaPersist.SaveDataPCL("PCLXL",
                                             _indxOrientationPCLXL,
                                             _indxPaperSizePCLXL,
                                             _indxPaperTypePCLXL,
                                             _indxPlexModePCLXL,
                                             _indxPJLCommandPCLXL,
                                             _formAsMacroPCLXL,
                                             _flagCustomUseMetricPCLXL,
                                             _customShortEdgeDotsPCLXL,
                                             _customLongEdgeDotsPCLXL);
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
                cbPlexMode.SelectedIndex = _indxPlexModePCL;
                cbPJLCommand.SelectedIndex = _indxPJLCommandPCL;
                chkOptFormAsMacro.IsChecked = _formAsMacroPCL;

                lbLogPageMarginLR.Visibility = Visibility.Visible;
                lbLogPageMarginTB.Visibility = Visibility.Visible;
                lbLogPageLength.Visibility = Visibility.Visible;
                lbLogPageWidth.Visibility = Visibility.Visible;

                txtLogPageMarginLRDots.Visibility = Visibility.Visible;
                txtLogPageMarginLRMetric.Visibility = Visibility.Visible;
                txtLogPageMarginLRImperial.Visibility = Visibility.Visible;

                txtLogPageMarginTBDots.Visibility = Visibility.Visible;
                txtLogPageMarginTBMetric.Visibility = Visibility.Visible;
                txtLogPageMarginTBImperial.Visibility = Visibility.Visible;

                txtLogPageLengthDots.Visibility = Visibility.Visible;
                txtLogPageLengthMetric.Visibility = Visibility.Visible;
                txtLogPageLengthImperial.Visibility = Visibility.Visible;

                txtLogPageWidthDots.Visibility = Visibility.Visible;
                txtLogPageWidthMetric.Visibility = Visibility.Visible;
                txtLogPageWidthImperial.Visibility = Visibility.Visible;

                _flagCustomUseMetric = _flagCustomUseMetricPCL;
                _customShortEdgeDots = _customShortEdgeDotsPCL;
                _customLongEdgeDots = _customLongEdgeDotsPCL;
            }
            else
            {
                cbOrientation.SelectedIndex = _indxOrientationPCLXL;
                cbPaperSize.SelectedIndex = _indxPaperSizePCLXL;
                cbPaperType.SelectedIndex = _indxPaperTypePCLXL;
                cbPlexMode.SelectedIndex = _indxPlexModePCLXL;
                cbPJLCommand.SelectedIndex = _indxPJLCommandPCLXL;
                chkOptFormAsMacro.IsChecked = _formAsMacroPCLXL;

                lbLogPageMarginLR.Visibility = Visibility.Hidden;
                lbLogPageMarginTB.Visibility = Visibility.Hidden;
                lbLogPageLength.Visibility = Visibility.Hidden;
                lbLogPageWidth.Visibility = Visibility.Hidden;

                txtLogPageMarginLRDots.Visibility = Visibility.Hidden;
                txtLogPageMarginLRMetric.Visibility = Visibility.Hidden;
                txtLogPageMarginLRImperial.Visibility = Visibility.Hidden;

                txtLogPageMarginTBDots.Visibility = Visibility.Hidden;
                txtLogPageMarginTBMetric.Visibility = Visibility.Hidden;
                txtLogPageMarginTBImperial.Visibility = Visibility.Hidden;

                txtLogPageLengthDots.Visibility = Visibility.Hidden;
                txtLogPageLengthMetric.Visibility = Visibility.Hidden;
                txtLogPageLengthImperial.Visibility = Visibility.Hidden;

                txtLogPageWidthDots.Visibility = Visibility.Hidden;
                txtLogPageWidthMetric.Visibility = Visibility.Hidden;
                txtLogPageWidthImperial.Visibility = Visibility.Hidden;

                _flagCustomUseMetric = _flagCustomUseMetricPCLXL;
                _customShortEdgeDots = _customShortEdgeDotsPCLXL;
                _customLongEdgeDots = _customLongEdgeDotsPCLXL;
            }

            if (_flagCustomUseMetric)
                rbCustomUseMetric.IsChecked = true;
            else
                rbCustomUseImperial.IsChecked = true;

            PCLPaperSizes.SetCustomShortEdge(_customShortEdgeDots,
                                             _sessionUPI);
            PCLPaperSizes.SetCustomLongEdge(_customLongEdgeDots,
                                            _sessionUPI);

            SetCustomPaperControls();

            SetPaperIdentifiers();
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
                _indxPlexModePCL = cbPlexMode.SelectedIndex;
                _indxPJLCommandPCL = cbPJLCommand.SelectedIndex;

                _flagCustomUseMetricPCL = _flagCustomUseMetric;
                _customShortEdgeDotsPCL = _customShortEdgeDots;
                _customLongEdgeDotsPCL = _customLongEdgeDots;

                if (chkOptFormAsMacro.IsChecked == true)
                    _formAsMacroPCL = true;
                else
                    _formAsMacroPCL = false;
            }
            else
            {
                _indxOrientationPCLXL = cbOrientation.SelectedIndex;
                _indxPaperSizePCLXL = cbPaperSize.SelectedIndex;
                _indxPaperTypePCLXL = cbPaperType.SelectedIndex;
                _indxPlexModePCLXL = cbPlexMode.SelectedIndex;
                _indxPJLCommandPCLXL = cbPJLCommand.SelectedIndex;

                _flagCustomUseMetricPCLXL = _flagCustomUseMetric;
                _customShortEdgeDotsPCLXL = _customShortEdgeDots;
                _customLongEdgeDotsPCLXL = _customLongEdgeDots;

                if (chkOptFormAsMacro.IsChecked == true)
                    _formAsMacroPCLXL = true;
                else
                    _formAsMacroPCLXL = false;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b C u s t o m U s e I m p e r i a l _ C l i c k                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Custom size preference - Imperial' radio button   //
        // is selected.                                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbCustomUseImperial_Click(object sender,
                                               RoutedEventArgs e)
        {
            _flagCustomUseMetric = false;

            SetCustomPaperControls();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b C u s t o m U s e M e t r i c _ C l i c k                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Custom size preference - Metric' radio button is  //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbCustomUseMetric_Click(object sender,
                                             RoutedEventArgs e)
        {
            _flagCustomUseMetric = true;

            SetCustomPaperControls();
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
        // s e t C u s t o m P a p e r C o n t r o l s                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set the visiblility etc. of the custom paper size controls.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetCustomPaperControls()
        {
            int indxPaperSize;

            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                indxPaperSize = _subsetPaperSizes[_indxPaperSizePCL];
            else
                indxPaperSize = _subsetPaperSizes[_indxPaperSizePCLXL];

            _flagForceCustomPaperSize = false;

            if (PCLPaperSizes.IsCustomSize(indxPaperSize))
            {
                _flagCustomPaperSize = true;

                lbCustomUnits.Visibility = Visibility.Visible;
                rbCustomUseImperial.Visibility = Visibility.Visible;
                rbCustomUseMetric.Visibility = Visibility.Visible;

                if (_flagCustomUseMetric)
                {
                    txtLongEdgeImperial.IsReadOnly = true;
                    txtLongEdgeMetric.IsReadOnly = false;
                    txtShortEdgeImperial.IsReadOnly = true;
                    txtShortEdgeMetric.IsReadOnly = false;
                }
                else
                {
                    txtLongEdgeImperial.IsReadOnly = false;
                    txtLongEdgeMetric.IsReadOnly = true;
                    txtShortEdgeImperial.IsReadOnly = false;
                    txtShortEdgeMetric.IsReadOnly = true;
                }
            }
            else
            {
                _flagCustomPaperSize = false;

                lbCustomUnits.Visibility = Visibility.Hidden;
                rbCustomUseImperial.Visibility = Visibility.Hidden;
                rbCustomUseMetric.Visibility = Visibility.Hidden;

                txtLongEdgeImperial.IsReadOnly = true; // not needed if hidden ??
                txtLongEdgeMetric.IsReadOnly = true;
                txtShortEdgeImperial.IsReadOnly = true;
                txtShortEdgeMetric.IsReadOnly = true;
            }

            //----------------------------------------------------------------//

            if (_flagCustomPaperSize)
                PCLPaperSizes.ResetCustomDesc();

            SetPaperMetrics(false);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t P a p e r I d e n t i f i e r s                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set the contents of the Paper identifications fields.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetPaperIdentifiers()
        {
            int indxPaperSize;

            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                indxPaperSize = _subsetPaperSizes[_indxPaperSizePCL];

                txtIdPaperName.Text =
                    PCLPaperSizes.GetName(indxPaperSize);

                txtIdPaperDesc.Text =
                    PCLPaperSizes.GetDesc(indxPaperSize);

                grpIdData.Visibility = Visibility.Visible;

                if (PCLPaperSizes.IsIdUnknownPCL(indxPaperSize))
                {
                    txtIdEnum.Text = "<unknown>";
                    _flagTrayIdUnknown = true;
                }
                else
                {
                    txtIdEnum.Text =
                        PCLPaperSizes.GetIdPCL(indxPaperSize).ToString();
                    _flagTrayIdUnknown = false;
                }

                lbIdEnum.Content = "PCL identifier:";

                lbIdName.Visibility = Visibility.Hidden;
                txtIdName.Visibility = Visibility.Hidden;

                if (_flagTrayIdUnknown)
                {
                    _flagForceCustomPaperSize = true;
                    lbIdUnknown.Visibility = Visibility.Visible;
                }
                else
                {
                    _flagForceCustomPaperSize = false;
                    lbIdUnknown.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                string idName = string.Empty;

                indxPaperSize = _subsetPaperSizes[_indxPaperSizePCLXL];

                txtIdPaperName.Text =
                    PCLPaperSizes.GetName(indxPaperSize);

                txtIdPaperDesc.Text =
                    PCLPaperSizes.GetDesc(indxPaperSize);

                if (PCLPaperSizes.IsCustomSize(indxPaperSize))
                {
                    grpIdData.Visibility = Visibility.Hidden;
                }
                else
                {
                    grpIdData.Visibility = Visibility.Visible;

                    if (PCLPaperSizes.IsEnumUnknownPCLXL(indxPaperSize))
                    {
                        txtIdEnum.Text = "<unknown>";
                        _flagTrayIdUnknown = true;
                    }
                    else
                    {
                        txtIdEnum.Text =
                            PCLPaperSizes.GetIdPCLXL(indxPaperSize).ToString();
                        _flagTrayIdUnknown = false;
                    }

                    idName = PCLPaperSizes.GetNamePCLXL(indxPaperSize);

                    txtIdName.Text = idName;

                    lbIdEnum.Content = "PCL XL enum.:";

                    lbIdName.Visibility = Visibility.Visible;
                    txtIdName.Visibility = Visibility.Visible;

                    if ((_flagTrayIdUnknown) && (idName == string.Empty))
                    {
                        _flagForceCustomPaperSize = true;
                        lbIdUnknown.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _flagForceCustomPaperSize = false;
                        lbIdUnknown.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t P a p e r M e t r i c s                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set the contents of the Paper metrics physical and/or logical data //
        // fields.                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetPaperMetrics(bool logicalOnly)
        {
            const int zero = 0;

            PCLOrientations.eAspect aspect;

            int indxOrientation,
                  indxPaperSize,
                  indxPaperType;

            ushort sizeShortEdge,
                   sizeLongEdge,
                   marginsLogicalLand,
                   marginsLogicalPort,
                   marginsLogical,
                   marginsUnprintable;

            ushort widthPrintable,
                   widthLogical,
                   lengthPrintable,
                   lengthLogical;

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

            sizeLongEdge = PCLPaperSizes.GetSizeLongEdge(indxPaperSize,
                                                         _sessionUPI);

            sizeShortEdge = PCLPaperSizes.GetSizeShortEdge(indxPaperSize,
                                                           _sessionUPI);

            marginsUnprintable =
                PCLPaperSizes.GetMarginsUnprintable(indxPaperSize,
                                                    _sessionUPI);

            marginsLogicalLand =
                PCLPaperSizes.GetMarginsLogicalLand(indxPaperSize,
                                                    _sessionUPI);

            marginsLogicalPort =
                PCLPaperSizes.GetMarginsLogicalPort(indxPaperSize,
                                                    _sessionUPI);

            if (aspect == PCLOrientations.eAspect.Portrait)
            {
                marginsLogical = marginsLogicalPort;

                widthPrintable = (ushort)(sizeShortEdge -
                                          (marginsUnprintable * 2));

                widthLogical = (ushort)(sizeShortEdge -
                                          (marginsLogicalPort * 2));

                lengthPrintable = (ushort)(sizeLongEdge -
                                          (marginsUnprintable * 2));

                lengthLogical = sizeLongEdge;
            }
            else
            {
                marginsLogical = marginsLogicalLand;

                widthPrintable = (ushort)(sizeLongEdge -
                                          (marginsUnprintable * 2));

                widthLogical = (ushort)(sizeLongEdge -
                                          (marginsLogicalLand * 2));

                lengthPrintable = (ushort)(sizeShortEdge -
                                          (marginsUnprintable * 2));

                lengthLogical = sizeShortEdge;
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Display the size values (in dots at specified dpi).            //
            //                                                                //
            //----------------------------------------------------------------//

            if (!logicalOnly)
            {
                txtShortEdgeDots.Text = sizeShortEdge.ToString("F0");
                txtLongEdgeDots.Text = sizeLongEdge.ToString("F0");
                txtDotsPerInch.Text = _sessionUPI.ToString("F0");
            }

            txtLogPageMarginLRDots.Text = marginsLogical.ToString("F0");
            txtLogPageMarginTBDots.Text = zero.ToString("F0");
            txtLogPageWidthDots.Text = widthLogical.ToString("F0");
            txtLogPageLengthDots.Text = lengthLogical.ToString("F0");

            txtUnprintableMarginDots.Text = marginsUnprintable.ToString("F0");
            txtPrintWidthDots.Text = widthPrintable.ToString("F0");
            txtPrintLengthDots.Text = lengthPrintable.ToString("F0");

            //----------------------------------------------------------------//
            //                                                                //
            // Display the derived metric values.                             //
            //                                                                //
            //----------------------------------------------------------------//

            if (!logicalOnly)
            {
                txtShortEdgeMetric.Text =
                    (Math.Round((sizeShortEdge *
                                  _unitsToMilliMetres), 2)).ToString("F1");

                txtLongEdgeMetric.Text =
                    (Math.Round((sizeLongEdge *
                                  _unitsToMilliMetres), 2)).ToString("F1");
            }

            txtLogPageMarginLRMetric.Text
                = (Math.Round((marginsLogical *
                                _unitsToMilliMetres), 2)).ToString("F1");
            txtLogPageMarginTBMetric.Text
                = zero.ToString("F1");
            txtLogPageWidthMetric.Text
                = (Math.Round((widthLogical *
                               _unitsToMilliMetres), 2)).ToString("F1");
            txtLogPageLengthMetric.Text
                = (Math.Round((lengthLogical *
                                _unitsToMilliMetres), 2)).ToString("F1");

            txtUnprintableMarginMetric.Text
                = (Math.Round((marginsUnprintable *
                                _unitsToMilliMetres), 2)).ToString("F1");
            txtPrintWidthMetric.Text
                = (Math.Round((widthPrintable *
                                _unitsToMilliMetres), 2)).ToString("F1");
            txtPrintLengthMetric.Text
                = (Math.Round((lengthPrintable *
                                _unitsToMilliMetres), 2)).ToString("F1");

            //----------------------------------------------------------------//
            //                                                                //
            // Display the derived imperial values.                           //
            //                                                                //
            //----------------------------------------------------------------//

            if (!logicalOnly)
            {
                txtShortEdgeImperial.Text =
                    (Math.Round((sizeShortEdge *
                                  _unitsToInches), 3)).ToString("F3");

                txtLongEdgeImperial.Text =
                    (Math.Round((sizeLongEdge *
                                  _unitsToInches), 3)).ToString("F3");
            }

            txtLogPageMarginLRImperial.Text
                = (Math.Round((marginsLogical *
                                _unitsToInches), 3)).ToString("F3");
            txtLogPageMarginTBImperial.Text
                = zero.ToString("F3");
            txtLogPageWidthImperial.Text
                = (Math.Round((widthLogical *
                                _unitsToInches), 3)).ToString("F3");
            txtLogPageLengthImperial.Text
                = (Math.Round((lengthLogical *
                                _unitsToInches), 3)).ToString("F3");

            txtUnprintableMarginImperial.Text
                = (Math.Round((marginsUnprintable *
                                _unitsToInches), 3)).ToString("F3");
            txtPrintWidthImperial.Text
                = (Math.Round((widthPrintable *
                                _unitsToInches), 3)).ToString("F3");
            txtPrintLengthImperial.Text
                = (Math.Round((lengthPrintable *
                                _unitsToInches), 3)).ToString("F3");
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t L o n g E d g e I m p e r i a l _ L o s t F o c u s          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // The 'long edge (imperial) text box has changed.                    //
        // If the current paper size is 'Custom', validate the value and (if  //
        // valid) store the value (converted to 600 dpi dots) as the current  //
        // short edge dimension of the Custom paper size.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtLongEdgeImperial_LostFocus(object sender,
                                                 RoutedEventArgs e)
        {
            if ((_flagCustomPaperSize) && (!_flagCustomUseMetric))
            {
                if (ValidateEdgeImperial(false, true))
                {
                    txtLongEdgeDots.Text = _customLongEdgeDots.ToString();

                    txtLongEdgeMetric.Text =
                        Math.Round((_customLongEdgeDots *
                                     _unitsToMilliMetres), 2).ToString("F1");

                    PCLPaperSizes.SetCustomLongEdge(_customLongEdgeDots,
                                                    _sessionUPI);

                    SetPaperMetrics(true);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t L o n g E d g e M e t r i c _ L o s t F o c u s              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // The 'long edge (metric) text box has changed.                      //
        // If the current paper size is 'Custom', validate the value and (if  //
        // valid) store the value (converted to 600 dpi dots) as the current  //
        // short edge dimension of the Custom paper size.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtLongEdgeMetric_LostFocus(object sender,
                                                 RoutedEventArgs e)
        {
            if ((_flagCustomPaperSize) && (_flagCustomUseMetric))
            {
                if (ValidateEdgeMetric(false, true))
                {
                    txtLongEdgeDots.Text = _customLongEdgeDots.ToString();

                    txtLongEdgeImperial.Text =
                        (Math.Round((_customLongEdgeDots *
                                     _unitsToInches), 3)).ToString("F3");

                    PCLPaperSizes.SetCustomLongEdge(_customLongEdgeDots,
                                                    _sessionUPI);

                    SetPaperMetrics(true);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t S h o r t E d g e I m p e r i a l _ L o s t F o c u s        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // The 'short edge (imperial) text box has lost focus.                //
        // If the current paper size is 'Custom', validate the value and (if  //
        // valid) store the value (converted to 600 dpi dots) as the current  //
        // short edge dimension of the Custom paper size.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtShortEdgeImperial_LostFocus(object sender,
                                                    RoutedEventArgs e)
        {
            if ((_flagCustomPaperSize) && (!_flagCustomUseMetric))
            {
                if (ValidateEdgeImperial(true, true))
                {
                    txtShortEdgeDots.Text = _customShortEdgeDots.ToString();

                    txtShortEdgeMetric.Text =
                        (Math.Round((_customShortEdgeDots *
                                      _unitsToMilliMetres), 2)).ToString("F1");

                    PCLPaperSizes.SetCustomShortEdge(_customShortEdgeDots,
                                                    _sessionUPI);

                    SetPaperMetrics(true);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t S h o r t E d g e M e t r i c _ L o s t F o c u s            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // The 'short edge (metric) text box has lost focus.                  //
        // If the current paper size is 'Custom', validate the value and (if  //
        // valid) store the value (converted to 600 dpi dots) as the current  //
        // short edge dimension of the Custom paper size.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtShortEdgeMetric_LostFocus(object sender,
                                                  RoutedEventArgs e)
        {
            if ((_flagCustomPaperSize) && (_flagCustomUseMetric))
            {
                if (ValidateEdgeMetric(true, true))
                {
                    txtShortEdgeDots.Text = _customShortEdgeDots.ToString();

                    txtShortEdgeImperial.Text =
                        (Math.Round((_customShortEdgeDots *
                                      _unitsToInches), 3)).ToString("F3");

                    PCLPaperSizes.SetCustomShortEdge(_customShortEdgeDots,
                                                    _sessionUPI);

                    SetPaperMetrics(true);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e E d g e I m p e r i a l                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate page size Edge (imperial) value.                          //
        // This is only used when dealing with a Custom page size, and when   //
        // Imperial has been selected for the Custom Size units.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidateEdgeImperial(bool shortEdge,
                                             bool lostFocusEvent)
        {
            const double scaleToDots = 1 / _unitsToInches;
            const double minValShort = 3.00;
            const double maxValShort = 12.00;
            const double minValLong = 5.00;
            const double maxValLong = 18.00;
            const double defValShort = 8.27;  // A4 dimension
            const double defValLong = 11.69;

            double minVal,
                   maxVal,
                   value = 0;

            ushort valueDots = 0;

            bool OK = true;

            string crntText;
            string edgeThis;
            string edgeOther;
            string edgeOtherVal;

            if (shortEdge)
            {
                edgeThis = "Short";
                edgeOther = "Long";
                edgeOtherVal =
                    (Math.Round((_customLongEdgeDots *
                                 _unitsToInches), 3)).ToString("F3");
                minVal = minValShort;
                maxVal = maxValShort;
                crntText = txtShortEdgeImperial.Text;
            }
            else
            {
                edgeThis = "Long";
                edgeOther = "Short";
                edgeOtherVal =
                    (Math.Round((_customShortEdgeDots *
                                 _unitsToInches), 3)).ToString("F3");
                minVal = minValLong;
                maxVal = maxValLong;
                crntText = txtLongEdgeImperial.Text;
            }

            OK = double.TryParse(crntText, out value);

            if ((value < minVal) || (value > maxVal))
                OK = false;
            else
            {
                valueDots = (ushort)(value * scaleToDots);

                if (shortEdge)
                {
                    if (valueDots > _customLongEdgeDots)
                        OK = false;
                }
                else
                {
                    if (valueDots < _customShortEdgeDots)
                        OK = false;
                }
            }

            if (OK)
            {
                if (shortEdge)
                    _customShortEdgeDots = valueDots;
                else
                    _customLongEdgeDots = valueDots;
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Report validation error.                                   //
                // Which branch is selected depends on whether the validation //
                // was triggered by a LostFocus event (in which case focus is //
                // now elsewhere), or by some other event (the most usual     //
                // being a TextChanged event).                                //
                //                                                            // 
                // As the minimum value is several digits long, a TextChanged //
                // event is not used, since this would cause a validation     //
                // error if an existing value was highlighted and replaced by //
                // (atarting to) type a new value, since the initial single   //
                // digit, then double-digit values, would be considered to be //
                // invalid.                                                   //
                //                                                            //
                //------------------------------------------------------------//

                string errData = edgeThis + " Edge value " + crntText +
                                 " inch is invalid, or incompatible with " +
                                 edgeOther + " Edge value.\n\n" +
                                 "Valid range is :\n\t" +
                                 minVal.ToString("F3") + " <= value <= " +
                                 maxVal.ToString("F3") + "\n\n" +
                                 edgeOther + " Edge value is " + edgeOtherVal +
                                 " inch.";
                if (lostFocusEvent)
                {
                    string newText;

                    if (shortEdge)
                        newText = defValShort.ToString("F2");
                    else
                        newText = defValLong.ToString("F2");

                    MessageBox.Show(errData + "\n\n" +
                                    "Value will be reset to default " +
                                    newText,
                                    "Custom page size data",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    if (shortEdge)
                    {
                        _customShortEdgeDots =
                            (ushort)(defValShort * scaleToDots);

                        txtShortEdgeImperial.Text = newText;
                    }
                    else
                    {
                        _customLongEdgeDots =
                            (ushort)(defValLong * scaleToDots);

                        txtLongEdgeImperial.Text = newText;
                    }
                }
                else
                {
                    MessageBox.Show(errData,
                                    "Custom page size data",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    if (shortEdge)
                    {
                        txtShortEdgeImperial.Focus();
                        txtShortEdgeImperial.SelectAll();
                    }
                    else
                    {
                        txtLongEdgeImperial.Focus();
                        txtLongEdgeImperial.SelectAll();
                    }
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e E d g e M e t r i c                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate page size Edge (metric) value.                            //
        // This is only used when dealing with a Custom page size, and when   //
        // Metric has been selected for the Custom Size units.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidateEdgeMetric(bool shortEdge,
                                           bool lostFocusEvent)
        {
            const double scaleToDots = 1 / _unitsToMilliMetres;
            const double scaleMinMax = _sessionUPI * _unitsToMilliMetres;
            const double minValShort = scaleMinMax * 3.00;
            const double maxValShort = scaleMinMax * 12.00;
            const double minValLong = scaleMinMax * 5.00;
            const double maxValLong = scaleMinMax * 18.00;
            const double defValShort = 210;  // A4 dimension
            const double defValLong = 297;

            double minVal,
                   maxVal,
                   value = 0;

            ushort valueDots = 0;

            bool OK = true;

            string crntText;
            string edgeThis;
            string edgeOther;
            string edgeOtherVal;

            if (shortEdge)
            {
                edgeThis = "Short";
                edgeOther = "Long";
                edgeOtherVal =
                    (Math.Round((_customLongEdgeDots *
                                 _unitsToMilliMetres), 2)).ToString("F1");
                minVal = minValShort;
                maxVal = maxValShort;
                crntText = txtShortEdgeMetric.Text;
            }
            else
            {
                edgeThis = "Long";
                edgeOther = "Short";
                edgeOtherVal =
                    (Math.Round((_customShortEdgeDots *
                                 _unitsToMilliMetres), 2)).ToString("F1");
                minVal = minValLong;
                maxVal = maxValLong;
                crntText = txtLongEdgeMetric.Text;
            }

            OK = double.TryParse(crntText, out value);

            if ((value < minVal) || (value > maxVal))
                OK = false;
            else
            {
                valueDots = (ushort)(value * scaleToDots);

                if (shortEdge)
                {
                    if (valueDots > _customLongEdgeDots) // need to consider rounding
                        OK = false;
                }
                else
                {
                    if (valueDots < _customShortEdgeDots)
                        OK = false;
                }
            }

            if (OK)
            {
                if (shortEdge)
                    _customShortEdgeDots = valueDots;
                else
                    _customLongEdgeDots = valueDots;
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Report validation error.                                   //
                // Which branch is selected depends on whether the validation //
                // was triggered by a LostFocus event (in which case focus is //
                // now elsewhere), or by some other event (the most usual     //
                // being a TextChanged event).                                //
                //                                                            // 
                // As the minimum value is several digits long, a TextChanged //
                // event is not used, since this would cause a validation     //
                // error if an existing value was highlighted and replaced by //
                // (atarting to) type a new value, since the initial single   //
                // digit, then double-digit values, would be considered to be //
                // invalid.                                                   //
                //                                                            //
                //------------------------------------------------------------//

                string errData = edgeThis + " Edge value " + crntText +
                                 " mm is invalid, or incompatible with " +
                                 edgeOther + " Edge value.\n\n" +
                                 "Valid range is :\n\t" +
                                 minVal.ToString("F0") + " <= value <= " +
                                 maxVal.ToString("F0") + "\n\n" +
                                 edgeOther + " Edge value is " + edgeOtherVal +
                                 " mm.";

                if (lostFocusEvent)
                {
                    string newText;

                    if (shortEdge)
                        newText = defValShort.ToString("F0");
                    else
                        newText = defValLong.ToString("F0");

                    MessageBox.Show(errData + "\n\n" +
                                    "Value will be reset to default " +
                                    newText,
                                    "Custom page size data",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    if (shortEdge)
                    {
                        _customShortEdgeDots =
                            (ushort)(defValShort * scaleToDots);

                        txtShortEdgeMetric.Text = newText;
                    }
                    else
                    {
                        _customLongEdgeDots =
                            (ushort)(defValLong * scaleToDots);

                        txtLongEdgeMetric.Text = newText;
                    }
                }
                else
                {
                    MessageBox.Show(errData,
                                    "Custom page size data",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    if (shortEdge)
                    {
                        txtShortEdgeMetric.Focus();
                        txtShortEdgeMetric.SelectAll();
                    }
                    else
                    {
                        txtLongEdgeMetric.Focus();
                        txtLongEdgeMetric.SelectAll();
                    }
                }
            }

            return OK;
        }
    }
}
