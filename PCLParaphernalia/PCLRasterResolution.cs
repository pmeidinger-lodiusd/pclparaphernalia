namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles a PCL Raster Resolution object.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    class PCLRasterResolution
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly ushort _resolution;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L R a s t e r R e s o l u t i o n                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLRasterResolution(ushort value)
        {
            _resolution = value;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t V a l u e                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the resolution value.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort GetValue()
        {
            return _resolution;
        }
    }
}