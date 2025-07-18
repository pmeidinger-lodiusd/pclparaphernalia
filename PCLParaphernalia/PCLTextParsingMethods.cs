﻿namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines a set of PCL Text Parsing Method objects.
    /// 
    /// © Chris Hutchinson 2015
    /// 
    /// </summary>

    static class PCLTextParsingMethods
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        // Note that the length of the index array must be the same as that   //
        // of the definition array; the entries must be in the same order.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum eIndex
        {
            not_specified,
            m0_1_byte_default,
            m1_1_byte_alt,
            m2_2_byte,
            m21_1_or_2_byte_Asian7bit,
            m31_1_or_2_byte_ShiftJIS,
            m38_1_or_2_byte_Asian8bit,
            m83_UTF8,
            m1008_UTF8_alt
        }

        public enum ePCLVal
        {
            not_specified = -1,
            m0_1_byte_default = 0,
            m1_1_byte_alt = 1,
            m2_2_byte = 2,
            m21_1_or_2_byte_Asian7bit = 21,
            m31_1_or_2_byte_ShiftJIS = 31,
            m38_1_or_2_byte_Asian8bit = 38,
            m83_UTF8 = 83,
            m1008_UTF8_alt = 1008
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly PCLTextParsingMethod[] _methods =
        {
            //----------------------------------------------------------------//
            //                                                                //
            // Entries must be in the order of the eIndex enumeration.        //
            //                                                                //
            // Note that whilst the UTF-8 entries specify single-byte ranges  //
            // of 0x00 -> 0xff, the valid single-byte values are < 0x80; this //
            // is catered for in the relevant PCL-specific code functions.    // 
            //                                                                //
            //----------------------------------------------------------------//

            new PCLTextParsingMethod (eIndex.not_specified,
                                      (short) ePCLVal.not_specified,
                                      "<not specified>",
                                      new ushort [] {0x00, 0xff},
                                      null),
            new PCLTextParsingMethod (eIndex.m0_1_byte_default,
                                      (short) ePCLVal.m0_1_byte_default,
                                      "1-byte (default)",
                                      new ushort [] {0x00, 0xff},
                                      null),
            new PCLTextParsingMethod (eIndex.m1_1_byte_alt,
                                      (short) ePCLVal.m1_1_byte_alt,
                                      "1-byte",
                                      new ushort [] {0x00, 0xff},
                                      null),
            new PCLTextParsingMethod (eIndex.m2_2_byte,
                                      (short) ePCLVal.m2_2_byte,
                                      "2-byte",
                                      null,
                                      new ushort [] {0x00, 0xff, 0x0100, 0xffff}),
            new PCLTextParsingMethod (eIndex.m21_1_or_2_byte_Asian7bit,
                                      (short) ePCLVal.m21_1_or_2_byte_Asian7bit,
                                      "1- or 2-byte Asian 7-bit",
                                      new ushort [] {0x00, 0x20},
                                      new ushort [] {0x2100, 0xffff}),
            new PCLTextParsingMethod (eIndex.m31_1_or_2_byte_ShiftJIS,
                                      (short) ePCLVal.m31_1_or_2_byte_ShiftJIS,
                                      "1- or 2-byte Shift-JIS",
                                      new ushort [] {0x00, 0x80, 0xa0, 0xdf, 0xfd, 0xff},
                                      new ushort [] {0x8100, 0x9fff, 0xe000, 0xfcff}),
            new PCLTextParsingMethod (eIndex.m38_1_or_2_byte_Asian8bit,
                                      (short) ePCLVal.m38_1_or_2_byte_Asian8bit,
                                      "1- or 2-byte Asian 8-bit",
                                      new ushort [] {0x00, 0x7f},
                                      new ushort [] {0x8000, 0xffff}),
            new PCLTextParsingMethod (eIndex.m83_UTF8,
                                      (short) ePCLVal.m83_UTF8,
                                      "UTF-8",
                                      new ushort [] {0x00, 0xff},
                                      new ushort [] {0x0100, 0xffff}),
            new PCLTextParsingMethod (eIndex.m1008_UTF8_alt,
                                      (short) ePCLVal.m1008_UTF8_alt,
                                      "UTF-8 (alternative)",
                                      new ushort [] {0x00, 0xff},
                                      new ushort [] {0x0100, 0xffff})
        };

        private static readonly int _methodCount =
            _methods.GetUpperBound(0) + 1;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o u n t                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return count of Text Parsing Methods definitions.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetCount()
        {
            return _methodCount;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e s c                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return description associated with specified text parsing method   //
        // index.                                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetDesc(int index)
        {
            return _methods[index].GetDesc();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e s c L o n g                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return value and description associated with specified text        //
        // parsing method index.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetDescLong(int index)
        {
            return _methods[index].GetDescLong();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t M e t h o d T y p e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return method type associated with specified text parsing method   //
        // index.                                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static eIndex GetMethodType(int index)
        {
            return _methods[index].GetMethodType();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t R a n g e D a t a D o u b l e                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the double-byte range(s) associated with the specified text //
        // parsing method index.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static ushort[] GetRangeDataDouble(int index)
        {
            return _methods[index].GetRangeDataDouble();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t R a n g e D a t a D o u b l e C t                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the count of double-byte range(s) associated with the       //
        // specified text parsing method index.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetRangeDataDoubleCt(int index)
        {
            return _methods[index].GetRangeDataDoubleCt();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t R a n g e D a t a S i n g l e                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the single-byte range(s) associated with the specified text //
        // parsing method index.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static ushort[] GetRangeDataSingle(int index)
        {
            return _methods[index].GetRangeDataSingle();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t R a n g e D a t a S i n g l e C t                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the count of single-byte range(s) associated with the       //
        // specified text parsing method index.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetRangeDataSingleCt(int index)
        {
            return _methods[index].GetRangeDataSingleCt();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t V a l u e                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return PCL value associated with specified text parsing method     //
        // index.                                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetValue(int index)
        {
            return _methods[index].GetValue();
        }
    }
}