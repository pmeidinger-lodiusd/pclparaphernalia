﻿using System;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles a PCL Paper Size object.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(Feature = "renaming", ApplyToMembers = true)]

    class PCLPaperSize
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const double _unitsToInches = (1.00 / PCLPaperSizes._paperSizeUPI);
        const double _unitsToMilliMetres = (25.4 / PCLPaperSizes._paperSizeUPI);

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly PCLPaperSizes.Index _paperSizeIndex;
        private string _paperSizeDesc;
        private readonly byte _paperSizeIdPCL;
        private readonly byte _paperSizeIdPCLXL;
        private readonly string _paperSizeNamePCLXL;
        private bool _paperSizeIsRare;

        private ushort _sizeUnitsPerInch;
        private ushort _marginsLogicalPort;
        private ushort _marginsLogicalLand;
        private ushort _marginsUnprintable;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L P a p e r S i z e                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLPaperSize(PCLPaperSizes.Index sizeIndex,
                            string name,
                            string desc,
                            byte idPCL,
                            byte idPCLXL,
                            string namePCLXL,
                            bool isMetricSize,
                            bool isRareSize,
                            ushort sizeUnitsPerInch,
                            uint sizeShortEdge,
                            uint sizeLongEdge,
                            ushort marginsLogicalPort,
                            ushort marginsLogicalLand,
                            ushort marginsUnprintable)
        {
            _paperSizeIndex = sizeIndex;
            Name = name;
            _paperSizeDesc = desc;
            _paperSizeIdPCL = idPCL;
            _paperSizeIdPCLXL = idPCLXL;
            _paperSizeNamePCLXL = namePCLXL;
            IsMetricSize = isMetricSize;
            _paperSizeIsRare = isRareSize;

            _sizeUnitsPerInch = sizeUnitsPerInch;
            CustomShortEdge = sizeShortEdge;
            CustomLongEdge = sizeLongEdge;
            _marginsLogicalPort = marginsLogicalPort;
            _marginsLogicalLand = marginsLogicalLand;
            _marginsUnprintable = marginsUnprintable;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c u s t o m D a t a C o p y                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Copy the 'name' and size data fields to the 'custom' entry.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void CustomDataCopy(PCLPaperSize customEntry)
        {
            customEntry.CustomDataPaste(Name,
                                         IsMetricSize,
                                         _paperSizeIsRare,
                                         _sizeUnitsPerInch,
                                         CustomShortEdge,
                                         CustomLongEdge,
                                         _marginsLogicalPort,
                                         _marginsLogicalLand,
                                         _marginsUnprintable);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c u s t o m D a t a P a s t e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Copy the various size data fields to the 'custom' entry.           //
        // Also copy the 'name' field from the 'donor' entry to the 'desc'    //
        // fiedl of the 'custom' entry.                                       //
        // This should only ever be called when the current Paper Size object //
        // instance is the "Custom" one.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void CustomDataPaste(string donorName,
                                      bool isMetricSize,
                                      bool isRareSize,
                                      ushort sizeUnitsPerInch,
                                      uint sizeShortEdge,
                                      uint sizeLongEdge,
                                      ushort marginsLogicalPort,
                                      ushort marginsLogicalLand,
                                      ushort marginsUnprintable)
        {
            _paperSizeDesc = donorName;

            IsMetricSize = isMetricSize;
            _paperSizeIsRare = isRareSize;

            _sizeUnitsPerInch = sizeUnitsPerInch;
            CustomShortEdge = sizeShortEdge;
            CustomLongEdge = sizeLongEdge;
            _marginsLogicalPort = marginsLogicalPort;
            _marginsLogicalLand = marginsLogicalLand;
            _marginsUnprintable = marginsUnprintable;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // C u s t o m D e s c                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Sets the 'desc' value for the special 'custom' paper size          //
        // instance; this is used to temporarily hold extra data.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string CustomDesc
        {
            set { _paperSizeDesc = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // C u s t o m L o n g E d g e                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Sets or returns the 'long edge' dimension for the special          //
        // 'custom' paper size instance.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public uint CustomLongEdge { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // C u s t o m S h o r t E d g e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Sets or returns the 'short edge' dimension for the special         //
        // 'custom' paper size instance.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public uint CustomShortEdge { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c                                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the paper size description.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Desc => _paperSizeDesc;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // E d g e L o n g                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the long edge dimension for the paper size.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string EdgeLong
        {
            get
            {
                string size;

                if ((_paperSizeIndex == PCLPaperSizes.Index.Custom) ||
                    (_paperSizeIndex == PCLPaperSizes.Index.Default) ||
                    (_paperSizeIndex == PCLPaperSizes.Index.Card_Custom) ||
                    (_paperSizeIndex == PCLPaperSizes.Index.Env_Custom))
                {
                    size = "?";
                }
                else
                {
                    if (IsMetricSize)
                        size = (Math.Round(CustomLongEdge * _unitsToMilliMetres, 3)).ToString("F0") + " mm";
                    else
                        size = (Math.Round(CustomLongEdge * _unitsToInches, 3)).ToString("F3") + "\"";
                }

                return size;
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // E d g e S h o r t                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the short edge dimension for the paper size.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string EdgeShort
        {
            get
            {
                string size;

                if ((_paperSizeIndex == PCLPaperSizes.Index.Custom) ||
                    (_paperSizeIndex == PCLPaperSizes.Index.Default) ||
                    (_paperSizeIndex == PCLPaperSizes.Index.Card_Custom) ||
                    (_paperSizeIndex == PCLPaperSizes.Index.Env_Custom))
                {
                    size = "?";
                }
                else
                {
                    if (IsMetricSize)
                        size = (Math.Round(CustomShortEdge * _unitsToMilliMetres, 3)).ToString("F0") + " mm";
                    else
                        size = (Math.Round(CustomShortEdge * _unitsToInches, 3)).ToString("F3") + "\"";
                }

                return size;
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g C u s t o m S i z e                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the flag indicating whether or not the paper size is the    //
        // special "Custom" value.                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagCustomSize => _paperSizeIndex == PCLPaperSizes.Index.Custom;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e s c                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the paper size description.                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetDesc()
        {
            return _paperSizeDesc;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d P C L                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL identifier value.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte GetIdPCL()
        {
            return _paperSizeIdPCL;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d P C L X L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL XL identifier value.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte GetIdPCLXL()
        {
            return _paperSizeIdPCLXL;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t L o g i c a l O f f s e t                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the logical page offset of the paper for a given aspect.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort GetLogicalOffset(ushort sessionUPI, PCLOrientations.Aspect aspect)
        {
            if (aspect == PCLOrientations.Aspect.Portrait)
                return (ushort)((_marginsLogicalPort * sessionUPI) / _sizeUnitsPerInch);
            else
                return (ushort)((_marginsLogicalLand * sessionUPI) / _sizeUnitsPerInch);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t L o g P a g e L e n g t h                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the length of the PCL logical page for a given aspect.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort GetLogPageLength(ushort sessionUPI, PCLOrientations.Aspect aspect)
        {
            if (aspect == PCLOrientations.Aspect.Portrait)
                return (ushort)((CustomLongEdge * sessionUPI) / _sizeUnitsPerInch);
            else
                return (ushort)((CustomShortEdge * sessionUPI) / _sizeUnitsPerInch);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t L o g P a g e W i d t h                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the width of the PCL logical page for a given aspect.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort GetLogPageWidth(ushort sessionUPI, PCLOrientations.Aspect aspect)
        {
            if (aspect == PCLOrientations.Aspect.Portrait)
            {
                return (ushort)(((CustomShortEdge - (_marginsLogicalPort * 2)) * sessionUPI) / _sizeUnitsPerInch);
            }
            else
            {
                return (ushort)(((CustomLongEdge - (_marginsLogicalPort * 2)) * sessionUPI) / _sizeUnitsPerInch);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t M a r g i n s L o g i c a l L a n d                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the size of the PCL Landscape logical margins.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort GetMarginsLogicalLand(ushort sessionUPI)
        {
            return (ushort)((_marginsLogicalLand * sessionUPI) / _sizeUnitsPerInch);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t M a r g i n s L o g i c a l P o r t                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the size of the PCL Portrait logical margins.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort GetMarginsLogicalPort(ushort sessionUPI)
        {
            return (ushort)((_marginsLogicalPort * sessionUPI) / _sizeUnitsPerInch);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t M a r g i n s U n p r i n t a b l e                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the size of the unprintable margins; these are the same for //
        // both standard orientations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort GetMarginsUnprintable(ushort sessionUPI)
        {
            return (ushort)((_marginsUnprintable * sessionUPI) / _sizeUnitsPerInch);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the paper size name.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetName()
        {
            return Name;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e P C L X L                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL XL string name.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetNamePCLXL()
        {
            return _paperSizeNamePCLXL;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P a p e r L e n g t h                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the length of the paper for a given aspect.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort GetPaperLength(ushort sessionUPI, PCLOrientations.Aspect aspect)
        {
            if (aspect == PCLOrientations.Aspect.Portrait)
                return (ushort)((CustomLongEdge * sessionUPI) / _sizeUnitsPerInch);
            else
                return (ushort)((CustomShortEdge * sessionUPI) / _sizeUnitsPerInch);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P a p e r W i d t h                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the width of the paper for a given aspect.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort GetPaperWidth(ushort sessionUPI, PCLOrientations.Aspect aspect)
        {
            if (aspect == PCLOrientations.Aspect.Portrait)
                return (ushort)((CustomShortEdge * sessionUPI) / _sizeUnitsPerInch);
            else
                return (ushort)((CustomLongEdge * sessionUPI) / _sizeUnitsPerInch);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t S i z e L o n g E d g e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the size of the long edge of the paper.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort GetSizeLongEdge(ushort sessionUPI)
        {
            return (ushort)((CustomLongEdge * sessionUPI) / _sizeUnitsPerInch);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t S i z e S h o r t E d g e                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the size of the short edge of the paper.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort GetSizeShortEdge(ushort sessionUPI)
        {
            return (ushort)((CustomShortEdge * sessionUPI) / _sizeUnitsPerInch);
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d P C L                                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL identifier (if known) for the paper size.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string IdPCL
        {
            get
            {
                if (_paperSizeIdPCL == 0xff)
                    return "?";
                else
                    return _paperSizeIdPCL.ToString();
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d P C L X L                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL XL enumeration (if known) for the paper size.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string IdPCLXL
        {
            get
            {
                if (_paperSizeIdPCLXL == 0xff)
                    return "?";
                else
                    return _paperSizeIdPCLXL.ToString();
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d N a m e P C L X L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL XL enumeration and string name (if known) for the   //
        // paper size.                                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string IdNamePCLXL
        {
            get
            {
                string id,
                       name;

                if (_paperSizeIdPCLXL == 0xff)
                    id = "?";
                else
                    id = _paperSizeIdPCLXL.ToString();

                if (_paperSizeNamePCLXL?.Length == 0)
                    name = "?";
                else
                    name = _paperSizeNamePCLXL;

                return id + " / " + name;
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I s M e t r i c S i z e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Sets or returns the 'is metric size' attribute.                    //
        // The 'set' option would only apply to the special 'Custom' paper    //
        // size.                                                              // 
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsMetricSize { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I s R a r e S i z e                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the 'is rare (or obsolete) size' attribute.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsRareSize => _paperSizeIsRare;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // N a m e                                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the paper size reference name.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Name { get; }
    }
}