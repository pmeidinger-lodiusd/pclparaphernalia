﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// Interaction logic for ToolPrinterInfo.xaml
    /// 
    /// Class handles the PrinterInfo tool form.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    [System.Reflection.Obfuscation(Feature = "renaming",
                                            ApplyToMembers = true)]

    public partial class ToolStatusReadback : Window
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const int _defaultPJLFSCount = 100;
        private const int _defaultPJLFSEntry = 1;
        private const int _defaultPJLFSOffset = 0;
        private const int _defaultPJLFSSize = 999999;

        private const string _defaultPJLFSPassword = "65535";

        private const string _defaultPJLFSObjPath = "0:\\pcl\\macros\\macro1";
        private const string _defaultPJLFSVolume = "0:";

        private static readonly PJLCommands.eCmdIndex[] _subsetPJLCommands =
        {
            PJLCommands.eCmdIndex.DINQUIRE,
            PJLCommands.eCmdIndex.INFO,
            PJLCommands.eCmdIndex.INQUIRE
        };

        private static readonly PJLCommands.eCmdIndex[] _subsetPJLFSCommands =
        {
            PJLCommands.eCmdIndex.FSAPPEND,
            PJLCommands.eCmdIndex.FSDELETE,
            PJLCommands.eCmdIndex.FSDIRLIST,
            PJLCommands.eCmdIndex.FSDOWNLOAD,
            PJLCommands.eCmdIndex.FSINIT,
            PJLCommands.eCmdIndex.FSMKDIR,
            PJLCommands.eCmdIndex.FSQUERY,
            PJLCommands.eCmdIndex.FSUPLOAD
        };

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private TargetCore.eTarget _targetType;

        private ToolCommonData.ePrintLang _crntPDL;

        PJLCommands.eRequestType _reqTypePJL;
        PJLCommands.eRequestType _reqTypePJLFS;
        PJLCommands.eCmdIndex _cmdIndxPJL;
        PJLCommands.eCmdIndex _cmdIndxPJLFS;

        private int _ctPCLEntityTypes;
        private int _ctPCLLocTypes;
        private int _ctPJLCategories;
        private int _ctPJLCommands;
        private int _ctPJLFSCommands;
        private int _ctPJLVariables;

        private int _indxPCLEntityType;
        private int _indxPCLLocType;
        private int _indxPJLCategory;
        private int _indxPJLCommand;
        private int _indxPJLFSCommand;
        private int _indxPJLVariable;

        private int _valPJLFSOpt1;
        private int _valPJLFSOpt2;

        private static string _reportFilenamePCL;
        private static string _reportFilenamePJL;

        private static string _customCatPJL;
        private static string _customVarPJL;

        private static string _binSrcFilenamePJLFS;
        private static string _binTgtFilenamePJLFS;
        private static string _objPathPJLFS;
        private static string _objVolPJLFS;
        private static string _objDirPJLFS;
        private static string _objFilPJLFS;
        private static string _passwordPJLFS;

        private bool _flagPJLFS;
        private bool _flagPJLFSSecJob;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T o o l P r i n t e r I n f o                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolStatusReadback(ref ToolCommonData.ePrintLang crntPDL)
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
            try
            {
                BinaryWriter requestWriter = null;

                //------------------------------------------------------------//
                //                                                            //
                // Generate request data, and write this to a file.           //
                // If Target = File, the file is the nominated file,          //
                // otherwise a temporary file is used.                        //
                //                                                            //
                //------------------------------------------------------------//

                TargetCore.RequestStreamOpen(
                    ref requestWriter,
                    ToolCommonData.eToolIds.StatusReadback,
                    ToolCommonData.eToolSubIds.None,
                    _crntPDL);

                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // PCL Status Readback.                                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    ToolStatusReadbackPCL.generateRequest(requestWriter,
                                                           _indxPCLEntityType,
                                                           _indxPCLLocType);
                }
                else if (!_flagPJLFS)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // PJL Status Readback.                                   //
                    //                                                        //
                    //--------------------------------------------------------//

                    ToolStatusReadbackPJL.generateRequest(requestWriter,
                                                           _cmdIndxPJL,
                                                           _indxPJLCategory,
                                                           _indxPJLVariable,
                                                           _customCatPJL,
                                                           _customVarPJL);
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // PJL File System commands.                              //
                    //                                                        //
                    //--------------------------------------------------------//

                    string path;

                    if (_reqTypePJLFS == PJLCommands.eRequestType.FSInit)
                        path = _objVolPJLFS;
                    else if ((_reqTypePJLFS == PJLCommands.eRequestType.FSDirList) ||
                             (_reqTypePJLFS == PJLCommands.eRequestType.FSMkDir))
                        path = _objDirPJLFS;
                    else
                        path = _objPathPJLFS;

                    ToolStatusReadbackPJLFS.generateRequest(requestWriter,
                                                             _cmdIndxPJLFS,
                                                             _flagPJLFSSecJob,
                                                             _passwordPJLFS,
                                                             path,
                                                             _binSrcFilenamePJLFS,
                                                             _valPJLFSOpt1,
                                                             _valPJLFSOpt2);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Send generated request data (read from the temporary file) //
                // to the target device, then (in most cases) read the        //
                // response.                                                  //
                //                                                            //
                //------------------------------------------------------------//

                _targetType = TargetCore.getType();

                if (_targetType == TargetCore.eTarget.File)
                {
                    TargetCore.RequestStreamWrite(false);

                    txtReply.Text = "Request sequence has been saved to" +
                                    " file.\n\n" +
                                    "Specified target of 'File' means that" +
                                    " a reply is not meaningful.";
                }
                else if (_targetType == TargetCore.eTarget.NetPrinter)
                {
                    if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // PCL Status Readback.                               //
                        // Response from printer is expected.                 //
                        //                                                    //
                        //----------------------------------------------------//

                        ToolStatusReadbackPCL.sendRequest();

                        txtReply.Text = ToolStatusReadbackPCL.readResponse();
                    }
                    else if (!_flagPJLFS)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // PJL Status Readback.                               //
                        // Response from printer is expected.                 //
                        //                                                    //
                        //----------------------------------------------------//

                        ToolStatusReadbackPJL.sendRequest();

                        txtReply.Text = ToolStatusReadbackPJL.readResponse();
                    }
                    else
                    {
                        //--------------------------------------------------------//
                        //                                                        //
                        // PJL File System commands.                              //
                        // Response from printer is expected for some of the      //
                        // commands, but not all.                                 //
                        //                                                        //
                        //--------------------------------------------------------//

                        ToolStatusReadbackPJLFS.sendRequest(_cmdIndxPJLFS);

                        txtReply.Text = ToolStatusReadbackPJLFS.readResponse(
                            _cmdIndxPJLFS,
                            _binTgtFilenamePJLFS);
                    }
                }
                else if (_targetType == TargetCore.eTarget.WinPrinter)
                {
                    txtReply.Text = "This application does not support" +
                                    " Status readback via a Windows printer" +
                                    " instance.\n\n" +
                                    "Choose a network printer Target instead.";
                }
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

            btnSaveReport.Visibility = Visibility.Visible;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n P J L F S L o c P a t h B r o w s e _ C l i c k              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'binary source/target' browse button is clicked.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnPJLFSLocPathBrowse_Click(object sender, EventArgs e)
        {
            bool selected,
                    upload;

            string filename;

            if (_reqTypePJLFS == PJLCommands.eRequestType.FSUpload)
            {
                upload = true;
                filename = _binTgtFilenamePJLFS;
            }
            else
            {
                upload = false;
                filename = _binSrcFilenamePJLFS;
            }

            selected = SelectLocBinFile(upload, ref filename);

            if (selected)
            {
                txtPJLFSLocPath.Text = filename;

                if (upload)
                {
                    _binTgtFilenamePJLFS = filename;
                }
                else
                {
                    _binSrcFilenamePJLFS = filename;
                }
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

        private void btnSaveReport_Click(object sender, EventArgs e)
        {
            bool flagOptRptWrap = false;

            ReportCore.eRptFileFmt rptFileFmt = ReportCore.eRptFileFmt.NA;
            ReportCore.eRptChkMarks rptChkMarks = ReportCore.eRptChkMarks.NA;

            TargetCore.MetricsReturnFileRpt(
                ToolCommonData.eToolIds.StatusReadback,
                ref rptFileFmt,
                ref rptChkMarks,
                ref flagOptRptWrap);

            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                ToolStatusReadbackReport.generate(rptFileFmt,
                                                  txtReply,
                                                  ref _reportFilenamePCL);
            else if (_crntPDL == ToolCommonData.ePrintLang.PJL)
                ToolStatusReadbackReport.generate(rptFileFmt,
                                                  txtReply,
                                                  ref _reportFilenamePJL);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P C L E n t i t y _ S e l e c t i o n C h a n g e d            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL 'Entity' selection is changed.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPCLEntity_SelectionChanged(object sender,
                                                  SelectionChangedEventArgs e)
        {
            _indxPCLEntityType = cbPCLEntityType.SelectedIndex;

            txtReply.Clear();

            btnSaveReport.Visibility = Visibility.Hidden;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P C L L o c T y p e _ S e l e c t i o n C h a n g e d          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL 'Entity' selection is changed.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPCLLocType_SelectionChanged(object sender,
                                                   SelectionChangedEventArgs e)
        {
            _indxPCLLocType = cbPCLLocType.SelectedIndex;

            txtReply.Clear();

            btnSaveReport.Visibility = Visibility.Hidden;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P J L C a t e g o r y _ S e l e c t i o n C h a n g e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PJL 'Category' selection is changed.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPJLCategory_SelectionChanged(object sender,
                                                    SelectionChangedEventArgs e)
        {
            _indxPJLCategory = cbPJLCategory.SelectedIndex;

            if (PJLCategories.GetType(_indxPJLCategory) ==
                PJLCategories.eCategoryType.Custom)
                txtPJLCustomCat.Visibility = Visibility.Visible;
            else
                txtPJLCustomCat.Visibility = Visibility.Hidden;

            txtReply.Clear();

            btnSaveReport.Visibility = Visibility.Hidden;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P J L C o m m a n d _ S e l e c t i o n C h a n g e d          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PJL 'Command' selection is changed.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPJLCommand_SelectionChanged(object sender,
                                                   SelectionChangedEventArgs e)
        {
            _indxPJLCommand = cbPJLCommand.SelectedIndex;

            _cmdIndxPJL = _subsetPJLCommands[_indxPJLCommand];

            _reqTypePJL = PJLCommands.GetType(_cmdIndxPJL);

            txtPJLCustomCat.Visibility = Visibility.Hidden;
            txtPJLCustomVar.Visibility = Visibility.Hidden;

            if (_reqTypePJL == PJLCommands.eRequestType.Category)
            {
                lbPJLCategory.Visibility = Visibility.Visible;
                cbPJLCategory.Visibility = Visibility.Visible;
                lbPJLVariable.Visibility = Visibility.Hidden;
                cbPJLVariable.Visibility = Visibility.Hidden;

                if (PJLCategories.GetType(_indxPJLCategory) ==
                    PJLCategories.eCategoryType.Custom)
                    txtPJLCustomCat.Visibility = Visibility.Visible;
            }
            else
            {
                lbPJLCategory.Visibility = Visibility.Hidden;
                cbPJLCategory.Visibility = Visibility.Hidden;
                lbPJLVariable.Visibility = Visibility.Visible;
                cbPJLVariable.Visibility = Visibility.Visible;

                if (PJLVariables.GetType(_indxPJLVariable) ==
                    PJLVariables.eVarType.Custom)
                    txtPJLCustomVar.Visibility = Visibility.Visible;
            }

            txtReply.Clear();

            btnSaveReport.Visibility = Visibility.Hidden;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P J L F S C o m m a n d _ S e l e c t i o n C h a n g e d      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PJL 'Command' selection is changed.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPJLFSCommand_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            _indxPJLFSCommand = cbPJLFSCommand.SelectedIndex;

            _cmdIndxPJLFS = _subsetPJLFSCommands[_indxPJLFSCommand];

            _reqTypePJLFS = PJLCommands.GetType(_cmdIndxPJLFS);

            lbPJLFSLocPath.Visibility = Visibility.Hidden;
            txtPJLFSLocPath.Visibility = Visibility.Hidden;
            btnPJLFSLocPathBrowse.Visibility = Visibility.Hidden;

            lbPJLFSOpt1.Visibility = Visibility.Hidden;
            txtPJLFSOpt1.Visibility = Visibility.Hidden;

            lbPJLFSOpt2.Visibility = Visibility.Hidden;
            txtPJLFSOpt2.Visibility = Visibility.Hidden;

            if (_reqTypePJLFS == PJLCommands.eRequestType.FSInit)
            {
                txtPJLFSPath.Text = _objVolPJLFS;
            }
            else if (_reqTypePJLFS == PJLCommands.eRequestType.FSBinSrc)
            {
                txtPJLFSPath.Text = _objPathPJLFS;

                lbPJLFSLocPath.Content = "Source file:";
                lbPJLFSLocPath.Visibility = Visibility.Visible;

                txtPJLFSLocPath.Text = _binSrcFilenamePJLFS;
                txtPJLFSLocPath.Visibility = Visibility.Visible;

                btnPJLFSLocPathBrowse.Visibility = Visibility.Visible;
            }
            else if (_reqTypePJLFS == PJLCommands.eRequestType.FSUpload)
            {
                txtPJLFSPath.Text = _objPathPJLFS;

                lbPJLFSLocPath.Content = "Target file:";
                lbPJLFSLocPath.Visibility = Visibility.Visible;

                txtPJLFSLocPath.Text = _binTgtFilenamePJLFS;
                txtPJLFSLocPath.Visibility = Visibility.Visible;

                btnPJLFSLocPathBrowse.Visibility = Visibility.Visible;

                lbPJLFSOpt1.Content = "Size:";
                lbPJLFSOpt1.Visibility = Visibility.Visible;
                txtPJLFSOpt1.Visibility = Visibility.Visible;

                _valPJLFSOpt1 = _defaultPJLFSSize;
                txtPJLFSOpt1.Text = _valPJLFSOpt1.ToString();

                lbPJLFSOpt2.Content = "Offset:";
                lbPJLFSOpt2.Visibility = Visibility.Visible;
                txtPJLFSOpt2.Visibility = Visibility.Visible;

                _valPJLFSOpt2 = _defaultPJLFSOffset;
                txtPJLFSOpt2.Text = _valPJLFSOpt2.ToString();
            }
            else if (_reqTypePJLFS == PJLCommands.eRequestType.FSDirList)
            {
                txtPJLFSPath.Text = _objDirPJLFS;

                lbPJLFSOpt1.Content = "Count:";
                lbPJLFSOpt1.Visibility = Visibility.Visible;
                txtPJLFSOpt1.Visibility = Visibility.Visible;

                _valPJLFSOpt1 = _defaultPJLFSCount;
                txtPJLFSOpt1.Text = _valPJLFSOpt1.ToString();

                lbPJLFSOpt2.Content = "Entry:";
                lbPJLFSOpt2.Visibility = Visibility.Visible;
                txtPJLFSOpt2.Visibility = Visibility.Visible;

                _valPJLFSOpt2 = _defaultPJLFSEntry;
                txtPJLFSOpt2.Text = _valPJLFSOpt2.ToString();
            }
            else if (_reqTypePJLFS == PJLCommands.eRequestType.FSMkDir)
            {
                txtPJLFSPath.Text = _objDirPJLFS;
            }
            else    // reqTypePJLFS == .FSDelete || .FsQuery //
            {
                txtPJLFSPath.Text = _objPathPJLFS;
            }

            txtReply.Clear();

            btnSaveReport.Visibility = Visibility.Hidden;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P J L V a r i a b l e _ S e l e c t i o n C h a n g e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PJL 'Variable' selection is changed.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPJLVariable_SelectionChanged(object sender,
                                                    SelectionChangedEventArgs e)
        {
            _indxPJLVariable = cbPJLVariable.SelectedIndex;

            if (PJLVariables.GetType(_indxPJLVariable) ==
                PJLVariables.eVarType.Custom)
                txtPJLCustomVar.Visibility = Visibility.Visible;
            else
                txtPJLCustomVar.Visibility = Visibility.Hidden;

            txtReply.Clear();

            btnSaveReport.Visibility = Visibility.Hidden;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P J L F S S e c J o b_ C h e c k e d                         //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PJLFS secure job checkbox is checked.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPJLFSSecJob_Checked(object sender, RoutedEventArgs e)
        {
            _flagPJLFSSecJob = true;

            lbPJLFSPwd.Visibility = Visibility.Visible;
            txtPJLFSPwd.Visibility = Visibility.Visible;

            txtPJLFSPwd.Text = _defaultPJLFSPassword.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P J L F S S e c J o b_ U n c h e c k e d                     //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PJLFS secure job checkbox is unchecked.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPJLFSSecJob_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPJLFSSecJob = false;

            lbPJLFSPwd.Visibility = Visibility.Hidden;
            txtPJLFSPwd.Visibility = Visibility.Hidden;
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
            PJLVariables.eVarType varType;

            PJLCommands.eCmdIndex indxCmd;

            string personality;

            //----------------------------------------------------------------//
            //                                                                //
            // Populate form.                                                 //
            //                                                                //
            //----------------------------------------------------------------//

            cbPCLEntityType.Items.Clear();

            _ctPCLEntityTypes = PCLEntityTypes.GetCount();

            for (int i = 0; i < _ctPCLEntityTypes; i++)
            {
                cbPCLEntityType.Items.Add(PCLEntityTypes.GetName(i));
            }

            //----------------------------------------------------------------//

            cbPCLLocType.Items.Clear();

            _ctPCLLocTypes = PCLLocationTypes.GetCount();

            for (int i = 0; i < _ctPCLLocTypes; i++)
            {
                cbPCLLocType.Items.Add(PCLLocationTypes.GetName(i));
            }

            //----------------------------------------------------------------//

            cbPJLCommand.Items.Clear();

            _ctPJLCommands = _subsetPJLCommands.Length;

            for (int i = 0; i < _ctPJLCommands; i++)
            {
                indxCmd = _subsetPJLCommands[i];

                cbPJLCommand.Items.Add(PJLCommands.GetName(indxCmd));
            }

            //----------------------------------------------------------------//

            cbPJLFSCommand.Items.Clear();

            _ctPJLFSCommands = _subsetPJLFSCommands.Length;

            for (int i = 0; i < _ctPJLFSCommands; i++)
            {
                indxCmd = _subsetPJLFSCommands[i];

                cbPJLFSCommand.Items.Add(PJLCommands.GetName(indxCmd));
            }

            //----------------------------------------------------------------//

            cbPJLCategory.Items.Clear();

            _ctPJLCategories = PJLCategories.GetCount();

            for (int i = 0; i < _ctPJLCategories; i++)
            {
                cbPJLCategory.Items.Add(PJLCategories.GetName(i));
            }

            //----------------------------------------------------------------//

            cbPJLVariable.Items.Clear();

            _ctPJLVariables = PJLVariables.GetCount();

            for (int i = 0; i < _ctPJLVariables; i++)
            {
                varType = PJLVariables.GetType(i);

                if (varType == PJLVariables.eVarType.PCL)
                    personality = "PCL: ";
                else if (varType == PJLVariables.eVarType.PDF)
                    personality = "PDF: ";
                else if (varType == PJLVariables.eVarType.PS)
                    personality = "POSTSCRIPT: ";
                else
                    personality = string.Empty;

                cbPJLVariable.Items.Add(personality + PJLVariables.GetName(i));
            }

            //----------------------------------------------------------------//

            ResetTarget();

            //----------------------------------------------------------------//
            //                                                                //
            // Reinstate settings from persistent storage.                    //
            //                                                                //
            //----------------------------------------------------------------//

            MetricsLoad();

            cbPCLEntityType.SelectedIndex = _indxPCLEntityType;
            cbPCLLocType.SelectedIndex = _indxPCLLocType;
            cbPJLCategory.SelectedIndex = _indxPJLCategory;
            cbPJLCommand.SelectedIndex = _indxPJLCommand;
            cbPJLFSCommand.SelectedIndex = _indxPJLFSCommand;
            cbPJLVariable.SelectedIndex = _indxPJLVariable;

            _cmdIndxPJL = _subsetPJLCommands[_indxPJLCommand];
            _cmdIndxPJLFS = _subsetPJLFSCommands[_indxPJLFSCommand];

            _reqTypePJL = PJLCommands.GetType(_cmdIndxPJL);
            _reqTypePJLFS = PJLCommands.GetType(_cmdIndxPJLFS);

            _passwordPJLFS = _defaultPJLFSPassword;

            ToolCommonFunctions.SplitPathName(_objPathPJLFS,
                                               ref _objVolPJLFS,
                                               ref _objDirPJLFS,
                                               ref _objFilPJLFS);

            if (_crntPDL == ToolCommonData.ePrintLang.PJL)
            {
                if (_flagPJLFS)
                {
                    rbSelTypePJLFS.IsChecked = true;
                    tabPDLs.SelectedItem = tabPJLFS;

                    if (_reqTypePJLFS == PJLCommands.eRequestType.FSUpload)
                        txtPJLFSLocPath.Text = _binTgtFilenamePJLFS;
                    else
                        txtPJLFSLocPath.Text = _binSrcFilenamePJLFS;

                    if (_reqTypePJLFS == PJLCommands.eRequestType.FSInit)
                        txtPJLFSPath.Text = _objVolPJLFS;
                    else if ((_reqTypePJLFS == PJLCommands.eRequestType.FSDirList) ||
                             (_reqTypePJLFS == PJLCommands.eRequestType.FSMkDir))
                        txtPJLFSPath.Text = _objDirPJLFS;
                    else
                        txtPJLFSPath.Text = _objPathPJLFS;

                    if (_flagPJLFSSecJob)
                    {
                        lbPJLFSPwd.Visibility = Visibility.Visible;
                        txtPJLFSPwd.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        lbPJLFSPwd.Visibility = Visibility.Hidden;
                        txtPJLFSPwd.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    rbSelTypePJL.IsChecked = true;
                    tabPDLs.SelectedItem = tabPJL;

                    txtPJLCustomCat.Visibility = Visibility.Hidden;
                    txtPJLCustomVar.Visibility = Visibility.Hidden;

                    txtPJLCustomCat.Text = _customCatPJL;
                    txtPJLCustomVar.Text = _customVarPJL;

                    if (_reqTypePJL == PJLCommands.eRequestType.Category)
                    {
                        if (PJLCategories.GetType(_indxPJLCategory) ==
                            PJLCategories.eCategoryType.Custom)
                            txtPJLCustomCat.Visibility = Visibility.Visible;

                    }
                    else if (_reqTypePJL == PJLCommands.eRequestType.Variable)
                    {
                        if (PJLVariables.GetType(_indxPJLVariable) ==
                            PJLVariables.eVarType.Custom)
                            txtPJLCustomVar.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                rbSelTypePCL.IsChecked = true;
                tabPDLs.SelectedItem = tabPCL;
            }

            txtReply.Clear();

            btnSaveReport.Visibility = Visibility.Hidden;
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
            int indxTemp = 0;

            ToolStatusReadbackPersist.LoadDataCommon(ref indxTemp);

            ToolStatusReadbackPersist.LoadDataPCL(ref _indxPCLEntityType,
                                                   ref _indxPCLLocType,
                                                   ref _reportFilenamePCL);

            ToolStatusReadbackPersist.LoadDataPJL(ref _indxPJLCategory,
                                                   ref _indxPJLCommand,
                                                   ref _indxPJLVariable,
                                                   ref _customCatPJL,
                                                   ref _customVarPJL,
                                                   ref _reportFilenamePJL);

            ToolStatusReadbackPersist.LoadDataPJLFS(ref _indxPJLFSCommand,
                                                     ref _objPathPJLFS,
                                                     ref _binSrcFilenamePJLFS,
                                                     ref _binTgtFilenamePJLFS,
                                                     ref _flagPJLFS,
                                                     ref _flagPJLFSSecJob);

            //----------------------------------------------------------------//

            if (indxTemp == (int)ToolCommonData.ePrintLang.PJL)
                _crntPDL = ToolCommonData.ePrintLang.PJL;
            else
                _crntPDL = ToolCommonData.ePrintLang.PCL;

            //----------------------------------------------------------------//

            if ((_indxPCLEntityType < 0) ||
                (_indxPCLEntityType >= _ctPCLEntityTypes))
                _indxPCLEntityType = 0;

            if ((_indxPCLLocType < 0) ||
                (_indxPCLLocType >= _ctPCLLocTypes))
                _indxPCLLocType = 0;

            //----------------------------------------------------------------//

            if ((_indxPJLCategory < 0) ||
                (_indxPJLCategory >= _ctPJLCategories))
                _indxPJLCategory = 0;

            if ((_indxPJLCommand < 0) ||
                (_indxPJLCommand >= _ctPJLCommands))
                _indxPJLCommand = 0;

            if ((_indxPJLVariable < 0) ||
                (_indxPJLVariable >= _ctPJLVariables))
                _indxPJLVariable = 0;

            if ((_indxPJLFSCommand < 0) ||
                (_indxPJLFSCommand >= _ctPJLFSCommands))
                _indxPJLFSCommand = 0;
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
            ToolStatusReadbackPersist.SaveDataCommon((int)_crntPDL);

            ToolStatusReadbackPersist.SaveDataPCL(_indxPCLEntityType,
                                               _indxPCLLocType,
                                               _reportFilenamePCL);

            ToolStatusReadbackPersist.SaveDataPJL(_indxPJLCategory,
                                               _indxPJLCommand,
                                               _indxPJLVariable,
                                               _customCatPJL,
                                               _customVarPJL,
                                               _reportFilenamePJL);

            ToolStatusReadbackPersist.SaveDataPJLFS(_indxPJLFSCommand,
                                                     _objPathPJLFS,
                                                     _binSrcFilenamePJLFS,
                                                     _binTgtFilenamePJLFS,
                                                     _flagPJLFS,
                                                     _flagPJLFSSecJob);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S e l T y p e P C L _ C l i c k                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting PCL clicked.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSelTypePCL_Click(object sender, RoutedEventArgs e)
        {
            _crntPDL = ToolCommonData.ePrintLang.PCL;

            tabPDLs.SelectedItem = tabPCL;

            txtReply.Clear();

            btnSaveReport.Visibility = Visibility.Hidden;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S e l T y p e P J L _ C l i c k                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting PJL clicked.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSelTypePJL_Click(object sender, RoutedEventArgs e)
        {
            _crntPDL = ToolCommonData.ePrintLang.PJL;
            _flagPJLFS = false;

            tabPDLs.SelectedItem = tabPJL;

            txtReply.Clear();

            btnSaveReport.Visibility = Visibility.Hidden;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S e l T y p e P J L F S _ C l i c k                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting PJL FS clicked.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSelTypePJLFS_Click(object sender, RoutedEventArgs e)
        {
            _crntPDL = ToolCommonData.ePrintLang.PJL;
            _flagPJLFS = true;

            tabPDLs.SelectedItem = tabPJLFS;

            if (_flagPJLFSSecJob)
            {
                lbPJLFSPwd.Visibility = Visibility.Visible;
                txtPJLFSPwd.Visibility = Visibility.Visible;
            }
            else
            {
                lbPJLFSPwd.Visibility = Visibility.Hidden;
                txtPJLFSPwd.Visibility = Visibility.Hidden;
            }

            txtReply.Clear();

            btnSaveReport.Visibility = Visibility.Hidden;
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
                btnGenerate.Content = "Generate request & save to file";
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

                btnGenerate.Content = "Generate request & read reply from " +
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
        // s e l e c t L o c B i n F i l e                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initiate 'open file' dialogue for binary file, used with:          //
        //  -   FSAPPEND   with binary Source file                            //
        //  -   FSDOWNLOAD             Source                                 //
        //  -   FSUPLOAD               Target                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool SelectLocBinFile(bool upload,
                                          ref string locBinFilename)
        {
            bool selected;

            if (upload)
                selected = SelectLocBinTgtFile(ref locBinFilename);
            else
                selected = SelectLocBinSrcFile(ref locBinFilename);

            return selected;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e l e c t L o c B i n S r c F i l e                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initiate 'open file' dialogue for binary Source file, used with:   //
        //  -   FSAPPEND                                                      //
        //  -   FSDOWNLOAD                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool SelectLocBinSrcFile(ref string locBinFilename)
        {
            OpenFileDialog openDialog = ToolCommonFunctions.CreateOpenFileDialog(locBinFilename);

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
                locBinFilename = openDialog.FileName;

            return dialogResult == true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e l e c t L o c B i n T g t F i l e                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initiate 'open file' dialogue for binary Target file, used with:   //
        //  -   FSUPLOAD                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool SelectLocBinTgtFile(ref string locBinFilename)
        {
            SaveFileDialog saveDialog = ToolCommonFunctions.CreateSaveFileDialog(locBinFilename);

            saveDialog.Filter = "Print Files|" +
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

            saveDialog.DefaultExt = "pcl";

            bool? dialogResult = saveDialog.ShowDialog();

            if (dialogResult == true)
                locBinFilename = saveDialog.FileName;

            return dialogResult == true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P J L C u s t o m C a t _ G o t F o c u s                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for PJL Category custom valuie has focus.                 //
        // Select all text in the box, so that it can be over-written easily, //
        // without inadvertently causing validation failures.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPJLCustomCat_GotFocus(object sender,
                                           RoutedEventArgs e)
        {
            txtPJLCustomCat.SelectAll();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P J L C u s t o m C a t _ L o s t F o c u s                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for PJL Category custom valuie has lost focus.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPJLCustomCat_LostFocus(object sender,
                                            RoutedEventArgs e)
        {
            _customCatPJL = txtPJLCustomCat.Text;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P J L C u s t o m V a r _ G o t F o c u s                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for PJL Variable custom valuie has focus.                 //
        // Select all text in the box, so that it can be over-written easily, //
        // without inadvertently causing validation failures.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPJLCustomVar_GotFocus(object sender,
                                           RoutedEventArgs e)
        {
            txtPJLCustomVar.SelectAll();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P J L C u s t o m V a r _ L o s t F o c u s                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for PJL Variable custom valuie has lost focus.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPJLCustomVar_LostFocus(object sender,
                                            RoutedEventArgs e)
        {
            _customVarPJL = txtPJLCustomVar.Text;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P J L F S L o c P a t h _ G o t F o c u s                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for PJL FileSystem binary source / target file has focus. //
        // Select all text in the box, so that it can be over-written easily, //
        // without inadvertently causing validation failures.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPJLFSLocPath_GotFocus(object sender,
                                               RoutedEventArgs e)
        {
            txtPJLFSLocPath.SelectAll();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P J L F S L o c P a t h _ L o s t F o c u s                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for PJL FileSystem binary source / target  file has lost  //
        // focus.                                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPJLFSLocPath_LostFocus(object sender,
                                                RoutedEventArgs e)
        {
            if (_reqTypePJLFS == PJLCommands.eRequestType.FSUpload)
                _binTgtFilenamePJLFS = txtPJLFSLocPath.Text;
            else
                _binSrcFilenamePJLFS = txtPJLFSLocPath.Text;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P J L F S O p t 1 _ G o t F o c u s                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for PJL FileSystem option 1 has focus.                    //
        // Select all text in the box, so that it can be over-written easily, //
        // without inadvertently causing validation failures.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPJLFSOpt1_GotFocus(object sender,
                                            RoutedEventArgs e)
        {
            txtPJLFSOpt1.SelectAll();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P J L F S O p t 1 _ L o s t F o c u s                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for PJL FileSystem option 1 has lost focus.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPJLFSOpt1_LostFocus(object sender,
                                             RoutedEventArgs e)
        {
            if (!ValidatePJLFSOpt1(true, ref _valPJLFSOpt1))
            {
                txtPJLFSOpt1.Focus();
                txtPJLFSOpt1.SelectAll();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P J L F S O p t 2 _ G o t F o c u s                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for PJL FileSystem option 2 has focus.                    //
        // Select all text in the box, so that it can be over-written easily, //
        // without inadvertently causing validation failures.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPJLFSOpt2_GotFocus(object sender,
                                            RoutedEventArgs e)
        {
            txtPJLFSOpt2.SelectAll();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P J L F S O p t 2 _ L o s t F o c u s                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for PJL FileSystem option 2 has lost focus.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPJLFSOpt2_LostFocus(object sender,
                                             RoutedEventArgs e)
        {
            if (!ValidatePJLFSOpt2(true, ref _valPJLFSOpt2))
            {
                txtPJLFSOpt2.Focus();
                txtPJLFSOpt2.SelectAll();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P J L F S P a t h _ G o t F o c u s                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for PJL FileSystem object name has focus.                 //
        // Select all text in the box, so that it can be over-written easily, //
        // without inadvertently causing validation failures.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPJLFSPath_GotFocus(object sender,
                                            RoutedEventArgs e)
        {
            txtPJLFSPath.SelectAll();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P J L F S P a t h _ L o s t F o c u s                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for PJL FileSystem object name has lost focus.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPJLFSPath_LostFocus(object sender,
                                             RoutedEventArgs e)
        {
            if (_reqTypePJLFS == PJLCommands.eRequestType.FSInit)
                _objVolPJLFS = txtPJLFSPath.Text;
            else if ((_reqTypePJLFS == PJLCommands.eRequestType.FSDirList) ||
                     (_reqTypePJLFS == PJLCommands.eRequestType.FSMkDir))
                _objDirPJLFS = txtPJLFSPath.Text;
            else
                _objPathPJLFS = txtPJLFSPath.Text;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P J L F S P w d _ G o t F o c u s                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for PJL FileSystem password has focus.                    //
        // Select all text in the box, so that it can be over-written easily, //
        // without inadvertently causing validation failures.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPJLFSPwd_GotFocus(object sender,
                                           RoutedEventArgs e)
        {
            txtPJLFSPwd.SelectAll();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t P J L F S P w d _ L o s t F o c u s                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Text box for PJL FileSystem password has lost focus.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtPJLFSPwd_LostFocus(object sender,
                                            RoutedEventArgs e)
        {
            _passwordPJLFS = txtPJLFSPwd.Text;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P J L F S O p t 1                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PJL File System optional paramater 1.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePJLFSOpt1(bool lostFocusEvent,
                                           ref int newValue)
        {
            int value;
            int defVal;

            string boxName;

            bool OK = true;

            if (_reqTypePJLFS == PJLCommands.eRequestType.FSDirList)
            {
                boxName = "Count";
                defVal = _defaultPJLFSCount;
            }
            else
            {
                boxName = "Size";
                defVal = _defaultPJLFSSize;
            }

            string crntText = txtPJLFSOpt1.Text;

            OK = int.TryParse(crntText, out value);

            if (OK)
                if (value < 0)
                    OK = false;

            if (OK)
            {
                newValue = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString();

                    MessageBox.Show(boxName + " value is invalid.\n\n" +
                                     "Value will be reset to default '" +
                                     newText + "'",
                                     "Option value invalid",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Warning);

                    txtPJLFSOpt1.Text = newText;
                    newValue = defVal;

                    OK = true;
                }
                else
                {
                    MessageBox.Show(boxName + " value is invalid.\n\n",
                                     "Option value invalid",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Warning);

                    txtPJLFSOpt1.Focus();
                    txtPJLFSOpt1.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e P J L F S O p t 2                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate PJL File System optional paramater 2.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidatePJLFSOpt2(bool lostFocusEvent,
                                           ref int newValue)
        {
            int value;
            int defVal;

            string boxName;

            bool OK = true;

            if (_reqTypePJLFS == PJLCommands.eRequestType.FSDirList)
            {
                boxName = "Entry";
                defVal = _defaultPJLFSEntry;
            }
            else
            {
                boxName = "Offset";
                defVal = _defaultPJLFSOffset;
            }

            string crntText = txtPJLFSOpt2.Text;

            OK = int.TryParse(crntText, out value);

            if (OK)
                if (value < 0)
                    OK = false;

            if (OK)
            {
                newValue = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString();

                    MessageBox.Show(boxName + " value is invalid.\n\n" +
                                     "Value will be reset to default '" +
                                     newText + "'",
                                     "Option value invalid",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Warning);

                    txtPJLFSOpt2.Text = newText;
                    newValue = defVal;

                    OK = true;
                }
                else
                {
                    MessageBox.Show(boxName + " value is invalid.\n\n",
                                     "Option value invalid",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Warning);

                    txtPJLFSOpt2.Focus();
                    txtPJLFSOpt2.SelectAll();
                }
            }

            return OK;
        }
    }
}
