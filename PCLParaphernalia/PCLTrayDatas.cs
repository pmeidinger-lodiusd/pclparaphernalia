namespace PCLParaphernalia;

/// <summary>
/// 
/// Class defines a set of PCL Input Tray (Paper Source) objects.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

static class PCLTrayDatas
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private enum ePDL
    {
        // must be in same order as _trayDatas array 
        PCL,
        PCLXL
    }

    private static readonly PCLTrayData[] _trayDatas =
    {
        // must be in same order as ePDL enumeration 
        new PCLTrayData(7, 0, 299, -1),
        new PCLTrayData(1, 0, 255, -1)
    };

    private static readonly int _trayDataCount = _trayDatas.GetUpperBound(0) + 1;

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t I d A u t o S e l e c t P C L                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return PCL ID associated with auto-select tray.                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static short GetIdAutoSelectPCL()
    {
        return _trayDatas[(int)ePDL.PCL].GetIdAutoSelect();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t I d A u t o S e l e c t P C L X L                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return PCLXL ID associated with auto-select tray.                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static short GetIdAutoSelectPCLXL()
    {
        return _trayDatas[(int)ePDL.PCLXL].GetIdAutoSelect();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t I d D e f a u l t P C L                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return PCL ID associated with default tray.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static short GetIdDefaultPCL()
    {
        return _trayDatas[(int)ePDL.PCL].GetIdDefault();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t I d D e f a u l t P C L X L                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return PCLXL ID associated with default tray.                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static short GetIdDefaultPCLXL()
    {
        return _trayDatas[(int)ePDL.PCLXL].GetIdDefault();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t I d M a x i m u m P C L                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return maximum PCL tray ID.                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static short GetIdMaximumPCL()
    {
        return _trayDatas[(int)ePDL.PCL].GetIdMaximum();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t I d M a x i m u m P C L X L                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return maximum PCLXL tray ID.                                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static short GetIdMaximumPCLXL()
    {
        return _trayDatas[(int)ePDL.PCLXL].GetIdMaximum();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t I d N o t S e t P C L                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return dummy 'not set' PCL tray ID.                                //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static short GetIdNotSetPCL()
    {
        return _trayDatas[(int)ePDL.PCL].GetIdNotSet();
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t I d N o t S e t P C L X L                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return dummy 'not set' PCLXL tray ID.                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static short GetIdNotSetPCLXL()
    {
        return _trayDatas[(int)ePDL.PCLXL].GetIdNotSet();
    }
}