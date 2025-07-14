namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines functions to get & set PCLXL element metrics.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    class PrnParsePCLXLElementMetrics
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        bool _flagUbyteAsAscii,
                _flagUint16AsUnicode,
                _flagArrayType;

        int _decodeIndent,
              _groupSize,
              _unitSize;

        PCLXLDataTypes.eBaseType _baseDataType;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P r n P a r s e P C L X L E l e m e n t M e t r i c s              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParsePCLXLElementMetrics(
            bool flagUbyteAsAscii,
            bool flagUint16AsUnicode,
            bool flagArrayType,
            int decodeIndent,
            int groupSize,
            int unitSize,
            PCLXLDataTypes.eBaseType baseDataType)
        {
            _flagUbyteAsAscii = flagUbyteAsAscii;
            _flagUint16AsUnicode = flagUint16AsUnicode;
            _flagArrayType = flagArrayType;
            _decodeIndent = decodeIndent;
            _groupSize = groupSize;
            _unitSize = unitSize;
            _baseDataType = baseDataType;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e c o d e I n d e n t                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int DecodeIndent
        {
            get { return _decodeIndent; }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D a t a                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetData(
            ref bool flagUbyteAsAscii,
            ref bool flagUint16AsUnicode,
            ref bool flagArrayType,
            ref int decodeIndent,
            ref int groupSize,
            ref int unitSize,
            ref PCLXLDataTypes.eBaseType baseDataType)
        {
            flagUbyteAsAscii = _flagUbyteAsAscii;
            flagUint16AsUnicode = _flagUint16AsUnicode;
            flagArrayType = _flagArrayType;
            decodeIndent = _decodeIndent;
            groupSize = _groupSize;
            unitSize = _unitSize;
            baseDataType = _baseDataType;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t D a t a                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetData(
            bool flagUbyteAsAscii,
            bool flagUint16AsUnicode,
            bool flagArrayType,
            int decodeIndent,
            int groupSize,
            int unitSize,
            PCLXLDataTypes.eBaseType baseDataType)
        {
            _flagUbyteAsAscii = flagUbyteAsAscii;
            _flagUint16AsUnicode = flagUint16AsUnicode;
            _flagArrayType = flagArrayType;
            _decodeIndent = decodeIndent;
            _groupSize = groupSize;
            _unitSize = unitSize;
            _baseDataType = baseDataType;
        }
    }
}