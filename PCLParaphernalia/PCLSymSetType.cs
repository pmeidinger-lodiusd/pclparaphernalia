namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles a PCL Symbol Set Type.
    /// 
    /// © Chris Hutchinson 2015
    /// 
    /// </summary>

    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(
        Feature = "renaming",
        ApplyToMembers = true)]

    class PCLSymSetType
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly string _descStd;
        private readonly string _descShort;

        private readonly byte _idPCL;

        private readonly bool _flagBound;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L S y m b o l S e t T y p e                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLSymSetType(byte idPCL,
                                 bool flagBound,
                                 string descStd,
                                 string descShort)
        {
            _idPCL = idPCL;
            _flagBound = flagBound;
            _descStd = descStd;
            _descShort = descShort;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c S h o r t                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the symbol set type short description.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string DescShort
        {
            get { return _descShort; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c S t d                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the symbol set type standard description.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string DescStd
        {
            get { return _descStd; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d P C L                                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL identifier string.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte IdPCL
        {
            get { return _idPCL; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I s B o u n d                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the flag indicating whether or not the symbol set type is   //
        // bound or unbound.                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsBound
        {
            get { return _flagBound; }
        }
    }
}