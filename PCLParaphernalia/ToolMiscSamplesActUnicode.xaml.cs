﻿using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// Interaction logic for ToolMiscSamples.xaml
    /// 
    /// Class handles the MiscSamples: Unicode Characters tab.
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

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly int _ctUnicodeFonts = PCLFonts.GetCountUnique();

        private static readonly int[] _subsetUnicodeFonts = new int[_ctUnicodeFonts];

        private static uint _unicodeUCS2PCL;
        private static uint _unicodeUCS2PCLXL;

        private static int _indxUnicodeFontPCL;
        private static int _indxUnicodeFontPCLXL;

        private static PCLFonts.eVariant _unicodeFontVarPCL;
        private static PCLFonts.eVariant _unicodeFontVarPCLXL;

        private bool _flagUnicodeFormAsMacroPCL;
        private bool _flagUnicodeFormAsMacroPCLXL;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b U n i c o d e C p _ S e l e c t i o n C h a n g e d            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Code-point item has changed.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbUnicodeCp_SelectionChanged(object sender,
                                                   SelectionChangedEventArgs e)
        {
            if (_initialised)
            {
                string utf8HexVal = string.Empty;
                uint unicodeUCS2;

                unicodeUCS2 = (uint)((cbUnicodeCp01.SelectedIndex * 256) +
                                         cbUnicodeCp02.SelectedIndex);

                string unicodeBlock =
                    UnicodeBlocks.GetBlocknameForCodepoint(unicodeUCS2);

                UnicodeCategory unicodeCat =
                    CharUnicodeInfo.GetUnicodeCategory((char)unicodeUCS2);

                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                    _unicodeUCS2PCL = unicodeUCS2;
                else
                    _unicodeUCS2PCLXL = unicodeUCS2;

                PrnParseDataUTF8.ConvertUTF32ToUTF8HexString(unicodeUCS2,
                                                              true,
                                                              ref utf8HexVal);

                txtUnicodeUTF8.Text = utf8HexVal;

                txtUnicodeBlock.Text = unicodeBlock;
                txtUnicodeCat.Text = unicodeCat.ToString();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c b U n i c o d e F o n t _ S e l e c t i o n C h a n g e d        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Font item has changed.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void cbUnicodeFont_SelectionChanged(object sender,
                                                    SelectionChangedEventArgs e)
        {
            if (_initialised && cbUnicodeFont.HasItems)
            {
                int indxFont = cbUnicodeFont.SelectedIndex;
                bool samePreset = false;

                if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                {
                    if (indxFont == _indxUnicodeFontPCL)
                        samePreset = true;
                    else
                        _indxUnicodeFontPCL = indxFont;

                    setFontOptionsVariants(_indxUnicodeFontPCL,
                                            samePreset,
                                            ref _unicodeFontVarPCL);
                }
                else
                {
                    if (indxFont == _indxUnicodeFontPCLXL)
                        samePreset = true;
                    else
                        _indxUnicodeFontPCLXL = indxFont;

                    setFontOptionsVariants(_indxUnicodeFontPCLXL,
                                            samePreset,
                                            ref _unicodeFontVarPCLXL);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e D a t a U n i c o d e                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialise 'Unicode chars' data.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void initialiseDataUnicode()
        {
            int index,
                  ctr;

            _initialised = false;

            lbOrientation.Visibility = Visibility.Hidden;
            cbOrientation.Visibility = Visibility.Hidden;

            //----------------------------------------------------------------//

            cbUnicodeFont.Items.Clear();

            ctr = PCLFonts.GetCount();
            index = 0;

            for (int i = 0; i < ctr; i++)
            {
                if ((PCLFonts.IsBoundFont(i) == false) &&
                    ((PCLFonts.GetType(i) ==
                     PCLFonts.eFontType.PresetTypeface) ||
                    (PCLFonts.GetType(i) ==
                     PCLFonts.eFontType.PresetFamilyMember)))
                {
                    _subsetUnicodeFonts[index++] = i;
                    cbUnicodeFont.Items.Add(PCLFonts.GetName(i));
                }
            }

            //----------------------------------------------------------------//

            cbUnicodeCp01.Items.Clear();

            for (int i = 0; i < 0x0100; i++)
            {
                cbUnicodeCp01.Items.Add(i.ToString("x2"));
            }

            cbUnicodeCp02.Items.Clear();

            for (int i = 0; i < 0x0100; i++)
            {
                cbUnicodeCp02.Items.Add(i.ToString("x2"));
            }

            //----------------------------------------------------------------//

            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                if ((_indxUnicodeFontPCL < 0) ||
                    (_indxUnicodeFontPCL >= _ctUnicodeFonts))
                    _indxUnicodeFontPCL = 0;

                cbUnicodeFont.SelectedIndex = _indxUnicodeFontPCL;

                setFontOptionsVariants(_indxUnicodeFontPCL,
                                        true,
                                        ref _unicodeFontVarPCL);

                cbUnicodeCp01.SelectedIndex = (int)(_unicodeUCS2PCL / 256);
                cbUnicodeCp02.SelectedIndex = (int)(_unicodeUCS2PCL % 256);

                chkOptFormAsMacro.IsChecked = _flagUnicodeFormAsMacroPCL;
            }
            else
            {
                if ((_indxUnicodeFontPCLXL < 0) ||
                    (_indxUnicodeFontPCLXL >= _ctUnicodeFonts))
                    _indxUnicodeFontPCLXL = 0;

                cbUnicodeFont.SelectedIndex = _indxUnicodeFontPCLXL;

                setFontOptionsVariants(_indxUnicodeFontPCLXL,
                                        true,
                                        ref _unicodeFontVarPCLXL);

                cbUnicodeCp01.SelectedIndex = (int)(_unicodeUCS2PCLXL / 256);
                cbUnicodeCp02.SelectedIndex = (int)(_unicodeUCS2PCLXL % 256);

                chkOptFormAsMacro.IsChecked = _flagUnicodeFormAsMacroPCLXL;
            }

            _initialised = true;

            initialiseDescUnicode();

            cbUnicodeCp_SelectionChanged(this, null);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e D e s c U n i c o d e                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Initialise 'Unicode' description.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void initialiseDescUnicode()
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                txtUnicodeDesc.Text =
                    "Most printer-resident fonts are unbound, which means" +
                    " that they include more than the maximum of 256" +
                    " characters that can be selected using an 8-bit" +
                    " character-code." +
                    "\r\n\r\n" +
                    "Different sets of characters are usually accessible " +
                    " via use of character sets (known as symbol sets " +
                    " within PCL), which map sets of character-codes to the " +
                    " required glyphs (the character shapes)." +
                    "\r\n\r\n" +
                    "Most modern printer-resident fonts are encapsulated" +
                    " TrueType outline fonts, and the glyphs are indexed" +
                    " using Unicode code-point values." +
                    "\r\n\r\n" +
                    "Some PCL printers support a multi-byte text-parsing " +
                    " mechanism which, in conjunction with" +
                    " selection of the Unicode symbol set, allows the direct" +
                    " selection of Unicode code-points (albeit encoded as" +
                    " UTF-8 strings)." +
                    "\r\n\r\n" +
                    "The desired effect will obviously only be achieved if" +
                    " the selected font includes the targetted glyph." +
                    "\r\n\r\n" +
                    "The Unicode standard allows code-point values up to" +
                    " and including U+10FFFF.\r\n" +
                    "Printer fonts appear to be limited to 16-bit character" +
                    " code identifiers, so this tool limits the sample " +
                    " code-point to be no more than U+FFFF.";
            }
            else
            {
                txtUnicodeDesc.Text =
                    "Most printer-resident fonts are unbound, which means" +
                    " that they include more than the maximum of 256" +
                    " characters that can be selected using an 8-bit" +
                    " character-code." +
                    "\r\n\r\n" +
                    "Different sets of characters are usually accessible " +
                    " via use of character sets (known as symbol sets " +
                    " within PCL), which map sets of character-codes to the " +
                    " required glyphs (the character shapes)." +
                    "\r\n\r\n" +
                    "Most modern printer-resident fonts are encapsulated" +
                    " TrueType outline fonts, and the glyphs are indexed" +
                    " using Unicode code-point values." +
                    "\r\n\r\n" +
                    "With PCL XL, character-codes can be specified as 8-bit" +
                    " or 16-bit; using the latter, in conjunction with" +
                    " selection of the Unicode symbol set, allows the direct" +
                    " selection of Unicode code-points." +
                    "\r\n\r\n" +
                    "The desired effect will obviously only be achieved if" +
                    " the selected font includes the targetted glyph." +
                    "\r\n\r\n" +
                    "The Unicode standard allows code-point values up to" +
                    " and including U+10FFFF.\r\n" +
                    "Printer fonts appear to be limited to 16-bit character" +
                    " code identifiers, so this tool limits the sample " +
                    " code-point to be no more than U+FFFF.";
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d D a t a U n i c o d e                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load current metrics from persistent storage.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void metricsLoadDataUnicode()
        {
            int tmpInt = 0;

            ToolMiscSamplesPersist.LoadDataTypeUnicode(
                "PCL",
                ref _indxUnicodeFontPCL,
                ref _unicodeFontVarPCL,
                ref tmpInt,
                ref _flagUnicodeFormAsMacroPCL);

            _unicodeUCS2PCL = (uint)tmpInt;

            ToolMiscSamplesPersist.LoadDataTypeUnicode(
                "PCLXL",
                ref _indxUnicodeFontPCLXL,
                ref _unicodeFontVarPCLXL,
                ref tmpInt,
                ref _flagUnicodeFormAsMacroPCLXL);

            _unicodeUCS2PCLXL = (uint)tmpInt;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s S a v e D a t a U n i c o d e                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Save current metrics to persistent storage.                        //
        // Only relevant to PCL.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void metricsSaveDataUnicode()
        {
            ToolMiscSamplesPersist.SaveDataTypeUnicode(
                "PCL",
                _indxUnicodeFontPCL,
                _unicodeFontVarPCL,
                (int)_unicodeUCS2PCL,
                _flagUnicodeFormAsMacroPCL);

            ToolMiscSamplesPersist.SaveDataTypeUnicode(
                "PCLXL",
                _indxUnicodeFontPCLXL,
                _unicodeFontVarPCLXL,
                (int)_unicodeUCS2PCLXL,
                _flagUnicodeFormAsMacroPCLXL);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b U n i c o d e F o n t V a r B _ C l i c k                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Bold' font variant radio button is selected.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbUnicodeFontVarB_Click(object sender, RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _unicodeFontVarPCL = PCLFonts.eVariant.Bold;
            else
                _unicodeFontVarPCLXL = PCLFonts.eVariant.Bold;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b U n i c o d e F o n t V a r B I _ C l i c k                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Bold Italic' font variant radio button is         //
        // selected.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbUnicodeFontVarBI_Click(object sender, RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _unicodeFontVarPCL = PCLFonts.eVariant.BoldItalic;
            else
                _unicodeFontVarPCLXL = PCLFonts.eVariant.BoldItalic;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b U n i c o d e F o n t V a r I _ C l i c k                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Italic' font variant radio button is selected.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbUnicodeFontVarI_Click(object sender, RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _unicodeFontVarPCL = PCLFonts.eVariant.Italic;
            else
                _unicodeFontVarPCLXL = PCLFonts.eVariant.Italic;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r b U n i c o d e F o n t V a r R _ C l i c k                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Called when the 'Regular' font variant radio button is selected.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void rbUnicodeFontVarR_Click(object sender, RoutedEventArgs e)
        {
            if (_crntPDL == ToolCommonData.ePrintLang.PCL)
                _unicodeFontVarPCL = PCLFonts.eVariant.Regular;
            else
                _unicodeFontVarPCLXL = PCLFonts.eVariant.Regular;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F l a g U n i c o d e F o r m A s M a c r o                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set or unset 'Render fixed text as overlay' flag.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void setFlagUnicodeFormAsMacro(
            bool setFlag,
            ToolCommonData.ePrintLang crntPDL)
        {
            if (crntPDL == ToolCommonData.ePrintLang.PCL)
            {
                if (setFlag)
                    _flagUnicodeFormAsMacroPCL = true;
                else
                    _flagUnicodeFormAsMacroPCL = false;
            }
            else if (crntPDL == ToolCommonData.ePrintLang.PCLXL)
            {
                if (setFlag)
                    _flagUnicodeFormAsMacroPCLXL = true;
                else
                    _flagUnicodeFormAsMacroPCLXL = false;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t F o n t O p t i o n s V a r i a n t s                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set variant options relevant to selected font.                     //
        // Assume that this is a PresetTypeface or PresetFamilyMember type,   //
        // because this is all that we've added to the font selection subset. //
        //                                                                    // 
        //--------------------------------------------------------------------//

        private void setFontOptionsVariants(int indxFont,
                                             bool samePreset,
                                             ref PCLFonts.eVariant fontVar)
        {
            bool varB,
                    varBI,
                    varI,
                    varR,
                    varSet;

            int fontIndx;

            fontIndx = _subsetUnicodeFonts[indxFont];

            varSet = false;

            //----------------------------------------------------------------//

            rbUnicodeFontVarB.Visibility = Visibility.Hidden;
            rbUnicodeFontVarBI.Visibility = Visibility.Hidden;
            rbUnicodeFontVarI.Visibility = Visibility.Hidden;
            rbUnicodeFontVarR.Visibility = Visibility.Hidden;

            rbUnicodeFontVarB.IsChecked = false;
            rbUnicodeFontVarBI.IsChecked = false;
            rbUnicodeFontVarI.IsChecked = false;
            rbUnicodeFontVarR.IsChecked = false;

            //----------------------------------------------------------------//

            varR = PCLFonts.VariantExists(fontIndx,
                                           PCLFonts.eVariant.Regular);

            varI = PCLFonts.VariantExists(fontIndx,
                                           PCLFonts.eVariant.Italic);

            varB = PCLFonts.VariantExists(fontIndx,
                                           PCLFonts.eVariant.Bold);

            varBI = PCLFonts.VariantExists(fontIndx,
                                            PCLFonts.eVariant.BoldItalic);

            //----------------------------------------------------------------//

            if (varR)
                rbUnicodeFontVarR.Visibility = Visibility.Visible;

            if (varI)
                rbUnicodeFontVarI.Visibility = Visibility.Visible;

            if (varB)
                rbUnicodeFontVarB.Visibility = Visibility.Visible;

            if (varBI)
                rbUnicodeFontVarBI.Visibility = Visibility.Visible;

            //----------------------------------------------------------------//

            if (samePreset)
            {
                if ((varR) && (fontVar == PCLFonts.eVariant.Regular))
                {
                    rbUnicodeFontVarR.IsChecked = true;
                    varSet = true;
                }

                if ((varI) && (fontVar == PCLFonts.eVariant.Italic))
                {
                    rbUnicodeFontVarI.IsChecked = true;
                    varSet = true;
                }

                if ((varB) && (fontVar == PCLFonts.eVariant.Bold))
                {
                    rbUnicodeFontVarB.IsChecked = true;
                    varSet = true;
                }

                if ((varBI) && (fontVar == PCLFonts.eVariant.BoldItalic))
                {
                    rbUnicodeFontVarBI.IsChecked = true;
                    varSet = true;
                }
            }

            //----------------------------------------------------------------//

            if (!varSet)
            {
                if (varR)
                {
                    rbUnicodeFontVarR.IsChecked = true;
                    fontVar = PCLFonts.eVariant.Regular;
                }
                else if (varI)
                {
                    rbUnicodeFontVarI.IsChecked = true;
                    fontVar = PCLFonts.eVariant.Italic;
                }
                else if (varB)
                {
                    rbUnicodeFontVarB.IsChecked = true;
                    fontVar = PCLFonts.eVariant.Bold;
                }
                else if (varBI)
                {
                    rbUnicodeFontVarBI.IsChecked = true;
                    fontVar = PCLFonts.eVariant.BoldItalic;
                }
            }
        }
    }
}
