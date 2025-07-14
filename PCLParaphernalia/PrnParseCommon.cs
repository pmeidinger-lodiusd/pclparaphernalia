using System.Data;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides print-language-independent routines associated with
    /// 'parsing' of print file.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class PrnParseCommon
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

        static bool _showMacroData = true;

        static PrnParse.eParseType _parseType;

        static int _macroLevel = 0;

        static readonly string _colName_RowType = PrnParseConstants.cRptA_colName_RowType;
        static readonly string _colName_Action = PrnParseConstants.cRptA_colName_Action;
        static readonly string _colName_Offset = PrnParseConstants.cRptA_colName_Offset;
        static readonly string _colName_Type = PrnParseConstants.cRptA_colName_Type;
        static readonly string _colName_Seq = PrnParseConstants.cRptA_colName_Seq;
        static readonly string _colName_Desc = PrnParseConstants.cRptA_colName_Desc;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d D a t a R o w                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Adds a row, with numeric offset value, to the output table.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void addDataRow(
            PrnParseRowTypes.eType rowType,
            DataTable table,
            PrnParseConstants.eOvlShow makeOvlShow,
            PrnParseConstants.eOptOffsetFormats indxOffsetFormat,
            int offset,
            int level,
            string type,
            string seq,
            string desc)
        {
            if (_parseType == PrnParse.eParseType.ScanForPDL)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Output inhibited.                                          //
                //                                                            //
                //------------------------------------------------------------//
            }
            else if ((_parseType == PrnParse.eParseType.MakeOverlay)
                                           &&
                     (makeOvlShow ==
                        PrnParseConstants.eOvlShow.None))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Output inhibited.                                          //
                //                                                            //
                //------------------------------------------------------------//
            }
            else if ((!_showMacroData) && (_macroLevel > 0))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Output inhibited (e.g. don't show PCL macro contents); do  //
                // nothing.                                                   //
                //                                                            //
                //------------------------------------------------------------//
            }
            else
            {
                DataRow row;

                string offsetText;

                row = table.NewRow();

                if (offset < 0)
                {
                    if (offset ==
                        (int)PrnParseConstants.eOffsetPosition.StartOfFile)
                        offsetText = "<Start>";
                    else if (offset ==
                        (int)PrnParseConstants.eOffsetPosition.EndOfFile)
                        offsetText = "<End>";
                    else
                        offsetText = string.Empty;
                }
                else
                {
                    if (indxOffsetFormat ==
                        PrnParseConstants.eOptOffsetFormats.Decimal)
                    {
                        if (level == 0)
                            offsetText = string.Format("{0:d10}", offset);
                        else
                            offsetText = string.Format("{0:d2}", level) + ":" +
                                         string.Format("{0:d10}", offset);
                    }
                    else
                    {
                        if (level == 0)
                            offsetText = string.Format("{0:x8}", offset);
                        else
                            offsetText = string.Format("{0:x2}", level) + ":" +
                                         string.Format("{0:x8}", offset);
                    }
                }

                if ((_parseType == PrnParse.eParseType.MakeOverlay) &&
                    (makeOvlShow != PrnParseConstants.eOvlShow.None))
                {
                    row[_colName_Action] = makeOvlShow.ToString();
                }

                row[_colName_RowType] = (int)rowType;
                // row[_colName_RowType] = rowType.ToString(); // **** DIAG ************
                row[_colName_Offset] = offsetText;
                row[_colName_Type] = type;
                row[_colName_Seq] = seq;
                row[_colName_Desc] = desc;

                table.Rows.Add(row);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // a d d T e x t R o w                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Adds a row, without numeric offset value, to the output table.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void addTextRow(
            PrnParseRowTypes.eType rowType,
            DataTable table,
            PrnParseConstants.eOvlShow makeOvlShow,
            string offsetText,
            string type,
            string seq,
            string desc)
        {
            if (_parseType == PrnParse.eParseType.ScanForPDL)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Output inhibited.                                          //
                //                                                            //
                //------------------------------------------------------------//
            }
            else if ((_parseType == PrnParse.eParseType.MakeOverlay)
                                           &&
                     (makeOvlShow ==
                        PrnParseConstants.eOvlShow.None))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Output inhibited.                                          //
                //                                                            //
                //------------------------------------------------------------//
            }
            else if ((!_showMacroData) && (_macroLevel > 0))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Output inhibited (e.g. don't show PCL macro contents); do  //
                // nothing.                                                   //
                //                                                            //
                //------------------------------------------------------------//
            }
            else
            {
                DataRow row;

                row = table.NewRow();

                if (_parseType == PrnParse.eParseType.MakeOverlay)
                {
                    if (makeOvlShow == PrnParseConstants.eOvlShow.Insert)
                        row[_colName_Action] = "Insert";
                    else
                        row[_colName_Action] = string.Empty;
                }

                row[_colName_RowType] = (int)rowType;
                // row[_colName_RowType] = rowType.ToString(); // **** DIAG ************
                row[_colName_Offset] = offsetText;
                row[_colName_Type] = type;
                row[_colName_Seq] = seq;
                row[_colName_Desc] = desc;

                table.Rows.Add(row);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b y t e A r r a y T o H e x S t r i n g                            //
        //--------------------------------------------------------------------//

        public static string byteArrayToHexString(byte[] byteArray,
                                                  int startByte,
                                                  int byteCt)
        {
            const int triplet = 3;

            int arrayLen = byteArray.Length;

            char[] chars = new char[byteCt * triplet];

            for (int i = 0; i < byteCt; i++)
            {
                int b = byteArray[startByte + i];
                int j = i * triplet;

                chars[j] = PrnParseConstants.cHexChars[b >> 4];
                chars[j + 1] = PrnParseConstants.cHexChars[b & 0xF];
                chars[j + 2] = (char)PrnParseConstants.asciiSpace;
            }

            return new string(chars);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b y t e A r r a y P a i r T o H e x S t r i n g                    //
        //--------------------------------------------------------------------//

        public static bool byteArrayPairToHexString(byte[] byteArray,
                                                        int startByte,
                                                        int byteCt,
                                                        ref string hexData)
        {
            const int quintet = 5;

            bool all_ffs = true;

            int pairCt = byteCt / 2;

            int b,
                  c;

            int j,
                  k;

            char[] chars = new char[pairCt * quintet];

            for (int i = 0; i < pairCt; i++)
            {
                k = i * 2;

                b = byteArray[startByte + k];
                c = byteArray[startByte + k + 1];

                if ((b != 0xff) || (c != 0xff))
                    all_ffs = false;

                j = i * quintet;

                chars[j] = PrnParseConstants.cHexChars[b >> 4];
                chars[j + 1] = PrnParseConstants.cHexChars[b & 0xF];
                chars[j + 2] = PrnParseConstants.cHexChars[c >> 4];
                chars[j + 3] = PrnParseConstants.cHexChars[c & 0xF];
                chars[j + 4] = (char)PrnParseConstants.asciiSpace;
            }

            hexData = new string(chars);

            //    return new String(chars);
            return all_ffs;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b y t e T o H e x S t r i n g                                      //
        //--------------------------------------------------------------------//

        public static string byteToHexString(byte byteVal)
        {
            char[] chars = new char[2];

            int b = byteVal;

            chars[0] = PrnParseConstants.cHexChars[b >> 4];
            chars[1] = PrnParseConstants.cHexChars[b & 0xF];

            return new string(chars);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // b y t e T o S t r i n g                                            //
        //--------------------------------------------------------------------//

        public static string byteToString(byte byteVal)
        {
            return ((char)byteVal).ToString();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e R u n T y p e                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set value indicating current tool ('PrnAnalyse' or 'MakeMacro'.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void initialiseRunType(PrnParse.eParseType parseType)
        {
            _parseType = parseType;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i s A l p h a b e t i c                                            //
        //--------------------------------------------------------------------//

        public static bool isAlphabetic(byte byteVal)
        {
            if (((byteVal >= PrnParseConstants.asciiAlphaLCMin)
                                   &&
                 (byteVal <= PrnParseConstants.asciiAlphaLCMax))
                                   ||
                ((byteVal >= PrnParseConstants.asciiAlphaUCMin)
                                   &&
                 (byteVal <= PrnParseConstants.asciiAlphaUCMax)))
                return true;
            else
                return false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t D i s p l a y C r i t e r i a                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set markers to indicate whether subsequent AddRow actions will     //
        // display data, or not.                                              //
        // This feature is intended to allow for the display, or otherwise,   //
        // of the contents of PCL macros:                                     //
        //    -  The ShowData variable indicates the state of the             //
        //       PCLShowMacroData option.                                     //
        //    -  Each time that a PCL StartMacro sequence is found, the       //
        //       current level is incremented.                                //
        //    -  Each time that a PCL StopMacro sequence is found, the        //
        //       current level is decremented.                                //
        //    -  Thus we know whether we are within a macro.                  //
        //       Although macro Calls can be nested, macro definitions        //
        //       cannot; but we need to cater for someone trying to do this.  //
        //    -  Exit from the language (via UEL sequence) should reset the   //
        //       level to zero.                                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void setDisplayCriteria(bool showMacroData,
                                              int macroLevel)
        {
            _showMacroData = showMacroData;
            _macroLevel = macroLevel;
        }
    }
}