using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Windows;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides the core Target functions.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

static class TargetCore
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public enum eTarget
    {
        File,
        NetPrinter,     // Port 9100 network printer
        WinPrinter,     // Windows printer instance
        Max
    }

    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Fields (class variables).                                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static eTarget _targetType;
    private static ReportCore.eRptFileFmt _rptFileFmt;
    private static ReportCore.eRptChkMarks _rptChkMarks;

    private static int _netPrinterPort;

    private static int _netPrinterTimeoutSend;
    private static int _netPrinterTimeoutReceive;

    private static string _netPrinterAddress;
    private static string _winPrinterName;

    private static string _crntFilename;
    private static string _saveFilename;

    private static Stream _opStream = null;
    private static BinaryWriter _binWriter = null;

    private static bool _flagOptRptWrap;

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t T y p e                                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return current target type.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static eTarget getType()
    {
        return _targetType;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i n i t i a l i s e S e t t i n g s                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Load current target metrics data from regisry.                     //
    // Note that 'capture file' data is not relevant for those tools      //
    // which don't output a printer ready job.                            // 
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void InitialiseSettings()
    {
        int temp = 0;

        TargetPersist.LoadDataCommon(ref temp);

        if (temp < (int)eTarget.Max)
            _targetType = (eTarget)temp;
        else
            _targetType = eTarget.NetPrinter;

        TargetPersist.LoadDataNetPrinter(ref _netPrinterAddress,
                                          ref _netPrinterPort,
                                          ref _netPrinterTimeoutSend,
                                          ref _netPrinterTimeoutReceive);

        TargetPersist.LoadDataWinPrinter(ref _winPrinterName);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m e t r i c s L o a d F i l e C a p t                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Load current target File capture metrics data.                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void MetricsLoadFileCapt(
        ToolCommonData.eToolIds crntToolId,
        ToolCommonData.eToolSubIds crntToolSubId,
        ToolCommonData.ePrintLang crntPDL)
    {
        //----------------------------------------------------------------//

        if (crntToolId == ToolCommonData.eToolIds.FontSample)
            ToolFontSamplePersist.LoadDataCapture(crntPDL,
                                                   ref _saveFilename);
        else if (crntToolId == ToolCommonData.eToolIds.FormSample)
            ToolFormSamplePersist.LoadDataCapture(crntPDL,
                                                   ref _saveFilename);
        else if (crntToolId == ToolCommonData.eToolIds.ImageBitmap)
            ToolImageBitmapPersist.LoadDataCapture(crntPDL,
                                                    ref _saveFilename);
        else if (crntToolId == ToolCommonData.eToolIds.PrintArea)
            ToolPrintAreaPersist.LoadDataCapture(crntPDL,
                                                  ref _saveFilename);
        else if (crntToolId == ToolCommonData.eToolIds.PrnPrint)
            ToolPrnPrintPersist.LoadDataCapture(crntPDL,
                                                 ref _saveFilename);
        else if (crntToolId == ToolCommonData.eToolIds.StatusReadback)
            ToolStatusReadbackPersist.LoadDataCapture(crntPDL,
                                                       ref _saveFilename);
        else if (crntToolId == ToolCommonData.eToolIds.TrayMap)
            ToolTrayMapPersist.LoadDataCapture(crntPDL,
                                                ref _saveFilename);

        //----------------------------------------------------------------//

        else if (crntToolId == ToolCommonData.eToolIds.MiscSamples)
            ToolMiscSamplesPersist.LoadDataCapture(
                         crntToolSubId,
                         crntPDL,
                         ref _saveFilename);

        //----------------------------------------------------------------//

        else
            //     Tool MakeOverlay // ***** Do DUMMY procs ? ***** //
            //     Tool PrintLang
            //     Tool PrnAnalyse
            //     Tool SoftFontGen
            //     Tool SymbolSetGen
            //     Tool XXXDiags

            _saveFilename = string.Empty;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m e t r i c s L o a d F i l e R p t                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Load current target report file metrics data.                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void MetricsLoadFileRpt(
        ToolCommonData.eToolIds crntToolId)
    {
        int tmpFmt = 0,
              tmpChkMarks = 0;

        byte indxFmtNA = (byte)ReportCore.eRptFileFmt.NA;
        byte indxOptChkNA = (byte)ReportCore.eRptChkMarks.NA;

        bool flagNA = false;

        //----------------------------------------------------------------//

        if (crntToolId == ToolCommonData.eToolIds.MakeOverlay)
            ToolMakeOverlayPersist.LoadDataRpt(ref tmpFmt);
        else if (crntToolId == ToolCommonData.eToolIds.PrintLang)
            ToolPrintLangPersist.LoadDataRpt(ref tmpFmt, ref tmpChkMarks,
                                              ref _flagOptRptWrap);
        else if (crntToolId == ToolCommonData.eToolIds.PrnAnalyse)
            ToolPrnAnalysePersist.LoadDataRpt(ref tmpFmt);
        else if (crntToolId == ToolCommonData.eToolIds.SoftFontGenerate)
            ToolSoftFontGenPersist.LoadDataRpt(ref tmpFmt,
                                                ref tmpChkMarks);
        else if (crntToolId == ToolCommonData.eToolIds.StatusReadback)
            ToolStatusReadbackPersist.LoadDataRpt(ref tmpFmt);
        else if (crntToolId == ToolCommonData.eToolIds.SymbolSetGenerate)
            ToolSymbolSetGenPersist.LoadDataRpt(ref tmpFmt);
        else
            flagNA = true;

        //----------------------------------------------------------------//

        if (flagNA)
            tmpFmt = indxFmtNA;
        else
        {
            if (tmpFmt >= indxFmtNA)
                tmpFmt = 0;

            if (tmpChkMarks >= indxOptChkNA)
                tmpChkMarks = 0;
        }

        _rptFileFmt = (ReportCore.eRptFileFmt)tmpFmt;
        _rptChkMarks = (ReportCore.eRptChkMarks)tmpChkMarks;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m e t r i c s L o a d N e t P r i n t e r                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Load current target network printer metrics data.                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void MetricsLoadNetPrinter(ref string printerAddress,
                                              ref int printerPort,
                                              ref int timeoutSend,
                                              ref int timeoutReceive)
    {
        printerAddress = _netPrinterAddress;
        printerPort = _netPrinterPort;

        timeoutSend = _netPrinterTimeoutSend;
        timeoutReceive = _netPrinterTimeoutReceive;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m e t r i c s L o a d W i n P r i n t e r                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Load current target windows printer metrics data.                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void MetricsLoadWinPrinter(ref string printerName)
    {
        printerName = _winPrinterName;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m e t r i c s R e t u r n F i l e C a p t                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Load and return current target File capture metrics data.          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void MetricsReturnFileCapt(
        ToolCommonData.eToolIds crntToolId,
        ToolCommonData.eToolSubIds crntToolSubId,
        ToolCommonData.ePrintLang crntPDL,
        ref string saveFilename)
    {
        MetricsLoadFileCapt(crntToolId, crntToolSubId, crntPDL);

        saveFilename = _saveFilename;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m e t r i c s R e t u r n F i l e R p t                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Load and return current report file metrics data.                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void MetricsReturnFileRpt(
        ToolCommonData.eToolIds crntToolId,
        ref ReportCore.eRptFileFmt rptFileFmt,
        ref ReportCore.eRptChkMarks rptChkMarks,
        ref bool flagOptWrap)
    {
        MetricsLoadFileRpt(crntToolId);

        rptFileFmt = _rptFileFmt;
        rptChkMarks = _rptChkMarks;
        flagOptWrap = _flagOptRptWrap;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m e t r i c s S a v e F i l e C a p t                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current target File capture metrics data.                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void MetricsSaveFileCapt(
        ToolCommonData.eToolIds crntToolId,
        ToolCommonData.eToolSubIds crntToolSubId,
        ToolCommonData.ePrintLang crntPDL,
        string saveFilename)
    {
        _targetType = eTarget.File;

        _saveFilename = saveFilename;

        TargetPersist.SaveDataCommon((int)_targetType);

        //----------------------------------------------------------------//

        if (crntToolId == ToolCommonData.eToolIds.FontSample)
            ToolFontSamplePersist.SaveDataCapture(crntPDL,
                                                   saveFilename);
        else if (crntToolId == ToolCommonData.eToolIds.FormSample)
            ToolFormSamplePersist.SaveDataCapture(crntPDL,
                                                   saveFilename);
        else if (crntToolId == ToolCommonData.eToolIds.ImageBitmap)
            ToolImageBitmapPersist.SaveDataCapture(crntPDL,
                                                   saveFilename);
        else if (crntToolId == ToolCommonData.eToolIds.PrintArea)
            ToolPrintAreaPersist.SaveDataCapture(crntPDL,
                                                  saveFilename);
        else if (crntToolId == ToolCommonData.eToolIds.PrnPrint)
            ToolPrnPrintPersist.SaveDataCapture(crntPDL,
                                                 saveFilename);
        else if (crntToolId == ToolCommonData.eToolIds.StatusReadback)
            ToolStatusReadbackPersist.SaveDataCapture(crntPDL,
                                                       saveFilename);
        else if (crntToolId == ToolCommonData.eToolIds.TrayMap)
            ToolTrayMapPersist.SaveDataCapture(crntPDL,
                                                saveFilename);

        //----------------------------------------------------------------//

        else if (crntToolId == ToolCommonData.eToolIds.MiscSamples)
            ToolMiscSamplesPersist.SaveDataCapture(crntToolSubId,
                                                    crntPDL,
                                                    saveFilename);

        //----------------------------------------------------------------//

        //  else
        //     Tool MakeOverlay // ***** Do DUMMY procs ? ***** //
        //     Tool PrintLang
        //     Tool PrnAnalyse
        //     Tool SoftFontGen
        //     Tool SymbolSetGen
        //     Tool XXXDiags
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m e t r i c s S a v e F i l e R p t                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current target report file metrics data.                     //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void MetricsSaveFileRpt(
        ToolCommonData.eToolIds crntToolId,
        ReportCore.eRptFileFmt rptFileFmt,
        ReportCore.eRptChkMarks rptChkMarks,
        bool flagOptRptWrap)
    {
        int tmpFmt = (int)rptFileFmt;
        int tmpChkMarks = (int)rptChkMarks;

        if (crntToolId == ToolCommonData.eToolIds.MakeOverlay)
            ToolMakeOverlayPersist.SaveDataRpt(tmpFmt);
        else if (crntToolId == ToolCommonData.eToolIds.PrintLang)
            ToolPrintLangPersist.SaveDataRpt(tmpFmt, tmpChkMarks,
                                              flagOptRptWrap);
        else if (crntToolId == ToolCommonData.eToolIds.PrnAnalyse)
            ToolPrnAnalysePersist.SaveDataRpt(tmpFmt);
        else if (crntToolId == ToolCommonData.eToolIds.SoftFontGenerate)
            ToolSoftFontGenPersist.SaveDataRpt(tmpFmt, tmpChkMarks);
        else if (crntToolId == ToolCommonData.eToolIds.StatusReadback)
            ToolStatusReadbackPersist.SaveDataRpt(tmpFmt);
        else if (crntToolId == ToolCommonData.eToolIds.SymbolSetGenerate)
            ToolSymbolSetGenPersist.SaveDataRpt(tmpFmt);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m e t r i c s S a v e N e t P r i n t e r                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current target network printer metrics data.                 //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void MetricsSaveNetPrinter(
        string netPrinterAddress,
        int netPrinterPort,
        int netPrinterTimeoutSend,
        int netPrinterTimeoutReceive)
    {
        _targetType = eTarget.NetPrinter;

        _netPrinterAddress = netPrinterAddress;
        _netPrinterPort = netPrinterPort;
        _netPrinterTimeoutSend = netPrinterTimeoutSend;
        _netPrinterTimeoutReceive = netPrinterTimeoutReceive;

        TargetPersist.SaveDataNetPrinter((int)_targetType,
                                          _netPrinterAddress,
                                          _netPrinterPort,
                                          _netPrinterTimeoutSend,
                                          _netPrinterTimeoutReceive);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m e t r i c s S a v e W i n P r i n t e r                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current target windows printer metrics data.                 //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void MetricsSaveWinPrinter(string printerName)
    {
        _targetType = eTarget.WinPrinter;

        _winPrinterName = printerName;

        TargetPersist.SaveDataWinPrinter((int)_targetType,
                                          _winPrinterName);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // m e t r i c s S a v e T y p e                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Store current target type index.                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void MetricsSaveType(eTarget type)
    {
        _targetType = type;

        TargetPersist.SaveDataCommon((int)_targetType);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r e q u e s t S t r e a m O p e n                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Open target stream for print job / request.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void RequestStreamOpen(
        ref BinaryWriter binWriter,
        ToolCommonData.eToolIds crntToolId,
        ToolCommonData.eToolSubIds crntToolSubId,
        ToolCommonData.ePrintLang crntPDL)
    {
        //----------------------------------------------------------------//
        //                                                                //
        // Create output file.                                            //
        //                                                                //
        //----------------------------------------------------------------//

        if (_targetType == eTarget.File)
        {
            //------------------------------------------------------------//
            //                                                            //
            // Invoke 'Save As' dialogue.                                 //
            //                                                            //
            //------------------------------------------------------------//

            SaveFileDialog saveDialog;

            int ptr,
                  len;

            string saveDirectory;

            ptr = _saveFilename.LastIndexOf("\\");

            if (ptr <= 0)
            {
                saveDirectory = string.Empty;
                _crntFilename = _saveFilename;
            }
            else
            {
                len = _saveFilename.Length;

                saveDirectory = _saveFilename.Substring(0, ptr);
                _crntFilename = _saveFilename.Substring(ptr + 1,
                                                       len - ptr - 1);
            }

            saveDialog = new SaveFileDialog();

            saveDialog.Filter = "Print Files | *.prn";
            saveDialog.DefaultExt = "prn";
            saveDialog.RestoreDirectory = true;
            saveDialog.InitialDirectory = saveDirectory;
            saveDialog.OverwritePrompt = true;
            saveDialog.FileName = _crntFilename;

            bool? dialogResult = saveDialog.ShowDialog();

            if (dialogResult == true)
            {
                _saveFilename = saveDialog.FileName;
                _crntFilename = _saveFilename;

                MetricsSaveFileCapt(crntToolId, crntToolSubId, crntPDL,
                                     _saveFilename);
            }
        }
        else
        {
            //------------------------------------------------------------//
            //                                                            //
            // The print file is created in the folder associated with    //
            // the TMP environment variable.                              //
            //                                                            //
            //------------------------------------------------------------//

            _crntFilename = Environment.GetEnvironmentVariable("TMP") +
                            "\\" +
                            DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") +
                            ".dia";
        }

        try
        {
            _opStream = File.Create(_crntFilename);
        }

        catch (IOException e)
        {
            MessageBox.Show("IO Exception:\r\n" +
                             e.Message + "\r\n" +
                             "Creating file '" + _crntFilename,
                             "Target file",
                             MessageBoxButton.OK,
                             MessageBoxImage.Error);
        }

        if (_opStream != null)
        {
            _binWriter = new BinaryWriter(_opStream);
            binWriter = _binWriter;
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r e q u e s t S t r e a m W r i t e                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write print job / request stream to target device.                 //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void RequestStreamWrite(bool keepNetConnect)
    {
        if ((_targetType == eTarget.NetPrinter) &&
            (_binWriter != null))
        {
            //------------------------------------------------------------//
            //                                                            //
            // Output to network printer port.                            //
            //                                                            //
            //------------------------------------------------------------//

            bool OK;

            IPAddress ipAddress = new IPAddress(0x00);

            OK = TargetNetPrint.CheckIPAddress(_netPrinterAddress,
                                               ref ipAddress);

            if (!OK)
            {
                MessageBox.Show("invalid address" + _netPrinterAddress,
                                "Printer IP Address",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
            else
            {
                //----------------------------------------------------//
                //                                                    //
                // Ensure that the address is stored in the standard  //
                // notation (dotted-quad for IPv4; colon-hexadecimal  //
                // for IPv6).                                         //
                //                                                    //
                // Then send the generated print stream to the target //
                // printer port.                                      //
                //                                                    //
                //----------------------------------------------------//

                BinaryReader binReader =
                    new BinaryReader(_binWriter.BaseStream);

                TargetNetPrint.SendData(binReader,
                                        _netPrinterAddress,
                                        _netPrinterPort,
                                        _netPrinterTimeoutSend,
                                        _netPrinterTimeoutReceive,
                                        keepNetConnect);

                binReader.Close();
            }
        }
        else if ((_targetType == eTarget.WinPrinter) &&
                 (_binWriter != null))
        {
            //------------------------------------------------------------//
            //                                                            //
            // Output to windows printer.                                 //
            //                                                            //
            //------------------------------------------------------------//

            BinaryReader binReader =
                new BinaryReader(_binWriter.BaseStream);

            TargetWinPrint.SendData(binReader,
                                     _winPrinterName);

            binReader.Close();
        }

        _binWriter.Close();
        _opStream.Close();

        if (_targetType != eTarget.File)
        {
            try
            {
                File.Delete(_crntFilename);
            }

            catch (IOException e)
            {
                MessageBox.Show("IO Exception:\r\n" +
                                 e.Message + "\r\n" +
                                 "Deleting file '" + _crntFilename,
                                 "Target stream",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r e s p o n s e C l o s e C o n n e c t i o n                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Close connection (after having read response block(s)).            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void ResponseCloseConnection()
    {
        if ((_targetType == eTarget.NetPrinter) &&
            (_binWriter != null))
        {
            TargetNetPrint.CloseResponseConnection();
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r e s p o n s e R e a d B l o c k                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Read response block into supplied buffer.                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static bool ResponseReadBlock(int offset,
                                             int bufRem,
                                             ref byte[] replyData,
                                             ref int blockLen)
    {
        bool OK = true;

        //----------------------------------------------------------------//
        //                                                                //
        // Read response block from target.                               //
        //                                                                //
        //----------------------------------------------------------------//

        if ((_targetType == eTarget.NetPrinter) &&
            (_binWriter != null))
        {
            OK = TargetNetPrint.ReadResponseBlock(offset,
                                                   bufRem,
                                                   ref replyData,
                                                   ref blockLen);
        }

        return OK;
    }
}
