namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles a PCL 'status readback' Location Type object.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    class PCLLocationType
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly PCLLocationTypes.Type _locationType;
        private readonly string _locationName;
        private readonly string _locationIdPCL;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L L o c a t i o n T y p e                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLLocationType(PCLLocationTypes.Type type, string id, string name)
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

        public PCLLocationTypes.Type GetLocationType()
        {
            return _locationType;
        }
    }
}