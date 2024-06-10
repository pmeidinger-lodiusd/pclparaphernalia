using System.IO;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles PCL XL downloadable user stream.</para>
    /// <para>© Chris Hutchinson 2012</para>
    ///
    /// </summary>
    internal static class PCLXLDownloadStream
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
        // c h e c k F o r S t r e a m N a m e                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read PCL XL BeginStream operator, etc.                             //
        //                                                                    //
        // Analysis is minimal; we expect to see:                             //
        //                                                                    //
        // Attribute StreamName        represented by ubyte_array of variable //
        //                             length.                                //
        // Operator  BeginStream                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool CheckForStreamName(string fileName, long fileSize, ref string streamName)
        {
            const ushort minFileSize = 128; // enough to read initial operators
            const byte minStreamNameLen = 1;
            const byte maxStreamNameLen = 80;

            bool OK = true;
            bool beginFound = false;

            int dataLen,
                  pos;

            byte[] buf = new byte[minFileSize];

            _ipStream.Seek(0, SeekOrigin.Begin);

            if (fileSize < minFileSize)
                OK = false;
            else
                _binReader.Read(buf, 0, minFileSize);

            pos = 0;

            while (OK && !beginFound)
            {
                if (buf[pos] == (byte)PCLXLDataTypes.Tag.UbyteArray)
                {
                    // start of ubyte array for StreamName attribute;

                    if (buf[pos + 1] == (byte)PCLXLDataTypes.Tag.Uint16)
                    {
                        dataLen = (buf[pos + 3] * 256) + buf[pos + 2];
                        pos += 4;
                    }
                    else if (buf[pos + 1] == (byte)PCLXLDataTypes.Tag.Ubyte)
                    {
                        dataLen = buf[pos + 2];
                        pos += 3;
                    }
                    else
                    {
                        dataLen = 0;
                    }

                    if ((dataLen < minStreamNameLen) || (dataLen > maxStreamNameLen))
                    {
                        OK = false;
                    }
                    else
                    {
                        char[] streamNameArray = new char[dataLen];

                        for (int i = 0; i < dataLen; i++)
                        {
                            streamNameArray[i] = (char)buf[pos + i];
                        }

                        streamName = new string(streamNameArray);

                        pos += dataLen;
                        pos += 2;
                    }
                }
                else if (buf[pos] == (byte)PCLXLOperators.Tag.BeginStream)
                {
                    beginFound = true;
                    pos++;
                }
                else
                {
                    OK = false;
                }
            }

            return beginFound;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k S t r e a m F i l e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check macro file to see if it starts with a BeginStream operator   //
        // and associated attribute list; if so, return the stream name.      //
        //                                                                    //
        // TODO perhaps we ought to check that ReadStream and EndStream       //
        //      operators are also present?                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool CheckStreamFile(string filename, ref string streamName)
        {
            long fileSize = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Read file to obtain characteristics.                           //
            //                                                                //
            //----------------------------------------------------------------//

            bool streamNamePresent;

            if (!TryStreamFileOpen(filename, ref fileSize))
                return false;

            streamNamePresent = CheckForStreamName(filename, fileSize, ref streamName);

            StreamFileClose();

            return streamNamePresent;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s t r e a m F i l e C l o s e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close input (reader) stream and file.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void StreamFileClose()
        {
            _binReader.Close();

            try
            {
                _ipStream.Close();
            }
            catch (IOException e)
            {
                MessageBox.Show($"IO Exception:\r\n{e.Message}\r\n\r\nClosing stream/file.",
                                 "PCL XL user stream analysis",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s t r e a m F i l e E m b e d                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Embed user-defined stream in output stream.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool StreamFileEmbed(BinaryWriter prnWriter, string filename, string streamName, bool encapsulated)
        {
            long fileSize = 0;

            if (!TryStreamFileOpen(filename, ref fileSize))
                return false;

            const int bufSize = 2048;
            int readSize;

            bool endLoop;

            byte[] buffer = new byte[bufSize];

            endLoop = false;

            if (!encapsulated)
                PCLXLWriter.StreamBegin(prnWriter, streamName);

            while (!endLoop)
            {
                readSize = _binReader.Read(buffer, 0, bufSize);

                if (readSize == 0)
                {
                    endLoop = true;
                }
                else
                {
                    PCLXLWriter.WriteStreamBlock(prnWriter, !encapsulated, buffer, ref readSize);
                }
            }

            if (!encapsulated)
                PCLXLWriter.StreamEnd(prnWriter);

            StreamFileClose();

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s t r e a m F i l e O p e n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open user-defined stream file, input stream and reader.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool TryStreamFileOpen(string fileName, ref long fileSize)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Download stream file name is null.",
                                "PCL XL stream invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            if (!File.Exists(fileName))
            {
                MessageBox.Show($"Download stream file '{fileName}' does not exist.",
                                "PCL XL stream invalid",
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
                MessageBox.Show($"IO Exception:\r\n{e.Message}\r\nOpening file '{fileName}'.",
                                "PCL XL user stream analysis",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            if (_ipStream == null)
                return false;

            fileSize = new FileInfo(fileName).Length;

            _binReader = new BinaryReader(_ipStream);

            return true;
        }
    }
}