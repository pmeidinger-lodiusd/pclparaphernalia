﻿namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines an HP-GL/2 Command.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    [System.Reflection.Obfuscation(Feature = "properties renaming")]
    internal class HPGL2Command
    {
        private int _statsCtParent;
        private int _statsCtChild;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // H P G L 2 C o m m a n d                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public HPGL2Command(string mnemonic,
                            bool flagResetHPGL2,
                            bool flagBinaryData,
                            bool flagFlipTransp,
                            bool flagSetLblTerm,
                            bool flagUseLblTerm,
                            bool flagUseStdTerm,
                            bool flagQuotedData,
                            bool flagSymbolMode,
                            string description)
        {
            Mnemonic = mnemonic;
            Description = description;

            FlagResetHPGL2 = flagResetHPGL2;
            FlagBinaryData = flagBinaryData;
            FlagFlipTransp = flagFlipTransp;
            FlagSetLblTerm = flagSetLblTerm;
            FlagUseLblTerm = flagUseLblTerm;
            FlagUseStdTerm = flagUseStdTerm;
            FlagQuotedData = flagQuotedData;
            FlagSymbolMode = flagSymbolMode;

            _statsCtParent = 0;
            _statsCtChild = 0;
        }

        public HPGL2Command(string mnemonic, string description)
        {
            Mnemonic = mnemonic;
            Description = description;

            FlagResetHPGL2 = false;
            FlagBinaryData = false;
            FlagFlipTransp = false;
            FlagSetLblTerm = false;
            FlagUseLblTerm = false;
            FlagUseStdTerm = false;
            FlagQuotedData = false;
            FlagSymbolMode = false;

            _statsCtParent = 0;
            _statsCtChild = 0;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c r i p t i o n                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Description { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g B i n a r y D a t a                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagBinaryData { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g F l i p T r a n s p                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagFlipTransp { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g Q u o t e d D a t a                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagQuotedData { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g R e s e t H P G L 2                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagResetHPGL2 { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g S e t L b l T e r m                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagSetLblTerm { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g S y m b o l M o d e                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagSymbolMode { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g U s e L b l T e r m                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagUseLblTerm { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g U s e S t d T e r m                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagUseStdTerm { get; }

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
        // M n e m o n i c                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Mnemonic { get; }

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
        // S t a t s C t C h i l d                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int StatsCtChild => _statsCtChild;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S t a t s C t P a r e n t                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int StatsCtParent => _statsCtParent;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // S t a t s C t T o t a l                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int StatsCtTotal => _statsCtParent + _statsCtChild;
    }
}