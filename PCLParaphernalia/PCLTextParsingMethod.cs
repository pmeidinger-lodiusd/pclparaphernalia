namespace PCLParaphernalia;

/// <summary>
/// 
/// Class handles a PCL Text Parsing Method object.
/// 
/// © Chris Hutchinson 2015
/// 
/// </summary>

class PCLTextParsingMethod
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private readonly PCLTextParsingMethods.eIndex _indxMethod;

    private readonly short _value;

    private readonly string _desc;

    private readonly ushort[] _rangeDataSingle;
    private readonly ushort[] _rangeDataDouble;

    //--------------------------------------------------------------------//
    //                                              C o n s t r u c t o r //
    // P C L T e x t P a r s i n g M e t h o d                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public PCLTextParsingMethod(
        PCLTextParsingMethods.eIndex indxMethod,
        short value,
        string desc,
        ushort[] rangeDataSingle,
        ushort[] rangeDataDouble)
    {
        _indxMethod = indxMethod;
        _value = value;
        _desc = desc;

        _rangeDataSingle = rangeDataSingle;
        _rangeDataDouble = rangeDataDouble;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t D e s c                                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return the description.                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public string GetDesc()
    {
        return _desc;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t D e s c L o n g                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return the PCL identifier value and description, except for the    //
    // "<not specified>" entry (which has a dummy negative value).        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public string GetDescLong()
    {
        if (_value < 0)
            return _desc;
        else
            return (_value.ToString() + ": " + _desc);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t M e t h o d T y p e                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return the method index.                                           //
    //                                                                    //
    //--------------------------------------------------------------------//

    public PCLTextParsingMethods.eIndex GetMethodType()
    {
        return _indxMethod;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t R a n g e D a t a D o u b l e                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return the double-byte range(s).                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    public ushort[] GetRangeDataDouble()
    {
        return _rangeDataDouble;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t R a n g e D a t a D o u b l e C t                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return the count of double-byte range(s).                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public int GetRangeDataDoubleCt()
    {
        if (_rangeDataDouble == null)
            return 0;
        else
            return (_rangeDataDouble.Length / 2);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t R a n g e D a t a S i n g l e                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return the single-byte range(s).                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    public ushort[] GetRangeDataSingle()
    {
        return _rangeDataSingle;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t R a n g e D a t a S i n g l e C t                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return the count of single-byte range(s).                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public int GetRangeDataSingleCt()
    {
        if (_rangeDataSingle == null)
            return 0;
        else
            return (_rangeDataSingle.Length / 2);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t V a l u e                                                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return the PCL identifier value.                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    public int GetValue()
    {
        return _value;
    }
}