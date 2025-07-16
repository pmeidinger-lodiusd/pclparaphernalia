namespace PCLParaphernalia;

/// <summary>
/// 
/// Class defines a PCL Simple Escape Sequence.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

// [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
[System.Reflection.Obfuscation(
    Feature = "renaming",
    ApplyToMembers = true)]

class PCLSimpleSeq
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private readonly byte _keySChar;
    private readonly string _description;

    private readonly bool _flagObsolete;
    private readonly bool _flagResetHPGL2;

    private int _statsCtParent;
    private int _statsCtChild;

    private readonly PrnParseConstants.eOvlAct _makeOvlAct;

    private readonly PrnParseConstants.eSeqGrp _seqGrp;

    //--------------------------------------------------------------------//
    //                                              C o n s t r u c t o r //
    // P C L S i m p l e S e q                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public PCLSimpleSeq(
        byte keySChar,
        bool flagObsolete,
        bool flagResetHPGL2,
        PrnParseConstants.eOvlAct makeOvlAct,
        PrnParseConstants.eSeqGrp seqGrp,
        string description)
    {
        _keySChar = keySChar;
        _description = description;

        _flagObsolete = flagObsolete;
        _flagResetHPGL2 = flagResetHPGL2;

        _makeOvlAct = makeOvlAct;
        _seqGrp = seqGrp;

        _statsCtParent = 0;
        _statsCtChild = 0;
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // D e s c r i p t i o n                                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    public string Description
    {
        get { return _description; }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // F l a g O b s o l e t e                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool FlagObsolete
    {
        get { return _flagObsolete; }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // F l a g R e s e t H P G L 2                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool FlagResetHPGL2
    {
        get { return _flagResetHPGL2; }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // F l a g V a l I s L e n                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool FlagValIsLen
    {
        get { return false; }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // i n c r e m e n t S t a t i s t i c s C o u n t                    //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Increment 'statistics' count.                                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    public void IncrementStatisticsCount(int level)
    {
        if (level == 0)
            _statsCtParent++;
        else
            _statsCtChild++;
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // M a k e M a c r o A c t                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public PrnParseConstants.eOvlAct MakeOvlAct
    {
        get { return _makeOvlAct; }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r e s e t S t a t i s t i c s                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Reset 'statistics' counts.                                         //
    //                                                                    //
    //--------------------------------------------------------------------//

    public void ResetStatistics()
    {
        _statsCtParent = 0;
        _statsCtChild = 0;
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // S e q u e n c e                                                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    public string Sequence
    {
        get { return "<Esc>" + (char)_keySChar; }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // S t a t s C t C h i l d                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public int StatsCtChild
    {
        get { return _statsCtChild; }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // S t a t s C t P a r e n t                                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public int StatsCtParent
    {
        get { return _statsCtParent; }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // S t a t s C t T o t a l                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public int StatsCtTotal
    {
        get { return (_statsCtParent + _statsCtChild); }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // T y p e                                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public string Type
    {
        get { return "Simple"; }
    }
}