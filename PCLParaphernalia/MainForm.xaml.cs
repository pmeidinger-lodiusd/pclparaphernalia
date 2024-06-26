﻿using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// <para>Interaction logic for MainForm.xaml</para>
    /// <para>This is the main form of the PCLParaphernalia application.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public partial class MainForm : Window
    {
        public const string _regMainKey = @"Software\PCLParaphernalia";

        public bool _runXXXDiags;  // ****  design time toggle ****//

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private ToolFontSample _subFormToolFontSample;
        private ToolFormSample _subFormToolFormSample;
        private ToolImageBitmap _subFormToolImageBitmap;
        private ToolMakeOverlay _subFormToolMakeOverlay;
        private ToolMiscSamples _subFormToolMiscSamples;

        //    private ToolPatternGenerate     _subFormToolPatternGenerate     = null;
        private ToolPrintArea _subFormToolPrintArea;

        private ToolPrintLang _subFormToolPrintLang;
        private ToolPrnAnalyse _subFormToolPrnAnalyse;
        private ToolPrnPrint _subFormToolPrnPrint;
        private ToolSoftFontGenerate _subFormToolSoftFontGenerate;
        private ToolStatusReadback _subFormToolStatusReadback;
        private ToolSymbolSetGenerate _subFormToolSymbolSetGenerate;
        private ToolTrayMap _subFormToolTrayMap;
        private ToolXXXDiags _subFormToolXXXDiags;

        private ToolCommonData.ToolIds _crntToolId = ToolCommonData.ToolIds.Min;

        private ToolCommonData.PrintLang _crntPDL = ToolCommonData.PrintLang.Unknown;

        private ToolCommonData.ToolSubIds _crntSubId = ToolCommonData.ToolSubIds.None;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // M a i n f o r m                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public MainForm(string filename)
        {
            InitializeComponent();

            int mwLeft = -1,
                  mwTop = -1,
                  mwHeight = -1,
                  mwWidth = -1,
                  mwScale = 100;

            int versionMajorOld = -1;
            int versionMinorOld = -1;
            int versionBuildOld = -1;
            int versionRevisionOld = -1;

            //----------------------------------------------------------------//
            //                                                                //
            // Load window state values from registry.                        //
            //                                                                //
            //----------------------------------------------------------------//

            MainFormPersist.LoadWindowData(out mwLeft,
                                           out mwTop,
                                           out mwHeight,
                                           out mwWidth,
                                           out mwScale);

            if ((mwLeft == -1) || (mwTop == -1) || (mwHeight == -1) || (mwWidth == -1))
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                Width = 801;
                Height = 842;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.Manual;

                Left = mwLeft;
                Top = mwTop;
                Height = mwHeight;
                Width = mwWidth;
            }

            if ((mwScale < 25) || (mwScale > 1000))
            {
                mwScale = 100;
            }

            double windowScale = (double)mwScale / 100;

            MainFormData.WindowScale = windowScale;

            zoomSlider.Value = windowScale;

            //----------------------------------------------------------------//
            //                                                                //
            // Check for version-specific updates.                            //
            //                                                                //
            //----------------------------------------------------------------//

            Assembly assembly = Assembly.GetExecutingAssembly();

            AssemblyName assemblyName = assembly.GetName();

            int versionMajorCrnt = assemblyName.Version.Major;
            int versionMinorCrnt = assemblyName.Version.Minor;
            int versionBuildCrnt = assemblyName.Version.Build;
            int versionRevisionCrnt = assemblyName.Version.Revision;

            MainFormData.SetVersionData(true, versionMajorCrnt,
                                               versionMinorCrnt,
                                               versionBuildCrnt,
                                               versionRevisionCrnt);

            MainFormPersist.LoadVersionData(out versionMajorOld,
                                            out versionMinorOld,
                                            out versionBuildOld,
                                            out versionRevisionOld);

            MainFormData.SetVersionData(false, versionMajorOld,
                                                versionMinorOld,
                                                versionBuildOld,
                                                versionRevisionOld);

            MainFormData.VersionChange = (versionMajorCrnt != versionMajorOld) ||
                (versionMinorCrnt != versionMinorOld) ||
                (versionBuildCrnt != versionBuildOld) ||
                (versionRevisionCrnt != versionRevisionOld);

            if (versionMajorOld == -1)
            {
                //----------------------------------------------------------------//
                //                                                                //
                // First run of post 2.5.0.0 version.                             //
                // Invoke default working folder dialogue.                        //
                //                                                                //
                //----------------------------------------------------------------//

                WorkFolder workFolder = new WorkFolder();

                bool? dialogResult = workFolder.ShowDialog();
            }

            MainFormPersist.SaveVersionData(versionMajorCrnt,
                                             versionMinorCrnt,
                                             versionBuildCrnt,
                                             versionRevisionCrnt);

            ToolCommonData.LoadWorkFoldername();

            //----------------------------------------------------------------//
            //                                                                //
            // Load Target state values from registry.                        //
            //                                                                //
            //----------------------------------------------------------------//

            TargetCore.InitialiseSettings();

            if (TargetCore.GetTargetType() == TargetCore.Target.File)
            {
                menuItemTargetFile.IsChecked = true;
                menuItemTargetNetPrinter.IsChecked = false;
                menuItemTargetWinPrinter.IsChecked = false;
            }
            else if (TargetCore.GetTargetType() == TargetCore.Target.NetPrinter)
            {
                menuItemTargetFile.IsChecked = false;
                menuItemTargetNetPrinter.IsChecked = true;
                menuItemTargetWinPrinter.IsChecked = false;
            }
            else if (TargetCore.GetTargetType() == TargetCore.Target.WinPrinter)
            {
                menuItemTargetFile.IsChecked = false;
                menuItemTargetNetPrinter.IsChecked = false;
                menuItemTargetWinPrinter.IsChecked = true;
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Load tool.                                                     //
            // If a command-line parameter is present, load the               //
            // 'PRN File Analyse' tool, and pass the parameter which          //
            // identifies the file to be analysed.                            //
            // Otherwise, load the tool in use when the application was last  //
            // closed.                                                        //
            //                                                                //
            //----------------------------------------------------------------//

            ToolCommonData.ToolIds startToolId;

            _crntToolId = ToolCommonData.ToolIds.Min;

            if (_runXXXDiags)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Load 'XXX Diags' tool.                                     //
                //                                                            //
                // ***** for design time use only *****                       //
                //                                                            //
                //------------------------------------------------------------//

                toolXXXDiags_Selected(this, null);
            }
            else if (filename != string.Empty)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Load 'PRN File Analyse' tool and pass in file name.        //
                //                                                            //
                //------------------------------------------------------------//

                toolPrnAnalyse_Selected(this, null);

                if (filename != string.Empty)
                    _subFormToolPrnAnalyse.PrnFileProcess(filename);
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // Load Tool state values from registry.                      //
                //                                                            //
                //------------------------------------------------------------//

                int crntToolIndex = 0;

                ToolCommonPersist.LoadData(out crntToolIndex);

                if ((crntToolIndex > (int)ToolCommonData.ToolIds.Min) && (crntToolIndex < (int)ToolCommonData.ToolIds.Max))
                    startToolId = (ToolCommonData.ToolIds)crntToolIndex;
                else
                    startToolId = ToolCommonData.ToolIds.PrintLang;

                if (startToolId == ToolCommonData.ToolIds.FontSample)
                    toolFontSample_Selected(this, null);
                else if (startToolId == ToolCommonData.ToolIds.FormSample)
                    toolFormSample_Selected(this, null);
                else if (startToolId == ToolCommonData.ToolIds.ImageBitmap)
                    toolImageBitmap_Selected(this, null);
                else if (startToolId == ToolCommonData.ToolIds.MakeOverlay)
                    toolMakeOverlay_Selected(this, null);
                else if (startToolId == ToolCommonData.ToolIds.MiscSamples)
                    toolMiscSamples_Selected(this, null);
                //      else if (startToolId == ToolCommonData.eToolIds.PatternGenerate)
                //          toolPatternGenerate_Selected(this, null);
                else if (startToolId == ToolCommonData.ToolIds.PrintArea)
                    toolPrintArea_Selected(this, null);
                else if (startToolId == ToolCommonData.ToolIds.PrintLang)
                    toolPrintLang_Selected(this, null);
                else if (startToolId == ToolCommonData.ToolIds.PrnAnalyse)
                    toolPrnAnalyse_Selected(this, null);
                else if (startToolId == ToolCommonData.ToolIds.PrnPrint)
                    toolPrnPrint_Selected(this, null);
                else if (startToolId == ToolCommonData.ToolIds.SoftFontGenerate)
                    toolSoftFontGenerate_Selected(this, null);
                else if (startToolId == ToolCommonData.ToolIds.StatusReadback)
                    toolStatusReadback_Selected(this, null);
                else if (startToolId == ToolCommonData.ToolIds.SymbolSetGenerate)
                    toolSymbolSetGenerate_Selected(this, null);
                else if (startToolId == ToolCommonData.ToolIds.TrayMap)
                    toolTrayMap_Selected(this, null);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c r n t T o o l R e s e t P D L                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve the current PDL selected within the current tool.         //
        // This is so that if TargetFile is configured, any new value is      //
        // stored in the appropriate PDL-specific registry key.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void CrntToolResetPDL()
        {
            if (_crntToolId == ToolCommonData.ToolIds.FontSample)
                _crntPDL = _subFormToolFontSample.GetCurrentPDL();
            else if (_crntToolId == ToolCommonData.ToolIds.FormSample)
                _crntPDL = _subFormToolFormSample.GetCurrentPDL();
            else if (_crntToolId == ToolCommonData.ToolIds.ImageBitmap)
                _crntPDL = _subFormToolImageBitmap.GetCurrentPDL();
            else if (_crntToolId == ToolCommonData.ToolIds.MakeOverlay)
                _crntPDL = _subFormToolMakeOverlay.GetCurrentPDL();
            else if (_crntToolId == ToolCommonData.ToolIds.MiscSamples)
                _crntPDL = _subFormToolMiscSamples.GetCurrentPDL();
            //     else if (_crntToolId == ToolCommonData.eToolIds.PatternGenerate)
            //         _crntPDL = _subFormToolPatternGenerate.GetCurrentPDL();
            else if (_crntToolId == ToolCommonData.ToolIds.PrintArea)
                _crntPDL = _subFormToolPrintArea.GetCurrentPDL();
            else if (_crntToolId == ToolCommonData.ToolIds.PrintLang)
                _crntPDL = _subFormToolPrintLang.GetCurrentPDL();
            else if (_crntToolId == ToolCommonData.ToolIds.PrnAnalyse)
                _crntPDL = _subFormToolPrnAnalyse.GetCurrentPDL();
            else if (_crntToolId == ToolCommonData.ToolIds.PrnPrint)
                _crntPDL = _subFormToolPrnPrint.GetCurrentPDL();
            else if (_crntToolId == ToolCommonData.ToolIds.SoftFontGenerate)
                _crntPDL = _subFormToolSoftFontGenerate.GetCurrentPDL();
            else if (_crntToolId == ToolCommonData.ToolIds.StatusReadback)
                _crntPDL = _subFormToolStatusReadback.GetCurrentPDL();
            else if (_crntToolId == ToolCommonData.ToolIds.SymbolSetGenerate)
                _crntPDL = _subFormToolSymbolSetGenerate.GetCurrentPDL();
            else if (_crntToolId == ToolCommonData.ToolIds.TrayMap)
                _crntPDL = _subFormToolTrayMap.GetCurrentPDL();
            else if (_crntToolId == ToolCommonData.ToolIds.XXXDiags)
                _crntPDL = _subFormToolXXXDiags.GetCurrentPDL();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c r n t T o o l R e s e t S u b I d                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Retrieve the current sub-identifier (if any) selected within the   //
        // current tool.                                                      //
        // This is so that if TargetFile is configured, any new value is      //
        // stored in the appropriate PDL-specific registry key.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void CrntToolResetSubId()
        {
            _crntSubId = ToolCommonData.ToolSubIds.None;

            if (_crntToolId == ToolCommonData.ToolIds.MiscSamples)
                _crntSubId = _subFormToolMiscSamples.GetCurrentType();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c r n t T o o l R e s e t T a r g e t                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset 'current tool' button details (where necessary) after Target //
        // changed.                                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void CrntToolResetTarget()
        {
            if (_crntToolId == ToolCommonData.ToolIds.FontSample)
                _subFormToolFontSample.ResetTarget();
            else if (_crntToolId == ToolCommonData.ToolIds.FormSample)
                _subFormToolFormSample.ResetTarget();
            else if (_crntToolId == ToolCommonData.ToolIds.ImageBitmap)
                _subFormToolImageBitmap.ResetTarget();
            else if (_crntToolId == ToolCommonData.ToolIds.MakeOverlay)
                _subFormToolMakeOverlay.ResetTarget();
            else if (_crntToolId == ToolCommonData.ToolIds.MiscSamples)
                _subFormToolMiscSamples.ResetTarget();
            //      else if (_crntToolId == ToolCommonData.eToolIds.PatternGenerate)
            //          _subFormToolPatternGenerate.resetTarget();
            else if (_crntToolId == ToolCommonData.ToolIds.PrintArea)
                _subFormToolPrintArea.ResetTarget();
            else if (_crntToolId == ToolCommonData.ToolIds.PrintLang)
                _subFormToolPrintLang.ResetTarget();
            else if (_crntToolId == ToolCommonData.ToolIds.PrnAnalyse)
                _subFormToolPrnAnalyse.ResetTarget();
            else if (_crntToolId == ToolCommonData.ToolIds.PrnPrint)
                _subFormToolPrnPrint.ResetTarget();
            else if (_crntToolId == ToolCommonData.ToolIds.SoftFontGenerate)
                _subFormToolSoftFontGenerate.ResetTarget();
            else if (_crntToolId == ToolCommonData.ToolIds.StatusReadback)
                _subFormToolStatusReadback.ResetTarget();
            else if (_crntToolId == ToolCommonData.ToolIds.SymbolSetGenerate)
                _subFormToolSymbolSetGenerate.ResetTarget();
            else if (_crntToolId == ToolCommonData.ToolIds.TrayMap)
                _subFormToolTrayMap.ResetTarget();
            else if (_crntToolId == ToolCommonData.ToolIds.XXXDiags)
                _subFormToolXXXDiags.ResetTarget();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c r n t T o o l S a v e M e t r i c s                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Save metrics for last active subform.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void CrntToolSaveMetrics()
        {
            if (_crntToolId != ToolCommonData.ToolIds.Min)
                ToolCommonPersist.SaveData((int)_crntToolId);

            if (_crntToolId == ToolCommonData.ToolIds.FontSample)
                _subFormToolFontSample.MetricsSave();
            else if (_crntToolId == ToolCommonData.ToolIds.FormSample)
                _subFormToolFormSample.MetricsSave();
            else if (_crntToolId == ToolCommonData.ToolIds.ImageBitmap)
                _subFormToolImageBitmap.MetricsSave();
            else if (_crntToolId == ToolCommonData.ToolIds.MakeOverlay)
                _subFormToolMakeOverlay.MetricsSave();
            else if (_crntToolId == ToolCommonData.ToolIds.MiscSamples)
                _subFormToolMiscSamples.MetricsSave();
            //     else if (_crntToolId == ToolCommonData.eToolIds.PatternGenerate)
            //         _subFormToolPatternGenerate.metricsSave();
            else if (_crntToolId == ToolCommonData.ToolIds.PrintArea)
                _subFormToolPrintArea.MetricsSave();
            else if (_crntToolId == ToolCommonData.ToolIds.PrintLang)
                _subFormToolPrintLang.MetricsSave();
            else if (_crntToolId == ToolCommonData.ToolIds.PrnAnalyse)
                _subFormToolPrnAnalyse.MetricsSave();
            else if (_crntToolId == ToolCommonData.ToolIds.PrnPrint)
                _subFormToolPrnPrint.MetricsSave();
            else if (_crntToolId == ToolCommonData.ToolIds.SoftFontGenerate)
                _subFormToolSoftFontGenerate.MetricsSave();
            else if (_crntToolId == ToolCommonData.ToolIds.StatusReadback)
                _subFormToolStatusReadback.MetricsSave();
            else if (_crntToolId == ToolCommonData.ToolIds.SymbolSetGenerate)
                _subFormToolSymbolSetGenerate.MetricsSave();
            else if (_crntToolId == ToolCommonData.ToolIds.TrayMap)
                _subFormToolTrayMap.MetricsSave();
            else if (_crntToolId == ToolCommonData.ToolIds.XXXDiags)
                _subFormToolXXXDiags.MetricsSave();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c r n t T o o l U n c h e c k A l l                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called whenever current tool is selected/changed.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void CrntToolUncheckAll()
        {
            menuItemToolFontSample.IsChecked = false;
            menuItemToolFormSample.IsChecked = false;
            menuItemToolImageBitmap.IsChecked = false;
            menuItemToolMakeOverlay.IsChecked = false;
            menuItemToolMiscSamples.IsChecked = false;
            //  menuItemToolPatternGenerate.IsChecked = false;
            menuItemToolPrintArea.IsChecked = false;
            menuItemToolPrintLang.IsChecked = false;
            menuItemToolPrnAnalyse.IsChecked = false;
            menuItemToolPrnPrint.IsChecked = false;
            menuItemToolSoftFontGenerate.IsChecked = false;
            menuItemToolStatusReadback.IsChecked = false;
            menuItemToolSymbolSetGenerate.IsChecked = false;
            menuItemToolTrayMap.IsChecked = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f i l e E x i t _ C l i c k                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Application shutdown.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void fileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // h e l p A b o u t _ C l i c k                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Help | About' menu item is selected.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void helpAbout_Click(object sender, RoutedEventArgs e)
        {
            string deploymentVersion;
            if (ApplicationDeployment.IsNetworkDeployed)
                deploymentVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            else
                deploymentVersion = "Stand-alone";

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            string assemblyVersion = assemblyName.Version.ToString();
            string crntVersion;
            if (deploymentVersion == assemblyVersion)
                crntVersion = "Version " + deploymentVersion;
            else
                crntVersion = "Deployment Version: " + deploymentVersion + "\r\nAssembly Version: " + assemblyVersion;

            MessageBox.Show("PCL Paraphernalia\r\n\r\n" +
                             crntVersion + "\r\n\r\n" +
                             "To report errors, please open an issue on\r\n" +
                             "https://github.com/michaelknigge/pclparaphernalia/issues\r\n\r\n" +
                             "Source code is available on GitHub, see\r\n" +
                             "https://github.com/michaelknigge/pclparaphernalia",
                             "Help About",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // h e l p C o n t e n t s _ C l i c k                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Help | Contents' menu item is selected.           //
        // Note that WPF does not have the Help class as per WinForms.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void helpContents_Click(object sender, RoutedEventArgs e)
        {
            string helpFile = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "PCLParaphernalia.chm");

            if (File.Exists(helpFile))
            {
                Process.Start(helpFile);
            }
            else
            {
                MessageBox.Show($"Help file '{helpFile}' does not exist.",
                                "Help file selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a r g e t F i l e S e l e c t _ C l i c k                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Target | File | Select' item is selected.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void targetFileSelect_Click(object sender, RoutedEventArgs e)
        {
            menuItemTargetFile.IsChecked = true;
            menuItemTargetNetPrinter.IsChecked = false;
            menuItemTargetWinPrinter.IsChecked = false;

            TargetCore.MetricsSaveType(TargetCore.Target.File);

            CrntToolResetTarget();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a r g e t F i l e C o n f i g u r e _ C l i c k                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Target | File | Configure' item is selected.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void targetFileConfigure_Click(object sender, RoutedEventArgs e)
        {
            CrntToolResetPDL();

            CrntToolResetSubId();

            TargetFile targetFile = new TargetFile(_crntToolId, _crntSubId, _crntPDL);

            if (targetFile.ShowDialog() == false)
                return;

            menuItemTargetFile.IsChecked = true;
            menuItemTargetNetPrinter.IsChecked = false;
            menuItemTargetWinPrinter.IsChecked = false;

            TargetCore.MetricsSaveType(TargetCore.Target.File);

            CrntToolResetTarget();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a r g e t N e t P r i n t e r S e l e c t _ C l i c k            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Target | Network Printer | Select' item is        //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void targetNetPrinterSelect_Click(object sender, RoutedEventArgs e)
        {
            menuItemTargetFile.IsChecked = false;
            menuItemTargetNetPrinter.IsChecked = true;
            menuItemTargetWinPrinter.IsChecked = false;

            TargetCore.MetricsSaveType(TargetCore.Target.NetPrinter);

            CrntToolResetTarget();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a r g e t N e t P r i n t e r C o n f i g u r e _ C l i c k      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Target | Network Printer | Configure' item is     //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void targetNetPrinterConfigure_Click(object sender, RoutedEventArgs e)
        {
            TargetNetPrintConfig targetNetPrintConfig = new TargetNetPrintConfig();

            if (targetNetPrintConfig.ShowDialog() == false)
                return;

            menuItemTargetFile.IsChecked = false;
            menuItemTargetNetPrinter.IsChecked = true;
            menuItemTargetWinPrinter.IsChecked = false;

            TargetCore.MetricsSaveType(TargetCore.Target.NetPrinter);

            CrntToolResetTarget();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a r g e t R p t F i l e C o n f i g u r e _ C l i c k            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Target | Report File | Configure' item is         //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void targetRptFileConfigure_Click(object sender, RoutedEventArgs e)
        {
            CrntToolResetPDL();

            CrntToolResetSubId();

            TargetRptFile targetRptFile = new TargetRptFile(_crntToolId, _crntSubId, _crntPDL);

            bool? dialogResult = targetRptFile.ShowDialog();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a r g e t W i n P r i n t e r S e l e c t _ C l i c k            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Target | Windows Printer | Select' item is        //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void targetWinPrinterSelect_Click(object sender, RoutedEventArgs e)
        {
            menuItemTargetFile.IsChecked = false;
            menuItemTargetNetPrinter.IsChecked = false;
            menuItemTargetWinPrinter.IsChecked = true;

            TargetCore.MetricsSaveType(TargetCore.Target.WinPrinter);

            CrntToolResetTarget();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a r g e t W i n P r i n t e r C o n f i g u r e _ C l i c k      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Target | Printer | Configure' item is selected.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void targetWinPrinterConfigure_Click(object sender, RoutedEventArgs e)
        {
            TargetWinPrintConfig targetWinPrintConfig = new TargetWinPrintConfig();

            if (targetWinPrintConfig.ShowDialog() == false)
                return;

            menuItemTargetFile.IsChecked = false;
            menuItemTargetNetPrinter.IsChecked = false;
            menuItemTargetWinPrinter.IsChecked = true;

            TargetCore.MetricsSaveType(TargetCore.Target.WinPrinter);

            CrntToolResetTarget();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l F o n t S a m p l e _ S e l e c t e d                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Font Sample' item is selected.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolFontSample_Selected(object sender, RoutedEventArgs e)
        {
            CrntToolSaveMetrics();
            CrntToolUncheckAll();

            menuItemToolFontSample.IsChecked = true;

            _crntToolId = ToolCommonData.ToolIds.FontSample;

            _subFormToolFontSample = new ToolFontSample(ref _crntPDL);

            TargetCore.MetricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolFontSample.Content;

            _subFormToolFontSample.Content = null;
            _subFormToolFontSample.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l F o r m S a m p l e _ S e l e c t e d                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Form Sample' item is selected.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolFormSample_Selected(object sender, RoutedEventArgs e)
        {
            CrntToolSaveMetrics();
            CrntToolUncheckAll();

            menuItemToolFormSample.IsChecked = true;

            _crntToolId = ToolCommonData.ToolIds.FormSample;

            _subFormToolFormSample = new ToolFormSample(ref _crntPDL);

            TargetCore.MetricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolFormSample.Content;

            _subFormToolFormSample.Content = null;
            _subFormToolFormSample.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l I m a g e B i t m a p _ S e l e c t e d                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Image Bitmap' item is selected.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolImageBitmap_Selected(object sender, RoutedEventArgs e)
        {
            CrntToolSaveMetrics();
            CrntToolUncheckAll();

            menuItemToolImageBitmap.IsChecked = true;

            _crntToolId = ToolCommonData.ToolIds.ImageBitmap;

            _subFormToolImageBitmap = new ToolImageBitmap(ref _crntPDL);

            TargetCore.MetricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolImageBitmap.Content;

            _subFormToolImageBitmap.Content = null;
            _subFormToolImageBitmap.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l M a k e O v e r l a y _ S e l e c t e d                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Make Overlay' item is selected.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolMakeOverlay_Selected(object sender, RoutedEventArgs e)
        {
            CrntToolSaveMetrics();
            CrntToolUncheckAll();

            menuItemToolMakeOverlay.IsChecked = true;

            _crntToolId = ToolCommonData.ToolIds.MakeOverlay;

            _subFormToolMakeOverlay = new ToolMakeOverlay(ref _crntPDL);

            TargetCore.MetricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolMakeOverlay.Content;

            _subFormToolMakeOverlay.Content = null;
            _subFormToolMakeOverlay.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l M i s c S a m p l e s _ S e l e c t e d                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Misc Samples' item is selected.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolMiscSamples_Selected(object sender, RoutedEventArgs e)
        {
            CrntToolSaveMetrics();
            CrntToolUncheckAll();

            menuItemToolMiscSamples.IsChecked = true;

            _crntToolId = ToolCommonData.ToolIds.MiscSamples;

            _subFormToolMiscSamples = new ToolMiscSamples(ref _crntPDL, ref _crntSubId);

            TargetCore.MetricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolMiscSamples.Content;

            _subFormToolMiscSamples.Content = null;
            _subFormToolMiscSamples.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        /*
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l P a t t e r n G e n e r a t e _ S e l e c t e d            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Pattern Generate' item is selected.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolPatternGenerate_Selected (
            object sender,
            RoutedEventArgs e)
        {
            crntToolSaveMetrics();
            crntToolUncheckAll();

            menuItemToolPatternGenerate.IsChecked = true;

            _crntToolId = ToolCommonData.eToolIds.PatternGenerate;

            _subFormToolPatternGenerate = new ToolPatternGenerate(ref _crntPDL);

            TargetCore.metricsLoadFile(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolPatternGenerate.Content;

            _subFormToolPatternGenerate.Content = null;
            _subFormToolPatternGenerate.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }
        */
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l P r i n t A r e a _ S e l e c t e d                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Print Area' item is selected.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolPrintArea_Selected(object sender, RoutedEventArgs e)
        {
            CrntToolSaveMetrics();
            CrntToolUncheckAll();

            menuItemToolPrintArea.IsChecked = true;

            _crntToolId = ToolCommonData.ToolIds.PrintArea;

            _subFormToolPrintArea = new ToolPrintArea(ref _crntPDL);

            TargetCore.MetricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolPrintArea.Content;

            _subFormToolPrintArea.Content = null;
            _subFormToolPrintArea.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l P r i n t L a n g _ S e l e c t e d                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Print Languages' item is selected.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolPrintLang_Selected(object sender, RoutedEventArgs e)
        {
            CrntToolSaveMetrics();
            CrntToolUncheckAll();

            menuItemToolPrintLang.IsChecked = true;

            _crntToolId = ToolCommonData.ToolIds.PrintLang;

            _subFormToolPrintLang = new ToolPrintLang(ref _crntPDL);

            TargetCore.MetricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolPrintLang.Content;

            _subFormToolPrintLang.Content = null;
            _subFormToolPrintLang.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l P r n A n a l y s e _ S e l e c t e d                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Prn Analyse' item is selected.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolPrnAnalyse_Selected(object sender, RoutedEventArgs e)
        {
            CrntToolSaveMetrics();
            CrntToolUncheckAll();

            menuItemToolPrnAnalyse.IsChecked = true;

            _crntToolId = ToolCommonData.ToolIds.PrnAnalyse;

            _subFormToolPrnAnalyse = new ToolPrnAnalyse(ref _crntPDL);

            TargetCore.MetricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolPrnAnalyse.Content;

            _subFormToolPrnAnalyse.Content = null;
            _subFormToolPrnAnalyse.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l P r n P r i n t _ S e l e c t e d                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Prn Print' item is selected.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolPrnPrint_Selected(object sender, RoutedEventArgs e)
        {
            CrntToolSaveMetrics();
            CrntToolUncheckAll();

            menuItemToolPrnPrint.IsChecked = true;

            _crntToolId = ToolCommonData.ToolIds.PrnPrint;

            _subFormToolPrnPrint = new ToolPrnPrint(ref _crntPDL);

            TargetCore.MetricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolPrnPrint.Content;

            _subFormToolPrnPrint.Content = null;
            _subFormToolPrnPrint.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l S o f t F o n t G e n e r a t e _ S e l e c t e d          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Soft Font Generate' item is selected.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolSoftFontGenerate_Selected(object sender, RoutedEventArgs e)
        {
            CrntToolSaveMetrics();
            CrntToolUncheckAll();

            menuItemToolSoftFontGenerate.IsChecked = true;

            _crntToolId = ToolCommonData.ToolIds.SoftFontGenerate;

            _subFormToolSoftFontGenerate = new ToolSoftFontGenerate(ref _crntPDL);

            TargetCore.MetricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolSoftFontGenerate.Content;

            _subFormToolSoftFontGenerate.Content = null;
            _subFormToolSoftFontGenerate.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l S t a t u s R e a d b a c k _ S e l e c t e d              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'StatusReadback' item is selected.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolStatusReadback_Selected(object sender, RoutedEventArgs e)
        {
            CrntToolSaveMetrics();
            CrntToolUncheckAll();

            menuItemToolStatusReadback.IsChecked = true;

            _crntToolId = ToolCommonData.ToolIds.StatusReadback;

            _subFormToolStatusReadback = new ToolStatusReadback(ref _crntPDL);

            TargetCore.MetricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolStatusReadback.Content;

            _subFormToolStatusReadback.Content = null;
            _subFormToolStatusReadback.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l S y m b o l S e t G e n e r a t e _ S e l e c t e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Symbol Set Generate' item is selected.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolSymbolSetGenerate_Selected(object sender, RoutedEventArgs e)
        {
            CrntToolSaveMetrics();
            CrntToolUncheckAll();

            menuItemToolSymbolSetGenerate.IsChecked = true;

            _crntToolId = ToolCommonData.ToolIds.SymbolSetGenerate;

            _subFormToolSymbolSetGenerate = new ToolSymbolSetGenerate(ref _crntPDL);

            TargetCore.MetricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolSymbolSetGenerate.Content;

            _subFormToolSymbolSetGenerate.Content = null;
            _subFormToolSymbolSetGenerate.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l T r a y M a p _ S e l e c t e d                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Tray Map' item is selected.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolTrayMap_Selected(object sender, RoutedEventArgs e)
        {
            CrntToolSaveMetrics();
            CrntToolUncheckAll();

            menuItemToolTrayMap.IsChecked = true;

            _crntToolId = ToolCommonData.ToolIds.TrayMap;

            _subFormToolTrayMap = new ToolTrayMap(ref _crntPDL);

            TargetCore.MetricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolTrayMap.Content;

            _subFormToolTrayMap.Content = null;
            _subFormToolTrayMap.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t o o l X X X D i a g s _ S e l e c t e d                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'XXX Diags' item is selected.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void toolXXXDiags_Selected(object sender, RoutedEventArgs e)
        {
            CrntToolSaveMetrics();
            CrntToolUncheckAll();

            menuItemToolTrayMap.IsChecked = true;

            _crntToolId = ToolCommonData.ToolIds.XXXDiags;

            _subFormToolXXXDiags = new ToolXXXDiags(ref _crntPDL);

            TargetCore.MetricsLoadFileCapt(_crntToolId, _crntSubId, _crntPDL);

            object content = _subFormToolXXXDiags.Content;

            _subFormToolXXXDiags.Content = null;
            _subFormToolXXXDiags.Close();

            grid1.Children.Clear();
            grid1.Children.Add(content as UIElement);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // W i n d o w _ C l o s i n g                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store target and window metrics.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //----------------------------------------------------------------//
            //                                                                //
            // Save data from last active subform.                            //
            //                                                                //
            //----------------------------------------------------------------//

            CrntToolSaveMetrics();

            //----------------------------------------------------------------//
            //                                                                //
            // Store current window metrics.                                  //
            //                                                                //
            //----------------------------------------------------------------//

            MainFormPersist.SaveWindowData(
                (int)Left,
                (int)Top,
                (int)Height,
                (int)Width,
                (int)(MainFormData.WindowScale * 100));
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // z o o m S l i d e r _ V a l u e C h a n g e d                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'zoomSlider' object is changed.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MainFormData.WindowScale = zoomSlider.Value;
        }

        /*
        private void Form1_Load(object sender, EventArgs e)
        {
            // set F1 help topic for this form
            helpProvider1.HelpNamespace = Application.StartupPath + @"\" + sHTMLHelpFileName;
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, @"/Garden/garden.htm");
            helpProvider1.SetHelpNavigator(this.btnStart, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this.btnStart, @"/Garden/flowers.htm");
            helpProvider1.SetHelpNavigator(this.btnExit, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this.btnExit, @"/Garden/tree.htm");
            helpProvider1.SetHelpNavigator(this.chkMain, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this.chkMain, @"/HTMLHelp_Examples/jump_to_anchor.htm#AnchorSample");
        }
        */
    }
}