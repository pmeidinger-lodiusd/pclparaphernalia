using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines the sets of PCL escape sequences.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class PCLComplexSeqs
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public const int _valueVarious = 99999;
        public const int _valueGeneric = 99998;
        public const int _valueDummy = 99997;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly SortedList<string, PCLComplexSeq> _seqs = new SortedList<string, PCLComplexSeq>();

        private static PCLComplexSeq _seqUnknown;

        private static int _seqsCount;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L C o m p l e x S e q s                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PCLComplexSeqs()
        {
            PopulateTable();
        }

        //
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k C o m p l e x S e q                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Searches the PCL Complex sequence table for an entry identified by //
        // the I_char, G_char & T_char values, and (for certain 'flag' type   //
        // sequences) the (integer) value field.                              //
        //                                                                    //
        // If found, the description and option flags of the sequence are     //
        // returned, otherwise details of the 'unknown' entry are returned.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool CheckComplexSeq(
            int macroLevel,
            byte iChar,
            byte gChar,
            byte tChar,
            bool vCheck,
            int vInt,
            ref bool optObsolete,
            ref bool optResetGL2,
            ref bool optNilGChar,
            ref bool optNilValue,
            ref bool optValIsLen,
            ref bool optDisplayHexVal,
            ref PrnParseConstants.ActPCL actType,
            ref PrnParseConstants.OvlAct makeOvlAct,
            ref string description)
        {
            bool seqKnown;
            bool flagDiscrete;
            bool flagValGeneric;

            PCLComplexSeq seq;

            string keyRoot,
                   key;

            keyRoot = iChar.ToString("X2") +
                      gChar.ToString("X2") +
                      tChar.ToString("X2");

            key = keyRoot + ":" + vInt.ToString("X4");

            if (vCheck && (_seqs.IndexOfKey(key) != -1))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Value associated with sequence is integer only (no         //
                // fractional part) and hence may be significant for          //
                // sequences with discrete values.                            //
                //                                                            //
                //------------------------------------------------------------//

                seqKnown = true;
                seq = _seqs[key];
            }
            else if (_seqs.IndexOfKey(keyRoot) != -1)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Either the value associated with the sequence is not a     //
                // pure integer, or a sequence with that discrete value has   //
                // not been found in the table.                               //
                //                                                            //
                //------------------------------------------------------------//

                seqKnown = true;
                seq = _seqs[keyRoot];
            }
            else
            {
                seqKnown = false;
                seq = _seqUnknown;
            }

            optObsolete = seq.FlagObsolete;
            optResetGL2 = seq.FlagResetGL2;
            optNilGChar = seq.FlagNilGChar;
            optNilValue = seq.FlagNilValue;
            optValIsLen = seq.FlagValIsLen;
            optDisplayHexVal = seq.FlagDisplayHexVal;

            flagDiscrete = seq.FlagDiscrete;
            flagValGeneric = seq.FlagValGeneric;

            actType = seq.ActionType;
            makeOvlAct = seq.MakeOvlAct;

            description = seq.Description;

            if (flagDiscrete && flagValGeneric)
            {
                int ptr = description.IndexOf("discrete");

                if (ptr != -1)
                {
                    string desc = seq.Description;

                    description = desc.Substring(0, ptr) +
                                  "unknown/illegal" +
                                  description.Substring(ptr + 8);
                }
            }

            if ((actType == PrnParseConstants.ActPCL.MacroStop) && (macroLevel > 0))
                seq.IncrementStatisticsCount(macroLevel - 1);
            else
                seq.IncrementStatisticsCount(macroLevel);

            return seqKnown;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y S e q L i s t                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Add list of sequences to nominated data grid.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int DisplaySeqList(DataGrid grid, bool incObsoleteSeqs, bool incDiscreteVal)
        {
            int count = 0;

            bool seqDiscrete;
            bool seqObsolete;
            bool valGeneric;
            bool valVarious;
            //      Boolean valPresent;

            bool displaySeq;

            foreach (KeyValuePair<string, PCLComplexSeq> kvp in _seqs)
            {
                displaySeq = true;

                seqObsolete = kvp.Value.FlagObsolete;
                seqDiscrete = kvp.Value.FlagDiscrete;

                if (seqObsolete && (!incObsoleteSeqs))
                    displaySeq = false;

                if (seqDiscrete)
                {
                    valGeneric = kvp.Value.FlagValGeneric;
                    valVarious = kvp.Value.FlagValVarious;

                    if (incDiscreteVal && valGeneric)
                        displaySeq = false;
                    else if ((!incDiscreteVal) && (!valGeneric) && (!valVarious))
                        displaySeq = false;
                }

                if (displaySeq)
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

        public static void DisplayStatsCounts(DataTable table, bool incUsedSeqsOnly, bool excUnusedObsSeqs)
        {
            bool displaySeq,
                    hddrWritten;

            DataRow row;

            hddrWritten = false;

            //----------------------------------------------------------------//

            displaySeq = true;

            int count = _seqUnknown.StatsCtTotal;
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

            foreach (KeyValuePair<string, PCLComplexSeq> kvp in _seqs)
            {
                displaySeq = true;

                count = kvp.Value.StatsCtTotal;

                if (count == 0)
                {
                    if (incUsedSeqsOnly)
                        displaySeq = false;
                    else if (excUnusedObsSeqs && (kvp.Value.FlagObsolete))
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
            //----------------------------------------------------------------//

            DataRow row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "____________________________";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "PCL parameterised sequences:";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
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
        // Populate the table of complex PCL sequences.                       //
        //                                                                    //
        // Note:                                                              //
        //                                                                    //
        // - The key of the first entry (for 'unknown' sequences) MUST be the //
        //   lowest key, and this key must NOT be a potential match for any   //
        //   valid sequence.                                                  //
        //                                                                    //
        // - The 'valueGeneric' entries for those sequences which have        //
        //   'limited-value-sets' include the word 'discrete' in the          //
        //   description text; do not remove this word, because:              //
        //                                                                    //
        //   -  The 'Print Languages' tool uses this text directly when the   //
        //      'Show Discrete Values' option is unset.                       //
        //                                                                    //
        //   -  The 'PRN File Analyse' tool replaces the word 'discrete' by   //
        //      'unknown/illegal' when reporting a sequence with a value      //
        //      which does not match any in the limited value set.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PopulateTable()
        {
            const bool flagNone = false;
            const bool flagDiscrete = true;
            const bool flagNilGChar = true;
            const bool flagNilValue = true;
            const bool flagValIsLen = true;
            const bool flagObsolete = true;
            const bool flagDisplayHexVal = true;
            // const Boolean flagResetGL2 = true;      // not used //

            byte iChar, gChar, tChar;
            int value,
                  count;

            string root;

            iChar = 0x20;   //   //
            gChar = 0x20;   //   //
            tChar = 0x20;   //   //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // ???       //
            _seqUnknown =
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "*** Unknown sequence ***");

            iChar = 0x25;   // % //
            gChar = 0x20;   //   //
            tChar = 0x41;   // A //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // %#A       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "Enter PCL Mode (# = discrete value)"));
            value = 0;                                           // %0A       //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "Enter PCL Mode: Cursor = PCL"));
            value = 1;                                           // %1A       //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "Enter PCL Mode: Cursor = HP-GL/2"));

            iChar = 0x25;   // % //
            gChar = 0x20;   //   //
            tChar = 0x42;   // B //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // %#B       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.SwitchToHPGL2,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "Enter HP-GL/2 Mode (# = discrete value)"));
            value = -1;                                          // %-1B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.SwitchToHPGL2,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "Enter HP-GL/2 Mode: Single Context"));
            value = 0;                                           // %0B       //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.SwitchToHPGL2,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "Enter HP-GL/2 Mode: Pen = HP-GL/2"));
            value = 1;                                           // %1B       //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.SwitchToHPGL2,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "Enter HP-GL/2 Mode: Pen = PCL"));
            value = 2;                                           // %2B       //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.SwitchToHPGL2,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "Enter HP-GL/2 Mode: Pen = HP-GL/2; Cursor = PCL"));
            value = 3;                                           // %3B       //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.SwitchToHPGL2,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "Enter HP-GL/2 Mode: Pen = PCL; Cursor = PCL"));

            iChar = 0x25;   // % //
            gChar = 0x20;   //   //
            tChar = 0x58;   // X //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // %#X       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.SwitchToPJL,
                                   PrnParseConstants.OvlAct.Reset,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Universal Exit Language (# = -12345)"));
            value = -12345;                                      // %-12345X  //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.SwitchToPJL,
                                   PrnParseConstants.OvlAct.Reset,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Universal Exit Language (UEL)"));

            iChar = 0x26;   // & //
            gChar = 0x61;   // a //
            tChar = 0x43;   // C //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &a#C      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Cursor Position Horizontal (column #)"));

            iChar = 0x26;   // & //
            gChar = 0x61;   // a //
            tChar = 0x47;   // G //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &a#G      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Duplex Page Side Selection (# = discrete value)"));
            value = 0;                                           // &a0G      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Duplex Page Side Selection: Next Side"));
            value = 1;                                           // &a1G      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Duplex Page Side Selection: Front Side"));
            value = 2;                                           // &a2G      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Duplex Page Side Selection: Back Side"));

            iChar = 0x26;   // & //
            gChar = 0x61;   // a //
            tChar = 0x48;   // H //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &a#H      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Cursor Position Horizontal (# decipoints)"));

            iChar = 0x26;   // & //
            gChar = 0x61;   // a //
            tChar = 0x4c;   // L //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &a#L      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Left Margin (column #)"));

            iChar = 0x26;   // & //
            gChar = 0x61;   // a //
            tChar = 0x4d;   // M //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &a#M      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Right Margin (column #)"));

            iChar = 0x26;   // & //
            gChar = 0x61;   // a //
            tChar = 0x4e;   // N //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &a#N      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Negative Motion (# = discrete value)"));
            value = 0;                                           // &a0N      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Negative Motion: Present"));
            value = 1;                                           // &a1N      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Negative Motion: Not Present"));

            iChar = 0x26;   // & //
            gChar = 0x61;   // a //
            tChar = 0x50;   // P //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &a#P      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Print Direction (# = discrete value)"));
            value = 0;                                           // &a0P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Print Direction: 0 degree rotation"));
            value = 90;                                          // &a90P     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Print Direction: 90 degree ccw rotation"));
            value = 180;                                         // &a180P    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Print Direction: 180 degree ccw rotation"));
            value = 270;                                         // &a270P    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Print Direction: 270 degree ccw rotation"));

            iChar = 0x26;   // & //
            gChar = 0x61;   // a //
            tChar = 0x52;   // R //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &a#R      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Cursor Position Vertical   (row #)"));

            iChar = 0x26;   // & //
            gChar = 0x61;   // a //
            tChar = 0x54;   // T //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &a#T      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Set Horizontal Tab (column #)"));

            iChar = 0x26;   // & //
            gChar = 0x61;   // a //
            tChar = 0x55;   // U //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &a#U      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Clear Horizontal Tab (column #)"));

            iChar = 0x26;   // & //
            gChar = 0x61;   // a //
            tChar = 0x56;   // V //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &a#V      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Cursor Position Vertical   (# decipoints)"));

            iChar = 0x26;   // & //
            gChar = 0x61;   // a //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &a#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.LogicalPageData,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Define Logical Page (data length = #)"));

            iChar = 0x26;   // & //
            gChar = 0x62;   // b //
            tChar = 0x46;   // F //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &b#F      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Finish Mode (# = discrete value)"));
            value = 0;                                           // &b0F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Finish Mode: Matte"));
            value = 1;                                           // &b1F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Finish Mode: Glossy"));

            iChar = 0x26;   // & //
            gChar = 0x62;   // b //
            tChar = 0x4d;   // M //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &b#M      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Monochrome Print Mode (# = discrete value)"));
            value = 0;                                           // &b0M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Monochrome Print Mode: Mixed Rendering"));
            value = 1;                                           // &b1M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Monochrome Print Mode: Grey Equivalent"));

            iChar = 0x26;   // & //
            gChar = 0x62;   // b //
            tChar = 0x54;   // T //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &b#T      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Dry Timer (# seconds)"));

            iChar = 0x26;   // & //
            gChar = 0x62;   // b //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &b#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.ConfigurationIO,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Configuration (I/O) (data length = #)"));

            iChar = 0x26;   // & //
            gChar = 0x63;   // c //
            tChar = 0x54;   // T //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &c#T      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Text Path Direction (# = discrete value)"));
            value = -1;                                          // &c-1T     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Text Path Direction: Vertical Rotated"));
            value = 0;                                           // &c0T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Text Path Direction: Horizontal"));

            iChar = 0x26;   // & //
            gChar = 0x63;   // c //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &c#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Cluster Printing (data length = #)"));

            iChar = 0x26;   // & //
            gChar = 0x64;   // d //
            tChar = 0x40;   // @ //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &d#@      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNilValue, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Underline Disable"));

            iChar = 0x26;   // & //
            gChar = 0x64;   // d //
            tChar = 0x44;   // D //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &d#D      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Underline Enable (# = discrete value)"));
            value = 0;                                           // &d0D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Underline Enable: default"));
            value = 1;                                           // &d1D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Underline Enable: single"));
            value = 2;                                           // &d2D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Underline Enable: double"));
            value = 3;                                           // &d3D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Underline Enable: Floating"));
            value = 4;                                           // &d4D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Underline Enable: floating double"));

            iChar = 0x26;   // & //
            gChar = 0x66;   // f //
            tChar = 0x46;   // F //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &f#F      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Media Eject Length (# decipoints)"));

            iChar = 0x26;   // & //
            gChar = 0x66;   // f //
            tChar = 0x47;   // G //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &f#G      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Page Width (# decipoints)"));

            iChar = 0x26;   // & //
            gChar = 0x66;   // f //
            tChar = 0x49;   // I //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &f#I      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Custom Paper Width  (# decipoints)"));

            iChar = 0x26;   // & //
            gChar = 0x66;   // f //
            tChar = 0x4a;   // J //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &f#J      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Custom Paper Length (# decipoints)"));

            iChar = 0x26;   // & //
            gChar = 0x66;   // f //
            tChar = 0x53;   // S //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &f#S      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Cursor Position Stack (# = discrete value)"));
            value = 0;                                           // &f0S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Cursor Position Stack: Push (Store)"));
            value = 1;                                           // &f1S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Cursor Position Stack: Pop (Recall) "));

            iChar = 0x26;   // & //
            gChar = 0x66;   // f //
            tChar = 0x58;   // X //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &f#X      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Macros,
                                   "Macro Control (# = discrete value)"));
            value = 0;                                           // &f0X      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.MacroStart,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Macros,
                                   "Macro Control: Start Macro Definition"));
            value = 1;                                           // &f1X      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.MacroStop,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Macros,
                                   "Macro Control: Stop Macro Definition"));
            value = 2;                                           // &f2X      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Macros,
                                   "Macro Control: Execute Macro"));
            value = 3;                                           // &f3X      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Macros,
                                   "Macro Control: Call Macro"));
            value = 4;                                           // &f4X      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.Macros,
                                   "Macro Control: Enable Macro For Overlay"));
            value = 5;                                           // &f5X      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.Macros,
                                   "Macro Control: Disable Overlay"));
            value = 6;                                           // &f6X      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.Macros,
                                   "Macro Control: Delete All Macros"));
            value = 7;                                           // &f7X      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.Macros,
                                   "Macro Control: Delete Temporary Macros"));
            value = 8;                                           // &f8X      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.DownloadDelete,
                                   PrnParseConstants.SeqGrp.Macros,
                                   "Macro Control: Delete Macro"));
            value = 9;                                           // &f9X      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Macros,
                                   "Macro Control: Make Macro Temporary"));
            value = 10;                                          // &f10X     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Macros,
                                   "Macro Control: Make Macro Permanent"));
            value = 11;                                          // &f11X     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Macros,
                                   "Macro Control: Create Bitmap of Macro"));

            iChar = 0x26;   // & //
            gChar = 0x66;   // f //
            tChar = 0x59;   // Y //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &f#Y      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.IdMacro,
                                   PrnParseConstants.SeqGrp.Macros,
                                   "Macro Control ID (identifer = #)"));

            iChar = 0x26;   // & //
            gChar = 0x69;   // i //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &i#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Underware Function Configuration (data length = #)"));
            iChar = 0x26;   // & //
            gChar = 0x6b;   // k //
            tChar = 0x45;   // E //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &k#E      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Underline Enhancement (# = discrete value)"));
            value = 0;                                           // &k0E      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Underline Enhancement: Line-By-Line"));
            value = 1;                                           // &k1E      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "UnderLine Enhancement: Modal"));
            iChar = 0x26;   // & //
            gChar = 0x6b;   // k //
            tChar = 0x46;   // F //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &k#F      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Shift In/Out Control (# = discrete value)"));
            value = 0;                                           // &k0F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Shift In/Out Control: Line-By-Line"));
            value = 1;                                           // &k1F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Shift In/Out Control: Modal"));

            iChar = 0x26;   // & //
            gChar = 0x6b;   // k //
            tChar = 0x47;   // G //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &k#G      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Line Termination (# = discrete value)"));
            value = 0;                                           // &k0G      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Line Termination: CR=CR, LF=LF, FF=FF"));
            value = 1;                                           // &k1G      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Line Termination: CR=CR+LF, LF=LF, FF=FF"));
            value = 2;                                           // &k2G      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Line Termination: CR=CR, LF=CR+LF, FF=CR+FF"));
            value = 3;                                           // &k3G      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Line Termination: CR=CR+LF, LF=CR+LF, FF=CR+FF"));

            iChar = 0x26;   // & //
            gChar = 0x6b;   // k //
            tChar = 0x48;   // H //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &k#H      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Horizontal Motion Index (#/120 inches)"));
            iChar = 0x26;   // & //
            gChar = 0x6b;   // k //
            tChar = 0x49;   // I //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &k#I      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Character Set Selection Control (# = discrete value)"));
            value = 0;                                           // &k0I      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Character Set Selection Control: SI/SO"));
            value = 1;                                           // &k1I      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Character Set Selection Control: bit 8"));

            iChar = 0x26;   // & //
            gChar = 0x6b;   // k //
            tChar = 0x53;   // S //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &k#S      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Pitch Mode (# = discrete value)"));
            value = 0;                                           // &k0S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Pitch Mode: Pica (10 cpi)"));
            value = 1;                                           // &k1S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Pitch Mode: Double Wide (5 cpi)"));
            value = 2;                                           // &k2S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Pitch Mode: Compressed (16.6 cpi)"));
            value = 3;                                           // &k3S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Pitch Mode: Double Wide Comp. (8.3 cpi)"));
            value = 4;                                           // &k4S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Pitch Mode: Elite (12 cpi)"));
            value = 8;                                           // &k8S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Pitch Mode: Double Width/Height"));

            iChar = 0x26;   // & //
            gChar = 0x6b;   // k //
            tChar = 0x56;   // V //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &k#V      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Head View Mode (# = discrete value)"));
            value = 0;                                           // &k0V      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Head View Mode: Enable"));
            value = 1;                                           // &k1V      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Head View Mode: Disable"));

            iChar = 0x26;   // & //
            gChar = 0x6b;   // k //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &k#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Mode (Text) (# = discrete value)"));
            value = 0;                                           // &k0W      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Mode (Text): Unidirectional (L->R)"));
            value = 1;                                           // &k1W      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Mode (Text): Bidirectional"));
            value = 2;                                           // &k2W      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Mode (Text): Unidirectional (R->L)"));
            value = 3;                                           // &k3W      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Mode (Text): High Intensity"));
            value = 5;                                           // &k5W      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Mode (Text): Text Scale Off"));
            value = 6;                                           // &k6W      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Mode (Text): Text Scale On"));
            value = 7;                                           // &k7W      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Mode (Text): Fast High Intensity"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x41;   // A //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &l#A      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Page Size (# = discrete value)"));

            PopulateTableAddPaperSizes(iChar, gChar, tChar, root);

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x43;   // C //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &l#C      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Vertical Motion Index (#/48 inches)"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x44;   // D //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &l#D      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Line Spacing (# = lines-per-inch discrete value)"));
            value = 0;                                           // &l0D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Line Spacing (0 lines-per-inch)"));
            value = 1;                                           // &l1D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Line Spacing (1 line-per-inch)"));
            value = 2;                                           // &l2D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Line Spacing (2 lines-per-inch)"));
            value = 3;                                           // &l3D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Line Spacing (3 lines-per-inch)"));
            value = 4;                                           // &l4D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Line Spacing (4 lines-per-inch)"));
            value = 6;                                           // &l6D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Line Spacing (6 lines-per-inch)"));
            value = 8;                                           // &l8D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Line Spacing (8 lines-per-inch)"));
            value = 12;                                          // &l12D     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Line Spacing (12 lines-per-inch)"));
            value = 16;                                          // &l16D     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Line Spacing (16 lines-per-inch)"));
            value = 24;                                          // &l24D     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Line Spacing (24 lines-per-inch)"));
            value = 48;                                          // &l48D     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Line Spacing (48 lines-per-inch)"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x45;   // E //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &l#E      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Top Margin (# lines)"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x46;   // F //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &l#F      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Text Length (# lines)"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x47;   // G //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueVarious;                               // &l#G      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Output Bin: id # is Printer Dependent"));
            value = 0;                                           // &l0G      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Output Bin: Automatic Selection"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x48;   // H //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueVarious;                               // &l#H      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Paper Source: id # is Printer Dependent"));
            value = 0;                                           // &l0H      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Paper Source: Unchanged (eject page)"));
            value = 7;                                           // &l7H      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Paper Source: Automatic Selection"));
            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x4a;   // J //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &l#J      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Auto Justification (# = discrete value)"));
            value = 0;                                           // &l0J      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Auto Justification: None"));
            value = 1;                                           // &l1J      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Auto Justification: Right Flush"));
            value = 2;                                           // &l2J      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Auto Justification: Centred"));
            value = 3;                                           // &l3J      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Auto Justification: Justified"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x4c;   // L //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &l#L      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Perforation Skip (# = discrete value)"));
            value = 0;                                           // &l0L      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Perforation Skip: Disable"));
            value = 1;                                           // &l1L      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Perforation Skip: Enable"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x4d;   // M //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &l#M      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Media Type (# = discrete value)"));
            value = 0;                                           // &l0M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Media Type: Plain Paper"));
            value = 1;                                           // &l1M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Media Type: Bond Paper"));
            value = 2;                                           // &l2M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Media Type: Special Paper"));
            value = 3;                                           // &l3M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Media Type: Glossy Film"));
            value = 4;                                           // &l4M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Media Type: Transparency Film"));
            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x4f;   // O //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &l#O      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Orientation (# = discrete value)"));

            count = PCLOrientations.GetCount();

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    value = PCLOrientations.GetIdPCL(i);

                    _seqs.Add(root + ":" + value.ToString("X4"),
                         new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                            flagNone, flagNone, flagNone,
                                            flagNone, flagNone, flagNone,
                                            PrnParseConstants.ActPCL.None,
                                            PrnParseConstants.OvlAct.PageChange,
                                            PrnParseConstants.SeqGrp.PageControl,
                                            "Orientation: " +
                                            PCLOrientations.GetName(i)));
                }
            }

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x50;   // P //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &l#P      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Page Length (# lines)"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x52;   // R //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &l#R      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Clear Vertical Tab Absolute (line #)"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x53;   // S //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &l#S      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Simplex/Duplex (# = discrete value)"));
            value = 0;                                           // &l0S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Simplex/Duplex: Simplex"));
            value = 1;                                           // &l1S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Simplex/Duplex: Duplex Long-Edge Bind"));
            value = 2;                                           // &l2S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Simplex/Duplex: Duplex Short-Edge Bind"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x54;   // T //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &l#T      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Job Separation (# = discrete value)"));
            value = 1;                                           // &l1T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Job Separation: On"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x55;   // U //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &l#U      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Left (Long-Edge) Offset (# decipoints)"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x56;   // V //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &l#V      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Vertical Position Via VFC (channel #)"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &l#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Define VFC Table (data length = #)"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x58;   // X //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &l#X      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Number of Copies (#)"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x59;   // Y //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &l#Y      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Set Vertical Tab Absolute (line #)"));

            iChar = 0x26;   // & //
            gChar = 0x6c;   // l //
            tChar = 0x5a;   // Z //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &l#Z      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Top (Short-Edge) Offset (# decipoints)"));

            iChar = 0x26;   // & //
            gChar = 0x6e;   // n //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &n#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.AlphaNumericID,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Alphanumeric ID (data length = #)"));

            iChar = 0x26;   // & //
            gChar = 0x70;   // p //
            tChar = 0x43;   // C //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &p#C      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Palette Control (# = discrete value)"));
            value = 0;                                           // &p0C      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Palette Control: Delete All in Store"));
            value = 1;                                           // &p1C      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Palette Control: Delete All in Stack"));
            value = 2;                                           // &p2C      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.DownloadDelete,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Palette Control: Delete Palette"));
            value = 6;                                           // &p6C      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Palette Control: Copy Palette"));

            iChar = 0x26;   // & //
            gChar = 0x70;   // p //
            tChar = 0x49;   // I //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &p#I      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.IdPalette,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Palette Control ID (identifier = #)"));

            iChar = 0x26;   // & //
            gChar = 0x70;   // p //
            tChar = 0x53;   // S //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &p#S      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Select Palette (identifier = #)"));
            iChar = 0x26;   // & //
            gChar = 0x70;   // p //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &p#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.EscEncText,
                                   PrnParseConstants.OvlAct.PageMark,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Escapement Encapsulated Text (data length = #)"));

            iChar = 0x26;   // & //
            gChar = 0x70;   // p //
            tChar = 0x58;   // X //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // &p#X      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.EmbeddedData,
                                   PrnParseConstants.OvlAct.PageMark,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Transparent Print (data length = #)"));

            iChar = 0x26;   // & //
            gChar = 0x72;   // r //
            tChar = 0x46;   // F //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &r#F      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Flush (# = discrete value)"));
            value = 0;                                           // &r0F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Flush: All Complete Pages"));
            value = 1;                                           // &r1F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageChange,
                                   PrnParseConstants.SeqGrp.PageControl,
                                   "Flush: All Pages"));
            iChar = 0x26;   // & //
            gChar = 0x73;   // s //
            tChar = 0x43;   // C //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &s#C      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "End-of-Line Wrap (# = discrete value)"));
            value = 0;                                           // &s0C      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "End-of-Line Wrap: Enable"));
            value = 1;                                           // &s1C      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "End-of-Line Wrap: Disable"));
            iChar = 0x26;   // & //
            gChar = 0x73;   // s //
            tChar = 0x49;   // I //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &s#I      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Character Set Default Control (# = discrete value)"));
            value = 0;                                           // &s0I      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Character Set Default Control: Blank"));
            value = 1;                                           // &s1I      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Character Set Default Control: Ignore"));
            iChar = 0x26;   // & //
            gChar = 0x74;   // t //
            tChar = 0x50;   // P //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &t#P      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.TextParsing,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Text Parsing Method (# = discrete value)"));

            count = PCLTextParsingMethods.GetCount();

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (i != (byte)PCLTextParsingMethods.Index.not_specified)
                    {
                        value = PCLTextParsingMethods.GetValue(i);

                        _seqs.Add(root + ":" + value.ToString("X4"),
                             new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                                flagNone, flagNone, flagNone,
                                                flagNone, flagNone, flagNone,
                                                PrnParseConstants.ActPCL.TextParsing,
                                                PrnParseConstants.OvlAct.None,
                                                PrnParseConstants.SeqGrp.FontSelection,
                                                "Text Parsing Method: " +
                                                PCLTextParsingMethods.GetDesc(i)));
                    }
                }
            }

            iChar = 0x26;   // & //
            gChar = 0x75;   // u //
            tChar = 0x44;   // D //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &u#D      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (# PCL units per inch discrete value)"));
            value = 96;                                          // &u96D     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (96 PCL units per inch)"));
            value = 100;                                         // &u100D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (100 PCL units per inch)"));
            value = 120;                                         // &u120D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (120 PCL units per inch)"));
            value = 144;                                         // &u144D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (144 PCL units per inch)"));
            value = 150;                                         // &u150D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (150 PCL units per inch)"));
            value = 160;                                         // &u160D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (160 PCL units per inch)"));
            value = 180;                                         // &u180D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (180 PCL units per inch)"));
            value = 200;                                         // &u200D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (200 PCL units per inch)"));
            value = 225;                                         // &u225D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (225 PCL units per inch)"));
            value = 240;                                         // &u240D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (240 PCL units per inch)"));
            value = 288;                                         // &u288D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (288 PCL units per inch)"));
            value = 300;                                         // &u300D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (300 PCL units per inch)"));
            value = 360;                                         // &u360D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (360 PCL units per inch)"));
            value = 400;                                         // &u400D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (400 PCL units per inch)"));
            value = 450;                                         // &u450D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (450 PCL units per inch)"));
            value = 480;                                         // &u480D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (480 PCL units per inch)"));
            value = 600;                                         // &u600D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (600 PCL units per inch)"));
            value = 720;                                         // &u720D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (720 PCL units per inch)"));
            value = 800;                                         // &u800D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (800 PCL units per inch)"));
            value = 900;                                         // &u900D    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (900 PCL units per inch)"));
            value = 1200;                                        // &u1200D   //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (1200 PCL units per inch)"));
            value = 1440;                                        // &u1440D   //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (1440 PCL units per inch)"));
            value = 1800;                                        // &u1800D   //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (1800 PCL units per inch)"));
            value = 2400;                                        // &u2400D   //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (2400 PCL units per inch)"));
            value = 3600;                                        // &u3600D   //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (3600 PCL units per inch)"));
            value = 7200;                                        // &u7200D   //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Unit-of-Measure (7200 PCL units per inch)"));
            iChar = 0x26;   // & //
            gChar = 0x76;   // v //
            tChar = 0x53;   // S //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // &v#S      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Text Colour (# = discrete value)"));
            value = 0;                                           // &v0S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Text Colour: Black"));
            value = 1;                                           // &v1S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Text Colour: Red"));
            value = 2;                                           // &v2S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Text Colour: Green"));
            value = 3;                                           // &v3S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Text Colour: Yellow"));
            value = 4;                                           // &v4S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Text Colour: Blue"));
            value = 5;                                           // &v5S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Text Colour: Magenta"));
            value = 6;                                           // &v6S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Text Colour: Cyan"));
            value = 7;                                           // &v7S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Text Colour: White"));

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x40;   // @ //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // (#@       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Select Default (# = discrete value)"));
            value = 0;                                           // (0@       //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Select Default Character Set"));
            value = 2;                                           // (2@       //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Select Current Character Set"));
            value = 3;                                           // (3@       //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Select Default font"));

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x41;   // A //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#A       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #A)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x42;   // B //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#B       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #B)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x43;   // C //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#C       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #C)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x44;   // D //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#D       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #D)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x45;   // E //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#E       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #E)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x46;   // F //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#F       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #F)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x47;   // G //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#G       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #G)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x48;   // H //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#H       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #H)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x49;   // I //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#I       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #I)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x4a;   // J //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#J       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #J)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x4b;   // K //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#K       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #K)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x4c;   // L //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#L       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #L)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x4d;   // M //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#M       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #M)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x4e;   // N //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#N       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #N)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x4f;   // O //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#O       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #O)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x50;   // P //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#P       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #P)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x51;   // Q //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#Q       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #Q)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x52;   // R //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#R       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #R)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x53;   // S //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#S       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #S)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x54;   // T //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#T       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #T)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x55;   // U //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#U       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #U)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x56;   // V //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#V       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #V)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#W       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #W)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x58;   // X //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#X       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Select by ID (identifier = #)"));

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x59;   // Y //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#Y       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #Y)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x20;   //   //
            tChar = 0x5a;   // Z //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (#Z       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Symbol Set (identifier = #Z)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x66;   // f //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (f#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.DefineSymbolSet,
                                   PrnParseConstants.OvlAct.Download,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Define Symbol Set (data length = #)"));

            iChar = 0x28;   // ( //
            gChar = 0x73;   // s //
            tChar = 0x42;   // B //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // (s#B      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight (# = discrete value)"));
            value = -7;                                          // (s-7B     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Ultra Thin"));
            value = -6;                                          // (s-6B     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Extra Thin"));
            value = -5;                                          // (s-5B     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Thin"));
            value = -4;                                          // (s-4B     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Extra Light"));
            value = -3;                                          // (s-3B     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Light"));
            value = -2;                                          // (s-2B     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Demi Light"));
            value = -1;                                          // (s-1B     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Semi Light"));
            value = 0;                                           // (s0B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Medium"));
            value = 1;                                           // (s1B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Semi Bold"));
            value = 2;                                           // (s2B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Demi Bold"));
            value = 3;                                           // (s3B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Bold"));
            value = 4;                                           // (s4B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Extra Bold"));
            value = 5;                                           // (s5B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Black"));
            value = 6;                                           // (s6B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Extra Black"));
            value = 7;                                           // (s7B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Stroke Weight: Ultra Black"));

            iChar = 0x28;   // ( //
            gChar = 0x73;   // s //
            tChar = 0x48;   // H //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (s#H      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Pitch (# characters per inch)"));

            iChar = 0x28;   // ( //
            gChar = 0x73;   // s //
            tChar = 0x50;   // P //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // (s#P      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Spacing (# = discrete value)"));
            value = 0;                                           // (s0P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Spacing: Fixed"));
            value = 1;                                           // (s1P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Spacing: Proportional"));

            iChar = 0x28;   // ( //
            gChar = 0x73;   // s //
            tChar = 0x51;   // Q //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // (s#Q      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Quality (# = discrete value)"));
            value = 0;                                           // (s0Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Quality: Data Processing"));
            value = 1;                                           // (s1Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Quality: Near Letter"));
            value = 2;                                           // (s2Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Quality: Letter"));

            iChar = 0x28;   // ( //
            gChar = 0x73;   // s //
            tChar = 0x53;   // S //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // (s#S      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Style (identifier = #)"));
            value = 0;                                           // (s0S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Style (Upright, solid)"));
            value = 1;                                           // (s1S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Style (Italic)"));
            value = 4;                                           // (s4S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Style (Condensed)"));
            value = 5;                                           // (s5S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Style (Condensed Italic)"));
            value = 8;                                           // (s8S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Style (Compressed)"));
            value = 24;                                          // (s24S     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Style (Expanded)"));
            value = 32;                                          // (s32S     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Style (Outline)"));
            value = 64;                                          // (s64S     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Style (Inline)"));
            value = 128;                                         // (s128S    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Style (Shadowed)"));
            value = 160;                                         // (s160S    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Style (Outline Shadowed)"));

            iChar = 0x28;   // ( //
            gChar = 0x73;   // s //
            tChar = 0x54;   // T //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (s#T      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Typeface (identifier = #)"));

            PopulateTableAddFonts(iChar, gChar, tChar, root);

            iChar = 0x28;   // ( //
            gChar = 0x73;   // s //
            tChar = 0x55;   // U //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // (s#U      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Placement (# = discrete value)"));
            value = -1;                                          // (s-1U     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Placement: Superior"));
            value = 0;                                           // (s0U      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Placement: Normal"));
            value = 1;                                           // (s1U      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Placement: Inferior"));

            iChar = 0x28;   // ( //
            gChar = 0x73;   // s //
            tChar = 0x56;   // V //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (s#V      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Primary Font: Height (# points)"));

            iChar = 0x28;   // ( //
            gChar = 0x73;   // s //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // (s#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.FontChar,
                                   PrnParseConstants.OvlAct.Download,
                                   PrnParseConstants.SeqGrp.SoftFontCreation,
                                   "Download Character (data length = #)"));

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x40;   // @ //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // )#@       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Select Default"));
            value = 0;                                           // )0@       //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Select Default Character Set"));
            value = 1;                                           // )1@       //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Select Default Primary Character Set"));
            value = 2;                                           // )2@       //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Select Current Primary Character Set"));
            value = 3;                                           // )3@       //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Select Default font"));

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x41;   // A //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#A       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #A)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x42;   // B //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#B       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #B)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x43;   // C //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#C       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #C)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x44;   // D //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#D       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #D)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x45;   // E //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#E       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #E)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x46;   // F //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#F       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #F)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x47;   // G //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#G       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #G)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x48;   // H //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#H       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #H)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x49;   // I //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#I       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #I)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x4a;   // J //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#J       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #J)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x4b;   // K //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#K       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #K)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x4c;   // L //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#L       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #L)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x4d;   // M //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#M       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #M)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x4e;   // N //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#N       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #N)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x4f;   // O //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#O       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #O)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x50;   // P //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#P       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #P)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x51;   // Q //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#Q       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #Q)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x52;   // R //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#R       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #R)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x53;   // S //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#S       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #S)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x54;   // T //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#T       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #T)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x55;   // U //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#U       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #U)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x56;   // V //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#V       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #V)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#W       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #W)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x58;   // X //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#X       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Select by ID (identifier = #)"));

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x59;   // Y //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#Y       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #Y)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x20;   //   //
            tChar = 0x5a;   // Z //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )#Z       //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNilGChar, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Symbol Set (identifier = #Z)"));

            PopulateTableAddSymsets(iChar, gChar, tChar, root);

            iChar = 0x29;   // ) //
            gChar = 0x73;   // s //
            tChar = 0x42;   // B //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // )s#B      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight (# = discrete value)"));
            value = -7;                                          // )s-7B     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Ultra Thin"));
            value = -6;                                          // )s-6B     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Extra Thin"));
            value = -5;                                          // )s-5B     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Thin"));
            value = -4;                                          // )s-4B     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Extra Light"));
            value = -3;                                          // )s-3B     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Light"));
            value = -2;                                          // )s-2B     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Demi Light"));
            value = -1;                                          // )s-1B     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Semi Light"));
            value = 0;                                           // )s0B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Medium"));
            value = 1;                                           // )s1B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Semi Bold"));
            value = 2;                                           // )s2B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Demi Bold"));
            value = 3;                                           // )s3B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Bold"));
            value = 4;                                           // )s4B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Extra Bold"));
            value = 5;                                           // )s5B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Black"));
            value = 6;                                           // )s6B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Extra Black"));
            value = 7;                                           // )s7B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Stroke Weight: Ultra Black"));
            iChar = 0x29;   // ) //
            gChar = 0x73;   // s //
            tChar = 0x48;   // H //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )s#H      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Pitch (# characters per inch)"));
            iChar = 0x29;   // ) //
            gChar = 0x73;   // s //
            tChar = 0x50;   // P //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // )s#P      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Spacing (# = discrete value)"));
            value = 0;                                           // )s0P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Spacing: Fixed"));
            value = 1;                                           // )s1P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Spacing: Proportional"));
            iChar = 0x29;   // ) //
            gChar = 0x73;   // s //
            tChar = 0x51;   // Q //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // )s#Q      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Quality (#=discrete value)"));
            value = 0;                                           // )s0Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Quality: Data Processing"));
            value = 1;                                           // )s1Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Quality: Near Letter"));
            value = 2;                                           // )s2Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Quality: Letter"));
            iChar = 0x29;   // ) //
            gChar = 0x73;   // s //
            tChar = 0x53;   // S //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // )s#S      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Style (identifier = #)"));
            value = 0;                                           // )s0S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Style (Upright, solid)"));
            value = 1;                                           // )s1S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Style (Italic)"));
            value = 4;                                           // )s4S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Style (Condensed)"));
            value = 5;                                           // )s5S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Style (Condensed Italic)"));
            value = 8;                                           // )s8S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Style (Compressed)"));
            value = 24;                                          // )s24S     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Style (Expanded)"));
            value = 32;                                          // )s32S     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Style (Outline)"));
            value = 64;                                          // )s64S     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Style (Inline)"));
            value = 128;                                         // )s128S    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Style (Shadowed)"));
            value = 160;                                         // )s160S    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.StyleData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Style (Outline Shadowed)"));
            iChar = 0x29;   // ) //
            gChar = 0x73;   // s //
            tChar = 0x54;   // T //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )s#T      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Typeface (identifier = #)"));

            PopulateTableAddFonts(iChar, gChar, tChar, root);
            iChar = 0x29;   // ) //
            gChar = 0x73;   // s //
            tChar = 0x55;   // U //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // )s#U      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Placement (# = discrete value)"));
            value = -1;                                          // )s-1U     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Placement: Superior"));
            value = 0;                                           // )s0U      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Placement: Normal"));
            value = 1;                                           // )s1U      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Placement: Inferior"));
            iChar = 0x29;   // ) //
            gChar = 0x73;   // s //
            tChar = 0x56;   // V //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )s#V      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Secondary Font: Height (# points)"));
            iChar = 0x29;   // ) //
            gChar = 0x73;   // s //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // )s#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.FontHddr,
                                   PrnParseConstants.OvlAct.Download,
                                   PrnParseConstants.SeqGrp.SoftFontCreation,
                                   "Download Font Header (data length = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x62;   // b //
            tChar = 0x42;   // B //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *b#B      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Grey Balancing (# = discrete value)"));
            value = 1;                                           // *b1B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Grey Balancing: Enable"));
            value = 2;                                           // *b2B      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Grey Balancing: Disable"));

            iChar = 0x2a;   // * //
            gChar = 0x62;   // b //
            tChar = 0x4c;   // L //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *b#L      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Line Path (# = discrete value)"));
            value = 0;                                           // *b0L      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Line Path: rows increment +Y"));
            value = 1;                                           // *b1L      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Line Path: rows increment -Y"));

            iChar = 0x2a;   // * //
            gChar = 0x62;   // b //
            tChar = 0x4d;   // M //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *b#M      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Set Compression Mode (# = discrete value)"));
            value = 0;                                           // *b0M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Set Compression Mode: Unencoded"));
            value = 1;                                           // *b1M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Set Compression Mode: Run-Length Encoded"));
            value = 2;                                           // *b2M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Set Compression Mode: TIFF"));
            value = 3;                                           // *b3M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Set Compression Mode: Delta Row"));
            value = 4;                                           // *b4M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Set Compression Mode: Block Data"));
            value = 5;                                           // *b5M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Set Compression Mode: Adaptive"));
            value = 6;                                           // *b6M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Set Compression Mode: CCITT G3 1-D (MH)"));
            value = 7;                                           // *b7M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Set Compression Mode: CCITT G3 2-D (MR)"));
            value = 8;                                           // *b8M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Set Compression Mode: CCITT G4 2-D (MMR)"));
            value = 9;                                           // *b9M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Set Compression Mode: Replaced Delta Row"));

            iChar = 0x2a;   // * //
            gChar = 0x62;   // b //
            tChar = 0x53;   // S //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *b#S      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Seed Row Source (# = discrete value)"));
            value = 0;                                           // *b0S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Seed Row Source: same plane as previous row"));
            value = 1;                                           // *b1S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Seed Row Source: previous plane"));
            value = 2;                                           // *b2S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Seed Row Source: 2nd previous plane"));
            value = 3;                                           // *b3S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Seed Row Source: 3rd previous plane"));

            iChar = 0x2a;   // * //
            gChar = 0x62;   // b //
            tChar = 0x56;   // V //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *b#V      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageMark,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Transfer Raster Data By Plane {data length = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x62;   // b //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *b#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageMark,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Transfer Raster Data By Row/Block (data length = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x62;   // b //
            tChar = 0x58;   // X //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *b#X      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Line X Offset (# pixels)"));

            iChar = 0x2a;   // * //
            gChar = 0x62;   // b //
            tChar = 0x59;   // Y //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *b#Y      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Y Offset (# raster lines)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x41;   // A //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#A      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RectangularAreaFill,
                                   "Rectangle Size Horizontal (# PCL units)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x42;   // B //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#B      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RectangularAreaFill,
                                   "Rectangle Size Vertical   (# PCL units)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x43;   // C //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#C      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Large Character Placement (column #)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x44;   // D //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#D      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.IdFont,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Assign Font ID Number (identifier = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x45;   // E //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#E      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagDisplayHexVal,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.SoftFontCreation,
                                   "Character Code (code-point = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x46;   // F //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *c#F      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Font Control (# = discrete value)"));
            value = 0;                                           // *c0F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Font Control: Delete All Soft Fonts"));
            value = 1;                                           // *c1F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Font Control: Delete All Temporary Fonts"));
            value = 2;                                           // *c2F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.DownloadDelete,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Font Control: Delete Soft Font"));
            value = 3;                                           // *c3F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Font Control: Delete Character Code"));
            value = 4;                                           // *c4F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Font Control: Make Soft Font Temporary"));
            value = 5;                                           // *c5F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Font Control: Make Soft Font Permanent"));
            value = 6;                                           // *c6F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Font Control: Copy/Assign Font As Temp."));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x47;   // G //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#G      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.IdPattern,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Assign Pattern ID (identifier = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x48;   // H //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#H      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RectangularAreaFill,
                                   "Rectangle Size Horizontal (# decipoints)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x4b;   // K //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#K      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "HP-GL/2 Plot Size Horizontal (# inches)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x4c;   // L //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#L      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "HP-GL/2 Plot Size Vertical   (# inches)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x4d;   // M //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#M      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Large Character Size (magnification = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x4e;   // N //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#N      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Large Character Tab #"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x50;   // P //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *c#P      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageMark,
                                   PrnParseConstants.SeqGrp.RectangularAreaFill,
                                   "Fill Rectangular Area (# = discrete value)"));
            value = 0;                                           // *c0P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageMark,
                                   PrnParseConstants.SeqGrp.RectangularAreaFill,
                                   "Fill Rectangular Area: Solid Area"));
            value = 1;                                           // *c1P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageMark,
                                   PrnParseConstants.SeqGrp.RectangularAreaFill,
                                   "Fill Rectangular Area: Solid White Areas"));
            value = 2;                                           // *c2P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageMark,
                                   PrnParseConstants.SeqGrp.RectangularAreaFill,
                                   "Fill Rectangular Area: Shading"));
            value = 3;                                           // *c3P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageMark,
                                   PrnParseConstants.SeqGrp.RectangularAreaFill,
                                   "Fill Rectangular Area: Cross-Hatch"));
            value = 4;                                           // *c4P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageMark,
                                   PrnParseConstants.SeqGrp.RectangularAreaFill,
                                   "Fill Rectangular Area: User Pattern"));
            value = 5;                                           // *c5P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageMark,
                                   PrnParseConstants.SeqGrp.RectangularAreaFill,
                                   "Fill Rectangular Area: Current Pattern"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x51;   // Q //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *c#Q      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Pattern Control (# = discrete value)"));
            value = 0;                                           // *c0Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Pattern Control: Delete All Patterns"));
            value = 1;                                           // *c1Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Pattern Control: Delete Temporary Patterns"));
            value = 2;                                           // *c2Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.DownloadDelete,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Pattern Control: Delete Pattern"));
            value = 3;                                           // *c3Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Pattern Control: Reserved"));
            value = 4;                                           // *c4Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Pattern Control: Make Pattern Temporary"));
            value = 5;                                           // *c5Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Pattern Control: Make Pattern Permanent"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x52;   // R //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#R      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.IdSymSet,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Assign Symbol Set ID (identifier = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x53;   // S //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *c#S      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Symbol Set Control (# = discrete value)"));
            value = 0;                                           // *c0S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Symbol Set Control: Delete User-Defined"));
            value = 1;                                           // *c1S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Symbol Set Control: Delete All Temporary"));
            value = 2;                                           // *c2S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.DownloadDelete,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Symbol Set Control: Delete Symbol Set"));
            value = 4;                                           // *c4S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Symbol Set Control: Make Temporary"));
            value = 5;                                           // *c5S      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.FontManagement,
                                   "Symbol Set Control: Make Permanent"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x54;   // T //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *c#T      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "Set Picture Frame Anchor Point (# = discrete value)"));
            value = 0;                                           // *c0T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "Set Picture Frame Anchor Point"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x56;   // V //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#V      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RectangularAreaFill,
                                   "Rectangle Size Vertical   (# decipoints)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.UserDefinedPattern,
                                   PrnParseConstants.OvlAct.Download,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Download User-Defined Pattern (data length = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x58;   // X //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#X      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "Picture Frame Size Width  (# decipoints)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x59;   // Y //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *c#Y      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PictureFrame,
                                   "Picture Frame Size Height (# decipoints)"));

            iChar = 0x2a;   // * //
            gChar = 0x63;   // c //
            tChar = 0x5a;   // Z //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *c#Z      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageMark,
                                   PrnParseConstants.SeqGrp.FontSelection,
                                   "Large Character Print Data (<encapsulated text>)"));

            iChar = 0x2a;   // * //
            gChar = 0x64;   // d //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *d#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.PaletteConfiguration,
                                   PrnParseConstants.OvlAct.Download,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Palette Configuration (data length = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x67;   // g //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *g#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.ConfigureRasterData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Configure Raster Data (data length = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x69;   // i //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *i#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.ViewIlluminant,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Viewing Illuminant (data length = #)"));
            iChar = 0x2a;   // * //
            gChar = 0x6c;   // l //
            tChar = 0x4f;   // O //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *l#O      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Logical Operation (ROP3) (# = discrete value)"));

            count = PCLLogicalOperations.GetCount();

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    value = PCLLogicalOperations.GetROPId(i);

                    _seqs.Add(root + ":" + i.ToString("X4"),
                         new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                           flagNone, flagNone, flagNone,
                                           flagNone, flagNone, flagNone,
                                           PrnParseConstants.ActPCL.None,
                                           PrnParseConstants.OvlAct.None,
                                           PrnParseConstants.SeqGrp.PrintModel,
                                           "Logical Op. " +
                                           PCLLogicalOperations.GetDescLong(i)));
                }
            }

            iChar = 0x2a;   // * //
            gChar = 0x6c;   // l //
            tChar = 0x50;   // P //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *l#P      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Clip Mask (# = discrete value)"));
            value = 0;                                           // *l0P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Clip Mask: Turn Off / Delete"));
            value = 1;                                           // *l1P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Clip Mask: Start Define"));
            value = 2;                                           // *l2P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Clip Mask: End Define (Inclusive Clip)"));
            value = 3;                                           // *l3P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Clip Mask: End Define (Exclusive Clip)"));

            iChar = 0x2a;   // * //
            gChar = 0x6c;   // l //
            tChar = 0x52;   // R //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *l#R      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Pixel Placement (# = discrete value)"));
            value = 0;                                           // *l0R      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Pixel Placement: Grid Intersection"));
            value = 1;                                           // *l1R      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Pixel Placement: Grid Centred"));

            iChar = 0x2a;   // * //
            gChar = 0x6c;   // l //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *l#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.ColourLookup,
                                   PrnParseConstants.OvlAct.Download,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Colour Lookup Tables (data length = #)"));
            iChar = 0x2a;   // * //
            gChar = 0x6d;   // m //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *m#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.DitherMatrix,
                                   PrnParseConstants.OvlAct.Download,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Download Dither Matrix (data length = #)"));
            iChar = 0x2a;   // * //
            gChar = 0x6f;   // o //
            tChar = 0x44;   // D //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *o#D      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Colour Raster Depletion (# = discrete value)"));
            value = 1;                                           // *o1D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Colour Raster Depletion: None"));
            value = 2;                                           // *o2D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Colour Raster Depletion: 25%"));
            value = 3;                                           // *o3D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Colour Raster Depletion: 50%"));
            value = 4;                                           // *o4D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Colour Raster Depletion: 25% compensated"));
            value = 5;                                           // *o5D      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Colour Raster Depletion: 50% compensated"));
            iChar = 0x2a;   // * //
            gChar = 0x6f;   // o //
            tChar = 0x4d;   // M //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *o#M      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Quality (# = discrete value)"));
            value = -1;                                          // *o-1M     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Quality: Draft"));
            value = 0;                                           // *o0M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Quality: Normal"));
            value = 1;                                           // *o1M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Quality: Presentation"));

            iChar = 0x2a;   // * //
            gChar = 0x6f;   // o //
            tChar = 0x51;   // Q //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *o#Q      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Mechanical Print Quality (# = discrete value)"));
            value = -1;                                          // *o-1Q     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Mechanical Print Quality: EconoFast"));
            value = 0;                                           // *o0Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Mechanical Print Quality: Normal"));
            value = 1;                                           // *o1Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Mechanical Print Quality: Presentation"));

            iChar = 0x2a;   // * //
            gChar = 0x6f;   // o //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *o#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.DriverConfiguration,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Driver Configuration (data length = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x70;   // p //
            tChar = 0x4e;   // N //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *p#N      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Mode (Graphics) (# = discrete value)"));
            value = 0;                                           // *p0N      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Mode (Graphics): Default"));
            value = 1;                                           // *p1N      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Mode (Graphics): Bidirectional"));
            value = 2;                                           // *p2N      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Mode (Graphics): Left-To-Right"));
            value = 3;                                           // *p3N      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Mode (Graphics): Right-To-Left"));
            value = 4;                                           // *p4N      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Print Mode (Graphics): Conditional"));
            iChar = 0x2a;   // * //
            gChar = 0x70;   // p //
            tChar = 0x50;   // P //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *p#P      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Palette Stack (# = discrete value)"));
            value = 0;                                           // *p0P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Palette Stack: Push (Store)"));
            value = 1;                                           // *p1P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Palette Stack: Pop (Recall)"));

            iChar = 0x2a;   // * //
            gChar = 0x70;   // p //
            tChar = 0x52;   // R //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *p#R      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Set Pattern Reference Point (# = discrete value)"));
            value = 0;                                           // *p0R      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Set Pattern Reference Point: Logical"));
            value = 1;                                           // *p1R      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Set Pattern Reference Point: Physical"));

            iChar = 0x2a;   // * //
            gChar = 0x70;   // p //
            tChar = 0x58;   // X //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *p#X      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Cursor Position Horizontal (# PCL units)"));

            iChar = 0x2a;   // * //
            gChar = 0x70;   // p //
            tChar = 0x59;   // Y //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *p#Y      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.CursorPositioning,
                                   "Cursor Position Vertical   (# PCL units)"));

            iChar = 0x2a;   // * //
            gChar = 0x72;   // r //
            tChar = 0x41;   // A //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *r#A      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Start Raster Graphics (# = discrete value)"));
            value = 0;                                           // *r0A      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Start Raster Graphics: Left Margin at 0"));
            value = 1;                                           // *r1A      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Start Raster Graphics: Left Margin at X"));
            value = 2;                                           // *r2A      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Start Raster Graphics: Scale Mode"));
            value = 3;                                           // *r3A      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Start Raster Graphics: Scale Mode at CAP"));

            iChar = 0x2a;   // * //
            gChar = 0x72;   // r //
            tChar = 0x42;   // B //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *r#B      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNilValue, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "End Raster Graphics (PCL4)"));

            iChar = 0x2a;   // * //
            gChar = 0x72;   // r //
            tChar = 0x43;   // C //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *r#C      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNilValue, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "End Raster Graphics"));

            iChar = 0x2a;   // * //
            gChar = 0x72;   // r //
            tChar = 0x46;   // F //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *r#F      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Graphics Presentation (# = discrete value)"));
            value = 0;                                           // *r0F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Graphics Presentation: Logical"));
            value = 3;                                           // *r3F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Graphics Presentation: Physical"));
            iChar = 0x2a;   // * //
            gChar = 0x72;   // r //
            tChar = 0x4c;   // L //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *r#L      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Resolution Horizontal (# dots per inch)"));

            iChar = 0x2a;   // * //
            gChar = 0x72;   // r //
            tChar = 0x51;   // Q //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *r#Q      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Graphics Quality (# = discrete value)"));
            value = 0;                                           // *r0Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Graphics Quality: User-Selected"));
            value = 1;                                           // *r1Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Graphics Quality: Draft"));
            value = 2;                                           // *r2Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Graphics Quality: High"));

            iChar = 0x2a;   // * //
            gChar = 0x72;   // r //
            tChar = 0x53;   // S //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *r#S      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Width: Source (# pixels)"));
            iChar = 0x2a;   // * //
            gChar = 0x72;   // r //
            tChar = 0x54;   // T //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *r#T      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Height: Source (# raster rows)"));
            iChar = 0x2a;   // * //
            gChar = 0x72;   // r //
            tChar = 0x55;   // U //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *r#U      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Simple Colour (# = discrete value)"));
            value = -4;                                          // *r-4U     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Simple Colour: 4-Plane KCMY Palette"));
            value = -3;                                          // *r-3U     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Simple Colour: 3-Plane CMY Palette"));
            value = 1;                                           // *r1U      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Simple Colour: 1-Plane K Palette"));
            value = 3;                                           // *r3U      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Simple Colour: 3-Plane RGB Palette"));
            iChar = 0x2a;   // * //
            gChar = 0x72;   // r //
            tChar = 0x56;   // V //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *r#V      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Resolution Vertical   (# dots per inch)"));

            iChar = 0x2a;   // * //
            gChar = 0x73;   // s //
            tChar = 0x49;   // I //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *s#I      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Entity (# = discrete value)"));
            value = 0;                                           // *s0I      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Entity: Font"));
            value = 1;                                           // *s1I      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Entity: Macro"));
            value = 2;                                           // *s2I      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Entity: User Pattern"));
            value = 3;                                           // *s3I      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Entity: Symbol Set"));
            value = 4;                                           // *s4I      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Entity: Font Extended"));
            iChar = 0x2a;   // * //
            gChar = 0x73;   // s //
            tChar = 0x4d;   // M //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *s#M      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Free Space (# = discrete value)"));
            value = 1;                                           // *s1M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Free Space"));
            iChar = 0x2a;   // * //
            gChar = 0x73;   // s //
            tChar = 0x54;   // T //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *s#T      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Loc. Type (# = discrete value)"));
            value = 0;                                           // *s0T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Loc. Type: Invalid"));
            value = 1;                                           // *s1T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Loc. Type: Current"));
            value = 2;                                           // *s2T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Loc. Type: All"));
            value = 3;                                           // *s3T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Loc. Type: Internal"));
            value = 4;                                           // *s4T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Loc. Type: Download"));
            value = 5;                                           // *s5T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Loc. Type: Cartridge"));
            value = 7;                                           // *s7T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Loc. Type: SIMMs"));

            iChar = 0x2a;   // * //
            gChar = 0x73;   // s //
            tChar = 0x55;   // U //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *s#U      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Loc. Unit (# = discrete value)"));
            value = 0;                                           // *s0U      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Loc. Unit: All"));
            value = 1;                                           // *s1U      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Loc. Unit: Temp./Hi-Priority"));
            value = 2;                                           // *s2U      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Loc. Unit: Perm./Lo-Priority"));
            value = 3;                                           // *s3U      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Loc. Unit: Entity 3"));
            value = 4;                                           // *s4U      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Status Readback Loc. Unit: Entity 4"));

            iChar = 0x2a;   // * //
            gChar = 0x73;   // s //
            tChar = 0x58;   // X //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *s#X      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Echo (identifier = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x73;   // s //
            tChar = 0x5e;   // ^ //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *s#^      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.StatusReadback,
                                   "Return Model Number"));

            iChar = 0x2a;   // * //
            gChar = 0x74;   // t //
            tChar = 0x46;   // F //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *t#F      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "QMS Magnum-5 Interpreter (# = discrete value)"));
            value = 0;                                           // *t0F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "QMS Magnum-5 Interpreter: Disable"));
            value = 1;                                           // *t1F      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "QMS Magnum-5 Interpreter: Enable"));
            iChar = 0x2a;   // * //
            gChar = 0x74;   // t //
            tChar = 0x47;   // G //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *t#G      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "GPIS Data Binding (# = discrete value)"));
            value = 0;                                           // *t0G      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "GPIS Data Binding: Exit Vector Graphics"));
            value = 1;                                           // *t1G      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "GPIS Data Binding: 16-bit binary"));
            value = 2;                                           // *t2G      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "GPIS Data Binding: 8-bit binary"));
            value = 3;                                           // *t3G      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "GPIS Data Binding: Character-Encoded"));
            iChar = 0x2a;   // * //
            gChar = 0x74;   // t //
            tChar = 0x48;   // H //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *t#H      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Width: Destination (# decipoints)"));

            iChar = 0x2a;   // * //
            gChar = 0x74;   // t //
            tChar = 0x49;   // I //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *t#I      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Gamma Correction (gamma number = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x74;   // t //
            tChar = 0x4a;   // J //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *t#J      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm (# = discrete value)"));
            value = 0;                                           // *t0J      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Continuous Tone"));
            value = 1;                                           // *t1J      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Snap To Primaries"));
            value = 2;                                           // *t2J      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Snap B/W; Colour>Black"));
            value = 3;                                           // *t3J      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Device Best Dither"));
            value = 4;                                           // *t4J      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Error Diffusion"));
            value = 5;                                           // *t5J      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Mono Best Dither"));
            value = 6;                                           // *t6J      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Mono Error Diffusion"));
            value = 7;                                           // *t7J      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Cluster Ordered Dither"));
            value = 8;                                           // *t8J      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Mono Cluster Ordered"));
            value = 9;                                           // *t9J      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: User-Defined Dither"));
            value = 10;                                          // *t10J     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Mono User-Def. Dither"));
            value = 11;                                          // *t11J     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Ordered Dither"));
            value = 12;                                          // *t12J     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Mono Ordered Dither"));
            value = 13;                                          // *t13J     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Noise Ordered Dither"));
            value = 14;                                          // *t14J     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Mono Noise Ordered"));
            value = 15;                                          // *t15J     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Continuous Tone Smooth"));
            value = 16;                                          // *t16J     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Mono Cont. Tone Detail"));
            value = 17;                                          // *t17J     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Mono Cont. Tone Smooth"));
            value = 18;                                          // *t18J     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Continuous Tone Basic"));
            value = 19;                                          // *t19J     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Render Algorithm: Mono Cont. Tone Basic"));
            iChar = 0x2a;   // * //
            gChar = 0x74;   // t //
            tChar = 0x4b;   // K //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *t#K      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Scale Algorithm (# = discrete value)"));
            value = 0;                                           // *t0K      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Scale Algorithm: Source/Light Background"));
            value = 1;                                           // *t1K      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Scale Algorithm: Source/Dark Background"));
            iChar = 0x2a;   // * //
            gChar = 0x74;   // t //
            tChar = 0x4d;   // M //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *t#M      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Vector Graphics Operating Mode (# = discrete value)"));
            value = 0;                                           // *t0M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Vector Graphics Operating Mode: Plotter"));
            value = 1;                                           // *t1M      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Vector Graphics Operating Mode: Printer"));

            iChar = 0x2a;   // * //
            gChar = 0x74;   // t //
            tChar = 0x4e;   // N //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *t#N      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Vector Graphics Mapping Mode (# = discrete value)"));
            value = 0;                                           // *t0N      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Vector Graphics Mapping Mode: Default"));
            value = 1;                                           // *t1N      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Vector Graphics Mapping Mode: Normal"));
            value = 2;                                           // *t2N      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Vector Graphics Mapping Mode: Distorted"));

            iChar = 0x2a;   // * //
            gChar = 0x74;   // t //
            tChar = 0x50;   // P //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *t#P      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Vector Graphics Print Control (# = discrete value)"));
            value = 0;                                           // *t0P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Vector Graphics Print Control: Retain"));
            value = 1;                                           // *t1P      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.JobControl,
                                   "Vector Graphics Print Control: Clear"));

            iChar = 0x2a;   // * //
            gChar = 0x74;   // t //
            tChar = 0x52;   // R //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *t#R      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Graphics Resolution (# = dots-per-inch discrete value)"));
            value = 75;                                          // *t75P     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Graphics Resolution (75 dots-per-inch)"));
            value = 100;                                         // *t100P    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Graphics Resolution (100 dots-per-inch)"));
            value = 150;                                         // *t150P    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Graphics Resolution (150 dots-per-inch)"));
            value = 200;                                         // *t200P    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Graphics Resolution (200 dots-per-inch)"));
            value = 300;                                         // *t300P    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Graphics Resolution (300 dots-per-inch)"));
            value = 600;                                         // *t600P    //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Graphics Resolution (600 dots-per-inch)"));

            iChar = 0x2a;   // * //
            gChar = 0x74;   // t //
            tChar = 0x56;   // V //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *t#V      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.RasterGraphics,
                                   "Raster Height: Destination (# decipoints)"));

            iChar = 0x2a;   // * //
            gChar = 0x74;   // t //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *t#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageMark,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "GPIS Data Transfer (data length = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x76;   // v //
            tChar = 0x41;   // A //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *v#A      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Colour Component 1 (primary value = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x76;   // v //
            tChar = 0x42;   // B //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *v#B      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Colour Component 2 (primary value = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x76;   // v //
            tChar = 0x43;   // C //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *v#C      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Colour Component 3 (primary value = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x76;   // v //
            tChar = 0x49;   // I //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *v#I      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Assign Colour Index (index = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x76;   // v //
            tChar = 0x4e;   // N //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *v#N      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Source Transparency Mode (# = discrete value)"));
            value = 0;                                           // *v0N      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Source Transparency Mode: Transparent"));
            value = 1;                                           // *v1N      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Source Transparency Mode: Opaque"));

            iChar = 0x2a;   // * //
            gChar = 0x76;   // v //
            tChar = 0x4f;   // O //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *v#O      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Pattern Transparency Mode (# = discrete value)"));
            value = 0;                                           // *v0O      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Pattern Transparency Mode: Transparent"));
            value = 1;                                           // *v1O      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.PrintModel,
                                   "Pattern Transparency Mode: Opaque"));

            iChar = 0x2a;   // * //
            gChar = 0x76;   // v //
            tChar = 0x53;   // S //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *v#S      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Foreground Colour (index = #)"));

            iChar = 0x2a;   // * //
            gChar = 0x76;   // v //
            tChar = 0x54;   // T //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *v#T      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Select Current Pattern (# = discrete value)"));
            value = 0;                                           // *v0T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Select Current Pattern: Solid Black"));
            value = 1;                                           // *v1T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Select Current Pattern: Solid White"));
            value = 2;                                           // *v2T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Select Current Pattern: Shading"));
            value = 3;                                           // *v3T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Select Current Pattern: Cross-Hatch"));
            value = 4;                                           // *v4T      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.UserPattern,
                                   "Select Current Pattern: User-Defined"));

            iChar = 0x2a;   // * //
            gChar = 0x76;   // v //
            tChar = 0x57;   // W //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *v#W      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagValIsLen,
                                   flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.ConfigureImageData,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Colour,
                                   "Configure Image Data (data length = #)"));
            iChar = 0x2a;   // * //
            gChar = 0x7a;   // z //
            tChar = 0x43;   // C //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *z#C      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Label Placement (column #)"));
            iChar = 0x2a;   // * //
            gChar = 0x7a;   // z //
            tChar = 0x48;   // H //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *z#H      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Label Height (#/10 inches)"));
            iChar = 0x2a;   // * //
            gChar = 0x7a;   // z //
            tChar = 0x51;   // Q //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *z#Q      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Header Control (# = discrete value)"));
            value = 0;                                           // *z0Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Header Control: Disable Print"));
            value = 1;                                           // *z1Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Header Control: Print Above"));
            value = 2;                                           // *z2Q      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Header Control: Print Below"));
            iChar = 0x2a;   // * //
            gChar = 0x7a;   // z //
            tChar = 0x52;   // R //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *z#R      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Wide Bar Width (# dots)"));
            iChar = 0x2a;   // * //
            gChar = 0x7a;   // z //
            tChar = 0x53;   // S //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *z#S      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Narrow Bar Width (# dots)"));
            iChar = 0x2a;   // * //
            gChar = 0x7a;   // z //
            tChar = 0x54;   // T //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *z#T      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Wide Space Width (# dots)"));
            iChar = 0x2a;   // * //
            gChar = 0x7a;   // z //
            tChar = 0x55;   // U //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *z#U      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Narrow Space Width (# dots)"));
            iChar = 0x2a;   // * //
            gChar = 0x7a;   // z //
            tChar = 0x56;   // V //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueGeneric;                               // *z#V      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection (# = discrete value)"));
            value = 0;                                           // *z0V      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection: Code 3 of 9"));
            value = 1;                                           // *z1V      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection: Industrial 2 of 5"));
            value = 2;                                           // *z2V      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection: Matrix 2 of 5"));
            value = 3;                                           // *z3V      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection: User-Defined"));
            value = 4;                                           // *z4V      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection: 2 of 5 Interleaved"));
            value = 5;                                           // *z5V      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection: Codabar"));
            value = 6;                                           // *z6V      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection: MSI/Plessey"));
            value = 8;                                           // *z8V      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection: UPC A"));
            value = 9;                                           // *z9V      //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection: UPC E"));
            value = 10;                                          // *z10V     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection: EAN 8"));
            value = 11;                                          // *z11V     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection: EAN 13"));
            value = 12;                                          // *z12V     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection: Code 128"));
            value = 13;                                          // *z13V     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection: Code 93"));
            value = 14;                                          // *z14V     //
            _seqs.Add(root + ":" + value.ToString("X4"),
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Selection: Extended Code 3 of 9"));

            iChar = 0x2a;   // * //
            gChar = 0x7a;   // z //
            tChar = 0x58;   // X //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *z#X      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.None,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Label X Offset (# dots)"));

            iChar = 0x2a;   // * //
            gChar = 0x7a;   // z //
            tChar = 0x5a;   // Z //
            root = iChar.ToString("X2") +
                   gChar.ToString("X2") +
                   tChar.ToString("X2");
            value = _valueDummy;                                 // *z#Z      //
            _seqs.Add(root,
                 new PCLComplexSeq(iChar, gChar, tChar, value, flagNone,
                                   flagNone, flagNone, flagNone,
                                   flagObsolete, flagNone, flagNone,
                                   PrnParseConstants.ActPCL.None,
                                   PrnParseConstants.OvlAct.PageMark,
                                   PrnParseConstants.SeqGrp.Unknown,
                                   "Bar Code Label"));

            _seqsCount = _seqs.Count;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        //  p o p u l a t e T a b l e A d d F o n t s                         //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Add any 'preset' fonts to the table of escape sequences.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PopulateTableAddFonts(byte iChar, byte gChar, byte tChar, string root)
        {
            const bool flagNone = false;
            const bool flagDiscrete = true;

            int ctFonts = PCLFonts.GetCount();

            if (ctFonts > 0)
            {
                string type;
                string fontName = string.Empty;

                int value;
                ushort typeface = 0;
                if (iChar == 0x28)
                    type = "Primary";
                else
                    type = "Secondary";

                for (int i = 0; i < ctFonts; i++)
                {
                    bool presetFont = PCLFonts.GetPresetFontData(i, ref typeface, ref fontName);
                    if (presetFont)
                    {
                        value = typeface;

                        _seqs.Add(root + ":" + value.ToString("X4"),
                             new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                               flagNone, flagNone, flagNone,
                                               flagNone, flagNone, flagNone,
                                               PrnParseConstants.ActPCL.None,
                                               PrnParseConstants.OvlAct.None,
                                               PrnParseConstants.SeqGrp.FontSelection,
                                               type + " Font: Typeface (" +
                                               value + " = " + fontName + ")"));
                    }
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        //  p o p u l a t e T a b l e A d d P a p e r S i z e s               //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Add any paper sizes which have a known PCL identifier to the table //
        // of escape sequences.                                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PopulateTableAddPaperSizes(byte iChar, byte gChar, byte tChar, string root)
        {
            const bool flagNone = false;
            const bool flagDiscrete = true;

            int count = PCLPaperSizes.GetCount();

            if (count > 0)
            {
                int value;

                for (int i = 0; i < count; i++)
                {
                    if (!PCLPaperSizes.IsIdUnknownPCL(i))
                    {
                        value = PCLPaperSizes.GetIdPCL(i);

                        _seqs.Add(root + ":" + value.ToString("X4"),
                             new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                                flagNone, flagNone, flagNone,
                                                flagNone, flagNone, flagNone,
                                                PrnParseConstants.ActPCL.None,
                                                PrnParseConstants.OvlAct.PageChange,
                                                PrnParseConstants.SeqGrp.PageControl,
                                                "Page Size: " +
                                                PCLPaperSizes.GetNameAndDesc(i)));
                    }
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        //  p o p u l a t e T a b l e A d d S y m s e t s                     //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Add any symbol sets which match the specified 'id Alpha' value to  //
        // the table of escape sequences.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PopulateTableAddSymsets(byte iChar, byte gChar, byte tChar, string root)
        {
            const bool flagNone = false;
            const bool flagDiscrete = true;
            const bool flagNilGChar = true;

            int ctSymsets = PCLSymbolSets.GetCount();

            if (ctSymsets > 0)
            {
                int value;
                ushort kind1 = 0;
                ushort idNum = 0;

                string name = string.Empty;
                for (int i = 0; i < ctSymsets; i++)
                {
                    bool matchFound = PCLSymbolSets.GetSymsetDataForIdAlpha(i, tChar, ref kind1, ref idNum, ref name);
                    if (matchFound)
                    {
                        string type;

                        value = idNum;

                        if (iChar == 0x28)
                            type = "Primary";
                        else
                            type = "Secondary";

                        string id = PCLSymbolSets.TranslateKind1ToId(kind1);
                        _seqs.Add(root + ":" + value.ToString("X4"),
                             new PCLComplexSeq(iChar, gChar, tChar, value, flagDiscrete,
                                               flagNilGChar, flagNone, flagNone,
                                               flagNone, flagNone, flagNone,
                                               PrnParseConstants.ActPCL.None,
                                               PrnParseConstants.OvlAct.None,
                                               PrnParseConstants.SeqGrp.FontSelection,
                                               type + " Font: Symbol Set (" +
                                               id + " = " + name + ")"));
                    }
                }
            }
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
            PCLComplexSeq seq;

            _seqUnknown.ResetStatistics();

            foreach (KeyValuePair<string, PCLComplexSeq> kvp in _seqs)
            {
                seq = kvp.Value;

                seq.ResetStatistics();
            }
        }
    }
}