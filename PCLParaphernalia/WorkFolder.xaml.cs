﻿using System;
using System.Windows;
using System.Windows.Forms;

namespace PCLParaphernalia
{
    /// <summary>
    /// Interaction logic for WorkFolder.xaml
    /// 
    /// Class handles the working folder definition form.
    /// 
    /// © Chris Hutchinson 2014
    /// 
    /// </summary>

    [System.Reflection.Obfuscation(Feature = "renaming",
                                            ApplyToMembers = true)]

    public partial class WorkFolder : Window
    {
        readonly string _tmpFolder = Environment.GetEnvironmentVariable("TMP");

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private string _workFoldername;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // W o r k F o l d e r                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public WorkFolder()
        {
            InitializeComponent();

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
        // b t n F o l d e r n a m e B r o w s e _ C l i c k                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Browse' button is clicked.                        //
        // Invoke 'Save As' dialogue to select target folder.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnFoldernameBrowse_Click(object sender, RoutedEventArgs e)
        {
            bool selected;

            string filename = _workFoldername;

            selected = SelectDefWorkFolder(ref filename);

            if (selected)
            {
                _workFoldername = filename;
                txtFoldername.Text = _workFoldername;
            }
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
        // i n i t i a l i s e                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialise 'target' data.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void Initialise()
        {
            _workFoldername = _tmpFolder;

            txtFoldername.Text = _workFoldername;

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
            Width = 720 * windowScale;
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
            TargetPersist.SaveDataWorkFolder(_workFoldername);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e l e c t D e f W o r k F o l d e r                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Browse' button is clicked.                        //
        // Invoke the (Windows Forms) 'folder browser' dialogue to select the //
        // default working folder.                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool SelectDefWorkFolder(ref string targetFolder)
        {
            bool selected = false;

            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            folderDialog.SelectedPath = _tmpFolder;

            DialogResult dlgResult = folderDialog.ShowDialog();

            if (dlgResult == System.Windows.Forms.DialogResult.OK)
            {
                selected = true;
                targetFolder = folderDialog.SelectedPath;
            }
            else
            {
                selected = false;
            }

            return selected;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t F o l d e r n a m e _ Lo s t F o c u s                       //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the target 'Foldername' text has lost focus.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtFoldername_LostFocus(object sender,
                                            RoutedEventArgs e)
        {
            _workFoldername = txtFoldername.Text;
        }
    }
}
