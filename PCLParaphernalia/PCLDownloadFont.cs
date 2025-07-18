﻿using System.IO;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles PCL downloadable soft fonts.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class PCLDownloadFont
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const int _minHddrDescLen = 64;

        const ushort _defaultPCLDotRes = 300;

        private enum ePCLFontFormat : byte
        {
            Bitmap = 0,
            IntellifontBound = 10,
            IntellifontUnbound = 11,
            TrueType = 15,
            Universal = 16,
            BitmapResSpec = 20
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

        private static void FontFileClose()
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

        public static bool FontFileCopy(BinaryWriter prnWriter,
                                           string fontFilename)
        {
            bool OK = true;

            bool fileOpen = false;

            long fileSize = 0;

            fileOpen = FontFileOpen(fontFilename, ref fileSize);

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

                FontFileClose();
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

        private static bool FontFileOpen(string fileName,
                                            ref long fileSize)
        {
            bool open = false;

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Download font file name is null.",
                                "PCL font selection attribute invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }
            else if (!File.Exists(fileName))
            {
                MessageBox.Show("Download font file '" + fileName +
                                "' does not exist.",
                                "PCL font selection attribute invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }
            else
            {
                _ipStream = File.Open(fileName,
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.None);

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
        /*
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t F o n t C h a r a c t e r i s t i c s                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Validate font header and return font characteristics.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static Boolean getFontCharacteristics (
            String      fontFilename,
            ref Boolean proportional,
            ref Boolean scalable,
            ref Boolean bound,
            ref Double  pitch,
            ref Double  height,
            ref UInt16  style,
            ref Int16   weight,   
            ref UInt16  typeface,
            ref UInt16  symSetNo,
            ref PCLSymSetTypes.eIndex symSetType)
        {
            Boolean OK = true;

            Boolean fileOpen = false;

            UInt16 hddrLen     = 0;
            UInt16 hddrOffset  = 0;
            UInt16 hddrDescLen = 0;
            Int64  fileSize    = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Read file to obtain characteristics.                           //
            //                                                                //
            //----------------------------------------------------------------//

            fileOpen = fontFileOpen (fontFilename, ref fileSize);

            if (!fileOpen)
            {
                OK = false;
            }
            else
            {
                OK = readHddrIntro (fontFilename,
                                    fileSize,
                                    ref hddrLen,
                                    ref hddrOffset,
                                    ref hddrDescLen);

                if (OK)
                {
                    OK = readHddrDescriptor (hddrOffset,
                                             hddrDescLen,
                                             ref proportional,
                                             ref scalable,
                                             ref bound,
                                             ref pitch,
                                             ref height,
                                             ref style,
                                             ref weight,   
                                             ref typeface,
                                             ref symSetNo,
                                             ref symSetType);
                }

                fontFileClose ();
            }

            return OK;
        }
        */

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t F o n t C h a r a c t e r i s t i c s                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read font header and return font characteristics.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool GetFontCharacteristics(
            string fontFilename,
            ref bool proportional,
            ref bool scalable,
            ref bool bound,
            ref double pitch,
            ref double height,
            ref ushort style,
            ref short weight,
            ref ushort typeface,
            ref ushort symSetNo,
            ref PCLSymSetTypes.eIndex symSetType)
        {
            bool OK = true;

            bool fileOpen = false;

            ushort hddrLen = 0;

            int fileOffset = 0;

            long fontFileSize = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Read file to obtain characteristics.                           //
            //                                                                //
            //----------------------------------------------------------------//

            //      _fontFileName = fontFilename;

            fileOpen = FontFileOpen(fontFilename, ref fontFileSize);

            if (!fileOpen)
            {
                OK = false;
            }
            else
            {
                OK = ReadHddrIntro(fontFilename,
                                    fontFileSize,
                                    ref fileOffset,
                                    ref hddrLen);

                if (OK)
                {
                    OK = GetFontSelectionData(fileOffset,
                                               hddrLen,
                                               ref proportional,
                                               ref scalable,
                                               ref bound,
                                               ref pitch,
                                               ref height,
                                               ref style,
                                               ref weight,
                                               ref typeface,
                                               ref symSetNo,
                                               ref symSetType);
                }

                FontFileClose();
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t F o n t S e l e c t i o n D a t a                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read font selection characteristics from PCL soft font descriptor. //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool GetFontSelectionData(
            int hddrOffset,
            int hddrLen,
            ref bool proportional,
            ref bool scalable,
            ref bool bound,
            ref double pitch,
            ref double height,
            ref ushort style,
            ref short weight,
            ref ushort typeface,
            ref ushort symSetNo,
            ref PCLSymSetTypes.eIndex symSetType)
        {
            bool OK = true;

            ushort hddrDescLen;

            byte[] hddr;

            bool bitmapFont = false;

            ushort dotResX,
                   dotResY;

            byte[] buf = new byte[2];

            _ipStream.Seek(hddrOffset, SeekOrigin.Begin);

            _binReader.Read(buf, 0, 2);

            hddrDescLen = (ushort)((buf[0] << 8) + buf[1]);

            hddr = new byte[hddrDescLen];       // if universal bitmap, want whole header read, inclduing segments

            _ipStream.Seek(hddrOffset, SeekOrigin.Begin);

            _binReader.Read(hddr, 0, hddrDescLen);

            //----------------------------------------------------------------//

            ePCLFontFormat hddrFormat;

            hddrFormat = (ePCLFontFormat)hddr[2];

            switch (hddrFormat)
            {
                case ePCLFontFormat.Bitmap:
                    bitmapFont = true;
                    break;

                case ePCLFontFormat.BitmapResSpec:
                    bitmapFont = true;
                    break;

                case ePCLFontFormat.IntellifontBound:
                    //  intelliFont = true;
                    break;

                case ePCLFontFormat.IntellifontUnbound:
                    //  intelliFont = true;
                    break;

                case ePCLFontFormat.TrueType:
                    //  truetypeFont = true;
                    break;

                case ePCLFontFormat.Universal:
                    //  universalFont = true;
                    if (hddr[70] == 254)    // scaling technology
                        bitmapFont = true;
                    break;
            }

            //----------------------------------------------------------------//

            scalable = !bitmapFont;

            //----------------------------------------------------------------//

            dotResX = _defaultPCLDotRes;
            dotResY = _defaultPCLDotRes;

            if (hddrFormat == ePCLFontFormat.Universal)
            {
                if (bitmapFont)     // scaling technology = 254
                {
                    // need to read BR segment to get resolutions

                    bool endSegs = false;

                    byte[] segData;

                    int segDataLen;
                    int segSize;
                    int segType;
                    int offset;

                    segDataLen = hddrLen - hddrDescLen;

                    segData = new byte[segDataLen];

                    //      _ipStream.Seek (hddrOffset, SeekOrigin.Begin); // already at correct position ?

                    _binReader.Read(segData, 0, segDataLen);

                    offset = 0;

                    while (!endSegs)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Segment Type and Length data is in buffer.         //
                        //                                                    //
                        //----------------------------------------------------//

                        segType = (segData[offset] << 8) +
                                   segData[offset + 1];

                        segSize = (segData[offset + 2] << 24) +
                                  (segData[offset + 3] << 16) +
                                  (segData[offset + 4] << 8) +
                                   segData[offset + 5];

                        if ((segType == 0x4252) &&
                            ((offset + 6 + segSize) < hddrLen))
                        {
                            // BR
                            dotResX = (ushort)((segData[offset + 6] << 8) +
                                                segData[offset + 7]);
                            dotResY = (ushort)((segData[offset + 8] << 8) +
                                                segData[offset + 9]);
                        }
                        else if (segType == 0xffff)
                        {
                            // null

                            endSegs = true;
                        }

                        offset += (6 + segSize);

                        if ((offset + 6) > hddrLen)
                        {
                            endSegs = true;
                        }
                    }
                }
            }
            else if (hddrFormat == ePCLFontFormat.BitmapResSpec)
            {
                dotResX = (ushort)((hddr[64] << 8) + hddr[65]);
                dotResY = (ushort)((hddr[66] << 8) + hddr[67]);
            }
            else //if (hddrFormat == PCLSoftFonts.eIdHddrFormat.BitmapResSpec)
            {
                dotResX = _defaultPCLDotRes;
                dotResY = _defaultPCLDotRes;
            }

            //----------------------------------------------------------------//

            symSetType = PCLSymSetTypes.GetIndexForIdPCL(hddr[3]);

            bound = PCLSymSetTypes.IsBound((int)symSetType);

            //----------------------------------------------------------------//

            proportional = hddr[13] != 0;

            //----------------------------------------------------------------//

            symSetNo = (ushort)((hddr[14] << 8) + hddr[15]);

            //----------------------------------------------------------------//

            if (bitmapFont)
            {
                ushort dotsQtr = 0;
                ushort dotsExt = 0;
                double dotsK = 0.0;

                dotsQtr = (ushort)((hddr[16] << 8) + hddr[17]);
                dotsExt = hddr[40];
                dotsK = (dotsQtr << 8) + dotsExt;

                if ((dotsQtr == 0) && (dotsExt == 0))
                {
                    pitch = 0.0;
                }
                else
                {
                    dotsK = (dotsQtr << 8) + dotsExt;
                    pitch = (dotResX << 10) / dotsK;
                }

                dotsQtr = (ushort)((hddr[18] << 8) + hddr[19]);
                dotsExt = hddr[41];

                if ((dotsQtr == 0) && (dotsExt == 0))
                {
                    height = 0.0;
                }
                else
                {
                    dotsK = (dotsQtr << 8) + dotsExt;
                    height = (dotsK * 72) / (dotResY << 10);
                }
            }
            else
            {
                pitch = 0.0;
                height = 0.0;
            }

            //----------------------------------------------------------------//

            style = (ushort)((hddr[4] << 8) + hddr[23]);
            weight = hddr[24];
            typeface = (ushort)((hddr[26] << 8) + hddr[25]);

            //----------------------------------------------------------------//

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d F o n t H d d r I n t r o                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read PCL 'Download Soft Font' escape sequence.                     //
        // Format should be "<esc>)s#W"                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool ReadHddrIntro(
            string fileName,
            long fontFileSize,
            ref int fileOffset,
            ref ushort hddrLen)
        {
            string messHeader = "Download font file '" + fileName + "':\r\n";
            string messTrailer = "\r\nYou will have to choose another file.";

            bool OK = false;

            ushort value = 0;

            ushort seqLen = 0;

            byte[] buf = new byte[3];

            _binReader.Read(buf, 0, 3);

            if ((buf[0] != '\x1b') ||
                (buf[1] != ')') ||
                (buf[2] != 's'))
            {
                OK = false;
            }
            else
            {
                byte x;

                for (int i = 0; i < 12; i++)
                {
                    x = _binReader.ReadByte();

                    if (x == 'W')
                    {
                        OK = true;

                        seqLen = (ushort)(i + 4);

                        i = 12;
                    }
                    else if (x < '\x30')
                        OK = false;
                    else if (x > '\x39')
                        OK = false;
                    else
                        value = (ushort)((value * 10) + (x - '\x30'));
                }
            }

            if (!OK)
            {
                MessageBox.Show(
                    messHeader +
                    "File does not start with a valid escape" +
                    " sequence in the format <esc>)s#W" +
                    " (where # is a numeric value)." +
                    messTrailer,
                    "PCL soft font file",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            hddrLen = value;

            if ((hddrLen + fileOffset) > fontFileSize)
            {
                MessageBox.Show(
                    messHeader +
                    "Header (offset = '" + fileOffset + "') of" +
                    "length '" + hddrLen + "' is inconsistent" +
                    " with a file size of '" + fontFileSize + "'." +
                    messTrailer,
                    "PCL soft font file",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            //----------------------------------------------------------------//

            fileOffset += seqLen;

            return true;
        }
        /*
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d H d d r D e s c r i p t o r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read PCL soft font descriptor.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static Boolean readHddrDescriptor (
            UInt16      hddrOffset,
            UInt16      hddrDescLen,
            ref Boolean proportional,
            ref Boolean scalable,
            ref Boolean bound,
            ref Double  pitch,
            ref Double  height,
            ref UInt16  style,
            ref Int16   weight,
            ref UInt16  typeface,
            ref UInt16  symSetNo,
            ref PCLSymSetTypes.eIndex symSetType)
        {
            Boolean OK = true;

            Byte [] hddr = new Byte[hddrDescLen];

            Boolean bitmapFont    = false;
        //  Boolean intelliFont   = false;
        //  Boolean truetypeFont  = false;
        //  Boolean universalFont = false;

            UInt16 dotResX,
                   dotResY;

            _ipStream.Seek(hddrOffset, SeekOrigin.Begin);

            _binReader.Read(hddr, 0, hddrDescLen);
            
            //----------------------------------------------------------------//
            
        //  bitmapFont   = false;
        //  intelliFont  = false;
        //  truetypeFont = false;
            
            ePCLFontFormat hddrFormat;
            
            hddrFormat  = (ePCLFontFormat) hddr[2];

            switch (hddrFormat)
            {
                case ePCLFontFormat.Bitmap:
                    bitmapFont = true;
                    break;

                case ePCLFontFormat.BitmapResSpec:
                    bitmapFont = true;
                    break;

                case ePCLFontFormat.IntellifontBound:
                //  intelliFont = true;
                    break;

                case ePCLFontFormat.IntellifontUnbound:
                //  intelliFont = true;
                    break;

                case ePCLFontFormat.TrueType:
                //  truetypeFont = true;
                    break;

                case ePCLFontFormat.Universal:
                //  universalFont = true;
                    break;
            }
            
            //----------------------------------------------------------------//

            if (bitmapFont)
                scalable = false;
            else
                scalable = true;

            //----------------------------------------------------------------//

            dotResX = _defaultPCLDotRes;
            dotResY = _defaultPCLDotRes;

            if (hddrFormat == ePCLFontFormat.BitmapResSpec)
            {
                dotResX = (UInt16)((hddr[64] * 256) + hddr[65]); 
                dotResX = (UInt16)((hddr[66] * 256) + hddr[67]); 
            }

            //----------------------------------------------------------------//
            
            symSetType = PCLSymSetTypes.getIndexForIdPCL (hddr[3]);

            bound = PCLSymSetTypes.isBound ((Int32) symSetType);

            //----------------------------------------------------------------//
            
            if (hddr[13] == 0)
                proportional = false;
            else
                proportional = true;

            //----------------------------------------------------------------//

            symSetNo = (UInt16)((hddr[14] * 256) + hddr[15]); 

            //----------------------------------------------------------------//

            if (bitmapFont)
            {
                UInt16 dotsQtr = 0;
                UInt16 dotsExt = 0;
                Double dotsK = 0.0;

                dotsQtr = (UInt16)((hddr[16] * 256) + hddr[17]);
                dotsExt = (UInt16)(hddr[40]);
                dotsK = (dotsQtr * 256) + dotsExt;

                if ((dotsQtr == 0) && (dotsExt == 0))
                {
                    pitch = 0.0;
                }
                else
                {
                    dotsK = (dotsQtr * 256) + dotsExt;
                    pitch = (dotResX * 1024) / dotsK;
                }

                dotsQtr = (UInt16)((hddr[18] * 256) + hddr[19]);
                dotsExt = (UInt16)(hddr[41]);

                if ((dotsQtr == 0) && (dotsExt == 0))
                {
                    height = 0.0;
                }
                else
                {
                    dotsK = (dotsQtr * 256) + dotsExt;
                    height = (dotsK * 72) / (dotResY * 1024);
                }
            }
            else
            {
                pitch  = 0.0;
                height = 0.0;
            }

            //----------------------------------------------------------------//

            style    = (UInt16)((hddr[4] * 256) + hddr[23]);
            weight   = (Int16)hddr[24];
            typeface = (UInt16)((hddr[26] * 256) + hddr[25]);

            //----------------------------------------------------------------//

            return OK;
        }         
        */
        /*
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d H d d r I n t r o                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read PCL 'Download Soft Font' escape sequence.                     //
        // Format should be "<esc>(s#W"                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static Boolean readHddrIntro(String     fileName,
                                             Int64      fileSize,
                                             ref UInt16 hddrLen,
                                             ref UInt16 hddrOffset,
                                             ref UInt16 hddrDescLen)
        {
            String messHeader  = "Download font file '" + fileName + "':\n\n";
            String messTrailer = "\n\nYou will have to choose another file.";

            Boolean OK = false;

            UInt16 value  = 0;

            Byte[] buf = new Byte[3];
            
            _binReader.Read(buf, 0, 3);

            if ((buf[0] != '\x1b') ||
                (buf[1] != ')')    ||
                (buf[2] != 's'))
            {
                OK = false;
            }
            else
            {
                Byte x;

                for (Int32 i = 0; i < 12; i++)
                {
                    x = _binReader.ReadByte();

                    if (x == 'W')
                    {
                        OK = true;

                        hddrOffset = (UInt16)(i + 4);

                        i = 12;
                    }
                    else if (x < '\x30')
                        OK = false;
                    else if (x > '\x39')
                        OK = false;
                    else
                        value = (UInt16)((value * 10) + (x - '\x30'));
                }
            }

            if (! OK)
            {
                MessageBox.Show(messHeader + 
                                "File does not start with a valid escape" +
                                " sequence in the format <esc>)s#W"       +
                                " (where # is a numeric value)."          + 
                                messTrailer,
                                "PCL soft font file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return false;
            }

            hddrLen = value;

            if ((hddrLen + hddrOffset) > fileSize)
            {
                MessageBox.Show(messHeader + 
                                "Header (offset = '" + hddrOffset + "') of" +
                                "length '" + hddrLen + "' is inconsistent"  +
                                " with a file size of '" + fileSize + "'."  +
                                messTrailer,
                                "PCL soft font file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return false;
            }

            _binReader.Read(buf, 0, 2);
    
            hddrDescLen = (UInt16)((buf[0]  * 256) + buf[1]);

            if (hddrDescLen > hddrLen)
            {
                MessageBox.Show(messHeader + 
                                "Descriptor size '" + hddrDescLen + "' is" +
                                " inconsistent with a header size of '"    +
                                hddrLen + "'."                             +
                                messTrailer,
                                "PCL soft font file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return false;

            }   

            if (hddrDescLen < _minHddrDescLen)
            {
                MessageBox.Show(messHeader +
                                "Descriptor size '" + hddrDescLen             +
                                "' is less than minimum descriptor size of '" +
                                _minHddrDescLen + "'."                        +
                                messTrailer,
                                "PCL soft font file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return false;

            }   
         
            return true;
        }
        */
    }
}