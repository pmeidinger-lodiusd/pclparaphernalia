namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles a PCL Pattern Def object.</para>
    /// <para>© Chris Hutchinson 2016</para>
    ///
    /// </summary>
    internal class PCLPatternDef
    {
        private readonly PCLPatternDefs.Type _type;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L P a t t e r n D e f                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLPatternDef(string desc,
                              PCLPatternDefs.Type type,
                              ushort id,
                              ushort idSec,
                              ushort height,
                              ushort width,
                              byte[] pattern)
        {
            Desc = desc;
            _type = type;
            Id = id;
            IdSec = idSec;
            Height = height;
            Width = width;
            Pattern = pattern;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c                                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the pattern decription.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Desc { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // H e i g h t                                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the Height of the pattern definition.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort Height { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d                                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL identifier value for the pattern.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort Id { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d S e c                                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the secondary 'identifier' value for the pattern.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort IdSec { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P a t t e r n                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the bytes which define the pattern.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte[] Pattern { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // W i d t h                                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the Width of the pattern definition.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort Width { get; }
    }
}