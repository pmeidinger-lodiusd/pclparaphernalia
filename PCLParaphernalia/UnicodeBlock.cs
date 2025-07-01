namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines a Unicode block.
    /// 
    /// © Chris Hutchinson 2017
    /// 
    /// </summary>

    class UnicodeBlock
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private uint _rangeStart;
        private uint _rangeEnd;
        private string _name;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // U n i c o d e B l o c k                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public UnicodeBlock(
            uint rangeStart,
            uint rangeEnd,
            string name)
        {
            _rangeStart = rangeStart;
            _rangeEnd = rangeEnd;
            _name = name;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // N a m e                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Name
        {
            get { return _name; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // R a n g e E n d                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public uint RangeEnd
        {
            get { return _rangeEnd; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // R a n g e S t a r t                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public uint RangeStart
        {
            get { return _rangeStart; }
        }
    }
}