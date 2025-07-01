namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles a PML 'data type' object.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(
        Feature = "renaming",
        ApplyToMembers = true)]

    class PMLDataType
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly byte _tag;
        private readonly string _description;

        private int _statsCtParent;
        private int _statsCtChild;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P M L D a t a T y p e                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PMLDataType(byte tag,
                           string description)
        {
            _tag = tag;
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
        //                                                        M e t h o d //
        // g e t D e s c                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the description.                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string getDesc()
        {
            return _description;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T a g                                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the tag.                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte getTag()
        {
            return _tag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n c r e m e n t S t a t i s t i c s C o u n t                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Increment 'statistics' count.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void incrementStatisticsCount(int level)
        {
            if (level == 0)
                _statsCtParent++;
            else
                _statsCtChild++;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S t a t s C t C h i l d                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int StatsCtChild
        {
            get { return _statsCtChild; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S t a t s C t P a r e n t                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int StatsCtParent
        {
            get { return _statsCtParent; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S t a t s C t T o t a l                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int StatsCtTotal
        {
            get { return (_statsCtParent + _statsCtChild); }
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
            get { return "Data Type"; }
        }
    }
}