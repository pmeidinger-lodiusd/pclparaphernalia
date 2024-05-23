namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles a PCL Symbol Set Type.</para>
    /// <para>© Chris Hutchinson 2015</para>
    ///
    /// </summary>
    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(Feature = "renaming", ApplyToMembers = true)]

    class PCLSymSetType
    {
        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L S y m b o l S e t T y p e                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLSymSetType(byte idPCL, bool flagBound, string descStd, string descShort)
        {
            IdPCL = idPCL;
            IsBound = flagBound;
            DescStd = descStd;
            DescShort = descShort;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c S h o r t                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the symbol set type short description.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string DescShort { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c S t d                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the symbol set type standard description.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string DescStd { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d P C L                                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL identifier string.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte IdPCL { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I s B o u n d                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the flag indicating whether or not the symbol set type is   //
        // bound or unbound.                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsBound { get; }
    }
}