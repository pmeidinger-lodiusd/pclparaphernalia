using System;
using System.IO;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides primitive and macro operations for PCL XL print language.
/// 
/// © Chris Hutchinson 2010
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
    private const int cSizeAttrSint16ArrayBase = 6;
    private const int cSizeAttrSint16ArrayUnit = 2;
    private const int cSizeAttrSint16Box = 11;
    private const int cSizeAttrSint16XY = 7;

    private const int cSizeAttrUbyte = 4;
    private const int cSizeAttrUbyteArrayBase = 6;
    private const int cSizeAttrUbyteArrayUnit = 1;

    private const int cSizeAttrUint16 = 5;
    private const int cSizeAttrUint16ArrayBase = 6;
    private const int cSizeAttrUint16ArrayUnit = 2;
    private const int cSizeAttrUint16Box = 11;
    private const int cSizeAttrUint16XY = 7;

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
                                     PCLXLAttributes.eTag attributeTag,
                                     float valReal32)
    {
        byte[] tempArray;

        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Real32;

        tempArray = BitConverter.GetBytes(valReal32);

        buffer[bufIndex++] = tempArray[0];
        buffer[bufIndex++] = tempArray[1];
        buffer[bufIndex++] = tempArray[2];
        buffer[bufIndex++] = tempArray[3];

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
                                       PCLXLAttributes.eTag attributeTag,
                                       float valReal32_x,
                                       float valReal32_y)
    {
        byte[] tempArray;

        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Real32XY;

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

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
                                     PCLXLAttributes.eTag attributeTag,
                                     short valSint16)
    {
        byte[] tempArray;

        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Sint16;

        tempArray = BitConverter.GetBytes(valSint16);

        buffer[bufIndex++] = tempArray[0];
        buffer[bufIndex++] = tempArray[1];

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
                                          PCLXLAttributes.eTag attributeTag,
                                          short arraySize,
                                          short[] arraySint16)
    {
        byte[] tempArray;

        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Sint16Array;
        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Uint16;

        tempArray = BitConverter.GetBytes(arraySize);

        buffer[bufIndex++] = tempArray[0];
        buffer[bufIndex++] = tempArray[1];

        for (int i = 0; i < arraySize; i++)
        {
            tempArray = BitConverter.GetBytes(arraySint16[i]);
            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];
        }

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
                                        PCLXLAttributes.eTag attributeTag,
                                        short valSint16_x1,
                                        short valSint16_y1,
                                        short valSint16_x2,
                                        short valSint16_y2)
    {
        byte[] tempArray;

        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Sint16Box;

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

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
                                       PCLXLAttributes.eTag attributeTag,
                                       short valSint16_x,
                                       short valSint16_y)
    {
        byte[] tempArray;

        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Sint16XY;

        tempArray = BitConverter.GetBytes(valSint16_x);

        buffer[bufIndex++] = tempArray[0];
        buffer[bufIndex++] = tempArray[1];

        tempArray = BitConverter.GetBytes(valSint16_y);

        buffer[bufIndex++] = tempArray[0];
        buffer[bufIndex++] = tempArray[1];

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
                                    PCLXLAttributes.eTag attributeTag,
                                    byte valUbyte)
    {
        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Ubyte;

        buffer[bufIndex++] = valUbyte;

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
                                         PCLXLAttributes.eTag attributeTag,
                                         short arraySize,
                                         short[] arraySint16)
    {
        byte[] tempArray;

        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.UbyteArray;
        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Uint16;

        tempArray = BitConverter.GetBytes(arraySize);

        buffer[bufIndex++] = tempArray[0];
        buffer[bufIndex++] = tempArray[1];

        for (int i = 0; i < arraySize; i++)
        {
            tempArray = BitConverter.GetBytes(arraySint16[i]);
            buffer[bufIndex++] = tempArray[0];
        }

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
                                         PCLXLAttributes.eTag attributeTag,
                                         short arraySize,
                                         byte[] arrayUbyte)
    {
        byte[] tempArray;

        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.UbyteArray;
        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Uint16;

        tempArray = BitConverter.GetBytes(arraySize);

        buffer[bufIndex++] = tempArray[0];
        buffer[bufIndex++] = tempArray[1];

        for (int i = 0; i < arraySize; i++)
        {
            buffer[bufIndex++] = arrayUbyte[i];
        }

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
                                         PCLXLAttributes.eTag attributeTag,
                                         string valString)
    {
        byte[] tempArray;
        short arraySize;

        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.UbyteArray;
        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Uint16;

        arraySize = (short)valString.Length;
        tempArray = BitConverter.GetBytes(arraySize);

        buffer[bufIndex++] = tempArray[0];
        buffer[bufIndex++] = tempArray[1];

        for (int i = 0; i < arraySize; i++)
        {
            buffer[bufIndex++] = (byte)valString[i];
        }

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
                                     PCLXLAttributes.eTag attributeTag,
                                     ushort valUint16)
    {
        byte[] tempArray;

        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Uint16;

        tempArray = BitConverter.GetBytes(valUint16);

        buffer[bufIndex++] = tempArray[0];
        buffer[bufIndex++] = tempArray[1];

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
                                          PCLXLAttributes.eTag attributeTag,
                                          short arraySize,
                                          ushort[] arrayUint16)
    {
        byte[] tempArray;

        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Uint16Array;
        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Uint16;

        tempArray = BitConverter.GetBytes(arraySize);

        buffer[bufIndex++] = tempArray[0];
        buffer[bufIndex++] = tempArray[1];

        for (int i = 0; i < arraySize; i++)
        {
            tempArray = BitConverter.GetBytes(arrayUint16[i]);
            buffer[bufIndex++] = tempArray[0];
            buffer[bufIndex++] = tempArray[1];
        }

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
                                        PCLXLAttributes.eTag attributeTag,
                                        ushort valUint16_x1,
                                        ushort valUint16_y1,
                                        ushort valUint16_x2,
                                        ushort valUint16_y2)
    {
        byte[] tempArray;

        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Uint16Box;

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

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
                                       PCLXLAttributes.eTag attributeTag,
                                       ushort valUint16_x,
                                       ushort valUint16_y)
    {
        byte[] tempArray;

        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Uint16XY;

        tempArray = BitConverter.GetBytes(valUint16_x);

        buffer[bufIndex++] = tempArray[0];
        buffer[bufIndex++] = tempArray[1];

        tempArray = BitConverter.GetBytes(valUint16_y);

        buffer[bufIndex++] = tempArray[0];
        buffer[bufIndex++] = tempArray[1];

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
                                     PCLXLAttributes.eTag attributeTag,
                                     uint valUint32)
    {
        byte[] tempArray;

        buffer[bufIndex++] = (byte)PCLXLDataTypes.eTag.Uint32;

        tempArray = BitConverter.GetBytes(valUint32);

        buffer[bufIndex++] = tempArray[0];
        buffer[bufIndex++] = tempArray[1];
        buffer[bufIndex++] = tempArray[2];
        buffer[bufIndex++] = tempArray[3];

        buffer[bufIndex++] = (byte)PCLXLAttrDefiners.eTag.Ubyte;
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
        byte[] tempArray;

        tempArray = BitConverter.GetBytes(dataLen);

        if (dataLen < 256)
        {
            buffer[bufIndex++] = (byte)PCLXLEmbedDataDefs.eTag.Byte;
            buffer[bufIndex++] = tempArray[0];
        }
        else
        {
            buffer[bufIndex++] = (byte)PCLXLEmbedDataDefs.eTag.Int;
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
                                   PCLXLOperators.eTag operatorTag)
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

        int indBuf;

        indBuf = 0;

        lenBuf = cSizeAttrSint16 +
                 cSizeOperator;

        byte[] buffer = new byte[lenBuf];

        //----------------------------------------------------------------//

        AddAttrSint16(ref buffer,
                       ref indBuf,
                       PCLXLAttributes.eTag.CharAngle,
                       charAngle);

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.SetCharAngle);

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

        int indBuf;

        indBuf = 0;

        lenBuf = cSizeAttrReal32 +
                 cSizeOperator;

        byte[] buffer = new byte[lenBuf];

        //----------------------------------------------------------------//

        AddAttrReal32(ref buffer,
                       ref indBuf,
                       PCLXLAttributes.eTag.CharBoldValue,
                       charBoldValue);

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.SetCharBoldValue);

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

        int indBuf;

        indBuf = 0;

        lenBuf = cSizeAttrReal32XY +
                 cSizeOperator;

        byte[] buffer = new byte[lenBuf];

        //----------------------------------------------------------------//

        AddAttrReal32XY(ref buffer,
                         ref indBuf,
                         PCLXLAttributes.eTag.CharScale,
                         charScaleX,
                         charScaleY);

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.SetCharScale);

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

        int indBuf;

        indBuf = 0;

        lenBuf = cSizeAttrReal32XY +
                 cSizeOperator;

        byte[] buffer = new byte[lenBuf];

        //----------------------------------------------------------------//

        AddAttrReal32XY(ref buffer,
                         ref indBuf,
                         PCLXLAttributes.eTag.CharShear,
                         charShearX,
                         charShearY);

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.SetCharShear);

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

        int indBuf;

        indBuf = 0;

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

        charSize = (pointSize * _sessionUPI) / _pointsPerInch;

        len = fontName.Length;

        if (len < lenHPFontName)
        {
            string nameFiller = new string('\x20', lenHPFontName - len);

            AddAttrUbyteArray(ref buffer,
                              ref indBuf,
                              PCLXLAttributes.eTag.FontName,
                              fontName + nameFiller);
        }
        else
        {
            AddAttrUbyteArray(ref buffer,
                              ref indBuf,
                              PCLXLAttributes.eTag.FontName,
                              fontName);
        }

        AddAttrUint16(ref buffer,
                      ref indBuf,
                      PCLXLAttributes.eTag.SymbolSet,
                      symbolSet);

        AddAttrReal32(ref buffer,
                      ref indBuf,
                      PCLXLAttributes.eTag.CharSize,
                      charSize);

        AddOperator(ref buffer,
                    ref indBuf,
                    PCLXLOperators.eTag.SetFont);

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

        int indBuf;

        indBuf = 0;

        lenBuf = cSizeAttrUbyteArrayBase +
                 (cSizeAttrUbyteArrayUnit * fontNameLen) +
                 cSizeOperator;

        byte[] buffer = new byte[lenBuf];

        //----------------------------------------------------------------//

        AddAttrUbyteArray(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.eTag.FontName,
                           fontNameLen,
                           fontName);

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.BeginChar);

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

        int indBuf;

        indBuf = 0;

        lenBuf = cSizeOperator;

        byte[] buffer = new byte[lenBuf];

        //----------------------------------------------------------------//

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.EndChar);

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

        int indBuf;

        indBuf = 0;

        lenBuf = cSizeAttrUint16 + cSizeAttrUint16 + cSizeOperator;
        byte[] buffer = new byte[lenBuf];

        //----------------------------------------------------------------//

        AddAttrUint16(ref buffer,
                       ref indBuf,
                       PCLXLAttributes.eTag.CharCode,
                       charCode);

        AddAttrUint16(ref buffer,
                       ref indBuf,
                       PCLXLAttributes.eTag.CharDataSize,
                       charDataSize);

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.ReadChar);

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

        int indBuf;

        indBuf = 0;

        lenBuf = cSizeAttrUbyteArrayBase +
                 (cSizeAttrUbyteArrayUnit * fontNameLen) +
                 cSizeAttrUbyte +
                 cSizeOperator;

        byte[] buffer = new byte[lenBuf];

        //----------------------------------------------------------------//

        AddAttrUbyteArray(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.eTag.FontName,
                           fontNameLen,
                           fontName);

        AddAttrUbyte(ref buffer,
                      ref indBuf,
                      PCLXLAttributes.eTag.FontFormat,
                      fontFormat);

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.BeginFontHeader);

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

        int indBuf;

        indBuf = 0;

        lenBuf = cSizeOperator;

        byte[] buffer = new byte[lenBuf];

        //----------------------------------------------------------------//

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.EndFontHeader);

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

        int indBuf;

        indBuf = 0;

        lenBuf = cSizeAttrUint16 + cSizeOperator;

        byte[] buffer = new byte[lenBuf];

        //----------------------------------------------------------------//

        AddAttrUint16(ref buffer,
                       ref indBuf,
                       PCLXLAttributes.eTag.FontHeaderLength,
                       hddrLen);

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.ReadFontHeader);

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
                              PCLXLAttributes.eTag.FontName,
                              fontName + nameFiller);
        }
        else
        {
            AddAttrUbyteArray(ref buffer,
                              ref indBuf,
                              PCLXLAttributes.eTag.FontName,
                              fontName);
        }

        AddOperator(ref buffer,
                    ref indBuf,
                    PCLXLOperators.eTag.RemoveFont);

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
        return (pointSize * _sessionUPI) / _pointsPerInch;
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
                                   PCLXLAttrEnums.eVal colorMapping,
                                   PCLXLAttrEnums.eVal colorDepth)
    {
        const int lenBuf = 64;

        byte[] buffer = new byte[lenBuf];

        int indBuf;

        indBuf = 0;

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.ColorMapping,
                                 (byte)colorMapping);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.ColorDepth,
                                 (byte)colorDepth);

        PCLXLWriter.AddAttrUint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.SourceWidth,
                                  srcWidth);

        PCLXLWriter.AddAttrUint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.SourceHeight,
                                  srcHeight);

        PCLXLWriter.AddAttrUint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.DestinationSize,
                                    destWidth, destHeight);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.BeginImage);

        PCLXLWriter.WriteStreamBlock(prnWriter, embeddedStream,
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

        int indBuf;

        indBuf = 0;

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.EndImage);

        PCLXLWriter.WriteStreamBlock(prnWriter, embeddedStream,
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
                                  PCLXLAttrEnums.eVal compressMode,
                                  byte[] data)
    {
        const int lenBuf = 64;

        byte[] buffer = new byte[lenBuf];

        int indBuf,
              dataLen;

        indBuf = 0;
        dataLen = data.Length;

        PCLXLWriter.AddAttrUint16(ref buffer,
                                   ref indBuf,
                                   PCLXLAttributes.eTag.StartLine,
                                   startLine);

        PCLXLWriter.AddAttrUint16(ref buffer,
                                   ref indBuf,
                                   PCLXLAttributes.eTag.BlockHeight,
                                   blockHeight);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.CompressMode,
                                  (byte)compressMode);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.PadBytesMultiple,
                                  1);

        PCLXLWriter.AddAttrUint32(ref buffer,
                                   ref indBuf,
                                   PCLXLAttributes.eTag.BlockByteLength,
                                   (ushort)dataLen);

        PCLXLWriter.AddOperator(ref buffer,
                                 ref indBuf,
                                 PCLXLOperators.eTag.ReadImage);

        //----------------------------------------------------------------//

        PCLXLWriter.AddEmbedDataIntro(ref buffer,
                                      ref indBuf,
                                      dataLen);

        PCLXLWriter.WriteStreamBlock(prnWriter, embeddedStream,
                                     buffer, ref indBuf);

        PCLXLWriter.WriteStreamBlock(prnWriter, embeddedStream,
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
            PCLXLWriter.AddAttrUbyte(
                ref buffer,
                ref indBuf,
                PCLXLAttributes.eTag.Orientation,
                PCLOrientations.GetIdPCLXL(indxOrientation));
        }

        if (indxPaperSize < PCLPaperSizes.GetCount())
        {
            PCLXLWriter.AddAttrUbyte(
                ref buffer,
                ref indBuf,
                PCLXLAttributes.eTag.MediaSize,
                PCLPaperSizes.GetIdPCLXL(indxPaperSize));
        }

        if (indxPaperTray >= 0)                 // -ve value indicates <not set>
        {
            if (indxPaperTray < 256)
            {
                PCLXLWriter.AddAttrUbyte(ref buffer,
                                   ref indBuf,
                                   PCLXLAttributes.eTag.MediaSource,
                                   (byte)indxPaperTray);
            }
            else
            {
                PCLXLWriter.AddAttrUint16(ref buffer,
                                   ref indBuf,
                                   PCLXLAttributes.eTag.MediaSource,
                                   (ushort)indxPaperTray);

            }
        }

        if (flagFirstPage)
        {
            if ((indxPaperType < PCLPaperTypes.GetCount()) &&
                (PCLPaperTypes.GetType(indxPaperType) !=
                    PCLPaperTypes.eEntryType.NotSet))
            {
                PCLXLWriter.AddAttrUbyteArray(
                    ref buffer,
                    ref indBuf,
                    PCLXLAttributes.eTag.MediaType,
                    PCLPaperTypes.GetName(indxPaperType));
            }

            if (flagSimplexJob)
            {
                PCLXLWriter.AddAttrUbyte(
                    ref buffer,
                    ref indBuf,
                    PCLXLAttributes.eTag.SimplexPageMode,
                    (byte)PCLXLAttrEnums.eVal.eSimplexFrontSide);
            }
        }

        if (!flagSimplexJob)
        {
            bool flagLandscape =
                PCLOrientations.IsLandscape(indxOrientation);

            byte binding = PCLPlexModes.GetIdPCLXL(indxPlexMode,
                                                   flagLandscape);

            PCLXLWriter.AddAttrUbyte(
                ref buffer,
                ref indBuf,
                PCLXLAttributes.eTag.DuplexPageMode,
                binding);

            if (flagFrontFace)
            {
                PCLXLWriter.AddAttrUbyte(
                    ref buffer,
                    ref indBuf,
                    PCLXLAttributes.eTag.DuplexPageSide,
                    (byte)PCLXLAttrEnums.eVal.eFrontMediaSide);
            }
            else
            {
                PCLXLWriter.AddAttrUbyte(
                    ref buffer,
                    ref indBuf,
                    PCLXLAttributes.eTag.DuplexPageSide,
                    (byte)PCLXLAttrEnums.eVal.eBackMediaSide);
            }
        }

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.BeginPage);

        PCLXLWriter.AddAttrUint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.PageOrigin,
                                    0, 0);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.SetPageOrigin);

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

        int indBuf;

        indBuf = 0;

        AddAttrUint16(ref buffer,
                       ref indBuf,
                       PCLXLAttributes.eTag.PageCopies,
                       pageCopies);

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.EndPage);

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
                                     PCLXLAttrEnums.eVal colorMapping,
                                     PCLXLAttrEnums.eVal colorDepth,
                                     PCLXLAttrEnums.eVal persistence,
                                     PCLXLAttrEnums.eVal compressMode)
    {
        const int lenBuf = 64;

        byte[] buffer = new byte[lenBuf];

        int indBuf;

        indBuf = 0;

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.ColorMapping,
                                 (byte)colorMapping);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.ColorDepth,
                                 (byte)colorDepth);

        PCLXLWriter.AddAttrUint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.SourceWidth,
                                  patWidth);

        PCLXLWriter.AddAttrUint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.SourceHeight,
                                  patHeight);

        PCLXLWriter.AddAttrUint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.DestinationSize,
                                    destWidth, destHeight);

        PCLXLWriter.AddAttrSint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.PatternDefineID,
                                  patternID);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.PatternPersistence,
                                 (byte)persistence);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.BeginRastPattern);

        PCLXLWriter.WriteStreamBlock(prnWriter, embeddedStream,
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
                                     PCLXLAttrEnums.eVal colorMapping,
                                     PCLXLAttrEnums.eVal colorDepth,
                                     PCLXLAttrEnums.eVal persistence,
                                     PCLXLAttrEnums.eVal compressMode,
                                     byte[] pattern)
    {
        const int lenBuf = 64;

        byte[] buffer = new byte[lenBuf];

        int indBuf,
              patLen;

        indBuf = 0;
        patLen = pattern.Length;

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.ColorMapping,
                                 (byte)colorMapping);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.ColorDepth,
                                 (byte)colorDepth);

        PCLXLWriter.AddAttrUint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.SourceWidth,
                                  patWidth);

        PCLXLWriter.AddAttrUint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.SourceHeight,
                                  patHeight);

        PCLXLWriter.AddAttrUint16XY(ref buffer,
                                    ref indBuf,
                                    PCLXLAttributes.eTag.DestinationSize,
                                    destWidth, destHeight);

        PCLXLWriter.AddAttrSint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.PatternDefineID,
                                  patternID);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.PatternPersistence,
                                 (byte)persistence);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.BeginRastPattern);

        //----------------------------------------------------------------//

        PCLXLWriter.AddAttrUint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.StartLine,
                                  0);

        PCLXLWriter.AddAttrUint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.BlockHeight,
                                  patHeight);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.CompressMode,
                                 (byte)compressMode);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.PadBytesMultiple,
                                 1);

        PCLXLWriter.AddAttrUint32(ref buffer,
                                   ref indBuf,
                                   PCLXLAttributes.eTag.BlockByteLength,
                                   (ushort)patLen);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.ReadRastPattern);

        //----------------------------------------------------------------//

        PCLXLWriter.AddEmbedDataIntro(ref buffer,
                                      ref indBuf,
                                      patLen);

        PCLXLWriter.WriteStreamBlock(prnWriter, embeddedStream,
                                     buffer, ref indBuf);

        PCLXLWriter.WriteStreamBlock(prnWriter, embeddedStream,
                                      pattern, ref patLen);

        //----------------------------------------------------------------//

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.EndRastPattern);

        PCLXLWriter.WriteStreamBlock(prnWriter, embeddedStream,
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

        int indBuf;

        indBuf = 0;

        //----------------------------------------------------------------//

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.EndRastPattern);

        PCLXLWriter.WriteStreamBlock(prnWriter, embeddedStream,
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
                                     PCLXLAttrEnums.eVal compressMode,
                                     byte[] pattern)
    {
        const int lenBuf = 64;

        byte[] buffer = new byte[lenBuf];

        int indBuf,
              patLen;

        indBuf = 0;
        patLen = pattern.Length;

        //----------------------------------------------------------------//

        PCLXLWriter.AddAttrUint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.StartLine,
                                  startLine);

        PCLXLWriter.AddAttrUint16(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.BlockHeight,
                                  blockHeight);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.CompressMode,
                                 (byte)compressMode);

        PCLXLWriter.AddAttrUbyte(ref buffer,
                                 ref indBuf,
                                 PCLXLAttributes.eTag.PadBytesMultiple,
                                 1);

        PCLXLWriter.AddAttrUint32(ref buffer,
                                   ref indBuf,
                                   PCLXLAttributes.eTag.BlockByteLength,
                                   (ushort)patLen);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.ReadRastPattern);

        //----------------------------------------------------------------//

        PCLXLWriter.AddEmbedDataIntro(ref buffer,
                                      ref indBuf,
                                      patLen);

        PCLXLWriter.WriteStreamBlock(prnWriter, embeddedStream,
                                     buffer, ref indBuf);

        PCLXLWriter.WriteStreamBlock(prnWriter, embeddedStream,
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

        int indBuf;

        indBuf = 0;

        lenBuf = cSizeAttrUint16Box +
                 cSizeOperator;

        byte[] buffer = new byte[lenBuf];

        //----------------------------------------------------------------//

        AddAttrUint16Box(ref buffer,
                          ref indBuf,
                          PCLXLAttributes.eTag.BoundingBox,
                          coordX,
                          coordY,
                          (ushort)(coordX + width),
                          (ushort)(coordY + height));

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.Rectangle);

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

        seq = "@PJL Enter Language = PCLXL" + "\x0d" + "\x0a" +
              ") HP-PCL XL;2;0;" + "PCL Paraphernalia" + "\x0a";

        prnWriter.Write(seq.ToCharArray(), 0, seq.Length);

        indStd = 0;

        AddAttrUbyte(ref bufStd,
                     ref indStd,
                     PCLXLAttributes.eTag.Measure,
                     (byte)PCLXLAttrEnums.eVal.eInch);

        AddAttrUint16XY(ref bufStd,
                        ref indStd,
                        PCLXLAttributes.eTag.UnitsPerMeasure,
                        _sessionUPI,
                        _sessionUPI);

        AddAttrUbyte(ref bufStd,
                     ref indStd,
                     PCLXLAttributes.eTag.ErrorReport,
                     (byte)PCLXLAttrEnums.eVal.eErrorPage);

        AddOperator(ref bufStd,
                    ref indStd,
                    PCLXLOperators.eTag.BeginSession);

        AddAttrUbyte(ref bufStd,
                     ref indStd,
                     PCLXLAttributes.eTag.SourceType,
                     (byte)PCLXLAttrEnums.eVal.eDefaultDataSource);

        AddAttrUbyte(ref bufStd,
                     ref indStd,
                     PCLXLAttributes.eTag.DataOrg,
                     (byte)PCLXLAttrEnums.eVal.eBinaryLowByteFirst);

        AddOperator(ref bufStd,
                    ref indStd,
                    PCLXLOperators.eTag.OpenDataSource);

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
                              PCLXLAttributes.eTag.StreamName,
                              streamName);

            AddOperator(ref bufStd,
                        ref indStd,
                        PCLXLOperators.eTag.RemoveStream);
        }

        AddOperator(ref bufStd,
                    ref indStd,
                    PCLXLOperators.eTag.CloseDataSource);

        AddOperator(ref bufStd,
                    ref indStd,
                    PCLXLOperators.eTag.EndSession);

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

        int indBuf;

        indBuf = 0;

        AddAttrUbyteArray(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.eTag.StreamName,
                           streamName);

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.BeginStream);

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

        int indBuf;

        indBuf = 0;

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.EndStream);

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

        int indBuf;

        indBuf = 0;

        PCLXLWriter.AddAttrUbyteArray(ref buffer,
                                      ref indBuf,
                                      PCLXLAttributes.eTag.StreamName,
                                      streamName);

        PCLXLWriter.AddOperator(ref buffer,
                                ref indBuf,
                                PCLXLOperators.eTag.ExecStream);

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

        int indBuf;

        indBuf = 0;

        if (embeddedStream)
        {
            AddAttrUbyteArray(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.StreamName,
                               streamName);

            AddOperator(ref buffer,
                         ref indBuf,
                         PCLXLOperators.eTag.BeginStream);

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
                        PCLXLAttributes.eTag.UnitsPerMeasure,
                        _sessionUPI,
                        _sessionUPI);

        AddAttrUbyte(ref buffer,
                     ref indBuf,
                     PCLXLAttributes.eTag.Measure,
                     (byte)PCLXLAttrEnums.eVal.eInch);

        AddOperator(ref buffer,
                    ref indBuf,
                    PCLXLOperators.eTag.SetPageScale);

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

        int indBuf;

        indBuf = 0;

        //----------------------------------------------------------------//

        AddAttrUbyteArray(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.eTag.StreamName,
                           streamName);

        AddOperator(ref buffer,
                     ref indBuf,
                     PCLXLOperators.eTag.RemoveStream);

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
            advance = advanceTable[((byte)text[i] - 0x20)];
            tmpAdvance[i] = (short)(advance * scaleFactor);
        }

        AddAttrSint16XY(ref buffer,
                        ref indBuf,
                        PCLXLAttributes.eTag.Point,
                        coordX, coordY);

        if (relativePoint)
        {
            AddOperator(ref buffer,
                        ref indBuf,
                        PCLXLOperators.eTag.SetCursorRel);
        }
        else
        {
            AddOperator(ref buffer,
                        ref indBuf,
                        PCLXLOperators.eTag.SetCursor);
        }

        AddAttrUbyteArray(ref buffer,
                          ref indBuf,
                          PCLXLAttributes.eTag.TextData,
                          text);

        AddAttrUbyteArray(ref buffer,
                          ref indBuf,
                          PCLXLAttributes.eTag.XSpacingData,
                          textLen,
                          tmpAdvance);

        AddOperator(ref buffer,
                    ref indBuf,
                    PCLXLOperators.eTag.Text);

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
            advance = advanceTable[((byte)text[i] - 0x20)];
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

        int indBuf;

        float scaleFactor;

        indBuf = 0;

        scaleFactor = pointSize / cAdvanceTablePPI;

        //----------------------------------------------------------------//

        AddAttrSint16XY(ref buffer,
                        ref indBuf,
                        PCLXLAttributes.eTag.Point,
                        coordX, coordY);

        AddOperator(ref buffer,
                    ref indBuf,
                    PCLXLOperators.eTag.SetCursor);

        codePointArray[0] = codePoint;
        advanceArray[0] = advance;

        AddAttrUint16Array(ref buffer,
                          ref indBuf,
                          PCLXLAttributes.eTag.TextData,
                          1,
                          codePointArray);

        AddAttrUbyteArray(ref buffer,
                          ref indBuf,
                          PCLXLAttributes.eTag.XSpacingData,
                          1,
                          advanceArray);

        AddOperator(ref buffer,
                    ref indBuf,
                    PCLXLOperators.eTag.Text);

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
            advance = advanceTable[((byte)text[i] - 0x20)];
            tmpAdvanceX[i] = (short)(advance * scaleFactor);
            tmpAdvanceY[i] = (short)(advance * scaleFactor * tangent);
        }

        AddAttrSint16XY(ref buffer,
                        ref indBuf,
                        PCLXLAttributes.eTag.Point,
                        coordX, coordY);

        AddOperator(ref buffer,
                    ref indBuf,
                    PCLXLOperators.eTag.SetCursor);

        AddAttrUbyteArray(ref buffer,
                          ref indBuf,
                          PCLXLAttributes.eTag.TextData,
                          text);

        AddAttrUbyteArray(ref buffer,
                          ref indBuf,
                          PCLXLAttributes.eTag.XSpacingData,
                          textLen,
                          tmpAdvanceX);

        AddAttrSint16Array(ref buffer,
                           ref indBuf,
                           PCLXLAttributes.eTag.YSpacingData,
                           textLen,
                           tmpAdvanceY);

        AddOperator(ref buffer,
                    ref indBuf,
                    PCLXLOperators.eTag.Text);

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
                                     PCLXLOperators.eTag opTag,
                                     bool embeddedStream)
    {
        const int lenBuf = 64;

        byte[] buffer = new byte[lenBuf];

        int indBuf;

        indBuf = 0;

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
            int indTemp;

            indTemp = 0;

            lenTemp = cSizeAttrUint32 + cSizeOperator;

            if (indBuf > 256)
                lenTemp += cSizeEmbedDataInt32;
            else
                lenTemp += cSizeEmbedDataByte;

            byte[] bufTemp = new byte[lenTemp];

            AddAttrUint32(ref bufTemp,
                          ref indTemp,
                          PCLXLAttributes.eTag.StreamDataLength,
                          (uint)indBuf);

            AddOperator(ref bufTemp,
                        ref indTemp,
                        PCLXLOperators.eTag.ReadStream);

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
