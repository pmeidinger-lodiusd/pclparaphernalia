namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines a set of PCL Symbol Set map objects.
    /// 
    /// © Chris Hutchinson 2015
    /// 
    /// </summary>

    static partial class PCLSymSetMaps
    {
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // u n i c o d e M a p _ 7 H                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Maps characters in symbol set to Unicode (UCS-2) code-points.      //
        //                                                                    //
        // ID       7H                                                        //
        // Kind1    232                                                       //
        // Name     ISO 8859-8 Latin/Hebrew                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void unicodeMap_7H()
        {
            const eSymSetMapId mapId = eSymSetMapId.map_7H;

            const int rangeCt = 3;

            ushort[][] rangeData = new ushort[rangeCt][]
            {
                new ushort [2] {0x20, 0x7f},
                new ushort [2] {0xa0, 0xbe},
                new ushort [2] {0xdf, 0xff}
            };

            ushort[] rangeSizes = new ushort[rangeCt];

            ushort[][] mapDataStd = new ushort[rangeCt][];
            ushort[][] mapDataPCL = new ushort[rangeCt][];

            ushort rangeMin,
                   rangeMax,
                   rangeSize,
                   offset;

            //----------------------------------------------------------------//

            for (int i = 0; i < rangeCt; i++)
            {
                rangeSizes[i] = (ushort)(rangeData[i][1] -
                                           rangeData[i][0] + 1);
            }

            for (int i = 0; i < rangeCt; i++)
            {
                mapDataStd[i] = new ushort[rangeSizes[i]];
                mapDataPCL[i] = new ushort[rangeSizes[i]];
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Range 0                                                        //
            //                                                                //
            //----------------------------------------------------------------//

            rangeMin = rangeData[0][0];
            rangeMax = rangeData[0][1];
            rangeSize = rangeSizes[0];

            for (ushort i = rangeMin; i <= rangeMax; i++)
            {
                mapDataStd[0][i - rangeMin] = i;
            }

            mapDataStd[0][0x7f - rangeMin] = 0xffff;    //<not a character> //

            //----------------------------------------------------------------//

            for (ushort i = 0; i < rangeSize; i++)
            {
                mapDataPCL[0][i] = mapDataStd[0][i];
            }

            mapDataPCL[0][0x5e - rangeMin] = 0x02c6;
            mapDataPCL[0][0x7e - rangeMin] = 0x02dc;
            mapDataPCL[0][0x7f - rangeMin] = 0x2592;

            //----------------------------------------------------------------//
            //                                                                //
            // Range 1                                                        //
            //                                                                //
            //----------------------------------------------------------------//

            rangeMin = rangeData[1][0];
            rangeMax = rangeData[1][1];
            rangeSize = rangeSizes[1];

            for (ushort i = rangeMin; i <= rangeMax; i++)
            {
                mapDataStd[1][i - rangeMin] = i;
            }

            mapDataStd[1][0xa1 - rangeMin] = 0xffff;    //<not a character> //
            mapDataStd[1][0xaa - rangeMin] = 0x00d7;
            mapDataStd[1][0xb7 - rangeMin] = 0x2219;
            mapDataStd[1][0xba - rangeMin] = 0x00f7;

            //----------------------------------------------------------------//

            for (ushort i = 0; i < rangeSize; i++)
            {
                mapDataPCL[1][i] = mapDataStd[1][i];
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Range 2                                                        //
            //                                                                //
            //----------------------------------------------------------------//

            rangeMin = rangeData[2][0];
            rangeMax = rangeData[2][1];
            rangeSize = rangeSizes[2];
            offset = 0x05d0 - 0x00e0;     // 0x05d0 - 0x00e0 = 0x04f0

            for (ushort i = rangeMin; i <= rangeMax; i++)
            {
                mapDataStd[2][i - rangeMin] = (ushort)(offset + i);
            }

            mapDataStd[2][0xdf - rangeMin] = 0x2017;
            mapDataStd[2][0xfb - rangeMin] = 0xffff;    //<not a character> //
            mapDataStd[2][0xfc - rangeMin] = 0xffff;    //<not a character> //
            mapDataStd[2][0xfd - rangeMin] = 0x200e;
            mapDataStd[2][0xfe - rangeMin] = 0x200f;
            mapDataStd[2][0xff - rangeMin] = 0xffff;    //<not a character> //

            //----------------------------------------------------------------//

            for (ushort i = 0; i < rangeSize; i++)
            {
                mapDataPCL[2][i] = mapDataStd[2][i];
            }

            mapDataPCL[2][0xfd - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[2][0xfe - rangeMin] = 0xffff;    //<not a character> //

            //----------------------------------------------------------------//

            _sets.Add(new PCLSymSetMap(mapId,
                                         rangeCt,
                                         rangeData,
                                         mapDataStd,
                                         mapDataPCL));
        }
    }
}