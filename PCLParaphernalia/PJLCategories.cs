﻿namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines a set of PJL 'status readback' Category objects.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class PJLCategories
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum eCategoryType
        {
            Custom,
            Std
        }

        private static readonly PJLCategory[] _categories =
        {
            new PJLCategory(eCategoryType.Custom,
                            "<specify value>"),
            new PJLCategory(eCategoryType.Std,
                            "ID"),
            new PJLCategory(eCategoryType.Std,
                            "CONFIG"),
            new PJLCategory(eCategoryType.Std,
                            "FILESYS"),
            new PJLCategory(eCategoryType.Std,
                            "LOG"),
            new PJLCategory(eCategoryType.Std,
                            "MEMORY"),
            new PJLCategory(eCategoryType.Std,
                            "PAGECOUNT"),
            new PJLCategory(eCategoryType.Std,
                            "PRODINFO"),
            new PJLCategory(eCategoryType.Std,
                            "STATUS"),
            new PJLCategory(eCategoryType.Std,
                            "SUPPLIES"),
            new PJLCategory(eCategoryType.Std,
                            "VARIABLES"),
            new PJLCategory(eCategoryType.Std,
                            "USTATUS")
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

        public static int GetCount()
        {
            return _categoryCount;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return name associated with specified command.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetName(int selection)
        {
            return _categories[selection].GetName();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return type of command.                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static eCategoryType GetType(int selection)
        {
            return _categories[selection].GetType();
        }
    }
}