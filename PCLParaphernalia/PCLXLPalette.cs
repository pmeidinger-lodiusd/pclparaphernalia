﻿namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles a PCLXL Palette object.</para>
    /// <para>© Chris Hutchinson 2014</para>
    ///
    /// </summary>
    internal class PCLXLPalette
    {
        private byte _crntClrItem;

        private byte _clrItemWhite;
        private byte _clrItemBlack;

        private readonly byte[] _colourIds;
        private readonly int[] _colourRGBs;
        private readonly string[] _colourNames;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L P a l e t t e                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLXLPalette(string name, bool flagMonochrome, byte ctClrItems)
        {
            PaletteName = name;
            Monochrome = flagMonochrome;
            CtClrItems = ctClrItems;

            _colourIds = new byte[ctClrItems];
            _colourNames = new string[ctClrItems];
            _colourRGBs = new int[ctClrItems];

            _crntClrItem = 0;
            _clrItemBlack = 0;
            _clrItemWhite = 0;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d C o l o u r                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Add a colour to the next slot in the palette.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void AddColour(string name, int RGB, byte id)
        {
            _colourNames[_crntClrItem] = name;
            _colourRGBs[_crntClrItem] = RGB;
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
        // Return the count of colour items.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte CtClrItems { get; }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o l o u r N a m e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the colour name for the specified colour item.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetColourName(int item) => _colourNames[item];

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o l o u r I d                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the colour identifier for the specified colour item.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte GetColourId(int item) => _colourIds[item];

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o l o u r R G B                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the RGB value for the specified colour item.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int GetColourRGB(int item) => _colourRGBs[item];

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t G r a y L e v e l                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the name of the gray level associated with the specified value.//
        // This is only relevant to the Gray palette.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetGrayLevel(byte item)
        {
            if (item == 0)
            {
                return item.ToString() +
                       " (" + _colourNames[_clrItemBlack] + ")";
            }
            else if (item == 255)
            {
                return item.ToString() +
                       " (" + _colourNames[_clrItemWhite] + ")";
            }
            else
            {
                return item.ToString();
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M o n o c h r o m e                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool Monochrome { get; }

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

        public void SetClrItemBlack() => _clrItemBlack = (byte)(_crntClrItem - 1);

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C l r I t e m W h i t e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set the White index to the current entry.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetClrItemWhite() => _clrItemWhite = (byte)(_crntClrItem - 1);
    }
}