﻿namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a set of PCL 'status readback' Entity Type objects.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class PCLEntityTypes
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum Type : byte
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
            new PCLEntityType(Type.Memory,       "1", "Memory"),
            new PCLEntityType(Type.Font,         "0", "Font"),
            new PCLEntityType(Type.Macro,        "1", "Macro"),
            new PCLEntityType(Type.Pattern,      "2", "User-defined pattern"),
            new PCLEntityType(Type.SymbolSet,    "3", "Symbol Set"),
            new PCLEntityType(Type.FontExtended, "4", "Font Extended")
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

        public static int GetCount() => _entityTypeCount;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d P C L                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return PCL ID associated with specified Entity Type index.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetIdPCL(int selection) => _entityTypes[selection].GetIdPCL();

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return name associated with specified Entity Type index.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetName(int selection) => _entityTypes[selection].GetName();

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return type of Entity.                                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static Type GetType(int selection) => _entityTypes[selection].GetEntityType();
    }
}