﻿namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a set of PCL Raster Resolution objects.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class PCLRasterResolutions
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly PCLRasterResolution[] _rasterResolutions =
        {
            new PCLRasterResolution(75),
            new PCLRasterResolution(100),
            new PCLRasterResolution(150),
            new PCLRasterResolution(200),
            new PCLRasterResolution(300),
            new PCLRasterResolution(600)
        };

        private static readonly int _rasterResolutionCount = _rasterResolutions.GetUpperBound(0) + 1;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o u n t                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return count of Raster Resolution definitions.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetCount() => _rasterResolutionCount;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t V a l u e                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return value associated with specified Raster Resolution index.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static ushort GetValue(int selection) => _rasterResolutions[selection].GetValue();
    }
}