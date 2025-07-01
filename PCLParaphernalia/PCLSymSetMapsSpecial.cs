using System.Windows;

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
        // u n i c o d e M a p N u l l                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Null map array.                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void unicodeMapNull()
        {
            const eSymSetMapId mapId = eSymSetMapId.mapNull;

            const int rangeCt = 1;

            ushort[][] rangeData = new ushort[rangeCt][]
            {
                new ushort [2] {0x00, 0x01}
            };

            ushort[] rangeSizes = new ushort[rangeCt];

            ushort[][] mapDataStd = new ushort[rangeCt][];

            ushort rangeMin,
                   rangeMax;

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

            //----------------------------------------------------------------//

            _sets.Add(new PCLSymSetMap(mapId,
                                         rangeCt,
                                         rangeData,
                                         mapDataStd,
                                         null));
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // u n i c o d e M a p M i s s i n g                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Missing map array.                                                 //
        // Should only be invoked if the case statement in the 'populate'     //
        // function is out of step with the enumeration list.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void unicodeMapMissing()
        {
            MessageBox.Show("missing map array",
                             "internal error",
                             MessageBoxButton.OK,
                             MessageBoxImage.Error);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // u n i c o d e M a p S y m b o l                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Map array for TTF 'symbol' encoding.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void unicodeMapSymbol()
        {
            const eSymSetMapId mapId = eSymSetMapId.mapSymbol;

            const int rangeCt = 1;

            ushort[][] rangeData = new ushort[rangeCt][]
            {
                new ushort [2] {0x00, 0xff}
            };

            ushort[] rangeSizes = new ushort[rangeCt];

            ushort[][] mapDataStd = new ushort[rangeCt][];

            ushort rangeMin,
                   rangeMax;

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
                mapDataStd[0][i - rangeMin] = (ushort)(0xf000 + i);
            }

            //----------------------------------------------------------------//

            _sets.Add(new PCLSymSetMap(mapId,
                                         rangeCt,
                                         rangeData,
                                         mapDataStd,
                                         mapDataStd));
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // u n i c o d e M a p U s e r S e t                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Default map array for 'user-defined' encoding.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void unicodeMapUserSet()
        {
            const eSymSetMapId mapId = eSymSetMapId.mapUserSet;

            const int rangeCt = 1;

            ushort[][] rangeData = new ushort[rangeCt][]
            {
                new ushort [2] {0x0000, 0xffff} // rangeMax + 1 > UInt16 max
            };

            int[] rangeSizes = new int[rangeCt];

            ushort[][] mapDataStd = new ushort[rangeCt][];

            ushort rangeMin,
                   rangeMax;

            //----------------------------------------------------------------//

            for (int i = 0; i < rangeCt; i++)
            {
                rangeSizes[i] = (rangeData[i][1] - rangeData[i][0] + 1);
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

            for (int i = rangeMin; i <= rangeMax; i++) // i > UInt6Max at end loop 
            {
                mapDataStd[0][i - rangeMin] = (ushort)i;
            }

            //----------------------------------------------------------------//

            _sets.Add(new PCLSymSetMap(mapId,
                                         rangeCt,
                                         rangeData,
                                         mapDataStd,
                                         null));
        }
    }
}