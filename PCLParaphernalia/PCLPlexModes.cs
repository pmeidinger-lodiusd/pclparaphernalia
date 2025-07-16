namespace PCLParaphernalia;

/// <summary>
/// 
/// Class defines a set of PCL Plex Mode objects.
/// 
/// © Chris Hutchinson 2012
/// 
/// </summary>

static class PCLPlexModes
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Constants and enumerations.                                        //
    //                                                                    //
    // Note that the length of the index array must be the same as that   //
    // of the definition array; the entries must be in the same order.    //
    //                                                                    //
    //--------------------------------------------------------------------//

    public enum ePlexType
    {
        Simplex,
        Duplex
    }

    public enum eIndex
    {
        Simplex,
        DuplexLongEdge,
        DuplexShortEdge
    }

    public const int eSimplex = (int)eIndex.Simplex;

    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static readonly PCLPlexMode[] _plexModes =
    {
        new PCLPlexMode (ePlexType.Simplex,
                         "Simplex",
                         0x00,
                         0x00,
                         0x00),
        new PCLPlexMode (ePlexType.Duplex,
                         "Duplex Long-edge binding",
                         0x01,
                         (byte) PCLXLAttrEnums.eVal.eDuplexHorizontalBinding,
                         (byte) PCLXLAttrEnums.eVal.eDuplexVerticalBinding),
        new PCLPlexMode (ePlexType.Duplex,
                         "Duplex Short-edge binding",
                         0x02,
                         (byte) PCLXLAttrEnums.eVal.eDuplexVerticalBinding,
                         (byte) PCLXLAttrEnums.eVal.eDuplexHorizontalBinding)
    };

    private static readonly int _plexModeCount =
        _plexModes.GetUpperBound(0) + 1;

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t C o u n t                                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return count of Plex Mode definitions.                             //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static int GetCount()
    {
        return _plexModeCount;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t I d P C L                                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return PCL ID associated with specified plex mode index.           //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static byte GetIdPCL(int index)
    {
        return _plexModes[index].GetIdPCL();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t I d P C L X L                                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return PCL XL ID associated with specified plex mode index.        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static byte GetIdPCLXL(int index, bool flagLandscape)
    {
        return _plexModes[index].GetIdPCLXL(flagLandscape);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t N a m e                                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return name associated with specified plex mode index.             //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static string GetName(int index)
    {
        return _plexModes[index].GetName();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t P l e x T y p e                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return plex type associated with specified plex mode index.        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static ePlexType GetPlexType(int index)
    {
        return _plexModes[index].GetPlexType();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i s S i m p l e x                                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return value indicating whether or not plex mode is simplex.       //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static bool IsSimplex(int index)
    {
        return _plexModes[index].GetPlexType() == ePlexType.Simplex;
    }
}