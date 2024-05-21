﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class provides PCL handling for the Symbol Set Generate tool.</para>
    /// <para>© Chris Hutchinson 2013</para>
    ///
    /// </summary>
    class ToolSymbolSetGenPCL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const int cSizeHddrFixed = 18;

        const int cSizeCharSet_8bit = 256;
        const int cCodePointUnused  = 65535;
        const int cCodePointC0Min   = 0x00;
        const int cCodePointC0Max   = 0x1f;
        const int cCodePointC1Min   = 0x80;
        const int cCodePointC1Max   = 0x9f;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private Stream _opStream = null;
        private BinaryWriter _binWriter = null;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T o o l S y m b o l S e t G e n P C L                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolSymbolSetGenPCL ()
        {
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e S y m S e t                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate symbol set definition.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool generateSymSet (ref string symSetFilename,
                                       bool flagIgnoreC0,
                                       bool flagIgnoreC1,
                                       ushort symSetNo,
                                       ushort codeMin,
                                       ushort codeMax,
                                       ulong charCollReq,
                                       ushort[] symSetMap,
                                       PCLSymSetTypes.eIndex symSetType)
        {
            bool flagOK = true;

            //----------------------------------------------------------------//
            //                                                                //
            // Open print file and stream.                                    //
            //                                                                //
            //----------------------------------------------------------------//

            try
            {
                streamOpen (ref symSetFilename,
                            ref _binWriter,
                            ref _opStream);
            }
            catch (Exception exc)
            {
                flagOK = false;

                MessageBox.Show (exc.ToString (),
                                "Failure to open symbol set file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            if (flagOK)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Generate symbol set file contents.                         //
                //                                                            //
                //------------------------------------------------------------//

                try
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Write symbol set identifier sequence.                  //
                    //                                                        //
                    //--------------------------------------------------------//

                    PCLWriter.symSetDownloadCode (_binWriter,
                                                  (ushort)symSetNo);

                    //--------------------------------------------------------//
                    //                                                        //
                    // Write symbol set descriptor header.                    //
                    //                                                        //
                    //--------------------------------------------------------//

                    writeHddr (symSetNo, codeMin, codeMax, charCollReq,
                               PCLSymSetTypes.getIdPCL((int) symSetType));

                    //--------------------------------------------------------//
                    //                                                        //
                    // Write symbol set map data.                             //
                    //                                                        //
                    //--------------------------------------------------------//

                    writeMapData (flagIgnoreC1, codeMin, codeMax, symSetMap);

                    //--------------------------------------------------------//
                    //                                                        //
                    // Write symbol set save sequence.                        //
                    //                                                        //
                    //--------------------------------------------------------//

                    PCLWriter.symSetDownloadSave (_binWriter, true);

                    //--------------------------------------------------------//
                    //                                                        //
                    // Close streams and files.                               //
                    //                                                        //
                    //--------------------------------------------------------//

                    _binWriter.Close ();
                    _opStream.Close ();
                }
                catch (Exception exc)
                {
                    flagOK = false;

                    MessageBox.Show (exc.ToString (),
                                    "Failure to write symbol set file",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            return flagOK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l s B y t e                                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return low (least-significant) byte from supplied unsigned 16-bit  //
        // integer.                                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private byte lsByte (ushort value)
        {
            return (byte)(value & 0x00ff);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l s U I n t 1 6                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return low (least-significant) unsigned 16-bit integer from        //
        // supplied unsigned 32-bit integer.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private ushort lsUInt16 (uint value)
        {
            return (ushort)(value & 0x0000ffff);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l s U I n t 3 2                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return low (least-significant) unsigned 32-bit integer from        //
        // supplied unsigned 64-bit integer.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private uint lsUInt32 (ulong value)
        {
            return (uint)(value & 0x0000ffffffff);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m s B y t e                                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return high (most-significant) byte from supplied unsigned 16-bit  //
        // integer.                                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private byte msByte (ushort value)
        {
            return (byte)((value & 0xff00) >> 8);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m s U I n t 1 6                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return high (most-significant) unsigned 16-bit integer from        //
        // supplied unsigned 32-bit integer.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private ushort msUInt16 (uint value)
        {
            return (ushort)((value & 0xffff0000) >> 16);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m s U I n t 3 2                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return high (most-significant) unsigned 32-bit integer from        //
        // supplied unsigned 64-bit integer.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private uint msUInt32 (ulong value)
        {
            return (uint)((value & 0xffffffff00000000) >> 32);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s t r e a m O p e n                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Create output file via 'Save As' dialogue.                         //
        // Then open target stream and binary writer.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void streamOpen (ref string symSetFilename,
                                ref BinaryWriter binWriter,
                                ref Stream opStream)
        {
            SaveFileDialog saveDialog;

            int ptr,
                  len;

            string saveDirectory;
            string tmpFilename;

            ptr = symSetFilename.LastIndexOf ("\\");

            if (ptr <= 0)
            {
                saveDirectory = string.Empty;
                tmpFilename = symSetFilename;
            }
            else
            {
                len = symSetFilename.Length;

                saveDirectory = symSetFilename.Substring (0, ptr);
                tmpFilename = symSetFilename.Substring (ptr + 1,
                                                          len - ptr - 1);
            }

            saveDialog = new SaveFileDialog
            {
                Filter = "PCL Files | *.pcl",
                DefaultExt = "pcl",

                RestoreDirectory = true,
                InitialDirectory = saveDirectory,
                OverwritePrompt = true,
                FileName = tmpFilename
            };

            bool? dialogResult = saveDialog.ShowDialog ();

            if (dialogResult == true)
            {
                symSetFilename = saveDialog.FileName;
                tmpFilename = symSetFilename;
            }

            opStream = File.Create (tmpFilename);

            if (opStream != null)
            {
                _binWriter = new BinaryWriter (opStream);
                binWriter = _binWriter;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // w r i t e B u f f e r                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write contents of supplied buffer to output symbol set file.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void writeBuffer (int bufLen,
                                 byte[] buffer)
        {
            _binWriter.Write (buffer, 0, bufLen);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // w r i t e H d d r                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate symbol set download header sequence and fixed part of     //
        // header.                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void writeHddr (ushort symSetNo,
                                ushort codeMin,
                                ushort codeMax,
                                ulong charCollReq,
                                byte symSetType)
        {
            int mapSize;
            int descSize;

            ushort valUInt16;

            uint valUInt32;

            byte[] hddrDesc = new byte[cSizeHddrFixed];

            //----------------------------------------------------------------//
            //                                                                //
            // Calculate total size of header.                                //
            // Write PCL 'download header' escape sequence.                   //
            //                                                                //
            //----------------------------------------------------------------//

            mapSize = (codeMax - codeMin + 1) * 2;
            descSize = mapSize + cSizeHddrFixed;

            PCLWriter.symSetDownloadDesc (_binWriter, (uint) descSize);

            //----------------------------------------------------------------//
            //                                                                //
            // Write font header descriptor.                                  //
            //                                                                //
            //----------------------------------------------------------------//

            hddrDesc [0] = msByte (cSizeHddrFixed);
            hddrDesc [1] = lsByte (cSizeHddrFixed);

            hddrDesc [2] = msByte (symSetNo);   // Symbol set Kind1 Id MSB
            hddrDesc [3] = lsByte (symSetNo);   // Symbol set Kind1 Id LSB

            hddrDesc [4] = 3;                   // Format = Unicode
            hddrDesc [5] = symSetType;          // Type
            hddrDesc [6] = msByte (codeMin);    // First code MSB
            hddrDesc [7] = lsByte (codeMin);    // First code LSB
            hddrDesc [8] = msByte (codeMax);    // Last code MSB
            hddrDesc [9] = lsByte (codeMax);    // Last code LSB

            valUInt32 = msUInt32 (charCollReq);
            valUInt16 = msUInt16 (valUInt32);
            hddrDesc [10] = msByte (valUInt16); // Char. Req. byte 0
            hddrDesc [11] = lsByte (valUInt16); // Char. Req. byte 1

            valUInt16 = lsUInt16 (valUInt32);
            hddrDesc [12] = msByte (valUInt16); // Char. Req. byte 2
            hddrDesc [13] = lsByte (valUInt16); // Char. Req. byte 3

            valUInt32 = lsUInt32 (charCollReq);
            valUInt16 = msUInt16 (valUInt32);
            hddrDesc [14] = msByte (valUInt16); // Char. Req. byte 4
            hddrDesc [15] = lsByte (valUInt16); // Char. Req. byte 5

            valUInt16 = lsUInt16 (valUInt32);
            hddrDesc [16] = msByte (valUInt16); // Char. Req. byte 6
            hddrDesc [17] = lsByte (valUInt16); // Char. Req. byte 7

            writeBuffer (cSizeHddrFixed, hddrDesc);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // w r i t e M a p H d d r                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate symbol set mapping data.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void writeMapData (bool flagIgnoreC1,
                                   ushort codeMin,
                                   ushort codeMax,
                                   ushort[] symSetMap)
        {
            int mapSize = (codeMax - codeMin + 1) * 2;

            byte[] mapArray = new byte[mapSize];

            for (int i = codeMin; i <= codeMax; i++)
            {
                int j = (i - codeMin) * 2;

                if (flagIgnoreC1 &&
                    (i >= cCodePointC1Min) && (i <= cCodePointC1Max))
                {
                    mapArray [j]     = 0xff;
                    mapArray [j + 1] = 0xff;
                }
                else
                {
                    mapArray [j]     = msByte (symSetMap [i]);
                    mapArray [j + 1] = lsByte (symSetMap [i]);
                }
            }

            writeBuffer (mapSize, mapArray);
        }
    }
}
