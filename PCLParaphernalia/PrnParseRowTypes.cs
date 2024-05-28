namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a set of PCL 'status readback' Entity Type objects.</para>
    /// <para>© Chris Hutchinson 2017</para>
    ///
    /// </summary>
    static class PrnParseRowTypes
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum Type
        {
            MsgError = 0,
            MsgWarning,
            MsgComment,
            MsgDiag,
            DataBinary,
            DataText,
            PCLSeqComplex,
            PCLSeqSimple,
            PCLControlCode,
            PCLDecode,
            PCLFontHddr,
            PCLFontChar,
            HPGL2Command,
            HPGL2ControlCode,
            PCLXLStreamHddr,
            PCLXLWhiteSpace,
            PCLXLOperator,
            PCLXLAttribute,
            PCLXLDataType,
            PCLXLDataValue,
            PCLXLFontHddr,
            PCLXLFontChar,
            PJLCommand,
            PMLSeq,
            PrescribeCommand
        }

        //--------------------------------------------------------------------//
        //                                                                    //
        // Must keep this in the same order as the eType enumeration.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly PrnParseRowType[] _rowTypes =
        {
            new PrnParseRowType(Type.MsgError, "Error message"),
            new PrnParseRowType(Type.MsgWarning, "Warning message"),
            new PrnParseRowType(Type.MsgComment, "Comment"),
            new PrnParseRowType(Type.MsgDiag, "Diagnostic"),
            new PrnParseRowType(Type.DataBinary, "Binary data"),
            new PrnParseRowType(Type.DataText, "Text data"),
            new PrnParseRowType(Type.PCLSeqComplex, "PCL complex sequence"),
            new PrnParseRowType(Type.PCLSeqSimple, "PCL simple sequence"),
            new PrnParseRowType(Type.PCLControlCode, "PCL control code"),
            new PrnParseRowType(Type.PCLDecode, "PCL binary data decode"),
            new PrnParseRowType(Type.PCLFontHddr, "PCL soft font header"),
            new PrnParseRowType(Type.PCLFontChar, "PCL soft font character"),
            new PrnParseRowType(Type.HPGL2Command, "HP-GL/2 command"),
            new PrnParseRowType(Type.HPGL2ControlCode, "HP-GL/2 control code"),
            new PrnParseRowType(Type.PCLXLStreamHddr, "PCL XL stream header"),
            new PrnParseRowType(Type.PCLXLWhiteSpace, "PCL XL whitespace"),
            new PrnParseRowType(Type.PCLXLOperator, "PCL XL operator"),
            new PrnParseRowType(Type.PCLXLAttribute, "PCL XL attribute"),
            new PrnParseRowType(Type.PCLXLDataType, "PCL XL data type"),
            new PrnParseRowType(Type.PCLXLDataValue, "PCL XL data value"),
            new PrnParseRowType(Type.PCLXLFontHddr, "PCL XL soft font header"),
            new PrnParseRowType(Type.PCLXLFontChar, "PCL XL soft font character"),
            new PrnParseRowType(Type.PJLCommand, "PJL command"),
            new PrnParseRowType(Type.PMLSeq, "PML sequence"),
            new PrnParseRowType(Type.PrescribeCommand, "Prescribe command")
        };

        private static readonly int _rowTypeCount = _rowTypes.GetUpperBound(0) + 1;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o u n t                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return count of Entity Type definitions.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetCount()
        {
            return _rowTypeCount;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e s c                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return description associated with specified row type index.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetDesc(int selection)
        {
            return _rowTypes[selection].GetDesc();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return type of type.                                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static Type GetType(int selection)
        {
            return _rowTypes[selection].GetRowType();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t D e f a u l t C l r s                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Sets default background/foreground colours for each row type.      //
        //                                                                    //
        // Use the colours defined by the standard 'Colors' class object.     //
        // This list should consist of 140 standard colours (as defined in    //
        // .net, Unix X11, etc.) plus the 'transparent' colour.               //
        // This list is unlikely to change, so we can store the indices of    //
        // the selected colours and be relatively confident that these will   //
        // always refer to the same actual colours.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SetDefaultClrs(ref int[] indxClrMapBack, ref int[] indxClrMapFore)
        {
            Type rowType;

            for (int i = 0; i < _rowTypeCount; i++)
            {
                rowType = (Type)i;

                switch (rowType)
                {
                    case Type.MsgError:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.Crimson;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Yellow;

                        break;

                    case Type.MsgWarning:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.Red;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.White;

                        break;

                    case Type.MsgComment:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.Bisque;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Blue;

                        break;

                    case Type.MsgDiag:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.Fuchsia;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Yellow;

                        break;

                    case Type.DataBinary:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.Ivory;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.SlateGray;

                        break;

                    case Type.DataText:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.Azure;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Black;

                        break;

                    case Type.PCLSeqComplex:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.LightYellow;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.DarkBlue;

                        break;

                    case Type.PCLSeqSimple:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.LightYellow;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Blue;

                        break;

                    case Type.PCLControlCode:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.Honeydew;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Black;

                        break;

                    case Type.PCLDecode:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.White;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Black;

                        break;

                    case Type.PCLFontHddr:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.LightCyan;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.BlueViolet;

                        break;

                    case Type.PCLFontChar:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.LightCyan;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.DarkMagenta;

                        break;

                    case Type.HPGL2Command:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.LemonChiffon;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Teal;

                        break;

                    case Type.HPGL2ControlCode:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.PaleGoldenrod;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Teal;

                        break;

                    case Type.PCLXLStreamHddr:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.LightGreen;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Blue;

                        break;

                    case Type.PCLXLWhiteSpace:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.SeaShell;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Black;

                        break;

                    case Type.PCLXLOperator:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.LightCyan;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Red;

                        break;

                    case Type.PCLXLAttribute:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.MistyRose;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Red;

                        break;

                    case Type.PCLXLDataType:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.FloralWhite;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Teal;

                        break;

                    case Type.PCLXLDataValue:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.FloralWhite;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Blue;

                        break;

                    case Type.PCLXLFontHddr:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.LightCyan;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Blue;

                        break;

                    case Type.PCLXLFontChar:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.LightCyan;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.DarkMagenta;

                        break;

                    case Type.PJLCommand:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.Khaki;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Black;

                        break;

                    case Type.PMLSeq:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.Lavender;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Blue;

                        break;

                    case Type.PrescribeCommand:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.PeachPuff;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Teal;

                        break;

                    default:

                        indxClrMapBack[i] = (int)PrnParseConstants.StdClrs.White;
                        indxClrMapFore[i] = (int)PrnParseConstants.StdClrs.Black;

                        break;
                }
            }
        }
    }
}