using System;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a PCL Simple Escape Sequence.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(
        Feature = "renaming",
        ApplyToMembers = true)]

    class PCLComplexSeq
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly byte _keyPChar;
        private readonly byte _keyGChar;
        private readonly byte _keyTChar;

        private readonly int _value;
        private int _statsCtParent;
        private int _statsCtChild;

        private readonly PrnParseConstants.eSeqGrp _seqGrp;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L C o m p l e x S e q                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLComplexSeq(
            byte keyPChar,
            byte keyGChar,
            byte keyTChar,
            int value,
            bool flagDiscrete,
            bool flagNilGChar,
            bool flagNilValue,
            bool flagValIsLen,
            bool flagObsolete,
            bool flagResetGL2,
            bool flagDisplayHexVal,
            PrnParseConstants.eActPCL actionType,
            PrnParseConstants.eOvlAct makeOvlAct,
            PrnParseConstants.eSeqGrp seqGrp,
            string description)
        {
            _keyPChar = keyPChar;
            _keyGChar = keyGChar;
            _keyTChar = keyTChar;

            _value = value;
            ActionType = actionType;

            Description = description;

            FlagDiscrete = flagDiscrete;
            FlagNilGChar = flagNilGChar;
            FlagNilValue = flagNilValue;
            FlagValIsLen = flagValIsLen;
            FlagObsolete = flagObsolete;
            FlagResetGL2 = flagResetGL2;

            FlagDisplayHexVal = flagDisplayHexVal;

            this.makeOvlAct = makeOvlAct;
            _seqGrp = seqGrp;

            FlagValGeneric = value == PCLComplexSeqs._valueGeneric;

            FlagValVarious = value == PCLComplexSeqs._valueVarious;

            _statsCtParent = 0;
            _statsCtChild = 0;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // A c t i o n T y p e                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseConstants.eActPCL ActionType { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c r i p t i o n                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Description { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g D i s c r e t e                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagDiscrete { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g D i s p l a y H e x V a l                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagDisplayHexVal { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g O b s o l e t e                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagObsolete { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g N i l G C h a r                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagNilGChar { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g N i l V a l u e                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagNilValue { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g R e s e t G L 2                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagResetGL2 { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g V a l G e n e r i c                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagValGeneric { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g V a l I s L e n                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagValIsLen { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g V a l V a r i o u s                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagValVarious { get; }

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
        // M a k e O v e r l a y A c t i o n                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseConstants.eOvlAct makeOvlAct { get; }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s e t S t a t i s t i c s                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset 'statistics' counts.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void resetStatistics()
        {
            _statsCtParent = 0;
            _statsCtChild = 0;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S e q u e n c e                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Sequence
        {
            get
            {
                string seq;
                string value;

                if (FlagDiscrete && (!FlagValGeneric) && (!FlagValVarious))
                    value = " (#=" + _value.ToString() + ")";
                else
                    value = string.Empty;
                if (FlagNilValue)
                {
                    if (FlagNilGChar)
                        seq = "<Esc>" + (char)_keyPChar +
                                        (char)_keyTChar;
                    else
                        seq = "<Esc>" + (char)_keyPChar +
                                        (char)_keyGChar +
                                        (char)_keyTChar;
                }
                else if (FlagNilGChar)
                {
                    seq = "<Esc>" + (char)_keyPChar + "#" +
                                    (char)_keyTChar +
                                    value;
                }
                else
                {
                    seq = "<Esc>" + (char)_keyPChar +
                                    (char)_keyGChar + "#" +
                                    (char)_keyTChar +
                                    value;
                }

                return seq;
            }
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
            get { return _statsCtParent + _statsCtChild; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T y p e                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Type
        {
            get { return "Complex"; }
        }
    }
}