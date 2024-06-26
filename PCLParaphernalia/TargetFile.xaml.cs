﻿using Microsoft.Win32;
using System;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// <para>Interaction logic for TargetFile.xaml</para>
    /// <para>Class handles the Target (file) definition form.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    [System.Reflection.Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public partial class TargetFile : Window
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private string _saveFilename;
        private readonly ToolCommonData.ToolIds _crntToolId;
        private readonly ToolCommonData.ToolSubIds _crntSubId;
        private readonly ToolCommonData.PrintLang _crntPDL;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T a r g e t F i l e                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public TargetFile(ToolCommonData.ToolIds crntToolId,
                           ToolCommonData.ToolSubIds crntSubId,
                           ToolCommonData.PrintLang crntPDL)
        {
            InitializeComponent();

            _crntToolId = crntToolId;
            _crntSubId = crntSubId;
            _crntPDL = crntPDL;

            Initialise();
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
            MetricsSave();

            DialogResult = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n O p F i l e n a m e B r o w s e _ C l i c k                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the output file 'Browse' button is clicked.            //
        // Invoke 'Save As' dialogue to select target file.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnOpFilenameBrowse_Click(object sender, RoutedEventArgs e)
        {
            string filename = _saveFilename;

            bool selected = SelectTargetFile(ref filename);

            if (selected)
            {
                _saveFilename = filename;
                txtOpFilename.Text = _saveFilename;
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

        private void Initialise()
        {
            btnOK.Visibility = Visibility.Hidden;

            //----------------------------------------------------------------//
            //                                                                //
            // Tool and PDL identifiers.                                      //
            //                                                                //
            //----------------------------------------------------------------//

            if (_crntSubId == ToolCommonData.ToolSubIds.None)
            {
                txtCrntTool.Text = Enum.GetName(typeof(ToolCommonData.ToolIds), _crntToolId);
            }
            else
            {
                txtCrntTool.Text = Enum.GetName(typeof(ToolCommonData.ToolIds), _crntToolId) +
                                    "|" +
                                    Enum.GetName(typeof(ToolCommonData.ToolSubIds), _crntSubId);
            }

            txtCrntPDL.Text = _crntPDL.ToString();

            //----------------------------------------------------------------//
            //                                                                //
            // Output file data.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            if ((_crntToolId == ToolCommonData.ToolIds.FontSample)
                                          ||
                (_crntToolId == ToolCommonData.ToolIds.FormSample)
                                          ||
                (_crntToolId == ToolCommonData.ToolIds.ImageBitmap)
                                          ||
                (_crntToolId == ToolCommonData.ToolIds.MiscSamples)
                                          ||
                (_crntToolId == ToolCommonData.ToolIds.PrintArea)
                                          ||
                (_crntToolId == ToolCommonData.ToolIds.PrnPrint)
                                          ||
                (_crntToolId == ToolCommonData.ToolIds.StatusReadback)
                                          ||
                (_crntToolId == ToolCommonData.ToolIds.TrayMap))
            {
                grpOpFile.Visibility = Visibility.Visible;
                lbFileNA.Visibility = Visibility.Hidden;
                btnOK.Visibility = Visibility.Visible;

                TargetCore.MetricsReturnFileCapt(_crntToolId, _crntSubId, _crntPDL, out _saveFilename);

                txtOpFilename.Text = _saveFilename;
            }
            else
            {
                grpOpFile.Visibility = Visibility.Hidden;
                lbFileNA.Visibility = Visibility.Visible;
            }

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

            Height = 300 * windowScale;
            Width = 730 * windowScale;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s S a v e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Save the current settings.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void MetricsSave()
        {
            TargetCore.MetricsSaveFileCapt(_crntToolId, _crntSubId, _crntPDL, _saveFilename);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e l e c t T a r g e t F i l e                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Browse' button is clicked.                        //
        // Invoke 'Save As' dialogue to select target file.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool SelectTargetFile(ref string targetFile)
        {
            SaveFileDialog saveDialog = ToolCommonFunctions.CreateSaveFileDialog(targetFile);

            if (saveDialog.ShowDialog() == false)
                return false;

            targetFile = saveDialog.FileName;

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t O p F i l e n a m e _ Lo s t F o c u s                       //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the target output 'Filename' text has lost focus.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtOpFilename_LostFocus(object sender, RoutedEventArgs e)
        {
            _saveFilename = txtOpFilename.Text;
        }
    }
}