using System;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles a PCL Symbol Set object.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(Feature = "renaming", ApplyToMembers = true)]

    class PCLSymbolSet
    {
        private readonly PCLSymSetMaps.SymSetMapId _mapId;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L S y m b o l S e t                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLSymbolSet(PCLSymbolSets.SymSetGroup group,
                             PCLSymSetTypes.Index indxType,
                             PCLTextParsingMethods.Index parsingMethod,
                             ushort kind1,
                             string alias,
                             string name,
                             bool mapped,
                             PCLSymSetMaps.SymSetMapId mapId)
        {
            Group = group;
            Type = indxType;
            ParsingMethod = parsingMethod;
            Kind1 = kind1;
            Alias = alias;
            Name = name;

            if ((kind1 < 1)         // 1        = 0A    //
                    ||
                (kind1 > 65530))    // 65530    = 2047Z //
            {
                IdNum = 0;
                IdAlpha = 0x3f;    // ? //
            }
            else
            {
                IdNum = (ushort)(kind1 / 32);
                IdAlpha = (byte)(kind1 - (IdNum * 32) + 64);
            }

            Id = IdNum.ToString() + Convert.ToChar(IdAlpha);

            FlagMapped = mapped;
            _mapId = mapId;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // A l i a s                                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the alias name.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Alias { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g M a p P C L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the flag indicating whether or not the symbol set has a     //
        // defined mapping table for the PCL (LaserJet) variant.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagMapPCL
        {
            get
            {
                if (!FlagMapped)
                    return false;
                else if (PCLSymSetMaps.NullMapPCL((int)_mapId))
                    return false;
                else
                    return true;
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g M a p p e d                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the flag indicating whether or not the symbol set has a     //
        // defined mapping table.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagMapped { get; } = false;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g M a p S t d                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the flag indicating whether or not the symbol set has a     //
        // defined mapping table for the Standard (Strict) variant.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagMapStd
        {
            get
            {
                if (!FlagMapped)
                    return false;
                else if (PCLSymSetMaps.NullMapStd((int)_mapId))
                    return false;
                else
                    return true;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t S y m s e t D a t a                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return identifying (reference) name and Kind1 value of the         //
        // selected symbol set.                                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool GetSymsetData(ref ushort kind1, ref ushort idNum, ref string name)
        {
            bool matches = false;

            if ((Group == PCLSymbolSets.SymSetGroup.Preset) ||
                (Group == PCLSymbolSets.SymSetGroup.Unicode))
            {
                matches = true;
                kind1 = Kind1;
                idNum = IdNum;
                name = Name;
            }

            return matches;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t S y m s e t D a t a F o r I d A l p h a                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return identifying (reference) name and Kind1 value of the         //
        // selected symbol set if the 'Id Alpha' value matches the specified  //
        // value.                                                             //
        // The symbol set is only eligible for the check if it is of one of   //
        // the following types:                                               //
        //      Preset  -   standard HP-defined sets                          //
        //      Unbound -   special value for PCL unbound fonts               //
        //      Unicode -   standard HP-defined set                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool GetSymsetDataForIdAlpha(byte idAlpha, ref ushort kind1, ref ushort idNum, ref string name)
        {
            bool matches = false;

            if ((Group != PCLSymbolSets.SymSetGroup.Preset) &&
                (Group != PCLSymbolSets.SymSetGroup.Unbound) &&
                (Group != PCLSymbolSets.SymSetGroup.Unicode))
            {
                matches = false;
            }
            else if (idAlpha == IdAlpha)
            {
                matches = true;
                kind1 = Kind1;
                idNum = IdNum;
                name = Name;
            }
            else
            {
                matches = false;
            }

            return matches;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // G r o u p                                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the symbol set group.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLSymbolSets.SymSetGroup Group { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // G r o u p n a m e                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the symbol set group name.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Groupname
        {
            get { return Group.ToString(); }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d                                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // set or return the PCL identifier string.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Id { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d A l p h a                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the alphabetic part of the PCL identifier string.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte IdAlpha { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d N u m                                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the numeric part of the PCL identifier string.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort IdNum { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // K i n d 1                                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set or return the 'Kind1' symbol set number.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort Kind1 { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // K i n d 1 J u s t R                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the 'Kind1' symbol set number, justifed right.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Kind1JustR
        {
            get { return Kind1.ToString().PadLeft(5); }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p A r r a y M a x                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the maximum codepoint defined in the mapping array(s) for  //
        // this symbol set.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort MapArrayMax
        {
            get
            {
                return PCLSymSetMaps.GetCodepointMax((int)_mapId);
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p A r r a y P C L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the PCL (LaserJet) mapping array(s) for this symbol set.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort[] MapArrayPCL
        {
            get
            {
                return PCLSymSetMaps.GetMapArray((int)_mapId, true);
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p A r r a y S t d                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the standard (Strict) mapping array for this symbol set.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort[] MapArrayStd
        {
            get
            {
                return PCLSymSetMaps.GetMapArray((int)_mapId, false);
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p A r r a y U s e r S e t                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Sets or returns the mapping array(s) for the user-defined symbol   //
        // set.                                                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort[] MapArrayUserSet
        {
            get
            {
                return PCLSymSetMaps.GetMapArrayUserSet();
            }

            set
            {
                PCLSymSetMaps.SetMapArrayUserSet(value);
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p p i n g D i f f                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If defined, return a string showing the differences (if any)       //
        // between the standard (Strict) and PCL (LaserJet) mapping tables of //
        // this symbol set.                                                   //
        // Returns null if a mapping table has not been defined.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string MappingDiff
        {
            get
            {
                if (!FlagMapped)
                {
                    return string.Empty;
                }
                else
                {
                    return PCLSymSetMaps.MappingDiff((int)_mapId);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p p i n g P C L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If defined, return a string showing the mapping table of this      //
        // symbol set - the PCL (LaserJet) variant.                           //
        // Returns null if a mapping table has not been defined.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string MappingPCL
        {
            get
            {
                if (!FlagMapped)
                {
                    return string.Empty;
                }
                else
                {
                    return PCLSymSetMaps.Mapping((int)_mapId, true, false);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p p i n g P C L D i f f                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If defined, return a string showing the mapping table of this      //
        // symbol set - the PCL (LaserJet) variant.                           //
        // Only show the mapping if it differs from the Standard (Strict)     //
        // mapping.                                                           //
        // Returns null if a mapping table has not been defined.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string MappingPCLDiff
        {
            get
            {
                if (!FlagMapped)
                {
                    return string.Empty;
                }
                else
                {
                    return PCLSymSetMaps.Mapping((int)_mapId, true, true);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p p i n g S t d                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If defined, return a string showing the mapping table of this      //
        // symbol set - the standard (Strict) variant.                        //
        // Returns null if a mapping table has not been defined.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string MappingStd
        {
            get
            {
                if (!FlagMapped)
                {
                    return string.Empty;
                }
                else
                {
                    return PCLSymSetMaps.Mapping((int)_mapId, false, false);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p R o w s D i f f                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If defined, return an array of strings (one per row, plus any      //
        // inter-range gaps) showing the differences (if any) between the     //
        // standard (Strict) and PCL (LaserJet) mapping tables of this symbol //
        // set.                                                               //
        // Returns null if a mapping table has not been defined.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string[] MapRowsDiff
        {
            get
            {
                if (!FlagMapped)
                {
                    string[] noMap = new string[1] { string.Empty };

                    return noMap;
                }
                else
                {
                    return PCLSymSetMaps.MapRowsDiff((int)_mapId);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p R o w s P C L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If defined, return an array of strings (one per row, plus any      //
        // inter-range gaps) showing the mapping table of this symbol set -   //
        // the PCL (LaserJet) variant.                                        //
        // Returns null if a mapping table has not been defined.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string[] MapRowsPCL
        {
            get
            {
                if (!FlagMapped)
                {
                    string[] noMap = new string[1] { string.Empty };

                    return noMap;
                }
                else
                {
                    return PCLSymSetMaps.MapRows((int)_mapId, true, false);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p R o w s P C L D i f f                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If defined, return an array of strings (one per row, plus any      //
        // inter-range gaps) showing the mapping table of this symbol set -   //
        // the PCL (LaserJet) variant.                                        //
        // Only show the mapping if it differs from the Standard (Strict)     //
        // mapping.                                                           //
        // Returns null if a mapping table has not been defined.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string[] MapRowsPCLDiff
        {
            get
            {
                if (!FlagMapped)
                {
                    string[] noMap = new string[1] { string.Empty };

                    return noMap;
                }
                else
                {
                    return PCLSymSetMaps.MapRows((int)_mapId, true, true);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p R o w s S t d                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If defined, return an array of strings (one per row, plus any      //
        // inter-range gaps) showing the mapping table of this symbol set -   //
        // the standard (Strict) variant.                                     //
        // Returns null if a mapping table has not been defined.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string[] MapRowsStd
        {
            get
            {
                if (!FlagMapped)
                {
                    string[] noMap = new string[1] { string.Empty };

                    return noMap;
                }
                else
                {
                    return PCLSymSetMaps.MapRows((int)_mapId, false, false);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // N a m e                                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the symbol set name.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Name { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // N u l l M a p P C L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns a boolean value indicating whether or not the LaserJet     //
        // (PCL) map associated with this symbol set is null.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool NullMapPCL
        {
            get
            {
                return PCLSymSetMaps.NullMapPCL((int)_mapId);
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // N u l l M a p S t d                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns a boolean value indicating whether or not the Standard     //
        // (Strict) map associated with this symbol set is null.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool NullMapStd
        {
            get
            {
                return PCLSymSetMaps.NullMapStd((int)_mapId);
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P a r s i n g M e t h o d                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the default PCL text parsing method.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLTextParsingMethods.Index ParsingMethod { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T y p e                                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return or set the symbol set type.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLSymSetTypes.Index Type { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T y p e D e s c S h o r t                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the symbol set type short description.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string TypeDescShort
        {
            get
            {
                return PCLSymSetTypes.GetDescShort((int)Type);
            }
        }
    }
}