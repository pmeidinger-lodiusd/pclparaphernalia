using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides details of PCL XL Operator tags.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class PCLXLOperators
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum eEmbedDataType : byte
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

        public enum eTag : byte
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

        private static SortedList<byte, PCLXLOperator> _tags =
            new SortedList<byte, PCLXLOperator>();

        private static PCLXLOperator _tagUnknown;

        private static int _tagCount;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L O p e r a t o r s                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PCLXLOperators()
        {
            populateTable();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k T a g                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Searches the PCL XL Operator tag table for a matching entry.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool checkTag(
            byte tagToCheck,
            ref bool flagEndSession,
            ref bool flagReserved,
            ref eEmbedDataType embedDataType,
            ref PrnParseConstants.eOvlAct makeOvlAct,
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

            tag.getDetails(ref flagEndSession,
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

        public static void displayStatsCounts(DataTable table,
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
                    displayStatsCountsHddr(table);
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
                        displaySeq = false;
                    else if ((excUnusedResTags) &&
                             (kvp.Value.FlagReserved == true))
                        displaySeq = false;
                }

                if (displaySeq)
                {
                    if (!hddrWritten)
                    {
                        displayStatsCountsHddr(table);
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

        public static void displayStatsCountsHddr(DataTable table)
        {
            DataRow row;

            //----------------------------------------------------------------//

            row = table.NewRow();

            row[0] = "";
            row[1] = "_____________________";
            row[2] = "";
            row[3] = "";
            row[4] = "";

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = "";
            row[1] = "PCL XL Operator tags:";
            row[2] = "";
            row[3] = "";
            row[4] = "";

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = "";
            row[1] = "¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
            row[2] = "";
            row[3] = "";
            row[4] = "";

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

        public static int displayTags(DataGrid grid,
                                        bool incResTags)
        {
            int count = 0;

            bool tagReserved;

            foreach (KeyValuePair<byte, PCLXLOperator> kvp in _tags)
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
        // g e t D e s c                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return description for specified Operator tag.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string getDesc(byte tag)
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

        public static void incrementStatsCount(byte tagByte,
                                                int level)
        {
            PCLXLOperator tag;

            if (_tags.IndexOfKey(tagByte) != -1)
                tag = _tags[tagByte];
            else
                tag = _tagUnknown;

            tag.incrementStatisticsCount(level);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p o p u l a t e T a b l e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Populate the table of Operator tags.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void populateTable()
        {
            const bool flagNone = false;
            const bool flagReserved = true;
            const bool flagEndSession = true;

            byte tag;

            tag = 0x20;                                              // ?    //
            _tagUnknown =
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "*** Unknown tag ***");

            tag = (byte)eTag.BeginSession;                          // 0x41 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.Replace_0x77,
                                     "BeginSession"));

            tag = (byte)eTag.EndSession;                            // 0x42 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagEndSession, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.Remove,
                                     "EndSession"));

            tag = (byte)eTag.BeginPage;                             // 0x43 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.PageBegin,
                                     "BeginPage"));

            tag = (byte)eTag.EndPage;                               // 0x44 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.PageEnd,
                                     "EndPage"));

            tag = 0x45;                                               // 0x45 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = (byte)eTag.VendorUnique;                          // 0x46 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "VendorUnique"));

            tag = (byte)eTag.Comment;                               // 0x47 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "Comment"));

            tag = (byte)eTag.OpenDataSource;                        // 0x48 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.Remove,
                                     "OpenDataSource"));

            tag = (byte)eTag.CloseDataSource;                       // 0x49 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.Remove,
                                     "CloseDataSource"));

            tag = (byte)eTag.EchoComment;                           // 0x4a //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "EchoComment"));

            tag = (byte)eTag.Query;                                 // 0x4b //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "Query"));

            tag = 0x4c;                                               // 0x4c //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0x4d;                                               // 0x4d //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0x4e;                                               // 0x4e //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = (byte)eTag.BeginFontHeader;                       // 0x4f //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "BeginFontHeader"));

            tag = (byte)eTag.ReadFontHeader;                        // 0x50 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.FontHeader,
                                     PrnParseConstants.eOvlAct.None,
                                     "ReadFontHeader"));

            tag = (byte)eTag.EndFontHeader;                         // 0x51 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "EndFontHeader"));

            tag = (byte)eTag.BeginChar;                             // 0x52 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "BeginChar"));

            tag = (byte)eTag.ReadChar;                              // 0x53 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.FontChar,
                                     PrnParseConstants.eOvlAct.None,
                                     "ReadChar"));

            tag = (byte)eTag.EndChar;                               // 0x54 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "EndChar"));

            tag = (byte)eTag.RemoveFont;                            // 0x55 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "RemoveFont"));

            tag = (byte)eTag.SetCharAttributes;                     // 0x56 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetCharAttributes"));

            tag = (byte)eTag.SetDefaultGS;                          // 0x57 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetDefaultGS"));

            tag = (byte)eTag.SetColorTreatment;                     // 0x58 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetColorTreatment"));

            tag = 0x59;                                               // 0x59 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0x5a;                                               // 0x5a //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = (byte)eTag.BeginStream;                           // 0x5b //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.Illegal,
                                     "BeginStream"));

            tag = (byte)eTag.ReadStream;                            // 0x5c //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.Stream,
                                     PrnParseConstants.eOvlAct.Illegal,
                                     "ReadStream"));

            tag = (byte)eTag.EndStream;                             // 0x5d //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.Illegal,
                                     "EndStream"));

            tag = (byte)eTag.ExecStream;                            // 0x5e //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "ExecStream"));

            tag = (byte)eTag.RemoveStream;                          // 0x5f //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "RemoveStream"));

            tag = (byte)eTag.PopGS;                                 // 0x60 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "PopGS"));

            tag = (byte)eTag.PushGS;                                // 0x61 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "PushGS"));

            tag = (byte)eTag.SetClipReplace;                        // 0x62 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetClipReplace"));

            tag = (byte)eTag.SetBrushSource;                        // 0x63 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetBrushSource"));

            tag = (byte)eTag.SetCharAngle;                          // 0x64 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetCharAngle"));

            tag = (byte)eTag.SetCharScale;                          // 0x65 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetCharScale"));

            tag = (byte)eTag.SetCharShear;                          // 0x66 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetCharShear"));

            tag = (byte)eTag.SetClipIntersect;                      // 0x67 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetClipIntersect"));

            tag = (byte)eTag.SetClipRectangle;                      // 0x68 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetClipRectangle"));

            tag = (byte)eTag.SetClipToPage;                         // 0x69 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetClipToPage"));

            tag = (byte)eTag.SetColorSpace;                         // 0x6a //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetColorSpace"));

            tag = (byte)eTag.SetCursor;                             // 0x6b //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetCursor"));

            tag = (byte)eTag.SetCursorRel;                          // 0x6c //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetCursorRel"));

            tag = (byte)eTag.SetHalftoneMethod;                     // 0x6d //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.DitherMatrix,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetHalftoneMethod"));

            tag = (byte)eTag.SetFillMode;                           // 0x6e //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetFillMode"));

            tag = (byte)eTag.SetFont;                               // 0x6f //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetFont"));

            tag = (byte)eTag.SetLineDash;                           // 0x70 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetLineDash"));

            tag = (byte)eTag.SetLineCap;                            // 0x71 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetLineCap"));

            tag = (byte)eTag.SetLineJoin;                           // 0x72 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetLineJoin"));

            tag = (byte)eTag.SetMiterLimit;                         // 0x73 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetMiterLimit"));

            tag = (byte)eTag.SetPageDefaultCTM;                     // 0x74 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.Remove,
                                     "SetPageDefaultCTM"));

            tag = (byte)eTag.SetPageOrigin;                         // 0x75 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetPageOrigin"));

            tag = (byte)eTag.SetPageRotation;                       // 0x76 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetPageRotation"));

            tag = (byte)eTag.SetPageScale;                          // 0x77 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetPageScale"));

            tag = (byte)eTag.SetPatternTxMode;                      // 0x78 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetPatternTxMode"));

            tag = (byte)eTag.SetPenSource;                          // 0x79 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetPenSource"));

            tag = (byte)eTag.SetPenWidth;                           // 0x7a //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetPenWidth"));

            tag = (byte)eTag.SetROP;                                // 0x7b //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetROP"));

            tag = (byte)eTag.SetSourceTxMode;                       // 0x7c //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetSourceTxMode"));

            tag = (byte)eTag.SetCharBoldValue;                      // 0x7d //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetCharBoldValue"));

            tag = (byte)eTag.SetNeutralAxis;                        // 0x7e //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetNeutralAxis"));

            tag = (byte)eTag.SetClipMode;                           // 0x7f //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetClipMode"));

            tag = (byte)eTag.SetPathToClip;                         // 0x80 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetPathToClip"));

            tag = (byte)eTag.SetCharSubMode;                        // 0x81 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetCharSubMode"));

            tag = 0x82;                                               // 0x82 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0x83;                                               // 0x83 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = (byte)eTag.CloseSubPath;                          // 0x84 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "CloseSubPath"));

            tag = (byte)eTag.NewPath;                               // 0x85 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "NewPath"));

            tag = (byte)eTag.PaintPath;                             // 0x86 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "PaintPath"));

            tag = 0x87;                                               // 0x87 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0x88;                                               // 0x88 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0x89;                                               // 0x89 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0x8a;                                               // 0x8a //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0x8b;                                               // 0x8b //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0x8c;                                               // 0x8c //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0x8d;                                               // 0x8d //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0x8e;                                               // 0x8e //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0x8f;                                               // 0x8f //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0x90;                                               // 0x90 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = (byte)eTag.ArcPath;                               // 0x91 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "ArcPath"));

            tag = (byte)eTag.SetColorTrapping;                      // 0x92 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetColorTrapping"));

            tag = (byte)eTag.BezierPath;                            // 0x93 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.Points,
                                     PrnParseConstants.eOvlAct.None,
                                     "BezierPath"));

            tag = (byte)eTag.SetAdaptiveHalftoning;                 // 0x94 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SetAdaptiveHalftoning"));

            tag = (byte)eTag.BezierRelPath;                         // 0x95 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.Points,
                                     PrnParseConstants.eOvlAct.None,
                                     "BezierRelPath"));

            tag = (byte)eTag.Chord;                                 // 0x96 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "Chord"));

            tag = (byte)eTag.ChordPath;                             // 0x97 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "ChordPath"));

            tag = (byte)eTag.Ellipse;                               // 0x98 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "Ellipse"));

            tag = (byte)eTag.EllipsePath;                           // 0x99 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "EllipsePath"));

            tag = 0x9a;                                               // 0x9a //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = (byte)eTag.LinePath;                              // 0x9b //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.Points,
                                     PrnParseConstants.eOvlAct.None,
                                     "LinePath"));

            tag = 0x9c;                                               // 0x9c //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = (byte)eTag.LineRelPath;                           // 0x9d //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.Points,
                                     PrnParseConstants.eOvlAct.None,
                                     "LineRelPath"));

            tag = (byte)eTag.Pie;                                   // 0x9e //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "Pie"));

            tag = (byte)eTag.PiePath;                               // 0x9f //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "PiePath"));

            tag = (byte)eTag.Rectangle;                             // 0xa0 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "Rectangle"));

            tag = (byte)eTag.RectanglePath;                         // 0xa1 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "RectanglePath"));

            tag = (byte)eTag.RoundRectangle;                        // 0xa2 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "RoundRectangle"));

            tag = (byte)eTag.RoundRectanglePath;                    // 0xa3 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "RoundRectanglePath"));

            tag = 0xa4;                                               // 0xa4 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0xa5;                                               // 0xa5 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0xa6;                                               // 0xa6 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0xa7;                                               // 0xa7 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = (byte)eTag.Text;                                  // 0xa8 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "Text"));

            tag = (byte)eTag.TextPath;                              // 0xa9 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "TextPath"));

            tag = (byte)eTag.SystemText;                            // 0xaa //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "SystemText"));

            tag = 0xab;                                               // 0xab //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0xac;                                               // 0xac //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0xad;                                               // 0xad //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0xae;                                               // 0xae //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = 0xaf;                                               // 0xaf //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = (byte)eTag.BeginImage;                            // 0xb0 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "BeginImage"));

            tag = (byte)eTag.ReadImage;                             // 0xb1 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.Image,
                                     PrnParseConstants.eOvlAct.None,
                                     "ReadImage"));

            tag = (byte)eTag.EndImage;                              // 0xb2 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "EndImage"));

            tag = (byte)eTag.BeginRastPattern;                      // 0xb3 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "BeginRastPattern"));

            tag = (byte)eTag.ReadRastPattern;                       // 0xb4 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.RasterPattern,
                                     PrnParseConstants.eOvlAct.None,
                                     "ReadRastPattern"));

            tag = (byte)eTag.EndRastPattern;                        // 0xb5 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "EndRastPattern"));

            tag = (byte)eTag.BeginScan;                             // 0xb6 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.Scan,
                                     PrnParseConstants.eOvlAct.None,
                                     "BeginScan"));

            tag = 0xb7;                                               // 0xb7 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagReserved,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "* Reserved *"));

            tag = (byte)eTag.EndScan;                               // 0xb8 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "EndScan"));

            tag = (byte)eTag.ScanLineRel;                           // 0xb9 //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.None,
                                     PrnParseConstants.eOvlAct.None,
                                     "ScanLineRel"));

            tag = (byte)eTag.PassThrough;                           // 0xbf //
            _tags.Add(tag,
                new PCLXLOperator(tag, flagNone, flagNone,
                                     eEmbedDataType.PassThrough,
                                     PrnParseConstants.eOvlAct.None,
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

        public static void resetStatsCounts()
        {
            PCLXLOperator tag;

            _tagUnknown.resetStatistics();

            foreach (KeyValuePair<byte, PCLXLOperator> kvp in _tags)
            {
                tag = kvp.Value;

                tag.resetStatistics();
            }
        }
    }
}
