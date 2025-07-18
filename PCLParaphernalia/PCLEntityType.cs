﻿namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles a PCL 'status readback' Entity Type object.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    class PCLEntityType
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly PCLEntityTypes.eType _entityType;
        private readonly string _entityName;
        private readonly string _entityIdPCL;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L E n t i t y T y p e                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLEntityType(PCLEntityTypes.eType type,
                             string id,
                             string name)
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

        public string GetIdPCL()
        {
            return _entityIdPCL;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the entity type name.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetName()
        {
            return _entityName;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the entity type.                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLEntityTypes.eType GetType()
        {
            return _entityType;
        }
    }
}