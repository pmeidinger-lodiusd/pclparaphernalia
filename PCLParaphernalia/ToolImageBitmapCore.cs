using System;
using System.IO;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides core support for the ImageBitmap tool.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class ToolImageBitmapCore
    {
        //--------------------------------------------------------------------//
        //                                                  S t r u c t u r e //
        // B i t m a p F i l e H e a d e r                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private struct BitmapFileHeader
        {
            public uint fileSize;
            public uint dataOffset;
        };

        //--------------------------------------------------------------------//
        //                                                  S t r u c t u r e //
        // B i t m a p I n f o H e a d e r                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private struct BitmapInfoHeader
        {
            public uint infoSize;
            public int width;
            public int height;
            public ushort planes;
            public ushort bitsPerPixel;
            public uint compressionType;
            public uint imageSize;
            public int xPelsPerMetre;
            public int yPelsPerMetre;
            public uint coloursUsed;
            public uint coloursImportant;
        };

        /*
        //--------------------------------------------------------------------//
        //                                                  S t r u c t u r e //
        // B i t m a p V 4 H e a d e r                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private struct BitmapV4Header
        {
            public UInt32 infoSize;
            public Int32  width;
            public Int32  height;
            public UInt16 planes;
            public UInt16 bitsPerPixel;
            public UInt32 compressionType;
            public UInt32 imageSize;
            public Int32  xPelsPerMetre;
            public Int32  yPelsPerMetre;
            public UInt32 coloursUsed;
            public UInt32 coloursImportant;
            public UInt32 maskRed;
            public UInt32 maskGreen;
            public UInt32 maskBlue;
            public UInt32 maskAlpha;
            public UInt32 csType;
            public CIEXYZTriple endPoints;
            public UInt32 gammaRed;
            public UInt32 gammaGreen;
            public UInt32 gammaBlue;
        };
        */

        /*
        //--------------------------------------------------------------------//
        //                                                  S t r u c t u r e //
        // B i t m a p V 5 H e a d e r                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private struct BitmapV5Header
        {
            public UInt32 infoSize;
            public Int32  width;
            public Int32  height;
            public UInt16 planes;
            public UInt16 bitsPerPixel;
            public UInt32 compressionType;
            public UInt32 imageSize;
            public Int32  xPelsPerMetre;
            public Int32  yPelsPerMetre;
            public UInt32 coloursUsed;
            public UInt32 coloursImportant;
            public UInt32 maskRed;
            public UInt32 maskGreen;
            public UInt32 maskBlue;
            public UInt32 maskAlpha;
            public UInt32 csType;
            public CIEXYZTriple endPoints;
            public UInt32 gammaRed;
            public UInt32 gammaGreen;
            public UInt32 gammaBlue;
            public UInt32 intent;
            public UInt32 profileData;
            public UInt32 profileSize;
            public UInt32 reserved;
        };
        */

        //--------------------------------------------------------------------//
        //                                                  S t r u c t u r e //
        // B i t m a p R G B Q u a d                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private struct BitmapRGBQuad
        {
            public byte blue;
            public byte green;
            public byte red;
            public byte reserved;
        };

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool _monochromeBlackWhite;

        private static uint _paletteSize;

        private static BitmapRGBQuad[] _palette;

        private static BitmapFileHeader _fileHeader;
        private static BitmapInfoHeader _infoHeader;

        private static Stream _ipStream = null;
        private static BinaryReader _binReader = null;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b i t m a p C l o s e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close stream and file.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void bitmapClose()
        {
            _binReader.Close();
            _binReader = null;

            _ipStream.Close();
            _ipStream = null;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b i t m a p O p e n                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open read stream for specified bitmap file.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool bitmapOpen(string filename)
        {
            bool open = false;

            if ((filename == null) || (filename == ""))
            {
                MessageBox.Show("Bitmap file name is null.",
                                "Bitmap file selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }
            else if (!File.Exists(filename))
            {
                MessageBox.Show("Bitmap file '" + filename +
                                "' does not exist.",
                                "Bitmap file selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }
            else
            {
                _ipStream = File.Open(filename,
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.None);

                if (_ipStream != null)
                {
                    open = true;

                    _binReader = new BinaryReader(_ipStream);
                }
            }

            return open;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t B m p I n f o                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return metrics for the current bitmap file.                        //
        // Height and width are in pixels.                                    //
        // Resolution values are in pixels-per-metre; divide by 39.37 to      //
        // obtain equivalent dots-per-inch value.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void getBmpInfo(ref int Width,
                                      ref int Height,
                                      ref ushort BitsPerPixel,
                                      ref uint Compression,
                                      ref int ResX,
                                      ref int ResY,
                                      ref uint PaletteSize,
                                      ref bool MonoBW)
        {
            Width = _infoHeader.width;
            Height = _infoHeader.height;
            BitsPerPixel = _infoHeader.bitsPerPixel;
            Compression = _infoHeader.compressionType;
            ResX = _infoHeader.xPelsPerMetre;
            ResY = _infoHeader.yPelsPerMetre;
            PaletteSize = _paletteSize;
            MonoBW = _monochromeBlackWhite;

            return;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t B m p P a l e t t e E n t r y                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return specified palette entry for the current bitmap file.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void getBmpPaletteEntry(int Index,
                                              ref byte Red,
                                              ref byte Green,
                                              ref byte Blue)
        {
            Red = _palette[Index].red;
            Green = _palette[Index].green;
            Blue = _palette[Index].blue;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N e x t I m a g e B l o c k                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read next block of image data; return length.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void getNextImageBlock(ref byte[] bufSub,
                                             int bufSize,
                                             bool firstBlock)
        {

            if (firstBlock)
            {
                int offset = (int)_fileHeader.dataOffset +
                               (bufSize * _infoHeader.height);

                _ipStream.Seek(offset, SeekOrigin.Begin);
            }

            _ipStream.Seek((-1 * bufSize), SeekOrigin.Current);

            _binReader.Read(bufSub, 0, bufSize);

            _ipStream.Seek((-1 * bufSize), SeekOrigin.Current);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d B m p F i l e H e a d e r                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read bitmap file header block.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int readBmpFileHeader()
        {
            int result = 0;

            uint temp;
            byte[] id = new byte[2];

            _binReader.Read(id, 0, 2);

            if ((id[0] != 'B') || (id[1] != 'M'))
            {
                MessageBox.Show("Only type BM files supported",
                                "Bitmap file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return -1;
            }

            _fileHeader.fileSize = readUInt32LE();

            temp = readUInt32LE();

            _fileHeader.dataOffset = readUInt32LE();

            return result;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d B m p I n f o H e a d e r                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read bitmap information header block.                              //
        // Currently only support the Windows v3 (BitMapInfoHeader) format,   //
        // but not the later (less-used) BitMapV4Header or BitMapV5Header     //
        // formats, or the various (now obsolete) OS/2 formats.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int readBmpInfoHeader()
        {
            int result = 0;

            _infoHeader.infoSize = readUInt32LE();

            if (_infoHeader.infoSize != 40)
            {
                MessageBox.Show("Only Windows V3 files supported",
                                "Bitmap file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return -1;
            }

            _infoHeader.width = readInt32LE();
            _infoHeader.height = readInt32LE();
            _infoHeader.planes = readUInt16LE();
            _infoHeader.bitsPerPixel = readUInt16LE();
            _infoHeader.compressionType = readUInt32LE();
            _infoHeader.imageSize = readUInt32LE();
            _infoHeader.xPelsPerMetre = readInt32LE();
            _infoHeader.yPelsPerMetre = readInt32LE();
            _infoHeader.coloursUsed = readUInt32LE();
            _infoHeader.coloursImportant = readUInt32LE();

            return result;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d B m p P a l e t t e                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read bitmap palette (if one exists).                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int readBmpPalette()
        {
            int result = 0;

            ushort bitsPerPixel;

            uint coloursUsed;

            uint entryMax;

            byte[] colourData = new byte[4];

            _monochromeBlackWhite = false;

            bitsPerPixel = _infoHeader.bitsPerPixel;
            coloursUsed = _infoHeader.coloursUsed;

            entryMax = (uint)0x00000001 << bitsPerPixel;

            if (bitsPerPixel < 16)
            {
                if (coloursUsed == 0)
                    _paletteSize = entryMax;
                else
                    _paletteSize = coloursUsed;
            }
            else
            {
                if (coloursUsed == 0)
                    _paletteSize = 0;
                else
                    _paletteSize = coloursUsed;
            }

            _palette = new BitmapRGBQuad[_paletteSize];

            int offset = 14 + (int)_infoHeader.infoSize;

            _ipStream.Seek(offset, SeekOrigin.Begin);

            for (int i = 0; i < _paletteSize; i++)
            {
                _binReader.Read(colourData, 0, 4);

                _palette[i].blue = colourData[0];
                _palette[i].green = colourData[1];
                _palette[i].red = colourData[2];
                _palette[i].reserved = colourData[3];
            }

            if (_paletteSize == 2)
            {
                if ((_palette[0].red == 0x00) &&
                    (_palette[0].green == 0x00) &&
                    (_palette[0].blue == 0x00) &&
                    (_palette[1].red == 0xff) &&
                    (_palette[1].green == 0xff) &&
                    (_palette[1].blue == 0xff))
                {
                    _monochromeBlackWhite = true;
                }
            }

            return result;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d I n t 3 2 L E                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read four bytes and interpret as Little-Endian signed 32-bit       //
        // integer.                                                           //
        // If run on Big-Endian architecture, need to reverse the read bytes  //
        // before conversion.                                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static int readInt32LE()
        {
            byte[] buf = new byte[4];

            _binReader.Read(buf, 0, 4);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buf, 0, 4);

            return BitConverter.ToInt32(buf, 0);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d U I n t 1 6 L E                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read two bytes and interpret as Little-Endian unsigned 16-bit      //
        // integer.                                                           //
        // If run on Big-Endian architecture, need to reverse the read bytes  //
        // before conversion.                                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static ushort readUInt16LE()
        {
            byte[] buf = new byte[2];

            _binReader.Read(buf, 0, 2);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buf, 0, 2);

            return BitConverter.ToUInt16(buf, 0);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d U I n t 3 2 L E                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read four bytes and interpret as Little-Endian unsigned 32-bit     //
        // integer.                                                           //
        // If run on Big-Endian architecture, need to reverse the read bytes  //
        // before conversion.                                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static uint readUInt32LE()
        {
            byte[] buf = new byte[4];

            _binReader.Read(buf, 0, 4);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buf, 0, 4);

            return BitConverter.ToUInt32(buf, 0);
        }
    }
}
