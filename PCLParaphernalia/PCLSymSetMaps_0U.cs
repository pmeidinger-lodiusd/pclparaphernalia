﻿using System;
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
        //                                                        M e t h o d //
        // u n i c o d e M a p _ 0 U                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Maps characters in symbol set to Unicode (UCS-2) code-points.      //
        //                                                                    //
        // ID       0U                                                        //
        // Kind1    21                                                        //
        // Name     ISO 646: ASCII (7-bit) (ISO 6) US                         //
        //          Basic (7-bit) US-ASCII set.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void unicodeMap_0U ()
        {
            const eSymSetMapId mapId = eSymSetMapId.map_0U;

            const int rangeCt = 1;

            ushort[] [] rangeData = new ushort[rangeCt] []
            {
                new ushort [2] {0x20, 0x7f}
            };

            ushort[] rangeSizes = new ushort[rangeCt];

            ushort[] [] mapDataStd = new ushort[rangeCt] [];
            ushort[] [] mapDataPCL = new ushort[rangeCt] [];

            ushort rangeMin,
                   rangeMax,
                   rangeSize;

            //----------------------------------------------------------------//

            for (int i = 0; i < rangeCt; i++)
            {
                rangeSizes [i] = (ushort) (rangeData [i] [1] -
                                           rangeData [i] [0] + 1);
            }

            for (int i = 0; i < rangeCt; i++)
            {
                mapDataStd [i] = new ushort[rangeSizes [i]];
                mapDataPCL [i] = new ushort[rangeSizes [i]];
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Range 0                                                        //
            //                                                                //
            //----------------------------------------------------------------//

            rangeMin = rangeData [0] [0];
            rangeMax = rangeData [0] [1];
            rangeSize = rangeSizes [0];

            for (ushort i = rangeMin; i <= rangeMax; i++)
            {
                mapDataStd [0] [i - rangeMin] = i;
            }

            mapDataStd [0] [0x7f - rangeMin] = 0xffff;    //<not a character> //

            //----------------------------------------------------------------//

            for (ushort i = 0; i < rangeSize; i++)
            {
                mapDataPCL [0] [i] = mapDataStd [0] [i];
            }

            mapDataPCL [0] [0x27 - rangeMin] = 0x2019;
            mapDataPCL [0] [0x60 - rangeMin] = 0x2018;
            mapDataPCL [0] [0x7f - rangeMin] = 0x2592;

            //----------------------------------------------------------------//

            _sets.Add (new PCLSymSetMap (mapId,
                                         rangeCt,
                                         rangeData,
                                         mapDataStd,
                                         mapDataPCL));
        }
    }
}