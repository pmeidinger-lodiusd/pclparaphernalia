using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace PCLParaphernalia;

/// <summary>
/// Interaction logic for ToolPatternGenerate.xaml
/// 
/// Class handles the pattern Generate tool form.
/// 
/// © Chris Hutchinson 2017
/// 
/// </summary>

[System.Reflection.Obfuscation(Feature = "renaming",
                                        ApplyToMembers = true)]

public partial class ToolPatternGenerate : Window
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

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
        (int) PCLPaperSizes.eIndex.ANSI_A_Letter
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
    //   private Int32 _ctRasterResolutions;

    private int _indxPDL;
    private int _indxOrientationPCL;
    private int _indxOrientationPCLXL;
    private int _indxPaperSizePCL;
    private int _indxPaperSizePCLXL;
    private int _indxPaperTypePCL;
    private int _indxPaperTypePCLXL;
    //   private Int32 _indxRasterResolutionPCL;

    //   private Int32 _destScalePercentX;
    //   private Int32 _destScalePercentY;

    //   private Single _destPosX;
    //   private Single _destPosY;

    private string _bitmapFilename;

    private bool _initialised;

    //   private static Boolean stateTglBtn_01f = false;
    //   private static Boolean stateBtn_01f = false;
    private static bool[] _patternStateBtns;
    private static UniformGrid _uGridPattern;

    //--------------------------------------------------------------------//
    //                                              C o n s t r u c t o r //
    // T o o l P a t t e r n G e n e r a t e                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    public ToolPatternGenerate(ref ToolCommonData.ePrintLang crntPDL)
    {
        InitializeComponent();

        initialise();

        crntPDL = _crntPDL;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // b t n F i l e n a m e B r o w s e _ C l i c k                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the 'Browse' button is clicked.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void btnFilenameBrowse_Click(object sender, RoutedEventArgs e)
    {
        bool selected;

        string filename = _bitmapFilename;

        selected = selectImageFile(ref filename);

        if (selected)
        {
            _bitmapFilename = filename;
            txtFilename.Text = _bitmapFilename;
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // b t n G e n e r a t e _ C l i c k                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the 'Generate' button is clicked.                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void btnGenerate_Click(object sender, RoutedEventArgs e)
    {
        /*
        Int32 result = 0;

        Boolean bitmapOpen = false;

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
        _crntPDL = (ToolCommonData.ePrintLang) _subsetPDLs[_indxPDL];

        pdlOptionsStore();

        //----------------------------------------------------------------//
        //                                                                //
        // Generate test print file.                                      //
        //                                                                //
        //----------------------------------------------------------------//

        bitmapOpen = ToolImageBitmapCore.bitmapOpen(_bitmapFilename);

        if (!bitmapOpen)
        {
            result = -1;
        }

        if (result == 0)
        {
            result = ToolImageBitmapCore.readBmpFileHeader();
        }

        if (result == 0)
        {
            result = ToolImageBitmapCore.readBmpInfoHeader();
        }

        if (result == 0)
        {
            result = ToolImageBitmapCore.readBmpPalette();
        }

        if (result == 0)
        {
            BinaryWriter binWriter = null;

            TargetCore.requestStreamOpen (
                ref binWriter,
                ToolCommonData.eToolIds.ImageBitmap,
                ToolCommonData.eToolSubIds.None,
                _crntPDL);

            if (result == 0)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Write test data to output file.                        //
                //                                                        //
                //--------------------------------------------------------//

                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                {
                    ToolImageBitmapPCL.generateJob(
                        binWriter,
                        _subsetPaperSizes[_indxPaperSizePCL],
                        _subsetPaperTypes[_indxPaperTypePCL],
                        _subsetOrientations[_indxOrientationPCL],
                        _destPosX,
                        _destPosY,
                        _destScalePercentX,
                        _destScalePercentY,
                        _indxRasterResolutionPCL);
                }
                else    // if (_crntPDL == (Int32)ToolCommonData.ePrintLang.PCLXL)
                {
                    ToolImageBitmapPCLXL.generateJob(
                        binWriter,
                        _subsetPaperSizes[_indxPaperSizePCLXL],
                        _subsetPaperTypes[_indxPaperTypePCLXL],
                        _subsetOrientations[_indxOrientationPCLXL],
                        _destPosX,
                        _destPosY,
                        _destScalePercentX,
                        _destScalePercentY);
                }
            }

            if (result == 0)
            {
                TargetCore.requestStreamWrite(false);
            }
        }

        if (bitmapOpen)
        {
            ToolImageBitmapCore.bitmapClose();
        }
        */
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // b t n G e t P r o p e r t i e s _ C l i c k                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the 'Get Properties' button is clicked.                //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void btnGetProperties_Click(object sender, RoutedEventArgs e)
    {
        /*
        Int32 result = 0;

        Int32 srcWidth = 0,
              srcHeight = 0,
              srcResX = 0,
              srcResY = 0;

        UInt32 srcCompression = 0,
               srcPaletteEntries = 0;

        UInt16 srcBitsPerPixel = 0;

        Boolean srcBlackWhite = false;

        Boolean bitmapOpen = false;

        bitmapOpen = ToolImageBitmapCore.bitmapOpen(_bitmapFilename);

        if (!bitmapOpen)
        {
            result = -1;
        }

        if (result == 0)
        {
            result = ToolImageBitmapCore.readBmpFileHeader();
        }

        if (result == 0)
        {
            result = ToolImageBitmapCore.readBmpInfoHeader();
        }

        if (result == 0)
        {
            result = ToolImageBitmapCore.readBmpPalette();
        }

        ToolImageBitmapCore.getBmpInfo(ref srcWidth,
                                       ref srcHeight,
                                       ref srcBitsPerPixel,
                                       ref srcCompression,
                                       ref srcResX,
                                       ref srcResY,
                                       ref srcPaletteEntries,
                                       ref srcBlackWhite);

        txtSrcWidth.Text        = srcWidth.ToString();
        txtSrcHeight.Text       = srcHeight.ToString();
        txtSrcResX.Text         = srcResX.ToString();
        txtSrcResY.Text         = srcResY.ToString();
        txtSrcResXDpi.Text      = ((Int32)(srcResX/39.37)).ToString();
        txtSrcResYDpi.Text      = ((Int32)(srcResY/39.37)).ToString();
        txtSrcBPP.Text          = srcBitsPerPixel.ToString();
        txtSrcCompression.Text  = srcCompression.ToString();
        txtSrcMonoBW.Text       = srcBlackWhite.ToString();

        grpProps.Visibility = Visibility.Visible;
        
        if (bitmapOpen)
        {
            ToolImageBitmapCore.bitmapClose();
        }
        */
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c b D o n o r P a t t e r n _ S e l e c t i o n C h a n g e d      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Donor pattern item has changed.                                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void cbDonorPattern_SelectionChanged(object sender,
                                                 SelectionChangedEventArgs e)
    {
        if (_initialised && cbDonorPattern.HasItems)
        {
            // to be added 
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
            pdlOptionsStore();

            _indxPDL = cbPDL.SelectedIndex;
            _crntPDL = (ToolCommonData.ePrintLang)_subsetPDLs[_indxPDL];

            pdlOptionsRestore();

            /*
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                lbResolution.Visibility = Visibility.Visible;
                cbResolution.Visibility = Visibility.Visible;
                lbPCLNote.Visibility = Visibility.Visible;
            }
            else
            {
                lbResolution.Visibility = Visibility.Hidden;
                cbResolution.Visibility = Visibility.Hidden;
                lbPCLNote.Visibility = Visibility.Hidden;
            }
            */
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c b R e s o l u t i o n _ S e l e c t i o n C h a n g e d          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Raster Resolution item has changed.                                //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void cbResolution_SelectionChanged(object sender,
                                               SelectionChangedEventArgs e)
    {
        /*
        if (_initialised && cbResolution.HasItems)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _indxRasterResolutionPCL =
                    (UInt16)cbResolution.SelectedIndex;
        }
        */
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g i v e C r n t P D L                                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    public void giveCrntPDL(ref ToolCommonData.ePrintLang crntPDL)
    {
        crntPDL = _crntPDL;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i n i t i a l i s e                                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Initialise 'target' data.                                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void initialise()
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

        /*
        cbResolution.Items.Clear();
        /*
        _ctRasterResolutions = PCLRasterResolutions.getCount();

        for (Int32 i = 0; i < _ctRasterResolutions; i++)
        {
            cbResolution.Items.Add(
                PCLRasterResolutions.getValue(i).ToString());
        }
        */
        //----------------------------------------------------------------//

        resetTarget();

        //----------------------------------------------------------------//
        //                                                                //
        // Reinstate settings from persistent storage.                    //
        //                                                                //
        //----------------------------------------------------------------//
        metricsLoad();

        /*
        grpProps.Visibility = Visibility.Hidden;

        txtDestPosX.Text = _destPosX.ToString("F2");
        txtDestPosY.Text = _destPosY.ToString("F2");

        txtDestScaleX.Text = _destScalePercentX.ToString();
        txtDestScaleY.Text = _destScalePercentY.ToString();

        txtFilename.Text = _bitmapFilename;
        */

        pdlOptionsRestore();

        cbPDL.SelectedIndex = (byte)_indxPDL;

        _uGridPattern = new UniformGrid();

        gridPattern.Children.Add(_uGridPattern);

        _uGridPattern.Rows = 16;
        _uGridPattern.Columns = 16;

        int btnCt = _uGridPattern.Rows * _uGridPattern.Columns;

        _patternStateBtns = new bool[btnCt];

        //   btn_04.Style = (Style)FindResource("PrimaryButton");
        //   btn_04.Style = (Style)("{StaticResource PrimaryButton}");
        //   btn_04.Style = PrimaryButton;

        for (int i = 0; i < btnCt; i++)
        {
            ToggleButton x = new ToggleButton();

            x.Background = Brushes.White;
            x.IsChecked = false;
            x.Style = (Style)FindResource("ToggleButton");

            _uGridPattern.Children.Add(x);

            _patternStateBtns[i] = false;
        }

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

    private void metricsLoad()
    {
        /*
        Int32 tempPosX = 100,
              tempPosY = 100;
              
        Int32 tempRasterRes = 0;

        ToolImageBitmapPersist.loadDataCommon(ref _indxPDL,
                                              ref _bitmapFilename,
                                              ref tempPosX,
                                              ref tempPosY,
                                              ref _destScalePercentX,
                                              ref _destScalePercentY,
                                              ref tempRasterRes);

        if ((_indxPDL < 0) || (_indxPDL >= _ctPDLs))
            _indxPDL = 0;

        _crntPDL = (ToolCommonData.ePrintLang) _subsetPDLs[_indxPDL];

        _destPosX = (Single) (tempPosX / 100);
        _destPosY = (Single) (tempPosY / 100);

        if ((tempRasterRes < 0) ||
            (tempRasterRes >= _ctRasterResolutions))
            _indxRasterResolutionPCL = 0;
        else
            _indxRasterResolutionPCL = tempRasterRes;

        ToolImageBitmapPersist.loadDataPCL("PCL",
                                           ref _indxOrientationPCL,
                                           ref _indxPaperSizePCL,
                                           ref _indxPaperTypePCL);

        if ((_indxOrientationPCL < 0) ||
            (_indxOrientationPCL >= _ctOrientations))
            _indxOrientationPCL = 0;

        if ((_indxPaperSizePCL < 0) ||
            (_indxPaperSizePCL >= _ctPaperSizes))
            _indxPaperSizePCL = 0;

        if ((_indxPaperTypePCL < 0) ||
            (_indxPaperTypePCL >= _ctPaperTypes))
            _indxPaperTypePCL = 0;

        ToolImageBitmapPersist.loadDataPCL("PCLXL",
                                           ref _indxOrientationPCLXL,
                                           ref _indxPaperSizePCLXL,
                                           ref _indxPaperTypePCLXL);

        if ((_indxOrientationPCLXL < 0) ||
            (_indxOrientationPCLXL >= _ctOrientations))
            _indxOrientationPCLXL = 0;

        if ((_indxPaperSizePCLXL  < 0) ||
            (_indxPaperSizePCLXL >= _ctPaperSizes))
            _indxPaperSizePCLXL = 0;

        if ((_indxPaperTypePCLXL < 0) ||
            (_indxPaperTypePCLXL >= _ctPaperTypes))
            _indxPaperTypePCLXL = 0;
        */
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m e t r i c s S a v e                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Save current metrics to persistent storage.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public void metricsSave()
    {
        /*
        ToolImageBitmapPersist.saveDataCommon(_indxPDL,
                                              _bitmapFilename,
                                              (Int32)(_destPosX * 100),
                                              (Int32)(_destPosY * 100),
                                              _destScalePercentX,
                                              _destScalePercentY,
                                              _indxRasterResolutionPCL);

        ToolImageBitmapPersist.saveDataPCL("PCL",
                                           _indxOrientationPCL,
                                           _indxPaperSizePCL,
                                           _indxPaperTypePCL);

        ToolImageBitmapPersist.saveDataPCL("PCLXL",
                                           _indxOrientationPCLXL,
                                           _indxPaperSizePCLXL,
                                           _indxPaperTypePCLXL);
        */
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p d l O p t i o n s R e s t o r e                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Restore the test metrics options for the current PDL.              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void pdlOptionsRestore()
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

    private void pdlOptionsStore()
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

    public void resetTarget()
    {
        TargetCore.eTarget targetType = TargetCore.GetType();

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
    // s e l e c t I m a g e F i l e                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Initiate 'open file' dialogue.                                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    private bool selectImageFile(ref string selectedName)
    {
        bool selected = false;
        /*

        String folderName = null;
        String fileName = null;

        OpenFileDialog openDialog = new OpenFileDialog();

        ToolCommonFunctions.splitPathName (selectedName,
                                           ref folderName,
                                           ref fileName);

        openDialog.InitialDirectory = folderName;
        openDialog.FileName = fileName;
        openDialog.CheckFileExists = true;

        openDialog.Filter = "Bitmap Files|*.bmp; *.BMP";
        Nullable<Boolean> dialogResult = openDialog.ShowDialog();

        if (dialogResult == true)
        {
            selected = true;
            selectedName = openDialog.FileName;
        }
        else
        {
            selected = false;
        }
        */

        return selected;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t F i l e n a m e _ L o s t F o c u s                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Image filename text has lost focus.                                //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtFilename_LostFocus(object sender,
                                       RoutedEventArgs e)
    {
        if (_initialised)
        {
            _bitmapFilename = txtFilename.Text;

            //   grpProps.Visibility = Visibility.Hidden;
        }
    }

    //--------------------------------------------------------------------//

    private void btn_CheckPattern_Click(object sender, RoutedEventArgs e)
    {
        int btnCt = _uGridPattern.Children.Count;

        for (int i = 0; i < btnCt; i++)
        {
            ToggleButton x = (ToggleButton)_uGridPattern.Children[i];

            x.IsThreeState = false;

            _patternStateBtns[i] = x.IsChecked == true;
        }

        StringBuilder setBtns = new StringBuilder();

        for (int i = 0; i < btnCt; i++)
        {

            if (_patternStateBtns[i] == true)
            {
                setBtns.Append(" " + i);
            }
        }

        MessageBox.Show(setBtns.ToString(),
                        "Set buttons",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

    }
}
