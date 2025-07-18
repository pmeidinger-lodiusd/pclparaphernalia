﻿using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines the sets of PCL escape sequences.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class PCLSimpleSeqs
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly SortedList<byte, PCLSimpleSeq> _seqs =
            new SortedList<byte, PCLSimpleSeq>();

        private static PCLSimpleSeq _seqUnknown;

        private static int _seqsCount;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L S i m p l e S e q s                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PCLSimpleSeqs()
        {
            PopulateTable();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k S i m p l e S e q                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Searches the PCL Simple sequence table for an entry identified by  //
        // the I_char value.                                                  //
        //                                                                    //
        // If found, the description and option flags of the sequence are     //
        // returned.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool CheckSimpleSeq(
            int macroLevel,
            byte iChar,
            ref bool optObsolete,
            ref bool optResetHPGL2,
            ref PrnParseConstants.eOvlAct makeOvlAct,
            ref string description)
        {
            bool seqKnown;

            PCLSimpleSeq seq;

            if (_seqs.IndexOfKey(iChar) != -1)
            {
                seqKnown = true;
                seq = _seqs[iChar];
            }
            else
            {
                seqKnown = false;
                seq = _seqUnknown;
            }

            optObsolete = seq.FlagObsolete;
            optResetHPGL2 = seq.FlagResetHPGL2;
            description = seq.Description;
            makeOvlAct = seq.MakeOvlAct;

            seq.IncrementStatisticsCount(macroLevel);   // Statistical data

            return seqKnown;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y S e q L i s t                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display list of sequences in nominated data grid.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int DisplaySeqList(DataGrid grid,
                                           bool incObsSeqs)
        {
            int count = 0;

            bool seqObsolete;

            foreach (KeyValuePair<byte, PCLSimpleSeq> kvp in _seqs)
            {
                seqObsolete = kvp.Value.FlagObsolete;

                if ((incObsSeqs == true) ||
                    ((incObsSeqs == false) && (!seqObsolete)))
                {
                    count++;
                    grid.Items.Add(kvp.Value);
                }
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
                                               bool incUsedSeqsOnly,
                                               bool excUnusedObsSeqs)
        {
            int count = 0;

            bool displaySeq,
                    hddrWritten;

            DataRow row;

            hddrWritten = false;

            //----------------------------------------------------------------//

            displaySeq = true;

            count = _seqUnknown.StatsCtTotal;

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

                row[0] = _seqUnknown.Sequence;
                row[1] = _seqUnknown.Description;
                row[2] = _seqUnknown.StatsCtParent;
                row[3] = _seqUnknown.StatsCtChild;
                row[4] = _seqUnknown.StatsCtTotal;

                table.Rows.Add(row);
            }

            //----------------------------------------------------------------//

            foreach (KeyValuePair<byte, PCLSimpleSeq> kvp in _seqs)
            {
                displaySeq = true;

                count = kvp.Value.StatsCtTotal;

                if (count == 0)
                {
                    if (incUsedSeqsOnly)
                        displaySeq = false;
                    else if ((excUnusedObsSeqs) &&
                             (kvp.Value.FlagObsolete == true))
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
            row[1] = "_____________________";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "PCL simple sequences:";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
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

        public static int GetSeqCount()
        {
            return _seqsCount;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p o p u l a t e T a b l e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Populate the table of simple PCL sequences.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PopulateTable()
        {
            const bool flagNone = false;
            const bool flagObsolete = true;
            const bool flagResetGL2 = true;

            byte sChar;

            sChar = 0x20;                                                // ? //
            _seqUnknown =
               new PCLSimpleSeq(sChar,
                                flagNone, flagNone,
                                PrnParseConstants.eOvlAct.None,
                                PrnParseConstants.eSeqGrp.Unknown,
                                "*** Unknown sequence ***");

            sChar = 0x31;                                                // 1 //
            _seqs.Add(sChar,
                 new PCLSimpleSeq(sChar,
                                  flagObsolete, flagNone,
                                 PrnParseConstants.eOvlAct.None,
                                 PrnParseConstants.eSeqGrp.PageControl,
                                  "Set Horizontal Tab At Current Column"));

            sChar = 0x32;                                                // 2 //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagObsolete, flagNone,
                                 PrnParseConstants.eOvlAct.None,
                                 PrnParseConstants.eSeqGrp.PageControl,
                                 "Clear Horizontal Tab At Current Column"));

            sChar = 0x33;                                                // 3 //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagObsolete, flagNone,
                                 PrnParseConstants.eOvlAct.None,
                                 PrnParseConstants.eSeqGrp.PageControl,
                                 "Clear All Horizontal Tabs"));

            sChar = 0x34;                                                // 4 //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagObsolete, flagNone,
                                 PrnParseConstants.eOvlAct.None,
                                 PrnParseConstants.eSeqGrp.PageControl,
                                 "Set Left Margin At Current Position"));

            sChar = 0x35;                                                // 5 //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagObsolete, flagNone,
                                 PrnParseConstants.eOvlAct.None,
                                 PrnParseConstants.eSeqGrp.PageControl,
                                 "Set Right Margin At Current Position"));

            sChar = 0x39;                                                // 9 //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagNone, flagNone,
                                 PrnParseConstants.eOvlAct.None,
                                 PrnParseConstants.eSeqGrp.PageControl,
                                 "Clear Horizontal Margins"));

            sChar = 0x3d;                                                // = //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagObsolete, flagNone,
                                 PrnParseConstants.eOvlAct.None,
                                 PrnParseConstants.eSeqGrp.CursorPositioning,
                                 "Half Line Feed"));

            sChar = 0x3f;                                                // ? //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagObsolete, flagNone,
                                 PrnParseConstants.eOvlAct.None,
                                 PrnParseConstants.eSeqGrp.StatusReadback,
                                 "I/O Status Request"));

            sChar = 0x45;                                                // E //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagNone, flagResetGL2,
                                 PrnParseConstants.eOvlAct.Reset,
                                 PrnParseConstants.eSeqGrp.JobControl,
                                 "Printer Reset"));

            sChar = 0x49;                                                // I //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagObsolete, flagNone,
                                 PrnParseConstants.eOvlAct.None,
                                 PrnParseConstants.eSeqGrp.PageControl,
                                 "Horizontal Tabulation"));

            sChar = 0x59;                                                // Y //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagNone, flagNone,
                                 PrnParseConstants.eOvlAct.Remove,
                                 PrnParseConstants.eSeqGrp.JobControl,
                                 "Display Functions - Enable"));

            sChar = 0x5a;                                                // Z //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagNone, flagNone,
                                 PrnParseConstants.eOvlAct.Remove,
                                 PrnParseConstants.eSeqGrp.JobControl,
                                 "Display Functions - Disable"));

            sChar = 0x5e;                                                // ^ //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagObsolete, flagNone,
                                 PrnParseConstants.eOvlAct.None,
                                 PrnParseConstants.eSeqGrp.StatusReadback,
                                 "Primary Status Request"));

            sChar = 0x6e;                                                // n //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagObsolete, flagNone,
                                 PrnParseConstants.eOvlAct.None,
                                 PrnParseConstants.eSeqGrp.JobControl,
                                 "Transfer To On-Line State"));

            sChar = 0x6f;                                                // o //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagObsolete, flagNone,
                                 PrnParseConstants.eOvlAct.None,
                                 PrnParseConstants.eSeqGrp.JobControl,
                                 "Transfer To Off-Line State"));

            sChar = 0x7a;                                                // z //
            _seqs.Add(sChar,
                new PCLSimpleSeq(sChar,
                                 flagNone, flagNone,
                                 PrnParseConstants.eOvlAct.Remove,
                                 PrnParseConstants.eSeqGrp.JobControl,
                                 "Self Test"));

            _seqsCount = _seqs.Count;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        //  r e s e t S t a t s C o u n t s                                   //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset counts of referenced sequences.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void ResetStatsCounts()
        {
            PCLSimpleSeq seq;

            _seqUnknown.ResetStatistics();

            foreach (KeyValuePair<byte, PCLSimpleSeq> kvp in _seqs)
            {
                seq = kvp.Value;

                seq.ResetStatistics();
            }
        }
    }
}
