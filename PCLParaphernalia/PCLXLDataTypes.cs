using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class provides details of PCL XL Data Type tags.</para>
    /// <para>� Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class PCLXLDataTypes
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum BaseType : byte
        {
            Unknown = 0,
            Ubyte,
            Uint16,
            Uint32,
            Sint16,
            Sint32,
            Real32
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // PCLXL Data Type tags.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum Tag : byte
        {
            Real32 = 0xc5,
            Real32Array = 0xcd,
            Real32Box = 0xe5,
            Real32XY = 0xd5,
            Sint16 = 0xc3,
            Sint16Array = 0xcb,
            Sint16Box = 0xe3,
            Sint16XY = 0xd3,
            Sint32 = 0xc4,
            Sint32Array = 0xcc,
            Sint32Box = 0xe4,
            Sint32XY = 0xd4,
            Ubyte = 0xc0,
            UbyteArray = 0xc8,
            UbyteBox = 0xe0,
            UbyteXY = 0xd0,
            Uint16 = 0xc1,
            Uint16Array = 0xc9,
            Uint16Box = 0xe1,
            Uint16XY = 0xd1,
            Uint32 = 0xc2,
            Uint32Array = 0xca,
            Uint32Box = 0xe2,
            Uint32XY = 0xd2
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly SortedList<byte, PCLXLDataType> _tags = new SortedList<byte, PCLXLDataType>();

        private static PCLXLDataType _tagUnknown;

        private static int _tagCount;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L D a t a T y p e s                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PCLXLDataTypes()
        {
            PopulateTable();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k T a g                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Searches the PCL XL DataType tag table for a matching entry.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool CheckTag(byte tagToCheck,
                                        ref bool flagReserved,
                                        ref bool flagArray,
                                        ref int groupSize,
                                        ref int unitSize,
                                        ref BaseType baseType,
                                        ref string description)
        {
            bool seqKnown;

            PCLXLDataType tag;

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

            tag.GetDetails(ref flagReserved,
                            ref flagArray,
                            ref groupSize,
                            ref unitSize,
                            ref baseType,
                            ref description);

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

        public static void DisplayStatsCounts(DataTable table, bool incUsedSeqsOnly, bool excUnusedResTags)
        {
            bool displaySeq,
                    hddrWritten;

            DataRow row;

            hddrWritten = false;

            //----------------------------------------------------------------//

            displaySeq = true;

            int count = _tagUnknown.StatsCtTotal;
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
                row[1] = _tagUnknown.Description;
                row[2] = _tagUnknown.StatsCtParent;
                row[3] = _tagUnknown.StatsCtChild;
                row[4] = _tagUnknown.StatsCtTotal;

                table.Rows.Add(row);
            }

            //----------------------------------------------------------------//

            foreach (KeyValuePair<byte, PCLXLDataType> kvp in _tags)
            {
                displaySeq = true;

                count = kvp.Value.StatsCtTotal;

                if (count == 0)
                {
                    if (incUsedSeqsOnly)
                        displaySeq = false;
                    else if (excUnusedResTags && (kvp.Value.FlagReserved))
                        displaySeq = false;
                }

                if (displaySeq)
                {
                    if (!hddrWritten)
                    {
                        DisplayStatsCountsHddr(table);
                        hddrWritten = true;
                    }

                    row = table.NewRow();

                    row[0] = kvp.Value.Tag;
                    row[1] = kvp.Value.Description;
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
            //----------------------------------------------------------------//

            DataRow row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "_____________________";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "PCL XL DataType tags:";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "���������������������";
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
        // Display list of Data Type tags.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int DisplayTags(DataGrid grid, bool incResTags)
        {
            int count = 0;

            bool tagReserved;

            foreach (KeyValuePair<byte, PCLXLDataType> kvp in _tags)
            {
                tagReserved = kvp.Value.FlagReserved;

                if (incResTags ||
                    ((!incResTags) && (!tagReserved)))
                {
                    count++;
                    grid.Items.Add(kvp.Value);
                }
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

        public static void IncrementStatsCount(byte tagByte, int level)
        {
            PCLXLDataType tag;

            if (_tags.IndexOfKey(tagByte) != -1)
                tag = _tags[tagByte];
            else
                tag = _tagUnknown;

            tag.IncrementStatisticsCount(level);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p o p u l a t e T a b l e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Populate the table of Data Type tags.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PopulateTable()
        {
            const bool flagNone = false;
            const bool flagReserved = true;
            const bool flagArray = true;

            const int sizeSingle = 1;
            const int sizeDouble = 2;
            const int sizeQuad = 4;

            byte tag = 0x20;                                              // ?    //
            _tagUnknown =
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "*** Unknown tag ***");

            tag = (byte)Tag.Ubyte;                                 // 0xc0 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Ubyte,
                                  "ubyte"));

            tag = (byte)Tag.Uint16;                                // 0xc1 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeSingle, sizeDouble,
                                  BaseType.Uint16,
                                  "uint16"));

            tag = (byte)Tag.Uint32;                                // 0xc2 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeSingle, sizeQuad,
                                  BaseType.Uint32,
                                  "uint32"));

            tag = (byte)Tag.Sint16;                                // 0xc3 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeSingle, sizeDouble,
                                  BaseType.Sint16,
                                  "sint16"));

            tag = (byte)Tag.Sint32;                                // 0xc4 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeSingle, sizeQuad,
                                  BaseType.Sint32,
                                  "sint32"));

            tag = (byte)Tag.Real32;                                // 0xc5 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeSingle, sizeQuad,
                                  BaseType.Real32,
                                  "real32"));

            tag = 0xc6;                                               // 0xc6 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xc7;                                               // 0xc7 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = (byte)Tag.UbyteArray;                            // 0xc8 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagArray,
                                  sizeSingle, sizeSingle,
                                  BaseType.Ubyte,
                                  "ubyte_array"));

            tag = (byte)Tag.Uint16Array;                           // 0xc9 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagArray,
                                  sizeSingle, sizeDouble,
                                  BaseType.Uint16,
                                  "uint16_array"));

            tag = (byte)Tag.Uint32Array;                           // 0xca //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagArray,
                                  sizeSingle, sizeQuad,
                                  BaseType.Uint32,
                                  "uint32_array"));

            tag = (byte)Tag.Sint16Array;                           // 0xcb //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagArray,
                                  sizeSingle, sizeDouble,
                                  BaseType.Sint16,
                                  "sint16_array"));

            tag = (byte)Tag.Sint32Array;                           // 0xcc //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagArray,
                                  sizeSingle, sizeQuad,
                                  BaseType.Sint32,
                                  "sint32_array"));

            tag = (byte)Tag.Real32Array;                           // 0xcd //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagArray,
                                  sizeSingle, sizeQuad,
                                  BaseType.Real32,
                                  "real32_array"));

            tag = 0xce;                                               // 0xce //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xcf;                                               // 0xcf //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = (byte)Tag.UbyteXY;                               // 0xd0 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeDouble, sizeSingle,
                                  BaseType.Ubyte,
                                  "ubyte_xy"));

            tag = (byte)Tag.Uint16XY;                              // 0xd1 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeDouble, sizeDouble,
                                  BaseType.Uint16,
                                  "uint16_xy"));

            tag = (byte)Tag.Uint32XY;                              // 0xd2 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeDouble, sizeQuad,
                                  BaseType.Uint32,
                                  "uint32_xy"));

            tag = (byte)Tag.Sint16XY;                              // 0xd3 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeDouble, sizeDouble,
                                  BaseType.Sint16,
                                  "sint16_xy"));

            tag = (byte)Tag.Sint32XY;                              // 0xd4 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeDouble, sizeQuad,
                                  BaseType.Sint32,
                                  "sint32_xy"));

            tag = (byte)Tag.Real32XY;                              // 0xd5 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeDouble, sizeQuad,
                                  BaseType.Real32,
                                  "real32_xy"));

            tag = 0xd6;                                               // 0xd6 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xd7;                                               // 0xd7 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xd8;                                               // 0xd8 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xd9;                                               // 0xd9 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xda;                                               // 0xda //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xdb;                                               // 0xdb //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xdc;                                               // 0xdc //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xdd;                                               // 0xdd //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xde;                                               // 0xde //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xdf;                                               // 0xdf //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = (byte)Tag.UbyteBox;                              // 0xe0 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeQuad, sizeSingle,
                                  BaseType.Ubyte,
                                  "ubyte_box"));

            tag = (byte)Tag.Uint16Box;                             // 0xe1 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeQuad, sizeDouble,
                                  BaseType.Uint16,
                                  "uint16_box"));

            tag = (byte)Tag.Uint32Box;                             // 0xe2 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeQuad, sizeQuad,
                                  BaseType.Uint32,
                                  "uint32_box"));

            tag = (byte)Tag.Sint16Box;                             // 0xe3 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeQuad, sizeDouble,
                                  BaseType.Sint16,
                                  "sint16_box"));

            tag = (byte)Tag.Sint32Box;                             // 0xe4 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeQuad, sizeQuad,
                                  BaseType.Sint32,
                                  "sint32_box"));

            tag = (byte)Tag.Real32Box;                             // 0xe5 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagNone, flagNone,
                                  sizeQuad, sizeQuad,
                                  BaseType.Real32,
                                  "real32_box"));

            tag = 0xe6;                                               // 0xe6 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xe7;                                               // 0xe7 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xe8;                                               // 0xe8 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xe9;                                               // 0xe9 //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xea;                                               // 0xea //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xeb;                                               // 0xeb //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xec;                                               // 0xec //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xed;                                               // 0xed //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xee;                                               // 0xee //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

            tag = 0xef;                                               // 0xef //
            _tags.Add(tag,
                new PCLXLDataType(tag,
                                  flagReserved, flagNone,
                                  sizeSingle, sizeSingle,
                                  BaseType.Unknown,
                                  "* Reserved *"));

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
            PCLXLDataType tag;

            _tagUnknown.ResetStatistics();

            foreach (KeyValuePair<byte, PCLXLDataType> kvp in _tags)
            {
                tag = kvp.Value;

                tag.ResetStatistics();
            }
        }
    }
}