using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides details of PCL XL Whitespace tags.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class PCLXLWhitespaces
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly SortedList<byte, PCLXLWhitespace> _tags =
            new SortedList<byte, PCLXLWhitespace>();

        private static PCLXLWhitespace _tagUnknown;

        private static int _tagCount;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L W h i t e s p a c e s                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PCLXLWhitespaces()
        {
            PopulateTable();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k T a g                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Searches the PCL XL Whitespace tag table for a matching entry.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool CheckTag(byte tagToCheck,
                                        ref string mnemonic,
                                        ref string description)
        {
            bool seqKnown;

            PCLXLWhitespace tag;

            if (_tags.IndexOfKey(tagToCheck) != -1)
            {
                seqKnown = true;
                tag = _tags[tagToCheck];
            }
            else
            {
                seqKnown = false;
                tag = _tagUnknown;
            }

            mnemonic = tag.Mnemonic;
            description = tag.Description;

            return seqKnown;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y S t a t s C o u n t s                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Add counts of referenced sequences to nominated data table.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DisplayStatsCounts(DataTable table,
                                               bool incUsedSeqsOnly,
                                               bool excUnusedResTags)
        {
            int count = 0;

            bool displaySeq,
                    hddrWritten;

            DataRow row;

            hddrWritten = false;

            //----------------------------------------------------------------//

            displaySeq = true;

            count = _tagUnknown.StatsCtTotal;

            if (count == 0)
                displaySeq = false;

            if (displaySeq)
            {
                if (!hddrWritten)
                {
                    DisplayStatsCountsHddr(table);
                    hddrWritten = true;
                }

                row = table.NewRow();

                row[0] = _tagUnknown.Tag;
                row[1] = _tagUnknown.Mnemonic + ": " + _tagUnknown.Description;
                row[2] = _tagUnknown.StatsCtParent;
                row[3] = _tagUnknown.StatsCtChild;
                row[4] = _tagUnknown.StatsCtTotal;

                table.Rows.Add(row);
            }

            //----------------------------------------------------------------//

            foreach (KeyValuePair<byte, PCLXLWhitespace> kvp in _tags)
            {
                displaySeq = true;

                count = kvp.Value.StatsCtTotal;

                if (count == 0 && incUsedSeqsOnly)
                    displaySeq = false;

                if (displaySeq)
                {
                    if (!hddrWritten)
                    {
                        DisplayStatsCountsHddr(table);
                        hddrWritten = true;
                    }

                    row = table.NewRow();

                    row[0] = kvp.Value.Tag;
                    row[1] = kvp.Value.Mnemonic + ": " + kvp.Value.Description;
                    row[2] = kvp.Value.StatsCtParent;
                    row[3] = kvp.Value.StatsCtChild;
                    row[4] = kvp.Value.StatsCtTotal;

                    table.Rows.Add(row);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y S t a t s C o u n t s H d d r                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Add statistics header lines to nominated data table.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DisplayStatsCountsHddr(DataTable table)
        {
            DataRow row;

            //----------------------------------------------------------------//

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "_______________________";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "PCL XL Whitespace tags:";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y T a g s                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display list of Whitespace tags.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int DisplayTags(DataGrid grid)
        {
            int count = 0;

            foreach (KeyValuePair<byte, PCLXLWhitespace> kvp in _tags)
            {
                count++;
                grid.Items.Add(kvp.Value);
            }

            return count;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n c r e m e n t S t a t s C o u n t                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Increment the relevant statistics count for the DataType tag.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void IncrementStatsCount(byte tagByte,
                                                int level)
        {
            PCLXLWhitespace tag;

            if (_tags.IndexOfKey(tagByte) != -1)
                tag = _tags[tagByte];
            else
                tag = _tagUnknown;

            tag.incrementStatisticsCount(level);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i s K n o w n T a g                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Searches the PCL XL Whitespace tag table for a matching entry.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool IsKnownTag(byte tagToCheck)
        {
            return _tags.IndexOfKey(tagToCheck) != -1;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p o p u l a t e T a b l e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Populate the table of Whitespace tags.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PopulateTable()
        {
            byte tag;

            tag = 0x20;                                              // ?    //
            _tagUnknown =
                new PCLXLWhitespace(tag,
                                     "??",
                                     "*** Unknown tag ***");

            tag = 0x00;                                               // 0x00 //
            _tags.Add(tag,
                new PCLXLWhitespace(tag,
                                     "<NUL>",
                                     "Null"));

            tag = 0x09;                                               // 0x09 //
            _tags.Add(tag,
                new PCLXLWhitespace(tag,
                                     "<HT>",
                                     "Horizontal Tab"));

            tag = 0x0a;                                               // 0x0a //
            _tags.Add(tag,
                new PCLXLWhitespace(tag,
                                     "<LF>",
                                     "Line Feed"));

            tag = 0x0b;                                               // 0x0b //
            _tags.Add(tag,
                new PCLXLWhitespace(tag,
                                     "<VT>",
                                     "Vertical Tab"));

            tag = 0x0c;                                               // 0x0c //
            _tags.Add(tag,
                new PCLXLWhitespace(tag,
                                     "<FF>",
                                     "Form Feed"));

            tag = 0x0d;                                               // 0x0d //
            _tags.Add(tag,
                new PCLXLWhitespace(tag,
                                     "<CR>",
                                     "Carriage Return"));

            tag = 0x20;                                               // 0x20 //
            _tags.Add(tag,
                new PCLXLWhitespace(tag,
                                     "<SP>",
                                     "Space"));

            _tagCount = _tags.Count;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        //  r e s e t S t a t s C o u n t s                                   //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset counts of referenced tags.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void ResetStatsCounts()
        {
            PCLXLWhitespace tag;

            _tagUnknown.ResetStatistics();

            foreach (KeyValuePair<byte, PCLXLWhitespace> kvp in _tags)
            {
                tag = kvp.Value;

                tag.ResetStatistics();
            }
        }
    }
}
