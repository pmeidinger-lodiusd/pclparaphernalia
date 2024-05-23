namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a Unicode block.</para>
    /// <para>© Chris Hutchinson 2017</para>
    ///
    /// </summary>
    class UnicodeBlock
    {
        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // U n i c o d e B l o c k                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public UnicodeBlock(uint rangeStart, uint rangeEnd, string name)
        {
            RangeStart = rangeStart;
            RangeEnd = rangeEnd;
            Name = name;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // N a m e                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Name { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // R a n g e E n d                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public uint RangeEnd { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // R a n g e S t a r t                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public uint RangeStart { get; }
    }
}