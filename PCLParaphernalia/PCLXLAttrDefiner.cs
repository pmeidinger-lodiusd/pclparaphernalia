namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines a PCL XL Attribute Definer tag.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(
        Feature = "renaming",
        ApplyToMembers = true)]

    class PCLXLAttrDefiner
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly byte _tag;

        private readonly string _description;

        private readonly bool _flagReserved;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L A t t r D e f i n e r                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLXLAttrDefiner(byte tag,
                                bool flagReserved,
                                string description)
        {
            _tag = tag;
            _flagReserved = flagReserved;
            _description = description;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c r i p t i o n                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Description
        {
            get { return _description; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g R e s e r v e d                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagReserved
        {
            get { return _flagReserved; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T a g                                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Tag
        {
            get { return "0x" + _tag.ToString("x2"); }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T y p e                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Type
        {
            get { return "Attribute Definer"; }
        }
    }
}