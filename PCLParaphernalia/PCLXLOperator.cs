namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines a PCL XL Operator tag.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(
        Feature = "renaming",
        ApplyToMembers = true)]

    class PCLXLOperator
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly byte _tag;

        private readonly string _description;

        private readonly bool _flagReserved;
        private readonly bool _flagEndSession;

        private readonly PCLXLOperators.eEmbedDataType _embedDataType;

        private readonly PrnParseConstants.eOvlAct _makeOvlAct;

        private int _statsCtParent;
        private int _statsCtChild;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L O p e r a t o r                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLXLOperator(byte tag,
                             bool flagEndSession,
                             bool flagReserved,
                             PCLXLOperators.eEmbedDataType embedDataType,
                             PrnParseConstants.eOvlAct makeOvlAct,
                             string description)
        {
            _tag = tag;
            _flagEndSession = flagEndSession;
            _flagReserved = flagReserved;
            _embedDataType = embedDataType;
            _description = description;
            _makeOvlAct = makeOvlAct;

            _statsCtParent = 0;
            _statsCtChild = 0;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e t a i l s                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void getDetails(
            ref bool flagEndSession,
            ref bool flagReserved,
            ref PCLXLOperators.eEmbedDataType embedDataType,
            ref PrnParseConstants.eOvlAct makeOvlAct,
            ref string description)
        {
            flagEndSession = _flagEndSession;
            flagReserved = _flagReserved;
            embedDataType = _embedDataType;
            makeOvlAct = _makeOvlAct;
            description = _description;
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
        // E m b e d D a t a T y p e                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLXLOperators.eEmbedDataType EmbedDataType
        {
            get { return _embedDataType; }
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
        // F l a g E n d S e s s i o n                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagEndSession
        {
            get { return _flagEndSession; }
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
        // M a k e O v e r l a y A c t i o n                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseConstants.eOvlAct makeOvlAct
        {
            get { return _makeOvlAct; }
        }

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
            get { return "Operator"; }
        }
    }
}