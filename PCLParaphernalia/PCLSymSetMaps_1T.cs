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
        // u n i c o d e M a p _ 1 T                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Maps characters in symbol set to Unicode (UCS-2) code-points.      //
        //                                                                    //
        // ID       1T                                                        //
        // Kind1    52                                                        //
        // Name     TISI 620-2533 (Thai)                                      //
        // Note     Similar to ISO-8859-11 which also includes no-break-space //
        //          at code-point 0xA0                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void unicodeMap_1T()
        {
            const eSymSetMapId mapId = eSymSetMapId.map_1T;

            const int rangeCt = 2;

            ushort[][] rangeData = new ushort[rangeCt][]
            {
                new ushort [2] {0x20, 0x7f},
                new ushort [2] {0xa0, 0xff}
            };

            ushort[] rangeSizes = new ushort[rangeCt];

            ushort[][] mapDataStd = new ushort[rangeCt][];

            ushort rangeMin,
                   rangeMax,
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
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Range 0                                                        //
            //                                                                //
            //----------------------------------------------------------------//

            rangeMin = rangeData[0][0];
            rangeMax = rangeData[0][1];

            for (ushort i = rangeMin; i <= rangeMax; i++)
            {
                mapDataStd[0][i - rangeMin] = i;
            }

            mapDataStd[0][0x7f - rangeMin] = 0xffff;    //<not a character> //

            //----------------------------------------------------------------//
            //                                                                //
            // Range 1                                                        //
            //                                                                //
            //----------------------------------------------------------------//

            rangeMin = rangeData[1][0];
            rangeMax = rangeData[1][1];

            offset = 0x0e00 - 0x00a0;     // 0x0e00 - 0x00a0 = 0x0d60

            for (ushort i = rangeMin; i <= rangeMax; i++)
            {
                mapDataStd[1][i - rangeMin] = (ushort)(offset + i);
            }

            mapDataStd[1][0xa0 - rangeMin] = 0xffff;    //<not a character> //
            mapDataStd[1][0xdb - rangeMin] = 0xffff;    //<not a character> //
            mapDataStd[1][0xdc - rangeMin] = 0xffff;    //<not a character> //
            mapDataStd[1][0xdd - rangeMin] = 0xffff;    //<not a character> //
            mapDataStd[1][0xde - rangeMin] = 0xffff;    //<not a character> //

            mapDataStd[1][0xfc - rangeMin] = 0xffff;    //<not a character> //
            mapDataStd[1][0xfd - rangeMin] = 0xffff;    //<not a character> //
            mapDataStd[1][0xfe - rangeMin] = 0xffff;    //<not a character> //
            mapDataStd[1][0xff - rangeMin] = 0xffff;    //<not a character> //

            //----------------------------------------------------------------//

            _sets.Add(new PCLSymSetMap(mapId,
                                         rangeCt,
                                         rangeData,
                                         mapDataStd,
                                         null));
        }
    }
}