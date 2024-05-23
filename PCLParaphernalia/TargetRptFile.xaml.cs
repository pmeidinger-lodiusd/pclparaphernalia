using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// <para>Interaction logic for TargetRptFile.xaml</para>
    /// <para>Class handles the Target (report file) definition form.</para>
    /// <para>© Chris Hutchinson 2017</para>
    ///
    /// </summary>
    [System.Reflection.Obfuscation(Feature = "renaming", ApplyToMembers = true)]

    public partial class TargetRptFile : Window
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly ToolCommonData.ToolIds _crntToolId;
        private readonly ToolCommonData.ToolSubIds _crntSubId;
        private readonly ToolCommonData.PrintLang _crntPDL;
        private ReportCore.RptFileFmt _rptFileFmt;
        private ReportCore.RptChkMarks _rptChkMarks;

        private bool _flagOptRptWrap;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T a r g e t R p t F i l e                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public TargetRptFile(ToolCommonData.ToolIds crntToolId,
                              ToolCommonData.ToolSubIds crntSubId,
                              ToolCommonData.PrintLang crntPDL)
        {
            InitializeComponent();

            _crntToolId = crntToolId;
            _crntSubId = crntSubId;
            _crntPDL = crntPDL;

            initialise();
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
        // c h k R p t O p t W r a p _ C h e c k e d                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the Report file 'wrap' checkbox is checked.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkRptOptWrap_Checked(object sender, RoutedEventArgs e)
        {
            _flagOptRptWrap = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k R p t O p t W r a p _ U n c h e c k e d                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the Report file 'wrap' checkbox is unchecked.          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkRptOptWrap_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagOptRptWrap = false;
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
            btnOK.Visibility = Visibility.Hidden;

            //----------------------------------------------------------------//
            //                                                                //
            // Tool and PDL identifiers.                                      //
            //                                                                //
            //----------------------------------------------------------------//

            if (_crntSubId == ToolCommonData.ToolSubIds.None)
            {
                txtCrntTool.Text =
                    Enum.GetName(typeof(ToolCommonData.ToolIds),
                                  _crntToolId);
            }
            else
            {
                txtCrntTool.Text =
                    Enum.GetName(typeof(ToolCommonData.ToolIds),
                                  _crntToolId) +
                    "|" +
                    Enum.GetName(typeof(ToolCommonData.ToolSubIds),
                                  _crntSubId);
            }

            txtCrntPDL.Text = _crntPDL.ToString();

            //----------------------------------------------------------------//
            //                                                                //
            // Report file data.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            if ((_crntToolId == ToolCommonData.ToolIds.MakeOverlay)
                                          ||
                (_crntToolId == ToolCommonData.ToolIds.PrintLang)
                                          ||
                (_crntToolId == ToolCommonData.ToolIds.PrnAnalyse)
                                          ||
                (_crntToolId == ToolCommonData.ToolIds.SoftFontGenerate)
                                          ||
                (_crntToolId == ToolCommonData.ToolIds.StatusReadback)
                                          ||
                (_crntToolId == ToolCommonData.ToolIds.SymbolSetGenerate))
            {
                grpRptFile.Visibility = Visibility.Visible;
                lbFileNA.Visibility = Visibility.Hidden;
                btnOK.Visibility = Visibility.Visible;

                TargetCore.MetricsReturnFileRpt(_crntToolId,
                                                 ref _rptFileFmt,
                                                 ref _rptChkMarks,
                                                 ref _flagOptRptWrap);

                if (_rptFileFmt == ReportCore.RptFileFmt.html)
                    rbRptFmtHtml.IsChecked = true;
                else if (_rptFileFmt == ReportCore.RptFileFmt.xml)
                    rbRptFmtXml.IsChecked = true;
                else
                    rbRptFmtText.IsChecked = true;

                grpRptChkMarks.Visibility = Visibility.Hidden;
                grpRptOpt.Visibility = Visibility.Hidden;

                if (_crntToolId == ToolCommonData.ToolIds.PrintLang)
                {
                    grpRptChkMarks.Visibility = Visibility.Visible;
                    grpRptOpt.Visibility = Visibility.Visible;

                    if (_rptChkMarks >= ReportCore.RptChkMarks.NA)
                        _rptChkMarks = ReportCore.RptChkMarks.text;

                    if (_rptChkMarks == ReportCore.RptChkMarks.boxsym)
                        rbRptChkMarksBoxSym.IsChecked = true;
                    else if (_rptChkMarks == ReportCore.RptChkMarks.txtsym)
                        rbRptChkMarksTxtSym.IsChecked = true;
                    else
                        rbRptChkMarksText.IsChecked = true;

                    chkRptOptWrap.IsChecked = _flagOptRptWrap;
                }
                else if (_crntToolId == ToolCommonData.ToolIds.SoftFontGenerate)
                {
                    grpRptChkMarks.Visibility = Visibility.Visible;

                    if (_rptChkMarks >= ReportCore.RptChkMarks.NA)
                        _rptChkMarks = ReportCore.RptChkMarks.text;

                    if (_rptChkMarks == ReportCore.RptChkMarks.boxsym)
                        rbRptChkMarksBoxSym.IsChecked = true;
                    else if (_rptChkMarks == ReportCore.RptChkMarks.txtsym)
                        rbRptChkMarksTxtSym.IsChecked = true;
                    else
                        rbRptChkMarksText.IsChecked = true;
                }
            }
            else
            {
                grpRptFile.Visibility = Visibility.Hidden;
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

            Height = 350 * windowScale;
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
            TargetCore.MetricsSaveFileRpt(_crntToolId, _rptFileFmt, _rptChkMarks, _flagOptRptWrap);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b R p t F m t H t m l _ C l i c k                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the report format 'html' radio button is selected.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void RbRptFmtHtml_Click(object sender, RoutedEventArgs e)
        {
            _rptFileFmt = ReportCore.RptFileFmt.html;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b R p t F m t T e x t _ C l i c k                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the report format 'text' radio button is selected.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbRptFmtText_Click(object sender, RoutedEventArgs e)
        {
            _rptFileFmt = ReportCore.RptFileFmt.text;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b R p t F m t X m l _ C l i c k                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the report format 'xml' radio button is selected.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbRptFmtXml_Click(object sender, RoutedEventArgs e)
        {
            _rptFileFmt = ReportCore.RptFileFmt.xml;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b R p t C h k M a r k s B o x S y m _ C l i c k                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the report option 'true/false' as 'ballot box' radio   //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbRptChkMarksBoxSym_Click(object sender, RoutedEventArgs e)
        {
            _rptChkMarks = ReportCore.RptChkMarks.boxsym;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b R p t C h k M a r k s T e x t _ C l i c k                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the report option 'true/false' as 'text' radio         //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbRptChkMarksText_Click(object sender, RoutedEventArgs e)
        {
            _rptChkMarks = ReportCore.RptChkMarks.text;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b R p t C h k M a r k s T x t S y m _ C l i c k                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the report option 'true/false' as 'plus/minus' radio   //
        // button is selected.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbRptChkMarksTxtSym_Click(object sender, RoutedEventArgs e)
        {
            _rptChkMarks = ReportCore.RptChkMarks.txtsym;
        }
    }
}
