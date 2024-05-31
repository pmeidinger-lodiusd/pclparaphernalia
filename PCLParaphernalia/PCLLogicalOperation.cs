namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles a PCL Logical Operation object.</para>
    /// <para>© Chris Hutchinson 2014</para>
    ///
    /// </summary>
    internal class PCLLogicalOperation
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const int _maxPFLen = 11;
        private readonly short _opId;
        private readonly short _opCode;

        private readonly string _actPostfix;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L L o g i c a l O p e r a t i o n                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLLogicalOperation(short opId, short opCode, string actPostfix, string actInfix)
        {
            _opId = opId;
            _opCode = opCode;       // not used at present //

            _actPostfix = actPostfix;
            ActInfix = actInfix;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // A c t I n f i x                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the Infix representation of the action associated with this //
        // operator.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string ActInfix { get; }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e s c L o n g                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the long form of the ROP description; this includes:        //
        //  - the ROP index                                                   //
        //  - the Postfix representation of the action                        //
        //  - the Infix representation of the action                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetDescLong()
        {
            string prefix;

            if (_opId < 10)
                prefix = "  ";
            else if (_opId < 100)
                prefix = " ";
            else
                prefix = string.Empty;

            return prefix + _opId.ToString() +
                   ": " + _actPostfix.PadRight(_maxPFLen) +
                   " = " + ActInfix;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e s c S h o r t                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the short form of the ROP description; this includes:       //
        //  - the ROP index                                                   //
        //  - the Postfix representation of the action                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetDescShort()
        {
            string prefix;

            if (_opId < 10)
                prefix = "  ";
            else if (_opId < 100)
                prefix = " ";
            else
                prefix = string.Empty;

            return prefix + _opId.ToString() +
                   ": " + _actPostfix;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t R O P I d                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the identifier of the ROP.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public short GetROPId()
        {
            return _opId;
        }
    }
}