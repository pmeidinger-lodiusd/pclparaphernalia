using System.IO;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles PCL XL downloadable soft fonts.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class PCLXLDownloadFont
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const int _minHddrDescLen = 8;

        private const ushort _defaultPCLDotRes = 600;

        private enum ePCLXLFontTechnology : byte
        {
            TrueType = 1,
            Bitmap = 254,
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static Stream _ipStream = null;
        private static BinaryReader _binReader = null;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t F i l e C l o s e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close stream and file.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void fontFileClose()
        {
            _binReader.Close();
            _ipStream.Close();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t F i l e C o p y                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Copy font file contents to output stream.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool fontFileCopy(BinaryWriter prnWriter,
                                           string fontFilename)
        {
            bool fileOpen = false;

            bool OK = true;

            long fileSize = 0;

            fileOpen = fontFileOpen(fontFilename, ref fileSize);

            if (!fileOpen)
            {
                OK = false;
            }
            else
            {
                const int bufSize = 2048;
                int readSize;

                bool endLoop;

                byte[] buf = new byte[bufSize];

                endLoop = false;

                while (!endLoop)
                {
                    readSize = _binReader.Read(buf, 0, bufSize);

                    if (readSize == 0)
                        endLoop = true;
                    else
                        prnWriter.Write(buf, 0, readSize);
                }

                fontFileClose();
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o n t F i l e O p e n                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open soft font file, stream and reader.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool fontFileOpen(string fileName,
                                            ref long fileSize)
        {
            bool open = false;

            if ((fileName == null) || (fileName == string.Empty))
            {
                MessageBox.Show("Download font file name is null.",
                                "PCL XL font selection attribute invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }
            else if (!File.Exists(fileName))
            {
                MessageBox.Show("Download font file '" + fileName +
                                "' does not exist.",
                                "PCL XL font selection attribute invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }
            else
            {
                try
                {
                    _ipStream = File.Open(fileName,
                                           FileMode.Open,
                                           FileAccess.Read,
                                           FileShare.None);
                }

                catch (IOException e)
                {
                    MessageBox.Show("IO Exception:\r\n" +
                                     e.Message + "\r\n" +
                                     "Opening soft font file '" +
                                     fileName + "'",
                                     "PCL XL soft font analysis",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);
                }

                if (_ipStream != null)
                {
                    open = true;

                    FileInfo fi = new FileInfo(fileName);

                    fileSize = fi.Length;

                    _binReader = new BinaryReader(_ipStream);
                }
            }

            return open;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t F o n t C h a r a c t e r i s t i c s                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate font header and return font characteristics.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool getFontCharacteristics(string fontFilename,
                                                     ref string fontName,
                                                     ref bool scalable,
                                                     ref bool bound,
                                                     ref ushort symSetNo)
        {
            bool fileOpen = false;

            bool OK = true;

            ushort hddrOffset = 0;
            long fileSize = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Read file to obtain characteristics.                           //
            //                                                                //
            //----------------------------------------------------------------//

            fileOpen = fontFileOpen(fontFilename, ref fileSize);

            if (!fileOpen)
            {
                OK = false;
            }
            else
            {
                OK = readHddrIntro(fontFilename,
                                    fileSize,
                                    ref fontName,
                                    ref hddrOffset);

                if (OK)
                {
                    OK = readHddrDescriptor(hddrOffset,
                                             ref scalable,
                                             ref bound,
                                             ref symSetNo);
                }

                fontFileClose();
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d H d d r D e s c r i p t o r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read PCL XL soft font descriptor.                                  //
        // We do minimal validation.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool readHddrDescriptor(ushort hddrOffset,
                                                  ref bool scalable,
                                                  ref bool bound,
                                                  ref ushort symSetNo)
        {
            const ushort symSetUnicode = 590;

            const byte techTrueType = 1;
            //  const Byte techBitmap   = 254;

            bool OK = true;

            byte[] hddr = new byte[_minHddrDescLen];

            byte technology;

            _ipStream.Seek(hddrOffset, SeekOrigin.Begin);

            _binReader.Read(hddr, 0, _minHddrDescLen);

            //----------------------------------------------------------------//

            symSetNo = (ushort)((hddr[2] * 256) + hddr[3]);

            bound = symSetNo != symSetUnicode;

            //----------------------------------------------------------------//

            technology = hddr[4];

            scalable = technology == techTrueType;

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d H d d r I n t r o                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read PCL XL BeginFontHeader and (initial) ReadFontHeader           //
        // operators, etc.                                                    //
        //                                                                    //
        // Analysis is minimal; we expect to see:                             //
        //                                                                    //
        // Attribute FontName          represented by ubyte_array of (fixed)  //
        //                             length 16.                             //
        // Attribute FontFormat        represented by ubyte value of zero.    //
        // Operator  BeginFontHeader   attributes may appear in either order. //
        //                                                                    //
        // Attribute FontHeaderLength  represented by uint16 value.           //
        // Operator  ReadFontHeader    first or only such operator, followed  //
        //                             by 'embedded data', which will be      //
        //                             introduced by one of:                  //
        //     embedded_data_byte tag  followed by ubyte length value         //
        //  OR embedded_data tag       followed by uint32 length value        //
        //                                                                    //
        // Then the specified (length) number of font header bytes; we need   //
        // a minimum of 8 bytes for the standard format-0 font header.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool readHddrIntro(string fileName,
                                             long fileSize,
                                             ref string fontName,
                                             ref ushort hddrOffset)
        {
            const ushort minFileSize = 64; // enough to read initial operators
            const byte minFontNameLen = 12;
            const byte maxFontNameLen = 20;

            string messHeader = "Download font file '" + fileName + "':\n\n";
            string messTrailer = "\n\nYou will have to choose another file.";

            bool OK = true;
            bool beginFound = false;

            int hddrDescLen = 0,
                  dataLen,
                  pos;

            if (fileSize < minFileSize)
            {
                MessageBox.Show(messHeader +
                                "File size < minimum (" + minFileSize +
                                " bytes)." +
                                messTrailer,
                                "PCL XL soft font file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return false;
            }

            byte[] buf = new byte[minFileSize];

            _ipStream.Seek(0, SeekOrigin.Begin);

            _binReader.Read(buf, 0, minFileSize);

            pos = 0;

            while (OK && !beginFound)
            {
                if (buf[pos] == (byte)PCLXLDataTypes.eTag.UbyteArray)
                {
                    // start of ubyte array for FontName attribute;

                    if (buf[pos + 1] == (byte)PCLXLDataTypes.eTag.Uint16)
                    {
                        dataLen = (buf[pos + 3] * 256) + buf[pos + 2];
                        pos += 4;
                    }
                    else if (buf[pos + 1] == (byte)PCLXLDataTypes.eTag.Ubyte)
                    {
                        dataLen = buf[pos + 2];
                        pos += 3;
                    }
                    else
                        dataLen = 0;

                    if ((dataLen < minFontNameLen) || (dataLen > maxFontNameLen))
                    {
                        OK = false;
                    }
                    else
                    {
                        char[] fontNameArray = new char[dataLen];

                        for (int i = 0; i < dataLen; i++)
                        {
                            fontNameArray[i] = (char)buf[pos + i];
                        }

                        fontName = new string(fontNameArray);

                        pos += dataLen;

                        if ((buf[pos] != (byte)PCLXLAttrDefiners.eTag.Ubyte) ||
                            (buf[pos + 1] != (byte)PCLXLAttributes.eTag.FontName))
                            OK = false;
                        else
                            pos += 2;
                    }
                }
                else if (buf[pos] == (byte)PCLXLDataTypes.eTag.Ubyte)
                {
                    // start of FontFormat attribute.

                    if ((buf[pos + 1] != 0) ||
                        (buf[pos + 2] != (byte)PCLXLAttrDefiners.eTag.Ubyte) ||
                        (buf[pos + 3] != (byte)PCLXLAttributes.eTag.FontFormat))
                    {
                        OK = false;
                    }
                    else
                        pos += 4;
                }
                else if (buf[pos] == (byte)PCLXLOperators.eTag.BeginFontHeader)
                {
                    beginFound = true;
                    pos += 1;
                }
                else
                {
                    OK = false;
                }
            }

            if (OK)
            {
                if (buf[pos] == (byte)PCLXLDataTypes.eTag.Uint16)
                {
                    dataLen = (ushort)((buf[pos + 2] * 256) + buf[pos + 1]);

                    if ((dataLen < _minHddrDescLen) ||
                        (buf[pos + 3] != (byte)PCLXLAttrDefiners.eTag.Ubyte) ||
                        (buf[pos + 4] !=
                            (byte)PCLXLAttributes.eTag.FontHeaderLength) ||
                        (buf[pos + 5] != (byte)PCLXLOperators.eTag.ReadFontHeader))
                    {
                        OK = false;
                    }
                    else
                    {
                        pos += 6;

                        if (buf[pos] == (byte)PCLXLEmbedDataDefs.eTag.Byte)
                        {
                            hddrDescLen = buf[pos + 1];

                            pos += 2;
                        }
                        else if (buf[pos] == (byte)PCLXLEmbedDataDefs.eTag.Int)
                        {
                            hddrDescLen = (buf[pos + 4] * 256 * 256 * 256) +
                                          (buf[pos + 5] * 256 * 256) +
                                          (buf[pos + 6] * 256) +
                                          (buf[pos + 7]);

                            pos += 5;

                            if (hddrDescLen < _minHddrDescLen)
                                OK = false;
                        }
                    }
                }
                else
                {
                    OK = false;
                }
            }

            if (OK)
            {
                hddrOffset = (ushort)pos;

                return true;
            }
            else
            {
                MessageBox.Show(messHeader +
                                "Font file format not recognised." +
                                 messTrailer,
                                "PCL XL soft font file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return false;
            }
        }
    }
}