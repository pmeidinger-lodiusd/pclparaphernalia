using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides PCL handling for the Symbol Set Generate tool.
/// 
/// © Chris Hutchinson 2013
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
    const int cCodePointUnused = 65535;
    const int cCodePointC0Min = 0x00;
    const int cCodePointC0Max = 0x1f;
    const int cCodePointC1Min = 0x80;
    const int cCodePointC1Max = 0x9f;

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

    public ToolSymbolSetGenPCL()
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

    public bool GenerateSymSet(ref string symSetFilename,
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
            StreamOpen(ref symSetFilename,
                        ref _binWriter,
                        ref _opStream);
        }

        catch (Exception exc)
        {
            flagOK = false;

            MessageBox.Show(exc.ToString(),
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

                PCLWriter.SymSetDownloadCode(_binWriter,
                                              symSetNo);

                //--------------------------------------------------------//
                //                                                        //
                // Write symbol set descriptor header.                    //
                //                                                        //
                //--------------------------------------------------------//

                WriteHddr(symSetNo, codeMin, codeMax, charCollReq,
                           PCLSymSetTypes.GetIdPCL((int)symSetType));

                //--------------------------------------------------------//
                //                                                        //
                // Write symbol set map data.                             //
                //                                                        //
                //--------------------------------------------------------//

                WriteMapData(flagIgnoreC1, codeMin, codeMax, symSetMap);

                //--------------------------------------------------------//
                //                                                        //
                // Write symbol set save sequence.                        //
                //                                                        //
                //--------------------------------------------------------//

                PCLWriter.SymSetDownloadSave(_binWriter, true);

                //--------------------------------------------------------//
                //                                                        //
                // Close streams and files.                               //
                //                                                        //
                //--------------------------------------------------------//

                _binWriter.Close();
                _opStream.Close();
            }

            catch (Exception exc)
            {
                flagOK = false;

                MessageBox.Show(exc.ToString(),
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

    private byte LsByte(ushort value)
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

    private ushort LsUInt16(uint value)
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

    private uint LsUInt32(ulong value)
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

    private byte MsByte(ushort value)
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

    private ushort MsUInt16(uint value)
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

    private uint MsUInt32(ulong value)
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

    public void StreamOpen(ref string symSetFilename,
                            ref BinaryWriter binWriter,
                            ref Stream opStream)
    {
        SaveFileDialog saveDialog;

        int len;

        string saveDirectory;
        string tmpFilename;

        int ptr = symSetFilename.LastIndexOf("\\");

        if (ptr <= 0)
        {
            saveDirectory = string.Empty;
            tmpFilename = symSetFilename;
        }
        else
        {
            len = symSetFilename.Length;

            saveDirectory = symSetFilename.Substring(0, ptr);
            tmpFilename = symSetFilename.Substring(ptr + 1,
                                                      len - ptr - 1);
        }

        saveDialog = new SaveFileDialog();

        saveDialog.Filter = "PCL Files | *.pcl";
        saveDialog.DefaultExt = "pcl";

        saveDialog.RestoreDirectory = true;
        saveDialog.InitialDirectory = saveDirectory;
        saveDialog.OverwritePrompt = true;
        saveDialog.FileName = tmpFilename;

        bool? dialogResult = saveDialog.ShowDialog();

        if (dialogResult == true)
        {
            symSetFilename = saveDialog.FileName;
            tmpFilename = symSetFilename;
        }

        opStream = File.Create(tmpFilename);

        if (opStream != null)
        {
            _binWriter = new BinaryWriter(opStream);
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

    public void WriteBuffer(int bufLen,
                             byte[] buffer)
    {
        _binWriter.Write(buffer, 0, bufLen);
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

    private void WriteHddr(ushort symSetNo,
                            ushort codeMin,
                            ushort codeMax,
                            ulong charCollReq,
                            byte symSetType)
    {
        ushort valUInt16;

        uint valUInt32;

        byte[] hddrDesc = new byte[cSizeHddrFixed];

        //----------------------------------------------------------------//
        //                                                                //
        // Calculate total size of header.                                //
        // Write PCL 'download header' escape sequence.                   //
        //                                                                //
        //----------------------------------------------------------------//

        int mapSize = (codeMax - codeMin + 1) * 2;
        int descSize = mapSize + cSizeHddrFixed;

        PCLWriter.SymSetDownloadDesc(_binWriter, (uint)descSize);

        //----------------------------------------------------------------//
        //                                                                //
        // Write font header descriptor.                                  //
        //                                                                //
        //----------------------------------------------------------------//

        hddrDesc[0] = MsByte(cSizeHddrFixed);
        hddrDesc[1] = LsByte(cSizeHddrFixed);

        hddrDesc[2] = MsByte(symSetNo);   // Symbol set Kind1 Id MSB
        hddrDesc[3] = LsByte(symSetNo);   // Symbol set Kind1 Id LSB

        hddrDesc[4] = 3;                   // Format = Unicode
        hddrDesc[5] = symSetType;          // Type
        hddrDesc[6] = MsByte(codeMin);    // First code MSB
        hddrDesc[7] = LsByte(codeMin);    // First code LSB
        hddrDesc[8] = MsByte(codeMax);    // Last code MSB
        hddrDesc[9] = LsByte(codeMax);    // Last code LSB

        valUInt32 = MsUInt32(charCollReq);
        valUInt16 = MsUInt16(valUInt32);
        hddrDesc[10] = MsByte(valUInt16); // Char. Req. byte 0
        hddrDesc[11] = LsByte(valUInt16); // Char. Req. byte 1

        valUInt16 = LsUInt16(valUInt32);
        hddrDesc[12] = MsByte(valUInt16); // Char. Req. byte 2
        hddrDesc[13] = LsByte(valUInt16); // Char. Req. byte 3

        valUInt32 = LsUInt32(charCollReq);
        valUInt16 = MsUInt16(valUInt32);
        hddrDesc[14] = MsByte(valUInt16); // Char. Req. byte 4
        hddrDesc[15] = LsByte(valUInt16); // Char. Req. byte 5

        valUInt16 = LsUInt16(valUInt32);
        hddrDesc[16] = MsByte(valUInt16); // Char. Req. byte 6
        hddrDesc[17] = LsByte(valUInt16); // Char. Req. byte 7

        WriteBuffer(cSizeHddrFixed, hddrDesc);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // w r i t e M a p H d d r                                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Generate symbol set mapping data.                                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    private void WriteMapData(bool flagIgnoreC1,
                               ushort codeMin,
                               ushort codeMax,
                               ushort[] symSetMap)
    {
        int mapSize = (codeMax - codeMin + 1) * 2;

        byte[] mapArray = new byte[mapSize];

        for (int i = codeMin; i <= codeMax; i++)
        {
            int j = (i - codeMin) * 2;

            if ((flagIgnoreC1) &&
                ((i >= cCodePointC1Min) && (i <= cCodePointC1Max)))
            {
                mapArray[j] = 0xff;
                mapArray[j + 1] = 0xff;
            }
            else
            {
                mapArray[j] = MsByte(symSetMap[i]);
                mapArray[j + 1] = LsByte(symSetMap[i]);
            }
        }

        WriteBuffer(mapSize, mapArray);
    }
}
