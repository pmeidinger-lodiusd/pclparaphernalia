﻿using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // A p p _ S t a r t u p                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Application startup: process command line arguments (if any), then //
        // create and start main window.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void App_Startup(object sender, StartupEventArgs e)
        {
            string filename = string.Empty;

            int argCt = e.Args.Length;

            if (argCt != 0)
            {
                filename = e.Args[0];
            }

            MainForm mainForm = new MainForm(filename);

            mainForm.Show();
        }
    }
}