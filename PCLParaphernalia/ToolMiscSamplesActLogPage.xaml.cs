﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// Interaction logic for ToolMiscSamples.xaml
    /// 
    /// Class handles the MiscSamples: Define Logical Page tool form.
    /// 
    /// © Chris Hutchinson 2015
    /// 
    /// </summary>

    [System.Reflection.Obfuscation(Feature = "renaming",
                                            ApplyToMembers = true)]

    public partial class ToolMiscSamples : Window
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const int _defLogPageOffLeftDPt = 170;
        const int _defLogPageOffTopDPt = 0;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private int _logPageOffLeftDPt;
        private int _logPageOffTopDPt;

        private int _logPageWidthDPt;
        private int _logPageHeightDPt;

        private bool _flagLogPageOptStdPagePCL;

        private bool _flagLogPageFormAsMacroPCL;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b t n G e n e r a t e _ C l i c k                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Reset logical page' button is clicked.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void btnLogPageReset_Click(object sender, EventArgs e)
        {
            SetPaperMetricsLogPage();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k L o g P a g e O p t S t d P a g e _ C h e c k e d            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Option 'Add standard page' checked.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkLogPageOptStdPage_Checked(object sender,
                                                RoutedEventArgs e)
        {
            _flagLogPageOptStdPagePCL = true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k L o g P a g e O p t S t d P a g e _ U n c h e c k e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Option 'Add standard page' unchecked.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkLogPageOptStdPage_Unchecked(object sender,
                                                RoutedEventArgs e)
        {
            _flagLogPageOptStdPagePCL = false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e D a t a L o g P a g e                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialise 'Define logical page' data.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void InitialiseDataLogPage()
        {
            lbOrientation.Visibility = Visibility.Visible;
            cbOrientation.Visibility = Visibility.Visible;

            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                txtLogPageDesc.Text =
                    "The standard PCL logical page is smaller than the" +
                    " physical page of the selected page size.\r\n" +
                    "Some LaserJet printers support a 'Define Logical Page'" +
                    " escape sequence via which the size and position of the" +
                    " logical page can be specified (although this doesn't" +
                    " mean that marks can be made within the unprintable" +
                    " regions of the physical page).";

                SetPaperMetricsLogPage();

                grpLogPagePhysical.Visibility = Visibility.Visible;
                grpLogPageLogical.Visibility = Visibility.Visible;
                grpLogPageOptions.Visibility = Visibility.Visible;

                chkOptFormAsMacro.IsChecked = _flagLogPageFormAsMacroPCL;
            }
            else
            {
                txtLogPageDesc.Text =
                    "PCL XL can address all points on the physical page" +
                    " (although there are still unprintable regions all" +
                    " around the page), so there is no need for the" +
                    " equivalent of the PCL logical page.";

                grpLogPagePhysical.Visibility = Visibility.Hidden;
                grpLogPageLogical.Visibility = Visibility.Hidden;
                grpLogPageOptions.Visibility = Visibility.Hidden;

                btnGenerate.IsEnabled = false;

                chkOptFormAsMacro.IsChecked = false;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d D a t a L o g P a g e                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load current metrics from persistent storage.                      //
        // Only relevant to PCL.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void MetricsLoadDataLogPage()
        {
            ToolMiscSamplesPersist.LoadDataTypeLogPage(
                "PCL",
                ref _logPageOffLeftDPt,
                ref _logPageOffTopDPt,
                ref _logPageHeightDPt,
                ref _logPageWidthDPt,
                ref _flagLogPageFormAsMacroPCL,
                ref _flagLogPageOptStdPagePCL);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s S a v e D a t a L o g P a g e                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Save current metrics to persistent storage.                        //
        // Only relevant to PCL.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void MetricsSaveDataLogPage()
        {
            ToolMiscSamplesPersist.SaveDataTypeLogPage("PCL",
                                             _logPageOffLeftDPt,
                                             _logPageOffTopDPt,
                                             _logPageHeightDPt,
                                             _logPageWidthDPt,
                                             _flagLogPageFormAsMacroPCL,
                                             _flagLogPageOptStdPagePCL);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F l a g L o g P a g e F o r m A s M a c r o                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set or unset 'Render fixed text as overlay' flag.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetFlagLogPageFormAsMacro(
            bool setFlag,
            ToolCommonData.ePrintLang crntPDL)
        {
            if (crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                _flagLogPageFormAsMacroPCL = setFlag;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t P a p e r M e t r i c s L o g P a g e                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set the contents of the Paper metrics fields.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetPaperMetricsLogPage()
        {
            //----------------------------------------------------------------//
            //                                                                //
            // Display the physical paper data.                               //
            //                                                                //
            //----------------------------------------------------------------//

            txtLogPagePaperWidthDPt.Text =
                (_paperWidthPhysical * _unitsToDPts).ToString("F0");
            txtLogPagePaperHeightDPt.Text =
                (_paperLengthPhysical * _unitsToDPts).ToString("F0");

            txtLogPagePaperWidthMet.Text
                = (_paperWidthPhysical * _unitsToMilliMetres).ToString("F1");
            txtLogPagePaperHeightMet.Text
                = (_paperLengthPhysical * _unitsToMilliMetres).ToString("F1");

            txtLogPagePaperWidthImp.Text
                = (_paperWidthPhysical * _unitsToInches).ToString("F2");
            txtLogPagePaperHeightImp.Text
                = (_paperLengthPhysical * _unitsToInches).ToString("F2");

            txtLogPageUnprintableDPt.Text =
                (_paperMarginsUnprintable * _unitsToDPts).ToString("F0");
            txtLogPageUnprintableMet.Text =
                (_paperMarginsUnprintable * _unitsToMilliMetres).ToString("F1");
            txtLogPageUnprintableImp.Text =
                (_paperMarginsUnprintable * _unitsToInches).ToString("F2");

            //----------------------------------------------------------------//
            //                                                                //
            // Display the (default) logical page offset values appropriate   //
            // to the selected paper size.                                    //
            //                                                                //
            //----------------------------------------------------------------//

            txtLogPageOffLeftDPt.Text =
                (_paperMarginsLogicalLeft * _unitsToDPts).ToString("F0");

            txtLogPageOffTopDPt.Text =
                (_paperMarginsLogicalTop * _unitsToDPts).ToString("F0");

            txtLogPageOffLeftMet.Text =
                (_paperMarginsLogicalLeft * _unitsToMilliMetres).ToString("F1");

            txtLogPageOffTopMet.Text =
                (_paperMarginsLogicalTop * _unitsToMilliMetres).ToString("F1");

            txtLogPageOffLeftImp.Text =
                (_paperMarginsLogicalLeft * _unitsToInches).ToString("F2");

            txtLogPageOffTopImp.Text =
                (_paperMarginsLogicalTop * _unitsToInches).ToString("F2");

            //----------------------------------------------------------------//
            //                                                                //
            // Display the (default) logical page size values appropriate     //
            // to the selected paper size.                                    //
            //                                                                //
            //----------------------------------------------------------------//

            txtLogPageHeightDPt.Text =
                (_paperLengthLogical * _unitsToDPts).ToString("F0");

            txtLogPageWidthDPt.Text =
                (_paperWidthLogical * _unitsToDPts).ToString("F0");

            txtLogPageHeightMet.Text =
                (_paperLengthLogical * _unitsToMilliMetres).ToString("F1");

            txtLogPageWidthMet.Text =
                (_paperWidthLogical * _unitsToMilliMetres).ToString("F1");

            txtLogPageHeightImp.Text =
                (_paperLengthLogical * _unitsToInches).ToString("F2");

            txtLogPageWidthImp.Text =
                (_paperWidthLogical * _unitsToInches).ToString("F2");
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t L o g P a g e O f f L e f t D P t _ L o s t F o c u s        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Logical Offset Left DeciPoints item has lost focus.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtLogPageOffLeftDPt_LostFocus(object sender,
                                             RoutedEventArgs e)
        {
            if (ValidateLogPageOffLeftDPt(true))
            {
                txtLogPageOffLeftMet.Text =
                    (_logPageOffLeftDPt * _dPtsToMilliMetres).ToString("F1");

                txtLogPageOffLeftImp.Text =
                    (_logPageOffLeftDPt * _dPtsToInches).ToString("F2");
            }
            else
            {
                txtLogPageOffLeftMet.Text = string.Empty;
                txtLogPageOffLeftImp.Text = string.Empty;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t L o g P a g e O f f L e f t D P t _ T e x t C h a n g e d    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Logical Offset Left DeciPoints item has changed.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtLogPageOffLeftDPt_TextChanged(object sender,
                                               TextChangedEventArgs e)
        {
            if (ValidateLogPageOffLeftDPt(false))
            {
                txtLogPageOffLeftMet.Text =
                    (_logPageOffLeftDPt * _dPtsToMilliMetres).ToString("F1");

                txtLogPageOffLeftImp.Text =
                    (_logPageOffLeftDPt * _dPtsToInches).ToString("F2");
            }
            else
            {
                txtLogPageOffLeftMet.Text = string.Empty;
                txtLogPageOffLeftImp.Text = string.Empty;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t L o g P a g e O f f T o p D P t _ L o s t F o c u s          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Logical Offset Top DeciPoints item has lost focus.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtLogPageOffTopDPt_LostFocus(object sender,
                                             RoutedEventArgs e)
        {
            if (ValidateLogPageOffTopDPt(true))
            {
                txtLogPageOffTopMet.Text =
                    (_logPageOffTopDPt * _dPtsToMilliMetres).ToString("F1");

                txtLogPageOffTopImp.Text =
                    (_logPageOffTopDPt * _dPtsToInches).ToString("F2");
            }
            else
            {
                txtLogPageOffTopMet.Text = string.Empty;
                txtLogPageOffTopImp.Text = string.Empty;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t L o g P a g e O f f T o p D P t _ T e x t C h a n g e d      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Logical Offset Top DeciPoints item has changed.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtLogPageOffTopDPt_TextChanged(object sender,
                                               TextChangedEventArgs e)
        {
            if (ValidateLogPageOffTopDPt(false))
            {
                txtLogPageOffTopMet.Text =
                    (_logPageOffTopDPt * _dPtsToMilliMetres).ToString("F1");

                txtLogPageOffTopImp.Text =
                    (_logPageOffTopDPt * _dPtsToInches).ToString("F2");
            }
            else
            {
                txtLogPageOffTopMet.Text = string.Empty;
                txtLogPageOffTopImp.Text = string.Empty;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t L o g P a g e H e i g h t D P t _ L o s t F o c u s          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Logical Page Height DeciPoints item has lost focus.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtLogPageHeightDPt_LostFocus(object sender,
                                             RoutedEventArgs e)
        {
            if (ValidateLogPageHeightDPt(true))
            {
                txtLogPageHeightMet.Text =
                    (_logPageHeightDPt * _dPtsToMilliMetres).ToString("F1");

                txtLogPageHeightImp.Text =
                    (_logPageHeightDPt * _dPtsToInches).ToString("F2");
            }
            else
            {
                txtLogPageHeightMet.Text = string.Empty;
                txtLogPageHeightImp.Text = string.Empty;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t L o g P a g e H e i g h t D P t _ T e x t C h a n g e d      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Logical Page Height DeciPoints item has changed.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtLogPageHeightDPt_TextChanged(object sender,
                                               TextChangedEventArgs e)
        {
            if (ValidateLogPageHeightDPt(false))
            {
                txtLogPageHeightMet.Text =
                    (_logPageHeightDPt * _dPtsToMilliMetres).ToString("F1");

                txtLogPageHeightImp.Text =
                    (_logPageHeightDPt * _dPtsToInches).ToString("F2");
            }
            else
            {
                txtLogPageHeightMet.Text = string.Empty;
                txtLogPageHeightImp.Text = string.Empty;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t L o g P a g e W i d t h D P t _ L o s t F o c u s            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Logical Page Width DeciPoints item has lost focus.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtLogPageWidthDPt_LostFocus(object sender,
                                             RoutedEventArgs e)
        {
            if (ValidateLogPageWidthDPt(true))
            {
                txtLogPageWidthMet.Text =
                    (_logPageWidthDPt * _dPtsToMilliMetres).ToString("F1");

                txtLogPageWidthImp.Text =
                    (_logPageWidthDPt * _dPtsToInches).ToString("F2");
            }
            else
            {
                txtLogPageWidthMet.Text = string.Empty;
                txtLogPageWidthImp.Text = string.Empty;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t x t L o g P a g e W i d t h D P t _ T e x t C h a n g e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Logical Page Width DeciPoints item has changed.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void txtLogPageWidthDPt_TextChanged(object sender,
                                               TextChangedEventArgs e)
        {
            if (ValidateLogPageWidthDPt(false))
            {
                txtLogPageWidthMet.Text =
                    (_logPageWidthDPt * _dPtsToMilliMetres).ToString("F1");

                txtLogPageWidthImp.Text =
                    (_logPageWidthDPt * _dPtsToInches).ToString("F2");
            }
            else
            {
                txtLogPageWidthMet.Text = string.Empty;
                txtLogPageWidthImp.Text = string.Empty;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e L o g P a g e O f f L e f t D P t                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate logical offset left decipoints value.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidateLogPageOffLeftDPt(bool lostFocusEvent)
        {
            const int minVal = -32767;
            const int maxVal = 32767;
            const int defVal = _defLogPageOffLeftDPt;

            int value = 0;

            bool OK = true;

            string crntText = txtLogPageOffLeftDPt.Text;

            if (crntText == string.Empty)
            {
                value = 0;
            }
            else
            {
                OK = int.TryParse(crntText, out value);

                if ((value < minVal) || (value > maxVal))
                    OK = false;
            }

            if (OK)
            {
                _logPageOffLeftDPt = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString("F2");

                    MessageBox.Show("Left offset value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    newText + "'",
                                    "Logical page attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    _logPageOffLeftDPt = defVal;

                    txtLogPageOffLeftDPt.Text = newText;
                }
                else
                {
                    MessageBox.Show("Left offset value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Valid range is :\n\t" +
                                    minVal + " <= value <= " + maxVal,
                                    "Logical Page attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtLogPageOffLeftDPt.Focus();
                    txtLogPageOffLeftDPt.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e L o g P a g e O f f T o p D P t                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate logical offset top decipoints value.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidateLogPageOffTopDPt(bool lostFocusEvent)
        {
            const int minVal = -32767;
            const int maxVal = 32767;
            const int defVal = _defLogPageOffTopDPt;

            int value = 0;

            bool OK = true;

            string crntText = txtLogPageOffTopDPt.Text;

            if (crntText == string.Empty)
            {
                value = 0;
            }
            else
            {
                OK = int.TryParse(crntText, out value);

                if ((value < minVal) || (value > maxVal))
                    OK = false;
            }

            if (OK)
            {
                _logPageOffTopDPt = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString("F2");

                    MessageBox.Show("Top offset value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    newText + "'",
                                    "Logical page attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    _logPageOffTopDPt = defVal;

                    txtLogPageOffTopDPt.Text = newText;
                }
                else
                {
                    MessageBox.Show("Top offset value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Valid range is :\n\t" +
                                    minVal + " <= value <= " + maxVal,
                                    "Logical Page attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtLogPageOffTopDPt.Focus();
                    txtLogPageOffTopDPt.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e L o g P a g e H e i g h t D P t                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate logical page height value.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidateLogPageHeightDPt(bool lostFocusEvent)
        {
            const int minVal = 1;
            const int maxVal = 65535;
            int defVal = (int)(_paperLengthLogical * _unitsToDPts);

            int value = 0;

            bool OK = true;

            string crntText = txtLogPageHeightDPt.Text;

            if (crntText == string.Empty)
            {
                value = 0;
            }
            else
            {
                OK = int.TryParse(crntText, out value);

                if ((value < minVal) || (value > maxVal))
                    OK = false;
            }

            if (OK)
            {
                _logPageHeightDPt = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString("F2");

                    MessageBox.Show("Logical page height value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    newText + "'",
                                    "Logical page attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    _logPageHeightDPt = defVal;

                    txtLogPageHeightDPt.Text = newText;
                }
                else
                {
                    MessageBox.Show("Logical page height value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Valid range is :\n\t" +
                                    minVal + " <= value <= " + maxVal,
                                    "Logical Page attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtLogPageHeightDPt.Focus();
                    txtLogPageHeightDPt.SelectAll();
                }
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a l i d a t e L o g P a g e W i d t h D P t                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate logical page height value.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool ValidateLogPageWidthDPt(bool lostFocusEvent)
        {
            const int minVal = 1;
            const int maxVal = 65535;
            int defVal = (int)(_paperWidthLogical * _unitsToDPts);

            int value = 0;

            bool OK = true;

            string crntText = txtLogPageWidthDPt.Text;

            if (crntText == string.Empty)
            {
                value = 0;
            }
            else
            {
                OK = int.TryParse(crntText, out value);

                if ((value < minVal) || (value > maxVal))
                    OK = false;
            }

            if (OK)
            {
                _logPageWidthDPt = value;
            }
            else
            {
                if (lostFocusEvent)
                {
                    string newText = defVal.ToString("F2");

                    MessageBox.Show("Logical page width value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Value will be reset to default '" +
                                    newText + "'",
                                    "Logical page attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                    _logPageWidthDPt = defVal;

                    txtLogPageWidthDPt.Text = newText;
                }
                else
                {
                    MessageBox.Show("Logical page width value '" + crntText +
                                    "' is invalid.\n\n" +
                                    "Valid range is :\n\t" +
                                    minVal + " <= value <= " + maxVal,
                                    "Logical Page attribute invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    txtLogPageWidthDPt.Focus();
                    txtLogPageWidthDPt.SelectAll();
                }
            }

            return OK;
        }
    }
}
