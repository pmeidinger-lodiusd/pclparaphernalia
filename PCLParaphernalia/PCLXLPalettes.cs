namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides definitions for simple colour palettes.
    /// 
    /// © Chris Hutchinson 2014
    /// 
    /// </summary>

    public static class PCLXLPalettes
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum eIndex : byte
        {
            PCLXLGray = 0,
            PCLXLRGB
        }

        private enum eMono_2 : byte
        {
            White = 0,
            Black
        }

        private enum eRGB_16 : byte
        {
            Black = 0,
            Red,
            Green,
            Yellow,
            Blue,
            Magenta,
            Cyan,
            White,
            Gray1,
            Gray2,
            Gray3,
            Gray4,
            Gray5,
            Gray6,
            Gray7,
            Gray8
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly PCLXLPalette[] _palettes =
        {
            new PCLXLPalette ("Gray",
                              true,
                              2),   // black & white

            new PCLXLPalette ("RGB",
                              false,
                              16)   // include 8 'pure' colours + 8 grays
        };

        private static readonly int _paletteCount =
            _palettes.GetUpperBound(0) + 1;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L P a l e t t e s                                          //
        //                                                                    //
        // In each palette, the colours are added in alphabetic order, rather //
        // than the order of the colour values or the PCL indices.            //
        // This is in order to to make it easier to keep the same list of     //
        // available colours in client interfaces when switching between the  //
        // different palettes.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PCLXLPalettes()
        {
            int crntIndex;

            //----------------------------------------------------------------//

            crntIndex = (int)eIndex.PCLXLGray;

            _palettes[crntIndex].addColour("Black", 0x00000000,
                                             (byte)eMono_2.Black);
            _palettes[crntIndex].setClrItemBlack();
            _palettes[crntIndex].addColour("White", 0x00ffffff,
                                             (byte)eMono_2.White);
            _palettes[crntIndex].setClrItemWhite();

            //----------------------------------------------------------------//

            crntIndex = (int)eIndex.PCLXLRGB;

            _palettes[crntIndex].addColour("Black", 0x00000000,
                                             (byte)eRGB_16.Black);
            _palettes[crntIndex].setClrItemBlack();
            _palettes[crntIndex].addColour("Blue", 0x000000ff,
                                             (byte)eRGB_16.Blue);
            _palettes[crntIndex].addColour("Cyan", 0x0000ffff,
                                             (byte)eRGB_16.Cyan);
            _palettes[crntIndex].addColour("Green", 0x0000ff00,
                                             (byte)eRGB_16.Green);
            _palettes[crntIndex].addColour("Magenta", 0x00ff00ff,
                                             (byte)eRGB_16.Magenta);
            _palettes[crntIndex].addColour("Red", 0x00ff0000,
                                             (byte)eRGB_16.Red);
            _palettes[crntIndex].addColour("White", 0x00ffffff,
                                             (byte)eRGB_16.White);
            _palettes[crntIndex].setClrItemWhite();
            _palettes[crntIndex].addColour("Yellow", 0x00ffff00,
                                             (byte)eRGB_16.Yellow);
            _palettes[crntIndex].addColour("Gray1", 0x00f0f0f0,
                                             (byte)eRGB_16.Gray1);
            _palettes[crntIndex].addColour("Gray2", 0x00d0d0d0,
                                             (byte)eRGB_16.Gray2);
            _palettes[crntIndex].addColour("Gray3", 0x00b0b0b0,
                                             (byte)eRGB_16.Gray3);
            _palettes[crntIndex].addColour("Gray4", 0x00909090,
                                             (byte)eRGB_16.Gray4);
            _palettes[crntIndex].addColour("Gray5", 0x00707070,
                                             (byte)eRGB_16.Gray5);
            _palettes[crntIndex].addColour("Gray6", 0x00505050,
                                             (byte)eRGB_16.Gray6);
            _palettes[crntIndex].addColour("Gray7", 0x00303030,
                                             (byte)eRGB_16.Gray7);
            _palettes[crntIndex].addColour("Gray8", 0x00101010,
                                             (byte)eRGB_16.Gray8);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o l o u r i d                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the colour identifier for the specified colour item in the     //
        // specified palette.                                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static byte getColourId(int paletteIndex,
                                        int colourIndex)
        {
            return _palettes[paletteIndex].getColourId(colourIndex);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o l o u r N a m e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the colour name for the specified palette index.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string getColourName(int paletteIndex,
                                            int colourIndex)
        {
            return _palettes[paletteIndex].getColourName(colourIndex);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o l o u r R G B                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the colour RGB value for the specified palette index.          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int getColourRGB(int paletteIndex,
                                          int colourIndex)
        {
            return _palettes[paletteIndex].getColourRGB(colourIndex);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C l r I t e m B l a c k                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the index of the black colour in the specified palette.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static byte getClrItemBlack(int paletteIndex)
        {
            return _palettes[paletteIndex].ClrItemBlack;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C l r I t e m W h i t e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the index of the white colour in the specified palette.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static byte getClrItemWhite(int paletteIndex)
        {
            return _palettes[paletteIndex].ClrItemWhite;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C t C l r I t e m s                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the count of colour items in the specified palette.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static byte getCtClrItems(int paletteIndex)
        {
            return _palettes[paletteIndex].CtClrItems;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t G r a y L e v e l                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the name of the gray level associated with the specified value //
        // in the specified palette.                                          //
        // This is only relevant to the Gray palette.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string getGrayLevel(int paletteIndex,
                                           byte level)
        {
            return _palettes[paletteIndex].getGrayLevel(level);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P a l e t t e N a m e                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the name for the specified palette index.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string getPaletteName(int paletteIndex)
        {
            return _palettes[paletteIndex].PaletteName;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i s M o n o c h r o m e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Is the selected palette monochrome?                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool isMonochrome(int paletteIndex)
        {
            return _palettes[paletteIndex].Monochrome;
        }
    }
}
