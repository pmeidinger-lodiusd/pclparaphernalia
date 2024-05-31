﻿using System.IO;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles PCL (download) symbol sets.</para>
    /// <para>© Chris Hutchinson 2013</para>
    ///
    /// </summary>
    internal static class PCLDownloadSymSet
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

        public static bool CheckSymSetFile(
            string filename,
            ref ushort symSetNo,
            ref ushort firstCode,
            ref ushort lastCode,
            ref PCLSymSetTypes.Index symSetType)
        {
            long fileSize = 0,
                   offset = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Read file to obtain characteristics.                           //
            //                                                                //
            //----------------------------------------------------------------//

            bool flagOK;
            if (!SymSetFileOpen(filename, ref fileSize))
            {
                MessageBox.Show($"Unable to open symbol set definition file '{filename}'.",
                                 "Symbol Set file invalid",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);

                return false;
            }

            firstCode = 0;
            lastCode = 0;

            if (!ReadSymSetId(fileSize, ref offset, ref symSetNo))
            {
                MessageBox.Show($"Symbol set definition file '{filename}':\r\n\r\nFile does not start with 'symbol set Id' escape sequence.",
                                "Symbol Set file invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                SymSetFileClose();
                return false;
            }

            byte symSetFormat = 0;
            byte symSetTypeId = 0;

            flagOK = ReadSymSetHddr(filename,
                                        fileSize,
                                        symSetNo,
                                        ref symSetFormat,
                                        ref symSetTypeId,
                                        ref offset,
                                        ref firstCode,
                                        ref lastCode);

            if (!flagOK)
            {
                MessageBox.Show($"Symbol set definition file '{filename}':\r\n\r\nHeader is invalid.",
                                    "Symbol Set file invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                SymSetFileClose();
                return false;
            }

            flagOK = ReadAndStoreSymSetMap(offset, symSetNo, firstCode, lastCode);

            if (!flagOK)
            {
                MessageBox.Show($"Symbol set definition file '{filename}':\r\n\r\nMapping data is invalid.",
                                    "Symbol Set file invalid",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                SymSetFileClose();
                return false;
            }

            symSetType = PCLSymSetTypes.GetIndexForIdPCL(symSetTypeId);

            SymSetFileClose();

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

        private static bool ReadSymSetId(long fileSize, ref long fileOffset, ref ushort symSetId)
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
                return false;
            }

            const int maxRead = 12;
            bool foundTerm = false;

            int pos,
                    maxPos;

            byte x;

            offset += prefixLen;

            maxPos = offset + maxRead;

            if (fileSize <= maxPos)
                maxPos = (int)(fileSize - 1);

            for (pos = offset; flagOK && (!foundTerm) && (pos < maxPos); pos++)
            {
                x = _binReader.ReadByte();

                if (x == 'R')
                    foundTerm = true;
                else if (x < '\x30' || x > '\x39')
                    flagOK = false;
                else
                    value = ((value * 10) + (x - '\x30'));
            }

            if (foundTerm)
            {
                symSetId = (ushort)value;
                fileOffset = pos;
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

        private static bool ReadSymSetHddr(string fileName,
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

            string messHeader = $"Download symbol set file '{fileName}':\n\n";
            const string messTrailer = "\n\nYou will have to choose another file.";

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
                return false;
            }

            const int maxRead = 12;
            bool foundTerm = false;

            int pos,
                    maxPos;

            byte x;

            offset += prefixLen;

            maxPos = offset + maxRead;

            if (fileSize <= maxPos)
                maxPos = (int)(fileSize - 1);

            for (pos = offset; flagOK && (!foundTerm) && (pos < maxPos); pos++)
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

            //----------------------------------------------------------------//
            //                                                                //
            //                                                                //
            //                                                                //
            //----------------------------------------------------------------//

            if (!flagOK)
            {
                MessageBox.Show($"{messHeader}File does not start with a valid escape sequence in the format <esc>(f#W (where # is a numeric value).{messTrailer}",
                                "PCL symbol set file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            if ((hddrLen + hddrOffset) > fileSize)
            {
                MessageBox.Show($"{messHeader}Header (offset = '{hddrOffset}') of length '{hddrLen} ' is inconsistent with a file size of '{fileSize}'.{messTrailer}",
                                "PCL symbol set file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            //------------------------------------------------------------//
            //                                                            //
            // Read header base data.                                     //
            //                                                            //
            //------------------------------------------------------------//

            _binReader.Read(buf, 0, hddrDescLen);

            hddrSize = (ushort)((buf[0] * 256) + buf[1]);

            if (hddrSize > hddrLen)
            {
                MessageBox.Show($"{messHeader}Header size '{hddrSize}' is inconsistent with sequence data size of '{hddrLen}'.{messTrailer}",
                                "PCL symbol set file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            if (hddrSize != hddrDescLen)
            {
                MessageBox.Show($"{messHeader}Header size '{hddrSize}' does not equal expected size of '{hddrDescLen}'.{messTrailer}",
                                "PCL symbol set file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

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

            var message = string.Empty;

            if (symSetDes != symSetNo)
            {
                flagOK = false;
                message = "Symbol set designator '" + symSetDes + "' is != number '" + symSetNo + "' from Assign sequence";
            }
            else if (format != 3)            // 3 = Unicode
            {
                flagOK = false;
                message = "Format '" + format + "' is != required value (3 = Unicode)";
            }
            else if (firstCode > lastCode)
            {
                flagOK = false;
                message = "First code '" + firstCode + " > Last Code ' " + lastCode + "'";
            }
            else if (hddrLen != (hddrDescLen + (codeCt * 2)))
            {
                flagOK = false;
                message = "Data length '" + hddrLen + "' is inconsistent with mapping for ";
            }

            if (!flagOK)
            {
                MessageBox.Show(messHeader + message + messTrailer,
                    "PCL symbol set file",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            return true;
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

        private static bool ReadAndStoreSymSetMap(long mapOffset, ushort symSetNo, ushort firstCode, ushort lastCode)
        {
            const int rangeC1Min = 0x80;
            const int rangeC1Max = 0x9f;

            const bool OK = true;

            bool usesC1Range = false;

            PCLSymSetTypes.Index symSetType;

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

                if ((mapIndx >= rangeC1Min) && (mapIndx <= rangeC1Max) &&
                    (mapCode != 0xffff))
                {
                    usesC1Range = true;
                }

                map[mapIndx] = mapCode;
            }

            //----------------------------------------------------------------//

            if (lastCode > 0xff)
                symSetType = PCLSymSetTypes.Index.Bound_16bit;
            else if ((firstCode >= 0x20) && (lastCode <= 0x7f))
                symSetType = PCLSymSetTypes.Index.Bound_7bit;
            else if ((firstCode >= 0x20) || usesC1Range)
                symSetType = PCLSymSetTypes.Index.Bound_PC8;
            else
                symSetType = PCLSymSetTypes.Index.Bound_8bit;

            //----------------------------------------------------------------//

            PCLSymbolSets.SetDataUserSet(symSetNo, symSetType, map);

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

        private static void SymSetFileClose()
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

        public static bool SymSetFileCopy(BinaryWriter prnWriter, string filename)
        {
            long fileSize = 0;

            if (!SymSetFileOpen(filename, ref fileSize))
                return false;

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

            SymSetFileClose();

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s y m S e t F i l e O p e n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open symbol set file, stream and reader.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool SymSetFileOpen(string fileName, ref long fileSize)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Download symbol set file name is null.",
                                "PCL symbol set file name invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            if (!File.Exists(fileName))
            {
                MessageBox.Show($"Download symbol set file '{fileName}' does not exist.",
                                "PCL symbol set file name invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            try
            {
                _ipStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException e)
            {
                MessageBox.Show($"IO Exception:\r\n{e.Message}\r\nOpening symbol set file '{fileName}'.",
                                    "PCL symbol set file",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                return false;
            }

            if (_ipStream == null)
                return false;

            FileInfo fi = new FileInfo(fileName);

            fileSize = fi.Length;

            _binReader = new BinaryReader(_ipStream);

            return true;
        }
    }
}