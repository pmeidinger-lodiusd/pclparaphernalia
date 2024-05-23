using System;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a set of PCL 'status readback' Location Type objects.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    static class PCLLocationTypes
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum Type : byte
        {
            Current,
            All,
            Internal,
            Downloaded,
            Cartridge,
            ROMDevice
        }

        private static readonly PCLLocationType[] _locationTypes =
        {
            new PCLLocationType(Type.All,        "2", "All locations"),
            new PCLLocationType(Type.Current,    "1", "Currently selected"),
            new PCLLocationType(Type.Internal,   "3", "Internal"),
            new PCLLocationType(Type.Downloaded, "4", "Downloaded entities"),
            new PCLLocationType(Type.Cartridge,  "5", "Cartridge"),
            new PCLLocationType(Type.ROMDevice,  "7", "SIMMs/DIMMs")
        };

        private static readonly int _locationTypeCount = _locationTypes.GetUpperBound(0) + 1;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o u n t                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return count of Entity Type definitions.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetCount()
        {
            return _locationTypeCount;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d P C L                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return PCL ID associated with specified Entity Type index.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetIdPCL(int selection)
        {
            return _locationTypes[selection].GetIdPCL();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return name associated with specified Entity Type index.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetName(int selection)
        {
            return _locationTypes[selection].GetName();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return type of Entity.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static Type GetType(int selection)
        {
            return _locationTypes[selection].GetLocationType();
        }
    }
}