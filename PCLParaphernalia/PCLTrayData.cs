﻿namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles a PCL Input Tray (Paper Source) object.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    class PCLTrayData
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly short _idAutoSelect;
        private readonly short _idDefault;
        private readonly short _idMaximum;
        private readonly short _idNotSet;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L T r a y D a t a                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLTrayData(short idAutoSelect,
                           short idDefault,
                           short idMaximum,
                           short idNotSet)
        {
            _idDefault = idDefault;
            _idAutoSelect = idAutoSelect;
            _idMaximum = idMaximum;
            _idNotSet = idNotSet;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d A u t o S e l e c t                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the auto-select tray identifier.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public short GetIdAutoSelect()
        {
            return _idAutoSelect;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d D e f a u l t                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the default tray identifier.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public short GetIdDefault()
        {
            return _idDefault;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d M a x i m u m                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the maximum tray identifier.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public short GetIdMaximum()
        {
            return _idMaximum;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d N o t S e t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the dummy 'not set' tray identifier.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public short GetIdNotSet()
        {
            return _idNotSet;
        }
    }
}