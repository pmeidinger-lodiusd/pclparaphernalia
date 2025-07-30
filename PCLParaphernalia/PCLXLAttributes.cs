using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides details of PCL XL Attribute tags.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

static class PCLXLAttributes
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

    public enum eTag : byte
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

    private static readonly SortedList<int, PCLXLAttribute> _tags =
        new SortedList<int, PCLXLAttribute>();

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
        ref bool flagReserved,
        ref bool flagAttrEnum,
        ref bool flagOperEnum,
        ref bool flagUbyteTxt,
        ref bool flagUintTxt,
        ref bool flagValIsLen,
        ref bool flagValIsPCL,
        ref PrnParseConstants.eActPCLXL actionType,
        ref PrnParseConstants.eOvlAct makeOvlAct,
        ref string description)
    {
        bool seqKnown;

        PCLXLAttribute tag;

        int key = (((tagLen1 * 256) + tagA) * 256) + tagB;

        if (_tags.IndexOfKey(key) != -1)
        {
            seqKnown = true;
            tag = _tags[key];
        }
        else
        {
            seqKnown = false;
            tag = _tagUnknown;
        }

        tag.GetDetails(ref flagReserved,
                        ref flagAttrEnum,
                        ref flagOperEnum,
                        ref flagUbyteTxt,
                        ref flagUintTxt,
                        ref flagValIsLen,
                        ref flagValIsPCL,
                        ref actionType,
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

    public static void DisplayStatsCounts(DataTable table,
                                           bool incUsedSeqsOnly,
                                           bool excUnusedResTags)
    {
        bool displaySeq = true,
                hddrWritten = false;

        DataRow row;

        //----------------------------------------------------------------//

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
                    displaySeq = false;
                else if (excUnusedResTags && kvp.Value.FlagReserved)
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

    public static int DisplayTags(DataGrid grid,
                                    bool incResTags)
    {
        int count = 0;

        bool tagReserved;

        foreach (KeyValuePair<int, PCLXLAttribute> kvp
            in _tags)
        {
            tagReserved = kvp.Value.FlagReserved;

            if (incResTags || (!incResTags && !tagReserved))
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

    public static string GetDesc(byte tagA,
                                 byte tagB,
                                 int tagLen)
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

    public static void IncrementStatsCount(int tagLen,
                                            byte tagByteA,
                                            byte tagByteB,
                                            int level)
    {
        PCLXLAttribute tag;

        int key = (((tagLen * 256) + tagByteA) * 256) + tagByteB;

        if (_tags.IndexOfKey(key) != -1)
            tag = _tags[key];
        else
            tag = _tagUnknown;

        tag.incrementStatisticsCount(level);
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

        int tagLen1 = 1;
        //   Int32 tagLen2 = 2; // no 2-byte tags yet defined

        int key;

        byte tagA;
        byte tagB = 0x00;

        tagA = 0x20;                                              // ?    //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tagUnknown =
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "*** Unknown tag ***");

        tagA = (byte)eTag.PaletteDepth;                          // 0x02 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PaletteDepth"));

        tagA = (byte)eTag.ColorSpace;                            // 0x03 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "ColorSpace"));

        tagA = (byte)eTag.NullBrush;                             // 0x04 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "NullBrush"));

        tagA = (byte)eTag.NullPen;                               // 0x05 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "NullPen"));

        tagA = (byte)eTag.PaletteData;                           // 0x06 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PaletteData"));

        tagA = (byte)eTag.PaletteIndex;                          // 0x07 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PaletteIndex"));

        tagA = (byte)eTag.PatternSelectID;                       // 0x08 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PatternSelectID"));

        tagA = (byte)eTag.GrayLevel;                             // 0x09 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "GrayLevel"));

        tagA = (byte)eTag.RGBColor;                              // 0x0b //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "RGBColor"));

        tagA = (byte)eTag.PatternOrigin;                         // 0x0c //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PatternOrigin"));

        tagA = (byte)eTag.NewDestinationSize;                    // 0x0d //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "NewDestinationSize"));

        tagA = (byte)eTag.PrimaryArray;                          // 0x0e //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PrimaryArray"));

        tagA = (byte)eTag.PrimaryDepth;                          // 0x0f //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PrimaryDepth"));

        tagA = (byte)eTag.AllObjectTypes;                        // 0x1d //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagOperEnum,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "AllObjectTypes"));

        tagA = (byte)eTag.TextObjects;                           // 0x1e //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagOperEnum,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "TextObjects"));

        tagA = (byte)eTag.VectorObjects;                         // 0x1f //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagOperEnum,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VectorObjects"));

        tagA = (byte)eTag.RasterObjects;                         // 0x20 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagOperEnum,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "RasterObjects"));

        tagA = (byte)eTag.DeviceMatrix;                          // 0x21 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "DeviceMatrix"));

        tagA = (byte)eTag.DitherMatrixDataType;                  // 0x22 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "DitherMatrixDataType"));

        tagA = (byte)eTag.DitherOrigin;                          // 0x23 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "DitherOrigin"));

        tagA = (byte)eTag.MediaDestination;                      // 0x24 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "MediaDestination"));

        tagA = (byte)eTag.MediaSize;                             // 0x25 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagUbyteTxt, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "MediaSize"));

        tagA = (byte)eTag.MediaSource;                           // 0x26 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "MediaSource"));

        tagA = (byte)eTag.MediaType;                             // 0x27 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagUbyteTxt, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "MediaType"));

        tagA = (byte)eTag.Orientation;                           // 0x28 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "Orientation"));

        tagA = (byte)eTag.PageAngle;                             // 0x29 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PageAngle"));

        tagA = (byte)eTag.PageOrigin;                            // 0x2a //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PageOrigin"));

        tagA = (byte)eTag.PageScale;                             // 0x2b //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PageScale"));

        tagA = (byte)eTag.ROP3;                                  // 0x2c //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "ROP3"));

        tagA = (byte)eTag.TxMode;                                // 0x2d //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "TxMode"));

        tagA = (byte)eTag.CustomMediaSize;                       // 0x2f //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "CustomMediaSize"));

        tagA = (byte)eTag.CustomMediaSizeUnits;                  // 0x30 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "CustomMediaSizeUnits"));

        tagA = (byte)eTag.PageCopies;                            // 0x31 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PageCopies"));

        tagA = (byte)eTag.DitherMatrixSize;                      // 0x32 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "DitherMatrixSize"));

        tagA = (byte)eTag.DitherMatrixDepth;                     // 0x33 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "DitherMatrixDepth"));

        tagA = (byte)eTag.SimplexPageMode;                       // 0x34 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "SimplexPageMode"));

        tagA = (byte)eTag.DuplexPageMode;                        // 0x35 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "DuplexPageMode"));

        tagA = (byte)eTag.DuplexPageSide;                        // 0x36 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "DuplexPageSide"));

        tagA = (byte)eTag.ArcDirection;                          // 0x41 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "ArcDirection"));

        tagA = (byte)eTag.BoundingBox;                           // 0x42 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "BoundingBox"));

        tagA = (byte)eTag.DashOffset;                            // 0x43 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "DashOffset"));

        tagA = (byte)eTag.EllipseDimension;                      // 0x44 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "EllipseDimension"));

        tagA = (byte)eTag.EndPoint;                              // 0x45 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "EndPoint"));

        tagA = (byte)eTag.FillMode;                              // 0x46 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "FillMode"));

        tagA = (byte)eTag.LineCapStyle;                          // 0x47 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "LineCapStyle"));

        tagA = (byte)eTag.LineJoinStyle;                         // 0x48 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "LineJoinStyle"));

        tagA = (byte)eTag.MiterLength;                           // 0x49 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "MiterLength"));

        tagA = (byte)eTag.LineDashStyle;                         // 0x4a //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "LineDashStyle"));

        tagA = (byte)eTag.PenWidth;                              // 0x4b //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PenWidth"));

        tagA = (byte)eTag.Point;                                 // 0x4c //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "Point"));

        tagA = (byte)eTag.NumberOfPoints;                        // 0x4d //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "NumberOfPoints"));

        tagA = (byte)eTag.SolidLine;                             // 0x4e //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "SolidLine"));

        tagA = (byte)eTag.StartPoint;                            // 0x4f //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "StartPoint"));

        tagA = (byte)eTag.PointType;                             // 0x50 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PointType"));

        tagA = (byte)eTag.ControlPoint1;                         // 0x51 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "ControlPoint1"));

        tagA = (byte)eTag.ControlPoint2;                         // 0x52 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "ControlPoint2"));

        tagA = (byte)eTag.ClipRegion;                            // 0x53 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "ClipRegion"));

        tagA = (byte)eTag.ClipMode;                              // 0x54 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "ClipMode"));

        tagA = (byte)eTag.ColorDepthArray;                       // 0x61 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "ColorDepthArray"));

        tagA = (byte)eTag.ColorDepth;                            // 0x62 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "ColorDepth"));

        tagA = (byte)eTag.BlockHeight;                           // 0x63 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "BlockHeight"));

        tagA = (byte)eTag.ColorMapping;                          // 0x64 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "ColorMapping"));

        tagA = (byte)eTag.CompressMode;                          // 0x65 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "CompressMode"));

        tagA = (byte)eTag.DestinationBox;                        // 0x66 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "DestinationBox"));

        tagA = (byte)eTag.DestinationSize;                       // 0x67 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "DestinationSize"));

        tagA = (byte)eTag.PatternPersistence;                    // 0x68 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PatternPersistence"));

        tagA = (byte)eTag.PatternDefineID;                       // 0x69 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PatternDefineID"));

        tagA = (byte)eTag.SourceHeight;                          // 0x6b //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "SourceHeight"));

        tagA = (byte)eTag.SourceWidth;                           // 0x6c //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "SourceWidth"));

        tagA = (byte)eTag.StartLine;                             // 0x6d //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "StartLine"));

        tagA = (byte)eTag.PadBytesMultiple;                      // 0x6e //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PadBytesMultiple"));

        tagA = (byte)eTag.BlockByteLength;                       // 0x6f //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "BlockByteLength"));

        tagA = (byte)eTag.NumberOfScanLines;                     // 0x73 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "NumberOfScanLines"));

        tagA = (byte)eTag.PrintableArea;                         // 0x74 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PrintableArea"));

        tagA = (byte)eTag.TumbleMode;                            // 0x75 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "TumbleMode"));

        tagA = (byte)eTag.ContentOrientation;                    // 0x76 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "ContentOrientation"));

        tagA = (byte)eTag.FeedOrientation;                       // 0x77 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "FeedOrientation"));

        tagA = (byte)eTag.ColorTreatment;                        // 0x78 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "ColorTreatment"));

        tagA = (byte)eTag.CommentData;                           // 0x81 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagUbyteTxt, flagUintTxt, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "CommentData"));

        tagA = (byte)eTag.DataOrg;                               // 0x82 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "DataOrg"));

        tagA = (byte)eTag.Measure;                               // 0x86 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.Measure,
                               PrnParseConstants.eOvlAct.None,
                               "Measure"));

        tagA = (byte)eTag.SourceType;                            // 0x88 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "SourceType"));

        tagA = (byte)eTag.UnitsPerMeasure;                       // 0x89 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.UnitsPerMeasure,
                               PrnParseConstants.eOvlAct.None,
                               "UnitsPerMeasure"));

        tagA = (byte)eTag.QueryKey;                              // 0x8a //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "QueryKey"));

        tagA = (byte)eTag.StreamName;                            // 0x8b //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagUbyteTxt, flagUintTxt, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "StreamName"));

        tagA = (byte)eTag.StreamDataLength;                      // 0x8c //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "StreamDataLength"));

        tagA = (byte)eTag.PCLSelectFont;                         // 0x8d //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagUbyteTxt, flagNone, flagNone, flagValIsPCL,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "PCLSelectFont"));

        tagA = (byte)eTag.ErrorReport;                           // 0x8f //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.Remove,
                               "ErrorReport"));

        tagA = (byte)eTag.VUExtension;                           // 0x91 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VUExtension"));

        tagA = (byte)eTag.VUDataLength;                          // 0x92 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagValIsLen, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VUDataLength"));

        tagA = (byte)eTag.VUAttr1;                               // 0x93 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VUAttr1"));

        tagA = (byte)eTag.VUAttr2;                               // 0x94 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VUAttr2"));

        tagA = (byte)eTag.VUAttr3;                               // 0x95 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VUAttr3"));

        tagA = (byte)eTag.VUAttr4;                               // 0x96 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VUAttr4"));

        tagA = (byte)eTag.VUAttr5;                               // 0x97 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VUAttr5"));

        tagA = (byte)eTag.VUAttr6;                               // 0x98 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VUAttr6"));

        tagA = (byte)eTag.VUAttr7;                               // 0x99 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VUAttr7"));

        tagA = (byte)eTag.VUAttr8;                               // 0x9a //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VUAttr8"));

        tagA = (byte)eTag.VUAttr9;                               // 0x9b //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VUAttr9"));

        tagA = (byte)eTag.VUAttr10;                              // 0x9c //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VUAttr10"));

        tagA = (byte)eTag.VUAttr11;                              // 0x9d //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VUAttr11"));

        tagA = (byte)eTag.VUAttr12;                              // 0x9e //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "VUAttr12"));

        tagA = 0x9f;                                               // 0x9f //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagReserved, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "* Reserved *"));

        tagA = (byte)eTag.EnableDiagnostics;                     // 0xa0 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "EnableDiagnostics"));

        tagA = (byte)eTag.CharAngle;                             // 0xa1 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "CharAngle"));

        tagA = (byte)eTag.CharCode;                              // 0xa2 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "CharCode"));

        tagA = (byte)eTag.CharDataSize;                          // 0xa3 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "CharDataSize"));

        tagA = (byte)eTag.CharScale;                             // 0xa4 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "CharScale"));

        tagA = (byte)eTag.CharShear;                             // 0xa5 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "CharShear"));

        tagA = (byte)eTag.CharSize;                              // 0xa6 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.CharSize,
                               PrnParseConstants.eOvlAct.None,
                               "CharSize"));

        tagA = (byte)eTag.FontHeaderLength;                      // 0xa7 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "FontHeaderLength"));

        tagA = (byte)eTag.FontName;                              // 0xa8 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagUbyteTxt, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "FontName"));

        tagA = (byte)eTag.FontFormat;                            // 0xa9 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "FontFormat"));

        tagA = (byte)eTag.SymbolSet;                             // 0xaa //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "SymbolSet"));

        tagA = (byte)eTag.TextData;                              // 0xab //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagUbyteTxt, flagUintTxt, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "TextData"));

        tagA = (byte)eTag.CharSubModeArray;                      // 0xac //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "CharSubModeArray"));

        tagA = (byte)eTag.WritingMode;                           // 0xad //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagAttrEnum, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "WritingMode"));

        tagA = (byte)eTag.BitmapCharScaling;                     // 0xae //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "BitmapCharScaling"));

        tagA = (byte)eTag.XSpacingData;                          // 0xaf //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "XSpacingData"));

        tagA = (byte)eTag.YSpacingData;                          // 0xb0 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
                               "YSpacingData"));

        tagA = (byte)eTag.CharBoldValue;                         // 0xb1 //
        key = (((tagLen1 * 256) + tagA) * 256) + tagB;
        _tags.Add(key,
            new PCLXLAttribute(tagLen1, tagA, tagB,
                               flagNone, flagNone, flagNone,
                               flagNone, flagNone, flagNone, flagNone,
                               PrnParseConstants.eActPCLXL.None,
                               PrnParseConstants.eOvlAct.None,
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
