﻿namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a set of PCL Input Tray (Paper Source) objects.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class PCLTrayDatas
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private enum PDL
        {
            // must be in same order as _trayDatas array
            PCL,

            PCLXL
        }

        private static readonly PCLTrayData[] _trayDatas =
        {
            // must be in same order as ePDL enumeration
            new PCLTrayData(7, 0, 299, -1),
            new PCLTrayData(1, 0, 255, -1)
        };

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d A u t o S e l e c t P C L                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return PCL ID associated with auto-select tray.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static short GetIdAutoSelectPCL() => _trayDatas[(int)PDL.PCL].GetIdAutoSelect();

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d A u t o S e l e c t P C L X L                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return PCLXL ID associated with auto-select tray.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static short GetIdAutoSelectPCLXL() => _trayDatas[(int)PDL.PCLXL].GetIdAutoSelect();

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d D e f a u l t P C L                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return PCL ID associated with default tray.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static short GetIdDefaultPCL() => _trayDatas[(int)PDL.PCL].GetIdDefault();

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d D e f a u l t P C L X L                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return PCLXL ID associated with default tray.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static short GetIdDefaultPCLXL() => _trayDatas[(int)PDL.PCLXL].GetIdDefault();

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d M a x i m u m P C L                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return maximum PCL tray ID.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static short GetIdMaximumPCL() => _trayDatas[(int)PDL.PCL].GetIdMaximum();

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d M a x i m u m P C L X L                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return maximum PCLXL tray ID.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static short GetIdMaximumPCLXL() => _trayDatas[(int)PDL.PCLXL].GetIdMaximum();

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d N o t S e t P C L                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return dummy 'not set' PCL tray ID.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static short GetIdNotSetPCL() => _trayDatas[(int)PDL.PCL].GetIdNotSet();

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d N o t S e t P C L X L                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return dummy 'not set' PCLXL tray ID.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static short GetIdNotSetPCLXL() => _trayDatas[(int)PDL.PCLXL].GetIdNotSet();
    }
}