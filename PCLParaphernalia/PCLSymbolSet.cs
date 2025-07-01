using System;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles a PCL Symbol Set object.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(
        Feature = "renaming",
        ApplyToMembers = true)]

    class PCLSymbolSet
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

        private readonly PCLSymbolSets.eSymSetGroup _group;
        private PCLSymSetTypes.eIndex _indxType;
        private readonly PCLSymSetMaps.eSymSetMapId _mapId;

        private PCLTextParsingMethods.eIndex _parsingMethod;

        private readonly string _name;
        private readonly string _alias;

        private ushort _kind1;

        private string _id;
        private readonly ushort _idNum;
        private readonly byte _idAlpha;

        private readonly bool _mapped = false;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L S y m b o l S e t                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLSymbolSet(PCLSymbolSets.eSymSetGroup group,
                             PCLSymSetTypes.eIndex indxType,
                             PCLTextParsingMethods.eIndex parsingMethod,
                             ushort kind1,
                             string alias,
                             string name,
                             bool mapped,
                             PCLSymSetMaps.eSymSetMapId mapId)
        {
            _group = group;
            _indxType = indxType;
            _parsingMethod = parsingMethod;
            _kind1 = kind1;
            _alias = alias;
            _name = name;

            if ((kind1 < 1)         // 1        = 0A    //
                    ||
                (kind1 > 65530))    // 65530    = 2047Z //
            {
                _idNum = 0;
                _idAlpha = 0x3f;    // ? //
            }
            else
            {
                _idNum = (ushort)(kind1 / 32);
                _idAlpha = (byte)((kind1 - (_idNum * 32)) + 64);
            }

            _id = _idNum.ToString() + Convert.ToChar(_idAlpha);

            _mapped = mapped;
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

        public string Alias
        {
            get { return _alias; }
        }

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
                if (!_mapped)
                {
                    return false;
                }
                else if (PCLSymSetMaps.nullMapPCL((int)_mapId))
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

        public bool FlagMapped
        {
            get { return _mapped; }
        }

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
                if (!_mapped)
                {
                    return false;
                }
                else if (PCLSymSetMaps.nullMapStd((int)_mapId))
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

        public bool getSymsetData(ref ushort kind1,
                                     ref ushort idNum,
                                     ref string name)
        {
            bool matches = false;

            if ((_group == PCLSymbolSets.eSymSetGroup.Preset) ||
                (_group == PCLSymbolSets.eSymSetGroup.Unicode))
            {
                matches = true;
                kind1 = _kind1;
                idNum = _idNum;
                name = _name;
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

        public bool getSymsetDataForIdAlpha(byte idAlpha,
                                               ref ushort kind1,
                                               ref ushort idNum,
                                               ref string name)
        {
            bool matches = false;

            if ((_group != PCLSymbolSets.eSymSetGroup.Preset) &&
                (_group != PCLSymbolSets.eSymSetGroup.Unbound) &&
                (_group != PCLSymbolSets.eSymSetGroup.Unicode))
            {
                matches = false;
            }
            else if (idAlpha == _idAlpha)
            {
                matches = true;
                kind1 = _kind1;
                idNum = _idNum;
                name = _name;
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

        public PCLSymbolSets.eSymSetGroup Group
        {
            get { return _group; }
        }

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
            get { return _group.ToString(); }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d                                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // set or return the PCL identifier string.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d A l p h a                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the alphabetic part of the PCL identifier string.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte IdAlpha
        {
            get { return _idAlpha; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d N u m                                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the numeric part of the PCL identifier string.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort IdNum
        {
            get { return _idNum; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // K i n d 1                                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set or return the 'Kind1' symbol set number.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort Kind1
        {
            get { return _kind1; }
            set { _kind1 = value; }
        }

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
            get { return _kind1.ToString().PadLeft(5); }
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
                return PCLSymSetMaps.getCodepointMax((int)_mapId);
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
                return PCLSymSetMaps.getMapArray((int)_mapId, true);
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
                return PCLSymSetMaps.getMapArray((int)_mapId, false);
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
                return PCLSymSetMaps.getMapArrayUserSet();
            }

            set
            {
                PCLSymSetMaps.setMapArrayUserSet(value);
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
                if (!_mapped)
                {
                    return "";
                }
                else
                {
                    return PCLSymSetMaps.mappingDiff((int)_mapId);
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
                if (!_mapped)
                {
                    return "";
                }
                else
                {
                    return PCLSymSetMaps.mapping((int)_mapId, true, false);
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
                if (!_mapped)
                {
                    return "";
                }
                else
                {
                    return PCLSymSetMaps.mapping((int)_mapId, true, true);
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
                if (!_mapped)
                {
                    return "";
                }
                else
                {
                    return PCLSymSetMaps.mapping((int)_mapId, false, false);
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
                if (!_mapped)
                {
                    string[] noMap = new string[1] { "" };

                    return noMap;
                }
                else
                {
                    return PCLSymSetMaps.mapRowsDiff((int)_mapId);
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
                if (!_mapped)
                {
                    string[] noMap = new string[1] { "" };

                    return noMap;
                }
                else
                {
                    return PCLSymSetMaps.mapRows((int)_mapId, true, false);
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
                if (!_mapped)
                {
                    string[] noMap = new string[1] { "" };

                    return noMap;
                }
                else
                {
                    return PCLSymSetMaps.mapRows((int)_mapId, true, true);
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
                if (!_mapped)
                {
                    string[] noMap = new string[1] { "" };

                    return noMap;
                }
                else
                {
                    return PCLSymSetMaps.mapRows((int)_mapId, false, false);
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

        public string Name
        {
            get { return _name; }
        }

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
                return PCLSymSetMaps.nullMapPCL((int)_mapId);
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
                return PCLSymSetMaps.nullMapStd((int)_mapId);
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

        public PCLTextParsingMethods.eIndex ParsingMethod
        {
            get { return _parsingMethod; }

            set { _parsingMethod = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T y p e                                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return or set the symbol set type.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLSymSetTypes.eIndex Type
        {
            get { return _indxType; }

            set { _indxType = value; }
        }

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
                return PCLSymSetTypes.getDescShort((int)_indxType);
            }
        }
    }
}