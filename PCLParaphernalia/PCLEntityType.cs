﻿namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles a PCL 'status readback' Entity Type object.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal class PCLEntityType
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly PCLEntityTypes.Type _entityType;
        private readonly string _entityName;
        private readonly string _entityIdPCL;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L E n t i t y T y p e                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLEntityType(PCLEntityTypes.Type type, string id, string name)
        {
            _entityType = type;
            _entityIdPCL = id;
            _entityName = name;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d P C L                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the identifier value.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetIdPCL() => _entityIdPCL;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the entity type name.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetName() => _entityName;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the entity type.                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLEntityTypes.Type GetEntityType() => _entityType;
    }
}