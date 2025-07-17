using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace PCLParaphernalia;

/// <summary>
/// Interaction logic for ToolPrnPrint.xaml
/// 
/// Class handles the Prn Print tool form.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

[System.Reflection.Obfuscation(Feature = "renaming",
                                        ApplyToMembers = true)]

public partial class ToolPrnPrint : Window
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private string _prnFilename;

    //   private Boolean _initialised;

    private static Stream _ipStream = null;
    private static BinaryReader _binReader = null;

    //--------------------------------------------------------------------//
    //                                              C o n s t r u c t o r //
    // T o o l P R N P r i n t                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public ToolPrnPrint(ref ToolCommonData.ePrintLang crntPDL)
    {
        InitializeComponent();

        Initialise();

        crntPDL = ToolCommonData.ePrintLang.Unknown;
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
            ToolCommonData.eToolIds.PrnPrint,
            ToolCommonData.eToolSubIds.None,
            ToolCommonData.ePrintLang.Unknown);

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

    public static bool CopyPrnFile(string prnFilename,
                                      BinaryWriter prnWriter)
    {
        bool OK = true;

        bool fileOpen = false;

        fileOpen = PrnOpen(prnFilename);

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

    public void GiveCrntPDL(ref ToolCommonData.ePrintLang crntPDL)
    {
        crntPDL = ToolCommonData.ePrintLang.Unknown;
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
        ToolPrnPrintPersist.LoadDataGeneral(ref _prnFilename);
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
        bool open = false;

        if (string.IsNullOrEmpty(fileName))
        {
            MessageBox.Show("Print file name is null.",
                            "Print file selection",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

            return false;
        }
        else if (!File.Exists(fileName))
        {
            MessageBox.Show("Print file '" + fileName +
                            "' does not exist.",
                            "Print file selection",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

            return false;
        }
        else
        {
            _ipStream = File.Open(fileName,
                                  FileMode.Open,
                                  FileAccess.Read,
                                  FileShare.None);

            if (_ipStream != null)
            {
                open = true;

                _binReader = new BinaryReader(_ipStream);
            }
        }

        return open;
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
        TargetCore.eTarget targetType = TargetCore.GetType();

        if (targetType == TargetCore.eTarget.File)
        {
            btnGenerate.Content = "Copy PRN file contents to file";
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

            btnGenerate.Content = "Send PRN file contents to " +
                                  "\r\n" +
                                  netPrnAddress + " : " +
                                  netPrnPort.ToString();
        }
        else if (targetType == TargetCore.eTarget.WinPrinter)
        {
            string winPrintername = string.Empty;

            TargetCore.MetricsLoadWinPrinter(ref winPrintername);

            btnGenerate.Content = "Send PRN file contents to printer " +
                                  "\r\n" +
                                  winPrintername;
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

        openDialog.Filter = "Print Files|" +
                            "*.prn; *.pcl; *.dia;" +
                            "*.PRN; *.PCL; *.DIA" +
                            "|All files|" +
                            "*.*";

        bool? dialogResult = openDialog.ShowDialog();

        if (dialogResult == true)
            prnFilename = openDialog.FileName;

        return dialogResult == true;
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
        _prnFilename = txtFilename.Text;
    }
}
