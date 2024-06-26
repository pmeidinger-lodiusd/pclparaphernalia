﻿namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a PCL XL Data Type tag.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    internal class PCLXLDataType
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly byte _tag;
        private readonly bool _flagArray;

        private readonly int _groupSize;
        private readonly int _unitSize;

        private int _statsCtParent;
        private int _statsCtChild;

        private readonly PCLXLDataTypes.BaseType _baseType;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L D a t a T y p e                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLXLDataType(byte tag,
                             bool flagReserved,
                             bool flagArray,
                             int groupSize,
                             int unitSize,
                             PCLXLDataTypes.BaseType baseType,
                             string description)
        {
            _tag = tag;
            FlagReserved = flagReserved;
            _flagArray = flagArray;
            _groupSize = groupSize;
            _unitSize = unitSize;
            _baseType = baseType;
            Description = description;

            _statsCtParent = 0;
            _statsCtChild = 0;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e t a i l s                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetDetails(ref bool flagReserved,
                                ref bool flagArray,
                                ref int groupSize,
                                ref int unitSize,
                                ref PCLXLDataTypes.BaseType baseType,
                                ref string description)
        {
            flagReserved = FlagReserved;
            flagArray = _flagArray;
            groupSize = _groupSize;
            unitSize = _unitSize;
            baseType = _baseType;
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

        public int StatsCtChild => _statsCtChild;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S t a t s C t P a r e n t                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int StatsCtParent => _statsCtParent;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S t a t s C t T o t a l                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int StatsCtTotal => _statsCtParent + _statsCtChild;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T a g                                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Tag => "0x" + _tag.ToString("x2");

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T y p e                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Type => "Data Type";
    }
}