﻿using System.Text;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles a PCL Symbol Set map object.
    /// 
    /// © Chris Hutchinson 2015
    /// 
    /// </summary>

    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(
        Feature = "renaming",
        ApplyToMembers = true)]

    class PCLSymSetMap
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

        private readonly ushort[][] _rangeData;

        private readonly ushort[][] _mapDataStd;
        private readonly ushort[][] _mapDataPCL;

        private readonly ushort _rangeCt;

        private readonly ushort _codepointMin;
        private ushort _codepointMax;

        private readonly bool _flagMapDiff;
        private readonly bool _flagNullMapPCL;
        private readonly bool _flagNullMapStd;

        private readonly PCLSymSetMaps.eSymSetMapId _mapId;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L S y m S e t M a p                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLSymSetMap(PCLSymSetMaps.eSymSetMapId mapId,
                             ushort rangeCt,
                             ushort[][] rangeData,
                             ushort[][] mapDataStd,
                             ushort[][] mapDataPCL)
        {
            _mapId = mapId;
            _rangeCt = rangeCt;
            _rangeData = rangeData;
            _mapDataStd = mapDataStd;
            _mapDataPCL = mapDataPCL;

            _codepointMin = _rangeData[0][0];
            _codepointMax = _rangeData[_rangeCt - 1][1];

            _flagNullMapStd = false;
            _flagNullMapPCL = false;
            _flagMapDiff = false;

            if (mapDataStd == null)
            {
                _flagNullMapStd = true;

                if (mapDataPCL == null)
                {
                    // Should not happen                                      //
                    // If this message shown, fix the reason before release!  //

                    MessageBox.Show(
                        "Standard and PCL maps both null!" +
                        " for map " + mapId.ToString(),
                        "Internal error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else if (mapDataPCL == null)
            {
                _flagNullMapPCL = true;
            }
            else
            {
                int lenMainPCL = _mapDataPCL.Length;
                int lenMainStd = _mapDataStd.Length;
                int lenSubPCL;
                int lenSubStd;

                if (lenMainPCL != lenMainStd)
                {
                    _flagMapDiff = true;
                }
                else
                {
                    for (int i = 0; i < lenMainStd; i++)
                    {
                        lenSubPCL = _mapDataPCL[i].Length;
                        lenSubStd = _mapDataStd[i].Length;

                        if (lenSubPCL != lenSubStd)
                        {
                            _flagMapDiff = true;
                        }
                        else
                        {
                            for (int j = 0; j < lenSubStd; j++)
                            {
                                if (_mapDataPCL[i][j] != _mapDataStd[i][j])
                                {
                                    _flagMapDiff = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // C o d e p o i n t M a x                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the maximum code point.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort CodepointMax
        {
            get { return _codepointMax; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // C o d e p o i n t M i n                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the minimum code point.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort CodepointMin
        {
            get { return _codepointMin; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p D a t a P C L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the PCL (LaserJet) mapping array for this symbol set.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort[] MapDataPCL
        {
            get
            {
                ushort rangeMin,
                       rangeMax,
                       rangeSize;

                ushort[] mapData = new ushort[_codepointMax + 1];

                for (int i = 0; i < _codepointMax; i++)
                {
                    mapData[i] = 0xffff;
                }

                for (int i = 0; i < _rangeCt; i++)
                {
                    rangeMin = _rangeData[i][0];
                    rangeMax = _rangeData[i][1];
                    rangeSize = (ushort)(rangeMax - rangeMin + 1);

                    if (!_flagNullMapPCL)
                    {
                        for (int j = 0; j < rangeSize; j++)
                        {
                            mapData[rangeMin + j] = _mapDataPCL[i][j];
                        }
                    }
                    else
                    {
                        for (int j = 0; j < rangeSize; j++)
                        {
                            mapData[rangeMin + j] = _mapDataStd[i][j];
                        }
                    }
                }

                return mapData;
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p D a t a S t d                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the standard (Strict) mapping array for this symbol set.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort[] MapDataStd
        {
            get
            {
                ushort rangeMin,
                       rangeMax,
                       rangeSize;

                ushort[] mapData = new ushort[_codepointMax + 1];

                for (int i = 0; i < _codepointMax; i++)
                {
                    mapData[i] = 0xffff;
                }

                for (int i = 0; i < _rangeCt; i++)
                {
                    rangeMin = _rangeData[i][0];
                    rangeMax = _rangeData[i][1];
                    rangeSize = (ushort)(rangeMax - rangeMin + 1);

                    if (!_flagNullMapStd)
                    {
                        for (int j = 0; j < rangeSize; j++)
                        {
                            mapData[rangeMin + j] = _mapDataStd[i][j];
                        }
                    }
                    else
                    {
                        for (int j = 0; j < rangeSize; j++)
                        {
                            mapData[rangeMin + j] = _mapDataPCL[i][j];
                        }
                    }
                }

                return mapData;
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p D a t a U s e r S e t                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Sets or returns the mapping array for the user-defined symbol set. //
        // This 'special' symbol set instance has a single 'range' (allows    //
        // all 16-bit values, but what is returned depends on the maximum     //
        // code-point currently set for this special set.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort[] MapDataUserSet
        {
            get
            {
                ushort rangeMin;

                ushort[] mapData = new ushort[_codepointMax + 1];

                for (int i = 0; i < _codepointMax; i++)
                {
                    mapData[i] = 0xffff;
                }

                rangeMin = _rangeData[0][0];
                //    rangeMax = _rangeData [0] [1];
                //    rangeSize = (UInt16) (rangeMax - rangeMin + 1);

                for (int j = 0; j <= _codepointMax; j++)
                {
                    mapData[rangeMin + j] = _mapDataStd[0][j];
                }

                return mapData;
            }

            //----------------------------------------------------------------//

            set
            {
                _codepointMax = (ushort)(value.Length - 1);

                for (int j = 0; j <= _codepointMax; j++)
                {
                    _mapDataStd[0][j] = value[j];
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p p i n g D i f f                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If mapping tables have been defined, return a string showing the   //
        // differences (if any) between the standard and PCL variant mapping  //
        // tables of this symbol set.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string MappingDiff
        {
            get
            {
                if (!_flagMapDiff)
                {
                    if ((_flagNullMapStd) || (_flagNullMapPCL))
                        return "Not applicable (only one set defined)";
                    else
                        return "None";
                }
                else
                {
                    const int rowLen = 16;
                    const int rowSize = 9 + (rowLen * 5) + 2;

                    bool difference = false;

                    ushort mapValStd,
                           mapValPCL;

                    ushort rangeMin,
                           rangeMax;

                    int rowCt,
                          rowTot,
                          rowId,
                          mapIndx,
                          cell,
                          rangeLastIndx,
                          rowLastIndx;

                    StringBuilder map;

                    //--------------------------------------------------------//
                    //                                                        //
                    // Calculate number of map rows, including those for      //
                    // inter-range gaps.                                      //
                    //                                                        //
                    //--------------------------------------------------------//

                    rowTot = 0;
                    rangeLastIndx = _rangeCt - 1;

                    for (int i = 0; i < _rangeCt; i++)
                    {
                        rowTot += (_rangeData[i][1] >> 4) -
                                  (_rangeData[i][0] >> 4) + 1;

                        if (i < rangeLastIndx)
                            rowTot += 1;    // for inter-range gap //
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Populate row definitions.                              //
                    //                                                        //
                    //--------------------------------------------------------//

                    map = new StringBuilder(rowSize * rowTot);

                    map.Clear();

                    for (int i = 0; i < _rangeCt; i++)
                    {
                        rangeMin = _rangeData[i][0];
                        rangeMax = _rangeData[i][1];

                        rowId = rangeMin >> 4;
                        rowCt = (rangeMax >> 4) - rowId + 1;
                        rowLastIndx = rowCt - 1;

                        mapIndx = 0;

                        for (int j = 0; j < rowCt; j++)
                        {
                            if (rowId < 0x10)
                                map.Append("0x" + rowId.ToString("X1") +
                                            "0->   ");
                            else
                                map.Append("0x" + rowId.ToString("X3") +
                                            "0-> ");

                            cell = (rowId * rowLen);

                            for (int k = 0; k < rowLen; k++)
                            {
                                if (cell < rangeMin)
                                {
                                    map.Append("---- ");
                                }
                                else if (cell > rangeMax)
                                {
                                    map.Append("---- ");
                                }
                                else
                                {
                                    mapValPCL = _mapDataPCL[i][mapIndx];
                                    mapValStd = _mapDataStd[i][mapIndx];

                                    if (mapValPCL == mapValStd)
                                        map.Append("---- ");
                                    else
                                    {
                                        difference = true;
                                        map.Append(mapValPCL.ToString("X4") +
                                                    " ");
                                    }

                                    mapIndx++;
                                }

                                cell++;
                            }

                            if (i < rangeLastIndx)
                                map.Append("\r\n");
                            else if (j < rowLastIndx)
                                map.Append("\r\n");

                            rowId++;
                        }

                        if (i < rangeLastIndx)
                        {
                            map.Append("\r\n"); // inter-range gap //
                        }
                    }

                    if (difference)
                        return map.ToString();
                    else
                        return "none";
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p p i n g P C L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If mapping tables have been defined, return a string showing the   //
        // PCL (LaserJet) variant mapping table of this symbol set.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string MappingPCL
        {
            get
            {
                if (_flagNullMapPCL)
                {
                    return "Not defined" +
                           " - see Standard (Strict) mapping definition";
                }
                else
                {
                    const int rowLen = 16;
                    const int rowSize = 9 + (rowLen * 5) + 2;

                    ushort mapVal;

                    ushort rangeMin,
                           rangeMax;

                    int rowCt,
                          rowTot,
                          rowId,
                          mapIndx,
                          cell,
                          rangeLastIndx,
                          rowLastIndx;

                    StringBuilder map;

                    //--------------------------------------------------------//
                    //                                                        //
                    // Calculate number of map rows, including those for      //
                    // inter-range gaps.                                      //
                    //                                                        //
                    //--------------------------------------------------------//

                    rowTot = 0;
                    rangeLastIndx = _rangeCt - 1;

                    for (int i = 0; i < _rangeCt; i++)
                    {
                        rowTot += (_rangeData[i][1] >> 4) -
                                  (_rangeData[i][0] >> 4) + 1;

                        if (i < rangeLastIndx)
                            rowTot += 1;    // for inter-range gap //
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Populate row definitions.                              //
                    //                                                        //
                    //--------------------------------------------------------//

                    map = new StringBuilder(rowSize * rowTot);

                    map.Clear();

                    for (int i = 0; i < _rangeCt; i++)
                    {
                        rangeMin = _rangeData[i][0];
                        rangeMax = _rangeData[i][1];

                        rowId = rangeMin >> 4;
                        rowCt = (rangeMax >> 4) - rowId + 1;
                        rowLastIndx = rowCt - 1;

                        mapIndx = 0;

                        for (int j = 0; j < rowCt; j++)
                        {
                            if (rowId < 0x10)
                                map.Append("0x" + rowId.ToString("X1") + "0->   ");
                            else
                                map.Append("0x" + rowId.ToString("X3") + "0-> ");

                            cell = (rowId * rowLen);

                            for (int k = 0; k < rowLen; k++)
                            {
                                if (cell < rangeMin)
                                {
                                    map.Append("---- ");
                                }
                                else if (cell > rangeMax)
                                {
                                    map.Append("---- ");
                                }
                                else
                                {
                                    mapVal = _mapDataPCL[i][mapIndx];

                                    if (mapVal == 0xffff)
                                        map.Append("---- ");
                                    else
                                        map.Append(mapVal.ToString("X4") + " ");

                                    mapIndx++;
                                }

                                cell++;
                            }

                            if (i < rangeLastIndx)
                                map.Append("\r\n");
                            else if (j < rowLastIndx)
                                map.Append("\r\n");

                            rowId++;
                        }

                        if (i < rangeLastIndx)
                        {
                            map.Append("\r\n"); // inter-range gap //
                        }
                    }

                    return map.ToString();
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p p i n g P C L D i f f                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If mapping tables have been defined, return a string showing the   //
        // PCL (LaserJet) variant mapping table of this symbol set.           //
        // Only show the mapping if it differs from the Standard (Strict)     //
        // mapping.                                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string MappingPCLDiff
        {
            get
            {
                if (_flagMapDiff)
                    return MappingPCL;
                else if (_flagNullMapStd)
                    return MappingPCL;
                else
                    return "Not defined" +
                           " - see Standard (Strict) mapping definition";
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p p i n g S t d                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If mapping tables have been defined, return a string showing the   //
        // standard (Strict) variant mapping table of this symbol set.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string MappingStd
        {
            get
            {
                if (_flagNullMapStd)
                {
                    return "Not defined" +
                           " - see LaserJet mapping definition";
                }
                else
                {
                    const int rowLen = 16;
                    const int rowSize = 9 + (rowLen * 5) + 2;

                    ushort mapVal;

                    ushort rangeMin,
                           rangeMax;

                    int rowCt,
                          rowTot,
                          rowId,
                          mapIndx,
                          cell,
                          rangeLastIndx,
                          rowLastIndx;

                    StringBuilder map;

                    //--------------------------------------------------------//
                    //                                                        //
                    // Calculate number of map rows, including those for      //
                    // inter-range gaps.                                      //
                    //                                                        //
                    //--------------------------------------------------------//

                    rowTot = 0;
                    rangeLastIndx = _rangeCt - 1;

                    for (int i = 0; i < _rangeCt; i++)
                    {
                        rowTot += (_rangeData[i][1] >> 4) -
                                  (_rangeData[i][0] >> 4) + 1;

                        if (i < rangeLastIndx)
                            rowTot += 1;    // for inter-range gap //
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Populate row definitions.                              //
                    //                                                        //
                    //--------------------------------------------------------//

                    map = new StringBuilder(rowSize * rowTot);

                    map.Clear();

                    for (int i = 0; i < _rangeCt; i++)
                    {
                        rangeMin = _rangeData[i][0];
                        rangeMax = _rangeData[i][1];

                        rowId = rangeMin >> 4;
                        rowCt = (rangeMax >> 4) - rowId + 1;
                        rowLastIndx = rowCt - 1;

                        mapIndx = 0;

                        for (int j = 0; j < rowCt; j++)
                        {
                            if (rowId < 0x10)
                                map.Append("0x" + rowId.ToString("X1") +
                                           "0->   ");
                            else
                                map.Append("0x" + rowId.ToString("X3") +
                                           "0-> ");

                            cell = (rowId * rowLen);

                            for (int k = 0; k < rowLen; k++)
                            {
                                if (cell < rangeMin)
                                {
                                    map.Append("---- ");
                                }
                                else if (cell > rangeMax)
                                {
                                    map.Append("---- ");
                                }
                                else
                                {
                                    mapVal = _mapDataStd[i][mapIndx];

                                    if (mapVal == 0xffff)
                                        map.Append("---- ");
                                    else
                                        map.Append(mapVal.ToString("X4") + " ");

                                    mapIndx++;
                                }

                                cell++;
                            }

                            if (i < rangeLastIndx)
                                map.Append("\r\n");
                            else if (j < rowLastIndx)
                                map.Append("\r\n");

                            rowId++;
                        }

                        if (i < rangeLastIndx)
                        {
                            map.Append("\r\n"); // inter-range gap //
                        }
                    }

                    return map.ToString();
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p R o w s D i f f                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If mapping tables have been defined, return an array of strings    //
        // (one per row, plus any inter-range gaps) showing differences (if   //
        // any) between the standard and PCL variant mapping tables of this   //
        // symbol set.                                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string[] MapRowsDiff
        {
            get
            {
                string[] mapRows;

                bool difference = false;

                if (!_flagMapDiff)
                {
                    mapRows = new string[1];

                    if ((_flagNullMapStd) || (_flagNullMapPCL))
                        mapRows[0] = "Not applicable (only one set defined)";
                    else
                        mapRows[0] = "None";
                }
                else
                {
                    const int rowLen = 16;
                    const int rowSize = 9 + (rowLen * 5) + 2;

                    ushort mapValStd,
                           mapValPCL;

                    ushort rangeMin,
                           rangeMax;

                    int rowCt,
                          rowTot,
                          rowIndx,
                          rowId,
                          mapIndx,
                          cell,
                          rangeLastIndx;

                    StringBuilder crntRow;

                    //--------------------------------------------------------//
                    //                                                        //
                    // Calculate number of map rows, including those for      //
                    // inter-range gaps.                                      //
                    //                                                        //
                    //--------------------------------------------------------//

                    rowTot = 0;
                    rangeLastIndx = _rangeCt - 1;

                    for (int i = 0; i < _rangeCt; i++)
                    {
                        rowTot += (_rangeData[i][1] >> 4) -
                                  (_rangeData[i][0] >> 4) + 1;

                        if (i < rangeLastIndx)
                            rowTot += 1;    // for inter-range gap //
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Populate row definitions.                              //
                    //                                                        //
                    //--------------------------------------------------------//

                    mapRows = new string[rowTot];

                    crntRow = new StringBuilder(rowSize);

                    crntRow.Clear();

                    rowIndx = 0;

                    for (int i = 0; i < _rangeCt; i++)
                    {
                        rangeMin = _rangeData[i][0];
                        rangeMax = _rangeData[i][1];

                        rowId = rangeMin >> 4;
                        rowCt = (rangeMax >> 4) - rowId + 1;

                        mapIndx = 0;

                        for (int j = 0; j < rowCt; j++)
                        {
                            if (rowId < 0x10)
                                crntRow.Append("0x" + rowId.ToString("X1") +
                                                "0->   ");
                            else
                                crntRow.Append("0x" + rowId.ToString("X3") +
                                                "0-> ");

                            cell = (rowId * rowLen);

                            for (int k = 0; k < rowLen; k++)
                            {
                                if (cell < rangeMin)
                                {
                                    crntRow.Append("---- ");
                                }
                                else if (cell > rangeMax)
                                {
                                    crntRow.Append("---- ");
                                }
                                else
                                {
                                    mapValPCL = _mapDataPCL[i][mapIndx];
                                    mapValStd = _mapDataStd[i][mapIndx];

                                    if (mapValPCL == mapValStd)
                                        crntRow.Append("---- ");
                                    else
                                    {
                                        difference = true;
                                        crntRow.Append(mapValPCL.ToString("X4") +
                                                         " ");
                                    }

                                    mapIndx++;
                                }

                                cell++;
                            }

                            mapRows[rowIndx] = crntRow.ToString();

                            crntRow.Clear();

                            rowId++;
                            rowIndx++;
                        }

                        if (i < rangeLastIndx)
                        {
                            mapRows[rowIndx] = string.Empty; // inter-range gap //
                            rowIndx++;
                        }
                    }
                }

                if (difference)
                    return mapRows;
                else
                {
                    string[] mapRowsSame = new string[1] { "none" };
                    return mapRowsSame;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p R o w s P C L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If mapping tables have been defined, return an array of strings    //
        // (one per row, plus any inter-range gaps) showing the PCL           //
        // (LaserJet) variant mapping table of this symbol set.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string[] MapRowsPCL
        {
            get
            {
                string[] mapRows;

                if (_flagNullMapPCL)
                {
                    mapRows = new string[1];
                    mapRows[0] = "Not defined - see Standard (Strict)" +
                                  " mapping definition";
                }
                else
                {
                    const int rowLen = 16;
                    const int rowSize = 9 + (rowLen * 5) + 2;

                    ushort mapVal;

                    ushort rangeMin,
                           rangeMax;

                    int rowCt,
                          rowTot,
                          rowIndx,
                          rowId,
                          mapIndx,
                          cell,
                          rangeLastIndx;

                    StringBuilder crntRow;

                    //--------------------------------------------------------//
                    //                                                        //
                    // Calculate number of map rows, including those for      //
                    // inter-range gaps.                                      //
                    //                                                        //
                    //--------------------------------------------------------//

                    rowTot = 0;
                    rangeLastIndx = _rangeCt - 1;

                    for (int i = 0; i < _rangeCt; i++)
                    {
                        rowTot += (_rangeData[i][1] >> 4) -
                                  (_rangeData[i][0] >> 4) + 1;

                        if (i < rangeLastIndx)
                            rowTot += 1;    // for inter-range gap //
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Populate row definitions.                              //
                    //                                                        //
                    //--------------------------------------------------------//

                    mapRows = new string[rowTot];

                    crntRow = new StringBuilder(rowSize);

                    crntRow.Clear();

                    rowIndx = 0;

                    for (int i = 0; i < _rangeCt; i++)
                    {
                        rangeMin = _rangeData[i][0];
                        rangeMax = _rangeData[i][1];

                        rowId = rangeMin >> 4;
                        rowCt = (rangeMax >> 4) - rowId + 1;

                        mapIndx = 0;

                        for (int j = 0; j < rowCt; j++)
                        {
                            if (rowId < 0x10)
                                crntRow.Append("0x" + rowId.ToString("X1") +
                                                "0->   ");
                            else
                                crntRow.Append("0x" + rowId.ToString("X3") +
                                                "0-> ");

                            cell = (rowId * rowLen);

                            for (int k = 0; k < rowLen; k++)
                            {
                                if (cell < rangeMin)
                                {
                                    crntRow.Append("---- ");
                                }
                                else if (cell > rangeMax)
                                {
                                    crntRow.Append("---- ");
                                }
                                else
                                {
                                    mapVal = _mapDataPCL[i][mapIndx];

                                    if (mapVal == 0xffff)
                                        crntRow.Append("---- ");
                                    else
                                        crntRow.Append(mapVal.ToString("X4") + " ");

                                    mapIndx++;
                                }

                                cell++;
                            }

                            mapRows[rowIndx] = crntRow.ToString();

                            crntRow.Clear();

                            rowId++;
                            rowIndx++;
                        }

                        if (i < rangeLastIndx)
                        {
                            mapRows[rowIndx] = string.Empty; // inter-range gap //
                            rowIndx++;
                        }
                    }
                }

                return mapRows;
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p R o w s P C L D i f f                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If mapping tables have been defined, return return an array of     //
        // strings (one per row, plus any inter-range gaps) showing the PCL   //
        // (LaserJet) variant mapping table of this symbol set.               //
        // Only show the mapping if it differs from the Standard (Strict)     //
        // mapping.                                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string[] MapRowsPCLDiff
        {
            get
            {
                if (_flagMapDiff)
                    return MapRowsPCL;
                else if (_flagNullMapStd)
                    return MapRowsPCL;
                else
                {
                    string[] noDiff = new string[1] {
                        "Not defined" +
                        " - see Standard (Strict) mapping definition"};

                    return noDiff;
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a p R o w s S t d                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // If mapping tables have been defined, return an array of strings    //
        // (one per row, plus any inter-range gaps) showing the standard      //
        // (Strict) variant mapping table of this symbol set.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string[] MapRowsStd
        {
            get
            {
                string[] mapRows;

                if (_flagNullMapStd)
                {
                    mapRows = new string[1];
                    mapRows[0] = "Not defined - see LaserJet " +
                                  " mapping definition";
                }
                else
                {
                    const int rowLen = 16;
                    const int rowSize = 9 + (rowLen * 5) + 2;

                    ushort mapVal;

                    ushort rangeMin,
                           rangeMax;

                    int rowCt,
                          rowTot,
                          rowIndx,
                          rowId,
                          mapIndx,
                          cell,
                          rangeLastIndx;

                    StringBuilder crntRow;

                    //--------------------------------------------------------//
                    //                                                        //
                    // Calculate number of map rows, including those for      //
                    // inter-range gaps.                                      //
                    //                                                        //
                    //--------------------------------------------------------//

                    rowTot = 0;
                    rangeLastIndx = _rangeCt - 1;

                    for (int i = 0; i < _rangeCt; i++)
                    {
                        rowTot += (_rangeData[i][1] >> 4) -
                                  (_rangeData[i][0] >> 4) + 1;

                        if (i < rangeLastIndx)
                            rowTot += 1;    // for inter-range gap //
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Populate row definitions.                              //
                    //                                                        //
                    //--------------------------------------------------------//

                    mapRows = new string[rowTot];

                    crntRow = new StringBuilder(rowSize);

                    crntRow.Clear();

                    rowIndx = 0;

                    for (int i = 0; i < _rangeCt; i++)
                    {
                        rangeMin = _rangeData[i][0];
                        rangeMax = _rangeData[i][1];

                        rowId = rangeMin >> 4;
                        rowCt = (rangeMax >> 4) - rowId + 1;

                        mapIndx = 0;

                        for (int j = 0; j < rowCt; j++)
                        {
                            if (rowId < 0x10)
                                crntRow.Append("0x" + rowId.ToString("X1") +
                                                "0->   ");
                            else
                                crntRow.Append("0x" + rowId.ToString("X3") +
                                                "0-> ");

                            cell = (rowId * rowLen);

                            for (int k = 0; k < rowLen; k++)
                            {
                                if (cell < rangeMin)
                                {
                                    crntRow.Append("---- ");
                                }
                                else if (cell > rangeMax)
                                {
                                    crntRow.Append("---- ");
                                }
                                else
                                {
                                    mapVal = _mapDataStd[i][mapIndx];

                                    if (mapVal == 0xffff)
                                        crntRow.Append("---- ");
                                    else
                                        crntRow.Append(mapVal.ToString("X4") + " ");

                                    mapIndx++;
                                }

                                cell++;
                            }

                            mapRows[rowIndx] = crntRow.ToString();

                            crntRow.Clear();

                            rowId++;
                            rowIndx++;
                        }

                        if (i < rangeLastIndx)
                        {
                            mapRows[rowIndx] = string.Empty; // inter-range gap //
                            rowIndx++;
                        }
                    }
                }

                return mapRows;
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // N u l l M a p P C L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns a boolean value indicating whether or not the PCL map is   //
        // null.                                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool NullMapPCL
        {
            get { return _flagNullMapPCL; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // N u l l M a p S t d                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns a boolean value indicating whether or not the standard     //
        // (strict) map is null.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool NullMapStd
        {
            get { return _flagNullMapStd; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // R a n g e C t                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the count of mapping ranges defined in this symbol set.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort RangeCt
        {
            get { return _rangeCt; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // R a n g e D a t a                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the mapping ranges defined in this symbol set.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort[][] RangeData
        {
            get { return _rangeData; }
        }
    }
}