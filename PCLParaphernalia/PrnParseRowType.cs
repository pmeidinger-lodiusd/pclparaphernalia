﻿using System;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles a PrnParse 'row type' object.</para>
    /// <para>© Chris Hutchinson 2017</para>
    ///
    /// </summary>
    class PrnParseRowType
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly PrnParseRowTypes.eType _rowType;
        private readonly string _rowTypeDesc;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P R N P a r s e R o w T y p e                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseRowType (PrnParseRowTypes.eType type,
                                string desc)
        {
            _rowType     = type;
            _rowTypeDesc = desc;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e s c                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the row type description.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string getDesc()
        {
            return _rowTypeDesc;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the entity type.                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseRowTypes.eType getType()
        {
            return _rowType;
        }
    }
}