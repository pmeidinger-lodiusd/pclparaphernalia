using System;
using System.IO;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class provides primitive and macro operations for PCL XL print language.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    static class PCLXLWriter
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const int cSizeAttrReal32 = 7;
        private const int cSizeAttrReal32XY = 11;

        private const int cSizeAttrSint16 = 5;
        //private const int cSizeAttrSint16ArrayBase = 6;
        //private const int cSizeAttrSint16ArrayUnit = 2;
        //private const int cSizeAttrSint16Box = 11;
        //private const int cSizeAttrSint16XY = 7;

        private const int cSizeAttrUbyte = 4;
        private const int cSizeAttrUbyteArrayBase = 6;
        private const int cSizeAttrUbyteArrayUnit = 1;

        private const int cSizeAttrUint16 = 5;
        //private const int cSizeAttrUint16ArrayBase = 6;
        //private const int cSizeAttrUint16ArrayUnit = 2;
        private const int cSizeAttrUint16Box = 11;
        //private const int cSizeAttrUint16XY = 7;

        private const int cSizeAttrUint32 = 7;

        private const int cSizeEmbedDataByte = 2;
        private const int cSizeEmbedDataInt32 = 5;

        private const int cSizeOperator = 1;

        public static readonly byte[] rgbBlack = { 0, 0, 0 }; // Black
        public static readonly byte[] monoPalette = { 255, 0 };  // white, black

        public const ushort _sessionUPI = 600;
        public const ushort _pointsPerInch = 72;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Font advance metrics.                                              //
        //                                                                    //
        // Values:                                                            //
        //  - are for the ASCII range (0x20-0x7e subset) only;                //
        //  - are for point size 360; assume linear scaling for other values; //
        //                                                                    //
        //  - are for units-per-inch of 600.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const ushort cAdvanceTablePPI = 360;

        public static readonly short[] advances_ArialRegular =
        {
               833,  833, 1064, 1668, 1668, 2667, 2000,  572,     // 0x20 --> //
               999,  999, 1167, 1751,  833,  999,  833,  833,
              1668, 1668, 1668, 1668, 1668, 1668, 1668, 1668,     // 0x30 --> //
              1668, 1668,  833,  833, 1751, 1751, 1751, 1668,
              3045, 2000, 2000, 2166, 2166, 2000, 1832, 2333,     // 0x40 --> //
              2166,  833, 1500, 2000, 1668, 2499, 2166, 2333,
              2000, 2333, 2166, 2000, 1832, 2166, 2000, 2831,     // 0x50 --> //
              2000, 2000, 1832,  833,  833,  833, 1407, 1668,
               999, 1668, 1668, 1500, 1668, 1668,  833, 1668,     // 0x60 --> //
              1668,  666,  666, 1500,  666, 2499, 1668, 1668,
              1668, 1668,  999, 1500,  833, 1668, 1500, 2166,     // 0x70 --> //
              1500, 1500, 1500, 1001,  779, 1001, 1751
        };

        public static readonly short[] advances_ArialBold =
        {
               833,  999, 1422, 1668, 1668, 2667, 2166,  713,     // 0x20 --> //
               999,  999, 1167, 1751,  833,  999,  833,  833,
              1668, 1668, 1668, 1668, 1668, 1668, 1668, 1668,     // 0x30 --> //
              1668, 1668,  999,  999, 1751, 1751, 1751, 1832,
              2925, 2166, 2166, 2166, 2166, 2000, 1832, 2333,     // 0x40 --> //
              2166,  833, 1668, 2166, 1832, 2499, 2166, 2333,
              2000, 2333, 2166, 2000, 1832, 2166, 2000, 2831,     // 0x50 --> //
              2000, 2000, 1832,  999,  833,  999, 1751, 1668,
               999, 1668, 1832, 1668, 1832, 1668,  999, 1832,     // 0x60 --> //
              1832,  833,  833, 1668,  833, 2667, 1832, 1832,
              1832, 1832, 1167, 1668,  999, 1832, 1668, 2333,     // 0x70 --> //
              1668, 1668, 1500, 1167,  839, 1167, 1751
        };

        public static readonly short[] advances_Courier =
        {
              1800, 1800, 1800, 1800, 1800, 1800, 1800, 1800,     // 0x20 --> //
              1800, 1800, 1800, 1800, 1800, 1800, 1800, 1800,
              1800, 1800, 1800, 1800, 1800, 1800, 1800, 1800,     // 0x30 --> //
              1800, 1800, 1800, 1800, 1800, 1800, 1800, 1800,
              1800, 1800, 1800, 1800, 1800, 1800, 1800, 1800,     // 0x40 --> //
              1800, 1800, 1800, 1800, 1800, 1800, 1800, 1800,
              1800, 1800, 1800, 1800, 1800, 1800, 1800, 1800,     // 0x50 --> //
              1800, 1800, 1800, 1800, 1800, 1800, 1800, 1800,
              1800, 1800, 1800, 1800, 1800, 1800, 1800, 1800,     // 0x60 --> //
              1800, 1800, 1800, 1800, 1800, 1800, 1800, 1800,
              1800, 1800, 1800, 1800, 1800, 1800, 1800, 1800,     // 0x70 --> //
              1800, 1800, 1800, 1800, 1800, 1800, 1800
        };

        //--------------------------------------------------------------------//
        //                                                                    //
        // Notes:                                                             //
        //                                                                    //
        // 1    As C# does not support the concept of unions, many of the     //
        //      following methods use the BitConverter class.                 //
        //                                                                    //
        //      Note that the conversion operation assumes the Endian-ness of //
        //      the architecture on which it is running.                      //
        //      On Intel, this is Little-Endian, which is what is             //
        //      (fortunately) the setting we specify in the PCL XL stream     //
        //      header in the PCL XL class.                                   //
        //                                                                    //
        // 2    None of the methods do any bound checking on the buffer being //
        //      updated.                                                      //
        //      This is partially excused by the fact that we are writing     //
        //      very specific data (not user-supplied) so we (should) know at //
        //      design time how big a buffer we need.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r R e a l 3 2                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'real32'       //
        // data type, to the target buffer.                                   //
        // The data is supplied as a 'float' value.                           //
        // Generated sequence size is 7 bytes.                                // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrReal32(ref byte[] buffer,
                                         ref int bufIndex,
                                         PCLXLAttributes.Tag attributeTag,
                                         float valReal32)
        {
            byte[] tempArray;

            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Real32;

            tempArray = BitConverter.GetBytes(valReal32);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];
            buffer[bufIndex++] = tempArray[2];
            buffer[bufIndex++] = tempArray[3];

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r R e a l 3 2 X Y                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'real32_xy'    //
        // data type, to the target buffer.                                   //
        // The data is supplied as a pair of 'float' values.                  //
        // Generated sequence size is 11 bytes.                               // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrReal32XY(ref byte[] buffer,
                                           ref int bufIndex,
                                           PCLXLAttributes.Tag attributeTag,
                                           float valReal32_x,
                                           float valReal32_y)
        {
            byte[] tempArray;

            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Real32XY;

            tempArray = BitConverter.GetBytes(valReal32_x);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];
            buffer[bufIndex++] = tempArray[2];
            buffer[bufIndex++] = tempArray[3];

            tempArray = BitConverter.GetBytes(valReal32_y);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];
            buffer[bufIndex++] = tempArray[2];
            buffer[bufIndex++] = tempArray[3];

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r S i n t 1 6                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'sint16'       //
        // data type, to the target buffer.                                   //
        // The data is supplied as a 'short' value.                           //
        // Generated sequence size is 5 bytes.                                // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrSint16(ref byte[] buffer,
                                         ref int bufIndex,
                                         PCLXLAttributes.Tag attributeTag,
                                         short valSint16)
        {
            byte[] tempArray;

            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Sint16;

            tempArray = BitConverter.GetBytes(valSint16);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r S i n t 1 6 A r r a y                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'sint16_array' //
        // data type, to the target buffer.                                   //
        // The data is supplied as an array of 'short' values.                //
        // Generated sequence size is 6 + (2 * arraySize) bytes.              // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrSint16Array(ref byte[] buffer,
                                              ref int bufIndex,
                                              PCLXLAttributes.Tag attributeTag,
                                              short arraySize,
                                              short[] arraySint16)
        {
            byte[] tempArray;

            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Sint16Array;
            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Uint16;

            tempArray = BitConverter.GetBytes(arraySize);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            for (int i = 0; i < arraySize; i++)
            {
                tempArray = BitConverter.GetBytes(arraySint16[i]);
                buffer[bufIndex++] = tempArray[0];
                buffer[bufIndex++] = tempArray[1];
            }

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r S i n t 1 6 B o x                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'sint16_box'   //
        // data type, to the target buffer.                                   //
        // The data is supplied as a quartet of 'short' values.               //
        // Generated sequence size is 11 bytes.                               // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrSint16Box(ref byte[] buffer,
                                            ref int bufIndex,
                                            PCLXLAttributes.Tag attributeTag,
                                            short valSint16_x1,
                                            short valSint16_y1,
                                            short valSint16_x2,
                                            short valSint16_y2)
        {
            byte[] tempArray;

            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Sint16Box;

            tempArray = BitConverter.GetBytes(valSint16_x1);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            tempArray = BitConverter.GetBytes(valSint16_y1);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            tempArray = BitConverter.GetBytes(valSint16_x2);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            tempArray = BitConverter.GetBytes(valSint16_y2);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r S i n t 1 6 X Y                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'sint16_xy'    //
        // data type, to the target buffer.                                   //
        // The data is supplied as a pair of 'short' values.                  //
        // Generated sequence size is 7 bytes.                                // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrSint16XY(ref byte[] buffer,
                                           ref int bufIndex,
                                           PCLXLAttributes.Tag attributeTag,
                                           short valSint16_x,
                                           short valSint16_y)
        {
            byte[] tempArray;

            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Sint16XY;

            tempArray = BitConverter.GetBytes(valSint16_x);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            tempArray = BitConverter.GetBytes(valSint16_y);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r U b y t e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'ubyte'        //
        // data type, to the target buffer.                                   //
        // The data is supplied as a 'byte' value.                            //
        // Generated sequence size is 4 bytes.                                // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrUbyte(ref byte[] buffer,
                                        ref int bufIndex,
                                        PCLXLAttributes.Tag attributeTag,
                                        byte valUbyte)
        {
            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Ubyte;

            buffer[bufIndex++] = valUbyte;

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r U b y t e A r r a y                          (1)     //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'ubyte_array'  //
        // data type, to the target buffer.                                   //
        // The data is supplied as an array of 'short' values.                //
        //                                                                    //
        // The assumption is made that all the values are in the range:       //
        //  0 <= x <= 255                                                     //
        // i.e. we only use the least significant byte of each value.         //
        // Generated sequence size is 6 + arraySize bytes.                    // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrUbyteArray(ref byte[] buffer,
                                             ref int bufIndex,
                                             PCLXLAttributes.Tag attributeTag,
                                             short arraySize,
                                             short[] arraySint16)
        {
            byte[] tempArray;

            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.UbyteArray;
            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Uint16;

            tempArray = BitConverter.GetBytes(arraySize);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            for (int i = 0; i < arraySize; i++)
            {
                tempArray = BitConverter.GetBytes(arraySint16[i]);
                buffer[bufIndex++] = tempArray[0];
            }

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r U b y t e A r r a y                          (2)     //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'ubyte_array'  //
        // data type, to the target buffer.                                   //
        // The data is supplied as an array of 'byte' values.                 //
        // Generated sequence size is 6 + arraySize bytes.                    // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrUbyteArray(ref byte[] buffer,
                                             ref int bufIndex,
                                             PCLXLAttributes.Tag attributeTag,
                                             short arraySize,
                                             byte[] arrayUbyte)
        {
            byte[] tempArray;

            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.UbyteArray;
            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Uint16;

            tempArray = BitConverter.GetBytes(arraySize);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            for (int i = 0; i < arraySize; i++)
            {
                buffer[bufIndex++] = arrayUbyte[i];
            }

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r U b y t e A r r a y                          (3)     //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'ubyte_array'  //
        // data type, to the target buffer.                                   //
        // The data is supplied as a string of (UTF-8) characters.            //
        //                                                                    //
        // The assumption is made that all the characters are in the ASCII    //
        // range (0x00 -> 0x7f) and are hence each represented using one      //
        // byte (i.e. no multi-byte characters are present).                  //
        // Generated sequence size is 6 + string-length bytes.                // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrUbyteArray(ref byte[] buffer,
                                             ref int bufIndex,
                                             PCLXLAttributes.Tag attributeTag,
                                             string valString)
        {
            byte[] tempArray;
            short arraySize;

            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.UbyteArray;
            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Uint16;

            arraySize = (short)valString.Length;
            tempArray = BitConverter.GetBytes(arraySize);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            for (int i = 0; i < arraySize; i++)
            {
                buffer[bufIndex++] = (byte)valString[i];
            }

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r U i n t 1 6                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'uint16'       //
        // data type, to the target buffer.                                   //
        // The data is supplied as a 'ushort' value.                          //
        // Generated sequence size is 5 bytes.                                // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrUint16(ref byte[] buffer,
                                         ref int bufIndex,
                                         PCLXLAttributes.Tag attributeTag,
                                         ushort valUint16)
        {
            byte[] tempArray;

            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Uint16;

            tempArray = BitConverter.GetBytes(valUint16);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r U i n t 1 6 A r r a y                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'uint16_array' //
        // data type, to the target buffer.                                   //
        // The data is supplied as an array of 'ushort' values.               //
        // Generated sequence size is 6 + (2 * arraySize) bytes.              // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrUint16Array(ref byte[] buffer,
                                              ref int bufIndex,
                                              PCLXLAttributes.Tag attributeTag,
                                              short arraySize,
                                              ushort[] arrayUint16)
        {
            byte[] tempArray;

            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Uint16Array;
            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Uint16;

            tempArray = BitConverter.GetBytes(arraySize);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            for (int i = 0; i < arraySize; i++)
            {
                tempArray = BitConverter.GetBytes(arrayUint16[i]);
                buffer[bufIndex++] = tempArray[0];
                buffer[bufIndex++] = tempArray[1];
            }

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r U i n t 1 6 B o x                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'uint16_box'   //
        // data type, to the target buffer.                                   //
        // The data is supplied as a quartet of 'ushort' values.              //
        // Generated sequence size is 11 bytes.                               // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrUint16Box(ref byte[] buffer,
                                            ref int bufIndex,
                                            PCLXLAttributes.Tag attributeTag,
                                            ushort valUint16_x1,
                                            ushort valUint16_y1,
                                            ushort valUint16_x2,
                                            ushort valUint16_y2)
        {
            byte[] tempArray;

            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Uint16Box;

            tempArray = BitConverter.GetBytes(valUint16_x1);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            tempArray = BitConverter.GetBytes(valUint16_y1);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            tempArray = BitConverter.GetBytes(valUint16_x2);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            tempArray = BitConverter.GetBytes(valUint16_y2);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r U i n t 1 6 X Y                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'uint16_xy'    //
        // data type, to the target buffer.                                   //
        // The data is supplied as a pair of 'ushort' values.                 //
        // Generated sequence size is 7 bytes.                                // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrUint16XY(ref byte[] buffer,
                                           ref int bufIndex,
                                           PCLXLAttributes.Tag attributeTag,
                                           ushort valUint16_x,
                                           ushort valUint16_y)
        {
            byte[] tempArray;

            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Uint16XY;

            tempArray = BitConverter.GetBytes(valUint16_x);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            tempArray = BitConverter.GetBytes(valUint16_y);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d A t t r U i n t 3 2                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Attribute sequence, with a 'uint32'       //
        // data type, to the target buffer.                                   //
        // The data is supplied as a 'uint' value.                            //
        // Generated sequence size is 7 bytes.                                // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddAttrUint32(ref byte[] buffer,
                                         ref int bufIndex,
                                         PCLXLAttributes.Tag attributeTag,
                                         uint valUint32)
        {
            byte[] tempArray;

            buffer[bufIndex++] = (byte)PCLXLDataTypes.Tag.Uint32;

            tempArray = BitConverter.GetBytes(valUint32);

            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];
            buffer[bufIndex++] = tempArray[2];
            buffer[bufIndex++] = tempArray[3];

            buffer[bufIndex++] = (byte)PCLXLAttrDefiners.Tag.Ubyte;
            buffer[bufIndex++] = (byte)attributeTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d E m b e d D a t a I n t r o                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return PCLXL Embedded Data introduction sequence.                  //
        // Generated sequence size is 2 bytes if dataLen value < 256          // 
        //                         or 5 bytes otherwise.                      // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddEmbedDataIntro(ref byte[] buffer,
                                             ref int bufIndex,
                                             int dataLen)
        {
            byte[] tempArray = BitConverter.GetBytes(dataLen);

            if (dataLen < 256)
            {
                buffer[bufIndex++] = (byte)PCLXLEmbedDataDefs.Tag.Byte;
                buffer[bufIndex++] = tempArray[0];
            }
            else
            {
                buffer[bufIndex++] = (byte)PCLXLEmbedDataDefs.Tag.Int;
                buffer[bufIndex++] = tempArray[0];
                buffer[bufIndex++] = tempArray[1];
                buffer[bufIndex++] = tempArray[2];
                buffer[bufIndex++] = tempArray[3];
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d O p e r a t o r                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Append a specified PCLXL Operator.                                 //
        // Generated sequence size is 1 byte.                                 // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void AddOperator(ref byte[] buffer,
                                       ref int bufIndex,
                                       PCLXLOperators.Tag operatorTag)
        {
            buffer[bufIndex++] = (byte)operatorTag;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h a r A n g l e                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate SetCharAngle operator and associated Attribute List.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void CharAngle(BinaryWriter prnWriter,
                                      bool embeddedStream,
                                      short charAngle)
        {
            int lenBuf;

            int indBuf = 0;

            lenBuf = cSizeAttrSint16 +
                     cSizeOperator;

            byte[] buffer = new byte[lenBuf];

            //----------------------------------------------------------------//

            AddAttrSint16(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.Tag.CharAngle,
                           charAngle);

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.SetCharAngle);

            WriteStreamBlock(prnWriter, embeddedStream,
                              buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h a r B o l d                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate SetCharBoldValue operator and associated Attribute List.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void CharBold(BinaryWriter prnWriter,
                                     bool embeddedStream,
                                     float charBoldValue)
        {
            int lenBuf;

            int indBuf = 0;

            lenBuf = cSizeAttrReal32 +
                     cSizeOperator;

            byte[] buffer = new byte[lenBuf];

            //----------------------------------------------------------------//

            AddAttrReal32(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.Tag.CharBoldValue,
                           charBoldValue);

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.SetCharBoldValue);

            WriteStreamBlock(prnWriter, embeddedStream,
                              buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h a r S c a l e                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate SetCharScale operator and associated Attribute List.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void CharScale(BinaryWriter prnWriter,
                                      bool embeddedStream,
                                      float charScaleX,
                                      float charScaleY)
        {
            int lenBuf;

            int indBuf = 0;

            lenBuf = cSizeAttrReal32XY +
                     cSizeOperator;

            byte[] buffer = new byte[lenBuf];

            //----------------------------------------------------------------//

            AddAttrReal32XY(ref buffer,
                             ref indBuf,
                             PCLXLAttributes.Tag.CharScale,
                             charScaleX,
                             charScaleY);

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.SetCharScale);

            WriteStreamBlock(prnWriter, embeddedStream,
                              buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h a r S h e a r                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate SetCharShear operator and associated Attribute List.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void CharShear(BinaryWriter prnWriter,
                                      bool embeddedStream,
                                      float charShearX,
                                      float charShearY)
        {
            int lenBuf;

            int indBuf = 0;

            lenBuf = cSizeAttrReal32XY +
                     cSizeOperator;

            byte[] buffer = new byte[lenBuf];

            //----------------------------------------------------------------//

            AddAttrReal32XY(ref buffer,
                             ref indBuf,
                             PCLXLAttributes.Tag.CharShear,
                             charShearX,
                             charShearY);

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.SetCharShear);

            WriteStreamBlock(prnWriter, embeddedStream,
                              buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // e m b e d D a t a I n t r o                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate embedded data introduction.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void EmbedDataIntro(BinaryWriter prnWriter,
                                           bool embeddedStream,
                                           ushort hddrLen)
        {
            int lenBuf;

            int indBuf = 0;

            // TODO: Why is this assigned here?
            if (hddrLen > 256)
                lenBuf = 5;
            else
                lenBuf = 2;

            lenBuf = cSizeAttrUint16 + cSizeOperator;

            byte[] buffer = new byte[lenBuf];

            //----------------------------------------------------------------//

            AddEmbedDataIntro(ref buffer,
                               ref indBuf,
                               hddrLen);

            WriteStreamBlock(prnWriter, embeddedStream,
                              buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t                                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate font selection operators and associated Attribute Lists   //
        // for specified font name, size and symbol set.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void Font(BinaryWriter prnWriter,
                                bool embeddedStream,
                                float pointSize,
                                ushort symbolSet,
                                string fontName)
        {
            const int lenHPFontName = 16;
            const int lenBuf = 256;

            byte[] buffer = new byte[lenBuf];

            int indBuf,
                  len;

            float charSize;

            indBuf = 0;

            //----------------------------------------------------------------//

            charSize = pointSize * _sessionUPI / _pointsPerInch;

            len = fontName.Length;

            if (len < lenHPFontName)
            {
                string nameFiller = new string('\x20', lenHPFontName - len);

                AddAttrUbyteArray(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.Tag.FontName,
                                  fontName + nameFiller);
            }
            else
            {
                AddAttrUbyteArray(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.Tag.FontName,
                                  fontName);
            }

            AddAttrUint16(ref buffer,
                          ref indBuf,
                          PCLXLAttributes.Tag.SymbolSet,
                          symbolSet);

            AddAttrReal32(ref buffer,
                          ref indBuf,
                          PCLXLAttributes.Tag.CharSize,
                          charSize);

            AddOperator(ref buffer,
                        ref indBuf,
                        PCLXLOperators.Tag.SetFont);

            WriteStreamBlock(prnWriter, embeddedStream,
                             buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t C h a r B e g i n                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate BeginChar operator and associated Attribute List.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void FontCharBegin(BinaryWriter prnWriter,
                                          bool embeddedStream,
                                          short fontNameLen,
                                          byte[] fontName)
        {
            int lenBuf;

            int indBuf = 0;

            lenBuf = cSizeAttrUbyteArrayBase +
                     (cSizeAttrUbyteArrayUnit * fontNameLen) +
                     cSizeOperator;

            byte[] buffer = new byte[lenBuf];

            //----------------------------------------------------------------//

            AddAttrUbyteArray(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.FontName,
                               fontNameLen,
                               fontName);

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.BeginChar);

            WriteStreamBlock(prnWriter, embeddedStream,
                              buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t C h a r E n d                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate EndChar operator.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void FontCharEnd(BinaryWriter prnWriter,
                                        bool embeddedStream)
        {
            int lenBuf;

            int indBuf = 0;

            lenBuf = cSizeOperator;

            byte[] buffer = new byte[lenBuf];

            //----------------------------------------------------------------//

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.EndChar);

            WriteStreamBlock(prnWriter, embeddedStream,
                              buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t C h a r R e a d                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate ReadChar operator and associated Attribute List.          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void FontCharRead(BinaryWriter prnWriter,
                                         bool embeddedStream,
                                         ushort charCode,
                                         ushort charDataSize)
        {
            int lenBuf;

            int indBuf = 0;

            lenBuf = cSizeAttrUint16 + cSizeAttrUint16 + cSizeOperator;
            byte[] buffer = new byte[lenBuf];

            //----------------------------------------------------------------//

            AddAttrUint16(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.Tag.CharCode,
                           charCode);

            AddAttrUint16(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.Tag.CharDataSize,
                           charDataSize);

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.ReadChar);

            WriteStreamBlock(prnWriter, embeddedStream,
                              buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t H d d r B e g i n                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate BeginFontHeader operator and associated Attribute List.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void FontHddrBegin(BinaryWriter prnWriter,
                                          bool embeddedStream,
                                          short fontNameLen,
                                          byte[] fontName,
                                          byte fontFormat)
        {
            int lenBuf;

            int indBuf = 0;

            lenBuf = cSizeAttrUbyteArrayBase +
                     (cSizeAttrUbyteArrayUnit * fontNameLen) +
                     cSizeAttrUbyte +
                     cSizeOperator;

            byte[] buffer = new byte[lenBuf];

            //----------------------------------------------------------------//

            AddAttrUbyteArray(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.FontName,
                               fontNameLen,
                               fontName);

            AddAttrUbyte(ref buffer,
                          ref indBuf,
                          PCLXLAttributes.Tag.FontFormat,
                          fontFormat);

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.BeginFontHeader);

            WriteStreamBlock(prnWriter, embeddedStream,
                              buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t H d d r E n d                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate EndFontHeader operator.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void FontHddrEnd(BinaryWriter prnWriter,
                                        bool embeddedStream)
        {
            int lenBuf;

            int indBuf = 0;

            lenBuf = cSizeOperator;

            byte[] buffer = new byte[lenBuf];

            //----------------------------------------------------------------//

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.EndFontHeader);

            WriteStreamBlock(prnWriter, embeddedStream,
                              buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t H d d r R e a d                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate ReadFontHeader operator and associated Attribute List.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void FontHddrRead(BinaryWriter prnWriter,
                                         bool embeddedStream,
                                         ushort hddrLen)
        {
            int lenBuf;

            int indBuf = 0;

            lenBuf = cSizeAttrUint16 + cSizeOperator;

            byte[] buffer = new byte[lenBuf];

            //----------------------------------------------------------------//

            AddAttrUint16(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.Tag.FontHeaderLength,
                           hddrLen);

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.ReadFontHeader);

            WriteStreamBlock(prnWriter, embeddedStream,
                              buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t R e m o v e                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate font remove operators and associated Attribute List for   //
        // for specified font name.                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void FontRemove(BinaryWriter prnWriter,
                                      bool embeddedStream,
                                      string fontName)
        {
            const int lenHPFontName = 16;
            const int lenBuf = 256;

            byte[] buffer = new byte[lenBuf];

            int indBuf,
                  len;

            indBuf = 0;

            //----------------------------------------------------------------//

            len = fontName.Length;

            if (len < lenHPFontName)
            {
                string nameFiller = new string('\x20', lenHPFontName - len);

                AddAttrUbyteArray(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.Tag.FontName,
                                  fontName + nameFiller);
            }
            else
            {
                AddAttrUbyteArray(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.Tag.FontName,
                                  fontName);
            }

            AddOperator(ref buffer,
                        ref indBuf,
                        PCLXLOperators.Tag.RemoveFont);

            WriteStreamBlock(prnWriter, embeddedStream,
                             buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C h a r S i z e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return 'session units' equivalent of specified pointsize.          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static float GetCharSize(float pointSize)
        {
            return pointSize * _sessionUPI / _pointsPerInch;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i m a g e B e g i n                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate BeginImage operator and associated attribute list to      //
        // define the metrics of a raster image.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void ImageBegin(BinaryWriter prnWriter,
                                       bool embeddedStream,
                                       ushort srcWidth,
                                       ushort srcHeight,
                                       ushort destWidth,
                                       ushort destHeight,
                                       PCLXLAttrEnums.Val colorMapping,
                                       PCLXLAttrEnums.Val colorDepth)
        {
            const int lenBuf = 64;

            byte[] buffer = new byte[lenBuf];

            int indBuf = 0;

            AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.Tag.ColorMapping,
                                     (byte)colorMapping);

            AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.Tag.ColorDepth,
                                     (byte)colorDepth);

            AddAttrUint16(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.Tag.SourceWidth,
                                      srcWidth);

            AddAttrUint16(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.Tag.SourceHeight,
                                      srcHeight);

            AddAttrUint16XY(ref buffer,
                                        ref indBuf,
                                        PCLXLAttributes.Tag.DestinationSize,
                                        destWidth, destHeight);

            AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.Tag.BeginImage);

            WriteStreamBlock(prnWriter, embeddedStream,
                                         buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i m a g e E n d                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate EndImage operator.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void ImageEnd(BinaryWriter prnWriter,
                                     bool embeddedStream)
        {
            const int lenBuf = 4;

            byte[] buffer = new byte[lenBuf];

            int indBuf = 0;

            AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.Tag.EndImage);

            WriteStreamBlock(prnWriter, embeddedStream,
                                         buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i m a g e R e a d                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate ReadImage operator and associated attribute list to       //
        // define the contents of (a block) of a raster image.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void ImageRead(BinaryWriter prnWriter,
                                      bool embeddedStream,
                                      ushort startLine,
                                      ushort blockHeight,
                                      PCLXLAttrEnums.Val compressMode,
                                      byte[] data)
        {
            const int lenBuf = 64;

            byte[] buffer = new byte[lenBuf];

            int indBuf,
                  dataLen;

            indBuf = 0;
            dataLen = data.Length;

            AddAttrUint16(ref buffer,
                                       ref indBuf,
                                       PCLXLAttributes.Tag.StartLine,
                                       startLine);

            AddAttrUint16(ref buffer,
                                       ref indBuf,
                                       PCLXLAttributes.Tag.BlockHeight,
                                       blockHeight);

            AddAttrUbyte(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.Tag.CompressMode,
                                      (byte)compressMode);

            AddAttrUbyte(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.Tag.PadBytesMultiple,
                                      1);

            AddAttrUint32(ref buffer,
                                       ref indBuf,
                                       PCLXLAttributes.Tag.BlockByteLength,
                                       (ushort)dataLen);

            AddOperator(ref buffer,
                                     ref indBuf,
                                     PCLXLOperators.Tag.ReadImage);

            //----------------------------------------------------------------//

            AddEmbedDataIntro(ref buffer,
                                          ref indBuf,
                                          dataLen);

            WriteStreamBlock(prnWriter, embeddedStream,
                                         buffer, ref indBuf);

            WriteStreamBlock(prnWriter, embeddedStream,
                                          data, ref dataLen);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a g e B e g i n                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate BeginPage operator and associated attribute list.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void PageBegin(BinaryWriter prnWriter,
                                     int indxPaperSize,
                                     int indxPaperType,
                                     int indxPaperTray,
                                     int indxOrientation,
                                     int indxPlexMode,
                                     bool flagFirstPage,
                                     bool flagFrontFace)
        {
            const int lenBuf = 1024;

            byte[] buffer = new byte[lenBuf];

            int indBuf;

            bool flagSimplexJob = PCLPlexModes.IsSimplex(indxPlexMode);

            indBuf = 0;

            if (indxOrientation < PCLOrientations.GetCount())
            {
                AddAttrUbyte(
                    ref buffer,
                    ref indBuf,
                    PCLXLAttributes.Tag.Orientation,
                    PCLOrientations.GetIdPCLXL(indxOrientation));
            }

            if (indxPaperSize < PCLPaperSizes.GetCount())
            {
                AddAttrUbyte(
                    ref buffer,
                    ref indBuf,
                    PCLXLAttributes.Tag.MediaSize,
                    PCLPaperSizes.GetIdPCLXL(indxPaperSize));
            }

            if (indxPaperTray >= 0)                 // -ve value indicates <not set>
            {
                if (indxPaperTray < 256)
                {
                    AddAttrUbyte(ref buffer,
                                       ref indBuf,
                                       PCLXLAttributes.Tag.MediaSource,
                                       (byte)indxPaperTray);
                }
                else
                {
                    AddAttrUint16(ref buffer,
                                       ref indBuf,
                                       PCLXLAttributes.Tag.MediaSource,
                                       (ushort)indxPaperTray);
                }
            }

            if (flagFirstPage)
            {
                if ((indxPaperType < PCLPaperTypes.GetCount()) &&
                    (PCLPaperTypes.GetType(indxPaperType) !=
                        PCLPaperTypes.EntryType.NotSet))
                {
                    AddAttrUbyteArray(
                        ref buffer,
                        ref indBuf,
                        PCLXLAttributes.Tag.MediaType,
                        PCLPaperTypes.GetName(indxPaperType));
                }

                if (flagSimplexJob)
                {
                    AddAttrUbyte(
                        ref buffer,
                        ref indBuf,
                        PCLXLAttributes.Tag.SimplexPageMode,
                        (byte)PCLXLAttrEnums.Val.eSimplexFrontSide);
                }
            }

            if (!flagSimplexJob)
            {
                bool flagLandscape =
                    PCLOrientations.IsLandscape(indxOrientation);

                byte binding = PCLPlexModes.GetIdPCLXL(indxPlexMode,
                                                       flagLandscape);

                AddAttrUbyte(
                    ref buffer,
                    ref indBuf,
                    PCLXLAttributes.Tag.DuplexPageMode,
                    binding);

                if (flagFrontFace)
                {
                    AddAttrUbyte(
                        ref buffer,
                        ref indBuf,
                        PCLXLAttributes.Tag.DuplexPageSide,
                        (byte)PCLXLAttrEnums.Val.eFrontMediaSide);
                }
                else
                {
                    AddAttrUbyte(
                        ref buffer,
                        ref indBuf,
                        PCLXLAttributes.Tag.DuplexPageSide,
                        (byte)PCLXLAttrEnums.Val.eBackMediaSide);
                }
            }

            AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.Tag.BeginPage);

            AddAttrUint16XY(ref buffer,
                                        ref indBuf,
                                        PCLXLAttributes.Tag.PageOrigin,
                                        0, 0);

            AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.Tag.SetPageOrigin);

            prnWriter.Write(buffer, 0, indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a g e E n d                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate EndPage operator and associated attribute list.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void PageEnd(BinaryWriter prnWriter,
                                   ushort pageCopies)
        {
            const int lenBuf = 64;

            byte[] buffer = new byte[lenBuf];

            int indBuf = 0;

            AddAttrUint16(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.Tag.PageCopies,
                           pageCopies);

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.EndPage);

            prnWriter.Write(buffer, 0, indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a t t e r n B e g i n                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate BeginRastPattern operator and associated attribute list.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void PatternBegin(BinaryWriter prnWriter,
                                         bool embeddedStream,
                                         short patternID,
                                         ushort patWidth,
                                         ushort patHeight,
                                         ushort destWidth,
                                         ushort destHeight,
                                         PCLXLAttrEnums.Val colorMapping,
                                         PCLXLAttrEnums.Val colorDepth,
                                         PCLXLAttrEnums.Val persistence,
                                         PCLXLAttrEnums.Val compressMode)
        {
            const int lenBuf = 64;

            byte[] buffer = new byte[lenBuf];

            int indBuf = 0;

            AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.Tag.ColorMapping,
                                     (byte)colorMapping);

            AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.Tag.ColorDepth,
                                     (byte)colorDepth);

            AddAttrUint16(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.Tag.SourceWidth,
                                      patWidth);

            AddAttrUint16(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.Tag.SourceHeight,
                                      patHeight);

            AddAttrUint16XY(ref buffer,
                                        ref indBuf,
                                        PCLXLAttributes.Tag.DestinationSize,
                                        destWidth, destHeight);

            AddAttrSint16(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.Tag.PatternDefineID,
                                      patternID);

            AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.Tag.PatternPersistence,
                                     (byte)persistence);

            AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.Tag.BeginRastPattern);

            WriteStreamBlock(prnWriter, embeddedStream,
                                         buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a t t e r n D e f i n e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate BeginRastPattern, ReadRastPattern and EndRastPattern      //
        // operators and associated attribute lists to sequences to define a  //
        // raster pattern.                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void PatternDefine(BinaryWriter prnWriter,
                                         bool embeddedStream,
                                         short patternID,
                                         ushort patWidth,
                                         ushort patHeight,
                                         ushort destWidth,
                                         ushort destHeight,
                                         PCLXLAttrEnums.Val colorMapping,
                                         PCLXLAttrEnums.Val colorDepth,
                                         PCLXLAttrEnums.Val persistence,
                                         PCLXLAttrEnums.Val compressMode,
                                         byte[] pattern)
        {
            const int lenBuf = 64;

            byte[] buffer = new byte[lenBuf];

            int indBuf,
                  patLen;

            indBuf = 0;
            patLen = pattern.Length;

            AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.Tag.ColorMapping,
                                     (byte)colorMapping);

            AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.Tag.ColorDepth,
                                     (byte)colorDepth);

            AddAttrUint16(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.Tag.SourceWidth,
                                      patWidth);

            AddAttrUint16(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.Tag.SourceHeight,
                                      patHeight);

            AddAttrUint16XY(ref buffer,
                                        ref indBuf,
                                        PCLXLAttributes.Tag.DestinationSize,
                                        destWidth, destHeight);

            AddAttrSint16(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.Tag.PatternDefineID,
                                      patternID);

            AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.Tag.PatternPersistence,
                                     (byte)persistence);

            AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.Tag.BeginRastPattern);

            //----------------------------------------------------------------//

            AddAttrUint16(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.Tag.StartLine,
                                      0);

            AddAttrUint16(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.Tag.BlockHeight,
                                      patHeight);

            AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.Tag.CompressMode,
                                     (byte)compressMode);

            AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.Tag.PadBytesMultiple,
                                     1);

            AddAttrUint32(ref buffer,
                                       ref indBuf,
                                       PCLXLAttributes.Tag.BlockByteLength,
                                       (ushort)patLen);

            AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.Tag.ReadRastPattern);

            //----------------------------------------------------------------//

            AddEmbedDataIntro(ref buffer,
                                          ref indBuf,
                                          patLen);

            WriteStreamBlock(prnWriter, embeddedStream,
                                         buffer, ref indBuf);

            WriteStreamBlock(prnWriter, embeddedStream,
                                          pattern, ref patLen);

            //----------------------------------------------------------------//

            AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.Tag.EndRastPattern);

            WriteStreamBlock(prnWriter, embeddedStream,
                                         buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a t t e r n E n d                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate EndRastPattern operator and associated attribute list.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void PatternEnd(BinaryWriter prnWriter,
                                         bool embeddedStream)
        {
            const int lenBuf = 16;

            byte[] buffer = new byte[lenBuf];

            int indBuf = 0;

            //----------------------------------------------------------------//

            AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.Tag.EndRastPattern);

            WriteStreamBlock(prnWriter, embeddedStream,
                                         buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a t t e r n R e a d                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate ReadRastPattern operator and associated attribute list.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void PatternRead(BinaryWriter prnWriter,
                                         bool embeddedStream,
                                         ushort startLine,
                                         ushort blockHeight,
                                         PCLXLAttrEnums.Val compressMode,
                                         byte[] pattern)
        {
            const int lenBuf = 64;

            byte[] buffer = new byte[lenBuf];

            int indBuf,
                  patLen;

            indBuf = 0;
            patLen = pattern.Length;

            //----------------------------------------------------------------//

            AddAttrUint16(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.Tag.StartLine,
                                      startLine);

            AddAttrUint16(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.Tag.BlockHeight,
                                      blockHeight);

            AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.Tag.CompressMode,
                                     (byte)compressMode);

            AddAttrUbyte(ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.Tag.PadBytesMultiple,
                                     1);

            AddAttrUint32(ref buffer,
                                       ref indBuf,
                                       PCLXLAttributes.Tag.BlockByteLength,
                                       (ushort)patLen);

            AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.Tag.ReadRastPattern);

            //----------------------------------------------------------------//

            AddEmbedDataIntro(ref buffer,
                                          ref indBuf,
                                          patLen);

            WriteStreamBlock(prnWriter, embeddedStream,
                                         buffer, ref indBuf);

            WriteStreamBlock(prnWriter, embeddedStream,
                                          pattern, ref patLen);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e c t a n g l e                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate operators and attribute lists for a rectangle.            //
        // The outline (stroke) and fill properties of the rectangle are      //
        // defined externally (via Pen and Brush definitions).                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void Rectangle(BinaryWriter prnWriter,
                                      bool embeddedStream,
                                      ushort coordX,
                                      ushort coordY,
                                      ushort height,
                                      ushort width)
        {
            int lenBuf;

            int indBuf = 0;

            lenBuf = cSizeAttrUint16Box +
                     cSizeOperator;

            byte[] buffer = new byte[lenBuf];

            //----------------------------------------------------------------//

            AddAttrUint16Box(ref buffer,
                              ref indBuf,
                              PCLXLAttributes.Tag.BoundingBox,
                              coordX,
                              coordY,
                              (ushort)(coordX + width),
                              (ushort)(coordY + height));

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.Rectangle);

            WriteStreamBlock(prnWriter, embeddedStream,
                              buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s t d J o b H e a d e r                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate and write standard job header sequences.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void StdJobHeader(BinaryWriter prnWriter,
                                        string pjlCommand)
        {
            const int lenStd = 32;

            byte[] bufStd = new byte[lenStd];

            string seq;

            int indStd;

            seq = "\x1b" + "%-12345X";          // Universal Exit Language

            prnWriter.Write(seq.ToCharArray(), 0, seq.Length);

            if (pjlCommand != string.Empty)
            {
                seq = pjlCommand + "\x0d" + "\x0a";

                prnWriter.Write(seq.ToCharArray(), 0, seq.Length);
            }

            seq = "@PJL Enter Language = PCLXL\x0d" + "\x0a" +
                  ") HP-PCL XL;2;0;PCL Paraphernalia\x0a";

            prnWriter.Write(seq.ToCharArray(), 0, seq.Length);

            indStd = 0;

            AddAttrUbyte(ref bufStd,
                         ref indStd,
                         PCLXLAttributes.Tag.Measure,
                         (byte)PCLXLAttrEnums.Val.eInch);

            AddAttrUint16XY(ref bufStd,
                            ref indStd,
                            PCLXLAttributes.Tag.UnitsPerMeasure,
                            _sessionUPI,
                            _sessionUPI);

            AddAttrUbyte(ref bufStd,
                         ref indStd,
                         PCLXLAttributes.Tag.ErrorReport,
                         (byte)PCLXLAttrEnums.Val.eErrorPage);

            AddOperator(ref bufStd,
                        ref indStd,
                        PCLXLOperators.Tag.BeginSession);

            AddAttrUbyte(ref bufStd,
                         ref indStd,
                         PCLXLAttributes.Tag.SourceType,
                         (byte)PCLXLAttrEnums.Val.eDefaultDataSource);

            AddAttrUbyte(ref bufStd,
                         ref indStd,
                         PCLXLAttributes.Tag.DataOrg,
                         (byte)PCLXLAttrEnums.Val.eBinaryLowByteFirst);

            AddOperator(ref bufStd,
                        ref indStd,
                        PCLXLOperators.Tag.OpenDataSource);

            prnWriter.Write(bufStd, 0, indStd);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s t d J o b T r a i l e r                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate and write standard job trailer sequences.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void StdJobTrailer(BinaryWriter prnWriter,
                                         bool embeddedStream,
                                         string streamName)
        {
            const int lenStd = 64;

            byte[] bufStd = new byte[lenStd];

            int indStd = 0;

            if (embeddedStream)
            {
                AddAttrUbyteArray(ref bufStd,
                                  ref indStd,
                                  PCLXLAttributes.Tag.StreamName,
                                  streamName);

                AddOperator(ref bufStd,
                            ref indStd,
                            PCLXLOperators.Tag.RemoveStream);
            }

            AddOperator(ref bufStd,
                        ref indStd,
                        PCLXLOperators.Tag.CloseDataSource);

            AddOperator(ref bufStd,
                        ref indStd,
                        PCLXLOperators.Tag.EndSession);

            // seq: Universal Exit Language
            bufStd[indStd++] = 0x1b;          // {esc}
            bufStd[indStd++] = 0x25;          // %
            bufStd[indStd++] = 0x2d;          // -
            bufStd[indStd++] = 0x31;          // 1
            bufStd[indStd++] = 0x32;          // 2
            bufStd[indStd++] = 0x33;          // 3
            bufStd[indStd++] = 0x34;          // 4
            bufStd[indStd++] = 0x35;          // 5
            bufStd[indStd++] = 0x58;          // X

            prnWriter.Write(bufStd, 0, indStd);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s t r e a m B e g i n                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate and write BeginStream operator and associated attribute   //
        // list.                                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void StreamBegin(BinaryWriter prnWriter,
                                       string streamName)
        {
            const int lenBuf = 256;

            byte[] buffer = new byte[lenBuf];

            int indBuf = 0;

            AddAttrUbyteArray(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.StreamName,
                               streamName);

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.BeginStream);

            prnWriter.Write(buffer, 0, indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s t r e a m E n d                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate and write EndStream operator.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void StreamEnd(BinaryWriter prnWriter)
        {
            const int lenBuf = 64;

            byte[] buffer = new byte[lenBuf];

            int indBuf = 0;

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.EndStream);

            prnWriter.Write(buffer, 0, indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s t r e a m E x e c                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate and write ExecStream operator and associated attribute    //
        // list.                                                              //
        //                                                                    //
        // Note that the existing application version does not use nested     //
        // streams; this is because:                                          //
        //  - PCL XL Class/Revision 2.0 does not support nested streams;      //
        //  - Even with a C/R 2.1 stream sent to a local LJ M475dn, nested    //
        //    streams produce a PCL XL error, so I've no way of testing.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void StreamExec(BinaryWriter prnWriter,
                                      bool embeddedStream,
                                      string streamName)
        {
            const int lenBuf = 64;

            byte[] buffer = new byte[lenBuf];

            int indBuf = 0;

            AddAttrUbyteArray(ref buffer,
                                          ref indBuf,
                                          PCLXLAttributes.Tag.StreamName,
                                          streamName);

            AddOperator(ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.Tag.ExecStream);

            WriteStreamBlock(prnWriter, embeddedStream,
                              buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s t r e a m H e a d e r                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate and write standard user-defined stream header sequences.  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void StreamHeader(BinaryWriter prnWriter,
                                        bool embeddedStream,
                                        string streamName)
        {
            const int lenBuf = 64;

            byte[] buffer = new byte[lenBuf];

            int indBuf = 0;

            if (embeddedStream)
            {
                AddAttrUbyteArray(ref buffer,
                                   ref indBuf,
                                   PCLXLAttributes.Tag.StreamName,
                                   streamName);

                AddOperator(ref buffer,
                             ref indBuf,
                             PCLXLOperators.Tag.BeginStream);

                prnWriter.Write(buffer, 0, indBuf);
                indBuf = 0;
            }

            // stream header
            buffer[indBuf++] = 0x29;    // ) = binding format (binary L-E)
            buffer[indBuf++] = 0x20;    //   = reserved byte 
            buffer[indBuf++] = 0x48;    // H = stream class name
            buffer[indBuf++] = 0x50;    // P
            buffer[indBuf++] = 0x2d;    // -
            buffer[indBuf++] = 0x50;    // P
            buffer[indBuf++] = 0x43;    // C
            buffer[indBuf++] = 0x4c;    // L
            buffer[indBuf++] = 0x20;    //  
            buffer[indBuf++] = 0x58;    // X
            buffer[indBuf++] = 0x4c;    // L
            buffer[indBuf++] = 0x3b;    // ;
            buffer[indBuf++] = 0x32;    // 2 = protocol class number
            buffer[indBuf++] = 0x3b;    // ;
            buffer[indBuf++] = 0x30;    // 0 = protocol class revision
            buffer[indBuf++] = 0x3b;    // ; 
            buffer[indBuf++] = 0x63;    // c = comment
            buffer[indBuf++] = 0x6f;    // o
            buffer[indBuf++] = 0x6d;    // m
            buffer[indBuf++] = 0x6d;    // m
            buffer[indBuf++] = 0x65;    // e
            buffer[indBuf++] = 0x6e;    // n
            buffer[indBuf++] = 0x74;    // t
            buffer[indBuf++] = 0x0a;    //   = terminator

            AddAttrUint16XY(ref buffer,
                            ref indBuf,
                            PCLXLAttributes.Tag.UnitsPerMeasure,
                            _sessionUPI,
                            _sessionUPI);

            AddAttrUbyte(ref buffer,
                         ref indBuf,
                         PCLXLAttributes.Tag.Measure,
                         (byte)PCLXLAttrEnums.Val.eInch);

            AddOperator(ref buffer,
                        ref indBuf,
                        PCLXLOperators.Tag.SetPageScale);

            WriteStreamBlock(prnWriter, embeddedStream,
                             buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s t r e a m R e m o v e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate user-defined stream remove operator and associated        //
        // Attribute List for for specified stream name.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void StreamRemove(BinaryWriter prnWriter,
                                        string streamName)
        {
            const int lenBuf = 256;

            byte[] buffer = new byte[lenBuf];

            int indBuf = 0;

            //----------------------------------------------------------------//

            AddAttrUbyteArray(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.StreamName,
                               streamName);

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.Tag.RemoveStream);

            WriteStreamBlock(prnWriter, false,
                              buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t e x t                                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate 'Text' operator and associated Attribute List for         //
        // specified text string, position and size.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void Text(BinaryWriter prnWriter,
                                bool embeddedStream,
                                bool relativePoint,
                                short[] advanceTable,
                                float pointSize,
                                short coordX,
                                short coordY,
                                string text)
        {
            const int lenBuf = 512; // need to cater for text up to 180 bytes?

            byte[] buffer = new byte[lenBuf];

            int indBuf;

            short textLen;

            float scaleFactor,
                   advance;

            indBuf = 0;

            scaleFactor = pointSize / cAdvanceTablePPI;

            //----------------------------------------------------------------//

            textLen = (short)text.Length;

            short[] tmpAdvance = new short[textLen];

            for (int i = 0; i < textLen; i++)
            {
                advance = advanceTable[(byte)text[i] - 0x20];
                tmpAdvance[i] = (short)(advance * scaleFactor);
            }

            AddAttrSint16XY(ref buffer,
                            ref indBuf,
                            PCLXLAttributes.Tag.Point,
                            coordX, coordY);

            if (relativePoint)
                AddOperator(ref buffer, ref indBuf, PCLXLOperators.Tag.SetCursorRel);
            else
                AddOperator(ref buffer, ref indBuf, PCLXLOperators.Tag.SetCursor);

            AddAttrUbyteArray(ref buffer,
                              ref indBuf,
                              PCLXLAttributes.Tag.TextData,
                              text);

            AddAttrUbyteArray(ref buffer,
                              ref indBuf,
                              PCLXLAttributes.Tag.XSpacingData,
                              textLen,
                              tmpAdvance);

            AddOperator(ref buffer,
                        ref indBuf,
                        PCLXLOperators.Tag.Text);

            //----------------------------------------------------------------//

            WriteStreamBlock(prnWriter, embeddedStream,
                             buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t e x t A d v a n c e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return advance associated with supplied text string and size.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static short TextAdvance(short[] advanceTable,
                                         float pointSize,
                                         string text)
        {
            short textLen;

            float scaleFactor,
                   advance;

            short totalAdvance = 0;

            scaleFactor = pointSize / cAdvanceTablePPI;

            //----------------------------------------------------------------//

            textLen = (short)text.Length;

            for (int i = 0; i < textLen; i++)
            {
                advance = advanceTable[(byte)text[i] - 0x20];
                totalAdvance += (short)(advance * scaleFactor);
            }

            return totalAdvance;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t e x t C h a r                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate 'Text' operator and associated Attribute List for         //
        // specified single character, position and size.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void TextChar(BinaryWriter prnWriter,
                                     bool embeddedStream,
                                     short advance,
                                     float pointSize,
                                     short coordX,
                                     short coordY,
                                     ushort codePoint)
        {
            const int lenBuf = 512; // need to cater for text up to 180 bytes?

            byte[] buffer = new byte[lenBuf];

            short[] advanceArray = { 0x00 };
            ushort[] codePointArray = { 0x00 };

            int indBuf = 0;

            //----------------------------------------------------------------//

            AddAttrSint16XY(ref buffer,
                            ref indBuf,
                            PCLXLAttributes.Tag.Point,
                            coordX, coordY);

            AddOperator(ref buffer,
                        ref indBuf,
                        PCLXLOperators.Tag.SetCursor);

            codePointArray[0] = codePoint;
            advanceArray[0] = advance;

            AddAttrUint16Array(ref buffer,
                              ref indBuf,
                              PCLXLAttributes.Tag.TextData,
                              1,
                              codePointArray);

            AddAttrUbyteArray(ref buffer,
                              ref indBuf,
                              PCLXLAttributes.Tag.XSpacingData,
                              1,
                              advanceArray);

            AddOperator(ref buffer,
                        ref indBuf,
                        PCLXLOperators.Tag.Text);

            //----------------------------------------------------------------//

            WriteStreamBlock(prnWriter, embeddedStream,
                             buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t e x t A n g l e d                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate 'Text' operator and associated Attribute List for         //
        // specified text string, position, size and angle.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void TextAngled(BinaryWriter prnWriter,
                                       bool embeddedStream,
                                       short[] advanceTable,
                                       float pointSize,
                                       short coordX,
                                       short coordY,
                                       short angle,
                                       string text)
        {
            const int lenBuf = 512; // need to cater for text up to 180 bytes?

            byte[] buffer = new byte[lenBuf];

            int indBuf;

            short textLen;

            float scaleFactor,
                   advance;

            double radians,
                   tangent;

            indBuf = 0;

            scaleFactor = pointSize / cAdvanceTablePPI;

            // Calculate the tangent of specified angle (given in degrees).
            // If angle is positive, make tangent negative to cause Y decrement
            // If angle is negative, make tangent positive to cause Y increment

            radians = angle * (Math.PI / 180);
            tangent = -Math.Tan(radians);

            //----------------------------------------------------------------//

            textLen = (short)text.Length;

            short[] tmpAdvanceX = new short[textLen];
            short[] tmpAdvanceY = new short[textLen];

            for (int i = 0; i < textLen; i++)
            {
                advance = advanceTable[(byte)text[i] - 0x20];
                tmpAdvanceX[i] = (short)(advance * scaleFactor);
                tmpAdvanceY[i] = (short)(advance * scaleFactor * tangent);
            }

            AddAttrSint16XY(ref buffer,
                            ref indBuf,
                            PCLXLAttributes.Tag.Point,
                            coordX, coordY);

            AddOperator(ref buffer,
                        ref indBuf,
                        PCLXLOperators.Tag.SetCursor);

            AddAttrUbyteArray(ref buffer,
                              ref indBuf,
                              PCLXLAttributes.Tag.TextData,
                              text);

            AddAttrUbyteArray(ref buffer,
                              ref indBuf,
                              PCLXLAttributes.Tag.XSpacingData,
                              textLen,
                              tmpAdvanceX);

            AddAttrSint16Array(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.Tag.YSpacingData,
                               textLen,
                               tmpAdvanceY);

            AddOperator(ref buffer,
                        ref indBuf,
                        PCLXLOperators.Tag.Text);

            //----------------------------------------------------------------//

            WriteStreamBlock(prnWriter, embeddedStream,
                             buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // w r i t e O p e r a t o r                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write specified operator.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void WriteOperator(BinaryWriter prnWriter,
                                         PCLXLOperators.Tag opTag,
                                         bool embeddedStream)
        {
            const int lenBuf = 64;

            byte[] buffer = new byte[lenBuf];

            int indBuf = 0;

            AddOperator(ref buffer,
                         ref indBuf,
                         opTag);

            WriteStreamBlock(prnWriter, embeddedStream, buffer, ref indBuf);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // w r i t e S t r e a m B l o c k                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write out contents of supplied buffer(s), pre-pending this with a  //
        // ReadStream Operator and its associated Attribute List if writing   //
        // an embedded stream.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void WriteStreamBlock(BinaryWriter prnWriter,
                                            bool embeddedStream,
                                            byte[] buffer,
                                            ref int indBuf)
        {
            if (embeddedStream)
            {
                int lenTemp;
                int indTemp = 0;

                lenTemp = cSizeAttrUint32 + cSizeOperator;

                if (indBuf > 256)
                    lenTemp += cSizeEmbedDataInt32;
                else
                    lenTemp += cSizeEmbedDataByte;

                byte[] bufTemp = new byte[lenTemp];

                AddAttrUint32(ref bufTemp,
                              ref indTemp,
                              PCLXLAttributes.Tag.StreamDataLength,
                              (uint)indBuf);

                AddOperator(ref bufTemp,
                            ref indTemp,
                            PCLXLOperators.Tag.ReadStream);

                AddEmbedDataIntro(ref bufTemp,
                                  ref indTemp,
                                  indBuf);

                prnWriter.Write(bufTemp, 0, indTemp);
            }

            //----------------------------------------------------------------//

            prnWriter.Write(buffer, 0, indBuf);
            indBuf = 0;
        }
    }
}
