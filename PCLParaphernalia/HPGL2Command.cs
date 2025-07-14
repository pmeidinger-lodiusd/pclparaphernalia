namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines an HP-GL/2 Command.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    [System.Reflection.Obfuscation(Feature = "properties renaming")]

    class HPGL2Command
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly string _mnemonic;
        private readonly string _description;

        private readonly bool _flagResetHPGL2;
        private readonly bool _flagBinaryData;
        private readonly bool _flagFlipTransp;
        private readonly bool _flagSetLblTerm;
        private readonly bool _flagUseLblTerm;
        private readonly bool _flagUseStdTerm;
        private readonly bool _flagQuotedData;
        private readonly bool _flagSymbolMode;

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
            _mnemonic = mnemonic;
            _description = description;

            _flagResetHPGL2 = flagResetHPGL2;
            _flagBinaryData = flagBinaryData;
            _flagFlipTransp = flagFlipTransp;
            _flagSetLblTerm = flagSetLblTerm;
            _flagUseLblTerm = flagUseLblTerm;
            _flagUseStdTerm = flagUseStdTerm;
            _flagQuotedData = flagQuotedData;
            _flagSymbolMode = flagSymbolMode;

            _statsCtParent = 0;
            _statsCtChild = 0;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c r i p t i o n                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Description => _description;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g B i n a r y D a t a                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagBinaryData => _flagBinaryData;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g F l i p T r a n s p                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagFlipTransp => _flagFlipTransp;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g Q u o t e d D a t a                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagQuotedData => _flagQuotedData;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g R e s e t H P G L 2                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagResetHPGL2 => _flagResetHPGL2;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g S e t L b l T e r m                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagSetLblTerm => _flagSetLblTerm;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g S y m b o l M o d e                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagSymbolMode => _flagSymbolMode;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g U s e L b l T e r m                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagUseLblTerm => _flagUseLblTerm;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g U s e S t d T e r m                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagUseStdTerm => _flagUseStdTerm;

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

        public string Mnemonic => _mnemonic;

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