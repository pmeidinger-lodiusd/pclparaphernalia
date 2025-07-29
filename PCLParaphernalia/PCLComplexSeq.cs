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

class PCLComplexSeq
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private readonly byte _keyPChar;
    private readonly byte _keyGChar;
    private readonly byte _keyTChar;

    private readonly string _description;

    private readonly bool _flagDiscrete;
    private readonly bool _flagNilGChar;
    private readonly bool _flagNilValue;
    private readonly bool _flagObsolete;
    private readonly bool _flagResetGL2;
    private readonly bool _flagValIsLen;

    private readonly bool _flagDisplayHexVal;

    private readonly bool _flagValGeneric;
    private readonly bool _flagValVarious;

    private readonly int _value;
    private int _statsCtParent;
    private int _statsCtChild;

    private readonly PrnParseConstants.eActPCL _actionType;

    private readonly PrnParseConstants.eOvlAct _makeOvlAct;

    private readonly PrnParseConstants.eSeqGrp _seqGrp;

    //--------------------------------------------------------------------//
    //                                              C o n s t r u c t o r //
    // P C L C o m p l e x S e q                                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public PCLComplexSeq(
        byte keyPChar,
        byte keyGChar,
        byte keyTChar,
        int value,
        bool flagDiscrete,
        bool flagNilGChar,
        bool flagNilValue,
        bool flagValIsLen,
        bool flagObsolete,
        bool flagResetGL2,
        bool flagDisplayHexVal,
        PrnParseConstants.eActPCL actionType,
        PrnParseConstants.eOvlAct makeOvlAct,
        PrnParseConstants.eSeqGrp seqGrp,
        string description)
    {
        _keyPChar = keyPChar;
        _keyGChar = keyGChar;
        _keyTChar = keyTChar;

        _value = value;
        _actionType = actionType;

        _description = description;

        _flagDiscrete = flagDiscrete;
        _flagNilGChar = flagNilGChar;
        _flagNilValue = flagNilValue;
        _flagValIsLen = flagValIsLen;
        _flagObsolete = flagObsolete;
        _flagResetGL2 = flagResetGL2;

        _flagDisplayHexVal = flagDisplayHexVal;

        _makeOvlAct = makeOvlAct;
        _seqGrp = seqGrp;

        _flagValGeneric = value == PCLComplexSeqs._valueGeneric;

        _flagValVarious = value == PCLComplexSeqs._valueVarious;

        _statsCtParent = 0;
        _statsCtChild = 0;
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // A c t i o n T y p e                                                //
    //                                                                    //
    //--------------------------------------------------------------------//

    public PrnParseConstants.eActPCL ActionType
    {
        get { return _actionType; }
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
    // F l a g D i s c r e t e                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool FlagDiscrete
    {
        get { return _flagDiscrete; }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // F l a g D i s p l a y H e x V a l                                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool FlagDisplayHexVal
    {
        get { return _flagDisplayHexVal; }
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
    // F l a g N i l G C h a r                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool FlagNilGChar
    {
        get { return _flagNilGChar; }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // F l a g N i l V a l u e                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool FlagNilValue
    {
        get { return _flagNilValue; }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // F l a g R e s e t G L 2                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool FlagResetGL2
    {
        get { return _flagResetGL2; }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // F l a g V a l G e n e r i c                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool FlagValGeneric
    {
        get { return _flagValGeneric; }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // F l a g V a l I s L e n                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool FlagValIsLen
    {
        get { return _flagValIsLen; }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // F l a g V a l V a r i o u s                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool FlagValVarious
    {
        get { return _flagValVarious; }
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
    // M a k e O v e r l a y A c t i o n                                  //
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
        get
        {
            string seq;
            string value;

            if ((_flagDiscrete) && (!_flagValGeneric) && (!_flagValVarious))
                value = " (#=" + _value.ToString() + ")";
            else
                value = string.Empty;

            if (_flagNilValue)
            {
                if (_flagNilGChar)
                {
                    seq = "<Esc>" + (char)_keyPChar +
                                    (char)_keyTChar;
                }
                else
                {
                    seq = "<Esc>" + (char)_keyPChar +
                                    (char)_keyGChar +
                                    (char)_keyTChar;
                }
            }
            else if (_flagNilGChar)
            {
                seq = "<Esc>" + (char)_keyPChar + "#" +
                                (char)_keyTChar +
                                value;
            }
            else
            {
                seq = "<Esc>" + (char)_keyPChar +
                                (char)_keyGChar + "#" +
                                (char)_keyTChar +
                                value;
            }

            return seq;
        }
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
        get { return "Complex"; }
    }
}