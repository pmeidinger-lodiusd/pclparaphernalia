namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a set of PJL 'status readback' Category objects.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class PJLCategories
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum CategoryType
        {
            Custom,
            Std
        }

        private static readonly PJLCategory[] _categories =
        {
            new PJLCategory(CategoryType.Custom, "<specify value>"),
            new PJLCategory(CategoryType.Std, "ID"),
            new PJLCategory(CategoryType.Std, "CONFIG"),
            new PJLCategory(CategoryType.Std, "FILESYS"),
            new PJLCategory(CategoryType.Std, "LOG"),
            new PJLCategory(CategoryType.Std, "MEMORY"),
            new PJLCategory(CategoryType.Std, "PAGECOUNT"),
            new PJLCategory(CategoryType.Std, "PRODINFO"),
            new PJLCategory(CategoryType.Std, "STATUS"),
            new PJLCategory(CategoryType.Std, "SUPPLIES"),
            new PJLCategory(CategoryType.Std, "VARIABLES"),
            new PJLCategory(CategoryType.Std, "USTATUS")
        };

        private static readonly int _categoryCount = _categories.GetUpperBound(0) + 1;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o u n t                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return count of Category definitions.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetCount() => _categoryCount;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return name associated with specified command.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetName(int selection) => _categories[selection].GetName();

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return type of command.                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static CategoryType GetType(int selection) => _categories[selection].GetCategoryType();
    }
}