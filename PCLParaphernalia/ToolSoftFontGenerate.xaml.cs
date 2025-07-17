using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PCLParaphernalia;

/// <summary>
/// Interaction logic for ToolSoftFontGenerate.xaml
/// 
/// Class handles the SoftFontGenerate tool form.
/// 
/// © Chris Hutchinson 2011
/// 
/// </summary>

[System.Reflection.Obfuscation(Feature = "renaming",
                                        ApplyToMembers = true)]

public partial class ToolSoftFontGenerate : Window
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private const string _fontRegistryRoot = "HKEY_LOCAL_MACHINE";
    private const string _fontRegistryKey =
        "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Fonts";

    private const char _defaultSymSetIdAlpha = 'N';
    private const ushort _defaultSymSetIdNum = 0;
    private const ushort _defaultSymSetNo = 14;     //    0N //
    private const ushort _defaultSymSetNoSymbol = 32769;  // 1024A //
    private const ushort _symSetNoUnicode = 590;    //   18N //
    private const ushort _symSetNoUnbound = 56;     //    1X //

    private const ushort _defaultPCLTypefaceNo = 61440;
    private const ushort _defaultPCLStyleNo = 0;
    private const sbyte _defaultPCLWeightNo = 0;

    private const int cSizeCharSet_8bit = 256;
    private const int cSizeCharSet_UCS_2 = 65536;

    private enum ePCLTUse : byte
    {
        Ignore,
        Use,
        Max
    };

    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private bool _flagLogVerbose = false;
    private bool _flagFormat16 = false;
    private bool _flagSegGTLastPCL = false;
    private bool _flagUsePCLT = false;
    private bool _flagVMetricsPCL = false;
    private bool _flagVMetricsPCLXL = false;

    private bool _flagSymSetNullMapPCL = false;
    private bool _flagSymSetNullMapStd = false;

    private bool _flagCharCollCompInhibitPCL = false;
    private bool _flagCharCollCompSpecificPCL = false;

    private bool _initialised = false;
    private bool _symSetUnbound = false;
    private bool _symSetMapPCL = false;
    private bool _symSetUserSet = false;
    private bool _symbolMapping = false;

    private bool _fontWithinTTC = false;

    private ToolSoftFontGenTTF _ttfHandler;

    private ToolCommonData.ePrintLang _crntPDL;

    private DataSet _dataSetLogChars;
    private DataSet _dataSetLogDonor;
    private DataSet _dataSetLogMapping;
    private DataSet _dataSetLogTarget;

    private DataTable _tableLogChars;
    private DataTable _tableLogDonor;
    private DataTable _tableLogMapping;
    private DataTable _tableLogTarget;

    private readonly ASCIIEncoding _ascii = new ASCIIEncoding();

    private int _ctTTFFonts;

    private int[] _subsetSymSets;

    private int _ctMappedSymSets = 0;
    private int _sizeCharSet;

    private int _indxSymSetSubset;
    private int _indxSymSetTarget;
    private int _indxSymSetDefault;

    private int _indxFont;
    private int _indxFontTTC;

    private ushort _styleNoPCL;

    private sbyte _weightNoPCL;

    private ushort _symSetNoTargetPCL;
    private ushort _symSetNoTargetPCLXL;
    private ushort _symSetNoUserSet;
    private ushort _symSetNoPCL;
    private ushort _symSetNoPCLXL;

    private ushort _typefaceNoPCL;
    private ushort _typefaceVendorPCL = 0;
    private ushort _typefaceBasecodePCL = 0;

    private ulong _charCollCompPCL;
    private ulong _charCollCompPCLAll;
    private ulong _charCollCompPCLSpecific;

    private PCLSymbolSets.eSymSetGroup _symSetGroup;
    private PCLSymSetTypes.eIndex _symSetType;

    private string _fontNameBase;
    private string _fontNamePCLXL;
    private string _fontNameTTF;

    private string _fontFolderPCL;
    private string _fontFolderPCLXL;

    private string _fontFilenamePCL;
    private string _fontFilenamePCLXL;
    private string _fontFilenameTTF;
    private string _fontFileAdhocTTF;
    private string _symSetUserFile;

    private string _fontsFolder;

    private string[] _fontFiles;
    private string[] _fontNames;
    private string[] _fontTTCNames;

    private uint[] _fontTTCOffsets;

    private bool _tabPCLTPresent;
    private bool _tabvmtxPresent;

    private ObservableCollection<PCLCharCollItem> _charCollCompListUnicode =
                new ObservableCollection<PCLCharCollItem>();

    //--------------------------------------------------------------------//
    //                                              C o n s t r u c t o r //
    // T o o l S o f t F o n t G e n e r a t e                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public ToolSoftFontGenerate(ref ToolCommonData.ePrintLang crntPDL)
    {
        InitializeComponent();

        initialise();

        crntPDL = _crntPDL;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // b t n L o g S a v e _ C l i c k                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the 'Save log' button is clicked.                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void btnLogSave_Click(object sender, EventArgs e)
    {
        bool flagOptRptWrap = false;

        ReportCore.eRptFileFmt rptFileFmt = ReportCore.eRptFileFmt.NA;
        ReportCore.eRptChkMarks rptChkMarks = ReportCore.eRptChkMarks.NA;

        TargetCore.MetricsReturnFileRpt(
            ToolCommonData.eToolIds.SoftFontGenerate,
            ref rptFileFmt,
            ref rptChkMarks,
            ref flagOptRptWrap);

        if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            ToolSoftFontGenReport.generate(rptFileFmt,
                                            rptChkMarks,
                                            _tableLogDonor,
                                            _tableLogMapping,
                                            _tableLogTarget,
                                            _tableLogChars,
                                            _fontNameTTF,
                                            _fontFilenameTTF,
                                            _fontFilenamePCL);
        else
            ToolSoftFontGenReport.generate(rptFileFmt,
                                            rptChkMarks,
                                            _tableLogDonor,
                                            _tableLogMapping,
                                            _tableLogTarget,
                                            _tableLogChars,
                                            _fontNameTTF,
                                            _fontFilenameTTF,
                                            _fontFilenamePCLXL);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // b t n P C L F o n t F i l e B r o w s e _ C l i c k                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the 'Browse' button is clicked for a PCL 'download'    //
    // font.                                                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void btnPCLFontFileBrowse_Click(object sender,
                                             RoutedEventArgs e)
    {
        bool selected;

        string filename = _fontFilenamePCL;

        selected = selectPCLFontFile(ref filename);

        if (selected)
        {
            _fontFilenamePCL = filename;
            txtPCLFontFile.Text = _fontFilenamePCL;

            setPCLFontFileAttributes();
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // b t n P C L X L F o n t F i l e B r o w s e _ C l i c k            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the 'Browse' button is clicked for a PCL XL 'download' //
    // font.                                                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void btnPCLXLFontFileBrowse_Click(object sender,
                                             RoutedEventArgs e)
    {
        bool selected;

        string filename = _fontFilenamePCLXL;

        selected = selectPCLXLFontFile(ref filename);

        if (selected)
        {
            _fontFilenamePCLXL = filename;
            txtPCLXLFontFile.Text = _fontFilenamePCLXL;

            setPCLXLFontFileAttributes();
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // b t n P C L G e n e r a t e _ C l i c k                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the 'Generate soft font file' button is clicked.       //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void btnPCLGenerate_Click(object sender, EventArgs e)
    {
        bool proceed = true;

        DateTime dateTimeNow = DateTime.Now;

        string convTextStr = "Converted from '" + _fontFilenameTTF +
                             "' (" + _ttfHandler.FontFullname +
                             ") by user '" + Environment.UserName +
                             "' (domain user '" +
                             Environment.UserDomainName +
                             "') using system '" + Environment.MachineName +
                             "' on " + dateTimeNow;

        string licenceText = string.Empty;

        byte[] conversionText;

        PCLSymSetTypes.eIndex symSetType;

        ToolSoftFontGenTTF.eLicenceType licenceType;

        conversionText = _ascii.GetBytes(convTextStr);

        licenceType = _ttfHandler.CheckLicence(ref licenceText);

        if (licenceType == ToolSoftFontGenTTF.eLicenceType.NotAllowed)
        {
            MessageBoxResult msgBoxResult =
                    MessageBox.Show(
                    "Donor TrueType font has a restrictive license:\n\n" +
                    licenceText +
                    "\n\nConversion will proceed only if you confirm " +
                    "that you are allowed to convert the font;" +
                    "\n\nDid you obtained permission from the legal owner?",
                    "Donor font licensing rights",
                     MessageBoxButton.YesNo,
                     MessageBoxImage.Warning);

            if (msgBoxResult == MessageBoxResult.Yes)
                proceed = true;
            else
                proceed = false;
        }
        else if (licenceType == ToolSoftFontGenTTF.eLicenceType.OwnerOnly)
        {
            MessageBoxResult msgBoxResult =
                MessageBox.Show(
                    "Donor TrueType font has a restrictive license:\n\n" +
                    licenceText +
                    "\n\nConversion will proceed only if you agree " +
                    "to use the converted font solely on your own system;" +
                    "\n\nDo you agree?",
                    "Donor font licensing rights",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

            if (msgBoxResult == MessageBoxResult.Yes)
                proceed = true;
            else
                proceed = false;
        }

        if (proceed)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Generate soft font.                                        //
            //                                                            //
            //------------------------------------------------------------//

            byte symSetTypeID;

            bool monoSpaced = false;

            _tableLogTarget.Clear();
            _tableLogChars.Clear();

            symSetType = PCLSymbolSets.GetType(_indxSymSetTarget);
            symSetTypeID = PCLSymSetTypes.GetIdPCL((int)symSetType);

            if (tabDetails.SelectedItem.Equals(tabPCL))
            {
                ToolSoftFontGenPCL PCLHandler =
                    new ToolSoftFontGenPCL(_tableLogChars,
                                            _ttfHandler);

                PCLHandler.GenerateFont(ref _fontFilenamePCL,
                                         ref monoSpaced,
                                         _symbolMapping,
                                         _flagFormat16,
                                         _flagSegGTLastPCL,
                                         _flagUsePCLT,
                                         _symSetUnbound,
                                         _tabvmtxPresent,
                                         _flagVMetricsPCL,
                                         symSetTypeID,
                                         _sizeCharSet,
                                         _symSetNoPCL,
                                         _styleNoPCL,
                                         _weightNoPCL,
                                         _typefaceNoPCL,
                                         _charCollCompPCL,
                                         conversionText);

                txtPCLFontFile.Text = _fontFilenamePCL;

                logFontSelectDataPCL(monoSpaced);

                setPCLFontFileAttributes();
            }
            else if (tabDetails.SelectedItem.Equals(tabPCLXL))
            {
                ToolSoftFontGenPCLXL PCLXLHandler =
                    new ToolSoftFontGenPCLXL(_tableLogChars,
                                              _ttfHandler);

                byte[] fontName =
                    new byte[ToolSoftFontGenTTF.cSizeFontname];

                // perhaps need to use .GetByteCount and/or .GetMaxByteCount
                // and different overload of GetBytes to make sure that
                // Byte [] is not overflowed.

                fontName = _ascii.GetBytes(_fontNamePCLXL);

                PCLXLHandler.GenerateFont(ref _fontFilenamePCLXL,
                                          _symbolMapping,
                                          _symSetUnbound,
                                          _tabvmtxPresent,
                                          _flagVMetricsPCLXL,
                                          fontName,
                                          _sizeCharSet,
                                          _symSetNoPCLXL,
                                          conversionText);

                txtPCLXLFontFile.Text = _fontFilenamePCLXL;

                logFontSelectDataPCLXL();

                setPCLXLFontFileAttributes();
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // b t n R e s e t _ C l i c k                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the 'Reset' button is clicked.                         //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void btnReset_Click(object sender, EventArgs e)
    {
        resetFormState();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // b t n S y m S e t F i l e B r o w s e _ C l i c k                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Activated when the Browse button on the User-defined symbol set    //
    // file panel is clicked.                                             //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void btnSymSetFileBrowse_Click(object sender,
                                            RoutedEventArgs e)
    {
        bool selected;

        string filename = _symSetUserFile;

        selected = selectSymSetFile(ref filename);

        if (selected)
        {
            _symSetUserFile = filename;
            txtSymSetFile.Text = _symSetUserFile;

            CheckPCLSymSetFile();

            setSymSetAttributesTarget();
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // b t n T T F F o n t F i l e B r o w s e _ C l i c k                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Activated when the Browse button on the TrueType Font select panel //
    // is clicked.                                                        //
    //                                                                    //
    // Invoke File/Open dialogue to select target TTF file.               //
    // Note that this will only be done when the selected font is the     //
    // special value <font not installed>.                                //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void btnTTFFontFileBrowse_Click(object sender,
                                            RoutedEventArgs e)
    {
        bool selected;

        string filename = _fontFileAdhocTTF;

        selected = selectTTFFontFile(ref filename);

        if (selected)
        {
            _fontFileAdhocTTF = filename;
            _fontFilenameTTF = filename;
            txtTTFFile.Text = _fontFilenameTTF;

            resetFormState();
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // b t n T T F S e l e c t _ C l i c k                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the 'Select donor font' button is clicked.             //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void btnTTFSelect_Click(object sender, EventArgs e)
    {
        bool flagOK = true;

        int indx;
        int sfntOffset = 0;

        ushort symSetNoTargetPCL;
        ushort symSetNoTargetPCLXL;
        uint numFonts = 0;

        string filename,
               symSetIdTargetPCL,
               symSetIdTargetPCLXL;

        bool typeTTC = false,
                usePCLT = false;

        if (_fontWithinTTC)
        {
            indx = cbTTCName.SelectedIndex;

            sfntOffset = (int)_fontTTCOffsets[indx];

            ToolSoftFontGenLog.LogNameAndValue(
                _tableLogDonor, true, true,
                "Font",
                "'" + _fontTTCNames[indx].ToString() +
                "' selected from '" + _fontFilenameTTF + "' collection");
        }
        else
        {
            filename = _fontFilenameTTF;

            indx = filename.LastIndexOf("\\");

            if (indx == -1)
                _fontNameBase = filename;
            else
                _fontNameBase = filename.Substring(indx + 1);

            indx = _fontNameBase.LastIndexOf(".");

            if (indx != -1)
            {
                _fontNameBase = _fontNameBase.Substring(0, indx);
            }

            btnTTFSelect.IsEnabled = false;
            grpBinding.IsEnabled = false;
            grpSymSet.IsEnabled = false;
            grpSymSetFile.IsEnabled = false;
            grpSymSetMapType.IsEnabled = false;
            chkLogVerbose.IsEnabled = false;

            _tableLogDonor.Clear();
            _tableLogMapping.Clear();
            _tableLogTarget.Clear();
            _tableLogChars.Clear();

            if (_symSetUnbound)
                _sizeCharSet = cSizeCharSet_UCS_2;
            else if (_symSetUserSet)
                _sizeCharSet =
                    PCLSymbolSets.GetMapArrayMax(_indxSymSetTarget) + 1;
            else
                _sizeCharSet =
                    PCLSymbolSets.GetMapArrayMax(_indxSymSetTarget) + 1;

            _ttfHandler = new ToolSoftFontGenTTF(_tableLogDonor,
                                                  _tableLogMapping,
                                                  _flagLogVerbose,
                                                  _sizeCharSet);

            flagOK = _ttfHandler.CheckForTTC(_fontFilenameTTF,
                                              ref typeTTC,
                                              ref numFonts);
            if (flagOK)
            {
                if (typeTTC)
                {
                    _fontWithinTTC = true;

                    MessageBox.Show(
                        "Donor TrueType font is a Collection file\n\n" +
                        "Select component font & try again",
                        "Donor font type",
                         MessageBoxButton.OK,
                         MessageBoxImage.Information);

                    _fontTTCOffsets = new uint[numFonts];
                    _fontTTCNames = new string[numFonts];

                    flagOK = _ttfHandler.GetTTCData(_fontFilenameTTF,
                                                     numFonts,
                                                     ref _fontTTCOffsets,
                                                     ref _fontTTCNames);

                    if (flagOK)
                    {
                        cbTTCName.Items.Clear();

                        for (int i = 0; i < numFonts; i++)
                        {
                            cbTTCName.Items.Add(_fontTTCNames[i]);
                        }

                        cbTTCName.SelectedIndex = 0;

                        lbTTCFont.Visibility = Visibility.Visible;
                        cbTTCName.Visibility = Visibility.Visible;

                        btnTTFSelect.IsEnabled = true;
                    }
                }
            }
        }

        if (flagOK && !typeTTC)
        {
            ushort symSetNoPCLT = 0;
            ushort typefaceNoPCLT = 0;
            ushort styleNoPCLT = 0;

            ulong charCompPCLT = 0;

            sbyte weightNoPCLT = 0;

            //   Byte [] typefacePCLT = new Byte [ToolSoftFontGenPCLXL.cSizeFontname];

            string typefacePCLT = string.Empty;

            if (!_fontWithinTTC)
            {
                lbTTCFont.Visibility = Visibility.Hidden;
                cbTTCName.Visibility = Visibility.Hidden;
            }

            _ttfHandler.InitialiseFontData(_fontFilenameTTF,
                                            sfntOffset,
                                            _indxSymSetTarget,
                                            ref _tabPCLTPresent,
                                            ref _tabvmtxPresent,
                                            ref _symbolMapping,
                                            _symSetUnbound,
                                            _symSetUserSet,
                                            _symSetMapPCL);

            //--------------------------------------------------------//
            //                                                        //
            // Modify fields according to whether the TrueType font   //
            // uses Symbol encoding or Unicode encoding.              //
            //                                                        //
            //--------------------------------------------------------//

            if (_symbolMapping)
            {
                rbSymSetBound.IsChecked = true;
                _symSetUnbound = false;

                grpSymSet.Visibility = Visibility.Hidden;
                grpSymSetFile.Visibility = Visibility.Hidden;

                lbSymSetSymbol.Visibility = Visibility.Visible;

                grpSymSetMapType.Visibility = Visibility.Hidden;

                symSetNoTargetPCL = _defaultSymSetNoSymbol;
                symSetNoTargetPCLXL = _defaultSymSetNoSymbol;
            }
            else
            {
                if (_symSetUnbound)
                {
                    grpSymSet.Visibility = Visibility.Hidden;

                    symSetNoTargetPCL = _symSetNoUnbound;
                    symSetNoTargetPCLXL = _symSetNoUnicode;
                }
                else
                {
                    grpSymSet.Visibility = Visibility.Visible;

                    symSetNoTargetPCL = _symSetNoTargetPCL;
                    symSetNoTargetPCLXL = _symSetNoTargetPCLXL;
                }

                lbSymSetSymbol.Visibility = Visibility.Hidden;
            }

            symSetIdTargetPCL =
                PCLSymbolSets.TranslateKind1ToId(symSetNoTargetPCL);
            symSetIdTargetPCLXL =
               PCLSymbolSets.TranslateKind1ToId(symSetNoTargetPCLXL);

            //--------------------------------------------------------//
            //                                                        //
            // Set up default output soft font filenames, based on:   //
            //  -   the stored fontsFolder (PCL or PCLXL);            //
            //  -   the terminal name (excluding extension) of the    //
            //      donor TrueType font file;                         //
            //  -   the target symbol set identifier;                 //
            //  -   the extension (.sft or .sfx, for PCL or PCLXL).   //
            //                                                        //
            //--------------------------------------------------------//

            _fontFilenamePCL = _fontFolderPCL + "\\" +
                                _fontNameBase + "_" +
                                symSetIdTargetPCL + ".sft";
            _fontFilenamePCLXL = _fontFolderPCLXL + "\\" +
                                _fontNameBase + "_" +
                                symSetIdTargetPCLXL + ".sfx";

            //--------------------------------------------------------//
            //                                                        //
            // Modify fields according to presence or absence of PCLT //
            // table in TrueType font.                                //
            //                                                        //
            //--------------------------------------------------------//

            grpPCLTTreatment.IsEnabled = false; // re-enabled when another font or Reset

            if (_tabPCLTPresent)
            {
                lbPCLTNotPresent.Visibility = Visibility.Hidden;

                rbPCLTIgnore.Visibility = Visibility.Visible; // should already be visible?
                rbPCLTUse.Visibility = Visibility.Visible;    // should already be visible?

                if (_flagUsePCLT)
                    usePCLT = true;
                else
                    usePCLT = false;
            }
            else
            {
                lbPCLTNotPresent.Visibility = Visibility.Visible;

                rbPCLTIgnore.Visibility = Visibility.Hidden;
                rbPCLTUse.Visibility = Visibility.Hidden;
                usePCLT = false;
            }

            _ttfHandler.GetPCLFontSelectData(ref _styleNoPCL,
                                              ref _weightNoPCL,
                                              ref symSetNoPCLT,
                                              ref styleNoPCLT,
                                              ref weightNoPCLT,
                                              ref typefaceNoPCLT,
                                              ref typefacePCLT,
                                              ref charCompPCLT);

            _typefaceNoPCL = _defaultPCLTypefaceNo;

            //--------------------------------------------------------//

            if (usePCLT)
            {
                _fontNamePCLXL = typefacePCLT.ToString();

                if (!_symSetUnbound)
                {
                    if (symSetNoPCLT == 0)
                    {
                        _symSetNoPCL = symSetNoTargetPCL;
                        _symSetNoPCLXL = symSetNoTargetPCLXL;
                    }
                    else
                    {
                        _symSetNoPCL = symSetNoPCLT;
                        _symSetNoPCLXL = symSetNoPCLT;
                    }
                }

                _styleNoPCL = styleNoPCLT;
                _weightNoPCL = weightNoPCLT;
                _typefaceNoPCL = typefaceNoPCLT;

                _charCollCompPCLSpecific = charCompPCLT;
            }
            else
            {
                _fontNamePCLXL = _fontNameBase;

                if (!_symSetUnbound)
                {
                    _symSetNoPCL = symSetNoTargetPCL;
                    _symSetNoPCLXL = symSetNoTargetPCLXL;
                }
            }

            //--------------------------------------------------------//

            if (_symSetUnbound)
            {
                if (_flagCharCollCompSpecificPCL)
                    _charCollCompPCL = _charCollCompPCLSpecific;
                else
                    _charCollCompPCL = _charCollCompPCLAll;

                populatePCLCharCollComp(_charCollCompPCL);
            }

            //--------------------------------------------------------//
            //                                                        //
            // Show or hide vertical metrics options according to     //
            // whether or not the donor font contains a 'vmtx' table. //
            //                                                        //
            //--------------------------------------------------------//

            if (_tabvmtxPresent)
            {
                grpPCLHddrVMetrics.Visibility = Visibility.Visible;
                grpPCLXLHddrVMetrics.Visibility = Visibility.Visible;
            }
            else
            {
                grpPCLHddrVMetrics.Visibility = Visibility.Hidden;
                grpPCLXLHddrVMetrics.Visibility = Visibility.Hidden;
            }

            //--------------------------------------------------------//
            //                                                        //
            // Set other states.                                      //
            //                                                        //
            //--------------------------------------------------------//

            txtPCLFontFile.Text = _fontFilenamePCL;
            txtPCLXLFontFile.Text = _fontFilenamePCLXL;
            txtPCLXLFontName.Text = _fontNamePCLXL;

            grpTargetFont.Visibility = Visibility.Visible;

            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                tabPCL.IsSelected = true;

                if (!_flagFormat16)
                    grpPCLHddrVMetrics.Visibility = Visibility.Hidden;
            }
            else
            {
                tabPCLXL.IsSelected = true;
            }

            btnReset.Visibility = Visibility.Visible;
            btnTTFSelect.Visibility = Visibility.Hidden;

            btnPCLGenerate.Visibility = Visibility.Visible;
            btnLogSave.Visibility = Visibility.Visible;

            grpLog.Visibility = Visibility.Visible;

            txtPCLSymSetNo.Text = _symSetNoPCL.ToString();
            txtPCLStyleNo.Text = _styleNoPCL.ToString();
            txtPCLWeightNo.Text = _weightNoPCL.ToString();
            txtPCLTypefaceNo.Text = _typefaceNoPCL.ToString();

            txtPCLXLSymSetNo.Text = _symSetNoPCLXL.ToString();
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c b P C L C h a r C o l l _ S e l e c t i o n C h a n g e d        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // SelectionChanged event handler for Character Collections           //
    // combination box.                                                   //
    // Set the selected item to null, otherwise if one of the disabled    //
    // checkbox items is clicked, details of this item appear in the      //
    // combination box header.                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    void cbPCLCharColl_SelectionChanged(object sender,
                                          SelectionChangedEventArgs e)
    {
        cbPCLCharColls.SelectedItem = null;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c b P C L C h a r C o l l I t e m _ P r o p e r t y C h a n g e d  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // PropertyChanged event handler for Character Collection combination //
    // box item.                                                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    void cbPCLCharCollItem_PropertyChanged(object sender,
                                             PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsChecked")
        {
            if (!_flagCharCollCompInhibitPCL)
                setPCLCharCollCompArray();
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c b S y m S e t _ S e l e c t i o n C h a n g e d                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Symbol Set item has changed.                                       //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void cbSymSet_SelectionChanged(object sender,
                                           SelectionChangedEventArgs e)
    {
        if (_initialised && cbSymSet.HasItems)
        {
            setSymSetAttributesTarget();
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c b T T C N a m e _ S e l e c t i o n C h a n g e d                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Invoked when the state of the TTC Name combo box changes.          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void cbTTCName_SelectionChanged(object sender,
                                            SelectionChangedEventArgs e)
    {
        _indxFontTTC = cbTTCName.SelectedIndex;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c b T T F N a m e _ S e l e c t i o n C h a n g e d                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Invoked when the state of the TTF Name combo box changes.          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void cbTTFName_SelectionChanged(object sender,
                                            SelectionChangedEventArgs e)
    {
        bool allowFontFileSelect;

        resetFormState();

        _fontWithinTTC = false;

        _indxFont = cbTTFName.SelectedIndex;

        _fontNameTTF = _fontNames[_indxFont];

        if (_indxFont == 0)
        {
            allowFontFileSelect = true;
            txtTTFFile.Text = _fontFiles[_indxFont];
        }
        else
        {
            allowFontFileSelect = false;
            if (_fontFiles[_indxFont].Contains("\\"))
                _fontFilenameTTF = _fontFiles[_indxFont];
            else
                _fontFilenameTTF = _fontsFolder + "\\" +
                                   _fontFiles[_indxFont];

            txtTTFFile.Text = _fontFilenameTTF;
        }

        if (allowFontFileSelect)
        {
            txtTTFFile.IsReadOnly = false;
            btnTTFFontFileBrowse.IsEnabled = true;
        }
        else
        {
            txtTTFFile.IsReadOnly = true;
            btnTTFFontFileBrowse.IsEnabled = false;
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h e c k P C L S y m S e t F i l e                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Check the contents of the PCL (download) symbol set file.          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void CheckPCLSymSetFile()
    {
        bool flagOK = true;

        bool selected = true;

        ushort[] symSetMap = new ushort[256];

        if (!File.Exists(_symSetUserFile))
        {
            string filename = _symSetUserFile;

            flagOK = false;

            MessageBox.Show(
                "File " + _symSetUserFile + " does not exist",
                "Symbol Set definition file",
                 MessageBoxButton.OK,
                 MessageBoxImage.Information);

            //------------------------------------------------------------//
            //                                                            //
            // Invoke File/Open dialogue to select Symbol set file.       //
            //                                                            //
            //------------------------------------------------------------//

            selected = selectSymSetFile(ref filename);

            if (selected)
            {
                _symSetUserFile = filename;
                txtSymSetFile.Text = _symSetUserFile;
            }
        }

        if (selected)
        {
            ushort firstCode = 0,
                   lastCode = 0;

            PCLSymSetTypes.eIndex symSetType =
                PCLSymSetTypes.eIndex.Unknown;

            flagOK = PCLDownloadSymSet.CheckSymSetFile(
                _symSetUserFile,
                ref _symSetNoUserSet,
                ref firstCode,
                ref lastCode,
                ref symSetType);    // not used here at present
        }

        if (flagOK)
        {
            //  PCLSymbolSets.setDataUserSet (_symSetNoUserSet, symSetMap); // already done by check...
        }
        else
        {
            _symSetNoUserSet = _defaultSymSetNo;
            PCLSymbolSets.SetDataUserSetDefault(_defaultSymSetNo);

            txtSymSetFile.Text = "***** Invalid symbol set file *****";
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h k L o g V e r b o s e _ C h e c k e d                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // The 'log verbose' checkbox has been checked.                       //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void chkLogVerbose_Checked(object sender, RoutedEventArgs e)
    {
        _flagLogVerbose = true;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h k L o g V e r b o s e _ U n c h e c k e d                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // The 'log verbose' checkbox has been unchecked.                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void chkLogVerbose_Unchecked(object sender, RoutedEventArgs e)
    {
        _flagLogVerbose = false;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h k P C L S e g G T L a s t _ C h e c k e d                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // The PCL 'segment GT last' checkbox has been checked.               //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void chkPCLSegGTLast_Checked(object sender, RoutedEventArgs e)
    {
        _flagSegGTLastPCL = true;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h k P C L S e g G T L a s t _ U n c h e c k e d                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // The PCL 'segment GT last' checkbox has been unchecked.             //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void chkPCLSegGTLast_Unchecked(object sender, RoutedEventArgs e)
    {
        _flagSegGTLastPCL = false;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h k P C L V M e t r i c s _ C h e c k e d                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // The PCL 'vertical metrics' checkbox has been checked.              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void chkPCLVMetrics_Checked(object sender,
                                          RoutedEventArgs e)
    {
        _flagVMetricsPCL = true;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h k P C L V M e t r i c s _ U n c h e c k e d                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // The PCL 'vertical metrics' checkbox has been unchecked.            //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void chkPCLVMetrics_Unchecked(object sender,
                                            RoutedEventArgs e)
    {
        _flagVMetricsPCL = false;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h k P C L X L V M e t r i c s _ C h e c k e d                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // The PCLXL 'vertical metrics' checkbox has been checked.            //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void chkPCLXLVMetrics_Checked(object sender,
                                          RoutedEventArgs e)
    {
        _flagVMetricsPCLXL = true;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h k P C L X L V M e t r i c s _ U n c h e c k e d                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // The PCLXL 'vertical metrics' checkbox has been unchecked.          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void chkPCLXLVMetrics_Unchecked(object sender,
                                            RoutedEventArgs e)
    {
        _flagVMetricsPCLXL = false;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g i v e C r n t P D L                                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    public void giveCrntPDL(ref ToolCommonData.ePrintLang crntPDL)
    {
        crntPDL = _crntPDL;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i n i t i a l i s e                                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Initialisation.                                                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void initialise()
    {
        _initialised = false;

        _fontFilenameTTF = string.Empty;

        _fontWithinTTC = false;

        //----------------------------------------------------------------//
        //                                                                //
        // Populate form.                                                 //
        //                                                                //
        //----------------------------------------------------------------//

        initialiseFontList();

        initialiseSymSetList();

        initialiseLogGridDonor();
        initialiseLogGridMapping();
        initialiseLogGridTarget();
        initialiseLogGridChars();

        //----------------------------------------------------------------//

        initialisePCLCharCollCompLists();

        //----------------------------------------------------------------//

        btnPCLGenerate.Content = "Generate soft font file";

        //----------------------------------------------------------------//
        //                                                                //
        // Reinstate settings from persistent storage.                    //
        //                                                                //
        //----------------------------------------------------------------//

        metricsLoad();

        cbTTFName.SelectedIndex = _indxFont;
        cbSymSet.SelectedIndex = _indxSymSetSubset;

        txtSymSetFile.Text = _symSetUserFile;

        if (_symSetMapPCL)
            rbMapSymSetPCL.IsChecked = true;
        else
            rbMapSymSetStd.IsChecked = true;

        //----------------------------------------------------------------//

        if (_symSetUnbound)
        {
            rbSymSetUnbound.IsChecked = true;

            grpPCLCharComp.Visibility = Visibility.Visible;
            grpSymSetMapType.Visibility = Visibility.Hidden;
        }
        else if (_symSetUserSet)
        {
            rbSymSetUserSet.IsChecked = true;

            grpPCLCharComp.Visibility = Visibility.Hidden;
            grpSymSetMapType.Visibility = Visibility.Hidden;

            CheckPCLSymSetFile();

            setSymSetAttributesTarget();
        }
        else
        {
            rbSymSetBound.IsChecked = true;

            grpPCLCharComp.Visibility = Visibility.Hidden;
            grpSymSetMapType.Visibility = Visibility.Visible;
        }

        setSymSetAttributesTarget();

        //----------------------------------------------------------------//

        if (_flagLogVerbose)
            chkLogVerbose.IsChecked = true;
        else
            chkLogVerbose.IsChecked = false;

        //----------------------------------------------------------------//

        if (_flagFormat16)
            rbPCLHddrFmt16.IsChecked = true;
        else
            rbPCLHddrFmt15.IsChecked = true;

        //----------------------------------------------------------------//

        if (_flagSegGTLastPCL)
            chkPCLSegGTLast.IsChecked = true;
        else
            chkPCLSegGTLast.IsChecked = false;

        //----------------------------------------------------------------//

        if (_flagUsePCLT)
            rbPCLTUse.IsChecked = true;
        else
            rbPCLTIgnore.IsChecked = true;

        //----------------------------------------------------------------//

        if (_flagCharCollCompSpecificPCL)
        {
            rbPCLCharCompSpecific.IsChecked = true;
            cbPCLCharColls.Visibility = Visibility.Visible;
            tblkPCLCharCollsText.Visibility = Visibility.Visible;

            _charCollCompPCL = _charCollCompPCLSpecific;

            populatePCLCharCollComp(_charCollCompPCL);
        }
        else
        {
            rbPCLCharCompSpecific.IsChecked = false;
            cbPCLCharColls.Visibility = Visibility.Hidden;
            tblkPCLCharCollsText.Visibility = Visibility.Hidden;

            _charCollCompPCL = _charCollCompPCLAll;
        }

        //----------------------------------------------------------------//

        if (_flagVMetricsPCL)
            chkPCLVMetrics.IsChecked = true;
        else
            chkPCLVMetrics.IsChecked = false;

        if (_flagVMetricsPCLXL)
            chkPCLXLVMetrics.IsChecked = true;
        else
            chkPCLXLVMetrics.IsChecked = false;

        //----------------------------------------------------------------//

        setPCLCharCollCompValue(_charCollCompPCL);

        grpPCLHddrVMetrics.Visibility = Visibility.Hidden;
        grpPCLXLHddrVMetrics.Visibility = Visibility.Hidden;

        lbTTCFont.Visibility = Visibility.Hidden;
        cbTTCName.Visibility = Visibility.Hidden;

        if (_crntPDL == ToolCommonData.ePrintLang.PCLXL)
            tabDetails.SelectedItem = tabPCLXL;
        else
            tabDetails.SelectedItem = tabPCL;

        _initialised = true;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i n i t i a l i s e F o n t L i s t                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Populate list of installed fonts and corresponding list of font    //
    // file names.                                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void initialiseFontList()
    {
        //----------------------------------------------------------------//
        //                                                                //
        // Ideally, we should do this using standard API functions, but   //
        // there does not appear to be any standard method which produces //
        // this information - see extensive notes at end of this method.  //
        //                                                                //
        // So we do this instead by interrogating a 'well-known' registry //
        // key; the location of the font files is obtained using one of   //
        // the system environment variables.                              //
        //                                                                //
        //----------------------------------------------------------------//

        try
        {
            _fontsFolder =
                Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

            RegistryKey keyMain = Registry.LocalMachine;

            RegistryKey key = keyMain.OpenSubKey(_fontRegistryKey);

            _ctTTFFonts = 0;

            SortedList<string, string> fontList =
                new SortedList<string, string>();

            //------------------------------------------------------------//
            //                                                            //
            // We only want:                                              //
            //      .TTF    TrueType Font                                 // 
            //      .OTF    OpenType Font                                 //
            //      .TTC    TrueType Font Collection                      //
            // not  .FON    etc.                                          //
            //                                                            //
            //------------------------------------------------------------//

            foreach (string valueName in key.GetValueNames())
            {
                string value = key.GetValue(valueName).ToString();

                if ((value.EndsWith(".ttf")) || (value.EndsWith(".TTF"))
                                              ||
                    (value.EndsWith(".otf")) || (value.EndsWith(".OTF")))
                {
                    _ctTTFFonts++;

                    fontList.Add(valueName, value);
                }
                else if ((value.EndsWith(".ttc")) || (value.EndsWith(".TTC")))
                {
                    _ctTTFFonts++;

                    fontList.Add(valueName, value);
                }
            }

            _fontFiles = new string[_ctTTFFonts + 1];
            _fontNames = new string[_ctTTFFonts + 1];

            int indexFontFiles = 1;

            cbTTFName.Items.Clear();

            cbTTFName.Items.Add("< Choose font file >");

            _fontFiles[0] =
                "***** type or browse for required font file name *****";

            const string ttfDesc = "(TrueType)";
            int ttfDescLen = ttfDesc.Length;

            //------------------------------------------------------------//
            //                                                            //
            // Populate arrays and drop-down box.                         //
            //                                                            //
            // Remove "(TrueType)" from the font name if present.         //
            //                                                            //
            //------------------------------------------------------------//

            foreach (string valueName in fontList.Keys)
            {
                string value = key.GetValue(valueName).ToString();
                string name;

                int len = valueName.Length;
                int indx;

                if (valueName.EndsWith(ttfDesc))
                {
                    if (valueName.EndsWith(" " + ttfDesc))
                        indx = len - ttfDescLen - 1;
                    else
                        indx = len - ttfDescLen;

                    name = valueName.Substring(0, indx);
                }
                else
                {
                    name = valueName;
                }

                cbTTFName.Items.Add(name);

                _fontNames[indexFontFiles] = name;
                _fontFiles[indexFontFiles] = value;

                indexFontFiles++;
            }

            key.Close();
            keyMain.Close();
        }
        catch
        {
            MessageBox.Show("Unable to retrieve font file data from" +
                             " registry",
                             "Soft Font Generate",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Notes on alternative methods:                                  //
        //                                                                //
        //----------------------------------------------------------------//

        //----------------------------------------------------------------//
        //                                                                //
        // Enumerate the current set of system font families.             //
        // Note:                                                          //
        // (a) This doesn't return the standard font names.               //
        //     e.g. 'Arial' is returned, but not 'Arial Narrow' or        //
        //          'Arial Black'; the implication is that 'Arial'        //
        //          includes both of these (sub-?) families.              //
        // (b) The names are returned unsorted.                           //
        //                                                                //
        //----------------------------------------------------------------//
        /*
        {
            cbTTFFont.Items.Clear ();

            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                // FontFamily.Source contains the font family name.
                cbTTFFont.Items.Add (fontFamily.Source);
            }

            cbTTFFont.SelectedIndex = 0;
        }
        */
        //----------------------------------------------------------------//
        //                                                                //
        // Enumerate the current set of system typefaces.                 //
        // For each typeface, return the font family name, and stretch,   //
        // weight, and style values.                                      //
        //                                                                //
        // Note:                                                          //
        // (a) This doesn't return the standard font names.               //
        //     e.g. 'Arial Narrow Regular' appears to be reported as      //
        //          family='Arial'                                        //   
        //          stretch='Condensed'                                   //   
        //          weight='Normal'                                       //   
        //          style='Normal'                                        //   
        // (b) The names are returned unsorted.                           //
        // (c) It appears to return typeface variants which don't exist.  //
        //     e.g. Oblique, Bold and Bold Oblique variants of Symbol.    //
        //                                                                //
        // e.g. first entries on local Windows 7 system are:              //
        //                                                                //
        //  family      stretch     weight      style                     //
        //  ----------  ---------   ---------   ---------                 //
        //  Arial       Normal      Normal      Normal                    //
        //  Arial       Condensed   Normal      Normal                    //
        //  Arial       Normal      Normal      Italic                    //
        //  Arial       Condensed   Normal      Italic                    //
        //  Arial       Normal      Bold        Normal                    //
        //  Arial       Condensed   Bold        Normal                    //
        //  Arial       Normal      Bold        Italic                    //
        //  Arial       Condensed   Bold        Italic                    //
        //  Arial       Normal      Black       Normal                    //
        //  Arial       Normal      Normal      Oblique                   //
        //  Arial       Condensed   Normal      Oblique                   //
        //  Arial       Normal      Bold        Oblique                   //
        //  Arial       Condensed   Bold        Oblique                   //
        //  Arial       Normal      Black       Oblique                   //
        //   ... followed by entries for:                                 //
        //  Batang      ...                                               //
        //  BatangChe   ...                                               //
        //  Gungsuh     ...                                               //
        //  GunhsuhChe  ...                                               //
        //  Courier New ...                                               //
        //                                                                //
        //                                                                //
        //----------------------------------------------------------------//
        /*
        {
            cbTTFTypeface.Items.Clear ();

            foreach (Typeface typeface in Fonts.SystemTypefaces)
            {
                // Note that instead of selecting SystemTypefaces, it is  //
                // possible to get a collection of typefaces held in a    //
                // specified URI location; e.g.:                          //
                //   String fontFileURI = "file:///C:\\Windows\\Fonts";   //
                //   System.Collections.Generic.ICollection<Typeface>     //
                //       typefaces = Fonts.GetTypefaces (fontFileURI);    //

                // FontFamily.Source value is (usually) returned as an    //
                // of two String elements: directory source; font family. //
                // We only need the second of these.                      //
                
                string[] familyName = typeface.FontFamily.Source.Split ('#');

                // Return the font family name, and stretch, weight, and  //
                // style values to the typeface combo box.                //

                cbTTFTypeface.Items.Add (familyName[familyName.Length - 1] +
                                         " " + typeface.Stretch +
                                         " " + typeface.Weight  +
                                         " " + typeface.Style);
            }

            cbTTFTypeface.SelectedIndex = 0;
        }
        */
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i n i t i a l i s e L o g G r i d C h a r s                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Initialises 'Chars' dataset and associate with grid.               //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void initialiseLogGridChars()
    {
        _dataSetLogChars = new DataSet();
        _tableLogChars = new DataTable("Log Chars");

        _tableLogChars.Columns.Add("DecCode", typeof(int));
        _tableLogChars.Columns.Add("HexCode", typeof(string));
        _tableLogChars.Columns.Add("Unicode", typeof(string));
        _tableLogChars.Columns.Add("Glyph", typeof(int));
        _tableLogChars.Columns.Add("Abs", typeof(bool));
        _tableLogChars.Columns.Add("Prev", typeof(bool));
        _tableLogChars.Columns.Add("Comp", typeof(bool));
        _tableLogChars.Columns.Add("Depth", typeof(int));
        _tableLogChars.Columns.Add("Width", typeof(int));
        _tableLogChars.Columns.Add("LSB", typeof(int));
        _tableLogChars.Columns.Add("Height", typeof(int));
        _tableLogChars.Columns.Add("TSB", typeof(int));
        _tableLogChars.Columns.Add("Length", typeof(int));

        _dataSetLogChars.Tables.Add(_tableLogChars);

        dgLogChars.DataContext = _tableLogChars;  // bind to grid
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i n i t i a l i s e L o g G r i d D o n o r                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Initialises 'Donor' dataset and associate with grid.               //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void initialiseLogGridDonor()
    {
        _dataSetLogDonor = new DataSet();
        _tableLogDonor = new DataTable("Log Donor");

        _tableLogDonor.Columns.Add("Name", typeof(string));
        _tableLogDonor.Columns.Add("Value", typeof(string));

        _dataSetLogDonor.Tables.Add(_tableLogDonor);

        dgLogDonor.DataContext = _tableLogDonor;  // bind to grid
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i n i t i a l i s e L o g G r i d M a p p i n g                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Initialises 'Mapping' dataset and associate with grid.             //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void initialiseLogGridMapping()
    {
        _dataSetLogMapping = new DataSet();
        _tableLogMapping = new DataTable("Log Mapping");

        _tableLogMapping.Columns.Add("Name", typeof(string));
        _tableLogMapping.Columns.Add("Value", typeof(string));

        _dataSetLogMapping.Tables.Add(_tableLogMapping);

        dgLogMapping.DataContext = _tableLogMapping;  // bind to grid
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i n i t i a l i s e L o g G r i d T a r g e t                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Initialises 'Target' dataset and associate with grid.              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void initialiseLogGridTarget()
    {
        _dataSetLogTarget = new DataSet();
        _tableLogTarget = new DataTable("Log Target");

        _tableLogTarget.Columns.Add("Name", typeof(string));
        _tableLogTarget.Columns.Add("Value", typeof(string));

        _dataSetLogTarget.Tables.Add(_tableLogTarget);

        dgLogTarget.DataContext = _tableLogTarget;  // bind to grid
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i n i t i a l i s e P C L C h a r C o l l C o m p L i s t s        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Initialise lists of character collections.                         //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void initialisePCLCharCollCompLists()
    {
        int bitNo;

        ulong bitVal;

        bool bitSet;

        PCLCharCollections.eBitType bitType;

        //----------------------------------------------------------------//
        //                                                                //
        // Create lists of items and add to collection objects.           //
        //                                                                //
        //----------------------------------------------------------------//

        PCLCharCollItems items = new PCLCharCollItems();

        _charCollCompListUnicode = items.LoadCompListUnicode();

        //----------------------------------------------------------------//
        //                                                                //
        // Obtain the non-specific ('all') bit arrays.                    //
        //                                                                //
        //----------------------------------------------------------------//

        _charCollCompPCLAll = 0;

        foreach (PCLCharCollItem item in _charCollCompListUnicode)
        {
            bitType = item.BitType;

            if (bitType != PCLCharCollections.eBitType.Collection)
            {
                bitSet = !item.IsChecked;

                if (bitSet)
                {
                    bitNo = item.BitNo;
                    bitVal = ((ulong)0x01) << bitNo;

                    _charCollCompPCLAll |=
bitVal;
                }
            }
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Create PropertyChanged event handler for each item in the      //
        // collections.                                                   // 
        //                                                                //
        //----------------------------------------------------------------//

        foreach (PCLCharCollItem item in _charCollCompListUnicode)
        {
            item.PropertyChanged +=
                new PropertyChangedEventHandler(
                    cbPCLCharCollItem_PropertyChanged);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i n i t i a l i s e S y m S e t L i s t                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Populate list of symbol sets with defined mappings.                //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void initialiseSymSetList()
    {
        int index;

        cbSymSet.Items.Clear();

        _ctMappedSymSets = PCLSymbolSets.GetCountMapped();

        _subsetSymSets = new int[_ctMappedSymSets];

        PCLSymbolSets.GetIndicesMapped(0, ref _subsetSymSets);

        for (int i = 0; i < _ctMappedSymSets; i++)
        {
            index = _subsetSymSets[i];
            cbSymSet.Items.Add(PCLSymbolSets.GetName(index));
        }

        _indxSymSetDefault = PCLSymbolSets.GetIndexForId(_defaultSymSetNo);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o g F o n t S e l e c t D a t a P C L                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Log details of PCL font selection attributes.                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void logFontSelectDataPCL(bool monoSpaced)
    {
        string baseName = string.Empty;

        ToolSoftFontGenLog.LogNameAndValue(
            _tableLogTarget, false, false,
            "Font selection attributes:",
            string.Empty);

        if (monoSpaced)
            ToolSoftFontGenLog.LogNameAndValue(
                _tableLogTarget, false, false,
                "Spacing:",
                "Value:   " + "0 (= fixed-pitch)");
        else
            ToolSoftFontGenLog.LogNameAndValue(
                _tableLogTarget, false, false,
                "Spacing:",
                "Value:   " + "1 (= proportionally-spaced)");

        ToolSoftFontGenLog.LogNameAndValue(
            _tableLogTarget, false, false,
            "Symbol set:",
            "Kind1:   " + _symSetNoPCL.ToString());

        ToolSoftFontGenLog.LogNameAndValue(
            _tableLogTarget, false, false,
            string.Empty,
            "Id:      " + txtPCLSymSetIdNum.Text +
                          txtPCLSymSetIdAlpha.Text +
                      " (= " + txtPCLSymSetName.Text + ")");

        ToolSoftFontGenLog.LogNameAndValue(
            _tableLogTarget, false, false,
            "Style:",
            "Number:  " + _styleNoPCL +
                          " (= " +
                             txtPCLStylePosture.Text + " | " +
                             txtPCLStyleWidth.Text + " | " +
                             txtPCLStyleStructure.Text +
                           ")");

        ToolSoftFontGenLog.LogNameAndValue(
            _tableLogTarget, false, false,
            "Stroke Weight:",
            "Value:   " + _weightNoPCL +
                          " (= " +
                             txtPCLWeightDesc.Text +
                           ")");

        ToolSoftFontGenLog.LogNameAndValue(
            _tableLogTarget, false, false,
            "Typeface:",
            "Number:  " + _typefaceNoPCL +
                          " (= " +
                             txtPCLTypefaceName.Text +
                           ")");

        ToolSoftFontGenLog.LogNameAndValue(
            _tableLogTarget, false, false,
            string.Empty,
            "Vendor:  " + _typefaceVendorPCL);

        PCLFonts.GetNameForIdPCL(_typefaceBasecodePCL,
                                 ref baseName);

        ToolSoftFontGenLog.LogNameAndValue(
            _tableLogTarget, false, false,
            string.Empty,
            "Base:    " + _typefaceBasecodePCL +
                          " (= " + baseName + ")");
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // l o g F o n t S e l e c t D a t a P C L X L                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Log details of PCL XL font selection attributes.                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void logFontSelectDataPCLXL()
    {
        ToolSoftFontGenLog.LogNameAndValue(
            _tableLogTarget, false, false,
            "Font selection attributes:",
            string.Empty);

        ToolSoftFontGenLog.LogNameAndValue(
            _tableLogTarget, false, false,
            "Symbol set:",
            "Kind1:   " + _symSetNoPCL.ToString());

        ToolSoftFontGenLog.LogNameAndValue(
            _tableLogTarget, false, false,
            string.Empty,
            "Id:      " + txtPCLSymSetIdNum.Text +
                          txtPCLSymSetIdAlpha.Text +
                      " (= " + txtPCLSymSetName.Text + ")");

        ToolSoftFontGenLog.LogNameAndValue(
            _tableLogTarget, false, false,
            "Font name:",
            _fontNamePCLXL);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m e t r i c s L o a d                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Load metrics from persistent storage.                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void metricsLoad()
    {
        int indxTemp = 0;

        ToolSoftFontGenPersist.LoadDataCommon(ref indxTemp,
                                              ref _flagLogVerbose);

        if (indxTemp == (int)ToolCommonData.ePrintLang.PCLXL)
            _crntPDL = ToolCommonData.ePrintLang.PCLXL;
        else
            _crntPDL = ToolCommonData.ePrintLang.PCL;

        ToolSoftFontGenPersist.LoadDataTTF(ref _indxFont,
                                           ref _flagUsePCLT,
                                           ref _fontFileAdhocTTF);
        /*
        if ((indxTemp < 0) || (indxTemp >= (Int32) ePCLTUse.Max))
            _indxPCLTUse = ePCLTUse.Use;
        else
            _indxPCLTUse = (ePCLTUse) indxTemp;
        */
        if ((_indxFont < 0) || (_indxFont >= _ctTTFFonts))
            _indxFont = 0;

        ToolSoftFontGenPersist.LoadDataMapping(
            ref _indxSymSetSubset,
            ref _symSetMapPCL,
            ref _symSetUnbound,
            ref _symSetUserSet,
            ref _symSetUserFile);

        if ((_indxSymSetSubset < 0) ||
            (_indxSymSetSubset >= _ctMappedSymSets))
            _indxSymSetSubset = 0;

        ToolSoftFontGenPersist.LoadDataPCL(
            ref _fontFolderPCL,
            ref _flagFormat16,
            ref _flagCharCollCompSpecificPCL,
            ref _flagVMetricsPCL,
            ref _flagSegGTLastPCL,
            ref _charCollCompPCLSpecific);

        ToolSoftFontGenPersist.LoadDataPCLXL(
            ref _fontFolderPCLXL,
            ref _flagVMetricsPCLXL);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m e t r i c s S a v e                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Save current metrics to persistent storage.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public void metricsSave()
    {
        ToolSoftFontGenPersist.SaveDataCommon((int)_crntPDL,
                                               _flagLogVerbose);

        ToolSoftFontGenPersist.SaveDataTTF(_indxFont,
                                             _flagUsePCLT,
                                            _fontFileAdhocTTF);

        ToolSoftFontGenPersist.SaveDataMapping(_indxSymSetSubset,
                                                _symSetMapPCL,
                                                _symSetUnbound,
                                                _symSetUserSet,
                                                _symSetUserFile);

        ToolSoftFontGenPersist.SaveDataPCL(_fontFolderPCL,
                                             _flagFormat16,
                                             _flagCharCollCompSpecificPCL,
                                             _flagVMetricsPCL,
                                             _flagSegGTLastPCL,
                                             _charCollCompPCLSpecific);

        ToolSoftFontGenPersist.SaveDataPCLXL(_fontFolderPCLXL,
                                             _flagVMetricsPCLXL);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // p o p u l a t e P C L C h a r C o l l C o m p                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Populate the individual check boxes in the Character Collection    //
    // Complement combination box.                                        //
    // The only indexing used is Unicode, standard for TrueType fonts     //
    // (MSL indexing is only of use with Intellifont fonts).              //
    // Associate the collection with the ItemsSource of the combobox.     //
    //                                                                    //  
    // Assume that there are 64 items, and that the collection has        //
    // defined them in order, starting with the one for bit 0 (the least  //
    // significant bit of the 64-bit array).                              //
    //                                                                    //
    // Because fonts use the Complement of the corresponding symbol set   //
    // Requirements field, the IsChecked field is set if the bit is NOT   //
    // set.                                                               //
    //                                                                    //
    // Finally, set the appropriate value in the associated text block.   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void populatePCLCharCollComp(ulong collBits)
    {
        ulong bitVal;

        int bitNo;

        PCLCharCollections.eBitType bitType;

        _flagCharCollCompInhibitPCL = true;

        cbPCLCharColls.ItemsSource = _charCollCompListUnicode;

        foreach (PCLCharCollItem item in _charCollCompListUnicode)
        {
            bitNo = item.BitNo;
            bitVal = ((ulong)0x01) << bitNo;
            bitType = item.BitType;

            if (bitType == PCLCharCollections.eBitType.Collection)
            {
                //  if ((_charCollCompPCL & bitVal) == 0)
                if ((collBits & bitVal) == 0)
                    item.IsChecked = true;
                else
                    item.IsChecked = false;
            }
            else
            {
                if ((_charCollCompPCLAll & bitVal) == 0)
                    item.IsChecked = true;
                else
                    item.IsChecked = false;
            }
        }

        //    setPCLCharCollCompValue (_charCollCompPCL);
        setPCLCharCollCompValue(collBits);

        _flagCharCollCompInhibitPCL = false;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r b M a p S y m S e t P C L _ C l i c k                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Radio button selecting Symbol Set mapping 'PCL'.                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void rbMapSymSetPCL_Click(object sender,
                                       RoutedEventArgs e)
    {
        _symSetMapPCL = true;

        //    donorSymSetChange ();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r b M a p S y m S e t S t d _ C l i c k                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Radio button selecting Symbol Set mapping 'strict'.                //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void rbMapSymSetStd_Click(object sender,
                                       RoutedEventArgs e)
    {
        _symSetMapPCL = false;

        //    donorSymSetChange ();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r b P C L C h a r C o m p A l l _ C l i c k                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the PCL Character Complement Collection 'All' radio    //
    // button is clicked.                                                 //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void rbPCLCharCompAll_Click(object sender,
                                          RoutedEventArgs e)
    {
        _flagCharCollCompSpecificPCL = false;

        cbPCLCharColls.Visibility = Visibility.Hidden;
        tblkPCLCharCollsText.Visibility = Visibility.Hidden;

        _charCollCompPCL = _charCollCompPCLAll;

        setPCLCharCollCompValue(_charCollCompPCL);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r b P C L C h a r C o m p S p e c i f i c _ C l i c k              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the PCL Character Complement Collection 'Specific'     //
    // radio button is clicked.                                           //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void rbPCLCharCompSpecific_Click(object sender,
                                               RoutedEventArgs e)
    {
        _flagCharCollCompSpecificPCL = true;

        cbPCLCharColls.Visibility = Visibility.Visible;
        tblkPCLCharCollsText.Visibility = Visibility.Visible;

        _charCollCompPCL = _charCollCompPCLSpecific;

        populatePCLCharCollComp(_charCollCompPCL);

        setPCLCharCollCompValue(_charCollCompPCL);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r b P C L H d d r F m t 1 5 _ C l i c k                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the PCL 'Format 15' radio button is clicked.           //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void rbPCLHddrFmt15_Click(object sender, RoutedEventArgs e)
    {
        _flagFormat16 = false;

        grpPCLHddrVMetrics.Visibility = Visibility.Hidden;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r b P C L H d d r F m t 1 6 _ C l i c k                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the PCL 'Format 16' radio button is clicked.           //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void rbPCLHddrFmt16_Click(object sender, RoutedEventArgs e)
    {
        _flagFormat16 = true;

        if (_tabvmtxPresent)
            grpPCLHddrVMetrics.Visibility = Visibility.Visible;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r b P C L T I g n o r e _ C l i c k                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the 'PCLT treatment = Ignore' radio button is clicked. //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void rbPCLTIgnore_Click(object sender, RoutedEventArgs e)
    {
        _flagUsePCLT = false;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r b P C L T U s e _ C l i c k                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the 'PCLT treatment = Use' radio button is clicked.    //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void rbPCLTUse_Click(object sender, RoutedEventArgs e)
    {
        _flagUsePCLT = true;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r b S y m S e t B o u n d _ C l i c k                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the Binding 'Bound' radio button is clicked.           //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void rbSymSetBound_Click(object sender, RoutedEventArgs e)
    {
        _symSetUnbound = false;
        _symSetUserSet = false;

        setSymSetAttributesTarget();

        grpSymSet.Visibility = Visibility.Visible;
        cbSymSet.Visibility = Visibility.Visible;

        grpSymSetMapType.Visibility = Visibility.Visible;

        grpSymSetFile.Visibility = Visibility.Hidden;
        grpSymSetFile.IsEnabled = false;
        grpPCLCharComp.Visibility = Visibility.Hidden;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r b S y m S e t U n B o u n d _ C l i c k                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the Binding 'Unbound' radio button is clicked.         //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void rbSymSetUnbound_Click(object sender, RoutedEventArgs e)
    {
        _symSetUnbound = true;
        _symSetUserSet = false;

        setSymSetAttributesTarget();

        grpSymSet.Visibility = Visibility.Hidden;

        grpSymSetMapType.Visibility = Visibility.Hidden;

        grpSymSetFile.Visibility = Visibility.Hidden;
        grpSymSetFile.IsEnabled = false;
        grpPCLCharComp.Visibility = Visibility.Visible;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r b S y m S e t U s e r S e t _ C l i c k                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Called when the Binding 'User set' radio button is clicked.        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void rbSymSetUserSet_Click(object sender, RoutedEventArgs e)
    {
        _symSetUnbound = false;
        _symSetUserSet = true;

        CheckPCLSymSetFile();

        setSymSetAttributesTarget();

        grpSymSet.Visibility = Visibility.Visible;
        cbSymSet.Visibility = Visibility.Hidden;

        grpSymSetMapType.Visibility = Visibility.Hidden;

        grpSymSetFile.Visibility = Visibility.Visible;
        grpSymSetFile.IsEnabled = true;
        grpPCLCharComp.Visibility = Visibility.Hidden;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r e s e t F o r m S t a t e                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Invoked when a different donor font is selected.                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void resetFormState()
    {
        grpPCLTTreatment.Visibility = Visibility.Visible;
        lbPCLTNotPresent.Visibility = Visibility.Hidden;
        rbPCLTIgnore.Visibility = Visibility.Visible;
        rbPCLTUse.Visibility = Visibility.Visible;

        grpTargetFont.Visibility = Visibility.Hidden;
        btnPCLGenerate.Visibility = Visibility.Hidden;
        btnLogSave.Visibility = Visibility.Hidden;

        lbTTCFont.Visibility = Visibility.Hidden;
        cbTTCName.Visibility = Visibility.Hidden;

        if (_symSetUnbound)
        {
            grpSymSet.Visibility = Visibility.Hidden;
            grpSymSetMapType.Visibility = Visibility.Hidden;
        }
        else if (_symSetUserSet)
        {
            grpSymSet.Visibility = Visibility.Visible;
            grpSymSetMapType.Visibility = Visibility.Hidden;
        }
        else
        {
            grpSymSet.Visibility = Visibility.Visible;
            grpSymSetMapType.Visibility = Visibility.Visible;
        }

        lbSymSetSymbol.Visibility = Visibility.Hidden;

        btnTTFSelect.IsEnabled = true;
        btnTTFSelect.Visibility = Visibility.Visible;
        btnReset.Visibility = Visibility.Hidden;

        grpPCLTTreatment.IsEnabled = true;
        grpBinding.IsEnabled = true;
        grpSymSet.IsEnabled = true;
        grpSymSetFile.IsEnabled = true;
        grpSymSetMapType.IsEnabled = true;
        chkLogVerbose.IsEnabled = true;

        grpLog.Visibility = Visibility.Hidden;

        _fontWithinTTC = false;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r e s e t T a r g e t                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Reset the target type.                                             //
    //                                                                    //
    //--------------------------------------------------------------------//

    public void resetTarget()
    {
        // dummy method
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s e l e c t P C L F o n t F i l e                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Initiate 'open file' dialogue for target PCL font file.            //
    //                                                                    //
    //--------------------------------------------------------------------//

    private bool selectPCLFontFile(ref string fontFilename)
    {
        OpenFileDialog openDialog = ToolCommonFunctions.CreateOpenFileDialog(fontFilename);

        openDialog.CheckFileExists = false;
        openDialog.Filter = "PCLETTO font files|*.sft; *.SFT";

        bool? dialogResult = openDialog.ShowDialog();

        if (dialogResult == true)
            fontFilename = openDialog.FileName;

        return dialogResult == true;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s e l e c t P C L X L F o n t F i l e                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Initiate 'open file' dialogue for target PCLXL font file.          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private bool selectPCLXLFontFile(ref string fontFilename)
    {
        OpenFileDialog openDialog = ToolCommonFunctions.CreateOpenFileDialog(fontFilename);

        openDialog.CheckFileExists = false;
        openDialog.Filter = "PCLXLETTO font files|*.sfx; *.SFX";

        bool? dialogResult = openDialog.ShowDialog();

        if (dialogResult == true)
            fontFilename = openDialog.FileName;

        return dialogResult == true;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s e l e c t S y m S e t F i l e                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Initiate 'open file' dialogue for user-defined symbol set file.    //
    //                                                                    //
    //--------------------------------------------------------------------//

    private bool selectSymSetFile(ref string symSetFile)
    {
        OpenFileDialog openDialog = ToolCommonFunctions.CreateOpenFileDialog(symSetFile);

        openDialog.Filter = "PCL files|*.pcl; *.PCL;" +
                            "|All files|*.*";

        bool? dialogResult = openDialog.ShowDialog();

        if (dialogResult == true)
            symSetFile = openDialog.FileName;

        return dialogResult == true;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s e l e c t T T F F o n t F i l e                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Initiate 'open file' dialogue for donor TrueType font file.        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private bool selectTTFFontFile(ref string fontFilenameTTF)
    {
        OpenFileDialog openDialog = ToolCommonFunctions.CreateOpenFileDialog(fontFilenameTTF);

        openDialog.CheckFileExists = false;
        openDialog.Filter = "TrueType font files|" +
                            "*.ttf; *.otf; *.ttc" +
                            "*.TTF; *.OTF; *.TTC";

        bool? dialogResult = openDialog.ShowDialog();

        if (dialogResult == true)
            fontFilenameTTF = openDialog.FileName;

        return dialogResult == true;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s e t P C L C h a r C o l l C o m p A r r a y                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Generate and display the text associated with the current set of   //
    // Check boxes selected within the Character Collections combo box.   //
    // Also store the calculated array value.                             //
    //                                                                    //
    // Because fonts use the Complement of the corresponding symbol set   //
    // Requirements field, the IsChecked field is set if the bit is NOT   //
    // set.                                                               //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void setPCLCharCollCompArray()
    {
        ulong targetCharCollComp = 0,
               bitVal;

        int bitNo;

        if (cbPCLCharColls.ItemsSource != null)
        {
            foreach (PCLCharCollItem item in cbPCLCharColls.ItemsSource)
            {
                bitNo = item.BitNo;

                if (!item.IsChecked)
                {
                    bitVal = ((ulong)1) << bitNo;
                    targetCharCollComp |= bitVal;
                }
            }
        }

        setPCLCharCollCompValue(targetCharCollComp);

        //----------------------------------------------------------------//

        _charCollCompPCLSpecific = targetCharCollComp;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s e t P C L C h a r C o l l C o m p V a l u e                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Display the current Character Collection array value.              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void setPCLCharCollCompValue(ulong arrayVal)
    {
        tblkPCLCharColls.Text = "0x" + arrayVal.ToString("x16");
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s e t P C L F o n t F i l e A t t r i b u t e s                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Set the attributes of the selected PCL font file.                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void setPCLFontFileAttributes()
    {
        int indx;

        indx = _fontFilenamePCL.LastIndexOf("\\");

        if (indx == 0)
            _fontFolderPCL = string.Empty;
        else
            _fontFolderPCL = _fontFilenamePCL.Substring(0, indx);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s e t P C L X L F o n t F i l e A t t r i b u t e s                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Set the attributes of the selected PCLXL font file.                //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void setPCLXLFontFileAttributes()
    {
        int indx;

        indx = _fontFilenamePCLXL.LastIndexOf("\\");

        if (indx == 0)
            _fontFolderPCLXL = string.Empty;
        else
            _fontFolderPCLXL = _fontFilenamePCLXL.Substring(0, indx);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s e t S y m S e t A t t r i b u t e s T a r g e t                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Set the attributes of the selected symbol set.                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void setSymSetAttributesTarget()
    {
        string idNum = string.Empty,
               idAlpha = string.Empty;

        if (_symSetUnbound)
        {
            //--------------------------------------------------------//
            //                                                        //
            // Unbound - not bound to any particular symbol set.      //
            //                                                        //
            // PCL:    header must use Kind1 =  56; set ID = 1X       //
            // PCLXL:  header must use Kind1 = 590; set ID = 18N      //
            //                                                        //
            //--------------------------------------------------------//

            grpSymSet.Visibility = Visibility.Hidden;
            grpSymSetFile.Visibility = Visibility.Hidden;
            grpSymSetMapType.Visibility = Visibility.Hidden;

            _symSetNoTargetPCL = _symSetNoUnbound;
            _symSetNoTargetPCLXL = _symSetNoUnicode;

            txtPCLSymSetNo.Text = _symSetNoUnbound.ToString();

            PCLSymbolSets.TranslateKind1ToId(_symSetNoUnbound,
                                      ref idNum,
                                      ref idAlpha);

            txtPCLSymSetIdNum.Text = idNum;
            txtPCLSymSetIdAlpha.Text = idAlpha;

            txtPCLXLSymSetNo.Text = _symSetNoUnicode.ToString();

            PCLSymbolSets.TranslateKind1ToId(_symSetNoUnicode,
                                      ref idNum,
                                      ref idAlpha);

            txtPCLXLSymSetIdNum.Text = idNum;
            txtPCLXLSymSetIdAlpha.Text = idAlpha;
        }
        else if (_symSetUserSet)
        {
            //--------------------------------------------------------//
            //                                                        //
            // User set - bound to a user-defined symbol set.         //
            //                                                        //
            // The user-defined symbol set is defined via a PCL      //
            // symbol set definition held within a user-specified     //
            // file.                                                  //
            //                                                        //
            //--------------------------------------------------------//

            grpSymSetFile.Visibility = Visibility.Visible;
            grpSymSet.Visibility = Visibility.Visible;
            cbSymSet.Visibility = Visibility.Hidden;
            grpSymSetMapType.Visibility = Visibility.Hidden;

            _indxSymSetTarget = PCLSymbolSets.IndexUserSet;
            _symSetNoTargetPCL = _symSetNoUserSet;
            _symSetNoTargetPCLXL = _symSetNoUserSet;

            PCLSymbolSets.TranslateKind1ToId(_symSetNoUserSet,
                                              ref idNum,
                                              ref idAlpha);

            txtSymSetNo.Text = _symSetNoUserSet.ToString();

            txtSymSetIdNum.Text = idNum;
            txtSymSetIdAlpha.Text = idAlpha;

            _symSetType = PCLSymbolSets.GetType(_indxSymSetTarget);

            txtSymSetType.Text =
                PCLSymSetTypes.GetDescShort((int)_symSetType);

            txtPCLSymSetNo.Text = _symSetNoUserSet.ToString();

            txtPCLSymSetIdNum.Text = idNum;
            txtPCLSymSetIdAlpha.Text = idAlpha;

            txtPCLXLSymSetNo.Text = _symSetNoUserSet.ToString(); // or something else ?? //

            txtPCLXLSymSetIdNum.Text = idNum;
            txtPCLXLSymSetIdAlpha.Text = idAlpha;
        }
        else if (_indxSymSetSubset != -1)
        {
            int indxSymSet;

            _indxSymSetSubset = cbSymSet.SelectedIndex;

            indxSymSet = _subsetSymSets[_indxSymSetSubset];

            _symSetGroup = PCLSymbolSets.GetGroup(indxSymSet);
            _symSetType = PCLSymbolSets.GetType(indxSymSet);

            _flagSymSetNullMapPCL = PCLSymbolSets.NullMapPCL(indxSymSet);
            _flagSymSetNullMapStd = PCLSymbolSets.NullMapStd(indxSymSet);

            if ((_symSetGroup == PCLSymbolSets.eSymSetGroup.Preset) ||
                (_symSetGroup == PCLSymbolSets.eSymSetGroup.NonStd))
            {
                //--------------------------------------------------------//
                //                                                        //
                // Pre-defined symbol set.                                //
                // Obtain the number and equivalent ID values, and set    //
                // these values in the number and ID boxes (which should  //
                // be disabled to prevent user input).                    //
                //                                                        //
                //--------------------------------------------------------//

                grpSymSetFile.Visibility = Visibility.Hidden;
                grpSymSet.Visibility = Visibility.Visible;
                cbSymSet.Visibility = Visibility.Visible;
                grpSymSetMapType.Visibility = Visibility.Visible;

                txtSymSetNo.IsEnabled = false;
                txtSymSetIdNum.IsEnabled = false;
                txtSymSetIdAlpha.IsEnabled = false;

                //--------------------------------------------------------//

                txtSymSetType.Text =
                    PCLSymSetTypes.GetDescShort((int)_symSetType);

                _indxSymSetTarget = indxSymSet;
                _symSetNoTargetPCL = PCLSymbolSets.GetKind1(indxSymSet);
                _symSetNoTargetPCLXL = _symSetNoTargetPCL;

                txtSymSetNo.Text = _symSetNoTargetPCL.ToString();

                txtPCLSymSetNo.Text = _symSetNoTargetPCL.ToString();
                txtPCLXLSymSetNo.Text = _symSetNoTargetPCLXL.ToString();

                //--------------------------------------------------------//

                PCLSymbolSets.TranslateKind1ToId(_symSetNoTargetPCL,
                                          ref idNum,
                                          ref idAlpha);

                txtSymSetIdNum.Text = idNum;
                txtSymSetIdAlpha.Text = idAlpha;

                txtPCLSymSetIdNum.Text = idNum;
                txtPCLSymSetIdAlpha.Text = idAlpha;

                txtPCLXLSymSetIdNum.Text = idNum;
                txtPCLXLSymSetIdAlpha.Text = idAlpha;

                //--------------------------------------------------------//

                if (_flagSymSetNullMapPCL)
                {
                    rbMapSymSetPCL.IsEnabled = false;
                    rbMapSymSetStd.IsChecked = true;

                    _symSetMapPCL = false;
                }
                else if (_flagSymSetNullMapStd)
                {
                    rbMapSymSetStd.IsEnabled = false;
                    rbMapSymSetPCL.IsChecked = true;

                    _symSetMapPCL = true;
                }
                else
                {
                    rbMapSymSetPCL.IsEnabled = true;
                    rbMapSymSetStd.IsEnabled = true;
                }
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t a b D e t a i l s _ S e l e c t i o n C h a n g e d              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // The target print language tab (PCL or PCLXL) has changed.          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void tabDetails_SelectionChanged(object sender,
                                             SelectionChangedEventArgs e)
    {
        if (tabDetails.SelectedItem.Equals(tabPCLXL))
            _crntPDL = ToolCommonData.ePrintLang.PCLXL;
        else
            _crntPDL = ToolCommonData.ePrintLang.PCL;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t r a n s l a t e S t y l e N o                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Returns an interpretation of the components of the 16-bit style    //
    // value.                                                             //
    //                                                                    //
    // Bit numbers are zero-indexed from (left) Most Significant:         //
    //                                                                    //
    //    bits  0  -  5   Reserved                                        //
    //          6  - 10   Structure  (e.g. Solid)                         //
    //         11  - 13   Width      (e.g. Condensed)                     //
    //         14  - 15   Posture    (e.g. Italic)                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void translateStyleNo(ushort style,
                                   bool showSubIds,
                                   ref string posture,
                                   ref string width,
                                   ref string structure)
    {
        int index,
              subId;

        string subIdPosture,
               subIdWidth,
               subIdStructure;

        //----------------------------------------------------------------//

        index = (style >> 5) & 0x1f;
        subId = index * 32;

        if (showSubIds)
            subIdStructure = subId.ToString() + ": ";
        else
            subIdStructure = string.Empty;

        switch (index)
        {
            case (ushort)PCLFonts.eStyleStructure.Solid:
                structure = subIdStructure + "Solid";
                break;

            case (ushort)PCLFonts.eStyleStructure.Outline:
                structure = subIdStructure + "Outline";
                break;

            case (ushort)PCLFonts.eStyleStructure.Inline:
                structure = subIdStructure + "Inline";
                break;

            case (ushort)PCLFonts.eStyleStructure.Contour:
                structure = subIdStructure + "Contour";
                break;

            case (ushort)PCLFonts.eStyleStructure.Solid_Shadow:
                structure = subIdStructure + "Solid + Shadow";
                break;

            case (ushort)PCLFonts.eStyleStructure.Outline_Shadow:
                structure = subIdStructure + "Outline + Shadow";
                break;

            case (ushort)PCLFonts.eStyleStructure.Inline_Shadow:
                structure = subIdStructure + "Inline + Shadow";
                break;

            case (ushort)PCLFonts.eStyleStructure.Contour_Shadow:
                structure = subIdStructure + "Contour + Shadow";
                break;

            case (ushort)PCLFonts.eStyleStructure.Pattern:
                structure = subIdStructure + "Pattern";
                break;

            case (ushort)PCLFonts.eStyleStructure.Pattern1:
                structure = subIdStructure + "Pattern 1";
                break;

            case (ushort)PCLFonts.eStyleStructure.Pattern2:
                structure = subIdStructure + "Pattern 2";
                break;

            case (ushort)PCLFonts.eStyleStructure.Pattern3:
                structure = subIdStructure + "Pattern 3";
                break;

            case (ushort)PCLFonts.eStyleStructure.Pattern_Shadow:
                structure = subIdStructure + "Pattern + Shadow";
                break;

            case (ushort)PCLFonts.eStyleStructure.Pattern1_Shadow:
                structure = subIdStructure + "Pattern 1 + Shadow";
                break;

            case (ushort)PCLFonts.eStyleStructure.Pattern2_Shadow:
                structure = subIdStructure + "Pattern 2 + Shadow";
                break;

            case (ushort)PCLFonts.eStyleStructure.Pattern3_Shadow:
                structure = subIdStructure + "Pattern 3 + Shadow";
                break;

            case (ushort)PCLFonts.eStyleStructure.Inverse:
                structure = subIdStructure + "Inverse";
                break;

            case (ushort)PCLFonts.eStyleStructure.Inverse_Border:
                structure = subIdStructure + "Inverse + Border";
                break;

            case (ushort)PCLFonts.eStyleStructure.Unknown:
                structure = subIdStructure + "Unknown";
                break;

            default:
                structure = subIdStructure + "Reserved";
                break;
        }

        //----------------------------------------------------------------//

        index = (style >> 2) & 0x07;
        subId = index * 4;

        if (showSubIds)
            subIdWidth = subId.ToString() + ": ";
        else
            subIdWidth = string.Empty;

        switch (index)
        {
            case (byte)PCLFonts.eStyleWidth.Normal:
                width = subIdWidth + "Normal";
                break;

            case (byte)PCLFonts.eStyleWidth.Condensed:
                width = subIdWidth + "Condensed";
                break;

            case (byte)PCLFonts.eStyleWidth.Compressed:
                width = subIdWidth + "Compressed";
                break;

            case (byte)PCLFonts.eStyleWidth.ExtraCompressed:
                width = subIdWidth + "Extra Compressed";
                break;

            case (byte)PCLFonts.eStyleWidth.UltraCompressed:
                width = subIdWidth + "Ultra Compressed";
                break;

            case (byte)PCLFonts.eStyleWidth.Reserved:
                width = subIdWidth + "Reserved";
                break;

            case (byte)PCLFonts.eStyleWidth.Expanded:
                width = subIdWidth + "Expanded";
                break;

            case (byte)PCLFonts.eStyleWidth.ExtraExpanded:
                width = subIdWidth + "Extra Expanded";
                break;

            default:
                width = subIdWidth + "Impossible?";
                break;
        }

        //----------------------------------------------------------------//

        index = style & 0x03;
        subId = index;

        if (showSubIds)
            subIdPosture = subId.ToString() + ": ";
        else
            subIdPosture = string.Empty;

        switch (index)
        {
            case (byte)PCLFonts.eStylePosture.Upright:
                posture = subIdPosture + "Upright";
                break;

            case (byte)PCLFonts.eStylePosture.Italic:
                posture = subIdPosture + "Italic";
                break;

            case (byte)PCLFonts.eStylePosture.ItalicAlt:
                posture = subIdPosture + "Italic Alt.";
                break;

            case (byte)PCLFonts.eStylePosture.Reserved:
                posture = subIdPosture + "Reserved";
                break;

            default:
                posture = subIdPosture + "Impossible?";
                break;
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t r a n s l a t e W e i g h t N o                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Returns an interpretation of the components of the 8-bit weight    //
    // value.                                                             //
    //                                                                    //
    //--------------------------------------------------------------------//

    private string translateWeightNo(sbyte weight,
                                      bool showSubIds)
    {
        int subId;

        string desc;

        string subIdWeight;

        //----------------------------------------------------------------//

        subId = weight;

        if (showSubIds)
            subIdWeight = subId.ToString() + ": ";
        else
            subIdWeight = string.Empty;

        switch (weight)
        {
            case (sbyte)PCLFonts.eStrokeWeight.UltraThin:
                desc = subIdWeight + "Ultra Thin";
                break;

            case (sbyte)PCLFonts.eStrokeWeight.ExtraThin:
                desc = subIdWeight + "Extra Thin";
                break;

            case (sbyte)PCLFonts.eStrokeWeight.Thin:
                desc = subIdWeight + "Thin";
                break;

            case (sbyte)PCLFonts.eStrokeWeight.ExtraLight:
                desc = subIdWeight + "Extra Light";
                break;

            case (sbyte)PCLFonts.eStrokeWeight.Light:
                desc = subIdWeight + "Light";
                break;

            case (sbyte)PCLFonts.eStrokeWeight.DemiLight:
                desc = subIdWeight + "Demi Light";
                break;

            case (sbyte)PCLFonts.eStrokeWeight.SemiLight:
                desc = subIdWeight + "Semi Light";
                break;

            case (sbyte)PCLFonts.eStrokeWeight.Medium:
                desc = subIdWeight + "Medium";
                break;

            case (sbyte)PCLFonts.eStrokeWeight.SemiBold:
                desc = subIdWeight + "Semi Bold";
                break;

            case (sbyte)PCLFonts.eStrokeWeight.DemiBold:
                desc = subIdWeight + "Demi Bold";
                break;

            case (sbyte)PCLFonts.eStrokeWeight.Bold:
                desc = subIdWeight + "Bold";
                break;

            case (sbyte)PCLFonts.eStrokeWeight.ExtraBold:
                desc = subIdWeight + "Extra Bold";
                break;

            case (sbyte)PCLFonts.eStrokeWeight.Black:
                desc = subIdWeight + "Black";
                break;

            case (sbyte)PCLFonts.eStrokeWeight.ExtraBlack:
                desc = subIdWeight + "Extra Black";
                break;

            case (sbyte)PCLFonts.eStrokeWeight.UltraBlack:
                desc = subIdWeight + "Ultra Black";
                break;

            default:
                desc = subIdWeight + "Invalid";
                break;
        }

        return desc;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t P C L F o n t f i l e _ L o s t F o c u s                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // PCL (target) font filename item has lost focus.                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtPCLFontFile_LostFocus(object sender,
                                           RoutedEventArgs e)
    {
        _fontFilenamePCL = txtPCLFontFile.Text;

        setPCLFontFileAttributes();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t P C L S t y l e N o _ L o s t F o c u s                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // PCL (target) style number has lost focus.                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtPCLStyleNo_LostFocus(object sender,
                                          RoutedEventArgs e)
    {
        string posture = string.Empty,
               width = string.Empty,
               structure = string.Empty;

        if (validatePCLStyleNo(true, ref _styleNoPCL))
        {
            translateStyleNo(_styleNoPCL,
                              false,
                              ref posture,
                              ref width,
                              ref structure);
        }

        txtPCLStylePosture.Text = posture;
        txtPCLStyleWidth.Text = width;
        txtPCLStyleStructure.Text = structure;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t P C L S t y l e N o _ T e x t C h a n g e d                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // PCL (target) style number has changed.                             //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtPCLStyleNo_TextChanged(object sender,
                                            TextChangedEventArgs e)
    {
        string posture = string.Empty,
               width = string.Empty,
               structure = string.Empty;

        if (validatePCLStyleNo(false, ref _styleNoPCL))
        {
            translateStyleNo(_styleNoPCL,
                              false,
                              ref posture,
                              ref width,
                              ref structure);
        }

        txtPCLStylePosture.Text = posture;
        txtPCLStyleWidth.Text = width;
        txtPCLStyleStructure.Text = structure;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t P C L S y m S e t N o _ L o s t F o c u s                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // PCL (target) symbol set number has lost focus.                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtPCLSymSetNo_LostFocus(object sender,
                                           RoutedEventArgs e)
    {
        string idAlpha = string.Empty,
               idNum = string.Empty;

        string name = string.Empty;

        if (validatePCLSymSetNo(true, ref _symSetNoPCL))
        {
            {
                PCLSymbolSets.TranslateKind1ToId(_symSetNoPCL,
                                                 ref idNum,
                                                 ref idAlpha);

                PCLSymbolSets.GetNameForId(_symSetNoPCL,
                                            ref name);
            }
        }

        txtPCLSymSetIdNum.Text = idNum;
        txtPCLSymSetIdAlpha.Text = idAlpha;
        txtPCLSymSetName.Text = name.ToString();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t P C L S y m S e t N o _ T e x t C h a n g e d                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // PCL (target) symbol set number has changed.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtPCLSymSetNo_TextChanged(object sender,
                                             TextChangedEventArgs e)
    {
        string idAlpha = string.Empty,
               idNum = string.Empty;

        string name = string.Empty;

        if (validatePCLSymSetNo(false, ref _symSetNoPCL))
        {
            PCLSymbolSets.TranslateKind1ToId(_symSetNoPCL,
                                             ref idNum,
                                             ref idAlpha);

            PCLSymbolSets.GetNameForId(_symSetNoPCL,
                                        ref name);
        }

        txtPCLSymSetIdNum.Text = idNum;
        txtPCLSymSetIdAlpha.Text = idAlpha;
        txtPCLSymSetName.Text = name.ToString();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t P C L T y p e f a c e N o _ L o s t F o c u s                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // PCL (target) typeface number has lost focus.                       //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtPCLTypefaceNo_LostFocus(object sender,
                                             RoutedEventArgs e)
    {
        ushort vendor = 0,
               basecode = 0;

        string name = string.Empty;

        if (validatePCLTypefaceNo(true, ref _typefaceNoPCL))
        {
            PCLFonts.TranslateTypeface(_typefaceNoPCL,
                                        ref vendor,
                                        ref basecode);

            PCLFonts.GetNameForIdPCL(_typefaceNoPCL,
                                      ref name);
        }

        txtPCLTypefaceVendor.Text = vendor.ToString();
        txtPCLTypefaceBase.Text = basecode.ToString();
        txtPCLTypefaceName.Text = name.ToString();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t P C L T y p e f a c e N o _ T e x t C h a n g e d            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // PCL (target) typeface number has changed.                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtPCLTypefaceNo_TextChanged(object sender,
                                               TextChangedEventArgs e)
    {
        string name = string.Empty;

        if (validatePCLTypefaceNo(false, ref _typefaceNoPCL))
        {
            PCLFonts.TranslateTypeface(_typefaceNoPCL,
                                        ref _typefaceVendorPCL,
                                        ref _typefaceBasecodePCL);

            PCLFonts.GetNameForIdPCL(_typefaceNoPCL,
                                      ref name);
        }

        txtPCLTypefaceVendor.Text = _typefaceVendorPCL.ToString();
        txtPCLTypefaceBase.Text = _typefaceBasecodePCL.ToString();
        txtPCLTypefaceName.Text = name.ToString();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t P C L W e i g h t N o _ L o s t F o c u s                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // PCL (target) weight number has lost focus.                         //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtPCLWeightNo_LostFocus(object sender,
                                           RoutedEventArgs e)
    {
        string weight = string.Empty;

        if (validatePCLWeightNo(true, ref _weightNoPCL))
        {
            weight = translateWeightNo(_weightNoPCL,
                                        false);
        }

        txtPCLWeightDesc.Text = weight;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t P C L S t y l e N o _ T e x t C h a n g e d                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // PCL (target) style number has changed.                             //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtPCLWeightNo_TextChanged(object sender,
                                             TextChangedEventArgs e)
    {
        string weight = string.Empty;

        if (validatePCLWeightNo(false, ref _weightNoPCL))
        {
            weight = translateWeightNo(_weightNoPCL,
                                        false);
        }

        txtPCLWeightDesc.Text = weight;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t P C L X L F o n t f i l e _ L o s t F o c u s                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // PCLXL (target) font filename item has lost focus.                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtPCLXLFontFile_LostFocus(object sender,
                                           RoutedEventArgs e)
    {
        _fontFilenamePCLXL = txtPCLXLFontFile.Text;

        setPCLXLFontFileAttributes();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t P C L X L F o n t N a m e _ L o s t F o c u s                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // PCLXL font name item has lost focus.                               //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtPCLXLFontName_LostFocus(object sender,
                                           RoutedEventArgs e)
    {
        _fontNamePCLXL = txtPCLXLFontName.Text;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t P C L X L S y m S e t N o _ L o s t F o c u s                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // PCLXL (target) symbol set number has lost focus.                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtPCLXLSymSetNo_LostFocus(object sender,
                                           RoutedEventArgs e)
    {
        string idAlpha = string.Empty,
               idNum = string.Empty;

        string name = string.Empty;

        if (validatePCLXLSymSetNo(true, ref _symSetNoPCLXL))
        {
            {
                PCLSymbolSets.TranslateKind1ToId(_symSetNoPCLXL,
                                                 ref idNum,
                                                 ref idAlpha);

                PCLSymbolSets.GetNameForId(_symSetNoPCLXL,
                                            ref name);
            }
        }

        txtPCLXLSymSetIdNum.Text = idNum;
        txtPCLXLSymSetIdAlpha.Text = idAlpha;
        txtPCLXLSymSetName.Text = name.ToString();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t P C L X L S y m S e t N o _ T e x t C h a n g e d            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // PCLXL (target) symbol set number has changed.                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtPCLXLSymSetNo_TextChanged(object sender,
                                             TextChangedEventArgs e)
    {
        string idAlpha = string.Empty,
               idNum = string.Empty;

        string name = string.Empty;

        if (validatePCLXLSymSetNo(false, ref _symSetNoPCLXL))
        {
            PCLSymbolSets.TranslateKind1ToId(_symSetNoPCLXL,
                                             ref idNum,
                                             ref idAlpha);

            PCLSymbolSets.GetNameForId(_symSetNoPCLXL,
                                        ref name);
        }

        txtPCLXLSymSetIdNum.Text = idNum;
        txtPCLXLSymSetIdAlpha.Text = idAlpha;
        txtPCLXLSymSetName.Text = name.ToString();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t S y m S e t F i l e _ L o s t F o c u s                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // User-defined symbol set filename item has lost focus.              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtSymSetFile_LostFocus(object sender,
                                          RoutedEventArgs e)
    {
        if (_symSetUserFile != txtSymSetFile.Text)
        {
            _symSetUserFile = txtSymSetFile.Text;

            CheckPCLSymSetFile();

            setSymSetAttributesTarget();
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // t x t T T F F i l e _ L o s t F o c u s                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // TTF (donor) font filename item has lost focus.                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void txtTTFFile_LostFocus(object sender,
                                      RoutedEventArgs e)
    {
        if (_fontFileAdhocTTF != txtTTFFile.Text)
        {
            _fontFileAdhocTTF = txtTTFFile.Text;
            _fontFilenameTTF = txtTTFFile.Text;

            resetFormState();
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // v a l i d a t e P C L S t y l e N o                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Validate PCL Style number.                                         //
    //                                                                    //
    //--------------------------------------------------------------------//

    private bool validatePCLStyleNo(bool lostFocusEvent,
                                        ref ushort styleNo)
    {
        const ushort minVal = 0;
        const ushort maxVal = 1023;
        const ushort defVal = _defaultPCLStyleNo;

        ushort value;

        bool OK = true;

        string crntText = txtPCLStyleNo.Text;

        OK = ushort.TryParse(crntText, out value);

        if (OK)
        {
            if ((value < minVal) || (value > maxVal))
            {
                OK = false;
            }
        }

        if (OK)
        {
            styleNo = value;
        }
        else
        {
            if (lostFocusEvent)
            {
                string newText = defVal.ToString();

                MessageBox.Show
                    ("Style number '" + crntText +
                     "' is invalid.\n\n" +
                     "Value will be reset to default '" +
                     newText + "'",
                     "PCL style number invalid",
                     MessageBoxButton.OK,
                     MessageBoxImage.Warning);

                styleNo = defVal;

                txtPCLStyleNo.Text = newText;
            }
            else
            {
                MessageBox.Show
                    ("Style number '" + crntText +
                     "' is invalid.\n\n" +
                     "Valid range is :\n\t" +
                     minVal + " <= value <= " + maxVal,
                     "PCL style number invalid",
                     MessageBoxButton.OK,
                     MessageBoxImage.Error);

                txtPCLStyleNo.Focus();
                txtPCLStyleNo.SelectAll();
            }
        }

        return OK;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // v a l i d a t e P C L S y m S e t N o                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Validate PCL Symbol Set number (kind1).                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    private bool validatePCLSymSetNo(bool lostFocusEvent,
                                          ref ushort symSetNo)
    {
        const ushort minVal = 1;
        const ushort maxVal = 65530;
        const ushort defVal = _defaultSymSetNo;

        ushort value;

        bool OK = true;

        string crntText = txtPCLSymSetNo.Text;

        OK = ushort.TryParse(crntText, out value);

        if (OK)
        {
            if ((value < minVal) || (value > maxVal))
            {
                OK = false;
            }
            else
            {
                int modVal = value % 32;

                if ((modVal < 1) || (modVal > 26))
                    OK = false;

                if ((!_symSetUnbound) && (modVal == 24))
                    OK = false;
            }
        }

        if (OK)
        {
            symSetNo = value;
        }
        else
        {
            if (lostFocusEvent)
            {
                string newText = defVal.ToString();

                MessageBox.Show
                    ("Symbol Set (kind1) number '" + crntText +
                     "' is invalid.\n\n" +
                     "Value will be reset to default '" +
                     newText + "'",
                     "PCL symbol set number invalid",
                     MessageBoxButton.OK,
                     MessageBoxImage.Warning);

                symSetNo = defVal;

                txtPCLSymSetNo.Text = newText;
            }
            else
            {
                MessageBox.Show
                    ("Symbol Set (kind1) number '" + crntText +
                     "' is invalid.\n\n" +
                     "Valid range is :\n\t" +
                     minVal + " <= value <= " + maxVal +
                     "\nand the value modulo 32 must be in the range " +
                     "1 -> 26, excluding 24",
                     "PCL symbol set number invalid",
                     MessageBoxButton.OK,
                     MessageBoxImage.Error);

                txtPCLSymSetNo.Focus();
                txtPCLSymSetNo.SelectAll();
            }
        }

        return OK;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // v a l i d a t e P C L T y p e f a c e N o                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Validate PCL Typeface family number.                               //
    //                                                                    //
    //--------------------------------------------------------------------//

    private bool validatePCLTypefaceNo(bool lostFocusEvent,
                                            ref ushort typefaceNo)
    {
        const ushort minVal = 0;
        const ushort maxVal = 65535;
        const ushort defVal = _defaultPCLTypefaceNo;

        ushort value;

        bool OK = true;

        string crntText = txtPCLTypefaceNo.Text;

        OK = ushort.TryParse(crntText, out value);

        if (OK)
        {
            if ((value < minVal) || (value > maxVal))
            {
                OK = false;
            }
        }

        if (OK)
        {
            typefaceNo = value;
        }
        else
        {
            if (lostFocusEvent)
            {
                string newText = defVal.ToString();

                MessageBox.Show("Typeface number '" + crntText +
                                "' is invalid.\n\n" +
                                "Value will be reset to default '" +
                                newText + "'",
                                "PCL typeface number invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                typefaceNo = defVal;

                txtPCLTypefaceNo.Text = newText;
            }
            else
            {
                MessageBox.Show("Typeface number '" + crntText +
                                "' is invalid.\n\n" +
                                "Valid range is :\n\t" +
                                minVal + " <= value <= " + maxVal,
                                "PCL typeface number invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                txtPCLTypefaceNo.Focus();
                txtPCLTypefaceNo.SelectAll();
            }
        }

        return OK;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // v a l i d a t e P C L W e i g h t N o                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Validate PCL Weight number.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private bool validatePCLWeightNo(bool lostFocusEvent,
                                         ref sbyte weightNo)
    {
        const sbyte minVal = -7;
        const sbyte maxVal = 7;
        const sbyte defVal = _defaultPCLWeightNo;

        sbyte value;

        bool OK = true;

        string crntText = txtPCLWeightNo.Text;

        OK = sbyte.TryParse(crntText, out value);

        if (OK)
        {
            if ((value < minVal) || (value > maxVal))
            {
                OK = false;
            }
        }

        if (OK)
        {
            weightNo = value;
        }
        else
        {
            if (lostFocusEvent)
            {
                string newText = defVal.ToString();

                MessageBox.Show("Weight number '" + crntText +
                                "' is invalid.\n\n" +
                                "Value will be reset to default '" +
                                newText + "'",
                                "PCL weight number invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                weightNo = defVal;

                txtPCLWeightNo.Text = newText;
            }
            else
            {
                MessageBox.Show("Weight number '" + crntText +
                                "' is invalid.\n\n" +
                                "Valid range is :\n\t" +
                                minVal + " <= value <= " + maxVal,
                                "PCL weight number invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                txtPCLWeightNo.Focus();
                txtPCLWeightNo.SelectAll();
            }
        }

        return OK;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // v a l i d a t e P C L X L S y m S e t N o                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Validate PCLXL Symbol Set number (kind1).                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private bool validatePCLXLSymSetNo(bool lostFocusEvent,
                                          ref ushort symSetNo)
    {
        const ushort minVal = 0;
        const ushort maxVal = 65530;
        const ushort defVal = _defaultSymSetNo;

        ushort value;

        bool OK = true;

        string crntText = txtPCLXLSymSetNo.Text;

        OK = ushort.TryParse(crntText, out value);

        if (OK)
        {
            if ((value < minVal) || (value > maxVal))
            {
                OK = false;
            }
            else
            {
                int modVal = value % 32;

                if ((modVal < 1) || (modVal > 26) || (modVal == 24))
                    OK = false;
            }
        }

        if (OK)
        {
            symSetNo = value;
        }
        else
        {
            if (lostFocusEvent)
            {
                string newText = defVal.ToString();

                MessageBox.Show
                    ("Symbol Set (kind1) number '" + crntText +
                     "' is invalid.\n\n" +
                     "Value will be reset to default '" +
                     newText + "'",
                     "PCLXL symbol set number invalid",
                     MessageBoxButton.OK,
                     MessageBoxImage.Warning);

                symSetNo = defVal;

                txtPCLXLSymSetNo.Text = newText;
            }
            else
            {
                MessageBox.Show
                    ("Symbol Set (kind1) number '" + crntText +
                     "' is invalid.\n\n" +
                     "Valid range is :\n\t" +
                     minVal + " <= value <= " + maxVal +
                     "\nand the value modulo 32 must be in the range " +
                     "1 -> 26, excluding 24",
                     "PCL symbol set number invalid",
                     MessageBoxButton.OK,
                     MessageBoxImage.Error);

                txtPCLXLSymSetNo.Focus();
                txtPCLXLSymSetNo.SelectAll();
            }
        }

        return OK;
    }
}
