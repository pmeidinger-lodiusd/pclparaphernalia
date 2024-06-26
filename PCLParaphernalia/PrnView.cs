﻿using System.Data;
using System.IO;
using System.Text;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles viewing of content of (print) file.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    [System.Reflection.Obfuscation(Feature = "properties renaming")]
    internal class PrnView
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

        private static Stream _ipStream;
        private static BinaryReader _binReader;

        private static long _fileSize;

        private readonly bool _splitSlices;
        private PrnParseConstants.OptCharSetSubActs _indxCharSetSubAct;
        private PrnParseConstants.OptCharSets _indxCharSetName;
        private int _valCharSetSubCode;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P r n V i e w                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d R o w                                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Adds a row to the output grid.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void AddRow(DataTable table, string offset, string hexVal, string textVal)
        {
            const int colOffset = 0;
            const int colHex = 1;
            const int colText = 2;

            //  Int32 rowNo = _fixedRows + _rowCt;

            DataRow row = table.NewRow();

            row[colOffset] = offset;
            row[colHex] = hexVal;
            row[colText] = textVal;

            table.Rows.Add(row);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c l o s e I n p u t P r n                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close stream and file.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void CloseInputPrn()
        {
            _binReader.Close();
            _ipStream.Close();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // o p e n I n p u t P r n                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open read stream for specified print file.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private bool TryPrnFileOpen(string fileName, ref long fileSize)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Print file name is null.",
                                "Print file selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            if (!File.Exists(fileName))
            {
                MessageBox.Show($"Print file '{fileName}' does not exist.",
                                "Print file selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }

            try
            {
                _ipStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"IO Exception:\n\n{ex.Message}\n\nOpening file '{fileName}'.",
                                "Print file content",
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

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v i e w F i l e                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open print file and show content.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool ViewFile(string prnFilename, PrnParseOptions options, DataTable table)
        {
            if (!TryPrnFileOpen(prnFilename, ref _fileSize))
                return false;

            ViewFileAction(options, table);

            CloseInputPrn();

            return true;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // v i e w F i l e A c t i o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // View file contents.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void ViewFileAction(PrnParseOptions options, DataTable table)
        {
            int blockLen,
                  sliceLen;

            int offsetStart = 0,
                  offsetEnd = -1,
                  offsetCrnt;

            string offsetFormat;
            string offsetStr;

            const bool rowLimitReached = false;
            bool endReached = false;

            byte[] buf = new byte[PrnParseConstants.bufSize];

            //----------------------------------------------------------------//

            if (options.IndxGenOffsetFormat == PrnParseConstants.OptOffsetFormats.Hexadecimal)
            {
                offsetFormat = "{0:x8}";
            }
            else
            {
                offsetFormat = "{0:d10}";
            }

            options.GetOptCharSet(ref _indxCharSetName, ref _indxCharSetSubAct, ref _valCharSetSubCode);

            //----------------------------------------------------------------//
            //                                                                //
            // Check for start conditions specific to current file.           //
            //                                                                //
            //----------------------------------------------------------------//

            options.GetOptCurFOffsets(ref offsetStart,
                                       ref offsetEnd);

            int blockStart = offsetStart;
            _ipStream.Seek(offsetStart, SeekOrigin.Begin);

            if (offsetStart != 0)
            {
                AddRow(table,
                        "Comment",
                        $"Start Offset   = {offsetStart} (0x{offsetStart:X8}) requested",
                        string.Empty);
            }

            if (offsetEnd != -1)
            {
                AddRow(table,
                        "Comment",
                        $"End   Offset   = {offsetEnd} (0x{offsetEnd:X8}) requested",
                        string.Empty);
            }

            //----------------------------------------------------------------//

            while (!endReached && !rowLimitReached)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Read next 'block' of file.                                 //
                // If end-of-file detected, block will be less than full.     //
                //                                                            //
                //------------------------------------------------------------//

                blockLen = _binReader.Read(buf, 0, PrnParseConstants.bufSize);

                if (blockLen == 0)
                {
                    endReached = true;
                }
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Split the current 'block' into 'slices'.               //
                    // Each 'slice' will provide one line of the output       //
                    // display; the last 'slice' may be less than a full line.//
                    // Other slices may also be less than a full line if the  //
                    // option to split slices when LineFeed or FormFeed       //
                    // characters are encountered is in force.                //
                    //                                                        //
                    //--------------------------------------------------------//

                    for (int i = 0; (i < blockLen) && (!endReached); i += sliceLen)
                    {
                        if ((i + PrnParseConstants.viewBytesPerLine) > blockLen)
                        {
                            //------------------------------------------------//
                            //                                                //
                            // Last slice of data is less than full.          //
                            //                                                //
                            //------------------------------------------------//

                            sliceLen = blockLen - i;
                        }
                        else
                        {
                            sliceLen = PrnParseConstants.viewBytesPerLine;
                        }

                        //----------------------------------------------------//
                        //                                                    //
                        // Extract required details from current slice.       //
                        //                                                    //
                        //----------------------------------------------------//

                        offsetCrnt = blockStart + i;

                        offsetStr = string.Format(offsetFormat, offsetCrnt);

                        sliceLen = ViewFileSlice(buf, offsetStr, i, sliceLen, table);

                        if ((offsetEnd != -1) && (offsetCrnt > offsetEnd))
                            endReached = true;
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Increment 'block' offset value.                        //
                    //                                                        //
                    //--------------------------------------------------------//

                    blockStart += blockLen;

                    if ((offsetEnd != -1) && (blockStart > offsetEnd))
                        endReached = true;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // f o r m a t S l i c e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Format a 'slice' of the input PCL file contents in hexadecimal &   //
        // character formats, prefixed with the offset value.                 //
        //                                                                    //
        // The 'slice' is normally a fixed number of characters, but if the   //
        // option to split slices when LineFeed or FormFeed characters are    //
        // encountered is in force, the slice may be less; the actual length  //
        // processed is returned.                                             //
        //                                                                    //
        // 'Unprintable' characters are shown as '.'.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private int ViewFileSlice(byte[] buf,
                                    string crntOffset,
                                    int blockOffset,
                                    int sliceMax,
                                    //                      Int32     rowNo,
                                    DataTable table)
        {
            int sliceLen;

            bool endSlice;

            byte crntByte;

            char cx;

            int sub;

            StringBuilder hexBuf = new StringBuilder();
            StringBuilder strBuf = new StringBuilder();

            sliceLen = sliceMax;

            //----------------------------------------------------------------//
            //                                                                //
            // Construct hexadecimal and text representations of slice.       //
            //                                                                //
            //----------------------------------------------------------------//

            endSlice = false;

            for (int j = blockOffset; j < (blockOffset + sliceLen) && (!endSlice); j++)
            {
                crntByte = buf[j];

                if ((crntByte < 32) || (crntByte == 0x7f) ||
                    ((_indxCharSetName == PrnParseConstants.OptCharSets.ASCII) && (crntByte >= 0x80)) ||
                    ((_indxCharSetName == PrnParseConstants.OptCharSets.ISO_8859_1) && (crntByte >= 0x80) && (crntByte <= 0x9f)))
                {
                    switch (_indxCharSetSubAct)
                    {
                        case PrnParseConstants.OptCharSetSubActs.Spaces:

                            strBuf.Append(' ');
                            break;

                        case PrnParseConstants.OptCharSetSubActs.Substitute:

                            strBuf.Append((char)_valCharSetSubCode);
                            break;

                        default:

                            strBuf.Append('.');
                            break;
                    }
                }
                else
                {
                    strBuf.Append((char)crntByte);
                }

                sub = crntByte;
                sub >>= 4;

                cx = PrnParseConstants.cHexChars[sub];

                hexBuf.Append(cx);

                sub = crntByte & 0x0f;

                cx = PrnParseConstants.cHexChars[sub];

                hexBuf.Append(cx);
                hexBuf.Append(' ');

                //------------------------------------------------------------//
                //                                                            //
                // Terminate the slice if a LineFeed or FormFeed character    //
                // has been encountered and the option to split slices has    //
                // been selected.                                             //
                //                                                            //
                //------------------------------------------------------------//

                if (_splitSlices && ((crntByte == 0x0a) || (crntByte == 0x0c)))
                {
                    endSlice = true;
                    sliceLen = j - blockOffset + 1;
                }
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Add row to table and return count of characters processed.     //
            //                                                                //
            //----------------------------------------------------------------//

            AddRow(table, crntOffset, hexBuf.ToString(), strBuf.ToString());

            return sliceLen;
        }
    }
}