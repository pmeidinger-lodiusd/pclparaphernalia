using System.Text;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles a PCL Font object.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(
        Feature = "renaming",
        ApplyToMembers = true)]

    class PCLFont
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private PCLFonts.eFontType _fontType;

        private string _fontName;

        private short _fontIndex;

        private ushort _symSetNumber;
        private ushort _symSetDefault;

        private ushort _typeface;

        private double _pitch;
        private double _pointSize;
        private byte _contourRatio;

        private bool _bound;
        private bool _proportional;
        private bool _scalable;

        private bool _varRegular;
        private bool _varItalic;
        private bool _varBold;
        private bool _varBoldItalic;

        private ushort _styleRegular;
        private ushort _styleItalic;
        private ushort _styleBold;
        private ushort _styleBoldItalic;

        private short _weightRegular;
        private short _weightItalic;
        private short _weightBold;
        private short _weightBoldItalic;

        private string _nameRegular;
        private string _nameItalic;
        private string _nameBold;
        private string _nameBoldItalic;

        private ushort[] _symSets;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L F o n t                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLFont(short fontIndex,
                        PCLFonts.eFontType fontType,
                        string fontName,
                        bool bound,
                        bool proportional,
                        bool scalable,
                        ushort symbolSet,
                        ushort typeface,
                        byte contourRatio,
                        double pitch,
                        double pointSize,
                        bool varRegular,
                        ushort styleRegular,
                        short weightRegular,
                        string nameRegular,
                        bool varItalic,
                        ushort styleItalic,
                        short weightItalic,
                        string nameItalic,
                        bool varBold,
                        ushort styleBold,
                        short weightBold,
                        string nameBold,
                        bool varBoldItalic,
                        ushort styleBoldItalic,
                        short weightBoldItalic,
                        string nameBoldItalic,
                        ushort[] symSets)
        {
            _fontIndex = fontIndex;
            _fontType = fontType;
            _fontName = fontName;

            _bound = bound;
            _proportional = proportional;
            _scalable = scalable;

            _symSetNumber = symbolSet;
            _typeface = typeface;

            _contourRatio = contourRatio;
            _pitch = pitch;
            _pointSize = pointSize;

            _varRegular = varRegular;
            _varItalic = varItalic;
            _varBold = varBold;
            _varBoldItalic = varBoldItalic;

            _styleRegular = styleRegular;
            _styleItalic = styleItalic;
            _styleBold = styleBold;
            _styleBoldItalic = styleBoldItalic;

            _weightRegular = weightRegular;
            _weightItalic = weightItalic;
            _weightBold = weightBold;
            _weightBoldItalic = weightBoldItalic;

            _nameRegular = nameRegular;
            _nameItalic = nameItalic;
            _nameBold = nameBold;
            _nameBoldItalic = nameBoldItalic;

            _symSets = symSets;

            _symSetDefault = 14;       // = "0N"
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // B o u n d S y m b o l S e t                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If the font is bound to a particular symbol set, return the PCL    //
        // identifier of that set as a string; otherwise return a null string.//
        //                                                                    //
        //--------------------------------------------------------------------//

        public string BoundSymbolSet
        {
            get
            {
                if (_bound)
                    return PCLSymbolSets.translateKind1ToId(_symSetNumber);
                else
                    return "";
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t H P G L 2 F o n t D e f                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the HP-GL/2 font definition command.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string getHPGL2FontDef(PCLFonts.eVariant variant,
                                       ushort symbolSet,
                                       double height,
                                       double pitch)
        {
            StringBuilder cmd = new StringBuilder(255);

            if (_symSetNumber != 0)
                cmd.Append("1,").Append(_symSetNumber);
            else
                cmd.Append("1,").Append(symbolSet);

            if (_proportional)
                cmd.Append(",2,1");
            else
                cmd.Append(",2,0");

            if (_scalable)
            {
                // Scalable; the size parameter defines the required size.

                if (_proportional)
                {
                    // Scalable; proportionally-spaced
                    cmd.Append(",4,").Append(height);
                }
                else
                {
                    // Scalable; fixed-pitch
                    cmd.Append(",3,").Append(pitch);
                }
            }
            else
            {
                // Bitmap; the size is pre-defined.

                if (_proportional)
                    // Bitmap; proportionally-spaced
                    cmd.Append(",4,").Append(_pointSize);
                else
                    // Bitmap; fixed-pitch
                    cmd.Append(",3,").Append(_pitch);
                cmd.Append(",4,").Append(_pointSize);
            }

            if (variant == PCLFonts.eVariant.Italic)
            {
                cmd.Append(",5,").Append(_styleItalic);
                cmd.Append(",6,").Append(_weightItalic);
            }
            else if (variant == PCLFonts.eVariant.Bold)
            {
                cmd.Append(",5,").Append(_styleBold);
                cmd.Append(",6,").Append(_weightBold);
            }
            else if (variant == PCLFonts.eVariant.BoldItalic)
            {
                cmd.Append(",5,").Append(_styleBoldItalic);
                cmd.Append(",6,").Append(_weightBoldItalic);
            }
            else
            {
                cmd.Append(",5,").Append(_styleRegular);
                cmd.Append(",6,").Append(_weightRegular);
            }

            cmd.Append(",7,").Append(_typeface);

            return cmd.ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L C o n t o u r R a t i o                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL font contour ratio value.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte getPCLContourRatio()
        {
            return _contourRatio;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L F o n t I d D a t a                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return identifying (reference) name and PCL typeface identifier    //
        // of the font, for fonts of type 'PresetTypeface' or 'PresetFamily'. //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool getPCLFontIdData(ref ushort typeface,
                                         ref string fontName)
        {
            bool presetFont = false;

            if ((_fontType == PCLFonts.eFontType.PresetTypeface) ||
                (_fontType == PCLFonts.eFontType.PresetFamily))
            {
                presetFont = true;
                typeface = _typeface;
                fontName = _fontName;
            }
            else
            {
                presetFont = false;
            }

            return presetFont;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L F o n t S e l e c t                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL font selection sequence.                            //
        // ... except for the root '<esc>(' prefix.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string getPCLFontSelect(PCLFonts.eVariant variant,
                                        double height,
                                        double pitch)
        {
            string seq;

            if (_proportional)
                seq = "s1p";
            else
                seq = "s0p";

            if (_scalable)
            {
                // Scalable; the size parameter defines the required size.

                if (_proportional)
                {
                    // Scalable; proportionally-spaced
                    seq += height.ToString() + "v";
                }
                else
                {
                    // Scalable; fixed-pitch
                    if (pitch != 0.0)
                    {
                        seq += pitch.ToString() + "h";
                    }
                    else
                    {
                        double calcPitch = (7200 /
                                                (height * _contourRatio));
                        seq += calcPitch.ToString("F2") + "h";
                    }
                }
            }
            else
            {
                // Bitmap; the size is pre-defined.

                if (_proportional)
                    // Bitmap; proportionally-spaced
                    seq += _pointSize.ToString() + "v";
                else
                    // Bitmap; fixed-pitch
                    seq += _pointSize.ToString() + "v" +
                           _pitch.ToString() + "h";
            }

            if (variant == PCLFonts.eVariant.Italic)
            {
                seq += _styleItalic.ToString() + "s" +
                       _weightItalic.ToString() + "b";
            }
            else if (variant == PCLFonts.eVariant.Bold)
            {
                seq += _styleBold.ToString() + "s" +
                       _weightBold.ToString() + "b";
            }
            else if (variant == PCLFonts.eVariant.BoldItalic)
            {
                seq += _styleBoldItalic.ToString() + "s" +
                       _weightBoldItalic.ToString() + "b";
            }
            else
            {
                seq += _styleRegular.ToString() + "s" +
                       _weightRegular.ToString() + "b";
            }

            seq += _typeface + "T";

            return seq;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L H e i g h t                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL font height characteristic value.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public double getPCLHeight()
        {
            return _pointSize;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L P i t c h                                       I      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL font pitch characteristic value.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public double getPCLPitch()
        {
            return _pitch;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L P i t c h                                      I I     //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL font pitch characteristic value equivalent to the   //
        // supplied height.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public double getPCLPitch(double ptSize)
        {
            return (double)((7200 / (ptSize * _contourRatio)));
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L S p a c i n g                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL font spacing characteristic value.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte getPCLSpacing()
        {
            if (_proportional)
                return 1;
            else
                return 0;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L S t y l e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL font style characteristic value.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort getPCLStyle(PCLFonts.eVariant variant)
        {
            if (variant == PCLFonts.eVariant.Italic)
                return _styleItalic;
            else if (variant == PCLFonts.eVariant.Bold)
                return _styleBold;
            else if (variant == PCLFonts.eVariant.BoldItalic)
                return _styleBoldItalic;
            else
                return _styleRegular;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L W e i g h t                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL font weight characteristic value.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public short getPCLWeight(PCLFonts.eVariant variant)
        {
            if (variant == PCLFonts.eVariant.Italic)
                return _weightItalic;
            else if (variant == PCLFonts.eVariant.Bold)
                return _weightBold;
            else if (variant == PCLFonts.eVariant.BoldItalic)
                return _weightBoldItalic;
            else
                return _weightRegular;
        }
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L X L H e i g h t                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL XL font height (pointsize) value.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public double getPCLXLHeight()
        {
            return _pointSize;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L X L N a m e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL XL font name for the selected font.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string getPCLXLName(PCLFonts.eVariant variant)
        {
            if (variant == PCLFonts.eVariant.Italic)
                return _nameItalic;
            else if (variant == PCLFonts.eVariant.Bold)
                return _nameBold;
            else if (variant == PCLFonts.eVariant.BoldItalic)
                return _nameBoldItalic;
            else
                return _nameRegular;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t S y m b o l S e t N u m b e r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the symbol set (Kind1) value.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort getSymbolSetNumber()
        {
            if (_bound)
                return _symSetNumber;
            else
                return _symSetDefault;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // H e i g h t                                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If the font has a defined (non-zero) point size value, return the  //
        // value as a string; otherwise return a null string.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Height
        {
            get
            {
                if (_pitch != 0)
                    return _pointSize.ToString();
                else
                    return "";
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I n d e x N o                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the index number of the font.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public short IndexNo
        {
            get
            {
                return _fontIndex;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i s B o u n d F o n t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return a value indicating whether or not the font is bound to a    //
        // particular symbol set.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool isBoundFont()
        {
            return _bound;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i s P r e s e t F o n t                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return a value indicating whether or not the font is one of the    //
        // preset ones (i.e. NOT a <custom> or <soft font>) entry).           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool isPresetFont()
        {
            if ((_fontType == PCLFonts.eFontType.Custom) ||
                (_fontType == PCLFonts.eFontType.Download) ||
                (_fontType == PCLFonts.eFontType.PrnDisk))
                return false;
            else
                return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i s P r o p o r t i o n a l F o n t                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return a value indicating if the font is proportionally-spaced, or //
        // fixed-pitch.                                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool isProportionalFont()
        {
            return _proportional;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i s S y m S e t I n L i s t                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return a value indicating whether or not the specified symbol set  //
        // (Kind1) number is in the list of those (probably) supported by     //
        // this font.                                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool isSymSetInList(ushort symSetNo)
        {
            int symSetCt = _symSets.Length;

            bool symSetFound = false;

            //----------------------------------------------------------------//
            //                                                                //
            // Check whether or not symbol set is in the 'supported' list.    //
            //                                                                //
            //----------------------------------------------------------------//

            for (int i = 0; i < symSetCt; i++)
            {
                if (_symSets[i] == symSetNo)
                {
                    symSetFound = true;
                    i = symSetCt;   // force end of loop
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // If not in the list, allow it if it is an unbound font and the  //
            // symbol set is Unicode.                                         //
            //                                                                //
            //----------------------------------------------------------------//

            if ((!symSetFound) && (!_bound))
            {
                if (PCLSymbolSets.getKind1(PCLSymbolSets.IndexUnicode) ==
                    symSetNo)
                {
                    symSetFound = true;
                }
            }

            return symSetFound;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // N a m e                                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the font reference name.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Name
        {
            get { return _fontName; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P i t c h                                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If the font has a defined (non-zero) pitch value, return the value //
        // as a string; otherwise return a null string.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Pitch
        {
            get
            {
                if (_pitch != 0)
                    return _pitch.ToString();
                else
                    return "";
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S c a l a b l e                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return a value indicating if the font is scalable, or is a bitmap  //
        // font (available only in a particular fixed size).                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool Scalable
        {
            get
            {
                return _scalable;
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S p a c i n g                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return a string value describing the font spacing: fixed-pitch or  //
        // proportionally-spaced.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Spacing
        {
            get
            {
                if (_proportional)
                    return "proportional";
                else
                    return "fixed-pitch";
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S y m b o l S e t B i n d i n g                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If the font is bound to a specific symbol set, return the PCL      //
        // identifier of that set; otherwise return a null string.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string SymbolSetBinding
        {
            get
            {
                if (_proportional)
                    return "proportional";
                else
                    return "fixed-pitch";
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S y m b o l S e t C t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the count of a the (probable) supported symbol sets.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int SymbolSetCt
        {
            get
            {
                return _symSets.Length;
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S y m b o l S e t R o w s                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return an array of strings containing a list of the (probable)     //
        // supported symbol sets.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string[] SymbolSetRows
        {
            get
            {
                const int maxPerRow = 20;
                const int itemSize = 5;   // typical size e.g "19U, "

                int symSetCt,
                      rowCt,
                      rowSize,
                      rowIndx;

                string[] mapRows;

                StringBuilder crntRow;

                symSetCt = _symSets.Length;

                rowCt = symSetCt / maxPerRow;
                if (symSetCt - (rowCt * maxPerRow) != 0)
                    rowCt++;

                rowSize = itemSize * maxPerRow;
                rowIndx = 0;

                mapRows = new string[rowCt];

                crntRow = new StringBuilder(rowSize);

                for (int i = 0; i < symSetCt; i++)
                {
                    string symSetId =
                        PCLSymbolSets.translateKind1ToId(_symSets[i]);

                    crntRow.Append(symSetId);

                    if (i < (symSetCt - 1))
                        crntRow.Append(", ");

                    if ((i > 0) && ((i % maxPerRow) == 0))
                    {
                        mapRows[rowIndx] = crntRow.ToString();

                        crntRow.Clear();
                        rowIndx++;
                    }
                }

                if (crntRow.Length > 0)
                    mapRows[rowIndx] = crntRow.ToString();

                return mapRows;
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S y m b o l S e t s                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return a string containing a list of the (probable) supported      //
        // symbol sets.                                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string SymbolSets
        {
            get
            {
                const int maxPerRow = 20;

                int symSetCt = _symSets.Length;

                StringBuilder symSetList = new StringBuilder(255);

                for (int i = 0; i < symSetCt; i++)
                {
                    string symSetId =
                        PCLSymbolSets.translateKind1ToId(_symSets[i]);

                    symSetList.Append(symSetId);

                    if (i < (symSetCt - 1))
                        symSetList.Append(", ");

                    if ((i > 0) && ((i % maxPerRow) == 0))
                        symSetList.Append("\r\n");
                }

                return symSetList.ToString();
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T y p e                                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the symbol set type.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLFonts.eFontType Type
        {
            get { return _fontType; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T y p e f a c e                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL font typeface characteristic value.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort Typeface
        {
            get { return _typeface; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // V a r _ B o l d                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Indicates whether or not the font has a Bold variant.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool Var_Bold
        {
            get { return _varBold; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // V a r _ B o l d I t a l i c                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Indicates whether or not the font has a Bold Italic variant.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool Var_BoldItalic
        {
            get { return _varBoldItalic; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // V a r _ I t a l i c                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Indicates whether or not the font has an Italic variant.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool Var_Italic
        {
            get { return _varItalic; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // V a r _ R e g u l a r                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Indicates whether or not the font has a Regular variant.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool Var_Regular
        {
            get { return _varRegular; }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v a r i a n t A v a i l a b l e                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return indication of whether or not the selected variant exists.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool variantAvailable(PCLFonts.eVariant variant)
        {
            if (variant == PCLFonts.eVariant.Italic)
                return _varItalic;
            else if (variant == PCLFonts.eVariant.Bold)
                return _varBold;
            else if (variant == PCLFonts.eVariant.BoldItalic)
                return _varBoldItalic;
            else
                return _varRegular;
        }
    }
}