namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles a PJL 'status readback' Category object.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    class PJLCategory
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly PJLCategories.eCategoryType _categoryType;
        private readonly string _categoryName;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P J L C a t e g o r y                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PJLCategory(PJLCategories.eCategoryType type,
                           string name)
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

        public string getName()
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

        public PJLCategories.eCategoryType getType()
        {
            return _categoryType;
        }
    }
}