using System;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles a PCL Plex Mode object.
    /// 
    /// © Chris Hutchinson 2012
    /// 
    /// </summary>

    class PCLPlexMode
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private PCLPlexModes.ePlexType _plexType;

        private string _plexModeName;
        private byte _plexModeIdPCL;
        private byte _plexModeIdPCLXLLand;
        private byte _plexModeIdPCLXLPort;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L P l e x M o d e                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLPlexMode(PCLPlexModes.ePlexType plexType,
                            string name,
                            byte idPCL,
                            byte idPCLXLLand,
                            byte idPCLXLPort)
        {
            _plexType = plexType;
            _plexModeName = name;
            _plexModeIdPCL = idPCL;
            _plexModeIdPCLXLLand = idPCLXLLand;
            _plexModeIdPCLXLPort = idPCLXLPort;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d P C L                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL identifier value.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte getIdPCL()
        {
            return _plexModeIdPCL;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d P C L X L                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL XL (duplex) binding enumeration value for the       //
        // specified page orientation.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte getIdPCLXL(bool landscape)
        {
            if (landscape)
                return _plexModeIdPCLXLLand;
            else
                return _plexModeIdPCLXLPort;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the plex mode name.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string getName()
        {
            return _plexModeName;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P l e x T y p e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the plex type.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLPlexModes.ePlexType getPlexType()
        {
            return _plexType;
        }
    }
}