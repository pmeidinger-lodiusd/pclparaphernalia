﻿using Microsoft.Win32;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PCLParaphernalia
{
    /// <summary>
    /// Interaction logic for ToolPrnAnalyse.xaml
    /// 
    /// Class handles the Prn Analyse tool form.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    [Obfuscation(Feature = "renaming",
                                            ApplyToMembers = true)]

    public partial class ToolPrnAnalyse : Window
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum eInfoType : byte
        {
            Content,
            Analysis,
            Statistics
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        //  private BackgroundWorker _bkWk;        // But how to return DataGrid ??

        private PrnParseOptions _options;

        private PropertyInfo[] _stdClrsPropertyInfo;

        private int _ctClrMapStdClrs;

        private string _prnFilename;

        private long _fileSize;

        private int[] _indxClrMapBack;
        private int[] _indxClrMapFore;

        private bool _flagClrMapUseClr;

        //  private Boolean _initialised;

        private bool _redoAnalysis;
        private bool _redoContent;
        private bool _redoStatistics;

        private DataTable _tableAnalysis;
        private DataTable _tableContent;
        private DataTable _tableStatistics;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T o o l P r n A n a l y s e                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolPrnAnalyse(ref ToolCommonData.ePrintLang crntPDL)
        {
            InitializeComponent();

            initialise();

            crntPDL = ToolCommonData.ePrintLang.Unknown;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n A n a l y s i s _ C l i c k                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Analyse' button is clicked.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnAnalysis_Click(object sender, RoutedEventArgs e)
        {
            //    progBar1.Value = 0;

            if (_redoAnalysis)
            {
                /*
                //------------------------------------------------------------//
                //                                                            //
                // If we ever support the 'doWork' mechanism:                 //
                //                                                            //
                //------------------------------------------------------------//
                 
                this._bkWk.RunWorkerAsync (); // Then move following code to DoWork  
                */

                PrnParse parseFile = new PrnParse(PrnParse.eParseType.Analyse,
                                                   0);

                _tableAnalysis.Clear();
                _tableStatistics.Clear();

                initialiseGridAnalysis();

                parseFile.Analyse(_prnFilename,
                                   _options,
                                   _tableAnalysis);

                _redoAnalysis = false;
            }

            tabCtrl.SelectedItem = tabAnalysis;

            /*
            //----------------------------------------------------------------//
            //                                                                //
            // If we ever support the 'doWork' mechanism:                     //
            //      statusBar updates                                         //
            //                                                                //
            //----------------------------------------------------------------//
        
            statusBar.Items[2] = dgAnalysis.Items.Count;
            txtRptSizeAnalysis.Text = dgAnalysis.Items.Count.ToString();
            */

            txtRptSizeAnalysis.Text = _tableAnalysis.Rows.Count.ToString();

            btnSaveReport.Content = "Save Analysis Report ...";
            btnSaveReport.IsEnabled = true;
            btnStatistics.IsEnabled = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n C o n t e n t _ C l i c k                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Show Contents' button is clicked.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnContent_Click(object sender, RoutedEventArgs e)
        {
            if (_redoContent)
            {
                PrnView viewFile = new PrnView();

                _tableContent.Clear();

                initialiseGridContent();

                viewFile.viewFile(_prnFilename,
                                   _options,
                                   _tableContent);

                _redoContent = false;
            }

            tabCtrl.SelectedItem = tabContent;

            /*
            //----------------------------------------------------------------//
            //                                                                //
            // If we ever support the 'doWork' mechanism:                     //
            //      statusBar updates                                         //
            //                                                                //
            //----------------------------------------------------------------//

            statusBar.Items[2] = dgContent.Items.Count;
            txtRptSizeContent.Text = dgContent.Items.Count.ToString ();
            */

            txtRptSizeContent.Text = _tableContent.Rows.Count.ToString();

            btnSaveReport.Content = "Save Content Report ...";
            btnSaveReport.IsEnabled = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n O p e n F i l e _ C l i c k                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'open File ...' button is clicked.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            bool selected;
            string filename = _prnFilename;

            selected = prnFileSelect(ref filename);

            if (selected)
            {
                /*
                //------------------------------------------------------------//
                //                                                            //
                // If we ever support the 'doWork' mechanism:                 //
                //      statusBar updates                                     //
                //                                                            //
                //------------------------------------------------------------//
            
                statusBar.Items[0] = _prnFilename;
                statusBar.Items[1] = _fileSize;
                statusBar.Items[2] = 0;
                */

                _prnFilename = filename;

                _redoAnalysis = true;
                _redoContent = true;
                _redoStatistics = true;

                prnFileProcess(filename);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n O p t i o n s _ C l i c k                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Options ...' button is clicked.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            ToolPrnAnalyseOptDlg optDialog =
                new ToolPrnAnalyseOptDlg(_options, _fileSize);

            bool? dialogResult = optDialog.ShowDialog();

            if (dialogResult == true)
            {
                _redoAnalysis = true;     // TODO : be more discerning
                _redoContent = true;     // TODO : be more discerning
                _redoStatistics = true;     // TODO : be more discerning

                btnSaveReport.IsEnabled = false;

                ResetStatistics();

                _options.GetOptClrMap(ref _flagClrMapUseClr,
                                       ref _indxClrMapBack,
                                       ref _indxClrMapFore);

                _options.MetricsSave();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n S a v e R e p o r t _ C l i c k                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Save Report' button is clicked.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnSaveReport_Click(object sender, RoutedEventArgs e)
        {
            bool flagOffsetHex,
                    flagOptRptWrap = false;

            ReportCore.eRptFileFmt rptFileFmt = ReportCore.eRptFileFmt.NA;
            ReportCore.eRptChkMarks rptChkMarks = ReportCore.eRptChkMarks.NA;

            if (_options.IndxGenOffsetFormat ==
                PrnParseConstants.eOptOffsetFormats.Decimal)
                flagOffsetHex = false;
            else
                flagOffsetHex = true;

            TargetCore.MetricsReturnFileRpt(ToolCommonData.eToolIds.PrnAnalyse,
                                             ref rptFileFmt,
                                             ref rptChkMarks,
                                             ref flagOptRptWrap);

            if (tabCtrl.SelectedItem == tabStatistics)
            {
                ToolPrnAnalyseReport.generate(eInfoType.Statistics,
                                               rptFileFmt,
                                               _tableStatistics,
                                               _prnFilename,
                                               _fileSize,
                                               flagOffsetHex,
                                               _options);
            }
            else if (tabCtrl.SelectedItem == tabContent)
            {
                ToolPrnAnalyseReport.generate(eInfoType.Content,
                                               rptFileFmt,
                                               _tableContent,
                                               _prnFilename,
                                               _fileSize,
                                               flagOffsetHex,
                                               _options);
            }
            else
            {
                ToolPrnAnalyseReport.generate(eInfoType.Analysis,
                                               rptFileFmt,
                                               _tableAnalysis,
                                               _prnFilename,
                                               _fileSize,
                                               flagOffsetHex,
                                               _options);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n S t a t i s t i c s _ C l i c k                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Show Statistics' button is clicked.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            if (_redoStatistics)
            {
                PrnParseConstants.eOptStatsLevel level =
                    PrnParseConstants.eOptStatsLevel.ReferencedOnly;

                bool incUsedSeqsOnly = false;
                bool excUnusedObsPCLSeqs = false;
                bool excUnusedResPCLXLTags = false;

                _options.GetOptStats(ref level,
                                      ref excUnusedObsPCLSeqs,
                                      ref excUnusedResPCLXLTags);

                _tableStatistics.Clear();

                if (level ==
                    PrnParseConstants.eOptStatsLevel.ReferencedOnly)
                    incUsedSeqsOnly = true;
                else
                    incUsedSeqsOnly = false;

                PrescribeCommands.DisplayStatsCounts(_tableStatistics,
                                                      incUsedSeqsOnly);

                PJLCommands.DisplayStatsCounts(_tableStatistics,
                                                incUsedSeqsOnly);

                PCLControlCodes.DisplayStatsCounts(_tableStatistics,
                                                    incUsedSeqsOnly);

                PCLSimpleSeqs.DisplayStatsCounts(_tableStatistics,
                                                  incUsedSeqsOnly,
                                                  excUnusedObsPCLSeqs);

                PCLComplexSeqs.DisplayStatsCounts(_tableStatistics,
                                                  incUsedSeqsOnly,
                                                  excUnusedObsPCLSeqs);

                HPGL2Commands.DisplayStatsCounts(_tableStatistics,
                                                  incUsedSeqsOnly);

                HPGL2ControlCodes.DisplayStatsCounts(_tableStatistics,
                                                     incUsedSeqsOnly);

                PCLXLDataTypes.DisplayStatsCounts(_tableStatistics,
                                                  incUsedSeqsOnly,
                                                  excUnusedResPCLXLTags);

                PCLXLAttrEnums.DisplayStatsCounts(_tableStatistics,
                                                   incUsedSeqsOnly,
                                                   excUnusedResPCLXLTags);

                PCLXLAttributes.DisplayStatsCounts(_tableStatistics,
                                                    incUsedSeqsOnly,
                                                    excUnusedResPCLXLTags);

                PCLXLOperators.DisplayStatsCounts(_tableStatistics,
                                                   incUsedSeqsOnly,
                                                   excUnusedResPCLXLTags);

                PCLXLWhitespaces.DisplayStatsCounts(_tableStatistics,
                                                     incUsedSeqsOnly,
                                                     excUnusedResPCLXLTags);

                PMLDataTypes.DisplayStatsCounts(_tableStatistics,
                                                 incUsedSeqsOnly);

                PMLActions.DisplayStatsCounts(_tableStatistics,
                                               incUsedSeqsOnly);

                PMLOutcomes.DisplayStatsCounts(_tableStatistics,
                                                incUsedSeqsOnly);

                // TODO = remaining types
                // PCL XL Attribute Definers ??
                // PCL XL Embedded Data Definers ??

                _redoStatistics = false;
            }

            tabCtrl.SelectedItem = tabStatistics;

            /*
            //----------------------------------------------------------------//
            //                                                                //
            // If we ever support the 'doWork' mechanism:                     //
            //      statusBar updates                                         //
            //                                                                //
            //----------------------------------------------------------------//

            statusBar.Items[2] = dgStatistics.Items.Count;
            txtRptSizeStatistics.Text = dgStatistics.Items.Count.ToString ();
            */

            txtRptSizeStatistics.Text = _tableStatistics.Rows.Count.ToString();

            btnSaveReport.Content = "Save Statistics Report ...";
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d g A n a l y s i s _ A u t o G e n e r a t i n g C o l u m n      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Modifies column captions for 'Analysis' grid.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void dgAnalysis_AutoGeneratingColumn(
            object sender,
            DataGridAutoGeneratingColumnEventArgs e)
        {
            string headername = e.Column.Header.ToString();

            if (headername == PrnParseConstants.cRptA_colName_RowType)
            {
                e.Cancel = true;
            }
            else if (headername == PrnParseConstants.cRptA_colName_Offset)
            {
                if (_options.IndxGenOffsetFormat ==
                    PrnParseConstants.eOptOffsetFormats.Decimal)
                    e.Column.Header = headername + ": dec";
                else
                    e.Column.Header = headername + ": hex";
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d g A n a l y s i s _ L o a d i n g R o w                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Modifies 'Analysis' grid row when loading.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void dgAnalysis_LoadingRow(object sender,
                                            DataGridRowEventArgs e)
        {
            DataRowView rowView = (DataRowView)e.Row.Item;
            DataRow row = rowView.Row;

            bool nullRow =
                row.IsNull(PrnParseConstants.cRptA_colName_RowType);

            if (!nullRow)
            {
                if (_flagClrMapUseClr)
                {
                    int indxClrBack,
                          indxClrFore;

                    int rowType;

                    Color clrBack = new Color(),
                          clrFore = new Color();

                    SolidColorBrush brushBack = new SolidColorBrush(),
                                    brushFore = new SolidColorBrush();

                    rowType = (int)row[PrnParseConstants.cRptA_colName_RowType];

                    indxClrBack = _indxClrMapBack[rowType];
                    indxClrFore = _indxClrMapFore[rowType];

                    clrBack = (Color)_stdClrsPropertyInfo[indxClrBack].GetValue(null, null);
                    clrFore = (Color)_stdClrsPropertyInfo[indxClrFore].GetValue(null, null);

                    brushBack.Color = clrBack;
                    brushFore.Color = clrFore;

                    e.Row.Background = brushBack;
                    e.Row.Foreground = brushFore;
                }
                else
                {
                    e.Row.Background = Brushes.White;
                    e.Row.Foreground = Brushes.Black;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d g C o n t e n t _ A u t o G e n e r a t i n g C o l u m n        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Modifies column captions for 'Content' grid.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void dgContent_AutoGeneratingColumn(
            object sender,
            DataGridAutoGeneratingColumnEventArgs e)
        {
            string headername = e.Column.Header.ToString();

            if (headername == PrnParseConstants.cRptC_colName_Offset)
            {
                if (_options.IndxGenOffsetFormat ==
                    PrnParseConstants.eOptOffsetFormats.Decimal)
                    e.Column.Header = headername + ": dec";
                else
                    e.Column.Header = headername + ": hex";
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d g S t a t i s t i c s _ A u t o G e n e r a t i n g C o l u m n  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Modifies column captions for 'Statistics' grid.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void dgStatistics_AutoGeneratingColumn(
            object sender,
            DataGridAutoGeneratingColumnEventArgs e)
        {
            // none required //
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g i v e C r n t P D L                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void giveCrntPDL(ref ToolCommonData.ePrintLang crntPDL)
        {
            crntPDL = ToolCommonData.ePrintLang.Unknown;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialise . . .                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void initialise()
        {
            //    _initialised = false;

            _redoAnalysis = true;
            _redoContent = true;
            _redoStatistics = true;

            //----------------------------------------------------------------//
            //                                                                //
            // Populate form.                                                 //
            //                                                                //
            //----------------------------------------------------------------//

            btnAnalysis.IsEnabled = false;
            btnContent.IsEnabled = false;
            btnStatistics.IsEnabled = false;
            btnSaveReport.IsEnabled = false;

            //----------------------------------------------------------------//

            //     btnAnalyse.Content = "Analyse print file";

            //----------------------------------------------------------------//
            //                                                                //
            //                                                                //
            //                                                                //
            //----------------------------------------------------------------//

            _options = new PrnParseOptions();

            /*
            //----------------------------------------------------------------//
            //                                                                //
            // If we ever support the 'doWork' mechanism:                     //
            //                                                                //
            //----------------------------------------------------------------//

            _bkWk = new BackgroundWorker ();

            _bkWk.WorkerReportsProgress = true;

            _bkWk.DoWork += new DoWorkEventHandler (this.bkWk_DoWork);

            _bkWk.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler (this.bkWk_Completed);

            _bkWk.ProgressChanged +=
                new ProgressChangedEventHandler (this.bkWk_Progress); 
            */

            //----------------------------------------------------------------//
            //                                                                //
            // Reinstate settings from persistent storage.                    //
            //                                                                //
            //----------------------------------------------------------------//

            metricsLoad();

            _fileSize = -1;

            int ctRowTypes = PrnParseRowTypes.GetCount();

            _indxClrMapBack = new int[ctRowTypes];
            _indxClrMapFore = new int[ctRowTypes];

            _options.GetOptClrMap(ref _flagClrMapUseClr,
                                   ref _indxClrMapBack,
                                   ref _indxClrMapFore);

            _options.GetOptClrMapStdClrs(ref _ctClrMapStdClrs,
                                          ref _stdClrsPropertyInfo);

            /*
            //----------------------------------------------------------------//
            //                                                                //
            // If we ever support the 'doWork' mechanism:                     //
            //      statusBar updates                                         //
            //                                                                //
            //----------------------------------------------------------------//
            
            statusBar.Items[0] = string.Empty;
            statusBar.Items[1] = string.Empty;
            */

            txtFileName.Text = string.Empty;
            txtFileSize.Text = string.Empty;

            //----------------------------------------------------------------//

            initialiseGridAnalysis();
            initialiseGridContent();
            initialiseGridStatistics();

            //----------------------------------------------------------------//

            //     _initialised = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e G r i d A n a l y s i s                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialises 'Analysis' datatable and associate with grid.          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void initialiseGridAnalysis()
        {
            _tableAnalysis = new DataTable("Analysis");

            _tableAnalysis.Columns.Add(PrnParseConstants.cRptA_colName_RowType,
                                        typeof(int));
            _tableAnalysis.Columns.Add(PrnParseConstants.cRptA_colName_Offset,
                                        typeof(string));
            _tableAnalysis.Columns.Add(PrnParseConstants.cRptA_colName_Type,
                                        typeof(string));
            _tableAnalysis.Columns.Add(PrnParseConstants.cRptA_colName_Seq,
                                        typeof(string));
            _tableAnalysis.Columns.Add(PrnParseConstants.cRptA_colName_Desc,
                                        typeof(string));

            //----------------------------------------------------------------//
            //                                                                //
            // Bind grid to table.                                            //
            // Note that the AutoGeneratingColumn event function may modify   //
            // the columns.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            dgAnalysis.DataContext = _tableAnalysis;  // bind to grid
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e G r i d C o n t e n t                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialises 'Content' datatable and associate with grid.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void initialiseGridContent()
        {
            _tableContent = new DataTable("Content");

            _tableContent.Columns.Add(PrnParseConstants.cRptC_colName_Offset,
                                       typeof(string));
            _tableContent.Columns.Add(PrnParseConstants.cRptC_colName_Hex,
                                       typeof(string));
            _tableContent.Columns.Add(PrnParseConstants.cRptC_colName_Text,
                                       typeof(string));

            //----------------------------------------------------------------//
            //                                                                //
            // Bind grid to table.                                            //
            // Note that the AutoGeneratingColumn event function may modify   //
            // the columns.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            dgContent.DataContext = _tableContent;  // bind to grid
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e G r i d S t a t i s t i c s                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialises 'Statistics' datatable and associate with grid.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void initialiseGridStatistics()
        {
            _tableStatistics = new DataTable("Statistics");

            _tableStatistics.Columns.Add(PrnParseConstants.cRptS_colName_Seq,
                                          typeof(string));
            _tableStatistics.Columns.Add(PrnParseConstants.cRptS_colName_Desc,
                                          typeof(string));
            _tableStatistics.Columns.Add(PrnParseConstants.cRptS_colName_CtP,
                                          typeof(string));
            _tableStatistics.Columns.Add(PrnParseConstants.cRptS_colName_CtE,
                                          typeof(string));
            _tableStatistics.Columns.Add(PrnParseConstants.cRptS_colName_CtT,
                                          typeof(string));

            //----------------------------------------------------------------//
            //                                                                //
            // Bind grid to table.                                            //
            // Note that the AutoGeneratingColumn event function may modify   //
            // the columns.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            dgStatistics.DataContext = _tableStatistics;  // bind to grid
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
            ToolPrnAnalysePersist.LoadData(ref _prnFilename);

            _options.MetricsLoad();
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
            ToolPrnAnalysePersist.SaveData(_prnFilename);

            _options.MetricsSave();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r n F i l e G e t S i z e                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Obtain size of current PRN file.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private long prnFileGetSize(string fileName)
        {
            Stream ipStream = null;

            long fileSize = -1;

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Print file name is null.",
                                "Print file selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            else if (!File.Exists(fileName))
            {
                MessageBox.Show("Print file '" + fileName +
                                "' does not exist.",
                                "Print file selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    ipStream = File.Open(fileName,
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.None);
                }

                catch (IOException e)
                {
                    MessageBox.Show("IO Exception:\r\n" +
                                    e.Message + "\r\n" +
                                    "Opening print file '" + fileName,
                                    "Print file selection",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }

                if (ipStream != null)
                {
                    FileInfo fi = new FileInfo(fileName);

                    fileSize = fi.Length;

                    ipStream.Close();
                }
            }

            return fileSize;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r n F i l e P r o c e s s                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Process the selected print file.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void prnFileProcess(string filename)
        {
            _prnFilename = filename;

            _fileSize = prnFileGetSize(_prnFilename);

            txtFileName.Text = _prnFilename;
            txtFileSize.Text = _fileSize.ToString();

            txtRptSizeContent.Text = "0";
            txtRptSizeAnalysis.Text = "0";
            txtRptSizeStatistics.Text = "0";

            btnAnalysis.IsEnabled = true;
            btnContent.IsEnabled = true;
            btnStatistics.IsEnabled = false;
            btnSaveReport.IsEnabled = false;
            btnSaveReport.Content = "Save report ...";

            _tableAnalysis.Clear();

            ResetStatistics();

            _options.ResetOptCurF(_fileSize);

            if (_options.FlagGenMiscAutoAnalyse)
                btnAnalysis_Click(this, null);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r n F i l e S e l e c t                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initiate 'open file' dialogue for PRN file.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool prnFileSelect(ref string prnFilename)
        {
            OpenFileDialog openDialog = ToolCommonFunctions.CreateOpenFileDialog(prnFilename);

            openDialog.Filter = "Print Files|" +
                                "*.prn; *.pcl; *.dia;" +
                                "*.PRN; *.PCL; *.DIA" +
                                "|Font files|" +
                                "*.sfp; *.sfs; *.sft; *.sfx; " +
                                "*.SFP; *.SFS; *.SFT, *.SFX" +
                                "|Overlay files|" +
                                "*.ovl; *.ovx;" +
                                "*.OVL; *.OVX" +
                                "|All files|" +
                                "*.*";

            bool? dialogResult = openDialog.ShowDialog();

            if (dialogResult == true)
                prnFilename = openDialog.FileName;

            return dialogResult == true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s e t S t a t i s t i c s                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset analysis statisics counts.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void ResetStatistics()
        {
            btnStatistics.IsEnabled = false;
            txtRptSizeStatistics.Text = "0";

            _tableStatistics.Clear();

            PrescribeCommands.ResetStatsCounts();
            PJLCommands.ResetStatsCounts();
            PCLComplexSeqs.ResetStatsCounts();
            PCLSimpleSeqs.ResetStatsCounts();
            PCLControlCodes.ResetStatsCounts();
            HPGL2Commands.ResetStatsCounts();
            HPGL2ControlCodes.ResetStatsCounts();
            PCLXLDataTypes.ResetStatsCounts();
            //   PCLXLAttrDefiners.resetStatsCounts ();
            PCLXLAttributes.ResetStatsCounts();
            PCLXLAttrEnums.ResetStatsCounts();
            //   PCLXLEmbedDataDefs.resetStatsCounts ();
            PCLXLOperators.ResetStatsCounts();
            PCLXLWhitespaces.ResetStatsCounts();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s e t T a r g e t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset the target type.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void resetTarget()
        {
            // dummy method
        }
        /*
//--------------------------------------------------------------------//
//                                                        M e t h o d //
// b k W k _ C o m p l e t e d                                        //
//--------------------------------------------------------------------//
//                                                                    //
// Called when the background worker thread completes.                //
//                                                                    //
//--------------------------------------------------------------------//

private void bkWk_Completed(object sender,
         RunWorkerCompletedEventArgs e)
{
progressBar1.Value = 100;

if (e.Error != null)
{
//  dgAnalysis.BringIntoView ();
//  dgAnalysis.DataContext = _analysisTable;  // bind to grid
//  dgAnalysis.InvalidateVisual ();
//  dgAnalysis.Focus ();
}
else
{
MessageBox.Show ("Failure",
     "Background thread failed",
     MessageBoxButton.OK,
     MessageBoxImage.Error);
}
}
*/
        /*
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b k W k _ D o W o r k                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called to initiate the background worker thread.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void bkWk_DoWork(object sender,
                                 DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            PrnParseFile parseFile = new PrnParseFile ();

            PrnParseContData linkData = new PrnParseContData ();

            _analysisTable.Clear ();

            parseFile.analyse (_prnFilename,
                               linkData,
                               _options,
                               _analysisTable,
                               worker);
            
            dgAnalysis.DataContext = _analysisTable;  // bind to grid
        }
        */
        /*
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b k W k _ P r o g r e s s                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called by the background worker thread to update progress bar.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void bkWk_Progress(object sender,
                                   ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        */

    }
}
