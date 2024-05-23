using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class provides details of PCL XL Operator tags.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    static class PCLXLOperators
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum EmbedDataType : byte
        {
            None = 0,
            Stream,
            PassThrough,
            FontHeader,
            FontChar,
            DitherMatrix,
            Points,
            Image,
            RasterPattern,
            Scan
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // PCLXL Operator tags.                                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum Tag : byte
        {
            ArcPath = 0x91,
            BeginChar = 0x52,
            BeginFontHeader = 0x4f,
            BeginImage = 0xb0,
            BeginPage = 0x43,
            BeginRastPattern = 0xb3,
            BeginScan = 0xb6,
            BeginSession = 0x41,
            BeginStream = 0x5b,
            BezierPath = 0x93,
            BezierRelPath = 0x95,
            Chord = 0x96,
            ChordPath = 0x97,
            CloseDataSource = 0x49,
            CloseSubPath = 0x84,
            Comment = 0x47,
            EchoComment = 0x4a,
            Ellipse = 0x98,
            EllipsePath = 0x99,
            EndChar = 0x54,
            EndFontHeader = 0x51,
            EndImage = 0xb2,
            EndPage = 0x44,
            EndRastPattern = 0xb5,
            EndScan = 0xb8,
            EndSession = 0x42,
            EndStream = 0x5d,
            ExecStream = 0x5e,
            LinePath = 0x9b,
            LineRelPath = 0x9d,
            NewPath = 0x85,
            OpenDataSource = 0x48,
            PaintPath = 0x86,
            PassThrough = 0xbf,
            Pie = 0x9e,
            PiePath = 0x9f,
            PopGS = 0x60,
            PushGS = 0x61,
            Query = 0x4b,
            ReadChar = 0x53,
            ReadFontHeader = 0x50,
            ReadImage = 0xb1,
            ReadRastPattern = 0xb4,
            ReadStream = 0x5c,
            Rectangle = 0xa0,
            RectanglePath = 0xa1,
            RemoveFont = 0x55,
            RemoveStream = 0x5f,
            RoundRectangle = 0xa2,
            RoundRectanglePath = 0xa3,
            ScanLineRel = 0xb9,
            SetAdaptiveHalftoning = 0x94,
            SetBrushSource = 0x63,
            SetCharAngle = 0x64,
            SetCharAttributes = 0x56,
            SetCharBoldValue = 0x7d,
            SetCharScale = 0x65,
            SetCharShear = 0x66,
            SetCharSubMode = 0x81,
            SetClipIntersect = 0x67,
            SetClipMode = 0x7f,
            SetClipRectangle = 0x68,
            SetClipReplace = 0x62,
            SetClipToPage = 0x69,
            SetColorSpace = 0x6a,
            SetColorTrapping = 0x92,
            SetColorTreatment = 0x58,
            SetCursor = 0x6b,
            SetCursorRel = 0x6c,
            SetDefaultGS = 0x57,
            SetFillMode = 0x6e,
            SetFont = 0x6f,
            SetHalftoneMethod = 0x6d,
            SetLineCap = 0x71,
            SetLineDash = 0x70,
            SetLineJoin = 0x72,
            SetMiterLimit = 0x73,
            SetNeutralAxis = 0x7e,
            SetPageDefaultCTM = 0x74,
            SetPageOrigin = 0x75,
            SetPageRotation = 0x76,
            SetPageScale = 0x77,
            SetPathToClip = 0x80,
            SetPatternTxMode = 0x78,
            SetPenSource = 0x79,
            SetPenWidth = 0x7a,
            SetROP = 0x7b,
            SetSourceTxMode = 0x7c,
            SystemText = 0xaa,
            Text = 0xa8,
            TextPath = 0xa9,
            VendorUnique = 0x46
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly SortedList<byte, PCLXLOperator> _tags = new SortedList<byte, PCLXLOperator>();

        private static PCLXLOperator _tagUnknown;

        private static int _tagCount;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L O p e r a t o r s                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PCLXLOperators()
        {
            PopulateTable();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k T a g                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Searches the PCL XL Operator tag table for a matching entry.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool CheckTag(byte tagToCheck,
                                    ref bool flagEndSession,
                                    ref bool flagReserved,
                                    ref EmbedDataType embedDataType,
                                    ref PrnParseConstants.OvlAct makeOvlAct,
                                    ref string description)
        {
            bool seqKnown;

            PCLXLOperator tag;

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

            tag.GetDetails(ref flagEndSession,
                            ref flagReserved,
                            ref embedDataType,
                            ref makeOvlAct,
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
                row[1] = _tagUnknown.Description;
                row[2] = _tagUnknown.StatsCtParent;
                row[3] = _tagUnknown.StatsCtChild;
                row[4] = _tagUnknown.StatsCtTotal;

                table.Rows.Add(row);
            }

            //----------------------------------------------------------------//

            foreach (KeyValuePair<byte, PCLXLOperator> kvp in _tags)
            {
                displaySeq = true;

                count = kvp.Value.StatsCtTotal;

                if (count == 0)
                {
                    if (incUsedSeqsOnly)
                    {
                        displaySeq = false;
                    }
                    else if (excUnusedResTags &&
                                                 (kvp.Value.FlagReserved))
                    {
                        displaySeq = false;
                    }
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
            row[1] = "PCL XL Operator tags:";
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
        // d i s p l a y T a g s                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display list of Operator tags.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int DisplayTags(DataGrid grid, bool incResTags)
        {
            int count = 0;

            bool tagReserved;

            foreach (KeyValuePair<byte, PCLXLOperator> kvp in _tags)
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
        // g e t D e s c                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return description for specified Operator tag.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetDesc(byte tag)
        {
            return _tags[tag].Description;
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
            PCLXLOperator tag;

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
        // Populate the table of Operator tags.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PopulateTable()
        {
            const bool flagNone = false;
            const bool flagReserved = true;
            const bool flagEndSession = true;

            byte tag = 0x20;                                              // ?    //
            _tagUnknown =
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "*** Unknown tag ***");

            tag = (byte)Tag.BeginSession;                          // 0x41 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.Replace_0x77,
                                     "BeginSession"));

            tag = (byte)Tag.EndSession;                            // 0x42 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagEndSession, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.Remove,
                                     "EndSession"));

            tag = (byte)Tag.BeginPage;                             // 0x43 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.PageBegin,
                                     "BeginPage"));

            tag = (byte)Tag.EndPage;                               // 0x44 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.PageEnd,
                                     "EndPage"));

            tag = 0x45;                                               // 0x45 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = (byte)Tag.VendorUnique;                          // 0x46 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "VendorUnique"));

            tag = (byte)Tag.Comment;                               // 0x47 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "Comment"));

            tag = (byte)Tag.OpenDataSource;                        // 0x48 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.Remove,
                                     "OpenDataSource"));

            tag = (byte)Tag.CloseDataSource;                       // 0x49 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.Remove,
                                     "CloseDataSource"));

            tag = (byte)Tag.EchoComment;                           // 0x4a //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "EchoComment"));

            tag = (byte)Tag.Query;                                 // 0x4b //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "Query"));

            tag = 0x4c;                                               // 0x4c //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0x4d;                                               // 0x4d //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0x4e;                                               // 0x4e //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = (byte)Tag.BeginFontHeader;                       // 0x4f //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "BeginFontHeader"));

            tag = (byte)Tag.ReadFontHeader;                        // 0x50 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.FontHeader,
                                     PrnParseConstants.OvlAct.None,
                                     "ReadFontHeader"));

            tag = (byte)Tag.EndFontHeader;                         // 0x51 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "EndFontHeader"));

            tag = (byte)Tag.BeginChar;                             // 0x52 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "BeginChar"));

            tag = (byte)Tag.ReadChar;                              // 0x53 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.FontChar,
                                     PrnParseConstants.OvlAct.None,
                                     "ReadChar"));

            tag = (byte)Tag.EndChar;                               // 0x54 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "EndChar"));

            tag = (byte)Tag.RemoveFont;                            // 0x55 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "RemoveFont"));

            tag = (byte)Tag.SetCharAttributes;                     // 0x56 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetCharAttributes"));

            tag = (byte)Tag.SetDefaultGS;                          // 0x57 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetDefaultGS"));

            tag = (byte)Tag.SetColorTreatment;                     // 0x58 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetColorTreatment"));

            tag = 0x59;                                               // 0x59 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0x5a;                                               // 0x5a //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = (byte)Tag.BeginStream;                           // 0x5b //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.Illegal,
                                     "BeginStream"));

            tag = (byte)Tag.ReadStream;                            // 0x5c //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.Stream,
                                     PrnParseConstants.OvlAct.Illegal,
                                     "ReadStream"));

            tag = (byte)Tag.EndStream;                             // 0x5d //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.Illegal,
                                     "EndStream"));

            tag = (byte)Tag.ExecStream;                            // 0x5e //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "ExecStream"));

            tag = (byte)Tag.RemoveStream;                          // 0x5f //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "RemoveStream"));

            tag = (byte)Tag.PopGS;                                 // 0x60 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "PopGS"));

            tag = (byte)Tag.PushGS;                                // 0x61 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "PushGS"));

            tag = (byte)Tag.SetClipReplace;                        // 0x62 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetClipReplace"));

            tag = (byte)Tag.SetBrushSource;                        // 0x63 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetBrushSource"));

            tag = (byte)Tag.SetCharAngle;                          // 0x64 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetCharAngle"));

            tag = (byte)Tag.SetCharScale;                          // 0x65 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetCharScale"));

            tag = (byte)Tag.SetCharShear;                          // 0x66 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetCharShear"));

            tag = (byte)Tag.SetClipIntersect;                      // 0x67 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetClipIntersect"));

            tag = (byte)Tag.SetClipRectangle;                      // 0x68 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetClipRectangle"));

            tag = (byte)Tag.SetClipToPage;                         // 0x69 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetClipToPage"));

            tag = (byte)Tag.SetColorSpace;                         // 0x6a //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetColorSpace"));

            tag = (byte)Tag.SetCursor;                             // 0x6b //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetCursor"));

            tag = (byte)Tag.SetCursorRel;                          // 0x6c //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetCursorRel"));

            tag = (byte)Tag.SetHalftoneMethod;                     // 0x6d //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.DitherMatrix,
                                     PrnParseConstants.OvlAct.None,
                                     "SetHalftoneMethod"));

            tag = (byte)Tag.SetFillMode;                           // 0x6e //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetFillMode"));

            tag = (byte)Tag.SetFont;                               // 0x6f //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetFont"));

            tag = (byte)Tag.SetLineDash;                           // 0x70 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetLineDash"));

            tag = (byte)Tag.SetLineCap;                            // 0x71 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetLineCap"));

            tag = (byte)Tag.SetLineJoin;                           // 0x72 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetLineJoin"));

            tag = (byte)Tag.SetMiterLimit;                         // 0x73 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetMiterLimit"));

            tag = (byte)Tag.SetPageDefaultCTM;                     // 0x74 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.Remove,
                                     "SetPageDefaultCTM"));

            tag = (byte)Tag.SetPageOrigin;                         // 0x75 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetPageOrigin"));

            tag = (byte)Tag.SetPageRotation;                       // 0x76 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetPageRotation"));

            tag = (byte)Tag.SetPageScale;                          // 0x77 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetPageScale"));

            tag = (byte)Tag.SetPatternTxMode;                      // 0x78 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetPatternTxMode"));

            tag = (byte)Tag.SetPenSource;                          // 0x79 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetPenSource"));

            tag = (byte)Tag.SetPenWidth;                           // 0x7a //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetPenWidth"));

            tag = (byte)Tag.SetROP;                                // 0x7b //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetROP"));

            tag = (byte)Tag.SetSourceTxMode;                       // 0x7c //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetSourceTxMode"));

            tag = (byte)Tag.SetCharBoldValue;                      // 0x7d //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetCharBoldValue"));

            tag = (byte)Tag.SetNeutralAxis;                        // 0x7e //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetNeutralAxis"));

            tag = (byte)Tag.SetClipMode;                           // 0x7f //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetClipMode"));

            tag = (byte)Tag.SetPathToClip;                         // 0x80 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetPathToClip"));

            tag = (byte)Tag.SetCharSubMode;                        // 0x81 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetCharSubMode"));

            tag = 0x82;                                               // 0x82 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0x83;                                               // 0x83 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = (byte)Tag.CloseSubPath;                          // 0x84 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "CloseSubPath"));

            tag = (byte)Tag.NewPath;                               // 0x85 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "NewPath"));

            tag = (byte)Tag.PaintPath;                             // 0x86 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "PaintPath"));

            tag = 0x87;                                               // 0x87 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0x88;                                               // 0x88 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0x89;                                               // 0x89 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0x8a;                                               // 0x8a //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0x8b;                                               // 0x8b //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0x8c;                                               // 0x8c //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0x8d;                                               // 0x8d //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0x8e;                                               // 0x8e //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0x8f;                                               // 0x8f //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0x90;                                               // 0x90 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = (byte)Tag.ArcPath;                               // 0x91 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "ArcPath"));

            tag = (byte)Tag.SetColorTrapping;                      // 0x92 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetColorTrapping"));

            tag = (byte)Tag.BezierPath;                            // 0x93 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.Points,
                                     PrnParseConstants.OvlAct.None,
                                     "BezierPath"));

            tag = (byte)Tag.SetAdaptiveHalftoning;                 // 0x94 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SetAdaptiveHalftoning"));

            tag = (byte)Tag.BezierRelPath;                         // 0x95 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.Points,
                                     PrnParseConstants.OvlAct.None,
                                     "BezierRelPath"));

            tag = (byte)Tag.Chord;                                 // 0x96 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "Chord"));

            tag = (byte)Tag.ChordPath;                             // 0x97 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "ChordPath"));

            tag = (byte)Tag.Ellipse;                               // 0x98 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "Ellipse"));

            tag = (byte)Tag.EllipsePath;                           // 0x99 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "EllipsePath"));

            tag = 0x9a;                                               // 0x9a //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = (byte)Tag.LinePath;                              // 0x9b //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.Points,
                                     PrnParseConstants.OvlAct.None,
                                     "LinePath"));

            tag = 0x9c;                                               // 0x9c //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = (byte)Tag.LineRelPath;                           // 0x9d //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.Points,
                                     PrnParseConstants.OvlAct.None,
                                     "LineRelPath"));

            tag = (byte)Tag.Pie;                                   // 0x9e //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "Pie"));

            tag = (byte)Tag.PiePath;                               // 0x9f //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "PiePath"));

            tag = (byte)Tag.Rectangle;                             // 0xa0 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "Rectangle"));

            tag = (byte)Tag.RectanglePath;                         // 0xa1 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "RectanglePath"));

            tag = (byte)Tag.RoundRectangle;                        // 0xa2 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "RoundRectangle"));

            tag = (byte)Tag.RoundRectanglePath;                    // 0xa3 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "RoundRectanglePath"));

            tag = 0xa4;                                               // 0xa4 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0xa5;                                               // 0xa5 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0xa6;                                               // 0xa6 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0xa7;                                               // 0xa7 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = (byte)Tag.Text;                                  // 0xa8 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "Text"));

            tag = (byte)Tag.TextPath;                              // 0xa9 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "TextPath"));

            tag = (byte)Tag.SystemText;                            // 0xaa //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "SystemText"));

            tag = 0xab;                                               // 0xab //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0xac;                                               // 0xac //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0xad;                                               // 0xad //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0xae;                                               // 0xae //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = 0xaf;                                               // 0xaf //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = (byte)Tag.BeginImage;                            // 0xb0 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "BeginImage"));

            tag = (byte)Tag.ReadImage;                             // 0xb1 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.Image,
                                     PrnParseConstants.OvlAct.None,
                                     "ReadImage"));

            tag = (byte)Tag.EndImage;                              // 0xb2 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "EndImage"));

            tag = (byte)Tag.BeginRastPattern;                      // 0xb3 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "BeginRastPattern"));

            tag = (byte)Tag.ReadRastPattern;                       // 0xb4 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.RasterPattern,
                                     PrnParseConstants.OvlAct.None,
                                     "ReadRastPattern"));

            tag = (byte)Tag.EndRastPattern;                        // 0xb5 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "EndRastPattern"));

            tag = (byte)Tag.BeginScan;                             // 0xb6 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.Scan,
                                     PrnParseConstants.OvlAct.None,
                                     "BeginScan"));

            tag = 0xb7;                                               // 0xb7 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "* Reserved *"));

            tag = (byte)Tag.EndScan;                               // 0xb8 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "EndScan"));

            tag = (byte)Tag.ScanLineRel;                           // 0xb9 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.None,
                                     PrnParseConstants.OvlAct.None,
                                     "ScanLineRel"));

            tag = (byte)Tag.PassThrough;                           // 0xbf //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     EmbedDataType.PassThrough,
                                     PrnParseConstants.OvlAct.None,
                                     "PassThrough"));

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
            PCLXLOperator tag;

            _tagUnknown.ResetStatistics();

            foreach (KeyValuePair<byte, PCLXLOperator> kvp in _tags)
            {
                tag = kvp.Value;

                tag.ResetStatistics();
            }
        }
    }
}
