namespace PCLParaphernalia;

/// <summary>e
/// 
/// Class defines a set of PCL 'status readback' Entity Type objects.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

static class PCLEntityTypes
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    public enum eType : byte
    {
        Memory,
        Font,
        Macro,
        Pattern,
        SymbolSet,
        FontExtended
    }

    private static readonly PCLEntityType[] _entityTypes =
    {
        new PCLEntityType(eType.Memory,         "1",
                          "Memory"),
        new PCLEntityType(eType.Font,           "0",
                          "Font"),
        new PCLEntityType(eType.Macro,          "1",
                          "Macro"),
        new PCLEntityType(eType.Pattern,        "2",
                          "User-defined pattern"),
        new PCLEntityType(eType.SymbolSet,      "3",
                          "Symbol Set"),
        new PCLEntityType(eType.FontExtended,   "4",
                          "Font Extended")
    };

    private static readonly int _entityTypeCount = _entityTypes.GetUpperBound(0) + 1;

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
        return _entityTypeCount;
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
        return _entityTypes[selection].GetIdPCL();
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
        return _entityTypes[selection].GetName();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t T y p e                                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return type of Entity.                                             //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static eType GetType(int selection)
    {
        return _entityTypes[selection].GetType();
    }
}