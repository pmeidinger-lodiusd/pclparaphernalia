using System;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles a PCL Paper Type object.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    class PCLPaperType
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly PCLPaperTypes.EntryType _entryType;

        private readonly string _paperTypeName;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L P a p e r T y p e                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLPaperType(PCLPaperTypes.EntryType entryType,
                            string name)
        {
            _entryType     = entryType;
            _paperTypeName = name;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the paper type name.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string getName()
        {
            return _paperTypeName;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the entry type.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLPaperTypes.EntryType getType()
        {
            return _entryType;
        }
    }
}