using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// <para>Interaction logic for ToolPrnPrint.xaml</para>
    /// <para>Class handles the Prn Print tool form.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    [System.Reflection.Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public partial class ToolPrnPrint : Window
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private string _prnFilename;

        //   private Boolean _initialised;

        private static Stream _ipStream;
        private static BinaryReader _binReader;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T o o l P R N P r i n t                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolPrnPrint(ref ToolCommonData.PrintLang crntPDL)
        {
            InitializeComponent();

            Initialise();

            crntPDL = ToolCommonData.PrintLang.Unknown;
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

            string filename = _prnFilename;

            selected = SelectPrnFile(ref filename);

            if (selected)
            {
                _prnFilename = filename;
                txtFilename.Text = _prnFilename;
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
            BinaryWriter binWriter = null;

            TargetCore.RequestStreamOpen(
                ref binWriter,
                ToolCommonData.ToolIds.PrnPrint,
                ToolCommonData.ToolSubIds.None,
                ToolCommonData.PrintLang.Unknown);

            CopyPrnFile(_prnFilename, binWriter);

            TargetCore.RequestStreamWrite(false);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c o p y P r n F i l e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Copy print file contents to output stream.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool CopyPrnFile(string prnFilename, BinaryWriter prnWriter)
        {
            bool OK = true;

            bool fileOpen = PrnOpen(prnFilename);

            if (!fileOpen)
            {
                OK = false;
            }
            else
            {
                const int bufSize = 2048;
                int readSize;

                bool endLoop;

                byte[] buf = new byte[bufSize];

                endLoop = false;

                while (!endLoop)
                {
                    readSize = _binReader.Read(buf, 0, bufSize);

                    if (readSize == 0)
                        endLoop = true;
                    else
                        prnWriter.Write(buf, 0, readSize);
                }

                PrnClose();
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g i v e C r n t P D L                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GiveCrntPDL(ref ToolCommonData.PrintLang crntPDL)
        {
            crntPDL = ToolCommonData.PrintLang.Unknown;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialise 'target' data.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void Initialise()
        {
            //  _initialised = false;

            //----------------------------------------------------------------//
            //                                                                //
            // Populate form.                                                 //
            //                                                                //
            //----------------------------------------------------------------//

            ResetTarget();

            //----------------------------------------------------------------//
            //                                                                //
            // Reinstate settings from persistent storage.                    //
            //                                                                //
            //----------------------------------------------------------------//

            MetricsLoad();

            txtFilename.Text = _prnFilename;

            //  _initialised = true;
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
            ToolPrnPrintPersist.LoadDataGeneral(out _prnFilename);
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
            ToolPrnPrintPersist.SaveDataGeneral(_prnFilename);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r n C l o s e                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close stream and file.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void PrnClose()
        {
            _binReader.Close();
            _ipStream.Close();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p r n O p e n                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open read stream for specified print file.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool PrnOpen(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Print file name is null.",
                                "Print File Selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            if (!File.Exists(fileName))
            {
                MessageBox.Show($"Print file '{fileName}' does not exist.",
                                "Print File Selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            try
            {
                _ipStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException e)
            {
                MessageBox.Show($"IO Exception:\r\n{e.Message}\r\nOpening print file '{fileName}'.",
                                "Print File Selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            if (_ipStream == null)
                return false;

            _binReader = new BinaryReader(_ipStream);

            return true;
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
                btnGenerate.Content = "Copy PRN file contents to file";
            }
            else if (targetType == TargetCore.Target.NetPrinter)
            {
                TargetCore.MetricsLoadNetPrinter(out string netPrnAddress,
                                                  out int netPrnPort,
                                                  out _,
                                                  out _);

                btnGenerate.Content = $"Send PRN file contents to\n{netPrnAddress}: {netPrnPort}";
            }
            else if (targetType == TargetCore.Target.WinPrinter)
            {
                string winPrintername = string.Empty;

                TargetCore.MetricsLoadWinPrinter(out winPrintername);

                btnGenerate.Content = $"Send PRN file contents to printer\n{winPrintername}";
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e l e c t P r n F i l e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initiate 'open file' dialogue.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool SelectPrnFile(ref string prnFilename)
        {
            OpenFileDialog openDialog = ToolCommonFunctions.CreateOpenFileDialog(prnFilename);

            openDialog.Filter = "Print Files|*.prn; *.pcl; *.dia" +
                                "|All Files|*.*";

            if (openDialog.ShowDialog() == false)
                return false;

            prnFilename = openDialog.FileName;

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t F i l e n a m e _ L o s t F o c u s                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Image filename text has lost focus.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtFilename_LostFocus(object sender, RoutedEventArgs e)
        {
            _prnFilename = txtFilename.Text;
        }
    }
}