using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides the windows printer Target functions.
/// Class enables raw data to be sent to a Windows printer
/// Details obtained from Microsoft support document.
/// 
/// © Chris Hutchinson 2014-2018
/// 
/// </summary>

static class TargetWinPrint
{
    //--------------------------------------------------------------------//
    //                                            D e c l a r a t i o n s //
    // Structure and API declarations.                                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]

    public class DOCINFOA
    {
        [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
        [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
        [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
    }

    //--------------------------------------------------------------------//

    public struct DRIVER_INFO_8
    {
        public uint cVersion;
        [MarshalAs(UnmanagedType.LPTStr)] public string pName;
        [MarshalAs(UnmanagedType.LPTStr)] public string pEnvironment;
        [MarshalAs(UnmanagedType.LPTStr)] public string pDriverPath;
        [MarshalAs(UnmanagedType.LPTStr)] public string pDataFile;
        [MarshalAs(UnmanagedType.LPTStr)] public string pConfigFile;
        [MarshalAs(UnmanagedType.LPTStr)] public string pHelpFile;
        [MarshalAs(UnmanagedType.LPTStr)] public string pDependentFiles;
        [MarshalAs(UnmanagedType.LPTStr)] public string pMonitorName;
        [MarshalAs(UnmanagedType.LPTStr)] public string pDefaultDataType;
        [MarshalAs(UnmanagedType.LPTStr)] public string pszzPreviousNames;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftDriverDate;
        public ulong dwlDriverVersion;
        [MarshalAs(UnmanagedType.LPTStr)] public string pszMfgName;
        [MarshalAs(UnmanagedType.LPTStr)] public string pszOEMUrl;
        [MarshalAs(UnmanagedType.LPTStr)] public string pszHardwareID;
        [MarshalAs(UnmanagedType.LPTStr)] public string pszProvider;
        [MarshalAs(UnmanagedType.LPTStr)] public string pszPrintProcessor;
        [MarshalAs(UnmanagedType.LPTStr)] public string pszVendorSetup;
        [MarshalAs(UnmanagedType.LPTStr)] public string pszzColorProfiles;
        [MarshalAs(UnmanagedType.LPTStr)] public string pszInfPath;
        public uint dwPrinterDriverAttributes;
        [MarshalAs(UnmanagedType.LPTStr)] public string pszzCoreDriverDependencies;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftMinInboxDriverVerDate;
        public ulong dwlMinInboxDriverVerVersion;
    }

    //--------------------------------------------------------------------//

    [DllImport("winspool.drv", EntryPoint = "OpenPrinterA",
               SetLastError = true, CharSet = CharSet.Ansi,
               ExactSpelling = true,
               CallingConvention = CallingConvention.StdCall)]

    public static extern bool OpenPrinter(
        [MarshalAs(UnmanagedType.LPStr)] string szPrinter,
        out IntPtr hPrinter,
        IntPtr pd);

    //--------------------------------------------------------------------//

    [DllImport("winspool.drv", EntryPoint = "ClosePrinter",
               SetLastError = true,
               ExactSpelling = true,
               CallingConvention = CallingConvention.StdCall)]

    public static extern bool ClosePrinter(
        IntPtr hPrinter);

    //--------------------------------------------------------------------//

    [DllImport("winspool.drv", EntryPoint = "StartDocPrinterA",
               SetLastError = true, CharSet = CharSet.Ansi,
               ExactSpelling = true,
               CallingConvention = CallingConvention.StdCall)]

    public static extern bool StartDocPrinter(
        IntPtr hPrinter,
        int level,
        [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

    //--------------------------------------------------------------------//

    [DllImport("winspool.drv", EntryPoint = "EndDocPrinter",
                SetLastError = true,
                ExactSpelling = true,
                CallingConvention = CallingConvention.StdCall)]

    public static extern bool EndDocPrinter(
        IntPtr hPrinter);

    //--------------------------------------------------------------------//

    [DllImport("winspool.drv", EntryPoint = "StartPagePrinter",
                SetLastError = true,
                ExactSpelling = true,
                CallingConvention = CallingConvention.StdCall)]

    public static extern bool StartPagePrinter(
        IntPtr hPrinter);

    //--------------------------------------------------------------------//

    [DllImport("winspool.drv", EntryPoint = "EndPagePrinter",
                SetLastError = true,
                ExactSpelling = true,
                CallingConvention = CallingConvention.StdCall)]

    public static extern bool EndPagePrinter(
        IntPtr hPrinter);

    //--------------------------------------------------------------------//

    [DllImport("winspool.drv", EntryPoint = "WritePrinter",
                SetLastError = true,
                ExactSpelling = true,
                CallingConvention = CallingConvention.StdCall)]

    public static extern bool WritePrinter(
        IntPtr hPrinter,
        IntPtr pBytes,
        int dwCount,
        out int dwWritten);

    //--------------------------------------------------------------------//

    [DllImport("winspool.drv", EntryPoint = "GetPrinterDriverA",
                SetLastError = true,
                ExactSpelling = true,
                CallingConvention = CallingConvention.StdCall)]

    public static extern bool GetPrinterDriver(
        IntPtr hPrinter,
        string pEnvironment,
        uint Level,
        IntPtr pDriverInfo,
        int cbBuf,
        out int pcbNeeded);

    //--------------------------------------------------------------------//

    [DllImport("kernel32.dll", EntryPoint = "GetLastError",
                SetLastError = true,
                ExactSpelling = true,
                CallingConvention = CallingConvention.StdCall)]

    public static extern uint GetLastError();

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i s D r i v e r X P S                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Check if the driver associated with the specified printer is a     //
    // "PRINTER_DRIVER_XPS" driver.                                       //
    // If so, this indicates a "v4 printer model" driver, which does not  //
    // treat the "RAW" data type as indicating "pass through"; the        //
    // required data type in this case is "XPS_PASS"                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static bool IsDriverXPS(IntPtr hPrinter)
    {
        uint PRINTER_DRIVER_XPS_FLAG = 0x00000002;
        uint ERROR_INSUFFICIENT_BUFFER = 0x0000007a;

        bool bSuccess = false;
        bool bDriverXPS = false;

        IntPtr driverInfo = new IntPtr();
        int buf_len = 0;
        int IntPtrSize = Marshal.SizeOf(typeof(IntPtr));

        driverInfo = IntPtr.Zero;

        //----------------------------------------------------------------//
        //                                                                //
        // Get printer driver information.                                //
        //                                                                //
        // ***** What happens if we run on a pre-Vista system, which      //
        // ***** doesn't include the DRIVER_INFO_8 structure?             //
        // ***** The GetPrinterDriver interface has been supported from   //
        // ***** Windows 2000 Pro onwards.                                //
        //                                                                //
        // The first call will fail (because the buffer hasn't yet been   //
        // sized), but the returned buf_len parameter should give the     //
        // required size.                                                 //
        // After pointing the driverInfo pointer to a suitably sized      //
        // buffer, the second call should succeed.                        //
        //                                                                //
        // Then extract the dwPrinterDriverAttributes field from the      //
        // returned data.                                                 //
        //                                                                //
        //----------------------------------------------------------------//

        bSuccess = GetPrinterDriver(hPrinter, string.Empty, 8,
                                     driverInfo, 0, out buf_len);

        if (GetLastError() == ERROR_INSUFFICIENT_BUFFER)
        {
            driverInfo = Marshal.AllocHGlobal(buf_len);

            bSuccess = GetPrinterDriver(hPrinter, string.Empty, 8,
                                         driverInfo, buf_len, out buf_len);

            var info = (DRIVER_INFO_8)Marshal.PtrToStructure(
                driverInfo, typeof(DRIVER_INFO_8));

            uint attributes = info.dwPrinterDriverAttributes;

            if ((attributes & PRINTER_DRIVER_XPS_FLAG) != 0)
                bDriverXPS = true;
        }

        return bDriverXPS;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s e n d B y t e s T o P r i n t e r                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // The function is given a printer name and an unmanaged array of     //
    // bytes; the function sends those bytes to the print queue.          //
    // Returns true on success, false on failure.                         //
    //                                                                    //
    // ***** Not used at present *****                                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static bool SendBytesToPrinter(string szPrinterName,
                                               IntPtr pBytes,
                                               int dwCount)
    {
        int dwError = 0,
              dwWritten = 0;

        IntPtr hPrinter = new IntPtr(0);

        DOCINFOA di = new DOCINFOA();

        bool bSuccess = false;

        di.pDocName = "PCLParaphernalia RAW Document";

        //----------------------------------------------------------------//
        //                                                                //
        // Open the printer.                                              //
        //                                                                //
        //----------------------------------------------------------------//

        if (OpenPrinter(szPrinterName.Normalize(),
            out hPrinter,
            IntPtr.Zero))
        {
            //------------------------------------------------------------//
            //                                                            //
            // Check if XPS printer driver or not                         //
            //                                                            //
            //------------------------------------------------------------//

            if (IsDriverXPS(hPrinter))
                di.pDataType = "XPS_PASS";
            else
                di.pDataType = "RAW";

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

        if (!bSuccess)
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
    // s e n d D a t a                                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Send data in provided stream to specified printer port.            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static int SendData(BinaryReader prnReader,
                                  string printerName)
    {
        int result = 0;
        int dwError = 0,
              dwWritten = 0;

        IntPtr hPrinter = new IntPtr(0);

        DOCINFOA di = new DOCINFOA();

        bool bSuccess = true;

        di.pDocName = "PCLParaphernalia RAW Document";

        //----------------------------------------------------------------//
        //                                                                //
        // Open the printer.                                              //
        //                                                                //
        //----------------------------------------------------------------//

        if (OpenPrinter(printerName.Normalize(),
            out hPrinter,
            IntPtr.Zero))
        {
            //------------------------------------------------------------//
            //                                                            //
            // Check if XPS printer driver or not                         //
            //                                                            //
            //------------------------------------------------------------//

            if (IsDriverXPS(hPrinter))
                di.pDataType = "XPS_PASS";
            else
                di.pDataType = "RAW";

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
                    // Read contents of supplied stream and send to       //
                    // windows printer.                                   // 
                    //                                                    //
                    //----------------------------------------------------//

                    const int bufLen = 512;

                    int readLen;

                    byte[] prnData = new byte[bufLen];

                    prnReader.BaseStream.Position = 0;
                    prnData = prnReader.ReadBytes(bufLen);

                    while (((readLen = prnData.Length) != 0) && (bSuccess))
                    {
                        IntPtr pUnmanagedBytes = new IntPtr(0);

                        pUnmanagedBytes = Marshal.AllocCoTaskMem(readLen);

                        Marshal.Copy(prnData, 0, pUnmanagedBytes, readLen);

                        //------------------------------------------------//
                        //                                                //
                        // Send the unmanaged bytes to the printer.       //
                        //                                                //
                        //------------------------------------------------//

                        bSuccess = WritePrinter(hPrinter, pUnmanagedBytes,
                                                 readLen, out dwWritten);

                        prnData = prnReader.ReadBytes(bufLen);

                        //------------------------------------------------//
                        //                                                //
                        // Free the unmanaged memory and exit.            //
                        //                                                //
                        //------------------------------------------------//

                        Marshal.FreeCoTaskMem(pUnmanagedBytes);
                    }

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

        if (!bSuccess)
        {
            //------------------------------------------------------------//
            //                                                            //
            // If write did not succeed, GetLastError may give more       //
            // information about the failure.                             //
            //                                                            //
            //------------------------------------------------------------//

            dwError = Marshal.GetLastWin32Error();
        }

        return result;
    }
}
