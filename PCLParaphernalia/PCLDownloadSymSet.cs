using System.IO;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles PCL (download) symbol sets.
    /// 
    /// © Chris Hutchinson 2013
    /// 
    /// </summary>

    static class PCLDownloadSymSet
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static Stream _ipStream = null;
        private static BinaryReader _binReader = null;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k S y m S e t F i l e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check symbol set file to see if it starts with a 'symbol set       //
        // identifier' sequence; if so, return the identifier and first and   //
        // last code-point values.                                            //
        // The symbol set map id stored in the special 'user-defined' symbol  //
        // set instance.                                                      //
        //                                                                    //
        // TODO perhaps we ought to check that symbol set control (make       //
        // permanent) is also present?                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool checkSymSetFile(
            string filename,
            ref ushort symSetNo,
            ref ushort firstCode,
            ref ushort lastCode,
            ref PCLSymSetTypes.eIndex symSetType)
        {
            bool flagOK = true;

            bool fileOpen = false;

            long fileSize = 0,
                   offset = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Read file to obtain characteristics.                           //
            //                                                                //
            //----------------------------------------------------------------//

            fileOpen = symSetFileOpen(filename, ref fileSize);

            if (!fileOpen)
            {
                flagOK = false;

                MessageBox.Show("Unable to open symbol set definition" +
                                 " file '" + filename + "'",
                                 "Symbol Set file invalid",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            else
            {
                firstCode = 0;
                lastCode = 0;

                flagOK = readSymSetId(fileSize,
                                       ref offset,
                                       ref symSetNo);
                if (!flagOK)
                {
                    MessageBox.Show("Symbol set definition" +
                                     " file '" + filename + "':\r\n\r\n" +
                                     "File does not start with" +
                                     " 'symbol set Id' escape sequence",
                                     "Symbol Set file invalid",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);
                }
                else
                {
                    byte symSetFormat = 0;
                    byte symSetTypeId = 0;

                    flagOK = readSymSetHddr(filename,
                                             fileSize,
                                             symSetNo,
                                             ref symSetFormat,
                                             ref symSetTypeId,
                                             ref offset,
                                             ref firstCode,
                                             ref lastCode);

                    if (!flagOK)
                    {
                        MessageBox.Show("Symbol set definition" +
                                         " file '" + filename + "':\r\n\r\n" +
                                         "Header is invalid",
                                         "Symbol Set file invalid",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Error);
                    }
                    else
                    {
                        flagOK = readAndStoreSymSetMap(offset,
                                                        symSetNo,
                                                        firstCode,
                                                        lastCode);

                        if (!flagOK)
                        {
                            MessageBox.Show("Symbol set definition" +
                                             " file '" + filename + "':\r\n\r\n" +
                                             "Mapping data is invalid",
                                             "Symbol Set file invalid",
                                             MessageBoxButton.OK,
                                             MessageBoxImage.Error);
                        }
                        else
                        {
                            symSetType = PCLSymSetTypes.getIndexForIdPCL(symSetTypeId);
                        }
                    }
                }

                symSetFileClose();
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d S y m S e t I d                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check whether file starts with 'symbol set Id' escape sequence.    //
        // Format should be "<esc>*c#R".                                      //
        // If it does, extract and return identifier value.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool readSymSetId(long fileSize,
                                             ref long fileOffset,
                                             ref ushort symSetId)
        {
            const int prefixLen = 3;

            bool flagOK = true;

            int offset = (int)fileOffset;

            int value = 0;

            byte[] buf = new byte[prefixLen];

            _binReader.Read(buf, 0, prefixLen);

            if ((buf[0] != '\x1b') ||
                (buf[1] != '*') ||
                (buf[2] != 'c'))
            {
                flagOK = false;
            }
            else
            {
                const int maxRead = 12;
                bool foundTerm = false;

                int pos,
                      maxPos;

                byte x;

                offset += prefixLen;

                maxPos = offset + maxRead;

                if (fileSize <= maxPos)
                    maxPos = (int)(fileSize - 1);

                for (pos = offset;
                     (flagOK) && (!foundTerm) && (pos < maxPos);
                     pos++)
                {
                    x = _binReader.ReadByte();

                    if (x == 'R')
                        foundTerm = true;
                    else if (x < '\x30')
                        flagOK = false;
                    else if (x > '\x39')
                        flagOK = false;
                    else
                        value = ((value * 10) + (x - '\x30'));
                }

                if (foundTerm)
                {
                    symSetId = (ushort)value;
                    fileOffset = pos;
                }
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d S y m S e t H d d r                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read PCL 'Download Symbol Set' escape sequence.                    //
        // Format should be "<esc>(f#W"                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool readSymSetHddr(string fileName,
                                               long fileSize,
                                               ushort symSetNo,
                                               ref byte format,
                                               ref byte type,
                                               ref long fileOffset,
                                               ref ushort firstCode,
                                               ref ushort lastCode)
        {
            const int hddrDescLen = 18;
            const int prefixLen = 3;

            bool flagOK = true;

            string messHeader = "Download symbol set file '" + fileName + "':\n\n";
            string messTrailer = "\n\nYou will have to choose another file.";

            int offset = (int)fileOffset;
            int value = 0;

            int hddrSize = 0,
                  hddrLen = 0,
                  hddrOffset = 0;

            byte[] buf = new byte[hddrDescLen];

            _binReader.Read(buf, 0, prefixLen);

            if ((buf[0] != '\x1b') ||
                (buf[1] != '(') ||
                (buf[2] != 'f'))
            {
                flagOK = false;
            }
            else
            {
                const int maxRead = 12;
                bool foundTerm = false;

                int pos,
                      maxPos;

                byte x;

                offset += prefixLen;

                maxPos = offset + maxRead;

                if (fileSize <= maxPos)
                    maxPos = (int)(fileSize - 1);

                for (pos = offset;
                     (flagOK) && (!foundTerm) && (pos < maxPos);
                     pos++)
                {
                    x = _binReader.ReadByte();

                    if (x == 'W')
                        foundTerm = true;
                    else if (x < '\x30')
                        flagOK = false;
                    else if (x > '\x39')
                        flagOK = false;
                    else
                        value = ((value * 10) + (x - '\x30'));
                }

                if (foundTerm)
                {
                    hddrOffset = pos;
                    hddrLen = value;
                }
                else
                {
                    flagOK = false;
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            //                                                                //
            //                                                                //
            //----------------------------------------------------------------//

            if (!flagOK)
            {
                MessageBox.Show(messHeader +
                                "File does not start with a valid escape" +
                                " sequence in the format <esc>(f#W" +
                                " (where # is a numeric value)." +
                                messTrailer,
                                "PCL symbol set file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            else if ((hddrLen + hddrOffset) > fileSize)
            {
                flagOK = false;

                MessageBox.Show(messHeader +
                                "Header (offset = '" + hddrOffset + "') of" +
                                "length '" + hddrLen + "' is inconsistent" +
                                " with a file size of '" + fileSize + "'." +
                                messTrailer,
                                "PCL symbol set file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            if (flagOK)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Read header base data.                                     //
                //                                                            //
                //------------------------------------------------------------//

                _binReader.Read(buf, 0, hddrDescLen);

                hddrSize = (ushort)((buf[0] * 256) + buf[1]);

                if (hddrSize > hddrLen)
                {
                    flagOK = false;

                    MessageBox.Show(messHeader +
                                    "Header size '" + hddrSize + "' is" +
                                    " inconsistent with sequence data size of '" +
                                    hddrLen + "'." +
                                    messTrailer,
                                    "PCL symbol set file",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
                else if (hddrSize != hddrDescLen)
                {
                    flagOK = false;

                    MessageBox.Show(messHeader +
                                    "Header size '" + hddrSize +
                                    "' != expected size of '" +
                                    hddrDescLen + "'." +
                                    messTrailer,
                                    "PCL symbol set file",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            if (flagOK)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Read & check remaining header base items.                  //
                //                                                            //
                //------------------------------------------------------------//

                ushort symSetDes;

                int codeCt;

                symSetDes = (ushort)((buf[2] * 256) + buf[3]);

                format = buf[4];
                type = buf[5];

                firstCode = (ushort)((buf[6] * 256) + buf[7]);
                lastCode = (ushort)((buf[8] * 256) + buf[9]);

                codeCt = lastCode - firstCode + 1;

                fileOffset = (hddrOffset + hddrSize);

                if (symSetDes != symSetNo)
                {
                    flagOK = false;

                    MessageBox.Show(messHeader +
                                     "Symbol set designator '" + symSetDes + "' is" +
                                     " != number '" + symSetNo + "' from Assign sequence" +
                                     messTrailer,
                                     "PCL symbol set font file",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);
                }
                else if (format != 3)            // 3 = Unicode
                {
                    flagOK = false;

                    MessageBox.Show(messHeader +
                                     "Format '" + format + "' is" +
                                     " != required value (3 = Unicode)" +
                                     messTrailer,
                                     "PCL symbol set file",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);
                }
                else if (firstCode > lastCode)
                {
                    flagOK = false;

                    MessageBox.Show(messHeader +
                                     "First code '" + firstCode +
                                     " > Last Code ' " + lastCode + "'" +
                                     messTrailer,
                                     "PCL symbol set file",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);
                }
                else if (hddrLen != (hddrDescLen + (codeCt * 2)))
                {
                    flagOK = false;

                    MessageBox.Show(messHeader +
                                    "Data length '" + hddrLen + "' is" +
                                    " inconsistent with mapping for " +
                                    codeCt + " characters." +
                                    messTrailer,
                                    "PCL symbol set file",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d A n d S t o r e S y m S e t M a p                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read PCL symbol set map array and store in 'user-defined' symbol   //
        // set item.                                                          //
        // Also set the symbol set type from analysis of the first and last   //
        // code values.                                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool readAndStoreSymSetMap(long mapOffset,
                                                      ushort symSetNo,
                                                      ushort firstCode,
                                                      ushort lastCode)
        {
            const int rangeC1Min = 0x80;
            const int rangeC1Max = 0x9f;

            bool OK = true;

            bool usesC1Range = false;

            PCLSymSetTypes.eIndex symSetType;

            ushort mapCode;

            int codeCt,
                  mapBytes,
                  mapIndx,
                  mapPos;

            codeCt = lastCode - firstCode + 1;
            mapBytes = codeCt * 2;

            byte[] buf = new byte[mapBytes];
            ushort[] map = new ushort[lastCode + 1];

            _ipStream.Seek(mapOffset, SeekOrigin.Begin);

            _binReader.Read(buf, 0, mapBytes);

            //----------------------------------------------------------------//

            for (int i = 0; i <= lastCode; i++)
            {
                map[i] = 0xffff;
            }

            for (int i = 0; i < codeCt; i++)
            {
                mapPos = i * 2;

                mapCode = (ushort)((buf[mapPos] * 256) + buf[mapPos + 1]);

                mapIndx = firstCode + i;

                if (((mapIndx >= rangeC1Min) && (mapIndx <= rangeC1Max)) &&
                    (mapCode != 0xffff))
                    usesC1Range = true;

                map[mapIndx] = mapCode;
            }

            //----------------------------------------------------------------//

            if (lastCode > 0xff)
                symSetType = PCLSymSetTypes.eIndex.Bound_16bit;
            else if ((firstCode >= 0x20) && (lastCode <= 0x7f))
                symSetType = PCLSymSetTypes.eIndex.Bound_7bit;
            else if ((firstCode >= 0x20) || (usesC1Range))
                symSetType = PCLSymSetTypes.eIndex.Bound_PC8;
            else
                symSetType = PCLSymSetTypes.eIndex.Bound_8bit;

            //----------------------------------------------------------------//

            PCLSymbolSets.setDataUserSet(symSetNo, symSetType, map);

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s y m S e t F i l e C l o s e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close stream and file.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void symSetFileClose()
        {
            _binReader.Close();
            _ipStream.Close();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s y m S e t F i l e C o p y                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Copy symbol set file contents to output stream.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool symSetFileCopy(BinaryWriter prnWriter,
                                             string filename)
        {
            bool OK = true;

            bool fileOpen = false;

            long fileSize = 0;

            fileOpen = symSetFileOpen(filename, ref fileSize);

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

                symSetFileClose();
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s y m S e t F i l e O p e n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open symbol set file, stream and reader.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool symSetFileOpen(string fileName,
                                              ref long fileSize)
        {
            bool open = false;

            if ((fileName == null) || (fileName == ""))
            {
                MessageBox.Show("Download symbol set file name is null.",
                                "PCL symbol set file name invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }
            else if (!File.Exists(fileName))
            {
                MessageBox.Show("Download symbol set file '" + fileName +
                                "' does not exist.",
                                "PCL symbol set file name invalid",
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
    }
}