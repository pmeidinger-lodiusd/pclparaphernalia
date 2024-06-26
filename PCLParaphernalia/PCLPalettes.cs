﻿namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class provides definitions for simple colour palettes.</para>
    /// <para>© Chris Hutchinson 2014</para>
    ///
    /// </summary>
    public static class PCLPalettes
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum Index : byte
        {
            PCLMonochrome = 0,
            PCLSimpleColourCMY,
            PCLSimpleColourRGB
        }

        private enum SimpleMono : byte
        {
            White = 0,
            Black
        }

        private enum SimpleRGB : byte
        {
            Black = 0,
            Red,
            Green,
            Yellow,
            Blue,
            Magenta,
            Cyan,
            White
        }

        private enum SimpleCMY : byte
        {
            White = 0,
            Cyan,
            Magenta,
            Blue,
            Yellow,
            Green,
            Red,
            Black
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly PCLPalette[] _palettes =
        {
            new PCLPalette ("Monochrome", true, 1, 2),

            new PCLPalette ("Simple Colour CMY", false, -3, 8),

            new PCLPalette ("Simple Colour RGB", false, 3, 8)
        };

        //private static readonly int _paletteCount = _palettes.GetUpperBound(0) + 1;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L P a l e t t e s                                              //
        //                                                                    //
        // In each palette, the colours are added in alphabetic order, rather //
        // than the order of the colour indices.                              //
        // This is in order to to make it easier to keep the same list of     //
        // available colours in client interfaces when switching between the  //
        // different palettes.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PCLPalettes()
        {
            //----------------------------------------------------------------//

            int crntIndex = (int)Index.PCLMonochrome;

            _palettes[crntIndex].AddColour("Black", (byte)SimpleMono.Black);
            _palettes[crntIndex].SetClrItemBlack();
            _palettes[crntIndex].AddColour("White", (byte)SimpleMono.White);
            _palettes[crntIndex].SetClrItemWhite();

            //----------------------------------------------------------------//

            crntIndex = (int)Index.PCLSimpleColourCMY;

            _palettes[crntIndex].AddColour("Black", (byte)SimpleCMY.Black);
            _palettes[crntIndex].SetClrItemBlack();
            _palettes[crntIndex].AddColour("Blue", (byte)SimpleCMY.Blue);
            _palettes[crntIndex].AddColour("Cyan", (byte)SimpleCMY.Cyan);
            _palettes[crntIndex].AddColour("Green", (byte)SimpleCMY.Green);
            _palettes[crntIndex].AddColour("Magenta", (byte)SimpleCMY.Magenta);
            _palettes[crntIndex].AddColour("Red", (byte)SimpleCMY.Red);
            _palettes[crntIndex].AddColour("White", (byte)SimpleCMY.White);
            _palettes[crntIndex].SetClrItemWhite();
            _palettes[crntIndex].AddColour("Yellow", (byte)SimpleCMY.Yellow);

            //----------------------------------------------------------------//

            crntIndex = (int)Index.PCLSimpleColourRGB;

            _palettes[crntIndex].AddColour("Black", (byte)SimpleRGB.Black);
            _palettes[crntIndex].SetClrItemBlack();
            _palettes[crntIndex].AddColour("Blue", (byte)SimpleRGB.Blue);
            _palettes[crntIndex].AddColour("Cyan", (byte)SimpleRGB.Cyan);
            _palettes[crntIndex].AddColour("Green", (byte)SimpleRGB.Green);
            _palettes[crntIndex].AddColour("Magenta", (byte)SimpleRGB.Magenta);
            _palettes[crntIndex].AddColour("Red", (byte)SimpleRGB.Red);
            _palettes[crntIndex].AddColour("White", (byte)SimpleRGB.White);
            _palettes[crntIndex].SetClrItemWhite();
            _palettes[crntIndex].AddColour("Yellow", (byte)SimpleRGB.Yellow);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o l o u r I d                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the PCL id for the specified colour in the specified palette.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static byte GetColourId(int paletteIndex, int colourIndex) => _palettes[paletteIndex].GetColourId(colourIndex);

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o l o u r N a m e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the colour name for the specified palette index.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetColourName(int paletteIndex, int colourIndex) => _palettes[paletteIndex].GetColourName(colourIndex);

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C l r I t e m B l a c k                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the index of the black colour in the specified palette.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static byte GetClrItemBlack(int paletteIndex) => _palettes[paletteIndex].ClrItemBlack;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C l r I t e m W h i t e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the index of the white colour in the specified palette.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static byte GetClrItemWhite(int paletteIndex) => _palettes[paletteIndex].ClrItemWhite;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C t C l r I t e m s                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the count of colour items in the specified palette.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static byte GetCtClrItems(int paletteIndex) => _palettes[paletteIndex].CtClrItems;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P a l e t t e I d                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the PCL identifier for the specified (simple) palette index.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static short GetPaletteId(int paletteIndex) => _palettes[paletteIndex].PaletteId;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P a l e t t e N a m e                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Get the name for the specified palette index.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetPaletteName(int paletteIndex) => _palettes[paletteIndex].PaletteName;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i s M o n o c h r o m e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Is the selected palette monochrome?                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool IsMonochrome(int paletteIndex) => _palettes[paletteIndex].Monochrome;
    }
}