﻿namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines a PCL Character Complement/Requirement Collection bit,
    /// as used in (unbound) Font headers and Symbol Set definitions.
    /// 
    /// © Chris Hutchinson 2013
    /// 
    /// </summary>

    class PCLCharCollection
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly PCLCharCollections.eBitType _bitType;
        private readonly int _bitNo;
        private readonly string _descMSL;
        private readonly string _descUnicode;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L C h a r C o l l e c t i o n                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLCharCollection(
            PCLCharCollections.eBitType bitType,
            int bitNo,
            string descMSL,
            string descUnicode)
        {
            _bitType = bitType;
            _bitNo = bitNo;
            _descMSL = descMSL;
            _descUnicode = descUnicode;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t B i t N o                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return bit number associated with specified collection item.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int GetBitNo()
        {
            return _bitNo;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t B i t T y p e                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return bit type associated with specified collection item.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLCharCollections.eBitType GetBitType()
        {
            return _bitType;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e s c M S L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return MSL description associated with specified collection item.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetDescMSL()
        {
            return _descMSL;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e s c U n i c o d e                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return Unicode description associated with specified collection    //
        // item.                                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetDescUnicode()
        {
            return _descUnicode;
        }
    }
}