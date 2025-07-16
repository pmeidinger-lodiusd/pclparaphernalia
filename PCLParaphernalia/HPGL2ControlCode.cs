using System;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class defines a HP-GL/2 control code character.
/// 
/// © Chris Hutchinson 2013
/// 
/// </summary>

// [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
[System.Reflection.Obfuscation(
    Feature = "renaming",
    ApplyToMembers = true)]

class HPGL2ControlCode
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Class variables.                                                   //
    //                                                                    //
    //--------------------------------------------------------------------//

    private readonly byte _tag;

    //--------------------------------------------------------------------//
    //                                              C o n s t r u c t o r //
    // H P G L 2 C o n t r o l C o d e                                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    public HPGL2ControlCode(byte tag,
                             bool noOp,
                             string mnemonic,
                             string description)
    {
        _tag = tag;
        NoOp = noOp;
        Mnemonic = mnemonic;
        DescExcMnemonic = description;

        StatsCtParent = 0;
        StatsCtChild = 0;
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // D e s c E x c M n e m o n i c                                      //
    //                                                                    //
    //--------------------------------------------------------------------//

    public string DescExcMnemonic { get; }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // D e s c r i p t i o n                                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    public string Description => Mnemonic + ": " + DescExcMnemonic;

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // M n e m o n i c                                                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    public string Mnemonic { get; }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // N o O p                                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public bool NoOp { get; }

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
            StatsCtParent++;
        else
            StatsCtChild++;
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
        StatsCtParent = 0;
        StatsCtChild = 0;
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // S e q u e n c e                                                    //
    //                                                                    //
    //--------------------------------------------------------------------//

    public string Sequence => "0x" + _tag.ToString("x2");

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // S t a t s C t C h i l d                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public int StatsCtChild { get; private set; }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // S t a t s C t P a r e n t                                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    public int StatsCtParent { get; private set; }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // S t a t s C t T o t a l                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public int StatsCtTotal => StatsCtParent + StatsCtChild;

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // T a g                                                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    public string Tag => "0x" + _tag.ToString("x2");

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // T y p e                                                            //
    //                                                                    //
    //--------------------------------------------------------------------//

    public string Type => "Control Code";
}