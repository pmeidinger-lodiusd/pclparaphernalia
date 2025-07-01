using System;
using System.Data;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides log handling for the Soft Font Generate tool.
    /// 
    /// © Chris Hutchinson 2011
    /// 
    /// </summary>

    static class ToolSoftFontGenLog
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const int cGridMain_Col_0_Max = 18;
        const int cGridMain_Col_1_Max = 80;
        const int cColSeparatorLen = 2;

        const byte cASCII_Space = 0x20;
        const byte cASCII_LineFeed = 0x0a;
        const byte cASCII_CarriageReturn = 0x0d;
        const byte cASCII_HorizontalTab = 0x09;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o g C h a r D e t a i l s                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Log details of the specified character to the process log.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void logCharDetails(DataTable table,
                                           bool glyphAlreadyPresent,
                                           bool composite,
                                           ushort charCode,
                                           ushort codepoint,
                                           ushort glyphID,
                                           ushort depth,
                                           ushort glyphWidth,
                                           ushort glyphHeight,
                                           short glyphLSB,
                                           short glyphTSB,
                                           uint glyphOffset,
                                           uint glyphLength)
        {
            const int colDecCode = 0;
            const int colHexCode = 1;
            const int colUniCode = 2;
            const int colGlyphId = 3;
            const int colAbsent = 4;
            const int colPrevious = 5;
            const int colComposite = 6;
            const int colDepth = 7;
            const int colWidth = 8;
            const int colLSB = 9;
            const int colHeight = 10;
            const int colTSB = 11;
            const int colLength = 12;

            DataRow row;

            row = table.NewRow();

            if (glyphAlreadyPresent)
            {
                row[colGlyphId] = glyphID;
                row[colAbsent] = false;
                row[colPrevious] = true;

                if (composite)
                    row[colComposite] = "true";
                else
                    row[colComposite] = "false";
            }
            else
            {
                row[colDecCode] = charCode;
                row[colHexCode] = "0x" + charCode.ToString("x4");

                if ((codepoint == 0) && (charCode == 0xffff))           // ??????????????????????? //
                    row[colUniCode] = "";
                else
                    row[colUniCode] = "U+" + codepoint.ToString("x4");

                row[colGlyphId] = glyphID;
                row[colAbsent] = false;

                if (glyphAlreadyPresent)
                    row[colPrevious] = "true";
                else
                    row[colPrevious] = "false";

                if (composite)
                    row[colComposite] = "true";
                else
                    row[colComposite] = "false";

                row[colDepth] = depth;

                row[colWidth] = glyphWidth;
                row[colLSB] = glyphLSB;

                row[colHeight] = glyphHeight;   // for vertical rotated chars. 
                row[colTSB] = glyphTSB;         // for vertical rotated chars. 

                row[colLength] = glyphLength;
            }

            table.Rows.Add(row);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o g E r r o r                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Log specified error message to the process log, then display the   //
        // error dialogue.                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void logError(DataTable table,
                                    MessageBoxImage type,
                                    string message)
        {
            if (type == MessageBoxImage.Information)
                logNameAndValue(
                    table, true, true, "*** COMMENT ***", message);
            else if (type == MessageBoxImage.Warning)
                logNameAndValue(
                    table, true, true, "*** WARNING ***", message);
            else
                logNameAndValue(
                    table, true, true, "*** ERROR ***", message);

            MessageBox.Show(message,
                             "Processing font",
                             MessageBoxButton.OK,
                             type);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o g M i s s i n g C h a r                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Log details of the specified character which does not have a glyph.//
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void logMissingChar(DataTable table,
                                           ushort charCode,
                                           ushort codepoint)
        {
            const int colDecCode = 0;
            const int colHexCode = 1;
            const int colUniCode = 2;
            const int colAbsent = 4;
            const int colPrevious = 5;
            const int colComposite = 6;

            DataRow row;

            row = table.NewRow();

            row[colDecCode] = charCode;
            row[colHexCode] = "0x" + charCode.ToString("x4");
            row[colUniCode] = "U+" + codepoint.ToString("x4");
            row[colAbsent] = true;
            row[colPrevious] = false;
            row[colComposite] = false;

            table.Rows.Add(row);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o g N a m e A n d V a l u e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Log specified name and value pair to the process log.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void logNameAndValue(DataTable table,
                                           bool blankBefore,
                                           bool blankAfter,
                                           string name,
                                           string value)
        {
            const int colName = 0;
            const int colValue = 1;

            DataRow row;

            int vLen,
                  vStart,
                  vPos,
                  vRem,
                  rowLen;

            string logData,
                   tempStr;

            char lastChar,
                 nextChar;

            bool firstLine;

            firstLine = true;
            vLen = value.Length;
            vStart = 0;

            if (blankBefore)
            {
                row = table.NewRow();

                row[colName] = "";
                row[colValue] = "";

                table.Rows.Add(row);
            }

            if (vLen == 0)
            {
                row = table.NewRow();

                row[colName] = name;
                row[colValue] = "";

                table.Rows.Add(row);
            }
            else
            {
                while (vStart < vLen)
                {
                    row = table.NewRow();

                    if (firstLine)
                    {
                        row[colName] = name;
                    }
                    else
                    {
                        row[colName] = "";
                    }

                    //----------------------------------------------------//
                    //                                                    //
                    // Remove leading spaces.                             //
                    //                                                    //
                    //----------------------------------------------------//

                    while ((vStart < vLen) &&
                           (value[vStart] == cASCII_Space))
                    {
                        vStart++;
                    }

                    vRem = vLen - vStart;

                    //----------------------------------------------------//
                    //                                                    //
                    // Process value.                                     //
                    //                                                    //
                    //----------------------------------------------------//

                    if (vRem != 0)
                    {
                        if (vRem > cGridMain_Col_1_Max)
                        {
                            rowLen = cGridMain_Col_1_Max;
                            nextChar = value[vStart + rowLen];
                        }
                        else
                        {
                            rowLen = vRem;
                            nextChar = (char)cASCII_Space;
                        }

                        lastChar = value[vStart + rowLen - 1];

                        tempStr = value.Substring(vStart, rowLen);

                        //----------------------------------------------------//
                        //                                                    //
                        // Check for LineFeed and other special characters.   //
                        //                                                    //
                        //----------------------------------------------------//

                        logData = tempStr.Replace((char)cASCII_CarriageReturn,
                                                   (char)cASCII_Space);

                        tempStr = logData;

                        logData = tempStr.Replace((char)cASCII_HorizontalTab,
                                                   (char)cASCII_Space);

                        vPos = logData.IndexOf((char)cASCII_LineFeed);

                        if (vPos != -1)
                        {
                            rowLen = vPos;

                            if (rowLen == 0)
                            {
                                rowLen = 1;
                                logData = " ";
                                lastChar = (char)cASCII_Space;
                                nextChar = (char)cASCII_Space;
                            }
                            else
                            {
                                logData = value.Substring(vStart, rowLen - 1);
                                lastChar = value[vStart + rowLen - 1];
                                nextChar = (char)cASCII_Space;
                            }
                        }

                        //----------------------------------------------------//
                        //                                                    //
                        // Do primitive word wrap (search for previous space).//
                        // This is based only on the number of characters (it //
                        // does not take into account the widths of each      //
                        // character), and is therefore suitable only for use //
                        // with a fixed-pitch font.                           //
                        //                                                    //
                        //----------------------------------------------------//

                        if ((nextChar != cASCII_Space) &&
                            (lastChar != cASCII_Space))
                        {
                            vPos = logData.LastIndexOf((char)cASCII_Space);

                            if (vPos > 0)
                            {
                                rowLen = vPos;
                                logData = value.Substring(vStart, rowLen);
                            }
                        }

                        //----------------------------------------------------//
                        //                                                    //
                        // Write row.                                         //
                        //                                                    //
                        //----------------------------------------------------//

                        vStart = vStart + rowLen;
                        firstLine = false;

                        row[colValue] = logData;

                        table.Rows.Add(row);
                    }
                }
            }

            if (blankAfter)
            {
                row = table.NewRow();

                row[colName] = "";
                row[colValue] = "";

                table.Rows.Add(row);
            }
        }
    }
}
