using System.IO;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles PCL (download) macros.</para>
    /// <para>© Chris Hutchinson 2012</para>
    ///
    /// </summary>
    internal static class PCLDownloadMacro
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
        // c h e c k F o r M a c r o I d                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check whether file starts with 'macro Id' escape sequence.         //
        // Format should be "<esc>&f#y" or "<esc>&f#Y".                       //
        // If it does, extract and return identifier value.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool CheckForMacroId(string fileName, long fileSize, ref int macroId)
        {
            const int prefixLen = 3;

            bool flagOK = true;

            int offset = 0;
            int value = 0;

            byte[] buf = new byte[prefixLen];

            _binReader.Read(buf, 0, prefixLen);

            if ((buf[0] != '\x1b') ||
                (buf[1] != '&') ||
                (buf[2] != 'f'))
            {
                return false;
            }

            const int maxRead = 12;
            bool foundTerm = false;

            byte x;

            offset += prefixLen;

            int maxPos = offset + maxRead;

            if (fileSize <= maxPos)
                maxPos = (int)(fileSize - 1);

            for (var pos = offset; flagOK && (!foundTerm) && (pos < maxPos); pos++)
            {
                x = _binReader.ReadByte();

                if (x == 'y' || x == 'Y')
                    foundTerm = true;
                else if (x < '\x30' || x > '\x39')
                    flagOK = false;
                else
                    value = ((value * 10) + (x - '\x30'));
            }

            if (foundTerm)
            {
                macroId = value;
            }
            else
            {
                flagOK = false;
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k M a c r o F i l e                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check macro file to see if it starts with a 'macro identifier'     //
        // sequence; if so, return the identifier.                            //
        //                                                                    //
        // TODO perhaps we ought to check that macro start and stop are also  //
        //      present?                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool CheckMacroFile(string filename, ref int macroId)
        {
            long fileSize = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Read file to obtain characteristics.                           //
            //                                                                //
            //----------------------------------------------------------------//

            if (!MacroFileOpen(filename, ref fileSize))
                return false;

            var macroIdPresent = CheckForMacroId(filename, fileSize, ref macroId);

            MacroFileClose();

            return macroIdPresent;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m a c r o F i l e C l o s e                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close stream and file.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void MacroFileClose()
        {
            _binReader.Close();
            _ipStream.Close();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m a c r o F i l e C o p y                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Copy macro file contents to output stream.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool MacroFileCopy(BinaryWriter prnWriter, string filename)
        {
            long fileSize = 0;

            if (!MacroFileOpen(filename, ref fileSize))
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

            MacroFileClose();

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m a c r o F i l e O p e n                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open macro file, stream and reader.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool MacroFileOpen(string fileName, ref long fileSize)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Download macro file name is null.",
                                "PCL macro file name invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            if (!File.Exists(fileName))
            {
                MessageBox.Show($"Download macro file '{fileName}' does not exist.",
                                "PCL macro file name invalid",
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
                MessageBox.Show($"IO Exception:\r\n{e.Message}\r\n\r\nOpening file '{fileName}'.",
                                    "PCL macro file analysis",
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