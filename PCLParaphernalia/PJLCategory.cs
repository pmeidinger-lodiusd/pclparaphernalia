namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles a PJL 'status readback' Category object.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal class PJLCategory
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly PJLCategories.CategoryType _categoryType;
        private readonly string _categoryName;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P J L C a t e g o r y                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PJLCategory(PJLCategories.CategoryType type, string name)
        {
            _categoryType = type;
            _categoryName = name;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the category name.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetName()
        {
            return _categoryName;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the category type.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PJLCategories.CategoryType GetCategoryType()
        {
            return _categoryType;
        }
    }
}