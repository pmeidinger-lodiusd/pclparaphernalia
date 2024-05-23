namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a PCL XL Attribute tag.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(Feature = "renaming", ApplyToMembers = true)]

    class PCLXLAttribute
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly int _tagLen;
        private readonly byte _tagA;
        private readonly byte _tagB;
        private readonly bool _flagUbyteTxt;
        private readonly bool _flagUintTxt;
        private readonly bool _flagValIsLen;
        private readonly bool _flagValIsPCL;

        private readonly PrnParseConstants.ActPCLXL _actionType;

        private int _statsCtParent;
        private int _statsCtChild;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L A t t r i b u t e                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLXLAttribute(int tagLen,
                              byte tagA,
                              byte tagB,
                              bool flagReserved,
                              bool flagAttrEnum,
                              bool flagOperEnum,
                              bool flagUbyteTxt,
                              bool flagUintTxt,
                              bool flagValIsLen,
                              bool flagValIsPCL,
                              PrnParseConstants.ActPCLXL actionType,
                              PrnParseConstants.OvlAct makeOvlAct,
                              string description)
        {
            _tagLen = tagLen;
            _tagA = tagA;
            _tagB = tagB;
            FlagReserved = flagReserved;
            FlagAttrEnum = flagAttrEnum;
            FlagOperEnum = flagOperEnum;
            _flagUbyteTxt = flagUbyteTxt;
            _flagUintTxt = flagUintTxt;
            _flagValIsLen = flagValIsLen;
            _flagValIsPCL = flagValIsPCL;
            _actionType = actionType;
            this.makeOvlAct = makeOvlAct;
            Description = description;

            _statsCtParent = 0;
            _statsCtChild = 0;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e t a i l s                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetDetails(
            ref bool flagReserved,
            ref bool flagAttrEnum,
            ref bool flagOperEnum,
            ref bool flagUbyteTxt,
            ref bool flagUintTxt,
            ref bool flagValIsLen,
            ref bool flagValIsPCL,
            ref PrnParseConstants.ActPCLXL actionType,
            ref PrnParseConstants.OvlAct makeOvlAct,
            ref string description)
        {
            flagReserved = FlagReserved;
            flagAttrEnum = FlagAttrEnum;
            flagOperEnum = FlagOperEnum;
            flagUbyteTxt = _flagUbyteTxt;
            flagUintTxt = _flagUintTxt;
            flagValIsLen = _flagValIsLen;
            flagValIsPCL = _flagValIsPCL;
            makeOvlAct = this.makeOvlAct;
            description = Description;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c r i p t i o n                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Description { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g A t t r E n u m                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagAttrEnum { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g O p e r E n u m                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagOperEnum { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g R e s e r v e d                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagReserved { get; }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n c r e m e n t S t a t i s t i c s C o u n t                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Increment 'statistics' count.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void IncrementStatisticsCount(int level)
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

        public PrnParseConstants.OvlAct makeOvlAct { get; }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s e t S t a t i s t i c s                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset 'statistics' counts.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void ResetStatistics()
        {
            _statsCtParent = 0;
            _statsCtChild = 0;
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
        // T a g                                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Tag
        {
            get
            {
                if (_tagLen == 1)
                    return "0x" + _tagA.ToString("x2");
                else
                    return "0x" + _tagA.ToString("x2") + _tagB.ToString("x2");
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T y p e                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Type
        {
            get { return "Attribute"; }
        }
    }
}