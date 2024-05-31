namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a set of PCL Paper Type objects.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class PCLPaperTypes
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        // Note that the length of the index array must be the same as that   //
        // of the definition array; the entries must be in the same order.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum EntryType
        {
            Standard,
            NotSet
        }

        public enum Index
        {
            NotSet,
            Plain,
            Preprinted,
            Letterhead,
            Transparency,
            Prepunched,
            Labels,
            Bond,
            Recycled,
            Color,
            Rough
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly PCLPaperType[] _paperTypes =
        {
            new PCLPaperType(EntryType.NotSet,  "<not set>"),
            new PCLPaperType(EntryType.Standard,"Plain"),
            new PCLPaperType(EntryType.Standard,"Preprinted"),
            new PCLPaperType(EntryType.Standard,"Letterhead"),
            new PCLPaperType(EntryType.Standard,"Transparency"),
            new PCLPaperType(EntryType.Standard,"Prepunched"),
            new PCLPaperType(EntryType.Standard,"Labels"),
            new PCLPaperType(EntryType.Standard,"Bond"),
            new PCLPaperType(EntryType.Standard,"Recycled"),
            new PCLPaperType(EntryType.Standard,"Color"),
            new PCLPaperType(EntryType.Standard,"Rough")
        };

        private static readonly int _paperTypeCount = _paperTypes.GetUpperBound(0) + 1;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o u n t                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return count of Paper Type definitions.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetCount()
        {
            return _paperTypeCount;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return name associated with specified PaperType index.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetName(int index)
        {
            return _paperTypes[index].GetName();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return type of entry.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static EntryType GetType(int index)
        {
            return _paperTypes[index].GetPaperType();
        }
    }
}