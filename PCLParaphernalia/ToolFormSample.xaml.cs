﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// <para>Interaction logic for ToolFormSample.xaml</para>
    /// <para>Class handles the FormSample tool form.</para>
    /// <para>© Chris Hutchinson 2012</para>
    ///
    /// </summary>
    [System.Reflection.Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public partial class ToolFormSample : Window
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const ushort _defaultMacroIdMainPCL = 32767;
        private const ushort _defaultMacroIdRearPCL = 32766;

        private const ushort _defaultPageCount = 3;

        private const string _defaultFormNameRootPCLXL = "testform";

        private static readonly int[] _subsetPDLs =
        {
            (int) ToolCommonData.PrintLang.PCL,
            (int) ToolCommonData.PrintLang.PCLXL,
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
            (int) PCLPaperSizes.Index.ISO_A3,
            (int) PCLPaperSizes.Index.ISO_A5,
            (int) PCLPaperSizes.Index.ISO_A6,
            (int) PCLPaperSizes.Index.ANSI_A_Letter,
            (int) PCLPaperSizes.Index.ANSI_B_Ledger_Tabloid,
            (int) PCLPaperSizes.Index.Legal,
            (int) PCLPaperSizes.Index.Executive
        };

        private static readonly int[] _subsetPlexModes =
        {
            (int) PCLPlexModes.Index.Simplex,
            (int) PCLPlexModes.Index.DuplexLongEdge,
            (int) PCLPlexModes.Index.DuplexShortEdge
        };

        private static readonly int[] _subsetPaperTypes =
        {
            (int) PCLPaperTypes.Index.NotSet,
            (int) PCLPaperTypes.Index.Plain
        };

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private int _macroIdMainPCL = _defaultMacroIdMainPCL;
        private int _macroIdRearPCL = _defaultMacroIdRearPCL;

        private ToolCommonData.PrintLang _crntPDL;

        private ToolFormSamplePCL.MacroMethod _indxMethodPCL;
        private ToolFormSamplePCLX.StreamMethod _indxMethodPCLXL;

        private int _ctPDLs;
        private int _ctOrientations;
        private int _ctPaperSizes;
        private int _ctPaperTypes;
        private int _ctPlexModes;

        private int _indxPDL;
        private int _indxOrientationPCL;
        private int _indxOrientationPCLXL;
        private int _indxOrientRearPCL;
        private int _indxOrientRearPCLXL;
        private int _indxPaperSizePCL;
        private int _indxPaperSizePCLXL;
        private int _indxPaperTypePCL;
        private int _indxPaperTypePCLXL;
        private int _indxPlexModePCL;
        private int _indxPlexModePCLXL;

        private int _testPageCountPCL;
        private int _testPageCountPCLXL;

        private bool _flagMacroRemovePCL;
        private bool _flagMacroRemovePCLXL;

        private bool _flagMainFormPCL;
        private bool _flagMainFormPCLXL;

        private bool _flagRearFormPCL;
        private bool _flagRearFormPCLXL;

        private bool _flagRearBPlatePCL;
        private bool _flagRearBPlatePCLXL;

        private bool _flagMainEncapsulatedPCL;
        private bool _flagMainEncapsulatedPCLXL;

        private bool _flagRearEncapsulatedPCL;
        private bool _flagRearEncapsulatedPCLXL;

        private bool _flagPrintDescTextPCL;
        private bool _flagPrintDescTextPCLXL;

        private bool _flagMainOnPrnDiskPCL;
        private bool _flagRearOnPrnDiskPCL;

        private bool _flagGSPushPopPCLXL;

        private bool _initialised;

        private string _formNameMainPCLXL;
        private string _formNameRearPCLXL;

        private string _formFileMainPCL;
        private string _formFileMainPCLXL;

        private string _formFileRearPCL;
        private string _formFileRearPCLXL;

        private string _prnDiskFileMainPCL;
        private string _prnDiskFileRearPCL;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T o o l F o r m S a m p l e                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolFormSample(ref ToolCommonData.PrintLang crntPDL)
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

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            bool OK;

            _indxPDL = cbPDL.SelectedIndex;
            _crntPDL = (ToolCommonData.PrintLang)_subsetPDLs[_indxPDL];

            //----------------------------------------------------------------//
            //                                                                //
            // Generate test print file.                                      //
            //                                                                //
            //----------------------------------------------------------------//

            try
            {
                BinaryWriter binWriter = null;

                //------------------------------------------------------------//

                if (_crntPDL == ToolCommonData.PrintLang.PCL)
                {
                    OK = ValidatePCLSelectionCombo();

                    if (OK)
                    {
                        bool rearBPlate;

                        string formFileMain,
                               formFileRear;

                        TargetCore.RequestStreamOpen(
                            ref binWriter,
                            ToolCommonData.ToolIds.FormSample,
                            ToolCommonData.ToolSubIds.None,
                            _crntPDL);

                        if (_flagRearFormPCL)
                            rearBPlate = _flagRearBPlatePCL;
                        else
                            rearBPlate = false;

                        if (_flagMainOnPrnDiskPCL)
                            formFileMain = _prnDiskFileMainPCL;
                        else
                            formFileMain = _formFileMainPCL;

                        if (_flagRearOnPrnDiskPCL)
                            formFileRear = _prnDiskFileRearPCL;
                        else
                            formFileRear = _formFileRearPCL;

                        ToolFormSamplePCL.GenerateJob(
                            binWriter,
                            _subsetPaperSizes[_indxPaperSizePCL],
                            _subsetPaperTypes[_indxPaperTypePCL],
                            _subsetOrientations[_indxOrientationPCL],
                            _subsetOrientations[_indxOrientRearPCL],
                            _subsetPlexModes[_indxPlexModePCL],
                            _testPageCountPCL,
                            _flagMainEncapsulatedPCL,
                            _flagRearEncapsulatedPCL,
                            _flagMacroRemovePCL,
                            _flagMainFormPCL,
                            _flagRearFormPCL,
                            _flagMainOnPrnDiskPCL,
                            _flagRearOnPrnDiskPCL,
                            rearBPlate,
                            _flagPrintDescTextPCL,
                            formFileMain,
                            formFileRear,
                            _indxMethodPCL,
                            _macroIdMainPCL,
                            _macroIdRearPCL);

                        TargetCore.RequestStreamWrite(false);
                    }
                }
                else    // if (_crntPDL == ToolCommonData.ePrintLang.PCLXL)
                {
                    OK = ValidatePCLXLSelectionCombo();

                    if (OK)
                    {
                        TargetCore.RequestStreamOpen(
                            ref binWriter,
                            ToolCommonData.ToolIds.FormSample,
                            ToolCommonData.ToolSubIds.None,
                            _crntPDL);

                        ToolFormSamplePCLX.GenerateJob(
                                binWriter,
                                _subsetPaperSizes[_indxPaperSizePCLXL],
                                _subsetPaperTypes[_indxPaperTypePCLXL],
                                _subsetOrientations[_indxOrientationPCLXL],
                                _subsetOrientations[_indxOrientRearPCLXL],
                                _subsetPlexModes[_indxPlexModePCLXL],
                                _testPageCountPCLXL,
                                _flagMainEncapsulatedPCLXL,
                                _flagRearEncapsulatedPCLXL,
                                _flagMacroRemovePCLXL,
                                _flagMainFormPCLXL,
                                _flagRearFormPCLXL,
                                _flagRearBPlatePCLXL,
                                _flagGSPushPopPCLXL,
                                _flagPrintDescTextPCLXL,
                                _formFileMainPCLXL,
                                _formFileRearPCLXL,
                                _indxMethodPCLXL,
                                _formNameMainPCLXL,
                                _formNameRearPCLXL);

                        TargetCore.RequestStreamWrite(false);
                    }
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"SocketException:\n\nMessage: {ex.Message}\n\nErrorCode: {ex.ErrorCode}\n\nSocketErrorCode: {ex.SocketErrorCode}",
                                "Generate Test Data",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception:\n\n" + ex.Message,
                                "Generate Test Data",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n P C L F o r m F i l e M a i n B r o w s e _ C l i c k        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Browse' button is clicked for a PCL main form.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnPCLFormFileMainBrowse_Click(object sender, RoutedEventArgs e)
        {
            bool selected;

            string filename = _formFileMainPCL;

            selected = SelectPCLFormFile(ref filename);

            if (selected)
            {
                _formFileMainPCL = filename;
                txtPCLFormFileMain.Text = _formFileMainPCL;

                CheckPCLFormFile(true, _formFileMainPCL);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n P C L F o r m F i l e R e a r B r o w s e _ C l i c k        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Browse' button is clicked for a PCL rear form.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnPCLFormFileRearBrowse_Click(object sender, RoutedEventArgs e)
        {
            bool selected;

            string filename = _formFileRearPCL;

            selected = SelectPCLFormFile(ref filename);

            if (selected)
            {
                _formFileRearPCL = filename;
                txtPCLFormFileRear.Text = _formFileRearPCL;

                CheckPCLFormFile(false, _formFileRearPCL);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n P C L X L F o r m F i l e M a i n B r o w s e _ C l i c k    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Browse' button is clicked for a PCLXL main form.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnPCLXLFormFileMainBrowse_Click(object sender, RoutedEventArgs e)
        {
            bool selected;

            string filename = _formFileMainPCLXL;

            selected = SelectPCLXLFormFile(ref filename);

            if (selected)
            {
                _formFileMainPCLXL = filename;
                txtPCLXLFormFileMain.Text = _formFileMainPCLXL;

                CheckPCLXLFormFile(true, _formFileMainPCLXL);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n P C L X L F o r m F i l e R e a r B r o w s e _ C l i c k    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Browse' button is clicked for a PCLXL rear form.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnPCLXLFormFileRearBrowse_Click(object sender, RoutedEventArgs e)
        {
            bool selected;

            string filename = _formFileRearPCLXL;

            selected = SelectPCLXLFormFile(ref filename);

            if (selected)
            {
                _formFileRearPCLXL = filename;
                txtPCLXLFormFileRear.Text = _formFileRearPCLXL;

                CheckPCLXLFormFile(false, _formFileRearPCLXL);
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

        private void cbOrientation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initialised && cbOrientation.HasItems)
            {
                if (_crntPDL == ToolCommonData.PrintLang.PCL)
                    _indxOrientationPCL = cbOrientation.SelectedIndex;
                else
                    _indxOrientationPCLXL = cbOrientation.SelectedIndex;
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

        private void cbPlexMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initialised && cbPlexMode.HasItems)
            {
                int plexMode = cbPlexMode.SelectedIndex;

                if (_crntPDL == ToolCommonData.PrintLang.PCL)
                    _indxPlexModePCL = plexMode;
                else
                    _indxPlexModePCLXL = plexMode;

                PdlOptionsRestore();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b R e a r O r i e n t a t i o n _ S e l e c t i o n C h a n g e d//
        //--------------------------------------------------------------------//
        //                                                                    //
        // Rear face Orientation item has changed.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbRearOrientation_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (_initialised && cbRearOrientation.HasItems)
            {
                if (_crntPDL == ToolCommonData.PrintLang.PCL)
                    _indxOrientRearPCL = cbRearOrientation.SelectedIndex;
                else
                    _indxOrientRearPCLXL = cbRearOrientation.SelectedIndex;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k P C L F o r m F i l e                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check whether or not PCL (download) form starts with 'macro id'    //
        // sequence.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void CheckPCLFormFile(bool main, string filename)
        {
            bool idPresent;

            int macroId = 0;

            if (!File.Exists(filename))
                idPresent = false;
            else
                idPresent = PCLDownloadMacro.CheckMacroFile(filename, ref macroId);

            if (idPresent)
            {
                if (main)
                {
                    _flagMainEncapsulatedPCL = true;
                    _macroIdMainPCL = macroId;

                    txtPCLMacroIdMain.Text = macroId.ToString();
                    txtPCLMacroIdMain.IsEnabled = false;
                    lbPCLMacroIdMainComment.Content = "macro identifier defined in download file";
                }
                else
                {
                    _flagRearEncapsulatedPCL = true;
                    _macroIdRearPCL = macroId;

                    txtPCLMacroIdRear.Text = macroId.ToString();
                    txtPCLMacroIdRear.IsEnabled = false;
                    lbPCLMacroIdRearComment.Content = "macro identifier defined in download file";
                }
            }
            else
            {
                if (main)
                {
                    _flagMainEncapsulatedPCL = false;
                    txtPCLMacroIdMain.IsEnabled = true;
                    lbPCLMacroIdMainComment.Content = string.Empty;
                }
                else
                {
                    _flagRearEncapsulatedPCL = false;
                    txtPCLMacroIdRear.IsEnabled = true;
                    lbPCLMacroIdRearComment.Content = string.Empty;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k P C L X L F o r m F i l e                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check whether or not PCLXL (download) form starts with BeginStream //
        // operator sequence.                                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void CheckPCLXLFormFile(bool main, string filename)
        {
            bool idPresent;

            string streamName = string.Empty;

            if (!File.Exists(filename))
                idPresent = false;
            else
                idPresent = PCLXLDownloadStream.CheckStreamFile(filename, ref streamName);

            if (idPresent)
            {
                if (main)
                {
                    _flagMainEncapsulatedPCLXL = true;
                    _formNameMainPCLXL = streamName;

                    txtPCLXLFormNameMain.Text = streamName;
                    txtPCLXLFormNameMain.IsEnabled = false;
                    lbPCLXLFormNameMainComment.Content = "form (stream) name defined in download file";
                }
                else
                {
                    _flagRearEncapsulatedPCLXL = true;
                    _formNameRearPCLXL = streamName;

                    txtPCLXLFormNameRear.Text = streamName;
                    txtPCLXLFormNameRear.IsEnabled = false;
                    lbPCLXLFormNameRearComment.Content = "form (stream) name defined in download file";
                }
            }
            else
            {
                if (main)
                {
                    _flagMainEncapsulatedPCLXL = false;
                    txtPCLXLFormNameMain.IsEnabled = true;
                    lbPCLXLFormNameMainComment.Content = string.Empty;
                }
                else
                {
                    _flagRearEncapsulatedPCLXL = false;
                    txtPCLXLFormNameRear.IsEnabled = true;
                    lbPCLXLFormNameRearComment.Content = string.Empty;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L F o r m R e m o v e _ C h e c k e d                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL option 'Remove form at end of job' checked.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLFormRemove_Checked(object sender, RoutedEventArgs e)
        {
            _flagMacroRemovePCL = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L F o r m R e m o v e _ U n c h e c k e d                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL option 'Remove form at end of job' unchecked.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLFormRemove_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagMacroRemovePCL = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L P r i n t D e s c T e x t _ C h e c k e d              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL option 'Print descriptive text' checked.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLPrintDescText_Checked(object sender, RoutedEventArgs e)
        {
            _flagPrintDescTextPCL = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L P r i n t D e s c T e x t _ U n c h e c k e d          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL option 'Print descriptive text' unchecked.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLPrintDescText_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPrintDescTextPCL = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L R e a r B P l a t e _ C h e c k e d                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL option 'Rear face boilerplate' checked.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLRearBplate_Checked(object sender, RoutedEventArgs e)
        {
            _flagRearBPlatePCL = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L R e a r B P l a t e _ U n c h e c k e d                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL option 'Rear face boilerplate' unchecked.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLRearBPlate_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagRearBPlatePCL = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L F o r m R e m o v e _ C h e c k e d                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL Option 'Remove form at end of job' checked.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLFormRemove_Checked(object sender, RoutedEventArgs e)
        {
            _flagMacroRemovePCLXL = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L F o r m R e m o v e _ U n c h e c k e d            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL Option 'Remove form at end of job' unchecked.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLFormRemove_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagMacroRemovePCLXL = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L G S P u s h P o p _ C h e c k e d                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL option 'Restore graphics state after execution' checked.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLGSPushPop_Checked(object sender, RoutedEventArgs e)
        {
            _flagGSPushPopPCLXL = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L G S P u s h P o p _ U n c h e c k e d              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL option 'Restore graphics state after execution' unchecked.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLGSPushPop_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagGSPushPopPCLXL = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L P r i n t D e s c T e x t _ C h e c k e d          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL option 'Print descriptive text' checked.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLPrintDescText_Checked(object sender, RoutedEventArgs e)
        {
            _flagPrintDescTextPCLXL = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L P r i n t D e s c T e x t _ U n c h e c k e d      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL option 'Print descriptive text' unchecked.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLPrintDescText_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPrintDescTextPCLXL = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L R e a r B P l a t e _ C h e c k e d                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL option 'Rear face boilerplate' checked.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLRearBplate_Checked(object sender, RoutedEventArgs e)
        {
            _flagRearBPlatePCLXL = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L R e a r B P l a t e _ U n c h e c k e d            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL option 'Rear face boilerplate' unchecked.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLRearBPlate_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagRearBPlatePCLXL = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g i v e C r n t P D L                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolCommonData.PrintLang GetCurrentPDL() => _crntPDL;

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
                    typeof(ToolCommonData.PrintLang), i));
            }

            //----------------------------------------------------------------//

            cbOrientation.Items.Clear();
            cbRearOrientation.Items.Clear();

            _ctOrientations = _subsetOrientations.Length;

            for (int i = 0; i < _ctOrientations; i++)
            {
                index = _subsetOrientations[i];

                string name = PCLOrientations.GetName(index);

                cbOrientation.Items.Add(name);
                cbRearOrientation.Items.Add(name);
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

            ResetTarget();

            //----------------------------------------------------------------//
            //                                                                //
            // Reinstate settings from persistent storage.                    //
            //                                                                //
            //----------------------------------------------------------------//

            MetricsLoad();

            PdlOptionsRestore();

            cbPDL.SelectedIndex = (byte)_indxPDL;

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
            int indxMethodTemp = 0;

            ToolFormSamplePersist.LoadDataCommon(out _indxPDL);

            ToolFormSamplePersist.LoadDataGeneral("PCL",
                                                   out _indxPaperTypePCL,
                                                   out _indxPaperSizePCL,
                                                   out _indxOrientationPCL,
                                                   out _indxPlexModePCL,
                                                   out _indxOrientRearPCL,
                                                   out indxMethodTemp,
                                                   out _testPageCountPCL,
                                                   out _flagMacroRemovePCL,
                                                   out _flagMainFormPCL,
                                                   out _flagRearFormPCL,
                                                   out _flagRearBPlatePCL,
                                                   out _flagPrintDescTextPCL);

            if (indxMethodTemp < (int)ToolFormSamplePCL.MacroMethod.Max)
                _indxMethodPCL = (ToolFormSamplePCL.MacroMethod)indxMethodTemp;
            else
                _indxMethodPCL = ToolFormSamplePCL.MacroMethod.CallBegin;

            ToolFormSamplePersist.LoadDataGeneral("PCLXL",
                                                   out _indxPaperTypePCLXL,
                                                   out _indxPaperSizePCLXL,
                                                   out _indxOrientationPCLXL,
                                                   out _indxPlexModePCLXL,
                                                   out _indxOrientRearPCLXL,
                                                   out indxMethodTemp,
                                                   out _testPageCountPCLXL,
                                                   out _flagMacroRemovePCLXL,
                                                   out _flagMainFormPCLXL,
                                                   out _flagRearFormPCLXL,
                                                   out _flagRearBPlatePCLXL,
                                                   out _flagPrintDescTextPCLXL);

            if (indxMethodTemp < (int)ToolFormSamplePCLX.StreamMethod.Max)
                _indxMethodPCLXL = (ToolFormSamplePCLX.StreamMethod)indxMethodTemp;
            else
                _indxMethodPCLXL = ToolFormSamplePCLX.StreamMethod.ExecuteBegin;

            ToolFormSamplePersist.LoadDataPCL(out _flagMainOnPrnDiskPCL,
                                               out _flagRearOnPrnDiskPCL,
                                               out _formFileMainPCL,
                                               out _formFileRearPCL,
                                               out _prnDiskFileMainPCL,
                                               out _prnDiskFileRearPCL,
                                               out _macroIdMainPCL,
                                               out _macroIdRearPCL);

            ToolFormSamplePersist.LoadDataPCLXL(out _formFileMainPCLXL,
                                                 out _formFileRearPCLXL,
                                                 out _formNameMainPCLXL,
                                                 out _formNameRearPCLXL,
                                                 out _flagGSPushPopPCLXL);

            //----------------------------------------------------------------//

            if ((_indxPDL < 0) || (_indxPDL >= _ctPDLs))
                _indxPDL = 0;

            _crntPDL = (ToolCommonData.PrintLang)_subsetPDLs[_indxPDL];

            //----------------------------------------------------------------//

            if ((_indxOrientationPCL < 0) || (_indxOrientationPCL >= _ctOrientations))
                _indxOrientationPCL = 0;

            if ((_indxOrientRearPCL < 0) || (_indxOrientRearPCL >= _ctOrientations))
                _indxOrientRearPCL = 0;

            if ((_indxPaperSizePCL < 0) || (_indxPaperSizePCL >= _ctPaperSizes))
                _indxPaperSizePCL = 0;

            if ((_indxPaperTypePCL < 0) || (_indxPaperTypePCL >= _ctPaperTypes))
                _indxPaperTypePCL = 0;

            if ((_indxPlexModePCL < 0) || (_indxPlexModePCL >= _ctPlexModes))
                _indxPlexModePCL = 0;

            //----------------------------------------------------------------//

            if ((_indxOrientationPCLXL < 0) || (_indxOrientationPCLXL >= _ctOrientations))
                _indxOrientationPCLXL = 0;

            if ((_indxOrientRearPCLXL < 0) || (_indxOrientRearPCLXL >= _ctOrientations))
                _indxOrientRearPCLXL = 0;

            if ((_indxPaperSizePCLXL < 0) || (_indxPaperSizePCLXL >= _ctPaperSizes))
                _indxPaperSizePCLXL = 0;

            if ((_indxPaperTypePCLXL < 0) || (_indxPaperTypePCLXL >= _ctPaperTypes))
                _indxPaperTypePCLXL = 0;

            if ((_indxPlexModePCLXL < 0) || (_indxPlexModePCLXL >= _ctPlexModes))
                _indxPlexModePCLXL = 0;
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

            ToolFormSamplePersist.SaveDataCommon(_indxPDL);

            ToolFormSamplePersist.SaveDataGeneral(
                "PCL",
                _indxPaperTypePCL,
                _indxPaperSizePCL,
                _indxOrientationPCL,
                _indxPlexModePCL,
                _indxOrientRearPCL,
                (int)_indxMethodPCL,
                _testPageCountPCL,
                _flagMacroRemovePCL,
                _flagMainFormPCL,
                _flagRearFormPCL,
                _flagRearBPlatePCL,
                _flagPrintDescTextPCL);

            ToolFormSamplePersist.SaveDataGeneral(
                "PCLXL",
                _indxPaperTypePCLXL,
                _indxPaperSizePCLXL,
                _indxOrientationPCLXL,
                _indxPlexModePCLXL,
                _indxOrientRearPCLXL,
                (int)_indxMethodPCLXL,
                _testPageCountPCLXL,
                _flagMacroRemovePCLXL,
                _flagMainFormPCLXL,
                _flagRearFormPCLXL,
                _flagRearBPlatePCLXL,
                _flagPrintDescTextPCLXL);

            ToolFormSamplePersist.SaveDataPCL(_flagMainOnPrnDiskPCL,
                                               _flagRearOnPrnDiskPCL,
                                               _formFileMainPCL,
                                               _formFileRearPCL,
                                               _prnDiskFileMainPCL,
                                               _prnDiskFileRearPCL,
                                               _macroIdMainPCL,
                                               _macroIdRearPCL);

            ToolFormSamplePersist.SaveDataPCLXL(_formFileMainPCLXL,
                                                _formFileRearPCLXL,
                                                _formNameMainPCLXL,
                                                _formNameRearPCLXL,
                                                _flagGSPushPopPCLXL);
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
                cbPlexMode.SelectedIndex = _indxPlexModePCL;
                cbRearOrientation.SelectedIndex = _indxOrientRearPCL;

                //----------------------------------------------------------------//

                if (PCLPlexModes.GetPlexType(_indxPlexModePCL) == PCLPlexModes.PlexType.Simplex)
                {
                    cbRearOrientation.Visibility = Visibility.Hidden;
                    lbRearOrientation.Visibility = Visibility.Hidden;
                    grpPCLRearFormData.Visibility = Visibility.Hidden;
                    rbPCLOptFormBoth.Visibility = Visibility.Hidden;
                    rbPCLOptFormRear.Visibility = Visibility.Hidden;
                    _flagRearFormPCL = false;
                }
                else
                {
                    cbRearOrientation.Visibility = Visibility.Visible;
                    lbRearOrientation.Visibility = Visibility.Visible;
                    grpPCLRearFormData.Visibility = Visibility.Visible;
                    rbPCLOptFormBoth.Visibility = Visibility.Visible;
                    rbPCLOptFormRear.Visibility = Visibility.Visible;
                }

                //----------------------------------------------------------------//

                tabPCL.IsEnabled = true;
                tabPCLXL.IsEnabled = false;
                tabPCL.IsSelected = true;

                if (_flagMainOnPrnDiskPCL)
                {
                    rbPCLMacroSrcMainPrnDisk.IsChecked = true;

                    txtPCLFormFileMain.Text = _prnDiskFileMainPCL;
                    btnPCLFormFileMainBrowse.Visibility = Visibility.Hidden;

                    lbPCLMacroIdMainComment.Content = string.Empty;
                }
                else
                {
                    rbPCLMacroSrcMainHost.IsChecked = true;

                    txtPCLFormFileMain.Text = _formFileMainPCL;
                    btnPCLFormFileMainBrowse.Visibility = Visibility.Visible;
                }

                if (_flagRearOnPrnDiskPCL)
                {
                    rbPCLMacroSrcRearPrnDisk.IsChecked = true;

                    txtPCLFormFileRear.Text = _prnDiskFileRearPCL;
                    btnPCLFormFileRearBrowse.Visibility = Visibility.Hidden;

                    lbPCLMacroIdRearComment.Content = string.Empty;
                }
                else
                {
                    rbPCLMacroSrcRearHost.IsChecked = true;

                    txtPCLFormFileRear.Text = _formFileRearPCL;
                    btnPCLFormFileRearBrowse.Visibility = Visibility.Visible;
                }

                txtPCLMacroIdMain.Text = _macroIdMainPCL.ToString();
                txtPCLMacroIdRear.Text = _macroIdRearPCL.ToString();

                txtPCLTestPageCount.Text = _testPageCountPCL.ToString();

                chkPCLFormRemove.IsChecked = _flagMacroRemovePCL;
                chkPCLRearBPlate.IsChecked = _flagRearBPlatePCL;

                if (_flagRearFormPCL)
                {
                    if (_flagMainFormPCL)
                        rbPCLOptFormBoth.IsChecked = true;
                    else
                        rbPCLOptFormRear.IsChecked = true;

                    rbPCLMethodOverlay.Visibility = Visibility.Hidden;

                    if (_indxMethodPCL ==
                        ToolFormSamplePCL.MacroMethod.Overlay)
                    {
                        _indxMethodPCL =
                            ToolFormSamplePCL.MacroMethod.CallBegin;

                        rbPCLMethodCallBegin.IsChecked = true;
                    }
                }
                else
                {
                    rbPCLOptFormMain.IsChecked = true;
                    rbPCLMethodOverlay.Visibility = Visibility.Visible;
                }

                if (_flagMainFormPCL)
                    grpPCLMainFormData.Visibility = Visibility.Visible;
                else
                    grpPCLMainFormData.Visibility = Visibility.Hidden;

                if (_flagRearFormPCL)
                {
                    chkPCLRearBPlate.Visibility = Visibility.Visible;
                    grpPCLRearFormData.Visibility = Visibility.Visible;
                }
                else
                {
                    chkPCLRearBPlate.Visibility = Visibility.Hidden;
                    grpPCLRearFormData.Visibility = Visibility.Hidden;
                }

                if (_indxMethodPCL == ToolFormSamplePCL.MacroMethod.CallEnd)
                    rbPCLMethodCallEnd.IsChecked = true;
                else if (_indxMethodPCL == ToolFormSamplePCL.MacroMethod.ExecuteBegin)
                    rbPCLMethodExecuteBegin.IsChecked = true;
                else if (_indxMethodPCL == ToolFormSamplePCL.MacroMethod.ExecuteEnd)
                    rbPCLMethodExecuteEnd.IsChecked = true;
                else if (_indxMethodPCL == ToolFormSamplePCL.MacroMethod.Overlay)
                    rbPCLMethodOverlay.IsChecked = true;
                else
                    rbPCLMethodCallBegin.IsChecked = true;

                if (_flagMainFormPCL && (!_flagMainOnPrnDiskPCL) && File.Exists(_formFileMainPCL))
                    CheckPCLFormFile(true, _formFileMainPCL);

                if (_flagRearFormPCL && (!_flagRearOnPrnDiskPCL) && File.Exists(_formFileRearPCL))
                    CheckPCLFormFile(false, _formFileRearPCL);
            }
            else
            {
                cbOrientation.SelectedIndex = _indxOrientationPCLXL;
                cbPaperSize.SelectedIndex = _indxPaperSizePCLXL;
                cbPaperType.SelectedIndex = _indxPaperTypePCLXL;
                cbPlexMode.SelectedIndex = _indxPlexModePCLXL;
                cbRearOrientation.SelectedIndex = _indxOrientRearPCLXL;

                //----------------------------------------------------------------//

                if (PCLPlexModes.GetPlexType(_indxPlexModePCLXL) ==
                    PCLPlexModes.PlexType.Simplex)
                {
                    cbRearOrientation.Visibility = Visibility.Hidden;
                    lbRearOrientation.Visibility = Visibility.Hidden;
                    grpPCLXLRearFormData.Visibility = Visibility.Hidden;
                    rbPCLXLOptFormBoth.Visibility = Visibility.Hidden;
                    rbPCLXLOptFormRear.Visibility = Visibility.Hidden;
                    _flagRearFormPCLXL = false;
                }
                else
                {
                    cbRearOrientation.Visibility = Visibility.Visible;
                    lbRearOrientation.Visibility = Visibility.Visible;
                    grpPCLXLRearFormData.Visibility = Visibility.Visible;
                    rbPCLXLOptFormBoth.Visibility = Visibility.Visible;
                    rbPCLXLOptFormRear.Visibility = Visibility.Visible;
                }

                //----------------------------------------------------------------//

                tabPCL.IsEnabled = false;
                tabPCLXL.IsEnabled = true;
                tabPCLXL.IsSelected = true;

                txtPCLXLFormFileMain.Text = _formFileMainPCLXL;
                txtPCLXLFormFileRear.Text = _formFileRearPCLXL;

                txtPCLXLFormNameMain.Text = _formNameMainPCLXL;
                txtPCLXLFormNameRear.Text = _formNameRearPCLXL;

                txtPCLXLTestPageCount.Text = _testPageCountPCLXL.ToString();

                chkPCLXLFormRemove.IsChecked = _flagMacroRemovePCLXL;
                chkPCLXLRearBPlate.IsChecked = _flagRearBPlatePCLXL;

                chkPCLXLGSPushPop.IsChecked = _flagGSPushPopPCLXL;

                if (_flagRearFormPCLXL)
                {
                    if (_flagMainFormPCLXL)
                        rbPCLXLOptFormBoth.IsChecked = true;
                    else
                        rbPCLXLOptFormRear.IsChecked = true;
                }
                else
                {
                    rbPCLXLOptFormMain.IsChecked = true;
                }

                if (_flagMainFormPCLXL)
                    grpPCLXLMainFormData.Visibility = Visibility.Visible;
                else
                    grpPCLXLMainFormData.Visibility = Visibility.Hidden;

                if (_flagRearFormPCLXL)
                {
                    chkPCLXLRearBPlate.Visibility = Visibility.Visible;
                    grpPCLXLRearFormData.Visibility = Visibility.Visible;
                }
                else
                {
                    chkPCLXLRearBPlate.Visibility = Visibility.Hidden;
                    grpPCLXLRearFormData.Visibility = Visibility.Hidden;
                }

                if (_indxMethodPCLXL == ToolFormSamplePCLX.StreamMethod.ExecuteEnd)
                    rbPCLXLMethodExecuteEnd.IsChecked = true;
                else
                    rbPCLXLMethodExecuteBegin.IsChecked = true;

                if (_flagMainFormPCLXL && File.Exists(_formFileMainPCLXL))
                    CheckPCLXLFormFile(true, _formFileMainPCLXL);

                if (_flagRearFormPCLXL && File.Exists(_formFileRearPCLXL))
                    CheckPCLXLFormFile(false, _formFileRearPCLXL);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p d l O p t i o n s S t o r e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store the test metrics options for the current PDL and font type.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void PdlOptionsStore()
        {
            if (_crntPDL == ToolCommonData.PrintLang.PCL)
            {
                _indxOrientationPCL = cbOrientation.SelectedIndex;
                _indxPaperSizePCL = cbPaperSize.SelectedIndex;
                _indxPaperTypePCL = cbPaperType.SelectedIndex;
                _indxPlexModePCL = cbPlexMode.SelectedIndex;
                _indxOrientRearPCL = cbRearOrientation.SelectedIndex;
            }
            else
            {
                _indxOrientationPCLXL = cbOrientation.SelectedIndex;
                _indxPaperSizePCLXL = cbPaperSize.SelectedIndex;
                _indxPaperTypePCLXL = cbPaperType.SelectedIndex;
                _indxPlexModePCLXL = cbPlexMode.SelectedIndex;
                _indxOrientRearPCLXL = cbRearOrientation.SelectedIndex;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L M a c r o S r c M a i n H o s t _ C l i c k              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL main macro source = host workstation' radio   //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLMacroSrcMainHost_Click(object sender, RoutedEventArgs e)
        {
            _flagMainOnPrnDiskPCL = false;

            txtPCLFormFileMain.Text = _formFileMainPCL;
            btnPCLFormFileMainBrowse.Visibility = Visibility.Visible;

            if (File.Exists(_formFileMainPCL))
                CheckPCLFormFile(true, _formFileMainPCL);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L M a c r o S r c M a i n P r n D i s k _ C l i c k        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL main macro source = printer hard disk' radio  //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLMacroSrcMainPrnDisk_Click(object sender, RoutedEventArgs e)
        {
            _flagMainOnPrnDiskPCL = true;

            txtPCLFormFileMain.Text = _prnDiskFileMainPCL;
            btnPCLFormFileMainBrowse.Visibility = Visibility.Hidden;

            lbPCLMacroIdMainComment.Content = string.Empty;

            txtPCLMacroIdMain.IsEnabled = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L M a c r o S r c R e a r H o s t _ C l i c k              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL rear macro source = host workstation' radio   //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLMacroSrcRearHost_Click(object sender, RoutedEventArgs e)
        {
            _flagRearOnPrnDiskPCL = false;

            txtPCLFormFileRear.Text = _formFileRearPCL;
            btnPCLFormFileRearBrowse.Visibility = Visibility.Visible;

            if (File.Exists(_formFileRearPCL))
                CheckPCLFormFile(false, _formFileRearPCL);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L M a c r o S r c R e a r P r n D i s k _ C l i c k        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL rear macro source = printer hard disk' radio  //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLMacroSrcRearPrnDisk_Click(object sender, RoutedEventArgs e)
        {
            _flagRearOnPrnDiskPCL = true;

            txtPCLFormFileRear.Text = _prnDiskFileRearPCL;
            btnPCLFormFileRearBrowse.Visibility = Visibility.Hidden;

            lbPCLMacroIdRearComment.Content = string.Empty;

            txtPCLMacroIdRear.IsEnabled = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L M e t h o d C a l l B e g i n _ C l i c k                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL method - Call Begin Page' radio button        //
        // is selected.                                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLMethodCallBegin_Click(object sender, RoutedEventArgs e)
        {
            _indxMethodPCL = ToolFormSamplePCL.MacroMethod.CallBegin;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L M e t h o d C a l l E n d _ C l i c k                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL method - Call End Page' radio button          //
        // is selected.                                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLMethodCallEnd_Click(object sender, RoutedEventArgs e)
        {
            _indxMethodPCL = ToolFormSamplePCL.MacroMethod.CallEnd;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L M e t h o d E x e c u t e B e g i n _ C l i c k          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL method - Execute Begin Page' radio button     //
        // is selected.                                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLMethodExecuteBegin_Click(object sender, RoutedEventArgs e)
        {
            _indxMethodPCL = ToolFormSamplePCL.MacroMethod.ExecuteBegin;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L M e t h o d E x e c u t e E n d _ C l i c k              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL method - Execute End Page' radio button       //
        // is selected.                                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLMethodExecuteEnd_Click(object sender, RoutedEventArgs e)
        {
            _indxMethodPCL = ToolFormSamplePCL.MacroMethod.ExecuteEnd;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L M e t h o d O v e r l a y _ C l i c k                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL method - Overlay' radio button is selected.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLMethodOverlay_Click(object sender, RoutedEventArgs e)
        {
            _indxMethodPCL = ToolFormSamplePCL.MacroMethod.Overlay;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L O p t F o r m B o t h _ C l i c k                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL options - Fornt and rear forms' radio button  //
        // is selected.                                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLOptFormBoth_Click(object sender, RoutedEventArgs e)
        {
            _flagMainFormPCL = true;
            _flagRearFormPCL = true;

            chkPCLRearBPlate.Visibility = Visibility.Visible;
            grpPCLMainFormData.Visibility = Visibility.Visible;
            grpPCLRearFormData.Visibility = Visibility.Visible;
            rbPCLMethodOverlay.Visibility = Visibility.Hidden;

            if (_indxMethodPCL == ToolFormSamplePCL.MacroMethod.Overlay)
            {
                _indxMethodPCL = ToolFormSamplePCL.MacroMethod.CallBegin;

                rbPCLMethodCallBegin.IsChecked = true;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L O p t F o r m M a i n _ C l i c k                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL options - Main (front) form only' radio       //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLOptFormMain_Click(object sender, RoutedEventArgs e)
        {
            _flagMainFormPCL = true;
            _flagRearFormPCL = false;

            chkPCLRearBPlate.Visibility = Visibility.Hidden;
            grpPCLMainFormData.Visibility = Visibility.Visible;
            grpPCLRearFormData.Visibility = Visibility.Hidden;
            rbPCLMethodOverlay.Visibility = Visibility.Visible;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L O p t F o r m R e a r _ C l i c k                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCL options - Rear form only' radio button is     //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLOptFormRear_Click(object sender, RoutedEventArgs e)
        {
            _flagMainFormPCL = false;
            _flagRearFormPCL = true;

            chkPCLRearBPlate.Visibility = Visibility.Visible;
            grpPCLMainFormData.Visibility = Visibility.Hidden;
            grpPCLRearFormData.Visibility = Visibility.Visible;
            rbPCLMethodOverlay.Visibility = Visibility.Hidden;

            if (_indxMethodPCL == ToolFormSamplePCL.MacroMethod.Overlay)
            {
                _indxMethodPCL = ToolFormSamplePCL.MacroMethod.CallBegin;

                rbPCLMethodCallBegin.IsChecked = true;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L X L M e t h o d E x e c u t e B e g i n _ C l i c k      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCLXL method - Execute Begin Page' radio button   //
        // is selected.                                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLXLMethodExecuteBegin_Click(object sender, RoutedEventArgs e)
        {
            _indxMethodPCLXL = ToolFormSamplePCLX.StreamMethod.ExecuteBegin;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L X L M e t h o d E x e c u t e E n d _ C l i c k          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCLXL method - Execute End Page' radio button     //
        // is selected.                                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLXLMethodExecuteEnd_Click(object sender, RoutedEventArgs e)
        {
            _indxMethodPCLXL = ToolFormSamplePCLX.StreamMethod.ExecuteEnd;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L X L O p t F o r m B o t h _ C l i c k                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCLXL options - Front and rear forms' radio       //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLXLOptFormBoth_Click(object sender, RoutedEventArgs e)
        {
            _flagMainFormPCLXL = true;
            _flagRearFormPCLXL = true;

            chkPCLXLRearBPlate.Visibility = Visibility.Visible;
            grpPCLXLMainFormData.Visibility = Visibility.Visible;
            grpPCLXLRearFormData.Visibility = Visibility.Visible;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L X L O p t F o r m M a i n _ C l i c k                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCLXL options - Main (front) form only' radio     //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLXLOptFormMain_Click(object sender, RoutedEventArgs e)
        {
            _flagMainFormPCLXL = true;
            _flagRearFormPCLXL = false;

            chkPCLXLRearBPlate.Visibility = Visibility.Hidden;
            grpPCLXLMainFormData.Visibility = Visibility.Visible;
            grpPCLXLRearFormData.Visibility = Visibility.Hidden;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b P C L X L O p t F o r m R e a r _ C l i c k                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'PCLXL options - Rear form only' radio button is   //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbPCLXLOptFormRear_Click(object sender, RoutedEventArgs e)
        {
            _flagMainFormPCLXL = false;
            _flagRearFormPCLXL = true;

            chkPCLXLRearBPlate.Visibility = Visibility.Visible;
            grpPCLXLMainFormData.Visibility = Visibility.Hidden;
            grpPCLXLRearFormData.Visibility = Visibility.Visible;
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
            var targetType = TargetCore.GetTargetType();

            if (targetType == TargetCore.Target.File)
            {
                btnGenerate.Content = "Generate & send test data to file";
            }
            else if (targetType == TargetCore.Target.NetPrinter)
            {
                TargetCore.MetricsLoadNetPrinter(out string netPrnAddress,
                                                  out int netPrnPort,
                                                  out _,
                                                  out _);

                btnGenerate.Content = $"Generate & send test data to\n{netPrnAddress} : {netPrnPort}";
            }
            else if (targetType == TargetCore.Target.WinPrinter)
            {
                TargetCore.MetricsLoadWinPrinter(out string winPrintername);

                btnGenerate.Content = $"Generate & send test data to printer\n{winPrintername}";
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e l e c t P C L F o r m F i l e                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initiate 'open file' dialogue for PCL form file.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool SelectPCLFormFile(ref string selectedName)
        {
            var openDialog = ToolCommonFunctions.CreateOpenFileDialog(selectedName);

            openDialog.Filter = "PCL Overlay Files|*.ovl|All Files|*.*";

            if (openDialog.ShowDialog() == false)
                return false;

            selectedName = openDialog.FileName;

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e l e c t P C L X L F o r m F i l e                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initiate 'open file' dialogue for PCLXL form file.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool SelectPCLXLFormFile(ref string selectedName)
        {
            var openDialog = ToolCommonFunctions.CreateOpenFileDialog(selectedName);

            openDialog.Filter = "PCLXL Overlay Files|*.ovx|All Files|*.*";

            if (openDialog.ShowDialog() == false)
                return false;

            selectedName = openDialog.FileName;

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L F o r m F i l e M a i n _ L o s t F o c u s            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL (download) main form filename item has lost focus.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLFormFileMain_LostFocus(object sender, RoutedEventArgs e)
        {
            string fileName = txtPCLFormFileMain.Text;

            if (_flagMainOnPrnDiskPCL)
            {
                _prnDiskFileMainPCL = fileName;
            }
            else
            {
                bool selected = true;

                _formFileMainPCL = fileName;

                if (!File.Exists(fileName))
                {
                    MessageBox.Show($"Main form file '{fileName}' does not exist.\n\nPlease select an appropriate file.",
                                     "PCL Form File Invalid",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);

                    selected = SelectPCLFormFile(ref fileName);

                    if (selected)
                    {
                        _formFileMainPCL = fileName;
                        txtPCLFormFileMain.Text = _formFileMainPCL;
                    }
                }

                if (selected)
                {
                    CheckPCLFormFile(true, _formFileMainPCL);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L F o r m F i l e R e a r _ L o s t F o c u s            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL (download) rear form filename item has lost focus.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLFormFileRear_LostFocus(object sender, RoutedEventArgs e)
        {
            string fileName = txtPCLFormFileRear.Text;

            if (_flagRearOnPrnDiskPCL)
            {
                _prnDiskFileRearPCL = fileName;
            }
            else
            {
                bool selected = true;

                _formFileRearPCL = fileName;

                if (!File.Exists(fileName))
                {
                    MessageBox.Show($"Rear form file '{fileName}' does not exist.\n\nPlease select an appropriate file.",
                                     "PCL Form File Invalid",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);

                    selected = SelectPCLFormFile(ref fileName);

                    if (selected)
                    {
                        _formFileRearPCL = fileName;
                        txtPCLFormFileRear.Text = _formFileRearPCL;
                    }
                }

                if (selected)
                {
                    CheckPCLFormFile(false, _formFileRearPCL);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L M a c r o I d M a i n _ L o s t F o c u s              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL download ID for main form has lost focus.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLMacroIdMain_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_initialised)
                ValidatePCLMacroId(false, true);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L M a c r o I d M a i n _ T e x t C h a n g e d          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL download Id for main form has changed.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLMacroIdMain_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_initialised)
                ValidatePCLMacroId(false, false);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L M a c r o I d R e a r _ L o s t F o c u s              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL download ID for rear form has lost focus.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLMacroIdRear_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_initialised)
                ValidatePCLMacroId(true, true);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L M a c r o I d R e a r _ T e x t C h a n g e d          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL download Id for rear form has changed.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLMacroIdRear_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_initialised)
                ValidatePCLMacroId(true, false);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L T e s t P a g e C o u n t _ L o s t F o c u s          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL Test Page Count item has lost focus.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLTestPageCount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_initialised)
                ValidatePCLTestPageCount(true);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L T e s t P a g e C o u n t _ T e x t C h a n g e d      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCL Test Page Count item has changed.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLTestPageCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_initialised)
                ValidatePCLTestPageCount(false);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L X L F o r m F i l e M a i n _ L o s t F o c u s        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL (download) main form filename item has lost focus.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLXLFormFileMain_LostFocus(object sender, RoutedEventArgs e)
        {
            bool selected = true;

            string fileName = txtPCLXLFormFileMain.Text;

            _formFileMainPCLXL = fileName;

            if (!File.Exists(fileName))
            {
                MessageBox.Show($"Main form file '{fileName}' does not exist.\n\nPlease select an appropriate file.",
                                 "PCLXL Form File Invalid",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);

                selected = SelectPCLXLFormFile(ref fileName);

                if (selected)
                {
                    _formFileMainPCLXL = fileName;
                    txtPCLXLFormFileMain.Text = _formFileMainPCLXL;
                }
            }

            if (selected)
            {
                CheckPCLXLFormFile(true, _formFileMainPCLXL);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L X L F o r m F i l e R e a r _ L o s t F o c u s        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL (download) rear form filename item has lost focus.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLXLFormFileRear_LostFocus(object sender, RoutedEventArgs e)
        {
            bool selected = true;

            string fileName = txtPCLXLFormFileRear.Text;

            _formFileRearPCLXL = fileName;

            if (!File.Exists(fileName))
            {
                MessageBox.Show($"Rear form file '{fileName}' does not existr\n\nPlease select an appropriate file.",
                                 "PCLXL Form File Invalid",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);

                selected = SelectPCLXLFormFile(ref fileName);

                if (selected)
                {
                    _formFileRearPCLXL = fileName;
                    txtPCLXLFormFileRear.Text = _formFileRearPCLXL;
                }
            }

            if (selected)
            {
                CheckPCLXLFormFile(false, _formFileRearPCLXL);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L X L F o r m N a m e M a i n _ L o s t F o c u s        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL (download) main form name item has lost focus.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLXLFormNameMain_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ValidatePCLXLFormName(false, true))
                _formNameMainPCLXL = txtPCLXLFormNameMain.Text;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L X L F o r m N a m e M a i n _ T e x t C h a n g e d    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL download Id for main form has changed.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLXLFormNameMain_TextChanged(object sender, TextChangedEventArgs e)
        {
            _formNameMainPCLXL = txtPCLXLFormNameMain.Text;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L X L F o r m N a m e R e a r _ L o s t F o c u s        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL (download) main form name item has lost focus.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLXLFormNameRear_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ValidatePCLXLFormName(true, true))
                _formNameRearPCLXL = txtPCLXLFormNameRear.Text;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L X L F o r m N a m e R e a r _ T e x t C h a n g e d    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL download Id for rear form has changed.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLXLFormNameRear_TextChanged(object sender, TextChangedEventArgs e)
        {
            _formNameRearPCLXL = txtPCLXLFormNameRear.Text;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L X L T e s t P a g e C o u n t _ L o s t F o c u s      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL Test Page Count item has lost focus.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLXLTestPageCount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_initialised)
                ValidatePCLXLTestPageCount(true);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P C L X L T e s t P a g e C o u n t _ T e x t C h a n g e d  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // PCLXL Test Page Count item has changed.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPCLXLTestPageCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_initialised)
                ValidatePCLXLTestPageCount(false);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L M a c r o I d                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCL macro download Id value.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLMacroId(bool rearForm, bool lostFocusEvent)
        {
            const ushort minVal = 0;
            const ushort maxVal = 32767;
            ushort defVal;
            string crntText;
            string side;

            if (rearForm)
            {
                side = "rear";
                defVal = _defaultMacroIdRearPCL;
                crntText = txtPCLMacroIdRear.Text;
            }
            else
            {
                side = "main";
                defVal = _defaultMacroIdMainPCL;
                crntText = txtPCLMacroIdMain.Text;
            }

            bool OK = ushort.TryParse(crntText, out ushort value);
            if (OK)
            {
                if ((value < minVal) || (value > maxVal))
                    OK = false;
            }

            if (OK)
            {
                if (rearForm)
                    _macroIdRearPCL = value;
                else
                    _macroIdMainPCL = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString();

                    MessageBox.Show($"{side} macro Id value '{crntText}' is invalid.\n\nValue will be reset to default '{newText}'",
                                    "PCL Macro Identifier Invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    if (rearForm)
                    {
                        _macroIdRearPCL = defVal;
                        txtPCLMacroIdRear.Text = newText;
                    }
                    else
                    {
                        _macroIdMainPCL = defVal;
                        txtPCLMacroIdMain.Text = newText;
                    }
                }
                else
                {
                    MessageBox.Show($"{side} macro Id value '{crntText}' is invalid.\n\nValid range is:\n\t{minVal} <= value <= {maxVal}\nor\n\t<null> to represent <not applicable>.",
                                    "PCL Macro Identifier Invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    if (rearForm)
                    {
                        txtPCLMacroIdRear.Focus();
                        txtPCLMacroIdRear.SelectAll();
                    }
                    else
                    {
                        txtPCLMacroIdMain.Focus();
                        txtPCLMacroIdMain.SelectAll();
                    }
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L S e l e c t i o n C o m b o                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCL selection combination.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLSelectionCombo()
        {
            //----------------------------------------------------------------//

            if (_flagMainFormPCL && !_flagMainOnPrnDiskPCL && !File.Exists(_formFileMainPCL))
            {
                MessageBox.Show($"Form file '{_formFileMainPCL}' does not exist or is inaccesible.",
                                 "PCL Main Form",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);

                txtPCLFormFileMain.Focus();
                txtPCLFormFileMain.SelectAll();

                return false;
            }

            //----------------------------------------------------------------//

            if (_flagRearFormPCL && !_flagRearOnPrnDiskPCL && !File.Exists(_formFileRearPCL))
            {
                MessageBox.Show($"Form file '{_formFileRearPCL}' does not exist or is inaccesible.",
                                 "PCL Main Form",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);

                txtPCLFormFileRear.Focus();
                txtPCLFormFileRear.SelectAll();

                return false;
            }

            //----------------------------------------------------------------//

            if (_flagMainFormPCL && _flagRearFormPCL && _macroIdMainPCL == _macroIdRearPCL)
            {
                MessageBox.Show($"Macro identifiers '{_macroIdMainPCL}' for Main and Rear forms are both the same.",
                                 "PCL Selection Combination",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);

                if (_flagMainEncapsulatedPCL)
                {
                    txtPCLMacroIdRear.Focus();
                    txtPCLMacroIdRear.SelectAll();
                }
                else
                {
                    txtPCLMacroIdMain.Focus();
                    txtPCLMacroIdMain.SelectAll();
                }

                return false;
            }

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L T e s t P a g e C o u n t                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCL test page count value.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLTestPageCount(bool lostFocusEvent)
        {
            const ushort minVal = 1;
            const ushort maxVal = 20;
            const ushort defVal = _defaultPageCount;
            string crntText = txtPCLTestPageCount.Text;

            bool OK = ushort.TryParse(crntText, out ushort value);
            if (OK)
            {
                if ((value < minVal) || (value > maxVal))
                    OK = false;
            }

            if (OK)
            {
                _testPageCountPCL = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString();

                    MessageBox.Show($"Test page count value '{crntText}' is invalid.\r\nValue will be reset to default '{newText}'.",
                                    "PCL Test Page Count Invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    _testPageCountPCL = defVal;

                    txtPCLTestPageCount.Text = newText;
                }
                else
                {
                    MessageBox.Show($"Test page count value '{crntText}' is invalid.\nValid range is :\n\n\t{minVal} <= value <= {maxVal}.",
                                    "PCL Test Page Count Invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtPCLTestPageCount.Focus();
                    txtPCLTestPageCount.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L X L F o r m N a m e                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCLXL form name value.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLXLFormName(bool rearForm, bool lostFocusEvent)
        {
            //          const Int32 minLen = 14;
            const int maxLen = 20;

            string defVal;
            string crntText;
            string side;
            bool OK = true;

            if (rearForm)
            {
                side = "rear";
                defVal = _defaultFormNameRootPCLXL + "Rear";
                crntText = txtPCLXLFormNameRear.Text;
            }
            else
            {
                side = "main";
                defVal = _defaultFormNameRootPCLXL + "Main";
                crntText = txtPCLXLFormNameMain.Text;
            }

            int len = crntText.Length;
            if (crntText?.Length == 0)
            {
                OK = false;
            }
            else if (len > maxLen)
            {
                OK = false;
            }

            if (!OK)
            {
                if (lostFocusEvent)
                {
                    MessageBox.Show($"{side} form name value '{crntText}' is invalid.\r\nValue will be reset to default '{defVal}'.",
                                    "PCLXL Form Name Invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    if (rearForm)
                    {
                        _formNameRearPCLXL = defVal;
                        txtPCLXLFormNameRear.Text = _formNameRearPCLXL;
                    }
                    else
                    {
                        _formNameMainPCLXL = defVal;
                        txtPCLXLFormNameMain.Text = _formNameMainPCLXL;
                    }
                }
                else
                {
                    MessageBox.Show($"{side} form name value '{crntText}' is invalid.\r\nValid length is <= {maxLen}.",
                                    "PCLXL Form Name Invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    if (rearForm)
                    {
                        txtPCLXLFormNameRear.Text = crntText.Substring(0, maxLen);
                        txtPCLXLFormNameRear.Focus();
                        txtPCLXLFormNameRear.SelectAll();
                    }
                    else
                    {
                        txtPCLXLFormNameMain.Text = crntText.Substring(0, maxLen);
                        txtPCLXLFormNameMain.Focus();
                        txtPCLXLFormNameMain.SelectAll();
                    }
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L X L S e l e c t i o n C o m b o              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCLXL selection combination.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLXLSelectionCombo()
        {
            if (_flagMainFormPCLXL && !File.Exists(_formFileMainPCLXL))
            {
                MessageBox.Show($"Form file '{_formFileMainPCLXL}' does not exist or is inaccesible.",
                                 "PCLXL Main Form",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);

                txtPCLXLFormFileMain.Focus();
                txtPCLXLFormFileMain.SelectAll();

                return false;
            }

            if (_flagRearFormPCLXL && !File.Exists(_formFileRearPCLXL))
            {
                MessageBox.Show($"Form file '{_formFileRearPCLXL} does not exist or is inaccesible.",
                                 "PCLXL Rear Form",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);

                txtPCLXLFormFileRear.Focus();
                txtPCLXLFormFileRear.SelectAll();

                return false;
            }

            if (_flagMainFormPCLXL && _flagRearFormPCLXL && _formNameMainPCLXL == _formNameRearPCLXL)
            {
                MessageBox.Show($"Form names '{_formNameMainPCLXL}' for Main and Rear forms are both the same.",
                                 "PCLXL Selection Combination",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);

                if (_flagMainEncapsulatedPCLXL)
                {
                    txtPCLXLFormNameRear.Focus();
                    txtPCLXLFormNameRear.SelectAll();
                }
                else
                {
                    txtPCLXLFormNameMain.Focus();
                    txtPCLXLFormNameMain.SelectAll();
                }

                return false;
            }

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P C L X L T e s t P a g e C o u n t                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PCLXL test page count value.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePCLXLTestPageCount(bool lostFocusEvent)
        {
            const ushort minVal = 1;
            const ushort maxVal = 20;
            const ushort defVal = _defaultPageCount;
            string crntText = txtPCLXLTestPageCount.Text;

            bool OK = ushort.TryParse(crntText, out ushort value);
            if (OK)
            {
                if ((value < minVal) || (value > maxVal))
                    OK = false;
            }

            if (OK)
            {
                _testPageCountPCLXL = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString();

                    MessageBox.Show($"Test page count value '{crntText}' is invalid.\r\nValue will be reset to default '{newText}'.",
                                    "PCLXL Test Page Count Invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    _testPageCountPCLXL = defVal;

                    txtPCLXLTestPageCount.Text = newText;
                }
                else
                {
                    MessageBox.Show($"Test page count value '{crntText}' is invalid.\r\nValid range is :\r\n\t{minVal} <= value <= {maxVal}.",
                                    "PCLXL Test Page Count Invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtPCLXLTestPageCount.Focus();
                    txtPCLXLTestPageCount.SelectAll();
                }
            }

            return OK;
        }
    }
}