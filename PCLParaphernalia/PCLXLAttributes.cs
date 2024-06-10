using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class provides details of PCL XL Attribute tags.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class PCLXLAttributes
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // PCLXL Attribute tags.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum Tag : byte
        {
            AllObjectTypes = 0x1d,
            ArcDirection = 0x41,
            BitmapCharScaling = 0xae,
            BlockByteLength = 0x6f,
            BlockHeight = 0x63,
            BoundingBox = 0x42,
            CharAngle = 0xa1,
            CharBoldValue = 0xb1,
            CharCode = 0xa2,
            CharDataSize = 0xa3,
            CharScale = 0xa4,
            CharShear = 0xa5,
            CharSize = 0xa6,
            CharSubModeArray = 0xac,
            ClipMode = 0x54,
            ClipRegion = 0x53,
            ColorDepth = 0x62,
            ColorDepthArray = 0x61,
            ColorMapping = 0x64,
            ColorSpace = 0x03,
            ColorTreatment = 0x78,
            CommentData = 0x81,
            CompressMode = 0x65,
            ContentOrientation = 0x76,
            ControlPoint1 = 0x51,
            ControlPoint2 = 0x52,
            CustomMediaSize = 0x2f,
            CustomMediaSizeUnits = 0x30,
            DashOffset = 0x43,
            DataOrg = 0x82,
            DestinationBox = 0x66,
            DestinationSize = 0x67,
            DeviceMatrix = 0x21,
            DitherMatrixDataType = 0x22,
            DitherMatrixDepth = 0x33,
            DitherMatrixSize = 0x32,
            DitherOrigin = 0x23,
            DuplexPageMode = 0x35,
            DuplexPageSide = 0x36,
            EllipseDimension = 0x44,
            EnableDiagnostics = 0xa0,
            EndPoint = 0x45,
            ErrorReport = 0x8f,
            FeedOrientation = 0x77,
            FillMode = 0x46,
            FontFormat = 0xa9,
            FontHeaderLength = 0xa7,
            FontName = 0xa8,
            GrayLevel = 0x09,
            LineCapStyle = 0x47,
            LineDashStyle = 0x4a,
            LineJoinStyle = 0x48,
            Measure = 0x86,
            MediaDestination = 0x24,
            MediaSize = 0x25,
            MediaSource = 0x26,
            MediaType = 0x27,
            MiterLength = 0x49,
            NewDestinationSize = 0x0d,
            NullBrush = 0x04,
            NullPen = 0x05,
            NumberOfPoints = 0x4d,
            NumberOfScanLines = 0x73,
            Orientation = 0x28,
            PadBytesMultiple = 0x6e,
            PageAngle = 0x29,
            PageCopies = 0x31,
            PageOrigin = 0x2a,
            PageScale = 0x2b,
            PaletteData = 0x06,
            PaletteDepth = 0x02,
            PaletteIndex = 0x07,
            PatternDefineID = 0x69,
            PatternOrigin = 0x0c,
            PatternPersistence = 0x68,
            PatternSelectID = 0x08,
            PCLSelectFont = 0x8d,
            PenWidth = 0x4b,
            Point = 0x4c,
            PointType = 0x50,
            PrimaryArray = 0x0e,
            PrimaryDepth = 0x0f,
            PrintableArea = 0x74,
            QueryKey = 0x8a,
            RasterObjects = 0x20,
            RGBColor = 0x0b,
            ROP3 = 0x2c,
            SimplexPageMode = 0x34,
            SolidLine = 0x4e,
            SourceHeight = 0x6b,
            SourceType = 0x88,
            SourceWidth = 0x6c,
            StartLine = 0x6d,
            StartPoint = 0x4f,
            StreamDataLength = 0x8c,
            StreamName = 0x8b,
            SymbolSet = 0xaa,
            TextData = 0xab,
            TextObjects = 0x1e,
            TumbleMode = 0x75,
            TxMode = 0x2d,
            UnitsPerMeasure = 0x89,
            VectorObjects = 0x1f,
            VUExtension = 0x91,
            VUDataLength = 0x92,
            VUAttr1 = 0x93,
            VUAttr2 = 0x94,
            VUAttr3 = 0x95,
            VUAttr4 = 0x96,
            VUAttr5 = 0x97,
            VUAttr6 = 0x98,
            VUAttr7 = 0x99,
            VUAttr8 = 0x9a,
            VUAttr9 = 0x9b,
            VUAttr10 = 0x9c,
            VUAttr11 = 0x9d,
            VUAttr12 = 0x9e,
            WritingMode = 0xad,
            XSpacingData = 0xaf,
            YSpacingData = 0xb0
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly SortedList<int, PCLXLAttribute> _tags = new SortedList<int, PCLXLAttribute>();

        private static PCLXLAttribute _tagUnknown;

        private static int _tagCount;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L A t t r i b u t e s                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PCLXLAttributes()
        {
            PopulateTable();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k T a g                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Searches the PCL XL Attribute tag table for a matching entry.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool CheckTag(
            int tagLen1,
            byte tagA,
            byte tagB,
            out bool flagReserved,
            out bool flagAttrEnum,
            out bool flagOperEnum,
            out bool flagUbyteTxt,
            out bool flagUintTxt,
            out bool flagValIsLen,
            out bool flagValIsPCL,
            out PrnParseConstants.OvlAct makeOvlAct,
            out string description)
        {
            bool seqKnown = false;

            PCLXLAttribute tag = _tagUnknown;

            int key = (((tagLen1 * 256) + tagA) * 256) + tagB;

            if (_tags.IndexOfKey(key) != -1)
            {
                seqKnown = true;
                tag = _tags[key];
            }

            tag.GetDetails(out flagReserved,
                           out flagAttrEnum,
                           out flagOperEnum,
                           out flagUbyteTxt,
                           out flagUintTxt,
                           out flagValIsLen,
                           out flagValIsPCL,
                           out makeOvlAct,
                           out description);

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

            foreach (KeyValuePair<int, PCLXLAttribute> kvp in _tags)
            {
                displaySeq = true;

                count = kvp.Value.StatsCtTotal;

                if (count == 0)
                {
                    if (incUsedSeqsOnly)
                    {
                        displaySeq = false;
                    }
                    else if (excUnusedResTags && (kvp.Value.FlagReserved))
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
            row[1] = "______________________";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "PCL XL Attribute tags:";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
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
        // Display list of Attribute tags.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int DisplayTags(DataGrid grid, bool incResTags)
        {
            int count = 0;

            bool tagReserved;

            foreach (KeyValuePair<int, PCLXLAttribute> kvp in _tags)
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
        // Return description for specified Attribute tag.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetDesc(byte tagA, byte tagB, int tagLen)
        {
            int key = (((tagLen * 256) + tagA) * 256) + tagB;

            return _tags[key].Description;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n c r e m e n t S t a t s C o u n t s                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Increment the relevant statistics count for the Attribute tag.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void IncrementStatsCount(int tagLen, byte tagByteA, byte tagByteB, int level)
        {
            PCLXLAttribute tag;

            int key = (((tagLen * 256) + tagByteA) * 256) + tagByteB;

            if (_tags.IndexOfKey(key) != -1)
                tag = _tags[key];
            else
                tag = _tagUnknown;

            tag.IncrementStatisticsCount(level);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p o p u l a t e T a b l e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Populate the table of Attribute tags.                              //
        //                                                                    //
        // As at Class/Revision 3.0, all tags are 1-byte; no 2-byte tags have //
        // yet been defined.                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PopulateTable()
        {
            const bool flagNone = false;
            const bool flagReserved = true;
            const bool flagAttrEnum = true;
            const bool flagOperEnum = true;
            const bool flagUbyteTxt = true;
            const bool flagUintTxt = true;
            const bool flagValIsLen = true;
            const bool flagValIsPCL = true;

            const int tagLen1 = 1;
            //   Int32 tagLen2 = 2; // no 2-byte tags yet defined

            int key;
            const byte tagB = 0x00;

            byte tagA = 0x20;
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tagUnknown =
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "*** Unknown tag ***");

            tagA = (byte)Tag.PaletteDepth;                          // 0x02 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PaletteDepth"));

            tagA = (byte)Tag.ColorSpace;                            // 0x03 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "ColorSpace"));

            tagA = (byte)Tag.NullBrush;                             // 0x04 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "NullBrush"));

            tagA = (byte)Tag.NullPen;                               // 0x05 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "NullPen"));

            tagA = (byte)Tag.PaletteData;                           // 0x06 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PaletteData"));

            tagA = (byte)Tag.PaletteIndex;                          // 0x07 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PaletteIndex"));

            tagA = (byte)Tag.PatternSelectID;                       // 0x08 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PatternSelectID"));

            tagA = (byte)Tag.GrayLevel;                             // 0x09 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "GrayLevel"));

            tagA = (byte)Tag.RGBColor;                              // 0x0b //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "RGBColor"));

            tagA = (byte)Tag.PatternOrigin;                         // 0x0c //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PatternOrigin"));

            tagA = (byte)Tag.NewDestinationSize;                    // 0x0d //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "NewDestinationSize"));

            tagA = (byte)Tag.PrimaryArray;                          // 0x0e //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PrimaryArray"));

            tagA = (byte)Tag.PrimaryDepth;                          // 0x0f //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PrimaryDepth"));

            tagA = (byte)Tag.AllObjectTypes;                        // 0x1d //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagOperEnum,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "AllObjectTypes"));

            tagA = (byte)Tag.TextObjects;                           // 0x1e //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagOperEnum,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "TextObjects"));

            tagA = (byte)Tag.VectorObjects;                         // 0x1f //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagOperEnum,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VectorObjects"));

            tagA = (byte)Tag.RasterObjects;                         // 0x20 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagOperEnum,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "RasterObjects"));

            tagA = (byte)Tag.DeviceMatrix;                          // 0x21 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "DeviceMatrix"));

            tagA = (byte)Tag.DitherMatrixDataType;                  // 0x22 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "DitherMatrixDataType"));

            tagA = (byte)Tag.DitherOrigin;                          // 0x23 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "DitherOrigin"));

            tagA = (byte)Tag.MediaDestination;                      // 0x24 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "MediaDestination"));

            tagA = (byte)Tag.MediaSize;                             // 0x25 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagUbyteTxt, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "MediaSize"));

            tagA = (byte)Tag.MediaSource;                           // 0x26 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "MediaSource"));

            tagA = (byte)Tag.MediaType;                             // 0x27 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagUbyteTxt, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "MediaType"));

            tagA = (byte)Tag.Orientation;                           // 0x28 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "Orientation"));

            tagA = (byte)Tag.PageAngle;                             // 0x29 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PageAngle"));

            tagA = (byte)Tag.PageOrigin;                            // 0x2a //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PageOrigin"));

            tagA = (byte)Tag.PageScale;                             // 0x2b //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PageScale"));

            tagA = (byte)Tag.ROP3;                                  // 0x2c //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "ROP3"));

            tagA = (byte)Tag.TxMode;                                // 0x2d //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "TxMode"));

            tagA = (byte)Tag.CustomMediaSize;                       // 0x2f //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "CustomMediaSize"));

            tagA = (byte)Tag.CustomMediaSizeUnits;                  // 0x30 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "CustomMediaSizeUnits"));

            tagA = (byte)Tag.PageCopies;                            // 0x31 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PageCopies"));

            tagA = (byte)Tag.DitherMatrixSize;                      // 0x32 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "DitherMatrixSize"));

            tagA = (byte)Tag.DitherMatrixDepth;                     // 0x33 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "DitherMatrixDepth"));

            tagA = (byte)Tag.SimplexPageMode;                       // 0x34 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "SimplexPageMode"));

            tagA = (byte)Tag.DuplexPageMode;                        // 0x35 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "DuplexPageMode"));

            tagA = (byte)Tag.DuplexPageSide;                        // 0x36 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "DuplexPageSide"));

            tagA = (byte)Tag.ArcDirection;                          // 0x41 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "ArcDirection"));

            tagA = (byte)Tag.BoundingBox;                           // 0x42 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "BoundingBox"));

            tagA = (byte)Tag.DashOffset;                            // 0x43 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "DashOffset"));

            tagA = (byte)Tag.EllipseDimension;                      // 0x44 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "EllipseDimension"));

            tagA = (byte)Tag.EndPoint;                              // 0x45 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "EndPoint"));

            tagA = (byte)Tag.FillMode;                              // 0x46 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "FillMode"));

            tagA = (byte)Tag.LineCapStyle;                          // 0x47 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "LineCapStyle"));

            tagA = (byte)Tag.LineJoinStyle;                         // 0x48 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "LineJoinStyle"));

            tagA = (byte)Tag.MiterLength;                           // 0x49 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "MiterLength"));

            tagA = (byte)Tag.LineDashStyle;                         // 0x4a //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "LineDashStyle"));

            tagA = (byte)Tag.PenWidth;                              // 0x4b //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PenWidth"));

            tagA = (byte)Tag.Point;                                 // 0x4c //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "Point"));

            tagA = (byte)Tag.NumberOfPoints;                        // 0x4d //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "NumberOfPoints"));

            tagA = (byte)Tag.SolidLine;                             // 0x4e //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "SolidLine"));

            tagA = (byte)Tag.StartPoint;                            // 0x4f //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "StartPoint"));

            tagA = (byte)Tag.PointType;                             // 0x50 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PointType"));

            tagA = (byte)Tag.ControlPoint1;                         // 0x51 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "ControlPoint1"));

            tagA = (byte)Tag.ControlPoint2;                         // 0x52 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "ControlPoint2"));

            tagA = (byte)Tag.ClipRegion;                            // 0x53 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "ClipRegion"));

            tagA = (byte)Tag.ClipMode;                              // 0x54 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "ClipMode"));

            tagA = (byte)Tag.ColorDepthArray;                       // 0x61 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "ColorDepthArray"));

            tagA = (byte)Tag.ColorDepth;                            // 0x62 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "ColorDepth"));

            tagA = (byte)Tag.BlockHeight;                           // 0x63 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "BlockHeight"));

            tagA = (byte)Tag.ColorMapping;                          // 0x64 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "ColorMapping"));

            tagA = (byte)Tag.CompressMode;                          // 0x65 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "CompressMode"));

            tagA = (byte)Tag.DestinationBox;                        // 0x66 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "DestinationBox"));

            tagA = (byte)Tag.DestinationSize;                       // 0x67 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "DestinationSize"));

            tagA = (byte)Tag.PatternPersistence;                    // 0x68 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PatternPersistence"));

            tagA = (byte)Tag.PatternDefineID;                       // 0x69 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PatternDefineID"));

            tagA = (byte)Tag.SourceHeight;                          // 0x6b //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "SourceHeight"));

            tagA = (byte)Tag.SourceWidth;                           // 0x6c //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "SourceWidth"));

            tagA = (byte)Tag.StartLine;                             // 0x6d //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "StartLine"));

            tagA = (byte)Tag.PadBytesMultiple;                      // 0x6e //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PadBytesMultiple"));

            tagA = (byte)Tag.BlockByteLength;                       // 0x6f //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "BlockByteLength"));

            tagA = (byte)Tag.NumberOfScanLines;                     // 0x73 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "NumberOfScanLines"));

            tagA = (byte)Tag.PrintableArea;                         // 0x74 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PrintableArea"));

            tagA = (byte)Tag.TumbleMode;                            // 0x75 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "TumbleMode"));

            tagA = (byte)Tag.ContentOrientation;                    // 0x76 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "ContentOrientation"));

            tagA = (byte)Tag.FeedOrientation;                       // 0x77 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "FeedOrientation"));

            tagA = (byte)Tag.ColorTreatment;                        // 0x78 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "ColorTreatment"));

            tagA = (byte)Tag.CommentData;                           // 0x81 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagUbyteTxt, flagUintTxt, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "CommentData"));

            tagA = (byte)Tag.DataOrg;                               // 0x82 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "DataOrg"));

            tagA = (byte)Tag.Measure;                               // 0x86 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.Measure,
                                   PrnParseConstants.OvlAct.None,
                                   "Measure"));

            tagA = (byte)Tag.SourceType;                            // 0x88 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "SourceType"));

            tagA = (byte)Tag.UnitsPerMeasure;                       // 0x89 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.UnitsPerMeasure,
                                   PrnParseConstants.OvlAct.None,
                                   "UnitsPerMeasure"));

            tagA = (byte)Tag.QueryKey;                              // 0x8a //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "QueryKey"));

            tagA = (byte)Tag.StreamName;                            // 0x8b //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagUbyteTxt, flagUintTxt, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "StreamName"));

            tagA = (byte)Tag.StreamDataLength;                      // 0x8c //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "StreamDataLength"));

            tagA = (byte)Tag.PCLSelectFont;                         // 0x8d //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagUbyteTxt, flagNone, flagNone, flagValIsPCL,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "PCLSelectFont"));

            tagA = (byte)Tag.ErrorReport;                           // 0x8f //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.Remove,
                                   "ErrorReport"));

            tagA = (byte)Tag.VUExtension;                           // 0x91 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VUExtension"));

            tagA = (byte)Tag.VUDataLength;                          // 0x92 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagValIsLen, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VUDataLength"));

            tagA = (byte)Tag.VUAttr1;                               // 0x93 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VUAttr1"));

            tagA = (byte)Tag.VUAttr2;                               // 0x94 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VUAttr2"));

            tagA = (byte)Tag.VUAttr3;                               // 0x95 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VUAttr3"));

            tagA = (byte)Tag.VUAttr4;                               // 0x96 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VUAttr4"));

            tagA = (byte)Tag.VUAttr5;                               // 0x97 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VUAttr5"));

            tagA = (byte)Tag.VUAttr6;                               // 0x98 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VUAttr6"));

            tagA = (byte)Tag.VUAttr7;                               // 0x99 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VUAttr7"));

            tagA = (byte)Tag.VUAttr8;                               // 0x9a //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VUAttr8"));

            tagA = (byte)Tag.VUAttr9;                               // 0x9b //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VUAttr9"));

            tagA = (byte)Tag.VUAttr10;                              // 0x9c //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VUAttr10"));

            tagA = (byte)Tag.VUAttr11;                              // 0x9d //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VUAttr11"));

            tagA = (byte)Tag.VUAttr12;                              // 0x9e //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "VUAttr12"));

            tagA = 0x9f;                                               // 0x9f //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagReserved, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "* Reserved *"));

            tagA = (byte)Tag.EnableDiagnostics;                     // 0xa0 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "EnableDiagnostics"));

            tagA = (byte)Tag.CharAngle;                             // 0xa1 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "CharAngle"));

            tagA = (byte)Tag.CharCode;                              // 0xa2 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "CharCode"));

            tagA = (byte)Tag.CharDataSize;                          // 0xa3 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "CharDataSize"));

            tagA = (byte)Tag.CharScale;                             // 0xa4 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "CharScale"));

            tagA = (byte)Tag.CharShear;                             // 0xa5 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "CharShear"));

            tagA = (byte)Tag.CharSize;                              // 0xa6 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.CharSize,
                                   PrnParseConstants.OvlAct.None,
                                   "CharSize"));

            tagA = (byte)Tag.FontHeaderLength;                      // 0xa7 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "FontHeaderLength"));

            tagA = (byte)Tag.FontName;                              // 0xa8 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagUbyteTxt, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "FontName"));

            tagA = (byte)Tag.FontFormat;                            // 0xa9 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "FontFormat"));

            tagA = (byte)Tag.SymbolSet;                             // 0xaa //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "SymbolSet"));

            tagA = (byte)Tag.TextData;                              // 0xab //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagUbyteTxt, flagUintTxt, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "TextData"));

            tagA = (byte)Tag.CharSubModeArray;                      // 0xac //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "CharSubModeArray"));

            tagA = (byte)Tag.WritingMode;                           // 0xad //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagAttrEnum, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "WritingMode"));

            tagA = (byte)Tag.BitmapCharScaling;                     // 0xae //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "BitmapCharScaling"));

            tagA = (byte)Tag.XSpacingData;                          // 0xaf //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "XSpacingData"));

            tagA = (byte)Tag.YSpacingData;                          // 0xb0 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "YSpacingData"));

            tagA = (byte)Tag.CharBoldValue;                         // 0xb1 //
            key = (((tagLen1 * 256) + tagA) * 256) + tagB;
            _tags.Add(key,
                new PCLXLAttribute(tagLen1, tagA, tagB,
                                   flagNone, flagNone, flagNone,
                                   flagNone, flagNone, flagNone, flagNone,
                                   PrnParseConstants.ActPCLXL.None,
                                   PrnParseConstants.OvlAct.None,
                                   "CharBoldValue"));

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
            PCLXLAttribute tag;

            _tagUnknown.ResetStatistics();

            foreach (KeyValuePair<int, PCLXLAttribute> kvp in _tags)
            {
                tag = kvp.Value;

                tag.ResetStatistics();
            }
        }
    }
}