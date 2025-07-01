namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles a PCL Palette object.
    /// 
    /// © Chris Hutchinson 2014
    /// 
    /// </summary>

    class PCLPalette
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private string _name;

        private bool _flagMonochrome;

        private short _paletteId;
        private byte _ctClrItems;
        private byte _crntClrItem;

        private byte _clrItemWhite;
        private byte _clrItemBlack;

        private byte[] _colourIds;
        private string[] _colourNames;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L P a l e t t e                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLPalette(string name,
                           bool flagMonochrome,
                           short paletteId,
                           byte ctClrItems)
        {
            _name = name;
            _flagMonochrome = flagMonochrome;
            _paletteId = paletteId;
            _ctClrItems = ctClrItems;

            _colourIds = new byte[ctClrItems];
            _colourNames = new string[ctClrItems];

            _crntClrItem = 0;
            _clrItemWhite = 0;
            _clrItemBlack = 0;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d C o l o u r                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Add a colour to the next slot in the palette.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void addColour(string name,
                               byte id)
        {
            _colourNames[_crntClrItem] = name;
            _colourIds[_crntClrItem] = id;

            _crntClrItem++;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // C l r I t e m B l a c k                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte ClrItemBlack
        {
            get { return _clrItemBlack; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // C l r I t e m W h i t e                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte ClrItemWhite
        {
            get { return _clrItemWhite; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // C t C l r I t e m s                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the count of colour items in the palette.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte CtClrItems
        {
            get { return _ctClrItems; }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o l o u r I d                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the PCL colour identifier for the specified colour entry.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte getColourId(int item)
        {
            return _colourIds[item];
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o l o u r N a m e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the colour name for the specified colour entry.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string getColourName(int item)
        {
            return _colourNames[item];
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M o n o c h r o m e                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool Monochrome
        {
            get { return _flagMonochrome; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P a l e t t e I d                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL identifier value for the palette.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public short PaletteId
        {
            get { return _paletteId; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P a l e t t e N a m e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the palette name.                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string PaletteName
        {
            get { return _name; }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C l r I t e m B l a c k                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set the Black index to the current entry.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void setClrItemBlack()
        {
            _clrItemBlack = (byte)(_crntClrItem - 1);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C l r I t e m W h i t e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set the White index to the current entry.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void setClrItemWhite()
        {
            _clrItemWhite = (byte)(_crntClrItem - 1);
        }
    }
}