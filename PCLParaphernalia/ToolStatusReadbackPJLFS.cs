﻿using System;
using System.IO;
using System.Text;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides PJL File System support for the Status Readback tool.
    /// 
    /// © Chris Hutchinson 2015
    /// 
    /// </summary>

    static class ToolStatusReadbackPJLFS
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
        private static Stream _opStream = null;

        private static BinaryReader _binReader = null;
        private static BinaryWriter _binWriter = null;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b i n S r c F i l e C l o s e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close input stream and file.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void binSrcFileClose()
        {
            _binReader.Close();
            _ipStream.Close();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b i n S r c F i l e C o p y                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Copy binary source file contents to output stream.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void binSrcFileCopy(BinaryWriter prnWriter)
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
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b i n S r c F i l e O p e n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open binary source file, stream and reader.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool binSrcFileOpen(string fileName,
                                               ref long fileSize)
        {
            bool open = false;

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Binary source filename is null.",
                                "PJL FS file invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }
            else if (!File.Exists(fileName))
            {
                MessageBox.Show("Binary source file '" + fileName +
                                "' does not exist.",
                                "PJL FS file invalid",
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

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b i n T g t F i l e C l o s e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close output stream and file.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void binTgtFileClose()
        {
            _binWriter.Close();
            _opStream.Close();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b i n T g t F i l e O p e n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open binary target file, stream and reader.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static bool binTgtFileOpen(string fileName)
        {
            bool open = false;

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Target filename is null.",
                                "PJL FS file invalid",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return false;
            }
            else
            {
                _opStream = File.Open(fileName,
                                      FileMode.OpenOrCreate,
                                      FileAccess.Write,
                                      FileShare.None);

                if (_opStream != null)
                {
                    open = true;

                    _binWriter = new BinaryWriter(_opStream);
                }
            }

            return open;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b i n T g t F i l e W r i t e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write printer response data to binary target file.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void binTgtFileWrite(byte[] buf,
                                             int bufOffset,
                                             int writeLen)
        {
            _binWriter.Write(buf, bufOffset, writeLen);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e R e q u e s t                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate file system command data.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void generateRequest(BinaryWriter prnWriter,
                                            PJLCommands.eCmdIndex cmdIndx,
                                            bool secJob,
                                            string password,
                                            string pathname,
                                            string binSrcFilename,
                                            int option1,
                                            int option2)
        {
            string seq;
            string jobHddr;
            string jobEnd;

            if (cmdIndx != PJLCommands.eCmdIndex.Unknown)
            {
                PJLCommands.eRequestType reqType;

                string cmdName;

                reqType = PJLCommands.GetType(cmdIndx);
                cmdName = PJLCommands.GetName(cmdIndx);

                if (secJob)
                {
                    jobHddr = "\x1b" + "%-12345X" +
                                       "@PJL JOB PASSWORD=" +
                                       password +
                                       "\x0d\x0a";

                    jobEnd = "\x1b" + "%-12345X";
                }
                else
                {
                    jobHddr = "\x1b" + "%-12345X";

                    jobEnd = "\x1b" + "%-12345X";
                }

                if (reqType == PJLCommands.eRequestType.FSBinSrc)
                {
                    bool OK = true;

                    long fileSize = 0;

                    OK = binSrcFileOpen(binSrcFilename, ref fileSize);

                    if (OK)
                    {
                        seq = jobHddr +
                              "@PJL " + cmdName +
                              " FORMAT:BINARY SIZE=" + fileSize.ToString() +
                              " NAME=" + '"' + pathname + '"' +
                              "\x0d\x0a";

                        prnWriter.Write(seq.ToCharArray(), 0, seq.Length);

                        // Read and send content of binSrcFilename

                        binSrcFileCopy(prnWriter);
                        binSrcFileClose();

                        // terminate job with UEL

                        prnWriter.Write(jobEnd.ToCharArray(), 0, jobEnd.Length);
                    }
                }
                else if (reqType == PJLCommands.eRequestType.FSDirList)
                {
                    seq = jobHddr +
                          "@PJL " + cmdName +
                          " NAME=" + '"' + pathname + '"' +
                          " ENTRY=" + option2.ToString() +
                          " COUNT=" + option1.ToString() +
                          "\x0d\x0a" +
                          jobEnd;

                    prnWriter.Write(seq.ToCharArray(), 0, seq.Length);
                }
                else if (reqType == PJLCommands.eRequestType.FSInit)
                {
                    seq = jobHddr +
                          "@PJL " + cmdName +
                          " VOLUME=" + '"' + pathname + '"' +
                          "\x0d\x0a" +
                          jobEnd;

                    prnWriter.Write(seq.ToCharArray(), 0, seq.Length);
                }
                else if (reqType == PJLCommands.eRequestType.FSUpload)
                {
                    seq = jobHddr +
                          "@PJL " +
                          cmdName +
                          " NAME=" + '"' + pathname + '"' +
                          " OFFSET=" + option2.ToString() +
                          " SIZE=" + option1.ToString() +
                          "\x0d\x0a" +
                          jobEnd;

                    prnWriter.Write(seq.ToCharArray(), 0, seq.Length);
                }
                else
                {
                    seq = jobHddr +
                          "@PJL " + cmdName +
                          " NAME=" + '"' + pathname + '"' +
                          "\x0d\x0a" +
                          jobEnd;

                    prnWriter.Write(seq.ToCharArray(), 0, seq.Length);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d R e s p o n s e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read response from target.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string readResponse(
            PJLCommands.eCmdIndex cmdIndx,
            string binTgtFilenamePJLFS)
        {
            PJLCommands.eRequestType reqType = PJLCommands.GetType(cmdIndx);

            if ((reqType == PJLCommands.eRequestType.FSDirList) ||
                (reqType == PJLCommands.eRequestType.FSQuery))
            {
                //------------------------------------------------------------//
                //                                                            //
                // PJL FileSystem query; request types:                       //
                //  -   FSDIRLIST                                             //
                //  -   FSQUERY                                               //
                // Response from printer expected.                            //
                //                                                            //
                //------------------------------------------------------------//

                return readResponseQuery();
            }
            else if (reqType == PJLCommands.eRequestType.FSUpload)
            {
                //------------------------------------------------------------//
                //                                                            //
                // PJL FileSystem upload; request types:                      // 
                //  -   FSUPLOAD                                              //
                //                                                            //
                //------------------------------------------------------------//

                return readResponseUpload(binTgtFilenamePJLFS);
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // PJL FileSystem action; request types:                      //
                //  -   FSAPPEND                                              //
                //  -   FSDELETE                                              //
                //  -   FSDOWNLOAD                                            //
                //  -   FSINIT                                                //
                //  -   FSMDKIR                                               //
                // Response from printer not expected.                        //
                //                                                            //
                //------------------------------------------------------------//

                return "No response is expected from the " +
                        PJLCommands.GetName(cmdIndx) +
                        " action command";
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d R e s p o n s e Q u e r y                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read standard response from PJL FileSystem query; request types:   //
        //  -   FSDIRLIST                                                     //
        //  -   FSQUERY                                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static string readResponseQuery()
        {
            const int replyBufLen = 32768;

            byte[] replyData = new byte[replyBufLen];

            int replyLen = 0;

            //  Boolean readFF_A = true;    // only one <FF> expected //
            bool OK = false;
            bool replyComplete = false;

            int offset = 0;
            int endOffset = 0;
            int bufRem = replyBufLen;
            int blockLen = 0;

            while (!replyComplete)
            {
                OK = TargetCore.ResponseReadBlock(offset,
                                                   bufRem,
                                                   ref replyData,
                                                   ref blockLen);

                endOffset = offset + blockLen;

                if (!OK)
                {
                    replyComplete = true;
                }
                /*else if (!readFF_A)
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Not yet found any <FF> bytes.                          //
                    // Search buffer to see if first, or both first and       //
                    // second <FF> (as applicable) are present.               //
                    //                                                        //
                    // This branch will never be entered unless we include a  //
                    // PJL ECHO commmand in the job header; included in case  //
                    // we ever do this.                                       //
                    //                                                        //
                    //--------------------------------------------------------//

                    for (Int32 i = offset; i < endOffset; i++)
                    {
                        if (replyData[i] == 0x0c)
                        {
                            if ((readFF_A) && (replyData[endOffset - 1] == 0x0c))
                                replyComplete = true;
                            else
                                readFF_A = true;
                        }
                    }
                }*/
                else
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // <FF> at end of ECHO text is either not expected, or    //
                    // has been read in a previous read action.               // 
                    //                                                        //
                    //--------------------------------------------------------//

                    if (replyData[endOffset - 1] == 0x0c)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Terminating <FF> found (as last byte of data       //
                        // returned by current read action).                  // 
                        //                                                    //
                        //----------------------------------------------------//

                        replyComplete = true;
                    }
                }

                offset += blockLen;
                bufRem -= blockLen;

                if (bufRem <= 0)
                    replyComplete = true;
            }

            replyLen = endOffset;

            TargetCore.ResponseCloseConnection();

            return System.Text.Encoding.ASCII.GetString(replyData,
                                                         0,
                                                         replyLen);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d R e s p o n s e U p l o a d                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read standard response from PJL FileSystem query; request types:   //
        //  -   FSUPLOAD                                                      //
        //                                                                    //
        // Response from printer expected - write this direct to the target   //
        // file (it could be (much) larger than the standard reply buffer.    //
        //                                                                    //
        // The response terminates with a FormFeed byte, but the binary data  //
        // returned may also include FormFeed bytes, so have to ignore those  //
        // ones.                                                              //
        //                                                                    //
        // Also, at least one printer (LaserJet M553x) appears to return      //
        // extra bytes (sometimes hundreds or more) between the end of the    //
        // upload (of a size determined by the SIZE parameter returned at the //
        // start of the response) and the terminating <FF>.                   //
        // This is presumably a firmware bug?                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static string readResponseUpload(string binTgtFilename)
        {
            const int replyBufLen = 32768;

            string reply = string.Empty;

            bool binFileOpen = false;

            binFileOpen = binTgtFileOpen(binTgtFilename);

            if (!binFileOpen)
            {
                reply = "Failed to open target binary file:\r\n\r\n" +
                        binTgtFilename;
            }
            else
            {
                int binSize = -1;
                byte[] replyBlock = new byte[replyBufLen];

                //   Boolean readFF_A = true;    // only one <FF> expected //
                bool OK = true;
                bool replyComplete = false;
                bool firstBlock = true;
                bool supDataWritten = false;

                int offset = 0;
                int endOffset = 0;
                int bufRem = replyBufLen;
                int blockLen = 0;
                int binLen = 0;
                int binTot = 0;
                int binRem = 0;
                int supLen = 0;

                while (OK && !replyComplete)
                {
                    OK = TargetCore.ResponseReadBlock(offset,
                                                       bufRem,
                                                       ref replyBlock,
                                                       ref blockLen);

                    endOffset = offset + blockLen;

                    if (!OK)
                    {
                        replyComplete = true;
                    }
                    /*else if (!readFF_A)
                    {
                        //--------------------------------------------------------//
                        //                                                        //
                        // Not yet found any <FF> bytes.                          //
                        // Search buffer to see if first, or both first and       //
                        // second <FF> (as applicable) are present.               //
                        //                                                        //
                        // This branch will never be entered unless we include a  //
                        // PJL ECHO commmand in the job header; included in case  //
                        // we ever do this.                                       //
                        //                                                        //
                        //--------------------------------------------------------//

                        for (Int32 i = offset; i < endOffset; i++)
                        {
                            if (replyData[i] == 0x0c)
                            {
                                if ((readFF_A) && (replyData[endOffset - 1] == 0x0c))
                                    replyComplete = true;
                                else
                                    readFF_A = true;
                            }
                        }
                    }*/
                    else
                    {
                        //--------------------------------------------------------//
                        //                                                        //
                        // <FF> at end of ECHO text is either not expected, or    //
                        // has been read in a previous read action.               // 
                        //                                                        //
                        //--------------------------------------------------------//

                        if (replyBlock[endOffset - 1] == 0x0c)
                        {
                            //----------------------------------------------------//
                            //                                                    //
                            // Terminating <FF> found (as last byte of data       //
                            // returned by current read action).                  // 
                            //                                                    //
                            //----------------------------------------------------//

                            replyComplete = true;
                        }
                    }

                    if (firstBlock)
                    {
                        //--------------------------------------------------------//
                        //                                                        //
                        // Assume that the first block of the response data will  //
                        // contain (at least) the complete PJL FSUPLOAD command   //
                        // with its parameters.                                   //
                        //                                                        //
                        //--------------------------------------------------------//

                        int cmdLen;

                        firstBlock = false;

                        cmdLen = Array.IndexOf(replyBlock,
                                                PrnParseConstants.asciiLF,
                                                0, blockLen);

                        if (cmdLen != -1)
                        {
                            //----------------------------------------------------//
                            //                                                    //
                            // Terminating <LF> byte of PJL command found.        //
                            // Search for the value associated with the SIZE      //
                            // parameter.                                         //
                            //                                                    //
                            //----------------------------------------------------//

                            cmdLen += 1;    // account for <LF> byte //

                            binSize = readResponseUploadSize(replyBlock, cmdLen);

                            reply = Encoding.ASCII.GetString(replyBlock,
                                                              0,
                                                              cmdLen) +
                                    "\r\n" +
                                    binSize + " bytes will be written to the" +
                                    " target file:" + "\r\n\r\n" +
                                    binTgtFilename;

                            binLen = blockLen - cmdLen;

                            if (replyComplete)
                            {
                                // terminating <FF> found; ignore this in count
                                binLen -= 1;
                            }

                            binRem = binSize;

                            if (binRem > binLen)
                            {
                                supLen = 0;
                                binRem -= binLen;
                            }
                            else if (binRem < binLen)
                            {
                                supLen = binLen - binRem;
                                binLen = binRem;
                                binRem = 0;
                            }
                            else
                            {
                                supLen = 0;
                                binRem = 0;
                            }

                            binTot = binLen;

                            binTgtFileWrite(replyBlock, cmdLen, binLen);
                        }
                        else
                        {
                            //----------------------------------------------------//
                            //                                                    //
                            // Terminating <LF> byte of PJL command NOT found.    //
                            // Send response blocks to the binary file for        //
                            // diagnostic purposes.                               //
                            //                                                    //
                            //----------------------------------------------------//

                            reply = "SIZE data not found at start of response" +
                                    "\r\n\r\n" +
                                    "All response data will be written to the" +
                                    " target file:" + "\r\n\r\n" +
                                    binTgtFilename;

                            binSize = -1;
                            binRem = 0;

                            binTgtFileWrite(replyBlock, 0, blockLen);
                        }
                    }
                    else
                    {
                        //--------------------------------------------------------//
                        //                                                        //
                        // Not the first response block.                          //
                        //                                                        //
                        //--------------------------------------------------------//

                        if (binSize == -1)
                        {
                            // write everything to target file //
                            binLen = blockLen;
                        }
                        else
                        {
                            binLen = blockLen;

                            if (replyComplete)
                            {
                                // terminating <FF> found; ignore this in count
                                binLen -= 1;
                            }

                            if (binRem > binLen)
                            {
                                binRem -= binLen;
                            }
                            else if (binRem < binLen)
                            {
                                supLen += (binLen - binRem);
                                binLen = binRem;
                                binRem = 0;
                            }
                            else
                            {
                                binRem = 0;
                            }
                        }

                        binTot += binLen;

                        binTgtFileWrite(replyBlock, 0, binLen);
                    }

                    if ((supLen != 0) && (!supDataWritten))
                    {
                        supDataWritten = true;

                        reply += "\r\n\r\n" +
                                 "Response from device contains extra" +
                                 " bytes after the binary data of size " +
                                 binSize + " bytes.\r\n" +
                                 "Here are the first " + supLen +
                                 " such bytes:\r\n\r\n" +
                                 ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>" +
                                 "\r\n" +
                                 Encoding.ASCII.GetString(replyBlock,
                                                           binLen,
                                                           supLen) +
                                 "\r\n" +
                                 "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<";
                    }
                }

                //--------------------------------------------------------//
                //                                                        //
                // Response complete.                                     //
                // Close connection and output binary file.               //
                //                                                        //
                //--------------------------------------------------------//

                TargetCore.ResponseCloseConnection();

                binTgtFileClose();

                //--------------------------------------------------------//
                //                                                        //
                // Supplement reply data with details of any              //
                // inconsistencies.                                       //
                //                                                        //
                //--------------------------------------------------------//

                if (binTot < binSize)
                {
                    reply += "\r\n\r\n" +
                             "Response contains only " + binTot +
                             " bytes of binary data, but " + binSize +
                             " bytes were expected!";
                }

                if (supLen != 0)
                {
                    reply += "\r\n\r\n" +
                             "Response from device contains an extra " +
                             supLen + " bytes after the binary data!";
                }
            }

            return reply;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d R e s p o n s e U p l o a d S i z e                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // It is expected that the first block will contain a copy of the     //
        // PJL FSUPLOAD command, with the actual size of binary data to be    //
        // returned shown by the SIZE parameter:                              //
        //                                                                    //
        // @PJL FSUPLOAD FORMAT:BINARY NAME="path" OFFSET=mmm SIZE=nnn<CR><LF>//
        //                                                                    //
        // We need to extract this SIZE value in order to determine how much  //
        // data to save to the binary output file.                            //
        //                                                                    //
        // Assume that the SIZE parameter will start with the last occurrence //
        // of "S" before the terminating <LF> of the command.                 // 
        //                                                                    //
        //--------------------------------------------------------------------//

        private static int readResponseUploadSize(byte[] replyBlock,
                                                     int cmdLen)
        {
            int binSize = 0;

            byte[] textSize = new byte[] { 0x53, 0x49, 0x5a, 0x45 };

            byte firstByte = textSize[0];

            int cmdRem = cmdLen - 1;
            int cmdOffset = cmdLen - 1;
            int indxText = -1;

            bool sizeTextFound = false;

            indxText = Array.LastIndexOf(replyBlock, firstByte,
                                          cmdOffset, cmdRem);

            if (indxText >= 0)
            {
                sizeTextFound = true;

                for (int i = 0; i < textSize.Length; i++)
                {
                    if ((indxText + i >= cmdLen) ||
                        (textSize[i] != replyBlock[indxText + i]))
                        sizeTextFound = false;
                }
            }

            if (sizeTextFound)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Validate and extract the value.                            //
                //                                                            //
                //------------------------------------------------------------//

                int indxVal = indxText + textSize.Length;

                byte crntByte;

                bool valInvalid,
                        valStarted,
                        valComplete;

                valInvalid = false;
                valStarted = false;
                valComplete = false;

                for (int i = indxVal; i < cmdLen; i++)
                {
                    crntByte = replyBlock[i];

                    if (valStarted)
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Digit already found.                               //
                        // Only allow digits until terminating <CR> or <LF>.  //
                        //                                                    //
                        //----------------------------------------------------//

                        if ((crntByte >= PrnParseConstants.asciiDigit0)
                                         &&
                                 (crntByte <= PrnParseConstants.asciiDigit9))
                        {
                            binSize = (binSize * 10) +
                                   (crntByte - PrnParseConstants.asciiDigit0);
                        }
                        else if ((crntByte == PrnParseConstants.asciiCR)
                                         ||
                                 (crntByte == PrnParseConstants.asciiLF))
                        {
                            valComplete = true;
                        }
                        else
                        {
                            valInvalid = true;
                        }
                    }
                    else
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Only spaces (if anything) found so far.            //
                        // Allow spaces or equal sign.                        //
                        //                                                    //
                        //----------------------------------------------------//

                        if (crntByte == PrnParseConstants.asciiSpace)
                        {
                            // do nothing //
                        }
                        else if (crntByte == PrnParseConstants.asciiEquals)
                        {
                            // do nothing //
                        }
                        else if ((crntByte >= PrnParseConstants.asciiDigit0)
                                        &&
                                 (crntByte <= PrnParseConstants.asciiDigit9))
                        {
                            valStarted = true;
                            binSize = (crntByte - PrnParseConstants.asciiDigit0);
                        }
                        else
                        {
                            valInvalid = true;
                        }
                    }

                    if (valInvalid)
                    {
                        i = cmdLen;   // terminate loop //
                        binSize = -1;
                    }
                    else if (valComplete)
                    {
                        i = cmdLen;   // terminate loop //
                    }
                }
            }

            return binSize;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e n d R e q u e s t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Send previously generated request data to target.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void sendRequest(PJLCommands.eCmdIndex cmdIndx)
        {
            PJLCommands.eRequestType reqType =
                PJLCommands.GetType(cmdIndx);

            if ((reqType == PJLCommands.eRequestType.FSDirList) ||
                (reqType == PJLCommands.eRequestType.FSQuery))
            {
                //------------------------------------------------------------//
                //                                                            //
                // PJL FileSystem query; request types:                       //
                //  -   FSDIRLIST                                             //
                //  -   FSQUERY                                               //
                // Response from printer expected.                            //
                //                                                            //
                //------------------------------------------------------------//

                TargetCore.RequestStreamWrite(true);
            }
            else if (reqType == PJLCommands.eRequestType.FSUpload)
            {
                //------------------------------------------------------------//
                //                                                            //
                // PJL FileSystem upload to host workstation; request types:  // 
                //  -   FSUPLOAD                                              //
                // Response from printer expected - write this direct to the  //
                // target file (it could be (much) larger than standard reply //
                // buffer.                                                    //
                //                                                            //
                //------------------------------------------------------------//

                TargetCore.RequestStreamWrite(true);
            }
            else
            {
                //------------------------------------------------------------//
                //                                                            //
                // PJL FileSystem action; request types:                      //
                //  -   FSAPPEND                                              //
                //  -   FSDELETE                                              //
                //  -   FSDOWNLOAD                                            //
                //  -   FSINIT                                                //
                //  -   FSMDKIR                                               //
                // Response from printer not expected.                        //
                //                                                            //
                //------------------------------------------------------------//

                TargetCore.RequestStreamWrite(false);
            }
        }
    }
}
