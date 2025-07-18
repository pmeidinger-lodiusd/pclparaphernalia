﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines a class which enables raw data to be sent to a
    /// Windows printer - from Microsoft support document.
    ///
    /// © Chris Hutchinson 2014
    /// 
    /// </summary>

    public class Helper_RawPrinter
    {
        //--------------------------------------------------------------------//
        //                                            D e c l a r a t i o n s //
        // Structure and API declarations.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]

        private class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }

        //--------------------------------------------------------------------//

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA",
                   SetLastError = true, CharSet = CharSet.Ansi,
                   ExactSpelling = true,
                   CallingConvention = CallingConvention.StdCall)]

        private static extern bool OpenPrinter(
            [MarshalAs(UnmanagedType.LPStr)] string szPrinter,
            out IntPtr hPrinter,
            IntPtr pd);

        //--------------------------------------------------------------------//

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter",
                   SetLastError = true,
                   ExactSpelling = true,
                   CallingConvention = CallingConvention.StdCall)]

        private static extern bool ClosePrinter(IntPtr hPrinter);

        //--------------------------------------------------------------------//

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA",
                   SetLastError = true, CharSet = CharSet.Ansi,
                   ExactSpelling = true,
                   CallingConvention = CallingConvention.StdCall)]

        private static extern bool StartDocPrinter(
            IntPtr hPrinter, int level,
            [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        //--------------------------------------------------------------------//

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter",
            SetLastError = true, ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]

        private static extern bool EndDocPrinter(IntPtr hPrinter);

        //--------------------------------------------------------------------//

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter",
            SetLastError = true, ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]

        private static extern bool StartPagePrinter(IntPtr hPrinter);

        //--------------------------------------------------------------------//

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter",
            SetLastError = true, ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]

        private static extern bool EndPagePrinter(IntPtr hPrinter);

        //--------------------------------------------------------------------//

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter",
            SetLastError = true, ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]

        private static extern bool WritePrinter(
            IntPtr hPrinter, IntPtr pBytes,
            int dwCount, out int dwWritten);

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e n d B y t e s T o P r i n t e r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // The function is given a printer name and an unmanaged array of     //
        // bytes; the function sends those bytes to the print queue.          //
        // Returns true on success, false on failure.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool SendBytesToPrinter(string szPrinterName,
                                               IntPtr pBytes,
                                               int dwCount)
        {
            int dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);

            DOCINFOA di = new DOCINFOA();

            bool bSuccess = false;

            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";

            //----------------------------------------------------------------//
            //                                                                //
            // Open the printer.                                              //
            //                                                                //
            //----------------------------------------------------------------//

            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Start a document.                                          //
                //                                                            //
                //------------------------------------------------------------//

                if (StartDocPrinter(hPrinter, 1, di))
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Start a page.                                          //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (StartPagePrinter(hPrinter))
                    {
                        //----------------------------------------------------//
                        //                                                    //
                        // Write supplied bytes.                              //
                        //                                                    //
                        //----------------------------------------------------//

                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount,
                                                out dwWritten);

                        //----------------------------------------------------//
                        //                                                    //
                        // End page.                                          //
                        //                                                    //
                        //----------------------------------------------------//

                        EndPagePrinter(hPrinter);
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // End document.                                          //
                    //                                                        //
                    //--------------------------------------------------------//

                    EndDocPrinter(hPrinter);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Close the printer.                                         //
                //                                                            //
                //------------------------------------------------------------//

                ClosePrinter(hPrinter);
            }

            if (bSuccess == false)
            {
                //------------------------------------------------------------//
                //                                                            //
                // If write did not succeed, GetLastError may give more       //
                // information about the failure.                             //
                //                                                            //
                //------------------------------------------------------------//

                dwError = Marshal.GetLastWin32Error();
            }

            return bSuccess;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e n d F i l e T o P r i n t e r                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // The function is given a printer name and a file name; the function //
        // sends the contents of the file to the print queue.                 //
        // Returns true on success, false on failure.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool SendFileToPrinter(string szPrinterName,
                                              string szFileName)
        {
            //----------------------------------------------------------------//
            //                                                                //
            // Open the specified file.                                       //
            // Create a BinaryReader on the file.                             // 
            //                                                                //
            //----------------------------------------------------------------//

            FileStream fs = new FileStream(szFileName, FileMode.Open);

            BinaryReader br = new BinaryReader(fs);

            //----------------------------------------------------------------//
            //                                                                //
            // Open the specified file.                                       //
            // Create an array of bytes big enough to hold the file contents. //
            //                                                                //
            //----------------------------------------------------------------//

            byte[] bytes = new byte[fs.Length];

            bool bSuccess = false;

            IntPtr pUnmanagedBytes = new IntPtr(0);

            int nLength;

            nLength = Convert.ToInt32(fs.Length);

            //----------------------------------------------------------------//
            //                                                                //
            // Read the contents of the file into the array.                  //
            // Allocate some unmanaged memory for those bytes.                //
            // Copy the managed byte array into the unmanaged array.          //
            //                                                                //
            //----------------------------------------------------------------//

            bytes = br.ReadBytes(nLength);

            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);

            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);

            //----------------------------------------------------------------//
            //                                                                //
            // Send the unmanaged bytes to the printer.                       //
            //                                                                //
            //----------------------------------------------------------------//

            bSuccess = SendBytesToPrinter(szPrinterName,
                                           pUnmanagedBytes,
                                           nLength);

            //----------------------------------------------------------------//
            //                                                                //
            // Free the unmanaged memory and exit.                            //
            //                                                                //
            //----------------------------------------------------------------//

            Marshal.FreeCoTaskMem(pUnmanagedBytes);

            return bSuccess;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e n d S t r i n g T o P r i n t e r                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // The function is given a printer name and a string; the function    //
        // sends the contents of the string to the print queue.               //
        // Returns true on success, false on failure.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool SendStringToPrinter(
            string szPrinterName,
            string szString)
        {
            IntPtr pBytes;
            int dwCount;

            dwCount = szString.Length; // How many characters are in the string?

            //----------------------------------------------------------------//
            //                                                                //
            // Assume that the printer is expecting ANSI text, and then       //
            // convert the string to ANSI text.                               //
            // Then send the converted ANSI string to the printer.            //
            //                                                                //
            //----------------------------------------------------------------//

            pBytes = Marshal.StringToCoTaskMemAnsi(szString);

            SendBytesToPrinter(szPrinterName, pBytes, dwCount);

            Marshal.FreeCoTaskMem(pBytes);

            return true;
        }
    }
}
