namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles a PCL Palette object.</para>
    /// <para>© Chris Hutchinson 2014</para>
    ///
    /// </summary>
    internal class PCLPalette
    {
        private byte _crntClrItem;

        private byte _clrItemWhite;
        private byte _clrItemBlack;

        private readonly byte[] _colourIds;
        private readonly string[] _colourNames;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L P a l e t t e                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLPalette(string name, bool flagMonochrome, short paletteId, byte ctClrItems)
        {
            PaletteName = name;
            Monochrome = flagMonochrome;
            PaletteId = paletteId;
            CtClrItems = ctClrItems;

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

        public void AddColour(string name, byte id)
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

        public byte ClrItemBlack => _clrItemBlack;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // C l r I t e m W h i t e                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte ClrItemWhite => _clrItemWhite;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // C t C l r I t e m s                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the count of colour items in the palette.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte CtClrItems { get; }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o l o u r I d                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the PCL colour identifier for the specified colour entry.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte GetColourId(int item) => _colourIds[item];

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o l o u r N a m e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the colour name for the specified colour entry.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetColourName(int item) => _colourNames[item];

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M o n o c h r o m e                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool Monochrome { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P a l e t t e I d                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL identifier value for the palette.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public short PaletteId { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P a l e t t e N a m e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the palette name.                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string PaletteName { get; }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C l r I t e m B l a c k                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set the Black index to the current entry.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetClrItemBlack()
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

        public void SetClrItemWhite()
        {
            _clrItemWhite = (byte)(_crntClrItem - 1);
        }
    }
}