namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines a PCL XL Attribute Enumeration value.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(
        Feature = "renaming",
        ApplyToMembers = true)]

    class PCLXLAttrEnum
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly byte _operTag;
        private readonly byte _attrTagA;
        private readonly byte _attrTagB;

        private readonly int _value;
        private readonly int _attrTagLen;

        private readonly string _description;

        private readonly bool _flagValIsTxt;

        private int _statsCtParent;
        private int _statsCtChild;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L A t t r E n u m                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLXLAttrEnum(byte operTag,
                             byte attrTagA,
                             byte attrTagB,
                             int attrTagLen,
                             int value,
                             bool flagValIsTxt,
                             string description)
        {
            _operTag = operTag;
            _attrTagA = attrTagA;
            _attrTagB = attrTagB;
            _attrTagLen = attrTagLen;
            _value = value;
            _flagValIsTxt = flagValIsTxt;
            _description = description;

            _statsCtParent = 0;
            _statsCtChild = 0;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // A t t r i b u t e                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Attribute
        {
            get
            {
                return PCLXLAttributes.getDesc(_attrTagA,
                                               _attrTagB,
                                               _attrTagLen);
            }
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
        // F l a g V a l I s T x t                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagValIsTxt
        {
            get { return _flagValIsTxt; }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e t a i l s                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void getDetails(ref bool flagValIsTxt,
                                ref string description)
        {
            flagValIsTxt = _flagValIsTxt;
            description = _description;
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
        // O p e r a t o r                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Operator
        {
            get
            {
                if (_operTag == 0x00)
                    return "--";
                else
                    return PCLXLOperators.getDesc(_operTag);
            }
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
        // T y p e                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Type
        {
            get { return "Attribute Enumeration"; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // V a l u e                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Value
        {
            get
            {
                if (_flagValIsTxt)
                    return ("0x" + _value.ToString("X"));
                else
                    return _value.ToString();
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // V a l u e W i t h O p A n d A t t r                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string ValueWithOpAndAttr
        {
            get
            {
                string oper;
                string attr;
                string text;

                if (_operTag == 0x00)
                    oper = "----";
                else
                    oper = "0x" + _operTag.ToString("x2");

                if ((_attrTagLen == 1) && (_attrTagA == 0x00))
                {
                    text = oper + " " + "----  " + " " + "-";
                }
                else
                {
                    if (_attrTagLen == 1)
                        attr = "0x" + _attrTagA.ToString("x2") + "  ";
                    else
                        attr = "0x" + _attrTagA.ToString("x2") +
                                      _attrTagB.ToString("x2");

                    if (_value < 0x00ffff)
                        text = oper + " " +
                               attr + " " + _value.ToString();
                    else
                        text = oper + " " +
                                attr + " 0x" + _value.ToString("x8");
                }

                return text;
            }
        }
    }
}