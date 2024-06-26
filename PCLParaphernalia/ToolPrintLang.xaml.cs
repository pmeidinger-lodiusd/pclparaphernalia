﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PCLParaphernalia
{
    /// <summary>
    /// <para>Interaction logic for ToolPrintLang.xaml</para>
    /// <para>Class handles the Print Languages tool form.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    [System.Reflection.Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public partial class ToolPrintLang : Window
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private enum InfoType : byte
        {
            // must be in same order as _subsetTypes array

            PCL,
            HPGL2,
            PCLXLTags,
            PCLXLEnums,
            PJLCmds,
            PMLTags,
            SymbolSets,
            Fonts,
            PaperSizes,
            PrescribeCmds
        }

        public enum SymSetMapType : byte
        {
            Std,
            PCL,
            Both,
            Max
        }

        private readonly FontFamily _fontFixed = new FontFamily("Courier New");
        private readonly FontFamily _fontProp = new FontFamily("Arial");

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly int[] _subsetTypes =
        {
            // must be in same order as eInfoType enumeration

            (int) ToolCommonData.ToolSubIds.PCL,
            (int) ToolCommonData.ToolSubIds.HPGL2,
            (int) ToolCommonData.ToolSubIds.PCLXLTags,
            (int) ToolCommonData.ToolSubIds.PCLXLEnums,
            (int) ToolCommonData.ToolSubIds.PJLCmds,
            (int) ToolCommonData.ToolSubIds.PMLTags,
            (int) ToolCommonData.ToolSubIds.SymbolSets,
            (int) ToolCommonData.ToolSubIds.Fonts,
            (int) ToolCommonData.ToolSubIds.PaperSizes,
            (int) ToolCommonData.ToolSubIds.PrescribeCmds
        };

        private int _ctItems;
        private int _ctTypes;
        private int _indxType;

        private bool _flagPCLSeqControl,
                        _flagPCLSeqSimple,
                        _flagPCLSeqComplex,
                        _flagPCLOptObsolete,
                        _flagPCLOptDiscrete;

        private bool _flagPCLXLTagDataType,
                        _flagPCLXLTagAttribute,
                        _flagPCLXLTagOperator,
                        _flagPCLXLTagAttrDef,
                        _flagPCLXLTagEmbedDataLen,
                        _flagPCLXLTagWhitespace,
                        _flagPCLXLOptReserved;

        private bool _flagPMLTagDataType,
                        _flagPMLTagAction,
                        _flagPMLTagOutcome;

        private bool _flagSymSetList;

        private bool _flagSymSetMap;

        private SymSetMapType _symSetMapType = SymSetMapType.Std;

        private bool _initialised;

        private static string _saveFilename;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T o o l P r i n t L a n g                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolPrintLang(ref ToolCommonData.PrintLang crntPDL)
        {
            InitializeComponent();

            Initialise();

            crntPDL = ToolCommonData.PrintLang.Unknown;
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
            InfoType infoType = (InfoType)_indxType;

            if (infoType == InfoType.PCL)
                DisplayPCLSeqs();
            else if (infoType == InfoType.HPGL2)
                DisplayHPGL2Commands();
            else if (infoType == InfoType.PCLXLTags)
                DisplayPCLXLTags();
            else if (infoType == InfoType.PCLXLEnums)
                DisplayPCLXLEnums();
            else if (infoType == InfoType.PJLCmds)
                DisplayPJLCmds();
            else if (infoType == InfoType.PMLTags)
                DisplayPMLTags();
            else if (infoType == InfoType.SymbolSets)
                DisplaySymbolSetData();
            else if (infoType == InfoType.Fonts)
                DisplayFontData();
            else if (infoType == InfoType.PaperSizes)
                DisplayPaperSizeData();
            else if (infoType == InfoType.PrescribeCmds)
                DisplayPrescribeCmds();

            btnSaveReport.Visibility = Visibility.Visible;
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

            ReportCore.RptFileFmt rptFileFmt = ReportCore.RptFileFmt.NA;
            ReportCore.RptChkMarks rptChkMarks = ReportCore.RptChkMarks.NA;

            TargetCore.MetricsReturnFileRpt(ToolCommonData.ToolIds.PrintLang,
                                             out rptFileFmt,
                                             out rptChkMarks,
                                             out flagOptRptWrap);

            ToolPrintLangReport.Generate(_subsetTypes[_indxType],
                                          rptFileFmt,
                                          rptChkMarks,
                                          dgSeq,
                                          ref _saveFilename,
                                          _flagPCLSeqControl,
                                          _flagPCLSeqSimple,
                                          _flagPCLSeqComplex,
                                          _flagPCLOptObsolete,
                                          _flagPCLOptDiscrete,
                                          _flagPCLXLTagDataType,
                                          _flagPCLXLTagAttribute,
                                          _flagPCLXLTagOperator,
                                          _flagPCLXLTagAttrDef,
                                          _flagPCLXLTagEmbedDataLen,
                                          _flagPCLXLTagWhitespace,
                                          _flagPCLXLOptReserved,
                                          _flagPMLTagDataType,
                                          _flagPMLTagAction,
                                          _flagPMLTagOutcome,
                                          _flagSymSetList,
                                          _flagSymSetMap,
                                          flagOptRptWrap,
                                          _symSetMapType);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k F o n t O p t M a p _ C h e c k e d                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the Fonts option 'Show Symbol Set lists' checkbox is   //
        // checked.                                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkFontOptMap_Checked(object sender, RoutedEventArgs e)
        {
            _flagSymSetList = true;

            lbFontMapComment1.Visibility = Visibility.Visible;
            lbFontMapComment2.Visibility = Visibility.Visible;
            lbFontMapComment3.Visibility = Visibility.Visible;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k F o n t O p t M a p _ U n c h e c k e d                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the Fonts option 'Show Symbol Set lists' checkbox is   //
        // unchecked.                                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkFontOptMap_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagSymSetList = false;

            lbFontMapComment1.Visibility = Visibility.Hidden;
            lbFontMapComment2.Visibility = Visibility.Hidden;
            lbFontMapComment3.Visibility = Visibility.Hidden;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L O p t D i s c r e t e _ C h e c k e d                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL option 'Discrete' checkbox is checked.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLOptDiscrete_Checked(object sender, RoutedEventArgs e)
        {
            _flagPCLOptDiscrete = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L O p t D i s c r e t e _ U n c h e c k e d              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL option 'Discrete' checkbox is unchecked.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLOptDiscrete_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPCLOptDiscrete = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L O p t O b s o l e t e _ C h e c k e d                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL option 'Obsolete' checkbox is checked.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLOptObsolete_Checked(object sender, RoutedEventArgs e)
        {
            _flagPCLOptObsolete = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L O p t O b s o l e t e _ U n c h e c k e d              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL option 'Obsolete' checkbox is unchecked.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLOptObsolete_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPCLOptObsolete = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L S e q C o m p l e x _ C h e c k e d                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL sequence type 'Complex' checkbox is checked.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLSeqComplex_Checked(object sender, RoutedEventArgs e)
        {
            _flagPCLSeqComplex = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L S e q C o m p l e x _ U n c h e c k e d                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL sequence type 'Complex' checkbox is unchecked. //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLSeqComplex_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPCLSeqComplex = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L S e q C o n t r o l _ C h e c k e d                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL sequence type 'Control' checkbox is checked.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLSeqControl_Checked(object sender, RoutedEventArgs e)
        {
            _flagPCLSeqControl = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L S e q C o n t r o l _ U n c h e c k e d                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL sequence type 'Control' checkbox is unchecked. //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLSeqControl_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPCLSeqControl = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L S e q S i m p l e _ C h e c k e d                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL sequence type 'Simple' checkbox is checked.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLSeqSimple_Checked(object sender, RoutedEventArgs e)
        {
            _flagPCLSeqSimple = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L S e q S i m p l e _ U n c h e c k e d                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCL sequence type 'Simple' checkbox is unchecked.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLSeqSimple_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPCLSeqSimple = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L O p t R e s e r v e d _ C h e c k e d              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCLXL option 'Reserved' checkbox is checked.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLOptReserved_Checked(object sender, RoutedEventArgs e)
        {
            _flagPCLXLOptReserved = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L O p t R e s e r v e d _ U n c h e c k e d          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCLXL option 'Reserved' checkbox is unchecked.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLOptReserved_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPCLXLOptReserved = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L T a g A t t r D e f i n e r_ C h e c k e d         //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCLXL tag type 'Attr. Def.' checkbox is checked.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLTagAttrDefiner_Checked(object sender, RoutedEventArgs e)
        {
            _flagPCLXLTagAttrDef = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L T a g A t t r D e f i n e r_ U n c h e c k e d     //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCLXL tag type 'Attr. Def.' checkbox is unchecked. //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLTagAttrDefiner_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPCLXLTagAttrDef = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L T a g A t t r i b u t e _ C h e c k e d            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCLXL tag type 'Attribute' checkbox is checked.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLTagAttribute_Checked(object sender, RoutedEventArgs e)
        {
            _flagPCLXLTagAttribute = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L T a g A t t r i b u t e _ U n c h e c k e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCLXL tag type 'Attribute' checkbox is unchecked.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLTagAttribute_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPCLXLTagAttribute = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L T a g D a t a T y p e _ C h e c k e d              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCLXL tag type 'Data Type' checkbox is checked.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLTagDataType_Checked(object sender, RoutedEventArgs e)
        {
            _flagPCLXLTagDataType = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L T a g D a t a T y p e _ U n c h e c k e d          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCLXL tag type 'Data Type' checkbox is unchecked.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLTagDataType_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPCLXLTagDataType = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L T a g E m b e d D a t a L e n _ C h e c k e d      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCLXL tag type 'Embed Data' checkbox is checked.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLTagEmbedDataLen_Checked(object sender, RoutedEventArgs e)
        {
            _flagPCLXLTagEmbedDataLen = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L T a g E m b e d D a t a L e n _ U n c h e c k e d  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCLXL tag type 'Embed Data' checkbox is unchecked. //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLTagEmbedDataLen_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPCLXLTagEmbedDataLen = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L T a g O p e r a t o r _ C h e c k e d              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCLXL tag type 'Operator' checkbox is checked.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLTagOperator_Checked(object sender, RoutedEventArgs e)
        {
            _flagPCLXLTagOperator = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L T a g O p e r a t o r _ U n c h e c k e d          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCLXL tag type 'Operator' checkbox is unchecked.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLTagOperator_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPCLXLTagOperator = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L T a g W h i t e s p a c e _ C h e c k e d          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCLXL tag type 'Whitespace' checkbox is checked.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLTagWhitespace_Checked(object sender, RoutedEventArgs e)
        {
            _flagPCLXLTagWhitespace = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P C L X L T a g W h i t e s p a c e _ U n c h e c k e d      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PCLXL tag type 'Whitespace' checkbox is unchecked. //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPCLXLTagWhitespace_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPCLXLTagWhitespace = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P M L T a g A c t i o n _ C h e c k e d                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PML tag type 'Action' checkbox is checked.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPMLTagAction_Checked(object sender, RoutedEventArgs e)
        {
            _flagPMLTagAction = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P ML T a g A c t i o n _ U n c h e c k e d                   //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PML tag type 'Action' checkbox is unchecked.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPMLTagAction_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPMLTagAction = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P M L T a g D a t a T y p e _ C h e c k e d                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PML tag type 'Data Type' checkbox is checked.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPMLTagDataType_Checked(object sender, RoutedEventArgs e)
        {
            _flagPMLTagDataType = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P M L T a g D a t a T y p e _ U n c h e c k e d              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PML tag type 'Data Type' checkbox is unchecked.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPMLTagDataType_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPMLTagDataType = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P M L T a g O u t c o m e _ C h e c k e d                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PML tag type 'Outcome' checkbox is checked.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPMLTagOutcome_Checked(object sender, RoutedEventArgs e)
        {
            _flagPMLTagOutcome = true;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k P M L T a g O u t c o m e _ U n c h e c k e d                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the PML tag type 'Outcome' checkbox is unchecked.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkPMLTagOutcome_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagPMLTagOutcome = false;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k S y m S e t O p t M a p _ C h e c k e d                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the Symbol Sets option 'Show Mappings' checkbox is     //
        // checked.                                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkSymSetOptMap_Checked(object sender, RoutedEventArgs e)
        {
            _flagSymSetMap = true;

            //  grpSymSetMapSet.Visibility = Visibility.Visible;
            grpSymSetMapType.Visibility = Visibility.Visible;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h k S y m S e t O p t M a p _ U n c h e c k e d                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the Symbol Sets option 'Show Mappings' checkbox is     //
        // unchecked.                                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void chkSymSetOptMap_Unchecked(object sender, RoutedEventArgs e)
        {
            _flagSymSetMap = false;

            //   grpSymSetMapSet.Visibility = Visibility.Hidden;
            grpSymSetMapType.Visibility = Visibility.Hidden;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c l e a r D e t a i l s                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Clear the details area, etc.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ClearDetails()
        {
            if (_initialised)
            {
                dgSeq.Items.Clear();

                btnSaveReport.Visibility = Visibility.Hidden;

                txtCount.Text = "0";
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y F o n t D a t a                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display Font data.                                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void DisplayFontData()
        {
            SetColsFonts();

            dgSeq.Items.Clear();

            _ctItems = PCLFonts.DisplayFontList(dgSeq);

            txtCount.Text = _ctItems.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y H P G L 2 C o m m a n d s                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display HP-GL/2 commands.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void DisplayHPGL2Commands()
        {
            SetColsHPGL2();

            dgSeq.Items.Clear();

            _ctItems = HPGL2Commands.DisplaySeqList(dgSeq);

            txtCount.Text = _ctItems.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y P a p e r S i z e s                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display paper size data.                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void DisplayPaperSizeData()
        {
            SetColsPaperSizes();

            dgSeq.Items.Clear();

            _ctItems = PCLPaperSizes.DisplayPaperSizeList(dgSeq);

            txtCount.Text = _ctItems.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y P C L S e q s                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display PCL sequences.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void DisplayPCLSeqs()
        {
            if (!_flagPCLSeqSimple && !_flagPCLSeqComplex && !_flagPCLSeqControl)
            {
                MessageBox.Show("At least one sequence type must be selected.",
                                "PCL Sequence Type Selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            else
            {
                SetColsPCL();

                dgSeq.Items.Clear();

                _ctItems = 0;

                if (chkPCLSeqControl.IsChecked == true)
                    _ctItems += PCLControlCodes.DisplaySeqList(dgSeq);

                if (chkPCLSeqSimple.IsChecked == true)
                    _ctItems += PCLSimpleSeqs.DisplaySeqList(dgSeq, _flagPCLOptObsolete);

                if (chkPCLSeqComplex.IsChecked == true)
                    _ctItems += PCLComplexSeqs.DisplaySeqList(dgSeq, _flagPCLOptObsolete, _flagPCLOptDiscrete);

                txtCount.Text = _ctItems.ToString();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y P C L X L E n u m s                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display PCL XL enumeration details.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void DisplayPCLXLEnums()
        {
            SetColsPCLXLEnums();

            dgSeq.Items.Clear();

            _ctItems = PCLXLAttrEnums.DisplayTags(dgSeq);

            txtCount.Text = _ctItems.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y P C L X L T a g s                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display PCL XL tag details.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void DisplayPCLXLTags()
        {
            if (!_flagPCLXLTagDataType &&
                !_flagPCLXLTagAttribute &&
                !_flagPCLXLTagOperator &&
                !_flagPCLXLTagWhitespace &&
                !_flagPCLXLTagAttrDef &&
                !_flagPCLXLTagEmbedDataLen)
            {
                MessageBox.Show("At least one tag type must be selected.",
                                "PCL XL Tag Type Selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            else
            {
                SetColsPCLXLTags();

                dgSeq.Items.Clear();

                _ctItems = 0;

                if (_flagPCLXLTagAttrDef)
                    _ctItems += PCLXLAttrDefiners.DisplayTags(dgSeq, _flagPCLXLOptReserved);

                if (_flagPCLXLTagEmbedDataLen)
                    _ctItems += PCLXLEmbedDataDefs.DisplayTags(dgSeq, _flagPCLXLOptReserved);

                if (_flagPCLXLTagAttribute)
                    _ctItems += PCLXLAttributes.DisplayTags(dgSeq, _flagPCLXLOptReserved);

                if (_flagPCLXLTagDataType)
                    _ctItems += PCLXLDataTypes.DisplayTags(dgSeq, _flagPCLXLOptReserved);

                if (_flagPCLXLTagOperator)
                    _ctItems += PCLXLOperators.DisplayTags(dgSeq, _flagPCLXLOptReserved);

                if (_flagPCLXLTagWhitespace)
                    _ctItems += PCLXLWhitespaces.DisplayTags(dgSeq);

                txtCount.Text = _ctItems.ToString();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y P J L C m d s                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display PJL command details.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void DisplayPJLCmds()
        {
            SetColsPJLCmds();

            dgSeq.Items.Clear();

            _ctItems = PJLCommands.DisplayCmds(dgSeq);

            txtCount.Text = _ctItems.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y P M L T a g s                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display PML tag details.                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void DisplayPMLTags()
        {
            if (!_flagPMLTagDataType && !_flagPMLTagAction && !_flagPMLTagOutcome)
            {
                MessageBox.Show("At least one tag type must be selected.",
                                "PML Tag Type Selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            else
            {
                SetColsPMLTags();

                dgSeq.Items.Clear();

                _ctItems = 0;

                if (_flagPMLTagDataType)
                    _ctItems += PMLDataTypes.DisplayTags(dgSeq);

                if (_flagPMLTagAction)
                    _ctItems += PMLActions.DisplayTags(dgSeq);

                if (_flagPMLTagOutcome)
                    _ctItems += PMLOutcomes.DisplayTags(dgSeq);

                txtCount.Text = _ctItems.ToString();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y P r e s c r i b e C m d s                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display Prescribe command details.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void DisplayPrescribeCmds()
        {
            SetColsPrescribeCmds();

            dgSeq.Items.Clear();

            _ctItems = PrescribeCommands.DisplayCmds(dgSeq);

            txtCount.Text = _ctItems.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y S y m b o l S e t D a t a                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display Symbol Set data.                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void DisplaySymbolSetData()
        {
            SetColsSymbolSets();

            dgSeq.Items.Clear();

            _ctItems = PCLSymbolSets.DisplaySeqList(dgSeq);

            txtCount.Text = _ctItems.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g i v e C r n t P D L                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolCommonData.PrintLang GetCurrentPDL() => ToolCommonData.PrintLang.Unknown;

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
            InfoType infoType;

            _initialised = false;

            //----------------------------------------------------------------//
            //                                                                //
            // Populate form.                                                 //
            //                                                                //
            //----------------------------------------------------------------//

            btnSaveReport.Visibility = Visibility.Hidden;

            txtCount.Text = "0";

            //----------------------------------------------------------------//

            _ctTypes = _subsetTypes.Length;

            //----------------------------------------------------------------//

            ResetTarget();

            //----------------------------------------------------------------//
            //                                                                //
            // Reinstate settings from persistent storage.                    //
            //                                                                //
            //----------------------------------------------------------------//

            MetricsLoad();

            infoType = (InfoType)_indxType;

            if (infoType == InfoType.HPGL2)
            {
                rbSelTypeHPGL2.IsChecked = true;
                tabInfoType.SelectedItem = tabHPGL2;
                SetColsHPGL2();
            }
            else if (infoType == InfoType.PCL)
            {
                rbSelTypePCL.IsChecked = true;
                tabInfoType.SelectedItem = tabPCL;
                SetColsPCL();
            }
            else if (infoType == InfoType.PCLXLEnums)
            {
                rbSelTypePCLXLEnums.IsChecked = true;
                tabInfoType.SelectedItem = tabPCLXLEnums;
                SetColsPCLXLEnums();
            }
            else if (infoType == InfoType.PCLXLTags)
            {
                rbSelTypePCLXLTags.IsChecked = true;
                tabInfoType.SelectedItem = tabPCLXLTags;
                SetColsPCLXLTags();
            }
            else if (infoType == InfoType.PJLCmds)
            {
                rbSelTypePJLCmds.IsChecked = true;
                tabInfoType.SelectedItem = tabPJLCmds;
                SetColsPJLCmds();
            }
            else if (infoType == InfoType.PMLTags)
            {
                rbSelTypePMLTags.IsChecked = true;
                tabInfoType.SelectedItem = tabPMLTags;
                SetColsPMLTags();
            }
            else if (infoType == InfoType.SymbolSets)
            {
                rbSelTypeSymbolSets.IsChecked = true;
                tabInfoType.SelectedItem = tabSymbolSets;
                SetColsSymbolSets();
            }
            else if (infoType == InfoType.Fonts)
            {
                rbSelTypeFonts.IsChecked = true;
                tabInfoType.SelectedItem = tabFonts;
                SetColsFonts();
            }
            else if (infoType == InfoType.PaperSizes)
            {
                rbSelTypePaperSizes.IsChecked = true;
                tabInfoType.SelectedItem = tabPaperSizes;
                SetColsPaperSizes();
            }
            else
            {
                rbSelTypePCL.IsChecked = true;
                tabInfoType.SelectedItem = tabPCL;
                SetColsPCL();
            }

            _initialised = true;
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
            int tmpInt = 0;

            ToolPrintLangPersist.LoadDataCommon(out _indxType, out _saveFilename);

            ToolPrintLangPersist.LoadDataPCL(out _flagPCLSeqControl,
                                              out _flagPCLSeqSimple,
                                              out _flagPCLSeqComplex,
                                              out _flagPCLOptObsolete,
                                              out _flagPCLOptDiscrete);

            ToolPrintLangPersist.LoadDataPCLXL(out _flagPCLXLTagDataType,
                                                out _flagPCLXLTagAttribute,
                                                out _flagPCLXLTagOperator,
                                                out _flagPCLXLTagAttrDef,
                                                out _flagPCLXLTagEmbedDataLen,
                                                out _flagPCLXLTagWhitespace,
                                                out _flagPCLXLOptReserved);

            ToolPrintLangPersist.LoadDataPML(out _flagPMLTagDataType,
                                              out _flagPMLTagAction,
                                              out _flagPMLTagOutcome);

            ToolPrintLangPersist.LoadDataFonts(out _flagSymSetList);

            ToolPrintLangPersist.LoadDataSymSets(out _flagSymSetMap, out tmpInt);

            //----------------------------------------------------------------//

            if ((_indxType < 0) || (_indxType >= _ctTypes))
                _indxType = (int)InfoType.PCL;

            if ((tmpInt < 0) || (tmpInt >= (int)SymSetMapType.Max))
                _symSetMapType = SymSetMapType.Both;
            else
                _symSetMapType = (SymSetMapType)tmpInt;

            //----------------------------------------------------------------//

            chkPCLSeqControl.IsChecked = _flagPCLSeqControl;
            chkPCLSeqSimple.IsChecked = _flagPCLSeqSimple;
            chkPCLSeqComplex.IsChecked = _flagPCLSeqComplex;

            chkPCLOptObsolete.IsChecked = _flagPCLOptObsolete;
            chkPCLOptDiscrete.IsChecked = _flagPCLOptDiscrete;

            //----------------------------------------------------------------//

            chkPCLXLTagDataType.IsChecked = _flagPCLXLTagDataType;
            chkPCLXLTagAttribute.IsChecked = _flagPCLXLTagAttribute;
            chkPCLXLTagOperator.IsChecked = _flagPCLXLTagOperator;
            chkPCLXLTagAttrDefiner.IsChecked = _flagPCLXLTagAttrDef;
            chkPCLXLTagEmbedDataLen.IsChecked = _flagPCLXLTagEmbedDataLen;
            chkPCLXLTagWhitespace.IsChecked = _flagPCLXLTagWhitespace;

            chkPCLXLOptReserved.IsChecked = _flagPCLXLOptReserved;

            //----------------------------------------------------------------//

            chkPMLTagDataType.IsChecked = _flagPMLTagDataType;
            chkPMLTagAction.IsChecked = _flagPMLTagAction;
            chkPMLTagOutcome.IsChecked = _flagPMLTagOutcome;

            //----------------------------------------------------------------//

            chkSymSetOptMap.IsChecked = _flagSymSetMap;

            if (_symSetMapType == SymSetMapType.Std)
                rbSymSetMapStd.IsChecked = true;
            else if (_symSetMapType == SymSetMapType.PCL)
                rbSymSetMapPCL.IsChecked = true;
            else
                rbSymSetMapBoth.IsChecked = true;

            if (_flagSymSetMap)
            {
                grpSymSetMapType.Visibility = Visibility.Visible;
            }
            else
            {
                grpSymSetMapType.Visibility = Visibility.Hidden;
            }

            //----------------------------------------------------------------//

            chkFontOptMap.IsChecked = _flagSymSetList;

            if (_flagSymSetList)
            {
                lbFontMapComment1.Visibility = Visibility.Visible;
                lbFontMapComment2.Visibility = Visibility.Visible;
                lbFontMapComment3.Visibility = Visibility.Visible;
            }
            else
            {
                lbFontMapComment1.Visibility = Visibility.Hidden;
                lbFontMapComment2.Visibility = Visibility.Hidden;
                lbFontMapComment3.Visibility = Visibility.Hidden;
            }
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
            ToolPrintLangPersist.SaveDataCommon(_indxType,
                                                 _saveFilename);

            ToolPrintLangPersist.SaveDataPCL(_flagPCLSeqControl,
                                              _flagPCLSeqSimple,
                                              _flagPCLSeqComplex,
                                              _flagPCLOptObsolete,
                                              _flagPCLOptDiscrete);

            ToolPrintLangPersist.SaveDataPCLXL(_flagPCLXLTagDataType,
                                                _flagPCLXLTagAttribute,
                                                _flagPCLXLTagOperator,
                                                _flagPCLXLTagAttrDef,
                                                _flagPCLXLTagEmbedDataLen,
                                                _flagPCLXLTagWhitespace,
                                                _flagPCLXLOptReserved);

            ToolPrintLangPersist.SaveDataPML(_flagPMLTagDataType,
                                              _flagPMLTagAction,
                                              _flagPMLTagOutcome);

            ToolPrintLangPersist.SaveDataFonts(_flagSymSetList);

            ToolPrintLangPersist.SaveDataSymSets(_flagSymSetMap,
                                                  (int)_symSetMapType);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S e l T y p e F o n t s _ C l i c k                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting type Fonts clicked.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSelTypeFonts_Click(object sender, RoutedEventArgs e)
        {
            _indxType = (int)InfoType.Fonts;

            tabInfoType.SelectedItem = tabFonts;

            ClearDetails();

            SetColsFonts();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S e l T y p e H P G L 2 _ C l i c k                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting type HP-GL/2 clicked.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSelTypeHPGL2_Click(object sender, RoutedEventArgs e)
        {
            _indxType = (int)InfoType.HPGL2;

            tabInfoType.SelectedItem = tabHPGL2;

            ClearDetails();

            SetColsHPGL2();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S e l T y p e P a p e r S i z e s _ C l i c k                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting type Paper Sizes clicked.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSelTypePaperSizes_Click(object sender, RoutedEventArgs e)
        {
            _indxType = (int)InfoType.PaperSizes;

            tabInfoType.SelectedItem = tabPaperSizes;

            ClearDetails();

            SetColsPaperSizes();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S e l T y p e P C L _ C l i c k                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting type PCL clicked.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSelTypePCL_Click(object sender, RoutedEventArgs e)
        {
            _indxType = (int)InfoType.PCL;

            tabInfoType.SelectedItem = tabPCL;

            ClearDetails();

            SetColsPCL();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S e l T y p e P C L X L E n u m s _ C l i c k                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting type PCL XL Enums clicked.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSelTypePCLXLEnums_Click(object sender, RoutedEventArgs e)
        {
            _indxType = (int)InfoType.PCLXLEnums;

            tabInfoType.SelectedItem = tabPCLXLEnums;

            ClearDetails();

            SetColsPCLXLEnums();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S e l T y p e P C L X L T a g s _ C l i c k                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting type PCL XL Tags clicked.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSelTypePCLXLTags_Click(object sender, RoutedEventArgs e)
        {
            _indxType = (int)InfoType.PCLXLTags;

            tabInfoType.SelectedItem = tabPCLXLTags;

            ClearDetails();

            SetColsPCLXLTags();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S e l T y p e P J L C m d s _ C l i c k                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting type PJL Commands clicked.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSelTypePJLCmds_Click(object sender, RoutedEventArgs e)
        {
            _indxType = (int)InfoType.PJLCmds;

            tabInfoType.SelectedItem = tabPJLCmds;

            ClearDetails();

            SetColsPJLCmds();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S e l T y p e P M L T a g s _ C l i c k                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting type PML Tags clicked.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSelTypePMLTags_Click(object sender, RoutedEventArgs e)
        {
            _indxType = (int)InfoType.PMLTags;

            tabInfoType.SelectedItem = tabPMLTags;

            ClearDetails();

            SetColsPMLTags();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S e l T y p e P r e s c r i b e C m d s _ C l i c k            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting type PML Tags clicked.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSelTypePrescribeCmds_Click(object sender, RoutedEventArgs e)
        {
            _indxType = (int)InfoType.PrescribeCmds;

            tabInfoType.SelectedItem = tabPrescribeCmds;

            ClearDetails();

            SetColsPrescribeCmds();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S e l T y p e S y m b o l S e t s _ C l i c k                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting type Symbol Sets clicked.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSelTypeSymbolSets_Click(object sender, RoutedEventArgs e)
        {
            _indxType = (int)InfoType.SymbolSets;

            tabInfoType.SelectedItem = tabSymbolSets;

            ClearDetails();

            SetColsSymbolSets();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S y m S e t M a p B o t h _ C l i c k                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting Symbol Set mapping 'Both'.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSymSetMapBoth_Click(object sender, RoutedEventArgs e)
        {
            _symSetMapType = SymSetMapType.Both;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S y m S e t M a p P C L _ C l i c k                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting Symbol Set mapping 'PCL'.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSymSetMapPCL_Click(object sender, RoutedEventArgs e)
        {
            _symSetMapType = SymSetMapType.PCL;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b S y m S e t M a p S t d _ C l i c k                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Radio button selecting Symbol Set mapping 'strict'.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbSymSetMapStd_Click(object sender, RoutedEventArgs e)
        {
            _symSetMapType = SymSetMapType.Std;

            ClearDetails();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s e t T a r g e t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset the target type.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void ResetTarget()
        {
            btnGenerate.Content = "Display requested data";
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C o l s F o n t s                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Define datagrid columns for type Fonts.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetColsFonts()
        {
            DataGridTextColumn colTypeface = new DataGridTextColumn();
            DataGridTextColumn colName = new DataGridTextColumn();
            DataGridTextColumn colSpacing = new DataGridTextColumn();
            DataGridCheckBoxColumn colFlagScalable = new DataGridCheckBoxColumn();
            DataGridTextColumn colBound = new DataGridTextColumn();
            DataGridTextColumn colPitch = new DataGridTextColumn();
            DataGridTextColumn colHeight = new DataGridTextColumn();
            DataGridCheckBoxColumn colFlagVar_R = new DataGridCheckBoxColumn();
            DataGridCheckBoxColumn colFlagVar_I = new DataGridCheckBoxColumn();
            DataGridCheckBoxColumn colFlagVar_B = new DataGridCheckBoxColumn();
            DataGridCheckBoxColumn colFlagVar_BI = new DataGridCheckBoxColumn();
            DataGridTextColumn colSymbolSets = null;

            if (_flagSymSetList)
            {
                colSymbolSets = new DataGridTextColumn();
                colSymbolSets.Header = "Supported Symbol Sets?";
            }

            colTypeface.Header = "Typeface";
            colName.Header = "Name";
            colSpacing.Header = "Spacing";
            colFlagScalable.Header = "Scalable?";
            colBound.Header = "Bound?";
            colPitch.Header = "Pitch";
            colHeight.Header = "Height";
            colFlagVar_R.Header = "Regular?";
            colFlagVar_I.Header = "Italic?";
            colFlagVar_B.Header = "Bold?";
            colFlagVar_BI.Header = "Bold Italic?";

            dgSeq.Columns.Clear();
            dgSeq.Columns.Add(colTypeface);
            dgSeq.Columns.Add(colName);
            dgSeq.Columns.Add(colSpacing);
            dgSeq.Columns.Add(colFlagScalable);
            dgSeq.Columns.Add(colBound);
            dgSeq.Columns.Add(colPitch);
            dgSeq.Columns.Add(colHeight);
            dgSeq.Columns.Add(colFlagVar_R);
            dgSeq.Columns.Add(colFlagVar_I);
            dgSeq.Columns.Add(colFlagVar_B);
            dgSeq.Columns.Add(colFlagVar_BI);

            if (_flagSymSetList)
            {
                dgSeq.Columns.Add(colSymbolSets);
            }

            Binding bindId = new Binding("Typeface");
            bindId.Mode = BindingMode.OneWay;

            Binding bindName = new Binding("Name");
            bindName.Mode = BindingMode.OneWay;

            Binding bindSpacing = new Binding("Spacing");
            bindSpacing.Mode = BindingMode.OneWay;

            Binding bindFlagScalable = new Binding("Scalable");
            bindFlagScalable.Mode = BindingMode.OneWay;

            Binding bindBound = new Binding("BoundSymbolSet");
            bindBound.Mode = BindingMode.OneWay;

            Binding bindPitch = new Binding("Pitch");
            bindPitch.Mode = BindingMode.OneWay;

            Binding bindHeight = new Binding("Height");
            bindHeight.Mode = BindingMode.OneWay;

            Binding bindFlagVar_R = new Binding("Var_Regular");
            bindFlagVar_R.Mode = BindingMode.OneWay;

            Binding bindFlagVar_I = new Binding("Var_Italic");
            bindFlagVar_I.Mode = BindingMode.OneWay;

            Binding bindFlagVar_B = new Binding("Var_Bold");
            bindFlagVar_B.Mode = BindingMode.OneWay;

            Binding bindFlagVar_BI = new Binding("Var_BoldItalic");
            bindFlagVar_BI.Mode = BindingMode.OneWay;

            Binding bindSymbolSets = null;

            if (_flagSymSetList)
            {
                bindSymbolSets = new Binding("SymbolSets");
                bindSymbolSets.Mode = BindingMode.OneWay;
            }

            colTypeface.Binding = bindId;
            colName.Binding = bindName;
            colSpacing.Binding = bindSpacing;
            colFlagScalable.Binding = bindFlagScalable;
            colBound.Binding = bindBound;
            colPitch.Binding = bindPitch;
            colHeight.Binding = bindHeight;
            colFlagVar_R.Binding = bindFlagVar_R;
            colFlagVar_I.Binding = bindFlagVar_I;
            colFlagVar_B.Binding = bindFlagVar_B;
            colFlagVar_BI.Binding = bindFlagVar_BI;

            if (_flagSymSetList)
            {
                colSymbolSets.Binding = bindSymbolSets;
            }

            dgSeq.FontFamily = _fontProp;
            colTypeface.FontFamily = _fontFixed;
            colBound.FontFamily = _fontFixed;
            if (_flagSymSetList)
            {
                colSymbolSets.FontFamily = _fontFixed;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C o l s H P G L 2                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Define datagrid columns for type HP-GL/2.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetColsHPGL2()
        {
            DataGridTextColumn colMnemonic = new DataGridTextColumn();
            DataGridTextColumn colDescription = new DataGridTextColumn();

            colMnemonic.Header = "Mnemonic";
            colDescription.Header = "Description";

            dgSeq.Columns.Clear();
            dgSeq.Columns.Add(colMnemonic);
            dgSeq.Columns.Add(colDescription);

            Binding bindMnemonic = new Binding("Mnemonic");
            bindMnemonic.Mode = BindingMode.OneWay;

            Binding bindDescription = new Binding("Description");
            bindDescription.Mode = BindingMode.OneWay;

            colMnemonic.Binding = bindMnemonic;
            colDescription.Binding = bindDescription;

            dgSeq.FontFamily = _fontProp;
            colMnemonic.FontFamily = _fontFixed;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C o l s P a p e r S i z e s                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Define datagrid columns for type Paper Sizes.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetColsPaperSizes()
        {
            DataGridTextColumn colName = new DataGridTextColumn();
            DataGridTextColumn colDesc = new DataGridTextColumn();
            DataGridTextColumn colEdgeShort = new DataGridTextColumn();
            DataGridTextColumn colEdgeLong = new DataGridTextColumn();
            DataGridTextColumn colIdPCL = new DataGridTextColumn();
            DataGridTextColumn colIdNamePCLXL = new DataGridTextColumn();

            colName.Header = "Name";
            colDesc.Header = "Description";
            colEdgeShort.Header = "Short edge";
            colEdgeLong.Header = "Long edge";
            colIdPCL.Header = "PCL Id";
            colIdNamePCLXL.Header = "PCLXL Id/Name";

            dgSeq.Columns.Clear();
            dgSeq.Columns.Add(colName);
            dgSeq.Columns.Add(colDesc);
            dgSeq.Columns.Add(colEdgeShort);
            dgSeq.Columns.Add(colEdgeLong);
            dgSeq.Columns.Add(colIdPCL);
            dgSeq.Columns.Add(colIdNamePCLXL);

            Binding bindName = new Binding("Name");
            bindName.Mode = BindingMode.OneWay;

            Binding bindDesc = new Binding("Desc");
            bindDesc.Mode = BindingMode.OneWay;

            Binding bindEdgeShort = new Binding("EdgeShort");
            bindEdgeShort.Mode = BindingMode.OneWay;

            Binding bindEdgeLong = new Binding("EdgeLong");
            bindEdgeLong.Mode = BindingMode.OneWay;

            Binding bindIdPCL = new Binding("IdPCL");
            bindIdPCL.Mode = BindingMode.OneWay;

            Binding bindIdNamePCLXL = new Binding("IdNamePCLXL");
            bindIdNamePCLXL.Mode = BindingMode.OneWay;

            colName.Binding = bindName;
            colDesc.Binding = bindDesc;
            colEdgeShort.Binding = bindEdgeShort;
            colEdgeLong.Binding = bindEdgeLong;
            colIdPCL.Binding = bindIdPCL;
            colIdNamePCLXL.Binding = bindIdNamePCLXL;

            dgSeq.FontFamily = _fontFixed;
            dgSeq.FontSize = 11;
            colName.FontFamily = _fontProp;
            colDesc.FontFamily = _fontProp;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C o l s P C L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Define datagrid columns for type PCL.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetColsPCL()
        {
            DataGridTextColumn colSequence = new DataGridTextColumn();
            DataGridTextColumn colType = new DataGridTextColumn();
            DataGridTextColumn colDescription = new DataGridTextColumn();
            DataGridCheckBoxColumn colFlagObsolete = new DataGridCheckBoxColumn();
            DataGridCheckBoxColumn colFlagValIsLen = new DataGridCheckBoxColumn();

            colSequence.Header = "Sequence";
            colType.Header = "Type";
            colFlagObsolete.Header = "Obsolete?";
            colFlagValIsLen.Header = "#=length?";
            colDescription.Header = "Description";

            dgSeq.Columns.Clear();
            dgSeq.Columns.Add(colSequence);
            dgSeq.Columns.Add(colType);
            dgSeq.Columns.Add(colFlagObsolete);
            dgSeq.Columns.Add(colFlagValIsLen);
            dgSeq.Columns.Add(colDescription);

            Binding bindSequence = new Binding("Sequence");
            bindSequence.Mode = BindingMode.OneWay;

            Binding bindType = new Binding("Type");
            bindType.Mode = BindingMode.OneWay;

            Binding bindFlagObsolete = new Binding("FlagObsolete");
            bindFlagObsolete.Mode = BindingMode.OneWay;

            Binding bindFlagValIsLen = new Binding("FlagValIsLen");
            bindFlagValIsLen.Mode = BindingMode.OneWay;

            Binding bindDescription = new Binding("Description");
            bindDescription.Mode = BindingMode.OneWay;

            colSequence.Binding = bindSequence;
            colType.Binding = bindType;
            colFlagObsolete.Binding = bindFlagObsolete;
            colFlagValIsLen.Binding = bindFlagValIsLen;
            colDescription.Binding = bindDescription;

            dgSeq.FontFamily = _fontProp;
            colSequence.FontFamily = _fontFixed;
            colDescription.FontFamily = _fontFixed;
            colDescription.FontSize = 11;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C o l s P C L X L E n u m s                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Define datagrid columns for type PCL XL Enums.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetColsPCLXLEnums()
        {
            DataGridTextColumn colTagOper = new DataGridTextColumn();
            DataGridTextColumn colTagAttr = new DataGridTextColumn();
            DataGridTextColumn colValue = new DataGridTextColumn();
            DataGridTextColumn colDescription = new DataGridTextColumn();

            colTagOper.Header = "Operator";
            colTagAttr.Header = "Attribute";
            colValue.Header = "Value";
            colDescription.Header = "Description";

            dgSeq.Columns.Clear();
            dgSeq.Columns.Add(colTagOper);
            dgSeq.Columns.Add(colTagAttr);
            dgSeq.Columns.Add(colValue);
            dgSeq.Columns.Add(colDescription);

            Binding bindTagOper = new Binding("Operator");
            bindTagOper.Mode = BindingMode.OneWay;

            Binding bindTagAttr = new Binding("Attribute");
            bindTagAttr.Mode = BindingMode.OneWay;

            Binding bindValue = new Binding("Value");
            bindValue.Mode = BindingMode.OneWay;

            Binding bindDescription = new Binding("Description");
            bindDescription.Mode = BindingMode.OneWay;

            colTagOper.Binding = bindTagOper;
            colTagAttr.Binding = bindTagAttr;
            colValue.Binding = bindValue;
            colDescription.Binding = bindDescription;

            dgSeq.FontFamily = _fontProp;
            colTagOper.FontFamily = _fontFixed;
            colTagAttr.FontFamily = _fontFixed;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C o l s P C L X L T a g s                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Define datagrid columns for type PCL XL Tags.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetColsPCLXLTags()
        {
            DataGridTextColumn colTag = new DataGridTextColumn();
            DataGridTextColumn colType = new DataGridTextColumn();
            DataGridTextColumn colDescription = new DataGridTextColumn();
            DataGridCheckBoxColumn colFlagReserved = new DataGridCheckBoxColumn();

            colTag.Header = "Tag value";
            colType.Header = "Type";
            colFlagReserved.Header = "Reserved?";
            colDescription.Header = "Description";

            dgSeq.Columns.Clear();
            dgSeq.Columns.Add(colTag);
            dgSeq.Columns.Add(colType);
            dgSeq.Columns.Add(colFlagReserved);
            dgSeq.Columns.Add(colDescription);

            Binding bindTag = new Binding("Tag");
            bindTag.Mode = BindingMode.OneWay;

            Binding bindType = new Binding("Type");
            bindType.Mode = BindingMode.OneWay;

            Binding bindFlagReserved = new Binding("FlagReserved");
            bindFlagReserved.Mode = BindingMode.OneWay;

            Binding bindDescription = new Binding("Description");
            bindDescription.Mode = BindingMode.OneWay;

            colTag.Binding = bindTag;
            colType.Binding = bindType;
            colFlagReserved.Binding = bindFlagReserved;
            colDescription.Binding = bindDescription;
            /*
                BindingSource BS = new BindingSource();
                DataTable testTable = new DataTable();
                testTable.Columns.Add("Column1", typeof(int));
                testTable.Columns.Add("Column2", typeof(string));
                testTable.Columns.Add("Column3", typeof(string));
                testTable.Rows.Add(1, "Value1", "Test1");
                testTable.Rows.Add(2, "Value2", "Test2");
                testTable.Rows.Add(2, "Value2", "Test1");
                testTable.Rows.Add(3, "Value3", "Test3");
                testTable.Rows.Add(4, "Value4", "Test4");
                testTable.Rows.Add(4, "Value4", "Test3");
                DataView view = testTable.DefaultView;
                view.Sort = "Column2 ASC, Column3 ASC";
             // Sorting Column 2 and column 3
                BS.DataSource = view;
                DataGridViewTextBoxColumn textColumn0 = new DataGridViewTextBoxColumn();
                textColumn0.DataPropertyName = "Column1";
                dataGridView1.Columns.Add(textColumn0);
                textColumn0.SortMode = DataGridViewColumnSortMode.Programmatic;
                textColumn0.HeaderCell.SortGlyphDirection = SortOrder.None;
                DataGridViewTextBoxColumn textColumn1 = new DataGridViewTextBoxColumn();
                textColumn1.DataPropertyName = "Column2";
                dataGridView1.Columns.Add(textColumn1);
                textColumn1.SortMode = DataGridViewColumnSortMode.Programmatic;
                textColumn1.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                DataGridViewTextBoxColumn textColumn2 = new DataGridViewTextBoxColumn();
                textColumn2.DataPropertyName = "Column3";
                dataGridView1.Columns.Add(textColumn2);
                textColumn2.SortMode = DataGridViewColumnSortMode.Programmatic;
                textColumn2.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                dataGridView1.DataSource = BS;
            */

            dgSeq.FontFamily = _fontProp;
            colTag.FontFamily = _fontFixed;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C o l s P J L C m d s                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Define datagrid columns for type PJL commands.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetColsPJLCmds()
        {
            DataGridTextColumn colCmd = new DataGridTextColumn();
            DataGridTextColumn colDescription = new DataGridTextColumn();

            colCmd.Header = "Command";
            colDescription.Header = "Description";

            dgSeq.Columns.Clear();
            dgSeq.Columns.Add(colCmd);
            dgSeq.Columns.Add(colDescription);

            Binding bindCmd = new Binding("Name");
            bindCmd.Mode = BindingMode.OneWay;

            Binding bindDescription = new Binding("Description");
            bindDescription.Mode = BindingMode.OneWay;

            colCmd.Binding = bindCmd;
            colDescription.Binding = bindDescription;

            dgSeq.FontFamily = _fontProp;
            colCmd.FontFamily = _fontFixed;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C o l s P M L T a g s                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Define datagrid columns for type PML Tags.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetColsPMLTags()
        {
            DataGridTextColumn colTag = new DataGridTextColumn();
            DataGridTextColumn colType = new DataGridTextColumn();
            DataGridTextColumn colDescription = new DataGridTextColumn();

            colTag.Header = "Tag value";
            colType.Header = "Type";
            colDescription.Header = "Description";

            dgSeq.Columns.Clear();
            dgSeq.Columns.Add(colTag);
            dgSeq.Columns.Add(colType);
            dgSeq.Columns.Add(colDescription);

            Binding bindTag = new Binding("Tag");
            bindTag.Mode = BindingMode.OneWay;

            Binding bindType = new Binding("Type");
            bindType.Mode = BindingMode.OneWay;

            Binding bindDescription = new Binding("Description");
            bindDescription.Mode = BindingMode.OneWay;

            colTag.Binding = bindTag;
            colType.Binding = bindType;
            colDescription.Binding = bindDescription;

            dgSeq.FontFamily = _fontProp;
            colTag.FontFamily = _fontFixed;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C o l s P r e s c r i b e C m d s                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Define datagrid columns for type Prescribe commands.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetColsPrescribeCmds()
        {
            DataGridTextColumn colCmd = new DataGridTextColumn();
            DataGridTextColumn colDescription = new DataGridTextColumn();

            colCmd.Header = "Command";
            colDescription.Header = "Description";

            dgSeq.Columns.Clear();
            dgSeq.Columns.Add(colCmd);
            dgSeq.Columns.Add(colDescription);

            Binding bindCmd = new Binding("Name");
            bindCmd.Mode = BindingMode.OneWay;

            Binding bindDescription = new Binding("Description");
            bindDescription.Mode = BindingMode.OneWay;

            colCmd.Binding = bindCmd;
            colDescription.Binding = bindDescription;

            dgSeq.FontFamily = _fontProp;
            colCmd.FontFamily = _fontFixed;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C o l s S y m b o l S e t s                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Define datagrid columns for type Symbol Sets.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void SetColsSymbolSets()
        {
            DataGridTextColumn colGroup = new DataGridTextColumn();
            DataGridTextColumn colType = new DataGridTextColumn();
            DataGridTextColumn colId = new DataGridTextColumn();
            DataGridTextColumn colKind1 = new DataGridTextColumn();
            DataGridTextColumn colAlias = new DataGridTextColumn();
            DataGridTextColumn colName = new DataGridTextColumn();
            DataGridTextColumn colMapStd = null;
            DataGridTextColumn colMapPCL = null;
            DataGridTextColumn colMapDiff = null;
            DataGridCheckBoxColumn colFlagMapStd = null;
            DataGridCheckBoxColumn colFlagMapPCL = null;

            bool showMaps = _flagSymSetMap;

            if (showMaps)
            {
                if (_symSetMapType == SymSetMapType.Both)
                {
                    colMapStd = new DataGridTextColumn();
                    colMapStd.Header = "Mapping (Strict)";

                    colMapPCL = new DataGridTextColumn();
                    colMapPCL.Header = "Mapping (LaserJet)";

                    colMapDiff = new DataGridTextColumn();
                    colMapDiff.Header = "Mapping (difference)";
                }
                else if (_symSetMapType == SymSetMapType.PCL)
                {
                    colMapPCL = new DataGridTextColumn();
                    colMapPCL.Header = "Mapping (LaserJet)";
                }
                else
                {
                    colMapStd = new DataGridTextColumn();
                    colMapStd.Header = "Mapping (Strict)";
                }
            }
            else
            {
                colFlagMapStd = new DataGridCheckBoxColumn();
                colFlagMapStd.Header = "Map (Strict)?";
                colFlagMapPCL = new DataGridCheckBoxColumn();
                colFlagMapPCL.Header = "Map (Laserjet)?";
            }

            colGroup.Header = "Group";
            colType.Header = "Type";
            colId.Header = "PCL ID";
            colKind1.Header = "Kind1";
            colAlias.Header = "Alias";
            colName.Header = "Name";

            dgSeq.Columns.Clear();
            dgSeq.Columns.Add(colGroup);
            dgSeq.Columns.Add(colType);
            dgSeq.Columns.Add(colId);
            dgSeq.Columns.Add(colKind1);
            dgSeq.Columns.Add(colAlias);
            dgSeq.Columns.Add(colName);

            if (showMaps)
            {
                if (_symSetMapType == SymSetMapType.Both)
                {
                    dgSeq.Columns.Add(colMapStd);
                    dgSeq.Columns.Add(colMapPCL);
                    dgSeq.Columns.Add(colMapDiff);
                }
                else if (_symSetMapType == SymSetMapType.PCL)
                {
                    dgSeq.Columns.Add(colMapPCL);
                }
                else
                {
                    dgSeq.Columns.Add(colMapStd);
                }
            }
            else
            {
                dgSeq.Columns.Add(colFlagMapStd);
                dgSeq.Columns.Add(colFlagMapPCL);
            }

            Binding bindGroup = new Binding("Groupname");
            bindGroup.Mode = BindingMode.OneWay;

            Binding bindType = new Binding("TypeDescShort");
            bindType.Mode = BindingMode.OneWay;

            Binding bindId = new Binding("Id");
            bindId.Mode = BindingMode.OneWay;

            Binding bindKind1 = new Binding("Kind1");
            bindKind1.Mode = BindingMode.OneWay;

            Binding bindAlias = new Binding("Alias");
            bindAlias.Mode = BindingMode.OneWay;

            Binding bindName = new Binding("Name");
            bindName.Mode = BindingMode.OneWay;

            if (showMaps)
            {
                if (_symSetMapType == SymSetMapType.Both)
                {
                    var bindMapStd = new Binding("MappingStd");
                    var bindMapPCL = new Binding("MappingPCL");
                    var bindMapDiff = new Binding("MappingDiff");

                    bindMapStd.Mode = BindingMode.OneWay;
                    bindMapPCL.Mode = BindingMode.OneWay;
                    bindMapDiff.Mode = BindingMode.OneWay;

                    colMapStd.Binding = bindMapStd;
                    colMapPCL.Binding = bindMapPCL;
                    colMapDiff.Binding = bindMapDiff;
                }
                else if (_symSetMapType == SymSetMapType.PCL)
                {
                    var bindMapPCL = new Binding("MappingPCL");
                    bindMapPCL.Mode = BindingMode.OneWay;
                    colMapPCL.Binding = bindMapPCL;
                }
                else
                {
                    var bindMapStd = new Binding("MappingStd");
                    bindMapStd.Mode = BindingMode.OneWay;
                    colMapStd.Binding = bindMapStd;
                }
            }
            else
            {
                var bindFlagMapStd = new Binding("FlagMapStd");
                bindFlagMapStd.Mode = BindingMode.OneWay;
                colFlagMapStd.Binding = bindFlagMapStd;

                var bindFlagMapPCL = new Binding("FlagMapPCL");
                bindFlagMapPCL.Mode = BindingMode.OneWay;
                colFlagMapPCL.Binding = bindFlagMapPCL;
            }

            colGroup.Binding = bindGroup;
            colType.Binding = bindType;
            colId.Binding = bindId;
            colKind1.Binding = bindKind1;
            colAlias.Binding = bindAlias;
            colName.Binding = bindName;

            dgSeq.FontFamily = _fontProp;
            colGroup.FontFamily = _fontFixed;
            colType.FontFamily = _fontFixed;
            colId.FontFamily = _fontFixed;
            colKind1.FontFamily = _fontFixed;
            colAlias.FontFamily = _fontFixed;

            if (showMaps)
            {
                if (_symSetMapType == SymSetMapType.Both)
                {
                    colMapStd.FontFamily = _fontFixed;
                    colMapPCL.FontFamily = _fontFixed;
                    colMapDiff.FontFamily = _fontFixed;
                }
                else if (_symSetMapType == SymSetMapType.PCL)
                {
                    colMapPCL.FontFamily = _fontFixed;
                }
                else
                {
                    colMapStd.FontFamily = _fontFixed;
                }
            }
        }
    }
}