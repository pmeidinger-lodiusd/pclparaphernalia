﻿using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines the sets of PCL control codes.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class PCLControlCodes
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly SortedList<byte, PCLControlCode> _tags =
            new SortedList<byte, PCLControlCode>();

        private static PCLControlCode _tagUnknown;

        private static int _tagsCount;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L C o n t r o l C o d e s                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PCLControlCodes()
        {
            PopulateTable();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k C o n t r o l C o d e                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Searches the PCL Control Codes table for an entry identified by    //
        // the specified value.                                               //
        //                                                                    //
        // If found, the description and option flags of the sequence are     //
        // returned.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool CheckControlCode(
            int macroLevel,
            byte tagVal,
            ref bool flagLineTerm,
            ref string mnemonic,
            ref PrnParseConstants.eOvlAct makeOvlAct,
            ref string description)
        {
            bool tagKnown;

            PCLControlCode tag;

            if (_tags.IndexOfKey(tagVal) != -1)
            {
                tagKnown = true;
                tag = _tags[tagVal];
            }
            else
            {
                tagKnown = false;
                tag = _tagUnknown;
            }

            flagLineTerm = tag.FlagLineTerm;
            mnemonic = tag.Mnemonic;
            description = tag.DescExcMnemonic;

            makeOvlAct = tag.MakeOvlAct;

            tag.IncrementStatisticsCount(macroLevel);   // Statistical data

            return tagKnown;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y S e q L i s t                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display list of sequences in nominated data grid.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int DisplaySeqList(DataGrid grid)
        {
            int count = 0;

            foreach (KeyValuePair<byte, PCLControlCode> kvp in _tags)
            {
                count++;
                grid.Items.Add(kvp.Value);
            }

            return count;
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
                                               bool incUsedSeqsOnly)
        {
            int count = 0;

            bool displaySeq,
                    hddrWritten;

            DataRow row;

            hddrWritten = false;

            //----------------------------------------------------------------//

            foreach (KeyValuePair<byte, PCLControlCode> kvp in _tags)
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

                    row[0] = kvp.Value.Sequence;
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
            DataRow row;

            //----------------------------------------------------------------//

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "__________________";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "PCL control codes:";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t S e q C o u n t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return count of sequences.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetTagCount()
        {
            return _tagsCount;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // P o p u l a t e T a b l e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Populate the table of control codes.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PopulateTable()
        {
            byte codeVal;

            codeVal = 0xff;                                       // 0xff:    //
            _tagUnknown =
                new PCLControlCode(codeVal, false,
                                   string.Empty,
                                   PrnParseConstants.eOvlAct.None,
                                   PrnParseConstants.eSeqGrp.Unknown,
                                   string.Empty);

            codeVal = 0x08;                                       // 0x08: BS //
            _tags.Add(codeVal,
                new PCLControlCode(codeVal, false,
                                   "<BS>",
                                   PrnParseConstants.eOvlAct.None,
                                   PrnParseConstants.eSeqGrp.CursorPositioning,
                                   "Backspace"));

            codeVal = 0x09;                                       // 0x09: HT //
            _tags.Add(codeVal,
                new PCLControlCode(codeVal, false,
                                   "<HT>",
                                   PrnParseConstants.eOvlAct.None,
                                   PrnParseConstants.eSeqGrp.CursorPositioning,
                                   "Horizontal Tab"));

            codeVal = 0x0a;                                       // 0x0a: LF //
            _tags.Add(codeVal,
                new PCLControlCode(codeVal, true,
                                   "<LF>",
                                   PrnParseConstants.eOvlAct.None,
                                   PrnParseConstants.eSeqGrp.CursorPositioning,
                                   "Line Feed"));

            codeVal = 0x0c;                                       // 0x0c: FF //
            _tags.Add(codeVal,
                new PCLControlCode(codeVal, false,
                                   "<FF>",
                                   PrnParseConstants.eOvlAct.PageChange,
                                   PrnParseConstants.eSeqGrp.CursorPositioning,
                                   "Form Feed"));

            codeVal = 0x0d;                                       // 0x0d: CR //
            _tags.Add(codeVal,
                new PCLControlCode(codeVal, true,
                                   "<CR>",
                                   PrnParseConstants.eOvlAct.None,
                                   PrnParseConstants.eSeqGrp.CursorPositioning,
                                   "Carriage Return"));

            codeVal = 0x0e;                                       // 0x0e: SO //
            _tags.Add(codeVal,
                new PCLControlCode(codeVal, false,
                                   "<SO>",
                                   PrnParseConstants.eOvlAct.None,
                                   PrnParseConstants.eSeqGrp.FontSelection,
                                   "Shift Out - select Secondary font"));

            codeVal = 0x0f;                                       // 0x0f: SI //
            _tags.Add(codeVal,
                new PCLControlCode(codeVal, false,
                                   "<SI>",
                                   PrnParseConstants.eOvlAct.None,
                                   PrnParseConstants.eSeqGrp.FontSelection,
                                   "Shift In - select Primary font"));

            /*
            // Escape is a control code in the sense that it is the           //
            // introductory character for simple and complex PCL escape       //
            // sequences.                                                     //
            // Don't include it here, as (with the current mechanism) it will //
            // always register a zero hit count in the statistics.            //
 
            codeVal = 0x1b;                                       // 0x1b: Esc//
            _tags.Add(codeVal,
                new PCLControlCode(codeVal, false,
                                   "<Esc>",
                                   PrnParseConstants.eOvlAct.None,
                                   "Escape"));
            */

            _tagsCount = _tags.Count;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        //  r e s e t S t a t s C o u n t s                                   //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset counts of referenced codes.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void ResetStatsCounts()
        {
            PCLControlCode tag;

            foreach (KeyValuePair<byte, PCLControlCode> kvp in _tags)
            {
                tag = kvp.Value;

                tag.ResetStatistics();
            }
        }
    }
}
