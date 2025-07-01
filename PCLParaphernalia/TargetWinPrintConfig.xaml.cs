using System.Drawing.Printing;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// Interaction logic for TargetWinPrintConfig.xaml
    /// 
    /// Class handles the Target (printer) definition form.
    /// 
    /// © Chris Hutchinson 2014
    /// 
    /// </summary>

    [System.Reflection.Obfuscation(Feature = "renaming",
                                            ApplyToMembers = true)]

    public partial class TargetWinPrintConfig : Window
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly bool _initialised;

        private string _printerName;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T a r g e t W i n P r i n t e r                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public TargetWinPrintConfig()
        {
            _initialised = false;

            InitializeComponent();

            initialise();

            _initialised = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n C a n c e l _ C l i c k                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Cancel' button is clicked.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n O K _ C l i c k                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'OK' button is clicked.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            metricsSave();

            DialogResult = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b P r i n t e r s _ S e l e c t i o n C h a n g e d              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Printers item has changed.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbPrinters_SelectionChanged(object sender,
                                                  System.EventArgs e)
        {
            if (_initialised)
            {
                if (cbPrinters.SelectedIndex != -1)
                {
                    _printerName = cbPrinters.SelectedItem.ToString();
                }
            }
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
            int indxPrinter = 0;
            int ctPrinters = 0;

            string printerName;

            //----------------------------------------------------------------//
            //                                                                //
            // Populate form.                                                 //
            //                                                                //
            //----------------------------------------------------------------//

            TargetCore.metricsLoadWinPrinter(ref _printerName);

            cbPrinters.Items.Clear();

            ctPrinters = PrinterSettings.InstalledPrinters.Count;

            for (int i = 0; i < ctPrinters; i++)
            {
                printerName = PrinterSettings.InstalledPrinters[i];

                cbPrinters.Items.Add(printerName);

                if (printerName == _printerName)
                    indxPrinter = i;
            }

            //----------------------------------------------------------------//

            if ((indxPrinter < 0) || (indxPrinter >= ctPrinters))
                indxPrinter = 0;

            cbPrinters.SelectedIndex = indxPrinter;
            _printerName = cbPrinters.Text;

            //----------------------------------------------------------------//
            //                                                                //
            // Set the (hidden) slider object to the passed-in scale value.   //
            // The slider is used as the source binding for a scale           //
            // transform in the (child) Options dialogue window, so that all  //
            // windows use the same scaling mechanism as the main window.     //
            //                                                                //
            // NOTE: it would be better to bind the transform directly to the //
            //       scale value (set and stored in the Main window), but (so //
            //       far) I've failed to find a way to bind directly to a     //
            //       class object Property value.                             //
            //                                                                //
            //----------------------------------------------------------------//

            double windowScale = MainFormData.WindowScale;

            zoomSlider.Value = windowScale;

            //----------------------------------------------------------------//
            //                                                                //
            // Setting sizes to the resizeable DockPanel element doesn't work!//
            //                                                                //
            //----------------------------------------------------------------//

            Height = 240 * windowScale;
            Width = 440 * windowScale;

            // Double h = resizeable.Height;
            // Double w = resizeable.Width;

            // this.Height = h;
            // this.Width = w;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s S a v e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Save the current settings.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void metricsSave()
        {
            TargetCore.metricsSaveWinPrinter(_printerName);
        }
    }
}
