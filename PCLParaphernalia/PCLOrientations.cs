﻿using System;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a set of PCL Orientation objects.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    static class PCLOrientations
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        // Note that the length of the index array must be the same as that   //
        // of the definition array; the entries must be in the same order.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum eAspect
        {
            Portrait,
            Landscape
        }

        public enum eIndex
        {
            Portrait,
            Landscape,
            ReversePortrait,
            ReverseLandscape
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly PCLOrientation[] _orientations =
        {
            new PCLOrientation(eAspect.Portrait,
                               "Portrait",
                               0x00,
                               (byte)PCLXLAttrEnums.eVal.ePortraitOrientation),
            new PCLOrientation(eAspect.Landscape,
                               "Landscape",
                               0x01,
                               (byte)PCLXLAttrEnums.eVal.eLandscapeOrientation),
            new PCLOrientation(eAspect.Portrait,
                               "Reverse Portrait",
                               0x02,
                               (byte)PCLXLAttrEnums.eVal.eReversePortrait),
            new PCLOrientation(eAspect.Landscape,
                               "Reverse Landscape",
                               0x03,
                               (byte)PCLXLAttrEnums.eVal.eReverseLandscape)
        };

        private static readonly int _orientationCount =
            _orientations.GetUpperBound(0) + 1;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t A s p e c t                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the orientation aspect.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static eAspect getAspect(int index)
        {
            return _orientations[index].getAspect();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o u n t                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return count of Orientation definitions.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int getCount()
        {
            return _orientationCount;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d P C L                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return PCL ID associated with specified Orientation index.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static byte getIdPCL(int index)
        {
            return _orientations[index].getIdPCL();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d P C L X L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return PCL XL ID associated with specified Orientation index.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static byte getIdPCLXL(int index)
        {
            return _orientations[index].getIdPCLXL();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return name associated with specified Orientation index.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string getName(int index)
        {
            return _orientations[index].getName();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i s L a n d s c a p e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return a flag indicating whether the aspect associated with the    //
        // specified Orientation index is Landscape or not.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool isLandscape(int index)
        {
            return _orientations[index].getAspect() == eAspect.Landscape;
        }
    }
}