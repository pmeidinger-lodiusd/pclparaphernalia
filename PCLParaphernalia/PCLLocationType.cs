namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles a PCL 'status readback' Location Type object.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    class PCLLocationType
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly PCLLocationTypes.eType _locationType;
        private readonly string _locationName;
        private readonly string _locationIdPCL;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L L o c a t i o n T y p e                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLLocationType(PCLLocationTypes.eType type,
                               string id,
                               string name)
        {
            _locationType = type;
            _locationIdPCL = id;
            _locationName = name;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d P C L                                                   //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the identifier value.                                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetIdPCL()
        {
            return _locationIdPCL;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the location type name.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetName()
        {
            return _locationName;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the entity type.                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLLocationTypes.eType GetType()
        {
            return _locationType;
        }
    }
}