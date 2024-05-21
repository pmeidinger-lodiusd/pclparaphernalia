using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a set of PCL Symbol Set map objects.</para>
    /// <para>© Chris Hutchinson 2015</para>
    ///
    /// </summary>
    static partial class PCLSymSetMaps
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private const int _hex0080 = 128;
        private const int _hex00a0 = 160;
        private const int _hex00c0 = 192;
        private const int _hex00e0 = 224;
        private const int _hex0100 = 256;

        public enum SymSetMapId
        {
            mapNull,        // special sets
            mapSymbol,
            mapUserSet,
            map_0D,         // mono-byte sets
            map_0H,
            map_0I,
            map_0N,
            map_0S,
            map_0U,
            map_1E,
            map_1F,
            map_1G,
            map_1T,
            map_1U,
            map_2N,
            map_2S,
            map_3N,
            map_3R,
            map_4N,
            map_4U,
            map_5M,
            map_5N,
            map_5T,
            map_6J,
            map_6N,
            map_7H,
            map_7J,
            map_8G,
            map_8H,
            map_8M,
            map_8U,
            map_8V,
            map_9E,
            map_9G,
            map_9J,
            map_9N,
            map_9R,
            map_9T,
            map_9U,
            map_9V,
            map_10G,
            map_10J,
            map_10N,
            map_10U,
            map_10V,
            map_11N,
            map_11U,
            map_12G,
            map_12J,
            map_12N,
            map_12U,
            map_13U,
            map_14R,
            map_15H,
            map_15U,
            map_17U,
            map_19L,
            map_19U,
            map_26U,
            map_x901T,
            map_x1001T,
            map_x1018C,     // duo-byte sets
            map_x1018T,
            map_x1019K,
            map_x1020C
        };

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly List<PCLSymSetMap> _sets =
            new List<PCLSymSetMap>();

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L S y m S e t M a p s                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PCLSymSetMaps()
        {
            PopulateUnicodeMaps();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o d e p o i n t M a x                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the maximum code-point defined by the mapping array(s).     //
        // This effectively provides the maximum character set size.          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static ushort GetCodepointMax(int selection)
        {
            return _sets[selection].CodepointMax;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o d e p o i n t M i n                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the minimum code-point defined by the mapping array(s).     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static ushort GetCodepointMin(int selection)
        {
            return _sets[selection].CodepointMin;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t M a p A r r a y                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return array defining mapping(s) of the character set to Unicode   //
        // for the specified Symbol Set map index.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static ushort[] GetMapArray(int selection,
                                             bool flagMapPCL)
        {
            if (flagMapPCL)
                return _sets[selection].MapDataPCL;
            else
                return _sets[selection].MapDataStd;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t M a p A r r a y S y m b o l                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return array defining mapStd of the TTF 'Symbol' encoding set to  //
        // Unicode.                                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static ushort[] GetMapArraySymbol()
        {
            return _sets[(int)SymSetMapId.mapSymbol].MapDataStd;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t M a p A r r a y U s e r S e t                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return array defining mapStd of the User-defined symbol set.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static ushort[] GetMapArrayUserSet()
        {
            return _sets[(int)SymSetMapId.mapUserSet].MapDataUserSet;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m a p p i n g                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If mapping tables have been defined, return string defining        //
        // mapping of the character set to Unicode for the specified Symbol   //
        // Set map index.                                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string Mapping(int selection,
                                      bool flagMapPCL,
                                      bool differentOnly)
        {
            if (flagMapPCL)
            {
                if (differentOnly)
                    return _sets[selection].MappingPCLDiff;
                else
                    return _sets[selection].MappingPCL;
            }
            else
            {
                return _sets[selection].MappingStd;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m a p p i n g D i f f                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If mapping tables have been defined, return string defining the    //
        // differences in mapping between the standard and LaserJet           //
        // definitions of the character set to Unicode  for the specified     //
        // Symbol Set map index.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string MappingDiff(int selection)
        {
            return _sets[selection].MappingDiff;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m a p R o w s                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If mapping tables have been defined, return an array of strings    //
        // (one per row, plus any inter-range gaps) defining mapping of the   //
        // character set to Unicode for the specified Symbol Set map index.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string[] MapRows(int selection, bool flagMapPCL, bool differentOnly)
        {
            if (flagMapPCL)
            {
                if (differentOnly)
                    return _sets[selection].MapRowsPCLDiff;
                else
                    return _sets[selection].MapRowsPCL;
            }
            else
            {
                return _sets[selection].MapRowsStd;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m a p R o w s D i f f                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If mapping tables have been defined, return string defining the    //
        // differences in mapping between the standard and LaserJet           //
        // definitions of the character set to Unicode for the specified      //
        // Symbol Set map index.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string[] MapRowsDiff(int selection)
        {
            return _sets[selection].MapRowsDiff;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // n u l l M a p P C L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns a boolean value indicating whether or not the LaserJet     //
        // (PCL) map is null for the specified Symbol Set map index.          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool NullMapPCL(int selection)
        {
            return _sets[selection].NullMapPCL;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // n u l l M a p S t d                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns a boolean value indicating whether or not the Standard     //
        // (Strict) map is null for the specified Symbol Set map index.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool NullMapStd(int selection)
        {
            return _sets[selection].NullMapStd;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p o p u l a t e U n i c o d e M a p s                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Populate the mapStd arrays for known mapped symbol sets.          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PopulateUnicodeMaps()
        {
            foreach (SymSetMapId x in Enum.GetValues(typeof(SymSetMapId)))
            {
                switch (x)
                {
                    // Must be in same order as eSymSetMap enum and all of    //
                    // the values must be present.                            //

                    case SymSetMapId.mapNull:
                        unicodeMapNull();
                        break;
                    case SymSetMapId.mapSymbol:
                        unicodeMapSymbol();
                        break;
                    case SymSetMapId.mapUserSet:
                        unicodeMapUserSet();
                        break;
                    case SymSetMapId.map_0D:
                        unicodeMap_0D();
                        break;
                    case SymSetMapId.map_0H:
                        unicodeMap_0H();
                        break;
                    case SymSetMapId.map_0I:
                        unicodeMap_0I();
                        break;
                    case SymSetMapId.map_0N:
                        unicodeMap_0N();
                        break;
                    case SymSetMapId.map_0S:
                        unicodeMap_0S();
                        break;
                    case SymSetMapId.map_0U:
                        unicodeMap_0U();
                        break;
                    case SymSetMapId.map_1E:
                        unicodeMap_1E();
                        break;
                    case SymSetMapId.map_1F:
                        unicodeMap_1F();
                        break;
                    case SymSetMapId.map_1G:
                        unicodeMap_1G();
                        break;
                    case SymSetMapId.map_1T:
                        unicodeMap_1T();
                        break;
                    case SymSetMapId.map_1U:
                        unicodeMap_1U();
                        break;
                    case SymSetMapId.map_2N:
                        unicodeMap_2N();
                        break;
                    case SymSetMapId.map_2S:
                        unicodeMap_2S();
                        break;
                    case SymSetMapId.map_3N:
                        unicodeMap_3N();
                        break;
                    case SymSetMapId.map_3R:
                        unicodeMap_3R();
                        break;
                    case SymSetMapId.map_4N:
                        unicodeMap_4N();
                        break;
                    case SymSetMapId.map_4U:
                        unicodeMap_4U();
                        break;
                    case SymSetMapId.map_5M:
                        unicodeMap_5M();
                        break;
                    case SymSetMapId.map_5N:
                        unicodeMap_5N();
                        break;
                    case SymSetMapId.map_5T:
                        unicodeMap_5T();
                        break;
                    case SymSetMapId.map_6J:
                        unicodeMap_6J();
                        break;
                    case SymSetMapId.map_6N:
                        unicodeMap_6N();
                        break;
                    case SymSetMapId.map_7H:
                        unicodeMap_7H();
                        break;
                    case SymSetMapId.map_7J:
                        unicodeMap_7J();
                        break;
                    case SymSetMapId.map_8G:
                        unicodeMap_8G();
                        break;
                    case SymSetMapId.map_8H:
                        UnicodeMap_8H();
                        break;
                    case SymSetMapId.map_8M:
                        UnicodeMap_8M();
                        break;
                    case SymSetMapId.map_8U:
                        UnicodeMap_8U();
                        break;
                    case SymSetMapId.map_8V:
                        unicodeMap_8V();
                        break;
                    case SymSetMapId.map_9E:
                        unicodeMap_9E();
                        break;
                    case SymSetMapId.map_9G:
                        unicodeMap_9G();
                        break;
                    case SymSetMapId.map_9J:
                        unicodeMap_9J();
                        break;
                    case SymSetMapId.map_9N:
                        unicodeMap_9N();
                        break;
                    case SymSetMapId.map_9R:
                        unicodeMap_9R();
                        break;
                    case SymSetMapId.map_9T:
                        unicodeMap_9T();
                        break;
                    case SymSetMapId.map_9U:
                        unicodeMap_9U();
                        break;
                    case SymSetMapId.map_9V:
                        unicodeMap_9V();
                        break;
                    case SymSetMapId.map_10G:
                        unicodeMap_10G();
                        break;
                    case SymSetMapId.map_10J:
                        unicodeMap_10J();
                        break;
                    case SymSetMapId.map_10N:
                        unicodeMap_10N();
                        break;
                    case SymSetMapId.map_10U:
                        unicodeMap_10U();
                        break;
                    case SymSetMapId.map_10V:
                        unicodeMap_10V();
                        break;
                    case SymSetMapId.map_11N:
                        unicodeMap_11N();
                        break;
                    case SymSetMapId.map_11U:
                        unicodeMap_11U();
                        break;
                    case SymSetMapId.map_12G:
                        unicodeMap_12G();
                        break;
                    case SymSetMapId.map_12J:
                        unicodeMap_12J();
                        break;
                    case SymSetMapId.map_12N:
                        unicodeMap_12N();
                        break;
                    case SymSetMapId.map_12U:
                        unicodeMap_12U();
                        break;
                    case SymSetMapId.map_13U:
                        unicodeMap_13U();
                        break;
                    case SymSetMapId.map_14R:
                        unicodeMap_14R();
                        break;
                    case SymSetMapId.map_15H:
                        unicodeMap_15H();
                        break;
                    case SymSetMapId.map_15U:
                        unicodeMap_15U();
                        break;
                    case SymSetMapId.map_17U:
                        unicodeMap_17U();
                        break;
                    case SymSetMapId.map_19L:
                        unicodeMap_19L();
                        break;
                    case SymSetMapId.map_19U:
                        unicodeMap_19U();
                        break;
                    case SymSetMapId.map_26U:
                        unicodeMap_26U();
                        break;
                    case SymSetMapId.map_x901T:
                        unicodeMap_x901T();
                        break;
                    case SymSetMapId.map_x1001T:
                        unicodeMap_x1001T();
                        break;
                    case SymSetMapId.map_x1018C:
                        unicodeMap_x1018C();
                        break;
                    case SymSetMapId.map_x1018T:
                        unicodeMap_x1018T();
                        break;
                    case SymSetMapId.map_x1019K:
                        unicodeMap_x1019K();
                        break;
                    case SymSetMapId.map_x1020C:
                        unicodeMap_x1020C();
                        break;
                    default:
                        unicodeMapMissing();   // to catch undefined maps //
                        break;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t M a p A r r a y U s e r S e t                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Store array defining mapStd of the User-defined symbol set.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void SetMapArrayUserSet(ushort[] map)
        {
            _sets[(int)SymSetMapId.mapUserSet].MapDataUserSet = map;
        }
    }
}