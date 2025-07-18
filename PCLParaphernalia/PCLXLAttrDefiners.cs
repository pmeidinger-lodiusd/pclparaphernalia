using System.Collections.Generic;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides details of PCL XL Attribute Definer tags.
    /// 
    /// � Chris Hutchinson 2010
    /// 
    /// </summary>

    static class PCLXLAttrDefiners
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // PCLXL Attribute Definer tags.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum eTag : byte
        {
            Ubyte = 0xf8,
            Uint16 = 0xf9
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly SortedList<byte, PCLXLAttrDefiner> _tags =
            new SortedList<byte, PCLXLAttrDefiner>();

        private static int _tagCount;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L A t t r D e f i n e r s                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PCLXLAttrDefiners()
        {
            PopulateTable();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y  T a g s                                             //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display list of Attribute tags.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int DisplayTags(DataGrid grid,
                                        bool incResTags)
        {
            int count = 0;

            bool tagReserved;

            foreach (KeyValuePair<byte, PCLXLAttrDefiner> kvp in _tags)
            {
                tagReserved = kvp.Value.FlagReserved;

                if ((incResTags == true) ||
                    ((incResTags == false) && (!tagReserved)))
                {
                    count++;
                    grid.Items.Add(kvp.Value);
                }
            }

            return count;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p o p u l a t e T a b l e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Populate the table of Attribute Definer tags.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PopulateTable()
        {
            const bool flagNone = false;
            const bool flagReserved = true;

            byte tag;

            tag = (byte)eTag.Ubyte;                                 // 0xf8 //
            _tags.Add(tag,
                new PCLXLAttrDefiner(tag,
                                     flagNone,
                                     "ubyte"));

            tag = (byte)eTag.Uint16;                                // 0xf9 //
            _tags.Add(tag,
                new PCLXLAttrDefiner(tag,
                                     flagReserved,
                                     "uint16"));

            _tagCount = _tags.Count;
        }
    }
}
